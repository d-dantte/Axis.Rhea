using Axis.Ion.Types;
using Axis.Luna.Extensions;
using static Axis.Luna.Extensions.Common;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Ion
{
    public readonly struct IonDuration : IStructValue<TimeSpan>, IIonDeepCopyable<IonDuration>
    {
        private readonly IIonType.Annotation[] _annotations;

        public TimeSpan? Value { get; }

        public IonTypes Type => (IonTypes)16;

        public IIonType.Annotation[] Annotations => _annotations?.ToArray() ?? Array.Empty<IIonType.Annotation>();

        public IonDuration(TimeSpan? value, params IIonType.Annotation[] annotations)
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
        public static IonDuration Null(params IIonType.Annotation[] annotations) => new IonDuration(null, annotations);

        #region IIonType

        public bool IsNull => Value == null;

        public bool ValueEquals(IStructValue<TimeSpan> other) => Value == other?.Value == true;

        public string ToIonText()
        {
            if (IsNull)
                return "null.duration";

            return $"'D {Value}'";
        }

        #endregion

        #region Record Implementation
        public override int GetHashCode()
            => HashCode.Combine(Value, ValueHash(Annotations.HardCast<IIonType.Annotation, object>()));

        public override bool Equals(object? obj)
        {
            return obj is IonDuration other
                && other.ValueEquals(this)
                && other.Annotations.SequenceEqual(Annotations);
        }

        public override string ToString() => Annotations
            .Select(a => a.ToString())
            .Concat(ToIonText())
            .JoinUsing("");


        public static bool operator ==(IonDuration first, IonDuration second) => first.Equals(second);

        public static bool operator !=(IonDuration first, IonDuration second) => !first.Equals(second);

        #endregion

        #region IIonDeepCopy<>
        IIonType IIonDeepCopyable<IIonType>.DeepCopy() => DeepCopy();

        public IonDuration DeepCopy() => new IonDuration(Value, Annotations);
        #endregion

        public static implicit operator IonDuration(TimeSpan? value) => new IonDuration(value);
    }
}
