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
    public class AnnotationPredicateTests
    {
        [TestMethod]
        public void MiscTest()
        {
            var predicate = AnnotationPredicate.Parse("@{abcd.{2\\}}");
            var annotation = IIonType.Annotation.Parse("abcdef");
            var result = predicate.Execute(annotation);
            result = predicate.Execute(annotation);
            Assert.IsTrue(result);
        }
    }
}
