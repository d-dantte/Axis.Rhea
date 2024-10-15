using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Service
{
    using WorkflowId = Workflow.Identifiers.Workflow;
    using ActivityId = Workflow.Identifiers.Activity;

    public class ServiceEvent :
        IValidatable,
        ICorrelatable
    {
        required public Guid CorrelationId { get; init; }

        required public Instruction Instruction { get; init; }

        #region Target
        required public Identifier<WorkflowId> Workflow { get; init; }

        required public Identifier<ActivityId> Activity { get; init; }
        #endregion

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (!Instruction.IsValid(out var instructionErrors))
                errors.AddRange(instructionErrors);

            if (Workflow.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(Workflow)}': default"));

            if (Activity.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(Activity)}': default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
