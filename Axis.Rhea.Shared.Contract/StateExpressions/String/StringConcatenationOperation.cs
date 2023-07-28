using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;
using System.Collections.Immutable;
using Axis.Rhea.Shared.Contract.StateExpressions.Value;

namespace Axis.Rhea.Shared.Contract.StateExpressions.String
{
    /// <summary>
    /// Represents concatenation of string expressions.
    /// <para>
    /// Note: the first expression must be a <c>ITypedExpression&lt;IonString&gt;</c>.
    /// Subsequent types are converted to string by the concatenation operation
    /// </para>
    /// </summary>
    public record StringConcatenationOperation :
        IParametarizedOperation<IonString>,
        IResultParsable<StringConcatenationOperation>
    {
        #region Grammar Symbols
        internal const string StringConcatenationOperationSymbol = "string-concatenation-expression";
        internal const string SValueSymbol = "svalue";
        internal const string SSValueSymbol = "ssvalue";

        internal const string ConstantStringExpressionSymbol = "constant-string-expression";
        internal const string StringStateSelectionSymbol = "string-state-selection-expression";
        internal const string BoolExpressionSymbol = "bool-expression";
        internal const string TimeStampExpressionSymbol = "timestamp-expression";
        internal const string DurationExpressionSymbol = "duration-expression";
        internal const string NumericExpressionSymbol = "numeric-expression";

        #endregion

        public string Name => Operation.ToString();

        public ImmutableArray<IExpression> Arguments { get; }


        public Operators Operation => Operators.Concatenate;

        IReadOnlyList<IExpression> IParametarizedOperation<IonString>.Arguments => Arguments;

        public StringConcatenationOperation(params IExpression[] strings)
        {
            Arguments = strings
                .ThrowIfNull(new ArgumentNullException(nameof(strings)))
                .ThrowIf(arr => arr.Length == 0, new ArgumentOutOfRangeException(nameof(strings)))
                .ThrowIfNot(
                    arr => arr[0] is ITypedExpression<IonString>,
                    new ArgumentException($"The initial expression must be a string expression"))
                .ToImmutableArray();
        }

        #region Evaluate
        public ITypedAtom<IonString> Evaluate()
        {
            return Operation switch
            {
                Operators.Concatenate => Concatenate(),
                _ => throw new InvalidOperationException($"Invalid operator: {Operation}")
            };
        }

        IAtom IExpression.Evaluate() => Evaluate();

        #endregion

        #region Oprations
        internal Atom<IonString> Concatenate()
        {
            return Arguments
                .Select(arg => arg.Evaluate().Ion.ToAtomText())
                .JoinUsing("")
                .ApplyTo(concatenated => new IonString(concatenated));
        }
        #endregion

        #region Parse

        public static bool TryParse(
            string text,
            out IResult<StringConcatenationOperation> result)
            => (result = Parse(text)) is IResult<StringConcatenationOperation>.DataResult;

        public static bool TryParse(
            CSTNode temporalOperationNode,
            out IResult<StringConcatenationOperation> result)
            => (result = Parse(temporalOperationNode)) is IResult<StringConcatenationOperation>.DataResult;

        public static IResult<StringConcatenationOperation> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(StringConcatenationOperationSymbol)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),

                FailureResult
                or ErrorResult => Result.Of<StringConcatenationOperation>(new RecognitionException(parseResult)),

                _ => Result.Of<StringConcatenationOperation>(
                    new InvalidOperationException($"Invalid recognition result: '{parseResult}'"))
            };
        }

        public static IResult<StringConcatenationOperation> Parse(CSTNode stringConcatenationNode)
        {
            try
            {
                return stringConcatenationNode
                    .FindNodes($"{SValueSymbol}|{SSValueSymbol}")
                    .Select(ParseElement)
                    .Fold()
                    .Map(svalues => new StringConcatenationOperation(svalues.ToArray()));
            }
            catch (Exception ex)
            {
                return Result.Of<StringConcatenationOperation>(ex);
            }
        }

        private static IResult<IExpression> ParseElement(CSTNode element)
        {
            var expression = element.FirstNode();
            return expression.SymbolName switch
            {
                NumericExpressionSymbol => Expression.ParseNumericExpression(expression),
                StringStateSelectionSymbol => StateSelectionValue.Parse(expression),

                BoolExpressionSymbol => Expression
                    .ParseBoolExpression(expression)
                    .Map(exp => (IExpression)exp),

                TimeStampExpressionSymbol => Expression
                    .ParseTimestampExpression(expression)
                    .Map(exp => (IExpression)exp),

                DurationExpressionSymbol => Expression
                    .ParseDurationExpression(expression)
                    .Map(exp => (IExpression)exp),

                ConstantStringExpressionSymbol => StringConstantExpression
                    .Parse(expression)
                    .Map(exp => (IExpression)exp),

                _  => Result.Of<IExpression>(
                    new ArgumentException(
                        $"Invalid symbol: '{expression.SymbolName}'. Expected "
                        + $"{BoolExpressionSymbol}"
                        + $", {TimeStampExpressionSymbol}"
                        + ",..."))
            };
        }
        #endregion


        public enum Operators
        {
            Concatenate,
        }
    }
}
