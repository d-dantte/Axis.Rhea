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
    public readonly struct PropertyPredicate : IPathPredicate<IonStruct.Property>, ISerializableEntity<PropertyPredicate>, Pulsar.Grammar.Utils.IParsable<PropertyPredicate>
    {
        internal static readonly string PropertyConditionalSymbol = "property-conditional";
        internal static readonly string RelationalPredicateSymbol = "relational-predicate";

        private readonly Script<bool> predicateScript;

        public string PredicateExpression { get; }

        internal PropertyPredicate(string scriptExpression)
        {
            PredicateExpression = scriptExpression.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException("Invalid script expression"));

            var options = ScriptOptions.Default
                .WithReferences(typeof(PropertyPredicate).Assembly)
                .WithImports(typeof(PropertyPredicate).Namespace, typeof(object).Namespace);

            predicateScript = CSharpScript.Create<bool>(PredicateExpression, options, typeof(ScriptGlobal));
            var errors = predicateScript
                .Compile()
                .Where(diagnostics => diagnostics.Severity == DiagnosticSeverity.Error)
                .ToArray();

            if (!errors.IsEmpty())
                throw new CompilationErrorException("Errors detected while compiling the predicate", errors);
        }

        public bool Execute(IonStruct.Property property)
        {
            return predicateScript
                .RunAsync(new ScriptGlobal(property))
                .Return();
        }

        public string Serialize() => PredicateExpression;

        #region Parsers
        /// <summary>
        /// Parse the given <paramref name="propertyConditionalNode"/>
        /// </summary>
        /// <param name="propertyConditionalNode">The node containing the expression tokens</param>
        /// <returns>The parsed predicate</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static PropertyPredicate Parse(CSTNode propertyConditionalNode)
        {
            _ = TryParse(propertyConditionalNode, out IResult<PropertyPredicate> result);

            return result switch
            {
                IResult<PropertyPredicate>.DataResult data => data.Data,
                IResult<PropertyPredicate>.ErrorResult error => error.Cause().Throw<PropertyPredicate>(),
                _ => throw new InvalidOperationException($"Invalid result: {result}")
            };
        }

        /// <summary>
        /// Parse the given <paramref name="propertyConditionalNode"/>
        /// </summary>
        /// <param name="propertyConditionalNode">The node containing the expression tokens</param>
        /// <param name="value">the output predicate</param>
        /// <returns>True if parsed successfully, false otherwise.</returns>

        public static bool TryParse(CSTNode propertyConditionalNode, out PropertyPredicate value)
        {
            _ = TryParse(propertyConditionalNode, out IResult<PropertyPredicate> result);

            value = result switch
            {
                IResult<PropertyPredicate>.DataResult data => data.Data,
                _ => default
            };

            return !value.IsDefault();
        }

        /// <summary>
        /// Parses the given <paramref name="propertyConditionalText"/>.
        /// </summary>
        /// <param name="propertyConditionalText">Text version of the predicate. e.g <code>"/@{^abc.*$}"</code></param>
        /// <param name="provider">ignored</param>
        /// <returns>the output predicate</returns>
        /// <exception cref="RecognitionException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static PropertyPredicate Parse(string propertyConditionalText, IFormatProvider provider = null)
        {
            var result = PulsarUtil.QueryGrammar
                .GetRecognizer(PropertyConditionalSymbol)
                .Recognize(propertyConditionalText);

            return result switch
            {
                SuccessResult success => Parse(success.Symbol),
                ErrorResult error => error.Exception.Throw<PropertyPredicate>(),
                FailureResult failure => throw new RecognitionException(failure),
                _ => throw new InvalidOperationException($"Invalid recognizer result: {result}")
            };
        }

        /// <summary>
        /// Parses the given <paramref name="propertyConditionalText"/>.
        /// </summary>
        /// <param name="propertyConditionalText">Text version of the predicate. e.g <code>"/@{^abc.*$}"</code></param>
        /// <param name="provider">ignored.</param>
        /// <param name="result">The output predicate.</param>
        /// <returns></returns>
        public static bool TryParse(
            [NotNullWhen(true)] string propertyConditionalText,
            IFormatProvider provider,
            [MaybeNullWhen(false)] out PropertyPredicate result)
        {
            var parseResult = PulsarUtil.QueryGrammar
                .GetRecognizer(PropertyConditionalSymbol)
                .Recognize(propertyConditionalText);

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
        public static bool TryParse(CSTNode predicateNode, out IResult<PropertyPredicate> value)
        {
            if (predicateNode is null)
                throw new ArgumentNullException(nameof(predicateNode));

            var relationalPredicateNode = predicateNode.FindNode($"{RelationalPredicateSymbol}");

            if (relationalPredicateNode is null)
            {
                value = Result.Of<PropertyPredicate>(new FormatException($"Could not find symbol: {RelationalPredicateSymbol}"));
                return false;
            }

            var scriptExpression = ExpressionTransformer
                .TransformExpression(relationalPredicateNode)
                .TokenValue()
                .TrimStart('{')
                .TrimEnd('}');

            value = Result.Of(new PropertyPredicate(scriptExpression));
            return true;
        }
        #endregion

        #region Nested Types
        public record ScriptGlobal
        {
            public ValueWrapper Value { get; }

            public ValueWrapper Key { get; }

            public ScriptGlobal(IonStruct.Property property)
            {
                Value = new ValueWrapper(property.Value);
                Key = property.NameText;
            }
        }
        #endregion
    }
}
