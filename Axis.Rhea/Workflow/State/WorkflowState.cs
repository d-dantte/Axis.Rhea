using Axis.Ion.Types;

namespace Axis.Rhea.Core.Workflow.State
{
    /// <summary>
    /// 
    /// </summary>
    public record WorkflowState
    {
        /// <summary>
        /// 
        /// </summary>
        public IonStruct Data { get; }

        public WorkflowState(IonStruct data)
        {
            Data = data;
        }
    }
}
