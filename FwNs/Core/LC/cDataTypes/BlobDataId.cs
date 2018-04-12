namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cResults;
    using System;

    public class BlobDataId : IBlobData, ILobData
    {
        private long _id;

        public BlobDataId(long id)
        {
            this._id = id;
        }

        public long BitLength(ISessionInterface session)
        {
            return 0L;
        }

        public IBlobData Duplicate(ISessionInterface session)
        {
            return null;
        }

        public void Free()
        {
        }

        public IBlobData GetBlob(ISessionInterface session, long pos, long length)
        {
            ResultLob r = ResultLob.NewLobGetRequest(this._id, pos, length);
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw Error.GetError(result);
            }
            return new BlobDataId(((ResultLob) result).GetLobId());
        }

        public byte[] GetBytes()
        {
            return null;
        }

        public byte[] GetBytes(ISessionInterface session, long pos, int length)
        {
            ResultLob r = ResultLob.NewLobGetBytesRequest(this._id, pos, length);
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw Error.GetError(result);
            }
            return ((ResultLob) result).GetByteArray();
        }

        public long GetId()
        {
            return this._id;
        }

        public int GetStreamBlockSize()
        {
            return 0;
        }

        public bool IsBinary()
        {
            return true;
        }

        public bool IsBits()
        {
            return false;
        }

        public bool IsClosed()
        {
            return false;
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

        public long NonZeroLength(ISessionInterface session)
        {
            return 0L;
        }

        public long Position(ISessionInterface session, IBlobData pattern, long start)
        {
            return 0L;
        }

        public long Position(ISessionInterface session, byte[] pattern, long start)
        {
            ResultLob r = ResultLob.NewLobGetBytePatternPositionRequest(this._id, pattern, start);
            return ((ResultLob) session.Execute(r)).GetOffset();
        }

        public int SetBytes(ISessionInterface session, long pos, byte[] bytes)
        {
            return this.SetBytes(session, pos, bytes, 0, bytes.Length);
        }

        public int SetBytes(ISessionInterface session, long pos, byte[] bytes, int offset, int len)
        {
            if ((offset != 0) || (len != bytes.Length))
            {
                if (!BinaryData.IsInLimits((long) bytes.Length, (long) offset, (long) len))
                {
                    throw new IndexOutOfRangeException();
                }
                byte[] destinationArray = new byte[len];
                Array.Copy(bytes, offset, destinationArray, 0, len);
                bytes = destinationArray;
            }
            ResultLob r = ResultLob.NewLobSetBytesRequest(this._id, pos, bytes);
            Result result = session.Execute(r);
            if (result.IsError())
            {
                throw result.GetException();
            }
            return bytes.Length;
        }

        public void SetId(long id)
        {
            this._id = id;
        }

        public void SetSession(ISessionInterface session)
        {
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

