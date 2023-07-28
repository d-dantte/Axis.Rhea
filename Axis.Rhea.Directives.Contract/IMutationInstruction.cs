namespace Axis.Rhea.Directives.Contract;


/// <summary>
/// Mutation instruction
/// </summary>
public interface IMutationInstruction
{
    /// <summary>
    /// The mutation action
    /// </summary>
    MutationAction Action { get; }

    /// <summary>
    /// Selector of the form "/abc/xyz/[3]
    /// </summary>
    string DataPathSelector { get; }
}
