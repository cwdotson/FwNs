namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class BaseList<T>
    {
        protected int ElementCount;

        protected BaseList()
        {
        }

        public abstract bool Add(T o);
        public bool AddAll(FwNs.Core.LC.cLib.ICollection<T> other)
        {
            bool flag = false;
            Iterator<T> iterator = other.GetIterator();
            while (iterator.HasNext())
            {
                flag = true;
                this.Add(iterator.Next());
            }
            return flag;
        }

        public bool AddAll(FwNs.Core.LC.cLib.ICollection<object> other)
        {
            bool flag = false;
            Iterator<object> iterator = other.GetIterator();
            while (iterator.HasNext())
            {
                flag = true;
                this.Add(iterator.Next());
            }
            return flag;
        }

        public bool AddAll(T[] array)
        {
            bool flag = false;
            for (int i = 0; i < array.Length; i++)
            {
                flag = true;
                this.Add(array[i]);
            }
            return flag;
        }

        public bool Contains(T o)
        {
            return (this.IndexOf(o) != -1);
        }

        public abstract T Get(int index);
        public Iterator<T> GetIterator()
        {
            return new BaseListIterator<T, T>((BaseList<T>) this);
        }

        public virtual int IndexOf(T o)
        {
            int index = 0;
            int num2 = this.Size();
            while (index < num2)
            {
                object obj2 = this.Get(index);
                if (obj2 == null)
                {
                    if (o == null)
                    {
                        return index;
                    }
                }
                else if (obj2.Equals(o))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public virtual bool IsEmpty()
        {
            return (this.ElementCount == 0);
        }

        public abstract T Remove(int index);
        public bool Remove(T o)
        {
            int index = this.IndexOf(o);
            if (index == -1)
            {
                return false;
            }
            this.Remove(index);
            return true;
        }

        public abstract int Size();
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x20 + (this.ElementCount * 3));
            builder.Append("List : size=");
            builder.Append(this.ElementCount);
            builder.Append(' ');
            builder.Append('{');
            Iterator<T> iterator = this.GetIterator();
            while (iterator.HasNext())
            {
                builder.Append(iterator.Next());
                if (iterator.HasNext())
                {
                    builder.Append(',');
                    builder.Append(' ');
                }
            }
            builder.Append('}');
            return builder.ToString();
        }

        private class BaseListIterator<TT> : Iterator<TT>
        {
            private readonly BaseList<TT> _o;
            private int _counter;
            private bool _removed;

            public BaseListIterator(BaseList<TT> o)
            {
                this._o = o;
            }

            public bool HasNext()
            {
                return (this._counter < this._o.ElementCount);
            }

            public TT Next()
            {
                if (this._counter >= this._o.ElementCount)
                {
                    throw new KeyNotFoundException();
                }
                this._removed = false;
                this._counter++;
                return this._o.Get(this._counter);
            }

            public void Remove()
            {
                if (this._removed)
                {
                    throw new KeyNotFoundException("Iterator");
                }
                this._removed = true;
                if (this._counter == 0)
                {
                    throw new KeyNotFoundException();
                }
                this._o.Remove((int) (this._counter - 1));
                this._counter--;
            }

            public void SetValue(TT value)
            {
                throw new KeyNotFoundException();
            }
        }
    }
}

