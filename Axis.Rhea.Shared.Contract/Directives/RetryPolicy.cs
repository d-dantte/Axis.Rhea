using Axis.Luna.Extensions;
using Axis.Rhea.Shared.Contract.Directives.PolicyTriggers;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Directives;

/// <summary>
/// Specifies 
/// <list type="number">
///     <item>What conditions trigger a retry - e.g: error, timeout, or specific response messages, etc.</item>
///     <item>The exponential back off parameters</item>
/// </list>
/// </summary>
public record RetryPolicy
{
    /// <summary>
    /// Condition that must be met for the policy to be applied
    /// </summary>
    public IRetryPolicyTriggerCondition Condition { get; }

    /// <summary>
    /// Millisecond base to calculate exponential backoff with
    /// </summary>
    public int BackoffBase{ get; }

    /// <summary>
    /// Maximum random value in milliseconds to be added to the exponential backoff
    /// </summary>
    public int MaxJitter { get; }

    /// <summary>
    /// Maximum number of retries
    /// </summary>
    public int MaxRetries { get; }

    public static RetryPolicy RetryOnFault { get; } =
        new RetryPolicy(100, 10, 20, FaultPayloadTrigger.Instance);

    public RetryPolicy(
        int backoffBase,
        int maxRetries,
        int maxJitter,
        IRetryPolicyTriggerCondition condition)
    {
        BackoffBase = backoffBase.ThrowIf(
            i => i <= 0,
            new ArgumentException($"Invalid {nameof(backoffBase)}: '{backoffBase}'"));

        MaxRetries = maxRetries.ThrowIf(
            i => i <= 0,
            new ArgumentException($"Invalid {nameof(maxRetries)}: '{maxRetries}'"));

        MaxJitter = maxJitter.ThrowIf(
            i => i <= 0,
            new ArgumentException($"Invalid {nameof(maxJitter)}: '{maxJitter}'"));

        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
    }
}
