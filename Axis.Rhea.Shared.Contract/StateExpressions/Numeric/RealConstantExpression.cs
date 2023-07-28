using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Numeric
{
    public record RealConstantExpression :
        INumericExpression,
        IConstantValueExpression<IonFloat>,
        IResultParsable<RealConstantExpression>
    {
        #region Grammar Symbols
        internal const string RealConstantSymbol = "float-notation";
        #endregion

        public IonFloat Ion { get; }

        IonNumber IConstantValueExpression<IonNumber>.Ion => new IonNumber(Ion.Value);

        public RealConstantExpression(double value)
        {
            Ion = value;
        }

        #region Evaluate
        public ITypedAtom<IonFloat> Evaluate() => new Atom<IonFloat>(Ion);

        ITypedAtom<IonNumber> ITypedExpression<IonNumber>.Evaluate()
            => new Atom<IonNumber>(((IConstantValueExpression<IonNumber>)this).Ion);

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse methods

        public static IResult<RealConstantExpression> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(RealConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
            {
                _ = TryParse(success.Symbol, out IResult<RealConstantExpression> result);
                return result;
            }

            return Result.Of<RealConstantExpression>(new RecognitionException(parseResult));
        }

        public static bool TryParse(string text, out IResult<RealConstantExpression> result)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(RealConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return TryParse(success.Symbol, out result);

            result = Result.Of<RealConstantExpression>(new RecognitionException(parseResult));
            return false;
        }

        public static IResult<RealConstantExpression> Parse(CSTNode node)
        {
            _ = TryParse(node, out var result);
            return result;
        }

        public static bool TryParse(CSTNode floatNotationNode, out IResult<RealConstantExpression> result)
        {
            if (floatNotationNode is null)
                throw new ArgumentNullException(nameof(floatNotationNode));

            var text = floatNotationNode.TokenValue();
            result = double.TryParse(text, out var val)
                ? Result.Of(new RealConstantExpression(val))
                : Result.Of<RealConstantExpression>(new FormatException($"Invalid double literal: {text}"));

            return result is IResult<RealConstantExpression>.DataResult;
        }
        #endregion

        public override string ToString()
        {
            return Ion.ToIonText();
        }
    }
}
