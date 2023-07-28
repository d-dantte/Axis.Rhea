namespace Axis.Rhea.Shared.Contract.Workflow;

/// <summary>
/// A container that pairs a workflow and the active timeline
/// </summary>
public record WorkflowInstance
{
    /// <summary>
    /// the workflow from which this instance is created
    /// </summary>
    public WorkflowDefinition Workflow { get; }

    /// <summary>
    /// Fully qualified identifier of the workflow executed in this instance
    /// </summary>
    public string WorkflowQName => Workflow.QualifiedName;

    /// <summary>
    /// Unique Id for this instance
    /// </summary>
    public Guid InstanceId { get; }

    /// <summary>
    /// the current active timeline
    /// </summary>
    public ActiveTimeline CurrentTimeline { get; }

    public WorkflowInstance(WorkflowDefinition workflow, ActiveTimeline currentTimeline)
    : this(Guid.NewGuid(), workflow, currentTimeline)
    {
    }

    public WorkflowInstance(Guid instanceId, WorkflowDefinition workflow, ActiveTimeline currentTimeline)
    {
        InstanceId = instanceId;
        Workflow = workflow;
        CurrentTimeline = currentTimeline ?? throw new ArgumentNullException(nameof(currentTimeline));
    }
}
