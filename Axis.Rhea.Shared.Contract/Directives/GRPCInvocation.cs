using Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

namespace Axis.Rhea.Shared.Contract.Directives;

/// <summary>
/// Defines configuraiton needed to make a gRPC Service invocation
/// </summary>
public record GRPCInvocation : IDirectiveInvocation
{
    public string InvocationId => throw new NotImplementedException();

    public int Timeout => throw new NotImplementedException();

    public RootNode StateSelector => throw new NotImplementedException();

    public RetryPolicy RetryPolicy => throw new NotImplementedException();

    public int Delay => throw new NotImplementedException();

    #region Nested types

    /// <summary>
    /// Builds the <see cref="GRPCInvocation"/> instance
    /// </summary>
    public class Builder
    {

    }
    #endregion
}
