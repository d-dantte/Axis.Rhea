using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.State;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Axis.Rhea.Core.Workflow
{
    /// <summary>
    /// History of all timelines for this workflow. This is an immutable record
    /// </summary>
    public record WorkflowHistory
    {
        private readonly IImmutableList<FrozenTimeline> timelines;

        /// <summary>
        /// The unique id of the workflow
        /// </summary>
        public string WorkflowId { get; }

        /// <summary>
        /// All timelines for the workflow
        /// </summary>
        public IEnumerable<FrozenTimeline> Timelines => timelines;

        /// <summary>
        /// The initial state of the workflow
        /// </summary>
        public WorkflowState State { get; }


        public WorkflowHistory(string workflowId, WorkflowState state, IEnumerable<FrozenTimeline> timelines)
        {
            WorkflowId = workflowId.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invlalid id: {workflowId}"));

            State = state  ?? throw new ArgumentNullException(nameof(state));

            this.timelines = timelines?
                .ThrowIfNull(new ArgumentNullException(nameof(timelines)))
                .ToImmutableList();
        }

        public WorkflowHistory(string workflowId, WorkflowState state, params FrozenTimeline[] timelines)
        : this(workflowId, state, (IEnumerable<FrozenTimeline>)timelines)
        {
        }
    }
}
