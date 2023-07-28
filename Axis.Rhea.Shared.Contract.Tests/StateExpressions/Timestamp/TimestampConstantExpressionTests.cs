using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Timestamp;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Timestamp
{
    [TestClass]
    public class TimestampConstantExpressionTests
    {
        [TestMethod]
        public void Evaluate_Tests()
        {
            var now = DateTimeOffset.Now;
            var cnst = new TimestampConstantExpression(now);
            var atom = cnst.Evaluate();

            Assert.IsNotNull(atom);
            Assert.AreEqual(now, atom.Ion.Value);
        }

        [TestMethod]
        public void Parse_WithValidText()
        {
            // "2023-05-14T11:30:41.527287+02:00";

            var result = TimestampConstantExpression.Parse("'T now'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            // UTC precision
            result = TimestampConstantExpression.Parse("'T 2023-05-14T11:30:41.5272871Z'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            var instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 14, 11, 30, 41, 527, 287, 100, (0, 0));

            // millisecond precision
            result = TimestampConstantExpression.Parse("'T 2023-05-14T11:30:41.5272871+02:00'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 14, 11, 30, 41, 527, 287, 100, (2, 0));

            // second precision
            result = TimestampConstantExpression.Parse("'T 2023-05-14T11:30:41+02:00'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 14, 11, 30, 41, 0, 0, 0, (2, 0));

            // minute precision
            result = TimestampConstantExpression.Parse("'T 2023-05-14T11:30+02:00'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 14, 11, 30, 0, 0, 0, 0, (2, 0));

            // day precision
            result = TimestampConstantExpression.Parse("'T 2023-05-14T'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 14, 0, 0, 0, 0, 0, 0, (2, 0));

            result = TimestampConstantExpression.Parse("'T 2023-05-14TZ'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 14, 0, 0, 0, 0, 0, 0, (0, 0));

            result = TimestampConstantExpression.Parse("'T 2023-05-14T-06:00'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 14, 0, 0, 0, 0, 0, 0, (0, 0));

            // month precision
            result = TimestampConstantExpression.Parse("'T 2023-05T'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 1, 0, 0, 0, 0, 0, 0, (2, 0));

            result = TimestampConstantExpression.Parse("'T 2023-05TZ'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 1, 0, 0, 0, 0, 0, 0, (0, 0));

            result = TimestampConstantExpression.Parse("'T 2023-05T-06:00'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 5, 1, 0, 0, 0, 0, 0, 0, (0, 0));

            // year precision
            result = TimestampConstantExpression.Parse("'T 2023T'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 1, 1, 0, 0, 0, 0, 0, 0, (2, 0));

            result = TimestampConstantExpression.Parse("'T 2023TZ'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 1, 1, 0, 0, 0, 0, 0, 0, (0, 0));

            result = TimestampConstantExpression.Parse("'T 2023T-06:00'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<TimestampConstantExpression>.DataResult);

            instant = result.Resolve().Ion.Value!.Value;
            AssertDate(instant, 2023, 1, 1, 0, 0, 0, 0, 0, 0, (0, 0));
        }

        private static void AssertDate(
            DateTimeOffset instant,
            int year,
            int month = -1,
            int day = 0,
            int hour = -1,
            int minute = -1,
            int second = -1,
            short milliseconds = -1,
            short microseconds = -1,
            short nanoseconds = -1,
            (int hour, int minute) zoneOffset = default)
        {
            Assert.AreEqual(year, instant.Year);

            if (month >= 0)
                Assert.AreEqual(month, instant.Month);

            if (day > 0)
                Assert.AreEqual(day, instant.Day);

            if (hour >= 0)
                Assert.AreEqual(hour, instant.Hour);

            if (minute >= 0)
                Assert.AreEqual(minute, instant.Minute);

            if (second >= 0)
                Assert.AreEqual(second, instant.Second);

            if (milliseconds >= 0)
                Assert.AreEqual(milliseconds, instant.Millisecond);

            if (microseconds >= 0)
                Assert.AreEqual(microseconds, instant.Microsecond);

            if (nanoseconds >= 0)
                Assert.AreEqual(nanoseconds, instant.Nanosecond);

            if (zoneOffset != default)
                Assert.AreEqual(zoneOffset, (instant.Offset.Hours, instant.Offset.Minutes));
        }
    }
}
