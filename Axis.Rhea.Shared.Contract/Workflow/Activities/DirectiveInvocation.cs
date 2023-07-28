using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Workflow.Activities;

/// <summary>
/// 
/// </summary>
public class DirectiveInvocation: IActivity
{
    /// <summary>
    /// The ID of the <see cref="Directives.IDirectiveInvocation"/> to be used.
    /// </summary>
    public string DirectiveId { get; }

    public string Name { get; }

    /// <summary>
    /// A map of <see cref="Rhea.Directives.Contract.Responses.SuccessPayload.ResponseCode"/> to <see cref="IActivity.Name"/>.
    /// <para>
    /// This table tells what activity to transition to based on the returned response identifier.
    /// </para>
    /// </summary>
    public ImmutableDictionary<string, string> TransitionTable { get; }

    public DirectiveInvocation(
        string directiveId,
        string name,
        params (string ResponseCode, string ActivityName)[] transitionTable)
    {
        DirectiveId = directiveId.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(directiveId)}: '{directiveId}'"));

        Name = name.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(name)}: '{name}'"));

        TransitionTable = transitionTable
            .ThrowIfNull(new ArgumentNullException(nameof(transitionTable)))
            .ThrowIfAny(
                tuple => string.IsNullOrWhiteSpace(tuple.ResponseCode) || string.IsNullOrWhiteSpace(tuple.ActivityName),
                tuple => new ArgumentException($"Invalid transition pair: {tuple}"))
            .ToImmutableDictionary(
                pair => pair.ResponseCode,
                pair => pair.ActivityName);
    }
}
