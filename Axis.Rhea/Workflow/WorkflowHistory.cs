using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Axis.Rhea.Core.Workflow
{
    public record WorkflowHistory
    {
        private readonly IImmutableList<FrozenTimeline> timelines;

        /// <summary>
        /// 
        /// </summary>
        public string WorkflowId { get; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<FrozenTimeline> Timelines => timelines;


        public WorkflowHistory(string workflowId, IEnumerable<FrozenTimeline> timelines)
        {
            WorkflowId = workflowId.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invlalid id: {workflowId}"));

            this.timelines = timelines?
                .ThrowIfNull(new ArgumentNullException(nameof(timelines)))
                .ToImmutableList();
        }

        public WorkflowHistory(string workflowId, params FrozenTimeline[] timelines)
        : this(workflowId, (IEnumerable<FrozenTimeline>)timelines)
        {
        }
    }
}
