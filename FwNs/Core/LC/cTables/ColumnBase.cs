namespace FwNs.Core.LC.cTables
{
    using FwNs.Core.LC.cDataTypes;
    using System;

    public class ColumnBase
    {
        private const bool _isSearchable = true;
        private readonly string _name;
        private readonly string _table;
        private readonly string _schema;
        private readonly string _catalog;
        private bool _isWriteable;
        public byte ParameterMode;
        protected bool _isIdentity;
        protected byte Nullability;
        public SqlType DataType;

        public ColumnBase()
        {
        }

        public ColumnBase(string catalog, string schema, string table, string name)
        {
            this._catalog = catalog;
            this._schema = schema;
            this._table = table;
            this._name = name;
        }

        public virtual string GetCatalogNameString()
        {
            return this._catalog;
        }

        public SqlType GetDataType()
        {
            return this.DataType;
        }

        public virtual string GetNameString()
        {
            return this._name;
        }

        public virtual byte GetNullability()
        {
            if (!this._isIdentity)
            {
                return this.Nullability;
            }
            return 0;
        }

        public byte GetParameterMode()
        {
            return this.ParameterMode;
        }

        public virtual string GetSchemaNameString()
        {
            return this._schema;
        }

        public virtual string GetTableNameString()
        {
            return this._table;
        }

        public bool IsIdentity()
        {
            return this._isIdentity;
        }

        public virtual bool IsNullable()
        {
            return (!this._isIdentity && (this.Nullability == 1));
        }

        public virtual bool IsSearchable()
        {
            return true;
        }

        public virtual bool IsWriteable()
        {
            return this._isWriteable;
        }

        public void SetIdentity(bool value)
        {
            this._isIdentity = value;
        }

        public void SetNullability(byte value)
        {
            this.Nullability = value;
        }

        public void SetNullable(bool value)
        {
            this.Nullability = value ? ((byte) 1) : ((byte) 0);
        }

        public void SetParameterMode(byte mode)
        {
            this.ParameterMode = mode;
        }

        public virtual void SetType(SqlType type)
        {
            this.DataType = type;
        }

        protected void SetType(ColumnBase other)
        {
            this.Nullability = other.Nullability;
            this.DataType = other.DataType;
        }

        public virtual void SetWriteable(bool value)
        {
            this._isWriteable = value;
        }
    }
}

