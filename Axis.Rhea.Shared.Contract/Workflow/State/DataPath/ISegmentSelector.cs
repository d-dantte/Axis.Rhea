using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

/// <summary>
/// Interface that presents a property used in selecting elements from <see cref="IIonType"/>s.
/// </summary>
/// <typeparam name="TSelectorType"></typeparam>
public interface ISegmentSelector<TSelectorType>
{
    /// <summary>
    /// The selector value. See <typeparamref name="TSelectorType"/>
    /// </summary>
    TSelectorType Selector { get; }

    /// <summary>
    /// Indicates if a miss or fall-back selection is created when matches dont exist
    /// </summary>
    bool IsRequired { get; }
}
