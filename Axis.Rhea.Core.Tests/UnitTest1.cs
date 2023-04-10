using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Axis.Rhea.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var option = ScriptOptions.Default
                .WithReferences(typeof(DateTimeOffset).Assembly)
                .WithImports(typeof(DateTimeOffset).Namespace);

            var script = CSharpScript.Create("DateTimeOffset.Now", option);
            var diag = script.Compile();
            var result = script.RunAsync().Result.ReturnValue;

        }
    }
}