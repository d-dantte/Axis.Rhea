using Axis.Ion.Types;
using Axis.Luna.Extensions;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

public record PropertySelection : IDataPathSelection, ISelectionCreator<PropertySelection, IonStruct, string>
{
    public IonStruct Container { get; }

    IIonType IDataPathSelection.Container => Container;

    public IIonType? Value { get; }

    public string Property { get; }

    public MatchType MatchType { get; }

    private PropertySelection(IonStruct parent, string property, MatchType matchType, IIonType? value)
    {
        Container = parent;
        Value = value;
        MatchType = matchType;
        Property = property.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(property)}: '{property}'"));
    }

    public static PropertySelection Hit(
        IonStruct container,
        string selector,
        IIonType value)
        => new PropertySelection(container, selector, MatchType.Hit, value);

    public static PropertySelection Fallback(
        IonStruct container,
        string selector)
        => new PropertySelection(container, selector, MatchType.FallBack, null);

    public static PropertySelection Miss(
        IonStruct container,
        string selector)
        => new PropertySelection(container, selector, MatchType.Miss, null);
}
