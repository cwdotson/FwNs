namespace FwNs.Core.LC.cLib
{
    using System;

    public class HashMappedList<TKey, TValue> : HashMap<TKey, TValue>
    {
        public HashMappedList() : this(8)
        {
        }

        public HashMappedList(int initialCapacity) : base(initialCapacity)
        {
            base.IsList = true;
        }

        public bool Add(TKey key, TValue value)
        {
            if (base.GetLookup(key, key.GetHashCode()) >= 0)
            {
                return false;
            }
            base.Put(key, value);
            return true;
        }

        private void CheckRange(int i)
        {
            if ((i < 0) || (i >= base.Size()))
            {
                throw new IndexOutOfRangeException();
            }
        }

        public TValue Get(int index)
        {
            return base.ObjectValueTable[index];
        }

        public int GetIndex(TKey key)
        {
            return base.GetLookup(key, key.GetHashCode());
        }

        public TKey GetKey(int index)
        {
            this.CheckRange(index);
            return base.ObjectKeyTable[index];
        }

        public override TValue Remove(TKey key)
        {
            int lookup = base.GetLookup(key, key.GetHashCode());
            if (lookup < 0)
            {
                return default(TValue);
            }
            base.RemoveRow(lookup);
            return base.Remove(key);
        }

        public TValue Remove(int index)
        {
            this.CheckRange(index);
            return this.Remove(base.ObjectKeyTable[index]);
        }

        public bool Set(int index, TKey key, TValue value)
        {
            this.CheckRange(index);
            if (base.GetKeySet().Contains(key) && (this.GetIndex(key) != index))
            {
                return false;
            }
            base.Remove(base.ObjectKeyTable[index]);
            base.Put(key, value);
            return true;
        }

        public bool SetKey(int index, TKey key)
        {
            this.CheckRange(index);
            TValue local = base.ObjectValueTable[index];
            return this.Set(index, key, local);
        }
    }
}

