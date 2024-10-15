using Axis.Dia.Core.Types;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Contract.Service
{
    public class Request :
        ICorrelatable,
        IValidatable
    {
        required public Guid CorrelationId { get; init; }

        /// <summary>
        /// This is either the entire workflow-data at the time the call was made, or the a pruned version
        /// based off the <c>StateSelector</c>
        /// </summary>
        required public Record Payload { get; init; }

        public bool IsValid(out ValidationResult[] validationResults)
        {
            validationResults = [];
            return true;
        }
    }
}
