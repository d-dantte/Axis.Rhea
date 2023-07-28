using Axis.Ion.Types;

namespace Axis.Rhea.Directives.Contract;

public record RequestPayload : ICorrelatable
{
    public string CorrelationId { get; }

    public IonStruct? Data { get; }


    public RequestPayload(string correlationId, IonStruct? data)
    {
        CorrelationId = correlationId;
        Data = data;
    }
}
