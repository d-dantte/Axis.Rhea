using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.Workflow.Events;
using Axis.Rhea.Shared.Contract.Workflow.State;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Workflow;

/// <summary>
/// A timeline is essentially a series of events, arranged in order of their occurence in time.
/// </summary>
public interface ITimeline
{
    /// <summary>
    /// The events, in chronological order
    /// </summary>
    IEnumerable<TimelineEvent> Events { get; }

    /// <summary>
    /// The state upon which timeline events are applied
    /// </summary>
    WorkflowState State { get; }

    /// <summary>
    /// The sequential id of this timeline
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

    public WorkflowState State { get; }

    public uint SequentialId { get; }

    public FrozenTimeline(uint sequentialId, WorkflowState state, params TimelineEvent[] events)
    {
        SequentialId = sequentialId;
        State = state?.DeepClone() ?? throw new ArgumentNullException(nameof(state));
        this.events = events?
            .OrderBy(e => e.Timestamp)
            .ToImmutableList()
            ?? throw new ArgumentNullException(nameof(events));
    }
}

/// <summary>
/// Represents an active timeline - one that can be notified of recent events
/// </summary>
public class ActiveTimeline: ITimeline
{
    internal static readonly string Format = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";

    private readonly List<TimelineEvent> events;

    public IEnumerable<TimelineEvent> Events => events;

    public WorkflowState State { get; }

    public uint SequentialId { get; private set; }

    public ActiveTimeline(uint sequentialId, WorkflowState state, params TimelineEvent[] events)
    {
        SequentialId = sequentialId;
        State = state?.DeepClone() ?? throw new ArgumentNullException(nameof(state));
        this.events = events?
            .OrderBy(e => e.Timestamp)
            .ToList()
            ?? throw new ArgumentNullException(nameof(events));
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
                if (@event.Timestamp <= lastEvent!.Timestamp)
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

    public FrozenTimeline Freeze() => new(SequentialId, State, Events.ToArray());
}
