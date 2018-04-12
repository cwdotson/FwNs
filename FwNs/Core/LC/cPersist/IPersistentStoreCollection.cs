namespace FwNs.Core.LC.cPersist
{
    using System;

    public interface IPersistentStoreCollection
    {
        IPersistentStore GetStore(object key);
        void SetStore(object key, IPersistentStore store);
    }
}

