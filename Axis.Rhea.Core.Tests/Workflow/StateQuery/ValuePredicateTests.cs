using Axis.Ion.Types;
using Axis.Rhea.Core.Workflow.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Rhea.Core.Tests.Workflow.StateQuery
{
    [TestClass]
    public class ValuePredicateTests
    {
        [TestMethod]
        public void MiscTest()
        {

            var predicate = ValuePredicate.Parse("${($ matches \"abcd.{2}\") && ((4 + 5) > $.count)}");
            var value = new IonString("abcdef");
            var result = predicate.Execute(value);
            Assert.IsTrue(result);
        }
    }
}
