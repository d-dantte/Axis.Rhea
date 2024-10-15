using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Requests
{
    public class RetryWorkflowPayload : IValidatable
    {
        public Identifier<Identifiers.Workflow> PreviousWorkflowId { get; }

        // TODO: other properties will come here

        public bool TryValidate(out ValidationResult[] validationException)
        {
            throw new NotImplementedException();
        }
    }
}
