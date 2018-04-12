namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cStore;
    using System;

    public class MultiValueHashMap<TKey, TValue> : BaseHashMap<TKey, TValue, TValue, TValue>
    {
        protected ISet<TKey> keySet;
        protected Iterator<TValue> ValueIterator;
        protected ICollection<TValue> values;

        public MultiValueHashMap() : this(8)
        {
        }

        public MultiValueHashMap(int initialCapacity) : base(initialCapacity, 3, 3, false)
        {
            base.MultiValueTable = new bool[base.ObjectValueTable.Length];
        }

        public Iterator<TValue> Get(TKey key)
        {
            int hashCode = key.GetHashCode();
            return base.GetValuesIterator(key, hashCode);
        }

        public ISet<TKey> GetKeySet()
        {
            ISet<TKey> keySet = this.keySet;
            if (keySet == null)
            {
                keySet = this.keySet = new KeySet<TKey, TValue, TKey, TValue>((MultiValueHashMap<TKey, TValue>) this);
            }
            return keySet;
        }

        public ICollection<TValue> GetValues()
        {
            ICollection<TValue> values = this.values;
            if (values == null)
            {
                values = this.values = new Values<TKey, TValue, TKey, TValue>((MultiValueHashMap<TKey, TValue>) this);
            }
            return values;
        }

        public object Put(TKey key, TValue value)
        {
            return base.AddOrRemoveMultiVal(key, value, false, false);
        }

        public object Remove(TKey key)
        {
            return base.AddOrRemoveMultiVal(key, default(TValue), true, false);
        }

        public object Remove(TKey key, TValue value)
        {
            return base.AddOrRemoveMultiVal(key, value, false, true);
        }

        private class KeySet<TTKey, TTValue> : ISet<TTKey>, ICollection<TTKey>
        {
            private readonly MultiValueHashMap<TTKey, TTValue> _o;

            public KeySet(MultiValueHashMap<TTKey, TTValue> o)
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
                throw new Exception();
            }

            public Iterator<TTKey> GetIterator()
            {
                return new BaseHashMap<TKey, TValue, TValue, TValue>.MultiValueKeyIterator<TTKey, TTValue, TTValue, TTValue>(this._o);
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
            private readonly MultiValueHashMap<TTKey, TTValue> _o;

            public Values(MultiValueHashMap<TTKey, TTValue> o)
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

