using Axis.Ion.Types;
using Axis.Rhea.Directives.Contract;
using Axis.Rhea.Directives.Contract.Instructions;
using Axis.Rhea.Shared.Contract.Workflow.State;
using System.Numerics;

namespace Axis.Rhea.Core.Tests.Workflow.State
{
    [TestClass]
    public class WorkflowStateTests
    {
        [TestMethod]
        public void Apply_WithDeleteAndList_ShouldDeleteData()
        {
            // setup
            var state = new WorkflowState(new IonStruct.Initializer
            {
                ["items"] = new IonList.Initializer
                {
                    1,2,3,4,5
                },
                ["elements"] = new IonSexp.Initializer
                {
                    1,2,3,4,5
                }
            });

            var mutation = new StateMutation(
                new Delete("/items/[3]"),
                new Delete("/items/[1]"));

            var mutation2 = new StateMutation(
                new Delete("/elements/[3]"),
                new Delete("/elements/[1]"));

            // test
            _ = state
                .Apply(mutation)
                .Apply(mutation2);

            // assert
            var list = (IonList)state.Data.Properties["items"];
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(new BigInteger[] { 1, 3, 5 }.SequenceEqual(list.Value.Select(v => ((IonInt)v).Value.Value)));

            var sexp = (IonSexp)state.Data.Properties["elements"];
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(new BigInteger[] { 1, 3, 5 }.SequenceEqual(sexp.Value.Select(v => ((IonInt)v).Value.Value)));
        }

        [TestMethod]
        public void Apply_WithModifyAndList_ShouldModifyData()
        {
            // setup
            var state = new WorkflowState(new IonStruct.Initializer
            {
                ["items"] = new IonList.Initializer
                {
                    1,2,3,4,5
                },
                ["elements"] = new IonSexp.Initializer
                {
                    1,2,3,4,5
                }
            });

            var mutation = new StateMutation(
                Modify.Append("/items/[-1]", new IonTimestamp(DateTimeOffset.Now)),
                Modify.Replace("/items/[1]", new IonBool(false)),
                Modify.Replace("/items/[1000]", new IonBool(true)));

            var mutation2 = new StateMutation(
                Modify.Append("/elements/[-1]", new IonTimestamp(DateTimeOffset.Now)),
                Modify.Replace("/elements/[1]", new IonBool(false)),
                Modify.Replace("/elements/[1000]", new IonBool(true)));

            // test
            _ = state
                .Apply(mutation)
                .Apply(mutation2);

            // assert
            var list = (IonList)state.Data.Properties["items"];
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual(IonTypes.Bool, list.Items?[1].Type);
            Assert.AreEqual(IonTypes.Timestamp, list.Items?[^2].Type);

            var sexp = (IonSexp)state.Data.Properties["elements"];
            Assert.AreEqual(7, sexp.Count);
            Assert.AreEqual(IonTypes.Bool, sexp.Items?[1].Type);
            Assert.AreEqual(IonTypes.Timestamp, sexp.Items?[^2].Type);
        }

        [TestMethod]
        public void Apply_WithDeleteAndStruct_ShouldDeleteData()
        {
            // setup
            var state = new WorkflowState(new IonStruct.Initializer
            {
                ["root"] = new IonStruct.Initializer
                {
                    ["first"] = 3,
                    ["second"] = new IonList.Initializer
                    {
                        1,2,3,4,5
                    },
                    ["third"] = DateTimeOffset.Now,
                    ["fourth"] = "string theory",
                    ["fifth"] = false,
                    ["sixth"] = new IonStruct.Initializer
                    {
                        ["seventh"] = 43m
                    }
                }
            });

            var mutation = new StateMutation(
                new Delete("/root/sixth/seventh"),
                new Delete("/root/fifth"));

            // test
            _ = state.Apply(mutation);

            // assert
            var root = (IonStruct)state.Data.Properties["root"];
            var sixth = (IonStruct)root.Properties["sixth"];
            Assert.IsFalse(root.Properties.Contains("fifth"));
            Assert.IsFalse(sixth.Properties.Contains("seventh"));
        }

        [TestMethod]
        public void Apply_WithModifyAndStruct_ShouldDeleteData()
        {
            // setup
            var state = new WorkflowState(new IonStruct.Initializer
            {
                ["root"] = new IonStruct.Initializer
                {
                    ["first"] = 3,
                    ["second"] = new IonList.Initializer
                    {
                        1,2,3,4,5
                    },
                    ["third"] = DateTimeOffset.Now,
                    ["fourth"] = "string theory",
                    ["fifth"] = false,
                    ["sixth"] = new IonStruct.Initializer
                    {
                        ["seventh"] = 43m
                    }
                }
            });

            var mutation = new StateMutation(
                Modify.Append("/root/sixth/eight", new IonFloat(43d)),
                Modify.Replace("/root/fourth", new IonBool(true)));

            // test
            _ = state.Apply(mutation);

            // assert
            var root = (IonStruct)state.Data.Properties["root"];
            var sixth = (IonStruct)root.Properties["sixth"];
            Assert.AreEqual(IonTypes.Bool, root.Properties["fourth"].Type);
            Assert.AreEqual(IonTypes.Float, sixth.Properties["eight"].Type);
        }
    }
}
