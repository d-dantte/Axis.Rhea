using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Requests
{
    public class RetryWorkflow : IValidatable
    {
        /// <summary>
        /// The ID of the workflow to be re-tried
        /// </summary>
        public Identifier<Identifiers.Workflow> TargetId { get; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (TargetId.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(TargetId)}': default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
