using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Numeric;
using System.Numerics;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Numeric
{
    [TestClass]
    public class IntConstantExpressionTests
    {
        [TestMethod]
        public void Evaluate_Test()
        {
            var value = BigInteger.Parse("1000001234500000678900000000001");
            var cnst = new IntegerConstantExpression(value);
            var atom = cnst.Evaluate();

            Assert.IsNotNull(atom);
            Assert.AreEqual(value, atom.Ion.Value);
        }

        [TestMethod]
        public void Parse_Test()
        {
            var value = BigInteger.Parse("56767898765456789098098789087");
            var result = IntegerConstantExpression.Parse(value.ToString());
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<IntegerConstantExpression>.DataResult);
            Assert.AreEqual(value, result.Resolve().Ion.Value);

            result = IntegerConstantExpression.Parse(BigInteger.Negate(value).ToString());
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<IntegerConstantExpression>.DataResult);
            Assert.AreEqual(BigInteger.Negate(value), result.Resolve().Ion.Value);
        }
    }
}
