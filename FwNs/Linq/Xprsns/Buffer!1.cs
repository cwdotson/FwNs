namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Buffer<T>
    {
        public readonly T[] Items;
        public readonly int Count;
        internal Buffer(IEnumerable<T> source)
        {
            T[] array = null;
            int length = 0;
            ICollection<T> is2 = source as ICollection<T>;
            if (is2 != null)
            {
                length = is2.Count;
                if (length > 0)
                {
                    array = new T[length];
                    is2.CopyTo(array, 0);
                }
            }
            else
            {
                foreach (T local in source)
                {
                    if (array == null)
                    {
                        array = new T[4];
                    }
                    else if (array.Length == length)
                    {
                        T[] destinationArray = new T[length * 2];
                        Array.Copy(array, 0, destinationArray, 0, length);
                        array = destinationArray;
                    }
                    array[length] = local;
                    length++;
                }
            }
            this.Items = array;
            this.Count = length;
        }

        public Buffer(T[] items, int count)
        {
            this.Items = items;
            this.Count = count;
        }

        public T[] ToArray()
        {
            if (this.Count == 0)
            {
                return new T[0];
            }
            if (this.Items.Length == this.Count)
            {
                return this.Items;
            }
            T[] destinationArray = new T[this.Count];
            Array.Copy(this.Items, 0, destinationArray, 0, this.Count);
            return destinationArray;
        }

        public void Sort(IComparer<T> comparer)
        {
            if (this.Items != null)
            {
                Array.Sort<T>(this.Items, 0, this.Count, comparer);
            }
        }
    }
}

