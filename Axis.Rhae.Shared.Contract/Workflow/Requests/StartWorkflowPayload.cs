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

        public bool TryValidate(out AggregateException? validationException)
        {
            validationException = null;
            var errors = new List<ValidationException>();

            if (Name.IsDefault)
                errors.Add(new ValidationException("Invalid name: default"));

            if (Namespace.IsDefault)
                errors.Add(new ValidationException("Invalid namespace: default"));

            if (Version is null)
                errors.Add(new ValidationException("Invalid version: null"));

            if (errors.Count == 0)
                return true;

            else
            {
                validationException = new AggregateException([.. errors]);
                return false;
            }
        }
    }
}
