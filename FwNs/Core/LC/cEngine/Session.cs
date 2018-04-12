namespace FwNs.Core.LC.cEngine
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.LibCore;
    using System.IO;
    using System.Text;
    using System.Threading;

    public sealed class Session : ISessionInterface, IDisposable
    {
        private bool _isClosed;
        public Database database;
        private readonly User _sessionUser;
        private User _user;
        private Grantee _role;
        private bool _isReadOnlyDefault;
        private int _isolationLevelDefault = 0x1000;
        public int IsolationLevel = 0x1000;
        public int ActionIndex;
        public long ActionTimestamp;
        public long TransactionTimestamp;
        public long TransactionEndTimestamp;
        public bool IsPreTransaction;
        public bool IsTransaction;
        public bool IsBatch;
        public bool AbortTransaction;
        public bool RedoAction;
        public List<RowAction> RowActionList;
        public bool TempUnlocked;
        public OrderedHashSet<Session> WaitedSessions;
        public OrderedHashSet<Session> WaitingSessions;
        public OrderedHashSet<Session> TempSet;
        public CountUpDownLatch Latch = new CountUpDownLatch();
        public Statement LockStatement;
        public string ZoneString;
        public int SessionTimeZoneSeconds;
        public int TimeZoneSeconds;
        public bool IsNetwork;
        private int _sessionMaxRows;
        private object _lastIdentity = 0;
        private readonly long _sessionId;
        public int SessionTxId = -1;
        private bool _script;
        public bool IgnoreCase;
        private DbConnection _intConnection;
        public QNameManager.QName CurrentSchema;
        public QNameManager.QName LoggedSchema;
        public ParserCommand Parser;
        private bool _isProcessingScript;
        private bool _isProcessingLog;
        public SessionContext sessionContext;
        public int ResultMaxMemoryRows;
        public SessionData sessionData;
        public StatementManager statementManager;
        private Scanner _scanner;
        private readonly long _connectTime = DateTime.Now.Ticks;
        private long _currentTimestampScn;
        private long _currentMillis;
        private TimestampData _currentDate;
        private TimestampData _currentTimestamp;
        private TimestampData _localTimestamp;
        private TimeData _currentTime;
        private TimeData _localTime;
        private HashMappedList<string, Table> _sessionTables;
        private Queue<CoreException> _sqlWarnings;
        private Queue<Result> _resultSets;
        private TimeZoneInfo _timezone;
        private Scanner _secondaryScanner;
        private System.Random _randomGenerator = new System.Random();
        private long _seed = -1L;

        public Session(Database db, User user, bool autocommit, bool rdy, long id, string zoneString, int timeZoneSeconds)
        {
            this._sessionId = id;
            this.database = db;
            this._user = user;
            this._sessionUser = user;
            this.ZoneString = zoneString;
            this.SessionTimeZoneSeconds = timeZoneSeconds;
            this.TimeZoneSeconds = timeZoneSeconds;
            this.RowActionList = new List<RowAction>();
            this.WaitedSessions = new OrderedHashSet<Session>();
            this.WaitingSessions = new OrderedHashSet<Session>();
            this.TempSet = new OrderedHashSet<Session>();
            this._isolationLevelDefault = this.database.GetDefaultIsolationLevel();
            this.IsolationLevel = this._isolationLevelDefault;
            SessionContext context1 = new SessionContext(this) {
                IsAutoCommit = autocommit,
                IsReadOnly = rdy
            };
            this.sessionContext = context1;
            this._scanner = new Scanner();
            this.Parser = new ParserCommand(this, this._scanner);
            this.SetResultMemoryRowCount(this.database.GetResultMaxMemoryRows());
            this.ResetSchema();
            this.sessionData = new SessionData(this.database, this);
            this.statementManager = new StatementManager(this.database);
        }

        public void AddDeleteAction(Table table, Row row, int[] colMap)
        {
            bool abortTransaction = this.AbortTransaction;
            this.database.TxManager.AddDeleteAction(this, table, row, colMap);
        }

        public void AddInsertAction(Table table, Row row)
        {
            this.database.TxManager.AddInsertAction(this, table, row);
            bool abortTransaction = this.AbortTransaction;
        }

        public void AddResultSet(Result result)
        {
            if (this._resultSets == null)
            {
                this._resultSets = new Queue<Result>();
            }
            this._resultSets.Enqueue(result);
        }

        public void AddSessionTable(Table table)
        {
            if (this._sessionTables == null)
            {
                this._sessionTables = new HashMappedList<string, Table>();
            }
            if (this._sessionTables.ContainsKey(table.GetName().Name))
            {
                throw Error.GetError(0x1580);
            }
            this._sessionTables.Add(table.GetName().Name, table);
        }

        public void AddWarning(CoreException warning)
        {
            if (this._sqlWarnings == null)
            {
                this._sqlWarnings = new Queue<CoreException>();
            }
            if (this._sqlWarnings.Count > 9)
            {
                this._sqlWarnings.Dequeue();
            }
            if (!this._sqlWarnings.Contains(warning))
            {
                this._sqlWarnings.Enqueue(warning);
            }
        }

        public void AllocateResultLob(ResultLob result, Stream inputStream)
        {
            this.sessionData.AllocateLobForResult(result, inputStream);
        }

        private static void AppendIsolationSql(StringBuilder sb, int isolationLevel)
        {
            sb.Append("ISOLATION").Append(' ');
            sb.Append("LEVEL").Append(' ');
            if (isolationLevel <= 0x1000)
            {
                if ((isolationLevel == 0x100) || (isolationLevel == 0x1000))
                {
                    sb.Append("READ").Append(' ');
                    sb.Append("COMMITTED");
                }
            }
            else if ((isolationLevel == 0x10000) || (isolationLevel == 0x100000))
            {
                sb.Append("SERIALIZABLE");
            }
        }

        public void BeginAction(Statement cs)
        {
            this.ActionIndex = this.RowActionList.Count;
            this.database.TxManager.BeginAction(this, cs);
            this.database.TxManager.BeginActionResume(this);
        }

        public void CheckAdmin()
        {
            this._user.CheckAdmin();
        }

        public void CheckDdlWrite()
        {
            this.CheckReadWrite();
            if (!this._isProcessingScript)
            {
                bool flag1 = this._isProcessingLog;
            }
        }

        public void CheckReadWrite()
        {
            if (this.sessionContext.IsReadOnly)
            {
                throw Error.GetError(0xe7a);
            }
        }

        public void ClearWarnings()
        {
            if (this._sqlWarnings != null)
            {
                this._sqlWarnings.Clear();
            }
        }

        public void Close()
        {
            lock (this)
            {
                if (!this.IsClosed())
                {
                    this.Rollback(false);
                    try
                    {
                        this.database.logger.WriteToLog(this, "DISCONNECT");
                    }
                    catch (Exception)
                    {
                    }
                    this.sessionData.CloseAllNavigators();
                    this.sessionData.persistentStoreCollection.ClearAllTables();
                    this.sessionData.CloseResultCache();
                    this.statementManager.Reset();
                    this.database.sessionManager.RemoveSession(this);
                    this.database.CloseIfLast();
                    this.database = null;
                    this._user = null;
                    this.RowActionList = null;
                    this.sessionContext.Savepoints = null;
                    this._intConnection = null;
                    this._lastIdentity = null;
                    this._isClosed = true;
                }
            }
        }

        public void CloseNavigator(long id)
        {
            lock (this)
            {
                this.sessionData.CloseNavigator(id);
            }
        }

        public void Commit(bool chain)
        {
            lock (this)
            {
                if (!this._isClosed && (this.sessionContext.Depth <= 0))
                {
                    if (!this.IsTransaction)
                    {
                        this.sessionContext.IsReadOnly = this._isReadOnlyDefault;
                        this.IsolationLevel = this._isolationLevelDefault;
                    }
                    else
                    {
                        if (!this.database.TxManager.CommitTransaction(this))
                        {
                            this.Rollback(false);
                            throw Error.GetError(0x12fd);
                        }
                        this.EndTransaction(true);
                        if ((this.database != null) && this.database.logger.NeedsCheckpointReset())
                        {
                            Statement checkpointStatement = ParserCommand.GetCheckpointStatement(this.database, false);
                            this.ExecuteCompiledStatement(checkpointStatement, new object[0]);
                        }
                    }
                }
            }
        }

        public Statement CompileStatement(string sql, int props)
        {
            this.Parser.Reset(sql);
            return this.Parser.CompileStatement(props);
        }

        public BlobDataId CreateBlob(long length)
        {
            long id = this.database.lobManager.CreateBlob(length);
            if (id == 0)
            {
                throw Error.GetError(0xd92);
            }
            return new BlobDataId(id);
        }

        public ClobDataId CreateClob(long length)
        {
            long id = this.database.lobManager.CreateClob(length);
            if (id == 0)
            {
                throw Error.GetError(0xd92);
            }
            return new ClobDataId(id);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.sessionData.Dispose();
                this.Latch.Dispose();
                if (this._intConnection != null)
                {
                    this._intConnection.Dispose();
                }
                this._scanner.Dispose();
            }
        }

        public void DropSessionTable(string name)
        {
            this._sessionTables.Remove(name);
        }

        public void EndAction(Result r)
        {
            this.sessionData.persistentStoreCollection.ClearStatementTables();
            if (r.Mode == 2)
            {
                this.sessionData.persistentStoreCollection.ClearResultTables(this.ActionTimestamp);
                this.database.TxManager.RollbackAction(this);
            }
            else
            {
                this.database.TxManager.CompleteActions(this);
            }
        }

        private void EndTransaction(bool commit)
        {
            this.sessionContext.Savepoints.Clear();
            this.sessionContext.SavepointTimestamps.Clear();
            this.RowActionList.Clear();
            this.sessionData.persistentStoreCollection.ClearTransactionTables();
            this.sessionData.CloseAllTransactionNavigators();
            this.sessionContext.IsReadOnly = this._isReadOnlyDefault;
            this.IsolationLevel = this._isolationLevelDefault;
            this.LockStatement = null;
        }

        public Result Execute(Result cmd)
        {
            lock (this)
            {
                Statement statement;
                if (this._isClosed)
                {
                    return Result.NewErrorResult(Error.GetError(0x549));
                }
                this.sessionContext.CurrentMaxRows = 0;
                this.IsBatch = false;
                switch (cmd.Mode)
                {
                    case 0x22:
                    {
                        Result result2 = this.ExecuteDirectStatement(cmd);
                        return this.PerformPostExecute(cmd, result2);
                    }
                    case 0x23:
                    {
                        int updateCount = cmd.GetUpdateCount();
                        this.sessionContext.CurrentMaxRows = (updateCount == -1) ? 0 : updateCount;
                        statement = cmd.statement;
                        if ((statement != null) && (statement.CompileTimestamp >= this.database.schemaManager.GetSchemaChangeTimestamp()))
                        {
                            break;
                        }
                        long statementId = cmd.GetStatementId();
                        statement = this.statementManager.GetStatement(this, statementId);
                        if (statement != null)
                        {
                            break;
                        }
                        return Result.NewErrorResult(Error.GetError(0x4e4));
                    }
                    case 0x24:
                        this.statementManager.FreeStatement(cmd.GetStatementId());
                        return Result.UpdateZeroResult;

                    case 0x25:
                    {
                        Statement statement2;
                        try
                        {
                            statement2 = this.statementManager.Compile(this, cmd);
                        }
                        catch (Exception exception1)
                        {
                            string mainString = cmd.GetMainString();
                            if (LibCoreDatabaseProperties.GetErrorLevel() == 1)
                            {
                                mainString = null;
                            }
                            return Result.NewErrorResult(exception1, mainString);
                        }
                        Result result5 = Result.NewPrepareResponse(statement2);
                        if ((statement2.GetStatementType() == 0x55) || (statement2.GetStatementType() == 7))
                        {
                            this.sessionData.SetResultSetProperties(cmd, result5);
                        }
                        return this.PerformPostExecute(cmd, result5);
                    }
                    case 40:
                        this.CloseNavigator(cmd.GetResultId());
                        return Result.UpdateZeroResult;

                    case 0x29:
                    {
                        Result result6 = this.ExecuteResultUpdate(cmd);
                        return this.PerformPostExecute(cmd, result6);
                    }
                    case 0x12:
                        return this.PerformLobOperation((ResultLob) cmd);

                    case 8:
                    {
                        this.IsBatch = true;
                        Result result7 = this.ExecuteDirectBatchStatement(cmd);
                        return this.PerformPostExecute(cmd, result7);
                    }
                    default:
                        return Result.NewErrorResult(Error.RuntimeError(0xc9, "Session"));
                }
                object[] valueData = (object[]) cmd.ValueData;
                Result result = this.ExecuteCompiledStatement(statement, valueData, (cmd.ParameterMetaData != null) ? cmd.ParameterMetaData.GetParameterTypes() : null);
                return this.PerformPostExecute(cmd, result);
            }
        }

        public Result ExecuteCompiledStatement(Statement cs, object[] pvals)
        {
            return this.ExecuteCompiledStatement(cs, pvals, null);
        }

        public Result ExecuteCompiledStatement(Statement cs, object[] pvals, SqlType[] ptypes)
        {
            if (this.AbortTransaction)
            {
                this.Rollback(false);
                return Result.NewErrorResult(Error.GetError(0x12fd));
            }
            if ((this.sessionContext.Depth > 0) && this.sessionContext.NoSql)
            {
                return Result.NewErrorResult(Error.GetError(0x1770));
            }
            if (cs.IsAutoCommitStatement())
            {
                if (this.IsReadOnly())
                {
                    throw Error.GetError(0xe7a);
                }
                try
                {
                    this.Commit(false);
                }
                catch (CoreException)
                {
                    this.database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.Session_ExecuteCompiledStatement_Exception_at_commit);
                }
            }
            this.sessionContext.CurrentStatement = cs;
            if (!cs.IsTransactionStatement())
            {
                Result result2 = cs.Execute(this);
                this.sessionContext.CurrentStatement = null;
                if (result2.IsError())
                {
                    CoreException t = result2.GetException();
                    this.database.logger.LogSevereEvent(cs.Sql, t.GetMessage(), t);
                }
                return result2;
            }
        Label_00EC:
            this.ActionIndex = this.RowActionList.Count;
            this.database.TxManager.BeginAction(this, cs);
            if (!this.AbortTransaction)
            {
                try
                {
                    this.Latch.Await();
                }
                catch (AbandonedMutexException)
                {
                }
                if (this.AbortTransaction)
                {
                    this.Rollback(false);
                    this.sessionContext.CurrentStatement = null;
                    return Result.NewErrorResult(Error.GetError(0x12fd));
                }
                this.database.TxManager.BeginActionResume(this);
                if (ptypes != null)
                {
                    this.MaterializeTableParameters(pvals, ptypes);
                }
                this.sessionContext.SetDynamicArguments(pvals);
                Result r = cs.Execute(this);
                if (r.IsError())
                {
                    CoreException exception = r.GetException();
                    this.database.logger.LogSevereEvent(cs.Sql, exception.GetMessage(), exception);
                }
                this.LockStatement = this.sessionContext.CurrentStatement;
                this.EndAction(r);
                if (this.AbortTransaction)
                {
                    this.Rollback(false);
                    this.sessionContext.CurrentStatement = null;
                    return Result.NewErrorResult(Error.GetError(0x12fd));
                }
                if (!this.RedoAction)
                {
                    if ((this.sessionContext.Depth == 0) && (this.sessionContext.IsAutoCommit || cs.IsAutoCommitStatement()))
                    {
                        try
                        {
                            if (r.Mode == 2)
                            {
                                this.Rollback(false);
                            }
                            else
                            {
                                this.Commit(false);
                            }
                        }
                        catch (Exception exception3)
                        {
                            this.sessionContext.CurrentStatement = null;
                            return Result.NewErrorResult(Error.GetError(0x12fd, exception3));
                        }
                    }
                    this.sessionContext.CurrentStatement = null;
                    return r;
                }
                this.RedoAction = false;
                this.Latch.Await();
                goto Label_00EC;
            }
            this.Rollback(false);
            this.sessionContext.CurrentStatement = null;
            return Result.NewErrorResult(Error.GetError(0x12fd));
        }

        private Result ExecuteDirectBatchStatement(Result cmd)
        {
            int count = 0;
            RowSetNavigator navigator = cmd.InitialiseNavigator();
            int[] source = new int[navigator.GetSize()];
            Result e = null;
            while (navigator.HasNext())
            {
                Result result2;
                string sql = (string) navigator.GetNext()[0];
                try
                {
                    result2 = this.ExecuteDirectStatement(sql, ResultProperties.DefaultPropsValue);
                }
                catch (Exception exception1)
                {
                    result2 = Result.NewErrorResult(exception1);
                }
                if (!result2.IsUpdateCount())
                {
                    if (!result2.IsData())
                    {
                        if (result2.Mode != 2)
                        {
                            throw Error.RuntimeError(0xc9, "Session");
                        }
                        source = ArrayUtil.ArraySlice(source, 0, count);
                        e = result2;
                        break;
                    }
                    source[count++] = -2;
                }
                else
                {
                    source[count++] = result2.GetUpdateCount();
                    continue;
                }
            }
            return Result.NewBatchedExecuteResponse(source, null, e);
        }

        public Result ExecuteDirectStatement(Result cmd)
        {
            List<Statement> list;
            string mainString = cmd.GetMainString();
            int updateCount = cmd.GetUpdateCount();
            if (updateCount == -1)
            {
                this.sessionContext.CurrentMaxRows = 0;
            }
            else if (this._sessionMaxRows == 0)
            {
                this.sessionContext.CurrentMaxRows = updateCount;
            }
            else
            {
                this.sessionContext.CurrentMaxRows = this._sessionMaxRows;
            }
            try
            {
                list = this.Parser.CompileStatements(mainString, cmd);
            }
            catch (Exception exception)
            {
                this.database.logger.LogSevereEvent(mainString, exception.Message, exception);
                return Result.NewErrorResult(exception);
            }
            Result result = null;
            for (int i = 0; i < list.Count; i++)
            {
                Statement cs = list[i];
                cs.SetGeneratedColumnInfo(cmd.GetGeneratedResultType(), cmd.GetGeneratedResultMetaData());
                result = this.ExecuteCompiledStatement(cs, new object[0]);
                if (result.Mode == 2)
                {
                    CoreException t = result.GetException();
                    this.database.logger.LogSevereEvent(mainString, t.GetMessage(), t);
                    return result;
                }
            }
            return result;
        }

        public Result ExecuteDirectStatement(string sql, int props)
        {
            Statement statement;
            this.Parser.Reset(sql);
            try
            {
                statement = this.Parser.CompileStatement(props);
            }
            catch (CoreException exception1)
            {
                return Result.NewErrorResult(exception1);
            }
            return this.ExecuteCompiledStatement(statement, new object[0]);
        }

        private Result ExecuteResultUpdate(Result cmd)
        {
            long resultId = cmd.GetResultId();
            int actionType = cmd.GetActionType();
            Result dataResult = this.sessionData.GetDataResult(resultId);
            if (dataResult == null)
            {
                return Result.NewErrorResult(Error.GetError(0xe11));
            }
            object[] valueData = (object[]) cmd.ValueData;
            SqlType[] columnTypes = cmd.MetaData.ColumnTypes;
            Table baseTable = ((StatementQuery) dataResult.GetStatement()).queryExpression.GetBaseTable();
            int[] baseTableColumnMap = ((StatementQuery) dataResult.GetStatement()).queryExpression.GetBaseTableColumnMap();
            if (this.sessionContext.RowUpdateStatement == null)
            {
                this.sessionContext.RowUpdateStatement = new StatementResultUpdate();
            }
            this.sessionContext.RowUpdateStatement.SetRowActionProperties(dataResult, actionType, baseTable, columnTypes, baseTableColumnMap);
            return this.ExecuteCompiledStatement(this.sessionContext.RowUpdateStatement, valueData);
        }

        public Table FindSessionTable(string name)
        {
            if (this._sessionTables == null)
            {
                return null;
            }
            return this._sessionTables.Get(name);
        }

        public long GetActionTimestamp()
        {
            return this.ActionTimestamp;
        }

        public Result[] GetAndClearResultSets()
        {
            if (this._resultSets == null)
            {
                return null;
            }
            this._resultSets = null;
            return this._resultSets.ToArray();
        }

        public CoreException[] GetAndClearWarnings()
        {
            if (this._sqlWarnings == null)
            {
                return CoreException.EmptyArray;
            }
            this._sqlWarnings.Clear();
            return this._sqlWarnings.ToArray();
        }

        public object GetAttribute(int id)
        {
            lock (this)
            {
                switch (id)
                {
                    case 3:
                        return this.IsolationLevel;

                    case 4:
                        return this.sessionContext.IsAutoCommit;

                    case 6:
                        return this.sessionContext.IsReadOnly;

                    case 7:
                        return this.database.GetCatalogName().Name;
                }
                return null;
            }
        }

        public long GetConnectTime()
        {
            return this._connectTime;
        }

        public TimestampData GetCurrentDate()
        {
            lock (this)
            {
                this.ResetCurrentTimestamp();
                TimestampData data2 = this._currentDate;
                if (data2 == null)
                {
                    data2 = this._currentDate = (TimestampData) SqlType.SqlDate.GetValue(this._currentMillis / 0x3e8L, 0, this.GetZoneSeconds());
                }
                return data2;
            }
        }

        public QNameManager.QName GetCurrentSchemaQName()
        {
            return this.CurrentSchema;
        }

        public TimeData GetCurrentTime(bool withZone)
        {
            lock (this)
            {
                this.ResetCurrentTimestamp();
                if (withZone)
                {
                    if (this._currentTime == null)
                    {
                        long seconds = this._currentMillis / 0x3e8L;
                        int nanos = (int) ((this._currentMillis % 0x3e8L) * 0xf4240L);
                        this._currentTime = new TimeData(seconds, nanos, this.GetZoneSeconds());
                    }
                    return this._currentTime;
                }
                if (this._localTime == null)
                {
                    long seconds = (this._currentMillis + (this.GetZoneSeconds() * 0x3e8)) / 0x3e8L;
                    int nanos = (int) ((this._currentMillis % 0x3e8L) * 0xf4240L);
                    this._localTime = new TimeData(seconds, nanos, 0);
                }
                return this._localTime;
            }
        }

        public TimestampData GetCurrentTimestamp(bool withZone)
        {
            lock (this)
            {
                this.ResetCurrentTimestamp();
                if (withZone)
                {
                    if (this._currentTimestamp == null)
                    {
                        int nanos = (int) ((this._currentMillis % 0x3e8L) * 0xf4240L);
                        this._currentTimestamp = new TimestampData(this._currentMillis / 0x3e8L, nanos, this.GetZoneSeconds());
                    }
                    return this._currentTimestamp;
                }
                if (this._localTimestamp == null)
                {
                    int nanos = (int) ((this._currentMillis % 0x3e8L) * 0xf4240L);
                    this._localTimestamp = new TimestampData((this._currentMillis / 0x3e8L) + this.GetZoneSeconds(), nanos, 0);
                }
                return this._localTimestamp;
            }
        }

        public Database GetDatabase()
        {
            return this.database;
        }

        public Grantee GetGrantee()
        {
            return this._user;
        }

        public long GetId()
        {
            return this._sessionId;
        }

        public DbConnection GetInternalConnection()
        {
            if (this._intConnection == null)
            {
                this._intConnection = new UtlConnection(this);
            }
            return this._intConnection;
        }

        public string GetInternalConnectionUrl()
        {
            return ("jdbc:hsqldb:" + this.database.GetUri());
        }

        public int GetIsolation()
        {
            lock (this)
            {
                return this.IsolationLevel;
            }
        }

        public object GetLastIdentity()
        {
            return this._lastIdentity;
        }

        public CoreException GetLastWarning()
        {
            if ((this._sqlWarnings == null) || (this._sqlWarnings.Count == 0))
            {
                return null;
            }
            return this._sqlWarnings.Peek();
        }

        public int GetMaxRows()
        {
            return this.sessionContext.CurrentMaxRows;
        }

        public int GetResultMemoryRowCount()
        {
            return this.ResultMaxMemoryRows;
        }

        public Grantee GetRole()
        {
            return this._role;
        }

        public RowSetNavigatorClient GetRows(long navigatorId, int offset, int blockSize)
        {
            return this.sessionData.GetRowSetSlice(navigatorId, offset, blockSize);
        }

        private static string GetSavepointRollbackSql(string name)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("ROLLBACK").Append(' ').Append("TO").Append(' ');
            builder1.Append("SAVEPOINT").Append(' ');
            builder1.Append('"').Append(name).Append('"');
            return builder1.ToString();
        }

        private static string GetSavepointSql(string name)
        {
            StringBuilder builder1 = new StringBuilder("SAVEPOINT");
            builder1.Append(' ').Append('"').Append(name).Append('"');
            return builder1.ToString();
        }

        public Scanner GetScanner()
        {
            Scanner scanner = this._secondaryScanner;
            if (scanner == null)
            {
                scanner = this._secondaryScanner = new Scanner();
            }
            return scanner;
        }

        public string GetSchemaName(string name)
        {
            if (name != null)
            {
                return this.database.schemaManager.GetSchemaName(name);
            }
            return this.CurrentSchema.Name;
        }

        public QNameManager.QName GetSchemaQName(string name)
        {
            if (name != null)
            {
                return this.database.schemaManager.GetSchemaQName(name);
            }
            return this.CurrentSchema;
        }

        private string GetSessionIsolationSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SET").Append(' ').Append("SESSION");
            sb.Append(' ').Append("CHARACTERISTICS").Append(' ');
            sb.Append("AS").Append(' ').Append("TRANSACTION").Append(' ');
            AppendIsolationSql(sb, this._isolationLevelDefault);
            return sb.ToString();
        }

        public int GetSqlMaxRows()
        {
            return this._sessionMaxRows;
        }

        public int GetStreamBlockSize()
        {
            lock (this)
            {
                return 0x80000;
            }
        }

        public TimeZoneInfo GetTimeZone()
        {
            if (string.IsNullOrEmpty(this.ZoneString))
            {
                this._timezone = TimeZoneInfo.get_Utc();
            }
            else
            {
                this._timezone = TimeZoneInfo.FindSystemTimeZoneById(this.ZoneString);
                if (this._timezone == null)
                {
                    this._timezone = TimeZoneInfo.get_Local();
                }
            }
            return this._timezone;
        }

        private string GetTransactionIsolationSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SET").Append(' ').Append("TRANSACTION");
            sb.Append(' ');
            AppendIsolationSql(sb, this.IsolationLevel);
            return sb.ToString();
        }

        public int GetTransactionSize()
        {
            return this.RowActionList.Count;
        }

        public long GetTransactionTimestamp()
        {
            return this.TransactionTimestamp;
        }

        public User GetUser()
        {
            return this._user;
        }

        public string GetUsername()
        {
            return this._user.GetNameString();
        }

        public int GetZoneSeconds()
        {
            return this.TimeZoneSeconds;
        }

        public bool HasLocks(Statement statement)
        {
            if (this.LockStatement == statement)
            {
                if ((this.IsolationLevel == 0x10000) || (this.IsolationLevel == 0x100000))
                {
                    return true;
                }
                if (statement.GetTableNamesForRead().Length == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAdmin()
        {
            return this._user.IsAdmin();
        }

        public bool IsAutoCommit()
        {
            lock (this)
            {
                return this.sessionContext.IsAutoCommit;
            }
        }

        public bool IsClosed()
        {
            return this._isClosed;
        }

        public bool IsInMidTransaction()
        {
            return this.IsTransaction;
        }

        public bool IsProcessingLog()
        {
            return this._isProcessingLog;
        }

        public bool IsProcessingScript()
        {
            return this._isProcessingScript;
        }

        public bool IsReadOnly()
        {
            return this.sessionContext.IsReadOnly;
        }

        public bool IsReadOnlyDefault()
        {
            lock (this)
            {
                return this._isReadOnlyDefault;
            }
        }

        public void LogSequences()
        {
            OrderedHashSet<NumberSequence> sequenceUpdateSet = this.sessionData.SequenceUpdateSet;
            if ((sequenceUpdateSet != null) && !sequenceUpdateSet.IsEmpty())
            {
                int index = 0;
                int num2 = sequenceUpdateSet.Size();
                while (index < num2)
                {
                    NumberSequence s = sequenceUpdateSet.Get(index);
                    this.database.logger.WriteSequenceStatement(this, s);
                    index++;
                }
                this.sessionData.SequenceUpdateSet.Clear();
            }
        }

        private void MaterializeTableParameters(object[] pvals, SqlType[] ptypes)
        {
            for (int i = 0; i < ptypes.Length; i++)
            {
                if (ptypes[i].IsTableType())
                {
                    Table table = null;
                    TableType type = (TableType) ptypes[i];
                    DataTable dataTable = pvals[i] as DataTable;
                    if (dataTable == null)
                    {
                        UtlDataReader reader = pvals[i] as UtlDataReader;
                        if (reader == null)
                        {
                            throw Error.GetError(0x15b9);
                        }
                        table = type.MakeTable(this.database, reader);
                        IPersistentStore rowStore = this.sessionData.GetRowStore(table);
                        while (reader.Read())
                        {
                            object[] values = new object[reader.FieldCount];
                            reader.GetValues(values);
                            object[] data = (object[]) type.ConvertCSharpToSQL(this, values);
                            table.InsertData(this, rowStore, data);
                        }
                    }
                    else
                    {
                        table = type.MakeTable(this.database, dataTable);
                        IPersistentStore rowStore = this.sessionData.GetRowStore(table);
                        foreach (DataRow row in dataTable.Rows)
                        {
                            object[] data = (object[]) type.ConvertCSharpToSQL(this, row.ItemArray.Clone());
                            table.InsertData(this, rowStore, data);
                        }
                    }
                    pvals[i] = table;
                }
            }
        }

        private Result PerformLobOperation(ResultLob cmd)
        {
            long lobId = cmd.GetLobId();
            switch (cmd.GetSubType())
            {
                case 1:
                    return this.database.lobManager.GetBytes(lobId, cmd.GetOffset(), (int) cmd.GetBlockLength());

                case 2:
                    return this.database.lobManager.SetBytes(lobId, cmd.GetByteArray(), cmd.GetOffset());

                case 3:
                    return this.database.lobManager.GetChars(this, lobId, cmd.GetOffset(), (int) cmd.GetBlockLength());

                case 4:
                    return this.database.lobManager.SetChars(lobId, cmd.GetOffset(), cmd.GetCharArray());

                case 5:
                case 6:
                case 7:
                case 8:
                    throw Error.GetError(0x60f);

                case 9:
                    throw Error.GetError(0x60f);

                case 10:
                    return this.database.lobManager.GetLength(lobId);

                case 11:
                    return this.database.lobManager.GetLob(lobId, cmd.GetOffset(), cmd.GetBlockLength());
            }
            throw Error.RuntimeError(0xc9, "Session");
        }

        private Result PerformPostExecute(Result command, Result result)
        {
            if (result.Mode == 3)
            {
                result = this.sessionData.GetDataResultHead(command, result, this.IsNetwork);
            }
            if (this._resultSets != null)
            {
                Result[] andClearResultSets = this.GetAndClearResultSets();
                Result result2 = null;
                for (int i = 0; i < andClearResultSets.Length; i++)
                {
                    Result result3 = andClearResultSets[i];
                    result3 = this.sessionData.GetDataResultHead(command, result3, this.IsNetwork);
                    if (result2 == null)
                    {
                        result2 = result3;
                    }
                    else
                    {
                        result2.AddChainedResult(result3);
                    }
                }
                result = result2;
            }
            if ((this._sqlWarnings != null) && (this._sqlWarnings.Count > 0))
            {
                if (result.Mode == 1)
                {
                    result = new Result(1, result.GetUpdateCount());
                }
                CoreException[] andClearWarnings = this.GetAndClearWarnings();
                result.AddWarnings(andClearWarnings);
            }
            return result;
        }

        public void PrepareCommit()
        {
            lock (this)
            {
                if (this._isClosed)
                {
                    throw Error.GetError(0x517);
                }
                if (!this.database.TxManager.PrepareCommitActions(this))
                {
                    this.Rollback(false);
                    throw Error.GetError(0x12fd);
                }
            }
        }

        public double Random()
        {
            return this._randomGenerator.NextDouble();
        }

        public double Random(int seed)
        {
            if (this._seed != seed)
            {
                this._randomGenerator = new System.Random(seed);
                this._seed = seed;
            }
            return this._randomGenerator.NextDouble();
        }

        public void ReleaseSavepoint(string name)
        {
            lock (this)
            {
                int index = this.sessionContext.Savepoints.GetIndex(name);
                if (index >= 0)
                {
                    goto Label_005C;
                }
                throw Error.GetError(0x12d5, name);
            Label_0028:
                this.sessionContext.Savepoints.Remove((int) (this.sessionContext.Savepoints.Size() - 1));
                this.sessionContext.SavepointTimestamps.RemoveLast();
            Label_005C:
                if (this.sessionContext.Savepoints.Size() > index)
                {
                    goto Label_0028;
                }
            }
        }

        private void ResetCurrentTimestamp()
        {
            if (this._currentTimestampScn != this.ActionTimestamp)
            {
                this._currentTimestampScn = this.ActionTimestamp;
                this._currentMillis = DateTime.Now.Ticks / 0x2710L;
                this._currentDate = null;
                this._currentTimestamp = null;
                this._localTimestamp = null;
                this._currentTime = null;
                this._localTime = null;
            }
        }

        public void ResetSchema()
        {
            this.LoggedSchema = null;
            this.CurrentSchema = this._user.GetInitialOrDefaultSchema();
        }

        public void ResetSession()
        {
            lock (this)
            {
                this.Rollback(false);
                this.sessionData.CloseAllNavigators();
                this.sessionData.persistentStoreCollection.ClearAllTables();
                this.sessionData.CloseResultCache();
                this.statementManager.Reset();
                this._lastIdentity = 0;
                this.SetResultMemoryRowCount(this.database.GetResultMaxMemoryRows());
                this._user = this._sessionUser;
                this.ResetSchema();
                this.SetZoneSeconds(this.SessionTimeZoneSeconds);
                this._sessionMaxRows = 0;
                this.IgnoreCase = false;
            }
        }

        public void Rollback(bool chain)
        {
            lock (this)
            {
                if (!this._isClosed && (this.sessionContext.Depth <= 0))
                {
                    if (!this.IsTransaction)
                    {
                        this.sessionContext.IsReadOnly = this._isReadOnlyDefault;
                        this.IsolationLevel = this._isolationLevelDefault;
                    }
                    else
                    {
                        try
                        {
                            this.database.logger.WriteToLog(this, "ROLLBACK");
                        }
                        catch (CoreException)
                        {
                        }
                        this.database.TxManager.Rollback(this);
                        this.EndTransaction(false);
                    }
                }
            }
        }

        public void RollbackToSavepoint()
        {
            lock (this)
            {
                if (!this._isClosed)
                {
                    string key = this.sessionContext.Savepoints.GetKey(0);
                    this.database.TxManager.RollbackSavepoint(this, 0);
                    try
                    {
                        this.database.logger.WriteToLog(this, GetSavepointRollbackSql(key));
                    }
                    catch (CoreException)
                    {
                    }
                }
            }
        }

        public void RollbackToSavepoint(string name)
        {
            lock (this)
            {
                if (!this._isClosed)
                {
                    int index = this.sessionContext.Savepoints.GetIndex(name);
                    if (index < 0)
                    {
                        throw Error.GetError(0x12d5, name);
                    }
                    this.database.TxManager.RollbackSavepoint(this, index);
                    try
                    {
                        this.database.logger.WriteToLog(this, GetSavepointRollbackSql(name));
                    }
                    catch (CoreException)
                    {
                    }
                }
            }
        }

        public void Savepoint(string name)
        {
            lock (this)
            {
                int index = this.sessionContext.Savepoints.GetIndex(name);
                if (index != -1)
                {
                    this.sessionContext.Savepoints.Remove(name);
                    this.sessionContext.SavepointTimestamps.Remove(index);
                }
                this.sessionContext.Savepoints.Add(name, this.RowActionList.Count);
                this.sessionContext.SavepointTimestamps.AddLast(this.ActionTimestamp);
                try
                {
                    this.database.logger.WriteToLog(this, GetSavepointSql(name));
                }
                catch (CoreException)
                {
                }
            }
        }

        public void SetAttribute(int id, object obj)
        {
            lock (this)
            {
                switch (id)
                {
                    case 3:
                    {
                        int level = (int) obj;
                        this.SetIsolation(level);
                        break;
                    }
                    case 4:
                    {
                        bool autocommit = (bool) obj;
                        this.SetAutoCommit(autocommit);
                        break;
                    }
                    case 6:
                    {
                        bool rdy = (bool) obj;
                        this.SetReadOnlyDefault(rdy);
                        break;
                    }
                    case 7:
                    {
                        string catalog = (string) obj;
                        this.SetCatalog(catalog);
                        break;
                    }
                }
            }
        }

        public void SetAutoCommit(bool autocommit)
        {
            lock (this)
            {
                if (!this._isClosed && (autocommit != this.sessionContext.IsAutoCommit))
                {
                    this.Commit(false);
                    this.sessionContext.IsAutoCommit = autocommit;
                }
            }
        }

        public void SetCatalog(string catalog)
        {
            if (!this.database.GetCatalogName().Name.Equals(catalog))
            {
                throw Error.GetError(0x12e8);
            }
        }

        public void SetCurrentSchemaQName(QNameManager.QName name)
        {
            this.CurrentSchema = name;
        }

        public void SetIgnoreCase(bool mode)
        {
            this.IgnoreCase = mode;
        }

        public void SetIsolation(int level)
        {
            if (this.IsInMidTransaction())
            {
                throw Error.GetError(0xe75);
            }
            if (level == 0x100)
            {
                this.sessionContext.IsReadOnly = true;
            }
            if (this.IsolationLevel != level)
            {
                this.IsolationLevel = level;
                this.database.logger.WriteToLog(this, this.GetTransactionIsolationSql());
            }
        }

        public void SetIsolationDefault(int level)
        {
            lock (this)
            {
                if (level == 0x100)
                {
                    this._isReadOnlyDefault = true;
                }
                if (level != this._isolationLevelDefault)
                {
                    this._isolationLevelDefault = level;
                    if (!this.IsInMidTransaction())
                    {
                        this.IsolationLevel = this._isolationLevelDefault;
                    }
                    this.database.logger.WriteToLog(this, this.GetSessionIsolationSql());
                }
            }
        }

        public void SetLastIdentity(object i)
        {
            this._lastIdentity = i;
        }

        public void SetNoSql()
        {
            this.sessionContext.NoSql = true;
        }

        public void SetProcessingLog(bool val)
        {
            this._isProcessingLog = val;
        }

        public void SetProcessingScript(bool val)
        {
            this._isProcessingScript = val;
        }

        public void SetReadOnly(bool rdy)
        {
            if (!rdy && this.database.DatabaseReadOnly)
            {
                throw Error.GetError(0x1c7);
            }
            if (this.IsInMidTransaction())
            {
                throw Error.GetError(0xe75);
            }
            this.sessionContext.IsReadOnly = rdy;
        }

        public void SetReadOnlyDefault(bool rdy)
        {
            lock (this)
            {
                if (!rdy && this.database.DatabaseReadOnly)
                {
                    throw Error.GetError(0x1c7);
                }
                this._isReadOnlyDefault = rdy;
                if (!this.IsInMidTransaction())
                {
                    this.sessionContext.IsReadOnly = this._isReadOnlyDefault;
                }
            }
        }

        public void SetResultMemoryRowCount(int count)
        {
            if (this.database.logger.GetTempDirectoryPath() != null)
            {
                if (count < 0)
                {
                    count = 0;
                }
                this.ResultMaxMemoryRows = count;
            }
        }

        public void SetRole(Grantee role)
        {
            this._role = role;
        }

        public void SetSchema(string schema)
        {
            this.CurrentSchema = this.database.schemaManager.GetSchemaQName(schema);
        }

        public void SetScripting(bool script)
        {
            this._script = script;
        }

        public void SetSqlMaxRows(int rows)
        {
            this._sessionMaxRows = rows;
        }

        public void SetUser(User user)
        {
            this._user = user;
        }

        public void SetZoneSeconds(int seconds)
        {
            if (seconds == this.SessionTimeZoneSeconds)
            {
                this._timezone = null;
                this.TimeZoneSeconds = this.SessionTimeZoneSeconds;
            }
            else
            {
                TimeZoneInfo info = TimeZoneInfo.CreateCustomTimeZone("CustomSessionTimeZone", new TimeSpan(seconds * 0x989680L), "CustomSessionTimeZone", "CustomSessionTimeZone");
                this._timezone = info;
                this.TimeZoneSeconds = seconds;
            }
        }

        public void StartPhasedTransaction()
        {
        }

        public void StartTransaction()
        {
            this.database.TxManager.BeginTransaction(this);
        }
    }
}

