using Axis.Ion.Types;
using Axis.Luna.Extensions;
using Axis.Rhea.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.State
{
    public abstract record PathNode
    {
        /// <summary>
        /// Children nodes
        /// </summary>
        public abstract IEnumerable<PathNode> Children { get; }

        /// <summary>
        /// Indicates if this node is required. Required nodes raise exceptions if they are absent
        /// </summary>
        public abstract bool IsRequired { get; }

        /// <summary>
        /// Indicates that this node has no children
        /// </summary>
        public abstract bool IsLeafNode { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract IIonType Prune(IIonType type);

        #region Helpers
        internal static bool IsNotHomogeneous(IEnumerable<PathNode> children)
        {
            return children switch
            {
                IEnumerable<NameNode>
                or IEnumerable<IndexNode> => true,

                _ => false
            };
        }

        internal static IIonType Prune(PathNode parentPath, IIonType parentValue)
        {
            IIonType result = (parentPath.Children, parentValue) switch
            {
                (IEnumerable<NameNode> pathProperties, IonStruct parentStruct) => pathProperties
                    .Select(pathProperty =>
                    {
                        var ionValue =
                            parentStruct.Properties.TryGetvalue(pathProperty.Name, out var ion) ? ion :
                            !parentPath.IsRequired ? null : throw new MissingRequiredPropertyException(pathProperty.Name);

                        return (pathProperty, ion: ionValue);
                    })
                    .Where(tuple => tuple.ion is not null)
                    .Select(tuple => new IonStruct.Property(tuple.pathProperty.Name, Prune(tuple.pathProperty, tuple.ion)))
                    .ApplyTo(props =>
                    {
                        return new IonStruct(
                            new IonStruct.Initializer(
                                parentStruct.Annotations,
                                props.ToArray()));
                    }),

                (IEnumerable<IndexNode> pathItems, IonList parentList) => pathItems
                    .Select(pathItem =>
                    {
                        var ionValue = (pathItem.Index >= 0 && pathItem.Index < parentList.Count) switch
                        {
                            true => parentList.Value[pathItem.Index],
                            false => pathItem.IsRequired switch
                            {
                                true => throw new MissingRequiredIndexException(pathItem.Index),
                                false => null
                            }
                        };

                        return (pathItem, ion: ionValue);
                    })
                    .Where(tuple => tuple.ion is not null)
                    .ApplyTo(items =>
                    {
                        // tag the new list with an index-map annotation, telling the original index of the items in the list
                        var indexMapAnnotation = IndexNode.IndexMapAnnotationPrefix + items
                            .Select(item => item.pathItem.Index)
                            .OrderBy(index => index)
                            .Select(index => index.ToString())
                            .JoinUsing(",")
                            .ApplyTo(IIonType.Annotation.Parse);

                        return new IonList(
                            new IonList.Initializer(
                                parentList.Annotations.InsertAt(0, indexMapAnnotation).ToArray(),
                                items.Select(item => item.ion).ToArray()));
                    }),

                _ => parentPath.IsLeafNode ? parentValue:  throw new ArgumentException($"")
            };

            return result;
        }
        #endregion
    }

    public record NameNode: PathNode
    {
        private readonly ImmutableList<PathNode> _children;

        public override IEnumerable<PathNode> Children => _children;

        public override bool IsRequired { get; }

        public override bool IsLeafNode => _children.Count == 0;

        /// <summary>
        /// Name of the property represented by this node
        /// </summary>
        public string Name { get; }

        public NameNode(string name, bool isRequired, params PathNode[] children)
        {
            IsRequired = isRequired;

            Name = name.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(name)}: '{name}'"));

            _children = children
                .ThrowIfNull(new ArgumentNullException(nameof(children)))
                .ThrowIf(
                    PathNode.IsNotHomogeneous,
                    new ArgumentException($""))
                .ToImmutableList();
        }

        public NameNode(string name, params PathNode[] children)
            :this(name, true, children)
        {
        }

        public override IIonType Prune(IIonType value) => PathNode.Prune(this, value);
    }

    public record IndexNode: PathNode
    {
        internal static readonly string IndexMapAnnotationPrefix = $"{typeof(IndexNode).Namespace}.IndexMap@";

        private readonly ImmutableList<PathNode> _children;

        public override IEnumerable<PathNode> Children => _children;

        public override bool IsRequired { get; }

        public override bool IsLeafNode => _children.Count == 0;

        /// <summary>
        /// Index of the item represented by this node
        /// </summary>
        public int Index { get; }

        public IndexNode(int index, bool isRequired, params PathNode[] children)
        {
            IsRequired = isRequired;

            Index = index;

            _children = children?
                .ToImmutableList()
                .ThrowIfNull(new ArgumentNullException(nameof(children)))
                .ThrowIf(
                    PathNode.IsNotHomogeneous,
                    new ArgumentException($""));
        }

        public IndexNode(int index, params PathNode[] children)
            : this(index, true, children)
        {
        }

        public override IIonType Prune(IIonType value) => PathNode.Prune(this, value);
    }

    public record RootNode : PathNode
    {
        private readonly ImmutableList<PathNode> _children;

        public override IEnumerable<PathNode> Children => _children;

        public override bool IsRequired { get; }

        public override bool IsLeafNode => _children.Count == 0;

        public RootNode(bool isRequired, params PathNode[] children)
        {
            IsRequired = isRequired;

            _children = children
                .ThrowIfNull(new ArgumentNullException(nameof(children)))
                .ThrowIf(
                    PathNode.IsNotHomogeneous,
                    new ArgumentException($""))
                .ToImmutableList();
        }

        public RootNode(params PathNode[] children)
            :this(true, children)
        {
        }

        public override IIonType Prune(IIonType value) => PathNode.Prune(this, value);
    }
}
