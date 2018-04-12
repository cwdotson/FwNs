namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using System;

    public interface IScaledRAInterface : IStorage, IDisposable
    {
        bool CanAccess(int length);
        bool CanSeek(long position);
        Database GetDatabase();
    }
}

