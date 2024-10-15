using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Responses
{
    public record WorkflowStatusInfo : IValidatable
    {
        required public WorkflowStatus Status { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (!Enum.IsDefined(Status))
                errors.Add(new ValidationResult($"Invalid '{nameof(Status)}': undefined enum"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
