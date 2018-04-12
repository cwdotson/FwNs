namespace FwNs.Core.LC.cLib
{
    using System;

    public interface ICollection<T>
    {
        bool Add(T o);
        bool AddAll(ICollection<T> c);
        void Clear();
        bool Contains(T o);
        int GetHashCode();
        Iterator<T> GetIterator();
        bool IsEmpty();
        bool Remove(T o);
        int Size();
    }
}

