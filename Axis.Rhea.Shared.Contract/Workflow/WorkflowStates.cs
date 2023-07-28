namespace Axis.Rhea.Shared.Contract.Workflow;

/// <summary>
/// Workflow states. Note, these correspond to the names of the <see cref="Activities.IActivity"/> instances.
/// </summary>
public static class WorkflowStates
{
    public const string ConditionalTransition = nameof(ConditionalTransition);
    public const string DirectiveInvocation = nameof(DirectiveInvocation);
    public const string WorkflowInvocation = nameof(WorkflowInvocation);
    public const string WorkflowStateQuery = nameof(WorkflowStateQuery);
}
