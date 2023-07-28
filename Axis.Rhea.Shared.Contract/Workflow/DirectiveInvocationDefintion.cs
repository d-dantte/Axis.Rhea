using Axis.Luna.Extensions;
using Axis.Rhea.Shared.Contract.Directives;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.Workflow;

public record DirectiveInvocationDefintion
{
    /// <summary>
    /// Map of invocation names to the invocation instances
    /// </summary>
    public ImmutableDictionary<string, IDirectiveInvocation> DirectiveInvocations { get; }

    public DirectiveInvocationDefintion(params IDirectiveInvocation[] directiveInvocations)
    {
        DirectiveInvocations = directiveInvocations
            .ThrowIfNull(new ArgumentNullException(nameof(directiveInvocations)))
            .ThrowIfAny(di => di is null, new ArgumentException($"{nameof(directiveInvocations)} must not contain null values"))
            .ToImmutableDictionary(di => di.InvocationId, si => si);
    }
}
