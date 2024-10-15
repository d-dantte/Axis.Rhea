using Axis.Dia.Core.Types;
using Axis.Rhae.Contract.Service.Payloads;

namespace Axis.Rhae.Contract.Service
{
    /// <summary>
    /// Service unit that represents calling an external dia-service from the workflow
    /// </summary>
    public interface IServiceDirectiveUnit
    {
        Task<Response<DirectiveFeedback>> Invoke(Request<Record> request);
    }
}
