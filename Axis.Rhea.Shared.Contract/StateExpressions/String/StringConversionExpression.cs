using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.StateExpressions.String
{
    /// <summary>
    /// Represents a conversion of it's operand into a string atom.
    /// Takes the form:
    /// <code>
    /// as-string::&lt;expression&gt;.
    /// //e.g
    /// 1. as-string::int::/abc/xyz
    /// 2. as-string::543.5
    /// 3. as-string::'T 2023-05-11T10:34'
    /// </code>
    /// </summary>
    public record StringConversionExpression :
        IUnaryOperation<IonString>,
        IResultParsable<StringConversionExpression>
    {
        #region Grammar Symbols
        internal const string StringConversionOperationSymbol = "string-conversion-expression";
        internal const string ExpressionSymbol = "expression";
        #endregion

        public string Name => "AsString";

        public IExpression Subject { get; }

        public StringConversionExpression(IExpression subject)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        }


        #region Parse

        public static bool TryParse(
            string text,
            out IResult<StringConversionExpression> result)
            => (result = Parse(text)) is IResult<StringConversionExpression>.DataResult;

        public static bool TryParse(
            CSTNode temporalOperationNode,
            out IResult<StringConversionExpression> result)
            => (result = Parse(temporalOperationNode)) is IResult<StringConversionExpression>.DataResult;

        public static IResult<StringConversionExpression> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(StringConversionOperationSymbol)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol),

                FailureResult
                or ErrorResult => Result.Of<StringConversionExpression>(new RecognitionException(parseResult)),

                _ => Result.Of<StringConversionExpression>(
                    new InvalidOperationException($"Invalid recognition result: '{parseResult}'"))
            };
        }

        public static IResult<StringConversionExpression> Parse(CSTNode stringConversionNode)
        {
            try
            {
                return stringConversionNode.SymbolName switch
                {
                    StringConversionOperationSymbol => stringConversionNode
                        .FindNode(ExpressionSymbol)
                        .AsOptional()
                        .Map(Expression.Parse)
                        .ValueOr(Result.Of<IExpression>(new FormatException($"Symbol '{ExpressionSymbol}' not found")))
                        .Map(ex => new StringConversionExpression(ex)),

                    _ => throw new InvalidOperationException(
                        $"Invalid symbol: '{stringConversionNode.SymbolName}'. Expected "
                        + $"'{StringConversionOperationSymbol}'")
                };
            }
            catch (Exception ex)
            {
                return Result.Of<StringConversionExpression>(ex);
            }
        }
        #endregion

        #region Evaluate
        public ITypedAtom<IonString> Evaluate() => new Atom<IonString>(Subject.Evaluate().Ion.ToAtomText());

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion
    }
}
