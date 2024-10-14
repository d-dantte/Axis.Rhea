using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Service;
using Axis.Rhae.Workflow.Definition.ServiceDirective.PolicyTriggers;
using Axis.Rhae.Workflow.Definition.Utils;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Workflow.Definition.ServiceDirective.Invocations
{
    internal class GrpcDefinition : IInvocationDefinition
    {
        public static readonly uint DefaultMillisecondTimeout = 30000;
        public static readonly uint DefaultMillisecondDelay = 0;
        public static readonly RetryPolicy<Response> DefaultRetryPolicy = new(100, 10, 20, FaultResponseTrigger.Instance);

        #region Common
        public uint MillisecondTimeout { get; }

        public uint MillisecondDelay { get; }

        public Dia.PathQuery.Path? StateSelector { get; }

        public RetryPolicy<Response> RetryPolicy { get; }
        #endregion

        /// <summary>
        /// The communication protocol to use with the gRPC channel
        /// </summary>
        public Protocol ChannelProtocol { get; }

        /// <summary>
        /// The domain on which the service is hosted
        /// </summary>
        public string ChannelDomain { get; }

        /// <summary>
        /// The port on which the service is listening
        /// </summary>
        public ushort ChannelPort { get; }

        /// <summary>
        /// The maximum data size in bytes that can be sent to the service
        /// </summary>
        public uint MaxRequestSize { get; }

        /// <summary>
        /// The maximum data size in bytes that can be received from the service
        /// </summary>
        public uint MaxResponseSize { get; }

        private GrpcDefinition(
            uint millisecondTimeout,
            uint millisecondDelay,
            Dia.PathQuery.Path? stateSelector,
            RetryPolicy<Response> retryPolicy,
            Protocol protocol,
            string domain,
            ushort port,
            uint maxRequestSize,
            uint maxResponseSize)
        {
            MillisecondTimeout = millisecondTimeout;
            MillisecondDelay = millisecondDelay;
            StateSelector = stateSelector;
            RetryPolicy = retryPolicy;
            ChannelProtocol = protocol;
            ChannelDomain = domain;
            ChannelPort = port;
            MaxRequestSize = maxRequestSize;
            MaxResponseSize = maxResponseSize;
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

            private RetryPolicy<Response> retryPolicy = DefaultRetryPolicy;

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

            public Builder WithRetryPolicy(RetryPolicy<Response> policy)
            {
                retryPolicy = policy ?? throw new ArgumentNullException(nameof(policy));
                return this;
            }

            public GrpcDefinition Build()
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
