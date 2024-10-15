using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract
{
    /// <summary>
    /// Represents an event that occured in the workflow's lifecycle
    /// </summary>
    /// <typeparam name="TEventData">The event data type</typeparam>
    public record Event<TEventData> :
        ICorrelatable,
        IValidatable
    {
        required public Guid CorrelationId { get; init; }

        /// <summary>
        /// Timestamp for the event
        /// </summary>
        required public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        /// The event data
        /// </summary>
        required public TEventData Data { get; init; }

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Data is null)
                errors.Add(new ValidationResult($"{nameof(Data)} is null"));

            else if (Data is IValidatable v && !v.TryValidate(out var payloadErrors))
                errors.AddRange(payloadErrors);

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }

    /// <summary>
    /// Notifies observers that an event occured
    /// </summary>
    /// <typeparam name="TEventData">The event data type</typeparam>
    public record EventNotification<TEventData> :
        ICorrelatable,
        IValidatable
    {
        required public Event<TEventData> Event { get; init; }

        public Guid CorrelationId => Event.CorrelationId;

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Event is null)
                errors.Add(new ValidationResult($"{nameof(Event)} is null"));

            if (Event is IValidatable v && !v.TryValidate(out var payloadErrors))
                errors.AddRange(payloadErrors);

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
