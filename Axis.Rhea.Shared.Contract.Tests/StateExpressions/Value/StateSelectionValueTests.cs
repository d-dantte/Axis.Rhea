using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.StateExpressions;
using Axis.Rhea.Shared.Contract.StateExpressions.Value;
using Axis.Rhea.Shared.Contract.Workflow.State;
using Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions.Value
{
    [TestClass]
    public class StateSelectionValueTests
    {
        private static readonly IonStruct State = new IonStruct.Initializer
        {
            ["abc"] = new IonStruct.Initializer
            {
                ["def"] = "something",
                ["ghi"] = 54,
                ["jkl"] = true,
                ["mno"] = DateTimeOffset.Now
            }
        };

        [TestMethod]
        public void Evaluate_Test()
        {
            var path = DataPathSegment.Parse("/abc/def");
            var cnst = new StateSelectionValue<IonString>(path);
            var atom = cnst.EvaluateWithState(new WorkflowState(State));
            Assert.IsNotNull(atom);
            Assert.AreEqual("something", atom.Ion.Value);
        }

        [TestMethod]
        public void GenericParse_Tests()
        {
            var result = StateSelectionValue<IonInt>.Parse("/abc/ghi");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<StateSelectionValue<IonInt>>.DataResult);
            Assert.AreEqual("/abc/ghi", result.Resolve().Path.ToString());
        }

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<FormatException>(() => StateSelectionValue.Parse("/abc/def"));

            // int
            var result = StateSelectionValue.Parse("int::/abc/ghi");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<IExpression>.DataResult);
            var intExp = result
                .Map(exp => (ITypedExpression<IonInt>)exp)
                .Resolve();

            var intSelection = intExp as StateSelectionValue<IonInt>;
            Assert.IsNotNull(intSelection);
            Assert.AreEqual("/abc/ghi", intSelection.Path.ToString());

            // real
            result = StateSelectionValue.Parse("real::/abc/xyz");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<IExpression>.DataResult);
            var realExp = result
                .Map(exp => (ITypedExpression<IonFloat>)exp)
                .Resolve();

            var realSelection = realExp as StateSelectionValue<IonFloat>;
            Assert.IsNotNull(realSelection);
            Assert.AreEqual("/abc/xyz", realSelection.Path.ToString());

            // decimal
            result = StateSelectionValue.Parse("decimal::/abc/xyz");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<IExpression>.DataResult);
            var decimalExp = result
                .Map(exp => (ITypedExpression<IonDecimal>)exp)
                .Resolve();

            var decimalSelection = decimalExp as StateSelectionValue<IonDecimal>;
            Assert.IsNotNull(decimalSelection);
            Assert.AreEqual("/abc/xyz", decimalSelection.Path.ToString());

            // bool
            result = StateSelectionValue.Parse("bool::/abc/jkl");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<IExpression>.DataResult);
            var boolExp = result
                .Map(exp => (ITypedExpression<IonBool>)exp)
                .Resolve();

            var boolSelection = boolExp as StateSelectionValue<IonBool>;
            Assert.IsNotNull(boolSelection);
            Assert.AreEqual("/abc/jkl", boolSelection.Path.ToString());

            // string
            result = StateSelectionValue.Parse("string::/abc/xyz");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<IExpression>.DataResult);
            var stringExp = result
                .Map(exp => (ITypedExpression<IonString>)exp)
                .Resolve();

            var stringSelection = stringExp as StateSelectionValue<IonString>;
            Assert.IsNotNull(stringSelection);
            Assert.AreEqual("/abc/xyz", stringSelection.Path.ToString());

            // time stamp
            result = StateSelectionValue.Parse("time-stamp::/abc/mno");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is IResult<IExpression>.DataResult);
            var timestampExp = result
                .Map(exp => (ITypedExpression<IonTimestamp>)exp)
                .Resolve();

            var timestampSelection = timestampExp as StateSelectionValue<IonTimestamp>;
            Assert.IsNotNull(timestampSelection);
            Assert.AreEqual("/abc/mno", timestampSelection.Path.ToString());
        }
    }
}
