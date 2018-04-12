namespace FwNs.Core.LC.cLib
{
    using System;

    public interface Iterator<T>
    {
        bool HasNext();
        T Next();
        void Remove();
        void SetValue(T value);
    }
}

