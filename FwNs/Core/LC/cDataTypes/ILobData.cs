namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using System;

    public interface ILobData
    {
        long GetId();
        bool IsBinary();
        long Length(ISessionInterface session);
        void SetId(long id);
        void SetSession(ISessionInterface session);
    }
}

