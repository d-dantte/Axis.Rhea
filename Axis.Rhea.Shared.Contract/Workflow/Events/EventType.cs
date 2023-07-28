namespace Axis.Rhea.Shared.Contract.Workflow.Events;

/// <summary>
/// 
/// </summary>
public enum EventType
{
    /// <summary>
    /// Captures the transition of the workflow to a new state b99
    /// </summary>
    ActivityTransition,

    /// <summary>
    /// Captures the execution of a <see cref="ServiceInvocation.IServiceInvocation"/>
    /// </summary>
    DirectiveExecution,

    /// <summary>
    /// The directive is being retried, based on it's <see cref="ServiceInvocation.RetryPolicy"/>
    /// </summary>
    DirectiveRetry,

    /// <summary>
    /// The <see cref="DirectiveExecution"/> returns with a <see cref="ServiceContract.IResponsePayload"/>;
    /// the response is captured in this event
    /// </summary>
    DirectiveResponse,

    /// <summary>
    /// A new workflow is being started from this workflow. Typically, the <c>QName</c>, and alias are
    /// payloads of this event
    /// </summary>
    WorkflowExecution,

    /// <summary>
    /// Response of the workflow execution. Typically, the <c>QName</c>, and alias, and instance id are
    /// payloads of this event, unless the execution failed, then an error event is raised.
    /// </summary>
    WorkflowExecutionResponse,

    /// <summary>
    /// State mutation has occured following a <see cref="DirectiveExecution"/>.
    /// </summary>
    StateMutation,

    /// <summary>
    /// An error occurs during any phase of the workflows execution
    /// </summary>
    Error,

    /// <summary>
    /// Signifies the conclusion of the timeline, and by extension, the workflow instance.
    /// </summary>
    Conclude
}
