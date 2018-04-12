namespace FwNs.Core.LC.cRows
{
    using FwNs.Core.LC.cEngine;
    using System;

    public class RowActionBase
    {
        public const byte ActionNone = 0;
        public const byte ActionInsert = 1;
        public const byte ActionDelete = 2;
        public const byte ActionDeleteFinal = 3;
        public const byte ActionInsertDelete = 4;
        public const byte ActionRef = 5;
        public const byte ActionCheck = 6;
        public const byte ActionDebug = 7;
        public long ActionTimestamp;
        public int[] ChangeColumnMap;
        public long CommitTimestamp;
        public bool DeleteComplete;
        public RowActionBase Next;
        public bool Prepared;
        public bool Rolledback;
        public Session session;
        public byte type;

        public RowActionBase()
        {
        }

        public RowActionBase(Session session, byte type)
        {
            this.session = session;
            this.type = type;
            this.ActionTimestamp = session.ActionTimestamp;
        }

        public virtual void SetAsAction(RowActionBase action)
        {
            this.Next = action.Next;
            this.session = action.session;
            this.ActionTimestamp = action.ActionTimestamp;
            this.CommitTimestamp = action.CommitTimestamp;
            this.type = action.type;
            this.DeleteComplete = action.DeleteComplete;
            this.Rolledback = action.Rolledback;
            this.Prepared = action.Prepared;
            this.ChangeColumnMap = action.ChangeColumnMap;
        }
    }
}

