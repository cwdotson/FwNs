namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRowIO;
    using System;

    public interface ICachedObject
    {
        void Destroy();
        int GetAccessCount();
        int GetPos();
        int GetRealSize(IRowOutputInterface o);
        int GetStorageSize();
        bool HasChanged();
        bool IsInMemory();
        bool IsKeepInMemory();
        bool IsMemory();
        bool KeepInMemory(bool keep);
        void Restore();
        void SetInMemory(bool i);
        void SetPos(int pos);
        void SetStorageSize(int size);
        void UpdateAccessCount(int count);
        void Write(IRowOutputInterface output);
        void Write(IRowOutputInterface output, IntLookup lookup);
    }
}

