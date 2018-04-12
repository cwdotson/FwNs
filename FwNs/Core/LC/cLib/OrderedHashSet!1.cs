namespace FwNs.Core.LC.cLib
{
    using System;

    public class OrderedHashSet<T> : UtlHashSet<T>, IUtlList<T>, ICollection<T>
    {
        public OrderedHashSet() : base(8)
        {
            base.IsList = true;
        }

        public static OrderedHashSet<T> Add(OrderedHashSet<T> first, T value)
        {
            if (value != null)
            {
                if (first == null)
                {
                    first = new OrderedHashSet<T>();
                }
                first.Add(value);
            }
            return first;
        }

        public void Add(int index, T key)
        {
            throw new IndexOutOfRangeException();
        }

        public static OrderedHashSet<T> AddAll(OrderedHashSet<T> first, OrderedHashSet<T> second)
        {
            if (second != null)
            {
                if (first == null)
                {
                    first = new OrderedHashSet<T>();
                }
                first.AddAll(second);
            }
            return first;
        }

        private void CheckRange(int i)
        {
            if ((i < 0) || (i >= base.Size()))
            {
                throw new IndexOutOfRangeException();
            }
        }

        public T Get(int index)
        {
            this.CheckRange(index);
            return base.ObjectKeyTable[index];
        }

        public int GetCommonElementCount(ISet<T> other)
        {
            int num = 0;
            int index = 0;
            int num3 = base.Size();
            while (index < num3)
            {
                if (other.Contains(base.ObjectKeyTable[index]))
                {
                    num++;
                }
                index++;
            }
            return num;
        }

        public int GetIndex(T key)
        {
            return base.GetLookup(key, key.GetHashCode());
        }

        public int GetLargestIndex(OrderedHashSet<T> other)
        {
            int num = -1;
            int index = 0;
            int num3 = other.Size();
            while (index < num3)
            {
                int num4 = this.GetIndex(other.Get(index));
                if (num4 > num)
                {
                    num = num4;
                }
                index++;
            }
            return num;
        }

        public override bool Remove(T key)
        {
            base.RemoveObject(key, true);
            return (base.Size() != base.Size());
        }

        public T Remove(int index)
        {
            this.CheckRange(index);
            T key = base.ObjectKeyTable[index];
            this.Remove(key);
            return key;
        }

        public T Set(int index, T key)
        {
            throw new IndexOutOfRangeException();
        }
    }
}

