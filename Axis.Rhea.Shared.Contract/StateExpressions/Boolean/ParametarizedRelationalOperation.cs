using Axis.Ion.Types;
using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Boolean
{
    public record ParametarizedRelationalOperation : IParametarizedOperation<IonBool>
    {
        #region Grammar Symbols
        internal const string ParametarizedRelationalSymbol = "parametarized-relational-expression";
        #endregion

        private static Dictionary<Operators, string> OperatorSymbols = new()
        {
            [Operators.In] = "in",
            [Operators.NotIn] = "not-in",
            [Operators.Between] = "between",
            [Operators.NotBetween] = "not-between"
        };

        public string Name => Operation.ToString();

        public Operators Operation { get; }


        public IExpression Subject { get; }

        public ImmutableArray<IExpression> Arguments { get; }

        IReadOnlyList<IExpression> IParametarizedOperation<IonBool>.Arguments => Arguments;

        public ParametarizedRelationalOperation(Operators operation, IExpression subject, params IExpression[] arguments)
        {
            Operation = operation.ThrowIf(
                enm => !Enum.IsDefined(enm),
                new ArgumentException($"Invalid Operations: {operation}"));

            Subject = subject ?? throw new ArgumentNullException(nameof(subject));

            Arguments = arguments
                .ThrowIfNull(new ArgumentNullException(nameof(arguments)))
                .ThrowIfAny(exp => exp is null, new ArgumentException($"{nameof(arguments)} cannot contain null expressions"))
                .ToImmutableArray();

            if (arguments.Length == 0)
                throw new ArgumentException($"{nameof(arguments)} cannot be empty");

            if (operation == Operators.Between && Arguments.Length != 2)
                throw new ArgumentException($"the '{Operation}' operation requires 2 arguments");
        }

        #region Evaluate
        ITypedAtom<IonBool> ITypedExpression<IonBool>.Evaluate() => Evaluate();

        IAtom IExpression.Evaluate() => Evaluate();

        public bool EvaluateCondition()
        {
            return Evaluate().Ion.Value == true;
        }

        public Atom<IonBool> Evaluate()
        {
            return Operation switch
            {
                Operators.In => EvaluateIn(),
                Operators.Between => EvaluateBetween(),
                Operators.NotIn => EvaluateNotIn(),
                Operators.NotBetween => EvaluateNotBetween(),
                _ => throw new InvalidOperationException($"Invalid Operation: {Operation}")
            };
        }

        private Atom<IonBool> EvaluateIn()
        {
            var subjectAtom = Subject.Evaluate();
            foreach (var arg in Arguments)
            {
                var argAtom = arg.Evaluate();
                if (argAtom.Ion.Equals(subjectAtom))
                    return new IonBool(true);
            }

            return new IonBool(false);
        }

        private Atom<IonBool> EvaluateNotIn()
        {
            var subjectAtom = Subject.Evaluate();
            foreach (var arg in Arguments)
            {
                var argAtom = arg.Evaluate();
                if (argAtom.Ion.Equals(subjectAtom))
                    return new IonBool(false);
            }

            return new IonBool(true);
        }

        private Atom<IonBool> EvaluateBetween()
        {
            var l = new BinaryRelationalOperation(Subject, Arguments[0], BinaryRelationalOperation.Operators.GreaterOrEqual);
            var r = new BinaryRelationalOperation(Subject, Arguments[1], BinaryRelationalOperation.Operators.LessOrEqual);

            return new IonBool(l.EvaluateComparison() && r.EvaluateComparison());
        }

        private Atom<IonBool> EvaluateNotBetween()
        {
            var l = new BinaryRelationalOperation(Subject, Arguments[0], BinaryRelationalOperation.Operators.LessThan);
            var r = new BinaryRelationalOperation(Subject, Arguments[1], BinaryRelationalOperation.Operators.GreaterThan);

            return new IonBool(l.EvaluateComparison() || r.EvaluateComparison());
        }
        #endregion

        public override string ToString()
        {
            var args = Arguments
                .Select(arg => arg.ToString())
                .JoinUsing(", ");

            return $"{Subject} {OperatorSymbols[Operation]} ({args})";
        }

        public enum Operators
        {
            In,
            NotIn,
            Between,
            NotBetween,
        }
    }
}
