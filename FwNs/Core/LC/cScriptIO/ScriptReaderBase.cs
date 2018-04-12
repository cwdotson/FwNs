namespace FwNs.Core.LC.cScriptIO
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;

    public abstract class ScriptReaderBase
    {
        public const int AnyStatement = 1;
        public const int DeleteStatement = 2;
        public const int InsertStatement = 3;
        public const int CommitStatement = 4;
        public const int SessionId = 5;
        public const int SetSchemaStatement = 6;
        protected Database database;
        protected int LineCount;
        public int StatementType;
        public int SessionNumber;
        public bool SessionChanged;
        public object[] RowData;
        public long SequenceValue;
        public string statement;
        public Table CurrentTable;
        public IPersistentStore CurrentStore;
        public NumberSequence CurrentSequence;
        public string CurrentSchema;

        protected ScriptReaderBase(Database db)
        {
            this.database = db;
        }

        public abstract void Close();
        public string GetCurrentSchema()
        {
            return this.CurrentSchema;
        }

        public NumberSequence GetCurrentSequence()
        {
            return this.CurrentSequence;
        }

        public Table GetCurrentTable()
        {
            return this.CurrentTable;
        }

        public object[] GetData()
        {
            return this.RowData;
        }

        public int GetLineNumber()
        {
            return this.LineCount;
        }

        public string GetLoggedStatement()
        {
            return this.statement;
        }

        public long GetSequenceValue()
        {
            return this.SequenceValue;
        }

        public int GetSessionNumber()
        {
            return this.SessionNumber;
        }

        public int GetStatementType()
        {
            return this.StatementType;
        }

        public void ReadAll(Session session)
        {
            this.ReadDDL(session);
            this.ReadExistingData(session);
        }

        protected abstract void ReadDDL(Session session);
        protected abstract void ReadExistingData(Session session);
        public abstract bool ReadLoggedStatement(Session session);
    }
}

