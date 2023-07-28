using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Duration;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Duration
{
    [TestClass]
    public class DurationArithmeticOperationTests
    {
        [TestMethod]
        public void Parse_Tests()
        {
            var result = DurationArithmeticOperation.Parse("('D 5' + 'D 01:34' - 'D 00:00:27')");
            Assert.IsTrue(result is IResult<DurationArithmeticOperation>.DataResult);
        }

        [TestMethod]
        public void Evaluate_Tests()
        {
            var result = DurationArithmeticOperation.Parse("('D 5' + 'D 00:00:01' + 'D 00:00:01' + 'D 01:34' - 'D 00:00:27')");
            var value = result
                .Map(v => v.Evaluate())
                .Resolve();
            Assert.AreEqual(new TimeSpan(5, 1, 33, 35), value.Ion.Value!.Value);

        }
    }
}
