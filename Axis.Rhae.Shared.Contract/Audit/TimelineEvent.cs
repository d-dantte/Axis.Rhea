using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit
{
    /// <summary>
    /// Timeline event, recorded by the timeline
    /// </summary>
    public record TimelineEvent :
        ICorrelatable,
        IValidatable
    {
        required public Guid CorrelationId{ get; init; }

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

        public bool TryValidate(out ValidationResult[] validationException)
        {
            var errors = new List<ValidationResult>();

            if (Payload is null)
                errors.Add(new ValidationResult($"{nameof(Payload)} is null"));

            else if (!Payload.TryValidate(out var payloadErrors))
                errors.AddRange(payloadErrors);

            validationException = [.. errors];
            return validationException.IsEmpty();
        }
    }
}
