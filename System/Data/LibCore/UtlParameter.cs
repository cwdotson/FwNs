namespace System.Data.LibCore
{
    using FwNs.Core;
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;

    public sealed class UtlParameter : DbParameter, ICloneable
    {
        public FwNs.Core.UtlType UtlType;
        private DataRowVersion _rowVersion;
        private object _objValue;
        private string _sourceColumn;
        private string _parameterName;
        private ParameterDirection _direction;
        private int _dataSize;
        private bool _nullable;
        private bool _nullMapping;

        public UtlParameter() : this(null, FwNs.Core.UtlType.Null, 0, null, DataRowVersion.Current)
        {
        }

        public UtlParameter(FwNs.Core.UtlType UtlType) : this(null, UtlType, 0, null, DataRowVersion.Current)
        {
        }

        private UtlParameter(UtlParameter source) : this(source.ParameterName, source.UtlType, 0, source.Direction, source.IsNullable, 0, 0, source.SourceColumn, source.SourceVersion, source.Value)
        {
            this._nullMapping = source._nullMapping;
        }

        public UtlParameter(string parameterName) : this(parameterName, FwNs.Core.UtlType.Null, 0, null, DataRowVersion.Current)
        {
        }

        public UtlParameter(FwNs.Core.UtlType parameterType, int parameterSize) : this(null, parameterType, parameterSize, null, DataRowVersion.Current)
        {
        }

        public UtlParameter(FwNs.Core.UtlType UtlType, object value) : this(null, UtlType, 0, null, DataRowVersion.Current)
        {
            this.Value = value;
        }

        public UtlParameter(FwNs.Core.UtlType UtlType, string sourceColumn) : this(null, UtlType, 0, sourceColumn, DataRowVersion.Current)
        {
        }

        public UtlParameter(string parameterName, FwNs.Core.UtlType UtlType) : this(parameterName, UtlType, 0, null, DataRowVersion.Current)
        {
        }

        public UtlParameter(string parameterName, object value) : this(parameterName, FwNs.Core.UtlType.Null, 0, null, DataRowVersion.Current)
        {
            this.Value = value;
        }

        public UtlParameter(FwNs.Core.UtlType parameterType, int parameterSize, string sourceColumn) : this(null, parameterType, parameterSize, sourceColumn, DataRowVersion.Current)
        {
        }

        public UtlParameter(FwNs.Core.UtlType UtlType, string sourceColumn, DataRowVersion rowVersion) : this(null, UtlType, 0, sourceColumn, rowVersion)
        {
        }

        public UtlParameter(string parameterName, FwNs.Core.UtlType parameterType, int parameterSize) : this(parameterName, parameterType, parameterSize, null, DataRowVersion.Current)
        {
        }

        public UtlParameter(string parameterName, FwNs.Core.UtlType UtlType, string sourceColumn) : this(parameterName, UtlType, 0, sourceColumn, DataRowVersion.Current)
        {
        }

        public UtlParameter(FwNs.Core.UtlType parameterType, int parameterSize, string sourceColumn, DataRowVersion rowVersion) : this(null, parameterType, parameterSize, sourceColumn, rowVersion)
        {
        }

        public UtlParameter(string parameterName, FwNs.Core.UtlType parameterType, int parameterSize, string sourceColumn) : this(parameterName, parameterType, parameterSize, sourceColumn, DataRowVersion.Current)
        {
        }

        public UtlParameter(string parameterName, FwNs.Core.UtlType UtlType, string sourceColumn, DataRowVersion rowVersion) : this(parameterName, UtlType, 0, sourceColumn, rowVersion)
        {
        }

        public UtlParameter(string parameterName, FwNs.Core.UtlType parameterType, int parameterSize, string sourceColumn, DataRowVersion rowVersion)
        {
            this._parameterName = parameterName;
            this.UtlType = parameterType;
            this._sourceColumn = sourceColumn;
            this._rowVersion = rowVersion;
            this._dataSize = parameterSize;
            this._nullable = true;
            this._direction = ParameterDirection.Input;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public UtlParameter(string parameterName, FwNs.Core.UtlType parameterType, int parameterSize, ParameterDirection direction, bool isNullable, byte precision, byte scale, string sourceColumn, DataRowVersion rowVersion, object value) : this(parameterName, parameterType, parameterSize, sourceColumn, rowVersion)
        {
            this.Direction = direction;
            this.IsNullable = isNullable;
            this.Value = value;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public UtlParameter(string parameterName, FwNs.Core.UtlType parameterType, int parameterSize, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion rowVersion, bool sourceColumnNullMapping, object value) : this(parameterName, parameterType, parameterSize, sourceColumn, rowVersion)
        {
            this.Direction = direction;
            this.SourceColumnNullMapping = sourceColumnNullMapping;
            this.Value = value;
        }

        public object Clone()
        {
            return new UtlParameter(this);
        }

        public override void ResetDbType()
        {
            this.UtlType = FwNs.Core.UtlType.Null;
        }

        public override bool IsNullable
        {
            get
            {
                return this._nullable;
            }
            set
            {
                this._nullable = value;
            }
        }

        [RefreshProperties(RefreshProperties.All), DbProviderSpecificTypeProperty(true)]
        public override System.Data.DbType DbType
        {
            get
            {
                switch (this.UtlType)
                {
                    case FwNs.Core.UtlType.Boolean:
                        return System.Data.DbType.Boolean;

                    case FwNs.Core.UtlType.TinyInt:
                        return System.Data.DbType.Byte;

                    case FwNs.Core.UtlType.SmallInt:
                        return System.Data.DbType.Int16;

                    case FwNs.Core.UtlType.Int:
                        return System.Data.DbType.Int32;

                    case FwNs.Core.UtlType.BigInt:
                        return System.Data.DbType.Int64;

                    case FwNs.Core.UtlType.Decimal:
                    case FwNs.Core.UtlType.Money:
                        return System.Data.DbType.Decimal;

                    case FwNs.Core.UtlType.Double:
                        return System.Data.DbType.Double;

                    case FwNs.Core.UtlType.Char:
                        return System.Data.DbType.String;

                    case FwNs.Core.UtlType.VarChar:
                    case FwNs.Core.UtlType.VarCharIngnoreCase:
                    case FwNs.Core.UtlType.Clob:
                        return System.Data.DbType.String;

                    case FwNs.Core.UtlType.UniqueIdentifier:
                        return System.Data.DbType.Guid;

                    case FwNs.Core.UtlType.Variant:
                        return System.Data.DbType.Object;

                    case FwNs.Core.UtlType.Time:
                    case FwNs.Core.UtlType.TimeTZ:
                        return System.Data.DbType.Time;

                    case FwNs.Core.UtlType.TimeStamp:
                    case FwNs.Core.UtlType.TimeStampTZ:
                        return System.Data.DbType.DateTime;

                    case FwNs.Core.UtlType.Date:
                        return System.Data.DbType.DateTime;

                    case FwNs.Core.UtlType.Binary:
                    case FwNs.Core.UtlType.VarBinary:
                    case FwNs.Core.UtlType.Blob:
                        return System.Data.DbType.Binary;

                    case FwNs.Core.UtlType.Structured:
                        return System.Data.DbType.Object;
                }
                return System.Data.DbType.String;
            }
            set
            {
                switch (value)
                {
                    case System.Data.DbType.AnsiString:
                    case System.Data.DbType.String:
                        this.UtlType = FwNs.Core.UtlType.VarChar;
                        return;

                    case System.Data.DbType.Binary:
                    case System.Data.DbType.Object:
                        this.UtlType = FwNs.Core.UtlType.VarBinary;
                        return;

                    case System.Data.DbType.Byte:
                        this.UtlType = FwNs.Core.UtlType.TinyInt;
                        return;

                    case System.Data.DbType.Boolean:
                        this.UtlType = FwNs.Core.UtlType.Boolean;
                        return;

                    case System.Data.DbType.Date:
                        this.UtlType = FwNs.Core.UtlType.Date;
                        return;

                    case System.Data.DbType.DateTime:
                        this.UtlType = FwNs.Core.UtlType.TimeStamp;
                        return;

                    case System.Data.DbType.Decimal:
                        this.UtlType = FwNs.Core.UtlType.Decimal;
                        return;

                    case System.Data.DbType.Double:
                    case System.Data.DbType.Single:
                        this.UtlType = FwNs.Core.UtlType.Double;
                        return;

                    case System.Data.DbType.Guid:
                        this.UtlType = FwNs.Core.UtlType.UniqueIdentifier;
                        return;

                    case System.Data.DbType.Int16:
                        this.UtlType = FwNs.Core.UtlType.SmallInt;
                        return;

                    case System.Data.DbType.Int32:
                        this.UtlType = FwNs.Core.UtlType.Int;
                        return;

                    case System.Data.DbType.Int64:
                        this.UtlType = FwNs.Core.UtlType.BigInt;
                        return;

                    case System.Data.DbType.Time:
                        this.UtlType = FwNs.Core.UtlType.Time;
                        return;

                    case System.Data.DbType.AnsiStringFixedLength:
                    case System.Data.DbType.StringFixedLength:
                        this.UtlType = FwNs.Core.UtlType.Char;
                        return;
                }
                throw new InvalidEnumArgumentException();
            }
        }

        public override ParameterDirection Direction
        {
            get
            {
                return this._direction;
            }
            set
            {
                this._direction = value;
            }
        }

        public override string ParameterName
        {
            get
            {
                return this._parameterName;
            }
            set
            {
                this._parameterName = value;
            }
        }

        [DefaultValue(0)]
        public override int Size
        {
            get
            {
                return this._dataSize;
            }
            set
            {
                this._dataSize = value;
            }
        }

        public override string SourceColumn
        {
            get
            {
                return this._sourceColumn;
            }
            set
            {
                this._sourceColumn = value;
            }
        }

        public override bool SourceColumnNullMapping
        {
            get
            {
                return this._nullMapping;
            }
            set
            {
                this._nullMapping = value;
            }
        }

        public override DataRowVersion SourceVersion
        {
            get
            {
                return this._rowVersion;
            }
            set
            {
                this._rowVersion = value;
            }
        }

        [RefreshProperties(RefreshProperties.All), TypeConverter(typeof(StringConverter))]
        public override object Value
        {
            get
            {
                return this._objValue;
            }
            set
            {
                this._objValue = value;
            }
        }
    }
}

