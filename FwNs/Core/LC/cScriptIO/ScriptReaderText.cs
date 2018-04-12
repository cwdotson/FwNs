namespace FwNs.Core.LC.cScriptIO
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cStatements;
    using System;
    using System.IO;

    public class ScriptReaderText : ScriptReaderBase, IDisposable
    {
        protected StreamReader DataStreamIn;
        protected RowInputTextLog RowIn;
        private bool _isInsert;

        public ScriptReaderText(Database db) : base(db)
        {
        }

        public ScriptReaderText(Database db, string fileName) : base(db)
        {
            Stream stream = base.database.logger.GetFileAccess().OpenInputStreamElement(fileName);
            this.DataStreamIn = new StreamReader(stream);
            this.RowIn = new RowInputTextLog();
        }

        public override void Close()
        {
            try
            {
                this.DataStreamIn.Close();
            }
            catch (Exception)
            {
            }
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
                this.DataStreamIn.Dispose();
                this.RowIn.Dispose();
            }
        }

        public void ProcessStatement(Session session)
        {
            try
            {
                if (base.statement.StartsWith("/*C"))
                {
                    int index = base.statement.IndexOf('*', 4);
                    base.SessionNumber = int.Parse(base.statement.Substring(3, index - 3));
                    base.statement = base.statement.Substring(index + 2);
                    base.SessionChanged = true;
                    base.StatementType = 5;
                }
                else
                {
                    base.SessionChanged = false;
                    this.RowIn.SetSource(base.statement);
                    base.StatementType = this.RowIn.GetStatementType();
                    if (base.StatementType == 1)
                    {
                        base.RowData = null;
                        base.CurrentTable = null;
                    }
                    else if (base.StatementType == 4)
                    {
                        base.RowData = null;
                        base.CurrentTable = null;
                    }
                    else if (base.StatementType == 6)
                    {
                        base.RowData = null;
                        base.CurrentTable = null;
                        base.CurrentSchema = this.RowIn.GetSchemaName();
                    }
                    else
                    {
                        SqlType[] columnTypes;
                        string tableName = this.RowIn.GetTableName();
                        string name = session.GetCurrentSchemaQName().Name;
                        base.CurrentTable = base.database.schemaManager.GetUserTable(session, tableName, name);
                        base.CurrentStore = base.database.persistentStoreCollection.GetStore(base.CurrentTable);
                        if (base.StatementType == 3)
                        {
                            columnTypes = base.CurrentTable.GetColumnTypes();
                        }
                        else if (base.CurrentTable.HasPrimaryKey())
                        {
                            columnTypes = base.CurrentTable.GetPrimaryKeyTypes();
                        }
                        else
                        {
                            columnTypes = base.CurrentTable.GetColumnTypes();
                        }
                        base.RowData = this.RowIn.ReadData(columnTypes);
                    }
                }
            }
            catch (Exception exception1)
            {
                throw new IOException(exception1.ToString());
            }
        }

        protected override void ReadDDL(Session session)
        {
            while (this.ReadLoggedStatement(session))
            {
                Result result;
                Statement cs = null;
                if (this.RowIn.GetStatementType() == 3)
                {
                    this._isInsert = true;
                    return;
                }
                try
                {
                    cs = session.CompileStatement(base.statement, ResultProperties.DefaultPropsValue);
                    result = session.ExecuteCompiledStatement(cs, new object[0]);
                }
                catch (CoreException exception1)
                {
                    result = Result.NewErrorResult(exception1);
                }
                if (((!result.IsError() || (cs == null)) || ((cs.GetStatementType() != 0x30) && ((cs.GetStatementType() != 14) || !result.GetMainString().Contains("LibCore.LC.cLibrary")))) && result.IsError())
                {
                    base.database.logger.LogWarningEvent(result.GetMainString(), result.GetException());
                    if ((cs == null) || (cs.GetStatementType() != 14))
                    {
                        object[] add = new object[] { base.LineCount, result.GetMainString() };
                        throw Error.GetError(result.GetException(), 0x1cd, 0x19, add);
                    }
                }
            }
        }

        protected override void ReadExistingData(Session session)
        {
            try
            {
                string tableName = null;
                base.database.SetReferentialIntegrity(false);
                while (this._isInsert || this.ReadLoggedStatement(session))
                {
                    if (base.StatementType == 6)
                    {
                        session.SetSchema(base.CurrentSchema);
                    }
                    else
                    {
                        if (base.StatementType != 3)
                        {
                            throw Error.GetError(0x1cd);
                        }
                        if (!this.RowIn.GetTableName().Equals(tableName))
                        {
                            tableName = this.RowIn.GetTableName();
                            string schemaName = session.GetSchemaName(base.CurrentSchema);
                            base.CurrentTable = base.database.schemaManager.GetUserTable(session, tableName, schemaName);
                            base.CurrentStore = base.database.persistentStoreCollection.GetStore(base.CurrentTable);
                        }
                        base.CurrentTable.InsertFromScript(session, base.CurrentStore, base.RowData);
                    }
                    this._isInsert = false;
                }
                base.database.SetReferentialIntegrity(true);
            }
            catch (Exception exception)
            {
                base.database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.ScriptReaderText_ReadExistingData_readExistingData_failed, exception);
                object[] add = new object[] { exception.Message, base.LineCount };
                throw Error.GetError(exception, 0x1cd, 0x19, add);
            }
        }

        public override bool ReadLoggedStatement(Session session)
        {
            if (!base.SessionChanged)
            {
                string s = this.DataStreamIn.ReadLine();
                base.LineCount++;
                base.statement = StringConverter.UnicodeStringToString(s);
                if (base.statement == null)
                {
                    return false;
                }
            }
            this.ProcessStatement(session);
            return true;
        }
    }
}

