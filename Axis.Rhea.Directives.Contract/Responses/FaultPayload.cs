using Axis.Luna.Extensions;

namespace Axis.Rhea.Directives.Contract.Responses;

/// <summary>
/// Represents errors generated from the system trying to call the external service. Such errors indicate a fault while trying to send off 
/// the request to the external service.
/// <para>
/// TODO: Create a list of well-known exceptions that we are interested in, then have a map of string labels to those exceptions.
/// These exceptions cover for REST and gRPC.
/// </para>
/// </summary>
public record FaultPayload : IResponsePayload
{
    public string CorrelationId { get; }

    /// <summary>
    /// The exception that occured while trying to send the request
    /// </summary>
    public FaultTypes Fault { get; }

    public FaultPayload(string correlationId, Exception fault)
    {
        Fault = ToFaultType(fault);
        CorrelationId = correlationId.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid '{nameof(correlationId)}': '{correlationId}'"));
    }

    private static FaultTypes ToFaultType(Exception exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        throw new NotImplementedException("Not yet implemented");
    }

    #region Nested Types

    /// <summary>
    /// Well-Known Directive Invocation faults
    /// </summary>
    public enum FaultTypes
    {
        Timeout
    }
    #endregion
}
