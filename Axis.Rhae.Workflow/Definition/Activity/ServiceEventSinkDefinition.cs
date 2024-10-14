using Axis.Luna.Extensions;
using Axis.Rhae.Contract;
using Axis.Rhae.Workflow.Expression;
using System.Collections.Immutable;

namespace Axis.Rhae.Workflow.Definition.Activity
{
    #region Aliases
    using Identifiers = Contract.Workflow.Identifiers;

    using TransitionMap = (
        Identifier<Contract.Workflow.Identifiers.ResultLabel> Label,
        Identifier<Contract.Workflow.Identifiers.Activity> Activity);

    using ExpressionMap = (
        IStateExpression Expression,
        Identifier<Contract.Workflow.Identifiers.Activity> Activity);
    #endregion

    /// <summary>
    /// Activity that mimicks an event sink/handler: it only ever receives messages from external services.
    /// <para/>
    /// The events sent to the service sink contain data transformation instructions. After a successful data
    /// transformation has been performed, the list of <see cref="Expression.IStateExpression"/> predicates
    /// are applied on the data till one returns true, triggering a transformation.
    /// If no predicate succeeds, either throw the workflow in an error state, or ignore, depending on the
    /// <c>FailOnPredicateMiss</c> value. This means the activity will remain as the current one till the
    /// correct condition is met. For this reason, an optional ActiveDuration can be configured, after which a
    /// compulsury transition will happen.
    /// <para/>
    /// NOTE: the context made available to the scripting engine for this activity is ONLY the workflow data.
    /// </summary>
    public class ServiceEventSinkDefinition : BaseActivityDefinition
    {
        private static readonly string ExpressionResultLabelPrefix = "ServiceSinkExpressionResult_";

        public ImmutableDictionary<IStateExpression, TransitionMap> ExpressionMap { get; }

        public bool FailOnPredicateMiss { get; }

        public TimeSpan? ActiveDuration { get; }

        public ServiceEventSinkDefinition(
            Identifier<Identifiers.Activity> identifier,
            bool failOnPredicateMiss,
            TimeSpan? activeDuration,
            params ExpressionMap[] expressionList)
            : base(identifier, expressionList
                  .ToExpressionInfoList(ExpressionResultLabelPrefix)
                  .Select(t => (t.Label, t.Activity))
                  .ToArray())
        {
            FailOnPredicateMiss = failOnPredicateMiss;

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
