namespace Axis.Rhea.Shared.Contract.Workflow.Events;

/// <summary>
/// 
/// </summary>
public record TimelineEvent
{
    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// 
    /// </summary>
    public EventType EventType { get; }

    /// <summary>
    /// 
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// 
    /// </summary>
    public IEventPayload Payload { get; }

    public TimelineEvent(
        Guid id,
        DateTimeOffset timestamp,
        string message,
        IEventPayload payload)
    {
        Id = id;
        Timestamp = timestamp;
        Message = message;
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        EventType = Payload switch
        {
            ActivityTransition => EventType.ActivityTransition,
            DirectiveExecution => EventType.DirectiveExecution,
            DirectiveResponse => EventType.DirectiveResponse,
            StateMutation => EventType.StateMutation,
            Error => EventType.Error,
            _ => throw new ArgumentException($"Invalid payload type: {payload.GetType()}")
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static TimelineEvent Of(ActivityTransition payload, string message)
        => new(Guid.NewGuid(), DateTimeOffset.Now, message, payload);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static TimelineEvent Of(DirectiveExecution payload, string message)
        => new(Guid.NewGuid(), DateTimeOffset.Now, message, payload);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static TimelineEvent Of(DirectiveResponse payload, string message)
        => new(Guid.NewGuid(), DateTimeOffset.Now, message, payload);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static TimelineEvent Of(StateMutation payload, string message)
        => new(Guid.NewGuid(), DateTimeOffset.Now, message, payload);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static TimelineEvent Of(Error payload, string message)
        => new(Guid.NewGuid(), DateTimeOffset.Now, message, payload);
}
