using Axis.Ion.Types;
using Axis.Rhea.Shared.Contract.Workflow.State;

namespace Axis.Rhea.Shared.Contract.StateExpressions
{
    /// <summary>
    /// Create a context to use in evaluating the expressions.
    /// Essentially, this uses <see cref="AsyncLocal{T}"/> to store the <see cref="Workflow.State.WorkflowState"/>, making it
    /// available for all expressions within the evaluation context.
    /// </summary>
    public static class EvaluationContext
    {
        #region AsyncLocal
        private static readonly AsyncLocal<WorkflowState?> AsyncContext = new();

        /// <summary>
        /// Gets of sets the AsyncLocal instance.
        /// </summary>
        public static WorkflowState? AsyncLocal
        {
            get => AsyncContext.Value;
            set => AsyncContext.Value = value;
        }
        #endregion

        public static IAtom EvaluateWithState(this IExpression expression, WorkflowState workflowState)
        {
            try
            {
                AsyncLocal = workflowState;
                return expression.Evaluate();
            }
            finally
            {
                AsyncLocal = null;
            }
        }

        public static ITypedAtom<TType> EvaluateWithState<TType>(this
            ITypedExpression<TType> expression,
            WorkflowState workflowState)
            where TType : struct, IIonType
            => (ITypedAtom<TType>)EvaluateWithState((IExpression)expression, workflowState);
    }
}
