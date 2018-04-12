namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;

    public interface IPersistentStore
    {
        void Add(ICachedObject obj);
        void CommitPersistence(ICachedObject obj);
        void CommitRow(Session session, Row row, int changeAction, int txModel);
        void Delete(Session session, Row row);
        int ElementCount(Session session);
        int ElementCountUnique(Index index);
        ICachedObject Get(IRowInputInterface i);
        ICachedObject Get(int key);
        ICachedObject Get(ICachedObject obj, bool keep);
        ICachedObject Get(int key, bool keep);
        int GetAccessCount();
        ICachedObject GetAccessor(Index key);
        IPersistentStore GetAccessorStore(Index index);
        DataFileCache GetCache();
        long GetCreationTimestamp();
        Row GetNewCachedObject(Session session, object obj);
        ICachedObject GetNewInstance(int size);
        IRowIterator GetRowIterator();
        int GetStorageSize(int key);
        TableBase GetTable();
        long GetTimestamp();
        void IndexRow(Session session, Row row);
        void IndexRows();
        bool IsMemory();
        void LockStore();
        void MoveData(Session session, IPersistentStore other, int colindex, int adjust);
        void Release();
        void Release(int key);
        void Remove(int i);
        void RemoveAll();
        void RemovePersistence(int i);
        void ResetAccessorKeys(Index[] keys);
        void RollbackRow(Session session, Row row, int changeAction, int txModel);
        void Set(ICachedObject obj);
        void SetAccessor(Index key, ICachedObject accessor);
        void SetAccessor(Index key, int accessor);
        void SetCache(DataFileCache cache);
        void SetElementCount(Index key, int size, int uniqueSize);
        void SetTimestamp(long timestamp);
        void UnlockStore();
        void UpdateElementCount(Index key, int size, int uniqueSize);
    }
}

