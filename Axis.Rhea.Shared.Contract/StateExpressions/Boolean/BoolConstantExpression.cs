using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Boolean
{
    public record BoolConstantExpression :
        IConstantValueExpression<IonBool>,
        IResultParsable<BoolConstantExpression>
    {
        #region Grammar Symbols
        internal const string BoolConstantSymbol = "constant-boolean-expression";
        #endregion

        public IonBool Ion { get; }

        public BoolConstantExpression(bool value)
        {
            Ion = value;
        }

        #region Evaluate
        public ITypedAtom<IonBool> Evaluate() => new Atom<IonBool>(Ion);

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse functions

        public static IResult<BoolConstantExpression> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(BoolConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
            {
                _ = TryParse(success.Symbol, out IResult<BoolConstantExpression> result);
                return result;
            }

            return Result.Of<BoolConstantExpression>(new RecognitionException(parseResult));
        }

        public static bool TryParse(string text, out IResult<BoolConstantExpression> result)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(BoolConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return TryParse(success.Symbol, out result);

            result = Result.Of<BoolConstantExpression>(new RecognitionException(parseResult));
            return false;
        }

        public static IResult<BoolConstantExpression> Parse(CSTNode node)
        {
            _ = TryParse(node, out var result);
            return result;
        }

        public static bool TryParse(CSTNode constantBooleanNode, out IResult<BoolConstantExpression> result)
        {
            if (constantBooleanNode is null)
                throw new ArgumentNullException(nameof(constantBooleanNode));

            var text = constantBooleanNode.TokenValue();
            result = bool.TryParse(text, out var @bool)
                ? Result.Of(new BoolConstantExpression(@bool))
                : Result.Of<BoolConstantExpression>(new FormatException($"Invalid boolean literal: {text}"));

            return result is IResult<BoolConstantExpression>.DataResult;
        }
        #endregion

        public override string ToString()
        {
            return Ion.ToIonText();
        }
    }
}
