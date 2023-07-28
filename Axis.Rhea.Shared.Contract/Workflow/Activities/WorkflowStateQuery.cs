using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Workflow.Activities;

/// <summary>
/// 
/// </summary>
public class WorkflowStateQuery: IActivity
{
    public string Name { get; }

    /// <summary>
    /// The ref is either an alias, or the fqid + instanceid <+ sequenceid> of a workflow
    /// Sequence Id represents the timeline sequence id. If absent, the most recent one
    /// is assumed
    /// </summary>
    public WorkflowRef Workflow { get; }

    /// <summary>
    /// A map of <see cref="Rhea.Directives.Contract.Responses.SuccessPayload.ResponseCode"/> to <see cref="IActivity.Name"/>.
    /// <para>
    /// This table tells what activity to transition to based on the returned workflow state.
    /// </para>
    /// <para>
    /// See <see cref="WorkflowStates"/> for more info.
    /// </para>
    /// </summary>
    public ImmutableDictionary<string, string> TransitionTable { get; }

    public WorkflowStateQuery(
        string name,
        WorkflowRef workflowRef,
        params (string WorkflowState, string ActivityName)[] transitionTable)
    {
        Name = name.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(name)}: '{name}'"));

        Workflow = workflowRef ?? throw new ArgumentNullException(nameof(workflowRef));

        TransitionTable = transitionTable
            .ThrowIfNull(new ArgumentNullException(nameof(transitionTable)))
            .ThrowIfAny(
                tuple => string.IsNullOrWhiteSpace(tuple.WorkflowState) || string.IsNullOrWhiteSpace(tuple.ActivityName),
                tuple => new ArgumentException($"Invalid transition pair: {tuple}"))
            .ToImmutableDictionary(
                pair => pair.WorkflowState,
                pair => pair.ActivityName);
    } 
}
