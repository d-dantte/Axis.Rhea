using Axis.Luna.Common;
using Axis.Rhea.Core.Workflow.Events;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Rhea.Core.Workflow
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITimeline
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<TimelineEvent> Events { get; }

        /// <summary>
        /// 
        /// </summary>
        uint SequentialId { get; }
    }

    /// <summary>
    /// Represents an immutable timeline - a timeline that is frozen and cannot be notified of new events
    /// </summary>
    public record FrozenTimeline: ITimeline
    {
        private readonly ImmutableList<TimelineEvent> events;

        public IEnumerable<TimelineEvent> Events => events;

        public uint SequentialId { get; private set; }

        public FrozenTimeline(uint sequentialId, IEnumerable<TimelineEvent> events)
        {
            SequentialId = sequentialId;
            this.events = events?
                .OrderBy(e => e.Timestamp)
                .ToImmutableList()
                ?? throw new ArgumentNullException(nameof(events));
        }

        public FrozenTimeline(uint sequentialId, params TimelineEvent[] events)
        : this(sequentialId, (IEnumerable<TimelineEvent>)events)
        {
        }
    }

    /// <summary>
    /// Represents an active timeline - one that can be notified of recent events
    /// </summary>
    public class ActiveTimeline: ITimeline
    {
        internal static readonly string Format = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";

        private readonly List<TimelineEvent> events = new List<TimelineEvent>();

        public IEnumerable<TimelineEvent> Events => events;

        public uint SequentialId { get; private set; }

        public ActiveTimeline(uint sequentialId, IEnumerable<TimelineEvent> events)
        {
            SequentialId = sequentialId;
            this.events.AddRange(events?.OrderBy(e => e.Timestamp) ?? throw new ArgumentNullException(nameof(events)));
        }

        public ActiveTimeline(uint sequentialId, params TimelineEvent[] events)
        : this(sequentialId, (IEnumerable<TimelineEvent>)events)
        {
        }

        public IResult<TimelineEvent> Notify(IEventPayload payload, string message)
        {
            var @event = payload switch
            {
                ActivityTransition at => TimelineEvent.Of(at, message),
                DirectiveExecution de => TimelineEvent.Of(de, message),
                DirectiveResponse dr => TimelineEvent.Of(dr, message),
                StateMutation sm => TimelineEvent.Of(sm, message),
                Error er => TimelineEvent.Of(er, message),
                null => throw new ArgumentNullException(nameof(payload)),
                _ => throw new ArgumentException($"Invalid payload: {payload}")
            };

            return events
                .LastOrDefault()
                .AsOptional()
                .Map(lastEvent =>
                {
                    if (@event.Timestamp <= lastEvent.Timestamp)
                    {
                        var tlTimestamp = lastEvent.Timestamp.ToString(Format);
                        var neTimestamp = @event.Timestamp.ToString(Format);
                        return Result.Of<TimelineEvent>(new InvalidOperationException($"Cannot add a past event to the timeline. timeline: '{tlTimestamp}', new event: '{neTimestamp}'"));
                    }

                    events.Add(@event);
                    return Result.Of(@event);
                })
                .Value();
        }

        public FrozenTimeline Freeze() => new FrozenTimeline(SequentialId, Events);
    }
}
