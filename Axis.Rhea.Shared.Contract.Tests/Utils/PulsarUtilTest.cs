using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.Tests.Utils
{
    [TestClass]
    public class PulsarUtilTest
    {
        [TestMethod]
        public void GrammarImport_Test()
        {
            var dpGrammar = PulsarUtil.DataPathGrammar;
            var pcGrammar = PulsarUtil.PolicyTriggerConditionGrammar;
            var seGrammar = PulsarUtil.StateExpressionGrammar;
        }

        #region obsolete
        //[TestMethod]
        //public void TimestampConstant_Tests()
        //{
        //    // timestamp
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("timespan");
        //    var samples = new[] { "2.05:47", "05:12", "05:12:55", "05:12:55.1223", "2.05:12:55.1223" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //        Assert.IsTrue(TimeSpan.TryParse(sample, out _));
        //    }

        //    // date time
        //    recognizer = PulsarUtil.QueryGrammar.GetRecognizer("date-time");
        //    samples = new[] { "1997-04-20", "1997-04-20 23:15", "1997-04-20 23:15:22", "1997-04-20 23:15:22.443" };
        //    result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //        Assert.IsTrue(DateTimeOffset.TryParse(sample, out _));
        //    }
        //}


        //[TestMethod]
        //public void StringConstant_Tests()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("string-constant");
        //    var samples = new[] { @"""some string""", "\"some string with new line \\n\"" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}


        //[TestMethod]
        //public void Variable_Tests()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("variable-expression");
        //    var samples = new[] { "$", "#" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}


        //[TestMethod]
        //public void TransformationExpression_Tests()
        //{
        //    // transformation
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("transformation-expression");
        //    var samples = new[] { "$.to-lower", "#.to-lower .to-upper", "(4 * -2.008).count", "($ / 23.8).negation.is-null" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}


        //[TestMethod]
        //public void ArithmeticExpression_Tests()
        //{
        //    // arithmetic
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("arithmetic-expression");
        //    var samples = new[] { "2 * #", "($ - 6) / 5 + (1 % #)" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}


        //[TestMethod]
        //public void RelationalExpression_Tests()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("relational-expression");
        //    var samples = new[] { "# in (4, 5, 10)", "$ is Bool", "$ is not Decimal", "5 > $", "$ = 4", "$ not between 4..5" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}


        //[TestMethod]
        //public void LogicalExpression_Tests()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("logical-expression");
        //    var samples = new[] { "$ is Bool && ($ != # || now <= $)", "# in (4, 5, 10) && 5 != $", "(# = \"bleh\") && ($ is Struct)" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}


        //[TestMethod]
        //public void ValuePredicate()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("value-conditional");
        //    var samples = new[] { "value{$ < 5}", "${5 + ($ / 9) != 33}" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}


        //[TestMethod]
        //public void ListPredicate()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("list-conditional");
        //    var samples = new[] { "list{$ < 5}", "#{5 + ($ / 9) != 33}" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}

        //[TestMethod]
        //public void PropertyPredicate()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("property-conditional");
        //    var samples = new[] { "prop{$ < 5}", ".{5 + ($.count / 9) != 33}", "prop{(# = \"bleh\") && $ is Struct}" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}

        //[TestMethod]
        //public void AnnotationPredicate()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("annotation-conditional");
        //    var samples = new[] { "annot{$ < 5}", "@{5 + ($.count / 9\\}) != 33}" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}

        //[TestMethod]
        //public void PredicateNode()
        //{
        //    // string
        //    var x = PulsarUtil.QueryGrammar;
        //    var recognizer = x.GetRecognizer("query");
        //    var samples = new[] { "/annot{$ < 5} ${$ is Int}", "/@{ax.{4\\}} #{# between 3..12} /.{# = \"bleh\" && $ is Struct}" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}


        //[TestMethod]
        //public void Int_Exp()
        //{
        //    // string
        //    var recognizer = PulsarUtil.QueryGrammar.GetRecognizer("numeric-constant");
        //    var samples = new[] { "-3.45", "3.4", "3", "34", "323343", "-1", "-2343" };
        //    IRecognitionResult? result = null;
        //    foreach (var sample in samples)
        //    {
        //        result = recognizer.Recognize(sample);
        //        var success = result as SuccessResult;
        //        Assert.IsNotNull(success);
        //        Assert.AreEqual(sample, success.Symbol.TokenValue());
        //    }
        //}
        #endregion
    }
}
