using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Core.Workflow.State;

namespace Axis.Rhea.Core.Tests.Workflow.State
{
    [TestClass]
    public class ExpressionTransformerTests
    {
        [TestMethod]
        public void TransformNowConstant_Tests()
        {
            var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("now");
            var result = recognizer.Recognize("now");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            var resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("DateTimeOffset.Now", resultNode.TokenValue());
        }


        [TestMethod]
        public void TransformVariableExpression_Tests()
        {
            var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("variable-expression");
            var result = recognizer.Recognize("#");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            var resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Key", resultNode.TokenValue());

            result = recognizer.Recognize("$");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value", resultNode.TokenValue());
        }


        [TestMethod]
        public void TransformTransformationExpression_Tests()
        {
            var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("transformation-expression");
            var result = recognizer.Recognize("$.to-lower");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            var resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value.ToLower()", resultNode.TokenValue());

            result = recognizer.Recognize("$.to-upper");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value.ToUpper()", resultNode.TokenValue());

            result = recognizer.Recognize("$.reverse");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value.Reverse()", resultNode.TokenValue());

            result = recognizer.Recognize("$.negation");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value.Negation()", resultNode.TokenValue());

            result = recognizer.Recognize("$.count");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value.Count()", resultNode.TokenValue());

            result = recognizer.Recognize("$.is-null");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value.IsNull()", resultNode.TokenValue());

            result = recognizer.Recognize("$.is-not-null");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value.IsNotNull()", resultNode.TokenValue());

            result = recognizer.Recognize("\"bleh\".count.to-upper");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("\"bleh\".Count().ToUpper()", resultNode.TokenValue());
        }

        [TestMethod]
        public void TransformRelationalExpression_Tests()
        {
            var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("relational-expression");
            var result = recognizer.Recognize("# > 7");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            var resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Key > 7", resultNode.TokenValue());

            result = recognizer.Recognize("$ < 7");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value < 7", resultNode.TokenValue());

            result = recognizer.Recognize("$ = 7");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value == 7", resultNode.TokenValue());

            result = recognizer.Recognize("$ != 7");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value != 7", resultNode.TokenValue());

            result = recognizer.Recognize("$ >= 7");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value >= 7", resultNode.TokenValue());

            result = recognizer.Recognize("$ <= 7");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("Value <= 7", resultNode.TokenValue());

            result = recognizer.Recognize("$ in (7, 8)");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).In(7, 8)", resultNode.TokenValue());

            result = recognizer.Recognize("$ not in (7, 8)");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).NotIn(7, 8)", resultNode.TokenValue());

            result = recognizer.Recognize("$ starts with \"abc\"");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).StartsWith(\"abc\")", resultNode.TokenValue());

            result = recognizer.Recognize("$ ends with \"abc\"");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).EndsWith(\"abc\")", resultNode.TokenValue());

            result = recognizer.Recognize("$ between 7..#");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).Between(7, Key)", resultNode.TokenValue());

            result = recognizer.Recognize("$ not between 7..14");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).NotBetween(7, 14)", resultNode.TokenValue());

            result = recognizer.Recognize("$ is Decimal");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).IsType(typeof(IonDecimal))", resultNode.TokenValue());

            result = recognizer.Recognize("$ is not Struct");
            success = result as SuccessResult;
            Assert.IsNotNull(success);
            resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).IsNotType(typeof(IonStruct))", resultNode.TokenValue());
        }


        [TestMethod]
        public void TransformLogicalNorExpression_Tests()
        {
            var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("logical-nor-expression");
            var result = recognizer.Recognize("$ ~~ 5 < #");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            var resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).Nor(5 < Key)", resultNode.TokenValue());
        }


        [TestMethod]
        public void TransformLogicalXorExpression_Tests()
        {
            var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("logical-xor-expression");
            var result = recognizer.Recognize("$ ^^ 5 < #");
            var success = result as SuccessResult;
            Assert.IsNotNull(success);
            var resultNode = ExpressionTransformer.TransformExpression(success.Symbol);
            Assert.AreEqual("(Value).Xor(5 < Key)", resultNode.TokenValue());
        }
    }
}
