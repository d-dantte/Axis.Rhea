using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Timestamp
{
    public record TimestampArithmeticOperation :
        IBinaryOperation<IonTimestamp>,
        IResultParsable<TimestampArithmeticOperation>
    {
        #region Grammar Symbols
        internal const string DurationExpressionSymbol = "duration-expression";
        internal const string TimestampExpressionSymbol = "timestamp-expression";
        internal const string TimestampOperatorymbol = "timestamp-operator";
        internal const string TimestampArithmeticExpressionSymbol = "timestamp-arithmetic-expression";
        #endregion

        public string Name => Operation.ToString();

        public ITypedExpression<IonTimestamp> Subject { get; }

        public ITypedExpression<IonDuration> Object { get; }

        IExpression IBinaryOperation<IonTimestamp>.Subject => Subject;

        IExpression IBinaryOperation<IonTimestamp>.Object => Object;


        public Operators Operation { get; }

        public TimestampArithmeticOperation(
            ITypedExpression<IonTimestamp> subject,
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
        public ITypedAtom<IonTimestamp> Evaluate()
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
        private Atom<IonTimestamp> Add()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return subjectIon.IsNull || objectIon.IsNull
                ? new IonTimestamp()
                : subjectIon.Value!.Value + objectIon.Value!.Value;
        }

        private Atom<IonTimestamp> Subtract()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return subjectIon.IsNull || objectIon.IsNull
                ? new IonTimestamp()
                : subjectIon.Value!.Value - objectIon.Value!.Value;
        }
        #endregion

        #region Parse

        public static bool TryParse(
            string text,
            out IResult<TimestampArithmeticOperation> result)
            => (result = Parse(text)) is IResult<TimestampArithmeticOperation>.DataResult;

        public static bool TryParse(
            CSTNode temporalOperationNode,
            out IResult<TimestampArithmeticOperation> result)
            => (result = Parse(temporalOperationNode)) is IResult<TimestampArithmeticOperation>.DataResult;

        public static IResult<TimestampArithmeticOperation> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(TimestampArithmeticExpressionSymbol)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),

                FailureResult
                or ErrorResult => Result.Of<TimestampArithmeticOperation>(new RecognitionException(parseResult)),

                _ => Result.Of<TimestampArithmeticOperation>(
                    new InvalidOperationException($"Invalid recognition result: '{parseResult}'"))
            };
        }

        public static IResult<TimestampArithmeticOperation> Parse(CSTNode timestampOperationNode)
        {
            try
            {
                var subjectExpression = timestampOperationNode
                    .FindNode(TimestampExpressionSymbol)
                    .ApplyTo(Expression.Parse)
                    .Map(ex => (ITypedExpression<IonTimestamp>)ex)
                    .Resolve();

                var objectExpression = timestampOperationNode
                    .FindNode(DurationExpressionSymbol)
                    .ApplyTo(Expression.Parse)
                    .Map(ex => (ITypedExpression<IonDuration>)ex)
                    .Resolve();

                var @operator = timestampOperationNode
                    .FindNode(TimestampOperatorymbol)
                    .ApplyTo(ToOperator);

                return Result.Of(
                    new TimestampArithmeticOperation(
                        subjectExpression,
                        objectExpression,
                        @operator));
            }
            catch(Exception ex)
            {
                return Result.Of<TimestampArithmeticOperation>(ex);
            }
        }

        private static Operators ToOperator(CSTNode operatorNode)
        {
            return operatorNode.TokenValue() switch
            {
                "+" => Operators.Add,
                "-" => Operators.Subtract,
                _ => throw new InvalidOperationException($"Invalid timestamp operator symbol: '{operatorNode.TokenValue()}'")
            };
        }
        #endregion


        public enum Operators
        {
            Add,
            Subtract
        }
    }
}
