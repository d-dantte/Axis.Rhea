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

namespace Axis.Rhea.Core.Workflow.State
{
    public readonly struct ListPredicate : IPathPredicate<(int index, IIonType value)>, ISerializableEntity<ListPredicate>, Pulsar.Grammar.Utils.IParsable<ListPredicate>
    {
        internal static readonly string ListConditionalSymbol = "list-conditional";
        internal static readonly string RelationalPredicateSymbol = "relational-predicate";

        private readonly Script<bool> predicateScript;

        public string PredicateExpression { get; }

        internal ListPredicate(string scriptExpression)
        {
            PredicateExpression = scriptExpression.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException("Invalid script expression"));

            var options = ScriptOptions.Default
                .WithReferences(typeof(ListPredicate).Assembly)
                .WithImports(typeof(ListPredicate).Namespace, typeof(object).Namespace);

            predicateScript = CSharpScript.Create<bool>(PredicateExpression, options, typeof(ScriptGlobal));
            var errors = predicateScript
                .Compile()
                .Where(diagnostics => diagnostics.Severity == DiagnosticSeverity.Error)
                .ToArray();

            if (!errors.IsEmpty())
                throw new CompilationErrorException("Errors detected while compiling the predicate", errors);
        }

        public bool Execute((int index, IIonType value) item)
        {
            return predicateScript
                .RunAsync(new ScriptGlobal(item))
                .Return();
        }

        public string Serialize() => PredicateExpression;

        #region Parsers
        /// <summary>
        /// Parse the given <paramref name="listConditionalNode"/>
        /// </summary>
        /// <param name="listConditionalNode">The node containing the expression tokens</param>
        /// <returns>The parsed predicate</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ListPredicate Parse(CSTNode listConditionalNode)
        {
            _ = TryParse(listConditionalNode, out IResult<ListPredicate> result);

            return result switch
            {
                IResult<ListPredicate>.DataResult data => data.Data,
                IResult<ListPredicate>.ErrorResult error => error.Cause().Throw<ListPredicate>(),
                _ => throw new InvalidOperationException($"Invalid result: {result}")
            };
        }

        /// <summary>
        /// Parse the given <paramref name="listConditionalNode"/>
        /// </summary>
        /// <param name="listConditionalNode">The node containing the expression tokens</param>
        /// <param name="value">the output predicate</param>
        /// <returns>True if parsed successfully, false otherwise.</returns>

        public static bool TryParse(CSTNode listConditionalNode, out ListPredicate value)
        {
            _ = TryParse(listConditionalNode, out IResult<ListPredicate> result);

            value = result switch
            {
                IResult<ListPredicate>.DataResult data => data.Data,
                _ => default
            };

            return !value.IsDefault();
        }

        /// <summary>
        /// Parses the given <paramref name="listConditionalText"/>.
        /// </summary>
        /// <param name="listConditionalText">Text version of the predicate. e.g <code>"#{^abc.*$}"</code></param>
        /// <param name="provider">ignored</param>
        /// <returns>the output predicate</returns>
        /// <exception cref="RecognitionException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static ListPredicate Parse(string listConditionalText, IFormatProvider provider = null)
        {
            var result = PulsarUtil.QueryGrammar
                .GetRecognizer(ListConditionalSymbol)
                .Recognize(listConditionalText);

            return result switch
            {
                SuccessResult success => Parse(success.Symbol),
                ErrorResult error => error.Exception.Throw<ListPredicate>(),
                FailureResult failure => throw new RecognitionException(failure),
                _ => throw new InvalidOperationException($"Invalid recognizer result: {result}")
            };
        }

        /// <summary>
        /// Parses the given <paramref name="listConditionalText"/>.
        /// </summary>
        /// <param name="listConditionalText">Text version of the predicate. e.g <code>"/@{^abc.*$}"</code></param>
        /// <param name="provider">ignored.</param>
        /// <param name="result">The output predicate.</param>
        /// <returns></returns>
        public static bool TryParse(
            [NotNullWhen(true)] string listConditionalText,
            IFormatProvider provider,
            [MaybeNullWhen(false)] out ListPredicate result)
        {
            var parseResult = PulsarUtil.QueryGrammar
                .GetRecognizer(ListConditionalSymbol)
                .Recognize(listConditionalText);

            result = parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),
                _ => default
            };

            return !result.IsDefault();
        }

        /// <summary>
        /// Parses the tokens from the <paramref name="predicateNode"/>.
        /// </summary>
        /// <param name="predicateNode">The <see cref="CSTNode"/> containing the expression</param>
        /// <param name="value">The output from parsing the <paramref name="predicateNode"/></param>
        /// <returns>True if successfully parsed, false otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryParse(CSTNode predicateNode, out IResult<ListPredicate> value)
        {
            if (predicateNode is null)
                throw new ArgumentNullException(nameof(predicateNode));

            var relationalPredicateNode = predicateNode.FindNode($"{RelationalPredicateSymbol}");

            if (relationalPredicateNode is null)
            {
                value = Result.Of<ListPredicate>(new FormatException($"Could not find symbol: {RelationalPredicateSymbol}"));
                return false;
            }

            var scriptExpression = ExpressionTransformer.TransformExpression(relationalPredicateNode);
            value = Result.Of(new ListPredicate(scriptExpression.TokenValue()));
            return true;
        }
        #endregion

        #region Nested Types
        public record ScriptGlobal
        {
            public ValueWrapper Value { get; }

            public int Key { get; }

            public ScriptGlobal((int index, IIonType value) item)
            {
                Value = new ValueWrapper(item.value);
                Key = item.index;
            }
        }
        #endregion

    }
}
