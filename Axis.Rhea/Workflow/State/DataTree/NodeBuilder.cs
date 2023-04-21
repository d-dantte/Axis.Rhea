using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.State.DataTree
{
    public sealed class NodeBuilder
    {
        private object _selector;
        private bool _isRequired;
        private List<NodeBuilder> _children = new List<NodeBuilder>();

        public IEnumerable<NodeBuilder> Children => _children.ToArray();

        public NodeBuilder() { }

        public static NodeBuilder CreateBuilder() => new NodeBuilder();

        public NodeBuilder WithRequired(bool required)
        {
            _isRequired = required;
            return this;
        }

        public NodeBuilder WithSelector(int indexSelector)
        {
            _selector = indexSelector;
            return this;
        }

        public NodeBuilder WithSelector(string propertySelector)
        {
            _selector = propertySelector;
            return this;
        }

        public NodeBuilder WithChild(NodeBuilder child)
        {
            _children.Add(child);
            return this;
        }

        public NodeBuilder WithChildren(params NodeBuilder[] children)
        {
            _children.AddRange(children
                .ThrowIfNull(new ArgumentNullException(nameof(children)))
                .ThrowIfAny(n => n is null, new ArgumentException($"null item found in {nameof(children)}")));
            return this;
        }

        public DataTreeNode Build()
        {
            var children = _children
                .Select(child => child.Build())
                .ToArray();

            return _selector switch
            {
                int index => new IndexNode(index, _isRequired, children),
                string property => new PropertyNode(property, _isRequired, children),
                null => new RootNode(_isRequired, children),
                _ => throw new InvalidOperationException($"Invalid Selector: '{_selector}'")
            };
        }
    }
}
