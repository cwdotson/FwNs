namespace FwNs.Core.LC.cRows
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cTables;
    using System;

    public class Row : ICachedObject
    {
        public int Position;
        public object[] RowData;
        public RowAction rowAction;
        protected TableBase table;

        public Row(TableBase table, object[] data)
        {
            this.table = table;
            this.RowData = data;
        }

        public virtual void Delete(IPersistentStore store)
        {
        }

        public virtual void Destroy()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            Row row = obj as Row;
            return ((row != null) && (row.Position == this.Position));
        }

        public virtual int GetAccessCount()
        {
            return 0;
        }

        public RowAction GetAction()
        {
            return this.rowAction;
        }

        public override int GetHashCode()
        {
            return this.Position;
        }

        public long GetId()
        {
            return ((this.table.GetId() << 0x20) + this.Position);
        }

        public virtual int GetPos()
        {
            return this.Position;
        }

        public virtual int GetRealSize(IRowOutputInterface o)
        {
            return 0;
        }

        public virtual int GetStorageSize()
        {
            return 0;
        }

        public TableBase GetTable()
        {
            return this.table;
        }

        public virtual bool HasChanged()
        {
            return false;
        }

        public virtual bool IsDeleted(Session session, IPersistentStore store)
        {
            RowAction rowAction = ((Row) store.Get(this, false)).rowAction;
            return ((rowAction != null) && !rowAction.CanRead(session, 0));
        }

        public virtual bool IsInMemory()
        {
            return true;
        }

        public virtual bool IsKeepInMemory()
        {
            return true;
        }

        public virtual bool IsMemory()
        {
            return true;
        }

        public virtual bool KeepInMemory(bool keep)
        {
            return true;
        }

        public virtual void Restore()
        {
        }

        public virtual void SetChanged()
        {
        }

        public virtual void SetInMemory(bool i)
        {
        }

        public virtual void SetPos(int pos)
        {
            this.Position = pos;
        }

        public virtual void SetStorageSize(int size)
        {
        }

        public virtual void UpdateAccessCount(int count)
        {
        }

        public virtual void Write(IRowOutputInterface o)
        {
        }

        public virtual void Write(IRowOutputInterface o, IntLookup lookup)
        {
        }
    }
}

