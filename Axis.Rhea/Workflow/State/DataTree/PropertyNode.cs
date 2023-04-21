using Axis.Ion.Types;
using Axis.Luna.Extensions;
using System;

namespace Axis.Rhea.Core.Workflow.State.DataTree
{
    /// <summary>
    /// A node representing a property of a <see cref="IonStruct"/>
    /// </summary>
    public record PropertyNode : DataTreeNode, IIonSelector<string>
    {
        /// <summary>
        /// Name of the property represented by this node
        /// </summary>
        public string Property { get; }

        public string Selector => Property;

        public PropertyNode(string property, bool isRequired, params DataTreeNode[] children)
        : base(isRequired, children)
        {
            Property = property.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(property)}: '{property}'"));
        }

        public PropertyNode(string property, params DataTreeNode[] children)
        : this(property, true, children)
        {
        }
    }
}
