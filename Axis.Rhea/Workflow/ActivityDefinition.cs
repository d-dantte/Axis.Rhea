using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.Activities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Rhea.Core.Workflow
{
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
                    new ArgumentException($"Null activity found in activity list"))
                .ToImmutableDictionary(
                    activity => activity.Name,
                    activity => activity);
        }
    }
}
