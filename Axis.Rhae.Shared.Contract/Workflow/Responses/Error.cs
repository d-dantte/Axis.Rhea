using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Responses
{
    public class Error : IValidatable
    {
        required public Exception Exception { get; init; }

        public string? Code { get; init; }

        public bool TryValidate(out ValidationResult[] validationException)
        {
            var errors = new List<ValidationResult>();

            if (Exception is null)
                errors.Add(new ValidationResult($"'{nameof(Exception)}' is null"));

            validationException = [.. errors];
            return validationException.IsEmpty();
        }
    }
}
