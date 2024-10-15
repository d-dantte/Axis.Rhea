using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    public record ServiceSinkInstruction : IValidatable
    {
        required public Identifier<Workflow.Identifiers.Activity> ActivityId { get; init; }

        required public Service.Payloads.ServiceSinkInstruction Data { get; init; }

        public EventType EventType => EventType.ServiceDirectiveResponse;

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (ActivityId.IsDefault)
                errors.Add(new ValidationResult($"'{nameof(ActivityId)}' is default"));

            if (Data is null)
                errors.Add(new ValidationResult($"'{nameof(Data)}' is null"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
