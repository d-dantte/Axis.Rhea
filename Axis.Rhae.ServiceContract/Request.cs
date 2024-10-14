
using Axis.Dia.Core.Types;

namespace Axis.Rhae.ServiceContract
{
    public class Request : ICorrelatable
    {
        public Guid CorrelationId{ get; }

        /// <summary>
        /// State-Data sent as payload.
        /// <para/>
        /// Note: if the <c>IServiceDirectiveInvocationDefinition</c> defined a <c>StateSelector</c>, then this record will contain a single property,
        /// "<c>Items</c>", a list of 0 or more items selected from the original state-data, in the order they appear in the data. If on the other
        /// hand no selector is defined, this value will be the entire state data.
        /// </summary>
        public Record StateData { get; }
    }
}
