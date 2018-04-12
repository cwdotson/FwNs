namespace FwNs.Core.LC.cStore
{
    using FwNs.Core.LC.cLib;
    using System;
    using System.Collections.Generic;

    public class BaseHashMap<TKey, TValue, TValue2, TValue3>
    {
        protected const int NoKeyOrValue = 0;
        protected const int IntKeyOrValue = 1;
        protected const int LongKeyOrValue = 2;
        protected const int ObjectKeyOrValue = 3;
        public const int NoPurge = 0;
        public const int PurgeAll = 1;
        public const int PurgeHalf = 2;
        public const int PurgeQuarter = 3;
        public const int AccessMax = 0x7fefffff;
        protected bool IsTwoObjectValue;
        protected bool IsList;
        private ValuesIterator<TKey, TValue, TValue2, TValue3, TKey, TValue, TValue2, TValue3> _valuesIterator;
        private readonly HashIndex _hashIndex;
        protected TKey[] ObjectKeyTable;
        protected TValue[] ObjectValueTable;
        protected int AccessMin;
        protected int AccessCount;
        protected int[] AccessTable;
        protected bool[] MultiValueTable;
        protected TValue3[] ObjectValueTable3;
        protected TValue2[] ObjectValueTable2;
        protected int[] HashTable;
        protected float LoadFactor;
        protected int InitialCapacity;
        protected int Threshold;
        protected int MaxCapacity;
        protected int PurgePolicy;
        protected bool MinimizeOnEmpty;
        private bool _hasZeroKey;
        private int _zeroKeyIndex;
        private IEqualityComparer<TKey> keyComparer;
        private IEqualityComparer<TValue> valueComparer;

        protected BaseHashMap(int initialCapacity, int keyType, int valueType, bool hasAccessCount)
        {
            this._zeroKeyIndex = -1;
            if (initialCapacity <= 0)
            {
                throw new ArgumentException();
            }
            if (initialCapacity < 3)
            {
                initialCapacity = 3;
            }
            this.LoadFactor = 1f;
            this.InitialCapacity = initialCapacity;
            this.Threshold = initialCapacity;
            if (this.Threshold < 3)
            {
                this.Threshold = 3;
            }
            int hashTableSize = (int) (initialCapacity * this.LoadFactor);
            if (hashTableSize < 3)
            {
                hashTableSize = 3;
            }
            this._hashIndex = new HashIndex(hashTableSize, initialCapacity, true);
            int threshold = this.Threshold;
            this.ObjectKeyTable = new TKey[threshold];
            this.ObjectValueTable = new TValue[threshold];
            if (hasAccessCount)
            {
                this.AccessTable = new int[threshold];
            }
            this.keyComparer = EqualityComparer<TKey>.Default;
            this.valueComparer = EqualityComparer<TValue>.Default;
        }

        protected TValue AddOrRemove(TKey objectKey, TValue objectValue, bool remove)
        {
            if (objectKey != null)
            {
                int accessCount;
                int hashCode = objectKey.GetHashCode();
                int hashIndex = this._hashIndex.GetHashIndex(hashCode);
                int index = this._hashIndex.HashTable[hashIndex];
                int lastLookup = -1;
                TValue local2 = default(TValue);
                while ((index >= 0) && !this.keyComparer.Equals(this.ObjectKeyTable[index], objectKey))
                {
                    lastLookup = index;
                    index = this._hashIndex.GetNextLookup(index);
                }
                if (index >= 0)
                {
                    if (remove)
                    {
                        this.ObjectKeyTable[index] = default(TKey);
                        if (object.Equals(objectKey, default(TKey)))
                        {
                            this._hasZeroKey = false;
                            this._zeroKeyIndex = -1;
                        }
                        local2 = this.ObjectValueTable[index];
                        this.ObjectValueTable[index] = default(TValue);
                        this._hashIndex.UnlinkNode(hashIndex, lastLookup, index);
                        if (this.AccessTable != null)
                        {
                            this.AccessTable[index] = 0;
                        }
                        if (this.MinimizeOnEmpty && (this._hashIndex.ElementCount == 0))
                        {
                            this.Rehash(this.InitialCapacity);
                        }
                        return local2;
                    }
                    local2 = this.ObjectValueTable[index];
                    this.ObjectValueTable[index] = objectValue;
                    if (this.AccessTable != null)
                    {
                        accessCount = this.AccessCount + 1;
                        this.AccessCount = accessCount;
                        this.AccessTable[index] = accessCount;
                    }
                    return local2;
                }
                if (!remove)
                {
                    if (this._hashIndex.ElementCount < this.Threshold)
                    {
                        index = this._hashIndex.LinkNode(hashIndex, lastLookup);
                        this.ObjectKeyTable[index] = objectKey;
                        if (this.keyComparer.Equals(objectKey, default(TKey)))
                        {
                            this._hasZeroKey = true;
                            this._zeroKeyIndex = index;
                        }
                        this.ObjectValueTable[index] = objectValue;
                        if (this.AccessTable != null)
                        {
                            accessCount = this.AccessCount;
                            this.AccessCount = accessCount + 1;
                            this.AccessTable[index] = accessCount;
                        }
                        return local2;
                    }
                    if (this.Reset())
                    {
                        return this.AddOrRemove(objectKey, objectValue, remove);
                    }
                }
            }
            return default(TValue);
        }

        protected TValue AddOrRemove(TKey longKey, int hash, TValue objectValue, TValue2 objectValueTwo, bool remove)
        {
            TValue local;
            try
            {
                int num4;
                int hashIndex = this._hashIndex.GetHashIndex(hash);
                int index = this._hashIndex.HashTable[hashIndex];
                int lastLookup = -1;
                TValue local2 = default(TValue);
                while ((index >= 0) && !this.keyComparer.Equals(longKey, this.ObjectKeyTable[index]))
                {
                    lastLookup = index;
                    index = this._hashIndex.GetNextLookup(index);
                }
                if (index >= 0)
                {
                    if (remove)
                    {
                        TKey y = default(TKey);
                        if (this.keyComparer.Equals(longKey, y))
                        {
                            this._hasZeroKey = false;
                            this._zeroKeyIndex = -1;
                        }
                        this.ObjectKeyTable[index] = default(TKey);
                        local2 = this.ObjectValueTable[index];
                        this.ObjectValueTable[index] = default(TValue);
                        this._hashIndex.UnlinkNode(hashIndex, lastLookup, index);
                        if (this.IsTwoObjectValue)
                        {
                            this.ObjectValueTable2[index] = default(TValue2);
                            this.HashTable[index] = 0;
                        }
                        if (this.AccessTable != null)
                        {
                            this.AccessTable[index] = 0;
                        }
                        return local2;
                    }
                    local2 = this.ObjectValueTable[index];
                    this.ObjectValueTable[index] = objectValue;
                    if (this.IsTwoObjectValue)
                    {
                        this.ObjectValueTable2[index] = objectValueTwo;
                        this.HashTable[index] = hash;
                    }
                    if (this.AccessTable != null)
                    {
                        num4 = this.AccessCount + 1;
                        this.AccessCount = num4;
                        this.AccessTable[index] = num4;
                    }
                    return local2;
                }
                if (remove)
                {
                    return local2;
                }
                if (this._hashIndex.ElementCount >= this.Threshold)
                {
                    if (this.Reset())
                    {
                        return this.AddOrRemove(longKey, hash, objectValue, objectValueTwo, remove);
                    }
                    return default(TValue);
                }
                index = this._hashIndex.LinkNode(hashIndex, lastLookup);
                this.ObjectKeyTable[index] = longKey;
                if (this.keyComparer.Equals(longKey, default(TKey)))
                {
                    this._hasZeroKey = true;
                    this._zeroKeyIndex = index;
                }
                this.ObjectValueTable[index] = objectValue;
                if (this.IsTwoObjectValue)
                {
                    this.ObjectValueTable2[index] = objectValueTwo;
                    this.HashTable[index] = hash;
                }
                if (this.AccessTable != null)
                {
                    num4 = this.AccessCount + 1;
                    this.AccessCount = num4;
                    this.AccessTable[index] = num4;
                }
                local = local2;
            }
            catch (Exception exception1)
            {
                throw exception1;
            }
            return local;
        }

        protected object AddOrRemoveMultiVal(TKey objectKey, TValue objectValue, bool removeKey, bool removeValue)
        {
            if (objectKey != null)
            {
                int hashCode = objectKey.GetHashCode();
                int hashIndex = this._hashIndex.GetHashIndex(hashCode);
                int index = this._hashIndex.HashTable[hashIndex];
                int lastLookup = -1;
                TValue local = default(TValue);
                bool flag = false;
                while (index >= 0)
                {
                    if (this.keyComparer.Equals(this.ObjectKeyTable[index], objectKey))
                    {
                        if (removeKey)
                        {
                            do
                            {
                                this.ObjectKeyTable[index] = default(TKey);
                                local = this.ObjectValueTable[index];
                                this.ObjectValueTable[index] = default(TValue);
                                this._hashIndex.UnlinkNode(hashIndex, lastLookup, index);
                                this.MultiValueTable[index] = false;
                                index = this._hashIndex.HashTable[hashIndex];
                            }
                            while ((index >= 0) && this.keyComparer.Equals(this.ObjectKeyTable[index], objectKey));
                            return local;
                        }
                        if (this.valueComparer.Equals(this.ObjectValueTable[index], objectValue))
                        {
                            if (removeValue)
                            {
                                this.ObjectKeyTable[index] = default(TKey);
                                local = this.ObjectValueTable[index];
                                this.ObjectValueTable[index] = default(TValue);
                                this._hashIndex.UnlinkNode(hashIndex, lastLookup, index);
                                this.MultiValueTable[index] = false;
                                return local;
                            }
                            return this.ObjectValueTable[index];
                        }
                        flag = true;
                    }
                    lastLookup = index;
                    index = this._hashIndex.GetNextLookup(index);
                }
                if (removeKey | removeValue)
                {
                    return local;
                }
                if (this._hashIndex.ElementCount < this.Threshold)
                {
                    index = this._hashIndex.LinkNode(hashIndex, lastLookup);
                    this.ObjectKeyTable[index] = objectKey;
                    if (this.keyComparer.Equals(objectKey, default(TKey)))
                    {
                        this._hasZeroKey = true;
                        this._zeroKeyIndex = index;
                    }
                    this.ObjectValueTable[index] = objectValue;
                    if (flag)
                    {
                        this.MultiValueTable[index] = true;
                    }
                    if (this.AccessTable != null)
                    {
                        int accessCount = this.AccessCount;
                        this.AccessCount = accessCount + 1;
                        this.AccessTable[index] = accessCount;
                    }
                    return local;
                }
                if (this.Reset())
                {
                    return this.AddOrRemoveMultiVal(objectKey, objectValue, removeKey, removeValue);
                }
            }
            return null;
        }

        public virtual void Clear()
        {
            if (this._hashIndex.Modified)
            {
                this.AccessCount = 0;
                this.AccessMin = this.AccessCount;
                this._hasZeroKey = false;
                this._zeroKeyIndex = -1;
                this.ClearElementArrays(0, this._hashIndex.LinkTable.Length);
                this._hashIndex.Clear();
                if (this.MinimizeOnEmpty)
                {
                    this.Rehash(this.InitialCapacity);
                }
            }
        }

        protected void Clear(int count, int margin)
        {
            if (margin < 0x40)
            {
                margin = 0x40;
            }
            int newNodePointer = this._hashIndex.NewNodePointer;
            int accessCountCeiling = this.GetAccessCountCeiling(count, margin);
            for (int i = 0; i < newNodePointer; i++)
            {
                TKey objectKey = this.ObjectKeyTable[i];
                if ((objectKey != null) && (this.AccessTable[i] < accessCountCeiling))
                {
                    this.RemoveObject(objectKey, false);
                }
            }
            this.AccessMin = accessCountCeiling;
        }

        private void ClearElementArrays(int from, int to)
        {
            int index = to;
            while (--index >= from)
            {
                this.ObjectKeyTable[index] = default(TKey);
                this.ObjectValueTable[index] = default(TValue);
                if (this.AccessTable != null)
                {
                    this.AccessTable[index] = 0;
                }
                if (this.MultiValueTable != null)
                {
                    this.MultiValueTable[index] = false;
                }
            }
        }

        public virtual bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                return false;
            }
            if (this._hashIndex.ElementCount == 0)
            {
                return false;
            }
            return (this.GetLookup(key, key.GetHashCode()) != -1);
        }

        public virtual bool ContainsValue(TValue value)
        {
            int index = 0;
            if (this._hashIndex.ElementCount != 0)
            {
                if (value != null)
                {
                    while (index < this._hashIndex.NewNodePointer)
                    {
                        if (this.valueComparer.Equals(value, this.ObjectValueTable[index]))
                        {
                            return true;
                        }
                        index++;
                    }
                }
                else
                {
                    while (index < this._hashIndex.NewNodePointer)
                    {
                        if (this.ObjectValueTable[index] == null)
                        {
                            TKey y = default(TKey);
                            if (!this.keyComparer.Equals(this.ObjectKeyTable[index], y))
                            {
                                return true;
                            }
                            if (this._hasZeroKey && (index == this._zeroKeyIndex))
                            {
                                return true;
                            }
                        }
                        index++;
                    }
                }
            }
            return false;
        }

        public virtual int GetAccessCountCeiling(int count, int margin)
        {
            return ArrayCounter.Rank(this.AccessTable, this._hashIndex.NewNodePointer, count, this.AccessMin + 1, this.AccessCount, margin);
        }

        public int GetLookup(TKey key, int hash)
        {
            int lookup = this._hashIndex.GetLookup(hash);
            while (lookup >= 0)
            {
                TKey y = this.ObjectKeyTable[lookup];
                if (this.keyComparer.Equals(key, y))
                {
                    return lookup;
                }
                lookup = this._hashIndex.LinkTable[lookup];
            }
            return lookup;
        }

        protected Iterator<TValue> GetValuesIterator(TKey key, int hash)
        {
            int lookup = this.GetLookup(key, hash);
            if (this._valuesIterator == null)
            {
                this._valuesIterator = new ValuesIterator<TKey, TValue, TValue2, TValue3, TKey, TValue, TValue2, TValue3>((BaseHashMap<TKey, TValue, TValue2, TValue3>) this);
            }
            this._valuesIterator.Reset(key, lookup);
            return this._valuesIterator;
        }

        public int IncrementAccessCount()
        {
            int num = this.AccessCount + 1;
            this.AccessCount = num;
            return num;
        }

        public bool IsEmpty()
        {
            return (this._hashIndex.ElementCount == 0);
        }

        protected int NextLookup(int lookup)
        {
            lookup++;
            while (lookup < this._hashIndex.NewNodePointer)
            {
                TKey objB = default(TKey);
                if (object.Equals(this.ObjectKeyTable[lookup], objB) && (!this._hasZeroKey || (lookup != this._zeroKeyIndex)))
                {
                    lookup++;
                }
                else
                {
                    return lookup;
                }
            }
            return -1;
        }

        private int NextLookup(int lookup, int limitLookup, bool hasZeroKey, int zeroKeyIndex)
        {
            lookup++;
            while (lookup < limitLookup)
            {
                TKey y = default(TKey);
                if (this.keyComparer.Equals(this.ObjectKeyTable[lookup], y) && (!hasZeroKey || (lookup != zeroKeyIndex)))
                {
                    lookup++;
                }
                else
                {
                    return lookup;
                }
            }
            return lookup;
        }

        protected void Rehash(int newCapacity)
        {
            int newNodePointer = this._hashIndex.NewNodePointer;
            bool hasZeroKey = this._hasZeroKey;
            int zeroKeyIndex = this._zeroKeyIndex;
            if (newCapacity >= this._hashIndex.ElementCount)
            {
                this._hashIndex.Reset((int) (newCapacity * this.LoadFactor), newCapacity);
                if (this.MultiValueTable != null)
                {
                    int length = this.MultiValueTable.Length;
                    while (--length >= 0)
                    {
                        this.MultiValueTable[length] = false;
                    }
                }
                this._hasZeroKey = false;
                this._zeroKeyIndex = -1;
                this.Threshold = newCapacity;
                int lookup = -1;
                while ((lookup = this.NextLookup(lookup, newNodePointer, hasZeroKey, zeroKeyIndex)) < newNodePointer)
                {
                    TKey longKey = default(TKey);
                    TValue objectValue = default(TValue);
                    longKey = this.ObjectKeyTable[lookup];
                    objectValue = this.ObjectValueTable[lookup];
                    if (this.MultiValueTable == null)
                    {
                        if (this.IsTwoObjectValue)
                        {
                            TValue2 objectValueTwo = this.ObjectValueTable2[lookup];
                            int hash = this.HashTable[lookup];
                            this.AddOrRemove(longKey, hash, objectValue, objectValueTwo, false);
                        }
                        else
                        {
                            this.AddOrRemove(longKey, objectValue, false);
                        }
                    }
                    else
                    {
                        this.AddOrRemoveMultiVal(longKey, objectValue, false, false);
                    }
                    if (this.AccessTable != null)
                    {
                        this.AccessTable[this._hashIndex.ElementCount - 1] = this.AccessTable[lookup];
                    }
                }
                this.ResizeElementArrays(this._hashIndex.NewNodePointer, newCapacity);
            }
        }

        private void RemoveFromElementArrays(int lookup)
        {
            int length = this._hashIndex.LinkTable.Length;
            Array objectKeyTable = this.ObjectKeyTable;
            Array.Copy(objectKeyTable, lookup + 1, objectKeyTable, lookup, (length - lookup) - 1);
            this.ObjectKeyTable[length - 1] = default(TKey);
            Array objectValueTable = this.ObjectValueTable;
            Array.Copy(objectValueTable, lookup + 1, objectValueTable, lookup, (length - lookup) - 1);
            this.ObjectValueTable[length - 1] = default(TValue);
        }

        protected object RemoveLookup(int lookup)
        {
            return this.AddOrRemove(this.ObjectKeyTable[lookup], default(TValue), true);
        }

        protected TValue RemoveObject(TKey objectKey, bool rmvRow)
        {
            if (objectKey == null)
            {
                return default(TValue);
            }
            int hashCode = objectKey.GetHashCode();
            int hashIndex = this._hashIndex.GetHashIndex(hashCode);
            int index = this._hashIndex.HashTable[hashIndex];
            int lastLookup = -1;
            TValue local2 = default(TValue);
            while (index >= 0)
            {
                if (this.keyComparer.Equals(this.ObjectKeyTable[index], objectKey))
                {
                    this.ObjectKeyTable[index] = default(TKey);
                    this._hashIndex.UnlinkNode(hashIndex, lastLookup, index);
                    local2 = this.ObjectValueTable[index];
                    this.ObjectValueTable[index] = default(TValue);
                    if (rmvRow)
                    {
                        this.RemoveRow(index);
                    }
                    return local2;
                }
                lastLookup = index;
                index = this._hashIndex.GetNextLookup(index);
            }
            return local2;
        }

        protected void RemoveRow(int lookup)
        {
            this._hashIndex.RemoveEmptyNode(lookup);
            this.RemoveFromElementArrays(lookup);
        }

        protected bool Reset()
        {
            if ((this.MaxCapacity == 0) || (this.MaxCapacity > this.Threshold))
            {
                this.Rehash(this._hashIndex.LinkTable.Length * 2);
                return true;
            }
            if (this.PurgePolicy == 1)
            {
                this.Clear();
                return true;
            }
            if (this.PurgePolicy == 3)
            {
                this.Clear(this.Threshold / 4, this.Threshold >> 8);
                return true;
            }
            if (this.PurgePolicy == 2)
            {
                this.Clear(this.Threshold / 2, this.Threshold >> 8);
                return true;
            }
            return false;
        }

        protected void ResetAccessCount()
        {
            if (this.AccessCount >= 0x7fefffff)
            {
                if (this.AccessMin < 0x7effffff)
                {
                    this.AccessMin = 0x7effffff;
                }
                int length = this.AccessTable.Length;
                while (--length >= 0)
                {
                    if (this.AccessTable[length] <= this.AccessMin)
                    {
                        this.AccessTable[length] = 0;
                    }
                    else
                    {
                        this.AccessTable[length] -= this.AccessMin;
                    }
                }
                this.AccessCount -= this.AccessMin;
                this.AccessMin = 0;
            }
        }

        private void ResizeElementArrays(int dataLength, int newLength)
        {
            int length = (newLength > dataLength) ? dataLength : newLength;
            if (this.ObjectKeyTable != null)
            {
                this.ObjectKeyTable = new TKey[newLength];
                Array.Copy(this.ObjectKeyTable, 0, this.ObjectKeyTable, 0, length);
            }
            this.ObjectValueTable = new TValue[newLength];
            Array.Copy(this.ObjectValueTable, 0, this.ObjectValueTable, 0, length);
            if (this.IsTwoObjectValue)
            {
                this.ObjectValueTable2 = new TValue2[newLength];
                Array.Copy(this.ObjectValueTable2, 0, this.ObjectValueTable2, 0, length);
                this.HashTable = new int[newLength];
                Array.Copy(this.HashTable, 0, this.HashTable, 0, length);
            }
            if (this.ObjectValueTable3 != null)
            {
                this.ObjectValueTable3 = new TValue3[newLength];
                Array.Copy(this.ObjectValueTable3, 0, this.ObjectValueTable3, 0, length);
            }
            if (this.AccessTable != null)
            {
                this.AccessTable = new int[newLength];
                Array.Copy(this.AccessTable, 0, this.AccessTable, 0, length);
            }
            if (this.MultiValueTable != null)
            {
                this.MultiValueTable = new bool[newLength];
                Array.Copy(this.MultiValueTable, 0, this.MultiValueTable, 0, length);
            }
        }

        public virtual void SetAccessCountFloor(int count)
        {
            this.AccessMin = count;
        }

        public int Size()
        {
            return this._hashIndex.ElementCount;
        }

        public class BaseHashIterator<TTKey, TTValue, TTValue2, TTValue3> : Iterator<TTValue>
        {
            private int _lookup;
            private int _counter;
            private bool _removed;
            private BaseHashMap<TTKey, TTValue, TTValue2, TTValue3> _o;

            public BaseHashIterator(BaseHashMap<TTKey, TTValue, TTValue2, TTValue3> o)
            {
                this._lookup = -1;
                this._o = o;
            }

            public int GetAccessCount()
            {
                if (this._removed || (this._o.AccessTable == null))
                {
                    throw new InvalidOperationException("");
                }
                return this._o.AccessTable[this._lookup];
            }

            public int GetLookup()
            {
                return this._lookup;
            }

            public bool HasNext()
            {
                return (this._counter < this._o._hashIndex.ElementCount);
            }

            public TTValue Next()
            {
                this._removed = false;
                if (!this.HasNext())
                {
                    throw new InvalidOperationException("Hash Iterator");
                }
                this._counter++;
                this._lookup = this._o.NextLookup(this._lookup);
                return this._o.ObjectValueTable[this._lookup];
            }

            public void Remove()
            {
                if (this._removed)
                {
                    throw new InvalidOperationException("Hash Iterator");
                }
                this._counter--;
                this._removed = true;
                if (this._o.MultiValueTable == null)
                {
                    this._o.AddOrRemove(this._o.ObjectKeyTable[this._lookup], default(TTValue), true);
                }
                else
                {
                    this._o.AddOrRemoveMultiVal(this._o.ObjectKeyTable[this._lookup], this._o.ObjectValueTable[this._lookup], false, true);
                }
                if (this._o.IsList)
                {
                    this._o.RemoveRow(this._lookup);
                    this._lookup--;
                }
            }

            public void SetAccessCount(int count)
            {
                if (this._removed || (this._o.AccessTable == null))
                {
                    throw new KeyNotFoundException();
                }
                this._o.AccessTable[this._lookup] = count;
            }

            public void SetValue(TTValue value)
            {
                this._o.ObjectValueTable[this._lookup] = value;
            }
        }

        public class BaseHashIteratorKeys<TTKey, TTValue, TTValue2, TTValue3> : Iterator<TTKey>
        {
            private int _lookup;
            private int _counter;
            private bool _removed;
            private BaseHashMap<TTKey, TTValue, TTValue2, TTValue3> _o;

            public BaseHashIteratorKeys(BaseHashMap<TTKey, TTValue, TTValue2, TTValue3> o)
            {
                this._lookup = -1;
                this._o = o;
            }

            public int GetAccessCount()
            {
                if (this._removed || (this._o.AccessTable == null))
                {
                    throw new InvalidOperationException("");
                }
                return this._o.AccessTable[this._lookup];
            }

            public int GetLookup()
            {
                return this._lookup;
            }

            public bool HasNext()
            {
                return (this._counter < this._o._hashIndex.ElementCount);
            }

            public TTKey Next()
            {
                this._removed = false;
                if (!this.HasNext())
                {
                    throw new InvalidOperationException("Hash Iterator");
                }
                this._counter++;
                this._lookup = this._o.NextLookup(this._lookup);
                return this._o.ObjectKeyTable[this._lookup];
            }

            public void Remove()
            {
                if (this._removed)
                {
                    throw new InvalidOperationException("Hash Iterator");
                }
                this._counter--;
                this._removed = true;
                if (this._o.MultiValueTable == null)
                {
                    this._o.AddOrRemove(this._o.ObjectKeyTable[this._lookup], default(TTValue), true);
                }
                else
                {
                    this._o.AddOrRemoveMultiVal(this._o.ObjectKeyTable[this._lookup], default(TTValue), true, false);
                }
                if (this._o.IsList)
                {
                    this._o.RemoveRow(this._lookup);
                    this._lookup--;
                }
            }

            public void SetAccessCount(int count)
            {
                if (this._removed || (this._o.AccessTable == null))
                {
                    throw new KeyNotFoundException();
                }
                this._o.AccessTable[this._lookup] = count;
            }

            public void SetValue(TTKey value)
            {
                throw new KeyNotFoundException();
            }
        }

        protected class MultiValueKeyIterator<TTKey, TTValue, TTValue2, TTValue3> : Iterator<TTKey>
        {
            private int _lookup;
            private readonly BaseHashMap<TTKey, TTValue, TTValue2, TTValue3> _o;

            public MultiValueKeyIterator(BaseHashMap<TTKey, TTValue, TTValue2, TTValue3> o)
            {
                this._lookup = -1;
                this._o = o;
                this.ToNextLookup();
            }

            public bool HasNext()
            {
                return (this._lookup != -1);
            }

            public TTKey Next()
            {
                this.ToNextLookup();
                return this._o.ObjectKeyTable[this._lookup];
            }

            public void Remove()
            {
                throw new KeyNotFoundException("Hash Iterator");
            }

            public void SetValue(TTKey value)
            {
                throw new KeyNotFoundException("Hash Iterator");
            }

            private void ToNextLookup()
            {
                do
                {
                    this._lookup = this._o.NextLookup(this._lookup);
                }
                while ((this._lookup != -1) && this._o.MultiValueTable[this._lookup]);
            }
        }

        protected class ValuesIterator<TTKey, TTValue, TTValue2, TTValue3> : Iterator<TTValue>
        {
            private int _lookup;
            private TTKey _key;
            private readonly BaseHashMap<TTKey, TTValue, TTValue2, TTValue3> _o;

            public ValuesIterator(BaseHashMap<TTKey, TTValue, TTValue2, TTValue3> o)
            {
                this._lookup = -1;
                this._o = o;
            }

            public bool HasNext()
            {
                return (this._lookup != -1);
            }

            public TTValue Next()
            {
                if (this._lookup == -1)
                {
                    return default(TTValue);
                }
                TTValue local2 = this._o.ObjectValueTable[this._lookup];
                do
                {
                    this._lookup = this._o._hashIndex.GetNextLookup(this._lookup);
                }
                while ((this._lookup != -1) && !this._o.keyComparer.Equals(this._o.ObjectKeyTable[this._lookup], this._key));
                return local2;
            }

            public void Remove()
            {
                throw new KeyNotFoundException("Hash Iterator");
            }

            public void Reset(TTKey key, int lookup)
            {
                this._key = key;
                this._lookup = lookup;
            }

            public void SetValue(TTValue value)
            {
                throw new KeyNotFoundException("Hash Iterator");
            }
        }
    }
}

