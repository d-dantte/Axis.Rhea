using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Workflow.Identifiers;
using Semver;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Requests
{
    public record StartWorkflowPayload : IValidatable
    {
        required public Identifier<Namespace> Namespace { get; init; }

        required public Identifier<Name> Name { get; init; }

        required public SemVersion Version { get; init; }

        public bool TryValidate(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (Name.IsDefault)
                errors.Add(new ValidationResult("Invalid name: default"));

            if (Namespace.IsDefault)
                errors.Add(new ValidationResult("Invalid namespace: default"));

            if (Version is null)
                errors.Add(new ValidationResult("Invalid version: null"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
