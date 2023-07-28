using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Workflow;

/// <summary>
/// History of all timelines for this workflow. This is an immutable record
/// </summary>
public record WorkflowInstanceHistory
{
    private readonly ImmutableList<FrozenTimeline> timelines;

    /// <summary>
    /// The Fully Qualified id of the workflow
    /// </summary>
    public string WorkflowQualifiedName { get; }

    /// <summary>
    /// The Guid of the instance
    /// </summary>
    public Guid InstanceId { get; }

    /// <summary>
    /// All timelines for the workflow
    /// </summary>
    public IEnumerable<FrozenTimeline> Timelines => timelines;

    public WorkflowInstanceHistory(
        string workflowQualifiedName,
        Guid instanceId,
        params FrozenTimeline[] timelines)
    {
        InstanceId = instanceId;

        WorkflowQualifiedName = workflowQualifiedName.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invlalid id: {workflowQualifiedName}"));

        this.timelines = timelines
            .ThrowIfNull(new ArgumentNullException(nameof(timelines)))
            .ToImmutableList();
    }
}
