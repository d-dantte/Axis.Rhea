using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Workflow;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    public record WorkflowStatusChanged : IEventPayload
    {
        public EventType EventType => EventType.StatusChanged;

        required public WorkflowStatus PreviousStatus { get; init; }

        required public WorkflowStatus CurrentStatus { get; init; }

        /// <summary>
        /// The action that caused the state change. This, I may end up removing, as the individual actions may notify the
        /// workflow on their own, so this will be redundant.
        /// </summary>
        required public WorkflowAction Action { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (!Enum.IsDefined(PreviousStatus))
                errors.Add(new ValidationResult($"Invalid '{nameof(PreviousStatus)}': undefined enum"));

            if (!Enum.IsDefined(CurrentStatus))
                errors.Add(new ValidationResult($"Invalid '{nameof(CurrentStatus)}': undefined enum"));

            if (!Enum.IsDefined(Action))
                errors.Add(new ValidationResult($"Invalid '{nameof(Action)}': undefined enum"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
