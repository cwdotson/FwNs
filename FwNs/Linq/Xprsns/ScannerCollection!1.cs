namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ScannerCollection<T> : ICollection<IIndexScanner<T>>, IEnumerable<IIndexScanner<T>>, IEnumerable, IDefinitionCollection<T>
    {
        public void Add(IIndexScanner<T> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IIndexScanner<T> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IIndexScanner<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IIndexScanner<T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(IIndexScanner<T> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count { get; private set; }

        public bool IsReadOnly { get; private set; }
    }
}

