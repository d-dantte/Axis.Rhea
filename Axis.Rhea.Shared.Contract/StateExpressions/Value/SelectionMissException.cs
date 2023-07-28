using Axis.Luna.Extensions;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Value
{
    /// <summary>
    /// Thrown when a <see cref="Workflow.State.DataPath.DataPathSegment"/> is used to select a value 
    /// from the state, but doesn't return a <see cref="Workflow.State.DataPath.MatchType.Hit"/>
    /// </summary>
    public class SelectionMissException: Exception
    {
        public string Path { get; }

        public SelectionMissException(string path)
        : base($"The selection '{path}' yielded no results")
        {
            Path = path.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(path)}: '{path}'"));
        }
    }
}
