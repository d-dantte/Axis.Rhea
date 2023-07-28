using Axis.Luna.Extensions;

namespace Axis.Rhea.Shared.Contract.Workflow.Events;

/// <summary>
/// 
/// </summary>
public interface IEventPayload
{
}

/// <summary>
/// <see cref="EventType.ActivityTransition"/>
/// </summary>
public readonly struct ActivityTransition : IEventPayload
{
    /// <summary>
    /// 
    /// </summary>
    public string PreviousActivity { get; }

    /// <summary>
    /// 
    /// </summary>
    public string NextActivity { get; }

    public ActivityTransition(string previousActivity, string nextActivity)
    {
        PreviousActivity = previousActivity.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid previousActivity: {previousActivity}"));
        NextActivity = previousActivity.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid nextActivity: {nextActivity}"));
    }
}

/// <summary>
/// <see cref="EventType.DirectiveExecution"/>
/// </summary>
public readonly struct DirectiveExecution : IEventPayload
{

}

/// <summary>
/// <see cref="EventType.DirectiveResponse"/>
/// </summary>
public readonly struct DirectiveResponse : IEventPayload
{

}

/// <summary>
/// <see cref="EventType.DirectiveRetry"/>
/// </summary>
public readonly struct DirectiveRetry : IEventPayload
{

}

/// <summary>
/// <see cref="EventType.StateMutation"/>
/// </summary>
public readonly struct StateMutation : IEventPayload
{

}

/// <summary>
/// <see cref="EventType.Error"/>
/// </summary>
public readonly struct Error : IEventPayload
{

}

/// <summary>
/// <see cref="EventType.Conclude"/>
/// </summary>
public readonly struct Conclude : IEventPayload 
{
}
