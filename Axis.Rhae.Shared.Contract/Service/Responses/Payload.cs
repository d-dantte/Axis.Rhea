using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Service.Responses
{
    public record Payload : IValidatable
    {
        public Instruction? Instruction { get; init; }

        required public Identifier<Workflow.Identifiers.ResultLabel> ResultLabel { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (ResultLabel.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(ResultLabel)}': default"));

            if (Instruction is not null && !Instruction.IsValid(out var instructionErrors))
                errors.AddRange(instructionErrors);

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
