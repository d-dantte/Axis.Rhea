using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

public record PropertySegment: DataPathSegment,  ISegmentSelector<string?>
{
    /// <summary>
    /// The property name for the <see cref="IonStruct"/> during selection
    /// <para>
    /// Note: null values are an indication to select ANY property - essentially, selecting the first available property
    /// </para>
    /// </summary>
    public string? Property { get; }

    public string? Selector => Property;

    public bool IsRequired { get; }

    public PropertySegment(string? property, bool isRequired = true, DataPathSegment? next = null)
    : base(next)
    {
        Property = property;
        IsRequired = isRequired;
    }

    public override IDataPathSelection Select(IIonType ion)
    {
        var result = ion switch
        {
            IonStruct @struct => 
                @struct.IsNull ? throw new ArgumentNullException(nameof(ion)):
                TryGetValue(@struct, out var prop) ? PropertySelection.Hit(@struct, prop!.Value.Property, prop!.Value.Ion!) :
                !IsRequired ? PropertySelection.Fallback(@struct, Property ?? "*"):
                PropertySelection.Miss(@struct, Property ?? "*"),

            null => throw new ArgumentNullException(nameof(ion)),

            _ => throw new ArgumentException($"Invalid {nameof(ion)} type: '{ion.Type}'. Expected '{IonTypes.Struct}'")
        };

        return Next is not null && result.Value is not null
            ? Next.Select(result.Value)
            : result;
    }

    private bool TryGetValue(IonStruct @struct, out (string Property, IIonType? Ion)? value)
    {
        value = null;
        if (@struct.Properties.Count == 0)
            return false;

        if (Property is null)
        {
            var prop = @struct.Value!.First();
            value = (prop.NameText!, prop.Value);
            return true;
        }

        value = @struct.Properties.TryGetvalue(Property, out var v) ? (Property!, v) : null;
        return value is not null;
    }

    public override string ToString()
        => $"/{Selector?.ToString() ?? "*"}{(!IsRequired ? "?" : "")}{Next?.ToString()}";
}
