using Axis.Rhae.Contract.Service;
using Axis.Rhae.Contract.Service.Responses;
using Axis.Rhae.Workflow.Definition.Utils;

namespace Axis.Rhae.Workflow.Definition.ServiceDirective.PolicyTriggers
{
    /// <summary>
    /// Triggers a retry if the supplied <see cref="IResponsePayload"/> is an <see cref="FaultPayload"/>
    /// </summary>
    public class ErrorResponseTrigger : IRetryPolicyTrigger<Response>
    {
        public static readonly ErrorResponseTrigger Instance = new();

        public ErrorResponseTrigger()
        {
        }

        public bool IsMatch(Response response)
        {
            return response is null
                ? throw new ArgumentNullException(nameof(response))
                : response is ErrorResponse;
        }
    }
}
