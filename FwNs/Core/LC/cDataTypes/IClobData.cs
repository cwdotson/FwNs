namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using System;

    public interface IClobData : ILobData
    {
        char[] GetChars(ISessionInterface session, long position, int length);
        IClobData GetClob(ISessionInterface session, long pos, long length);
        long GetRightTrimSize(ISessionInterface session);
        string GetSubString(ISessionInterface session, long pos, int length);
        long NonSpaceLength(ISessionInterface session);
        long Position(ISessionInterface session, IClobData searchstr, long start);
        long Position(ISessionInterface session, string searchstr, long start);
        int SetChars(ISessionInterface session, long pos, char[] chars, int offset, int len);
        int SetString(ISessionInterface session, long pos, string str);
        int SetString(ISessionInterface session, long pos, string str, int offset, int len);
        void Truncate(ISessionInterface session, long len);
    }
}

