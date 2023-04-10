using Microsoft.CodeAnalysis.Scripting;
using System.Threading.Tasks;

namespace Axis.Rhea.Core.Workflow.State
{
    internal static class Extensions
    {
        public static TOut Return<TOut>(this Task<ScriptState<TOut>> scriptTask)
        {
            return Task.Run(async () => (await scriptTask.ConfigureAwait(true)).ReturnValue).Result;
        }
    }
}
