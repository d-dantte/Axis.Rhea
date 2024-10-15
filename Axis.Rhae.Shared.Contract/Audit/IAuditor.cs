namespace Axis.Rhae.Contract.Audit
{
    /// <summary>
    /// The auditor service, responsible for managing the timeline of a workflow
    /// </summary>
    public interface IAuditor
    {
        /// <summary>
        /// Notifies the timeline of an event. If there are observers registered on the timeline, Notify
        /// them of the event, using the <see cref="TimelineNotification"/> returned by the timeline.
        /// </summary>
        /// <param name="workflowId">The ID of the workflow that owns the timeline to be notified</param>
        /// <param name="payload">The event payload</param>
        void Notify(
            Identifier<Workflow.Identifiers.Workflow> workflowId,
            IEventPayload payload,
            DateTimeOffset? timestamp = null);
    }
}
