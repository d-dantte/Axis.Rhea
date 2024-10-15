using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    using Identifiers = Workflow.Identifiers;

    public abstract record ActionResult : IEventPayload
    {
        public EventType EventType => EventType.ActionResult;

        public abstract bool IsValid(out ValidationResult[] validationResults);
    }

    public record WorkflowInvocationResult :
        ActionResult,
        IEventPayload
    {
        required public Identifier<Identifiers.Workflow> WorkflowId { get; init; }

        public override bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (WorkflowId.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(WorkflowId)}': default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }

    public record ServiceInvocationResult :
        ActionResult,
        IEventPayload
    {
        public Instruction? Instruction { get; init; }

        required public Identifier<Identifiers.ResultLabel> ResultLabel { get; init; }

        public override bool IsValid(out ValidationResult[] validationResults)
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
