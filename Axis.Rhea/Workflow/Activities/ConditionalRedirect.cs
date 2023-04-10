using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.State;
using System;
using System.Collections.Immutable;

namespace Axis.Rhea.Core.Workflow.Activities
{
    /// <summary>
    /// 
    /// </summary>
    public record ConditionalRedirect: IActivity
    {
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableList<(PathSegment Root, string Activity)> Conditions { get; }

        public ConditionalRedirect(string name, params (PathSegment root, string activity)[] conditions)
        {
            Name = name.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid activity name: '{name}'"));

            Conditions = conditions
                .ThrowIfNull(new ArgumentNullException(nameof(conditions)))
                .ThrowIfAny(
                    IsInvalidPair,
                    new ArgumentException($""))
                .ToImmutableList();
        }

        private static bool IsInvalidPair((PathSegment root, string activity) pair)
        {
            if (pair.root is null)
                return true;

            if (string.IsNullOrWhiteSpace(pair.activity))
                return true;

            return false;
        }
    }
}
