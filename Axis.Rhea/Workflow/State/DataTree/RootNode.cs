namespace Axis.Rhea.Core.Workflow.State.DataTree
{
    /// <summary>
    /// Represents the root of the DataTree, ergo, the root <see cref="Ion.Types.IIonType"/> value to be pruned.
    /// </summary>
    public record RootNode : DataTreeNode
    {
        public RootNode(bool isRequired, params DataTreeNode[] children)
        : base(isRequired, children)
        {
        }

        public RootNode(params DataTreeNode[] children)
            : this(true, children)
        {
        }
    }
}
