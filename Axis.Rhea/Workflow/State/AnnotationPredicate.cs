using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Core.Serializers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Axis.Rhea.Core.Workflow.State
{
    public readonly struct AnnotationPredicate : IPathPredicate<IIonType.Annotation>, ISerializableEntity<AnnotationPredicate>, Pulsar.Grammar.Utils.IParsable<AnnotationPredicate>
    {
        internal const string AnnotationConditionalSymbol = "annotation-conditional";
        internal const string RegexPredicateSymbol = "regex-predicate";

        internal static readonly string MatchExpression = "Regex.IsMatch(Annotation)";

        private readonly Script<bool> regexScript;
        private readonly Regex compiledRegex;

        public string PredicateExpression { get; }

        internal AnnotationPredicate(string regexExpression)
        {
            PredicateExpression = regexExpression.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException("Invalid regex expression"));
            compiledRegex = new Regex(PredicateExpression, RegexOptions.Compiled);

            var options = ScriptOptions.Default
                .WithReferences(
                    typeof(AnnotationPredicate).Assembly,
                    typeof(Regex).Assembly)
                .WithImports(
                    typeof(AnnotationPredicate).Namespace,
                    typeof(Regex).Namespace,
                    typeof(object).Namespace);

            regexScript = CSharpScript.Create<bool>(MatchExpression, options, typeof(ScriptGlobal));
            var errors = regexScript
                .Compile()
                .Where(diagnostics => diagnostics.Severity == DiagnosticSeverity.Error)
                .ToArray();

            if (!errors.IsEmpty())
                throw new CompilationErrorException("Errors detected while compiling the predicate", errors);
        }

        public bool Execute(IIonType.Annotation annotation)
        {
            return regexScript
                .RunAsync(new ScriptGlobal(compiledRegex, annotation.Value))
                .Return();
        }
        
        public string Serialize() => PredicateExpression;

        #region Parsers
        /// <summary>
        /// Parse the given <paramref name="predicateNode"/>
        /// </summary>
        /// <param name="predicateNode">The node containing the expression tokens</param>
        /// <returns>The parsed predicate</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static AnnotationPredicate Parse(CSTNode predicateNode)
        {
            _ = TryParse(predicateNode, out IResult<AnnotationPredicate> result);

            return result switch
            {
                IResult<AnnotationPredicate>.DataResult data => data.Data,
                IResult<AnnotationPredicate>.ErrorResult error => error.Cause().Throw<AnnotationPredicate>(),
                _ => throw new InvalidOperationException($"Invalid result: {result}")
            };
        }

        /// <summary>
        /// Parse the given <paramref name="predicateNode"/>
        /// </summary>
        /// <param name="predicateNode">The node containing the expression tokens</param>
        /// <param name="value">the output predicate</param>
        /// <returns>True if parsed successfully, false otherwise.</returns>
        public static bool TryParse(CSTNode predicateNode, out AnnotationPredicate value)
        {
            _ = TryParse(predicateNode, out IResult<AnnotationPredicate> result);

            value = result switch
            {
                IResult<AnnotationPredicate>.DataResult data => data.Data,
                _ => default
            };

            return !value.IsDefault();
        }

        /// <summary>
        /// Parses the given <paramref name="predicateText"/>.
        /// </summary>
        /// <param name="predicateText">Text version of the predicate. e.g <code>"/@{^abc.*$}"</code></param>
        /// <param name="provider">ignored</param>
        /// <returns>the output predicate</returns>
        /// <exception cref="RecognitionException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static AnnotationPredicate Parse(string predicateText, IFormatProvider provider = null)
        {
            var result = PulsarUtil.QueryGrammar
                .GetRecognizer(AnnotationConditionalSymbol)
                .Recognize(predicateText);

            return result switch
            {
                SuccessResult success => Parse(success.Symbol),
                ErrorResult error => error.Exception.Throw<AnnotationPredicate>(),
                FailureResult failure => throw new RecognitionException(failure),
                _ => throw new InvalidOperationException($"Invalid recognizer result: {result}")
            };
        }

        /// <summary>
        /// Parses the given <paramref name="predicateText"/>.
        /// </summary>
        /// <param name="predicateText">Text version of the predicate. e.g <code>"/@{^abc.*$}"</code></param>
        /// <param name="provider">ignored.</param>
        /// <param name="result">The output predicate.</param>
        /// <returns></returns>
        public static bool TryParse(
            [NotNullWhen(true)] string predicateText,
            IFormatProvider provider,
            [MaybeNullWhen(false)] out AnnotationPredicate result)
        {
            var parseResult = PulsarUtil.QueryGrammar
                .GetRecognizer(AnnotationConditionalSymbol)
                .Recognize(predicateText);

            result = parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),
                _ => default
            };

            return !result.IsDefault();
        }

        /// <summary>
        /// Parses the tokens from the <paramref name="annotationNode"/>.
        /// </summary>
        /// <param name="annotationNode">The <see cref="CSTNode"/> containing the expression</param>
        /// <param name="value">The output from parsing the <paramref name="annotationNode"/></param>
        /// <returns>True if successfully parsed, false otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryParse(CSTNode annotationNode, out IResult<AnnotationPredicate> value)
        {
            if (annotationNode is null)
                throw new ArgumentNullException(nameof(annotationNode));

            var regexNode = annotationNode.FindNode($"{RegexPredicateSymbol}");

            if (regexNode is null)
            {
                value = Result.Of<AnnotationPredicate>(new FormatException("'*.condition' symbols were not found"));
                return false;
            }

            value = Result.Of(new AnnotationPredicate(NormalizeRegex(regexNode.TokenValue())));
            return value is IResult<AnnotationPredicate>.DataResult;
        }
        #endregion

        private static string NormalizeRegex(string regex)
        {
            return regex[1..^1].Replace("\\}", "}");
        }

        #region Nested Types
        public record ScriptGlobal
        {
            public Regex Regex { get; }

            public string Annotation { get; }

            public ScriptGlobal(Regex regex, string annotation)
            {
                Regex = regex.ThrowIfNull(new ArgumentNullException(nameof(regex)));
                Annotation = annotation;
            }
        }
        #endregion
    }
}
