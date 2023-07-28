using Axis.Ion.Numerics;
using Axis.Ion.Types;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.StateExpressions.Numeric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Numeric
{
    [TestClass]
    public class ArithmeticOperationTests
    {
        private static readonly IonStruct State = new IonStruct.Initializer
        {
            ["abc"] = new IonStruct.Initializer
            {
                ["xyz"] = 5m
            }
        };

        [TestMethod]
        public void Parse_Tests()
        {
            var ex = "(54 - 2.5 * (int::/abc/xyz ** 2) ** 2 ** 1 / 819)";
            var result = ArithmeticOperation.Parse(ex);
            Assert.IsTrue(result is IResult<ArithmeticOperation>.DataResult);
        }

        [TestMethod]
        public void Evaluate_Tests()
        {
            var ex = "(54 - 2.5 * (decimal::/abc/xyz ** 2) ** 2 ** 1 / 819)";
            var result = ArithmeticOperation.Parse(ex);
            var value = EvaluationContext.EvaluateWithState(
                result.Resolve(),
                State);

            Assert.AreEqual(
                BigDecimal.Parse("52.09218559218559218559218559225").Resolve(),
                value.Ion.Value!.Value);
        }

        [TestMethod]
        public void Subtract_Tests()
        {
            var testData = new List<(INumericType subject, INumericType @object, IonNumber result)>
            {
                (new IonInt(52), new IonDecimal(3.8m), new IonNumber(48.2m)),
                (new IonInt(52), new IonDecimal(0m), new IonNumber(52)),
                (new IonInt(0), new IonDecimal(0m), new IonNumber(0)),
                (new IonInt(0), new IonDecimal(-10.12m), new IonNumber(-10.12m)),
                (new IonInt(73), new IonDecimal(-10.12m), new IonNumber(83.12m)),
                (new IonInt(-73), new IonDecimal(-10.12m), new IonNumber(-62.88m)),
            };

            Test(ArithmeticOperation.Operators.Subtract, testData);
        }

        [TestMethod]
        public void Add_Tests()
        {
            var testData = new List<(INumericType subject, INumericType @object, IonNumber result)>
            {
                (new IonInt(52), new IonDecimal(3.8m), new IonNumber(55.8m)),
                (new IonInt(52), new IonDecimal(0m), new IonNumber(52)),
                (new IonInt(0), new IonDecimal(0m), new IonNumber(0)),
                (new IonInt(0), new IonDecimal(-10.12m), new IonNumber(-10.12m)),
                (new IonInt(73), new IonDecimal(-10.12m), new IonNumber(62.88m)),
                (new IonInt(-73), new IonDecimal(-10.12m), new IonNumber(-83.12m)),
            };

            Test(ArithmeticOperation.Operators.Add, testData);
        }

        private void Test(
            ArithmeticOperation.Operators @operator,
            List<(INumericType subject, INumericType @object, IonNumber result)> testData)
        {
            foreach (var data in testData)
            {
                var subject = CreateExpression(data.subject);
                var @object = CreateExpression(data.@object);
                var op = new ArithmeticOperation(subject, @object, @operator);
                var result = @operator switch
                {
                    ArithmeticOperation.Operators.Power => op.Power(),
                    ArithmeticOperation.Operators.Divide => op.Divide(),
                    ArithmeticOperation.Operators.Multiply => op.Multiply(),
                    ArithmeticOperation.Operators.Modulus => op.Modulus(),
                    ArithmeticOperation.Operators.Subtract => op.Subtract(),
                    ArithmeticOperation.Operators.Add => op.Add(),
                    _ => throw new ArgumentException($"Invalid operation: '{@operator}'")
                };

                Assert.AreEqual(data.result, result.Ion);
            }
        }

        private void Test(
            ArithmeticOperation.Operators @operator,
            List<(INumericType subject, INumericType @object, Action<ArithmeticOperation> assertion)> testData)
        {
            foreach (var data in testData)
            {
                var subject = CreateExpression(data.subject);
                var @object = CreateExpression(data.@object);
                var op = new ArithmeticOperation(subject, @object, @operator);
                data.assertion.Invoke(op);
            }
        }

        private INumericExpression CreateExpression(INumericType ion)
        {
            return ion switch
            {
                IonInt i => new IntegerConstantExpression(i.Value!.Value),
                IonDecimal d => new DecimalConstantExpression(d.Value!.Value),
                IonFloat f => new RealConstantExpression(f.Value!.Value),
                _ => throw new ArgumentException($"Invalid numeric type: '{ion?.GetType()}'")
            };
        }
    }
}
