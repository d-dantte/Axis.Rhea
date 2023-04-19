using System;

namespace Axis.Rhea.Core.Exceptions
{
    public class MissingRequiredPropertyException: Exception
    {
        public string Property { get; }

        public MissingRequiredPropertyException(string property, string message = null)
            : base(message)
        {
            Property = property;
        }
    }
}
