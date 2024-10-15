using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Workflow;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    using Identifiers = Workflow.Identifiers;

    /// <summary>
    /// Raised to i
    /// </summary>
    public abstract record InvokeWorkflow : IEventPayload
    {
        public EventType EventType => EventType.WorkflowInvocation;

        required public Identifier<Identifiers.Activity> SourceActivity { get; init; }

        required public Identifier<Identifiers.WorkflowDefinition> WorkflowDefinitionId { get; init; }

        public Record? Data { get; init; }

        required public byte Attempts { get; init; }

        public virtual bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (SourceActivity.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(SourceActivity)}': default"));

            if (Enum.IsDefined(EventType))
                errors.Add(new ValidationResult($"Invalid '{nameof(EventType)}': undefined enum"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}