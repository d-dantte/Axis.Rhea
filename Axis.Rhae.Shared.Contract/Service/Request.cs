using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Rhae.Contract.Service
{
    public class Request : ICorrelatable
    {
        public Guid CorrelationId { get; }

        public Record Payload { get; }

        public Request(Guid correlationId, Record payload)
        {
            CorrelationId = correlationId;
            Payload = payload.ThrowIfDefault(
                _ => new ArgumentException("Invalid payload: default"));
        }

        public Request(Record payload) : this(Guid.NewGuid(), payload)
        { }
    }
}
