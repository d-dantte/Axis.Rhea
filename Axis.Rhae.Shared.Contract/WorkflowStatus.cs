namespace Axis.Rhae.Contract
{ 
    public enum WorkflowStatus
    {
        /// <summary>
        /// Workflow has just been created but not yet executed
        /// </summary>
        Created = 0,

        /// <summary>
        /// Workflow is executing
        /// </summary>
        Active,

        /// <summary>
        /// Workflow is paused. No events will be responded to, nor will events/actions be performed by the workflow
        /// </summary>
        Paused,

        /// <summary>
        /// Workflow has been externally forced to stop.
        /// </summary>
        Halted,

        /// <summary>
        /// Workflow has been forced to stop due to internal errors.
        /// </summary>
        Errored
    }
}
