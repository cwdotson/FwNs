namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cStore;
    using System;

    public class LongKeyHashMap<T> : BaseHashMap<long, T, T, T>
    {
        private ISet<long> _keySet;
        private ICollection<T> _values;
        private object _lock;

        public LongKeyHashMap() : this(0x10)
        {
        }

        public LongKeyHashMap(int initialCapacity) : base(initialCapacity, 3, 3, false)
        {
            this._lock = new Dummy<T>();
        }

        public override bool ContainsKey(long key)
        {
            lock (this._lock)
            {
                return base.ContainsKey(key);
            }
        }

        public override bool ContainsValue(T value)
        {
            lock (this._lock)
            {
                return base.ContainsValue(value);
            }
        }

        public T Get(long key)
        {
            lock (this._lock)
            {
                int lookup = base.GetLookup(key, (int) key);
                if (lookup != -1)
                {
                    return base.ObjectValueTable[lookup];
                }
                return default(T);
            }
        }

        public ISet<long> GetKeySet()
        {
            if (this._keySet == null)
            {
                this._keySet = new KeySet<T, T>((LongKeyHashMap<T>) this);
            }
            return this._keySet;
        }

        public int GetOrderedMatchCount(int[] array)
        {
            int index = 0;
            lock (this._lock)
            {
                while ((index < array.Length) && base.ContainsKey((long) array[index]))
                {
                    index++;
                }
                return index;
            }
        }

        public ICollection<T> GetValues()
        {
            if (this._values == null)
            {
                this._values = new Values<T, T>((LongKeyHashMap<T>) this);
            }
            return this._values;
        }

        public T Put(long key, T value)
        {
            lock (this._lock)
            {
                return base.AddOrRemove(key, (int) key, value, default(T), false);
            }
        }

        public T Remove(long key)
        {
            lock (this._lock)
            {
                T objectValue = default(T);
                return base.AddOrRemove(key, (int) key, objectValue, default(T), true);
            }
        }

        public T[] ToArray()
        {
            lock (this._lock)
            {
                T[] localArray2 = new T[base.Size()];
                int num = 0;
                Iterator<T> iterator = new BaseHashMap<long, T, T, T>.BaseHashIterator<long, T, T, T>(this);
                while (iterator.HasNext())
                {
                    localArray2[num++] = iterator.Next();
                }
                return localArray2;
            }
        }

        private class Dummy
        {
        }

        private class KeySet<TT> : ISet<long>, ICollection<long>
        {
            private LongKeyHashMap<TT> _o;

            public KeySet(LongKeyHashMap<TT> _o)
            {
                this._o = _o;
            }

            public bool Add(long value)
            {
                throw new Exception();
            }

            public bool AddAll(ICollection<long> c)
            {
                throw new Exception();
            }

            public void Clear()
            {
                this._o.Clear();
            }

            public bool Contains(long o)
            {
                throw new Exception();
            }

            public long Get(long key)
            {
                throw new Exception();
            }

            public Iterator<long> GetIterator()
            {
                return new BaseHashMap<long, T, T, T>.BaseHashIteratorKeys<long, TT, TT, TT>(this._o);
            }

            public bool IsEmpty()
            {
                return (this.Size() == 0);
            }

            public bool Remove(long o)
            {
                throw new Exception();
            }

            public int Size()
            {
                return this._o.Size();
            }
        }

        private class Values<TT> : ICollection<TT>
        {
            private LongKeyHashMap<TT> _o;

            public Values(LongKeyHashMap<TT> _o)
            {
                this._o = _o;
            }

            public bool Add(TT value)
            {
                throw new Exception();
            }

            public bool AddAll(ICollection<TT> c)
            {
                throw new Exception();
            }

            public void Clear()
            {
                this._o.Clear();
            }

            public bool Contains(TT o)
            {
                throw new Exception();
            }

            public Iterator<TT> GetIterator()
            {
                return new BaseHashMap<long, T, T, T>.BaseHashIterator<long, TT, TT, TT>(this._o);
            }

            public bool IsEmpty()
            {
                return (this.Size() == 0);
            }

            public bool Remove(TT o)
            {
                throw new Exception();
            }

            public int Size()
            {
                return this._o.Size();
            }
        }
    }
}

