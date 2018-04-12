namespace FwNs.Core.LC.cNavigators
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using System;

    public abstract class RowSetNavigator : IRangeIterator, IRowIterator
    {
        public int CurrentPos = -1;
        protected long Id;
        public bool IsIterator;
        public int Mode;
        protected int RangePosition;
        protected ISessionInterface session;
        public int Size;

        protected RowSetNavigator()
        {
        }

        public virtual bool Absolute(int position)
        {
            if (position < 0)
            {
                position += this.Size;
            }
            if (position < 0)
            {
                this.BeforeFirst();
                return false;
            }
            if (position >= this.Size)
            {
                this.AfterLast();
                return false;
            }
            if (this.Size == 0)
            {
                return false;
            }
            if (position < this.CurrentPos)
            {
                this.BeforeFirst();
            }
            while (position > this.CurrentPos)
            {
                this.Next();
            }
            return true;
        }

        public abstract void Add(object[] data);
        public abstract bool AddRow(Row row);
        public bool AfterLast()
        {
            if (this.Size == 0)
            {
                return false;
            }
            this.Reset();
            this.CurrentPos = this.Size;
            return true;
        }

        public bool BeforeFirst()
        {
            this.Reset();
            this.CurrentPos = -1;
            return true;
        }

        public abstract void Clear();
        public virtual void ClearStructures()
        {
        }

        public virtual void Close()
        {
        }

        public bool First()
        {
            this.BeforeFirst();
            return this.Next();
        }

        public abstract object[] GetCurrent();
        public abstract Row GetCurrentRow();
        public long GetId()
        {
            return this.Id;
        }

        public object[] GetNext()
        {
            if (!this.Next())
            {
                return null;
            }
            return this.GetCurrent();
        }

        public virtual Row GetNextRow()
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigator");
        }

        public RangeVariable GetRange()
        {
            return null;
        }

        public int GetRangePosition()
        {
            return this.RangePosition;
        }

        public long GetRowId()
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigator");
        }

        public object GetRowidObject()
        {
            return null;
        }

        public int GetRowNumber()
        {
            return this.CurrentPos;
        }

        public ISessionInterface GetSession()
        {
            return this.session;
        }

        public int GetSize()
        {
            return this.Size;
        }

        public bool HasNext()
        {
            return (this.CurrentPos < (this.Size - 1));
        }

        public bool IsAfterLast()
        {
            return ((this.Size > 0) && (this.CurrentPos == this.Size));
        }

        public bool IsBeforeFirst()
        {
            return ((this.Size > 0) && (this.CurrentPos == -1));
        }

        public bool IsEmpty()
        {
            return (this.Size == 0);
        }

        public bool IsFirst()
        {
            return ((this.Size > 0) && (this.CurrentPos == 0));
        }

        public bool IsLast()
        {
            return ((this.Size > 0) && (this.CurrentPos == (this.Size - 1)));
        }

        public virtual bool IsMemory()
        {
            return true;
        }

        public bool Last()
        {
            if (this.Size == 0)
            {
                return false;
            }
            if (this.IsAfterLast())
            {
                this.BeforeFirst();
            }
            while (this.HasNext())
            {
                this.Next();
            }
            return true;
        }

        public virtual bool Next()
        {
            if (this.HasNext())
            {
                this.CurrentPos++;
                return true;
            }
            if (this.Size != 0)
            {
                this.CurrentPos = this.Size;
            }
            return false;
        }

        public bool Previous()
        {
            return this.Relative(-1);
        }

        public virtual void ReadSimple(IRowInputInterface i, ResultMetaData meta)
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigator");
        }

        public bool Relative(int rows)
        {
            int position = this.CurrentPos + rows;
            if (position < 0)
            {
                this.BeforeFirst();
                return false;
            }
            return this.Absolute(position);
        }

        public void Release()
        {
            this.Reset();
        }

        public abstract void Remove();
        public virtual void Reset()
        {
            this.CurrentPos = -1;
        }

        public void SetCurrent(object[] data)
        {
        }

        public void SetId(long id)
        {
            this.Id = id;
        }

        public bool SetRowColumns(bool[] columns)
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigator");
        }

        public void SetSession(ISessionInterface session)
        {
            this.session = session;
        }

        public virtual void WriteSimple(IRowOutputInterface o, ResultMetaData meta)
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigator");
        }
    }
}

