using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Service.Payloads;
using Axis.Rhae.Contract.Workflow.Identifiers;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Audit.Payloads
{
    public interface IServiceDirectivePayload : IEventPayload
    {
        Identifier<Activity> ActivityId { get; }
    }

    public record ServiceDirectiveRequest : IServiceDirectivePayload
    {
        required public Identifier<Activity> ActivityId { get; init; }

        required public Record RequestData { get; init; }

        public EventType EventType => EventType.ServiceDirectiveRequest;

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (ActivityId.IsDefault)
                errors.Add(new ValidationResult($"'{nameof(ActivityId)}' is default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }

    public record ServiceDirectiveResponse : IServiceDirectivePayload
    {
        required public Identifier<Activity> ActivityId { get; init; }

        required public DirectiveFeedback ResponseData { get; init; }

        public EventType EventType => EventType.ServiceDirectiveResponse;

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

    public record ServiceDirectiveRetry : IServiceDirectivePayload
    {
        required public Identifier<Activity> ActivityId { get; init; }

        public EventType EventType => EventType.ServiceDirectiveRetry;

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
