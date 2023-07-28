using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Language.Rules.CustomTerminals;
using Axis.Pulsar.Languages.xBNF;
using Axis.Rhea.Shared.Contract.Directives.PolicyTriggers;
using Axis.Rhea.Shared.Contract.StateExpressions;
using Axis.Rhea.Shared.Contract.Workflow.State.DataPath;
using static Axis.Pulsar.Grammar.Language.Rules.CustomTerminals.DelimitedString;

namespace Axis.Rhea.Shared.Contract.Utils;

internal static class PulsarUtil
{
    /// <summary>
    /// Custom Symbol: @regex-string
    /// </summary>
    internal readonly static string RegexString = "regex-string";

    /// <summary>
    /// Custom Symbol: @qas
    /// </summary>
    internal readonly static string QuotedAnnotationString = "qas";

    /// <summary>
    /// Custom Symbol: @qet
    /// </summary>
    internal readonly static string QuotedErrorTitleString = "qet";

    /// <summary>
    /// Custom Symbol: @dqd-string
    /// </summary>
    internal readonly static string DoubleQuotedString = "dqd-string";

    internal static Grammar DataPathGrammar { get; }

    internal static Grammar PolicyTriggerConditionGrammar { get; }

    internal static Grammar StateExpressionGrammar { get; }

    static PulsarUtil()
    {
        #region data path
        using var dataPathStream = typeof(PulsarUtil).Assembly
            .GetManifestResourceStream($"{typeof(DataPathSegment).Namespace}.DataPathDefinition.xbnf");

        var importer = new Importer();

        // register singleline-sqd string
        _ = importer.RegisterTerminal(
            new DelimitedString(
                QuotedAnnotationString,
                "\'",
                new[] { "\n", "\r" },
                new BSolGeneralEscapeMatcher()));

        DataPathGrammar = importer.ImportGrammar(dataPathStream);
        #endregion

        #region policy trigger condition
        using var policyTriggerConditionStream = typeof(PulsarUtil).Assembly
            .GetManifestResourceStream($"{typeof(PolicyTriggerParser).Namespace}.PolicyTriggerCondition.xbnf");

        importer = new Importer();

        // register singleline-sqdstring
        _ = importer.RegisterTerminal(
            new DelimitedString(
                QuotedErrorTitleString,
                "\'",
                new[] { "\n", "\r" },
                new BSolGeneralEscapeMatcher()));

        PolicyTriggerConditionGrammar = importer.ImportGrammar(policyTriggerConditionStream);
        #endregion

        #region State expresssion
        using var stateExpressionStream = typeof(PulsarUtil).Assembly
            .GetManifestResourceStream($"{typeof(IExpression).Namespace}.StateExpressions.xbnf");

        importer = new Importer();

        // register singleline-dqd string
        _ = importer.RegisterTerminal(
            new DelimitedString(
                DoubleQuotedString,
                "\"",
                new[] { "\n", "\r" },
                new BSolGeneralEscapeMatcher()));

        // register singleline-sqd string
        _ = importer.RegisterTerminal(
            new DelimitedString(
                QuotedAnnotationString,
                "\'",
                new[] { "\n", "\r" },
                new BSolGeneralEscapeMatcher()));

        StateExpressionGrammar = importer.ImportGrammar(stateExpressionStream);
        #endregion
    }
}
