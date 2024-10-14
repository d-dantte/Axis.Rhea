using Axis.Luna.Extensions;
using Axis.Rhae.Contract;
using System.Collections.Immutable;

namespace Axis.Rhae.Workflow.Definition.Activity
{
    using Identifiers = Contract.Workflow.Identifiers;

    public abstract class BaseActivityDefinition : IActivityDefinition
    {
        public Identifier<Identifiers.Activity> Identifier { get; }

        public ImmutableDictionary<Identifier<Identifiers.ResultLabel>, Identifier<Identifiers.Activity>> TransitionTable { get; }

        protected BaseActivityDefinition(
            Identifier<Identifiers.Activity> identifier,
            (Identifier<Identifiers.ResultLabel> Result, Identifier<Identifiers.Activity> Activity)[] transitionList)
        {
            Identifier = identifier.ThrowIfDefault(_ => new ArgumentException("Invalid identifier: default"));
            TransitionTable = transitionList
                .ThrowIfNull(() => new ArgumentNullException("Invalid transition list: null"))
                .ThrowIfAny(
                    tuple => tuple.Result.IsDefault || tuple.Activity.IsDefault,
                    _ => new ArgumentException("Invalid pair: default identifier detected"))
                .ToImmutableDictionary(
                    tuple => tuple.Result,
                    tuple => tuple.Activity);
        }
    }
}
