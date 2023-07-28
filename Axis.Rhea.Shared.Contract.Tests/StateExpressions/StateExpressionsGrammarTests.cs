using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.Tests.StateExpressions
{
    [TestClass]
    public class StateExpressionsGrammarTests
    {
        #region Boolean Expressions
        [TestMethod]
        public void BoolStateSelectionTests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("bool-state-selection-expression");

            var text = "bool::/some/path/selection?/[4]/here";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void BoolStateSelection_TypeCheck_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("state-selection-typecheck-expression");

            var text = "$bool::/some/path/selection?/[4]/here";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void BoolStateSelection_Existence_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("state-selection-existence-expression");

            var text = "?bool::/some/path/selection?/[4]/here";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void StringMatchExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("string-match-expression");

            #region matches
            var text = "(string::/some/path/selection?/[4]/here !! \"some regex pattern\")";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "(string::/some/path/selection?/[4]/here   matches \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region starts with
            text = "(string::/some/path/selection?/[4]/here !< \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "(string::/some/path/selection?/[4]/here   starts-with \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region ends with
            text = "(string::/some/path/selection?/[4]/here >! \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "(string::/some/path/selection?/[4]/here   ends-with \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region ends with
            text = "(string::/some/path/selection?/[4]/here >< \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "(string::/some/path/selection?/[4]/here   contains \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion
        }

        [TestMethod]
        public void BinaryRelationalExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("binary-relational-expression");

            #region greater than
            var text = "(string::/some/path/selection?/[4]/here > \"some regex pattern\")";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region greater or equal
            text = "(string::/some/path/selection?/[4]/here >= 56)";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region less than
            text = "(string::/some/path/selection?/[4]/here < 6.7)";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region less or equal
            text = "(string::/some/path/selection?/[4]/here <= \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region equal
            text = "(string::/some/path/selection?/[4]/here = bool::/other/path)"; //<-- semantic error picked up by parsing stage
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "(string::/some/path/selection?/[4]/here != \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region not equal
            text = "(string::/some/path/selection?/[4]/here != \"some regex pattern\")";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion
        }

        [TestMethod]
        public void ParametarizedRelationalExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("parametarized-relational-expression");

            #region in
            var text = "(string::/some/path/selection?/[4]/here in (\"some regex pattern\"))";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region not in
            text = "(\"something\" not-in (string::/bleh, string::/'else where', \"...\"))";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region between
            text = "(string::/some/path/selection?/[4]/here between (string::/a ,  bool::/b ))"; //<- semantic error picked up by parser
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion

            #region not between
            text = "(string::/some/path/selection?/[4]/here not-between (string::/a ,  string::/b ))";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
            #endregion
        }

        [TestMethod]
        public void ConditionalExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("conditional-expression");

            var text = "(?string::/some/path & $string::/some/path & true | (?int::/id ^ ?int::/long_id) ~ bool::/has_kids)";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void NegationExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("negation-expression");

            var text = "!?string::/some/path";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "!(int::/age > 45)";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        #endregion

        #region String Expressions

        [TestMethod]
        public void StringConstantExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("constant-string-expression");

            var text = "\"string literal. bla bla bla.\"";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void StringStateSelectionExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("string-state-selection-expression");

            var text = "string::/path/to/data";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void StringConcatenationExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("string-concatenation-expression");

            var text = "string::/path/to/data + string::/other/path + \"literal string\"";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        #endregion

        #region Arithmetic Expression

        [TestMethod]
        public void ConstantArithmeticExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("constant-numeric-expression");

            var text = "1";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "+123";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "-123";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "123.65";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "123.65E12";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "123.65E-23";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            text = "123.65E+23";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void NumericStateSelectionExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("int-state-selection-expression");
            var text = "int::/path/to/data";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("float-state-selection-expression");
            text = "real::/path/to/data";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());

            recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("decimal-state-selection-expression");
            text = "decimal::/path/to/data";
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void ArithmeticExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("arithmetic-expression");
            var text = "(int::/path/to/data ** 65.4E05 + real::/package/weight / 7 - 9 % real::/zero)";
            var result = recognizer.Recognize(text);
            result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        #endregion

        #region Temporal Expression

        [TestMethod]
        public void ConstantTimestampExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("constant-timestamp-expression");

            var text = "'T now'";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void ConstantTimespanExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("duration-expression");

            var text = "'D 3.12:34'";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        [TestMethod]
        public void TemporalStateSelectionExpression_Tests()
        {
            var recognizer = PulsarUtil.StateExpressionGrammar.GetRecognizer("timestamp-state-selection-expression");
            var text = "time-stamp::/path/to/data";
            var result = recognizer.Recognize(text);
            Assert.IsTrue(result is SuccessResult);
            var success = result as SuccessResult;
            Assert.AreEqual(text, success!.Symbol.TokenValue());
        }

        #endregion
    }
}
