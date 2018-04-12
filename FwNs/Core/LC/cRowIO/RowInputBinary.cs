namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cLib;
    using System;
    using System.IO;

    public class RowInputBinary : RowInputBase, IRowInputInterface
    {
        private readonly RowOutputBinary _out;

        public RowInputBinary(byte[] buf) : base(buf)
        {
        }

        public RowInputBinary(RowOutputBinary @out) : base(@out.GetBuffer())
        {
            this._out = @out;
        }

        protected override object[] ReadArray(SqlType type)
        {
            type = type.CollectionBaseType();
            int num = this.ReadInt();
            object[] objArray = new object[num];
            for (int i = 0; i < num; i++)
            {
                objArray[i] = base.ReadData(type);
            }
            return objArray;
        }

        protected override object ReadBigint()
        {
            return this.ReadLong();
        }

        protected override BinaryData ReadBinary()
        {
            return new BinaryData(this.ReadByteArray(), false);
        }

        protected override IBlobData ReadBlob()
        {
            return new BlobDataId(base.ReadLong());
        }

        protected override object ReadBoolean()
        {
            return this.ReadBool();
        }

        public byte[] ReadByteArray()
        {
            byte[] b = new byte[this.ReadInt()];
            this.ReadFully(b);
            return b;
        }

        protected override string ReadChar(SqlType type)
        {
            return this.ReadString();
        }

        public char[] ReadCharArray()
        {
            char[] chArray = new char[this.ReadInt()];
            if ((base.Count - base.Pos) < chArray.Length)
            {
                base.Pos = base.Count;
                throw new EndOfStreamException();
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                int pos = base.Pos;
                base.Pos = pos + 1;
                int num2 = base.Buf[pos] & 0xff;
                pos = base.Pos;
                base.Pos = pos + 1;
                int num3 = base.Buf[pos] & 0xff;
                chArray[i] = (char) ((num2 << 8) + num3);
            }
            return chArray;
        }

        protected override IClobData ReadClob()
        {
            return new ClobDataId(base.ReadLong());
        }

        protected override TimestampData ReadDate(SqlType type)
        {
            return new TimestampData(this.ReadLong());
        }

        protected override IntervalSecondData ReadDaySecondInterval(SqlType type)
        {
            return new IntervalSecondData(this.ReadLong(), this.ReadInt(), (IntervalType) type);
        }

        protected override object ReadDecimal(SqlType type)
        {
            return this.ReadDecimal();
        }

        protected override object ReadGuid()
        {
            return new Guid(this.ReadBinary().GetBytes());
        }

        protected override object ReadInteger()
        {
            return this.ReadInt();
        }

        protected override bool ReadNull()
        {
            return (this.ReadByte() == 0);
        }

        protected override object ReadOther()
        {
            return new OtherData(this.ReadByteArray());
        }

        protected override object ReadReal()
        {
            return this.ReadDouble();
        }

        protected override object ReadSmallint()
        {
            return this.ReadShort();
        }

        public override string ReadString()
        {
            int length = this.ReadInt();
            base.Pos += length;
            return StringConverter.ReadUtf(base.Buf, base.Pos, length);
        }

        protected override TimeData ReadTime(SqlType type)
        {
            if (type.TypeCode == 0x5c)
            {
                return new TimeData((long) this.ReadInt(), this.ReadInt(), 0);
            }
            return new TimeData((long) this.ReadInt(), this.ReadInt(), this.ReadInt());
        }

        protected override TimestampData ReadTimestamp(SqlType type)
        {
            if (type.TypeCode == 0x5d)
            {
                return new TimestampData(this.ReadLong(), this.ReadInt());
            }
            return new TimestampData(this.ReadLong(), this.ReadInt(), this.ReadInt());
        }

        public override int ReadType()
        {
            return this.ReadShort();
        }

        protected override IntervalMonthData ReadYearMonthInterval(SqlType type)
        {
            return new IntervalMonthData(this.ReadLong(), (IntervalType) type);
        }

        public void ResetRow(int rowsize)
        {
            if (this._out != null)
            {
                this._out.Reset(rowsize);
                base.Buf = this._out.GetBuffer();
            }
            base.Reset();
        }

        public override void ResetRow(int filepos, int rowsize)
        {
            if (this._out != null)
            {
                this._out.Reset(rowsize);
                base.Buf = this._out.GetBuffer();
            }
            base.ResetRow(filepos, rowsize);
        }
    }
}

