namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cLib;
    using System;

    public sealed class DataFileBlockManager
    {
        private readonly int _capacity;
        private readonly DoubleIntIndex _lookup;
        private readonly int _scale;
        private bool _isModified;
        private long _lostFreeBlockSize;
        private int _midSize;
        private long _releaseCount;
        private long _requestCount;
        private long _requestSize;

        public DataFileBlockManager(int capacity, int scale, long lostSize)
        {
            this._lookup = new DoubleIntIndex(capacity, true);
            this._lookup.SetValuesSearchTarget();
            this._capacity = capacity;
            this._scale = scale;
            this._lostFreeBlockSize = lostSize;
            this._midSize = 0x80;
        }

        public void Add(int pos, int rowSize)
        {
            this._isModified = true;
            if (this._capacity == 0)
            {
                this._lostFreeBlockSize += rowSize;
            }
            else
            {
                this._releaseCount += 1L;
                if (this._lookup.Size() == this._capacity)
                {
                    this.ResetList();
                }
                this._lookup.Add(pos, rowSize);
            }
        }

        public int Get(int rowSize)
        {
            if (this._lookup.Size() == 0)
            {
                return -1;
            }
            int i = this._lookup.FindFirstGreaterEqualKeyIndex(rowSize);
            if (i == -1)
            {
                return -1;
            }
            this._requestCount += 1L;
            this._requestSize += rowSize;
            int num3 = this._lookup.GetValue(i) - rowSize;
            int key = this._lookup.GetKey(i);
            this._lookup.Remove(i);
            if (num3 >= this._midSize)
            {
                int num5 = key + (rowSize / this._scale);
                this._lookup.Add(num5, num3);
            }
            else
            {
                this._lostFreeBlockSize += num3;
            }
            return key;
        }

        public long GetLostBlocksSize()
        {
            return this._lostFreeBlockSize;
        }

        public bool IsModified()
        {
            return this._isModified;
        }

        private void RemoveBlocks(int blocks)
        {
            for (int i = 0; i < blocks; i++)
            {
                this._lostFreeBlockSize += this._lookup.GetValue(i);
            }
            this._lookup.RemoveRange(0, blocks);
        }

        private void ResetList()
        {
            if (this._requestCount != 0)
            {
                this._midSize = (int) (this._requestSize / this._requestCount);
            }
            int blocks = this._lookup.FindFirstGreaterEqualSlotIndex(this._midSize);
            if (blocks < (this._lookup.Size() / 4))
            {
                blocks = this._lookup.Size() / 4;
            }
            this.RemoveBlocks(blocks);
        }

        public int Size()
        {
            return this._lookup.Size();
        }
    }
}

