using Axis.Ion.Types;
using Axis.Rhea.Core.Workflow.State;

namespace Axis.Rhea.Core.Tests.Workflow.State
{
    [TestClass]
    public class ListPredicateTests
    {
        [TestMethod]
        public void MiscTest()
        {
            var predicate = ListPredicate.Parse("#{# < 1}");
            var property = (0, new IonInt(3));
            var result = predicate.Execute(property);
            Assert.IsTrue(result);

            predicate = ListPredicate.Parse("#{(# < 1) && $ < 10}");
            property = (0, new IonInt(3));
            result = predicate.Execute(property);
            Assert.IsTrue(result);
        }
    }
}
