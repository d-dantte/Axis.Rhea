namespace Axis.Rhae.Contract.Utils
{
    #region Union-2
    public interface IUnion<T1, T2>
    {
        object Value { get; }
    }

    public interface IUnionOf<T1, T2, TSelf>
    where TSelf : IUnionOf<T1, T2, TSelf>
    {
        public static abstract TSelf Of(T1 value);

        public static abstract TSelf Of(T2 value);
    }
    #endregion

    #region Union-3
    public interface IUnion<T1, T2, T3>
    {
        object Value { get; }
    }

    public interface IUnionOf<T1, T2, T3, TSelf>
    where TSelf : IUnionOf<T1, T2, T3, TSelf>
    {
        public static abstract TSelf Of(T1 value);

        public static abstract TSelf Of(T2 value);

        public static abstract TSelf Of(T3 value);
    }
    #endregion
}
