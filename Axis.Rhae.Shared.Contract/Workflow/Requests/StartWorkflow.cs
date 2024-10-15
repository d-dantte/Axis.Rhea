using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Workflow.Requests
{
    public record StartWorkflow : IValidatable
    {
        /// <summary>
        /// Optional source. This value is given if the workflow is being created/started by another workflow
        /// </summary>
        required public Identifier<Identifiers.Workflow>? SourceWorkflowId { get; init; }

        /// <summary>
        /// ID of the workflow definition to start
        /// </summary>
        required public Identifier<Identifiers.WorkflowDefinition> WorkflowDefinitionId { get; init; }

        /// <summary>
        /// Optional data to start the workflow with. If not supplied, the default data is used.
        /// <para/>
        /// Note that this data must be structurally equivalent to the default-data of the workflow definition
        /// </summary>
        public Record? Data { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            var errors = new List<ValidationResult>();

            if (WorkflowDefinitionId.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(WorkflowDefinitionId)}': default"));

            if (SourceWorkflowId is not null
                && SourceWorkflowId!.Value.IsDefault)
                errors.Add(new ValidationResult($"Invalid '{nameof(SourceWorkflowId)}': default"));

            validationResults = [.. errors];
            return validationResults.IsEmpty();
        }
    }
}
