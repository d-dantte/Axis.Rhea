using Axis.Ion.Types;
using Axis.Rhea.Shared.Contract.StateExpressions.Ion;

namespace Axis.Rhea.Shared.Contract.StateExpressions
{
    public interface IAtom
    {
        IIonType Ion { get; }
    }

    public interface ITypedAtom<out TType>: IAtom
    where TType : struct, IIonType
    {
        new TType Ion { get; }
    }

    [Obsolete]
    public readonly struct Atom
    {
        public IIonType Ion { get; }

        public Atom(IIonType ion)
        {
            this.Ion = ion ?? throw new ArgumentNullException(nameof(ion));
        }


        #region implicits
        public static implicit operator Atom(IonNull ion) => new Atom(ion);
        public static implicit operator Atom(IonBool ion) => new Atom(ion);
        public static implicit operator Atom(IonInt ion) => new Atom(ion);
        public static implicit operator Atom(IonFloat ion) => new Atom(ion);
        public static implicit operator Atom(IonDecimal ion) => new Atom(ion);
        public static implicit operator Atom(IonTimestamp ion) => new Atom(ion);
        public static implicit operator Atom(IonString ion) => new Atom(ion);
        public static implicit operator Atom(IonOperator ion) => new Atom(ion);
        public static implicit operator Atom(IonIdentifier ion) => new Atom(ion);
        public static implicit operator Atom(IonQuotedSymbol ion) => new Atom(ion);
        public static implicit operator Atom(IonBlob ion) => new Atom(ion);
        public static implicit operator Atom(IonClob ion) => new Atom(ion);
        public static implicit operator Atom(IonSexp ion) => new Atom(ion);
        public static implicit operator Atom(IonList ion) => new Atom(ion);
        public static implicit operator Atom(IonStruct ion) => new Atom(ion);
        #endregion
    }

    public readonly struct Atom<TIonType>: ITypedAtom<TIonType>
    where TIonType: struct, IIonType
    {
        public TIonType Ion { get; }

        IIonType IAtom.Ion => Ion;

        public Atom(TIonType ion)
        {
            this.Ion = ion;
        }

        public override string ToString() => Ion.ToAtomText();

        public static implicit operator Atom<TIonType>(TIonType ion) => new Atom<TIonType>(ion);
    }

    public static class AtomUtil
    {
        /// <summary>
        /// Format types to conform to Atom text where necessary
        /// </summary>
        /// <param name="ion"></param>
        /// <returns></returns>
        public static string ToAtomText(this IIonType ion)
        {
            return ion switch
            {
                IonTimestamp t => $"'T {t.ToIonText()}'",
                _ => ion.ToIonText()
            };
        }
    }
}
