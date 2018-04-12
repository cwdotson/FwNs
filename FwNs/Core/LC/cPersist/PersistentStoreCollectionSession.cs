namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class PersistentStoreCollectionSession : IPersistentStoreCollection
    {
        private readonly LongKeyHashMap<IPersistentStore> _rowStoreMapSession = new LongKeyHashMap<IPersistentStore>();
        private LongKeyHashMap<IPersistentStore> _rowStoreMapStatement = new LongKeyHashMap<IPersistentStore>();
        private readonly LongKeyHashMap<IPersistentStore> _rowStoreMapTransaction = new LongKeyHashMap<IPersistentStore>();
        private readonly Session _session;
        private UtlDeque<LongKeyHashMap<IPersistentStore>> _rowStoreListStatement;
        private Pool<LongKeyHashMap<IPersistentStore>> longKeyhashMapPool = new Pool<LongKeyHashMap<IPersistentStore>>();

        public PersistentStoreCollectionSession(Session session)
        {
            this._session = session;
        }

        public void ClearAllTables()
        {
            this.ClearSessionTables();
            this.ClearTransactionTables();
            this.ClearStatementTables();
        }

        public void ClearResultTables(long actionTimestamp)
        {
            if (!this._rowStoreMapSession.IsEmpty())
            {
                Iterator<IPersistentStore> iterator = this._rowStoreMapSession.GetValues().GetIterator();
                while (iterator.HasNext())
                {
                    IPersistentStore store = iterator.Next();
                    if (store.GetTimestamp() == actionTimestamp)
                    {
                        store.Release();
                    }
                }
            }
        }

        public void ClearSessionTables()
        {
            if (!this._rowStoreMapSession.IsEmpty())
            {
                Iterator<IPersistentStore> iterator = this._rowStoreMapSession.GetValues().GetIterator();
                while (iterator.HasNext())
                {
                    iterator.Next().Release();
                }
                this._rowStoreMapSession.Clear();
            }
        }

        public void ClearStatementTables()
        {
            if (this._rowStoreMapStatement.Size() != 0)
            {
                Iterator<IPersistentStore> iterator = this._rowStoreMapStatement.GetValues().GetIterator();
                while (iterator.HasNext())
                {
                    iterator.Next().Release();
                }
                this._rowStoreMapStatement.Clear();
            }
        }

        public void ClearTransactionTables()
        {
            if (this._rowStoreMapTransaction.Size() != 0)
            {
                Iterator<IPersistentStore> iterator = this._rowStoreMapTransaction.GetValues().GetIterator();
                while (iterator.HasNext())
                {
                    iterator.Next().Release();
                }
                this._rowStoreMapTransaction.Clear();
            }
        }

        public IPersistentStore FindStore(Table table)
        {
            switch (table.PersistenceScope)
            {
                case 0x15:
                    return this._rowStoreMapStatement.Get(table.GetPersistenceId());

                case 0x16:
                case 0x18:
                    return this._rowStoreMapTransaction.Get(table.GetPersistenceId());

                case 0x17:
                    return this._rowStoreMapSession.Get(table.GetPersistenceId());
            }
            return null;
        }

        public IPersistentStore GetStore(object key)
        {
            try
            {
                TableBase table = (TableBase) key;
                switch (table.PersistenceScope)
                {
                    case 0x15:
                    {
                        IPersistentStore store = this._rowStoreMapStatement.Get(table.GetPersistenceId());
                        if (store == null)
                        {
                            store = this._session.database.logger.NewStore(this._session, this, table, true);
                        }
                        return store;
                    }
                    case 0x16:
                    case 0x18:
                    {
                        IPersistentStore store3 = this._rowStoreMapTransaction.Get(table.GetPersistenceId());
                        if (store3 == null)
                        {
                            store3 = this._session.database.logger.NewStore(this._session, this, table, true);
                        }
                        return store3;
                    }
                    case 0x17:
                    {
                        IPersistentStore store4 = this._rowStoreMapSession.Get(table.GetPersistenceId());
                        if (store4 == null)
                        {
                            store4 = this._session.database.logger.NewStore(this._session, this, table, true);
                        }
                        return store4;
                    }
                }
            }
            catch (CoreException)
            {
            }
            throw Error.RuntimeError(0xc9, "PersistentStoreCollectionSession");
        }

        public void MoveData(Table oldTable, Table newTable, int colIndex, int adjust)
        {
            IPersistentStore other = this.FindStore(oldTable);
            if (other != null)
            {
                this.GetStore(newTable).MoveData(this._session, other, colIndex, adjust);
                this.SetStore(oldTable, null);
            }
        }

        public void Pop()
        {
            this.ClearStatementTables();
            this.longKeyhashMapPool.Store(this._rowStoreMapStatement);
            this._rowStoreMapStatement = this._rowStoreListStatement.RemoveLast();
        }

        public void Push()
        {
            if (this._rowStoreListStatement == null)
            {
                this._rowStoreListStatement = new UtlDeque<LongKeyHashMap<IPersistentStore>>();
            }
            if (this._rowStoreMapStatement.IsEmpty())
            {
                this._rowStoreListStatement.Add(this.longKeyhashMapPool.Fetch());
            }
            else
            {
                this._rowStoreListStatement.Add(this._rowStoreMapStatement);
                this._rowStoreMapStatement = this.longKeyhashMapPool.Fetch();
            }
        }

        public void RegisterIndex(Table table)
        {
            IPersistentStore store = this.FindStore(table);
            if (store != null)
            {
                store.ResetAccessorKeys(table.GetIndexList());
            }
        }

        public void SetStore(object key, IPersistentStore store)
        {
            TableBase base2 = (TableBase) key;
            switch (base2.PersistenceScope)
            {
                case 0x15:
                    if (store != null)
                    {
                        this._rowStoreMapStatement.Put(base2.GetPersistenceId(), store);
                        return;
                    }
                    this._rowStoreMapStatement.Remove(base2.GetPersistenceId());
                    return;

                case 0x16:
                case 0x18:
                    if (store != null)
                    {
                        this._rowStoreMapTransaction.Put(base2.GetPersistenceId(), store);
                        return;
                    }
                    this._rowStoreMapTransaction.Remove(base2.GetPersistenceId());
                    return;

                case 0x17:
                    if (store != null)
                    {
                        this._rowStoreMapSession.Put(base2.GetPersistenceId(), store);
                        return;
                    }
                    this._rowStoreMapSession.Remove(base2.GetPersistenceId());
                    return;
            }
            throw Error.RuntimeError(0xc9, "PersistentStoreCollectionSession");
        }
    }
}

