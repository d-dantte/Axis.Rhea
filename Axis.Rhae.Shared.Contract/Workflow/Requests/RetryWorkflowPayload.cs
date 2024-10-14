namespace Axis.Rhae.Contract.Workflow.Requests
{
    public class RetryWorkflowPayload : IValidatable
    {
        public Identifier<Identifiers.Workflow> PreviousWorkflowId { get; }

        // TODO: other properties will come here

        public bool TryValidate(out AggregateException? validationException)
        {
            throw new NotImplementedException();
        }
    }
}
