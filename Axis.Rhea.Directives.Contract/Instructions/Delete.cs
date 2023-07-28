namespace Axis.Rhea.Directives.Contract.Instructions;


/// <summary>
/// Instruction to delete all data found at the end of the given <see cref="PathSegment"/>.
/// </summary>
public record Delete : IMutationInstruction
{
    public MutationAction Action => MutationAction.Delete;

    public string DataPathSelector { get; }

    public Delete(string dataPathSelector)
    {
        DataPathSelector = dataPathSelector ?? throw new ArgumentNullException(nameof(dataPathSelector));
    }
}
