namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cErrors;
    using System;
    using System.IO;

    public sealed class ReaderInputStream : Stream
    {
        private int _lastChar = -1;
        private long Pos;
        private TextReader Reader;

        public ReaderInputStream(TextReader reader)
        {
            this.Reader = reader;
            this.Pos = 0L;
        }

        public override void Close()
        {
            this.Reader.Close();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int len)
        {
            int num = 0;
            while (num < len)
            {
                int num2 = this.ReadByte();
                if (num2 < 0)
                {
                    return num;
                }
                buffer[offset + num] = (byte) num2;
                num++;
            }
            return num;
        }

        public override int ReadByte()
        {
            if (this._lastChar >= 0)
            {
                this._lastChar = -1;
                this.Pos += 1L;
                return (this._lastChar >> 8);
            }
            char[] buffer = new char[1];
            if (this.Reader.Read(buffer, 0, 1) == 1)
            {
                this._lastChar = buffer[0];
            }
            else
            {
                this._lastChar = -1;
            }
            if (this._lastChar < 0)
            {
                return this._lastChar;
            }
            this.Pos += 1L;
            return (this._lastChar & 0xff);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw Error.GetError(0x5dc);
        }

        public override void SetLength(long value)
        {
            throw Error.GetError(0x5dc);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get
            {
                throw Error.GetError(0x5dc);
            }
            set
            {
                throw Error.GetError(0x5dc);
            }
        }

        public override long Length
        {
            get
            {
                return 0L;
            }
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
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}

