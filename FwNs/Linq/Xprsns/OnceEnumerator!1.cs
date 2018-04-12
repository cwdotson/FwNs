namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class OnceEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private bool _moved;
        private readonly T _item;

        public OnceEnumerator(T item)
        {
            this._item = item;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (this._moved)
            {
                return false;
            }
            this._moved = true;
            return true;
        }

        public void Reset()
        {
            this._moved = false;
        }

        public T Current
        {
            get
            {
                return this._item;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this._item;
            }
        }
    }
}

