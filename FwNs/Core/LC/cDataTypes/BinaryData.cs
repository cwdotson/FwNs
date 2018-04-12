namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.IO;

    public sealed class BinaryData : IBlobData, ILobData
    {
        private static BinaryData SingleBitZero = new BinaryData(new byte[1], 1L);
        private static BinaryData SingleBitOne;
        private readonly bool _isBits;
        private long _bitLength;
        private byte[] _data;
        private long _id;

        static BinaryData()
        {
            byte[] data = new byte[] { 1 };
            SingleBitOne = new BinaryData(data, 1L);
        }

        public BinaryData(byte[] data, bool clone)
        {
            if (clone)
            {
                data = (byte[]) data.Clone();
            }
            this._data = data;
            this._bitLength = data.Length * 8;
        }

        public BinaryData(byte[] data, long bitLength)
        {
            this._data = data;
            this._bitLength = bitLength;
            this._isBits = true;
        }

        public BinaryData(long length, Stream stream)
        {
            this._data = new byte[(int) length];
            this._bitLength = this._data.Length * 8;
            try
            {
                stream.Read(this._data, 0, (int) length);
            }
            catch (IOException exception)
            {
                throw Error.GetError(0x1d3, exception);
            }
        }

        public BinaryData(ISessionInterface session, IBlobData b1, IBlobData b2)
        {
            long num = b1.Length(session) + b2.Length(session);
            if (num > 0x7fffffffL)
            {
                throw Error.GetError(0xd49);
            }
            this._data = new byte[(int) num];
            Array.Copy(b1.GetBytes(), 0, this._data, 0, (int) b1.Length(session));
            Array.Copy(b2.GetBytes(), 0, this._data, (int) b1.Length(session), (int) b2.Length(session));
            this._bitLength = (b1.Length(session) + b2.Length(session)) * 8L;
        }

        public long BitLength(ISessionInterface session)
        {
            return this._bitLength;
        }

        public IBlobData Duplicate(ISessionInterface session)
        {
            return new BinaryData(this._data, true);
        }

        public void Free()
        {
        }

        public static BinaryData GetBitData(byte[] data, long bitLength)
        {
            if (bitLength != 1L)
            {
                return new BinaryData(data, bitLength);
            }
            if (data[0] != 0)
            {
                return SingleBitOne;
            }
            return SingleBitZero;
        }

        public IBlobData GetBlob(ISessionInterface session, long pos, long length)
        {
            throw Error.RuntimeError(0xc9, "BinaryData");
        }

        public byte[] GetBytes()
        {
            return this._data;
        }

        public byte[] GetBytes(ISessionInterface session, long pos, int length)
        {
            if (!IsInLimits((long) this._data.Length, pos, (long) length))
            {
                throw new IndexOutOfRangeException();
            }
            byte[] destinationArray = new byte[length];
            Array.Copy(this._data, (int) pos, destinationArray, 0, length);
            return destinationArray;
        }

        public byte[] GetData()
        {
            return this._data;
        }

        public long GetId()
        {
            return this._id;
        }

        public int GetStreamBlockSize()
        {
            return 0x80000;
        }

        public bool IsBinary()
        {
            return true;
        }

        public bool IsBits()
        {
            return this._isBits;
        }

        public bool IsClosed()
        {
            return false;
        }

        public static bool IsInLimits(long fullLength, long pos, long len)
        {
            return (((pos >= 0L) && (len >= 0L)) && ((pos + len) <= fullLength));
        }

        public long Length(ISessionInterface session)
        {
            return (long) this._data.Length;
        }

        public long NonZeroLength(ISessionInterface session)
        {
            return (long) this._data.Length;
        }

        public long Position(ISessionInterface session, byte[] pattern, long start)
        {
            if (pattern.Length > this._data.Length)
            {
                return -1L;
            }
            if (start >= this._data.Length)
            {
                return -1L;
            }
            return ArrayUtil.Find(this._data, (int) start, this._data.Length, pattern);
        }

        public long Position(ISessionInterface session, IBlobData pattern, long start)
        {
            if (pattern.Length(session) > this._data.Length)
            {
                return -1L;
            }
            byte[] bytes = pattern.GetBytes();
            return this.Position(session, bytes, start);
        }

        public int SetBytes(ISessionInterface session, long pos, byte[] bytes)
        {
            this.SetBytes(session, pos, bytes, 0, bytes.Length);
            return bytes.Length;
        }

        public int SetBytes(ISessionInterface session, long pos, byte[] bytes, int offset, int length)
        {
            if (!IsInLimits((long) this._data.Length, pos, 0L))
            {
                throw new IndexOutOfRangeException();
            }
            if (!IsInLimits((long) this._data.Length, pos, (long) length))
            {
                this._data = ArrayUtil.ResizeArray<byte>(this._data, ((int) pos) + length);
            }
            Array.Copy(bytes, offset, this._data, (int) pos, length);
            this._bitLength = this._data.Length * 8;
            return length;
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
            if (this._data.Length > len)
            {
                this._data = ArrayUtil.ResizeArray<byte>(this._data, (int) len);
                this._bitLength = this._data.Length * 8;
            }
        }
    }
}

