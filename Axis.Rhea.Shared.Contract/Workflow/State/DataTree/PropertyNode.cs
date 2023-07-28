using Axis.Ion.Types;
using Axis.Luna.Extensions;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

/// <summary>
/// A node representing a property of a <see cref="IonStruct"/>
/// <para>
/// NOTE: <c>Property</c> is nullable because <c>null</c> values indicate "every" property
/// </para>
/// </summary>
public record PropertyNode : DataTreeNode, INodeSelector<string?>
{
    /// <summary>
    /// Name of the property represented by this node
    /// <para>
    /// Note: null values are an indication to select ALL properties - essentially, selecting all available properties
    /// </para>
    /// </summary>
    public string? Property { get; }

    public string? Selector => Property;

    public PropertyNode(string? property, bool isRequired, params DataTreeNode[] children)
    : base(isRequired, children)
    {
        Property = property;
    }

    public PropertyNode(string? property, params DataTreeNode[] children)
    : this(property, true, children)
    {
    }
}
