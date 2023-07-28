namespace Axis.Rhea.Directives.Contract;

/// <summary>
/// Defines resources that correlate based on an ID
/// </summary>
public interface ICorrelatable
{
    /// <summary>
    /// The correlation Id
    /// </summary>
    string CorrelationId { get; }
}
