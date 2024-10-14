namespace Axis.Rhae.Contract
{
    /// <summary>
    /// Defines the predicate for validating texts that conform to a given pattern.
    /// </summary>
    public interface IIdentifierPattern
    {
        static abstract bool IsValidPattern(string text);
    }
}
