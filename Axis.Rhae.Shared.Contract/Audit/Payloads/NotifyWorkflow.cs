using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    public record NotifyWorkflow : IEventPayload
    {
        public EventType EventType => EventType.WorkflowEventNotification;

        /// <summary>
        /// The "child" workflow from which the event was raised
        /// </summary>
        required public Identifier<Workflow.Identifiers.Workflow> Source { get; init; }

        /// <summary>
        /// The source eventId
        /// </summary>
        required public Guid SourceEventId { get; init; }

        required public EventType SourceEventType { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Source.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(Source)}': default"));

            if (!Enum.IsDefined(SourceEventType))
                errors.Add(new ValidationResult($"Invalid '{nameof(SourceEventType)}': undefined enum"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
