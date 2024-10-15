
using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow
{
    public record Request<TPayload> :
        ICorrelatable,
        IValidatable
        where TPayload : IValidatable
    {
        required public Guid CorrelationId { get; init; }

        required public TPayload Payload { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Payload is null)
                errors.Add(new ValidationResult($"Invalid '{nameof(Payload)}': null"));

            else if (!Payload.IsValid(out var payloadErrors))
                errors.AddRange(payloadErrors);

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
