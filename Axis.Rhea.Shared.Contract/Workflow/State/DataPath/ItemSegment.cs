using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

public record ItemSegment: DataPathSegment, ISegmentSelector<int?>
{
    /// <summary>
    /// The index into the <see cref="Ion.Types.IonList"/>, or <see cref="Ion.Types.IonSexp"/>
    /// <para>
    /// Note: null values are an indication to select ANY item - essentially, selecting the first available item
    /// </para>
    /// </summary>
    public int? Index { get; }

    public int? Selector => Index;

    public bool IsRequired { get; }

    public ItemSegment(int? index, bool isRequired = true, DataPathSegment? next = null)
    : base(next)
    {
        Index = index;
        IsRequired = isRequired;
    }

    public override IDataPathSelection Select(IIonType ion)
    {
        var result = ion switch
        {
            IonList list =>
                list.IsNull ? throw new ArgumentNullException(nameof(ion)):
                IsInRange(list.Value) ? ItemSelection.Hit(list, Index ?? 0, list.Value![Index ?? 0]):
                !IsRequired ? ItemSelection.Fallback(list, Index ?? 0):
                ItemSelection.Miss(list, Index ?? 0),

            IonSexp sexp =>
                sexp.IsNull ? throw new ArgumentNullException(nameof(ion)) :
                IsInRange(sexp.Value) ? ItemSelection.Hit(sexp, Index ?? 0, sexp.Value![Index ?? 0]) :
                !IsRequired ? ItemSelection.Fallback(sexp, Index ?? 0) :
                ItemSelection.Miss(sexp, Index ?? 0),

            null => throw new ArgumentNullException(nameof(ion)),

            _ => throw new ArgumentException($"Invalid {nameof(ion)} type: '{ion.Type}'. Expected '{IonTypes.List}' or '{IonTypes.Sexp}'")
        };

        return Next is not null && result.Value is not null
            ? Next.Select(result.Value)
            : result;
    }

    private bool IsInRange(IIonType[]? array)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        return array.Length > 0
            && (Index is null || (Index >= 0 && array.Length > Index));
    }

    public override string ToString()
        =>  $"/[{Selector?.ToString() ?? "*"}]{(!IsRequired ? "?" : "")}{Next?.ToString()}";
}
