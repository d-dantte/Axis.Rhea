using Axis.Ion.Types;

namespace Axis.Rhea.Core.Workflow.State.DataTree
{
    /// <summary>
    /// A node representing a property of a <see cref="IonList"/> or <see cref="IonSexp"/>
    /// </summary>
    public record IndexNode : DataTreeNode, IIonSelector<int>
    {
        internal static readonly string IndexMapAnnotationPrefix = $"{typeof(IndexNode).Namespace}.IndexMap@";

        /// <summary>
        /// Index of the item represented by this node
        /// </summary>
        public int Index { get; }

        public int Selector => Index;

        public IndexNode(int index, bool isRequired, params DataTreeNode[] children)
        : base(isRequired, children)
        {
            Index = index;
        }

        public IndexNode(int index, params DataTreeNode[] children)
        : this(index, true, children)
        {
        }
    }
}
