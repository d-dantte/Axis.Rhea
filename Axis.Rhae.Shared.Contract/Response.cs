using Axis.Luna.Extensions;
using Axis.Luna.Unions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract
{
    public record Response<TPayload> :
        ICorrelatable,
        IValidatable,
        IUnion<TPayload, Error, Fault, Response<TPayload>>
    {
        required public Guid CorrelationId { get; init; }

        required public object Value { get; init; }

        public bool TryValidate(out ValidationResult[] validationException)
        {
            var errors = new List<ValidationResult>();

            if (Value is null)
                errors.Add(new ValidationResult($"'{nameof(Value)}' is null"));

            else if (Value is IValidatable v && !v.TryValidate(out var payloadErrors))
                errors.AddRange(payloadErrors);

            validationException = [.. errors];
            return validationException.IsEmpty();
        }
    }

    public record Error : IValidatable
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

    public record Fault : IValidatable
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
