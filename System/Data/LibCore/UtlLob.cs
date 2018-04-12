namespace System.Data.LibCore
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using System;
    using System.IO;

    public class UtlLob : Stream
    {
        private readonly ILobData _lobData;
        private readonly ISessionInterface _session;
        private long _pos;

        public UtlLob(ILobData lobData)
        {
            this._lobData = lobData;
            this._session = null;
        }

        public UtlLob(ISessionInterface sessionInterface, ILobData lobData)
        {
            this._session = sessionInterface;
            this._lobData = lobData;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (((offset > buffer.Length) || (offset < 0)) || (count < 0))
            {
                throw new ArgumentException();
            }
            if ((offset + count) > buffer.Length)
            {
                count = buffer.Length - offset;
            }
            if (!this._lobData.IsBinary())
            {
                throw new InvalidOperationException();
            }
            byte[] sourceArray = ((IBlobData) this._lobData).GetBytes(this._session, this._pos, count);
            Array.Copy(sourceArray, 0, buffer, offset, sourceArray.Length);
            this._pos += sourceArray.Length;
            return sourceArray.Length;
        }

        public int Read(char[] buffer, int offset, int count)
        {
            if (((offset > buffer.Length) || (offset < 0)) || (count < 0))
            {
                throw new ArgumentException();
            }
            if ((offset + count) > buffer.Length)
            {
                count = buffer.Length - offset;
            }
            if (this._lobData.IsBinary())
            {
                throw new InvalidOperationException();
            }
            char[] sourceArray = ((IClobData) this._lobData).GetChars(this._session, this._pos, count);
            Array.Copy(sourceArray, 0, buffer, offset, sourceArray.Length);
            this._pos += sourceArray.Length;
            return sourceArray.Length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            if (this._lobData.IsBinary())
            {
                ((IBlobData) this._lobData).Truncate(this._session, value);
            }
            else
            {
                ((IClobData) this._lobData).Truncate(this._session, value);
            }
        }

        public override string ToString()
        {
            if (this._lobData.IsBinary())
            {
                return BitConverter.ToString((byte[]) this.Value).Replace("-", "");
            }
            return this.Value.ToString();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (((offset < 0) || (offset > buffer.Length)) || (count < 0))
            {
                throw new ArgumentException();
            }
            if ((offset + count) > buffer.Length)
            {
                count = buffer.Length - offset;
            }
            byte[] destinationArray = new byte[count];
            Array.Copy(buffer, offset, destinationArray, 0, count);
            if (!this._lobData.IsBinary())
            {
                throw new InvalidOperationException();
            }
            ((IBlobData) this._lobData).SetBytes(this._session, this._pos, destinationArray, 0, count);
            this._pos += count;
        }

        public void Write(char[] buffer, int offset, int count)
        {
            if (((offset < 0) || (offset > buffer.Length)) || (count < 0))
            {
                throw new ArgumentException();
            }
            if ((offset + count) > buffer.Length)
            {
                count = buffer.Length - offset;
            }
            char[] destinationArray = new char[count];
            Array.Copy(buffer, offset, destinationArray, 0, count);
            if (this._lobData.IsBinary())
            {
                throw new InvalidOperationException();
            }
            ((IClobData) this._lobData).SetChars(this._session, this._pos, destinationArray, 0, count);
            this._pos += count;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return this._lobData.Length(this._session);
            }
        }

        public override long Position
        {
            get
            {
                return this._pos;
            }
            set
            {
                long num = this._lobData.Length(this._session);
                if ((value < 0L) || (value > num))
                {
                    throw new ArgumentException();
                }
                this._pos = value;
            }
        }

        public object Value
        {
            get
            {
                if (this._lobData == null)
                {
                    return DBNull.Value;
                }
                if (this._lobData.IsBinary())
                {
                    int num = (int) this._lobData.Length(this._session);
                    return ((IBlobData) this._lobData).GetBytes(this._session, 0L, num);
                }
                int length = (int) this._lobData.Length(this._session);
                return new string(((IClobData) this._lobData).GetChars(this._session, 0L, length));
            }
        }
    }
}

