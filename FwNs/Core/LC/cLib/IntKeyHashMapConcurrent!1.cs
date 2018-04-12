namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cStore;
    using System;
    using System.Threading;

    public class IntKeyHashMapConcurrent<TValue> : BaseHashMap<int, TValue, TValue, TValue>, IDisposable
    {
        private ISet<int> _keySet;
        private ICollection<TValue> _values;
        private object _lock;

        public IntKeyHashMapConcurrent() : this(8)
        {
        }

        public IntKeyHashMapConcurrent(int initialCapacity) : base(initialCapacity, 1, 3, false)
        {
            this._lock = new Dummy<TValue>();
        }

        public override bool ContainsKey(int key)
        {
            bool flag;
            try
            {
                Monitor.Enter(this._lock);
                flag = base.ContainsKey(key);
            }
            finally
            {
                Monitor.Exit(this._lock);
            }
            return flag;
        }

        public override bool ContainsValue(TValue value)
        {
            bool flag;
            try
            {
                Monitor.Enter(this._lock);
                flag = base.ContainsValue(value);
            }
            finally
            {
                Monitor.Exit(this._lock);
            }
            return flag;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
        }

        public TValue Get(int key)
        {
            TValue local;
            try
            {
                Monitor.Enter(this._lock);
                int lookup = base.GetLookup(key, key);
                if (lookup != -1)
                {
                    return base.ObjectValueTable[lookup];
                }
                local = default(TValue);
            }
            finally
            {
                Monitor.Exit(this._lock);
            }
            return local;
        }

        public ISet<int> GetKeySet()
        {
            ISet<int> set = this._keySet;
            if (set == null)
            {
                set = this._keySet = new KeySet<TValue, TValue>((IntKeyHashMapConcurrent<TValue>) this);
            }
            return set;
        }

        public ICollection<TValue> GetValues()
        {
            ICollection<TValue> is2 = this._values;
            if (is2 == null)
            {
                is2 = this._values = new Values<TValue, TValue>((IntKeyHashMapConcurrent<TValue>) this);
            }
            return is2;
        }

        public object GetWriteLock()
        {
            return this._lock;
        }

        public TValue Put(int key, TValue value)
        {
            TValue local;
            try
            {
                Monitor.Enter(this._lock);
                local = base.AddOrRemove(key, value, false);
            }
            finally
            {
                Monitor.Exit(this._lock);
            }
            return local;
        }

        public TValue Remove(int key)
        {
            TValue local;
            try
            {
                Monitor.Enter(this._lock);
                local = base.AddOrRemove(key, default(TValue), true);
            }
            finally
            {
                Monitor.Exit(this._lock);
            }
            return local;
        }

        public void WriteLock()
        {
            Monitor.Enter(this._lock);
        }

        public void WriteUnLock()
        {
            Monitor.Exit(this._lock);
        }

        private class Dummy
        {
        }

        private class KeySet<TTValue> : ISet<int>, ICollection<int>
        {
            private readonly IntKeyHashMapConcurrent<TTValue> _o;

            public KeySet(IntKeyHashMapConcurrent<TTValue> o)
            {
                this._o = o;
            }

            public bool Add(int value)
            {
                throw new Exception();
            }

            public bool AddAll(ICollection<int> c)
            {
                throw new Exception();
            }

            public void Clear()
            {
                this._o.Clear();
            }

            public bool Contains(int o)
            {
                throw new Exception();
            }

            public int Get(int key)
            {
                throw new Exception();
            }

            public Iterator<int> GetIterator()
            {
                return new BaseHashMap<int, TValue, TValue, TValue>.BaseHashIteratorKeys<int, TTValue, TTValue, TTValue>(this._o);
            }

            public bool IsEmpty()
            {
                return (this.Size() == 0);
            }

            public bool Remove(int o)
            {
                throw new Exception();
            }

            public int Size()
            {
                return this._o.Size();
            }
        }

        private class Values<TTValue> : ICollection<TTValue>
        {
            private readonly IntKeyHashMapConcurrent<TTValue> _o;

            public Values(IntKeyHashMapConcurrent<TTValue> o)
            {
                this._o = o;
            }

            public bool Add(TTValue value)
            {
                throw new Exception();
            }

            public bool AddAll(ICollection<TTValue> c)
            {
                throw new Exception();
            }

            public void Clear()
            {
                this._o.Clear();
            }

            public bool Contains(TTValue o)
            {
                throw new Exception();
            }

            public Iterator<TTValue> GetIterator()
            {
                return new BaseHashMap<int, TValue, TValue, TValue>.BaseHashIterator<int, TTValue, TTValue, TTValue>(this._o);
            }

            public bool IsEmpty()
            {
                return (this.Size() == 0);
            }

            public bool Remove(TTValue o)
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

