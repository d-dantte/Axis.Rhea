
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

        public bool TryValidate(out ValidationResult[] validationException)
        {
            var errors = new List<ValidationResult>();

            if (Payload is null)
                errors.Add(new ValidationResult($"'{nameof(Payload)}' is null"));

            else if (!Payload.TryValidate(out var payloadErrors))
                errors.AddRange(payloadErrors);

            validationException = [.. errors];
            return validationException.IsEmpty();
        }
    }
}
