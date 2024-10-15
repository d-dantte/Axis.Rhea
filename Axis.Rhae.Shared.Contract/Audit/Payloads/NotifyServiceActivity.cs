using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    public record NotifyServiceActivity : IEventPayload
    {
        public EventType EventType => EventType.ServiceEventNotification;

        required public Instruction Instruction { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (!Instruction.IsValid(out var instructionErrors))
                errors.AddRange(instructionErrors);

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
