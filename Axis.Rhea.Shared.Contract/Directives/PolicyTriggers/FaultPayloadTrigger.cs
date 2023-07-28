using Axis.Rhea.Directives.Contract;
using Axis.Rhea.Directives.Contract.Responses;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers
{
    /// <summary>
    /// Triggers a retry if the supplied <see cref="IResponsePayload"/> is an <see cref="FaultPayload"/>
    /// </summary>
    public record FaultPayloadTrigger: IRetryPolicyTriggerCondition
    {
        public static readonly FaultPayloadTrigger Instance = new();

        public FaultPayloadTrigger()
        {
        }

        public bool IsMatch(IResponsePayload payload)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));

            return payload switch
            {
                FaultPayload => true,
                _ => false
            };
        }
    }
}
