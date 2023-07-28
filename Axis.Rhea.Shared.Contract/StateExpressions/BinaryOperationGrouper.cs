using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.StateExpressions
{
    internal static class BinaryOperationGrouper
    {
        internal static IEnumerable<OperationItem> Group(
            IEnumerable<OperationItem> items,
            Enum @operator)
        {
            var groupingState = new GenericState<GroupingStateData>("Grouping", ReadAndGroup);
            var notGroupingState = new GenericState<GroupingStateData>("NotGrouping", ReadIndividually);
            using var itemsEnumerator = items.GetEnumerator();
            var stateData = new GroupingStateData(itemsEnumerator, @operator);
            var machine = new StateMachine<GroupingStateData>(
                startState: "NotGrouping",
                stateData: stateData,
                states: new[] { groupingState, notGroupingState });

            while (machine.TryAct()) ;

            return stateData.GroupedItems;
        }


        internal static IResult<TOperation> GenerateOperation
            <TOperation, TOperators, TIonType>(
            CSTNode operationNode,
            IEnumerable<OperationItem> expressionItems,
            Func<IExpression, IExpression, Enum, IBinaryOperation<TIonType>> operationProvider,
            Func<Enum, PrecedenceDirection> precedenceDirectionEvaluator)
            where TOperators: struct, Enum
            where TOperation: class, IBinaryOperation<TIonType>
            where TIonType : struct, IIonType
        {
            return Enum
                .GetValues<TOperators>()
                .OrderByDescending(op => op)
                .Select(op => (Enum)op)
                .Aggregate(expressionItems, Group)
                .Select(opItem => opItem.As<GroupItem>())
                .Select(groupItem => groupItem.ToGroupedOperation(
                    operationProvider,
                    precedenceDirectionEvaluator))
                .Cast<TOperation>()
                .FirstOrOptional()
                .AsResult(new InvalidOperationException(
                    $"The '{typeof(TOperation)}' expression could not be grouped: "
                    + $"'{operationNode.TokenValue()}'"));
        }

        private static string? ReadAndGroup(GroupingStateData stateData)
        {
            while (stateData.Enumerator.MoveNext())
            {

                if (stateData.Enumerator.Current is OperatorItem opItem)
                {
                    if (stateData.GroupingOperator.Equals(opItem.Operator))
                        return "Grouping";

                    // else
                    stateData.GroupedItems.Add(opItem);
                    return "NotGrouping";
                }

                // else
                var group = (GroupItem)stateData.GroupedItems[^1];
                group.AddItem(stateData.Enumerator.Current);
                return "Grouping";
            }

            return null;
        }

        private static string? ReadIndividually(GroupingStateData stateData)
        {
            while (stateData.Enumerator.MoveNext())
            {
                if (stateData.Enumerator.Current is OperatorItem opItem
                    && stateData.GroupingOperator.Equals(opItem.Operator))
                {
                    var sub = stateData.GroupedItems[^1];
                    stateData.GroupedItems.RemoveAt(stateData.GroupedItems.Count - 1);
                    stateData.GroupedItems.Add(new GroupItem(opItem.Operator).AddItem(sub));
                    return "Grouping";
                }

                // else
                stateData.GroupedItems.Add(stateData.Enumerator.Current);
                return "NotGrouping";
            }

            return null;
        }

        #region Nested types
        public abstract record OperationItem
        {
        }

        internal record ExpressionItem : OperationItem
        {
            public IExpression Expression { get; }

            public ExpressionItem(IExpression expression)
            {
                Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            }
        }

        internal record OperatorItem : OperationItem
        {
            public Enum Operator { get; }

            public OperatorItem(Enum @operator)
            {
                Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
            }
        }

        internal record GroupItem : OperationItem
        {
            private readonly List<OperationItem> group = new List<OperationItem>();

            public ImmutableArray<OperationItem> Items => group.ToImmutableArray();

            public Enum Operator { get; }

            public GroupItem(Enum @operator)
            {
                Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
            }

            public GroupItem AddItem(OperationItem item)
            {
                group.Add(item);
                return this;
            }

            public IBinaryOperation<TIonType> ToGroupedOperation<TIonType>(
                Func<IExpression, IExpression, Enum, IBinaryOperation<TIonType>> operationProvider,
                Func<Enum, PrecedenceDirection> precedenceEvaluator)
                where TIonType : struct, IIonType
            {
                if (operationProvider is null)
                    throw new ArgumentNullException(nameof(operationProvider));

                if (precedenceEvaluator is null)
                    throw new ArgumentNullException(nameof(precedenceEvaluator));

                if (group.Count < 2)
                    throw new InvalidOperationException($"This group contains < 2 items");

                var direction = precedenceEvaluator.Invoke(Operator);
                return direction switch
                {
                    PrecedenceDirection.Ltr => ToLtrGroupedOperation(operationProvider, precedenceEvaluator),
                    PrecedenceDirection.Rtl => ToRtlGroupedOperation(operationProvider, precedenceEvaluator),
                    _ => throw new InvalidOperationException($"Invalid prcedence direction: '{direction}'")
                };
            }

            private IBinaryOperation<TIonType> ToLtrGroupedOperation<TIonType>(
                Func<IExpression, IExpression, Enum, IBinaryOperation<TIonType>> operationProvider,
                Func<Enum, PrecedenceDirection> precedenceEvaluator)
                where TIonType : struct, IIonType
            {
                return group
                    .Select(item =>
                        item is ExpressionItem ei ? ei.Expression :
                        item is GroupItem gi ? gi.ToGroupedOperation(operationProvider, precedenceEvaluator) :
                        throw new InvalidOperationException($"Invalid item type: '{item?.GetType()}'"))
                    .Aggregate(default(IBinaryOperation<TIonType>), (op, exp) => op switch
                    {
                        null => new PlaceHolderOperation<TIonType>(exp!),
                        PlaceHolderOperation<TIonType> ph => operationProvider.Invoke(ph.Subject, exp!, Operator),
                        _ => operationProvider.Invoke(op, exp!, Operator)
                    })!;
            }

            private IBinaryOperation<TIonType> ToRtlGroupedOperation<TIonType>(
                Func<IExpression, IExpression, Enum, IBinaryOperation<TIonType>> operationProvider,
                Func<Enum, PrecedenceDirection> precedenceEvaluator)
                where TIonType : struct, IIonType
            {
                return group
                    .Reverse<OperationItem>()
                    .Select(item =>
                        item is ExpressionItem ei ? ei.Expression :
                        item is GroupItem gi ? gi.ToGroupedOperation(operationProvider, precedenceEvaluator) :
                        throw new InvalidOperationException($"Invalid item type: '{item?.GetType()}'"))
                    .Aggregate(default(IBinaryOperation<TIonType>), (op, exp) => op switch
                    {
                        null => new PlaceHolderOperation<TIonType>(exp!),
                        PlaceHolderOperation<TIonType> ph => operationProvider.Invoke(exp!, ph.Subject, Operator),
                        _ => operationProvider.Invoke(exp!, op, Operator)
                    })!;
            }
        }

        internal record GroupingStateData
        {
            public IEnumerator<OperationItem> Enumerator { get; }

            public List<OperationItem> GroupedItems { get; }

            public Enum GroupingOperator { get; }

            public GroupingStateData(
                IEnumerator<OperationItem> itemEnumerator,
                Enum @operator)
            {
                GroupedItems = new List<OperationItem>();
                GroupingOperator = @operator ?? throw new ArgumentNullException(nameof(@operator));
                Enumerator = itemEnumerator ?? throw new ArgumentNullException(nameof(itemEnumerator));
            }
        }

        public enum PrecedenceDirection
        {
            Ltr,
            Rtl
        }

        internal class PlaceHolderOperation<T> : IBinaryOperation<T>
        where T : struct, IIonType
        {
            public string Name => "PlaceHolder";

            public IExpression Subject { get; }

            public IExpression Object => throw new NotImplementedException();

            public ITypedAtom<T> Evaluate() => throw new NotImplementedException();

            IAtom IExpression.Evaluate() => Evaluate();

            public PlaceHolderOperation(IExpression subject)
            {
                Subject = subject;
            }
        }
        #endregion
    }
}
