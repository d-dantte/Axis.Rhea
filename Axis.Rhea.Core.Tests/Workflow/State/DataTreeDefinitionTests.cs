using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Core.Workflow.State;

namespace Axis.Rhea.Core.Tests.Workflow.State
{
    [TestClass]
    public class DataTreeDefinitionTests
    {
        [TestMethod]
        public void Integer_Tests()
        {
            var recognizer = PulsarUtil.DataTreeGrammar.GetRecognizer("integer");

            var result = recognizer.Recognize("1");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("1", success.Symbol.TokenValue());

            result = recognizer.Recognize("12");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("12", success.Symbol.TokenValue());

            result = recognizer.Recognize("123");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("123", success.Symbol.TokenValue());

            result = recognizer.Recognize("-123");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("-123", success.Symbol.TokenValue());
        }

        [TestMethod]
        public void Index_Tests()
        {
            var recognizer = PulsarUtil.DataTreeGrammar.GetRecognizer("index");

            var result = recognizer.Recognize("[1]");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("[1]", success.Symbol.TokenValue());

            result = recognizer.Recognize("[12]");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("[12]", success.Symbol.TokenValue());

            result = recognizer.Recognize("[123]");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("[123]", success.Symbol.TokenValue());
        }

        [TestMethod]
        public void Property_Tests()
        {
            var recognizer = PulsarUtil.DataTreeGrammar.GetRecognizer("property");

            var result = recognizer.Recognize("stuff");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("stuff", success.Symbol.TokenValue());

            result = recognizer.Recognize("$tuffer_stuffest");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("$tuffer_stuffest", success.Symbol.TokenValue());

            result = recognizer.Recognize("'tuff.something-else/another-thing'");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("'tuff.something-else/another-thing'", success.Symbol.TokenValue());
        }

        [TestMethod]
        public void PathNode_Tests()
        {
            var recognizer = PulsarUtil.DataTreeGrammar.GetRecognizer("path-node");

            var result = recognizer.Recognize("- stuff");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("- stuff", success.Symbol.TokenValue());

            result = recognizer.Recognize("- [44]");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("- [44]", success.Symbol.TokenValue());

            result = recognizer.Recognize("    - [44]");    
            success = result as SuccessResult;  
            Assert.IsNotNull(success);  
            Assert.AreEqual("    - [44]", success.Symbol.TokenValue());

            result = recognizer.Recognize("    \t- stuff");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("    \t- stuff", success.Symbol.TokenValue());
        }

        [TestMethod]
        public void CommentLine_Tests()
        {
            var recognizer = PulsarUtil.DataTreeGrammar.GetRecognizer("comment-line");

            var result = recognizer.Recognize("#\n");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("#\n", success.Symbol.TokenValue());

            result = recognizer.Recognize("# dfaldkfalfk\n");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("# dfaldkfalfk\n", success.Symbol.TokenValue());

            result = recognizer.Recognize("    \n");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("    \n", success.Symbol.TokenValue());

            result = recognizer.Recognize("    # some comment\n");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("    # some comment\n", success.Symbol.TokenValue());
        }

        [TestMethod]
        public void PathDefinition_Tests()
        {
            var recognizer = PulsarUtil.DataTreeGrammar.GetRecognizer("path-definition");

            var result = recognizer.Recognize(PathDef);
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual(PathDef, success.Symbol.TokenValue());

            success.Symbol
                .FindNodes("path-node")
                .ForAll(node =>
                {
                    var depth = node.FindNodes("depth.tab").Count();
                    var optional = node.FindNode("optional");
                    Console.WriteLine($"For '{node.TokenValue()}', depth is '{depth}', and optional is '{(optional?.TokenValue() ?? ("null"))}'");
                });
        }

        private static string PathDef = @"
- something
- another_thing
    - child1  # <-- this is the thing, and is a comment
    - child2
- listish
    - [1]?
    - [4]
    - [3]
        - 'list-child'?
        - 'the-other-shild'
    - [10]";
    }
}
