namespace FwNs.Linq.Xprsns
{
    using System;

    internal static class LlError
    {
        public static Exception NoImplementation(bool supported)
        {
            if (!supported)
            {
                return new NotSupportedException();
            }
            return new NotImplementedException();
        }

        public static UnexpectedEnumValueException<T> UnexpectedEnumValue<T>(T value) where T: struct, IComparable
        {
            return new UnexpectedEnumValueException<T>(value);
        }
    }
}

