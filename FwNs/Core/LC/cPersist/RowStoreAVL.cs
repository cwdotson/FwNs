namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;

    public abstract class RowStoreAVL : IPersistentStore
    {
        private long _timestamp;
        public ICachedObject[] AccessorList;
        public int elementCount;
        public Index[] indexList = new IndexAVL[0];
        public IPersistentStoreCollection Manager;
        public TableBase table;
        public long Timestamp;

        protected RowStoreAVL()
        {
        }

        public abstract void Add(ICachedObject obj);
        public abstract void CommitPersistence(ICachedObject obj);
        public abstract void CommitRow(Session session, Row row, int changeAction, int txModel);
        public virtual void Delete(Session session, Row row)
        {
            row = (Row) this.Get(row, false);
            for (int i = this.indexList.Length - 1; i >= 0; i--)
            {
                this.indexList[i].Delete(session, this, row);
            }
            row.Delete(this);
        }

        public void DropIndexFromRows(Index primaryIndex, Index oldIndex)
        {
            IRowIterator iterator = primaryIndex.FirstRow(this);
            int num = oldIndex.GetPosition() - 1;
            while (iterator.HasNext())
            {
                int num2 = num - 1;
                NodeAVL node = ((RowAVL) iterator.GetNextRow()).GetNode(0);
                while (num2-- > 0)
                {
                    node = node.nNext;
                }
                node.nNext = node.nNext.nNext;
            }
        }

        public virtual int ElementCount(Session session)
        {
            if (this.elementCount < 0)
            {
                Index index = this.indexList[0];
                if (index == null)
                {
                    this.elementCount = 0;
                }
                else
                {
                    this.elementCount = ((IndexAVL) index).GetNodeCount(session, this);
                }
            }
            return this.elementCount;
        }

        public virtual int ElementCountUnique(Index index)
        {
            return 0;
        }

        public abstract ICachedObject Get(IRowInputInterface i);
        public abstract ICachedObject Get(int key);
        public abstract ICachedObject Get(ICachedObject obj, bool keep);
        public abstract ICachedObject Get(int key, bool keep);
        public abstract int GetAccessCount();
        public virtual ICachedObject GetAccessor(Index key)
        {
            int position = key.GetPosition();
            if (position >= this.AccessorList.Length)
            {
                throw Error.RuntimeError(0xc9, "RowStoreAVL");
            }
            return this.AccessorList[position];
        }

        public IPersistentStore GetAccessorStore(Index index)
        {
            return null;
        }

        public abstract DataFileCache GetCache();
        public long GetCreationTimestamp()
        {
            return this.Timestamp;
        }

        public abstract Row GetNewCachedObject(Session session, object obj);
        public abstract ICachedObject GetNewInstance(int size);
        public IRowIterator GetRowIterator()
        {
            if ((this.indexList.Length == 0) || (this.indexList[0] == null))
            {
                throw Error.RuntimeError(0xc9, "RowStoreAVL");
            }
            return this.indexList[0].FirstRow(this);
        }

        public abstract int GetStorageSize(int key);
        public TableBase GetTable()
        {
            return this.table;
        }

        public long GetTimestamp()
        {
            return this._timestamp;
        }

        public virtual void IndexRow(Session session, Row row)
        {
            int index = 0;
            try
            {
                while (index < this.indexList.Length)
                {
                    this.indexList[index].Insert(session, this, row);
                    index++;
                }
            }
            catch (CoreException)
            {
                index--;
                while (index >= 0)
                {
                    this.indexList[index].Delete(session, this, row);
                    index--;
                }
                this.Remove(row.GetPos());
                throw;
            }
        }

        public void IndexRows()
        {
            IRowIterator rowIterator = this.GetRowIterator();
            for (int i = 1; i < this.indexList.Length; i++)
            {
                this.SetAccessor(this.indexList[i], (ICachedObject) null);
            }
            while (rowIterator.HasNext())
            {
                Row nextRow = rowIterator.GetNextRow();
                ((RowAVL) nextRow).ClearNonPrimaryNodes();
                for (int j = 1; j < this.indexList.Length; j++)
                {
                    this.indexList[j].Insert(null, this, nextRow);
                }
            }
        }

        public bool InsertIndexNodes(Index primaryIndex, Index newIndex)
        {
            CoreException error;
            int position = newIndex.GetPosition();
            IRowIterator iterator = primaryIndex.FirstRow(this);
            int num2 = 0;
            try
            {
                while (iterator.HasNext())
                {
                    Row nextRow = iterator.GetNextRow();
                    ((RowAVL) nextRow).InsertNode(position);
                    num2++;
                    newIndex.Insert(null, this, nextRow);
                }
                return true;
            }
            catch (OutOfMemoryException)
            {
                error = Error.GetError(460);
            }
            catch (CoreException exception2)
            {
                error = exception2;
            }
            iterator = primaryIndex.FirstRow(this);
            for (int i = 0; i < num2; i++)
            {
                NodeAVL node = ((RowAVL) iterator.GetNextRow()).GetNode(0);
                int num4 = position;
                while (--num4 > 0)
                {
                    node = node.nNext;
                }
                node.nNext = node.nNext.nNext;
            }
            throw error;
        }

        public virtual bool IsMemory()
        {
            return false;
        }

        public virtual void LockStore()
        {
        }

        public void MoveData(Session session, IPersistentStore other, int colindex, int adjust)
        {
            object defaultValue = null;
            SqlType type = null;
            SqlType type2 = null;
            if ((adjust >= 0) && (colindex != -1))
            {
                defaultValue = ((Table) this.table).GetColumn(colindex).GetDefaultValue(session);
                type2 = this.table.GetColumnTypes()[colindex];
            }
            if ((adjust <= 0) && (colindex != -1))
            {
                type = other.GetTable().GetColumnTypes()[colindex];
            }
            IRowIterator rowIterator = other.GetRowIterator();
            Table table = (Table) this.table;
            try
            {
                while (rowIterator.HasNext())
                {
                    object[] rowData = rowIterator.GetNextRow().RowData;
                    object[] emptyRowData = table.GetEmptyRowData();
                    object a = null;
                    if ((adjust == 0) && (colindex != -1))
                    {
                        a = rowData[colindex];
                        defaultValue = type2.ConvertToType(session, a, type);
                    }
                    if ((defaultValue != null) && type2.IsLobType())
                    {
                        session.sessionData.AdjustLobUsageCount(defaultValue, 1);
                    }
                    if (((a != null) && (type != null)) && type.IsLobType())
                    {
                        session.sessionData.AdjustLobUsageCount(a, -1);
                    }
                    ArrayUtil.CopyAdjustArray<object>(rowData, emptyRowData, defaultValue, colindex, adjust);
                    table.SystemSetIdentityColumn(session, emptyRowData);
                    table.EnforceTypeLimits(session, emptyRowData);
                    table.EnforceRowConstraints(session, emptyRowData);
                    Row newCachedObject = this.GetNewCachedObject(null, emptyRowData);
                    this.IndexRow(null, newCachedObject);
                }
            }
            catch (OutOfMemoryException)
            {
                throw Error.GetError(460);
            }
        }

        public abstract void Release();
        public abstract void Release(int i);
        public abstract void Remove(int i);
        public abstract void RemoveAll();
        public abstract void RemovePersistence(int i);
        public virtual void ResetAccessorKeys(Index[] keys)
        {
            if (((this.indexList.Length == 0) || (this.indexList[0] == null)) || (this.AccessorList[0] == null))
            {
                this.indexList = keys;
                this.AccessorList = new ICachedObject[this.indexList.Length];
            }
            else
            {
                ICachedObject[] accessorList = this.AccessorList;
                Index[] indexList = this.indexList;
                int length = this.indexList.Length;
                int adjust = 1;
                int index = 0;
                if (keys.Length < this.indexList.Length)
                {
                    adjust = -1;
                    length = keys.Length;
                }
                while ((index < length) && (this.indexList[index] == keys[index]))
                {
                    index++;
                }
                this.AccessorList = ArrayUtil.ToAdjustedArray<ICachedObject>(this.AccessorList, null, index, adjust);
                this.indexList = keys;
                try
                {
                    if (adjust > 0)
                    {
                        this.InsertIndexNodes(this.indexList[0], this.indexList[index]);
                    }
                    else
                    {
                        this.DropIndexFromRows(this.indexList[0], indexList[index]);
                    }
                }
                catch (CoreException)
                {
                    this.AccessorList = accessorList;
                    this.indexList = indexList;
                    throw;
                }
            }
        }

        public abstract void RollbackRow(Session session, Row row, int changeAction, int txModel);
        public abstract void Set(ICachedObject obj);
        public abstract void SetAccessor(Index key, ICachedObject accessor);
        public abstract void SetAccessor(Index key, int accessor);
        public abstract void SetCache(DataFileCache cache);
        public void SetElementCount(Index key, int size, int uniqueSize)
        {
            this.elementCount = size;
        }

        public void SetTimestamp(long timestamp)
        {
            this._timestamp = timestamp;
        }

        public virtual void UnlockStore()
        {
        }

        public void UpdateElementCount(Index key, int size, int uniqueSize)
        {
            if ((key.GetPosition() == 0) && (this.elementCount > -1))
            {
                this.elementCount += size;
            }
        }
    }
}

