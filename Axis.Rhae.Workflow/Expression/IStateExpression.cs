using Axis.Dia.Core.Types;

namespace Axis.Rhae.Workflow.Expression
{
    /// <summary>
    /// A predicate expression on the state data.
    /// </summary>
    public interface IStateExpression
    {
        bool IsMatch(Record stateData);
    }
}
