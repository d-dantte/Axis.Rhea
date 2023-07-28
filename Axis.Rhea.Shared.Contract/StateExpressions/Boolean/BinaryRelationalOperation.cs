using Axis.Ion.Numerics;
using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.StateExpressions.Numeric;
using Axis.Rhea.Shared.Contract.Utils;
using static Axis.Rhea.Shared.Contract.StateExpressions.BinaryOperationGrouper;
using System.Text.RegularExpressions;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Boolean
{
    public record BinaryRelationalOperation :
        IBinaryOperation<IonBool>,
        IResultParsable<BinaryRelationalOperation>
    {
        #region Grammar Symbols
        internal const string RelationalExpressionSymbol = "binary-relational-expression";
        internal const string ExpressionSymbol = "expression";
        internal const string RelationalOperatorSymbol = "relational-operator";
        #endregion

        private static Dictionary<Operators, string> OperatorSymbols = new()
        {
            [Operators.GreaterThan] = ">",
            [Operators.LessThan] = "<",
            [Operators.GreaterOrEqual] = ">=",
            [Operators.LessOrEqual] = "<=",
            [Operators.Equal] = "=",
            [Operators.NotEqual] = "!="
        };

        public string Name => Operation.ToString();

        public Operators Operation { get; }

        public IExpression Subject { get; }

        public IExpression Object { get; }

        public BinaryRelationalOperation(IExpression subject, IExpression @object, Operators operation)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Object = @object ?? throw new ArgumentNullException(nameof(@object));
            Operation = operation.ThrowIf(
                enm => !Enum.IsDefined(enm),
                new ArgumentException($"Invalid Operations: {operation}"));

            if (!CanCompare(Subject, Object))
                throw new ArgumentException(
                    $"Cannot compare the given types. "
                    + $"lhs: {subject.GetType()}, rhs: {Object.GetType()}");
        }

        public Atom<IonBool> Evaluate()
        {
            var lhs = Subject.Evaluate().Ion;
            var rhs = Object.Evaluate().Ion;

            var compare = Compare(lhs, rhs);

            if (compare is null)
                return new IonBool();

            return Operation switch
            {
                Operators.Equal => new IonBool(compare == 0),
                Operators.NotEqual => new IonBool(compare != 0),
                Operators.GreaterThan => new IonBool(compare > 0),
                Operators.LessThan => new IonBool(compare < 0),
                Operators.GreaterOrEqual => new IonBool(compare >= 0),
                Operators.LessOrEqual => new IonBool(compare <= 0),

                _ => throw new InvalidOperationException($"Invalid Operation: {Operation}")
            };
        }

        public bool EvaluateComparison()
        {
            return Evaluate().Ion.Value == true;
        }

        private static int? Compare(IIonType lhs, IIonType rhs)
        {
            if (lhs.IsNull || rhs.IsNull)
                return null;

            return (lhs, rhs) switch
            {
                (null, _) => null,
                (_, null) => null,
                (INumericType nlhs, INumericType nrhs) => nlhs.ToBigDecimal()!.Value.CompareTo(nrhs.ToBigDecimal()!.Value),
                (IonBool blhs, IonBool brhs) => blhs.Value!.Value.CompareTo(brhs.Value!.Value),
                (IonTimestamp blhs, IonTimestamp brhs) => blhs.Value!.Value.CompareTo(brhs.Value!.Value),
                (IonDuration blhs, IonDuration brhs) => blhs.Value!.Value.CompareTo(brhs.Value!.Value),
                (IonString blhs, IonString brhs) => blhs.Value!.CompareTo(brhs.Value!),
                (IIonTextSymbol blhs, IIonTextSymbol brhs) => blhs.Value!.CompareTo(brhs.Value!),
                _ => throw new ArgumentException($"Cannot compare the given types: lhs: {lhs.Type}, rhs: {rhs.Type}")
            };
        }

        private static bool CanCompare(IExpression lhs, IExpression rhs)
        {
            return (lhs, rhs) switch
            {
                (null, _) => false,
                (_, null) => false,
                (ITypedExpression<IonBool>, ITypedExpression<IonBool>) => true,
                (ITypedExpression<IonDuration>, ITypedExpression<IonDuration>) => true,
                (ITypedExpression<IonTimestamp>, ITypedExpression<IonTimestamp>) => true,
                (ITypedExpression<IonString>, ITypedExpression<IonString>) => true,
                _ => IsNumericExpression(lhs) && IsNumericExpression(rhs),
            };
        }

        #region expression
        ITypedAtom<IonBool> ITypedExpression<IonBool>.Evaluate() => Evaluate();

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse

        public static bool TryParse(
            string text,
            out IResult<BinaryRelationalOperation> result)
            => (result = Parse(text)) is IResult<BinaryRelationalOperation>.DataResult;

        public static bool TryParse(
            CSTNode arithmeticExpressionNode,
            out IResult<BinaryRelationalOperation> result)
            => (result = Parse(arithmeticExpressionNode)) is IResult<BinaryRelationalOperation>.DataResult;


        public static IResult<BinaryRelationalOperation> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(RelationalExpressionSymbol)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),

                FailureResult
                or ErrorResult => Result.Of<BinaryRelationalOperation>(new RecognitionException(parseResult)),

                _ => Result.Of<BinaryRelationalOperation>(
                    new InvalidOperationException($"Invalid recognition result: '{parseResult}'"))
            };
        }

        public static IResult<BinaryRelationalOperation> Parse(CSTNode relationalExpressionNode)
        {
            if (relationalExpressionNode is null)
                throw new ArgumentNullException(nameof(relationalExpressionNode));

            try
            {
                var opNodes = $"{ExpressionSymbol}|{RelationalOperatorSymbol}";
                var expressionItems = relationalExpressionNode
                    .FindNodes(opNodes)
                    .ToArray();

                var exp1 = Expression.Parse(expressionItems[0]);
                var @operator = ToOperator(expressionItems[1].TokenValue());
                return Expression
                    .Parse(expressionItems[2])
                    .Combine(exp1, (xp2, xp1) => new BinaryRelationalOperation(xp1, xp2, @operator));
            }
            catch (Exception ex)
            {
                return Result.Of<BinaryRelationalOperation>(ex);
            }
        }
        #endregion

        private static Operators ToOperator(string symbol)
        {
            return symbol switch
            {
                "=" => Operators.Equal,
                "!=" => Operators.NotEqual,
                ">=" => Operators.GreaterOrEqual,
                ">" => Operators.GreaterThan,
                "<=" => Operators.LessOrEqual,
                "<" => Operators.LessThan,
                _ => throw new ArgumentException($"Invalid symbol: {symbol}")
            };
        }

        public override string ToString()
        {
            return $"({Subject} {OperatorSymbols[Operation]} {Object})";
        }

        private static bool IsNumericExpression(IExpression expression)
        {
            return expression switch
            {
                ITypedExpression<IonNumber>
                or ITypedExpression<IonInt>
                or ITypedExpression<IonFloat>
                or ITypedExpression<IonDecimal> => true,
                _ => false
            };
        }

        public enum Operators
        {
            Equal,
            NotEqual,
            GreaterThan,
            LessThan,
            GreaterOrEqual,
            LessOrEqual
        }
    }
}
