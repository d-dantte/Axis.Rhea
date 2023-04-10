using Axis.Ion.Types;
using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.State;
using System;
using System.Collections.Immutable;

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

        public IonStruct Mutate(IonStruct data)
        {

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

        public interface IMutationInstruction
        {
            Action Action { get; }

            PathSegment DataPathSelector { get; }
        }

        public record DeleteInstruction : IMutationInstruction
        {
            public Action Action => Action.Delete;

            public PathSegment DataPathSelector { get; }

            public DeleteInstruction(PathSegment dataPathSelector)
            {
                DataPathSelector = dataPathSelector ?? throw new ArgumentNullException(nameof(dataPathSelector));
            }
        }

        public record ModifyInstruction: IMutationInstruction
        {
            public Action Action { get; }

            public PathSegment DataPathSelector { get; }

            public IIonType Data { get; }

            private ModifyInstruction(
                Action action,
                PathSegment dataPathSelector,
                IIonType data)
            {
                Action = action;
                Data = data ?? throw new ArgumentNullException(nameof(data));
                DataPathSelector = dataPathSelector ?? throw new ArgumentNullException(nameof(dataPathSelector));
            }

            public static ModifyInstruction NewInsertInstruction(
                PathSegment dataPathSelector,
                IIonType data)
                => new ModifyInstruction(Action.Insert, dataPathSelector, data);

            public static ModifyInstruction NewUpdateInstruction(
                PathSegment dataPathSelector,
                IIonType data)
                => new ModifyInstruction(Action.Update, dataPathSelector, data);
        }

        #endregion
    }
}
