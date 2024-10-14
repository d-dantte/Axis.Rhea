using System.Text.RegularExpressions;

namespace Axis.Rhae.Contract.Workflow.Identifiers
{
    public class Name : IIdentifierPattern
    {
        internal static readonly string UnboundedPattern = "[a-zA-Z_][a-zA-Z0-9_-]*";

        public static readonly Regex Pattern = new(
            $"^{UnboundedPattern}$",
            RegexOptions.Compiled);

        public static bool IsValidPattern(string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            return Pattern.IsMatch(text);
        }
    }
}
