namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cStore;
    using System;

    public class OrderedLongKeyHashMap<TValue, TValue2, TValue3> : BaseHashMap<long, TValue, TValue2, TValue3>
    {
        public OrderedLongKeyHashMap(int initialCapacity, bool hasThirdValue) : base(initialCapacity, 2, 3, false)
        {
            base.ObjectKeyTable = new long[base.ObjectValueTable.Length];
            base.IsTwoObjectValue = true;
            base.ObjectValueTable2 = new TValue2[base.ObjectValueTable.Length];
            base.HashTable = new int[base.ObjectValueTable.Length];
            base.IsList = true;
            if (hasThirdValue)
            {
                base.ObjectValueTable3 = new TValue3[base.ObjectValueTable.Length];
            }
        }

        public TValue GetFirstByLookup(int lookup)
        {
            if (lookup == -1)
            {
                return default(TValue);
            }
            return base.ObjectValueTable[lookup];
        }

        public TValue2 GetSecondValueByIndex(int index)
        {
            return base.ObjectValueTable2[index];
        }

        public TValue3 GetThirdValueByIndex(int index)
        {
            return base.ObjectValueTable3[index];
        }

        public TValue GetValueByIndex(int index)
        {
            return base.ObjectValueTable[index];
        }

        public TValue Put(long key, TValue valueOne, TValue2 valueTwo)
        {
            return base.AddOrRemove(key, (int) key, valueOne, valueTwo, false);
        }

        public TValue3 SetThirdValueByIndex(int index, TValue3 value)
        {
            base.ObjectValueTable3[index] = value;
            return base.ObjectValueTable3[index];
        }
    }
}

