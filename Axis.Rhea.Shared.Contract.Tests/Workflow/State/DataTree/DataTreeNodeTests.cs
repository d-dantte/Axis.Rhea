using Axis.Ion.Types;
using Axis.Rhea.Shared.Contract.Exceptions;
using Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

namespace Axis.Rhea.Shared.Contract.Tests.Workflow.State.DataTree
{
    [TestClass]
    public class DataTreeNodeTests
    {

        [TestMethod]
        public void Parse_Tests()
        {
            var node = RootNode.Parse(PathDef);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void Prune_Tests()
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
                ["things_that_will_be_left_out"] = "bleh, bleh",
                ["'mimmic me or...'"] = true
            };

            var pruneDef = RootNode.Parse(PruneDef);
            var v = pruneDef.Prune(ion);
            Assert.IsNotNull(v);

            pruneDef = RootNode.Parse(PruneDef2);
            Assert.ThrowsException<MissingRequiredIndexException>(() => pruneDef.Prune(ion));
        }


        private static string PathDef = @"
/something/'another-thing'/child1
/something/'another-thing'/child2
listish/[1]?/[4]/[3]/'list-child'?
listish/[1]?/[4]/[3]/'the-other-shild'
listish/[1]?/[4]/[10]";

        private static string PruneDef = @"
/something
/otherthing/[2]
/otherthing/[8]?
";
        private static string PruneDef2 = @"
/something
/otherthing/[2]
/otherthing/[8]";
    }
}
