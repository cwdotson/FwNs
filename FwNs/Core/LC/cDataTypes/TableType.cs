namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.LibCore;
    using System.Text;

    public class TableType : SqlType
    {
        private readonly SqlType[] _dataTypes;
        private readonly Table _table;

        public TableType(Table table) : base(0x15, 0x15, 0L, 0)
        {
            this._table = table;
            this._dataTypes = table.GetColumnTypes();
        }

        public override bool CanBeAssignedFrom(SqlType otherType)
        {
            return false;
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            return false;
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            return 0;
        }

        public override object ConvertCSharpToSQL(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            object[] objArray = (object[]) a;
            if (objArray.Length != this._dataTypes.Length)
            {
                throw Error.GetError(0x15b9);
            }
            for (int i = 0; i < this._dataTypes.Length; i++)
            {
                objArray[i] = this._dataTypes[i].ConvertCSharpToSQL(session, objArray[i]);
            }
            return objArray;
        }

        public override object ConvertToDefaultType(ISessionInterface sessionInterface, object o)
        {
            return o;
        }

        public override string ConvertToSQLString(object a)
        {
            return "NULL";
        }

        public override string ConvertToString(object a)
        {
            return null;
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            return null;
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            return null;
        }

        public override int DisplaySize()
        {
            return 0;
        }

        public override int GetAdoPrecision()
        {
            return 0;
        }

        public override int GetAdoScale()
        {
            return 0;
        }

        public override int GetAdoTypeCode()
        {
            return 0;
        }

        public override SqlType GetAggregateType(SqlType otherType)
        {
            if ((otherType != null) && (otherType != this))
            {
                throw Error.GetError(0x15ba);
            }
            return this;
        }

        public override SqlType GetCombinedType(SqlType otherType, int operation)
        {
            if ((otherType != null) && (otherType != this))
            {
                throw Error.GetError(0x15ba);
            }
            return this;
        }

        public override Type GetCSharpClass()
        {
            return typeof(object);
        }

        public override string GetCSharpClassName()
        {
            return "System.Object";
        }

        public SqlType[] GetDataTypes()
        {
            return this._dataTypes;
        }

        public override string GetDefinition()
        {
            return this.GetNameString();
        }

        public override string GetNameString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TABLE");
            sb.Append('(');
            this._table.GetBodySql(sb);
            sb.Append(')');
            return sb.ToString();
        }

        public override int GetSqlGenericTypeCode()
        {
            return 0x15;
        }

        public Table GetTable()
        {
            return this._table;
        }

        public SqlType[] GetTypesArray()
        {
            return this._dataTypes;
        }

        public bool IsCompatible(DataTable dataTable)
        {
            if (dataTable.Columns.Count != this._dataTypes.Length)
            {
                return false;
            }
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn column = dataTable.Columns[i];
                ColumnSchema schema = this._table.GetColumn(i);
                if ((Types.GetParameterSqlType(column.DataType).TypeCode != schema.DataType.TypeCode) || (column.ColumnName.ToUpper() != schema.ColumnName.Name))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsCompatible(UtlDataReader reader)
        {
            if (reader.FieldCount != this._dataTypes.Length)
            {
                return false;
            }
            for (int i = 0; i < reader.FieldCount; i++)
            {
                ColumnSchema column = this._table.GetColumn(i);
                if ((reader.GetDataTypeName(i).ToUpper() != column.DataType.GetNameString()) || (reader.GetName(i).ToUpper() != column.ColumnName.Name))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool IsTableType()
        {
            return true;
        }

        public Table MakeTable(Database database, DataTable dataTable)
        {
            QNameManager.QName name = database.NameManager.NewQName(dataTable.TableName.ToUpper(), false, 3);
            Table table = new Table(database, name, 11);
            foreach (DataColumn column in dataTable.Columns)
            {
                QNameManager.QName name1 = database.NameManager.NewQName(column.ColumnName.ToUpper(), false, 9);
                name1.schema = SqlInvariants.ModuleQname;
                name1.Parent = name;
                ColumnSchema schema = new ColumnSchema(name1, Types.GetParameterSqlType(column.DataType), column.AllowDBNull, false, null);
                table.AddColumn(schema);
            }
            table.CreatePrimaryKey();
            return table;
        }

        public Table MakeTable(Database database, UtlDataReader reader)
        {
            QNameManager.QName name = database.NameManager.NewQName("PARAMETER___TABLE", false, 3);
            Table table = new Table(database, name, 11);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                QNameManager.QName name1 = database.NameManager.NewQName(reader.GetName(i).ToUpper(), false, 9);
                name1.schema = SqlInvariants.ModuleQname;
                name1.Parent = name;
                DataTable schemaTable = reader.GetSchemaTable();
                ColumnSchema column = new ColumnSchema(name1, Types.GetParameterSqlType((Type) schemaTable.Rows[i][SchemaTableColumn.DataType]), (bool) schemaTable.Rows[i][SchemaTableColumn.AllowDBNull], false, null);
                table.AddColumn(column);
            }
            table.CreatePrimaryKey();
            return table;
        }
    }
}

