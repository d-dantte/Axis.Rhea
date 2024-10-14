using Axis.Rhae.Contract;

namespace Axis.Rhae.Workflow.Runtime.Activity
{
    /// <summary>
    /// Each activity will usitlize a state object to keep track of any state that exists for it's lifetime. That state
    /// Will be an implementation of this contract, specific to the activity, and designed to be persisted easily.
    /// </summary>
    public interface IActivityState
    {
        /// <summary>
        /// The identifier of the owner activity
        /// </summary>
        Identifier<Contract.Workflow.Identifiers.Activity> ActivityIdentifier { get; }
    }
}
