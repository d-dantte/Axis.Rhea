using Axis.Luna.Extensions;

namespace Axis.Rhae.Workflow.Definition.Utils
{
    /// <summary>
    /// Specifies 
    /// <list type="number">
    ///     <item>What conditions trigger a retry - e.g: error, timeout, or specific response messages, etc.</item>
    ///     <item>The exponential back off parameters</item>
    /// </list>
    /// </summary>
    public class RetryPolicy<TResponse>
    {
        /// <summary>
        /// Condition that must be met for the policy to be applied
        /// </summary>
        public IRetryPolicyTrigger<TResponse> Trigger { get; }

        /// <summary>
        /// Millisecond base to calculate exponential backoff with
        /// </summary>
        public int BackoffBase { get; }

        /// <summary>
        /// Maximum random value in milliseconds to be added to the exponential backoff
        /// </summary>
        public int MaxJitter { get; }

        /// <summary>
        /// Maximum number of retries
        /// </summary>
        public int MaxRetries { get; }

        public RetryPolicy(
            int backoffBase,
            int maxRetries,
            int maxJitter,
            IRetryPolicyTrigger<TResponse> trigger)
        {
            BackoffBase = backoffBase.ThrowIf(
                i => i <= 0,
                _ => new ArgumentException($"Invalid {nameof(backoffBase)}: '{backoffBase}'"));

            MaxRetries = maxRetries.ThrowIf(
                i => i <= 0,
                _ => new ArgumentException($"Invalid {nameof(maxRetries)}: '{maxRetries}'"));

            MaxJitter = maxJitter.ThrowIf(
                i => i <= 0,
                _ => new ArgumentException($"Invalid {nameof(maxJitter)}: '{maxJitter}'"));

            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
        }
    }
}
