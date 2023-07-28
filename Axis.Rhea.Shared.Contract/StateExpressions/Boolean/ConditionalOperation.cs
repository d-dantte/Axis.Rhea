using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Duration;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;
using static Axis.Rhea.Shared.Contract.StateExpressions.BinaryOperationGrouper;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Boolean
{
    public record ConditionalOperation :
        IBinaryOperation<IonBool>,
        IResultParsable<ConditionalOperation>
    {
        #region Grammar Symbols
        internal const string ConditionalExpressionSymbol = "conditional-expression";
        internal const string ConditionalArgSymbol = "conditional-arg";
        internal const string ConditionalOperatorSymbol = "conditional-operator";
        internal const string BoolExpressionSymbol = "bool-expression";
        internal const string XorExpressionSymbol = "xor-expression";
        internal const string ExpressionSymbol = "expression";
        #endregion


        private static Dictionary<Operators, string> OperatorSymbols = new()
        {
            [Operators.And] = "&",
            [Operators.Or] = "|",
            [Operators.Nor] = "~",
            [Operators.Xor] = "^"
        };

        public string Name => Operation.ToString();

        public Operators Operation { get; }

        public ITypedExpression<IonBool> Subject { get; }

        public ITypedExpression<IonBool> Object { get; }

        IExpression IBinaryOperation<IonBool>.Subject => Subject;

        IExpression IBinaryOperation<IonBool>.Object => Object;

        public ConditionalOperation(ITypedExpression<IonBool> subject, ITypedExpression<IonBool> @object, Operators operation)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Object = @object ?? throw new ArgumentNullException(nameof(@object));
            Operation = operation.ThrowIf(
                enm => !Enum.IsDefined(enm),
                new ArgumentException($"Invalid Operations: {operation}"));
        }

        #region Evaluate
        ITypedAtom<IonBool> ITypedExpression<IonBool>.Evaluate() => Evaluate();

        IAtom IExpression.Evaluate() => Evaluate();

        public bool EvaluateCondition()
        {
            return Evaluate().Ion.Value == true;
        }

        public Atom<IonBool> Evaluate()
        {
            var lhs = Subject.Evaluate().Ion.As<IonBool>();
            var rhs = Object.Evaluate().Ion.As<IonBool>();

            return Operation switch
            {
                Operators.And => lhs.IsNull || rhs.IsNull
                    ? IonBool.Null()
                    : new IonBool(lhs.Value!.Value && rhs.Value!.Value),

                Operators.Or => lhs.IsNull || rhs.IsNull
                    ? IonBool.Null()
                    : new IonBool(lhs.Value!.Value || rhs.Value!.Value),

                Operators.Xor => lhs.IsNull || rhs.IsNull
                    ? IonBool.Null()
                    : new IonBool(lhs.Value!.Value ^ rhs.Value!.Value),

                Operators.Nor => lhs.IsNull || rhs.IsNull
                    ? IonBool.Null()
                    : new IonBool(!lhs.Value!.Value && !rhs.Value!.Value),

                _ => throw new InvalidOperationException($"Invalid Operation: {Operation}")
            };
        }
        #endregion

        #region Parse

        public static bool TryParse(
            string text,
            out IResult<ConditionalOperation> result)
            => (result = Parse(text)) is IResult<ConditionalOperation>.DataResult;

        public static bool TryParse(
            CSTNode conditionalExpressionNode,
            out IResult<ConditionalOperation> result)
            => (result = Parse(conditionalExpressionNode)) is IResult<ConditionalOperation>.DataResult;

        public static IResult<ConditionalOperation> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(ConditionalExpressionSymbol)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),

                FailureResult
                or ErrorResult => Result.Of<ConditionalOperation>(new RecognitionException(parseResult)),

                _ => Result.Of<ConditionalOperation>(
                    new InvalidOperationException($"Invalid recognition result: '{parseResult}'"))
            };
        }

        public static IResult<ConditionalOperation> Parse(CSTNode conditionalExpressionNode)
        {
            if (conditionalExpressionNode is null)
                throw new ArgumentNullException(nameof(conditionalExpressionNode));

            try
            {
                var opNodes = $"{ConditionalArgSymbol}|{ConditionalOperatorSymbol}";
                var expressionItems = conditionalExpressionNode
                    .FindNodes(opNodes)
                    .Select(node => node.SymbolName switch
                    {
                        ConditionalArgSymbol => node.FirstNode().SymbolName switch
                        {
                            XorExpressionSymbol => ToXorGroup(node),
                            BoolExpressionSymbol => new ExpressionItem(Expression.Parse(node).Resolve()),
                            _ => new InvalidOperationException($"Unexpected symbol: {node.SymbolName}").Throw<OperationItem>()
                        },
                        ConditionalOperatorSymbol => new OperatorItem(ToOperator(node.TokenValue())),
                        _ => new InvalidOperationException($"Unexpected symbol: {node.SymbolName}").Throw<OperationItem>()
                    });

                return GenerateOperation
                    <ConditionalOperation, Operators, IonBool>(
                    conditionalExpressionNode,
                    expressionItems,
                    OperationProvider,
                    PrecedenceDirectionEvaluator);
            }
            catch (Exception e)
            {
                return Result.Of<ConditionalOperation>(e);
            }
        }

        private static GroupItem ToXorGroup(
            CSTNode xorExpressionNode)
            => xorExpressionNode
                .FindNodes(ExpressionSymbol)
                .Select(node => new ExpressionItem(Expression.Parse(node).Resolve()))
                .Aggregate(new GroupItem(Operators.Xor), (group, item) => group.AddItem(item));

        internal static PrecedenceDirection PrecedenceDirectionEvaluator(
            Enum @operator)
            => PrecedenceDirection.Ltr;

        internal static ConditionalOperation OperationProvider(
            IExpression subject,
            IExpression @object,
            Enum @operation)
            => new ConditionalOperation(
                (ITypedExpression<IonBool>)subject,
                (ITypedExpression<IonBool>)@object,
                (Operators)@operation);

        #endregion

        public override string ToString()
        {
            return $"({Subject} {OperatorSymbols[Operation]} {Object})";
        }

        private static Operators ToOperator(string symbol)
        {
            return symbol switch
            {
                "&" => Operators.And,
                "|" => Operators.Or,
                "^" => Operators.Xor,
                "~" => Operators.Nor,
                _ => throw new ArgumentException($"Invalid symbol: {symbol}")
            };
        }

        public enum Operators
        {
            And,
            Or,
            Xor,
            Nor
        }
    }
}
