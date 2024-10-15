using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Utils;
using Axis.Rhae.Contract.Workflow.Responses;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow
{
    public record Response<TPayload> :
        ICorrelatable,
        IValidatable,
        IUnion<TPayload, Error>
        where TPayload : IValidatable
    {
        required public Guid CorrelationId{ get; init; }

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

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
