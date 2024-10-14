namespace Axis.Rhae.Contract.Audit
{
    /// <summary>
    /// Supported timeline event types
    /// </summary>
    public enum EventType
    {
        #region Service Directive
        /// <summary>
        /// Captures information about the execution of a <see cref="Definition.ServiceDirective.IInvocationDefinition"/>
        /// </summary>
        ServiceDirectiveRequest,

        /// <summary>
        /// Captures information about retrying a SDI
        /// </summary>
        ServiceDirectiveRetry,

        /// <summary>
        /// Captures information about the response of a <see cref="Definition.ServiceDirective.IInvocationDefinition"/>
        /// </summary>
        ServiceDirectiveResponse,
        #endregion

        #region Workflow Directive
        /// <summary>
        /// Captures information about starting a new "child" workflow from an active workflow
        /// </summary>
        WorkflowDirectiveRequest,

        /// <summary>
        /// Captures information about retrying a SDI
        /// </summary>
        WorkflowDirectiveRetry,

        /// <summary>
        /// Captures information about the response of a <see cref="Definition.ServiceDirective.IInvocationDefinition"/>
        /// </summary>
        WorkflowDirectiveResponse,
        #endregion

        #region Service Event Sink
        /// <summary>
        /// Captures information about the event received by a service sink activity
        /// </summary>
        ServiceEventNotification,
        #endregion

        #region Workflow/Timeline Event
        /// <summary>
        /// Captures information about the event received by a workflow from it's child workflow
        /// </summary>
        WorkflowEventNotification,
        #endregion

        #region Misc
        /// <summary>
        /// Captures the transition of the workflow to a new state
        /// </summary>
        ActivityTransition,

        /// <summary>
        /// State mutation has occured following a <see cref="DirectiveExecution"/>.
        /// </summary>
        StateMutation,
        #endregion

        #region Status
        /// <summary>
        /// Captures information about halting the workflow. Halted workflows can only be "retried".
        /// </summary>
        Halted,

        /// <summary>
        /// Captures information about the error that interrupted the workflow. Errored workflows can only be "retried".
        /// </summary>
        Errored,

        /// <summary>
        /// Captures information about pausing the workflow. Paused workflows can be resumed, or skipped
        /// </summary>
        Paused,

        /// <summary>
        /// Captures information about resuming a workflow. Resumed workflows are active.
        /// </summary>
        Resumed,

        /// <summary>
        /// Jumps to some arbitrary activity in the workflow. Only Paused or Active workflows can be skipped.
        /// </summary>
        Skipped,

        /// <summary>
        /// Captures information about retrying a workflow. This starts a new workflow, and leaves the old one as-is.
        /// </summary>
        Retried,

        /// <summary>
        /// Signifies the conclusion of the timeline, and by extension, the workflow instance.
        /// </summary>
        Concluded
        #endregion
    }
}
