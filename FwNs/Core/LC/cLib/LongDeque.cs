namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;

    public class LongDeque
    {
        private const int DefaultInitialCapacity = 10;
        protected int ElementCount;
        private int _endindex;
        private int _firstindex;
        private long[] _list = new long[10];

        public bool Add(long value)
        {
            this.ResetCapacity();
            if (this._endindex == this._list.Length)
            {
                this._endindex = 0;
            }
            this._list[this._endindex] = value;
            this.ElementCount++;
            this._endindex++;
            return true;
        }

        public bool AddLast(long value)
        {
            return this.Add(value);
        }

        public void Clear()
        {
            if (this.ElementCount != 0)
            {
                this._firstindex = this._endindex = this.ElementCount = 0;
                for (int i = 0; i < this._list.Length; i++)
                {
                    this._list[i] = 0L;
                }
            }
        }

        public bool Contains(long value)
        {
            for (int i = 0; i < this.ElementCount; i++)
            {
                int index = this._firstindex + i;
                if (index >= this._list.Length)
                {
                    index -= this._list.Length;
                }
                if (this._list[index] == value)
                {
                    return true;
                }
            }
            return false;
        }

        public long Get(int i)
        {
            int internalIndex = this.GetInternalIndex(i);
            return this._list[internalIndex];
        }

        public long GetFirst()
        {
            if (this.ElementCount == 0)
            {
                throw new KeyNotFoundException();
            }
            return this._list[this._firstindex];
        }

        private int GetInternalIndex(int i)
        {
            if ((i < 0) || (i >= this.ElementCount))
            {
                throw new IndexOutOfRangeException();
            }
            int num = this._firstindex + i;
            if (num >= this._list.Length)
            {
                num -= this._list.Length;
            }
            return num;
        }

        public int IndexOf(long value)
        {
            for (int i = 0; i < this.ElementCount; i++)
            {
                int index = this._firstindex + i;
                if (index >= this._list.Length)
                {
                    index -= this._list.Length;
                }
                if (this._list[index] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsEmpty()
        {
            return (this.ElementCount == 0);
        }

        public long Remove(int index)
        {
            int internalIndex = this.GetInternalIndex(index);
            if (internalIndex == this._firstindex)
            {
                this._list[this._firstindex] = 0L;
                this._firstindex++;
                if (this._firstindex == this._list.Length)
                {
                    this._firstindex = 0;
                }
            }
            else if (internalIndex > this._firstindex)
            {
                Array.Copy(this._list, this._firstindex, this._list, this._firstindex + 1, internalIndex - this._firstindex);
                this._list[this._firstindex] = 0L;
                this._firstindex++;
                if (this._firstindex == this._list.Length)
                {
                    this._firstindex = 0;
                }
            }
            else
            {
                Array.Copy(this._list, internalIndex + 1, this._list, internalIndex, (this._endindex - internalIndex) - 1);
                this._endindex--;
                this._list[this._endindex] = 0L;
                if (this._endindex == 0)
                {
                    this._endindex = this._list.Length;
                }
            }
            this.ElementCount--;
            if (this.ElementCount == 0)
            {
                this._firstindex = this._endindex = 0;
            }
            return this._list[internalIndex];
        }

        public long RemoveFirst()
        {
            // This item is obfuscated and can not be translated.
        }

        public long RemoveLast()
        {
            // This item is obfuscated and can not be translated.
        }

        private void ResetCapacity()
        {
            if (this.ElementCount >= this._list.Length)
            {
                long[] destinationArray = new long[this._list.Length * 2];
                Array.Copy(this._list, this._firstindex, destinationArray, this._firstindex, this._list.Length - this._firstindex);
                if (this._endindex <= this._firstindex)
                {
                    Array.Copy(this._list, 0, destinationArray, this._list.Length, this._endindex);
                    this._endindex = this._list.Length + this._endindex;
                }
                this._list = destinationArray;
            }
        }

        public int Size()
        {
            return this.ElementCount;
        }

        public void ToArray(int[] array)
        {
            for (int i = 0; i < this.ElementCount; i++)
            {
                array[i] = (int) this.Get(i);
            }
        }
    }
}

