using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Requests
{
    public record ActivitySkipPayload : IValidatable
    {

        public bool TryValidate(out ValidationResult[] validationException)
        {
            throw new NotImplementedException();
        }
    }
}
