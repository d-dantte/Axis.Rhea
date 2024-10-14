using Axis.Luna.Extensions;
using Axis.Rhae.Contract;
using Axis.Rhae.Workflow.Expression;
using System.Collections.Immutable;

namespace Axis.Rhae.Workflow.Definition.Activity
{
    using Identifiers = Contract.Workflow.Identifiers;

    /// <summary>
    /// Activity that mimicks an event sink/handler: it only ever receives events from CHILD WORKFLOW TIMELINEs, transitioning
    /// only if certain conditions are met.
    /// <para/>
    /// The WorkflowEventSink will do nothing if non of its predicates returns true. This means the activity will remain as
    /// the current one until the right event is received. For this reason, an optional ActiveDuration can be configured, after which a
    /// compulsury transition will happen.
    /// <para/>
    /// NOTE: the context made available to the scripting engine for this activity is ONLY the event object.
    /// </summary>
    public class WorkflowEventSinkDefinition : BaseActivityDefinition
    {
        private static readonly string ExpressionResultLabelPrefix = "WorkflowSinkExpressionResult_";
        public ImmutableDictionary<IStateExpression, (Identifier<Identifiers.ResultLabel> Label, Identifier<Identifiers.Activity> Activity)> ExpressionMap { get; }

        public TimeSpan? ActiveDuration { get; }

        public WorkflowEventSinkDefinition(
        Identifier<Identifiers.Activity> identifier,
            TimeSpan? activeDuration,
            params (IStateExpression, Identifier<Identifiers.Activity> Activity)[] expressionList)
            : base(identifier, expressionList
                  .ToExpressionInfoList(ExpressionResultLabelPrefix)
                  .Select(t => (t.Label, t.Activity))
                  .ToArray())
        {
            ActiveDuration = activeDuration.ThrowIf(
                ts => ts is TimeSpan tts
                    && tts.IsNegative(),
                ts => new ArgumentException($"Invalid timespan: {ts}"));

            ExpressionMap = expressionList
                .ToExpressionInfoList(ExpressionResultLabelPrefix)
                .ToImmutableDictionary(
                item => item.Expression,
                item => (item.Label, item.Activity));
        }
    }
}
