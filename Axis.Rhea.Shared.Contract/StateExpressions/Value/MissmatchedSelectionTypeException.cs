using Axis.Ion.Types;

namespace Axis.Rhea.Shared.Contract.StateExpressions.Value
{
    /// <summary>
    /// Thrown when the <see cref="IIonType"/> returned by a selection does not match the expected type
    /// </summary>
    public class MissmatchedSelectionTypeException : Exception
    {
        public IonTypes ExpectedType { get; }

        public IonTypes SelectedType { get; }

        public MissmatchedSelectionTypeException(IonTypes expectedType, IonTypes selectedType)
        : base($"Type mismatch. expected: {expectedType}, selected: {selectedType}")
        {
            ExpectedType = expectedType;
            SelectedType = selectedType;
        }
    }
}
