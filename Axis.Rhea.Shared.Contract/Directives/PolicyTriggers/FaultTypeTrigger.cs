using Axis.Rhea.Directives.Contract;
using Axis.Rhea.Directives.Contract.Responses;
using static Axis.Rhea.Directives.Contract.Responses.FaultPayload;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers;

/// <summary>
/// Triggers a retry if the <see cref="ErrorPayload.Title"/> matches the given <see cref="ErrorTitleTrigger.ErrorTitle"/>
/// <para>
/// See <see cref="FaultPayload"/>
/// </para>
/// </summary>
public record FaultTypeTrigger: IRetryPolicyTriggerCondition
{
    /// <summary>
    /// The expected error title
    /// </summary>
    public FaultTypes FaultType { get; }

    public FaultTypeTrigger(string faultType)
    : this(Enum.Parse<FaultTypes>(faultType))
    {
    }

    public FaultTypeTrigger(FaultTypes faultType)
    {
        FaultType = faultType;
    }

    public bool IsMatch(IResponsePayload payload)
    {
        if (payload is null)
            throw new ArgumentNullException(nameof(payload));

        return payload switch
        {
            FaultPayload fault => FaultType.Equals(fault.Fault),
            _ => false
        };
    }
}
