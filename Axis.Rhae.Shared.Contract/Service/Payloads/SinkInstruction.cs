using Axis.Luna.Extensions;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Service.Payloads
{
    public class ServiceSinkInstruction
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

        required public Identifier<Workflow.Identifiers.Workflow> WorkflowId { get; init; }

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (WorkflowId.IsDefault)
                errors.Add(new ValidationResult($"'{nameof(WorkflowId)}' is default"));

            if (instructions.Any(i => i.IsDefault))
                errors.Add(new ValidationResult($"'{nameof(Instructions)}' contains default value(s)"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
