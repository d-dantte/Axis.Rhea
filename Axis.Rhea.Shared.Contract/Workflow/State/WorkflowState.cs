using Axis.Ion.Types;
using Axis.Rhea.Directives.Contract;
using Axis.Rhea.Directives.Contract.Instructions;
using Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

namespace Axis.Rhea.Shared.Contract.Workflow.State;

/// <summary>
/// 
/// </summary>
public record WorkflowState
{
    /// <summary>
    /// 
    /// </summary>
    public IonStruct Data { get; }

    public WorkflowState(IonStruct data)
    {
        Data = data;
    }

    /// <summary>
    /// Applies the given mutation instructions on this <see cref="WorkflowState"/> instances <c>Data</c>
    /// <para>
    /// NOTE: for lists, to append data, select with an out of range index, something like the following is recommended:
    /// <code>
    /// /abcd/efgh/[-1]
    /// </code>
    /// The above will select the list at "<c>efgh</c>", with null as the selected value. This selection can be used to add
    /// a new value to the list.
    /// </para>
    /// </summary>
    /// <param name="mutation">The mutation instructions to apply</param>
    public WorkflowState Apply(StateMutation mutation)
    {
        foreach (var instruction in mutation.Instructions)
        {
            var dataPath = DataPathSegment.Parse(instruction.DataPathSelector);
            var selection = dataPath.Select(Data);

            switch ((instruction, selection))
            {
                case (Delete, ItemSelection itemSelection):
                    var items = itemSelection.Container switch
                    {
                        IonList list => list.Items,
                        IonSexp sexp => sexp.Items,
                        _ => throw new ArgumentException($"Invalid container: '{itemSelection.Container}'")
                    }
                    ?? throw new InvalidOperationException($"Attempting to delete from a null {itemSelection.Container.Type}");

                    items.RemoveAt(itemSelection.Index);
                    break;

                case (Modify modify, ItemSelection itemSelection):
                    items = itemSelection.Container switch
                    {
                        IonList list => list.Items,
                        IonSexp sexp => sexp.Items,
                        _ => throw new ArgumentException($"Invalid container: '{itemSelection.Container}'")
                    }
                    ?? throw new InvalidOperationException($"Attempting to delete from a null {itemSelection.Container.Type}"); ;

                    if (modify.Action == MutationAction.Append)
                        items.Add(modify.Payload);

                    else if (modify.Action == MutationAction.Replace)
                        items.AddOrInsert(modify.Payload, itemSelection.Index);

                    else throw new ArgumentException($"Invalid '{nameof(Modify)}' action: '{modify.Action}'");

                    break;

                case (Delete, PropertySelection propertySelection):
                    propertySelection.Container.Properties.Remove(propertySelection.Property, out _);
                    break;

                case (Modify modify, PropertySelection propertySelection):
                    var properties = propertySelection.Container.Properties;

                    if (modify.Action == MutationAction.Append
                        && !properties.Add(propertySelection.Property, modify.Payload))
                        throw new InvalidOperationException($"'{modify.Action}' instruction failed for: '{modify.DataPathSelector}'");

                    else if (modify.Action == MutationAction.Replace)
                        properties[propertySelection.Property] = modify.Payload;

                    break;
            }
        }

        return this;
    }

    /// <summary>
    /// Do a deep copy of the <see cref="WorkflowState.Data"/>
    /// </summary>
    /// <returns>the copied instance</returns>
    /// <exception cref="NotImplementedException"></exception>
    public WorkflowState DeepClone() => throw new NotImplementedException();


    public static implicit operator WorkflowState(IonStruct data) => new WorkflowState(data);
}
