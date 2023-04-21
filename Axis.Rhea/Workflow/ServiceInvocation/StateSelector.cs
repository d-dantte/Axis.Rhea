using Axis.Ion.Types;
using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.State;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.ServiceInvocation
{
    public record StateSelector
    {
        //public ImmutableList<PathSegment> RootSegments { get; }

        //public StateSelector(params PathSegment[] segments)
        //{
        //    RootSegments = segments
        //        .ThrowIfNull(new ArgumentNullException(nameof(segments)))
        //        .ThrowIfAny(
        //            segment => segment is null,
        //            new ArgumentException("Null segment found in the segments list"))
        //        .ToImmutableList();
        //}

        //public IonStruct Pick(IonStruct state)
        //{
        //    return RootSegments
        //        .SelectMany(segment => segment.Pick(state))
        //        .ApplyTo(Merge);
        //}

        //private IonStruct Merge(IEnumerable<IPathSelection> selections)
        //{
        //    return selections
        //        .Select(selection => (PropertyPathSelection)selection)
        //        .Aggregate(IonStruct.Empty(), MergeWith);
        //}

        //private IonStruct MergeWith(IonStruct ionStruct, PropertyPathSelection selection)
        //{
        //    if (!ionStruct.Properties.Contains(selection.PropertyName))
        //    {
        //        var props = ionStruct.Properties;
        //        props[selection.PropertyName] = selection.Value;
        //        return ionStruct;
        //    }

        //    // type mismatch
        //    var ionPropValue = ionStruct.Properties[selection.PropertyName];
        //    if (ionPropValue.Type != selection.Value.Type)
        //    {
        //        var name = selection.PropertyName;
        //        var ionType = ionPropValue.Type;
        //        var selectionType = selection.Value.Type;
        //        throw new ArgumentException($"IonType mismatch. Name: {name}, IonValue: {ionType}, SelectionValue: {selectionType}");
        //    }

        //    if (selection.Value is IonStruct selectionStruct)
        //    {
        //        _ = selectionStruct.Value
        //            .Select(prop => new PropertyPathSelection(prop.NameText, prop.Value))
        //            .Aggregate((IonStruct)ionPropValue, MergeWith);
        //    }
        //    else if(selection.Value is IonList selectionList)
        //    {
        //        // lists are immutable, so we create a new one
        //        var propList = (IonList)ionPropValue;
        //        var ionProps = ionStruct.Properties;
        //        ionProps[selection.PropertyName] = selectionList.Value
        //            .Concat(propList.Value)
        //            .GroupBy(GetMarker)
        //            .Select(Diff)
        //            .ToArray()
        //            .ApplyTo(arr => new IonList.Initializer(propList.Annotations, arr))
        //            .ApplyTo(initializer => new IonList(initializer));
        //    }

        //    return ionStruct;
        //}

        //private IIonType Diff(IGrouping<string, IIonType> group)
        //{
        //    if (group.Key is null)
        //        throw new ArgumentException($"Items must have a marker annotation: {group.First()}");

        //    var items = group.ToArray();
        //    if (items.Length == 1)
        //        return items[0];

        //    var marker = group.Key;

        //    // type mismatch
        //    var types = items.Select(item => item.Type).Distinct().ToArray();
        //    if (types.Length != 1)
        //    {
        //        var typeText = types.Select(t => t.ToString()).JoinUsing(", ");
        //        var index = marker.Split('@')[0];
        //        throw new ArgumentException($"IonType mismatch. Index: {index}, types: [{typeText}]");
        //    }

        //    // values are all structs
        //    if (types[0] == IonTypes.Struct)
        //    {
        //        var mainValue = (IonStruct)group.First();
        //        return group
        //            .Skip(1)
        //            .SelectMany(value => ((IonStruct)value).Value)
        //            .Select(prop => new PropertyPathSelection(prop.NameText, prop.Value))
        //            .Aggregate(mainValue, MergeWith);
        //    }
            
        //    // values are all lists
        //    if (types[0] == IonTypes.List)
        //    {
        //        // lists are immutable, so we create a new one
        //        var annotations = group.First().Annotations;
        //        return group
        //            .Select(list => (IonList)list)
        //            .SelectMany(list => list.Value)
        //            .GroupBy(GetMarker)
        //            .Select(Diff)
        //            .ToArray()
        //            .ApplyTo(arr => new IonList.Initializer(annotations, arr))
        //            .ApplyTo(initializer => new IonList(initializer));
        //    }

        //    // values are all sexps??

        //    // for every other type of value, just pick the first one because a match indicates they are identical
        //    return group.First();
        //}

        //private string GetMarker(IIonType value)
        //{
        //    return value
        //        .ThrowIfNull(new ArgumentNullException(nameof(value)))
        //        .Annotations
        //        .Where(annotation => annotation.Value.StartsWith(PathSegment.ListMarkerAnnotationPrefix))
        //        .Select(annotation => annotation.Value)
        //        .FirstOrDefault();
        //}
    }
}
