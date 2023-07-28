using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Duration;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Duration
{
    [TestClass]
    public class DurationConstantExpressionTests
    {
        [TestMethod]
        public void Evaluate_Tests()
        {
            var days = TimeSpan.FromMinutes(54.2);
            var cnst = new DurationConstantExpression(days);
            var atom = cnst.Evaluate();

            Assert.IsNotNull(atom);
            Assert.AreEqual(days, atom.Ion.Value);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            // 34.18:22:47.870032801

            // sub-second precision
            var result = DurationConstantExpression.Parse("'D 34.18:22:47.8700328'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DurationConstantExpression>.DataResult);
            var duration = result.Resolve().Ion.Value!.Value;
            AssertDuration(duration, 34, 18, 22, 47, 870, 32, 800);

            result = DurationConstantExpression.Parse("'D 18:22:47.870032'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DurationConstantExpression>.DataResult);
            duration = result.Resolve().Ion.Value!.Value;
            AssertDuration(duration, 0, 18, 22, 47, 870, 32);

            result = DurationConstantExpression.Parse("'D 18:22:47.870'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DurationConstantExpression>.DataResult);
            duration = result.Resolve().Ion.Value!.Value;
            AssertDuration(duration, 0, 18, 22, 47, 870);

            // second precision
            result = DurationConstantExpression.Parse("'D 34.18:22:47'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DurationConstantExpression>.DataResult);
            duration = result.Resolve().Ion.Value!.Value;
            AssertDuration(duration, 34, 18, 22, 47);

            // minute precision
            result = DurationConstantExpression.Parse("'D 34.18:22'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DurationConstantExpression>.DataResult);
            duration = result.Resolve().Ion.Value!.Value;
            AssertDuration(duration, 34, 18, 22);

            // day precision
            result = DurationConstantExpression.Parse("'D 34'");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DurationConstantExpression>.DataResult);
            duration = result.Resolve().Ion.Value!.Value;
            AssertDuration(duration, 34, 0, 0);
        }

        private static void AssertDuration(
            TimeSpan instant,
            int days,
            int hours,
            int minutes,
            int second = 0,
            short milliseconds = 0,
            short microseconds = 0,
            short nanoseconds = 0)
        {
            Assert.AreEqual(days, instant.Days);
            Assert.AreEqual(hours, instant.Hours);
            Assert.AreEqual(minutes, instant.Minutes);
            Assert.AreEqual(second, instant.Seconds);
            Assert.AreEqual(milliseconds, instant.Milliseconds);
            Assert.AreEqual(microseconds, instant.Microseconds);
            Assert.AreEqual(nanoseconds, instant.Nanoseconds);
        }
    }
}
