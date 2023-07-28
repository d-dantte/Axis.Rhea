using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;
using System.Numerics;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Numeric
{
    public record IntegerConstantExpression :
        INumericExpression,
        IConstantValueExpression<IonInt>,
        IResultParsable<IntegerConstantExpression>
    {
        #region Grammar Symbols
        internal const string IntegerConstantSymbol = "int-notation";
        #endregion

        public IonInt Ion { get; }

        IonNumber IConstantValueExpression<IonNumber>.Ion => new IonNumber(Ion.Value);

        public IntegerConstantExpression(BigInteger value)
        {
            Ion = value;
        }

        #region Evaluate
        public ITypedAtom<IonInt> Evaluate() => new Atom<IonInt>(Ion);

        ITypedAtom<IonNumber> ITypedExpression<IonNumber>.Evaluate()
            => new Atom<IonNumber>(((IConstantValueExpression<IonNumber>)this).Ion);

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse methods

        public static bool TryParse(
            string intNotation,
            out IResult<IntegerConstantExpression> result)
            => (result = Parse(intNotation)) is IResult<IntegerConstantExpression>.DataResult;

        public static bool TryParse(
            CSTNode intNotationNode,
            out IResult<IntegerConstantExpression> result)
            => (result = Parse(intNotationNode)) is IResult<IntegerConstantExpression>.DataResult;

        public static IResult<IntegerConstantExpression> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(IntegerConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return Parse(success.Symbol);

            return Result.Of<IntegerConstantExpression>(new RecognitionException(parseResult));
        }

        public static IResult<IntegerConstantExpression> Parse(CSTNode intNotationNode)
        {
            if (intNotationNode is null)
                throw new ArgumentNullException(nameof(intNotationNode));

            var text = intNotationNode.TokenValue();
            return BigInteger.TryParse(text, out var val)
                ? Result.Of(new IntegerConstantExpression(val))
                : Result.Of<IntegerConstantExpression>(new FormatException($"Invalid integer literal: {text}"));
        }
        #endregion

        public override string ToString()
        {
            return Ion.ToIonText();
        }
    }
}
