namespace Axis.Rhae.Contract.Workflow.Identifiers
{
    using Semver;
    using System.Text.RegularExpressions;
    using IDMetadata = (
        Identifier<Namespace> Namespace,
        Identifier<Name> Name,
        Semver.SemVersion Version);

    public class WorkflowDefinition : IIdentifierPattern
    {
        internal static readonly string UnboundedSemverPattern =
            @"(?<major>\d+)"
            + @"(?>\.(?<minor>\d+))?"
            + @"(?>\.(?<patch>\d+))?"
            + @"(?>\-(?<pre>[0-9A-Za-z\-\.]+))?"
            + @"(?>\+(?<metadata>[0-9A-Za-z\-\.]+))$";

        public static readonly Regex Pattern = new(
            $"^{Namespace.UnboundedPattern}"
            + $":{Name.UnboundedPattern}"
            + $"@{UnboundedSemverPattern}$",
            RegexOptions.Compiled);

        public static bool IsValidPattern(string text)
        {
            return Pattern.IsMatch(text);
        }

        public static Identifier<WorkflowDefinition> ToIdentifier(IDMetadata tuple)
        {
            return $"{tuple.Namespace}:{tuple.Name}@{tuple.Version}";
        }

        public static Identifier<WorkflowDefinition> ToIdentifier(
            string @namespace,
            string name,
            SemVersion version)
            => ToIdentifier(@namespace, name, version);
    }

    public static class WorkflowDefinitionExtension
    {
        public static IDMetadata Split(this Identifier<WorkflowDefinition> fqn)
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
            var version = parts[1];

            return (
                @namespace,
                name,
                SemVersion.Parse(version, Semver.SemVersionStyles.Strict));
        }

        public static void Deconstruct(
            this Identifier<WorkflowDefinition> fqn,
            out Identifier<Namespace> @namespace,
            out Identifier<Name> name,
            out SemVersion version)
        {
            (@namespace, name, version) = fqn.Split();
        }

        public static Identifier<WorkflowDefinition> ToIdentifier(this IDMetadata tuple)
        {
            return WorkflowDefinition.ToIdentifier(tuple);
        }
    }
}
