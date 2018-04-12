namespace FwNs.Core.LC.cNavigators
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cRows;
    using System;

    public class RowSetNavigatorClient : RowSetNavigator
    {
        public static object[][] EmptyTable = new object[0][];
        private readonly int _baseBlockSize;
        private int _currentOffset;
        private object[][] _table;

        public RowSetNavigatorClient()
        {
            this._table = EmptyTable;
        }

        public RowSetNavigatorClient(int blockSize)
        {
            this._table = new object[blockSize][];
        }

        public RowSetNavigatorClient(RowSetNavigator source, int offset, int blockSize)
        {
            base.Size = source.Size;
            this._baseBlockSize = blockSize;
            this._currentOffset = offset;
            this._table = new object[blockSize][];
            source.Absolute(offset);
            for (int i = 0; i < blockSize; i++)
            {
                this._table[i] = source.GetCurrent();
                source.Next();
            }
            source.BeforeFirst();
        }

        public override bool Absolute(int position)
        {
            if (position < 0)
            {
                position += base.Size;
            }
            if (position < 0)
            {
                base.BeforeFirst();
                return false;
            }
            if (position >= base.Size)
            {
                base.AfterLast();
                return false;
            }
            if (base.Size == 0)
            {
                return false;
            }
            base.CurrentPos = position;
            return true;
        }

        public override void Add(object[] data)
        {
            this.EnsureCapacity();
            this._table[base.Size] = data;
            base.Size++;
        }

        public override bool AddRow(Row row)
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigatorClient");
        }

        public override void Clear()
        {
            this.SetData(EmptyTable);
            this.Reset();
        }

        public override void Close()
        {
            if ((base.session != null) && ((this._currentOffset != 0) || (this._table.Length != base.Size)))
            {
                base.session.CloseNavigator(base.Id);
            }
        }

        private void EnsureCapacity()
        {
            if (base.Size == this._table.Length)
            {
                object[][] destinationArray = new object[(base.Size == 0) ? 4 : (base.Size * 2)][];
                Array.Copy(this._table, 0, destinationArray, 0, base.Size);
                this._table = destinationArray;
            }
        }

        private void GetBlock(int offset)
        {
            try
            {
                RowSetNavigatorClient client = base.session.GetRows(base.Id, offset, this._baseBlockSize);
                this._table = client._table;
                this._currentOffset = client._currentOffset;
            }
            catch (CoreException)
            {
            }
        }

        public override object[] GetCurrent()
        {
            if ((base.CurrentPos < 0) || (base.CurrentPos >= base.Size))
            {
                return null;
            }
            if (base.CurrentPos == (this._currentOffset + this._table.Length))
            {
                this.GetBlock(this._currentOffset + this._table.Length);
            }
            return this._table[base.CurrentPos - this._currentOffset];
        }

        public override Row GetCurrentRow()
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigatorClient");
        }

        public object[] GetData(int index)
        {
            return this._table[index];
        }

        public override void Remove()
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigatorClient");
        }

        public void SetData(object[][] table)
        {
            this._table = table;
            base.Size = table.Length;
        }

        public void SetData(int index, object[] data)
        {
            this._table[index] = data;
        }
    }
}

