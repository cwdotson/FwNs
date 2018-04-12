namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

    internal class ResultOrder<T> : IEquatable<ResultOrder<T>>
    {
        public bool Equals(ResultOrder<T> other)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<OrderKey<T>> Keys { get; private set; }
    }
}

