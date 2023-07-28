using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Boolean
{
    public record NegationOperation :
        IUnaryOperation<IonBool>,
        IResultParsable<NegationOperation>
    {
        #region Grammar Symbols
        internal const string NegationExpressionSymbol = "negation-expression";
        #endregion

        public string Name => "Not";

        public ITypedExpression<IonBool> Subject { get; }

        IExpression IUnaryOperation<IonBool>.Subject => Subject;

        public NegationOperation(ITypedExpression<IonBool> subject)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        }

        #region Evaluate
        IAtom IExpression.Evaluate() => Evaluate();

        ITypedAtom<IonBool> ITypedExpression<IonBool>.Evaluate() => Evaluate();

        public Atom<IonBool> Evaluate() => new Atom<IonBool>(!Subject.Evaluate().Ion.Value!);
        #endregion

        #region Parse

        public static IResult<NegationOperation> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(NegationExpressionSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
            {
                _ = TryParse(success.Symbol, out IResult<NegationOperation> result);
                return result;
            }

            return Result.Of<NegationOperation>(new RecognitionException(parseResult));
        }

        public static IResult<NegationOperation> Parse(CSTNode selectionExistenceNode)
        {
            _ = TryParse(selectionExistenceNode, out var result);
            return result;
        }

        public static bool TryParse(string text, out IResult<NegationOperation> result)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(NegationExpressionSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return TryParse(success.Symbol, out result);

            result = Result.Of<NegationOperation>(new RecognitionException(parseResult));
            return false;
        }

        public static bool TryParse(CSTNode negationNode, out IResult<NegationOperation> result)
        {
            if (negationNode is null)
                throw new ArgumentNullException(nameof(negationNode));

            var boolExpression = negationNode.NodeAt(1);
            result = Expression
                .Parse(boolExpression)
                .Map(ex => ex.As<ITypedExpression<IonBool>>())
                .Map(r => new NegationOperation(r));

            return result is IResult<NegationOperation>.DataResult;
        }
        #endregion

        public override string ToString()
        {
            return $"!{Subject}";
        }
    }
}
