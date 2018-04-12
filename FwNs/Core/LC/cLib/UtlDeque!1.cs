namespace FwNs.Core.LC.cLib
{
    using System;

    public class UtlDeque<T>
    {
        private const int DefaultInitialCapacity = 10;
        private int _endindex;
        private int _firstindex;
        private T[] _list;
        private int _elementCount;

        public UtlDeque()
        {
            this._list = new T[10];
        }

        public bool Add(T o)
        {
            this.ResetCapacity();
            if (this._endindex == this._list.Length)
            {
                this._endindex = 0;
            }
            this._list[this._endindex] = o;
            this._elementCount++;
            this._endindex++;
            return true;
        }

        public bool AddLast(T o)
        {
            return this.Add(o);
        }

        public T RemoveFirst()
        {
            // This item is obfuscated and can not be translated.
        }

        public T RemoveLast()
        {
            // This item is obfuscated and can not be translated.
        }

        private void ResetCapacity()
        {
            if (this._elementCount >= this._list.Length)
            {
                T[] destinationArray = new T[this._list.Length * 2];
                Array.Copy(this._list, this._firstindex, destinationArray, this._firstindex, this._list.Length - this._firstindex);
                if (this._endindex <= this._firstindex)
                {
                    Array.Copy(this._list, 0, destinationArray, this._list.Length, this._endindex);
                    this._endindex = this._list.Length + this._endindex;
                }
                this._list = destinationArray;
            }
        }
    }
}

