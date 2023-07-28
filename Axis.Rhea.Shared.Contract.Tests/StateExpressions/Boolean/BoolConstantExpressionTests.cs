using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Boolean;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Boolean
{
    [TestClass]
    public class BoolConstantExpressionTests
    {
        [TestMethod]
        public void Evaluate_Tests()
        {
            var @bool = false;
            var cnst = new BoolConstantExpression(@bool);
            var atom = cnst.Evaluate();

            Assert.IsNotNull(atom);
            Assert.AreEqual(@bool, atom.Ion.Value);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var result = BoolConstantExpression.Parse("true");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<BoolConstantExpression>.DataResult);
            Assert.AreEqual(true, result.Resolve().Ion.Value);

            result = BoolConstantExpression.Parse("False");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<BoolConstantExpression>.DataResult);
            Assert.AreEqual(false, result.Resolve().Ion.Value);
        }
    }
}
