namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cLib.IO;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Globalization;

    public abstract class RowOutputBase : ByteArrayOutputStream, IRowOutputInterface
    {
        public const int CachedRow160 = 0;
        public const int CachedRow170 = 1;
        protected bool SkipSystemId;

        protected RowOutputBase()
        {
        }

        protected RowOutputBase(int initialSize) : base(initialSize)
        {
        }

        protected RowOutputBase(byte[] buffer) : base(buffer)
        {
        }

        public abstract IRowOutputInterface Duplicate();
        public ByteArrayOutputStream GetOutputStream()
        {
            return this;
        }

        public abstract int GetSize(Row row);
        public abstract int GetStorageSize(int size);
        protected abstract void WriteArray(object[] o, SqlType type);
        protected abstract void WriteBigint(object o);
        protected abstract void WriteBinary(BinaryData o);
        protected abstract void WriteBlob(IBlobData o, SqlType type);
        protected abstract void WriteBoolean(object o);
        protected abstract void WriteChar(string s, SqlType t);
        protected abstract void WriteClob(IClobData o, SqlType type);
        public virtual void WriteData(object[] data, SqlType[] types)
        {
            this.WriteData(types.Length, types, data, null, null);
        }

        public void WriteData(SqlType t, object o)
        {
            if (o == null)
            {
                this.WriteNull(t);
                return;
            }
            this.WriteFieldType(t);
            int typeCode = t.TypeCode;
            if (typeCode <= 30)
            {
                if (typeCode > 12)
                {
                    switch (typeCode)
                    {
                        case 0x10:
                            this.WriteBoolean(Convert.ToBoolean(o, CultureInfo.InvariantCulture));
                            return;

                        case 0x19:
                            this.WriteBigint(Convert.ToInt64(o));
                            return;

                        case 30:
                            this.WriteBlob((IBlobData) o, t);
                            return;
                    }
                }
                else
                {
                    switch (typeCode)
                    {
                        case -11:
                            this.WriteGuid((Guid) o);
                            return;

                        case -6:
                        case 5:
                            this.WriteSmallint(Convert.ToInt16(o));
                            return;

                        case 0:
                            return;

                        case 1:
                        case 12:
                            goto Label_0235;

                        case 2:
                        case 3:
                            this.WriteDecimal(Convert.ToDecimal(o), t);
                            return;

                        case 4:
                            this.WriteInteger(Convert.ToInt32(o));
                            return;

                        case 6:
                        case 7:
                        case 8:
                            this.WriteReal(Convert.ToDouble(o));
                            return;
                    }
                }
                goto Label_0243;
            }
            if (typeCode <= 50)
            {
                switch (typeCode)
                {
                    case 40:
                        this.WriteClob((IClobData) o, t);
                        return;

                    case 50:
                        this.WriteArray((object[]) o, t);
                        return;
                }
                goto Label_0243;
            }
            if ((typeCode - 60) <= 1)
            {
                this.WriteBinary((BinaryData) o);
                return;
            }
            switch (typeCode)
            {
                case 0x5b:
                    this.WriteDate((TimestampData) o, t);
                    return;

                case 0x5c:
                case 0x5e:
                    this.WriteTime((TimeData) o, t);
                    return;

                case 0x5d:
                case 0x5f:
                    this.WriteTimestamp((TimestampData) o, t);
                    return;

                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                    goto Label_0243;

                case 100:
                    break;

                case 0x65:
                case 0x66:
                case 0x6b:
                    this.WriteYearMonthInterval((IntervalMonthData) o, t);
                    return;

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
                    this.WriteDaySecondInterval((IntervalSecondData) o, t);
                    return;

                default:
                    if (typeCode != 0x457)
                    {
                        goto Label_0243;
                    }
                    this.WriteOther((OtherData) o);
                    return;
            }
        Label_0235:
            this.WriteChar((string) o, t);
            return;
        Label_0243:
            throw Error.RuntimeError(0xc9, "RowOutputBase - " + t.GetNameString());
        }

        public void WriteData(int l, SqlType[] types, object[] data, HashMappedList<string, ColumnSchema> cols, int[] primaryKeys)
        {
            bool flag = (primaryKeys != null) && (primaryKeys.Length > 0);
            int num = flag ? primaryKeys.Length : l;
            for (int i = 0; i < num; i++)
            {
                int index = flag ? primaryKeys[i] : i;
                object o = data[index];
                SqlType t = types[index];
                if (cols != null)
                {
                    ColumnSchema schema = cols.Get(index);
                    this.WriteFieldPrefix();
                    this.WriteString(schema.GetName().StatementName);
                }
                this.WriteData(t, o);
            }
        }

        protected abstract void WriteDate(TimestampData o, SqlType type);
        protected abstract void WriteDaySecondInterval(IntervalSecondData o, SqlType type);
        protected abstract void WriteDecimal(object o, SqlType type);
        public abstract void WriteEnd();
        protected virtual void WriteFieldPrefix()
        {
        }

        protected abstract void WriteFieldType(SqlType type);
        protected abstract void WriteGuid(object o);
        public abstract void WriteIntData(int i, int position);
        protected abstract void WriteInteger(object o);
        protected abstract void WriteNull(SqlType type);
        protected abstract void WriteOther(OtherData o);
        protected abstract void WriteReal(object o);
        public abstract void WriteSize(int size);
        protected abstract void WriteSmallint(object o);
        public abstract void WriteString(string s);
        protected abstract void WriteTime(TimeData o, SqlType type);
        protected abstract void WriteTimestamp(TimestampData o, SqlType type);
        public abstract void WriteType(int type);
        protected abstract void WriteYearMonthInterval(IntervalMonthData o, SqlType type);
    }
}

