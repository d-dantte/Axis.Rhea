namespace Axis.Rhae.Contract.Audit
{
    /// <summary>
    /// Supported timeline event types
    /// </summary>
    public enum EventType
    {
        #region Actions

        #region Invocation
        /// <summary>
        /// Captures information about the execution of a <see cref="Definition.ServiceDirective.IInvocationDefinition"/>
        /// </summary>
        ServiceInvocation,

        /// <summary>
        /// Captures information about starting a new "child" workflow from an active workflow
        /// </summary>
        WorkflowInvocation,
        #endregion

        #region Misc
        /// <summary>
        /// Captures the transition of the workflow to a new state
        /// </summary>
        ActivityTransition,

        /// <summary>
        /// Jumps to some arbitrary activity in the workflow. Only Paused or Active workflows can be skipped.
        /// </summary>
        Skipped,

        /// <summary>
        /// Captures information about retrying a workflow. This starts a new workflow, and leaves the old one as-is.
        /// </summary>
        Retried,

        /// <summary>
        /// Captures information about the status of the workflow changing.
        /// </summary>
        StatusChanged,
        #endregion

        #endregion

        /// <summary>
        /// Every action-event SHOULD eventually have a action-result-event, which notifies the TL
        /// of the result of the action.
        /// </summary>
        ActionResult,

        #region Sinks
        /// <summary>
        /// Captures information about the event received by a service sink activity
        /// </summary>
        ServiceEventNotification,

        /// <summary>
        /// Captures information about the event received by a workflow from it's child workflow
        /// </summary>
        WorkflowEventNotification
        #endregion
    }
}
