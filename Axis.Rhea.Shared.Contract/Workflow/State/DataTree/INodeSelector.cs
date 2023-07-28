using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

/// <summary>
/// Interface that presents a property used in selecting elements from <see cref="IIonType"/>s.
/// </summary>
/// <typeparam name="TSelectorType"></typeparam>
public interface INodeSelector<TSelectorType>
{
    /// <summary>
    /// The selector value
    /// </summary>
    TSelectorType Selector { get; }
}

public static class NodeSelectorHelper
{
    public static bool SelectsAll(this DataTreeNode treeNode)
    {
        return treeNode switch
        {
            null => throw new ArgumentNullException(nameof(treeNode)),
            _ => treeNode.Children.Count() == 1 && treeNode.Children.First() switch
            {
                INodeSelector<string?> propSelector => propSelector.Selector is null,
                INodeSelector<int?> itemSelector => itemSelector.Selector is null,
                _ => throw new ArgumentException($"Invalid selector type: '{treeNode.Children.First().GetType()}'")
            }
        };
    }
}
