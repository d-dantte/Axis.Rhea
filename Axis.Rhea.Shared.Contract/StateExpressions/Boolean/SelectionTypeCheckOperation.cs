using Axis.Ion.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Value;
using Axis.Rhea.Shared.Contract.Utils;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Boolean
{
    public record SelectionTypeCheckOperation<TIonType> :
        IUnaryOperation<IonBool>,
        IResultParsable<SelectionTypeCheckOperation<TIonType>>
        where TIonType : struct, IIonType
    {
        #region Grammar Symbols
        internal const string SelectionTypeCheckSymbol = "state-selection-typecheck-expression";
        #endregion

        public string Name => "IsType";

        public IValueSelectionExpression<TIonType> Subject { get; }

        IExpression IUnaryOperation<IonBool>.Subject => Subject;

        public SelectionTypeCheckOperation(IValueSelectionExpression<TIonType> subject)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        }

        #region Evaluate

        public Atom<IonBool> Evaluate()
        {
            return Subject.Selection
                .Map(ion => new Atom<IonBool>(true))
                .MapError(error => new Atom<IonBool>(error.InnerException is not MissmatchedSelectionTypeException))
                .Resolve();
        }

        ITypedAtom<IonBool> ITypedExpression<IonBool>.Evaluate() => Evaluate();

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse

        public static IResult<SelectionTypeCheckOperation<TIonType>> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(SelectionTypeCheckSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
            {
                _ = TryParse(success.Symbol, out IResult<SelectionTypeCheckOperation<TIonType>> result);
                return result;
            }

            return Result.Of<SelectionTypeCheckOperation<TIonType>>(new RecognitionException(parseResult));
        }

        public static IResult<SelectionTypeCheckOperation<TIonType>> Parse(CSTNode selectionExistenceNode)
        {
            _ = TryParse(selectionExistenceNode, out var result);
            return result;
        }

        public static bool TryParse(string text, out IResult<SelectionTypeCheckOperation<TIonType>> result)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(SelectionTypeCheckSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return TryParse(success.Symbol, out result);

            result = Result.Of<SelectionTypeCheckOperation<TIonType>>(new RecognitionException(parseResult));
            return false;
        }

        public static bool TryParse(CSTNode selectionExistenceNode, out IResult<SelectionTypeCheckOperation<TIonType>> result)
        {
            if (selectionExistenceNode is null)
                throw new ArgumentNullException(nameof(selectionExistenceNode));

            var selectionType = selectionExistenceNode.NodeAt(1);
            var ionType = typeof(TIonType).AsIonType();
            result = (selectionType.SymbolName, ionType) switch
            {
                (StateSelectionValue.BoolSelectionSymbol, IonTypes.Bool) => StateSelectionValue<TIonType>
                    .Parse(selectionType.FindNode(StateSelectionValue.DataPathSymbol))
                    .Map(ssv => new SelectionTypeCheckOperation<TIonType>(ssv)),

                (StateSelectionValue.IntSelectionSymbol, IonTypes.Int) => StateSelectionValue<TIonType>
                    .Parse(selectionType.FindNode(StateSelectionValue.DataPathSymbol))
                    .Map(ssv => new SelectionTypeCheckOperation<TIonType>(ssv)),

                (StateSelectionValue.FloatSelectionSymbol, IonTypes.Float) => StateSelectionValue<TIonType>
                    .Parse(selectionType.FindNode(StateSelectionValue.DataPathSymbol))
                    .Map(ssv => new SelectionTypeCheckOperation<TIonType>(ssv)),

                (StateSelectionValue.DecimalSelectionSymbol, IonTypes.Decimal) => StateSelectionValue<TIonType>
                    .Parse(selectionType.FindNode(StateSelectionValue.DataPathSymbol))
                    .Map(ssv => new SelectionTypeCheckOperation<TIonType>(ssv)),

                (StateSelectionValue.TimestampSelectionSymbol, IonTypes.Timestamp) => StateSelectionValue<TIonType>
                    .Parse(selectionType.FindNode(StateSelectionValue.DataPathSymbol))
                    .Map(ssv => new SelectionTypeCheckOperation<TIonType>(ssv)),

                (StateSelectionValue.StringSelectionSymbol, IonTypes.String) => StateSelectionValue<TIonType>
                    .Parse(selectionType.FindNode(StateSelectionValue.DataPathSymbol))
                    .Map(ssv => new SelectionTypeCheckOperation<TIonType>(ssv)),

                _ => Result.Of<SelectionTypeCheckOperation<TIonType>>(new InvalidOperationException(
                    $"Could not parse the symbol ({selectionType.SymbolName}) "
                    + $"into the given ion type: {ionType}"))
            };

            return result is IResult<SelectionTypeCheckOperation<TIonType>>.DataResult;
        }
        #endregion

        public override string ToString()
        {
            return $"${Subject}";
        }
    }
}
