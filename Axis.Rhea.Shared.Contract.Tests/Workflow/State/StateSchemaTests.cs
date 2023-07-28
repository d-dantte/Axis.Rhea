using Axis.Ion.Types;
using Axis.Rhea.Shared.Contract.Workflow.State;
using Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

namespace Axis.Rhea.Shared.Contract.Tests.Workflow.State
{
    [TestClass]
    public class StateSchemaTests
    {
        [TestMethod]
        public void IsValid_WIthValidDataAndSchema_ShouldReturnTrue()
        {
            IonStruct ion = new IonStruct.Initializer
            {
                ["something"] = new IonStruct.Initializer
                {
                    ["randomStuff"] = "the result"
                },
                ["otherthing"] = new IonList.Initializer
                {
                    45, 23, 5
                },
                ["sexpthing"] = new IonSexp.Initializer
                {
                    45, 23
                },
                ["left_out"] = "bleh, bleh",
                ["'mimmic me or...'"] = true
            };

            var rootNode = RootNode.Parse(
                "/something/randomStuff\n"
                + "/something/missing?\n"
                + "/otherthing/[0]\n"
                + "/otherthing/[1]\n"
                + "/otherthing/[2]\n"
                + "/otherthing/[3]?\n"
                + "/sexpthing/[*]\n"
                + "/left_out\n");

            var schema = new StateSchema(rootNode);
            Assert.IsTrue(schema.IsValidState(ion));
        }

        [TestMethod]
        public void IsValid_WithUnmatchedSchema_ShouldReturnFalse()
        {
            IonStruct ion = new IonStruct.Initializer
            {
                ["something"] = new IonStruct.Initializer
                {
                    ["randomStuff"] = "the result"
                },
                ["otherthing"] = new IonList.Initializer
                {
                    45, 23, 5
                },
                ["sexpthing"] = new IonSexp.Initializer
                {
                    45, 23
                },
                ["left_out"] = "bleh, bleh",
                ["'mimmic me or...'"] = true
            };

            var rootNode = RootNode.Parse(
                "/something/randomStuff\n"
                + "/otherthing/[0]\n"
                + "/otherthing/[1]\n"
                + "/otherthing/[2]\n"
                + "/otherthing/[3]\n"
                + "/sexpthing/[*]\n"
                + "/left_out\n");

            var schema = new StateSchema(rootNode);
            Assert.IsFalse(schema.IsValidState(ion));

            rootNode = RootNode.Parse(
                "/something/randomStuff_ex\n"
                + "/otherthing/[0]\n"
                + "/sexpthing/[*]\n"
                + "/left_out\n");

            schema = new StateSchema(rootNode);
            Assert.IsFalse(schema.IsValidState(ion));
        }
    }
}
