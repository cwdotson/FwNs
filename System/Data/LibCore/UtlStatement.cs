namespace System.Data.LibCore
{
    using FwNs.Core;
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cResults;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.IO;
    using System.Text;

    public sealed class UtlStatement : IDisposable
    {
        private readonly UtlCommand _command;
        private readonly string _sqlStatement;
        private Result _generatedResult;
        private byte[] _parameterModes;
        private string[] _parameterNames;
        private SqlType[] _parameterTypes;
        private object[] _parameterValues;
        private ResultMetaData _pmdDescriptor;
        private bool _isNamedParameters = true;
        private Result _resultOut;
        public Queue<Result> ResultSets = new Queue<Result>();
        private List<UtlException> _sqlWarnings;
        private long _statementId = -1L;
        private long[] _streamLengths;
        private int queryTimeout;
        private int statementReturnType;
        public bool IsPrepared;

        public UtlStatement(UtlCommand command, string strCommand, bool prepare)
        {
            this._command = command;
            if (command.CommandType == CommandType.StoredProcedure)
            {
                this._sqlStatement = BuildStoredProcedureSql(strCommand, command.Connection);
                prepare = true;
            }
            else
            {
                this._sqlStatement = strCommand;
            }
            this.IsPrepared = false;
            if (prepare)
            {
                this.PrepareStatement();
                this.IsPrepared = true;
            }
        }

        public void BindParameters(UtlParameterCollection paramCollection, bool isNamedParameters)
        {
            for (int i = 0; i < this._parameterNames.Length; i++)
            {
                UtlParameter parameter;
                string str = this._parameterNames[i];
                if (isNamedParameters)
                {
                    parameter = paramCollection[str];
                }
                else
                {
                    parameter = paramCollection[i];
                }
                if (parameter == null)
                {
                    throw new ArgumentException("Missing Parameter " + str);
                }
                if ((this._parameterModes[i] == 1) && (parameter.Direction != ParameterDirection.Input))
                {
                    throw new ArgumentException(parameter.Direction.ToString());
                }
                if ((this._parameterModes[i] == 4) && (parameter.Direction != ParameterDirection.Output))
                {
                    throw new ArgumentException(parameter.Direction.ToString());
                }
                if (((this._parameterModes[i] == 2) && (parameter.Direction != ParameterDirection.Output)) && ((parameter.Direction != ParameterDirection.Input) && (parameter.Direction != ParameterDirection.InputOutput)))
                {
                    throw new ArgumentException(parameter.Direction.ToString());
                }
                this.SetParameter(i, parameter);
                this._streamLengths[i] = parameter.Size;
            }
        }

        private static void BreakName2SchemaSP(string fullName, ref string schema, ref string sp)
        {
            if (fullName.StartsWith("\""))
            {
                for (int i = 0; i < fullName.Length; i++)
                {
                    if (((fullName[i] == '"') && (i < (fullName.Length - 1))) && (fullName[i + 1] == '.'))
                    {
                        schema = fullName.Substring(0, i + 1);
                        sp = fullName.Substring(i + 2);
                        return;
                    }
                }
                schema = null;
                sp = fullName;
            }
            else
            {
                int index = fullName.IndexOf(".");
                if (index == -1)
                {
                    schema = null;
                    sp = fullName;
                }
                else
                {
                    schema = fullName.Substring(0, index);
                    sp = fullName.Substring(index + 1);
                }
            }
        }

        private static string BuildStoredProcedureSql(string spName, UtlConnection connection)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("CALL ").Append(spName).Append("( ");
            string schema = null;
            BreakName2SchemaSP(spName, ref schema, ref spName);
            if (spName.StartsWith("\"") && spName.EndsWith("\""))
            {
                spName = spName.Substring(1, spName.Length - 2);
            }
            if (((schema != null) && schema.StartsWith("\"")) && schema.EndsWith("\""))
            {
                schema = schema.Substring(1, schema.Length - 2);
            }
            string[] restrictionValues = new string[3];
            restrictionValues[1] = schema;
            restrictionValues[2] = spName;
            DataTable table = connection.GetSchema("PROCEDUREPARAMETERS", restrictionValues);
            if (table.Rows.Count == 0)
            {
                string[] textArray2 = new string[3];
                textArray2[1] = schema;
                textArray2[2] = spName;
                if (connection.GetSchema("PROCEDURES", textArray2).Rows.Count == 0)
                {
                    string[] textArray3 = new string[3];
                    textArray3[1] = schema;
                    textArray3[2] = spName.ToUpper();
                    table = connection.GetSchema("PROCEDUREPARAMETERS", textArray3);
                }
            }
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string str2 = (string) table.Rows[i]["COLUMN_NAME"];
                if (str2.StartsWith("@"))
                {
                    builder.Append(str2);
                }
                else
                {
                    builder.Append("@").Append(str2);
                }
                if (i != (table.Rows.Count - 1))
                {
                    builder.Append(" , ");
                }
            }
            builder.Append(" );");
            return builder.ToString();
        }

        public void ClearWarnings()
        {
            this._sqlWarnings = null;
        }

        public void Close()
        {
            if (this._statementId != -1L)
            {
                try
                {
                    this._command.Connection.InnerConnection.SessionProxy.Execute(Result.NewFreeStmtRequest(this._statementId));
                    this._statementId = -1L;
                    this._pmdDescriptor = null;
                }
                catch (Exception)
                {
                }
            }
            this._sqlWarnings = null;
            this._generatedResult = null;
        }

        public void Dispose()
        {
            this.Close();
        }

        public Result Execute(int maxrows, int fetchsize, bool updatable)
        {
            Result rResult = null;
            Result r = Result.NewExecuteDirectRequest();
            try
            {
                r.SetPrepareOrExecuteProperties(this._sqlStatement, maxrows, fetchsize, this.statementReturnType, this.queryTimeout, ResultProperties.DefaultPropsValue, this._command.FetchGeneratedResults ? 1 : 2, null, null);
                if (updatable)
                {
                    r.SetDataResultProperties(maxrows, fetchsize, 0x3eb, 0x3f0, 1);
                }
                rResult = this._command.Connection.InnerConnection.SessionProxy.Execute(r);
            }
            catch (CoreException exception1)
            {
                throw UtlException.GetException(exception1);
            }
            finally
            {
                this.PerformPostExecute(rResult);
                r.ClearLobResults();
            }
            if (rResult.IsError())
            {
                UtlException.ThrowError(rResult);
            }
            return rResult;
        }

        public Result Execute(UtlParameterCollection parameters, int maxrows, int fetchsize, bool updatable)
        {
            Result resultIn = null;
            this.BindParameters(this._command.Parameters, this._isNamedParameters);
            this.PerformPreExecute();
            try
            {
                this._resultOut.ParameterMetaData = this._pmdDescriptor;
                if (this._command.CommandType == CommandType.StoredProcedure)
                {
                    this._resultOut.SetPreparedResultUpdateProperties(this._parameterValues);
                }
                else
                {
                    this._resultOut.SetPreparedExecuteProperties(this._parameterValues, maxrows, fetchsize, ResultProperties.DefaultPropsValue);
                }
                if (updatable)
                {
                    this._resultOut.SetDataResultProperties(maxrows, fetchsize, 0x3eb, 0x3f0, 1);
                }
                resultIn = this._command.Connection.InnerConnection.SessionProxy.Execute(this._resultOut);
                if (resultIn.GetResultType() == 0x2b)
                {
                    this.SetOutParameterValues(resultIn);
                }
                this.SetReturnParameterValue(resultIn);
            }
            catch (CoreException exception1)
            {
                throw UtlException.GetException(exception1);
            }
            finally
            {
                this.PerformPostExecute(resultIn);
                this._resultOut.ClearLobResults();
            }
            if (resultIn.IsError())
            {
                UtlException.ThrowError(resultIn);
            }
            return resultIn;
        }

        private object GetColumnInType(object value, SqlType sourceType, SqlType targetType)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            if (targetType.TypeCode == 0x6b)
            {
                return new MonthSpan((int) ((IntervalMonthData) value).Units);
            }
            if (targetType.TypeCode == 110)
            {
                IntervalSecondData data = (IntervalSecondData) value;
                return new TimeSpan((data.Units * 0x989680L) + (data.Nanos / 100));
            }
            if (sourceType.TypeCode != targetType.TypeCode)
            {
                try
                {
                    value = targetType.ConvertToTypeAdo(this._command.Connection.InnerConnection.SessionProxy, value, sourceType);
                }
                catch (CoreException)
                {
                    string str = value + "instance of " + value.GetType().Name;
                    string[] textArray1 = new string[] { "from SQL type ", sourceType.GetNameString(), " to ", targetType.GetCSharpClassName(), ", value: ", str };
                    string add = string.Concat(textArray1);
                    throw UtlException.GetException(Error.GetError(0x15b9, add));
                }
            }
            return value;
        }

        private static string GetQuotedName(string name)
        {
            if (name.Equals(name.ToUpper()))
            {
                return name;
            }
            return string.Format("\"{0}\"", name);
        }

        public List<UtlException> GetWarnings()
        {
            return this._sqlWarnings;
        }

        private void PerformPostExecute(Result rResult)
        {
            this._generatedResult = null;
            if (rResult != null)
            {
                Result r = rResult;
                while (r.GetChainedResult() != null)
                {
                    r = r.GetUnlinkChainedResult();
                    if (r.GetResultType() == 0x13)
                    {
                        UtlException item = UtlException.SqlWarning(r);
                        if (this._sqlWarnings == null)
                        {
                            this._sqlWarnings = new List<UtlException>();
                        }
                        this._sqlWarnings.Add(item);
                    }
                    else if (r.GetResultType() != 2)
                    {
                        if (((this._generatedResult == null) && (r.GetResultType() == 3)) && rResult.IsUpdateCount())
                        {
                            this._generatedResult = r;
                            continue;
                        }
                        if (r.GetResultType() == 3)
                        {
                            this.ResultSets.Enqueue(r);
                        }
                    }
                }
                this._command.SetWarnings(this._sqlWarnings);
            }
        }

        private void PerformPreExecute()
        {
            for (int i = 0; i < this._parameterValues.Length; i++)
            {
                Stream stream;
                ClobDataId id2;
                TextReader reader;
                object obj2 = this._parameterValues[i];
                if (obj2 == null)
                {
                    continue;
                }
                ISessionInterface sessionProxy = this._command.Connection.InnerConnection.SessionProxy;
                if (this._parameterTypes[i].TypeCode != 30)
                {
                    goto Label_010B;
                }
                byte[] buffer = obj2 as byte[];
                if (buffer == null)
                {
                    goto Label_00B1;
                }
                long length = buffer.Length;
                BlobDataId id = sessionProxy.CreateBlob(length);
                long lobId = id.GetId();
                using (MemoryStream stream2 = new MemoryStream(buffer))
                {
                    ResultLob lob = ResultLob.NewLobCreateBlobRequest(sessionProxy.GetId(), lobId, stream2, length);
                    sessionProxy.AllocateResultLob(lob, null);
                    this._resultOut.AddLobResult(lob);
                }
            Label_00A2:
                this._parameterValues[i] = id;
                continue;
            Label_00B1:
                stream = obj2 as Stream;
                if (stream != null)
                {
                    long num4 = this._streamLengths[i];
                    long num5 = sessionProxy.CreateBlob(num4).GetId();
                    ResultLob lob2 = ResultLob.NewLobCreateBlobRequest(sessionProxy.GetId(), num5, stream, num4);
                    sessionProxy.AllocateResultLob(lob2, null);
                    this._resultOut.AddLobResult(lob2);
                    goto Label_00A2;
                }
                throw new InvalidCastException();
            Label_010B:
                if (this._parameterTypes[i].TypeCode != 40)
                {
                    continue;
                }
                string s = obj2 as string;
                if (s == null)
                {
                    char[] chArray = obj2 as char[];
                    if (chArray != null)
                    {
                        s = new string(chArray);
                    }
                }
                if (s == null)
                {
                    goto Label_01AD;
                }
                long num6 = s.Length;
                using (TextReader reader2 = new StringReader(s))
                {
                    id2 = sessionProxy.CreateClob(num6);
                    long num7 = id2.GetId();
                    ResultLob lob3 = ResultLob.NewLobCreateClobRequest(sessionProxy.GetId(), num7, reader2, num6);
                    sessionProxy.AllocateResultLob(lob3, null);
                    this._resultOut.AddLobResult(lob3);
                }
            Label_01A1:
                this._parameterValues[i] = id2;
                continue;
            Label_01AD:
                reader = obj2 as TextReader;
                if (reader == null)
                {
                    throw new InvalidCastException();
                }
                long num8 = this._streamLengths[i];
                long num9 = sessionProxy.CreateClob(num8).GetId();
                ResultLob result = ResultLob.NewLobCreateClobRequest(sessionProxy.GetId(), num9, reader, num8);
                sessionProxy.AllocateResultLob(result, null);
                this._resultOut.AddLobResult(result);
                goto Label_01A1;
            }
        }

        public Result PrepareStatement()
        {
            this._resultOut = Result.NewPrepareStatementRequest();
            this._resultOut.SetMainString(this._sqlStatement);
            this._resultOut.SetPrepareOrExecuteProperties(this._sqlStatement, 0, 0, 0, this.queryTimeout, ResultProperties.DefaultPropsValue, this._command.FetchGeneratedResults ? 1 : 2, null, null);
            Result r = this._command.Connection.InnerConnection.SessionProxy.Execute(this._resultOut);
            if (r.IsError())
            {
                UtlException.ThrowError(r);
            }
            try
            {
                this._statementId = r.GetStatementId();
                this._pmdDescriptor = r.ParameterMetaData;
                this._parameterTypes = this._pmdDescriptor.GetParameterTypes();
                this._parameterValues = new object[this._parameterTypes.Length];
                this._streamLengths = new long[this._parameterTypes.Length];
                this._parameterModes = this._pmdDescriptor.ParamModes;
                this._parameterNames = this._pmdDescriptor.ColumnLabels;
                this._isNamedParameters = this._pmdDescriptor.IsNamedParameters;
            }
            catch (Exception exception)
            {
                throw Error.GetError(0x1ca, exception.ToString());
            }
            this._resultOut = Result.NewPreparedExecuteRequest(this._parameterTypes, this._statementId);
            this._resultOut.SetStatement(r.GetStatement());
            return r;
        }

        private void SetOutParameterValues(Result resultIn)
        {
            object[] parameterData = resultIn.GetParameterData();
            for (int i = 0; i < this._parameterNames.Length; i++)
            {
                string str = this._parameterNames[i];
                UtlParameter parameter = this._command.Parameters[str];
                if ((parameter != null) && ((parameter.Direction == ParameterDirection.Output) || (parameter.Direction == ParameterDirection.InputOutput)))
                {
                    SqlType utlType = SqlType.GetUtlType(parameter.UtlType);
                    parameter.Value = this.GetColumnInType(parameterData[i], this._parameterTypes[i], utlType);
                }
            }
        }

        private void SetParameter(int i, UtlParameter param)
        {
            object o = param.Value;
            if ((o == null) || (o == DBNull.Value))
            {
                this._parameterValues[i] = null;
                return;
            }
            ISessionInterface sessionProxy = this._command.Connection.InnerConnection.SessionProxy;
            SqlType type = this._parameterTypes[i];
            int typeCode = type.TypeCode;
            if (typeCode <= 30)
            {
                if (typeCode > 0x15)
                {
                    switch (typeCode)
                    {
                        case 0x19:
                        case 30:
                            goto Label_028B;
                    }
                    this._parameterValues[i] = o;
                    return;
                }
                switch (typeCode)
                {
                    case 0x15:
                    {
                        DataTable dataTable = o as DataTable;
                        if (dataTable != null)
                        {
                            dataTable.TableName = this._parameterNames[i];
                            if (!((TableType) this._parameterTypes[i]).IsCompatible(dataTable))
                            {
                                goto Label_0379;
                            }
                            goto Label_03AF;
                        }
                        DbDataReader reader = o as DbDataReader;
                        if (reader == null)
                        {
                            goto Label_03AF;
                        }
                        dataTable = new DataTable(this._parameterNames[i]);
                        UtlMetaData.PopulateDataTable(dataTable, reader);
                        if (((TableType) this._parameterTypes[i]).IsCompatible(dataTable))
                        {
                            o = dataTable;
                            goto Label_03AF;
                        }
                        goto Label_0379;
                    }
                }
            }
            else
            {
                if (typeCode <= 0x3d)
                {
                    if (typeCode == 40)
                    {
                        this._parameterValues[i] = o;
                        return;
                    }
                    if ((typeCode - 60) > 1)
                    {
                        goto Label_028B;
                    }
                    byte[] data = o as byte[];
                    if (data != null)
                    {
                        o = new BinaryData(data, !this._command.Connection.IsNetConnection);
                    }
                    else
                    {
                        try
                        {
                            if (o is string)
                            {
                                o = type.ConvertToDefaultType(sessionProxy, o);
                                goto Label_03AF;
                            }
                        }
                        catch (CoreException exception1)
                        {
                            UtlException.ThrowError(exception1);
                        }
                        UtlException.ThrowError(Error.GetError(0x15bd));
                    }
                    goto Label_03AF;
                }
                switch (typeCode)
                {
                    case 0x5b:
                    case 0x5c:
                    case 0x5d:
                    case 0x5e:
                    case 0x5f:
                        try
                        {
                            if (((param.UtlType == UtlType.Null) && (o is string)) || (((param.UtlType == UtlType.VarChar) || (param.UtlType == UtlType.Char)) || (param.UtlType == UtlType.VarCharIngnoreCase)))
                            {
                                o = type.ConvertToType(sessionProxy, o, SqlType.SqlVarchar);
                            }
                            else
                            {
                                o = type.ConvertCSharpToSQL(sessionProxy, o);
                            }
                        }
                        catch (CoreException exception2)
                        {
                            UtlException.ThrowError(exception2);
                        }
                        goto Label_03AF;

                    case 0x65:
                    case 0x66:
                    case 0x6b:
                    {
                        MonthSpan span = (MonthSpan) o;
                        o = new IntervalMonthData((long) span.TotalMonths);
                        goto Label_03AF;
                    }
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
                    {
                        TimeSpan span2 = (TimeSpan) o;
                        long totalSeconds = (long) span2.TotalSeconds;
                        int nanos = (int) ((span2.TotalSeconds - totalSeconds) * 1000000000.0);
                        o = new IntervalSecondData(totalSeconds, nanos);
                        goto Label_03AF;
                    }
                    case 0x457:
                        try
                        {
                            if (o.GetType().IsSerializable)
                            {
                                o = new OtherData(o);
                                goto Label_03AF;
                            }
                        }
                        catch (CoreException exception3)
                        {
                            UtlException.ThrowError(exception3);
                        }
                        UtlException.ThrowError(Error.GetError(0x15bd));
                        goto Label_03AF;
                }
            }
        Label_028B:;
            try
            {
                if (((param.UtlType == UtlType.Null) && (o is string)) || (((param.UtlType == UtlType.VarChar) || (param.UtlType == UtlType.Char)) || (param.UtlType == UtlType.VarCharIngnoreCase)))
                {
                    o = type.ConvertToType(sessionProxy, o, SqlType.SqlVarchar);
                }
                else if (param.UtlType == UtlType.Null)
                {
                    o = type.ConvertToDefaultType(sessionProxy, o);
                }
                else
                {
                    SqlType type2 = this.UtlType2SqlType(param.UtlType);
                    o = type.ConvertToType(sessionProxy, o, type2);
                }
            }
            catch (CoreException exception4)
            {
                UtlException.ThrowError(exception4);
            }
            goto Label_03AF;
        Label_0379:;
            try
            {
                if (param.UtlType == UtlType.Null)
                {
                    o = type.ConvertToDefaultType(sessionProxy, o);
                }
                else
                {
                    SqlType type3 = this.UtlType2SqlType(param.UtlType);
                    o = type.ConvertToType(sessionProxy, o, type3);
                }
            }
            catch (CoreException exception5)
            {
                UtlException.ThrowError(exception5);
            }
        Label_03AF:
            this._parameterValues[i] = o;
        }

        private void SetReturnParameterValue(Result resultIn)
        {
            if (resultIn.IsData())
            {
                foreach (UtlParameter parameter in this._command.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.ReturnValue)
                    {
                        RowSetNavigator navigator = resultIn.GetNavigator();
                        if (navigator.Next())
                        {
                            object[] current = navigator.GetCurrent();
                            if (current.Length != 0)
                            {
                                SqlType sourceType = resultIn.MetaData.ColumnTypes[0];
                                SqlType utlType = SqlType.GetUtlType(parameter.UtlType);
                                parameter.Value = this.GetColumnInType(current[0], sourceType, utlType);
                            }
                        }
                        break;
                    }
                }
                resultIn.GetNavigator().Reset();
            }
        }

        private SqlType UtlType2SqlType(UtlType type)
        {
            switch (type)
            {
                case UtlType.Null:
                    return null;

                case UtlType.Boolean:
                    return SqlType.SqlBoolean;

                case UtlType.TinyInt:
                    return SqlType.Tinyint;

                case UtlType.SmallInt:
                    return SqlType.SqlSmallint;

                case UtlType.Int:
                    return SqlType.SqlInteger;

                case UtlType.BigInt:
                    return SqlType.SqlBigint;

                case UtlType.Decimal:
                    return SqlType.SqlDecimal;

                case UtlType.Double:
                    return SqlType.SqlDouble;

                case UtlType.Char:
                    return SqlType.SqlChar;

                case UtlType.VarChar:
                    return SqlType.SqlVarchar;

                case UtlType.VarCharIngnoreCase:
                    return SqlType.VarcharIgnorecase;

                case UtlType.UniqueIdentifier:
                    return SqlType.SqlUniqueIdentifier;

                case UtlType.Variant:
                    return SqlType.Other;

                case UtlType.Time:
                    return SqlType.SqlTime;

                case UtlType.TimeTZ:
                    return SqlType.SqlTimeWithTimeZone;

                case UtlType.TimeStamp:
                    return SqlType.SqlTimestamp;

                case UtlType.TimeStampTZ:
                    return SqlType.SqlTimestampWithTimeZone;

                case UtlType.Date:
                    return SqlType.SqlDate;

                case UtlType.IntervalDS:
                    return SqlType.SqlIntervalDayToSecond;

                case UtlType.IntervalYM:
                    return SqlType.SqlIntervalYearToMonth;

                case UtlType.Binary:
                case UtlType.VarBinary:
                    return SqlType.SqlBinary;

                case UtlType.Blob:
                    return SqlType.SqlBlob;

                case UtlType.Clob:
                    return SqlType.SqlClob;

                case UtlType.Money:
                    return SqlType.SqlDecimal;
            }
            return SqlType.Other;
        }

        public Queue<Result> ChainedResultSets
        {
            get
            {
                return this.ResultSets;
            }
        }

        public Result GeneratedResult
        {
            get
            {
                return this._generatedResult;
            }
        }
    }
}

