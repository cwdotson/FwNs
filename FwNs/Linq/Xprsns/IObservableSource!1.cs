namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public interface IObservableSource<T> : IEnumerable<T>, IEnumerable
    {
        event EventHandler<SourceChangeEventArgs<T>> Changed;

        void EnableItemOrdinals();

        bool SupportsItemOrdinals { get; }

        bool IsDeletedStateAvailable { get; }

        Func<T> CreateNew { get; }
    }
}

