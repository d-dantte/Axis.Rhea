using Axis.Rhae.Contract.Workflow.Identifiers;
using Axis.Rhae.Contract.Workflow;
using Axis.Rhae.Contract;
using Axis.Rhae.Workflow.Definition.Utils;
using Axis.Luna.Extensions;
using System.ComponentModel.DataAnnotations;
using Axis.Rhae.Workflow.Definition.WorkflowDirective.PolicyTriggers;

namespace Axis.Rhae.Workflow.Definition.WorkflowDirective
{
    using WorkflowFQNId = Identifier<Contract.Workflow.Identifiers.Workflow>;

    public class InvocationDefinition
    {
        public static readonly uint DefaultMillisecondTimeout = 30000;
        public static readonly uint DefaultMillisecondDelay = 0;
        public static readonly RetryPolicy<Response<WorkflowFQNId>> DefaultRetryPolicy = new(100, 10, 20, ErrorResponseTrigger.Instance);

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
        /// The retry policy for this invocation
        /// </summary>
        RetryPolicy<Response<WorkflowFQNId>> RetryPolicy { get; }

        /// <summary>
        /// The identifier of the next activity to transition to after the workflow has been created
        /// </summary>
        Identifier<Identifiers.Activity> NextActivity { get; }


        private InvocationDefinition(
            uint millisecondTimeout,
            uint millisecondDelay,
            Dia.PathQuery.Path? stateSelector,
            RetryPolicy<Response<WorkflowFQNId>> retryPolicy,
            Protocol protocol,
            string domain,
            ushort port,
            uint maxRequestSize,
            uint maxResponseSize)
        {
            MillisecondTimeout = millisecondTimeout;
            MillisecondDelay = millisecondDelay;
            RetryPolicy = retryPolicy;
        }

        public static Builder NewBuilder() => new Builder();

        #region Nested types
        public enum Protocol
        {
            Http,
            Https
        }

        public class Builder
        {
            private uint millisecondTimeout = DefaultMillisecondTimeout;

            private uint millisecondDelay = DefaultMillisecondDelay;

            private Dia.PathQuery.Path? stateSelector;

            private RetryPolicy<Response<WorkflowFQNId>> retryPolicy = DefaultRetryPolicy;

            private Protocol channelProtocol;

            private string? channelDomain;

            private ushort channelPort;

            private uint maxRequestSize;

            private uint maxResponseSize;

            public Builder WithDomain(string domain)
            {
                channelDomain = domain.ThrowIf(
                    string.IsNullOrWhiteSpace,
                    _ => new ArgumentException($"Invalid domain: '{domain}'"));

                return this;
            }

            public Builder WithProtocol(Protocol protocol)
            {
                channelProtocol = protocol.ThrowIf(
                    Enum.IsDefined,
                    p => new ArgumentException($"Invalid protocol: '{p}'"));

                return this;
            }

            public Builder WithPort(ushort port)
            {
                channelPort = port;
                return this;
            }

            public Builder WithMaxRequestSize(uint size)
            {
                maxRequestSize = size;
                return this;
            }

            public Builder WithMaxResponseSize(uint size)
            {
                maxResponseSize = size;
                return this;
            }

            public Builder WithTimeout(uint timeoutMilliseconds)
            {
                millisecondTimeout = timeoutMilliseconds;
                return this;
            }

            public Builder WithDelay(uint delayMilliseconds)
            {
                millisecondDelay = delayMilliseconds;
                return this;
            }

            public Builder WithStateSelector(Dia.PathQuery.Path? stateSelector)
            {
                this.stateSelector = stateSelector;
                return this;
            }

            public Builder WithRetryPolicy(RetryPolicy<Response<WorkflowFQNId>> policy)
            {
                retryPolicy = policy ?? throw new ArgumentNullException(nameof(policy));
                return this;
            }

            public InvocationDefinition Build()
            {
                Validate();
                return new(
                    millisecondTimeout,
                    millisecondDelay,
                    stateSelector,
                    retryPolicy,
                    channelProtocol,
                    channelDomain!,
                    channelPort,
                    maxRequestSize,
                    maxResponseSize);
            }

            private void Validate()
            {
                if (string.IsNullOrWhiteSpace(channelDomain))
                    throw new ValidationException($"Invalid endpointUrl: null/empty/whitespace");
            }
        }
        #endregion
    }
}
