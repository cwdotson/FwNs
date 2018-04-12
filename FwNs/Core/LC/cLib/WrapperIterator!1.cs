namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;

    public class WrapperIterator<T> : Iterator<T>
    {
        private static readonly T[] Emptyelements;
        private readonly bool _chained;
        private readonly bool _notNull;
        private T[] _elements;
        private int _i;
        private Iterator<T> _it1;
        private Iterator<T> _it2;

        static WrapperIterator()
        {
            WrapperIterator<T>.Emptyelements = new T[0];
        }

        public WrapperIterator()
        {
            this._elements = WrapperIterator<T>.Emptyelements;
        }

        public WrapperIterator(T[] elements)
        {
            this._elements = elements;
        }

        public WrapperIterator(T element)
        {
            this._elements = new T[] { element };
        }

        public WrapperIterator(Iterator<T> it1, Iterator<T> it2)
        {
            this._it1 = it1;
            this._it2 = it2;
            this._chained = true;
        }

        public WrapperIterator(T[] elements, bool notNull)
        {
            this._elements = elements;
            this._notNull = notNull;
        }

        public bool HasNext()
        {
            if (this._chained)
            {
                if (this._it1 == null)
                {
                    if (this._it2 != null)
                    {
                        if (this._it2.HasNext())
                        {
                            return true;
                        }
                        this._it2 = null;
                    }
                    return false;
                }
                if (this._it1.HasNext())
                {
                    return true;
                }
                this._it1 = null;
                return this.HasNext();
            }
            if (this._elements != null)
            {
                while ((this._notNull && (this._i < this._elements.Length)) && (this._elements[this._i] == null))
                {
                    this._i++;
                }
                if (this._i < this._elements.Length)
                {
                    return true;
                }
                this._elements = null;
                return false;
            }
            return false;
        }

        public T Next()
        {
            if (this._chained)
            {
                if (this._it1 == null)
                {
                    if (this._it2 == null)
                    {
                        throw new KeyNotFoundException();
                    }
                    if (this._it2.HasNext())
                    {
                        return this._it2.Next();
                    }
                    this._it2 = null;
                    this.Next();
                }
                else
                {
                    if (this._it1.HasNext())
                    {
                        return this._it1.Next();
                    }
                    this._it1 = null;
                    this.Next();
                }
            }
            if (!this.HasNext())
            {
                throw new KeyNotFoundException();
            }
            int index = this._i;
            this._i = index + 1;
            return this._elements[index];
        }

        public void Remove()
        {
            throw new KeyNotFoundException();
        }

        public void SetValue(T value)
        {
            throw new KeyNotFoundException();
        }
    }
}

