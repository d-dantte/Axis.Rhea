using System;

namespace Axis.Rhea.Shared.Contract.Exceptions
{
    public class MissingRequiredPropertyException: Exception
    {
        public string Property { get; }

        public MissingRequiredPropertyException(string property, string? message = null)
            : base(message)
        {
            Property = property;
        }
    }
}
