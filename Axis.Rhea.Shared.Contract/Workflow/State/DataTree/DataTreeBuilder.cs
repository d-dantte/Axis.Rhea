using Axis.Rhea.Shared.Contract.Workflow.State.DataPath;

namespace Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

public sealed class DataTreeBuilder
{
    private readonly NodeBuilder _rootNode = new NodeBuilder();

    private DataTreeBuilder()
    {
    }

    public static DataTreeBuilder Create() => new DataTreeBuilder();

    public DataTreeBuilder Combine(DataPathSegment path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        var treeNode = path
            .Segments()
            .Reverse()
            .Aggregate(default(NodeBuilder), (builder, segment) =>
            {
                var node = segment switch
                {
                    ItemSegment item => new NodeBuilder(item.Selector).WithRequired(item.IsRequired),
                    PropertySegment property => new NodeBuilder(property.Selector).WithRequired(property.IsRequired),
                    _ => throw new InvalidOperationException($"Invalid {nameof(segment)} type: '{segment?.GetType()}'")
                };

                return builder is not null
                    ? node.WithChild(builder)
                    : node;
            });

        _rootNode.WithChild(treeNode!);
        return this;
    }

    public RootNode Build() => (RootNode)_rootNode.Build();
}
