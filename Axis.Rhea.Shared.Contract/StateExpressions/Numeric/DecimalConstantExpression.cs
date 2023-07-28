using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;
using System.Globalization;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Numeric
{
    public record DecimalConstantExpression :
        INumericExpression,
        IConstantValueExpression<IonDecimal>,
        IResultParsable<DecimalConstantExpression>
    {
        #region Grammar Symbols
        internal const string DecimalConstantSymbol = "decimal-notation";
        #endregion

        public IonDecimal Ion { get; }

        IonNumber IConstantValueExpression<IonNumber>.Ion => new IonNumber(Ion.Value);

        public DecimalConstantExpression(decimal value)
        {
            Ion = value;
        }

        #region Evaluate
        public ITypedAtom<IonDecimal> Evaluate() => new Atom<IonDecimal>(Ion);

        ITypedAtom<IonNumber> ITypedExpression<IonNumber>.Evaluate()
            => new Atom<IonNumber>(((IConstantValueExpression<IonNumber>)this).Ion);


        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse methods

        public static bool TryParse(
            CSTNode decimalNotationNode,
            out IResult<DecimalConstantExpression> result)
            => (result = Parse(decimalNotationNode)) is IResult<DecimalConstantExpression>.DataResult;

        public static bool TryParse(
            string decimalNotation,
            out IResult<DecimalConstantExpression> result)
            => (result = Parse(decimalNotation)) is IResult<DecimalConstantExpression>.DataResult;

        public static IResult<DecimalConstantExpression> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(DecimalConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return Parse(success.Symbol);

            return Result.Of<DecimalConstantExpression>(new RecognitionException(parseResult));
        }

        public static IResult<DecimalConstantExpression> Parse(CSTNode decimalNotationNode)
        {
            if (decimalNotationNode is null)
                throw new ArgumentNullException(nameof(decimalNotationNode));

            var text = decimalNotationNode
                .TokenValue()
                .ToUpper()
                .Replace("D", "E");

            return decimal.TryParse(text, NumberStyles.Float, null, out var val)
                ? Result.Of(new DecimalConstantExpression(val))
                : Result.Of<DecimalConstantExpression>(new FormatException($"Invalid decimal literal: {text}"));
        }
        #endregion

        public override string ToString()
        {
            return Ion.ToIonText();
        }
    }
}
