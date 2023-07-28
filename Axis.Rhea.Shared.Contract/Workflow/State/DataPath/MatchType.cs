namespace Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

public enum MatchType
{
    /// <summary>
    /// When a match is found
    /// </summary>
    Hit,

    /// <summary>
    /// When a match is not found, and the <see cref="ISegmentSelector{TSelectorType}"/> is Required
    /// </summary>
    Miss,

    /// <summary>
    /// When a match is not found, and the <see cref="ISegmentSelector{TSelectorType}"/> is not Required, fall back to the parent selection
    /// </summary>
    FallBack
}
