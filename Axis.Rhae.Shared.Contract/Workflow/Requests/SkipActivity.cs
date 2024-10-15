using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Requests
{
    public record SkipActivity : IValidatable
    {
        /// <summary>
        /// The id of the activity to skip to. This activity MUST be a part of the WorkflowDefinition
        /// </summary>
        required public Identifier<Identifiers.Activity> ActivityId { get; init; }


        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (ActivityId.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(ActivityId)}': default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
