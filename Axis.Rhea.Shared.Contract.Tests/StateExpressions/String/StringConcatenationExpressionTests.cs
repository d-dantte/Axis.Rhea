using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions;
using Axis.Rhea.Shared.Contract.StateExpressions.Duration;
using Axis.Rhea.Shared.Contract.StateExpressions.Numeric;
using Axis.Rhea.Shared.Contract.StateExpressions.String;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.String
{
    [TestClass]
    public class StringConcatenationExpressionTests
    {
        private static readonly IonStruct State = new IonStruct.Initializer
        {
            ["abc"] = new IonStruct.Initializer
            {
                ["xyz"] = "something",
                ["ghi"] = 54,
                ["jkl"] = true,
                ["qrs"] = DateTimeOffset.Parse("2023-05-15T14:00:31.072713+05:00")
            }
        };

        [TestMethod]
        public void Parse_Tests()
        {
            var exp = "string::/abc/xyz + \" some string literal \" + 5.75D-5 + \" \" + time-stamp::/abc/qrs + \" \" + 'D 12:23:45'";
            var result = StringConcatenationOperation.Parse(exp);
            Assert.IsTrue(result is IResult<StringConcatenationOperation>.DataResult);
        }

        [TestMethod]
        public void Evaluate_Tests()
        {
            var exp = "string::/abc/xyz + \" some string literal \" + 5.75D-5 + \" \" + time-stamp::/abc/qrs + \" \" + 'D 12:23:45'";
            var result = StringConcatenationOperation.Parse(exp);
            Assert.IsTrue(result is IResult<StringConcatenationOperation>.DataResult);

            var concatenated = result
                .Map(exp => exp.EvaluateWithState(State))
                .Resolve();
            Assert.AreEqual(
                "something some string literal 5.75D-5 'T 2023-05-15T14:00:31.072713+05:00' 'D 12:23:45'",
                concatenated.Ion.ToAtomText());
        }

        [TestMethod]
        public void Concatenate_Tests()
        {
            var exp = new StringConcatenationOperation(
                new StringConstantExpression(""),
                new IntegerConstantExpression(34),
                new StringConstantExpression(" "),
                new DurationConstantExpression(TimeSpan.FromHours(2.5)),
                new StringConstantExpression("  the things"));

            var concatenated = exp.Concatenate();

            Assert.AreEqual("34 'D 02:30:00'  the things", concatenated.Ion.ToAtomText());
        }
    }
}
