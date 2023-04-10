using System;

namespace Axis.Rhea.Core.Workflow
{
    public record WorkflowInstance
    {
        /// <summary>
        /// 
        /// </summary>
        public WorkflowDefinition Workflow { get; }

        /// <summary>
        /// 
        /// </summary>
        public ActiveTimeline CurrentTimeline { get; }

        public WorkflowInstance(WorkflowDefinition workflow, ActiveTimeline currentTimeline)
        {
            Workflow = workflow;
            CurrentTimeline = currentTimeline ?? throw new ArgumentNullException(nameof(currentTimeline));
        }
    }
}
