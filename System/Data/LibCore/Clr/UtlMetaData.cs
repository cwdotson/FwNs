namespace System.Data.LibCore.Clr
{
    using System;

    public sealed class UtlMetaData
    {
        private readonly UtlSqlType _utlDbType;
        private readonly string _name;
        private readonly int _precision;
        private readonly byte _scale;

        public UtlMetaData(string name, UtlSqlType dbType)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            if ((((dbType != UtlSqlType.TinyInt) && (dbType != UtlSqlType.SmallInt)) && ((dbType != UtlSqlType.Int) && (dbType != UtlSqlType.BigInt))) && (((dbType != UtlSqlType.TimeStamp) && (dbType != UtlSqlType.Boolean)) && ((dbType != UtlSqlType.Decimal) && (dbType != UtlSqlType.Float))))
            {
                throw new ArgumentException();
            }
            this._name = name;
            this._utlDbType = dbType;
        }

        public UtlMetaData(string name, UtlSqlType dbType, int maxLength)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            if ((((dbType != UtlSqlType.Binary) && (dbType != UtlSqlType.VarBinary)) && ((dbType != UtlSqlType.VarChar) && (dbType != UtlSqlType.Char))) && ((dbType != UtlSqlType.Blob) && (dbType != UtlSqlType.Clob)))
            {
                throw new ArgumentException();
            }
            this._name = name;
            this._utlDbType = dbType;
            this._precision = maxLength;
        }

        public UtlMetaData(string name, UtlSqlType dbType, byte precision, byte scale)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            if (dbType != UtlSqlType.Decimal)
            {
                throw new ArgumentException();
            }
            this._name = name;
            this._utlDbType = dbType;
            this._precision = precision;
            this._scale = scale;
        }

        public long MaxLength
        {
            get
            {
                return (long) this._precision;
            }
        }

        public UtlSqlType UtlDbType
        {
            get
            {
                return this._utlDbType;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public int Precision
        {
            get
            {
                return this._precision;
            }
        }

        public byte Scale
        {
            get
            {
                return this._scale;
            }
        }
    }
}

