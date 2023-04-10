using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.State;
using System;

namespace Axis.Rhea.Core.Workflow
{
    /// <summary>
    /// TODO: how do we support sub-graphs?
    /// </summary>
    public record WorkflowDefinition
    {
        /// <summary>
        /// Name for the workflow. This can be repeated, though it is not tracked/managed by the system
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// version of the workflow
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Unique identifier across the entire system
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 
        /// </summary>
        public string FullyQualifiedIdentifier => $"{Id}::{Name}#{Version}";

        /// <summary>
        /// 
        /// </summary>
        public WorkflowState State { get; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDefinition ActivityDefinition { get; }

        /// <summary>
        /// 
        /// </summary>
        public ServiceInvocationDefinition ServiceInvocationDefinition { get; }

        public WorkflowDefinition(
            string id,
            WorkflowState state,
            ActivityDefinition activityDefinition,
            ServiceInvocationDefinition serviceInvocationDefinition)
        {
            Id = id.ThrowIf(string.IsNullOrWhiteSpace, new ArgumentException($"Invalid id: {id}"));
            State = state.ThrowIfNull(new ArgumentNullException(nameof(state)));
            ActivityDefinition = activityDefinition.ThrowIfNull(new ArgumentNullException(nameof(activityDefinition)));
            ServiceInvocationDefinition = serviceInvocationDefinition.ThrowIfNull(new ArgumentNullException(nameof(serviceInvocationDefinition)));
        }
    }
}
