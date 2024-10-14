namespace Axis.Rhae.Contract.Service
{
    public interface IServiceUnit
    {
        Task<Response> Invoke(Request request);
    }
}
