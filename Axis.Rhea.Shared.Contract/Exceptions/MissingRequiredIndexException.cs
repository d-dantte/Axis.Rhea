using System;

namespace Axis.Rhea.Shared.Contract.Exceptions
{
    public class MissingRequiredIndexException: Exception
    {
        public int Index { get; }

        public MissingRequiredIndexException(int index, string? message = null)
            :base(message)
        {
            Index = index;
        }
    }
}
