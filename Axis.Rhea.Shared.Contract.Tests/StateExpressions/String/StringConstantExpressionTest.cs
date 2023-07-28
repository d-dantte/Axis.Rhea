using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.String;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.String
{
    [TestClass]
    public class StringConstantExpressionTest
    {
        [TestMethod]
        public void Evaluate_Test()
        {
            var @string = "some string";
            var cnst = new StringConstantExpression(@string);
            var atom = cnst.Evaluate();

            Assert.IsNotNull(atom);
            Assert.AreEqual(@string, atom.Ion.Value);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var @string = "some string here";
            var result = StringConstantExpression.Parse($"\"{@string}\"");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<StringConstantExpression>.DataResult);
            var sresult = result.Resolve();
            Assert.AreEqual(@string, sresult.Ion.Value);
        }
    }
}
