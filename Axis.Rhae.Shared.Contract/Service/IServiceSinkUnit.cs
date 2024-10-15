using Axis.Rhae.Contract.Service.Payloads;

namespace Axis.Rhae.Contract.Service
{
    /// <summary>
    /// Service sink. Represents an external service making a call to the workflow. <para/>
    /// This is experimental!!!
    /// </summary>
    public interface IServiceSinkUnit
    {
        Task Invoke(Request<ServiceSinkInstruction> request);
    }
}
