namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;
    using System.IO;

    public sealed class RowStoreAVLHybrid : RowStoreAVL, IPersistentStore
    {
        private readonly bool _isTempTable;
        private readonly Session _session;
        private readonly bool _useCache;
        private DataFileCacheSession _cache;
        private bool _isCached;
        private int _maxMemoryRowCount;
        private int _memoryRowCount;
        private int _rowIdSequence;

        public RowStoreAVLHybrid(Session session, IPersistentStoreCollection manager, TableBase table) : this(session, manager, table, true)
        {
            this._cache = session.sessionData.GetResultCache();
            if (this._cache != null)
            {
                this._isCached = true;
            }
        }

        public RowStoreAVLHybrid(Session session, IPersistentStoreCollection manager, TableBase table, bool useCache)
        {
            this._session = session;
            base.Manager = manager;
            base.table = table;
            this._maxMemoryRowCount = session.GetResultMemoryRowCount();
            this._isTempTable = table.GetTableType() == 3;
            this._useCache = useCache;
            if (this._maxMemoryRowCount == 0)
            {
                this._useCache = false;
            }
            if (table.GetTableType() == 9)
            {
                base.Timestamp = session.GetActionTimestamp();
            }
            this.ResetAccessorKeys(table.GetIndexList());
            manager.SetStore(table, this);
        }

        public override void Add(ICachedObject obj)
        {
            if (this._isCached)
            {
                int realSize = obj.GetRealSize(this._cache.RowOut);
                realSize = this._cache.RowOut.GetStorageSize(realSize);
                obj.SetStorageSize(realSize);
                this._cache.Add(obj);
            }
        }

        public void ChangeToDiskTable()
        {
            this._cache = this._session.sessionData.GetResultCache();
            if (this._cache != null)
            {
                IRowIterator rowIterator = base.table.GetRowIterator(this);
                ArrayUtil.FillArray(base.AccessorList, null);
                this._isCached = true;
                this._cache.StoreCount++;
                while (rowIterator.HasNext())
                {
                    Row nextRow = rowIterator.GetNextRow();
                    Row newCachedObject = this.GetNewCachedObject(this._session, nextRow.RowData);
                    this.IndexRow(null, newCachedObject);
                    nextRow.Destroy();
                }
            }
            this._maxMemoryRowCount = 0x7fffffff;
        }

        public override void CommitPersistence(ICachedObject row)
        {
        }

        public override void CommitRow(Session session, Row row, int changeAction, int txModel)
        {
            switch (changeAction)
            {
                case 2:
                    if (txModel != 0)
                    {
                        break;
                    }
                    this.Remove(row.GetPos());
                    return;

                case 3:
                    this.Delete(session, row);
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

        public override ICachedObject Get(IRowInputInterface input)
        {
            try
            {
                if (this._isCached)
                {
                    return new RowAVLDisk(base.table, input);
                }
            }
            catch (CoreException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
            return null;
        }

        public override ICachedObject Get(int i)
        {
            try
            {
                if (!this._isCached)
                {
                    throw Error.RuntimeError(0xc9, "RowStoreAVLHybrid");
                }
                return this._cache.Get(i, this, false);
            }
            catch (CoreException)
            {
                return null;
            }
        }

        public override ICachedObject Get(ICachedObject obj, bool keep)
        {
            try
            {
                if (this._isCached)
                {
                    return this._cache.Get(obj, this, keep);
                }
                return obj;
            }
            catch (CoreException)
            {
                return null;
            }
        }

        public override ICachedObject Get(int i, bool keep)
        {
            try
            {
                if (!this._isCached)
                {
                    throw Error.RuntimeError(0xc9, "RowStoreAVLHybrid");
                }
                return this._cache.Get(i, this, keep);
            }
            catch (CoreException)
            {
                return null;
            }
        }

        public override int GetAccessCount()
        {
            lock (this)
            {
                return (this._isCached ? this._cache.GetAccessCount() : 0);
            }
        }

        public override DataFileCache GetCache()
        {
            return this._cache;
        }

        public ICachedObject GetKeep(int i)
        {
            try
            {
                if (!this._isCached)
                {
                    throw Error.RuntimeError(0xc9, "RowStoreAVLHybrid");
                }
                return this._cache.Get(i, this, true);
            }
            catch (CoreException)
            {
                return null;
            }
        }

        public override Row GetNewCachedObject(Session session, object obj)
        {
            int num2 = this._rowIdSequence;
            this._rowIdSequence = num2 + 1;
            int position = num2;
            if (this._isCached)
            {
                Row row2 = new RowAVLDisk(base.table, (object[]) obj);
                this.Add(row2);
                if (this._isTempTable)
                {
                    RowAction.AddInsertAction(session, base.table, row2);
                }
                return row2;
            }
            this._memoryRowCount++;
            if (this._useCache && (this._memoryRowCount > this._maxMemoryRowCount))
            {
                this.ChangeToDiskTable();
                return this.GetNewCachedObject(session, obj);
            }
            RowAVL row = new RowAVL(base.table, (object[]) obj, position);
            row.SetNewNodes();
            if (this._isTempTable)
            {
                RowAction action = new RowAction(session, base.table, 1, row, null);
                row.rowAction = action;
            }
            return row;
        }

        public override ICachedObject GetNewInstance(int size)
        {
            return null;
        }

        public override int GetStorageSize(int i)
        {
            try
            {
                if (this._isCached)
                {
                    return this._cache.Get(i, this, false).GetStorageSize();
                }
                return 0;
            }
            catch (CoreException)
            {
                return 0;
            }
        }

        public override bool IsMemory()
        {
            return !this._isCached;
        }

        public override void Release()
        {
            ArrayUtil.FillArray(base.AccessorList, null);
            if (this._isCached)
            {
                this._cache.StoreCount--;
                if (this._cache.StoreCount == 0)
                {
                    this._cache.Clear();
                }
                this._cache = null;
                this._isCached = false;
            }
            base.Manager.SetStore(base.table, null);
        }

        public override void Release(int i)
        {
            if (this._isCached)
            {
                this._cache.Release(i);
            }
        }

        public override void Remove(int i)
        {
            if (this._isCached)
            {
                this._cache.Remove(i, this);
            }
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
            lock (this)
            {
                if (((base.indexList.Length == 0) || (base.indexList[0] == null)) || (base.AccessorList[0] == null))
                {
                    base.indexList = keys;
                    base.AccessorList = new ICachedObject[base.indexList.Length];
                }
                else
                {
                    if (this._isCached)
                    {
                        throw Error.RuntimeError(0xc9, "RowStoreAVLHybrid");
                    }
                    base.ResetAccessorKeys(keys);
                }
            }
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
        }

        public override void SetAccessor(Index key, ICachedObject accessor)
        {
            base.AccessorList[key.GetPosition()] = accessor;
        }

        public override void SetAccessor(Index key, int accessor)
        {
        }

        public override void SetCache(DataFileCache cache)
        {
            throw Error.RuntimeError(0xc9, "RowStoreAVLHybrid");
        }
    }
}

