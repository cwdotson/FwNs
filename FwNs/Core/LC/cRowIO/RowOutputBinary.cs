namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRows;
    using System;

    public class RowOutputBinary : RowOutputBase
    {
        protected const int IntStoreSize = 4;
        private readonly int _mask;
        protected int Scale;
        private int _storageSize;

        public RowOutputBinary(byte[] buffer) : base(buffer)
        {
            this.Scale = 1;
            this._mask = ~(this.Scale - 1);
        }

        public RowOutputBinary(int initialSize, int scale) : base(initialSize)
        {
            this.Scale = scale;
            this._mask = ~(this.Scale - 1);
        }

        public override IRowOutputInterface Duplicate()
        {
            return new RowOutputBinary(0x80, this.Scale);
        }

        public override int GetSize(Row row)
        {
            object[] rowData = row.RowData;
            SqlType[] columnTypes = row.GetTable().GetColumnTypes();
            int dataColumnCount = row.GetTable().GetDataColumnCount();
            return (4 + GetSize(rowData, dataColumnCount, columnTypes));
        }

        private static int GetSize(object o, SqlType type)
        {
            int num = 1;
            if (o == null)
            {
                return num;
            }
            int typeCode = type.TypeCode;
            if (typeCode <= 30)
            {
                switch (typeCode)
                {
                    case 0x10:
                        num++;
                        return num;

                    case 0x19:
                    case 6:
                    case 7:
                    case 8:
                        return (num + 8);

                    case 30:
                        goto Label_01F4;

                    case -11:
                        num += 4;
                        return (num + 0x10);

                    case -6:
                    case 5:
                        return (num + 2);

                    case 0:
                        return num;

                    case 1:
                    case 12:
                        goto Label_01E0;

                    case 2:
                    case 3:
                        return (num + 0x10);

                    case 4:
                        return (num + 4);
                }
                goto Label_01FA;
            }
            if (typeCode <= 50)
            {
                switch (typeCode)
                {
                    case 40:
                        goto Label_01F4;

                    case 50:
                    {
                        num += 4;
                        object[] objArray = (object[]) o;
                        type = type.CollectionBaseType();
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            num += GetSize(objArray[i], type);
                        }
                        return num;
                    }
                }
                goto Label_01FA;
            }
            if ((typeCode - 60) <= 1)
            {
                num += 4;
                return (num + ((int) ((BinaryData) o).Length(null)));
            }
            switch (typeCode)
            {
                case 0x5b:
                    return (num + 8);

                case 0x5c:
                    return (num + 8);

                case 0x5d:
                    return (num + 12);

                case 0x5e:
                    return (num + 12);

                case 0x5f:
                    return (num + 0x10);

                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                    goto Label_01FA;

                case 100:
                    break;

                case 0x65:
                case 0x66:
                case 0x6b:
                    return (num + 8);

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
                    return (num + 12);

                default:
                {
                    if (typeCode != 0x457)
                    {
                        goto Label_01FA;
                    }
                    OtherData data = (OtherData) o;
                    num += 4;
                    return (num + data.GetBytesLength());
                }
            }
        Label_01E0:
            num += 4;
            return (num + StringConverter.GetUtfSize((string) o));
        Label_01F4:
            return (num + 8);
        Label_01FA:
            throw Error.RuntimeError(0xc9, "RowOutputBinary");
        }

        private static int GetSize(object[] data, int l, SqlType[] types)
        {
            int num = 0;
            for (int i = 0; i < l; i++)
            {
                object o = data[i];
                num += GetSize(o, types[i]);
            }
            return num;
        }

        public override int GetStorageSize(int size)
        {
            return (((size + this.Scale) - 1) & this._mask);
        }

        public override void Reset()
        {
            base.Reset();
            this._storageSize = 0;
        }

        public override void Reset(int newSize)
        {
            base.Reset(newSize);
            this._storageSize = 0;
        }

        public override void SetBuffer(byte[] buffer)
        {
            base.Buffer = buffer;
            this.Reset();
        }

        protected override void WriteArray(object[] o, SqlType type)
        {
            type = type.CollectionBaseType();
            this.WriteInt(o.Length);
            for (int i = 0; i < o.Length; i++)
            {
                base.WriteData(type, o[i]);
            }
        }

        protected override void WriteBigint(object o)
        {
            this.WriteLong(Convert.ToInt64(o));
        }

        protected override void WriteBinary(BinaryData o)
        {
            this.WriteByteArray(o.GetBytes());
        }

        protected override void WriteBlob(IBlobData o, SqlType type)
        {
            this.WriteLong(o.GetId());
        }

        protected override void WriteBoolean(object o)
        {
            base.WriteBool((bool) o);
        }

        public void WriteByteArray(byte[] b)
        {
            this.WriteInt(b.Length);
            this.Write(b, 0, b.Length);
        }

        protected override void WriteChar(string s, SqlType t)
        {
            this.WriteString(s);
        }

        public void WriteCharArray(char[] c)
        {
            this.WriteInt(c.Length);
            base.Write(c, 0, c.Length);
        }

        protected override void WriteClob(IClobData o, SqlType type)
        {
            this.WriteLong(o.GetId());
        }

        protected override void WriteDate(TimestampData o, SqlType type)
        {
            this.WriteLong(o.GetSeconds());
        }

        protected override void WriteDaySecondInterval(IntervalSecondData o, SqlType type)
        {
            this.WriteLong(o.Units);
            this.WriteInt(o.Nanos);
        }

        protected override void WriteDecimal(object o, SqlType type)
        {
            base.WriteDecimal((decimal) o);
        }

        public override void WriteEnd()
        {
            if (base.Count > this._storageSize)
            {
                Error.RuntimeError(0xc9, "RowOutputBinary");
            }
            while (base.Count < this._storageSize)
            {
                base.Write(0);
            }
        }

        protected override void WriteFieldType(SqlType type)
        {
            base.Write(1);
        }

        protected override void WriteGuid(object o)
        {
            byte[] data = ((Guid) o).ToByteArray();
            this.WriteBinary(new BinaryData(data, false));
        }

        public override void WriteIntData(int i, int position)
        {
            int count = base.Count;
            base.Count = position;
            this.WriteInt(i);
            if (base.Count < count)
            {
                base.Count = count;
            }
        }

        protected override void WriteInteger(object o)
        {
            this.WriteInt(Convert.ToInt32(o));
        }

        protected override void WriteNull(SqlType type)
        {
            base.Write(0);
        }

        protected override void WriteOther(OtherData o)
        {
            this.WriteByteArray(o.GetBytes());
        }

        protected override void WriteReal(object o)
        {
            base.WriteDouble((double) o);
        }

        public override void WriteSize(int size)
        {
            this._storageSize = size;
            this.WriteInt(size);
        }

        protected override void WriteSmallint(object o)
        {
            this.WriteShort(Convert.ToInt32(o));
        }

        public override void WriteString(string s)
        {
            int count = base.Count;
            this.WriteInt(0);
            if (!string.IsNullOrEmpty(s))
            {
                StringConverter.StringToUtfBytes(s, this);
                this.WriteIntData((base.Count - count) - 4, count);
            }
        }

        protected override void WriteTime(TimeData o, SqlType type)
        {
            this.WriteInt(o.GetSeconds());
            this.WriteInt(o.GetNanos());
            if (type.TypeCode == 0x5e)
            {
                this.WriteInt(o.GetZone());
            }
        }

        protected override void WriteTimestamp(TimestampData o, SqlType type)
        {
            this.WriteLong(o.GetSeconds());
            this.WriteInt(o.GetNanos());
            if (type.TypeCode == 0x5f)
            {
                this.WriteInt(o.GetZone());
            }
        }

        public override void WriteType(int type)
        {
            this.WriteShort(type);
        }

        protected override void WriteYearMonthInterval(IntervalMonthData o, SqlType type)
        {
            this.WriteLong(o.Units);
        }
    }
}

