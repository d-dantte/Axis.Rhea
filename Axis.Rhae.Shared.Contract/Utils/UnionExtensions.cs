using Axis.Luna.Extensions;

namespace Axis.Rhae.Contract.Utils
{
    public static class Union
    {
        #region Union-2

        #region Is
        public static bool Is<T1, T2>(
            this IUnion<T1, T2> union,
            out T1? value)
            => union.Value.Is(out value);

        public static bool Is<T1, T2>(
            this IUnion<T1, T2> union,
            out T2? value)
            => union.Value.Is(out value);

        public static bool IsNull<T1, T2>(
            this IUnion<T1, T2> union)
            => union.Value is null;
        #endregion

        #region Match
        public static TOut? Match<T1, T2, TOut>(
            this IUnion<T1, T2> union,
            Func<T1, TOut> t1Mapper,
            Func<T2, TOut> t2Mapper,
            Func<TOut>? nullMap = null)
        {
            ArgumentNullException.ThrowIfNull(t1Mapper);
            ArgumentNullException.ThrowIfNull(t2Mapper);

            return union.Value switch
            {
                T1 t => t1Mapper.Invoke(t),
                T2 t => t2Mapper.Invoke(t),
                null => nullMap switch
                {
                    null => default,
                    _ => nullMap.Invoke()
                },
                _ => throw new InvalidOperationException(
                    $"Invalid union-type: {union.Value.GetType()}")
            };
        }

        #endregion

        #region Consume
        public static void Consume<T1, T2>(
            this IUnion<T1, T2> union,
            Action<T1> t1Consumer,
            Action<T2> t2Consumer,
            Action? nullConsumer = null)
        {
            ArgumentNullException.ThrowIfNull(t1Consumer);
            ArgumentNullException.ThrowIfNull(t2Consumer);

            if (union.Value is T1 t1)
                t1Consumer.Invoke(t1);

            else if (union.Value is T2 t2)
                t2Consumer.Invoke(t2);

            else if (union.Value is null && nullConsumer is not null)
                nullConsumer.Invoke();
        }
        #endregion

        #region With
        public static TUnion With<TUnion, T1, T2>(
            this TUnion union,
            Action<T1> t1Consumer,
            Action<T2> t2Consumer,
            Action? nullConsumer = null)
            where TUnion : IUnion<T1, T2>
        {
            union.Consume(t1Consumer, t2Consumer, nullConsumer);
            return union;
        }
        #endregion

        #endregion

        #region Union-3

        #region Is
        public static bool Is<T1, T2, T3>(
            this IUnion<T1, T2, T3> union,
            out T1? value)
            => union.Value.Is(out value);

        public static bool Is<T1, T2, T3>(
            this IUnion<T1, T2, T3> union,
            out T2? value)
            => union.Value.Is(out value);

        public static bool Is<T1, T2, T3>(
            this IUnion<T1, T2, T3> union,
            out T3? value)
            => union.Value.Is(out value);

        public static bool IsNull<T1, T2, T3>(
            this IUnion<T1, T2, T3> union)
            => union.Value is null;
        #endregion

        #region Match
        public static TOut? Match<T1, T2, T3, TOut>(
            this IUnion<T1, T2, T3> union,
            Func<T1, TOut> t1Mapper,
            Func<T2, TOut> t2Mapper,
            Func<T3, TOut> t3Mapper,
            Func<TOut>? nullMap = null)
        {
            ArgumentNullException.ThrowIfNull(t1Mapper);
            ArgumentNullException.ThrowIfNull(t2Mapper);
            ArgumentNullException.ThrowIfNull(t3Mapper);

            return union.Value switch
            {
                T1 t => t1Mapper.Invoke(t),
                T2 t => t2Mapper.Invoke(t),
                T3 t => t3Mapper.Invoke(t),
                null => nullMap switch
                {
                    null => default,
                    _ => nullMap.Invoke()
                },
                _ => throw new InvalidOperationException(
                    $"Invalid union-type: {union.Value.GetType()}")
            };
        }

        #endregion

        #region Consume
        public static void Consume<T1, T2, T3>(
            this IUnion<T1, T2, T3> union,
            Action<T1> t1Consumer,
            Action<T2> t2Consumer,
            Action<T3> t3Consumer,
            Action? nullConsumer = null)
        {
            ArgumentNullException.ThrowIfNull(t1Consumer);
            ArgumentNullException.ThrowIfNull(t2Consumer);
            ArgumentNullException.ThrowIfNull(t3Consumer);

            if (union.Value is T1 t1)
                t1Consumer.Invoke(t1);

            else if (union.Value is T2 t2)
                t2Consumer.Invoke(t2);

            else if (union.Value is T3 t3)
                t3Consumer.Invoke(t3);

            else if (union.Value is null && nullConsumer is not null)
                nullConsumer.Invoke();
        }
        #endregion

        #region With
        public static TUnion With<TUnion, T1, T2, T3>(
            this TUnion union,
            Action<T1> t1Consumer,
            Action<T2> t2Consumer,
            Action<T3> t3Consumer,
            Action? nullConsumer = null)
            where TUnion : IUnion<T1, T2, T3>
        {
            union.Consume(t1Consumer, t2Consumer, t3Consumer, nullConsumer);
            return union;
        }
        #endregion

        #endregion
    }
}
