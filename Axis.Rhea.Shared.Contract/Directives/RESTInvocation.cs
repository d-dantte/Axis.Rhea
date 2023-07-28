using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Axis.Luna.Extensions;
using Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

namespace Axis.Rhea.Shared.Contract.Directives;

/// <summary>
/// Defines configuraiton needed to make a RESTful Service invocation
/// </summary>
public record RESTInvocation : IDirectiveInvocation
{
    /// <summary>
    /// The url for the web service
    /// </summary>
    public string EndpointUrl { get; private set; }

    /// <summary>
    /// Verb to use for this invocation
    /// </summary>
    public HttpVerb RequstVerb { get; private set; }

    /// <summary>
    /// Http headers
    /// </summary>
    public ImmutableDictionary<string, string> Headers { get; private set; }

    #region IServiceInvocation
    public string InvocationId { get; private set; }

    public int Timeout { get; private set; }

    public RootNode StateSelector { get; private set; }

    public RetryPolicy RetryPolicy { get; private set; }

    public int Delay { get; private set; }
    #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private RESTInvocation()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    #region nested types
    public enum HttpVerb
    {
        GET,
        POST,
        PUT
    }
    #endregion

    #region Builder
    public class Builder
    {
        private readonly RESTInvocation invocation = new();

        public Builder() => ValidateInvocation();

        public Builder WithEndpointUrl(string url)
        {
            invocation.EndpointUrl = url.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid url: '{url}'"));
            return this;
        }

        public Builder WithRequestVerb(HttpVerb verb)
        {
            invocation.RequstVerb = verb;
            return this;
        }

        public Builder WithHeader(string name, string value)
        {
            invocation.Headers = (invocation.Headers?
                .AsEnumerable()
                ?? Enumerable.Empty<KeyValuePair<string, string>>())
                .Append(name.ValuePair(value))
                .ToImmutableDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value);

            return this;
        }

        public Builder WithHeaders(params (string Name, string Value)[] headers)
        {
            invocation.Headers = (invocation.Headers?
                .AsEnumerable()
                ?? Enumerable.Empty<KeyValuePair<string, string>>())
                .Concat(headers
                    .ThrowIfNull(new ArgumentNullException(nameof(headers)))
                    .ThrowIfAny(
                        tuple => string.IsNullOrWhiteSpace(tuple.Name) || string.IsNullOrWhiteSpace(tuple.Value),
                        tuple => new ArgumentException($"Invalid value found: {tuple}"))
                    .Select(tuple => tuple.Name.ValuePair(tuple.Value)))
                .ToImmutableDictionary(
                    tuple => tuple.Key,
                    tuple => tuple.Value);

            return this;
        }

        public Builder WithInvocationId(string id)
        {
            invocation.InvocationId = id.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid id: '{id}'"));
            return this;
        }

        public Builder WithTimeout(int timeoutMilliseconds)
        {
            invocation.Timeout = timeoutMilliseconds.ThrowIf(
                i => i < 0,
                new ArgumentException($"Invalid timeout: '{timeoutMilliseconds}'"));
            return this;
        }

        public Builder WithStateSelector(RootNode stateSelector)
        {
            invocation.StateSelector = stateSelector ?? throw new ArgumentNullException(nameof(stateSelector));
            return this;
        }

        public Builder WithRetryPolicy(RetryPolicy policy)
        {
            invocation.RetryPolicy = policy ?? throw new ArgumentNullException(nameof(policy));
            return this;
        }

        public Builder WithDelay(int delayMilliseconds)
        {
            invocation.Timeout = delayMilliseconds.ThrowIf(
                d => d < 0,
                new ArgumentException($"Invalid timeout: '{delayMilliseconds}'"));
            return this;
        }

        public RESTInvocation Build() => invocation with { }; // clones the record


        private RESTInvocation ValidateInvocation()
        {
            if (string.IsNullOrWhiteSpace(invocation.EndpointUrl))
                throw new ValidationException($"Invalid {nameof(invocation.EndpointUrl)}");

            if (!Enum.IsDefined(invocation.RequstVerb))
                throw new ValidationException($"Invalid {nameof(invocation.RequstVerb)}");

            if (string.IsNullOrWhiteSpace(invocation.InvocationId))
                throw new ValidationException($"Invalid {nameof(invocation.InvocationId)}");

            if (invocation.Timeout < 0)
                throw new ValidationException($"Invalid {nameof(invocation.Timeout)}");

            if (invocation.StateSelector is null)
                throw new ValidationException($"Invalid {nameof(invocation.StateSelector)}");

            if (invocation.RetryPolicy is null)
                throw new ValidationException($"Invalid {nameof(invocation.RetryPolicy)}");

            if (invocation.Headers is null)
                throw new ValidationException($"Invalid {nameof(invocation.Headers)}");

            if (invocation.Headers.Any(kvp => 
                string.IsNullOrWhiteSpace(kvp.Key) 
                || string.IsNullOrWhiteSpace(kvp.Value)))
                throw new ValidationException($"Invalid {nameof(invocation.Headers)}");

            return invocation;
        }
    }
    #endregion
}
