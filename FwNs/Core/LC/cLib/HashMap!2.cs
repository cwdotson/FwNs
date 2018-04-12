namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cStore;
    using System;

    public class HashMap<TKey, TValue> : BaseHashMap<TKey, TValue, TValue, TValue>
    {
        public ISet<TKey> keySet;
        public ICollection<TValue> values;

        public HashMap() : this(8)
        {
        }

        public HashMap(int initialCapacity) : base(initialCapacity, 3, 3, false)
        {
        }

        public TValue Get(TKey key)
        {
            int hashCode = key.GetHashCode();
            int lookup = base.GetLookup(key, hashCode);
            if (lookup != -1)
            {
                return base.ObjectValueTable[lookup];
            }
            return default(TValue);
        }

        public ISet<TKey> GetKeySet()
        {
            ISet<TKey> keySet = this.keySet;
            if (keySet == null)
            {
                keySet = this.keySet = new KeySet<TKey, TValue, TKey, TValue>((HashMap<TKey, TValue>) this);
            }
            return keySet;
        }

        public ICollection<TValue> GetValues()
        {
            ICollection<TValue> values = this.values;
            if (values == null)
            {
                values = this.values = new Values<TKey, TValue, TKey, TValue>((HashMap<TKey, TValue>) this);
            }
            return values;
        }

        public virtual TValue Put(TKey key, TValue value)
        {
            return base.AddOrRemove(key, value, false);
        }

        public virtual TValue Remove(TKey key)
        {
            return base.RemoveObject(key, false);
        }

        private class KeySet<TTKey, TTValue> : ISet<TTKey>, ICollection<TTKey>
        {
            private readonly HashMap<TTKey, TTValue> _o;

            public KeySet(HashMap<TTKey, TTValue> o)
            {
                this._o = o;
            }

            public bool Add(TTKey value)
            {
                throw new Exception();
            }

            public bool AddAll(ICollection<TTKey> c)
            {
                throw new Exception();
            }

            public void Clear()
            {
                this._o.Clear();
            }

            public bool Contains(TTKey o)
            {
                return this._o.ContainsKey(o);
            }

            public TTKey Get(TTKey key)
            {
                int lookup = this._o.GetLookup(key, key.GetHashCode());
                if (lookup < 0)
                {
                    return default(TTKey);
                }
                return this._o.ObjectKeyTable[lookup];
            }

            public Iterator<TTKey> GetIterator()
            {
                return new BaseHashMap<TKey, TValue, TValue, TValue>.BaseHashIteratorKeys<TTKey, TTValue, TTValue, TTValue>(this._o);
            }

            public bool IsEmpty()
            {
                return (this.Size() == 0);
            }

            public bool Remove(TTKey o)
            {
                int num = this.Size();
                this._o.Remove(o);
                return (this.Size() != num);
            }

            public int Size()
            {
                return this._o.Size();
            }
        }

        private class Values<TTKey, TTValue> : ICollection<TTValue>
        {
            private readonly HashMap<TTKey, TTValue> _o;

            public Values(HashMap<TTKey, TTValue> o)
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
                return new BaseHashMap<TKey, TValue, TValue, TValue>.BaseHashIterator<TTKey, TTValue, TTValue, TTValue>(this._o);
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

