using Axis.Luna.Extensions;
using Axis.Rhea.Shared.Contract.Workflow.State;
using System.Text.RegularExpressions;

namespace Axis.Rhea.Shared.Contract.Workflow;

/// <summary>
/// TODO: how do we support sub-graphs?
/// </summary>
public record WorkflowDefinition
{
    internal static readonly Regex QualifiedNamePattern = new($"^(?'{nameof(_namespace)}'[a-zA-Z]\\w*(\\.[a-zA-Z]\\w*)*)\\.(?'{nameof(_name)}'[a-zA-Z]\\w*)$", RegexOptions.Compiled);

    private readonly string _namespace;
    private readonly string _name;

    /// <summary>
    /// Name for the workflow. This is unique across workflows in the system, and is assigned by the workflow owner
    /// <para>
    /// It consists of a <c>namespace</c> part, and a <c>name</c> part.
    /// <code>
    /// Abc.Xyz.A12.WorkflowName
    /// </code>
    /// From the above, 'Abc.Xyz.A12' is the namespace, while 'WorkflowName' is the workflow name.
    /// </para>
    /// </summary>
    public string QualifiedName => $"{_namespace}.{_name}";

    /// <summary>
    /// version of the workflow
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// Unique identifier across the entire system, assigned and managed by the system.
    /// This could very well be a GUID.
    /// </summary>
    public Guid SystemId { get; }

    /// <summary>
    /// A contextual-label given to a version of a workflow. Labels are unique across workflow versions.
    /// <para>
    /// To execute a workflow, only the name is given, along with an optional ExecutionContext. Essentially, the
    /// ExecutionContext helps select the version of the workflow to run. If no label is specified, the "live" context is assumed.
    /// If a label is specified and not found, an error is thrown
    /// </para>
    /// </summary>
    public string[] ExecutionContexts { get; }

    /// <summary>
    /// Abcd.Xyz.Workflow::f80d5fbf-0077-4bf2-9d3a-30f5f237de9f@2.3.340
    /// </summary>
    public string FullyQualifiedName => $"{QualifiedName}::{SystemId}@{Version}";

    /// <summary>
    /// The state schema for the workflow
    /// </summary>
    public StateSchema Schema { get; }

    /// <summary>
    /// The activity definition
    /// </summary>
    public ActivityDefinition ActivityDefinition { get; }

    /// <summary>
    /// The directive invocation definition
    /// </summary>
    public DirectiveInvocationDefintion DirectiveInvocationDefinition { get; }

    public WorkflowDefinition(
        string qualifiedName,
        string[] executionContexts,
        Guid systemId,
        Version version,
        StateSchema stateSchema,
        ActivityDefinition activityDefinition,
        DirectiveInvocationDefintion directiveInvocationDefinition)
    {
        Version = version;
        SystemId = systemId;
        Schema = stateSchema.ThrowIfNull(new ArgumentNullException(nameof(stateSchema)));
        ActivityDefinition = activityDefinition.ThrowIfNull(new ArgumentNullException(nameof(activityDefinition)));
        DirectiveInvocationDefinition = directiveInvocationDefinition.ThrowIfNull(new ArgumentNullException(nameof(directiveInvocationDefinition)));
        ExecutionContexts = executionContexts
            .ThrowIfNull(new ArgumentNullException(nameof(executionContexts)))
            .ThrowIfAny(string.IsNullOrWhiteSpace, new ArgumentException($"{nameof(executionContexts)} cannot contain null values"))
            .ToArray();

        var match = QualifiedNamePattern.Match(qualifiedName);
        if (!match.Success)
            throw new FormatException($"Invalid {nameof(qualifiedName)}: '{qualifiedName}'");

        _namespace = match.Groups[nameof(_namespace)].Value;
        _name = match.Groups[nameof(_name)].Value;
    }
}
