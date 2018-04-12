namespace FwNs.Core.LC.cNavigators
{
    using FwNs.Core.LC.cRows;
    using System;

    public interface IRowIterator
    {
        object[] GetNext();
        Row GetNextRow();
        long GetRowId();
        bool HasNext();
        void Release();
        void Remove();
        bool SetRowColumns(bool[] columns);
    }
}

