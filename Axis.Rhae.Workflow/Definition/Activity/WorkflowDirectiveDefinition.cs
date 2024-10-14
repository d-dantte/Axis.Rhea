using Axis.Luna.Extensions;
using Axis.Rhae.Contract;
using Axis.Rhae.Workflow.Definition.Identifiers;
using Axis.Rhae.Workflow.Definition.WorkflowDirective;

namespace Axis.Rhae.Workflow.Definition.Activity
{
    /// <summary>
    /// Represents starting another workflow, and possibly waiting for it's completion.
    /// </summary>
    public class WorkflowDirectiveDefinition : BaseActivityDefinition
    {
        public InvocationDefinition InvocationDefinition { get; }

        public WorkflowDirectiveDefinition(
            Identifier<Identifiers.Activity> identifier,
            InvocationDefinition invocationDefinition,
            (Identifier<ResultLabel> Result, Identifier<Identifiers.Activity> Activity)[] transitionList)
            : base(identifier, transitionList)
        {
            InvocationDefinition = invocationDefinition.ThrowIfNull(
                () => new ArgumentNullException(nameof(invocationDefinition)));
        }
    }
}
