namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib.IO;
    using System;

    public abstract class RowInputBase : ByteArrayInputStream
    {
        private const int NoPos = -1;
        protected int FilePos;
        protected int Size;

        protected RowInputBase() : this(new byte[4])
        {
        }

        protected RowInputBase(byte[] buf) : base(buf)
        {
            this.FilePos = -1;
            this.Size = buf.Length;
        }

        public byte[] GetBuffer()
        {
            return base.Buf;
        }

        public int GetPos()
        {
            int filePos = this.FilePos;
            return this.FilePos;
        }

        public int GetSize()
        {
            return this.Size;
        }

        protected abstract object[] ReadArray(SqlType type);
        protected abstract object ReadBigint();
        protected abstract BinaryData ReadBinary();
        protected abstract IBlobData ReadBlob();
        protected abstract object ReadBoolean();
        protected abstract string ReadChar(SqlType type);
        protected abstract IClobData ReadClob();
        public virtual object[] ReadData(SqlType[] colTypes)
        {
            int length = colTypes.Length;
            object[] objArray = new object[length];
            for (int i = 0; i < length; i++)
            {
                SqlType type = colTypes[i];
                objArray[i] = this.ReadData(type);
            }
            return objArray;
        }

        public object ReadData(SqlType type)
        {
            object obj2 = null;
            if (this.ReadNull())
            {
                return null;
            }
            int typeCode = type.TypeCode;
            if (typeCode <= 30)
            {
                if (typeCode > 12)
                {
                    switch (typeCode)
                    {
                        case 0x10:
                            return this.ReadBoolean();

                        case 0x19:
                            return this.ReadBigint();

                        case 30:
                            return this.ReadBlob();
                    }
                }
                else
                {
                    switch (typeCode)
                    {
                        case -11:
                            return this.ReadGuid();

                        case -6:
                        case 5:
                            return this.ReadSmallint();

                        case 0:
                            return obj2;

                        case 1:
                        case 12:
                            goto Label_01C8;

                        case 2:
                        case 3:
                            return this.ReadDecimal(type);

                        case 4:
                            return this.ReadInteger();

                        case 6:
                        case 7:
                        case 8:
                            return this.ReadReal();
                    }
                }
                goto Label_01D2;
            }
            if (typeCode <= 50)
            {
                switch (typeCode)
                {
                    case 40:
                        return this.ReadClob();

                    case 50:
                        return this.ReadArray(type);
                }
                goto Label_01D2;
            }
            if ((typeCode - 60) <= 1)
            {
                return this.ReadBinary();
            }
            switch (typeCode)
            {
                case 0x5b:
                    return this.ReadDate(type);

                case 0x5c:
                case 0x5e:
                    return this.ReadTime(type);

                case 0x5d:
                case 0x5f:
                    return this.ReadTimestamp(type);

                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                    goto Label_01D2;

                case 100:
                    break;

                case 0x65:
                case 0x66:
                case 0x6b:
                    return this.ReadYearMonthInterval(type);

                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                    return this.ReadDaySecondInterval(type);

                default:
                    if (typeCode != 0x457)
                    {
                        goto Label_01D2;
                    }
                    return this.ReadOther();
            }
        Label_01C8:
            return this.ReadChar(type);
        Label_01D2:
            throw Error.RuntimeError(0xc9, "RowInputBase - " + type.GetNameString());
        }

        protected abstract TimestampData ReadDate(SqlType type);
        protected abstract IntervalSecondData ReadDaySecondInterval(SqlType type);
        protected abstract object ReadDecimal(SqlType type);
        protected abstract object ReadGuid();
        protected abstract object ReadInteger();
        public override string ReadLine()
        {
            throw Error.RuntimeError(0xc9, "RowInputBase");
        }

        protected abstract bool ReadNull();
        protected abstract object ReadOther();
        protected abstract object ReadReal();
        protected abstract object ReadSmallint();
        public abstract string ReadString();
        protected abstract TimeData ReadTime(SqlType type);
        protected abstract TimestampData ReadTimestamp(SqlType type);
        public abstract int ReadType();
        protected abstract IntervalMonthData ReadYearMonthInterval(SqlType type);
        public virtual void ResetRow(int filepos, int rowsize)
        {
            base.mark = 0;
            this.Reset();
            if (base.Buf.Length < rowsize)
            {
                base.Buf = new byte[rowsize];
            }
            this.FilePos = filepos;
            base.Count = rowsize;
            this.Size = rowsize;
            base.Pos = 4;
            base.Buf[0] = (byte) ((rowsize >> 0x18) & 0xff);
            base.Buf[1] = (byte) ((rowsize >> 0x10) & 0xff);
            base.Buf[2] = (byte) ((rowsize >> 8) & 0xff);
            base.Buf[3] = (byte) (rowsize & 0xff);
        }

        public override int SkipBytes(int n)
        {
            throw Error.RuntimeError(0xc9, "RowInputBase");
        }
    }
}

