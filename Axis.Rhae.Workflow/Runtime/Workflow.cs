using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Rhae.Contract;
using Axis.Rhae.Contract.Workflow.Identifiers;
using Axis.Rhae.Workflow.Audit;
using Axis.Rhae.Workflow.Definition;
using Axis.Rhae.Workflow.Definition.Activity;
using Axis.Rhae.Workflow.Runtime.Activity;
using System.Collections.Immutable;

namespace Axis.Rhae.Workflow.Runtime
{
    using Identifiers = Contract.Workflow.Identifiers;

    public class Workflow
    {
        public WorkflowStatus Status { get; private set; }

        public Definition.WorkflowDefinition Definition { get; }

        /// <summary>
        /// Any workflow definition referenced by activities of this workflow - e.g, <see cref="WorkflowDirectiveDefinition"/> references other
        /// workflows to be started by the current instance. These referenced workflows need to be available at creation time.
        /// </summary>
        public ImmutableDictionary<Identifier<Identifiers.WorkflowDefinition>, Definition.WorkflowDefinition> ChildWorkflows { get; }

        public Guid InstanceId { get; }

        public string? InstanceAlias { get; }

        public Identifier<Identifiers.Workflow> WorkflowFQN => Identifiers.Workflow.ToIdentifier(
            Definition.Namespace,
            Definition.Name,
            Definition.Version,
            InstanceId,
            InstanceAlias);

        public Record Data { get; }

        public ITimeline Timeline { get; }

        public IActivityState? CurrentActivity { get; private set; }

        public Workflow(
            Definition.WorkflowDefinition workflowDefinition,
            Guid instanceId,
            string? instanceAlias,
            ITimeline timeline,
            Record data,
            WorkflowStatus status = WorkflowStatus.Created,
            params (Identifier<Identifiers.WorkflowDefinition> Id, Definition.WorkflowDefinition Definition)[] childWorkflows)
        {
            ArgumentNullException.ThrowIfNull(workflowDefinition);
            ArgumentNullException.ThrowIfNull(timeline);
            ArgumentNullException.ThrowIfNull(childWorkflows);

            Definition = workflowDefinition;
            InstanceId = instanceId;
            InstanceAlias = instanceAlias;
            Timeline = timeline;

            Data = data.ThrowIfDefault(
                _ => new ArgumentException("Invalid data: default"));

            Status = status.ThrowIfNot(
                Enum.IsDefined,
                _ => new ArgumentException($"Invalid status: {status}"));

            ChildWorkflows = childWorkflows
                .ThrowIfAny(
                    tuple => tuple.Id.IsDefault
                        || tuple.Definition is null
                        || tuple.Definition.Identifier.Equals(tuple.Id),
                    tuple => throw new ArgumentException("Invalid pair: contains null/default"))
                .ToImmutableDictionary(tuple => tuple.Id, tuple => tuple.Definition);
        }

        public Workflow(
            Definition.WorkflowDefinition workflowDefinition,
            Guid instanceId,
            ITimeline timeline,
            Record data,
            WorkflowStatus status = WorkflowStatus.Created,
            params (Identifier<Identifiers.WorkflowDefinition> Id, Definition.WorkflowDefinition Definition)[] childWorkflows)
            : this(workflowDefinition, instanceId, null, timeline, data, status, childWorkflows)
        { }

        /// <summary>
        /// Transition to a new state based on the given label.
        /// <para/>
        /// For the initial transition, the <paramref name="label"/> is <c>default</c>, because there is no previous activity result label.
        /// </summary>
        /// <param name="label">The label used to switch via the transition table, or default for the initial transition call</param>
        /// <param name="stateFactory">The activity state factory function</param>
        /// <returns>The state for the activity just transitioned to</returns>
        public IActivityState Transition(
            Identifier<ResultLabel> label,
            Func<IActivityDefinition, IActivityState> stateFactory)
        {
            ArgumentNullException.ThrowIfNull(stateFactory);

            return CurrentActivity = (label.IsDefault, CurrentActivity) switch
            {
                (true, null) => Definition
                    .Activities[Definition.StartActivity]
                    .ApplyTo(stateFactory.Invoke),

                (false, not null) => Definition
                    .Activities[CurrentActivity.ActivityIdentifier]
                    .TransitionTable[label]
                    .ApplyTo(id => Definition.Activities[id])
                    .ApplyTo(stateFactory.Invoke),

                _ => throw new InvalidOperationException($"Invalid state [label: {label}, CurrentActivity: {CurrentActivity?.ActivityIdentifier}]")
            };
        }
    }
}
