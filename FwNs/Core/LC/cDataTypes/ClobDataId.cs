namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cResults;
    using System;

    public class ClobDataId : IClobData, ILobData
    {
        private long _id;

        public ClobDataId(long id)
        {
            this._id = id;
        }

        public char[] GetChars(ISessionInterface session, long position, int length)
        {
            if (length == 0)
            {
                return new char[0];
            }
            Result r = ResultLob.NewLobGetCharsRequest(this._id, position, length);
            Result result2 = session.Execute(r);
            if (result2.IsError())
            {
                throw result2.GetException();
            }
            return ((ResultLob) result2).GetCharArray();
        }

        public IClobData GetClob(ISessionInterface session, long position, long length)
        {
            ResultLob r = ResultLob.NewLobGetRequest(this._id, position, length);
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw result.GetException();
            }
            return new ClobDataId(((ResultLob) result).GetLobId());
        }

        public long GetId()
        {
            return this._id;
        }

        public long GetRightTrimSize(ISessionInterface session)
        {
            return 0L;
        }

        public string GetSubString(ISessionInterface session, long pos, int length)
        {
            return new string(this.GetChars(session, pos, length));
        }

        public bool IsBinary()
        {
            return false;
        }

        private static bool IsInLimits(long fullLength, long pos, long len)
        {
            return (((pos >= 0L) && (len >= 0L)) && ((pos + len) <= fullLength));
        }

        public long Length(ISessionInterface session)
        {
            ResultLob r = ResultLob.NewLobGetLengthRequest(this._id);
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw result.GetException();
            }
            return ((ResultLob) result).GetBlockLength();
        }

        public long NonSpaceLength(ISessionInterface session)
        {
            return 0L;
        }

        public long Position(ISessionInterface session, IClobData searchstr, long start)
        {
            return 0L;
        }

        public long Position(ISessionInterface session, string searchstr, long start)
        {
            ResultLob r = ResultLob.NewLobGetCharPatternPositionRequest(this._id, searchstr.ToCharArray(), start);
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw result.GetException();
            }
            return ((ResultLob) result).GetOffset();
        }

        public int SetChars(ISessionInterface session, long pos, char[] chars, int offset, int len)
        {
            if (!IsInLimits((long) chars.Length, (long) offset, (long) len))
            {
                throw Error.GetError(0xd49);
            }
            char[] destinationArray = new char[len];
            Array.Copy(chars, offset, destinationArray, 0, len);
            ResultLob r = ResultLob.NewLobSetCharsRequest(this._id, pos, chars);
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw result.GetException();
            }
            return len;
        }

        public void SetId(long id)
        {
            this._id = id;
        }

        public void SetSession(ISessionInterface session)
        {
        }

        public int SetString(ISessionInterface session, long pos, string str)
        {
            ResultLob r = ResultLob.NewLobSetCharsRequest(this._id, pos, str.ToCharArray());
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw result.GetException();
            }
            return str.Length;
        }

        public int SetString(ISessionInterface session, long pos, string str, int offset, int len)
        {
            if (!IsInLimits((long) str.Length, (long) offset, (long) len))
            {
                throw Error.GetError(0xd49);
            }
            ResultLob r = ResultLob.NewLobSetCharsRequest(this._id, pos, str.Substring(offset, len).ToCharArray());
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw result.GetException();
            }
            return str.Length;
        }

        public void Truncate(ISessionInterface session, long len)
        {
            ResultLob r = ResultLob.NewLobTruncateRequest(this._id, len);
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw result.GetException();
            }
        }
    }
}

