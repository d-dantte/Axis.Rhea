using Axis.Luna.Extensions;
using System;

namespace Axis.Rhea.Core.Workflow.Events
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventPayload
    {
    }

    /// <summary>
    /// 
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
    /// Calling a service
    /// </summary>
    public readonly struct DirectiveExecution : IEventPayload
    {

    }

    /// <summary>
    /// Service response
    /// </summary>
    public readonly struct DirectiveResponse : IEventPayload
    {

    }

    /// <summary>
    /// Some directive responses may contain instructions to mutate state. The timeline is notified of these sorts of events
    /// </summary>
    public readonly struct StateMutation : IEventPayload
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public readonly struct Error : IEventPayload
    {

    }
}
