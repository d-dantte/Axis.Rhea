using Axis.Ion.Types;
using Axis.Luna.Extensions;
using Axis.Rhea.Shared.Contract.Exceptions;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

/// <summary>
/// Represents a node in the object graph.
/// <para>
/// Each node may have 0 or more children. Where a node has children, they must all be of the same type. This means that a node either
/// represents a struct - where items are mapped to property-names, or a list - where items are mapped to indexs.
/// </para>
/// </summary>
public abstract record DataTreeNode
{
    internal const string PathsSymbol = "paths";
    internal const string PathSymbol = "path";

    private readonly ImmutableList<DataTreeNode> _children;

    /// <summary>
    /// Children nodes. This list is homogenous - allowing only 1 type of child node. 
    /// </summary>
    public IEnumerable<DataTreeNode> Children => _children;

    /// <summary>
    /// Indicates if this node is required. Required nodes raise exceptions if they are absent
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    /// Indicates that this node has no children
    /// </summary>
    public bool IsLeafNode => _children.Count == 0;

    /// <summary>
    /// The homogeneous type of the children, or null if the node has no child
    /// </summary>
    public NodeHomogeneity Homogeneity { get; }

    protected DataTreeNode(bool isRequired, params DataTreeNode[] children)
    {
        IsRequired = isRequired;

        _children = children
            .ThrowIfNull(new ArgumentNullException(nameof(children)))
            .ToImmutableList();

        if (!IsHomogeneous(_children, out var homogeneity))
            throw new ArgumentException($"{nameof(children)} list is not homogeneous");

        Homogeneity = homogeneity;
    }

    /// <summary>
    /// Prune the given value
    /// </summary>
    /// <param name="value">The value to be pruned</param>
    /// <returns>A new value containing only values indicated by this <see cref="DataTreeNode"/></returns>
    public IIonType Prune(IIonType parentValue)
    {
        if (this.Homogeneity == NodeHomogeneity.Empty)
            return parentValue.DeepCopy();

        else if (this.SelectsAll())
        {
            var allNode = this.Children.First();
            var nullError = new InvalidOperationException("Cannot prune ALL (*) on null ion value");
            var emptyError = new InvalidOperationException("Cannot prune ALL (*) on empty ion container");
            return (this.Homogeneity, parentValue) switch
            {
                (NodeHomogeneity.Properties, IonStruct parentStruct) =>
                    parentStruct.IsNull ? throw nullError :
                    parentStruct.Value!.Length == 0 && IsRequired ? throw emptyError :
                    parentStruct.Value!
                        .Select(prop => new IonStruct.Property(prop.NameText!, allNode.Prune(prop.Value)))
                        .ApplyTo(props => CopyProperties(parentStruct, props.ToArray())),

                (NodeHomogeneity.Items, IonList parentList) =>
                    parentList.IsNull ? throw nullError :
                    parentList.Value!.Length == 0 && IsRequired ? throw emptyError :
                    parentList.Value!
                        .Select((item, index) => (index, allNode.Prune(item)))
                        .ApplyTo(items => CopyItems(parentList, items.ToArray())),

                (NodeHomogeneity.Items, IonSexp parentSexp) =>
                    parentSexp.IsNull ? throw nullError :
                    parentSexp.Value!.Length == 0 && IsRequired ? throw emptyError :
                    parentSexp.Value!
                        .Select((item, index) => (index, allNode.Prune(item)))
                        .ApplyTo(items => CopyItems(parentSexp, items.ToArray())),

                _ => throw new InvalidOperationException($"Invalid pair [homogeneity: {this.Homogeneity}, ion-type: {parentValue?.Type}")
            };
        }
        else
        {
            return (this.Homogeneity, parentValue) switch
            {
                (NodeHomogeneity.Properties, IonStruct parentStruct) => this.Children
                    .SelectAs<PropertyNode>()
                    .Select(propertyNode =>
                    {
                        var ionValue =
                            parentStruct.Properties.TryGetvalue(propertyNode.Property!, out var ion) ? ion :
                            propertyNode.IsRequired ? throw new MissingRequiredPropertyException(propertyNode.Property!):
                            null;

                        return (node: propertyNode, ion: ionValue);
                    })
                    .Where(tuple => tuple.ion is not null)
                    .Select(tuple => new IonStruct.Property(tuple.node.Property!, tuple.node.Prune(tuple.ion!)))
                    .ApplyTo(props => CopyProperties(parentStruct, props.ToArray())),

                (NodeHomogeneity.Items, IonList parentList) => this.Children
                    .SelectAs<ItemNode>()
                    .Select(itemNode =>
                    {
                        var ionValue = (itemNode.Index >= 0 && itemNode.Index < parentList.Count) switch
                        {
                            true => parentList.Value![itemNode.Index!.Value],
                            false => itemNode.IsRequired
                                ? throw new MissingRequiredIndexException(itemNode.Index!.Value)
                                : null
                        };

                        return (itemNode, ion: ionValue!);
                    })
                    .Where(tuple => tuple.ion is not null)
                    .Select(tuple => (tuple.itemNode.Index!.Value, tuple.itemNode.Prune(tuple.ion!)))
                    .ApplyTo(items => CopyItems(parentList, items.ToArray())),

                _ => throw new ArgumentException($"Invalid prune instruction. homogeneity: '{Homogeneity}', ion-type: '{parentValue.Type}'")
            };
        }
    }

    #region Helpers
    internal static bool IsHomogeneous(IList<DataTreeNode> children, out NodeHomogeneity homogeneity)
    {
        NodeHomogeneity? _homogeneity = null;
        foreach (var node in children)
        {
            if (node is null)
                throw new ArgumentException($"{nameof(children)} must not contain null values");

            var currentHomogeneity = node switch
            {
                ItemNode => NodeHomogeneity.Items,
                PropertyNode => NodeHomogeneity.Properties,
                _ => throw new ArgumentException($"Invalid node type: '{(node?.GetType().Name ?? "null")}'")
            };

            if (_homogeneity is null)
                _homogeneity = homogeneity = currentHomogeneity;

            else if (_homogeneity != currentHomogeneity)
            {
                homogeneity = NodeHomogeneity.NonHomogeneous;
                return false;
            }
        }

        homogeneity = _homogeneity ?? NodeHomogeneity.Empty;
        return true;
    }

    internal static IonStruct CopyProperties(IonStruct origin, IonStruct.Property[] properties)
    {
        return new IonStruct(
            new IonStruct.Initializer(
                origin.Annotations,
                properties.ToArray()));
    }

    internal static IonList CopyItems(IonList origin, (int index, IIonType ion)[] items)
    {
        // tag the new list with an index-map annotation, telling the original index of the items in the list
        var annotationText = ItemNode.IndexMapAnnotationPrefix + items
            .Select(item => item.index)
            .OrderBy(index => index)
            .Select(index => index.ToString())
            .JoinUsing(",");

        var indexMapAnnotation = IIonType.Annotation.Parse(annotationText);
        var annotations = origin.Annotations.InsertAt(0, indexMapAnnotation).ToArray();

        return new IonList(new IonList.Initializer(annotations, items.Select(item => item.ion!).ToArray()));
    }

    internal static IonSexp CopyItems(IonSexp origin, (int index, IIonType ion)[] items)
    {
        // tag the new list with an index-map annotation, telling the original index of the items in the list
        var annotationText = ItemNode.IndexMapAnnotationPrefix + items
            .Select(item => item.index)
            .OrderBy(index => index)
            .Select(index => index.ToString())
            .JoinUsing(",");

        var indexMapAnnotation = IIonType.Annotation.Parse(annotationText);
        var annotations = origin.Annotations.InsertAt(0, indexMapAnnotation).ToArray();

        return new IonSexp(new IonSexp.Initializer(annotations, items.Select(item => item.ion!).ToArray()));
    }
    #endregion
}

public enum NodeHomogeneity
{
    Empty,
    Items,
    Properties,
    NonHomogeneous
}
