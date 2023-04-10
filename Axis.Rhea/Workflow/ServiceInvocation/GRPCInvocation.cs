using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Rhea.Core.Workflow.ServiceInvocation
{
    /// <summary>
    /// Defines configuraiton needed to make a gRPC Service invocation
    /// </summary>
    public record GRPCInvocation : IServiceInvocation
    {
        public string InvocationId => throw new NotImplementedException();

        public int Timeout => throw new NotImplementedException();

        public StateSelector DataSource => throw new NotImplementedException();

        public RetryPolicy RetryPolicy => throw new NotImplementedException();
    }
}
