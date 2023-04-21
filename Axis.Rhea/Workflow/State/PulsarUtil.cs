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
        internal readonly static string QuotedAnnotationString = "qas";

        internal static Grammar DataTreeGrammar { get; }

        internal static Grammar DataPathGrammar { get; }

        static PulsarUtil()
        {
            try
            {
                #region data tree
                using var dataTreeStream = typeof(PulsarUtil).Assembly
                    .GetManifestResourceStream($"{typeof(PulsarUtil).Namespace}.DataTreeDefinition.xbnf");

                var importer = new Importer();

                // register singleline-sqdstring
                _ = importer.RegisterTerminal(
                    new DelimitedString(
                        QuotedAnnotationString,
                        "\'",
                        new[] { "\n", "\r" },
                        new BSolGeneralEscapeMatcher()));

                DataTreeGrammar = importer.ImportGrammar(dataTreeStream);
                #endregion

                #region data path
                using var dataPathStream = typeof(PulsarUtil).Assembly
                    .GetManifestResourceStream($"{typeof(PulsarUtil).Namespace}.DataPathDefinition.xbnf");

                importer = new Importer();

                // register singleline-sqdstring
                _ = importer.RegisterTerminal(
                    new DelimitedString(
                        QuotedAnnotationString,
                        "\'",
                        new[] { "\n", "\r" },
                        new BSolGeneralEscapeMatcher()));

                DataPathGrammar = importer.ImportGrammar(dataPathStream);
                #endregion
            }
            catch (Exception ex)
            {
                ex.Throw();
            }
        }
    }
}
