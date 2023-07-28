using Axis.Luna.Extensions;
using Axis.Rhea.Shared.Contract.Workflow.Activities;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Workflow;

/// <summary>
/// 
/// </summary>
public record ActivityDefinition
{
    /// <summary>
    /// 
    /// </summary>
    public string InitialActivity { get; }

    /// <summary>
    /// 
    /// </summary>
    public ImmutableDictionary<string, IActivity> ActivityMap { get; }


    public ActivityDefinition(string initialActivity, params IActivity[] activities)
    {
        InitialActivity = initialActivity.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid initial activity name: '{initialActivity}'"));

        ActivityMap = activities
            .ThrowIfAny(
                activity => activity is null,
                new ArgumentException($"{nameof(activities)} must not contain null values"))
            .ToImmutableDictionary(
                activity => activity.Name,
                activity => activity);
    }
}
