using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Exceptions;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Rhea.Shared.Contract.StateExpressions.Duration;
using Axis.Rhea.Shared.Contract.Utils;
using Axis.Rhea.Shared.Contract.Workflow.State.DataPath;
using static System.Net.Mime.MediaTypeNames;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Value
{
    public record StateSelectionValue<TIonType> : IValueSelectionExpression<TIonType>
    where TIonType : struct, IIonType
    {
        public DataPathSegment Path { get; }

        public IResult<TIonType> Selection => Select();

        public TIonType Ion => Selection.Resolve();

        public StateSelectionValue(DataPathSegment path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        private IResult<TIonType> Select()
        {
            var state = EvaluationContext.AsyncLocal
                ?? throw new InvalidOperationException("Evaluation context is invalid: null workflow state found");

            var selection = Path.Select(state.Data);

            if (selection.MatchType != Workflow.State.DataPath.MatchType.Hit)
                Result.Of<TIonType>(new SelectionMissException(Path.ToString()));

            if (selection.Value is TIonType typedIon)
                return Result.Of(typedIon);

            else return Result.Of<TIonType>(new MissmatchedSelectionTypeException(typeof(TIonType).AsIonType(), selection.Value!.Type));
        }

        #region Evaluate

        /// <summary>
        /// Note: If the value does not exist, i.e, the selection failed, this will throw an exception with the original failed selection reason.
        /// </summary>
        /// <returns></returns>
        public Atom<TIonType> Evaluate() => Selection
            .Map(ion => new Atom<TIonType>(ion))
            .Resolve();

        ITypedAtom<TIonType> ITypedExpression<TIonType>.Evaluate() => Evaluate();

        IAtom IExpression.Evaluate() => Evaluate();
        #endregion

        #region Parse methods

        public static bool TryParse(
            CSTNode dataPathNode,
            out IResult<StateSelectionValue<TIonType>> result)
            => (result = Parse(dataPathNode)) is IResult<StateSelectionValue<TIonType>>.DataResult;

        public static bool TryParse(
            string dataPath,
            out IResult<StateSelectionValue<TIonType>> result)
            => (result = Parse(dataPath)) is IResult<StateSelectionValue<TIonType>>.DataResult;

        public static IResult<StateSelectionValue<TIonType>> Parse(string text)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(StateSelectionValue.DataPathSymbol)
                .Recognize(text);

            if (parseResult is SuccessResult success)
                return Parse(success.Symbol);

            return Result.Of<StateSelectionValue<TIonType>>(new RecognitionException(parseResult));
        }

        public static IResult<StateSelectionValue<TIonType>> Parse(CSTNode dataPathNode)
        {
            if (dataPathNode is null)
                throw new ArgumentNullException(nameof(dataPathNode));

            return DataPathSegment.TryParse(dataPathNode, out IResult<DataPathSegment> val)
                ? val.Map(v => new StateSelectionValue<TIonType>(v))
                : Result.Of<StateSelectionValue<TIonType>>(
                    new FormatException($"Invalid state selection literal: {dataPathNode.TokenValue()}"));
        }
        #endregion

        public override string ToString()
        {
            return Path.ToString();
        }
    }

    public static class StateSelectionValue
    {
        internal const string IntSelectionSymbol = "int-state-selection-expression";
        internal const string FloatSelectionSymbol = "float-state-selection-expression";
        internal const string DecimalSelectionSymbol = "decimal-state-selection-expression";
        internal const string BoolSelectionSymbol = "bool-state-selection-expression";
        internal const string StringSelectionSymbol = "string-state-selection-expression";
        internal const string TimestampSelectionSymbol = "timestamp-state-selection-expression";
        internal const string DataPathSymbol = "data-path";

        public static bool TryParse(
            CSTNode stateSelection,
            out IResult<IExpression> result)
            => (result = Parse(stateSelection)) is IResult<IExpression>.DataResult;

        public static bool TryParse(
            string text,
            out IResult<IExpression> result)
            => (result = Parse(text)) is IResult<IExpression>.DataResult;


        public static IResult<IExpression> Parse(string stateSelection)
        {
            var parseResult = PulsarUtil.StateExpressionGrammar
                .GetRecognizer(DeduceSymbol(stateSelection))
                .Recognize(stateSelection);

            if (parseResult is SuccessResult success)
                return Parse(success.Symbol);

            return Result.Of<IExpression>(new RecognitionException(parseResult));
        }

        public static IResult<IExpression> Parse(CSTNode stateSelection)
        {
            var dataPathNode = stateSelection.FindNode(DataPathSymbol);
            return stateSelection.SymbolName switch
            {
                IntSelectionSymbol => StateSelectionValue<IonInt>
                    .Parse(dataPathNode)
                    .Map(ex => (IExpression)ex),

                FloatSelectionSymbol => StateSelectionValue<IonFloat>
                    .Parse(dataPathNode)
                    .Map(ex => (IExpression)ex),

                DecimalSelectionSymbol => StateSelectionValue<IonDecimal>
                    .Parse(dataPathNode)
                    .Map(ex => (IExpression)ex),

                BoolSelectionSymbol => StateSelectionValue<IonBool>
                    .Parse(dataPathNode)
                    .Map(ex => (IExpression)ex),

                StringSelectionSymbol => StateSelectionValue<IonString>
                    .Parse(dataPathNode)
                    .Map(ex => (IExpression)ex),

                TimestampSelectionSymbol => StateSelectionValue<IonTimestamp>
                    .Parse(dataPathNode)
                    .Map(ex => (IExpression)ex),

                _ => throw new ArgumentException(
                    $"Invalid symbol: '{stateSelection.SymbolName}'. Expected"
                    + $"'{IntSelectionSymbol}', '{FloatSelectionSymbol}',...")
            };
        }

        private static string DeduceSymbol(string text)
        {
            return
                text.StartsWith("bool::") ? BoolSelectionSymbol :
                text.StartsWith("int::") ? IntSelectionSymbol :
                text.StartsWith("real::") ? FloatSelectionSymbol :
                text.StartsWith("decimal::") ? DecimalSelectionSymbol :
                text.StartsWith("string::") ? StringSelectionSymbol :
                text.StartsWith("time-stamp::") ? TimestampSelectionSymbol :
                throw new FormatException($"Invalid typed-state selection expression: {text}");
        }
    }
}
