using Axis.Luna.Extensions;

namespace Axis.Rhea.Shared.Contract.Workflow.Activities;

/// <summary>
/// Represents starting a separate workflow from within another workflow. The newly started workflow is given an alias
/// that is used to reference the started workflow when querying it's status, or other details about it, from the parent
/// workflow
/// </summary>
public class WorkflowInvocation : IActivity
{
    public string Name { get; }

    /// <summary>
    /// An alias, unique across the workflow instance that is being executed, and used to identify
    /// the workflow that is being invoked.
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Fully Qualified ID of the workflow to invoke
    /// </summary>
    public string WorkflowFQId { get; }

    /// <summary>
    /// The selector for the workflow
    /// </summary>
    public WorkflowSelector Selector { get; }

    /// <summary>
    /// The activity to transition to if the workflow was successfully started
    /// </summary>
    public string NextActivity { get; }

    public WorkflowInvocation(
        string name,
        string workflowFQId,
        string alias,
        string nextActivity,
        WorkflowSelector instanceSelector)
    {
        Name = name.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(name)}: '{name}'"));

        NextActivity = nextActivity.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(nextActivity)}: '{nextActivity}'"));

        Alias = alias.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(alias)}: '{alias}'"));

        WorkflowFQId = workflowFQId.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(workflowFQId)}: '{workflowFQId}'"));

        Selector = instanceSelector ?? throw new ArgumentNullException(nameof(instanceSelector));
    }

    #region Nested types

    /// <summary>
    /// Extract this class to it's own file
    /// </summary>
    public abstract record WorkflowSelector
    {
        public static VersionSelector Of(Version version) => new VersionSelector(version);

        public static ExecutionContextSelector Of(string? executionContext) => new ExecutionContextSelector(executionContext);
    }

    public record VersionSelector : WorkflowSelector
    {
        public Version Version { get; }

        internal VersionSelector(Version version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
    }

    public record ExecutionContextSelector : WorkflowSelector
    {
        public string ExecutionContext { get; }

        internal ExecutionContextSelector(string? context)
        {
            ExecutionContext =
                (context ?? WellKnownWorkflowExecutionContexts.Live)
                .ThrowIf(
                    string.IsNullOrWhiteSpace,
                    new ArgumentException($"Invalid {nameof(context)}"));
        }
    }
    #endregion
}
