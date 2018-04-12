namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cParsing;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public sealed class RowInputTextLog : RowInputBase, IRowInputInterface
    {
        private readonly Scanner _scanner;
        private bool _noSeparators;
        private string _schemaName;
        private int _statementType;
        private string _tableName;
        private object _value;

        public RowInputTextLog() : base(new byte[0])
        {
            this._scanner = new Scanner(CultureInfo.InvariantCulture);
        }

        public string GetSchemaName()
        {
            return this._schemaName;
        }

        public int GetStatementType()
        {
            return this._statementType;
        }

        public string GetTableName()
        {
            return this._tableName;
        }

        protected override object[] ReadArray(SqlType type)
        {
            type = type.CollectionBaseType();
            this.ReadFieldPrefix();
            this._scanner.ScanNext();
            string str = this._scanner.GetString();
            this._value = null;
            if (str.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            if (!str.Equals("ARRAY", StringComparison.OrdinalIgnoreCase))
            {
                throw Error.GetError(0x15d0);
            }
            this._scanner.ScanNext();
            if (!this._scanner.GetString().Equals("[", StringComparison.OrdinalIgnoreCase))
            {
                throw Error.GetError(0x15d0);
            }
            List<object> list = new List<object>();
            this._noSeparators = true;
            for (int i = 0; !this._scanner.ScanSpecialIdentifier("]"); i++)
            {
                if ((i > 0) && !this._scanner.ScanSpecialIdentifier(","))
                {
                    throw Error.GetError(0x15d0);
                }
                object item = base.ReadData(type);
                list.Add(item);
            }
            this._noSeparators = false;
            return list.ToArray();
        }

        protected override object ReadBigint()
        {
            this.ReadNumberField(SqlType.SqlBigint);
            if (this._value == null)
            {
                return null;
            }
            return Convert.ToInt64(this._value, CultureInfo.InvariantCulture);
        }

        protected override BinaryData ReadBinary()
        {
            this.ReadFieldPrefix();
            if (this._scanner.ScanNull())
            {
                return null;
            }
            this._scanner.ScanBinaryStringWithQuote();
            if (this._scanner.GetTokenType() == 0x2f4)
            {
                throw Error.GetError(0x15d3);
            }
            this._value = this._scanner.GetValue();
            return (BinaryData) this._value;
        }

        protected override IBlobData ReadBlob()
        {
            this.ReadNumberField(SqlType.SqlBigint);
            if (this._value == null)
            {
                return null;
            }
            return new BlobDataId(Convert.ToInt64(this._value, CultureInfo.InvariantCulture));
        }

        protected override object ReadBoolean()
        {
            this.ReadFieldPrefix();
            this._scanner.ScanNext();
            string str = this._scanner.GetString();
            this._value = null;
            if (str.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
            {
                this._value = true;
            }
            else if (str.Equals("FALSE", StringComparison.OrdinalIgnoreCase))
            {
                this._value = false;
            }
            return this._value;
        }

        protected override string ReadChar(SqlType type)
        {
            this.ReadField();
            return (string) this._value;
        }

        protected override IClobData ReadClob()
        {
            this.ReadNumberField(SqlType.SqlBigint);
            if (this._value == null)
            {
                return null;
            }
            return new ClobDataId(Convert.ToInt64(this._value, CultureInfo.InvariantCulture));
        }

        protected override TimestampData ReadDate(SqlType type)
        {
            this.ReadField();
            if (this._value == null)
            {
                return null;
            }
            return this._scanner.NewDate((string) this._value);
        }

        protected override IntervalSecondData ReadDaySecondInterval(SqlType type)
        {
            this.ReadField();
            if (this._value == null)
            {
                return null;
            }
            return (IntervalSecondData) this._scanner.NewInterval((string) this._value, (IntervalType) type);
        }

        protected override object ReadDecimal(SqlType type)
        {
            this.ReadNumberField(type);
            if (this._value == null)
            {
                return null;
            }
            decimal a = Convert.ToDecimal(this._value, CultureInfo.InvariantCulture);
            return type.ConvertToTypeLimits(null, a);
        }

        private void ReadField()
        {
            this.ReadFieldPrefix();
            this._scanner.ScanNext();
            this._value = this._scanner.GetValue();
        }

        private void ReadFieldPrefix()
        {
            if (!this._noSeparators)
            {
                this._scanner.ScanNext();
                if (this._statementType == 2)
                {
                    this._scanner.ScanNext();
                    this._scanner.ScanNext();
                }
            }
        }

        protected override object ReadGuid()
        {
            string g = this.ReadString();
            if (g == null)
            {
                return null;
            }
            return new Guid(g);
        }

        public override int ReadInt()
        {
            throw Error.RuntimeError(0xc9, "");
        }

        protected override object ReadInteger()
        {
            this.ReadNumberField(SqlType.SqlInteger);
            if (this._value == null)
            {
                return null;
            }
            return Convert.ToInt32(this._value, CultureInfo.InvariantCulture);
        }

        public override long ReadLong()
        {
            throw Error.RuntimeError(0xc9, "");
        }

        protected override bool ReadNull()
        {
            return false;
        }

        private void ReadNumberField(SqlType type)
        {
            this.ReadFieldPrefix();
            this._scanner.ScanNext();
            bool flag1 = this._scanner.GetTokenType() == 0x2b5;
            if (flag1)
            {
                this._scanner.ScanNext();
            }
            this._value = this._scanner.GetValue();
            if (flag1)
            {
                try
                {
                    this._value = this._scanner.GetDataType().Negate(this._value);
                }
                catch (CoreException)
                {
                }
            }
        }

        protected override object ReadOther()
        {
            this.ReadFieldPrefix();
            if (this._scanner.ScanNull())
            {
                return null;
            }
            this._scanner.ScanBinaryStringWithQuote();
            if (this._scanner.GetTokenType() == 0x2f4)
            {
                throw Error.GetError(0x15d3);
            }
            this._value = this._scanner.GetValue();
            return new OtherData(((BinaryData) this._value).GetBytes());
        }

        protected override object ReadReal()
        {
            this.ReadNumberField(SqlType.SqlDouble);
            if (this._value == null)
            {
                return null;
            }
            return Convert.ToDouble(this._value, CultureInfo.InvariantCulture);
        }

        public override int ReadShort()
        {
            throw Error.RuntimeError(0xc9, "");
        }

        protected override object ReadSmallint()
        {
            this.ReadNumberField(SqlType.SqlSmallint);
            if (this._value == null)
            {
                return null;
            }
            return Convert.ToInt32(this._value, CultureInfo.InvariantCulture);
        }

        public override string ReadString()
        {
            this.ReadField();
            return (string) this._value;
        }

        protected override TimeData ReadTime(SqlType type)
        {
            this.ReadField();
            if (this._value == null)
            {
                return null;
            }
            return this._scanner.NewTime((string) this._value);
        }

        protected override TimestampData ReadTimestamp(SqlType type)
        {
            this.ReadField();
            if (this._value == null)
            {
                return null;
            }
            return this._scanner.NewTimestamp((string) this._value);
        }

        public override int ReadType()
        {
            return 0;
        }

        protected override IntervalMonthData ReadYearMonthInterval(SqlType type)
        {
            this.ReadField();
            if (this._value == null)
            {
                return null;
            }
            return (IntervalMonthData) this._scanner.NewInterval((string) this._value, (IntervalType) type);
        }

        public void SetSource(string text)
        {
            this._scanner.Reset(text);
            this._statementType = 1;
            this._scanner.ScanNext();
            switch (this._scanner.GetString())
            {
                case "INSERT":
                    this._statementType = 3;
                    this._scanner.ScanNext();
                    this._scanner.ScanNext();
                    this._tableName = this._scanner.GetString();
                    this._scanner.ScanNext();
                    return;

                case "DELETE":
                    this._statementType = 2;
                    this._scanner.ScanNext();
                    this._scanner.ScanNext();
                    this._tableName = this._scanner.GetString();
                    return;

                case "COMMIT":
                    this._statementType = 4;
                    return;

                case "SET":
                    this._scanner.ScanNext();
                    if ("SCHEMA".Equals(this._scanner.GetString()))
                    {
                        this._scanner.ScanNext();
                        this._schemaName = this._scanner.GetString();
                        this._statementType = 6;
                    }
                    break;
            }
        }
    }
}

