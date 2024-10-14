using Axis.Rhae.Contract;
using System.Text.RegularExpressions;

namespace Axis.Rhae.Contract.Workflow.Identifiers
{
    public class ResultLabel : IIdentifierPattern
    {
        public static readonly Regex Pattern = new(
            "^[a-zA-Z_][a-zA-Z0-9_-]*$",
            RegexOptions.Compiled);

        public static bool IsValidPattern(string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            return Pattern.IsMatch(text);
        }
    }
}
