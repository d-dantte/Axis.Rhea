using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit
{
    /// <summary>
    /// Timeline event, recorded by the timeline
    /// </summary>
    public record TimelineEvent :
        IValidatable
    {
        required public Guid EventId{ get; init; }

        /// <summary>
        /// Timestamp for the event
        /// </summary>
        required public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        /// The event payload
        /// </summary>
        required public IEventPayload Payload { get; init; }

        /// <summary>
        /// The event type
        /// </summary>
        public EventType EventType => Payload.EventType;

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Payload is null)
                errors.Add(new ValidationResult($"Invalid '{nameof(Payload)}': null"));

            else if (!Payload.IsValid(out var payloadErrors))
                errors.AddRange(payloadErrors);

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
