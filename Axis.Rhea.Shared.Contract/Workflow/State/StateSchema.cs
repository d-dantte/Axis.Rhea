using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Rhea.Shared.Contract.Workflow.State.DataTree;

namespace Axis.Rhea.Shared.Contract.Workflow.State
{
    /// <summary>
    /// A schema that defines the shape of the initial state of a workflow.
    /// Each time a workflow instance is created, the initial state is validated against the schema
    /// </summary>
    public record StateSchema
    {
        public RootNode Schema { get; }

        public StateSchema(RootNode node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            Schema = node;
        }

        /// <summary>
        /// Match the shape of the state against the schema
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsValidState(IonStruct state) => Validate(state) is IResult<IonStruct>.DataResult;

        /// <summary>
        /// This is essentially the same as pruning, the only difference being rather than creating new values when 
        /// a match is encuntered, we return the same value.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IResult<IonStruct> Validate(IonStruct state)
        {
            try
            {
                return IsValid(Schema, state)
                    ? Result.Of(state)
                    : throw new Exception("State does not fit schema");
            }
            catch(Exception ex)
            {
                return Result.Of<IonStruct>(ex);
            }
        }

        private static bool IsValid(DataTreeNode node, IIonType ion)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            if (ion is null)
                throw new ArgumentNullException(nameof(ion));

            return (node.Homogeneity, ion) switch
            {
                (NodeHomogeneity.Empty, _) => true,
                (NodeHomogeneity.Items, IonList list) => IsListValid(node, list),
                (NodeHomogeneity.Items, IonSexp sexp) => IsSexpValid(node, sexp),
                (NodeHomogeneity.Properties, IonStruct @struct) => IsStructValid(node, @struct),
                (NodeHomogeneity.NonHomogeneous, _) => throw new ArgumentException($"Invalid homogeneity: {NodeHomogeneity.NonHomogeneous}"),
                _ => throw new InvalidOperationException($"Invalid argument combination")
            };
        }

        private static bool IsListValid(DataTreeNode node, IonList ion)
        {
            if (ion.IsNull)
                return false;

            if (node.SelectsAll())
            {
                if (ion.Count == 0)
                    return false;

                var allNode = node.Children.First();
                return ion.Value!.All(value => IsValid(allNode, value));
            }

            return node.Children
                .SelectAs<ItemNode>()
                .All(itemNode =>
                    !itemNode.IsRequired
                    ||(itemNode.Index >= 0
                    && itemNode.Index < ion.Count
                    && IsValid(itemNode, ion.Value![itemNode.Index!.Value])));
        }

        private static bool IsSexpValid(DataTreeNode node, IonSexp ion)
        {
            if (ion.IsNull)
                return false;

            if (node.SelectsAll())
            {
                if (ion.Count == 0)
                    return false;

                var allNode = node.Children.First();
                return ion.Value!.All(value => IsValid(allNode, value));
            }

            return node.Children
                .SelectAs<ItemNode>()
                .All(itemNode =>
                    !itemNode.IsRequired
                    || (itemNode.Index >= 0
                    && itemNode.Index < ion.Count
                    && IsValid(itemNode, ion.Value![itemNode.Index!.Value])));
        }

        private static bool IsStructValid(DataTreeNode node, IonStruct ion)
        {
            if (ion.IsNull)
                return false;

            if (node.SelectsAll())
            {
                if (ion.Value!.Length == 0)
                    return false;

                var allNode = node.Children.First();
                return ion.Value!.All(value => IsValid(allNode, value.Value));
            }

            return node.Children
                .SelectAs<PropertyNode>()
                .All(propertyNode =>
                    !propertyNode.IsRequired
                    ||(ion.Properties.TryGetvalue(propertyNode.Property!, out var value)
                    && IsValid(propertyNode, value!)));
        }
    }
}
