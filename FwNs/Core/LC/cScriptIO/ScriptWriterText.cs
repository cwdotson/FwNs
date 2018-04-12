namespace FwNs.Core.LC.cScriptIO
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;

    public class ScriptWriterText : ScriptWriterBase
    {
        protected RowOutputTextLog RowOut;
        public static byte[] BytesLineSep = Encoding.UTF8.GetBytes(Environment.NewLine);
        private static readonly byte[] BytesCommit = Encoding.UTF8.GetBytes("COMMIT");
        private static readonly byte[] BytesInsertInto = Encoding.UTF8.GetBytes("INSERT INTO ");
        private static readonly byte[] BytesValues = Encoding.UTF8.GetBytes(" VALUES(");
        private static readonly byte[] BytesTerm = Encoding.UTF8.GetBytes(")");
        private static readonly byte[] BytesDeleteFrom = Encoding.UTF8.GetBytes("DELETE FROM ");
        private static readonly byte[] BytesWhere = Encoding.UTF8.GetBytes(" WHERE ");
        private static readonly byte[] BytesSequence = Encoding.UTF8.GetBytes("ALTER SEQUENCE ");
        private static readonly byte[] BytesSequenceMid = Encoding.UTF8.GetBytes(" RESTART WITH ");
        private static readonly byte[] BytesCIdInit = Encoding.UTF8.GetBytes("/*C");
        private static readonly byte[] BytesCIdTerm = Encoding.UTF8.GetBytes("*/");
        private static readonly byte[] BytesSchema = Encoding.UTF8.GetBytes("SET SCHEMA ");

        public ScriptWriterText(Database db, string file, bool includeCachedData, bool newFile, bool isDump) : base(db, file, includeCachedData, newFile, isDump)
        {
            this.InitBuffers();
        }

        protected void InitBuffers()
        {
            this.RowOut = new RowOutputTextLog();
        }

        public override void WriteCommitStatement(Session session)
        {
            base.BusyWriting = true;
            this.WriteSessionIdAndSchema(session);
            this.RowOut.Reset();
            this.RowOut.Write(BytesCommit);
            this.RowOut.Write(BytesLineSep);
            this.WriteRowOutToFile();
            base.ByteCount += this.RowOut.Size();
            base.NeedsSync = true;
            base.BusyWriting = false;
            if (base.forceSync || (base.WriteDelay == 0))
            {
                base.Sync();
            }
        }

        protected override void WriteDataTerm()
        {
        }

        public override void WriteDeleteStatement(Session session, Table table, object[] data)
        {
            base.SchemaToLog = table.GetName().schema;
            base.BusyWriting = true;
            this.WriteSessionIdAndSchema(session);
            this.RowOut.Reset();
            this.RowOut.SetMode(1);
            this.RowOut.Write(BytesDeleteFrom);
            this.RowOut.WriteString(table.GetName().StatementName);
            this.RowOut.Write(BytesWhere);
            this.RowOut.WriteData(table.GetColumnCount(), table.GetColumnTypes(), data, table.ColumnList, table.GetPrimaryKey());
            this.RowOut.Write(BytesLineSep);
            this.WriteRowOutToFile();
            base.ByteCount += this.RowOut.Size();
            base.BusyWriting = false;
            if (base.forceSync)
            {
                base.Sync();
            }
        }

        public override void WriteInsertStatement(Session session, Table table, object[] data)
        {
            base.SchemaToLog = table.GetName().schema;
            this.WriteRow(session, table, data);
        }

        public override void WriteLogStatement(Session session, string s)
        {
            base.SchemaToLog = session.CurrentSchema;
            base.BusyWriting = true;
            this.WriteSessionIdAndSchema(session);
            this.RowOut.Reset();
            this.RowOut.WriteString(s);
            this.RowOut.Write(BytesLineSep);
            this.WriteRowOutToFile();
            base.ByteCount += this.RowOut.Size();
            base.NeedsSync = true;
            base.BusyWriting = false;
            if (base.forceSync)
            {
                base.Sync();
            }
        }

        public override void WriteRow(Session session, Table table, object[] data)
        {
            base.SchemaToLog = table.GetName().schema;
            base.BusyWriting = true;
            this.WriteSessionIdAndSchema(session);
            this.RowOut.Reset();
            this.RowOut.SetMode(0);
            this.RowOut.Write(BytesInsertInto);
            this.RowOut.WriteString(table.GetName().StatementName);
            this.RowOut.Write(BytesValues);
            this.RowOut.WriteData(data, table.GetColumnTypes());
            this.RowOut.Write(BytesTerm);
            this.RowOut.Write(BytesLineSep);
            this.WriteRowOutToFile();
            base.ByteCount += this.RowOut.Size();
            base.BusyWriting = false;
            if (base.forceSync)
            {
                base.Sync();
            }
        }

        public virtual void WriteRowOutToFile()
        {
            lock (base.FileStreamOutLock)
            {
                base.FileStreamOut.Write(this.RowOut.GetBuffer(), 0, this.RowOut.Size());
            }
        }

        private void WriteSchemaStatement(QNameManager.QName schema)
        {
            this.RowOut.Write(BytesSchema);
            this.RowOut.WriteString(schema.StatementName);
            this.RowOut.Write(BytesLineSep);
        }

        public override void WriteSequenceStatement(Session session, NumberSequence seq)
        {
            base.SchemaToLog = seq.GetName().schema;
            base.BusyWriting = true;
            this.WriteSessionIdAndSchema(session);
            this.RowOut.Reset();
            this.RowOut.Write(BytesSequence);
            this.RowOut.WriteString(seq.GetSchemaName().StatementName);
            this.RowOut.Write(0x2e);
            this.RowOut.WriteString(seq.GetName().StatementName);
            this.RowOut.Write(BytesSequenceMid);
            this.RowOut.WriteLong(seq.Peek());
            this.RowOut.Write(BytesLineSep);
            this.WriteRowOutToFile();
            base.ByteCount += this.RowOut.Size();
            base.NeedsSync = true;
            base.BusyWriting = false;
            if (base.forceSync)
            {
                base.Sync();
            }
        }

        protected override void WriteSessionIdAndSchema(Session session)
        {
            if (session != null)
            {
                if (session != base.CurrentSession)
                {
                    this.RowOut.Reset();
                    this.RowOut.Write(BytesCIdInit);
                    this.RowOut.WriteLong(session.GetId());
                    this.RowOut.Write(BytesCIdTerm);
                    base.CurrentSession = session;
                    this.WriteRowOutToFile();
                    base.ByteCount += this.RowOut.Size();
                }
                if (base.SchemaToLog != session.LoggedSchema)
                {
                    this.RowOut.Reset();
                    this.WriteSchemaStatement(base.SchemaToLog);
                    session.LoggedSchema = base.SchemaToLog;
                    this.WriteRowOutToFile();
                    base.ByteCount += this.RowOut.Size();
                }
            }
        }

        protected override void WriteTableInit(Table t)
        {
            if (!t.IsEmpty(base.CurrentSession) && (base.SchemaToLog != base.CurrentSession.LoggedSchema))
            {
                this.RowOut.Reset();
                this.WriteSchemaStatement(t.GetName().schema);
                this.WriteRowOutToFile();
                base.CurrentSession.LoggedSchema = base.SchemaToLog;
            }
        }
    }
}

