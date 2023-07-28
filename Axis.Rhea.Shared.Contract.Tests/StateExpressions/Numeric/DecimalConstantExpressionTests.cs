using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Numeric;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Numeric
{
    [TestClass]
    public class DecimalConstantExpressionTests
    {
        [TestMethod]
        public void Evaluate_Test()
        {
            var value = 345676.9878987m;
            var cnst = new DecimalConstantExpression(value);
            var atom = cnst.Evaluate();

            Assert.IsNotNull(atom);
            Assert.AreEqual(value, atom.Ion.Value);
        }

        [TestMethod]
        public void Parse_Test()
        {
            var value = 0.008765m;
            var result = DecimalConstantExpression.Parse("8.765D-3");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DecimalConstantExpression>.DataResult);
            Assert.AreEqual(value, result.Resolve().Ion.Value);

            result = DecimalConstantExpression.Parse("-8.765d-03");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DecimalConstantExpression>.DataResult);
            Assert.AreEqual(-value, result.Resolve().Ion.Value);

            value = 8765m;
            result = DecimalConstantExpression.Parse("8.765D3");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DecimalConstantExpression>.DataResult);
            Assert.AreEqual(value, result.Resolve().Ion.Value);

            value = 8765m;
            result = DecimalConstantExpression.Parse("8.765d+3");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DecimalConstantExpression>.DataResult);
            Assert.AreEqual(value, result.Resolve().Ion.Value);

            value = 876.5m;
            result = DecimalConstantExpression.Parse("876.50");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<DecimalConstantExpression>.DataResult);
            Assert.AreEqual(value, result.Resolve().Ion.Value);
        }
    }
}
