namespace Axis.Rhae.Contract.Audit
{
    public interface IEventPayload: IValidatable
    {
        EventType EventType { get; }
    }
}
