namespace FwNs.Linq.Xprsns
{
    using System;

    internal interface IListener<in T> : IListener
    {
        void StartListening(T item);
        void StopListening(T item);
    }
}

