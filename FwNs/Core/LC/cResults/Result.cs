namespace FwNs.Core.LC.cResults
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.IO;

    public class Result
    {
        public static ResultMetaData SessionAttributesMetaData = GetSessionAttributesMetaData();
        private static readonly ResultMetaData EmptyMeta = ResultMetaData.NewResultMetaData(0);
        public static Result EmptyGeneratedResult = NewDataResult(EmptyMeta);
        public static Result UpdateZeroResult = NewUpdateCountResult(0);
        public static Result UpdateOneResult = NewUpdateCountResult(1);
        private readonly string _databaseName;
        private readonly string _zoneString;
        private Result _chainedResult;
        public int DatabaseId;
        private int _errorCode;
        private CoreException _exception;
        private int _fetchSize;
        private int _generateKeys;
        public ResultMetaData GeneratedMetaData;
        private long _id;
        private int _lobCount;
        private ResultLob _lobResults;
        private string _mainString;
        public ResultMetaData MetaData;
        public byte Mode;
        public RowSetNavigator navigator;
        public ResultMetaData ParameterMetaData;
        public int QueryTimeout;
        public int RsProperties;
        public long SessionId;
        public Statement statement;
        private long _statementId;
        private int _statementReturnType;
        private string _subString;
        public int UpdateCount;
        public object ValueData;

        public Result(int mode)
        {
            this._databaseName = string.Empty;
            this._zoneString = string.Empty;
            this._generateKeys = 2;
            this.Mode = (byte) mode;
        }

        public Result(int mode, int count)
        {
            this._databaseName = string.Empty;
            this._zoneString = string.Empty;
            this._generateKeys = 2;
            this.Mode = (byte) mode;
            this.UpdateCount = count;
        }

        public void AddChainedResult(Result result)
        {
            Result result2 = this;
            while (result2._chainedResult != null)
            {
                result2 = result2._chainedResult;
            }
            result2._chainedResult = result;
        }

        public void AddLobResult(ResultLob result)
        {
            Result result2 = this;
            while (result2._lobResults != null)
            {
                result2 = result2._lobResults;
            }
            result2._lobResults = result;
            this._lobCount++;
        }

        public void AddWarnings(CoreException[] warnings)
        {
            for (int i = 0; i < warnings.Length; i++)
            {
                Result result = NewWarningResult(warnings[i]);
                this.AddChainedResult(result);
            }
        }

        public void ClearLobResults()
        {
            this._lobResults = null;
            this._lobCount = 0;
        }

        public int GetActionType()
        {
            return this.UpdateCount;
        }

        public Result GetChainedResult()
        {
            return this._chainedResult;
        }

        public int GetConnectionAttrType()
        {
            return this.UpdateCount;
        }

        public int GetDatabaseId()
        {
            return this.DatabaseId;
        }

        public string GetDatabaseName()
        {
            return this._databaseName;
        }

        public int GetErrorCode()
        {
            return this._errorCode;
        }

        public CoreException GetException()
        {
            return this._exception;
        }

        public int GetExecuteProperties()
        {
            return this.RsProperties;
        }

        public int GetFetchSize()
        {
            return this._fetchSize;
        }

        public ResultMetaData GetGeneratedResultMetaData()
        {
            return this.GeneratedMetaData;
        }

        public int GetGeneratedResultType()
        {
            return this._generateKeys;
        }

        public int GetLobCount()
        {
            return this._lobCount;
        }

        public ResultLob GetLobResult()
        {
            return this._lobResults;
        }

        public string GetMainString()
        {
            return this._mainString;
        }

        public RowSetNavigator GetNavigator()
        {
            return this.navigator;
        }

        public object[] GetParameterData()
        {
            return (object[]) this.ValueData;
        }

        public long GetResultId()
        {
            return this._id;
        }

        public int GetResultType()
        {
            return this.Mode;
        }

        public object[] GetSessionAttributes()
        {
            return this.InitialiseNavigator().GetNext();
        }

        private static ResultMetaData GetSessionAttributesMetaData()
        {
            ResultMetaData data = ResultMetaData.NewResultMetaData(4);
            for (int i = 0; i < 4; i++)
            {
                data.columns[i] = new ColumnBase(null, null, null, null);
            }
            data.columns[0].SetType(SqlType.SqlInteger);
            data.columns[1].SetType(SqlType.SqlInteger);
            data.columns[2].SetType(SqlType.SqlBoolean);
            data.columns[3].SetType(SqlType.SqlVarchar);
            data.PrepareData();
            return data;
        }

        public long GetSessionId()
        {
            return this.SessionId;
        }

        public object[] GetSingleRowData()
        {
            return ArrayUtil.ResizeArrayIfDifferent<object>(this.InitialiseNavigator().GetNext(), this.MetaData.GetColumnCount());
        }

        public Statement GetStatement()
        {
            return this.statement;
        }

        public long GetStatementId()
        {
            return this._statementId;
        }

        public int GetStatementType()
        {
            return this._statementReturnType;
        }

        public string GetSubString()
        {
            return this._subString;
        }

        public Result GetUnlinkChainedResult()
        {
            this._chainedResult = null;
            return this._chainedResult;
        }

        public int GetUpdateCount()
        {
            return this.UpdateCount;
        }

        public object GetValueObject()
        {
            return this.ValueData;
        }

        public string GetZoneString()
        {
            return this._zoneString;
        }

        public bool HasGeneratedKeys()
        {
            return ((this.Mode == 1) && (this._chainedResult > null));
        }

        public RowSetNavigator InitialiseNavigator()
        {
            byte mode = this.Mode;
            switch (mode)
            {
                case 6:
                case 8:
                    break;

                case 7:
                    goto Label_003E;

                case 15:
                case 3:
                    this.navigator.Reset();
                    return this.navigator;

                default:
                    if ((mode - 0x10) > 1)
                    {
                        goto Label_003E;
                    }
                    break;
            }
            this.navigator.BeforeFirst();
            return this.navigator;
        Label_003E:
            throw Error.RuntimeError(0xc9, "Result");
        }

        public bool IsData()
        {
            if (this.Mode != 3)
            {
                return (this.Mode == 15);
            }
            return true;
        }

        public bool IsError()
        {
            return (this.Mode == 2);
        }

        public bool IsSimpleValue()
        {
            return (this.Mode == 0x2a);
        }

        public bool IsUpdateCount()
        {
            return (this.Mode == 1);
        }

        public bool IsWarning()
        {
            return (this.Mode == 0x13);
        }

        public static Result NewBatchedExecuteRequest()
        {
            SqlType[] types = new SqlType[] { SqlType.SqlVarchar };
            Result result1 = NewResult(8);
            result1.MetaData = ResultMetaData.NewSimpleResultMetaData(types);
            return result1;
        }

        public static Result NewBatchedExecuteResponse(int[] updateCounts, Result generatedResult, Result e)
        {
            Result result = NewResult(0x10);
            result.AddChainedResult(generatedResult);
            result.AddChainedResult(e);
            SqlType[] types = new SqlType[] { SqlType.SqlInteger };
            result.MetaData = ResultMetaData.NewSimpleResultMetaData(types);
            object[][] table = new object[updateCounts.Length][];
            for (int i = 0; i < updateCounts.Length; i++)
            {
                table[i] = new object[] { updateCounts[i] };
            }
            ((RowSetNavigatorClient) result.navigator).SetData(table);
            return result;
        }

        public static Result NewCallResponse(SqlType[] types, long statementId, object[] values)
        {
            Result result1 = NewResult(0x2b);
            result1.MetaData = ResultMetaData.NewSimpleResultMetaData(types);
            result1._statementId = statementId;
            result1.ValueData = values;
            return result1;
        }

        public static Result NewDataHeadResult(ISessionInterface session, Result source, int offset, int count)
        {
            if ((offset + count) > source.navigator.GetSize())
            {
                count = source.navigator.GetSize() - offset;
            }
            Result result1 = NewResult(15);
            result1.MetaData = source.MetaData;
            result1.navigator = new RowSetNavigatorClient(source.navigator, offset, count);
            result1.navigator.SetId(source.navigator.GetId());
            result1.SetSession(session);
            result1.RsProperties = source.RsProperties;
            result1._fetchSize = source._fetchSize;
            return result1;
        }

        public static Result NewDataResult(ResultMetaData md)
        {
            Result result1 = NewResult(3);
            result1.navigator = new RowSetNavigatorClient();
            result1.MetaData = md;
            return result1;
        }

        public static Result NewDataRowsResult(RowSetNavigator navigator)
        {
            Result result1 = NewResult(14);
            result1.navigator = navigator;
            return result1;
        }

        public static Result NewErrorResult(Exception t)
        {
            return NewErrorResult(t, null);
        }

        public static Result NewErrorResult(Exception t, string statement)
        {
            Result result = NewResult(2);
            CoreException exception = t as CoreException;
            if (exception != null)
            {
                result._exception = exception;
                result._mainString = result._exception.GetMessage();
                result._subString = result._exception.GetSqlState();
                if (statement != null)
                {
                    result._mainString = result._mainString + " in statement [" + statement + "]";
                }
                result._errorCode = result._exception.GetErrorCode();
                return result;
            }
            if (t is OutOfMemoryException)
            {
                result._exception = Error.GetError(460, t);
                result._mainString = result._exception.GetMessage();
                result._subString = result._exception.GetSqlState();
                result._errorCode = result._exception.GetErrorCode();
                return result;
            }
            result._exception = Error.GetError(0x1ca, t);
            result._mainString = result._exception.GetMessage() + " " + t.Message;
            result._subString = result._exception.GetSqlState();
            result._errorCode = result._exception.GetErrorCode();
            if (statement != null)
            {
                result._mainString = result._mainString + " in statement [" + statement + "]";
            }
            return result;
        }

        public static Result NewExecuteDirectRequest()
        {
            return NewResult(0x22);
        }

        public static Result NewFreeStmtRequest(long statementId)
        {
            Result result1 = NewResult(0x24);
            result1._statementId = statementId;
            return result1;
        }

        public static Result NewPreparedExecuteRequest(SqlType[] types, long statementId)
        {
            Result result1 = NewResult(0x23);
            result1.MetaData = ResultMetaData.NewSimpleResultMetaData(types);
            result1._statementId = statementId;
            result1.ValueData = new object[0];
            return result1;
        }

        public static Result NewPrepareResponse(Statement statement)
        {
            Result result1 = NewResult(4);
            result1.statement = statement;
            result1._statementId = statement.GetId();
            int statementType = statement.GetStatementType();
            result1._statementReturnType = ((statementType == 0x55) || (statementType == 7)) ? 2 : 1;
            result1.MetaData = statement.GetResultMetaData();
            result1.ParameterMetaData = statement.GetParametersMetaData();
            return result1;
        }

        public static Result NewPrepareStatementRequest()
        {
            return NewResult(0x25);
        }

        public static Result NewPsmResult(object value)
        {
            Result result1 = NewResult(0x2a);
            result1.ValueData = value;
            return result1;
        }

        public static Result NewPsmResult(int type, string label, object value)
        {
            Result result1 = NewResult(0x2a);
            result1._errorCode = type;
            result1._mainString = label;
            result1.ValueData = value;
            return result1;
        }

        public static Result NewRequestDataResult(long id, int offset, int count)
        {
            Result result1 = NewResult(13);
            result1._id = id;
            result1.UpdateCount = offset;
            result1._fetchSize = count;
            return result1;
        }

        public static Result NewResult(RowSetNavigator nav)
        {
            return new Result(3) { navigator = nav };
        }

        public static Result NewResult(int type)
        {
            RowSetNavigator navigator = null;
            if (type <= 0x12)
            {
                switch (type)
                {
                    case 6:
                    case 0x11:
                        navigator = new RowSetNavigatorClient(1);
                        break;

                    case 8:
                        navigator = new RowSetNavigatorClient(4);
                        break;

                    case 0x10:
                        navigator = new RowSetNavigatorClient(4);
                        break;

                    case 0x12:
                        throw Error.RuntimeError(0xc9, "Result");
                }
            }
            return new Result(type) { navigator = navigator };
        }

        public static Result NewSessionAttributesResult()
        {
            Result result1 = NewResult(3);
            result1.navigator = new RowSetNavigatorClient(1);
            result1.MetaData = SessionAttributesMetaData;
            result1.navigator.Add(new object[4]);
            return result1;
        }

        public static Result NewSetSavepointRequest(string name)
        {
            Result result1 = NewResult(0x26);
            result1.SetConnectionAttrType(0x272b);
            result1.SetMainString(name);
            return result1;
        }

        public static Result NewSingleColumnResult(ResultMetaData meta)
        {
            Result result1 = NewResult(3);
            result1.MetaData = meta;
            result1.navigator = new RowSetNavigatorClient();
            return result1;
        }

        public static Result NewSingleColumnResult(string colName, SqlType type)
        {
            Result result1 = NewResult(3);
            result1.MetaData = ResultMetaData.NewResultMetaData(1);
            result1.MetaData.columns[0] = new ColumnBase(null, null, null, colName);
            result1.MetaData.columns[0].SetType(type);
            result1.MetaData.PrepareData();
            result1.navigator = new RowSetNavigatorClient(8);
            return result1;
        }

        public static Result NewSingleColumnStringResult(string colName, string contents)
        {
            Result result = NewSingleColumnResult("OPERATION", SqlType.SqlVarchar);
            using (StringReader reader = new StringReader(contents))
            {
                string str;
            Label_0017:
                str = null;
                try
                {
                    str = reader.ReadLine();
                }
                catch (Exception)
                {
                }
                if (str != null)
                {
                    object[] data = new object[] { str };
                    result.GetNavigator().Add(data);
                    goto Label_0017;
                }
            }
            return result;
        }

        public static Result NewUpdateCountResult(int count)
        {
            return new Result(1, count);
        }

        public static Result NewUpdateCountResult(ResultMetaData meta, int count)
        {
            Result result = NewDataResult(meta);
            Result result1 = NewResult(1);
            result1.UpdateCount = count;
            result1.AddChainedResult(result);
            return result1;
        }

        public static Result NewUpdateResultRequest(SqlType[] types, long id)
        {
            Result result1 = NewResult(0x29);
            result1.MetaData = ResultMetaData.NewUpdateResultMetaData(types);
            result1._id = id;
            result1.ValueData = new object[0];
            return result1;
        }

        public static Result NewUpdateZeroResult()
        {
            return new Result(1, 0);
        }

        public static Result NewWarningResult(CoreException w)
        {
            Result result1 = NewResult(0x13);
            result1._mainString = w.GetMessage();
            result1._subString = w.GetSqlState();
            result1._errorCode = w.GetErrorCode();
            return result1;
        }

        public void SetActionType(int type)
        {
            this.UpdateCount = type;
        }

        public void SetAsTransactionEndRequest(int subType, string savepoint)
        {
            this.Mode = 0x21;
            this.UpdateCount = subType;
            this._mainString = savepoint ?? "";
        }

        public void SetConnectionAttrType(int type)
        {
            this.UpdateCount = type;
        }

        public void SetDatabaseId(int id)
        {
            this.DatabaseId = id;
        }

        public void SetDataResultProperties(int maxRows, int fetchSize, int resultSetScrollability, int resultSetConcurrency, int resultSetHoldability)
        {
            this.UpdateCount = maxRows;
            this._fetchSize = fetchSize;
            this.RsProperties = ResultProperties.GetValueForAdo(resultSetScrollability, resultSetConcurrency, resultSetHoldability);
        }

        public void SetFetchSize(int count)
        {
            this._fetchSize = count;
        }

        public void SetGeneratedKeys(int[] generatedIndexes, string[] generatedNames)
        {
            this._generateKeys = 1;
            this.GeneratedMetaData = ResultMetaData.NewGeneratedColumnsMetaData(generatedIndexes, generatedNames);
        }

        public void SetMainString(string sql)
        {
            this._mainString = sql;
        }

        public void SetMaxRows(int count)
        {
            this.UpdateCount = count;
        }

        public void SetNavigator(RowSetNavigator navigator)
        {
            this.navigator = navigator;
        }

        public void SetPreparedExecuteProperties(object[] parameterValues, int maxRows, int fetchSize, int resultProps)
        {
            this.Mode = 0x23;
            this.ValueData = parameterValues;
            this.UpdateCount = maxRows;
            this._fetchSize = fetchSize;
            this.RsProperties = resultProps;
        }

        public void SetPreparedResultUpdateProperties(object[] parameterValues)
        {
            this.ValueData = parameterValues;
        }

        public void SetPrepareOrExecuteProperties(string sql, int maxRows, int fetchSize, int statementReturnType, int timeout, int resultSetProperties, int keyMode, int[] generatedIndexes, string[] generatedNames)
        {
            this._mainString = sql;
            this.UpdateCount = maxRows;
            this._fetchSize = fetchSize;
            this._statementReturnType = statementReturnType;
            this.QueryTimeout = timeout;
            this.RsProperties = resultSetProperties;
            this._generateKeys = keyMode;
            this.GeneratedMetaData = ResultMetaData.NewGeneratedColumnsMetaData(generatedIndexes, generatedNames);
        }

        public void SetResultId(long id)
        {
            this._id = id;
            if (this.navigator != null)
            {
                this.navigator.SetId(id);
            }
        }

        public void SetResultType(int type)
        {
            this.Mode = (byte) type;
        }

        public void SetSession(ISessionInterface session)
        {
            if (this.navigator != null)
            {
                this.navigator.SetSession(session);
            }
        }

        public void SetSessionId(long id)
        {
            this.SessionId = id;
        }

        public void SetStatement(Statement statement)
        {
            this.statement = statement;
        }

        public void SetStatementId(long statementId)
        {
            this._statementId = statementId;
        }

        public void SetStatementType(int type)
        {
            this._statementReturnType = type;
        }

        public void SetUpdateCount(int count)
        {
            this.UpdateCount = count;
        }

        public void SetValueObject(object value)
        {
            this.ValueData = value;
        }
    }
}

