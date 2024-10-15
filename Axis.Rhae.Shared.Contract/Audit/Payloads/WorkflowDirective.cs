using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Service.Payloads;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    public interface IWorkflowDirectivePayload : IEventPayload
    {
        Identifier<Contract.Workflow.Identifiers.WorkflowDefinition> ActivityId { get; }
    }

    public record WorkflowDirectiveRequest : IWorkflowDirectivePayload
    {
        required public Identifier<Activity> ActivityId { get; init; }

        required public Record RequestData { get; init; }

        public EventType EventType => EventType.WorkflowDirectiveRequest;

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (ActivityId.IsDefault)
                errors.Add(new ValidationResult($"'{nameof(ActivityId)}' is default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }

    public record WorkflowDirectiveResponse : IWorkflowDirectivePayload
    {
        required public Identifier<Activity> ActivityId { get; init; }

        required public DirectiveFeedback ResponseData { get; init; }

        public EventType EventType => EventType.WorkflowDirectiveResponse;

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (ActivityId.IsDefault)
                errors.Add(new ValidationResult($"'{nameof(ActivityId)}' is default"));

            if (ResponseData is null)
                errors.Add(new ValidationResult($"'{nameof(ResponseData)}' is null"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }

    public record WorkflowDirectiveRetry : IWorkflowDirectivePayload
    {
        required public Identifier<Activity> ActivityId { get; init; }

        public EventType EventType => EventType.WorkflowDirectiveRetry;

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (ActivityId.IsDefault)
                errors.Add(new ValidationResult($"'{nameof(ActivityId)}' is default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
