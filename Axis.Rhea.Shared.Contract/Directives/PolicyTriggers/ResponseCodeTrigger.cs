using Axis.Luna.Extensions;
using Axis.Rhea.Directives.Contract;
using Axis.Rhea.Directives.Contract.Responses;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers;

/// <summary>
/// Triggers a retry if the <see cref="SuccessPayload.ResponseCode"/> matches the given <see cref="ResponseCodeTrigger.ResponseCode"/>
/// </summary>
public record ResponseCodeTrigger: IRetryPolicyTriggerCondition
{
    /// <summary>
    /// The expected response code
    /// </summary>
    public string ResponseCode { get; }

    public ResponseCodeTrigger(string responseCode)
    {
        ResponseCode = responseCode.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(responseCode)}: '{responseCode}'"));
    }

    public bool IsMatch(IResponsePayload payload)
    {
        if (payload is null)
            throw new ArgumentNullException(nameof(payload));

        return payload switch
        {
            SuccessPayload success => ResponseCode.Equals(success.ResponseCode),
            _ => false
        };
    }
}
