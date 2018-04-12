namespace FwNs.Linq.Xprsns
{
    using System.Collections;
    using System.Collections.Generic;

    public interface IIndexedSource<T> : IEnumerable<T>, IEnumerable
    {
        ScannerCollection<T> Indexes { get; }
    }
}

