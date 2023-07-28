using Axis.Ion.Numerics;
using Axis.Ion.Types;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Extensions;
using System.Text.RegularExpressions;
using static Axis.Luna.Extensions.Common;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Ion
{
    public readonly struct IonNumber : IStructValue<BigDecimal>, IIonDeepCopyable<IonNumber>, INumericType
    {
        internal static readonly Regex DeconstructedPattern = new Regex(
            "^\\[Mantissa\\:\\s*(?'mantissa'[\\+\\-]?\\d+),\\s*Scale\\:\\s*(?'scale'\\d+)\\]$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private readonly IIonType.Annotation[] _annotations;

        public BigDecimal? Value { get; }

        public IonTypes Type => IonTypes.Decimal;

        public IIonType.Annotation[] Annotations => _annotations?.ToArray() ?? Array.Empty<IIonType.Annotation>();

        public IonNumber(BigDecimal? value, params IIonType.Annotation[] annotations)
        {
            Value = value;
            _annotations = annotations
                .ThrowIfNull(new ArgumentNullException(nameof(annotations)))
                .ThrowIfAny(ann => default == ann, new ArgumentException($"{nameof(annotations)} must not contain default values"))
                .ToArray();
        }

        /// <summary>
        /// Creates a null instance of the <see cref="IonDecimal"/>
        /// </summary>
        /// <param name="annotations">any available annotation</param>
        /// <returns>The newly created null instance</returns>
        public static IonNumber Null(params IIonType.Annotation[] annotations) => new IonNumber(null, annotations);

        #region IIonType

        public bool IsNull => Value == null;

        public bool ValueEquals(IStructValue<BigDecimal> other) => Value == other?.Value == true;

        public string ToIonText()
        {
            if (IsNull)
                return "null.decimal";

            string componentText = Value!.Value.ToString();
            var match = DeconstructedPattern.Match(componentText);
            var mantissa = match.Groups["mantissa"].Value;
            var scale = int.Parse(match.Groups["scale"].Value);

            if (scale == 0)
                return mantissa;

            if (mantissa.Length > scale)
                return mantissa.InsertAt(mantissa.Length - scale, '.').AsString();

            else return $"0.{mantissa.PadLeft(scale, '0')}";
        }

        #endregion

        #region INumericType
        public BigDecimal? ToBigDecimal() => Value;
        #endregion

        #region Record Implementation
        public override int GetHashCode()
            => HashCode.Combine(Value, ValueHash(Annotations.HardCast<IIonType.Annotation, object>()));

        public override bool Equals(object? obj)
        {
            return obj is IonNumber other
                && other.ValueEquals(this)
                && other.Annotations.SequenceEqual(Annotations);
        }

        public override string ToString() => Annotations
            .Select(a => a.ToString())
            .Concat(ToIonText())
            .JoinUsing("");


        public static bool operator ==(IonNumber first, IonNumber second) => first.Equals(second);

        public static bool operator !=(IonNumber first, IonNumber second) => !first.Equals(second);

        #endregion

        #region IIonDeepCopy<>
        IIonType IIonDeepCopyable<IIonType>.DeepCopy() => DeepCopy();

        public IonNumber DeepCopy() => new IonNumber(Value, Annotations);
        #endregion

        public static implicit operator IonNumber(BigDecimal? value) => new IonNumber(value);
    }
}
