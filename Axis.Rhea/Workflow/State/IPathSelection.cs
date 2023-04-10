using Axis.Ion.Types;
using Axis.Luna.Extensions;
using System;

namespace Axis.Rhea.Core.Workflow.State
{
    /// <summary>
    /// Represents the result of running a path selection from a <see cref="PathSegment"/>
    /// </summary>
    public interface IPathSelection
    {
        /// <summary>
        /// The selected value
        /// </summary>
        IIonType Value { get; }
    }

    public record PropertyPathSelection : IPathSelection
    {
        public IIonType Value { get; }

        /// <summary>
        /// The name of the selected property
        /// </summary>
        public string PropertyName { get; }

        public PropertyPathSelection(string propertyName, IIonType value)
        {
            PropertyName = propertyName.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(propertyName)}: '{propertyName}'"));

            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public record ListPathSelection : IPathSelection
    {
        public IIonType Value { get; }

        /// <summary>
        /// the index within the list at which the value was selected
        /// </summary>
        public int ListIndex { get; }

        public ListPathSelection(int index, IIonType value)
        {
            ListIndex = index.ThrowIf(
                i => i < 0,
                new ArgumentException($"Invalid {nameof(index)}: '{index}'"));

            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
