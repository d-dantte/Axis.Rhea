namespace Axis.Rhea.Core.Workflow.State
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPathPredicate
    {
        /// <summary>
        /// The text form of the predicate expression. 
        /// </summary>
        string PredicateExpression { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPathPredicate<T>: IPathPredicate
    {
        /// <summary>
        /// Executes the underlying predicate, given the contextual value
        /// </summary>
        /// <param name="value">The contextual value</param>
        bool Execute(T value);
    }
}
