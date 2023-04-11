using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.State;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.ServiceInvocation
{
    public record StateMutator
    {
        public ImmutableList<IMutationInstruction> Instructions { get; }

        public StateMutator(params IMutationInstruction[] instructions)
        {
            Instructions = instructions
                .ThrowIfNull(new ArgumentNullException(nameof(instructions)))
                .ThrowIfAny(
                    segment => segment is null,
                    new ArgumentException("Null instruction found in the instructins list"))
                .ToImmutableList();
        }

        /// <summary>
        /// Modify the given <paramref name="data"/> based on the Instructions, and return the modified struct.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IResult<IonStruct> Mutate(IonStruct data)
        {
            foreach(var instruction in Instructions)
            {
                if (instruction is DeleteInstruction delete)
                {
                    var selectionGroups = delete.DataPathSelector
                        .Select(data)
                        .GroupBy(selection => selection.GetType());

                    foreach(var selectionGroup in selectionGroups)
                    {
                        if (typeof(PropertyPathSelection).Name.Equals(selectionGroup.Key.Name))
                        {
                            selectionGroup
                                .Cast<PropertyPathSelection>()
                                .ForAll(pps =>
                                {
                                    var structContainer = pps.Container.Value;
                                    structContainer.Properties.Remove(pps.PropertyName, out _);
                                });
                        }
                        else if (typeof(ListPathSelection).Name.Equals(selectionGroup.Key.Name))
                        {
                            selectionGroup
                                .Cast<ListPathSelection>()
                                .OrderByDescending(lps => lps.ListIndex)
                                .ForAll(lps =>
                                {
                                    var listContainer = lps.Container.Value;
                                    listContainer.Items.RemoveAt(lps.ListIndex);
                                });
                        }
                        else return Result.Of<IonStruct>(
                            new InvalidOperationException($"Invalid PathSelectionType: {selectionGroup.Key}"));
                    }
                }
                else if (instruction is ModifyInstruction modify)
                {
                    var selectionGroup = modify.DataPathSelector
                        .Select(data)
                        .ToArray()
                        .ThrowIf(
                            arr => arr.Length > 1,
                            new InvalidOperationException($"'{modify.Action}' instructions must mutate a single value."))
                        .FirstOrDefault();

                    switch((modify.Action, selectionGroup))
                    {
                        #region PropertyPathSelection
                        case (Action.Insert, PropertyPathSelection pps):
                            // adds a NEW property to the struct. If the property already exists, fail
                            var ppayload = (PropertyPayload)modify.Payload;
                            var props = ((IonStruct)pps.Value).Properties;
                            if (!props.Add(ppayload.Property, modify.Payload.Data))
                                throw new InvalidOperationException($"Payload could not be inserted into the struct");
                            break;

                        case (Action.Update, PropertyPathSelection pps):
                            // adds or updates the property of the struct
                            ppayload = (PropertyPayload)modify.Payload;
                            props = ((IonStruct)pps.Value).Properties;
                            props[ppayload.Property] = ppayload.Data;
                            break;
                        #endregion

                        #region ListPathSelection
                        case (Action.Insert, ListPathSelection lps):
                            var lpayload = (ListPayload)modify.Payload;
                            var items = ((IonList)lps.Value).Items;
                            items.Add(lpayload.Data);
                            break;

                        case (Action.Update, ListPathSelection lps):
                            lpayload = (ListPayload)modify.Payload;
                            items = ((IonList)lps.Value).Items;
                            var index = lpayload.Index ?? throw new ArgumentException("Insert index must not be null");
                            if (index > items.Count - 1)
                                items.Add(lpayload.Data);
                            else items.Insert(index, lpayload.Data);
                                break;
                        #endregion

                        default: throw new InvalidOperationException($"Invalid instruction/selection: '{modify.Action}', '{selectionGroup}'");
                    }
                }
                else return Result.Of<IonStruct>(
                    new InvalidOperationException($"Invalid Instruction Type: '{instruction?.GetType()}'"));
            }

            return Result.Of(data);
        }

        #region nested types
        public enum Action
        {
            /// <summary>
            /// Adds the given data to the site(s) represented by the path selector. If data already exists there, report exception
            /// </summary>
            Insert,

            /// <summary>
            /// Adds or updates the data to the site(s) represented by the path selector.
            /// </summary>
            Update,

            /// <summary>
            /// Delete all data found at the site(s) represented by the path selector.
            /// </summary>
            Delete
        }

        /// <summary>
        /// Mutation instruction
        /// </summary>
        public interface IMutationInstruction
        {
            /// <summary>
            /// The mutation action
            /// </summary>
            Action Action { get; }

            /// <summary>
            /// Selector that returns the data container whose contents will be mutated
            /// </summary>
            PathSegment DataPathSelector { get; }
        }

        /// <summary>
        /// For <see cref="Action.Insert"/>, or <see cref="Action.Update"/>, the data that is added or used in replacing.
        /// </summary>
        public interface IPayload
        {
            /// <summary>
            /// The data
            /// </summary>
            IIonType Data { get; }
        }

        /// <summary>
        /// Represents payload for <see cref="IonStruct"/> containers
        /// </summary>
        public record PropertyPayload: IPayload
        {
            public IIonType Data { get; }

            /// <summary>
            /// Name of the property to be mutated
            /// </summary>
            public string Property { get; }

            public PropertyPayload(IIonType data, string property)
            {
                Data = data ?? throw new ArgumentNullException(nameof(data));
                Property = property.ThrowIf(
                    string.IsNullOrWhiteSpace,
                    new ArgumentException($"Invalid {nameof(property)}: '{property}'"));
            }
        }

        /// <summary>
        /// Represetns payload for <see cref="IonList"/> containers
        /// </summary>
        public record ListPayload: IPayload
        {
            public IIonType Data { get; }

            /// <summary>
            /// Index in the list where the mutation will happen
            /// </summary>
            public int? Index { get; }

            public ListPayload(IIonType data, int? index = null)
            {
                Data = data ?? throw new ArgumentNullException(nameof(data));
                Index = index;
            }
        }

        /// <summary>
        /// Instruction to delete all data found at the end of the given <see cref="PathSegment"/>.
        /// </summary>
        public record DeleteInstruction : IMutationInstruction
        {
            public Action Action => Action.Delete;

            public PathSegment DataPathSelector { get; }

            public DeleteInstruction(PathSegment dataPathSelector)
            {
                DataPathSelector = dataPathSelector ?? throw new ArgumentNullException(nameof(dataPathSelector));
            }
        }

        /// <summary>
        /// Modify (add or update) a single datum found at the end of the given <see cref="PathSegment"/>.
        /// <para>
        /// If more than one data is found, the instruction is aborted
        /// </para>
        /// </summary>
        public record ModifyInstruction: IMutationInstruction
        {
            public Action Action { get; }

            public PathSegment DataPathSelector { get; }

            /// <summary>
            /// the mutation payload
            /// </summary>
            public IPayload Payload { get; }

            private ModifyInstruction(
                Action action,
                PathSegment dataPathSelector,
                IPayload payload)
            {
                Action = action;
                Payload = payload ?? throw new ArgumentNullException(nameof(payload));
                DataPathSelector = dataPathSelector ?? throw new ArgumentNullException(nameof(dataPathSelector));
            }

            public static ModifyInstruction NewInsertInstruction(
                PathSegment dataPathSelector,
                IPayload payload)
                => new ModifyInstruction(Action.Insert, dataPathSelector, payload);

            public static ModifyInstruction NewUpdateInstruction(
                PathSegment dataPathSelector,
                IPayload payload)
                => new ModifyInstruction(Action.Update, dataPathSelector, payload);
        }

        #endregion
    }
}
