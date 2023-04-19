using Axis.Ion.Types;
using Axis.Rhea.Core.Workflow.State;

namespace Axis.Rhea.Core.Tests.Workflow.State
{
    [TestClass]
    public class AnnotationPredicateTests
    {
        [TestMethod]
        public void MiscTest()
        {
            var predicate = AnnotationPredicate.Parse("@{abcd.{2\\}}");
            var annotation = IIonType.Annotation.Parse("abcdef");
            var result = predicate.Execute(annotation);
            Assert.IsTrue(result);
        }
    }
}
