using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;
using System.Xml.Linq;

namespace Axis.Rhea.Shared.Contract.StateExpressions.String
{
    public record StringConstantExpression :
        IConstantValueExpression<IonString>,
        IResultParsable<StringConstantExpression>
    {
        #region Grammar Symbols
        internal const string StringConstantSymbol = "constant-string-expression";
        #endregion
        public IonString Ion { get; }

        public StringConstantExpression(string value)
        {
            Ion = value;
        }

        #region Evaluate
        public ITypedAtom<IonString> Evaluate() => new Atom<IonString>(Ion);

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse methods

        public static bool TryParse(
            CSTNode constantStringNode,
            out IResult<StringConstantExpression> result)
            => (result = Parse(constantStringNode)) is IResult<StringConstantExpression>.DataResult;

        public static bool TryParse(
            string text,
            out IResult<StringConstantExpression> result)
            => (result = Parse(text)) is IResult<StringConstantExpression>.DataResult;

        public static IResult<StringConstantExpression> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(StringConstantSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return Parse(success.Symbol);

            return Result.Of<StringConstantExpression>(new RecognitionException(parseResult));
        }

        public static IResult<StringConstantExpression> Parse(CSTNode constantStringNode)
        {
            if (constantStringNode is null)
                throw new ArgumentNullException(nameof(constantStringNode));

            var text = constantStringNode.TokenValue();
            return Result.Of(new StringConstantExpression(text.UnwrapFrom("\"")));
        }
        #endregion

        public override string ToString()
        {
            return $"\"{Ion.ToIonText()}\"";
        }
    }
}
