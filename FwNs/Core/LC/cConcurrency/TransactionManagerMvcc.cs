namespace FwNs.Core.LC.cConcurrency
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Threading;

    public sealed class TransactionManagerMvcc : TransactionManagerCommon, ITransactionManager
    {
        private readonly LongDeque _committedTransactionTimestamps = new LongDeque();
        private readonly UtlDeque<object[]> _committedTransactions = new UtlDeque<object[]>();
        private Session _catalogWriteSession;
        private bool _isLockedMode;
        private int _redoCount;

        public TransactionManagerMvcc(Database db)
        {
            base.database = db;
            base.HasPersistence = base.database.logger.IsLogged();
            base.LobSession = base.database.sessionManager.GetSysLobSession();
            base.RowActionMap = new IntKeyHashMapConcurrent<RowAction>(0x2710);
        }

        public RowAction AddDeleteAction(Session session, Table table, Row row, int[] colMap)
        {
            RowAction item = this.AddDeleteActionToRow(session, table, row, colMap);
            if (item == null)
            {
                lock (base.Lock)
                {
                    this.RollbackAction(session);
                    if ((session.IsolationLevel == 0x10000) || (session.IsolationLevel == 0x100000))
                    {
                        session.TempSet.Clear();
                        session.AbortTransaction = true;
                        throw Error.GetError(0x1307);
                    }
                    if ((row.rowAction != null) && row.rowAction.IsDeleted())
                    {
                        session.TempSet.Clear();
                        session.RedoAction = true;
                        this._redoCount++;
                        throw Error.GetError(0x1307);
                    }
                    if (base.CheckDeadlock(session, session.TempSet))
                    {
                        Session key = session.TempSet.Get(0);
                        session.RedoAction = true;
                        key.WaitingSessions.Add(session);
                        session.WaitedSessions.Add(key);
                        session.Latch.CountUp();
                    }
                    else
                    {
                        session.RedoAction = false;
                        session.AbortTransaction = true;
                    }
                    session.TempSet.Clear();
                    this._redoCount++;
                    throw Error.GetError(0x1307);
                }
            }
            session.RowActionList.Add(item);
            return item;
        }

        private RowAction AddDeleteActionToRow(Session session, Table table, Row row, int[] colMap)
        {
            RowAction action;
            lock (row)
            {
                if (row.IsMemory())
                {
                    action = RowAction.AddDeleteAction(session, table, row, colMap);
                }
                else
                {
                    base.RowActionMap.WriteLock();
                    try
                    {
                        action = base.RowActionMap.Get(row.GetPos());
                        if (action == null)
                        {
                            action = RowAction.AddDeleteAction(session, table, row, colMap);
                            if (action != null)
                            {
                                base.RowActionMap.Put(row.GetPos(), action);
                            }
                            return action;
                        }
                        row.rowAction = action;
                        return RowAction.AddDeleteAction(session, table, row, colMap);
                    }
                    finally
                    {
                        base.RowActionMap.WriteUnLock();
                    }
                }
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

        public void AddToCommittedQueue(Session session, object[] list)
        {
            lock (this._committedTransactionTimestamps)
            {
                this._committedTransactions.AddLast(list);
                this._committedTransactionTimestamps.AddLast(session.ActionTimestamp);
            }
        }

        public void BeginAction(Session session, Statement cs)
        {
            if (!session.IsTransaction && (cs != null))
            {
                lock (base.Lock)
                {
                    session.IsPreTransaction = true;
                    if (this._isLockedMode || cs.IsCatalogChange())
                    {
                        this.BeingActionTpl(session, cs);
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
                session.IsPreTransaction = false;
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

        public bool BeingActionTpl(Session session, Statement cs)
        {
            if (cs != null)
            {
                if (session.AbortTransaction)
                {
                    return false;
                }
                session.TempSet.Clear();
                if (cs.IsCatalogChange())
                {
                    if (this._catalogWriteSession == null)
                    {
                        this.GetTransactionSessions(session.TempSet);
                        session.TempSet.Remove(session);
                        if (session.TempSet.IsEmpty())
                        {
                            this._catalogWriteSession = session;
                            this._isLockedMode = true;
                        }
                        else
                        {
                            this._catalogWriteSession = session;
                            this._isLockedMode = true;
                            TransactionManagerCommon.SetWaitingSessionTpl(session);
                        }
                        return true;
                    }
                    this._catalogWriteSession.WaitingSessions.Add(session);
                    session.Latch.CountUp();
                    return true;
                }
                if (this._isLockedMode)
                {
                    if ((cs.GetTableNamesForRead().Length == 0) && (cs.GetTableNamesForWrite().Length == 0))
                    {
                        return true;
                    }
                    if (cs.GetTableNamesForWrite().Length != 0)
                    {
                        if (cs.GetTableNamesForWrite()[0].schema == SqlInvariants.LobsSchemaQname)
                        {
                            return true;
                        }
                    }
                    else if ((cs.GetTableNamesForRead().Length != 0) && (cs.GetTableNamesForRead()[0].schema == SqlInvariants.LobsSchemaQname))
                    {
                        return true;
                    }
                    this._catalogWriteSession.WaitingSessions.Add(session);
                    session.Latch.CountUp();
                }
            }
            return true;
        }

        public bool CanRead(Session session, int id, int mode)
        {
            RowAction action = base.RowActionMap.Get(id);
            if (action != null)
            {
                return action.CanRead(session, mode);
            }
            return true;
        }

        public bool CanRead(Session session, Row row, int mode, int[] colMap)
        {
            RowAction rowAction = row.rowAction;
            if (mode == 0)
            {
                return ((rowAction == null) || rowAction.CanRead(session, 0));
            }
            if (mode == 2)
            {
                return ((rowAction == null) || rowAction.CanRead(session, 0));
            }
            return ((rowAction == null) || rowAction.CanRead(session, mode));
        }

        public bool CommitTransaction(Session session)
        {
            if (session.AbortTransaction)
            {
                return false;
            }
            int count = session.RowActionList.Count;
            RowAction[] list = session.RowActionList.ToArray();
            lock (base.Lock)
            {
                for (int i = 0; i < count; i++)
                {
                    if (!list[i].CanCommit(session, session.TempSet))
                    {
                        return false;
                    }
                }
                this.EndTransaction(session);
                session.ActionTimestamp = base.NextChangeTimestamp();
                session.TransactionEndTimestamp = session.ActionTimestamp;
                for (int j = 0; j < count; j++)
                {
                    list[j].Commit(session);
                }
                for (int k = 0; k < session.TempSet.Size(); k++)
                {
                    session.TempSet.Get(k).AbortTransaction = true;
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
                CountDownLatches(session);
            }
            session.TempSet.Clear();
            if ((session != base.LobSession) && (base.LobSession.RowActionList.Count > 0))
            {
                base.LobSession.IsTransaction = true;
                base.LobSession.ActionIndex = base.LobSession.RowActionList.Count;
                base.LobSession.Commit(false);
                return true;
            }
            return true;
        }

        public void CompleteActions(Session session)
        {
        }

        private static void CountDownLatches(Session session)
        {
            for (int i = 0; i < session.WaitingSessions.Size(); i++)
            {
                Session local1 = session.WaitingSessions.Get(i);
                local1.WaitedSessions.Remove(session);
                local1.Latch.CountDown();
            }
            session.WaitingSessions.Clear();
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

        public override void EndTransactionTpl(Session session)
        {
            if (this._catalogWriteSession == session)
            {
                this._catalogWriteSession = null;
                this._isLockedMode = false;
            }
        }

        public long GetGlobalChangeTimestamp()
        {
            return Interlocked.Read(ref this.GlobalChangeTimestamp);
        }

        public int GetTransactionControl()
        {
            return 2;
        }

        public void GetTransactionSessions(UtlHashSet<Session> set)
        {
            Session[] allSessions = base.database.sessionManager.GetAllSessions();
            for (int i = 0; i < allSessions.Length; i++)
            {
                long transactionTimestamp = allSessions[i].GetTransactionTimestamp();
                if (base.LiveTransactionTimestamps.Contains(transactionTimestamp))
                {
                    set.Add(allSessions[i]);
                }
                else if (allSessions[i].IsPreTransaction)
                {
                    set.Add(allSessions[i]);
                }
            }
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
                long first;
                object[] objArray;
                lock (this._committedTransactionTimestamps)
                {
                    if (this._committedTransactionTimestamps.IsEmpty())
                    {
                        break;
                    }
                    first = this._committedTransactionTimestamps.GetFirst();
                    if (first >= firstLiveTransactionTimestamp)
                    {
                        break;
                    }
                    this._committedTransactionTimestamps.RemoveFirst();
                    objArray = this._committedTransactions.RemoveFirst();
                }
                TransactionManagerCommon.MergeTransaction(session, objArray, 0, objArray.Length, first);
                base.FinaliseRows(session, objArray, 0, objArray.Length, true);
            }
        }

        public bool PrepareCommitActions(Session session)
        {
            bool flag2;
            RowAction[] actionArray = session.RowActionList.ToArray();
            int count = session.RowActionList.Count;
            if (session.AbortTransaction)
            {
                return false;
            }
            Monitor.Enter(base.Lock);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (!actionArray[i].CanCommit(session, session.TempSet))
                    {
                        return false;
                    }
                }
                session.ActionTimestamp = base.NextChangeTimestamp();
                for (int j = 0; j < count; j++)
                {
                    actionArray[j].PrepareCommit(session);
                }
                for (int k = 0; k < session.TempSet.Size(); k++)
                {
                    session.TempSet.Get(k).AbortTransaction = true;
                }
                flag2 = true;
            }
            finally
            {
                Monitor.Exit(base.Lock);
                session.TempSet.Clear();
            }
            return flag2;
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
                session.TransactionEndTimestamp = session.ActionTimestamp;
                this.RollbackPartial(session, 0, session.TransactionTimestamp);
                this.EndTransaction(session);
                this.EndTransactionTpl(session);
                CountDownLatches(session);
            }
        }

        public void RollbackAction(Session session)
        {
            this.RollbackPartial(session, session.ActionIndex, session.ActionTimestamp);
        }

        public void RollbackPartial(Session session, int start, long timestamp)
        {
            RowAction[] list = session.RowActionList.ToArray();
            int count = session.RowActionList.Count;
            if (start != count)
            {
                for (int i = start; i < count; i++)
                {
                    RowAction action = list[i];
                    if (action != null)
                    {
                        action.Rollback(session, timestamp);
                    }
                }
                lock (base.Lock)
                {
                    TransactionManagerCommon.MergeRolledBackTransaction(session, timestamp, list, start, count);
                    base.FinaliseRows(session, list, start, count, false);
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
                TransactionManagerMV2PL rmvpl;
                if (base.LiveTransactionTimestamps.Size() != 1)
                {
                    goto Label_00BC;
                }
                if (mode != 0)
                {
                    if (mode == 1)
                    {
                        goto Label_006B;
                    }
                }
                else
                {
                    TransactionManager2PL managerpl = new TransactionManager2PL(base.database);
                    Interlocked.Exchange(ref managerpl.GlobalChangeTimestamp, Interlocked.Read(ref this.GlobalChangeTimestamp));
                    managerpl.LiveTransactionTimestamps.AddLast(session.TransactionTimestamp);
                    base.database.TxManager = managerpl;
                }
                return;
            Label_006B:
                rmvpl = new TransactionManagerMV2PL(base.database);
                Interlocked.Exchange(ref rmvpl.GlobalChangeTimestamp, Interlocked.Read(ref this.GlobalChangeTimestamp));
                rmvpl.LiveTransactionTimestamps.AddLast(session.TransactionTimestamp);
                base.database.TxManager = rmvpl;
                return;
            Label_00BC:;
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

