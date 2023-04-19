using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Language.Rules.CustomTerminals;
using Axis.Pulsar.Languages.xBNF;
using System;
using System.Text.RegularExpressions;
using static Axis.Pulsar.Grammar.Language.Rules.CustomTerminals.DelimitedString;

namespace Axis.Rhea.Core.Workflow.State
{
    internal static class PulsarUtil
    {
        /// <summary>
        /// Custom Symbol: @regex-string
        /// </summary>
        internal readonly static string RegexString = "regex-string";

        /// <summary>
        /// Custom Symbol: @singleline-string
        /// </summary>
        internal readonly static string SingleLineString = "singleline-string";

        internal static Grammar QueryGrammar { get; }

        static PulsarUtil()
        {
            try
            {
                using var queryXbnfStream = typeof(PulsarUtil).Assembly
                    .GetManifestResourceStream($"{typeof(PulsarUtil).Namespace}.StateQuery.xbnf");

                var importer = new Importer();

                // register regex string
                _ = importer.RegisterTerminal(
                    new DelimitedString(
                        RegexString,
                        "{", "}",
                        new BSolGeneralAndBraceEscapeMatcher()));

                // register singleline-dqdstring
                _ = importer.RegisterTerminal(
                    new DelimitedString(
                        SingleLineString,
                        "\"",
                        new[] { "\n", "\r" },
                        new BSolGeneralEscapeMatcher()));

                QueryGrammar = importer.ImportGrammar(queryXbnfStream);
            }
            catch (Exception ex)
            {
                ex.Throw();
            }
        }


        public class BSolGeneralAndBraceEscapeMatcher : IEscapeSequenceMatcher
        {
            private readonly Regex HexPattern = new(@"^u[a-fA-F0-9]{0,4}$", RegexOptions.Compiled);

            public string EscapeDelimiter => "\\";

            public bool IsSubMatch(ReadOnlySpan<char> subTokens)
            {
                if (subTokens[0] == 'u')
                    return subTokens.Length <= 5
                        && HexPattern.IsMatch(new string(subTokens));

                return subTokens.Length == 1 && subTokens[0] switch
                {
                    '\'' => true,
                    '\"' => true,
                    '\\' => true,
                    'n' => true,
                    'r' => true,
                    'f' => true,
                    'b' => true,
                    't' => true,
                    'v' => true,
                    '0' => true,
                    'a' => true,
                    '{' => true,
                    '}' => true,
                    _ => false
                };
            }

            public bool IsMatch(ReadOnlySpan<char> escapeTokens)
            {
                if (escapeTokens.Length == 5)
                    return HexPattern.IsMatch(new string(escapeTokens));

                if (escapeTokens.Length == 1 && escapeTokens[0] != 'u')
                    return IsSubMatch(escapeTokens);

                return false;
            }
        }
    }
}
