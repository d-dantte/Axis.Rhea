using Axis.Luna.Extensions;
using Axis.Rhae.Contract;
using Axis.Rhae.Workflow.Definition.ServiceDirective;

namespace Axis.Rhae.Workflow.Definition.Activity
{
    using Identifiers = Contract.Workflow.Identifiers;

    /// <summary>
    /// Represents calling a "connected" service - i.e, calling a http endpoint, an in-process service, a gRpc service, etc.
    /// <para/>
    /// 
    /// </summary>
    public class ServiceDirectiveDefinition : BaseActivityDefinition
    {
        public IInvocationDefinition InvocationDefinition { get; }

        public ServiceDirectiveDefinition(
            Identifier<Identifiers.Activity> identifier,
            IInvocationDefinition invocationDefinition,
            params (Identifier<Identifiers.ResultLabel> Result, Identifier<Identifiers.Activity> Activity)[] transitionList)
            : base(identifier, transitionList)
        {
            InvocationDefinition = invocationDefinition.ThrowIfNull(
                () => new ArgumentNullException(nameof(invocationDefinition)));
        }
    }
}
