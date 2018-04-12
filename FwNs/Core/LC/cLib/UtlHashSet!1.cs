namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cStore;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class UtlHashSet<T> : BaseHashMap<T, T, T, T>, FwNs.Core.LC.cLib.ISet<T>, FwNs.Core.LC.cLib.ICollection<T>
    {
        public UtlHashSet() : this(8)
        {
        }

        public UtlHashSet(int initialCapacity) : base(initialCapacity, 3, 0, false)
        {
        }

        public bool Add(T key)
        {
            base.AddOrRemove(key, default(T), false);
            return (base.Size() != base.Size());
        }

        public bool AddAll(FwNs.Core.LC.cLib.ICollection<T> c)
        {
            bool flag = false;
            Iterator<T> iterator = c.GetIterator();
            while (iterator.HasNext())
            {
                flag |= this.Add(iterator.Next());
            }
            return flag;
        }

        public bool AddAll<TT>(FwNs.Core.LC.cLib.ICollection<TT> c) where TT: T
        {
            bool flag = false;
            Iterator<TT> iterator = c.GetIterator();
            while (iterator.HasNext())
            {
                flag |= this.Add(iterator.Next());
            }
            return flag;
        }

        public bool AddAll(List<T> c)
        {
            bool flag = false;
            foreach (T local in c)
            {
                flag |= this.Add(local);
            }
            return flag;
        }

        public bool AddAll(T[] keys)
        {
            bool flag = false;
            for (int i = 0; i < keys.Length; i++)
            {
                flag |= this.Add(keys[i]);
            }
            return flag;
        }

        public bool Contains(T key)
        {
            return base.ContainsKey(key);
        }

        public bool ContainsAll(FwNs.Core.LC.cLib.ICollection<T> col)
        {
            Iterator<T> iterator = col.GetIterator();
            while (iterator.HasNext())
            {
                if (!this.Contains(iterator.Next()))
                {
                    return false;
                }
            }
            return true;
        }

        public T Get(T key)
        {
            int lookup = base.GetLookup(key, key.GetHashCode());
            if (lookup < 0)
            {
                return default(T);
            }
            return base.ObjectKeyTable[lookup];
        }

        public Iterator<T> GetIterator()
        {
            return new BaseHashMap<T, T, T, T>.BaseHashIteratorKeys<T, T, T, T>(this);
        }

        public virtual bool Remove(T key)
        {
            return (base.RemoveObject(key, false) > null);
        }

        public bool RemoveAll(FwNs.Core.LC.cLib.ICollection<T> c)
        {
            Iterator<T> iterator = c.GetIterator();
            bool flag = true;
            while (iterator.HasNext())
            {
                flag &= this.Remove(iterator.Next());
            }
            return flag;
        }

        public bool RemoveAll(T[] keys)
        {
            bool flag = true;
            for (int i = 0; i < keys.Length; i++)
            {
                flag &= this.Remove(keys[i]);
            }
            return flag;
        }

        public T[] ToArray(T[] a)
        {
            if ((a == null) || (a.Length < base.Size()))
            {
                a = new T[base.Size()];
            }
            Iterator<T> iterator = this.GetIterator();
            for (int i = 0; iterator.HasNext(); i++)
            {
                a[i] = iterator.Next();
            }
            return a;
        }

        public override string ToString()
        {
            Iterator<T> iterator = this.GetIterator();
            StringBuilder builder = new StringBuilder();
            while (iterator.HasNext())
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                else
                {
                    builder.Append('[');
                }
                builder.Append(iterator.Next());
            }
            return (builder.ToString() + "]");
        }
    }
}

