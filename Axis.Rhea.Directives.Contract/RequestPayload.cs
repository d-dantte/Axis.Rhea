using Axis.Dia.Types;

namespace Axis.Rhea.Directives.Contract;

public record RequestPayload : ICorrelatable
{
    public string CorrelationId { get; }

    public RecordValue? Data { get; }


    public RequestPayload(string correlationId, RecordValue? data)
    {
        CorrelationId = correlationId;
        Data = data;
    }
}
