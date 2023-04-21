using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.State.DataTree
{
    /// <summary>
    /// Represents a node in the object graph.
    /// <para>
    /// Each node may have 0 or more children. Where a node has children, they must all be of the same type. This means that a node either
    /// represents a struct - where items are mapped to property-names, or a list - where items are mapped to indexs.
    /// </para>
    /// </summary>
    public abstract record DataTreeNode : Pulsar.Grammar.Utils.IParsable<DataTreeNode>
    {
        internal const string PathDefinitionSymbol = "path-definition";
        internal const string PathNodeSymbol = "path-node";
        internal const string DepthSymbol = "depth";
        internal const string TabSymbol = "tab";
        internal const string OptionalSymbol = "optional";
        internal const string PropertySymbol = "property";
        internal const string IndexSymbol = "index";

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
        public Type ChildType { get; }

        protected DataTreeNode(bool isRequired, params DataTreeNode[] children)
        {
            IsRequired = isRequired;

            _children = children
                .ThrowIfNull(new ArgumentNullException(nameof(children)))
                .ToImmutableList();

            if (!IsHomogeneous(_children, out var type))
                throw new ArgumentException($"{nameof(children)} list is not homogeneous");

            ChildType = type;
        }

        /// <summary>
        /// Prune the given value
        /// </summary>
        /// <param name="value">The value to be pruned</param>
        /// <returns>A new value containing only values indicated by this <see cref="DataTreeNode"/></returns>
        public IIonType Prune(IIonType value) => Prune(this, value);

        #region Pulsar.Grammar.Utils.IParsable<DataTreeNode>
        public static DataTreeNode Parse(CSTNode node)
        {
            _ = TryParse(node, out IResult<DataTreeNode> result);

            return result switch
            {
                IResult<DataTreeNode>.DataResult data => data.Data,
                IResult<DataTreeNode>.ErrorResult error => error.Cause().Throw<DataTreeNode>(),
                _ => throw new InvalidOperationException($"Invalid result: {result}")
            };
        }

        public static bool TryParse(CSTNode node, out DataTreeNode value)
        {
            _ = TryParse(node, out IResult<DataTreeNode> result);

            value = result switch
            {
                IResult<DataTreeNode>.DataResult data => data.Data,
                _ => default
            };

            return !value.IsDefault();
        }

        public static DataTreeNode Parse(string text, IFormatProvider provider = null)
        {
            var recognitionResult = PulsarUtil.DataTreeGrammar
                .GetRecognizer(PathDefinitionSymbol)
                .Recognize(text);

            return recognitionResult switch
            {
                SuccessResult success => Parse(success.Symbol),
                ErrorResult error => error.Exception.Throw<DataTreeNode>(),
                FailureResult failure => throw new RecognitionException(failure),
                _ => throw new InvalidOperationException($"Invalid recognizer result: {recognitionResult}")
            };
        }

        public static bool TryParse(
            [NotNullWhen(true)] string text,
            IFormatProvider provider,
            [MaybeNullWhen(false)] out DataTreeNode result)
        {
            var parseResult = PulsarUtil.DataTreeGrammar
                .GetRecognizer(PathDefinitionSymbol)
                .Recognize(text);

            result = parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),
                _ => default
            };

            return !result.IsDefault();
        }

        public static bool TryParse(CSTNode pathDefinitionNode, out IResult<DataTreeNode> result)
        {
            if (pathDefinitionNode is null)
                throw new ArgumentNullException(nameof(pathDefinitionNode));

            var builderStack = NodeBuilder
                .CreateBuilder()
                .Enumerate()
                .ApplyTo(root => new Stack<NodeBuilder>(root));

            try
            {
                var rootNodeBuilder = pathDefinitionNode
                    .FindNodes(PathNodeSymbol)
                    .Aggregate(builderStack, (stack, pathNode) =>
                    {
                        var depth = pathNode.FindNodes($"{DepthSymbol}.{TabSymbol}").Count();
                        var isOptional = pathNode.TryFindNode(OptionalSymbol, out _);
                        var selector = pathNode
                            .FindNode($"{PropertySymbol}|{IndexSymbol}")
                            ?? throw new DataTreeParserException("Missing Index/Property");

                        var builder = selector.SymbolName switch
                        {
                            IndexSymbol => NodeBuilder
                                .CreateBuilder()
                                .WithRequired(!isOptional)
                                .WithSelector(selector
                                    .TokenValue()
                                    .UnwrapFrom("[", "]")
                                    .ApplyTo(int.Parse)),

                            PropertySymbol => NodeBuilder
                                .CreateBuilder()
                                .WithRequired(!isOptional)
                                .WithSelector(selector.TokenValue()),

                            _ => throw new DataTreeParserException($"Invalid selector symbol: {selector.SymbolName}")
                        };


                        if (depth == builderStack.Count - 1)
                        {
                            stack.Peek().WithChild(builder);
                        }
                        else if (depth == builderStack.Count)
                        {
                            var parent = stack
                                .Peek().Children
                                .LastOrDefault()
                                .ThrowIfNull(new DataTreeParserException($"Skipping path depth(s) is not allowed"))
                                .WithChild(builder);

                            stack.Push(parent);
                        }
                        else if (depth == builderStack.Count - 2)
                        {
                            _ = stack.Pop();
                            stack.Peek().WithChild(builder);
                        }
                        else throw new DataTreeParserException($"Skipping path depth(s) is not allowed");

                        return stack;
                    })
                    .Last();

                result = Result.Of(rootNodeBuilder.Build());
                return true;
            }
            catch(Exception e) 
            {
                result = Result.Of<DataTreeNode>(e);
                return false;
            }
        }
        #endregion

        #region Helpers
        internal static bool IsHomogeneous(IList<DataTreeNode> children, out Type nodeType)
        {
            nodeType = null;

            if (children.Count == 0)
                return true;

            foreach (var node in children)
            {
                if (node is null)
                    throw new ArgumentException($"null item found in {nameof(children)} list");

                if (nodeType is null)
                {
                    nodeType = node.GetType();
                    continue;
                }

                if (!nodeType.Equals(node.GetType()))
                {
                    nodeType = null;
                    return false;
                }
            }
            return true;
        }

        internal static IIonType Prune(DataTreeNode parentPath, IIonType parentValue)
        {
            IIonType result = (parentPath.ChildType?.Name, parentValue) switch
            {
                (nameof(PropertyNode), IonStruct parentStruct) => parentPath.Children
                    .SelectAs<PropertyNode>()
                    .Select(pathProperty =>
                    {
                        var ionValue =
                            parentStruct.Properties.TryGetvalue(pathProperty.Property, out var ion) ? ion :
                            !parentPath.IsRequired ? null : throw new MissingRequiredPropertyException(pathProperty.Property);

                        return (pathProperty, ion: ionValue);
                    })
                    .Where(tuple => tuple.ion is not null)
                    .Select(tuple => new IonStruct.Property(tuple.pathProperty.Property, Prune(tuple.pathProperty, tuple.ion)))
                    .ApplyTo(props =>
                    {
                        return new IonStruct(
                            new IonStruct.Initializer(
                                parentStruct.Annotations,
                                props.ToArray()));
                    }),

                (nameof(IndexNode), IonList parentList) => parentPath.Children
                    .SelectAs<IndexNode>()
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
                        var annotationText = IndexNode.IndexMapAnnotationPrefix + items
                            .Select(item => item.pathItem.Index)
                            .OrderBy(index => index)
                            .Select(index => index.ToString())
                            .JoinUsing(",");

                        var indexMapAnnotation = IIonType.Annotation.Parse(annotationText);
                        var annotations = parentList.Annotations.InsertAt(0, indexMapAnnotation).ToArray();

                        return new IonList(new IonList.Initializer(annotations, items.Select(item => item.ion).ToArray()));
                    }),

                _ => !parentPath.IsLeafNode 
                    ? throw new ArgumentException($"Invalid prune instruction. node-child-type: '{parentPath.ChildType}', ion-type: '{parentValue.Type}' ")
                    : parentValue
            };

            return result;
        }
        #endregion
    }

    /// <summary>
    /// Interface that presents a property used in selecting elements from <see cref="IIonType"/>s.
    /// </summary>
    /// <typeparam name="TSelectorType"></typeparam>
    public interface IIonSelector<TSelectorType>
    {
        /// <summary>
        /// The selector value
        /// </summary>
        TSelectorType Selector { get; }
    }
}
