using Axis.Luna.Common.Results;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;
using static Axis.Rhea.Shared.Contract.StateExpressions.BinaryOperationGrouper;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Duration
{
    public record DurationArithmeticOperation :
        IBinaryOperation<IonDuration>,
        IResultParsable<DurationArithmeticOperation>
    {
        #region Grammar Symbols
        internal const string DurationExpressionSymbol = "duration-expression";
        internal const string DurationOperatorymbol = "duration-operator";
        internal const string DurationArithmeticExpressionSymbol = "duration-arithmetic-expression";
        #endregion

        public string Name => Operation.ToString();

        public ITypedExpression<IonDuration> Subject { get; }

        public ITypedExpression<IonDuration> Object { get; }

        IExpression IBinaryOperation<IonDuration>.Subject => Subject;

        IExpression IBinaryOperation<IonDuration>.Object => Object;


        public Operators Operation { get; }

        public DurationArithmeticOperation(
            ITypedExpression<IonDuration> subject,
            ITypedExpression<IonDuration> @object,
            Operators operation)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Object = @object ?? throw new ArgumentNullException(nameof(@object));

            Operation = operation.ThrowIf(
                enm => !Enum.IsDefined(enm),
                new ArgumentException($"Invalid Operations: {operation}"));
        }

        #region Evaluate
        public ITypedAtom<IonDuration> Evaluate()
        {
            return Operation switch
            {
                Operators.Add => Add(),
                Operators.Subtract => Subtract(),
                _ => throw new InvalidOperationException($"Invalid operator: {Operation}")
            };
        }

        IAtom IExpression.Evaluate() => Evaluate();

        #endregion

        #region Oprations
        private Atom<IonDuration> Add()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return subjectIon.IsNull || objectIon.IsNull
                ? new IonDuration()
                : subjectIon.Value!.Value + objectIon.Value!.Value;
        }

        private Atom<IonDuration> Subtract()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return subjectIon.IsNull || objectIon.IsNull
                ? new IonDuration()
                : subjectIon.Value!.Value - objectIon.Value!.Value;
        }
        #endregion

        #region Parse

        public static bool TryParse(
            string text,
            out IResult<DurationArithmeticOperation> result)
            => (result = Parse(text)) is IResult<DurationArithmeticOperation>.DataResult;

        public static bool TryParse(
            CSTNode temporalOperationNode,
            out IResult<DurationArithmeticOperation> result)
            => (result = Parse(temporalOperationNode)) is IResult<DurationArithmeticOperation>.DataResult;

        public static IResult<DurationArithmeticOperation> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(DurationArithmeticExpressionSymbol)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),

                FailureResult
                or ErrorResult => Result.Of<DurationArithmeticOperation>(new RecognitionException(parseResult)),

                _ => Result.Of<DurationArithmeticOperation>(
                    new InvalidOperationException($"Invalid recognition result: '{parseResult}'"))
            };
        }

        public static IResult<DurationArithmeticOperation> Parse(CSTNode durationArithmeticOpNode)
        {
            if (durationArithmeticOpNode is null)
                throw new ArgumentNullException(nameof(durationArithmeticOpNode));

            try
            {
                var opNodes = $"{DurationExpressionSymbol}|{DurationOperatorymbol}";
                var expressionItems = durationArithmeticOpNode
                    .FindNodes(opNodes)
                    .Select(node => node.SymbolName switch
                    {
                        DurationExpressionSymbol => new ExpressionItem(Expression.ParseDurationExpression(node).Resolve()),
                        DurationOperatorymbol => new OperatorItem(ToOperator(node)),
                        _ => new InvalidOperationException($"Unexpected symbol: {node.SymbolName}").Throw<OperationItem>()
                    });

                return GenerateOperation
                    <DurationArithmeticOperation, Operators, IonDuration>(
                    durationArithmeticOpNode,
                    expressionItems,
                    OperationProvider,
                    PrecedenceDirectionEvaluator);
            }
            catch (Exception ex)
            {
                return Result.Of<DurationArithmeticOperation>(ex);
            }
        }

        private static Operators ToOperator(CSTNode operatorNode)
        {
            return operatorNode.TokenValue() switch
            {
                "+" => Operators.Add,
                "-" => Operators.Subtract,
                _ => throw new InvalidOperationException($"Invalid duration operator symbol: '{operatorNode.TokenValue()}'")
            };
        }

        internal static PrecedenceDirection PrecedenceDirectionEvaluator(
            Enum @operator)
            => PrecedenceDirection.Ltr;

        internal static DurationArithmeticOperation OperationProvider(
            IExpression subject,
            IExpression @object,
            Enum @operation)
            => new DurationArithmeticOperation(
                (ITypedExpression<IonDuration>) subject,
                (ITypedExpression<IonDuration>)@object,
                (Operators)@operation);
        #endregion

        public enum Operators
        {
            Add,
            Subtract
        }
    }
}
