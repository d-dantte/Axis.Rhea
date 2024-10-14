using Axis.Rhae.Contract.Audit;
using System.Collections.Immutable;

namespace Axis.Rhae.Workflow.Audit
{
    /// <summary>
    /// A timeline is essentially a series of events, arranged in order of their occurence in time.
    /// </summary>
    public interface ITimeline
    {
        /// <summary>
        /// The events, in chronological order
        /// </summary>
        ImmutableArray<TimelineEvent> Events { get; }

        /// <summary>
        /// Notifies the timeline of an event
        /// </summary>
        /// <param name="payload">The event payload</param>
        /// <param name="message">The event message</param>
        /// <returns>The timeline event</returns>
        TimelineEvent Notify(IEventPayload payload, string message);
    }

    /// <summary>
    /// Represents an active timeline - one that can be notified of recent events.
    /// 
    /// NOTE: Figure out a way/mechanism for the TL to notify interested parties (parent workflows) of it's events.
    /// </summary>
    public class ActiveTimeline : ITimeline
    {
        internal static readonly string Format = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";

        private readonly List<TimelineEvent> events;

        public ImmutableArray<TimelineEvent> Events => [.. events];

        public ActiveTimeline(params TimelineEvent[] events)
        {
            this.events = events?
                .OrderBy(e => e.Timestamp)
                .ToList()
                ?? throw new ArgumentNullException(nameof(events));
        }

        public TimelineEvent Notify(IEventPayload payload, string message)
        {
            ArgumentNullException.ThrowIfNull(payload);

            var @event = TimelineEvent.Of(payload, message);
            var lastEvent = events.LastOrDefault();

            if (lastEvent is null || lastEvent.Timestamp < @event.Timestamp)
            {
                events.Add(@event);
                return @event;
            }
            else throw new InvalidOperationException(
                $"Invalid event-time [last-event: '{lastEvent.Timestamp.ToString(Format)}', "
                + $"current-event: '{@event.Timestamp.ToString(Format)}']");
        }
    }
}
