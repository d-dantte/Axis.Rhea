using Axis.Rhea.Shared.Contract.Directives.PolicyTriggers;

namespace Axis.Rhea.Shared.Contract.Tests.Directives.PolicyTriggers
{
    [TestClass]
    public class PolicyTriggerParserTests
    {
        [TestMethod]
        public void ParseTest()
        {
            var x = PolicyTriggerParser.Parse("($, [@Completed, !#544])");
        }
    }
}
