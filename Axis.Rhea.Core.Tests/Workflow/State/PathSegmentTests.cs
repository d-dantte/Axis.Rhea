using Axis.Ion.Types;
using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Rhea.Core.Tests.Workflow.State
{
    [TestClass]
    public class PathSegmentTests
    {
        private static IonStruct Ion = new IonStruct.Initializer
        {
            ["FamilyTree"] = new IonStruct.Initializer
            {
                ["Id"] = 3,
                ["FirstName"] = "Kain",
                ["SurName"] = "FirstMan",
                ["Age"] = 29,
                ["Father"] = new IonStruct.Initializer
                {
                    ["Id"] = 1,
                    ["FirstName"] = "Adam",
                    ["SurName"] = "FirstMan",
                    ["Age"] = 54,
                    ["Father"] = new IonStruct.Initializer
                    {
                        ["Id"] = 0,
                        ["FirstName"] = "God",
                        ["SurName"] = "God",
                        ["Age"] = -1,
                    },
                },
                ["Mother"] = new IonStruct.Initializer
                {
                    ["Id"] = 2,
                    ["FirstName"] = "Eve",
                    ["SurName"] = "FirstWoman",
                    ["Age"] = 53,
                },
                ["Things"] = new IonList.Initializer
                {
                    5,6,7
                }
            }
        };

        [TestMethod]
        public void ParsePathSegment_Tests()
        {
            var query = "/abcd/efgh";
            var pathSegment = PathSegment.Parse(query);
            Assert.IsNotNull(pathSegment);
            Assert.IsNotNull(pathSegment.Next);
        }

        [TestMethod]
        public void Paths_ShouldReturnAllPathsStartingFromRoot_Tests()
        {
            var query = "/abcd/efgh/ijkl";
            var pathSegment = PathSegment.Parse(query);
            var paths = pathSegment.Paths().ToArray();
            Assert.AreEqual(3, paths.Length);
            Assert.AreEqual(paths[0], pathSegment);
            Assert.AreEqual(paths[1], pathSegment.Next);
            Assert.AreEqual(paths[2], pathSegment.Next.Next);
            Assert.IsNull(pathSegment.Next.Next.Next);
        }

        [TestMethod]
        public void Select_ShouldSelectValuesMatchingQuery()
        {
            var query = "/FamilyTree";
            var pathSegment = PathSegment.Parse(query);

            // kain
            var data = pathSegment
                .Select(Ion)
                .Cast<PropertyPathSelection>()
                .ToArray();
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Length);
            var kain = (IonStruct)data[0].Value;
            Assert.IsTrue(kain.Properties.Contains("Id"));
            Assert.AreEqual(new IonInt(3), kain.Properties["Id"]);

            // adam
            query = "/FamilyTree/Father";
            pathSegment = PathSegment.Parse(query);
            data = pathSegment
                .Select(Ion)
                .Cast<PropertyPathSelection>()
                .ToArray();
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Length);
            var adam = (IonStruct)data[0].Value;
            Assert.IsTrue(kain.Properties.Contains("FirstName"));
            Assert.AreEqual(new IonString("Adam"), adam.Properties["FirstName"]);

            // adam & eve
            query = "/FamilyTree/.{# ends with \"ther\"}";
            pathSegment = PathSegment.Parse(query);
            data = pathSegment
                .Select(Ion)
                .Cast<PropertyPathSelection>()
                .ToArray();
            Assert.IsNotNull(data);
            Assert.AreEqual(2, data.Length);
            var firstNames = data.Select(selection => selection.Value)
                .Cast<IonStruct>()
                .Select(s => s.Properties["FirstName"].ToIonText())
                .JoinUsing(" & ");
            Assert.AreEqual("Adam & Eve", firstNames);
        }

        [TestMethod]
        public void Pick_ShouldPruneNonMatchingData()
        {
            var query = "/FamilyTree/Father/FirstName";
            var pathSegment = PathSegment.Parse(query);

            // kain
            var data = pathSegment
                .Pick(Ion)
                .Cast<PropertyPathSelection>()
                .ToArray();
            Assert.IsNotNull(data);
        }
    }
}
