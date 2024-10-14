using Axis.Rhae.Contract;
using System.Collections.Immutable;

namespace Axis.Rhae.Workflow.Definition.Activity
{
    using Identifiers = Contract.Workflow.Identifiers;

    public interface IActivityDefinition
    {
        Identifier<Identifiers.Activity> Identifier { get; }

        ImmutableDictionary<Identifier<Identifiers.ResultLabel>, Identifier<Identifiers.Activity>> TransitionTable { get; }
    }
}
