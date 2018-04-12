namespace FwNs.Core.LC.cLib
{
    using System;

    public interface IUtlList<T> : ICollection<T>
    {
        void Add(int index, T element);
        T Get(int index);
        T Remove(int index);
        T Set(int index, T element);
    }
}

