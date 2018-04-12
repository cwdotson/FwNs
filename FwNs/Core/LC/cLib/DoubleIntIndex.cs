namespace FwNs.Core.LC.cLib
{
    using System;

    public class DoubleIntIndex : IntLookup
    {
        private readonly bool _fixedSize;
        private int _capacity;
        private int _count;
        private int[] _keys;
        private bool _sortOnValues = true;
        private bool _sorted = true;
        private int[] _values;
        private int _targetSearchValue;

        public DoubleIntIndex(int capacity, bool fixedSize)
        {
            this._capacity = (capacity == 0) ? 0x20 : capacity;
            this._keys = new int[this._capacity];
            this._values = new int[this._capacity];
            this._fixedSize = fixedSize;
        }

        public bool Add(int key, int value)
        {
            lock (this)
            {
                if (this._count == this._capacity)
                {
                    if (this._fixedSize)
                    {
                        return false;
                    }
                    this.DoubleCapacity();
                }
                if (!this._sorted)
                {
                    this.FastQuickSort();
                }
                this._targetSearchValue = this._sortOnValues ? value : key;
                int fromIndex = this.BinarySlotSearch();
                if (fromIndex == -1)
                {
                    return false;
                }
                if (this._count != fromIndex)
                {
                    this.MoveRows(fromIndex, fromIndex + 1, this._count - fromIndex);
                }
                this._keys[fromIndex] = key;
                this._values[fromIndex] = value;
                this._count++;
                return true;
            }
        }

        public bool AddUnique(int key, int value)
        {
            lock (this)
            {
                if (this._count == this._capacity)
                {
                    if (this._fixedSize)
                    {
                        return false;
                    }
                    this.DoubleCapacity();
                }
                if (!this._sorted)
                {
                    this.FastQuickSort();
                }
                this._targetSearchValue = this._sortOnValues ? value : key;
                int fromIndex = this.BinaryEmptySlotSearch();
                if (fromIndex == -1)
                {
                    return false;
                }
                if (this._count != fromIndex)
                {
                    this.MoveRows(fromIndex, fromIndex + 1, this._count - fromIndex);
                }
                this._keys[fromIndex] = key;
                this._values[fromIndex] = value;
                this._count++;
                return true;
            }
        }

        public bool AddUnsorted(int key, int value)
        {
            lock (this)
            {
                if (this._count == this._capacity)
                {
                    if (this._fixedSize)
                    {
                        return false;
                    }
                    this.DoubleCapacity();
                }
                if (this._sorted && (this._count != 0))
                {
                    if (this._sortOnValues)
                    {
                        if (value < this._values[this._count - 1])
                        {
                            this._sorted = false;
                        }
                    }
                    else if (value < this._keys[this._count - 1])
                    {
                        this._sorted = false;
                    }
                }
                this._keys[this._count] = key;
                this._values[this._count] = value;
                this._count++;
                return true;
            }
        }

        private int BinaryEmptySlotSearch()
        {
            int num = 0;
            int num2 = this._count;
            while (num < num2)
            {
                int i = (num + num2) / 2;
                int num4 = this.Compare(i);
                if (num4 < 0)
                {
                    num2 = i;
                }
                else
                {
                    if (num4 <= 0)
                    {
                        return -1;
                    }
                    num = i + 1;
                }
            }
            return num;
        }

        private int BinaryFirstSearch()
        {
            int num = 0;
            int num2 = this._count;
            int num3 = this._count;
            while (num < num2)
            {
                int i = (num + num2) / 2;
                int num6 = this.Compare(i);
                if (num6 < 0)
                {
                    num2 = i;
                }
                else
                {
                    if (num6 > 0)
                    {
                        num = i + 1;
                        continue;
                    }
                    num2 = i;
                    num3 = i;
                }
            }
            if (num3 != this._count)
            {
                return num3;
            }
            return -1;
        }

        private int BinarySlotSearch()
        {
            int num = 0;
            int num2 = this._count;
            while (num < num2)
            {
                int i = (num + num2) / 2;
                if (this.Compare(i) <= 0)
                {
                    num2 = i;
                }
                else
                {
                    num = i + 1;
                }
            }
            return num;
        }

        public int Capacity()
        {
            lock (this)
            {
                return this._capacity;
            }
        }

        private int Compare(int i)
        {
            if (this._sortOnValues)
            {
                if (this._targetSearchValue > this._values[i])
                {
                    return 1;
                }
                if (this._targetSearchValue < this._values[i])
                {
                    return -1;
                }
            }
            else
            {
                if (this._targetSearchValue > this._keys[i])
                {
                    return 1;
                }
                if (this._targetSearchValue < this._keys[i])
                {
                    return -1;
                }
            }
            return 0;
        }

        private void DoubleCapacity()
        {
            Array.Resize<int>(ref this._keys, this._capacity * 2);
            Array.Resize<int>(ref this._values, this._capacity * 2);
            this._capacity *= 2;
        }

        private void FastQuickSort()
        {
            lock (this)
            {
                this.QuickSort(0, this._count - 1);
                this.InsertionSort(0, this._count - 1);
                this._sorted = true;
            }
        }

        public int FindFirstEqualKeyIndex(int value)
        {
            lock (this)
            {
                if (!this._sorted)
                {
                    this.FastQuickSort();
                }
                this._targetSearchValue = value;
                return this.BinaryFirstSearch();
            }
        }

        public int FindFirstGreaterEqualKeyIndex(int value)
        {
            lock (this)
            {
                int num2 = this.FindFirstGreaterEqualSlotIndex(value);
                return ((num2 == this._count) ? -1 : num2);
            }
        }

        public int FindFirstGreaterEqualSlotIndex(int value)
        {
            lock (this)
            {
                if (!this._sorted)
                {
                    this.FastQuickSort();
                }
                this._targetSearchValue = value;
                return this.BinarySlotSearch();
            }
        }

        public int GetKey(int i)
        {
            lock (this)
            {
                if ((i < 0) || (i >= this._count))
                {
                    throw new ArgumentOutOfRangeException();
                }
                return this._keys[i];
            }
        }

        public int GetValue(int i)
        {
            lock (this)
            {
                if ((i < 0) || (i >= this._count))
                {
                    throw new ArgumentOutOfRangeException();
                }
                return this._values[i];
            }
        }

        private void InsertionSort(int lo0, int hi0)
        {
            for (int i = lo0 + 1; i <= hi0; i++)
            {
                int j = i;
                while ((j > lo0) && this.LessThan(i, j - 1))
                {
                    j--;
                }
                if (i != j)
                {
                    this.MoveAndInsertRow(i, j);
                }
            }
        }

        private bool LessThan(int i, int j)
        {
            if (this._sortOnValues)
            {
                if (this._values[i] < this._values[j])
                {
                    return true;
                }
            }
            else if (this._keys[i] < this._keys[j])
            {
                return true;
            }
            return false;
        }

        public int LookupFirstEqual(int key)
        {
            if (this._sortOnValues)
            {
                this._sorted = false;
                this._sortOnValues = false;
            }
            int i = this.FindFirstEqualKeyIndex(key);
            if (i == -1)
            {
                throw new InvalidOperationException();
            }
            return this.GetValue(i);
        }

        public int LookupFirstGreaterEqual(int key)
        {
            if (this._sortOnValues)
            {
                this._sorted = false;
                this._sortOnValues = false;
            }
            int i = this.FindFirstGreaterEqualKeyIndex(key);
            if (i == -1)
            {
                throw new InvalidOperationException();
            }
            return this.GetValue(i);
        }

        private void MoveAndInsertRow(int i, int j)
        {
            int num = this._keys[i];
            int num2 = this._values[i];
            this.MoveRows(j, j + 1, i - j);
            this._keys[j] = num;
            this._values[j] = num2;
        }

        private void MoveRows(int fromIndex, int toIndex, int rows)
        {
            Array.Copy(this._keys, fromIndex, this._keys, toIndex, rows);
            Array.Copy(this._values, fromIndex, this._values, toIndex, rows);
        }

        private void QuickSort(int l, int r)
        {
            if ((r - l) <= 0x10)
            {
                return;
            }
            int i = (r + l) / 2;
            if (this.LessThan(i, l))
            {
                this.Swap(l, i);
            }
            if (this.LessThan(r, l))
            {
                this.Swap(l, r);
            }
            if (this.LessThan(r, i))
            {
                this.Swap(i, r);
            }
            int num2 = r - 1;
            this.Swap(i, num2);
            i = l;
            int j = num2;
        Label_0056:
            while (this.LessThan(++i, j))
            {
            }
            while (this.LessThan(j, --num2))
            {
            }
            if (num2 >= i)
            {
                this.Swap(i, num2);
                goto Label_0056;
            }
            this.Swap(i, r - 1);
            this.QuickSort(l, num2);
            this.QuickSort(i + 1, r);
        }

        public void Remove(int position)
        {
            lock (this)
            {
                this.MoveRows(position + 1, position, (this._count - position) - 1);
                this._count--;
                this._keys[this._count] = 0;
                this._values[this._count] = 0;
            }
        }

        public void RemoveRange(int start, int limit)
        {
            this.MoveRows(limit, start, this._count - limit);
            this._count -= limit - start;
        }

        public void SetKeysSearchTarget()
        {
            lock (this)
            {
                if (this._sortOnValues)
                {
                    this._sorted = false;
                }
                this._sortOnValues = false;
            }
        }

        public void SetValue(int i, int value)
        {
            lock (this)
            {
                if ((i < 0) || (i >= this._count))
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (this._sortOnValues)
                {
                    this._sorted = false;
                }
                this._values[i] = value;
            }
        }

        public void SetValuesSearchTarget()
        {
            lock (this)
            {
                if (!this._sortOnValues)
                {
                    this._sorted = false;
                }
                this._sortOnValues = true;
            }
        }

        public int Size()
        {
            lock (this)
            {
                return this._count;
            }
        }

        private void Swap(int i1, int i2)
        {
            int num = this._keys[i1];
            int num2 = this._values[i1];
            this._keys[i1] = this._keys[i2];
            this._values[i1] = this._values[i2];
            this._keys[i2] = num;
            this._values[i2] = num2;
        }
    }
}

