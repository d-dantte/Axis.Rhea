using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    using Identifiers = Workflow.Identifiers;
    using DiaQueryPath = Dia.PathQuery.Path;

    public abstract record InvokeService : IEventPayload
    {
        public EventType EventType => EventType.ServiceInvocation;

        required public Identifier<Identifiers.Activity> SourceActivity { get; init; }

        public DiaQueryPath? StateSelector { get; init; }

        required public byte Attempts { get; init; }

        public virtual bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (SourceActivity.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(SourceActivity)}': default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
