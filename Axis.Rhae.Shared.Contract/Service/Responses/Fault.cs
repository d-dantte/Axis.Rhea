using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Service.Responses
{
    public record Fault : IValidatable
    {
        required public string Message { get; init; }

        required public string Code { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Message))
                errors.Add(new ValidationResult($"Invalid '{nameof(Message)}': null/empty/whitespace"));

            if (string.IsNullOrWhiteSpace(Code))
                errors.Add(new ValidationResult($"Invalid '{nameof(Code)}': null/empty/whitespace"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
