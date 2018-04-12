namespace FwNs.Core.LC.cLib.IO
{
    using FwNs.Core.LC.cErrors;
    using System;
    using System.IO;
    using System.Text;

    public class ByteArrayOutputStream : Stream
    {
        public byte[] Buffer;
        public int Count;

        public ByteArrayOutputStream() : this(0x80)
        {
        }

        public ByteArrayOutputStream(int size)
        {
            if (size < 0x80)
            {
                size = 0x80;
            }
            this.Buffer = new byte[size];
        }

        public ByteArrayOutputStream(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public virtual void EnsureRoom(int extra)
        {
            int num = this.Count + extra;
            int length = this.Buffer.Length;
            if (num > length)
            {
                while (num > length)
                {
                    length *= 2;
                }
                byte[] destinationArray = new byte[length];
                Array.Copy(this.Buffer, 0, destinationArray, 0, this.Count);
                this.Buffer = destinationArray;
            }
        }

        public override void Flush()
        {
        }

        public byte[] GetBuffer()
        {
            return this.Buffer;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw Error.GetError(0x5dc);
        }

        public virtual void Reset()
        {
            this.Count = 0;
        }

        public virtual void Reset(int newSize)
        {
            this.Count = 0;
            if (newSize > this.Buffer.Length)
            {
                this.Buffer = new byte[newSize];
            }
        }

        public void Reset(byte[] buffer)
        {
            this.Count = 0;
            this.Buffer = buffer;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw Error.GetError(0x5dc);
        }

        public virtual void SetBuffer(byte[] buffer)
        {
            this.Count = 0;
            this.Buffer = buffer;
        }

        public override void SetLength(long value)
        {
            this.Count = (int) value;
        }

        public int Size()
        {
            return this.Count;
        }

        public byte[] ToByteArray()
        {
            byte[] destinationArray = new byte[this.Count];
            Array.Copy(this.Buffer, 0, destinationArray, 0, this.Count);
            return destinationArray;
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.Buffer, 0, this.Count);
        }

        public string ToString(string enc)
        {
            return Encoding.GetEncoding(enc).GetString(this.Buffer, 0, this.Count);
        }

        public void Write(int b)
        {
            this.EnsureRoom(1);
            int count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = (byte) b;
        }

        public void Write(byte[] b)
        {
            this.Write(b, 0, b.Length);
        }

        public override void Write(byte[] b, int off, int len)
        {
            this.EnsureRoom(len);
            Array.Copy(b, off, this.Buffer, this.Count, len);
            this.Count += len;
        }

        public void Write(char[] c, int off, int len)
        {
            this.EnsureRoom(len * 2);
            for (int i = off; i < len; i++)
            {
                int num2 = c[i];
                int count = this.Count;
                this.Count = count + 1;
                this.Buffer[count] = (byte) (num2 >> 8);
                count = this.Count;
                this.Count = count + 1;
                this.Buffer[count] = (byte) num2;
            }
        }

        public void WriteBool(bool v)
        {
            this.EnsureRoom(1);
            int count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = Convert.ToByte(v ? 1 : 0);
        }

        public void WriteByte(int v)
        {
            this.EnsureRoom(1);
            int count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = (byte) v;
        }

        public void WriteBytes(string s)
        {
            int length = s.Length;
            this.EnsureRoom(length);
            for (int i = 0; i < length; i++)
            {
                int count = this.Count;
                this.Count = count + 1;
                this.Buffer[count] = (byte) s[i];
            }
        }

        public void WriteDecimal(decimal v)
        {
            int[] bits = decimal.GetBits(v);
            this.WriteInt(bits[0]);
            this.WriteInt(bits[1]);
            this.WriteInt(bits[2]);
            this.WriteInt(bits[3]);
        }

        public void WriteDouble(double v)
        {
            this.WriteLong(BitConverter.DoubleToInt64Bits(v));
        }

        public void WriteFloat(float v)
        {
            this.WriteLong(BitConverter.DoubleToInt64Bits((double) v));
        }

        public virtual void WriteInt(int v)
        {
            this.EnsureRoom(4);
            byte[] bytes = BitConverter.GetBytes(v);
            int count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[0];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[1];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[2];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[3];
        }

        public virtual void WriteLong(long v)
        {
            this.EnsureRoom(8);
            byte[] bytes = BitConverter.GetBytes(v);
            int count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[0];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[1];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[2];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[3];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[4];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[5];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[6];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[7];
        }

        public void WriteNoCheck(int b)
        {
            int count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = (byte) b;
        }

        public virtual void WriteShort(int v)
        {
            this.EnsureRoom(2);
            byte[] bytes = BitConverter.GetBytes((short) v);
            int count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[0];
            count = this.Count;
            this.Count = count + 1;
            this.Buffer[count] = bytes[1];
        }

        public void WriteTo(Stream outs)
        {
            outs.Write(this.Buffer, 0, this.Count);
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
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
                return (long) this.Count;
            }
        }

        public override bool CanRead
        {
            get
            {
                return false;
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
                return true;
            }
        }
    }
}

