using Axis.Ion.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Collections;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;
using System.Runtime.ExceptionServices;

namespace Axis.Rhea.Shared.Contract
{
    internal static class Extensions
    {
        public static IEnumerable<TItem> ThrowIfAny<TItem>(this
            IEnumerable<TItem> items,
            Func<TItem, bool> predicate,
            Exception exception)
            => items.ThrowIfAny(predicate, item => exception);

        public static IEnumerable<TItem> ThrowIfAny<TItem>(this
            IEnumerable<TItem> items,
            Func<TItem, bool> predicate,
            Func<TItem, Exception> exceptionProvider)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (exceptionProvider is null)
                throw new ArgumentNullException(nameof(exceptionProvider));

            foreach (var item in items)
            {
                if (predicate.Invoke(item))
                    exceptionProvider.Invoke(item).Throw();

                yield return item;
            }
        }

        public static IEnumerable<TItem> ThrowIfNone<TItem>(this
            IEnumerable<TItem> items,
            Func<TItem, bool> predicate,
            Exception? exception = null)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var found = false;
            foreach (var item in items)
            {
                if (predicate.Invoke(item))
                    found = true;

                yield return item;
            }

            if (!found)
            {
                exception ??= new Exception("No element matched the predicate");

                if (exception.StackTrace == null) throw exception;
                else ExceptionDispatchInfo.Capture(exception).Throw();
            }
        }

        public static T ThrowIfNot<T>(this
            T value,
            Func<T, bool> predicate,
            Exception exception)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (exception is null)
                throw new ArgumentNullException(nameof(exception));

            if (!predicate.Invoke(value))
            {
                if (exception.StackTrace == null) throw exception;
                else ExceptionDispatchInfo.Capture(exception).Throw();
            }

            return value;
        }

        /// <summary>
        /// Casts the given <see cref="IEnumerable"/> items into the supplied type
        /// </summary>
        /// <typeparam name="TOut">The type to be casted into</typeparam>
        /// <param name="items">the items</param>
        /// <returns>An enumerable with items casted to the supplied <typeparamref name="TOut"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IEnumerable<TOut> SelectAs<TOut>(this IEnumerable items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                yield return (TOut)item;
            }
        }

        internal static List<T> AddOrInsert<T>(this List<T> items, T item, int? index = null)
        {
            if (index is null || index >= items.Count)
                items.Add(item);

            else items[index.Value] = item;

            return items;
        }

        internal static string AsString(this IEnumerable<char> chars) => new string(chars.ToArray());

        internal static IonTypes AsIonType(this Type gtype)
        {
            if (typeof(IonNull).Equals(gtype))
                return IonTypes.Null;

            if (typeof(IonBool).Equals(gtype))
                return IonTypes.Bool;

            if (typeof(IonInt).Equals(gtype))
                return IonTypes.Int;

            if (typeof(IonFloat).Equals(gtype))
                return IonTypes.Float;

            if (typeof(IonDecimal).Equals(gtype))
                return IonTypes.Decimal;

            if (typeof(IonNumber).Equals(gtype))
                return IonTypes.Decimal;

            if (typeof(IonTimestamp).Equals(gtype))
                return IonTypes.Timestamp;

            if (typeof(IonString).Equals(gtype))
                return IonTypes.String;

            if (typeof(IonOperator).Equals(gtype))
                return IonTypes.OperatorSymbol;

            if (typeof(IonIdentifier).Equals(gtype))
                return IonTypes.IdentifierSymbol;

            if (typeof(IonQuotedSymbol).Equals(gtype))
                return IonTypes.QuotedSymbol;

            if (typeof(IonBlob).Equals(gtype))
                return IonTypes.Blob;

            if (typeof(IonClob).Equals(gtype))
                return IonTypes.Clob;

            if (typeof(IonSexp).Equals(gtype))
                return IonTypes.Sexp;

            if (typeof(IonList).Equals(gtype))
                return IonTypes.List;

            if (typeof(IonStruct).Equals(gtype))
                return IonTypes.Struct;

            throw new ArgumentException($"Invalid ion type: {gtype}");
        }

        /// <summary>
        /// Returns the result in the optional instance if present, or the supplied exception.
        /// NOTE: this is temporary as it will be released in a future release of <c>Axis.Luna.Common</c>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="optional"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static IResult<TValue> AsResult<TValue>(this
            Optional<TValue> optional,
            Exception? exception = null)
            where TValue : class
        {
            return optional.IsEmpty
                ? Result.Of<TValue>(exception)
                : Result.Of(optional.Value());
        }
    }
}
