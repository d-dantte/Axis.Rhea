using System.Collections.Immutable;

namespace Axis.Rhea.Core.Workflow.ServiceInvocation
{
    public class InvocationResponse
    {
        /// <summary>
        /// A string identifier. These are used by the <see cref="Activities.ServiceInvocation"/> activities to determine which state to transition to.
        /// </summary>
        public string ResponseIdentifier { get; set; }

        /// <summary>
        /// Optional list of mutations to apply to the state based on data returned from the service
        /// </summary>
        public ImmutableList<StateMutator> StateMutators { get; set; }
    }
}
