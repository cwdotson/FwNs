namespace FwNs.Core.LC.cNavigators
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cRows;
    using System;

    public interface IRangeIterator : IRowIterator
    {
        object[] GetCurrent();
        Row GetCurrentRow();
        RangeVariable GetRange();
        int GetRangePosition();
        object GetRowidObject();
        bool IsBeforeFirst();
        bool Next();
        void Reset();
        void SetCurrent(object[] data);
    }
}

