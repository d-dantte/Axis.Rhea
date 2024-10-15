using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Audit;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Responses
{
    public record TimelineSnapshot : IValidatable
    {
        required public ImmutableArray<TimelineEvent> Events { get; init; }


        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Events.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(Events)}': default"));

            else if (Events.Any(@event => @event is null))
                errors.Add(new ValidationResult($"Invalid '{nameof(Events)}': contains null"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
