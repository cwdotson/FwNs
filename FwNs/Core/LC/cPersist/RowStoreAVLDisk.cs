namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;
    using System.IO;
    using System.Threading;

    public sealed class RowStoreAVLDisk : RowStoreAVL, IDisposable
    {
        private readonly Database _database;
        private DataFileCache _cache;
        private IRowOutputInterface RowOut;

        public RowStoreAVLDisk(IPersistentStoreCollection manager, DataFileCache cache, Table table)
        {
            this._database = table.database;
            base.Manager = manager;
            base.table = table;
            base.indexList = table.GetIndexList();
            base.AccessorList = new ICachedObject[base.indexList.Length];
            this._cache = cache;
            if (cache != null)
            {
                this.RowOut = cache.RowOut.Duplicate();
            }
            manager.SetStore(table, this);
            this._database = table.database;
        }

        public override void Add(ICachedObject obj)
        {
            int realSize = obj.GetRealSize(this.RowOut);
            realSize = this.RowOut.GetStorageSize(realSize);
            obj.SetStorageSize(realSize);
            this._cache.Add(obj);
        }

        public override void CommitPersistence(ICachedObject row)
        {
        }

        public override void CommitRow(Session session, Row row, int changeAction, int txModel)
        {
            object[] rowData = row.RowData;
            switch (changeAction)
            {
                case 1:
                    this._database.logger.WriteInsertStatement(session, (Table) base.table, rowData);
                    return;

                case 2:
                    this._database.logger.WriteDeleteStatement(session, (Table) base.table, rowData);
                    if (txModel != 0)
                    {
                        break;
                    }
                    this.Remove(row.GetPos());
                    return;

                case 3:
                    this.Delete(session, row);
                    this._database.TxManager.RemoveTransactionInfo(row);
                    this.Remove(row.GetPos());
                    return;

                case 4:
                    if (txModel == 0)
                    {
                        this.Remove(row.GetPos());
                    }
                    break;

                default:
                    return;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if ((disposing && (this.RowOut != null)) && (this.RowOut is RowOutputBinary))
            {
                ((RowOutputBinary) this.RowOut).Dispose();
            }
        }

        public override int ElementCount(Session session)
        {
            IndexAVL xavl = (IndexAVL) base.indexList[0];
            if (base.elementCount < 0)
            {
                if (xavl == null)
                {
                    base.elementCount = 0;
                }
                else
                {
                    base.elementCount = xavl.GetNodeCount(session, this);
                }
            }
            if (((session != null) && (xavl != null)) && (this._database.TxManager.GetTransactionControl() != 0))
            {
                return xavl.GetNodeCount(session, this);
            }
            return base.elementCount;
        }

        public override ICachedObject Get(IRowInputInterface input)
        {
            ICachedObject obj2;
            try
            {
                obj2 = new RowAVLDisk(base.table, input);
            }
            catch (IOException exception)
            {
                throw Error.GetError(0x1d2, exception);
            }
            return obj2;
        }

        public override ICachedObject Get(int key)
        {
            return this._cache.Get(key, this, false);
        }

        public override ICachedObject Get(ICachedObject obj, bool keep)
        {
            obj = this._cache.Get(obj, this, keep);
            return obj;
        }

        public override ICachedObject Get(int key, bool keep)
        {
            return this._cache.Get(key, this, keep);
        }

        public override int GetAccessCount()
        {
            return this._cache.GetAccessCount();
        }

        public override ICachedObject GetAccessor(Index key)
        {
            NodeAVL node = (NodeAVL) base.AccessorList[key.GetPosition()];
            if (node == null)
            {
                return null;
            }
            if (!node.IsInMemory())
            {
                node = ((RowAVL) this.Get(node.GetPos(), false)).GetNode(key.GetPosition());
                base.AccessorList[key.GetPosition()] = node;
            }
            return node;
        }

        public override DataFileCache GetCache()
        {
            return this._cache;
        }

        public ICachedObject GetKeep(int key)
        {
            return this._cache.Get(key, this, true);
        }

        public override Row GetNewCachedObject(Session session, object obj)
        {
            RowAVL wavl = new RowAVLDisk(base.table, (object[]) obj);
            this.Add(wavl);
            if (session != null)
            {
                RowAction action = new RowAction(session, base.table, 1, wavl, null);
                wavl.rowAction = action;
            }
            return wavl;
        }

        public override ICachedObject GetNewInstance(int size)
        {
            return null;
        }

        public override int GetStorageSize(int i)
        {
            return this._cache.Get(i, this, false).GetStorageSize();
        }

        public override void IndexRow(Session session, Row row)
        {
            int index = 0;
            try
            {
                while (index < base.indexList.Length)
                {
                    base.indexList[index].Insert(session, this, row);
                    index++;
                }
            }
            catch (CoreException)
            {
                index--;
                while (index >= 0)
                {
                    base.indexList[index].Delete(session, this, row);
                    index--;
                }
                this.Remove(row.GetPos());
                this._database.TxManager.RemoveTransactionInfo(row);
                throw;
            }
        }

        public override bool IsMemory()
        {
            return false;
        }

        public override void LockStore()
        {
            Monitor.Enter(this._cache.Lock);
        }

        public override void Release()
        {
            ArrayUtil.FillArray(base.AccessorList, null);
            this._cache = null;
        }

        public override void Release(int i)
        {
            this._cache.Release(i);
        }

        public override void Remove(int i)
        {
            this._cache.Remove(i, this);
        }

        public override void RemoveAll()
        {
            base.elementCount = 0;
            ArrayUtil.FillArray(base.AccessorList, null);
        }

        public override void RemovePersistence(int i)
        {
        }

        public override void ResetAccessorKeys(Index[] keys)
        {
            if (((base.indexList.Length != 0) && (base.indexList[0] != null)) && (base.AccessorList[0] != null))
            {
                throw Error.RuntimeError(0xc9, "RowStoreAVLDisk");
            }
            base.indexList = keys;
            base.AccessorList = new ICachedObject[base.indexList.Length];
        }

        public override void RollbackRow(Session session, Row row, int changeAction, int txModel)
        {
            switch (changeAction)
            {
                case 1:
                    if (txModel != 0)
                    {
                        break;
                    }
                    this.Delete(session, row);
                    this.Remove(row.GetPos());
                    return;

                case 2:
                    if (txModel != 0)
                    {
                        break;
                    }
                    row = (Row) this.Get(row, true);
                    ((RowAVL) row).SetNewNodes();
                    row.KeepInMemory(false);
                    this.IndexRow(session, row);
                    return;

                case 3:
                    break;

                case 4:
                    if (txModel == 0)
                    {
                        this.Remove(row.GetPos());
                    }
                    break;

                default:
                    return;
            }
        }

        public override void Set(ICachedObject obj)
        {
            Row row = (Row) obj;
            this._database.TxManager.SetTransactionInfo(row);
        }

        public override void SetAccessor(Index key, ICachedObject accessor)
        {
            base.AccessorList[key.GetPosition()] = accessor;
        }

        public override void SetAccessor(Index key, int accessor)
        {
            ICachedObject node = this.Get(accessor, false);
            if (node != null)
            {
                node = ((RowAVL) node).GetNode(key.GetPosition());
            }
            this.SetAccessor(key, node);
        }

        public override void SetCache(DataFileCache cache)
        {
            this._cache = cache;
        }

        public override void UnlockStore()
        {
            Monitor.Exit(this._cache.Lock);
        }
    }
}

