using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

/// <summary>
/// A node representing a property of a <see cref="IonList"/> or <see cref="IonSexp"/>
/// <para>
/// NOTE: <c>Index</c> is nullable because <c>null</c> values indicate "every" item
/// </para>
/// </summary>
public record ItemNode : DataTreeNode, INodeSelector<int?>
{
    internal static readonly string IndexMapAnnotationPrefix = $"{typeof(ItemNode).Namespace}.IndexMap@";

    /// <summary>
    /// Index of the item represented by this node
    /// <para>
    /// Note: null values are an indication to select ALL items - essentially, selecting all available items
    /// </para>
    /// </summary>
    public int? Index { get; }

    public int? Selector => Index;

    public ItemNode(int? index, bool isRequired, params DataTreeNode[] children)
    : base(isRequired, children)
    {
        Index = index;
    }

    public ItemNode(int? index, params DataTreeNode[] children)
    : this(index, true, children)
    {
    }
}
