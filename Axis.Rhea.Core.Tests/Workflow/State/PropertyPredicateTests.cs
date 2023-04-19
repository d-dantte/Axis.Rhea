using Axis.Ion.Types;
using Axis.Rhea.Core.Workflow.State;

namespace Axis.Rhea.Core.Tests.Workflow.State
{
    [TestClass]
    public class PropertyPredicateTests
    {
        [TestMethod]
        public void MiscTest()
        {
            var predicate = PropertyPredicate.Parse(".{# starts with \"abc\"}");
            var property = new IonStruct.Property("abcdef", new IonInt(3));
            var result = predicate.Execute(property);
            Assert.IsTrue(result);

            predicate = PropertyPredicate.Parse(".{(# starts with \"abc\") && $ < 10}");
            property = new IonStruct.Property("abcdef", new IonInt(3));
            result = predicate.Execute(property);
            Assert.IsTrue(result);
        }
    }
}
