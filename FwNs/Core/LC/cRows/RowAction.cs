namespace FwNs.Core.LC.cRows
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class RowAction : RowActionBase
    {
        public bool IsMemory;
        public Row MemoryRow;
        public int RowId;
        public IPersistentStore Store;
        public RowAction UpdatedAction;
        public TableBase table;

        public RowAction(Session session, TableBase table, byte type, Row row, int[] colMap)
        {
            base.session = session;
            base.type = type;
            base.ActionTimestamp = session.ActionTimestamp;
            this.table = table;
            this.Store = session.sessionData.GetRowStore(table);
            this.IsMemory = row.IsMemory();
            this.MemoryRow = row;
            this.RowId = row.GetPos();
            base.ChangeColumnMap = colMap;
        }

        public RowAction AddDeleteAction(Session session, int[] colMap)
        {
            RowAction action;
            lock (this)
            {
                RowActionBase base1;
                if (base.type == 0)
                {
                    this.SetAsAction(session, 2);
                    base.ChangeColumnMap = colMap;
                    goto Label_00E6;
                }
                RowActionBase next = this;
            Label_0024:
                if (next.type == 1)
                {
                    if ((next.CommitTimestamp == 0) && (session != next.session))
                    {
                        throw Error.RuntimeError(0xc9, "RowAction");
                    }
                }
                else
                {
                    if (next.type == 2)
                    {
                        if (session == next.session)
                        {
                            goto Label_007F;
                        }
                        goto Label_00A0;
                    }
                    if (((next.type == 5) && (session != next.session)) && ((next.CommitTimestamp == 0) && ((colMap == null) || ArrayUtil.HaveCommonElement(colMap, next.ChangeColumnMap))))
                    {
                        goto Label_00B8;
                    }
                }
            Label_007F:
                if (next.Next == null)
                {
                    goto Label_00D0;
                }
                next = next.Next;
                goto Label_0024;
            Label_00A0:
                session.TempSet.Add(next.session);
                return null;
            Label_00B8:
                session.TempSet.Add(next.session);
                return null;
            Label_00D0:
                base1 = new RowActionBase(session, 2);
                base1.ChangeColumnMap = colMap;
                RowActionBase base3 = base1;
                next.Next = base3;
            Label_00E6:
                action = this;
            }
            return action;
        }

        public static RowAction AddDeleteAction(Session session, TableBase table, Row row, int[] colMap)
        {
            RowAction rowAction = row.rowAction;
            if (rowAction == null)
            {
                rowAction = new RowAction(session, table, 2, row, colMap);
                row.rowAction = rowAction;
                return rowAction;
            }
            return rowAction.AddDeleteAction(session, colMap);
        }

        public static RowAction AddInsertAction(Session session, TableBase table, Row row)
        {
            RowAction action = new RowAction(session, table, 1, row, null);
            row.rowAction = action;
            return action;
        }

        public bool CanCommit(Session session, OrderedHashSet<Session> set)
        {
            bool flag;
            lock (this)
            {
                long transactionTimestamp = session.TransactionTimestamp;
                long commitTimestamp = -1L;
                bool flag2 = false;
                RowActionBase next = this;
                if (session.IsolationLevel == 0x1000)
                {
                    do
                    {
                        if (((next.session == session) && (next.type == 2)) && (next.CommitTimestamp == 0))
                        {
                            transactionTimestamp = next.ActionTimestamp;
                        }
                        next = next.Next;
                    }
                    while (next != null);
                    next = this;
                }
            Label_0059:
                if (next.session == session)
                {
                    if (next.type == 2)
                    {
                        flag2 = true;
                    }
                    goto Label_00CB;
                }
                if (next.Rolledback || (next.type != 2))
                {
                    next = next.Next;
                }
                else
                {
                    if (next.Prepared)
                    {
                        return false;
                    }
                    if (next.CommitTimestamp == 0)
                    {
                        set.Add(next.session);
                    }
                    else if (next.CommitTimestamp > commitTimestamp)
                    {
                        commitTimestamp = next.CommitTimestamp;
                    }
                    goto Label_00CB;
                }
            Label_00C5:
                if (next != null)
                {
                    goto Label_0059;
                }
                goto Label_00DD;
            Label_00CB:
                next = next.Next;
                goto Label_00C5;
            Label_00DD:
                flag = !flag2 || (commitTimestamp < transactionTimestamp);
            }
            return flag;
        }

        public bool CanRead(Session session, int mode)
        {
            bool flag;
            lock (this)
            {
                long transactionTimestamp;
                if (base.type == 0)
                {
                    return true;
                }
                if (base.type == 3)
                {
                    return false;
                }
                int type = 0;
                RowActionBase next = this;
                if (session == null)
                {
                    transactionTimestamp = 0x7fffffffffffffffL;
                }
                else
                {
                    int isolationLevel = session.IsolationLevel;
                    if (isolationLevel <= 0x1000)
                    {
                        if (isolationLevel == 0x1000)
                        {
                            transactionTimestamp = session.ActionTimestamp;
                        }
                    }
                    else if (isolationLevel == 0x10000)
                    {
                    }
                    transactionTimestamp = session.TransactionTimestamp;
                }
                do
                {
                    if (next.type == 5)
                    {
                        next = next.Next;
                    }
                    else if (next.Rolledback)
                    {
                        if (next.type == 1)
                        {
                            type = 2;
                        }
                        next = next.Next;
                    }
                    else if (session == next.session)
                    {
                        if (next.type == 2)
                        {
                            type = next.type;
                        }
                        else if (next.type == 1)
                        {
                            type = next.type;
                        }
                        next = next.Next;
                    }
                    else if (next.CommitTimestamp == 0)
                    {
                        if (next.type == 0)
                        {
                            throw Error.RuntimeError(0xc9, "RowAction");
                        }
                        if (next.type == 1)
                        {
                            goto Label_0164;
                        }
                        if (((next.type == 2) && (mode != 1)) && (mode == 2))
                        {
                            type = 2;
                        }
                        next = next.Next;
                    }
                    else
                    {
                        if (next.CommitTimestamp < transactionTimestamp)
                        {
                            if (next.type == 2)
                            {
                                type = next.type;
                            }
                            else if (next.type == 1)
                            {
                                type = next.type;
                            }
                        }
                        else if (next.type == 1)
                        {
                            type = 2;
                        }
                        next = next.Next;
                    }
                }
                while (next != null);
                goto Label_0196;
            Label_0164:
                if (mode == 0)
                {
                    type = 2;
                }
                else if (mode == 1)
                {
                    type = (next.ChangeColumnMap == null) ? 1 : 2;
                }
                else if (mode == 2)
                {
                    type = 2;
                }
            Label_0196:
                flag = (type == 0) || (type == 1);
            }
            return flag;
        }

        public static bool CheckDeleteActions()
        {
            return false;
        }

        public int Commit(Session session)
        {
            lock (this)
            {
                RowActionBase next = this;
                int type = 0;
                do
                {
                    if ((next.session == session) && (next.CommitTimestamp == 0))
                    {
                        next.CommitTimestamp = session.ActionTimestamp;
                        next.Prepared = false;
                        if (next.type == 1)
                        {
                            type = next.type;
                        }
                        else if (next.type == 2)
                        {
                            type = (type == 1) ? 4 : next.type;
                        }
                    }
                    next = next.Next;
                }
                while (next != null);
                return type;
            }
        }

        public void Complete(Session session)
        {
            lock (this)
            {
                RowActionBase next = this;
                do
                {
                    if ((next.session == session) && (next.ActionTimestamp == 0))
                    {
                        next.ActionTimestamp = session.ActionTimestamp;
                    }
                    next = next.Next;
                }
                while (next != null);
            }
        }

        public RowAction Duplicate(Row newRow)
        {
            lock (this)
            {
                return new RowAction(base.session, this.table, base.type, newRow, base.ChangeColumnMap);
            }
        }

        public int GetActionType()
        {
            lock (this)
            {
                return base.type;
            }
        }

        public int GetCommitTypeOn(long timestamp)
        {
            lock (this)
            {
                RowActionBase next = this;
                int type = 0;
                do
                {
                    if (next.CommitTimestamp == timestamp)
                    {
                        if (next.type == 1)
                        {
                            type = next.type;
                        }
                        else if (next.type == 2)
                        {
                            type = (type == 1) ? 4 : next.type;
                        }
                    }
                    next = next.Next;
                }
                while (next != null);
                return type;
            }
        }

        public int GetPos()
        {
            lock (this)
            {
                return this.RowId;
            }
        }

        private int GetRollbackType(Session session)
        {
            int type = 0;
            RowActionBase next = this;
            do
            {
                if ((next.session == session) && next.Rolledback)
                {
                    if (next.type == 2)
                    {
                        type = (type == 1) ? 4 : next.type;
                    }
                    else if (next.type == 1)
                    {
                        type = next.type;
                    }
                }
                next = next.Next;
            }
            while (next != null);
            return type;
        }

        public bool HasCurrentRefAction()
        {
            RowActionBase next = this;
            while ((next.type != 5) || (next.CommitTimestamp != 0))
            {
                next = next.Next;
                if (next == null)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsDeleted()
        {
            lock (this)
            {
                RowActionBase next = this;
                while ((next.CommitTimestamp == 0) || ((next.type != 2) && (next.type != 3)))
                {
                    next = next.Next;
                    if (next == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private RowAction MergeExpiredRefActions()
        {
            if (this.UpdatedAction != null)
            {
                this.UpdatedAction = this.UpdatedAction.MergeExpiredRefActions();
            }
            if (this.HasCurrentRefAction())
            {
                return this;
            }
            return this.UpdatedAction;
        }

        public int MergeRollback(Session session, long timestamp, Row row)
        {
            lock (this)
            {
                RowActionBase next = this;
                RowActionBase action = null;
                RowActionBase base4 = null;
                int rollbackType = this.GetRollbackType(session);
                do
                {
                    if ((next.session == session) && next.Rolledback)
                    {
                        if (base4 != null)
                        {
                            base4.Next = null;
                        }
                    }
                    else if (action == null)
                    {
                        action = base4 = next;
                    }
                    else
                    {
                        base4.Next = next;
                        base4 = next;
                    }
                    next = next.Next;
                }
                while (next != null);
                if (action == null)
                {
                    int num3 = rollbackType;
                    if ((num3 != 1) && (num3 != 4))
                    {
                        this.SetAsNoOp();
                    }
                    else
                    {
                        this.SetAsDeleteFinal(timestamp);
                    }
                }
                else if (action != this)
                {
                    base.SetAsAction(action);
                }
                return rollbackType;
            }
        }

        public void MergeToTimestamp(long timestamp)
        {
            lock (this)
            {
                RowActionBase next = this;
                RowActionBase action = null;
                RowActionBase base4 = null;
                int commitTypeOn = this.GetCommitTypeOn(timestamp);
                if ((base.type == 3) || (base.type == 0))
                {
                    goto Label_00C7;
                }
                switch (commitTypeOn)
                {
                    case 2:
                    case 4:
                        this.SetAsDeleteFinal(timestamp);
                        goto Label_00C7;

                    default:
                        do
                        {
                            bool flag = false;
                            if (next.CommitTimestamp != 0)
                            {
                                if (next.CommitTimestamp <= timestamp)
                                {
                                    flag = true;
                                }
                                else if (next.type == 5)
                                {
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                if (base4 != null)
                                {
                                    base4.Next = null;
                                }
                            }
                            else if (action == null)
                            {
                                action = base4 = next;
                            }
                            else
                            {
                                base4.Next = next;
                                base4 = next;
                            }
                            next = next.Next;
                        }
                        while (next != null);
                        if (action == null)
                        {
                            switch (commitTypeOn)
                            {
                                case 2:
                                case 4:
                                    this.SetAsDeleteFinal(timestamp);
                                    goto Label_00B7;
                            }
                            this.SetAsNoOp();
                        }
                        else if (action != this)
                        {
                            base.SetAsAction(action);
                        }
                        break;
                }
            Label_00B7:
                this.MergeExpiredRefActions();
            Label_00C7:;
            }
        }

        public void PrepareCommit(Session session)
        {
            lock (this)
            {
                RowActionBase next = this;
                do
                {
                    if ((next.session == session) && (next.CommitTimestamp == 0))
                    {
                        next.Prepared = true;
                    }
                    next = next.Next;
                }
                while (next != null);
            }
        }

        public void Rollback(Session session, long timestamp)
        {
            lock (this)
            {
                RowActionBase next = this;
                do
                {
                    if (((next.session == session) && (next.CommitTimestamp == 0)) && (next.ActionTimestamp >= timestamp))
                    {
                        next.CommitTimestamp = session.ActionTimestamp;
                        next.Rolledback = true;
                        next.Prepared = false;
                    }
                    next = next.Next;
                }
                while (next != null);
            }
        }

        public override void SetAsAction(RowActionBase action)
        {
            lock (this)
            {
                base.SetAsAction(action);
            }
        }

        public void SetAsAction(Session session, byte typ)
        {
            lock (this)
            {
                base.session = session;
                base.type = typ;
                base.ActionTimestamp = session.ActionTimestamp;
                base.ChangeColumnMap = null;
            }
        }

        private void SetAsDeleteFinal(long timestamp)
        {
            base.ActionTimestamp = 0L;
            base.CommitTimestamp = timestamp;
            base.Rolledback = false;
            base.DeleteComplete = false;
            base.Prepared = false;
            base.ChangeColumnMap = null;
            base.type = 3;
            base.Next = null;
        }

        public void SetAsNoOp()
        {
            base.session = null;
            base.ActionTimestamp = 0L;
            base.CommitTimestamp = 0L;
            base.Rolledback = false;
            base.DeleteComplete = false;
            base.ChangeColumnMap = null;
            base.Prepared = false;
            base.type = 0;
            base.Next = null;
        }

        public void SetPos(int pos)
        {
            lock (this)
            {
                this.RowId = pos;
            }
        }
    }
}

