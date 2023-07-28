using Axis.Ion.Numerics;
using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;
using static Axis.Rhea.Shared.Contract.StateExpressions.BinaryOperationGrouper;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Numeric
{
    public record ArithmeticOperation :
        IBinaryOperation<IonNumber>,
        IResultParsable<ArithmeticOperation>
    {
        #region Grammar Symbols
        internal const string ArithmeticExpressionSymbol = "arithmetic-expression";
        internal const string NumericExpressionSymbol = "numeric-expression";
        internal const string ArithmeticOperatorSymbol = "arithmetic-operator";
        #endregion

        private static Dictionary<Operators, string> OperatorSymbols = new()
        {
            [Operators.Add] = "+",
            [Operators.Subtract] = "-",
            [Operators.Multiply] = "*",
            [Operators.Divide] = "/",
            [Operators.Modulus] = "%",
            [Operators.Power] = "**",
        };

        public string Name => Operation.ToString();

        public Operators Operation { get; }

        public IExpression Subject { get; }

        public IExpression Object { get; }

        /// <summary>
        /// TODO: change subject and object types to match: <see cref="INumericExpression"/>
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="object"></param>
        /// <param name="operation"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ArithmeticOperation(IExpression subject, IExpression @object, Operators operation)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Object = @object ?? throw new ArgumentNullException(nameof(@object));
            Operation = operation.ThrowIf(
                enm => !Enum.IsDefined(enm),
                new ArgumentException($"Invalid Operations: {operation}"));
        }

        #region Evaluate
        ITypedAtom<IonNumber> ITypedExpression<IonNumber>.Evaluate() => Evaluate();

        IAtom IExpression.Evaluate() => Evaluate();

        public BigDecimal EvaluateArithmetic()
        {
            return Evaluate().Ion.Value ?? throw new InvalidOperationException("Evaluation yielded null");
        }

        public Atom<IonNumber> Evaluate()
        {
            return Operation switch
            {
                Operators.Add => Add(),
                Operators.Multiply => Multiply(),
                Operators.Subtract => Subtract(),
                Operators.Divide => Divide(),
                Operators.Modulus => Modulus(),
                Operators.Power => Power(),
                _ => throw new InvalidOperationException($"Invalid operation: {Operation}")
            };
        }
        #endregion

        #region Oprations
        internal Atom<IonNumber> Add()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return (subjectIon, objectIon) switch
            {
                (INumericType nsubject, INumericType nobject) => ((IIonType)nsubject).IsNull || ((IIonType)nobject).IsNull
                    ? new IonNumber()
                    : nsubject.ToBigDecimal() + nobject.ToBigDecimal(),

                _ => throw new InvalidOperationException(
                    $"Cannot perform arithmetic operation '{Operation}' on"
                    + $"non arithmetic types. subject: '{subjectIon.Type}', object: '{objectIon.Type}'")
            };
        }

        internal Atom<IonNumber> Multiply()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return (subjectIon, objectIon) switch
            {
                (INumericType nsubject, INumericType nobject) => ((IIonType)nsubject).IsNull || ((IIonType)nobject).IsNull
                    ? new IonNumber()
                    : nsubject.ToBigDecimal() * nobject.ToBigDecimal(),

                _ => throw new InvalidOperationException(
                    $"Cannot perform arithmetic operation '{Operation}' on"
                    + $"non arithmetic types. subject: '{subjectIon.Type}', object: '{objectIon.Type}'")
            };
        }

        internal Atom<IonNumber> Subtract()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return (subjectIon, objectIon) switch
            {
                (INumericType nsubject, INumericType nobject) => ((IIonType)nsubject).IsNull || ((IIonType)nobject).IsNull
                    ? new IonNumber()
                    : nsubject.ToBigDecimal() - nobject.ToBigDecimal(),

                _ => throw new InvalidOperationException(
                    $"Cannot perform arithmetic operation '{Operation}' on"
                    + $"non arithmetic types. subject: '{subjectIon.Type}', object: '{objectIon.Type}'")
            };
        }

        internal Atom<IonNumber> Divide()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return (subjectIon, objectIon) switch
            {
                (INumericType nsubject, INumericType nobject) => ((IIonType)nsubject).IsNull || ((IIonType)nobject).IsNull
                    ? new IonNumber()
                    : nsubject.ToBigDecimal() / nobject.ToBigDecimal(),

                _ => throw new InvalidOperationException(
                    $"Cannot perform arithmetic operation '{Operation}' on"
                    + $"non arithmetic types. subject: '{subjectIon.Type}', object: '{objectIon.Type}'")
            };
        }

        internal Atom<IonNumber> Modulus()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return (subjectIon, objectIon) switch
            {
                (INumericType nsubject, INumericType nobject) => ((IIonType)nsubject).IsNull || ((IIonType)nobject).IsNull
                    ? new IonNumber()
                    : nsubject.ToBigDecimal() % nobject.ToBigDecimal(),

                _ => throw new InvalidOperationException(
                    $"Cannot perform arithmetic operation '{Operation}' on"
                    + $"non arithmetic types. subject: '{subjectIon.Type}', object: '{objectIon.Type}'")
            };
        }

        internal Atom<IonNumber> Power()
        {
            var subjectIon = Subject.Evaluate().Ion;
            var objectIon = Object.Evaluate().Ion;

            return (subjectIon, objectIon) switch
            {
                (INumericType nsubject, INumericType nobject) => ((IIonType)nsubject).IsNull || ((IIonType)nobject).IsNull
                    ? new IonNumber()
                    : BigDecimal.Power(
                        nsubject.ToBigDecimal()!.Value!,
                        nobject.ToBigDecimal()!.Value),

                _ => throw new InvalidOperationException(
                    $"Cannot perform arithmetic operation '{Operation}' on"
                    + $"non arithmetic types. subject: '{subjectIon.Type}', object: '{objectIon.Type}'")
            };
        }
        #endregion

        #region Parse

        public static bool TryParse(
            string text,
            out IResult<ArithmeticOperation> result)
            => (result = Parse(text)) is IResult<ArithmeticOperation>.DataResult;

        public static bool TryParse(
            CSTNode arithmeticExpressionNode,
            out IResult<ArithmeticOperation> result)
            => (result = Parse(arithmeticExpressionNode)) is IResult<ArithmeticOperation>.DataResult;


        public static IResult<ArithmeticOperation> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(ArithmeticExpressionSymbol)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),

                FailureResult
                or ErrorResult => Result.Of<ArithmeticOperation>(new RecognitionException(parseResult)),

                _ => Result.Of<ArithmeticOperation>(
                    new InvalidOperationException($"Invalid recognition result: '{parseResult}'"))
            };
        }

        public static IResult<ArithmeticOperation> Parse(CSTNode arithmeticExpression)
        {
            if (arithmeticExpression is null)
                throw new ArgumentNullException(nameof(arithmeticExpression));

            try
            {
                var opNodes = $"{NumericExpressionSymbol}|{ArithmeticOperatorSymbol}";
                var expressionItems = arithmeticExpression
                    .FindNodes(opNodes)
                    .Select(node => node.SymbolName switch
                    {
                        NumericExpressionSymbol => new ExpressionItem(Expression.ParseNumericExpression(node).Resolve()),
                        ArithmeticOperatorSymbol => new OperatorItem(ToOperator(node.TokenValue())),
                        _ => new InvalidOperationException($"Unexpected symbol: {node.SymbolName}").Throw<OperationItem>()
                    });

                return GenerateOperation
                    <ArithmeticOperation, Operators, IonNumber>(
                    arithmeticExpression,
                    expressionItems,
                    OperationProvider,
                    PrecedenceDirectionEvaluator);
            }
            catch (Exception ex)
            {
                return Result.Of<ArithmeticOperation>(ex);
            }
        }

        private static Operators ToOperator(string symbol)
        {
            return symbol switch
            {
                "+" => Operators.Add,
                "-" => Operators.Subtract,
                "*" => Operators.Multiply,
                "/" => Operators.Divide,
                "%" => Operators.Modulus,
                "**" => Operators.Power,
                _ => throw new ArgumentException($"Invalid symbol: {symbol}")
            };
        }

        internal static PrecedenceDirection PrecedenceDirectionEvaluator(Enum @operator)
        {
            return @operator switch
            {
                Operators.Power => PrecedenceDirection.Rtl,
                _ => PrecedenceDirection.Ltr
            };
        }

        internal static ArithmeticOperation OperationProvider(
            IExpression subject,
            IExpression @object,
            Enum @operation)
            => new ArithmeticOperation(subject, @object, (Operators)@operation);

        #endregion

        public override string ToString()
        {
            return $"({Subject} {OperatorSymbols[Operation]} {Object})";
        }

        public enum Operators
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Modulus,
            Power
        }

    }
}
