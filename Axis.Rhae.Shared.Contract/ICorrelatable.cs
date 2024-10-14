namespace Axis.Rhae.Contract
{
    /// <summary>
    /// Contract for correlation
    /// </summary>
    public interface ICorrelatable
    {
        Guid CorrelationId { get; }
    }
}
