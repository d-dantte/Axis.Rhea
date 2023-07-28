using Axis.Ion.Types;
using Axis.Luna.Extensions;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Boolean
{
    internal class StringMatchingOperation : IBinaryOperation<IonBool>
    {
        private static Dictionary<Operators, string> OperatorSymbols = new()
        {
            [Operators.Matches] = "!!",
            [Operators.StartsWith] = "!<",
            [Operators.EndsWith] = ">!",
            [Operators.ContainsSubstring] = "><"
        };

        private static readonly MemoryCache PatternCache = new MemoryCache(typeof(StringMatchingOperation).FullName!);

        public string Name => Operation.ToString();

        public Operators Operation { get; }

        public ITypedExpression<IonString> Subject { get; }

        public ITypedExpression<IonString> Object { get; }

        IExpression IBinaryOperation<IonBool>.Subject => Subject;

        IExpression IBinaryOperation<IonBool>.Object => Object;


        public StringMatchingOperation(ITypedExpression<IonString> subject, ITypedExpression<IonString> @object, Operators operation)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Object = @object ?? throw new ArgumentNullException(nameof(@object));
            Operation = operation.ThrowIf(
                enm => !Enum.IsDefined(enm),
                new ArgumentException($"Invalid Operations: {operation}"));
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
            var lhs = Subject.Evaluate().Ion;
            var rhs = Object.Evaluate().Ion;

            return Operation switch
            {
                Operators.Matches => Matches(lhs, rhs),

                Operators.StartsWith => StartsWith(lhs, rhs),

                Operators.EndsWith => EndsWith(lhs, rhs),

                Operators.ContainsSubstring => IsSubstringOf(lhs, rhs),

                _ => throw new InvalidOperationException($"Invalid Operation: {Operation}")
            };
        }

        private static Atom<IonBool> IsSubstringOf(IonString lhs, IonString rhs)
        {
            if (lhs.IsNull || rhs.IsNull)
                return new IonBool();

            return new IonBool(lhs.Value!.Contains(rhs.Value!));
        }

        private static Atom<IonBool> EndsWith(IonString lhs, IonString rhs)
        {
            if (lhs.IsNull || rhs.IsNull)
                return new IonBool();

            return new IonBool(lhs.Value!.EndsWith(rhs.Value!));
        }

        private static Atom<IonBool> StartsWith(IonString lhs, IonString rhs)
        {
            if (lhs.IsNull || rhs.IsNull)
                return new IonBool();

            return new IonBool(lhs.Value!.StartsWith(rhs.Value!));
        }

        private static Atom<IonBool> Matches(IonString lhs, IonString rhs)
        {
            if (lhs.IsNull || rhs.IsNull)
                return new IonBool();

            var regex = GetOrAdd(rhs.Value!, pattern => new Regex(pattern, RegexOptions.Compiled));
            return new IonBool(regex.IsMatch(lhs.Value!));
        }

        private static Regex GetOrAdd(string pattern, Func<string, Regex> regexProvider)
        {
            if (PatternCache.Contains(pattern))
                return PatternCache.Get(pattern).As<Regex>();

            var regex = regexProvider.Invoke(pattern);
            PatternCache.Add(pattern, regex, new CacheItemPolicy());
            return regex;
        }
        #endregion

        public override string ToString()
        {
            return $"({Subject} {OperatorSymbols[Operation]} {Object})";
        }


        public enum Operators
        {
            /// <summary>
            /// Checks if a string matches a regular expression.
            /// <list type="bullet">
            /// <item>Lhs: the string to match</item>
            /// <item>Rhs: the regular expression pattern (in text form)</item>
            /// </list>
            /// </summary>
            Matches,

            /// <summary>
            /// checks if a string starts contains another string at its begining.
            /// <list type="bullet">
            /// <item>Lhs: the string</item>
            /// <item>Rhs: the substring to check for</item>
            /// </list>
            /// </summary>
            StartsWith,

            /// <summary>
            /// checks if a string starts contains another string at its end.
            /// <list type="bullet">
            /// <item>Lhs: the string</item>
            /// <item>Rhs: the substring to check for</item>
            /// </list>
            /// </summary>
            EndsWith,

            /// <summary>
            /// checks if a string starts contains another string at any position
            /// <list type="bullet">
            /// <item>Lhs: the string to match</item>
            /// <item>Rhs: the substring to check for</item>
            /// </list>
            /// </summary>
            ContainsSubstring
        }
    }
}
