using Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

namespace Axis.Rhea.Shared.Contract.Directives;

/// <summary>
/// 
/// </summary>
public interface IDirectiveInvocation
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
    /// Time in milliseconds to delay before sending off the request. Only applied before the very first invocation. Retries will depend
    /// solely on the <see cref="ServiceInvocation.RetryPolicy"/>
    /// </summary>
    int Delay { get; }

    /// <summary>
    /// <see cref="DataTreeNode"/> used to prune the <see cref="Workflow.State.WorkflowState"/>
    /// </summary>
    RootNode StateSelector { get; }

    /// <summary>
    /// The retry policy for this invocation
    /// </summary>
    RetryPolicy RetryPolicy { get; }
}
