namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRows;
    using System;

    public sealed class RowOutputTextLog : RowOutputBase
    {
        public const int ModeDelete = 1;
        public const int ModeInsert = 0;
        private static readonly byte[] BytesNull = Encoding.UTF8.GetBytes("NULL");
        private static readonly byte[] BytesTrue = Encoding.UTF8.GetBytes("TRUE");
        private static readonly byte[] BytesFalse = Encoding.UTF8.GetBytes("FALSE");
        private static readonly byte[] BytesAnd = Encoding.UTF8.GetBytes(" AND ");
        private static readonly byte[] BytesIs = Encoding.UTF8.GetBytes(" IS ");
        private static readonly byte[] BytesArray = Encoding.UTF8.GetBytes("ARRAY[");
        private bool _isWritten;
        private int _logMode;
        private bool _noSeparators;

        public override IRowOutputInterface Duplicate()
        {
            throw Error.RuntimeError(0xc9, "RowOutputText");
        }

        public override int GetSize(Row row)
        {
            return 0;
        }

        public override int GetStorageSize(int size)
        {
            return size;
        }

        public override void Reset()
        {
            base.Reset();
            this._isWritten = false;
        }

        public void SetMode(int mode)
        {
            this._logMode = mode;
        }

        protected override void WriteArray(object[] o, SqlType type)
        {
            type = type.CollectionBaseType();
            this._noSeparators = true;
            base.Write(BytesArray);
            for (int i = 0; i < o.Length; i++)
            {
                if (i > 0)
                {
                    base.Write(0x2c);
                }
                base.WriteData(type, o[i]);
            }
            base.Write(0x5d);
            this._noSeparators = false;
        }

        protected override void WriteBigint(object o)
        {
            base.WriteBytes(o.ToString());
        }

        protected override void WriteBinary(BinaryData o)
        {
            this.EnsureRoom((int) ((o.Length(null) * 2L) + 2L));
            base.Write(0x27);
            StringConverter.WriteHexBytes(base.GetBuffer(), base.Count, o.GetBytes());
            base.Count += (int) (o.Length(null) * 2L);
            base.Write(0x27);
        }

        protected override void WriteBlob(IBlobData o, SqlType type)
        {
            this.WriteString(o.GetId().ToString());
        }

        protected override void WriteBoolean(object o)
        {
            base.Write(((bool) o) ? BytesTrue : BytesFalse);
        }

        protected override void WriteChar(string s, SqlType t)
        {
            base.Write(0x27);
            StringConverter.StringToUnicodeBytes(this, s, true);
            base.Write(0x27);
        }

        protected override void WriteClob(IClobData o, SqlType type)
        {
            this.WriteString(o.GetId().ToString());
        }

        protected override void WriteDate(TimestampData o, SqlType type)
        {
            base.Write(0x27);
            base.WriteBytes(type.ConvertToString(o));
            base.Write(0x27);
        }

        protected override void WriteDaySecondInterval(IntervalSecondData o, SqlType type)
        {
            base.Write(0x27);
            base.WriteBytes(type.ConvertToString(o));
            base.Write(0x27);
        }

        protected override void WriteDecimal(object o, SqlType type)
        {
            base.WriteBytes(type.ConvertToSQLString(o));
        }

        public override void WriteEnd()
        {
        }

        protected override void WriteFieldPrefix()
        {
            if ((!this._noSeparators && (this._logMode == 1)) && this._isWritten)
            {
                base.Write(BytesAnd);
            }
        }

        protected override void WriteFieldType(SqlType type)
        {
            if (!this._noSeparators)
            {
                if (this._logMode == 1)
                {
                    base.Write(0x3d);
                }
                else if (this._isWritten)
                {
                    base.Write(0x2c);
                }
                this._isWritten = true;
            }
        }

        protected override void WriteGuid(object o)
        {
            base.Write(0x27);
            StringConverter.StringToUnicodeBytes(this, o.ToString(), true);
            base.Write(0x27);
        }

        public override void WriteInt(int i)
        {
            base.WriteBytes(i.ToString());
        }

        public override void WriteIntData(int i, int position)
        {
        }

        protected override void WriteInteger(object o)
        {
            base.WriteBytes(o.ToString());
        }

        public override void WriteLong(long value)
        {
            base.WriteBytes(value.ToString());
        }

        protected override void WriteNull(SqlType type)
        {
            if (!this._noSeparators)
            {
                if (this._logMode == 1)
                {
                    base.Write(BytesIs);
                }
                else if (this._isWritten)
                {
                    base.Write(0x2c);
                }
                this._isWritten = true;
            }
            base.Write(BytesNull);
        }

        protected override void WriteOther(OtherData o)
        {
            this.EnsureRoom((o.GetBytesLength() * 2) + 2);
            base.Write(0x27);
            StringConverter.WriteHexBytes(base.GetBuffer(), base.Count, o.GetBytes());
            base.Count += o.GetBytesLength() * 2;
            base.Write(0x27);
        }

        protected override void WriteReal(object o)
        {
            base.WriteBytes(SqlType.SqlDouble.ConvertToSQLString(o));
        }

        public override void WriteShort(int i)
        {
            base.WriteBytes(i.ToString());
        }

        public override void WriteSize(int size)
        {
        }

        protected override void WriteSmallint(object o)
        {
            base.WriteBytes(o.ToString());
        }

        public override void WriteString(string value)
        {
            StringConverter.StringToUnicodeBytes(this, value, false);
        }

        protected override void WriteTime(TimeData o, SqlType type)
        {
            base.Write(0x27);
            base.WriteBytes(type.ConvertToString(o));
            base.Write(0x27);
        }

        protected override void WriteTimestamp(TimestampData o, SqlType type)
        {
            base.Write(0x27);
            base.WriteBytes(type.ConvertToString(o));
            base.Write(0x27);
        }

        public override void WriteType(int type)
        {
        }

        protected override void WriteYearMonthInterval(IntervalMonthData o, SqlType type)
        {
            base.Write(0x27);
            base.WriteBytes(type.ConvertToString(o));
            base.Write(0x27);
        }
    }
}

