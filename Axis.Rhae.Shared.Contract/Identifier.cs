using Axis.Luna.Common;
using Axis.Luna.Result;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Rhae.Contract
{
    public readonly struct Identifier<TPattern> :
        IDefaultValueProvider<Identifier<TPattern>>,
        IResultParsable<Identifier<TPattern>>,
        IEquatable<Identifier<TPattern>>,
        IValidatable
        where TPattern : IIdentifierPattern
    {
        private readonly string value;

        #region IDefaultValueProvider
        public static Identifier<TPattern> Default => default;

        public bool IsDefault => value is null;
        #endregion

        #region Construction
        public Identifier(string value)
        {
            if (!TPattern.IsValidPattern(value))
                throw new FormatException($"Invalid namespace format: '{value}'");

            this.value = value;
        }

        public static implicit operator Identifier<TPattern>(string value) => new(value);

        public static implicit operator string(Identifier<TPattern> value) => value.ToString();

        #endregion

        #region IResultParsable
        public static bool TryParse(string text, out IResult<Identifier<TPattern>> result)
        {
            result = Result.Of(() => new Identifier<TPattern>(text));
            return result.IsDataResult();
        }

        public static IResult<Identifier<TPattern>> Parse(string text)
        {
            _ = TryParse(text, out var result);
            return result;
        }
        #endregion

        #region IEquatable
        public bool Equals(Identifier<TPattern> other)
        {
            return EqualityComparer<string>.Default.Equals(value, other.value);
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return value switch
            {
                null => "{*}",
                _ => value
            };
        }

        public override int GetHashCode() => HashCode.Combine(value);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Identifier<TPattern> other && Equals(other);
        }

        public static bool operator ==(Identifier<TPattern> left, Identifier<TPattern> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Identifier<TPattern> left, Identifier<TPattern> right)
        {
            return !(left == right);
        }
        #endregion

        public bool IsValid(out ValidationResult[] validationResults)
        {
            validationResults = [];
            return true;
        }
    }
}
