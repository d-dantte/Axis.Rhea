
using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Service.Responses;

namespace Axis.Rhae.Contract.Service
{
    public class Response : ICorrelatable
    {
        public Guid CorrelationId { get; }

        public IResponsePayload Payload { get; }

        public Response(Guid correlationId, IResponsePayload payload)
        {
            CorrelationId = correlationId;
            Payload = payload.ThrowIfNull(
                () => new ArgumentNullException(nameof(payload)));
        }
    }
}
