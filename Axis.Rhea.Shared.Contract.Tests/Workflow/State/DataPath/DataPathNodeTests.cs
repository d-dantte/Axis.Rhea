using Axis.Ion.Types;
using Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

namespace Axis.Rhea.Shared.Contract.Tests.Workflow.State.DataPath
{
    [TestClass]
    public class DataPathNodeTests
    {
        [TestMethod]
        public void Parse_Tests()
        {
            var node = DataPathSegment.Parse("/abc/'def ghi'/[4]");
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void Select_Tests()
        {
            IonStruct ion = new IonStruct.Initializer
            {
                ["something"] = new IonStruct.Initializer
                {
                    ["random stuff"] = new IonStruct.Initializer
                    {
                        ["abc"] = "the result",
                        ["def"] = 54,
                        ["ghi"] = false,
                        ["jkl"] = new IonList.Initializer
                        {
                            "me", "you", "them"
                        }
                    }
                },
                ["otherthing"] = new IonList.Initializer
                {
                    45, 23, 5
                },
                ["things_that_will_be_left_out"] = "bleh, bleh",
                ["'mimmic me or...'"] = true
            };

            var path = DataPathSegment.Parse("/something/'random stuff'");
            var selection = path.Select(ion);
            Assert.IsNotNull(selection);
            Assert.AreEqual(IonTypes.Struct, selection.Value!.Type);

            path = DataPathSegment.Parse("/something/'random stuff'/jkl/[0]");
            selection = path.Select(ion);
            Assert.IsNotNull(selection);
            Assert.AreEqual(IonTypes.String, selection.Value!.Type);
            Assert.AreEqual("me", ((IonString)selection.Value).Value);

            path = DataPathSegment.Parse("/something/'random stuff'/jkl/[9]");
            selection = path.Select(ion);
            Assert.IsNull(selection.Value);

            path = DataPathSegment.Parse("/something/*/jkl/[9]?");
            selection = path.Select(ion);
            Assert.IsNull(selection.Value);

            path = DataPathSegment.Parse("/somethingx?/*/jkl/[9]");
            selection = path.Select(ion);
            Assert.IsNull(selection.Value);
        }
    }
}
