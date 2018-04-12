namespace FwNs.Core.Typs
{
    using System;

    internal static class EmptyAry<T>
    {
        public static readonly T[] Instance;

        static EmptyAry()
        {
            EmptyAry<T>.Instance = new T[0];
        }
    }
}

