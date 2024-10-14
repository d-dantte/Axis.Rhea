namespace Axis.Rhae.Workflow
{
    internal static class Extensions
    {
        public static bool IsNegative(this TimeSpan value) => TimeSpan.Zero > value;
    }
}
