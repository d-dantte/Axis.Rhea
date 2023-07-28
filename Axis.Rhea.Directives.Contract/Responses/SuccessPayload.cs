using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Rhea.Directives.Contract.Responses;

/// <summary>
/// Represents results returned from a successfully completed invocation of a service
/// </summary>
public record SuccessPayload : IResponsePayload, IStatusCoded
{
    public int StatusCode { get; }

    public string CorrelationId { get; }

    /// <summary>
    /// A string identifier. These are used by activities to determine which state to transition to.
    /// </summary>
    public string ResponseCode { get; }

    /// <summary>
    /// Optional list of mutations to apply to the state based on data returned from the service
    /// </summary>
    public ImmutableList<StateMutation> StateMutators { get; }

    public SuccessPayload(
        string correlationId,
        int statusCode,
        string responseCode,
        params StateMutation[] mutators)
    {
        CorrelationId = correlationId.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid '{nameof(correlationId)}': '{correlationId}'"));

        ResponseCode = responseCode.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(responseCode)}: '{responseCode}'"));

        StateMutators = mutators
            .ThrowIfNull()
            .ThrowIf(
                t => t.Any(r => r is null),
                new ArgumentException($"null item found in {nameof(mutators)} list"))
            .ToImmutableList();

        StatusCode = statusCode;
    }
}
