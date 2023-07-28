using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions;
using Axis.Rhea.Shared.Contract.StateExpressions.String;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.String
{
    [TestClass]
    public class StringConversionExpressionTests
    {
        private static string Timestamp = "2023-05-15T14:00:31.072713+05:00";
        private static readonly IonStruct State = new IonStruct.Initializer
        {
            ["abc"] = new IonStruct.Initializer
            {
                ["qrs"] = DateTimeOffset.Parse(Timestamp)
            }
        };

        [TestMethod]
        public void Parse_Tests()
        {
            var exp = "as-string::'T 2023-05-14T11:30:41.527287Z'";
            var exp2 = "as-string::time-stamp::/abc/qrs";

            var result = StringConversionExpression.Parse(exp);
            Assert.IsTrue(result is IResult<StringConversionExpression>.DataResult);

            result = StringConversionExpression.Parse(exp2);
            Assert.IsTrue(result is IResult<StringConversionExpression>.DataResult);

        }

        [TestMethod]
        public void Evaluate_Tests()
        {
            var exp = "'T 2023-05-14T11:30:41.527287+00:00'";
            var exp2 = "as-string::time-stamp::/abc/qrs";

            var result = StringConversionExpression.Parse($"as-string::{exp}");
            var value = result.Resolve().Evaluate();
            Assert.AreEqual(exp, value.ToString());

            result = StringConversionExpression.Parse(exp2);
            value = EvaluationContext.EvaluateWithState(result.Resolve(), State);
            Assert.AreEqual($"'T {Timestamp}'", value.ToString());

        }
    }
}
