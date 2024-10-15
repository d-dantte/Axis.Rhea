using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Service.Responses
{
    public class Error : IValidatable
    {
        required public Exception Exception { get; init; }

        public string? Code { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Exception is null)
                errors.Add(new ValidationResult($"Invalid '{nameof(Exception)}': null"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
