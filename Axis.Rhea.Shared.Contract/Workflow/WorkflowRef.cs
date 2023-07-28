using Axis.Luna.Extensions;

namespace Axis.Rhea.Shared.Contract.Workflow;

/// <summary>
/// A structure used to refer to a workflow in a request to start a new instance of the referenced workflow.
/// <para>
/// Using this ref defaults to either the <see cref="WellKnownWorkflowExecutionContexts.Live"/> context, or the most recent version
/// of the workflow.
/// </para>
/// </summary>
public record WorkflowRef
{
    /// <summary>
    /// Qualified name of the workflow
    /// </summary>
    public string QualifiedName { get; }

    public WorkflowRef(string qname)
    {
        QualifiedName = qname.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentException($"Invalid {nameof(qname)}: '{qname}'"));
    }
}

/// <summary>
/// Specifies a version of the workflow to execute
/// </summary>
public record VersionRef : WorkflowRef
{
    /// <summary>
    /// The version of the workflow
    /// </summary>
    public Version Version { get; }

    protected VersionRef(string qname, Version version)
    : base(qname)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
    }
}

/// <summary>
/// Specifies a context of the workflow to execute
/// </summary>
public record ContextRef : WorkflowRef
{
    /// <summary>
    /// The version of the workflow
    /// </summary>
    public string Context { get; }

    protected ContextRef(string qname, string context)
    : base(qname)
    {
        Context = context.ThrowIf(
            string.IsNullOrWhiteSpace,
            new ArgumentNullException(nameof(context)));
    }
}
