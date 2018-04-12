namespace FwNs.Linq.Xprsns
{
    using System.Collections.Generic;
    using System.Linq;

    internal interface IKeyComponent<T> : IKeyComponent
    {
        IOrderedEnumerable<T> OrderBy(IEnumerable<T> source);
        IOrderedEnumerable<T> ThenBy(IOrderedEnumerable<T> source);
    }
}

