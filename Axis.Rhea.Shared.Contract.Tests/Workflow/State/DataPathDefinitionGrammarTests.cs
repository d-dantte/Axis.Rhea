using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.Tests.Workflow.State
{
    [TestClass]
    public class DataPathDefinitionGrammarTests
    {
        [TestMethod]
        public void Integer_Tests()
        {
            var recognizer = PulsarUtil.DataPathGrammar.GetRecognizer("integer");

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
            var recognizer = PulsarUtil.DataPathGrammar.GetRecognizer("index");

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
            var recognizer = PulsarUtil.DataPathGrammar.GetRecognizer("property");

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
        public void PathSegment_Tests()
        {
            var recognizer = PulsarUtil.DataPathGrammar.GetRecognizer("path-segment");

            var result = recognizer.Recognize("/stuff");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("/stuff", success.Symbol.TokenValue());

            result = recognizer.Recognize("/$tuffer_stuffest");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("/$tuffer_stuffest", success.Symbol.TokenValue());

            result = recognizer.Recognize("/'meta physical 123'");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("/'meta physical 123'", success.Symbol.TokenValue());

            result = recognizer.Recognize("/[21]");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("/[21]", success.Symbol.TokenValue());
        }

        [TestMethod]
        public void Path_Tests()
        {
            var recognizer = PulsarUtil.DataPathGrammar.GetRecognizer("path");

            var result = recognizer.Recognize("/stuff/'tuffer_stuffest'/[2]");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            Assert.AreEqual("/stuff/'tuffer_stuffest'/[2]", success.Symbol.TokenValue());
        }

        [TestMethod]
        public void Paths_Tests()
        {
            var recognizer = PulsarUtil.DataPathGrammar.GetRecognizer("paths");

            var result = recognizer.Recognize(@"
/stuff/'tuffer_stuffest'/[2]
/otherTHing/me/[0]/'them'
/*/*/[1]?");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            var paths = success.Symbol.FindNodes("path").ToArray();
            Assert.AreEqual(3, paths.Length);
        }
    }
}
