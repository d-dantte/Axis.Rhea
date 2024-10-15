using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Rhae.Contract;
using Axis.Rhae.Contract.Workflow.Identifiers;
using Axis.Rhae.Workflow.Definition.Activity;
using Semver;
using System.Collections.Immutable;

namespace Axis.Rhae.Workflow.Definition
{
    using Identifiers = Contract.Workflow.Identifiers;

    public class WorkflowDefinition
    {
        public Identifier<Namespace> Namespace { get; }

        public SemVersion Version { get; }

        public Identifier<Name> Name { get; }

        public Identifier<Identifiers.WorkflowDefinition> Identifier => (Namespace, Name, Version).ToIdentifier();

        public ImmutableDictionary<Identifier<Identifiers.Activity>, IActivityDefinition> Activities { get; }

        public Identifier<Identifiers.Activity> StartActivity { get; }

        public Record DefaultData { get; }

        public WorkflowDefinition(
            Identifier<Namespace> @namespace,
            Identifier<Name> name,
            SemVersion version,
            Identifier<Identifiers.Activity> startActivity,
            Record defaultData,
            params (Identifier<Identifiers.Activity> ActivityId, IActivityDefinition Definition)[] activities)
        {
            Namespace = @namespace;
            Name = name;
            Version = version;
            StartActivity = startActivity;

            DefaultData = defaultData.ThrowIfDefault(
                _ => new ArgumentException($"Invalid data: default"));

            Activities = activities
                .ThrowIfNull(() => new ArgumentNullException(nameof(activities)))
                .ThrowIfAny(
                    activity => activity.ActivityId.IsDefault
                        || activity.Definition is null
                        || !activity.Definition.Identifier.Equals(activity.ActivityId),
                    activity => new ArgumentException("Invalid pair: contains null/default"))
                .ToImmutableDictionary(
                    activity => activity.ActivityId,
                    activity => activity.Definition);
        }
    }
}
