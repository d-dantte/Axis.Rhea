namespace Axis.Rhae.Workflow.Definition.ServiceDirective
{
    using Axis.Rhae.Contract.Service;
    using Axis.Rhae.Workflow.Definition.Utils;
    using DiaQueryPath = Dia.PathQuery.Path;

    public interface IInvocationDefinition
    {
        /// <summary>
        /// Timeout value in milliseconds
        /// </summary>
        uint MillisecondTimeout { get; }

        /// <summary>
        /// Minimum time in milliseconds to delay before sending off the request. Only applied before the very first invocation. Retries will depend
        /// solely on the <see cref="ServiceInvocation.RetryPolicy"/>
        /// </summary>
        uint MillisecondDelay { get; }

        /// <summary>
        /// Optional selector for filtering portions of the state data that is sent along with this invocation.
        /// </summary>
        DiaQueryPath? StateSelector { get; }

        /// <summary>
        /// The retry policy for this invocation
        /// </summary>
        RetryPolicy<Response> RetryPolicy { get; }
    }
}
