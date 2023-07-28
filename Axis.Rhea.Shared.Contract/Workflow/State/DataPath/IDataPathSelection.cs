using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

/// <summary>
/// 
/// </summary>
public interface IDataPathSelection
{
    /// <summary>
    /// 
    /// </summary>
    IIonType Container { get; }

    /// <summary>
    /// 
    /// </summary>
    IIonType? Value { get; }

    /// <summary>
    /// 
    /// </summary>
    MatchType MatchType { get; }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TSelection"></typeparam>
/// <typeparam name="TContainer"></typeparam>
/// <typeparam name="TSelector"></typeparam>
public interface ISelectionCreator<TSelection, TContainer, TSelector>
where TSelection: ISelectionCreator<TSelection, TContainer, TSelector>
{
    /// <summary>
    /// When a match is found, create a selection having <see cref="IDataPathSelection.MatchType"/> = <see cref="MatchType.Hit"/>
    /// </summary>
    /// <param name="container">The container</param>
    /// <param name="selector">The selector</param>
    /// <param name="value">The value</param>
    /// <returns>The Hit Selection</returns>
    static abstract TSelection Hit(TContainer container, TSelector selector, IIonType value);

    /// <summary>
    /// When a match not is found, and <see cref="ISegmentSelector{TSelectorType}.IsRequired"/> is false,
    /// create a selection having <see cref="IDataPathSelection.MatchType"/> = <see cref="MatchType.FallBack"/>
    /// </summary>
    /// <param name="container">The container</param>
    /// <param name="selector">The selector</param>
    /// <returns>The Fallback Selection</returns>
    static abstract TSelection Fallback(TContainer container, TSelector selector);

    /// <summary>
    /// When a match not is found, and <see cref="ISegmentSelector{TSelectorType}.IsRequired"/> is true,
    /// create a selection having <see cref="IDataPathSelection.MatchType"/> = <see cref="MatchType.Miss"/>
    /// </summary>
    /// <param name="container">The container</param>
    /// <param name="selector">The selector</param>
    /// <returns>The Miss Selection</returns>
    static abstract TSelection Miss(TContainer container, TSelector selector);
}
