namespace Axis.Rhae.Workflow.Definition.Utils
{
    /// <summary>
    /// Condition that triggers a retry
    /// </summary>
    public interface IRetryPolicyTrigger<TResponse>
    {
        bool IsMatch(TResponse response);
    }
}
