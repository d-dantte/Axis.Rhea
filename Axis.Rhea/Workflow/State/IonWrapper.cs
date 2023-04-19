using Axis.Ion.Types;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Axis.Rhea.Core.Workflow.State
{
    public readonly struct ValueWrapper :
        IComparisonOperators<ValueWrapper, ValueWrapper, bool>
    {
        public IIonType Ion { get; }

        public ValueWrapper(IIonType ion)
        {
            Ion = ion.ThrowIfNull(new ArgumentNullException(nameof(ion)));
        }

        public ValueWrapper(int index)
            :this(new IonInt(index))
        {
        }

        public ValueWrapper(string @string)
            : this(new IonString(@string))
        {
        }


        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is IIonType ion
                && Ion.NullOrEquals(ion);
        }

        public override int GetHashCode() => Ion?.GetHashCode() ?? 0;

        #region transform functions
        public ValueWrapper ToLower()
        {
            return Ion switch
            {
                IonString str => new IonString(str.Value?.ToLower()),
                IonIdentifier id => new IonIdentifier(id.Value?.ToLower()),
                IonQuotedSymbol quote => new IonQuotedSymbol(quote.Value?.ToLower()),
                _ => throw new InvalidOperationException($"'ToLower' transformation cannot be performed on the type: {Ion.Type}")
            };
        }

        public ValueWrapper ToUpper()
        {
            return Ion switch
            {
                IonString str => new IonString(str.Value?.ToUpper()),
                IonIdentifier id => new IonIdentifier(id.Value?.ToUpper()),
                IonQuotedSymbol quote => new IonQuotedSymbol(quote.Value?.ToUpper()),
                _ => throw new InvalidOperationException($"'ToUpper' transformation cannot be performed on the type: {Ion.Type}")
            };
        }

        public ValueWrapper Reverse()
        {
            return Ion switch
            {
                IonString str => new IonString(str.Value?.Reverse().JoinUsing("")),
                IonIdentifier id => new IonIdentifier(id.Value?.Reverse().JoinUsing("")),
                IonQuotedSymbol quote => new IonQuotedSymbol(quote.Value?.Reverse().JoinUsing("")),

                IonList list => new IonList(
                    new IonList.Initializer(
                        list.Annotations,
                        list.Value?.Reverse().ToArray() ?? throw new ArgumentNullException())),

                IonSexp sexp => new IonSexp(
                    new IonSexp.Initializer(
                        sexp.Annotations,
                        sexp.Value?.Reverse().ToArray() ?? throw new ArgumentNullException())),

                _ => throw new InvalidOperationException($"'Reverse' transformation cannot be performed on the type: {Ion.Type}")
            };
        }

        public ValueWrapper Count()
        {
            return Ion switch
            {
                IonString str => new ValueWrapper(str.Value?.Length ?? throw new InvalidOperationException("Count called on null value")),
                IonIdentifier id => new ValueWrapper(id.Value?.Length ?? throw new InvalidOperationException("Count called on null value")),
                IonQuotedSymbol quote => new ValueWrapper(quote.Value?.Length ?? throw new InvalidOperationException("Count called on null value")),
                IonClob clob => new ValueWrapper(clob.Value?.Length ?? throw new InvalidOperationException("Count called on null value")),
                IonBlob blob => new ValueWrapper(blob.Value?.Length ?? throw new InvalidOperationException("Count called on null value")),
                IonList list => new ValueWrapper(list.Value?.Length ?? throw new InvalidOperationException("Count called on null value")),
                IonSexp sexp => new ValueWrapper(sexp.Value?.Length ?? throw new InvalidOperationException("Count called on null value")),
                IonStruct @struct => new ValueWrapper(@struct.Value?.Length ?? throw new InvalidOperationException("Count called on null value")),
                _ => throw new InvalidOperationException($"'Count' transformation cannot be performed on the type: {Ion.Type}")
            };
        }

        public ValueWrapper Negation()
        {
            return Ion switch
            {
                IonBool @bool => !(@bool.Value ?? throw new InvalidOperationException("Cannot negate a null boolean")),
                IonInt @int => ~(@int.Value ?? throw new InvalidOperationException("Cannot negate a null integer")),
                _ => throw new InvalidOperationException($"This transformation cannot be performed on the type: {Ion.Type}")
            };
        }

        public ValueWrapper IsNull() => Ion.IsNull;

        public ValueWrapper IsNotNull() => !Ion.IsNull;

        #endregion

        #region relational comparison functions
        public bool In(params ValueWrapper[] values)
        {
            var _ion = Ion;
            return values.Any(value => value.Ion.Equals(_ion));
        }

        public bool NotIn(params ValueWrapper[] values)
        {
            var _ion = Ion;
            return values.All(value => !value.Ion.Equals(_ion));
        }

        public bool Between(ValueWrapper first, ValueWrapper second)
        {
            return this >= first && this <= second;
        }

        public bool NotBetween(ValueWrapper first, ValueWrapper second)
        {
            return this < first || this > second;
        }

        public bool IsType(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return Ion.GetType().Equals(type);
        }

        public bool IsNotType(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return !Ion.GetType().Equals(type);
        }

        public bool StartsWith(ValueWrapper value)
        {
            return Ion switch
            {
                IRefValue<string> operand => value.Ion switch
                {
                    IRefValue<string> sub => operand.Value?.StartsWith(sub.Value) ?? false,
                    _ => throw new ArgumentException($"Invalid value type: {value.Ion.Type}")
                },
                _ => throw new InvalidOperationException($"Cannot perform the 'starts with' operation on type: {Ion.Type}")
            };
        }

        public bool EndsWith(ValueWrapper value)
        {
            return Ion switch
            {
                IRefValue<string> operand => value.Ion switch
                {
                    IRefValue<string> sub => operand.Value?.EndsWith(sub.Value) ?? false,
                    _ => throw new ArgumentException($"Invalid value type: {value.Ion.Type}")
                },
                _ => throw new InvalidOperationException($"Cannot perform the 'ends with' operation on type: {Ion.Type}")
            };
        }

        public bool Matches(ValueWrapper value)
        {
            return Ion switch
            {
                IRefValue<string> operand => value.Ion switch
                {
                    IRefValue<string> sub => new Regex(sub.Value).IsMatch(operand.Value),
                    _ => throw new ArgumentException($"Invalid value type: {value.Ion.Type}")
                },
                _ => throw new InvalidOperationException($"Cannot perform the operation on type: {Ion.Type}")
            };
        }
        #endregion

        #region logical operator functions
        public bool Xor(ValueWrapper value)
        {
            return (Ion, value.Ion) switch
            {
                (IonBool left, IonBool right) =>
                    (left.Value ?? throw new ArgumentNullException())
                    ^ (right.Value ?? throw new ArgumentNullException()),

                _ => throw new ArgumentException($"Cannot perform xor on {Ion.Type} and {value.Ion.Type}")
            };
        }
        public bool Nor(ValueWrapper value)
        {
            return (Ion, value.Ion) switch
            {
                (IonBool left, IonBool right) =>
                    !(left.Value ?? throw new ArgumentNullException())
                    && !(right.Value ?? throw new ArgumentNullException()),

                _ => throw new ArgumentException($"Cannot perform xor on {Ion.Type} and {value.Ion.Type}")
            };
        }
        #endregion

        #region implicits operators
        public static implicit operator ValueWrapper(IonNull ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonBool ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonInt ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonFloat ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonDecimal ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonTimestamp ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonString ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonIdentifier ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonQuotedSymbol ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonOperator ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonClob ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonBlob ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonList ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonSexp ion) => new ValueWrapper(ion);
        public static implicit operator ValueWrapper(IonStruct ion) => new ValueWrapper(ion);

        public static implicit operator ValueWrapper(BigInteger bigInt) => new ValueWrapper(new IonInt(bigInt));
        public static implicit operator ValueWrapper(long value) => new ValueWrapper(new IonInt(value));
        public static implicit operator ValueWrapper(decimal value) => new ValueWrapper(new IonDecimal(value));
        public static implicit operator ValueWrapper(double value) => new ValueWrapper(new IonFloat(value));
        public static implicit operator ValueWrapper(string value) => new ValueWrapper(new IonString(value));
        public static implicit operator ValueWrapper(bool value) => new ValueWrapper(new IonBool(value));
        public static implicit operator ValueWrapper(DateTimeOffset value) => new ValueWrapper(new IonTimestamp(value));
        #endregion

        #region Generic comparison
        public static bool operator >(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                return false;

            return (left.Ion, right.Ion) switch
            {
                #region Int > *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue > rvalue,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rint}'")
                },

                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => CompareTo(lvalue, rvalue) > 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rfloat}'")
                },

                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => CompareTo(lvalue, rvalue) > 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rdecimal}'")
                },
                #endregion

                #region Double > *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) < 0,
                    _ => throw new Exception($"Invalid comparison between '{lfloat}' and '{rint}'")
                },

                (IonFloat ldouble, IonFloat rfloat) => (ldouble.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue > rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rfloat}'")
                },

                (IonFloat ldouble, IonDecimal rdecimal) => (ldouble.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => CompareTo(rvalue, lvalue) < 0,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rdecimal}'")
                },
                #endregion

                #region Decimal > *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) < 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rint}'")
                },

                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => CompareTo(lvalue, rvalue) > 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rfloat}'")
                },

                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue > rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rdecimal}'")
                },
                #endregion

                #region Datetime > Datetime
                (IonTimestamp timestamp1, IonTimestamp timestamp2) => (timestamp1.Value, timestamp2.Value) switch
                {
                    (DateTimeOffset lvalue, DateTimeOffset rvalue) => lvalue > rvalue,
                    _ => throw new Exception($"Invalid comparison between '{timestamp1}' and '{timestamp2}'")
                },
                #endregion

                #region String|Symbol > *
                (IRefValue<string> ionString1, IRefValue<string> ionString2) => (ionString1.Value, ionString2.Value) switch
                {
                    (string lvalue, string rvalue) => lvalue.CompareTo(rvalue) > 0,
                    _ => throw new Exception($"Invalid comparison between '{ionString1}' and '{ionString2}'")
                },
                #endregion

                _ => throw new Exception($"Invalid comparison between '{left.Ion}' and '{right.Ion}'")
            };
        }

        public static bool operator >=(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                return false;

            return (left.Ion, right.Ion) switch
            {
                #region Int >= *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue >= rvalue,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rint}'")
                },

                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => CompareTo(lvalue, rvalue) >= 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rfloat}'")
                },

                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => CompareTo(lvalue, rvalue) >= 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rdecimal}'")
                },
                #endregion

                #region Double >= *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) <= 0,
                    _ => throw new Exception($"Invalid comparison between '{lfloat}' and '{rint}'")
                },

                (IonFloat ldouble, IonFloat rfloat) => (ldouble.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue >= rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rfloat}'")
                },

                (IonFloat ldouble, IonDecimal rdecimal) => (ldouble.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => CompareTo(rvalue, lvalue) <= 0,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rdecimal}'")
                },
                #endregion

                #region Decimal >= *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) <= 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rint}'")
                },

                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => CompareTo(lvalue, rvalue) >= 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rfloat}'")
                },

                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue >= rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rdecimal}'")
                },
                #endregion

                #region Datetime >= Datetime
                (IonTimestamp timestamp1, IonTimestamp timestamp2) => (timestamp1.Value, timestamp2.Value) switch
                {
                    (DateTimeOffset lvalue, DateTimeOffset rvalue) => lvalue >= rvalue,
                    _ => throw new Exception($"Invalid comparison between '{timestamp1}' and '{timestamp2}'")
                },
                #endregion

                #region String|Symbol >= *
                (IRefValue<string> ionString1, IRefValue<string> ionString2) => (ionString1.Value, ionString2.Value) switch
                {
                    (string lvalue, string rvalue) => lvalue.CompareTo(rvalue) >= 0,
                    _ => throw new Exception($"Invalid comparison between '{ionString1}' and '{ionString2}'")
                },
                #endregion

                _ => throw new Exception($"Invalid comparison between '{left.Ion}' and '{right.Ion}'")
            };
        }

        public static bool operator <(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                return false;

            return (left.Ion, right.Ion) switch
            {
                #region Int < *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue < rvalue,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rint}'")
                },

                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => CompareTo(lvalue, rvalue) < 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rfloat}'")
                },

                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => CompareTo(lvalue, rvalue) < 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rdecimal}'")
                },
                #endregion

                #region Double < *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) > 0,
                    _ => throw new Exception($"Invalid comparison between '{lfloat}' and '{rint}'")
                },

                (IonFloat ldouble, IonFloat rfloat) => (ldouble.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue < rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rfloat}'")
                },

                (IonFloat ldouble, IonDecimal rdecimal) => (ldouble.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => CompareTo(rvalue, lvalue) > 0,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rdecimal}'")
                },
                #endregion

                #region Decimal < *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) > 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rint}'")
                },

                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => CompareTo(lvalue, rvalue) < 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rfloat}'")
                },

                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue < rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rdecimal}'")
                },
                #endregion

                #region Datetime < Datetime
                (IonTimestamp timestamp1, IonTimestamp timestamp2) => (timestamp1.Value, timestamp2.Value) switch
                {
                    (DateTimeOffset lvalue, DateTimeOffset rvalue) => lvalue < rvalue,
                    _ => throw new Exception($"Invalid comparison between '{timestamp1}' and '{timestamp2}'")
                },
                #endregion

                #region String|Symbol < *
                (IRefValue<string> ionString1, IRefValue<string> ionString2) => (ionString1.Value, ionString2.Value) switch
                {
                    (string lvalue, string rvalue) => lvalue.CompareTo(rvalue) < 0,
                    _ => throw new Exception($"Invalid comparison between '{ionString1}' and '{ionString2}'")
                },
                #endregion

                _ => throw new Exception($"Invalid comparison between '{left.Ion}' and '{right.Ion}'")
            };
        }

        public static bool operator <=(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                return false;

            return (left.Ion, right.Ion) switch
            {
                #region Int <= *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue <= rvalue,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rint}'")
                },

                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => CompareTo(lvalue, rvalue) <= 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rfloat}'")
                },

                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => CompareTo(lvalue, rvalue) <= 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rdecimal}'")
                },
                #endregion

                #region Double <= *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) >= 0,
                    _ => throw new Exception($"Invalid comparison between '{lfloat}' and '{rint}'")
                },

                (IonFloat ldouble, IonFloat rfloat) => (ldouble.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue <= rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rfloat}'")
                },

                (IonFloat ldouble, IonDecimal rdecimal) => (ldouble.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => CompareTo(rvalue, lvalue) >= 0,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rdecimal}'")
                },
                #endregion

                #region Decimal <= *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) >= 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rint}'")
                },

                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => CompareTo(lvalue, rvalue) <= 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rfloat}'")
                },

                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue <= rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rdecimal}'")
                },
                #endregion

                #region Datetime <= Datetime
                (IonTimestamp timestamp1, IonTimestamp timestamp2) => (timestamp1.Value, timestamp2.Value) switch
                {
                    (DateTimeOffset lvalue, DateTimeOffset rvalue) => lvalue <= rvalue,
                    _ => throw new Exception($"Invalid comparison between '{timestamp1}' and '{timestamp2}'")
                },
                #endregion

                #region String|Symbol <= *
                (IRefValue<string> ionString1, IRefValue<string> ionString2) => (ionString1.Value, ionString2.Value) switch
                {
                    (string lvalue, string rvalue) => lvalue.CompareTo(rvalue) <= 0,
                    _ => throw new Exception($"Invalid comparison between '{ionString1}' and '{ionString2}'")
                },
                #endregion

                _ => throw new Exception($"Invalid comparison between '{left.Ion}' and '{right.Ion}'")
            };
        }

        public static bool operator ==(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                return false;

            return (left.Ion, right.Ion) switch
            {
                #region Int == *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue == rvalue,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rint}'")
                },

                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => CompareTo(lvalue, rvalue) == 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rfloat}'")
                },

                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => CompareTo(lvalue, rvalue) == 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rdecimal}'")
                },
                #endregion

                #region Double == *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) == 0,
                    _ => throw new Exception($"Invalid comparison between '{lfloat}' and '{rint}'")
                },

                (IonFloat ldouble, IonFloat rfloat) => (ldouble.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue == rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rfloat}'")
                },

                (IonFloat ldouble, IonDecimal rdecimal) => (ldouble.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => CompareTo(rvalue, lvalue) == 0,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rdecimal}'")
                },
                #endregion

                #region Decimal == *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) == 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rint}'")
                },

                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => CompareTo(lvalue, rvalue) == 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rfloat}'")
                },

                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue == rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rdecimal}'")
                },
                #endregion

                #region Datetime == Datetime
                (IonTimestamp timestamp1, IonTimestamp timestamp2) => (timestamp1.Value, timestamp2.Value) switch
                {
                    (DateTimeOffset lvalue, DateTimeOffset rvalue) => lvalue == rvalue,
                    _ => throw new Exception($"Invalid comparison between '{timestamp1}' and '{timestamp2}'")
                },
                #endregion

                #region String|Symbol == *
                (IRefValue<string> ionString1, IRefValue<string> ionString2) => (ionString1.Value, ionString2.Value) switch
                {
                    (string lvalue, string rvalue) => lvalue.CompareTo(rvalue) == 0,
                    _ => throw new Exception($"Invalid comparison between '{ionString1}' and '{ionString2}'")
                },
                #endregion

                #region bool == bool
                (IonBool ionBool1, IonBool ionBool2) => (ionBool1.Value, ionBool2.Value) switch
                {
                    (bool lvalue, bool rvalue) => lvalue == rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ionBool1}' and '{ionBool2}'")
                },
                #endregion

                _ => throw new Exception($"Invalid comparison between '{left.Ion}' and '{right.Ion}'")
            };
        }

        public static bool operator !=(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                return false;

            return (left.Ion, right.Ion) switch
            {
                #region Int != *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue != rvalue,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rint}'")
                },

                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => CompareTo(lvalue, rvalue) != 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rfloat}'")
                },

                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => CompareTo(lvalue, rvalue) != 0,
                    _ => throw new Exception($"Invalid comparison between '{lint}' and '{rdecimal}'")
                },
                #endregion

                #region Double != *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) != 0,
                    _ => throw new Exception($"Invalid comparison between '{lfloat}' and '{rint}'")
                },

                (IonFloat ldouble, IonFloat rfloat) => (ldouble.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue != rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rfloat}'")
                },

                (IonFloat ldouble, IonDecimal rdecimal) => (ldouble.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => CompareTo(rvalue, lvalue) != 0,
                    _ => throw new Exception($"Invalid comparison between '{ldouble}' and '{rdecimal}'")
                },
                #endregion

                #region Decimal != *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => CompareTo(rvalue, lvalue) != 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rint}'")
                },

                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => CompareTo(lvalue, rvalue) != 0,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rfloat}'")
                },

                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue != rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ldecimal}' and '{rdecimal}'")
                },
                #endregion

                #region Datetime != Datetime
                (IonTimestamp timestamp1, IonTimestamp timestamp2) => (timestamp1.Value, timestamp2.Value) switch
                {
                    (DateTimeOffset lvalue, DateTimeOffset rvalue) => lvalue != rvalue,
                    _ => throw new Exception($"Invalid comparison between '{timestamp1}' and '{timestamp2}'")
                },
                #endregion

                #region String|Symbol != *
                (IRefValue<string> ionString1, IRefValue<string> ionString2) => (ionString1.Value, ionString2.Value) switch
                {
                    (string lvalue, string rvalue) => lvalue.CompareTo(rvalue) != 0,
                    _ => throw new Exception($"Invalid comparison between '{ionString1}' and '{ionString2}'")
                },
                #endregion

                #region bool != bool
                (IonBool ionBool1, IonBool ionBool2) => (ionBool1.Value, ionBool2.Value) switch
                {
                    (bool lvalue, bool rvalue) => lvalue != rvalue,
                    _ => throw new Exception($"Invalid comparison between '{ionBool1}' and '{ionBool2}'")
                },
                #endregion

                _ => throw new Exception($"Invalid comparison between '{left.Ion}' and '{right.Ion}'")
            };
        }
        #endregion

        #region Generic Arithmetic
        public static ValueWrapper operator +(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                throw new ArgumentException($"Invalid argument(s). left: {left.Ion}, right: {right.Ion}");

            return (left.Ion, right.Ion) switch
            {
                #region Int + *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue + rvalue,
                    _ => throw new Exception($"Invalid addition between {lint.Value} and {rint.Value}")
                },
                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => (long)lvalue + rvalue,
                    _ => throw new Exception($"Invalid addition between {lint.Value} and {rfloat.Value}")
                },
                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => (long)lvalue + rvalue,
                    _ => throw new Exception($"Invalid addition between {lint.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Float + *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => lvalue + (double)rvalue,
                    _ => throw new Exception($"Invalid addition between {lfloat.Value} and {rint.Value}")
                },
                (IonFloat lfloat, IonFloat rfloat) => (lfloat.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue + rvalue,
                    _ => throw new Exception($"Invalid addition between {lfloat.Value} and {rfloat.Value}")
                },
                (IonFloat lfloat, IonDecimal rdecimal) => (lfloat.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => lvalue + (double)rvalue,
                    _ => throw new Exception($"Invalid addition between {lfloat.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Decimal + *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => lvalue + (decimal)rvalue,
                    _ => throw new Exception($"Invalid addition between {ldecimal.Value} and {rint.Value}")
                },
                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => lvalue + (decimal)rvalue,
                    _ => throw new Exception($"Invalid addition between {ldecimal.Value} and {rfloat.Value}")
                },
                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue + rvalue,
                    _ => throw new Exception($"Invalid addition between {ldecimal.Value} and {rdecimal.Value}")
                },
                #endregion

                _ => throw new Exception($"Invalid addition between {left.Ion} and {right.Ion}")
            };
        }

        public static ValueWrapper operator -(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                throw new ArgumentException($"Invalid argument(s). left: {left.Ion}, right: {right.Ion}");

            return (left.Ion, right.Ion) switch
            {
                #region Int - *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue - rvalue,
                    _ => throw new Exception($"Invalid subtraction between {lint.Value} and {rint.Value}")
                },
                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => (long)lvalue - rvalue,
                    _ => throw new Exception($"Invalid subtraction between {lint.Value} and {rfloat.Value}")
                },
                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => (long)lvalue - rvalue,
                    _ => throw new Exception($"Invalid subtraction between {lint.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Float - *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => lvalue - (double)rvalue,
                    _ => throw new Exception($"Invalid subtraction between {lfloat.Value} and {rint.Value}")
                },
                (IonFloat lfloat, IonFloat rfloat) => (lfloat.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue - rvalue,
                    _ => throw new Exception($"Invalid subtraction between {lfloat.Value} and {rfloat.Value}")
                },
                (IonFloat lfloat, IonDecimal rdecimal) => (lfloat.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => lvalue - (double)rvalue,
                    _ => throw new Exception($"Invalid subtraction between {lfloat.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Decimal - *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => lvalue - (decimal)rvalue,
                    _ => throw new Exception($"Invalid subtraction between {ldecimal.Value} and {rint.Value}")
                },
                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => lvalue - (decimal)rvalue,
                    _ => throw new Exception($"Invalid subtraction between {ldecimal.Value} and {rfloat.Value}")
                },
                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue - rvalue,
                    _ => throw new Exception($"Invalid subtraction between {ldecimal.Value} and {rdecimal.Value}")
                },
                #endregion

                _ => throw new Exception($"Invalid subtraction between {left.Ion} and {right.Ion}")
            };
        }

        public static ValueWrapper operator *(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                throw new ArgumentException($"Invalid argument(s). left: {left.Ion}, right: {right.Ion}");

            return (left.Ion, right.Ion) switch
            {
                #region Int * *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue * rvalue,
                    _ => throw new Exception($"Invalid multiplication between {lint.Value} and {rint.Value}")
                },
                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => (long)lvalue * rvalue,
                    _ => throw new Exception($"Invalid multiplication between {lint.Value} and {rfloat.Value}")
                },
                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => (long)lvalue * rvalue,
                    _ => throw new Exception($"Invalid multiplication between {lint.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Float * *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => lvalue * (double)rvalue,
                    _ => throw new Exception($"Invalid multiplication between {lfloat.Value} and {rint.Value}")
                },
                (IonFloat lfloat, IonFloat rfloat) => (lfloat.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue * rvalue,
                    _ => throw new Exception($"Invalid multiplication between {lfloat.Value} and {rfloat.Value}")
                },
                (IonFloat lfloat, IonDecimal rdecimal) => (lfloat.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => lvalue * (double)rvalue,
                    _ => throw new Exception($"Invalid multiplication between {lfloat.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Decimal * *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => lvalue * (decimal)rvalue,
                    _ => throw new Exception($"Invalid multiplication between {ldecimal.Value} and {rint.Value}")
                },
                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => lvalue * (decimal)rvalue,
                    _ => throw new Exception($"Invalid multiplication between {ldecimal.Value} and {rfloat.Value}")
                },
                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue * rvalue,
                    _ => throw new Exception($"Invalid multiplication between {ldecimal.Value} and {rdecimal.Value}")
                },
                #endregion

                _ => throw new Exception($"Invalid multiplication between {left.Ion} and {right.Ion}")
            };
        }

        public static ValueWrapper operator /(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                throw new ArgumentException($"Invalid argument(s). left: {left.Ion}, right: {right.Ion}");

            return (left.Ion, right.Ion) switch
            {
                #region Int / *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue / rvalue,
                    _ => throw new Exception($"Invalid division between {lint.Value} and {rint.Value}")
                },
                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => (long)lvalue / rvalue,
                    _ => throw new Exception($"Invalid division between {lint.Value} and {rfloat.Value}")
                },
                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => (long)lvalue / rvalue,
                    _ => throw new Exception($"Invalid division between {lint.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Float / *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => lvalue / (double)rvalue,
                    _ => throw new Exception($"Invalid division between {lfloat.Value} and {rint.Value}")
                },
                (IonFloat lfloat, IonFloat rfloat) => (lfloat.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue / rvalue,
                    _ => throw new Exception($"Invalid division between {lfloat.Value} and {rfloat.Value}")
                },
                (IonFloat lfloat, IonDecimal rdecimal) => (lfloat.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => lvalue / (double)rvalue,
                    _ => throw new Exception($"Invalid division between {lfloat.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Decimal / *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => lvalue / (decimal)rvalue,
                    _ => throw new Exception($"Invalid division between {ldecimal.Value} and {rint.Value}")
                },
                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => lvalue / (decimal)rvalue,
                    _ => throw new Exception($"Invalid division between {ldecimal.Value} and {rfloat.Value}")
                },
                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue / rvalue,
                    _ => throw new Exception($"Invalid division between {ldecimal.Value} and {rdecimal.Value}")
                },
                #endregion

                _ => throw new Exception($"Invalid division between {left.Ion} and {right.Ion}")
            };
        }

        public static ValueWrapper operator %(ValueWrapper left, ValueWrapper right)
        {
            if (left.Ion.IsNull || right.Ion.IsNull)
                throw new ArgumentException($"Invalid argument(s). left: {left.Ion}, right: {right.Ion}");

            return (left.Ion, right.Ion) switch
            {
                #region Int % *
                (IonInt lint, IonInt rint) => (lint.Value, rint.Value) switch
                {
                    (BigInteger lvalue, BigInteger rvalue) => lvalue % rvalue,
                    _ => throw new Exception($"Invalid modulo between {lint.Value} and {rint.Value}")
                },
                (IonInt lint, IonFloat rfloat) => (lint.Value, rfloat.Value) switch
                {
                    (BigInteger lvalue, double rvalue) => (long)lvalue % rvalue,
                    _ => throw new Exception($"Invalid modulo between {lint.Value} and {rfloat.Value}")
                },
                (IonInt lint, IonDecimal rdecimal) => (lint.Value, rdecimal.Value) switch
                {
                    (BigInteger lvalue, decimal rvalue) => (long)lvalue % rvalue,
                    _ => throw new Exception($"Invalid modulo between {lint.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Float % *
                (IonFloat lfloat, IonInt rint) => (lfloat.Value, rint.Value) switch
                {
                    (double lvalue, BigInteger rvalue) => lvalue % (double)rvalue,
                    _ => throw new Exception($"Invalid modulo between {lfloat.Value} and {rint.Value}")
                },
                (IonFloat lfloat, IonFloat rfloat) => (lfloat.Value, rfloat.Value) switch
                {
                    (double lvalue, double rvalue) => lvalue % rvalue,
                    _ => throw new Exception($"Invalid modulo between {lfloat.Value} and {rfloat.Value}")
                },
                (IonFloat lfloat, IonDecimal rdecimal) => (lfloat.Value, rdecimal.Value) switch
                {
                    (double lvalue, decimal rvalue) => lvalue % (double)rvalue,
                    _ => throw new Exception($"Invalid modulo between {lfloat.Value} and {rdecimal.Value}")
                },
                #endregion

                #region Decimal % *
                (IonDecimal ldecimal, IonInt rint) => (ldecimal.Value, rint.Value) switch
                {
                    (decimal lvalue, BigInteger rvalue) => lvalue % (decimal)rvalue,
                    _ => throw new Exception($"Invalid modulo between {ldecimal.Value} and {rint.Value}")
                },
                (IonDecimal ldecimal, IonFloat rfloat) => (ldecimal.Value, rfloat.Value) switch
                {
                    (decimal lvalue, double rvalue) => lvalue % (decimal)rvalue,
                    _ => throw new Exception($"Invalid modulo between {ldecimal.Value} and {rfloat.Value}")
                },
                (IonDecimal ldecimal, IonDecimal rdecimal) => (ldecimal.Value, rdecimal.Value) switch
                {
                    (decimal lvalue, decimal rvalue) => lvalue % rvalue,
                    _ => throw new Exception($"Invalid modulo between {ldecimal.Value} and {rdecimal.Value}")
                },
                #endregion

                _ => throw new Exception($"Invalid modulo between {left.Ion} and {right.Ion}")
            };
        }

        public static ValueWrapper operator +(ValueWrapper left, TimeSpan right)
        {
            if (left.Ion.IsNull)
                throw new ArgumentException($"Invalid argument: {left.Ion}");

            return left.Ion switch
            {
                IonTimestamp timestamp => new IonTimestamp(timestamp.Value + right),
                _ => throw new Exception($"Invalid addition between {left.Ion} and {right}")
            };
        }

        public static ValueWrapper operator -(ValueWrapper left, TimeSpan right)
        {
            if (left.Ion.IsNull)
                throw new ArgumentException($"Invalid argument: {left.Ion}");

            return left.Ion switch
            {
                IonTimestamp timestamp => new IonTimestamp(timestamp.Value - right),
                _ => throw new Exception($"Invalid addition between {left.Ion} and {right}")
            };
        }
        #endregion

        #region helpers
        private static int CompareTo(BigInteger @int, double @double)
        {
            var dint = (BigInteger)@double;
            var fractional = @double - double.Truncate(@double);

            if (fractional == 0)
            {
                if (@int == dint)
                    return 0;

                if (@int < dint)
                    return -1;

                // if (@int > dint)
                return 1;
            }
            else // fractional > 0
            {
                if (@int > dint)
                    return 1;

                if (@int < dint)
                    return -1;

                //if (@int == dint)
                return -1;
            }
        }

        private static int CompareTo(BigInteger @int, decimal @decimal)
        {
            var dint = (BigInteger)@decimal;
            var fractional = @decimal - decimal.Truncate(@decimal);

            if (fractional == 0)
            {
                if (@int == dint)
                    return 0;

                if (@int < dint)
                    return -1;

                // if (@int > dint)
                return 1;
            }
            else // fractional > 0
            {
                if (@int > dint)
                    return 1;

                if (@int < dint)
                    return -1;

                //if (@int == dint)
                return -1;
            }
        }

        private static int CompareTo(decimal @decimal, double @double)
        {
            var mint = (BigInteger)@decimal;
            var mfractional = @decimal - decimal.Truncate(@decimal);

            var dint = (BigInteger)@double;
            var dfractional = @double - double.Truncate(@double);

            if (mint < dint)
                return -1;

            if (mint > dint)
                return 1;

            else return ((double)mfractional).CompareTo(dfractional);
        }
        #endregion

    }
}
