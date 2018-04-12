namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cStore;
    using System;

    public class OrderedIntKeyHashMap<TValue> : BaseHashMap<int, TValue, TValue, TValue>
    {
        private ISet<int> _keySet;
        private ICollection<TValue> _values;

        public OrderedIntKeyHashMap() : this(8)
        {
        }

        public OrderedIntKeyHashMap(int initialCapacity) : base(initialCapacity, 1, 3, false)
        {
            base.IsList = true;
        }

        public ISet<int> GetKeySet()
        {
            ISet<int> set = this._keySet;
            if (set == null)
            {
                set = this._keySet = new KeySet<TValue, TValue>((OrderedIntKeyHashMap<TValue>) this);
            }
            return set;
        }

        public ICollection<TValue> GetValues()
        {
            ICollection<TValue> is2 = this._values;
            if (is2 == null)
            {
                is2 = this._values = new Values<TValue, TValue>((OrderedIntKeyHashMap<TValue>) this);
            }
            return is2;
        }

        public TValue Put(int key, TValue value)
        {
            return base.AddOrRemove(key, key, value, default(TValue), false);
        }

        public void ValuesToArray(TValue[] array)
        {
            Iterator<TValue> iterator = this.GetValues().GetIterator();
            for (int i = 0; iterator.HasNext(); i++)
            {
                array[i] = iterator.Next();
            }
        }

        private class KeySet<TTValue> : ISet<int>, ICollection<int>
        {
            private readonly OrderedIntKeyHashMap<TTValue> _o;

            public KeySet(OrderedIntKeyHashMap<TTValue> o)
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
            private readonly OrderedIntKeyHashMap<TTValue> _o;

            public Values(OrderedIntKeyHashMap<TTValue> o)
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

