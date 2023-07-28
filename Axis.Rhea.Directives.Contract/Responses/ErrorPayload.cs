using Axis.Ion.Types;
using Axis.Luna.Extensions;

namespace Axis.Rhea.Directives.Contract.Responses;

/// <summary>
/// Represents errors that originate from the service that was invoked
/// </summary>
public record ErrorPayload : IResponsePayload, IStatusCoded
{
    public string CorrelationId { get; }

    public int StatusCode { get; }

    public string Title { get; }

    public string Detail { get; }

    public IonStruct? ErrorData { get; }


    public ErrorPayload(
        string correlationId,
        int statusCode,
        string title,
        string detail,
        IonStruct? errorData = null)
    {
        CorrelationId = correlationId.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid '{nameof(correlationId)}': '{correlationId}'"));

        Title = title.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid '{nameof(title)}': '{title}'"));

        StatusCode = statusCode;

        Detail = detail; // allows nulls or empties, etc

        ErrorData = errorData;
    }
}
