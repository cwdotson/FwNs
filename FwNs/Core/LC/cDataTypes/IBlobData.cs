namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using System;

    public interface IBlobData : ILobData
    {
        long BitLength(ISessionInterface session);
        IBlobData Duplicate(ISessionInterface session);
        void Free();
        IBlobData GetBlob(ISessionInterface session, long pos, long length);
        byte[] GetBytes();
        byte[] GetBytes(ISessionInterface session, long pos, int length);
        int GetStreamBlockSize();
        bool IsBits();
        bool IsClosed();
        long NonZeroLength(ISessionInterface session);
        long Position(ISessionInterface session, byte[] pattern, long start);
        long Position(ISessionInterface session, IBlobData pattern, long start);
        int SetBytes(ISessionInterface session, long pos, byte[] bytes);
        int SetBytes(ISessionInterface session, long pos, byte[] bytes, int offset, int len);
        void Truncate(ISessionInterface session, long len);
    }
}

