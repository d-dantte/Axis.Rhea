namespace Axis.Rhea.Directives.Contract;

public interface IStatusCoded
{
    /// <summary>
    /// Http/gRPC/etc status code
    /// </summary>
    int StatusCode { get; }
}
