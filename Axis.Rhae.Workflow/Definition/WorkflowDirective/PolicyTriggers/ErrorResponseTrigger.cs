using Axis.Rhae.Contract;
using Axis.Rhae.Contract.Service.Responses;
using Axis.Rhae.Contract.Workflow.Identifiers;
using Axis.Rhae.Contract.Workflow;
using Axis.Rhae.Workflow.Definition.Utils;

namespace Axis.Rhae.Workflow.Definition.WorkflowDirective.PolicyTriggers
{
    /// <summary>
    /// Triggers a retry if the supplied <see cref="Response{TPayload}"/> is an <see cref="ErrorResponseTrigger"/>
    /// </summary>
    public class ErrorResponseTrigger : IRetryPolicyTrigger<Response<Identifier<Contract.Workflow.Identifiers.Workflow>>>
    {
        public static readonly ErrorResponseTrigger Instance = new();

        public ErrorResponseTrigger()
        {
        }

        public bool IsMatch(Response<Identifier<Contract.Workflow.Identifiers.Workflow>> response)
        {
            return response is null
                ? throw new ArgumentNullException(nameof(response))
                : response is ErrorResponse;
        }
    }
}
