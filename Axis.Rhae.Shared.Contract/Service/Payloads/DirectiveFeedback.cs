using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Service.Payloads
{
    public record DirectiveFeedback : IValidatable
    {
        private readonly ImmutableArray<Instruction> instructions = [];

        required public ImmutableArray<Instruction> Instructions
        {
            get => instructions;
            init
            {
                instructions = value.ThrowIf(
                    t => t.IsDefault,
                    t => new ArgumentException($"Invalid {nameof(value)}: default"));
            }
        }

        required public string Label { get; init; }

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Label is null)
                errors.Add(new ValidationResult($"'{nameof(Label)}' is null"));

            if (instructions.Any(i => i.IsDefault))
                errors.Add(new ValidationResult($"'{nameof(Instructions)}' contains default value(s)"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
