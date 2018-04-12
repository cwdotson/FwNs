namespace FwNs.Core.LC.cLib
{
    using System;

    public interface ISet<T> : ICollection<T>
    {
        bool Equals(object o);
        T Get(T o);
    }
}

