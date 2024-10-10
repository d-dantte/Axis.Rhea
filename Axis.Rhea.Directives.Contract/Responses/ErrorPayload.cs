using Axis.Dia.Types;
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

    public RecordValue? ErrorData { get; }


    public ErrorPayload(
        string correlationId,
        int statusCode,
        string title,
        string detail,
        RecordValue? errorData = null)
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
