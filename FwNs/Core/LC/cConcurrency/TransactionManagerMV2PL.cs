namespace FwNs.Core.LC.cConcurrency
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Threading;

    public class TransactionManagerMV2PL : TransactionManagerCommon, ITransactionManager
    {
        private UtlDeque<object[]> committedTransactions = new UtlDeque<object[]>();
        private LongDeque committedTransactionTimestamps = new LongDeque();

        public TransactionManagerMV2PL(Database db)
        {
            base.database = db;
            base.HasPersistence = base.database.logger.IsLogged();
            base.LobSession = base.database.sessionManager.GetSysLobSession();
            base.RowActionMap = new IntKeyHashMapConcurrent<RowAction>(0x2710);
            base.TxModel = 1;
            base.catalogNameList = new QNameManager.QName[] { base.database.GetCatalogName() };
        }

        public RowAction AddDeleteAction(Session session, Table table, Row row, int[] colMap)
        {
            bool flag;
            RowAction action;
            lock (row)
            {
                flag = row.rowAction == null;
                action = RowAction.AddDeleteAction(session, table, row, colMap);
            }
            session.RowActionList.Add(action);
            if (flag && !row.IsMemory())
            {
                base.RowActionMap.Put(action.GetPos(), action);
            }
            return action;
        }

        public void AddInsertAction(Session session, Table table, Row row)
        {
            RowAction rowAction = row.rowAction;
            session.RowActionList.Add(rowAction);
            if (!row.IsMemory())
            {
                base.RowActionMap.Put(rowAction.GetPos(), rowAction);
            }
        }

        private void AddToCommittedQueue(Session session, object[] list)
        {
            lock (this.committedTransactionTimestamps)
            {
                this.committedTransactions.AddLast(list);
                this.committedTransactionTimestamps.AddLast(session.ActionTimestamp);
            }
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
                    else
                    {
                        session.AbortTransaction = true;
                    }
                }
            }
        }

        public void BeginActionResume(Session session)
        {
            lock (base.Lock)
            {
                session.ActionTimestamp = base.NextChangeTimestamp();
                if (!session.IsTransaction)
                {
                    session.TransactionTimestamp = session.ActionTimestamp;
                    session.IsTransaction = true;
                    base.LiveTransactionTimestamps.AddLast(session.ActionTimestamp);
                    base.TransactionCount++;
                }
            }
        }

        public void BeginTransaction(Session session)
        {
            lock (base.Lock)
            {
                session.ActionTimestamp = base.NextChangeTimestamp();
                session.TransactionTimestamp = session.ActionTimestamp;
                session.IsTransaction = true;
                base.LiveTransactionTimestamps.AddLast(session.TransactionTimestamp);
                base.TransactionCount++;
            }
        }

        public bool CanRead(Session session, int id, int mode)
        {
            RowAction action = base.RowActionMap.Get(id);
            if (action != null)
            {
                return action.CanRead(session, 0);
            }
            return true;
        }

        public bool CanRead(Session session, Row row, int mode, int[] colMap)
        {
            RowAction rowAction = row.rowAction;
            if (rowAction != null)
            {
                return rowAction.CanRead(session, 0);
            }
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
                this.EndTransaction(session);
                session.ActionTimestamp = base.NextChangeTimestamp();
                for (int i = 0; i < count; i++)
                {
                    ((RowAction) list[i]).Commit(session);
                }
                base.PersistCommit(session, list, count);
                if (base.GetFirstLiveTransactionTimestamp() > session.ActionTimestamp)
                {
                    TransactionManagerCommon.MergeTransaction(session, list, 0, count, session.ActionTimestamp);
                    base.FinaliseRows(session, list, 0, count, true);
                }
                else
                {
                    list = session.RowActionList.ToArray();
                    this.AddToCommittedQueue(session, list);
                }
                this.EndTransactionTpl(session);
            }
            session.TempSet.Clear();
            if ((session != base.LobSession) && (base.LobSession.RowActionList.Count > 0))
            {
                base.LobSession.IsTransaction = true;
                base.LobSession.ActionIndex = base.LobSession.RowActionList.Count;
                base.LobSession.Commit(false);
            }
            return true;
        }

        public void CompleteActions(Session session)
        {
            base.EndActionTpl(session);
        }

        public override void ConvertTransactionIDs(DoubleIntIndex lookup)
        {
            base.ConvertTransactionIDs(lookup);
        }

        public void EndTransaction(Session session)
        {
            long transactionTimestamp = session.TransactionTimestamp;
            session.IsTransaction = false;
            int index = base.LiveTransactionTimestamps.IndexOf(transactionTimestamp);
            if (index >= 0)
            {
                base.TransactionCount--;
                base.LiveTransactionTimestamps.Remove(index);
                this.MergeExpiredTransactions(session);
            }
        }

        public long GetGlobalChangeTimestamp()
        {
            return Interlocked.Read(ref this.GlobalChangeTimestamp);
        }

        public int GetTransactionControl()
        {
            return 1;
        }

        public override DoubleIntIndex GetTransactionIDList()
        {
            return base.GetTransactionIDList();
        }

        public bool IsMvRows()
        {
            return true;
        }

        public void MergeExpiredTransactions(Session session)
        {
            long firstLiveTransactionTimestamp = base.GetFirstLiveTransactionTimestamp();
            while (true)
            {
                long timestamp = 0L;
                object[] list = null;
                lock (this.committedTransactionTimestamps)
                {
                    if (this.committedTransactionTimestamps.IsEmpty())
                    {
                        break;
                    }
                    timestamp = this.committedTransactionTimestamps.GetFirst();
                    if (timestamp >= firstLiveTransactionTimestamp)
                    {
                        break;
                    }
                    this.committedTransactionTimestamps.RemoveFirst();
                    list = this.committedTransactions.RemoveFirst();
                }
                TransactionManagerCommon.MergeTransaction(session, list, 0, list.Length, timestamp);
                base.FinaliseRows(session, list, 0, list.Length, true);
            }
        }

        public bool PrepareCommitActions(Session session)
        {
            object[] objArray = session.RowActionList.ToArray();
            int count = session.RowActionList.Count;
            lock (base.Lock)
            {
                session.ActionTimestamp = base.NextChangeTimestamp();
                for (int i = 0; i < count; i++)
                {
                    ((RowAction) objArray[i]).PrepareCommit(session);
                }
                return true;
            }
        }

        public void RemoveTransactionInfo(ICachedObject obj)
        {
            base.RowActionMap.Remove(obj.GetPos());
        }

        public void Rollback(Session session)
        {
            lock (base.Lock)
            {
                session.AbortTransaction = false;
                session.ActionTimestamp = base.NextChangeTimestamp();
                this.RollbackPartial(session, 0, session.TransactionTimestamp);
                this.EndTransaction(session);
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
            object[] list = session.RowActionList.ToArray();
            int count = session.RowActionList.Count;
            if (start != count)
            {
                for (int i = start; i < count; i++)
                {
                    RowAction action = (RowAction) list[i];
                    if (action != null)
                    {
                        action.Rollback(session, timestamp);
                    }
                }
                TransactionManagerCommon.MergeRolledBackTransaction(session, timestamp, list, start, count);
                base.FinaliseRows(session, list, start, count, false);
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
                if (base.LiveTransactionTimestamps.Size() != 1)
                {
                    goto Label_00AA;
                }
                if (mode != 0)
                {
                    if (mode == 2)
                    {
                        goto Label_0059;
                    }
                }
                else
                {
                    TransactionManager2PL managerpl = new TransactionManager2PL(base.database);
                    Interlocked.Exchange(ref managerpl.GlobalChangeTimestamp, Interlocked.Read(ref this.GlobalChangeTimestamp));
                    base.database.TxManager = managerpl;
                }
                return;
            Label_0059:
                mvcc = new TransactionManagerMvcc(base.database);
                Interlocked.Exchange(ref mvcc.GlobalChangeTimestamp, Interlocked.Read(ref this.GlobalChangeTimestamp));
                mvcc.LiveTransactionTimestamps.AddLast(session.TransactionTimestamp);
                base.database.TxManager = mvcc;
                return;
            Label_00AA:;
            }
            throw Error.GetError(0xe75);
        }

        public void SetTransactionInfo(ICachedObject obj)
        {
            Row row = (Row) obj;
            RowAction action = base.RowActionMap.Get(row.Position);
            row.rowAction = action;
        }
    }
}

