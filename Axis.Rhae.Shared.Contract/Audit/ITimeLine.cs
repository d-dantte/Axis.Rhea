using System.Collections.Immutable;

namespace Axis.Rhae.Contract.Audit
{
    /// <summary>
    /// A series of events that have happened during the lifetime of the workflow. These are recorded
    /// and collectively referred as the TimeLine of the workflow. <para/>
    /// The TimeLine also keeps a record of all workflows who have expressed interest in being notified
    /// of events happening on this TL
    /// </summary>
    public interface ITimeLine
    {
        /// <summary>
        /// ID of this timeline, unique across the entire system.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The events, in chronological order
        /// </summary>
        ImmutableArray<Event<IEventPayload>> Events { get; }

        /// <summary>
        /// The list of observer workflows
        /// </summary>
        ImmutableArray<Identifier<Workflow.Identifiers.Workflow>> Observers { get; }

        /// <summary>
        /// Records the event on the timeline.
        /// </summary>
        /// <param name="event">The event to be recorded</param>
        public EventNotification<IEventPayload> Record(Event<IEventPayload> @event);

        /// <summary>
        /// Registers an observer.
        /// </summary>
        /// <param name="observer">The observer Identifier</param>
        public void RegisterObserver(Identifier<Workflow.Identifiers.Workflow> observer);
    }
}
