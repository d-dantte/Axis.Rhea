using Axis.Luna.Extensions;
using Axis.Rhea.Shared.Contract.StateExpressions.Numeric;
using static Axis.Rhea.Shared.Contract.StateExpressions.BinaryOperationGrouper;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions
{
    [TestClass]
    public class BinaryExpressionGrouperTests
    {
        private static readonly IEnumerable<Enum> ArithmeticOperators = Enum
            .GetValues<ArithmeticOperation.Operators>()
            .OrderByDescending(op => op)
            .Select(op => (Enum)op);

        //43.5 + 12 + 5 - (-9) / 2 + (-1) * (-1) / 7 / 10 * 11 * 100 ^ 2 ^ 2 ^ 2 * 8
        private static readonly IEnumerable<OperationItem> Expression1 = new List<OperationItem>
        {
            new ExpressionItem(new RealConstantExpression(43.5d)),
            new OperatorItem(ArithmeticOperation.Operators.Add),
            new ExpressionItem(new IntegerConstantExpression(12)),
            new OperatorItem(ArithmeticOperation.Operators.Add),
            new ExpressionItem(new IntegerConstantExpression(5)),
            new OperatorItem(ArithmeticOperation.Operators.Subtract),
            new ExpressionItem(new IntegerConstantExpression(-9)),
            new OperatorItem(ArithmeticOperation.Operators.Divide),
            new ExpressionItem(new IntegerConstantExpression(2)),
            new OperatorItem(ArithmeticOperation.Operators.Add),
            new ExpressionItem(new IntegerConstantExpression(-1)),
            new OperatorItem(ArithmeticOperation.Operators.Multiply),
            new ExpressionItem(new IntegerConstantExpression(-1)),
            new OperatorItem(ArithmeticOperation.Operators.Divide),
            new ExpressionItem(new IntegerConstantExpression(7)),
            new OperatorItem(ArithmeticOperation.Operators.Divide),
            new ExpressionItem(new IntegerConstantExpression(10)),
            new OperatorItem(ArithmeticOperation.Operators.Multiply),
            new ExpressionItem(new IntegerConstantExpression(11)),
            new OperatorItem(ArithmeticOperation.Operators.Multiply),
            new ExpressionItem(new IntegerConstantExpression(100)),
            new OperatorItem(ArithmeticOperation.Operators.Power),
            new ExpressionItem(new IntegerConstantExpression(2)),
            new OperatorItem(ArithmeticOperation.Operators.Power),
            new ExpressionItem(new IntegerConstantExpression(2)),
            new OperatorItem(ArithmeticOperation.Operators.Power),
            new ExpressionItem(new IntegerConstantExpression(2)),
            new OperatorItem(ArithmeticOperation.Operators.Multiply),
            new ExpressionItem(new IntegerConstantExpression(8))
        };

        [TestMethod]
        public void Group_WithValidBinaryOperation_ShouldGroupProperly()
        {
            var grouped = ArithmeticOperators
                .Aggregate(Expression1, Group)
                .ToArray();

            Assert.AreEqual(1, grouped.Length);
            var baseGroup = grouped[0].As<GroupItem>();
            Assert.AreEqual(ArithmeticOperation.Operators.Add, baseGroup.Operator);
            Assert.AreEqual(4, baseGroup.Items.Length);

            Assert.IsTrue(baseGroup.Items[0] is ExpressionItem);
            Assert.IsTrue(baseGroup.Items[1] is ExpressionItem);
            Assert.IsTrue(baseGroup.Items[2] is GroupItem gi && gi.Operator.Equals(ArithmeticOperation.Operators.Subtract));
            Assert.IsTrue(baseGroup.Items[3] is GroupItem gi2 && gi2.Operator.Equals(ArithmeticOperation.Operators.Multiply));
        }

        [TestMethod]
        public void GroupItem_ToGroupedOperation_ShouldGroupProperly()
        {
            // add
            var itemGroup = new GroupItem(ArithmeticOperation.Operators.Add)
                .AddItem(new ExpressionItem(new IntegerConstantExpression(4)))
                .AddItem(new ExpressionItem(new IntegerConstantExpression(5)))
                .AddItem(new ExpressionItem(new IntegerConstantExpression(6)));

            var op = itemGroup.ToGroupedOperation(
                ArithmeticOperation.OperationProvider,
                ArithmeticOperation.PrecedenceDirectionEvaluator);
            Assert.AreEqual("((4 + 5) + 6)", op.ToString());

            // subtract
            itemGroup = new GroupItem(ArithmeticOperation.Operators.Subtract)
                .AddItem(new ExpressionItem(new IntegerConstantExpression(4)))
                .AddItem(new ExpressionItem(new IntegerConstantExpression(-5)));

            op = itemGroup.ToGroupedOperation(
                ArithmeticOperation.OperationProvider,
                ArithmeticOperation.PrecedenceDirectionEvaluator);

            Assert.AreEqual("(4 - -5)", op.ToString());

            // divide
            itemGroup = new GroupItem(ArithmeticOperation.Operators.Divide)
                .AddItem(new ExpressionItem(new IntegerConstantExpression(4)))
                .AddItem(new ExpressionItem(new IntegerConstantExpression(-1)))
                .AddItem(new ExpressionItem(new IntegerConstantExpression(0)))
                .AddItem(new ExpressionItem(new DecimalConstantExpression(2.7m)));

            op = itemGroup.ToGroupedOperation(
                ArithmeticOperation.OperationProvider,
                ArithmeticOperation.PrecedenceDirectionEvaluator);

            Assert.AreEqual("(((4 / -1) / 0) / 2.7D0)", op.ToString());

            // power
            itemGroup = new GroupItem(ArithmeticOperation.Operators.Power)
                .AddItem(new ExpressionItem(new IntegerConstantExpression(4)))
                .AddItem(new ExpressionItem(new IntegerConstantExpression(-1)))
                .AddItem(new ExpressionItem(new IntegerConstantExpression(2)))
                .AddItem(new ExpressionItem(new IntegerConstantExpression(5)));

            op = itemGroup.ToGroupedOperation(
                ArithmeticOperation.OperationProvider,
                ArithmeticOperation.PrecedenceDirectionEvaluator);

            Assert.AreEqual("(4 ^ (-1 ^ (2 ^ 5)))", op.ToString());
        }
    }
}
