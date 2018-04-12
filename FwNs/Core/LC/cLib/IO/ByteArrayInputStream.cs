namespace FwNs.Core.LC.cLib.IO
{
    using FwNs.Core.LC.cErrors;
    using System;
    using System.IO;

    public class ByteArrayInputStream : Stream
    {
        protected byte[] Buf;
        protected int Count;
        protected int mark;
        protected int Pos;

        public ByteArrayInputStream()
        {
            this.Buf = new byte[0x1000];
            this.Pos = 0;
            this.Count = this.Buf.Length;
        }

        public ByteArrayInputStream(byte[] buf)
        {
            this.Buf = buf;
            this.Pos = 0;
            this.Count = buf.Length;
        }

        public int Available()
        {
            return (this.Count - this.Pos);
        }

        public override void Flush()
        {
        }

        public void Mark(int readAheadLimit)
        {
            this.mark = this.Pos;
        }

        public static bool MarkSupported()
        {
            return true;
        }

        public byte Read()
        {
            if (this.Pos >= this.Count)
            {
                return Convert.ToByte(-1);
            }
            int pos = this.Pos;
            this.Pos = pos + 1;
            return this.Buf[pos];
        }

        public override int Read(byte[] b, int off, int len)
        {
            if (this.Pos >= this.Count)
            {
                return -1;
            }
            if ((this.Pos + len) > this.Count)
            {
                len = this.Count - this.Pos;
            }
            if (len <= 0)
            {
                return 0;
            }
            Array.Copy(this.Buf, this.Pos, b, off, len);
            this.Pos += len;
            return len;
        }

        public virtual bool ReadBool()
        {
            byte num1 = this.Read();
            if (num1 < 0)
            {
                throw new InvalidOperationException();
            }
            return (num1 > 0);
        }

        public override int ReadByte()
        {
            byte num1 = this.Read();
            if (num1 < 0)
            {
                throw new InvalidOperationException();
            }
            return num1;
        }

        public virtual decimal ReadDecimal()
        {
            return new decimal(new int[] { this.ReadInt(), this.ReadInt(), this.ReadInt(), this.ReadInt() });
        }

        public virtual double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(this.ReadLong());
        }

        public virtual float ReadFloat()
        {
            return (float) BitConverter.Int64BitsToDouble(this.ReadLong());
        }

        public virtual void ReadFully(byte[] b)
        {
            this.ReadFully(b, 0, b.Length);
        }

        public virtual void ReadFully(byte[] b, int off, int len)
        {
            int num;
            if (len < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            for (int i = 0; i < len; i += num)
            {
                num = this.Read(b, off + i, len - i);
                if (num < 0)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public virtual int ReadInt()
        {
            if ((this.Count - this.Pos) < 4)
            {
                this.Pos = this.Count;
                throw new InvalidOperationException();
            }
            this.Pos += 4;
            return BitConverter.ToInt32(this.Buf, this.Pos);
        }

        public virtual string ReadLine()
        {
            throw new Exception("not implemented.");
        }

        public virtual long ReadLong()
        {
            if ((this.Count - this.Pos) < 8)
            {
                this.Pos = this.Count;
                throw new InvalidOperationException();
            }
            this.Pos += 8;
            return BitConverter.ToInt64(this.Buf, this.Pos);
        }

        public virtual int ReadShort()
        {
            if ((this.Count - this.Pos) < 2)
            {
                this.Pos = this.Count;
                throw new InvalidOperationException();
            }
            this.Pos += 2;
            return BitConverter.ToInt16(this.Buf, this.Pos);
        }

        public virtual void Reset()
        {
            this.Pos = this.mark;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw Error.GetError(0x5dc);
        }

        public override void SetLength(long value)
        {
            this.Count = (int) value;
        }

        public long Skip(long n)
        {
            if ((this.Pos + n) > this.Count)
            {
                n = this.Count - this.Pos;
            }
            if (n < 0L)
            {
                return 0L;
            }
            this.Pos += (int) n;
            return n;
        }

        public virtual int SkipBytes(int n)
        {
            return (int) this.Skip((long) n);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw Error.GetError(0x5dc);
        }

        public override long Position
        {
            get
            {
                return (long) this.Pos;
            }
            set
            {
                this.Pos = (int) value;
            }
        }

        public override long Length
        {
            get
            {
                return (long) this.Count;
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

