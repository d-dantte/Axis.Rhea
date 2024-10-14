using Axis.Luna.Extensions;
using Axis.Luna.Result;
using Axis.Rhae.Contract;
using Axis.Rhae.Workflow.Expression;

namespace Axis.Rhae.Workflow.Definition.Activity
{
    #region Aliases
    using Identifiers = Contract.Workflow.Identifiers;

    using ExpressionMap = (
        IStateExpression Expression,
        Identifier<Contract.Workflow.Identifiers.Activity> Activity);

    using ExpressionInfo = (
        IStateExpression Expression,
        Identifier<Contract.Workflow.Identifiers.ResultLabel> Label,
        Identifier<Contract.Workflow.Identifiers.Activity> Activity);
    #endregion

    internal static class ExpressionExtensions
    {
        internal static ExpressionInfo[] ToExpressionInfoList(this
            ExpressionMap[] expressionList,
            string labelPrefix)
        {
            ArgumentNullException.ThrowIfNull(expressionList);

            return expressionList
                .ThrowIfNull(() => new ArgumentNullException(nameof(expressionList)))
                .ThrowIfAny(
                    item => item.Expression is null || item.Activity.IsDefault,
                    _ => new ArgumentException($"Invalid pair: contains null/default"))
                .Select((item, index) => (
                    item.Expression,
                    Label: Identifier<Identifiers.ResultLabel>
                        .Parse($"{labelPrefix}{index}")
                        .Resolve(),
                    item.Activity))
                .ToArray();
        }
    }
}
