using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Duration
{
    public record DurationConstantExpression :
        IConstantValueExpression<IonDuration>,
        IResultParsable<DurationConstantExpression>
    {
        #region Grammar Symbols
        internal const string DurationConstantSymbol = "constant-duration-expression";
        #endregion

        public IonDuration Ion { get; }

        public DurationConstantExpression(TimeSpan value)
        {
            Ion = value;
        }

        #region Evaluate
        public ITypedAtom<IonDuration> Evaluate() => new Atom<IonDuration>(Ion);

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse methods

        public static bool TryParse(
            CSTNode durationExpressionNode,
            out IResult<DurationConstantExpression> result)
            => (result = Parse(durationExpressionNode)) is IResult<DurationConstantExpression>.DataResult;

        public static bool TryParse(
            string durationExpression,
            out IResult<DurationConstantExpression> result)
            => (result = Parse(durationExpression)) is IResult<DurationConstantExpression>.DataResult;

        public static IResult<DurationConstantExpression> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(DurationConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return Parse(success.Symbol);

            return Result.Of<DurationConstantExpression>(new RecognitionException(parseResult));
        }

        public static IResult<DurationConstantExpression> Parse(CSTNode durationExpressionNode)
        {
            if (durationExpressionNode is null)
                throw new ArgumentNullException(nameof(durationExpressionNode));

            var text = durationExpressionNode
                .TokenValue()
                [2..^1];

            return TimeSpan.TryParse(text, out var val)
                ? Result.Of(new DurationConstantExpression(val))
                : Result.Of<DurationConstantExpression>(new FormatException($"Invalid TimeSpan literal: {text}"));

        }
        #endregion

        public override string ToString()
        {
            return Ion.ToIonText();
        }
    }
}
