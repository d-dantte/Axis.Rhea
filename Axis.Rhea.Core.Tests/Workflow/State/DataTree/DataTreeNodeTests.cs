using Axis.Ion.Types;
using Axis.Rhea.Core.Exceptions;
using Axis.Rhea.Core.Workflow.State.DataTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Rhea.Core.Tests.Workflow.State.DataTree
{
    [TestClass]
    public class DataTreeNodeTests
    {

        [TestMethod]
        public void Parse_Tests()
        {
            var node = DataTreeNode.Parse(PathDef);
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
                ["things_that_will_be_left_out"] = "bleh, bleh"
            };

            var pruneDef = DataTreeNode.Parse(PruneDef);
            var v = pruneDef.Prune(ion);
            Assert.IsNotNull(v);

            pruneDef = DataTreeNode.Parse(PruneDef2);
            Assert.ThrowsException<MissingRequiredIndexException>(() => pruneDef.Prune(ion));
        }


        private static string PathDef = @"
- something
- another-thing
    - child1  # <-- this is the thing, and is a comment
    - child2
- listish
    - [1]?
    - [4]
    - [3]
        - list-child?
        - the-other-shild
    - [10]";

        private static string PruneDef = @"
- something
- otherthing
    - [2]
    - [8]?
";
        private static string PruneDef2 = @"
- something
- otherthing
    - [2]
    - [8]
";
    }
}
