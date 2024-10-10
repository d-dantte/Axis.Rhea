using Axis.Dia.Contracts;
using Axis.Luna.Extensions;

namespace Axis.Rhea.Directives.Contract.Instructions;

/// <summary>
/// Modify (add or update) a single datum found at the end of the given <see cref="PathSegment"/>.
/// <para>
/// If more than one data is found, the instruction is aborted
/// </para>
/// </summary>
public record Modify : IMutationInstruction
{
    public MutationAction Action { get; }

    public string DataPathSelector { get; }

    /// <summary>
    /// the mutation payload
    /// </summary>
    public IDiaValue Payload { get; }

    private Modify(
        MutationAction action,
        string dataPathSelector,
        IDiaValue payload)
    {
        Action = action;
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        DataPathSelector = dataPathSelector.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentNullException(nameof(dataPathSelector)));
    }

    public static Modify Append(
        string dataPathSelector,
        IDiaValue payload)
        => new Modify(MutationAction.Append, dataPathSelector, payload);

    public static Modify Replace(
        string dataPathSelector,
        IDiaValue payload)
        => new Modify(MutationAction.Replace, dataPathSelector, payload);
}
