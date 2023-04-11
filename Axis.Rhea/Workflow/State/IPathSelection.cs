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

        /// <summary>
        /// The <see cref="IonList"/>, <see cref="IonStruct"/>, or <see cref="IonSexp"/> object
        /// that contains the <see cref="IPathSelection.Value"/>.
        /// </summary>
        IIonType Container { get; }
    }

    public record PropertyPathSelection : IPathSelection
    {
        IIonType IPathSelection.Container => Container;

        public IonStruct? Container { get; }

        public IIonType Value { get; }

        /// <summary>
        /// The name of the selected property
        /// </summary>
        public string PropertyName { get; }

        public PropertyPathSelection(string propertyName, IIonType value, IIonType container = null)
        {
            PropertyName = propertyName.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(propertyName)}: '{propertyName}'"));

            Value = value ?? throw new ArgumentNullException(nameof(value));

            Container = container as IonStruct?;
        }
    }

    public record ListPathSelection : IPathSelection
    {
        IIonType IPathSelection.Container => Container;

        public IonList? Container { get; }

        public IIonType Value { get; }

        /// <summary>
        /// the index within the list at which the value was selected
        /// </summary>
        public int ListIndex { get; }

        public ListPathSelection(int index, IIonType value, IIonType container = null)
        {
            ListIndex = index.ThrowIf(
                i => i < 0,
                new ArgumentException($"Invalid {nameof(index)}: '{index}'"));

            Value = value ?? throw new ArgumentNullException(nameof(value));

            Container = container as IonList?;
        }
    }
}
