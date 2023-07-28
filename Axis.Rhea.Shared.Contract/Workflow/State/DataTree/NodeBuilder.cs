using Axis.Luna.Common;
using Axis.Luna.Extensions;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

internal sealed class NodeBuilder
{
    private readonly NodeType _nodeType;
    private readonly object? _selector;
    private bool _isRequired;
    private Dictionary<string, NodeBuilder> _children = new Dictionary<string, NodeBuilder>();

    public IEnumerable<NodeBuilder> Children => _children.Values.ToArray();

    public NodeBuilder(int? indexSelector)
    {
        _selector = indexSelector;
        _nodeType = NodeType.Item;
    }

    public NodeBuilder(string? propertySelector) 
    {
        _selector = propertySelector;
        _nodeType = NodeType.Property;
    }

    public NodeBuilder()
    {
        _nodeType = NodeType.Root;
    }

    public static NodeBuilder CreateBuilder() => new();

    public NodeBuilder WithRequired(bool required)
    {
        _isRequired = required;
        return this;
    }

    /// <summary>
    /// Adds the given child. If another similar child node exists, merge their children.
    /// </summary>
    /// <param name="child">The child to add</param>
    /// <exception cref="ArgumentNullException"></exception>
    public NodeBuilder WithChild(NodeBuilder child)
    {
        if (child is null)
            throw new ArgumentNullException(nameof(child));

        if (child._nodeType == NodeType.Root)
            throw new ArgumentException($"Cannot add a '{nameof(NodeType.Root)}' as a child");

        // Homogeneous?

        // Either child is found, or child this node contains a '*' child. Merge children.
        if (_children.TryGetValue(child._selector?.ToString() ?? "*", out var originalChild)
            || _children.TryGetValue("*", out originalChild))
        {
            child.Children.ForAll(c => originalChild.WithChild(c));
        }
        else
        {
            _children[child._selector?.ToString() ?? "*"] = child;
        }

        return this;
    }

    public NodeBuilder WithChildren(params NodeBuilder[] children)
    {
        children
            .ThrowIfNull(new ArgumentNullException(nameof(children)))
            .ForAll(c => WithChild(c));
        return this;
    }

    public DataTreeNode Build()
    {
        var children = _children
            .Select(child => child.Value.Build())
            .ToArray();

        return _nodeType switch
        {
            NodeType.Item => new ItemNode(
                isRequired: _isRequired,
                children: children,
                index: _selector.AsOptional().Map(Common.As<int>)),

            NodeType.Property => new PropertyNode(
                isRequired: _isRequired,
                children: children,
                property: (string?)_selector),

            NodeType.Root => new RootNode(_isRequired, children),

            _ => throw new InvalidOperationException($"Invalid Selector: '{_selector}'")
        };
    }

    #region Nested types
    internal enum NodeType
    {
        Root,
        Item,
        Property
    }
    #endregion
}
