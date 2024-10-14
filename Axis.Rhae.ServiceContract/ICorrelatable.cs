namespace Axis.Rhae.ServiceContract
{
    /// <summary>
    /// Contract for correlation
    /// </summary>
    public interface ICorrelatable
    {
        Guid CorrelationId { get; }
    }
}
