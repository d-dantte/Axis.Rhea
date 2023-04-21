using Axis.Ion.Types;
using System;

namespace Axis.Rhea.Core.Workflow.State.DataPath
{
    public record PropertyNode: DataPathNode,  IIonSelector<string>
    {
        /// <summary>
        /// The property name
        /// </summary>
        public string Property { get; }

        public string Selector => Property;

        public PropertyNode(string property, DataPathNode next = null)
        : base(next)
        {
            Property = property;
        }

        public override IIonType Select(IIonType ion)
        {
            var result = ion switch
            {
                IonStruct @struct => @struct.IsNull
                    ? throw new ArgumentNullException(nameof(ion))
                    : @struct.Properties[Property],

                null => throw new ArgumentNullException(nameof(ion)),

                _ => throw new ArgumentException($"Invalid {nameof(ion)} type: '{ion.Type}'. Expected '{IonTypes.Struct}'")
            };

            return Next is not null
                ? Next.Select(result)
                : result;
        }
    }
}
