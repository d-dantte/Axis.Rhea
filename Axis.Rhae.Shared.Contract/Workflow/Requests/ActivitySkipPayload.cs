namespace Axis.Rhae.Contract.Workflow.Requests
{
    public record ActivitySkipPayload : IValidatable
    {
        public bool TryValidate(out AggregateException? validationException)
        {
            throw new NotImplementedException();
        }
    }
}
