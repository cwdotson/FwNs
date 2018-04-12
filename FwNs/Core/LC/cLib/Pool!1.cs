namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;

    public class Pool<T> where T: class, new()
    {
        private const int PoolSize = 10;
        private Queue<T> _items;
        private int _poolSize;

        public Pool()
        {
            this._items = new Queue<T>();
            this._poolSize = 10;
        }

        public Pool(int size)
        {
            this._items = new Queue<T>();
            this._poolSize = 10;
            this._poolSize = size;
        }

        public T Fetch()
        {
            if (this._items.Count == 0)
            {
                return Activator.CreateInstance<T>();
            }
            return this._items.Dequeue();
        }

        public void Store(T item)
        {
            if (this._items.Count < this._poolSize)
            {
                this._items.Enqueue(item);
            }
        }
    }
}

