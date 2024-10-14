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

        public bool TryValidate(out ValidationResult[] validationException)
        {
            var errors = new List<ValidationResult>();

            if (Value is null)
                errors.Add(new ValidationResult($"'{nameof(Value)}' is null"));

            if (Value is not IValidatable)
                errors.Add(new ValidationResult($"'{nameof(Value)}' is not an '{nameof(IValidatable)}' instance"));

            else if (!Value.As<IValidatable>().TryValidate(out var payloadErrors))
                errors.AddRange(payloadErrors);

            validationException = [.. errors];
            return validationException.IsEmpty();
        }
    }
}
