namespace Axis.Rhea.Shared.Contract.Workflow
{
    public static class WellKnownWorkflowExecutionContexts
    {
        /// <summary>
        /// Represents the default context used for execution. When no context is specified, the live one is used
        /// </summary>
        public static readonly string Live = "Live";

        /// <summary>
        /// Special context that is automatically assigned to the most recent version of a workflow definition.
        /// </summary>
        public static readonly string Latest = "Latest";
    }
}
