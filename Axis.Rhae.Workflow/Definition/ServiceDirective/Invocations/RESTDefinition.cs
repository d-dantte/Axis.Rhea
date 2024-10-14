
using Axis.Luna.Extensions;
using Axis.Rhae.Contract.Service;
using Axis.Rhae.Workflow.Definition.ServiceDirective.PolicyTriggers;
using Axis.Rhae.Workflow.Definition.Utils;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Axis.Rhae.Workflow.Definition.ServiceDirective.Invocations
{
    public class RESTDefinition : IInvocationDefinition
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
        /// The url for the web service
        /// </summary>
        public string EndpointUrl { get; }

        /// <summary>
        /// Http headers
        /// </summary>
        public ImmutableDictionary<string, string> Headers { get; }

        private RESTDefinition(
            uint millisecondTimeout,
            uint millisecondDelay,
            Dia.PathQuery.Path? stateSelector,
            RetryPolicy<Response> retryPolicy,
            string endpointUrl,
            ImmutableDictionary<string, string> headers)
        {
            MillisecondTimeout = millisecondTimeout;
            MillisecondDelay = millisecondDelay;
            StateSelector = stateSelector;
            RetryPolicy = retryPolicy;
            EndpointUrl = endpointUrl;
            Headers = headers;
        }

        public static Builder NewBuilder() => new Builder();

        #region Builder
        public class Builder
        {
            private uint millisecondTimeout = DefaultMillisecondTimeout;

            private uint millisecondDelay = DefaultMillisecondDelay;

            private Dia.PathQuery.Path? stateSelector;

            private RetryPolicy<Response> retryPolicy = DefaultRetryPolicy;

            private string? endpointUrl = null;

            private readonly Dictionary<string, string> headers = [];

            public Builder WithEndpointUrl(string url)
            {
                endpointUrl = url.ThrowIf(
                    string.IsNullOrWhiteSpace,
                    _ => new ArgumentException($"Invalid url: '{url}'"));

                return this;
            }

            public Builder WithHeader(string name, string value)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(name);
                ArgumentException.ThrowIfNullOrWhiteSpace(value);

                headers[name] = value;
                return this;
            }

            public bool HasHeader(string name)
            {
                ArgumentException.ThrowIfNullOrEmpty(name);
                return headers.ContainsKey(name);
            }

            public Builder WithHeaders(params (string Name, string Value)[] headers)
            {
                return headers
                    .ThrowIfNull(() => new ArgumentNullException(nameof(headers)))
                    .Aggregate(this, (builder, header) => builder.WithHeader(header.Name, header.Value));
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

            public RESTDefinition Build()
            {
                Validate();
                return new(
                    millisecondTimeout,
                    millisecondDelay,
                    stateSelector,
                    retryPolicy,
                    endpointUrl!,
                    headers.ToImmutableDictionary());
            }

            private void Validate()
            {
                if (string.IsNullOrWhiteSpace(endpointUrl))
                    throw new ValidationException($"Invalid endpointUrl: null/empty/whitespace");
            }
        }
        #endregion
    }
}
