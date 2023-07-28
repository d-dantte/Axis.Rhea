using Axis.Rhea.Directives.Contract;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers;

/// <summary>
/// Evaluates a negation on the inner condition
/// </summary>
public record NotTrigger : IRetryPolicyTriggerCondition
{
    /// <summary>
    /// The inner condition
    /// </summary>
    public IRetryPolicyTriggerCondition Condition { get; }

    public NotTrigger(IRetryPolicyTriggerCondition condition)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
    }

    public bool IsMatch(IResponsePayload payload)
    {
        if (payload is null)
            throw new ArgumentNullException(nameof(payload));

        return !Condition.IsMatch(payload);
    }
}
