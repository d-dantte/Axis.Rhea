using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

public record ItemSelection : IDataPathSelection, ISelectionCreator<ItemSelection, IRefValue<IIonType[]>, int>
{
    public IRefValue<IIonType[]> Container { get; }

    IIonType IDataPathSelection.Container => Container;

    public IIonType? Value { get; }

    public int Index { get; }

    public MatchType MatchType { get; }

    private ItemSelection(IRefValue<IIonType[]> parent, int index, MatchType matchType, IIonType? value)
    {
        Index = index;
        Value = value;
        MatchType = matchType;
        Container = parent switch
        {
            IonSexp or IonList => parent,
            _ => throw new ArgumentException($"Invalid {nameof(parent)} type: '{parent?.GetType()}'")
        };
    }

    public static ItemSelection Hit(
        IRefValue<IIonType[]> container,
        int selector,
        IIonType value)
        => new ItemSelection(container, selector, MatchType.Hit, value);

    public static ItemSelection Fallback(
        IRefValue<IIonType[]> container,
        int selector)
        => new ItemSelection(container, selector, MatchType.FallBack, null);

    public static ItemSelection Miss(
        IRefValue<IIonType[]> container,
        int selector)
        => new ItemSelection(container, selector, MatchType.Miss, null);
}
