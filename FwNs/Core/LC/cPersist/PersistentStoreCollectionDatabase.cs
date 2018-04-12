namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class PersistentStoreCollectionDatabase : IPersistentStoreCollection
    {
        private readonly LongKeyHashMap<IPersistentStore> _rowStoreMap = new LongKeyHashMap<IPersistentStore>();
        private long _persistentStoreIdSequence;

        public long GetNextId()
        {
            long num;
            this._persistentStoreIdSequence = (num = this._persistentStoreIdSequence) + 1L;
            return num;
        }

        public IPersistentStore GetStore(object key)
        {
            long persistenceId = ((TableBase) key).GetPersistenceId();
            return this._rowStoreMap.Get(persistenceId);
        }

        public void ReleaseStore(TableBase table)
        {
            IPersistentStore store = this._rowStoreMap.Get(table.GetPersistenceId());
            if (store != null)
            {
                store.Release();
                this._rowStoreMap.Remove(table.GetPersistenceId());
            }
        }

        public void SetStore(object key, IPersistentStore store)
        {
            long persistenceId = ((TableBase) key).GetPersistenceId();
            if (store == null)
            {
                this._rowStoreMap.Remove(persistenceId);
            }
            else
            {
                this._rowStoreMap.Put(persistenceId, store);
            }
        }
    }
}

