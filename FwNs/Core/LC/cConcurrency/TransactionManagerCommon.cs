namespace FwNs.Core.LC.cConcurrency
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cStatements;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class TransactionManagerCommon : IDisposable
    {
        public Database database;
        public Session LobSession;
        protected bool HasPersistence;
        protected int TxModel = 2;
        protected QNameManager.QName[] catalogNameList;
        protected object Lock = new object();
        public LongDeque LiveTransactionTimestamps = new LongDeque();
        public long GlobalChangeTimestamp;
        public int TransactionCount;
        private readonly HashMap<QNameManager.QName, Session> _tableWriteLocks = new HashMap<QNameManager.QName, Session>();
        private readonly MultiValueHashMap<QNameManager.QName, Session> _tableReadLocks = new MultiValueHashMap<QNameManager.QName, Session>();
        public IntKeyHashMapConcurrent<RowAction> RowActionMap;

        public bool CheckDeadlock(Session session, OrderedHashSet<Session> newWaits)
        {
            int num = session.WaitingSessions.Size();
            int index = 0;
            while (index < num)
            {
                Session key = session.WaitingSessions.Get(index);
                if (!newWaits.Contains(key) && this.CheckDeadlock(key, newWaits))
                {
                    index++;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public virtual void ConvertTransactionIDs(DoubleIntIndex lookup)
        {
            lock (this.Lock)
            {
                RowAction[] actionArray = new RowAction[this.RowActionMap.Size()];
                Iterator<RowAction> iterator = this.RowActionMap.GetValues().GetIterator();
                for (int i = 0; iterator.HasNext(); i++)
                {
                    actionArray[i] = iterator.Next();
                }
                this.RowActionMap.Clear();
                for (int j = 0; j < actionArray.Length; j++)
                {
                    int firstEqual = lookup.LookupFirstEqual(actionArray[j].GetPos());
                    actionArray[j].SetPos(firstEqual);
                    this.RowActionMap.Put(firstEqual, actionArray[j]);
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
        }

        public void EndActionTpl(Session session)
        {
            if (((session.IsolationLevel != 0x10000) && (session.IsolationLevel != 0x100000)) && ((session.sessionContext.CurrentStatement != null) && (session.sessionContext.Depth <= 0)))
            {
                QNameManager.QName[] tableNamesForRead = session.sessionContext.CurrentStatement.GetTableNamesForRead();
                if (tableNamesForRead.Length != 0)
                {
                    lock (this.Lock)
                    {
                        this.UnlockReadTablesTpl(session, tableNamesForRead);
                        int num = session.WaitingSessions.Size();
                        if (num != 0)
                        {
                            bool flag = false;
                            for (int i = 0; i < tableNamesForRead.Length; i++)
                            {
                                if (this._tableWriteLocks.Get(tableNamesForRead[i]) != session)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                flag = false;
                                for (int j = 0; j < num; j++)
                                {
                                    Session session2 = session.WaitingSessions.Get(j);
                                    if (ArrayUtil.ContainsAny(tableNamesForRead, session2.sessionContext.CurrentStatement.GetTableNamesForWrite()))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    this.ResetLocks(session);
                                    this.ResetLatchesMidTransaction(session);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void EndTransactionTpl(Session session)
        {
            this.UnlockTablesTpl(session);
            if (session.WaitingSessions.Size() != 0)
            {
                this.ResetLocks(session);
                this.ResetLatches(session);
            }
        }

        public void FinaliseRows(Session session, object[] list, int start, int limit, bool commit)
        {
            bool flag = false;
            for (int i = start; i < limit; i++)
            {
                RowAction action = (RowAction) list[i];
                if (!action.IsMemory && (action.type == 0))
                {
                    this.RowActionMap.WriteLock();
                    try
                    {
                        lock (action)
                        {
                            if (action.type == 0)
                            {
                                this.RowActionMap.Remove(action.GetPos());
                            }
                        }
                    }
                    finally
                    {
                        this.RowActionMap.WriteUnLock();
                    }
                }
                if ((action.type == 3) && !action.DeleteComplete)
                {
                    try
                    {
                        action.DeleteComplete = true;
                        if (action.table.GetTableType() != 3)
                        {
                            Row row = action.MemoryRow ?? ((Row) action.Store.Get(action.GetPos(), false));
                            if (commit && action.table.hasLobColumn)
                            {
                                flag = true;
                            }
                            action.Store.CommitRow(session, row, action.type, this.TxModel);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (flag && (this.TransactionCount == 0))
            {
                this.database.lobManager.DeleteUnusedLobs();
            }
        }

        public long GetFirstLiveTransactionTimestamp()
        {
            if (this.LiveTransactionTimestamps.IsEmpty())
            {
                return 0x7fffffffffffffffL;
            }
            return this.LiveTransactionTimestamps.Get(0);
        }

        public RowAction[] GetRowActionList()
        {
            lock (this.Lock)
            {
                bool flag;
                Session[] allSessions = this.database.sessionManager.GetAllSessions();
                int[] numArray = new int[allSessions.Length];
                int num = 0;
                int num2 = 0;
                for (int i = 0; i < allSessions.Length; i++)
                {
                    num2 += allSessions[i].GetTransactionSize();
                }
                RowAction[] actionArray2 = new RowAction[num2];
            Label_0053:
                flag = false;
                long actionTimestamp = 0x7fffffffffffffffL;
                int index = 0;
                for (int j = 0; j < allSessions.Length; j++)
                {
                    int transactionSize = allSessions[j].GetTransactionSize();
                    if (numArray[j] < transactionSize)
                    {
                        RowAction action = allSessions[j].RowActionList[numArray[j]];
                        if (action.ActionTimestamp < actionTimestamp)
                        {
                            actionTimestamp = action.ActionTimestamp;
                            index = j;
                        }
                        flag = true;
                    }
                }
                if (flag)
                {
                    List<RowAction> rowActionList = allSessions[index].RowActionList;
                    while (numArray[index] < rowActionList.Count)
                    {
                        RowAction action2 = rowActionList[numArray[index]];
                        if (action2.ActionTimestamp == (actionTimestamp + 1L))
                        {
                            actionTimestamp += 1L;
                        }
                        if (action2.ActionTimestamp != actionTimestamp)
                        {
                            break;
                        }
                        actionArray2[num++] = action2;
                        numArray[index]++;
                    }
                    goto Label_0053;
                }
                return actionArray2;
            }
        }

        public virtual DoubleIntIndex GetTransactionIDList()
        {
            lock (this.Lock)
            {
                DoubleIntIndex index2 = new DoubleIntIndex(this.RowActionMap.Size(), false);
                index2.SetKeysSearchTarget();
                Iterator<int> iterator = this.RowActionMap.GetKeySet().GetIterator();
                while (iterator.HasNext())
                {
                    index2.AddUnique(iterator.Next(), 0);
                }
                return index2;
            }
        }

        public bool HasLocks(Session session, Statement cs)
        {
            if (cs != null)
            {
                foreach (QNameManager.QName name in cs.GetTableNamesForWrite())
                {
                    if (name.schema != SqlInvariants.SystemSchemaQname)
                    {
                        Session session2 = this._tableWriteLocks.Get(name);
                        if ((session2 != null) && (session2 != session))
                        {
                            return false;
                        }
                        Iterator<Session> iterator = this._tableReadLocks.Get(name);
                        while (iterator.HasNext())
                        {
                            if (iterator.Next() != session)
                            {
                                return false;
                            }
                        }
                    }
                }
                foreach (QNameManager.QName name2 in cs.GetTableNamesForRead())
                {
                    if (name2.schema != SqlInvariants.SystemSchemaQname)
                    {
                        Session session3 = this._tableWriteLocks.Get(name2);
                        if ((session3 != null) && (session3 != session))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void LockTablesTpl(Session session, Statement cs)
        {
            if ((cs != null) && !session.AbortTransaction)
            {
                foreach (QNameManager.QName name in cs.GetTableNamesForWrite())
                {
                    if (name.schema != SqlInvariants.SystemSchemaQname)
                    {
                        this._tableWriteLocks.Put(name, session);
                    }
                }
                foreach (QNameManager.QName name2 in cs.GetTableNamesForRead())
                {
                    if (name2.schema != SqlInvariants.SystemSchemaQname)
                    {
                        this._tableReadLocks.Put(name2, session);
                    }
                }
            }
        }

        public static void MergeRolledBackTransaction(Session session, long timestamp, object[] list, int start, int limit)
        {
            int index = start;
            while (index < limit)
            {
                RowAction action = (RowAction) list[index];
                Row memoryRow = action.MemoryRow;
                if (memoryRow != null)
                {
                    goto Label_003F;
                }
                if (action.type != 0)
                {
                    memoryRow = (Row) action.Store.Get(action.GetPos(), false);
                    goto Label_003F;
                }
            Label_0039:
                index++;
                continue;
            Label_003F:
                if (memoryRow == null)
                {
                    goto Label_0039;
                }
                lock (memoryRow)
                {
                    action.MergeRollback(session, timestamp, memoryRow);
                    goto Label_0039;
                }
            }
        }

        public static void MergeTransaction(Session session, object[] list, int start, int limit, long timestamp)
        {
            for (int i = start; i < limit; i++)
            {
                ((RowAction) list[i]).MergeToTimestamp(timestamp);
            }
        }

        public long NextChangeTimestamp()
        {
            return Interlocked.Increment(ref this.GlobalChangeTimestamp);
        }

        public void PersistCommit(Session session, object[] list, int limit)
        {
            bool flag = false;
            for (int i = 0; i < limit; i++)
            {
                RowAction action = (RowAction) list[i];
                if (action.type != 0)
                {
                    int commitTypeOn = action.GetCommitTypeOn(session.ActionTimestamp);
                    Row row = action.MemoryRow ?? ((Row) action.Store.Get(action.GetPos(), false));
                    if (action.table.hasLobColumn)
                    {
                        switch (commitTypeOn)
                        {
                            case 1:
                                session.sessionData.AdjustLobUsageCount(action.table, row.RowData, 1);
                                break;

                            case 2:
                                session.sessionData.AdjustLobUsageCount(action.table, row.RowData, -1);
                                flag = true;
                                break;
                        }
                    }
                    try
                    {
                        action.Store.CommitRow(session, row, commitTypeOn, this.TxModel);
                    }
                    catch (CoreException exception)
                    {
                        this.database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.TransactionManagerCommon_PersistCommit_data_commit_failed, exception);
                    }
                }
            }
            try
            {
                if (flag && (this.TransactionCount == 0))
                {
                    this.database.lobManager.DeleteUnusedLobs();
                }
                session.LogSequences();
                if (limit > 0)
                {
                    this.database.logger.WriteCommitStatement(session);
                }
            }
            catch (CoreException)
            {
            }
        }

        public void ResetLatches(Session session)
        {
            int num = session.WaitingSessions.Size();
            for (int i = 0; i < num; i++)
            {
                Session session2 = session.WaitingSessions.Get(i);
                if (!session2.AbortTransaction)
                {
                    session2.TempSet.IsEmpty();
                }
                SetWaitingSessionTpl(session2);
            }
            session.WaitingSessions.Clear();
        }

        public void ResetLatchesMidTransaction(Session session)
        {
            session.TempSet.Clear();
            session.TempSet.AddAll(session.WaitingSessions);
            session.WaitingSessions.Clear();
            int num = session.TempSet.Size();
            for (int i = 0; i < num; i++)
            {
                Session session2 = session.TempSet.Get(i);
                if (!session2.AbortTransaction)
                {
                    session2.TempSet.IsEmpty();
                }
                SetWaitingSessionTpl(session2);
            }
            session.TempSet.Clear();
        }

        public void ResetLocks(Session session)
        {
            int num = session.WaitingSessions.Size();
            for (int i = 0; i < num; i++)
            {
                Session session2 = session.WaitingSessions.Get(i);
                session2.TempUnlocked = false;
                if (((session2.Latch.GetCount() == 1L) && this.SetWaitedSessionsTpl(session2, session2.sessionContext.CurrentStatement)) && session2.TempSet.IsEmpty())
                {
                    this.LockTablesTpl(session2, session2.sessionContext.CurrentStatement);
                    session2.TempUnlocked = true;
                }
            }
            for (int j = 0; j < num; j++)
            {
                Session session3 = session.WaitingSessions.Get(j);
                if (!session3.TempUnlocked && !session3.AbortTransaction)
                {
                    this.SetWaitedSessionsTpl(session3, session3.sessionContext.CurrentStatement);
                }
            }
        }

        public bool SetWaitedSessionsTpl(Session session, Statement cs)
        {
            session.TempSet.Clear();
            if (cs == null)
            {
                return true;
            }
            if (!session.AbortTransaction)
            {
                foreach (QNameManager.QName name in cs.GetTableNamesForWrite())
                {
                    if (name.schema != SqlInvariants.SystemSchemaQname)
                    {
                        Session key = this._tableWriteLocks.Get(name);
                        if ((key != null) && (key != session))
                        {
                            session.TempSet.Add(key);
                        }
                        Iterator<Session> iterator = this._tableReadLocks.Get(name);
                        while (iterator.HasNext())
                        {
                            key = iterator.Next();
                            if (key != session)
                            {
                                session.TempSet.Add(key);
                            }
                        }
                    }
                }
                QNameManager.QName[] tableNamesForRead = cs.GetTableNamesForRead();
                if ((this.TxModel == 1) && session.IsReadOnly())
                {
                    tableNamesForRead = this.catalogNameList;
                }
                for (int i = 0; i < tableNamesForRead.Length; i++)
                {
                    QNameManager.QName key = tableNamesForRead[i];
                    if (key.schema != SqlInvariants.SystemSchemaQname)
                    {
                        Session session3 = this._tableWriteLocks.Get(key);
                        if ((session3 != null) && (session3 != session))
                        {
                            session.TempSet.Add(session3);
                        }
                    }
                }
                if (session.TempSet.IsEmpty())
                {
                    return true;
                }
                if (this.CheckDeadlock(session, session.TempSet))
                {
                    return true;
                }
                session.TempSet.Clear();
                session.AbortTransaction = true;
            }
            return false;
        }

        public static void SetWaitingSessionTpl(Session session)
        {
            int count = session.TempSet.Size();
            for (int i = 0; i < count; i++)
            {
                session.TempSet.Get(i).WaitingSessions.Add(session);
            }
            session.TempSet.Clear();
            session.Latch.SetCount(count);
        }

        public void UnlockReadTablesTpl(Session session, QNameManager.QName[] locks)
        {
            for (int i = 0; i < locks.Length; i++)
            {
                this._tableReadLocks.Remove(locks[i], session);
            }
        }

        public void UnlockTablesTpl(Session session)
        {
            Iterator<Session> iterator = this._tableWriteLocks.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                if (iterator.Next() == session)
                {
                    iterator.SetValue(null);
                }
            }
            Iterator<Session> iterator2 = this._tableReadLocks.GetValues().GetIterator();
            while (iterator2.HasNext())
            {
                if (iterator2.Next() == session)
                {
                    iterator.Remove();
                }
            }
        }
    }
}

