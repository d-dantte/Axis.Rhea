
using Axis.Rhea.Directives.Contract;

namespace Axis.Rhea.Shared.Contract.Directives;

/// <summary>
/// Condition that triggers a retry
/// </summary>
public interface IRetryPolicyTriggerCondition
{
    bool IsMatch(IResponsePayload payload);
}
