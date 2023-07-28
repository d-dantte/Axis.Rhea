using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Numeric;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Numeric
{
    [TestClass]
    public class RealConstantExpressionTests
    {
        [TestMethod]
        public void Evaluate_Test()
        {
            var value = 345676.9878987d;
            var cnst = new RealConstantExpression(value);
            var atom = cnst.Evaluate();

            Assert.IsNotNull(atom);
            Assert.AreEqual(value, atom.Ion.Value);
        }

        [TestMethod]
        public void Parse_Test()
        {
            var value = 0.008765d;
            var result = RealConstantExpression.Parse("8.765E-3");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<RealConstantExpression>.DataResult);
            Assert.AreEqual(value, result.Resolve().Ion.Value);

            result = RealConstantExpression.Parse("-8.765E-03");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<RealConstantExpression>.DataResult);
            Assert.AreEqual(-value, result.Resolve().Ion.Value);

            value = 8765d;
            result = RealConstantExpression.Parse("8.765E3");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<RealConstantExpression>.DataResult);
            Assert.AreEqual(value, result.Resolve().Ion.Value);

            value = 8765d;
            result = RealConstantExpression.Parse("8.765e+3");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<RealConstantExpression>.DataResult);
            Assert.AreEqual(value, result.Resolve().Ion.Value);
        }
    }
}
