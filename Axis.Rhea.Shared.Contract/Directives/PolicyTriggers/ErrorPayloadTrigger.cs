using Axis.Rhea.Directives.Contract;
using Axis.Rhea.Directives.Contract.Responses;

namespace Axis.Rhea.Shared.Contract.Directives.PolicyTriggers
{
    /// <summary>
    /// Triggers a retry if the supplied <see cref="IResponsePayload"/> is an <see cref="ErrorPayload"/>
    /// </summary>
    public record ErrorPayloadTrigger: IRetryPolicyTriggerCondition
    {
        public static readonly ErrorPayloadTrigger Instance = new();

        public ErrorPayloadTrigger()
        {
        }

        public bool IsMatch(IResponsePayload payload)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));

            return payload switch
            {
                ErrorPayload => true,
                _ => false
            };
        }
    }
}
