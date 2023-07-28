using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Timestamp
{
    public record TimestampConstantExpression :
        IConstantValueExpression<IonTimestamp>,
        IResultParsable<TimestampConstantExpression>
    {
        #region Grammar Symbols
        internal const string TimestampConstantSymbol = "constant-timestamp-expression";
        internal const string NowSymbol = "now";
        internal const string MillisecondPrecisionSymbol = "millisecond-precision";
        internal const string SecondPrecisionSymbol = "second-precision";
        internal const string MinutePrecisionSymbol = "minute-precision";
        internal const string DayPrecisionSymbol = "day-precision";
        internal const string MonthPrecisionSymbol = "month-precision";
        internal const string YearPrecisionSymbol = "year-precision";
        internal const string TimeZoneOffsetSymbol = "time-zone-offset";

        internal const string YearSymbol = "year";
        internal const string MonthSymbol = "month";
        internal const string DaySymbol = "day";
        #endregion

        public IonTimestamp Ion { get; }

        public TimestampConstantExpression(DateTimeOffset value)
        {
            Ion = value;
        }

        #region Evaluate
        public ITypedAtom<IonTimestamp> Evaluate() => new Atom<IonTimestamp>(Ion);

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse methods

        public static bool TryParse(
            string text,
            out IResult<TimestampConstantExpression> result)
            => (result = Parse(text)) is IResult<TimestampConstantExpression>.DataResult;

        public static bool TryParse(
            CSTNode timestampExpressionNode,
            out IResult<TimestampConstantExpression> result)
            => (result = Parse(timestampExpressionNode)) is IResult<TimestampConstantExpression>.DataResult;

        public static IResult<TimestampConstantExpression> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(TimestampConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return Parse(success.Symbol);

            return Result.Of<TimestampConstantExpression>(new RecognitionException(parseResult));
        }

        public static IResult<TimestampConstantExpression> Parse(CSTNode timestampExpressionNode)
        {
            if (timestampExpressionNode is null)
                throw new ArgumentNullException(nameof(timestampExpressionNode));

            try
            {
                var precision = timestampExpressionNode.NodeAt(1);
                return precision.SymbolName switch
                {
                    NowSymbol => Result.Of(new TimestampConstantExpression(DateTimeOffset.Now)),

                    YearPrecisionSymbol => TimestampConstantExpression
                        .DeconstructYear(precision)
                        .ApplyTo(tokens => $"{tokens.year}-01-01 {tokens.zone}")
                        .ApplyTo(token => DateTimeOffset.TryParse(token, out var val)
                            ? Result.Of(new TimestampConstantExpression(val))
                            : Result.Of<TimestampConstantExpression>(
                                new FormatException($"Invalid timestamp literal: {precision.TokenValue()}"))),

                    MonthPrecisionSymbol => TimestampConstantExpression
                        .DeconstructMonth(precision)
                        .ApplyTo(tokens => $"{tokens.year}-{tokens.month}-01 {tokens.zone}")
                        .ApplyTo(token => DateTimeOffset.TryParse(token, out var val)
                            ? Result.Of(new TimestampConstantExpression(val))
                            : Result.Of<TimestampConstantExpression>(
                                new FormatException($"Invalid timestamp literal: {precision.TokenValue()}"))),

                    DayPrecisionSymbol => TimestampConstantExpression
                        .DeconstructDay(precision)
                        .ApplyTo(tokens => $"{tokens.year}-{tokens.month}-{tokens.day} {tokens.zone}")
                        .ApplyTo(token => DateTimeOffset.TryParse(token, out var val)
                            ? Result.Of(new TimestampConstantExpression(val))
                            : Result.Of<TimestampConstantExpression>(
                                new FormatException($"Invalid timestamp literal: {precision.TokenValue()}"))),

                    MillisecondPrecisionSymbol
                    or SecondPrecisionSymbol
                    or MinutePrecisionSymbol => DateTimeOffset.TryParse(precision.TokenValue(), out var val)
                        ? Result.Of(new TimestampConstantExpression(val))
                        : Result.Of<TimestampConstantExpression>(new FormatException($"Invalid timestamp literal: {precision.TokenValue()}")),

                    _ => throw new ArgumentException($"Invalid precision symbol: '{precision.SymbolName}'")
                };
            }
            catch(Exception e)
            {
                return Result.Of<TimestampConstantExpression>(e);
            }
        }
        #endregion

        public override string ToString()
        {
            return $"'T {Ion.ToIonText()}'";
        }

        private static (string year, string zone) DeconstructYear(CSTNode yearPrecision)
        {
            return (
                yearPrecision.FindNode(YearSymbol).TokenValue(),
                ToZone(yearPrecision.FindNode(TimeZoneOffsetSymbol)));
        }

        private static (string year, string month, string zone) DeconstructMonth(CSTNode monthPrecision)
        {
            return (
                monthPrecision.FindNode(YearSymbol).TokenValue(),
                monthPrecision.FindNode(MonthSymbol).TokenValue(),
                ToZone(monthPrecision.FindNode(TimeZoneOffsetSymbol)));
        }

        private static (string year, string month, string day, string zone) DeconstructDay(CSTNode dayPrecision)
        {
            return (
                dayPrecision.FindNode(YearSymbol).TokenValue(),
                dayPrecision.FindNode(MonthSymbol).TokenValue(),
                dayPrecision.FindNode(DaySymbol).TokenValue(),
                ToZone(dayPrecision.FindNode(TimeZoneOffsetSymbol)));
        }

        private static string ToZone(CSTNode zoneOffset)
        {
            var tokens = zoneOffset?
                .TokenValue()
                .ToUpper();

            var localOffset = DateTimeOffset.Now.Offset;
            return
                tokens is null ? $"{localOffset.Hours:+00;-00}:{localOffset.Minutes:00}" :
                "Z".Equals(tokens) ? "+00:00" :
                tokens;
        }
    }
}
