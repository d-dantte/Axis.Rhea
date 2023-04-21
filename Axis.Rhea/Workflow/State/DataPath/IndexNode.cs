using Axis.Ion.Types;
using System;

namespace Axis.Rhea.Core.Workflow.State.DataPath
{
    public record IndexNode: DataPathNode, IIonSelector<int>
    {
        /// <summary>
        /// The index into the <see cref="Ion.Types.IonList"/>, or <see cref="Ion.Types.IonSexp"/>
        /// </summary>
        public int Index { get; }

        public int Selector => Index;

        public IndexNode(int index, DataPathNode next = null)
        : base(next)
        {
            Index = index;
        }

        public override IIonType Select(IIonType ion)
        {
            var result = ion switch
            {
                IonList list => list.IsNull
                    ? throw new ArgumentNullException(nameof(ion))
                    : list.Value[Index],

                IonSexp sexp => sexp.IsNull
                    ? throw new ArgumentNullException(nameof(ion))
                    : sexp.Value[Index],

                null => throw new ArgumentNullException(nameof(ion)),

                _ => throw new ArgumentException($"Invalid {nameof(ion)} type: '{ion.Type}'. Expected '{IonTypes.List}' or '{IonTypes.Sexp}'")
            };

            return Next is not null
                ? Next.Select(result)
                : result;
        }
    }
}
