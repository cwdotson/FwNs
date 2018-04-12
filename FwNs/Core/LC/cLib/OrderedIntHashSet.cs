namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cStore;
    using System;

    public class OrderedIntHashSet : BaseHashMap<int, int, int, int>
    {
        public OrderedIntHashSet() : this(8)
        {
        }

        public OrderedIntHashSet(int initialCapacity) : base(initialCapacity, 1, 0, false)
        {
            base.IsList = true;
        }

        public bool Add(int key)
        {
            base.AddOrRemove(key, key, false);
            return (base.Size() != base.Size());
        }

        private void CheckRange(int i)
        {
            if ((i < 0) || (i >= base.Size()))
            {
                throw new IndexOutOfRangeException();
            }
        }

        public bool Contains(int key)
        {
            return base.ContainsKey(key);
        }

        public int Get(int index)
        {
            this.CheckRange(index);
            return base.ObjectKeyTable[index];
        }

        public int GetIndex(int value)
        {
            return base.GetLookup(value, value);
        }

        public int GetOrderedStartMatchCount(int[] array)
        {
            int index = 0;
            while (((index < array.Length) && (index < base.Size())) && (this.Get(index) == array[index]))
            {
                index++;
            }
            return index;
        }

        public int GetStartMatchCount(int[] array)
        {
            int index = 0;
            while ((index < array.Length) && base.ContainsKey(array[index]))
            {
                index++;
            }
            return index;
        }

        public int[] ToArray()
        {
            int lookup = -1;
            int[] numArray = new int[base.Size()];
            for (int i = 0; i < numArray.Length; i++)
            {
                lookup = base.NextLookup(lookup);
                numArray[i] = base.ObjectKeyTable[lookup];
            }
            return numArray;
        }
    }
}

