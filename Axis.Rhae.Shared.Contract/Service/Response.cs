using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Service.Responses;
using Axis.Rhae.Contract.Utils;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Service
{
    public record Response :
        ICorrelatable,
        IValidatable,
        IUnion<Payload, Error, Fault>
    {
        required public Guid CorrelationId { get; init; }

        required public object Value { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Value is null)
                errors.Add(new ValidationResult($"Invalid '{nameof(Value)}': null"));

            if (Value is not IValidatable)
                errors.Add(new ValidationResult($"Invalid '{nameof(Value)}': expected instance of '{nameof(IValidatable)}'"));

            else if (!Value.As<IValidatable>().IsValid(out var payloadErrors))
                errors.AddRange(payloadErrors);

            else if (
                Value is not Payload
                && Value is not Error
                && Value is not Fault)
                errors.Add(new ValidationResult($"Invalid '{nameof(Value)}' type: {Value.GetType()}"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
