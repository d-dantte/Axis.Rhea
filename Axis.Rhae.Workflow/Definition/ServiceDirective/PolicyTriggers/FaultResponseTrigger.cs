using Axis.Rhae.Contract.Service;
using Axis.Rhae.Contract.Service.Responses;
using Axis.Rhae.Workflow.Definition.Utils;

namespace Axis.Rhae.Workflow.Definition.ServiceDirective.PolicyTriggers
{
    /// <summary>
    /// Triggers a retry if the supplied <see cref="IResponsePayload"/> is an <see cref="FaultPayload"/>
    /// </summary>
    public class FaultResponseTrigger : IRetryPolicyTrigger<Response>
    {
        public static readonly FaultResponseTrigger Instance = new();

        public FaultResponseTrigger()
        {
        }

        public bool IsMatch(Response response)
        {
            return response is null
                ? throw new ArgumentNullException(nameof(response))
                : response is FaultResponse;
        }
    }
}
