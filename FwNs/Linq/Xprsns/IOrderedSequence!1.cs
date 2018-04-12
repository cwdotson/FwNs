namespace FwNs.Linq.Xprsns
{
    using System.Collections;
    using System.Collections.Generic;

    internal interface IOrderedSequence<T> : IEnumerable<T>, IEnumerable
    {
        ResultOrder<T> Order { get; }
    }
}

