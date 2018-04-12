namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ICachingEnumerable<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        void InvalidateCache();
    }
}

