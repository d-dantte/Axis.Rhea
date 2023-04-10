namespace Axis.Rhea.Core.Workflow.ServiceInvocation
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceInvocation
    {
        /// <summary>
        /// A unique identifier for this invocation instance. Must be unique across the whole workflow
        /// </summary>
        string InvocationId { get; }

        /// <summary>
        /// Timeout value in milliseconds
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// When applied on the workflow state, returns an <see cref="IonStruct"/> that only contains
        /// data that fits the criteria represented by each <see cref="State.PathSegment"/> contained within.
        /// </summary>
        StateSelector DataSource { get; }

        /// <summary>
        /// The retry policy for this invocation
        /// </summary>
        RetryPolicy RetryPolicy { get; }
    }
}
