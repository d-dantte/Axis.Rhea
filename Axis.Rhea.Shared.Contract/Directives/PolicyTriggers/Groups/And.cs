using Axis.Luna.Extensions;
using Axis.Rhea.Directives.Contract;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers.Groups;

/// <summary>
/// Evaluates a logical 'and' on all the inner conditions
/// </summary>
public record And : ITriggerConditionGroup
{
    private readonly ImmutableList<IRetryPolicyTriggerCondition> _conditions;

    public IEnumerable<IRetryPolicyTriggerCondition> Conditions => _conditions;

    public And(params IRetryPolicyTriggerCondition[] conditions)
    {
        _conditions = conditions
            .ThrowIfNull(new ArgumentNullException(nameof(conditions)))
            .ThrowIfAny(
                c => c is null,
                new ArgumentException($"{nameof(conditions)} must not contain null values"))
            .ToImmutableList()
            .ThrowIf(
                l => l.Count == 0,
                new ArgumentException($"{nameof(conditions)} must not be empty"));
    }

    public bool IsMatch(IResponsePayload payload)
    {
        if (payload is null)
            throw new ArgumentNullException(nameof(payload));

        return _conditions.All(c => c.IsMatch(payload));
    }
}
