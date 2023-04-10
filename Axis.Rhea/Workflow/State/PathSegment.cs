using Axis.Ion.Types;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.State
{
    public record PathSegment
    {
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

        public IEnumerable<IPathSelection> Select(IIonType value)
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

        private PropertyPathSelection Copy(IonStruct.Property property)
        {
            var value = Next is null
                ? CopyIon(property.Value)
                : CreateIon(property.Value.Type, Select(property.Value));

            return new PropertyPathSelection(property.Name.Value, value);
        }

        private ListPathSelection Copy((int listIndex, IIonType value) tuple)
        {
            var value = Next is null
                ? CopyIon(tuple.value, tuple.listIndex)
                : CreateIon(tuple.value.Type, Select(tuple.value), tuple.listIndex);

            return new ListPathSelection(tuple.listIndex, value);
        }

        private IIonType CopyIon(IIonType value, int? listIndex = null)
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

        private IIonType CreateIon(
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

    }
}
