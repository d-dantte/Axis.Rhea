using Axis.Ion.Types;
using Axis.Luna.Extensions;
using Axis.Rhea.Shared.Contract.StateExpressions;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Workflow.Activities;

/// <summary>
/// 
/// </summary>
public record ConditionalTransition: IActivity
{
    public string Name { get; }

    /// <summary>
    /// 
    /// </summary>
    public ImmutableList<(ITypedExpression<IonBool> Expression, string Activity)> Conditions { get; }

    public ConditionalTransition(string name, params (ITypedExpression<IonBool> Expression, string activity)[] conditions)
    {
        Name = name.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid activity name: '{name}'"));

        Conditions = conditions
            .ThrowIfNull(new ArgumentNullException(nameof(conditions)))
            .ThrowIfAny(
                IsInvalidPair,
                new ArgumentException($"Invalid item found in the {nameof(conditions)} list"))
            .ToImmutableList();
    }

    private static bool IsInvalidPair((ITypedExpression<IonBool> expression, string activity) pair)
    {
        if (pair.expression is null)
            return true;

        if (string.IsNullOrWhiteSpace(pair.activity))
            return true;

        return false;
    }
}
