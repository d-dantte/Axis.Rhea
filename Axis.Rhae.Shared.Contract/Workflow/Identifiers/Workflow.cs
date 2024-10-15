namespace Axis.Rhae.Contract.Workflow.Identifiers
{
    using Semver;
    using System.Text.RegularExpressions;
    using IDMetadata = (
        Identifier<Namespace> Namespace,
        Identifier<Name> Name,
        Semver.SemVersion Version,
        Guid InstanceId,
        string? Alias);

    public class Workflow : IIdentifierPattern
    {
        internal static readonly string UnboundedAliasPattern = "[a-zA-Z_][a-zA-Z0-9_-]*";
        internal static readonly string UnboundedGuidPattern = "[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}";

        public static readonly Regex Pattern = new(
            $"^{Namespace.UnboundedPattern}"
            + $":{Name.UnboundedPattern}"
            + $"@{WorkflowDefinition.UnboundedSemverPattern}"
            + $"(/{UnboundedAliasPattern})?"
            + $"/{UnboundedGuidPattern}$",
            RegexOptions.Compiled);

        public static bool IsValidPattern(string text)
        {
            return Pattern.IsMatch(text);
        }

        public static Identifier<Workflow> ToIdentifier(IDMetadata tuple)
        {
            return $"{tuple.Namespace}:{tuple.Name}@{tuple.Version}/{tuple.Alias switch
            {
                null => tuple.InstanceId.ToString(),
                string alias => $"{alias}/{tuple.InstanceId}"
            }}";
        }

        public static Identifier<Workflow> ToIdentifier(
            string @namespace,
            string name,
            SemVersion version,
            Guid instanceId,
            string? alias = null)
            => ToIdentifier(@namespace, name, version, instanceId, alias);
    }

    public static class WorkflowExtension
    {
        public static IDMetadata Split(this Identifier<Workflow> fqn)
        {
            if (fqn.IsDefault)
                throw new ArgumentException("Invalid fqn: default");

            // namespace
            var parts = fqn.ToString().Split(':');
            var @namespace = parts[0];

            // name
            parts = parts[1].Split('@');
            var name = parts[0];

            // version
            parts = parts[1].Split('/');
            var version = parts[0];

            // guid, alias
            var (guid, alias) = parts.Length switch
            {
                2 => (parts[1], default(string)),
                3 => (parts[2], parts[1]),
                _ => throw new FormatException($"Invalid fqn format: '{fqn}'")
            };

            return (
                @namespace,
                name,
                Semver.SemVersion.Parse(version, Semver.SemVersionStyles.Strict),
                Guid.Parse(guid), alias);
        }

        public static void Deconstruct(
            this Identifier<Workflow> fqn,
            out Identifier<Namespace> @namespace,
            out Identifier<Name> name,
            out SemVersion version,
            out Guid instanceId,
            out string? alias)
        {
            (@namespace, name, version, instanceId, alias) = fqn.Split();
        }

        public static Identifier<Workflow> ToIdentifier(this IDMetadata tuple)
        {
            return Workflow.ToIdentifier(tuple);
        }
    }
}
