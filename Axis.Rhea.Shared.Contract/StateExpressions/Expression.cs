using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.StateExpressions
{
    public static class Expression
    {
        #region Grammar Symbols
        internal const string ExpressionSymbol = "expression";
        internal const string TimestampExpressionSymbol = "timestamp-expression";
        internal const string DurationExpressionSymbol = "duration-expression";
        internal const string StringExpressionSymbol = "string-expression";
        internal const string NumericExpressionSymbol = "numeric-expression";
        internal const string BoolExpressionSymbol = "bool-expression";

        // duration
        internal const string ConstantDurationExpressionSymbol = "constant-duration-expression";
        internal const string DurationArithmeticExpressionSymbol = "duration-arithmetic-expression";

        // timestamp
        internal const string ConstantTimestampExpressionSymbol = "constant-timestamp-expression";
        internal const string TimestampArithmeticExpressionSymbol = "timestamp-arithmetic-expression";
        internal const string TimestampStateSelectionExpressionSymbol = "timestamp-state-selection-expression";

        // string
        internal const string ConstantStringExpressionSymbol = "constant-string-expression";
        internal const string StringConcatenationExpressionSymbol = "string-concatenetion-expression";
        internal const string StringStateSelectionExpressionSymbol = "string-state-selection-expression";

        // numeric
        internal const string ConstantNumericExpressionSymbol = "constant-numeric-expression";
        internal const string NumericArithmeticExpressionSymbol = "arithmetic-expression";
        internal const string IntStateSelectionExpressionSymbol = "int-state-selection-expression";
        internal const string FloatStateSelectionExpressionSymbol = "float-state-selection-expression";
        internal const string DecimalStateSelectionExpressionSymbol = "decimal-state-selection-expression";
        internal const string IntNotationSymbol = "int-notation";
        internal const string DecimalNotationSymbol = "decimal-notation";
        internal const string FloatNotationSymbol = "float-notation";

        // boolean
        internal const string ConstantBooleanExpressionSymbol = "constant-boolean-expression";
        internal const string BoolStateSelectionExpressionSymbol = "bool-state-selection-expression";
        internal const string StateSelectionTypeCheckExpressionSymbol = "state-selection-typecheck-expression";
        internal const string StateSelectionExistenceExpressionSymbol = "state-selection-existence-expression";
        internal const string RelationalExpressionSymbol = "relational-expression";
        internal const string ConditionalExpressionSymbol = "conditional-expression";
        internal const string NegationExpressionSymbol = "negation-expression";
        #endregion


        public static bool TryParse(
            string text,
            out IResult<IExpression> result)
            => (result = Parse(text)) is IResult<IExpression>.DataResult;

        public static bool TryParse(
            CSTNode node,
            out IResult<IExpression> result)
            => (result = Parse(node)) is IResult<IExpression>.DataResult;


        public static IResult<IExpression> Parse(string expression)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(ExpressionSymbol)
                .Recognize(expression);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),

                FailureResult
                or ErrorResult => Result.Of<IExpression>(new RecognitionException(parseResult)),

                _ => Result.Of<IExpression>(
                    new InvalidOperationException($"Invalid recognition result: '{parseResult}'"))
            };
        }

        public static IResult<IExpression> Parse(CSTNode expressionNode)
        {
            if (expressionNode is null)
                throw new ArgumentNullException(nameof(expressionNode));

            try
            {
                var typedExpression = expressionNode.FirstNode();
                return typedExpression.SymbolName switch
                {
                    TimestampExpressionSymbol => Expression
                        .ParseTimestampExpression(typedExpression)
                        .Map(exp => (IExpression)exp),

                    DurationExpressionSymbol => Expression
                        .ParseDurationExpression(typedExpression)
                        .Map(exp => (IExpression)exp),

                    StringExpressionSymbol => Expression
                        .ParseStringExpression(typedExpression)
                        .Map(exp => (IExpression)exp),

                    NumericExpressionSymbol => Expression
                        .ParseNumericExpression(typedExpression)
                        .Map(exp => (IExpression)exp),

                    BoolExpressionSymbol => Expression
                        .ParseBoolExpression(typedExpression)
                        .Map(exp => (IExpression)exp),

                    _ => throw new InvalidOperationException($"Invalid symbol encountered: '{expressionNode.SymbolName}'")
                };
            }
            catch (Exception e)
            {
                return Result.Of<IExpression>(e);
            }
        }

        public static IResult<ITypedExpression<IonTimestamp>> ParseTimestampExpression(
            CSTNode timestampExpressionNode)
        {
            if (timestampExpressionNode is null)
                throw new ArgumentNullException(nameof(timestampExpressionNode));

            var timestampType = timestampExpressionNode.FirstNode();
            return timestampType.SymbolName switch
            {
                ConstantTimestampExpressionSymbol => Timestamp.TimestampConstantExpression
                    .Parse(timestampType)
                    .Map(exp => (ITypedExpression<IonTimestamp>)exp),

                TimestampArithmeticExpressionSymbol => Timestamp.TimestampConstantExpression
                    .Parse(timestampType)
                    .Map(exp => (ITypedExpression<IonTimestamp>)exp),

                TimestampStateSelectionExpressionSymbol => Value.StateSelectionValue
                    .Parse(timestampType)
                    .Map(exp => (ITypedExpression<IonTimestamp>)exp),

                _ => throw new InvalidOperationException(
                        $"Invalid symbol: {timestampType.SymbolName}. Expected "
                        + $"'{ConstantTimestampExpressionSymbol}', '{TimestampArithmeticExpressionSymbol}',...")
            };
        }

        public static IResult<ITypedExpression<IonString>> ParseStringExpression(
            CSTNode stringExpressionNode)
        {
            if (stringExpressionNode is null)
                throw new ArgumentNullException(nameof(stringExpressionNode));

            var stringType = stringExpressionNode.FirstNode();
            return stringType.SymbolName switch
            {
                ConstantStringExpressionSymbol => String.StringConstantExpression
                    .Parse(stringType)
                    .Map(exp => (ITypedExpression<IonString>)exp),

                StringConcatenationExpressionSymbol => String.StringConcatenationOperation
                    .Parse(stringType)
                    .Map(exp => (ITypedExpression<IonString>)exp),

                StringStateSelectionExpressionSymbol => Value.StateSelectionValue
                    .Parse(stringType)
                    .Map(exp => (ITypedExpression<IonString>)exp),

                _ => throw new InvalidOperationException(
                        $"Invalid symbol: {stringType.SymbolName}. Expected "
                        + $"'{ConstantStringExpressionSymbol}' or '{StringConcatenationExpressionSymbol}'.")
            };
        }

        public static IResult<IExpression> ParseNumericExpression(
            CSTNode numericExpressionNode)
        {
            if (numericExpressionNode is null)
                throw new ArgumentNullException(nameof(numericExpressionNode));

            var numericType = numericExpressionNode.FirstNode();
            return numericType.SymbolName switch
            {
                ConstantNumericExpressionSymbol => numericType
                    .FirstNode()
                    .ApplyTo(notation => notation.SymbolName switch
                    {
                        IntNotationSymbol =>  Numeric.IntegerConstantExpression
                            .Parse(numericType)
                            .Map(exp => (IExpression)exp),

                        FloatNotationSymbol => Numeric.RealConstantExpression
                            .Parse(numericType)
                            .Map(exp => (IExpression)exp),

                        DecimalNotationSymbol => Numeric.DecimalConstantExpression
                            .Parse(numericType)
                            .Map(exp => (IExpression)exp),

                        _ => throw new ArgumentException(
                            $"Invalid symbol: {notation.SymbolName}. Expected "
                            + $"'{IntNotationSymbol}', "
                            + $"'{FloatNotationSymbol}',...")
                    }),

                NumericArithmeticExpressionSymbol => Numeric.ArithmeticOperation
                    .Parse(numericType)
                    .Map(exp => (IExpression)exp),

                IntStateSelectionExpressionSymbol
                or FloatStateSelectionExpressionSymbol
                or DecimalStateSelectionExpressionSymbol => Value.StateSelectionValue.Parse(numericType),

                _ => throw new InvalidOperationException(
                    $"Invalid symbol: {numericType.SymbolName}. Expected "
                    + $"'{ConstantNumericExpressionSymbol}', "
                    + $"'{NumericArithmeticExpressionSymbol}',...")
            };
        }

        public static IResult<ITypedExpression<IonBool>> ParseBoolExpression(
            CSTNode booleanExpressionNode)
        {
            if (booleanExpressionNode is null)
                throw new ArgumentNullException(nameof(booleanExpressionNode));

            var boolType = booleanExpressionNode.FirstNode();
            return boolType.SymbolName switch
            {
                StateSelectionTypeCheckExpressionSymbol => ParseStateTypeCheckExpression(boolType),
                StateSelectionExistenceExpressionSymbol => ParseStateExistenceCheckExpression(boolType),

                ConstantBooleanExpressionSymbol => Boolean.BoolConstantExpression
                    .Parse(boolType)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                BoolStateSelectionExpressionSymbol => Value.StateSelectionValue
                    .Parse(boolType)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                RelationalExpressionSymbol => Boolean.BinaryRelationalOperation
                    .Parse(boolType)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                ConditionalExpressionSymbol => Boolean.ConditionalOperation
                    .Parse(boolType)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                NegationExpressionSymbol => Boolean.NegationOperation
                    .Parse(boolType)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                _ => throw new InvalidOperationException(
                    $"Invalid symbol: {boolType.SymbolName}. Expected "
                    + $"'{ConstantBooleanExpressionSymbol}', "
                    + $"'{BoolStateSelectionExpressionSymbol}',...")
            };
        }

        public static IResult<ITypedExpression<IonDuration>> ParseDurationExpression(
            CSTNode durationExpressionNode)
        {
            if (durationExpressionNode is null)
                throw new ArgumentNullException(nameof(durationExpressionNode));

            var durationType = durationExpressionNode.FirstNode();
            return durationType.SymbolName switch
            {
                ConstantDurationExpressionSymbol => Duration.DurationConstantExpression
                    .Parse(durationType)
                    .Map(exp => (ITypedExpression<IonDuration>)exp),

                DurationArithmeticExpressionSymbol => Duration.DurationArithmeticOperation
                    .Parse(durationType)
                    .Map(exp => (ITypedExpression<IonDuration>)exp),

                _ => throw new InvalidOperationException(
                        $"Invalid symbol: {durationType.SymbolName}. Expected "
                        + $"'{ConstantDurationExpressionSymbol}' or '{DurationArithmeticExpressionSymbol}'.")
            };
        }

        private static IResult<ITypedExpression<IonBool>> ParseStateTypeCheckExpression(CSTNode typeCheckNode)
        {
            var stateSelection = typeCheckNode.NodeAt(1);
            return stateSelection.SymbolName switch
            {
                BoolStateSelectionExpressionSymbol => Boolean.SelectionTypeCheckOperation<IonBool>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                IntStateSelectionExpressionSymbol => Boolean.SelectionTypeCheckOperation<IonInt>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                FloatStateSelectionExpressionSymbol => Boolean.SelectionTypeCheckOperation<IonFloat>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                DecimalStateSelectionExpressionSymbol => Boolean.SelectionTypeCheckOperation<IonDecimal>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                StringStateSelectionExpressionSymbol => Boolean.SelectionTypeCheckOperation<IonString>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                TimestampStateSelectionExpressionSymbol => Boolean.SelectionTypeCheckOperation<IonTimestamp>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                _ => throw new InvalidOperationException(
                    $"Invalid symbol: {stateSelection.SymbolName}. Expected "
                    + $"'{BoolStateSelectionExpressionSymbol}', "
                    + $"'{IntStateSelectionExpressionSymbol}',...")

            };
        }

        private static IResult<ITypedExpression<IonBool>> ParseStateExistenceCheckExpression(CSTNode existenceCheck)
        {
            var stateSelection = existenceCheck.NodeAt(1);
            return stateSelection.SymbolName switch
            {
                BoolStateSelectionExpressionSymbol => Boolean.SelectionExistenceOperation<IonBool>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                IntStateSelectionExpressionSymbol => Boolean.SelectionExistenceOperation<IonInt>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                FloatStateSelectionExpressionSymbol => Boolean.SelectionExistenceOperation<IonFloat>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                DecimalStateSelectionExpressionSymbol => Boolean.SelectionExistenceOperation<IonDecimal>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                StringStateSelectionExpressionSymbol => Boolean.SelectionExistenceOperation<IonString>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                TimestampStateSelectionExpressionSymbol => Boolean.SelectionExistenceOperation<IonTimestamp>
                    .Parse(stateSelection)
                    .Map(exp => (ITypedExpression<IonBool>)exp),

                _ => throw new InvalidOperationException(
                    $"Invalid symbol: {stateSelection.SymbolName}. Expected "
                    + $"'{BoolStateSelectionExpressionSymbol}', "
                    + $"'{IntStateSelectionExpressionSymbol}',...")

            };
        }
    }
}
