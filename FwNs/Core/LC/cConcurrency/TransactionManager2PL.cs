namespace FwNs.Core.LC.cConcurrency
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Threading;

    public class TransactionManager2PL : TransactionManagerCommon, ITransactionManager
    {
        public TransactionManager2PL(Database db)
        {
            base.database = db;
            base.LobSession = base.database.sessionManager.GetSysLobSession();
            base.TxModel = 0;
        }

        public RowAction AddDeleteAction(Session session, Table table, Row row, int[] colMap)
        {
            RowAction action;
            lock (row)
            {
                action = RowAction.AddDeleteAction(session, table, row, colMap);
            }
            session.RowActionList.Add(action);
            session.sessionData.GetRowStore(table).Delete(session, row);
            row.rowAction = null;
            return action;
        }

        public void AddInsertAction(Session session, Table table, Row row)
        {
            RowAction rowAction = row.rowAction;
            session.RowActionList.Add(rowAction);
        }

        public void BeginAction(Session session, Statement cs)
        {
            if (!session.HasLocks(cs))
            {
                lock (base.Lock)
                {
                    if (base.SetWaitedSessionsTpl(session, cs))
                    {
                        if (session.TempSet.IsEmpty())
                        {
                            base.LockTablesTpl(session, cs);
                        }
                        else
                        {
                            TransactionManagerCommon.SetWaitingSessionTpl(session);
                        }
                    }
                }
            }
        }

        public void BeginActionResume(Session session)
        {
            session.ActionTimestamp = base.NextChangeTimestamp();
            if (!session.IsTransaction)
            {
                session.TransactionTimestamp = session.ActionTimestamp;
                session.IsTransaction = true;
                base.TransactionCount++;
            }
        }

        public void BeginTransaction(Session session)
        {
            if (!session.IsTransaction)
            {
                session.ActionTimestamp = base.NextChangeTimestamp();
                session.TransactionTimestamp = session.ActionTimestamp;
                session.IsTransaction = true;
                base.TransactionCount++;
            }
        }

        public bool CanRead(Session session, int id, int mode)
        {
            return true;
        }

        public bool CanRead(Session session, Row row, int mode, int[] colMap)
        {
            return true;
        }

        public bool CommitTransaction(Session session)
        {
            if (session.AbortTransaction)
            {
                return false;
            }
            int count = session.RowActionList.Count;
            object[] list = session.RowActionList.ToArray();
            lock (base.Lock)
            {
                session.ActionTimestamp = base.NextChangeTimestamp();
                session.TransactionEndTimestamp = session.ActionTimestamp;
                this.EndTransaction(session);
                for (int i = 0; i < count; i++)
                {
                    ((RowAction) list[i]).Commit(session);
                }
                base.PersistCommit(session, list, count);
                this.EndTransactionTpl(session);
            }
            session.TempSet.Clear();
            return true;
        }

        public void CompleteActions(Session session)
        {
            base.EndActionTpl(session);
        }

        public override void ConvertTransactionIDs(DoubleIntIndex lookup)
        {
        }

        public void EndTransaction(Session session)
        {
            if (session.IsTransaction)
            {
                session.IsTransaction = false;
                base.TransactionCount--;
            }
        }

        public long GetGlobalChangeTimestamp()
        {
            return Interlocked.Read(ref this.GlobalChangeTimestamp);
        }

        public int GetTransactionControl()
        {
            return 0;
        }

        public override DoubleIntIndex GetTransactionIDList()
        {
            return new DoubleIntIndex(10, false);
        }

        public bool IsMvcc()
        {
            return false;
        }

        public bool IsMvRows()
        {
            return false;
        }

        public bool PrepareCommitActions(Session session)
        {
            session.ActionTimestamp = base.NextChangeTimestamp();
            return true;
        }

        public void RemoveTransactionInfo(ICachedObject obj)
        {
        }

        public void Rollback(Session session)
        {
            session.AbortTransaction = false;
            session.ActionTimestamp = base.NextChangeTimestamp();
            session.TransactionEndTimestamp = session.ActionTimestamp;
            this.RollbackPartial(session, 0, session.TransactionTimestamp);
            this.EndTransaction(session);
            lock (base.Lock)
            {
                this.EndTransactionTpl(session);
            }
        }

        public void RollbackAction(Session session)
        {
            this.RollbackPartial(session, session.ActionIndex, session.ActionTimestamp);
            base.EndActionTpl(session);
        }

        public void RollbackPartial(Session session, int start, long timestamp)
        {
            object[] objArray = session.RowActionList.ToArray();
            int count = session.RowActionList.Count;
            if (start != count)
            {
                for (int i = count - 1; i >= start; i--)
                {
                    RowAction action = (RowAction) objArray[i];
                    if (((action != null) && (action.type != 0)) && (action.type != 3))
                    {
                        Row memoryRow = action.MemoryRow;
                        if (memoryRow == null)
                        {
                            memoryRow = (Row) action.Store.Get(action.GetPos(), false);
                        }
                        if (memoryRow != null)
                        {
                            action.Rollback(session, timestamp);
                            int changeAction = action.MergeRollback(session, timestamp, memoryRow);
                            action.Store.RollbackRow(session, memoryRow, changeAction, base.TxModel);
                        }
                    }
                }
                session.RowActionList.RemoveRange(start, session.RowActionList.Count - start);
            }
        }

        public void RollbackSavepoint(Session session, int index)
        {
            long timestamp = session.sessionContext.SavepointTimestamps.Get(index);
            int start = session.sessionContext.Savepoints.Get(index);
            while (session.sessionContext.Savepoints.Size() > (index + 1))
            {
                session.sessionContext.Savepoints.Remove((int) (session.sessionContext.Savepoints.Size() - 1));
                session.sessionContext.SavepointTimestamps.RemoveLast();
            }
            this.RollbackPartial(session, start, timestamp);
        }

        public void SetTransactionControl(Session session, int mode)
        {
            lock (base.Lock)
            {
                TransactionManagerMvcc mvcc;
                if (mode != 1)
                {
                    if (mode == 2)
                    {
                        goto Label_005B;
                    }
                }
                else
                {
                    TransactionManagerMV2PL rmvpl = new TransactionManagerMV2PL(base.database);
                    Interlocked.Exchange(ref rmvpl.GlobalChangeTimestamp, Interlocked.Read(ref this.GlobalChangeTimestamp));
                    rmvpl.LiveTransactionTimestamps.AddLast(session.TransactionTimestamp);
                    base.database.TxManager = rmvpl;
                }
                goto Label_00AA;
            Label_005B:
                mvcc = new TransactionManagerMvcc(base.database);
                Interlocked.Exchange(ref mvcc.GlobalChangeTimestamp, Interlocked.Read(ref this.GlobalChangeTimestamp));
                mvcc.LiveTransactionTimestamps.AddLast(session.TransactionTimestamp);
                base.database.TxManager = mvcc;
            Label_00AA:;
            }
        }

        public void SetTransactionInfo(ICachedObject obj)
        {
        }
    }
}

