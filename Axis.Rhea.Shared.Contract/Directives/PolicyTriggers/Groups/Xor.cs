using Axis.Rhea.Directives.Contract;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers.Groups;

/// <summary>
/// Evaluates a logical 'xor' on the inner conditions
/// </summary>
public record Xor : ITriggerConditionGroup
{
    public IRetryPolicyTriggerCondition Left { get; }

    public IRetryPolicyTriggerCondition Right { get; }

    public IEnumerable<IRetryPolicyTriggerCondition> Conditions => new[] { Left, Right };

    public Xor(IRetryPolicyTriggerCondition left,  IRetryPolicyTriggerCondition right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public bool IsMatch(IResponsePayload payload)
    {
        if (payload is null)
            throw new ArgumentNullException(nameof(payload));

        return Left.IsMatch(payload) ^ Right.IsMatch(payload);
    }
}
