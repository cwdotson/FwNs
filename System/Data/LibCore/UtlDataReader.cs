namespace System.Data.LibCore
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cResults;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Reflection;

    public sealed class UtlDataReader : DbDataReader
    {
        private UtlCommand _command;
        private Result _rResult;
        private Result _resultIn;
        private RowSetNavigator _nCurrent;
        private int _iCurrentRow;
        private int _iUpdateCount = -1;
        private bool _bInit;
        private int _iColumnCount;
        private int _readingState = -1;
        public long Version;
        private readonly CommandBehavior _commandBehavior;
        public bool DisposeCommand;
        private Result _generatedResult;
        private Queue<Result> _resultSets;

        public UtlDataReader(UtlCommand cmd, CommandBehavior behave)
        {
            this._command = cmd;
            this.Version = this._command.Connection.Version;
            this._commandBehavior = behave;
            this._iUpdateCount = -1;
            if (this._command != null)
            {
                this.NextResult();
            }
        }

        private void CheckClosed()
        {
            if (this._command == null)
            {
                throw new InvalidOperationException("DataReader has been closed");
            }
            if (this.Version == 0)
            {
                throw new UtlException("Execution was aborted by the user");
            }
            if ((this._command.Connection.State != ConnectionState.Open) || (this._command.Connection.Version != this.Version))
            {
                throw new InvalidOperationException("Connection was closed, statement was terminated");
            }
        }

        public void CheckNull(int i)
        {
            if (this.IsDBNull(i))
            {
                throw new ArgumentNullException();
            }
        }

        private void CheckValidRow()
        {
            if (this._readingState != 0)
            {
                throw UtlException.GetException(Error.GetError(0xe11));
            }
        }

        private void ClearResultStructures(Result result)
        {
            result.SetStatement(null);
            RowSetNavigator navigator = result.GetNavigator();
            if (navigator != null)
            {
                navigator.ClearStructures();
            }
        }

        public override void Close()
        {
            if (this._command != null)
            {
                try
                {
                    try
                    {
                        if (this.Version != 0)
                        {
                            try
                            {
                                while (this.NextResult())
                                {
                                }
                            }
                            catch (UtlException)
                            {
                            }
                        }
                        this._command.ClearDataReader();
                    }
                    finally
                    {
                        if (((this._commandBehavior & CommandBehavior.CloseConnection) != CommandBehavior.Default) && (this._command.Connection != null))
                        {
                            this._command.Connection.Close();
                        }
                    }
                }
                finally
                {
                    if (this.DisposeCommand)
                    {
                        this._command.Dispose();
                    }
                }
                this._command = null;
                this._rResult = null;
                this._resultIn = null;
                this._readingState = -1;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.Close();
        }

        private void FetchResult()
        {
            if (((this._command.Parameters.Count != 0) || (this._command.CommandType == CommandType.StoredProcedure)) && (this._command.Statement == null))
            {
                this._command.Prepare();
            }
            if (this._command.Statement != null)
            {
                this._resultIn = this._command.Statement.Execute(this._command.Parameters, this._command.MaxRows, this._command.FetchSize, false);
                this._generatedResult = this._command.Statement.GeneratedResult;
                this._resultSets = this._command.Statement.ChainedResultSets;
            }
            else
            {
                using (UtlStatement statement = new UtlStatement(this._command, this._command.CommandText, false))
                {
                    if ((this._commandBehavior & CommandBehavior.SchemaOnly) != CommandBehavior.Default)
                    {
                        try
                        {
                            Result result = statement.Execute(1, 1, false);
                            if (result.IsError())
                            {
                                throw new Exception();
                            }
                            this._resultIn = result;
                        }
                        catch (Exception)
                        {
                            this._resultIn = statement.PrepareStatement();
                        }
                    }
                    else
                    {
                        this._resultIn = statement.Execute(this._command.MaxRows, this._command.FetchSize, false);
                    }
                    this._generatedResult = statement.GeneratedResult;
                    this._resultSets = statement.ChainedResultSets;
                }
                if (this._resultIn != null)
                {
                    this.ClearResultStructures(this._resultIn);
                    if (this._generatedResult != null)
                    {
                        this.ClearResultStructures(this._generatedResult);
                    }
                    if (this._resultSets != null)
                    {
                        foreach (Result result2 in this._resultSets)
                        {
                            this.ClearResultStructures(result2);
                        }
                    }
                }
            }
        }

        ~UtlDataReader()
        {
            try
            {
                base.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public override bool GetBoolean(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlBoolean);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (bool) columnInType;
        }

        public override byte GetByte(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.Tinyint);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (byte) ((int) columnInType);
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            this.CheckNull(i);
            if (this._rResult.MetaData.ColumnTypes[i].TypeCode == 30)
            {
                BlobDataId id = this.GetColumnInType(i, this._rResult.MetaData.ColumnTypes[i]) as BlobDataId;
                if (id == null)
                {
                    throw new InvalidCastException();
                }
                ISessionInterface sessionProxy = this._command.Connection.InnerConnection.SessionProxy;
                long num2 = id.Length(sessionProxy);
                if (buffer == null)
                {
                    return num2;
                }
                if (fieldOffset > num2)
                {
                    return 0L;
                }
                length = (length > (num2 - fieldOffset)) ? ((int) (num2 - fieldOffset)) : length;
                if (length <= 0)
                {
                    return 0L;
                }
                byte[] sourceArray = id.GetBytes(sessionProxy, fieldOffset, length);
                Array.Copy(sourceArray, 0, buffer, bufferoffset, sourceArray.Length);
                return (long) sourceArray.Length;
            }
            object columnInType = this.GetColumnInType(i, SqlType.SqlVarbinary);
            if (columnInType == null)
            {
                return 0L;
            }
            if (buffer == null)
            {
                return (long) ((int) ((BinaryData) columnInType).Length(this._command.Connection.InnerConnection.SessionProxy));
            }
            byte[] bytes = ((BinaryData) columnInType).GetBytes();
            if (fieldOffset > bytes.Length)
            {
                return 0L;
            }
            length = (length > bytes.Length) ? bytes.Length : length;
            if (length < 0)
            {
                length = 0;
            }
            Array.Copy(bytes, fieldOffset, buffer, (long) bufferoffset, (long) length);
            return length;
        }

        public override char GetChar(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlChar);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (char) columnInType;
        }

        public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
        {
            this.CheckNull(i);
            if (this._rResult.MetaData.ColumnTypes[i].TypeCode == 40)
            {
                SqlType targetType = this._rResult.MetaData.ColumnTypes[i];
                object obj2 = this.GetColumnInType(i, targetType);
                if (obj2 == null)
                {
                    return 0L;
                }
                ClobDataId id = obj2 as ClobDataId;
                if (id == null)
                {
                    throw new InvalidCastException();
                }
                ISessionInterface sessionProxy = this._command.Connection.InnerConnection.SessionProxy;
                long num2 = id.Length(sessionProxy);
                if (fieldOffset > num2)
                {
                    return 0L;
                }
                length = (length > (num2 - fieldOffset)) ? ((int) (num2 - fieldOffset)) : length;
                if (length <= 0)
                {
                    return 0L;
                }
                char[] sourceArray = id.GetChars(sessionProxy, fieldOffset, length);
                Array.Copy(sourceArray, 0, buffer, bufferoffset, sourceArray.Length);
                return (long) sourceArray.Length;
            }
            string columnInType = (string) this.GetColumnInType(i, SqlType.SqlVarchar);
            if (columnInType == null)
            {
                return 0L;
            }
            if (fieldOffset > columnInType.Length)
            {
                return 0L;
            }
            length = (length > (columnInType.Length - fieldOffset)) ? length : (columnInType.Length - ((int) fieldOffset));
            if (length <= 0)
            {
                return 0L;
            }
            columnInType.CopyTo((int) fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        private object GetColumnInType(int columnIndex, SqlType targetType)
        {
            this.CheckClosed();
            this.CheckValidRow();
            SqlType otherType = this._rResult.MetaData.ColumnTypes[columnIndex];
            object a = this._nCurrent.GetCurrent()[columnIndex];
            if (a == null)
            {
                return null;
            }
            if (targetType.TypeCode == 0x6b)
            {
                return new MonthSpan((int) ((IntervalMonthData) a).Units);
            }
            if (targetType.TypeCode == 110)
            {
                IntervalSecondData data = (IntervalSecondData) a;
                return new TimeSpan((data.Units * 0x989680L) + (data.Nanos / 100));
            }
            if (otherType.TypeCode != targetType.TypeCode)
            {
                try
                {
                    a = targetType.ConvertToTypeAdo(this._command.Connection.InnerConnection.SessionProxy, a, otherType);
                }
                catch (Exception)
                {
                    try
                    {
                        return otherType.ConvertToTypeAdo(this._command.Connection.InnerConnection.SessionProxy, a, targetType);
                    }
                    catch (Exception)
                    {
                    }
                    object[] objArray1 = new object[] { "from SQL type ", otherType.GetNameString(), " to ", targetType.GetCSharpClassName(), ", value: ", a };
                    string add = string.Concat(objArray1);
                    UtlException.ThrowError(Error.GetError(0x15b9, add));
                }
                return a;
            }
            return a;
        }

        public override string GetDataTypeName(int i)
        {
            this.CheckClosed();
            this.CheckValidRow();
            return Types.GetTypeName(this._rResult.MetaData.ColumnTypes[i].TypeCode);
        }

        public override DateTime GetDateTime(int i)
        {
            this.CheckNull(i);
            ISessionInterface sessionProxy = this._command.Connection.InnerConnection.SessionProxy;
            switch (this._rResult.MetaData.ColumnTypes[i].TypeCode)
            {
                case 0x5b:
                {
                    TimestampData columnInType = (TimestampData) this.GetColumnInType(i, SqlType.SqlDate);
                    if (columnInType == null)
                    {
                        throw new InvalidCastException();
                    }
                    return (DateTime) SqlType.SqlDate.ConvertSQLToCSharp(sessionProxy, columnInType);
                }
                case 0x5c:
                {
                    TimeData columnInType = (TimeData) this.GetColumnInType(i, SqlType.SqlTime);
                    if (columnInType == null)
                    {
                        throw new InvalidCastException();
                    }
                    return (DateTime) SqlType.SqlTime.ConvertSQLToCSharp(sessionProxy, columnInType);
                }
                case 0x5d:
                {
                    TimestampData columnInType = (TimestampData) this.GetColumnInType(i, SqlType.SqlTimestamp);
                    if (columnInType == null)
                    {
                        throw new InvalidCastException();
                    }
                    return (DateTime) SqlType.SqlTimestamp.ConvertSQLToCSharp(sessionProxy, columnInType);
                }
                case 0x5e:
                {
                    TimeData columnInType = (TimeData) this.GetColumnInType(i, SqlType.SqlTimeWithTimeZone);
                    if (columnInType == null)
                    {
                        throw new InvalidCastException();
                    }
                    return (DateTime) SqlType.SqlTimeWithTimeZone.ConvertSQLToCSharp(sessionProxy, columnInType);
                }
                case 0x5f:
                {
                    TimestampData columnInType = (TimestampData) this.GetColumnInType(i, SqlType.SqlTimestampWithTimeZone);
                    if (columnInType == null)
                    {
                        throw new InvalidCastException();
                    }
                    return (DateTime) SqlType.SqlTimestampWithTimeZone.ConvertSQLToCSharp(sessionProxy, columnInType);
                }
            }
            throw new InvalidCastException();
        }

        public override decimal GetDecimal(int i)
        {
            this.CheckNull(i);
            SqlType targetType = this._rResult.MetaData.ColumnTypes[i];
            if ((targetType.TypeCode != 2) && (targetType.TypeCode != 3))
            {
                targetType = SqlType.SqlDecimalDefault;
            }
            object columnInType = this.GetColumnInType(i, targetType);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (decimal) columnInType;
        }

        public override double GetDouble(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlDouble);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (double) columnInType;
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, (this._commandBehavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection);
        }

        public override Type GetFieldType(int i)
        {
            this.CheckClosed();
            int typeCode = this._rResult.MetaData.ColumnTypes[i].TypeCode;
            if (typeCode <= 30)
            {
                switch (typeCode)
                {
                    case 0x10:
                        return typeof(bool);

                    case 0x19:
                        return typeof(long);

                    case 30:
                    case -4:
                        goto Label_014C;

                    case -11:
                        return typeof(Guid);

                    case -6:
                        return typeof(byte);

                    case 1:
                    case 12:
                        goto Label_0141;

                    case 2:
                    case 3:
                        return typeof(decimal);

                    case 4:
                        return typeof(int);

                    case 5:
                        return typeof(short);

                    case 6:
                        return typeof(float);

                    case 7:
                    case 8:
                        return typeof(double);
                }
                goto Label_0157;
            }
            if (typeCode <= 0x3d)
            {
                if (typeCode == 40)
                {
                    goto Label_0141;
                }
                if ((typeCode - 60) <= 1)
                {
                    goto Label_014C;
                }
                goto Label_0157;
            }
            if ((typeCode - 0x5b) <= 2)
            {
                return typeof(DateTime);
            }
            if (typeCode == 100)
            {
                goto Label_0141;
            }
            if (typeCode == 0x457)
            {
                return typeof(object);
            }
            goto Label_0157;
        Label_0141:
            return typeof(string);
        Label_014C:
            return typeof(byte[]);
        Label_0157:
            throw new InvalidCastException();
        }

        public override float GetFloat(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlDouble);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (float) ((double) columnInType);
        }

        public override Guid GetGuid(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlUniqueIdentifier);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (Guid) columnInType;
        }

        public override short GetInt16(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlSmallint);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (short) ((int) columnInType);
        }

        public override int GetInt32(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlInteger);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (int) columnInType;
        }

        public override long GetInt64(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlBigint);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (long) columnInType;
        }

        public Result GetInternalResult()
        {
            return this._rResult;
        }

        public MonthSpan GetMonthSpan(int i)
        {
            this.CheckNull(i);
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlIntervalYearToMonth);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (MonthSpan) columnInType;
        }

        public override string GetName(int columnIndex)
        {
            string nameString;
            try
            {
                nameString = this._rResult.MetaData.ColumnLabels[columnIndex];
                if (string.IsNullOrEmpty(nameString))
                {
                    nameString = this._rResult.MetaData.columns[columnIndex].GetNameString();
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw UtlException.GetException(0x1a5, columnIndex.ToString());
            }
            return nameString;
        }

        public override int GetOrdinal(string columnName)
        {
            this.CheckClosed();
            for (int i = 0; i < this._iColumnCount; i++)
            {
                string name = this.GetName(i);
                if (columnName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }
            throw new IndexOutOfRangeException(columnName);
        }

        public override DataTable GetSchemaTable()
        {
            this.CheckClosed();
            if ((this._rResult == null) || (!this._rResult.IsData() && ((this._commandBehavior & CommandBehavior.SchemaOnly) == CommandBehavior.Default)))
            {
                throw UtlException.GetException(0x1ca);
            }
            DataTable table = new DataTable("SchemaTable");
            ResultMetaData metaData = this._rResult.MetaData;
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add(SchemaTableColumn.ColumnName, typeof(string));
            table.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int));
            table.Columns.Add(SchemaTableColumn.ColumnSize, typeof(long));
            table.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(long));
            table.Columns.Add(SchemaTableColumn.NumericScale, typeof(short));
            table.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool));
            table.Columns.Add(SchemaTableColumn.IsKey, typeof(bool));
            table.Columns.Add(SchemaTableOptionalColumn.BaseServerName, typeof(string));
            table.Columns.Add(SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
            table.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string));
            table.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string));
            table.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string));
            table.Columns.Add(SchemaTableColumn.DataType, typeof(Type));
            table.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool));
            table.Columns.Add(SchemaTableColumn.ProviderType, typeof(int));
            table.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool));
            table.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool));
            table.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
            table.Columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool));
            table.Columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool));
            table.Columns.Add(SchemaTableColumn.IsLong, typeof(bool));
            table.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool));
            table.Columns.Add(SchemaTableOptionalColumn.ProviderSpecificDataType, typeof(Type));
            table.Columns.Add(SchemaTableOptionalColumn.DefaultValue, typeof(string));
            DataTable schema = null;
            DataTable table3 = null;
            if ((this._commandBehavior & CommandBehavior.KeyInfo) != CommandBehavior.Default)
            {
                string[] restrictionValues = new string[6];
                restrictionValues[1] = metaData.columns[0].GetSchemaNameString();
                restrictionValues[2] = metaData.columns[0].GetTableNameString();
                restrictionValues[5] = "true";
                schema = this._command.Connection.GetSchema("INDEXCOLUMNS", restrictionValues);
                string[] textArray2 = new string[3];
                textArray2[1] = metaData.columns[0].GetSchemaNameString();
                textArray2[2] = metaData.columns[0].GetTableNameString();
                table3 = this._command.Connection.GetSchema("COLUMNS", textArray2);
            }
            table.BeginLoadData();
            for (int i = 0; i < metaData.GetColumnCount(); i++)
            {
                DataRow row = table.NewRow();
                if ((metaData.ColumnLabels[i] == null) && string.IsNullOrEmpty(metaData.columns[i].GetNameString()))
                {
                    row[SchemaTableColumn.ColumnName] = DBNull.Value;
                }
                else
                {
                    row[SchemaTableColumn.ColumnName] = metaData.ColumnLabels[i] ?? metaData.columns[i].GetNameString();
                }
                row[SchemaTableColumn.ColumnOrdinal] = i;
                row[SchemaTableColumn.ColumnSize] = metaData.ColumnTypes[i].GetAdoPrecision();
                row[SchemaTableColumn.ProviderType] = metaData.ColumnTypes[i].GetAdoTypeCode();
                SqlType type = metaData.ColumnTypes[i];
                if (type.IsNumberType())
                {
                    if (type.AcceptsPrecision())
                    {
                        row[SchemaTableColumn.NumericPrecision] = ((NumberType) metaData.ColumnTypes[i]).GetNumericPrecisionInRadix();
                    }
                    else
                    {
                        row[SchemaTableColumn.NumericPrecision] = DBNull.Value;
                    }
                    if (type.AcceptsScale())
                    {
                        row[SchemaTableColumn.NumericScale] = type.GetAdoScale();
                    }
                    else
                    {
                        row[SchemaTableColumn.NumericScale] = DBNull.Value;
                    }
                }
                row[SchemaTableColumn.IsLong] = type.IsLobType();
                row[SchemaTableColumn.AllowDBNull] = metaData.columns[i].GetNullability() > 0;
                row[SchemaTableOptionalColumn.IsReadOnly] = !metaData.columns[i].IsWriteable();
                row[SchemaTableOptionalColumn.IsAutoIncrement] = metaData.columns[i].IsIdentity();
                row[SchemaTableOptionalColumn.IsHidden] = false;
                row[SchemaTableOptionalColumn.IsRowVersion] = false;
                row[SchemaTableColumn.IsUnique] = false;
                row[SchemaTableColumn.IsKey] = false;
                row[SchemaTableColumn.DataType] = this.GetFieldType(i);
                if (string.IsNullOrEmpty(metaData.columns[i].GetNameString()))
                {
                    row[SchemaTableColumn.BaseColumnName] = DBNull.Value;
                }
                else
                {
                    row[SchemaTableColumn.BaseColumnName] = metaData.columns[i].GetNameString();
                }
                row[SchemaTableColumn.IsExpression] = string.IsNullOrEmpty(metaData.columns[i].GetNameString());
                row[SchemaTableColumn.IsAliased] = string.Compare(metaData.columns[i].GetNameString(), metaData.ColumnLabels[i], StringComparison.OrdinalIgnoreCase) > 0;
                if (string.IsNullOrEmpty(metaData.columns[i].GetTableNameString()))
                {
                    row[SchemaTableColumn.BaseTableName] = DBNull.Value;
                }
                else
                {
                    row[SchemaTableColumn.BaseTableName] = metaData.columns[i].GetTableNameString();
                }
                if (string.IsNullOrEmpty(metaData.columns[i].GetCatalogNameString()))
                {
                    row[SchemaTableOptionalColumn.BaseCatalogName] = "PUBLIC";
                }
                else
                {
                    row[SchemaTableOptionalColumn.BaseCatalogName] = metaData.columns[i].GetCatalogNameString();
                }
                if (string.IsNullOrEmpty(metaData.columns[i].GetSchemaNameString()))
                {
                    row[SchemaTableColumn.BaseSchemaName] = DBNull.Value;
                }
                else
                {
                    row[SchemaTableColumn.BaseSchemaName] = metaData.columns[i].GetSchemaNameString();
                }
                if ((this._commandBehavior & CommandBehavior.KeyInfo) != CommandBehavior.Default)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    for (int j = 0; j < schema.Rows.Count; j++)
                    {
                        string key = (string) schema.Rows[j]["INDEX_NAME"];
                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary.Add(key, (string) schema.Rows[j]["COLUMN_NAME"]);
                        }
                        else
                        {
                            dictionary[key] = string.Empty;
                        }
                    }
                    foreach (string str2 in dictionary.Keys)
                    {
                        if (!string.IsNullOrEmpty(dictionary[str2]) && (string.Compare(metaData.columns[i].GetNameString(), dictionary[str2], StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            row[SchemaTableColumn.IsUnique] = true;
                        }
                    }
                    for (int k = 0; k < table3.Rows.Count; k++)
                    {
                        if (string.Compare(metaData.columns[i].GetNameString(), (string) table3.Rows[k]["COLUMN_NAME"], StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            row[SchemaTableOptionalColumn.DefaultValue] = table3.Rows[k]["COLUMN_DEF"];
                            row[SchemaTableColumn.IsKey] = table3.Rows[k]["IS_PRIMARY_KEY"];
                        }
                    }
                }
                table.Rows.Add(row);
            }
            table.AcceptChanges();
            table.EndLoadData();
            return table;
        }

        public override string GetString(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlVarchar);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (string) columnInType;
        }

        public TimeSpan GetTimeSpan(int i)
        {
            this.CheckNull(i);
            object columnInType = this.GetColumnInType(i, SqlType.SqlIntervalDayToSecond);
            if (columnInType == null)
            {
                throw new InvalidCastException();
            }
            return (TimeSpan) columnInType;
        }

        public UtlLob GetUtlBlob(int i)
        {
            this.CheckNull(i);
            return new UtlLob(this._command.Connection.InnerConnection.SessionProxy, (IBlobData) this.GetColumnInType(i, SqlType.SqlBlob));
        }

        public UtlLob GetUtlClob(int i)
        {
            this.CheckNull(i);
            return new UtlLob(this._command.Connection.InnerConnection.SessionProxy, (IClobData) this.GetColumnInType(i, SqlType.SqlClob));
        }

        public override object GetValue(int columnIndex)
        {
            this.CheckValidRow();
            ISessionInterface sessionProxy = this._command.Connection.InnerConnection.SessionProxy;
            if (this._nCurrent.GetCurrent()[columnIndex] != null)
            {
                return this._rResult.MetaData.ColumnTypes[columnIndex].ConvertSQLToCSharp(sessionProxy, this._nCurrent.GetCurrent()[columnIndex]);
            }
            return DBNull.Value;
        }

        public override int GetValues(object[] values)
        {
            this.CheckValidRow();
            int length = this._iColumnCount;
            if (values.Length < length)
            {
                length = values.Length;
            }
            for (int i = 0; i < length; i++)
            {
                values[i] = this.GetValue(i);
            }
            return length;
        }

        public override bool IsDBNull(int i)
        {
            return (this.GetValue(i) == DBNull.Value);
        }

        public override bool NextResult()
        {
            this.CheckClosed();
            if (this._readingState == -1)
            {
                this.FetchResult();
                this._rResult = this._resultIn;
            }
            else
            {
                if (this._resultIn == null)
                {
                    return false;
                }
                if ((this._rResult == this._resultIn) && (this._generatedResult != null))
                {
                    this._rResult = this._generatedResult;
                }
                else
                {
                    if ((this._resultSets == null) || (this._resultSets.Count == 0))
                    {
                        return false;
                    }
                    Result result = this._resultSets.Dequeue();
                    this._bInit = false;
                    this._rResult = result;
                }
            }
            if (this._rResult.IsUpdateCount())
            {
                if (this._iUpdateCount == -1)
                {
                    this._iUpdateCount = 0;
                }
                this._iUpdateCount += this._rResult.GetUpdateCount();
                this._readingState = 2;
                return this.NextResult();
            }
            if (this._rResult.IsError())
            {
                this._readingState = 2;
                UtlException.ThrowError(this._rResult);
            }
            else if (this._rResult.IsData())
            {
                this._iColumnCount = this._rResult.MetaData.GetColumnCount();
                this._readingState = 0;
            }
            else
            {
                this._readingState = 2;
            }
            return true;
        }

        public override bool Read()
        {
            this.CheckClosed();
            if (this._readingState == 2)
            {
                return false;
            }
            if ((this._rResult == null) || this._rResult.GetNavigator().IsEmpty())
            {
                this._readingState = 1;
                return false;
            }
            if (!this._bInit)
            {
                this._nCurrent = this._rResult.GetNavigator();
                this._nCurrent.GetNext();
                this._bInit = true;
                this._iCurrentRow = 1;
            }
            else if ((this._commandBehavior & CommandBehavior.SingleRow) == CommandBehavior.Default)
            {
                if (this._nCurrent == null)
                {
                    return false;
                }
                this._nCurrent.GetNext();
                this._iCurrentRow++;
            }
            else
            {
                this._nCurrent = null;
            }
            if (this._nCurrent.IsAfterLast())
            {
                this._iCurrentRow = this._rResult.GetNavigator().Size + 1;
                this._readingState = 1;
                return false;
            }
            return true;
        }

        public override int Depth
        {
            get
            {
                this.CheckClosed();
                return 0;
            }
        }

        public override int FieldCount
        {
            get
            {
                this.CheckClosed();
                return this._iColumnCount;
            }
        }

        public int RowCount
        {
            get
            {
                this.CheckClosed();
                if (this._nCurrent != null)
                {
                    return this._nCurrent.GetSize();
                }
                return 0;
            }
        }

        public override int VisibleFieldCount
        {
            get
            {
                this.CheckClosed();
                return this._iColumnCount;
            }
        }

        public override bool HasRows
        {
            get
            {
                this.CheckClosed();
                return (((this._rResult != null) && this._rResult.IsData()) && (this._readingState != 1));
            }
        }

        public override bool IsClosed
        {
            get
            {
                return (this._command == null);
            }
        }

        public override int RecordsAffected
        {
            get
            {
                if (this._iUpdateCount >= 0)
                {
                    return this._iUpdateCount;
                }
                return 0;
            }
        }

        public override object this[string name]
        {
            get
            {
                return this.GetValue(this.GetOrdinal(name));
            }
        }

        public override object this[int i]
        {
            get
            {
                return this.GetValue(i);
            }
        }
    }
}

