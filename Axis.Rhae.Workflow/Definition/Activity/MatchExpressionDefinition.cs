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
    /// Represents an expression based on the context-data, that resolves to a string. The resolved value is used to find the next activity to transition to
    /// from the transition table.
    /// </summary>
    public class MatchExpressionDefinition : BaseActivityDefinition
    {
        private static readonly string ExpressionResultLabelPrefix = "ExpressionResult_";

        public ImmutableDictionary<IStateExpression, TransitionMap> ExpressionMap { get; }

        public MatchExpressionDefinition(
            Identifier<Identifiers.Activity> identifier,
            params ExpressionMap[] expressionList)
            : base(identifier, expressionList
                  .ToExpressionInfoList(ExpressionResultLabelPrefix)
                  .Select(t => (t.Label, t.Activity))
                  .ToArray())
        {
            ExpressionMap = expressionList
                .ToExpressionInfoList(ExpressionResultLabelPrefix)
                .ToImmutableDictionary(
                    item => item.Expression,
                    item => (item.Label, item.Activity));
        }
    }
}
