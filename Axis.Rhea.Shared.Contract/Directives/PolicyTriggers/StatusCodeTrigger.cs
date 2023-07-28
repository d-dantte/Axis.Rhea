using Axis.Rhea.Directives.Contract;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers;

/// <summary>
/// Triggers a retry if the <see cref="IStatusCoded.StatusCode"/> matches the given <see cref="StatusCodeTrigger.StatusCode"/>
/// </summary>
public record StatusCodeTrigger: IRetryPolicyTriggerCondition
{
    /// <summary>
    /// The expected status code
    /// </summary>
    public int StatusCode { get; }

    public StatusCodeTrigger(int statusCode)
    {
        StatusCode = statusCode;
    }

    public bool IsMatch(IResponsePayload payload)
    {
        if (payload is null)
            throw new ArgumentNullException(nameof(payload));

        return payload switch
        {
            IStatusCoded statusCoded => StatusCode.Equals(statusCoded.StatusCode),
            _ => false
        };
    }
}
