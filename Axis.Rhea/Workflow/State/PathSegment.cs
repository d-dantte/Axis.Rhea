using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.State
{
    public record PathSegment : Pulsar.Grammar.Utils.IParsable<PathSegment>
    {
        internal const string QuerySymbol = "query";
        internal const string PredicateNodeSymbol = "predicate-node";
        internal const string PropertyNameSymbol = "property-name";
        internal const string ListIndexSymbol = "list-index";
        internal const string ConditionalsSymbol = "conditionals";
        internal const string PropertyConditionalSymbol = "property-conditional";
        internal const string ListConditionalSymbol = "list-conditional";
        internal const string AnnotationConditionalSymbol = "annotation-conditional";

        internal static readonly string ListMarkerAnnotationPrefix = "Rhea.PathSelection.ListMarker@";

        /// <summary>
        /// 
        /// </summary>
        public PathSegment Next { get; }

        public IPathPredicate Predicate { get; }
        public AnnotationPredicate? AnnotationPredicate { get; }


        public PathSegment(
            IPathPredicate predicate,
            AnnotationPredicate? annotationPredicate = null,
            PathSegment next = null)
        {
            Next = next;
            Predicate = predicate.ThrowIfNull(new ArgumentNullException(nameof(predicate)));
            AnnotationPredicate = annotationPredicate;
        }

        public PathSegment(
            IPathPredicate predicate,
            PathSegment next = null)
            : this(predicate, null, next)
        {
        }

        /// <summary>
        /// Find and copy (pick) all data along this path and it's child paths that fit the corresponding predicates,
        /// returning the objects at the start of the object graph.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public IEnumerable<IPathSelection> Pick(IIonType value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            // match annotations
            if (AnnotationPredicate is not null
                && !value.Annotations.Any(AnnotationPredicate.Value.Execute))
                return Enumerable.Empty<IPathSelection>();

            // match property/item
            return (value, Predicate) switch
            {
                (IonStruct @struct, PropertyPredicate propertyPredicate) => @struct.Value
                    .Where(propertyPredicate.Execute)
                    .Select(Copy),

                (IonList list, ListPredicate listPredicate) => list.Value
                    .Select((item, index) => (index, item))
                    .Where(listPredicate.Execute)
                    .Select(Copy),

                _ => throw new ArgumentException($"Supplied type '{value.Type}' does not match the underlying predicate type '{Predicate.GetType()}'")
            };
        }

        /// <summary>
        /// Continuously drill down through the object graph, following the path segments, and return the data that
        /// matches the final predicate.
        /// </summary>
        /// <param name="value">the data to test predicates on</param>
        /// <returns>The data that fits all predicates, or an empty enumerable</returns>
        public IEnumerable<IPathSelection> Select(IIonType value)
        {
            var paths = Paths().ToArray();
            var seedSelection = new IPathSelection[] { new PropertyPathSelection("$user", value) };
            return paths.Aggregate(seedSelection, (selections, path) =>
            {
                return selections
                    .SelectMany(selection =>
                    {
                        return (selection.Value, path.Predicate) switch
                        {
                            (IonStruct @struct, PropertyPredicate propertyPredicate) => @struct.Value
                                .Where(propertyPredicate.Execute)
                                .Select(property => new PropertyPathSelection(property.NameText, property.Value, selection.Value))
                                .Cast<IPathSelection>(),

                            (IonList list, ListPredicate listPredicate) => list.Value
                                .Select((item, index) => (index, item))
                                .Where(listPredicate.Execute)
                                .Select(listItem => new ListPathSelection(listItem.index, listItem.item, selection.Value))
                                .Cast<IPathSelection>(),

                            _ => throw new ArgumentException($"Supplied type '{selection.Value.Type}' does not match the underlying predicate type '{path.Predicate.GetType()}'")
                        };
                    })
                    .ToArray();
            });
        }

        public IIonType Prune(IIonType value)
        {
            var rootSelection = new RootValueSelection(value)
                .Enumerate()
                .ToArray();

            return this
                .Paths()
                .Aggregate(new List<IPathSelection[]> { rootSelection }, MultiFilter)
                .Reverse<IPathSelection[]>()
                .Aggregate((IIonType)null, Encapsulate);
        }

        private List<IPathSelection[]> MultiFilter(List<IPathSelection[]> nodes, PathSegment segment)
        {
            var parent = nodes[^1];
            var selections = nodes
                .SelectMany()
                .Select(Filter)
                .SelectMany()
                .ToArray();

            nodes.Add(selections);
            return nodes;
        }

        private IIonType Encapsulate(IIonType parent, IPathSelection[] children)
        {

        }

        private IEnumerable<IPathSelection> Filter(IPathSelection parent) => Filter(parent, this);

        private static IEnumerable<IPathSelection> Filter(IPathSelection parent, PathSegment segment)
        {
            return (parent.Value, segment.Predicate) switch
            {
                (IonStruct @struct, PropertyPredicate propertyPredicate) => @struct.Value
                    .Where(propertyPredicate.Execute)
                    .Select(property => new PropertyPathSelection(property.NameText, property.Value, parent.Value))
                    .Cast<IPathSelection>(),

                (IonList list, ListPredicate listPredicate) => list.Value
                    .Select((item, index) => (index, item))
                    .Where(listPredicate.Execute)
                    .Select(listItem => new ListPathSelection(listItem.index, listItem.item, parent.Value))
                    .Cast<IPathSelection>(),

                _ => throw new ArgumentException($"Supplied type '{parent.Value.Type}' does not match the underlying predicate type '{segment.Predicate.GetType()}'")
            };
        }

        public IEnumerable<PathSegment> Paths()
        {
            if (Next is null)
                return this.Enumerate();

            else return this
                    .Enumerate()
                    .Concat(Next.Paths());
        }

        private PropertyPathSelection Copy(IonStruct.Property property)
        {
            var value = Next is null
                ? CopyIon(property.Value)
                : CreateIon(property.Value.Type, Pick(property.Value));

            return new PropertyPathSelection(property.Name.Value, value);
        }

        private ListPathSelection Copy((int listIndex, IIonType value) tuple)
        {
            var value = Next is null
                ? CopyIon(tuple.value, tuple.listIndex)
                : CreateIon(tuple.value.Type, Pick(tuple.value), tuple.listIndex);

            return new ListPathSelection(tuple.listIndex, value);
        }

        private static IIonType CopyIon(IIonType value, int? listIndex = null)
        {
            var listMarkerAnnotation = listIndex is not null
                ?  $"{ListMarkerAnnotationPrefix}{listIndex}"
                : null;

            return value switch
            {
                IonNull => new IonNull(listMarkerAnnotation is null
                    ? Array.Empty<IIonType.Annotation>()
                    : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonBool ion => new IonBool(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonInt ion => new IonInt(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonFloat ion => new IonFloat(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonDecimal ion => new IonDecimal(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonTimestamp ion => new IonTimestamp(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonString ion => new IonString(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonIdentifier ion => new IonIdentifier(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonQuotedSymbol ion => new IonQuotedSymbol(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonOperator ion => new IonOperator(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonClob ion => new IonClob(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonBlob ion => new IonBlob(
                    ion.Value,
                    listMarkerAnnotation is null
                        ? Array.Empty<IIonType.Annotation>()
                        : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) }),

                IonSexp ion => new IonSexp(
                    new IonSexp.Initializer(
                        listMarkerAnnotation is null
                            ? Array.Empty<IIonType.Annotation>()
                            : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) },
                        ion.Value)),

                IonList ion => new IonList(
                    new IonList.Initializer(
                        listMarkerAnnotation is null
                            ? Array.Empty<IIonType.Annotation>()
                            : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) },
                        ion.Value)),

                IonStruct ion => new IonStruct(
                    new IonStruct.Initializer(
                        listMarkerAnnotation is null
                            ? Array.Empty<IIonType.Annotation>()
                            : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) },
                        ion.Value)),

                _ => throw new ArgumentException($"Invalid ion value: '{value}'")
            };
        }

        private static IIonType CreateIon(
            IonTypes ionType,
            IEnumerable<IPathSelection> items,
            int? listIndex = null)
        {
            var listMarkerAnnotation = listIndex is not null
                ? $"{ListMarkerAnnotationPrefix}{listIndex}"
                : null;

            return ionType switch
            {
                IonTypes.List => new IonList(
                    new IonList.Initializer(
                        listMarkerAnnotation is null
                            ? Array.Empty<IIonType.Annotation>()
                            : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) },
                        items.Select(item => item.Value).ToArray())),

                IonTypes.Sexp => new IonSexp(
                    new IonSexp.Initializer(
                        listMarkerAnnotation is null
                            ? Array.Empty<IIonType.Annotation>()
                            : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) },
                        items.Select(item => item.Value).ToArray())),

                IonTypes.Struct => new IonStruct(
                    new IonStruct.Initializer(
                        listMarkerAnnotation is null
                            ? Array.Empty<IIonType.Annotation>()
                            : new[] { IIonType.Annotation.Parse(listMarkerAnnotation) },
                        items
                            .Select(selection => (PropertyPathSelection)selection)
                            .Select(selection => new IonStruct.Property(selection.PropertyName, selection.Value))
                            .ToArray())),

                _ => throw new ArgumentException($"Invalid ion type: '{ionType}'")
            };
        }

        #region parser
        public static PathSegment Parse(CSTNode node)
        {
            _ = TryParse(node, out IResult<PathSegment> result);

            return result switch
            {
                IResult<PathSegment>.DataResult data => data.Data,
                IResult<PathSegment>.ErrorResult error => error.Cause().Throw<PathSegment>(),
                _ => throw new InvalidOperationException($"Invalid result: {result}")
            };
        }

        public static bool TryParse(CSTNode node, out PathSegment value)
        {
            _ = TryParse(node, out IResult<PathSegment> result);

            value = result switch
            {
                IResult<PathSegment>.DataResult data => data.Data,
                _ => default
            };

            return !value.IsDefault();
        }

        public static PathSegment Parse(string text, IFormatProvider provider = null)
        {
            var result = PulsarUtil.QueryGrammar
                .GetRecognizer(QuerySymbol)
                .Recognize(text);

            return result switch
            {
                SuccessResult success => Parse(success.Symbol),
                ErrorResult error => error.Exception.Throw<PathSegment>(),
                FailureResult failure => throw new RecognitionException(failure),
                _ => throw new InvalidOperationException($"Invalid recognizer result: {result}")
            };
        }

        public static bool TryParse(
            [NotNullWhen(true)] string text,
            IFormatProvider provider,
            [MaybeNullWhen(false)] out PathSegment result)
        {
            var parseResult = PulsarUtil.QueryGrammar
                .GetRecognizer(QuerySymbol)
                .Recognize(text);

            result = parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),
                _ => default
            };

            return !result.IsDefault();
        }

        public static bool TryParse(CSTNode node, out IResult<PathSegment> value)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var nodeTypes = $"{PredicateNodeSymbol}.{PropertyNameSymbol}|{ListIndexSymbol}|{ConditionalsSymbol}";

            value = node
                .FindNodes(nodeTypes)
                .Reverse()
                .Aggregate(default(PathSegment), (previousSegment, nextNode) =>
                {
                    return nextNode.SymbolName switch
                    {
                        PropertyNameSymbol => new PathSegment(
                            new PropertyPredicate($"Key.Matches(\"{nextNode.TokenValue()}\")"),
                            previousSegment),

                        ListIndexSymbol => new PathSegment(
                            new ListPredicate($"Key == {nextNode.TokenValue().UnwrapFrom("[", "]")}"),
                            previousSegment),

                        ConditionalsSymbol => ExtractConditional(nextNode, previousSegment),

                        _ => throw new ArgumentException($"Invalid symbol name: '{nextNode.SymbolName}'")
                    };
                })
                .ApplyTo(ps =>
                {
                    return ps is null
                        ? Result.Of<PathSegment>(new ArgumentException())
                        : Result.Of(ps);
                });

            return value is IResult<PathSegment>.DataResult;
        }

        private static PathSegment ExtractConditional(CSTNode conditionals, PathSegment previousSegment)
        {
            // annotation
            var annotationPredicate = conditionals
                .FindNode(AnnotationConditionalSymbol)
                .AsOptional()
                .Map(State.AnnotationPredicate.Parse);

            // primary predicate
            return conditionals
                .FindNode($"{ListConditionalSymbol}|{PropertyConditionalSymbol}")
                .AsOptional()
                .Map(node => node.SymbolName switch
                {
                    ListConditionalSymbol => (IPathPredicate) ListPredicate.Parse(node),
                    PropertyConditionalSymbol => PropertyPredicate.Parse(node),
                    _ => throw new ArgumentException($"Invalid symbol name encountered: '{node.SymbolName}'")
                })
                .Map(primaryPredicate => new PathSegment(primaryPredicate, annotationPredicate, previousSegment))
                .Value();
        }
        #endregion
    }
}
