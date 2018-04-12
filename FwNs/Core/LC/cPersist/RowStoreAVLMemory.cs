namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;

    public class RowStoreAVLMemory : RowStoreAVL
    {
        private readonly Database _database;
        public int RowIdSequence;

        public RowStoreAVLMemory(IPersistentStoreCollection manager, Table table)
        {
            this._database = table.database;
            base.Manager = manager;
            base.table = table;
            base.indexList = table.GetIndexList();
            base.AccessorList = new ICachedObject[base.indexList.Length];
            manager.SetStore(table, this);
        }

        public override void Add(ICachedObject obj)
        {
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
                    return;

                case 3:
                    this.Delete(session, row);
                    return;
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
            if (((session != null) && (xavl != null)) && ((base.table.GetTableType() == 1) || (this._database.TxManager.GetTransactionControl() != 0)))
            {
                return xavl.GetNodeCount(session, this);
            }
            return base.elementCount;
        }

        public override ICachedObject Get(IRowInputInterface input)
        {
            return null;
        }

        public override ICachedObject Get(int i)
        {
            throw Error.RuntimeError(0xc9, "RowStoreAVMemory");
        }

        public override ICachedObject Get(ICachedObject obj, bool keep)
        {
            return obj;
        }

        public override ICachedObject Get(int i, bool keep)
        {
            throw Error.RuntimeError(0xc9, "RowStoreAVMemory");
        }

        public override int GetAccessCount()
        {
            return 0;
        }

        public override DataFileCache GetCache()
        {
            return null;
        }

        public static ICachedObject GetKeep(int i)
        {
            throw Error.RuntimeError(0xc9, "RowStoreAVMemory");
        }

        public override Row GetNewCachedObject(Session session, object obj)
        {
            int num;
            lock (this)
            {
                int rowIdSequence = this.RowIdSequence;
                this.RowIdSequence = rowIdSequence + 1;
                num = rowIdSequence;
            }
            RowAVL row = new RowAVL(base.table, (object[]) obj, num);
            row.SetNewNodes();
            if (session != null)
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
            return 0;
        }

        public override bool IsMemory()
        {
            return true;
        }

        public override void Release()
        {
            ArrayUtil.FillArray(base.AccessorList, null);
        }

        public override void Release(int i)
        {
        }

        public override void Remove(int i)
        {
        }

        public override void RemoveAll()
        {
            base.elementCount = 0;
            ArrayUtil.FillArray(base.AccessorList, null);
        }

        public override void RemovePersistence(int i)
        {
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
                    ((RowAVL) row).SetNewNodes();
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
        }
    }
}

