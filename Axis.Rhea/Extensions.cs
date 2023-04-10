using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Rhea.Core
{
    public static class Extensions
    {

        public static bool IsDefault<T>(this T value) => EqualityComparer<T>.Default.Equals(default, value);

        public static TItem? FirstOrNull<TItem>(this IEnumerable<TItem> enumerable)
        where TItem : struct
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            var item = enumerable.FirstOrDefault();
            if (item.IsDefault())
                return null;

            return item;
        }

        public static TItem? LastOrNull<TItem>(this IEnumerable<TItem> enumerable)
        where TItem : struct
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            var item = enumerable.LastOrDefault();
            if (item.IsDefault())
                return null;

            return item;
        }

        public static IResult<TOut> Cast<TIn, TOut>(this IResult<TIn> rin)
        {
            if (rin is null)
                throw new ArgumentNullException(nameof(rin));

            return rin.Map(data => (TOut)(object)data);
        }

        public static IEnumerable<TItem> StuffUsing<TItem>(this IEnumerable<TItem> items, TItem joiner)
        {
            var started = false;
            foreach(var item in items)
            {
                if (started)
                    yield return joiner;

                yield return item;
                started = true;
            }
        }

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

            foreach(var item in items)
            {
                if (predicate.Invoke(item))
                    exceptionProvider.Invoke(item).Throw();

                yield return item;
            }
        }
    }
}
