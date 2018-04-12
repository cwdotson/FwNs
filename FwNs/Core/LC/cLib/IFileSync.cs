namespace FwNs.Core.LC.cLib
{
    using System;

    public interface IFileSync : IDisposable
    {
        void Sync();
    }
}

