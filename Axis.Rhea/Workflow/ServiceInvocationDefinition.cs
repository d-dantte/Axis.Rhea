using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.ServiceInvocation;
using System;
using System.Collections.Immutable;

namespace Axis.Rhea.Core.Workflow
{
    public record ServiceInvocationDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public ImmutableDictionary<string, IServiceInvocation> ServiceInvocations { get; }

        public ServiceInvocationDefinition(params IServiceInvocation[] serviceInvocations)
        {
            ServiceInvocations = serviceInvocations
                .ThrowIfNull(new ArgumentNullException(nameof(serviceInvocations)))
                .ThrowIfAny(si => si is null, new ArgumentException($"Null invocation found in the service invocations list"))
                .ToImmutableDictionary(si => si.InvocationId, si => si);
        }
    }
}
