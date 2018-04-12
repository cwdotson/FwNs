namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class PagedSequence<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
    {
        private const int pageSize = 8;
        private int _lastPageItemCount;
        private Page<T> _first;
        private Page<T> _last;
        private int _count;
        private int _version;
        private object _syncRoot;

        public PagedSequence()
        {
            this._first = this._last = new Page<T>();
        }

        public void Add(T item)
        {
            if (this._lastPageItemCount < 8)
            {
                int index = this._lastPageItemCount;
                this._lastPageItemCount = index + 1;
                this._last.Items[index] = item;
            }
            else
            {
                Page<T> page = new Page<T>();
                page.Items[0] = item;
                this._lastPageItemCount = 1;
                this._last.Next = page;
                this._last = page;
            }
            this._count++;
            this._version++;
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T local in items)
            {
                this.Add(local);
            }
        }

        public void Clear()
        {
            if (this._count != 0)
            {
                this._first = this._last;
                this._lastPageItemCount = 0;
                this._count = 0;
                this._version++;
                for (int i = 0; i < this._first.Items.Length; i++)
                {
                    this._first.Items[i] = default(T);
                }
            }
        }

        public bool Contains(T item)
        {
            if (this._count == 0)
            {
                return false;
            }
            EqualityComparer<T> comparer1 = EqualityComparer<T>.Default;
            for (Page<T> page = this._first; page.Next != null; page = page.Next)
            {
                if (Array.IndexOf<T>(page.Items, item) != -1)
                {
                    return true;
                }
            }
            return (Array.IndexOf<T>(this._last.Items, item, 0, this._lastPageItemCount) != -1);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            Argument.NotNull<Array>(array, "array");
            Argument.NotLess(arrayIndex, 0, "arrayIndex");
            if ((array.Length - arrayIndex) < this._count)
            {
                throw new OverflowException(Errorz.Argument_ArrayIsTooSmall);
            }
            if (this._count != 0)
            {
                if (array.Length > 0)
                {
                    Argument.NotGreater(arrayIndex, array.Length - 1, "arrayIndex");
                }
                for (Page<T> page = this._first; page.Next != null; page = page.Next)
                {
                    Array.Copy(page.Items, 0, array, arrayIndex, page.Items.Length);
                    arrayIndex += page.Items.Length;
                }
                Array.Copy(this._last.Items, 0, array, arrayIndex, this._lastPageItemCount);
            }
        }

        public Enumerator<T> GetEnumerator()
        {
            return new Enumerator<T>((PagedSequence<T>) this);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this._count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private readonly PagedSequence<T> _seq;
            private PagedSequence<T>.Page _page;
            private int _index;
            private T _current;
            private int _version;
            public T Current
            {
                get
                {
                    return this._current;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    return this._current;
                }
            }
            internal Enumerator(PagedSequence<T> seq)
            {
                this._seq = seq;
                this._index = -1;
                this._page = this._seq._first;
                this._version = this._seq._version;
                this._current = default(T);
            }

            void IDisposable.Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this._seq._version != this._version)
                {
                    throw new InvalidOperationException(EMDataStructures.EnumFailedVersion);
                }
                if (this._page == null)
                {
                    return false;
                }
                this._index++;
                if (this._page.Next != null)
                {
                    if (this._index >= 8)
                    {
                        this._page = this._page.Next;
                        this._index = 0;
                    }
                }
                else if (this._index >= this._seq._lastPageItemCount)
                {
                    this._page = null;
                    return false;
                }
                this._current = this._page.Items[this._index];
                return true;
            }

            public void Reset()
            {
                this._index = -1;
                this._page = this._seq._first;
                this._version = this._seq._version;
                this._current = default(T);
            }
        }

        private class Page
        {
            public readonly T[] Items;
            public PagedSequence<T>.Page Next;

            public Page()
            {
                this.Items = new T[8];
            }
        }
    }
}

