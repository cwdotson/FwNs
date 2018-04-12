namespace FwNs.Core.LC.cScriptIO
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.IO;
    using System.Threading;

    public abstract class ScriptWriterBase : IDisposable
    {
        private readonly Database _database;
        protected string OutFile;
        protected object FileStreamOutLock = new object();
        protected Stream FileStreamOut;
        private IFileSync _outDescriptor;
        public QNameManager.QName SchemaToLog;
        private bool _isClosed;
        private readonly bool _isDump;
        private readonly bool _includeCachedData;
        protected long ByteCount;
        protected bool NeedsSync;
        protected bool forceSync;
        public bool BusyWriting;
        private int _syncCount;
        public Session CurrentSession;
        public static string[] ListScriptFormats;
        private Timer _delayWriteTimer;
        protected int WriteDelay = 0xea60;

        static ScriptWriterBase()
        {
            string[] textArray1 = new string[4];
            textArray1[0] = "TEXT";
            textArray1[1] = "BINARY";
            textArray1[3] = "COMPRESSED";
            ListScriptFormats = textArray1;
        }

        protected ScriptWriterBase(Database db, string file, bool includeCachedData, bool isNewFile, bool isDump)
        {
            this._isDump = isDump;
            if ((isDump ? FileUtil.GetDefaultInstance().IsStreamElement(file) : db.logger.GetFileAccess().IsStreamElement(file)) & isNewFile)
            {
                throw Error.GetError(0x1c4, file);
            }
            this._database = db;
            this._includeCachedData = includeCachedData;
            this.OutFile = file;
            this.CurrentSession = this._database.sessionManager.GetSysSession();
            this.SchemaToLog = this.CurrentSession.LoggedSchema = this.CurrentSession.CurrentSchema;
            this.OpenFile();
        }

        public void Close()
        {
            this.Stop();
            if (!this._isClosed)
            {
                try
                {
                    lock (this.FileStreamOutLock)
                    {
                        this.NeedsSync = false;
                        this._isClosed = true;
                    }
                    this.FileStreamOut.Flush();
                    this._outDescriptor.Sync();
                }
                catch (IOException)
                {
                    throw Error.GetError(0x1c4);
                }
                finally
                {
                    this.FileStreamOut.Dispose();
                }
                this.ByteCount = 0L;
            }
        }

        public void DelayWrite(object obj)
        {
            try
            {
                if (this.WriteDelay != 0)
                {
                    this.Sync();
                }
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }
        }

        protected virtual void FinishStream()
        {
        }

        public void ForceSync()
        {
            lock (this)
            {
                if (!this._isClosed)
                {
                    lock (this.FileStreamOutLock)
                    {
                        try
                        {
                            this.FileStreamOut.Flush();
                            this._outDescriptor.Sync();
                            this._syncCount++;
                        }
                        catch (IOException exception)
                        {
                            Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.ScriptWriterBase_ForceSync_flush___or_sync___error__ + exception);
                        }
                        this.NeedsSync = false;
                        this.forceSync = false;
                    }
                }
            }
        }

        public int GetWriteDelay()
        {
            return this.WriteDelay;
        }

        protected void OpenFile()
        {
            try
            {
                UtlFileAccess access = this._isDump ? FileUtil.GetDefaultInstance() : this._database.logger.GetFileAccess();
                Stream s = access.OpenOutputStreamElement(this.OutFile);
                this._outDescriptor = access.GetFileSync(s);
                this.FileStreamOut = new BufferedStream(s, 0x2000);
            }
            catch (IOException exception)
            {
                object[] add = new object[] { exception.Message, this.OutFile };
                throw Error.GetError(exception, 0x1c4, 0x1a, add);
            }
        }

        public void Reopen()
        {
            this.OpenFile();
        }

        public void SetWriteDelay(int delay)
        {
            this.WriteDelay = delay;
            int dueTime = (this.WriteDelay == 0) ? 0x3e8 : this.WriteDelay;
            if (this._delayWriteTimer != null)
            {
                this._delayWriteTimer.Change(dueTime, dueTime);
            }
        }

        public long Size()
        {
            return this.ByteCount;
        }

        public void Start()
        {
            int writeDelay = this.WriteDelay;
            if (writeDelay != 0)
            {
                TimerCallback callback = new TimerCallback(this.DelayWrite);
                this._delayWriteTimer = new Timer(callback, null, writeDelay, writeDelay);
            }
        }

        public void Stop()
        {
            if (this._delayWriteTimer != null)
            {
                this._delayWriteTimer.Dispose();
                this._delayWriteTimer = null;
            }
        }

        public void Sync()
        {
            if (!this._isClosed)
            {
                lock (this.FileStreamOutLock)
                {
                    if (this.NeedsSync)
                    {
                        if (this.BusyWriting)
                        {
                            this.forceSync = true;
                        }
                        else
                        {
                            try
                            {
                                this.FileStreamOut.Flush();
                                this._outDescriptor.Sync();
                                this._syncCount++;
                            }
                            catch (IOException exception)
                            {
                                Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.ScriptWriterBase_ForceSync_flush___or_sync___error__ + exception);
                            }
                            this.NeedsSync = false;
                            this.forceSync = false;
                        }
                    }
                }
            }
        }

        public void WriteAll()
        {
            try
            {
                this.WriteDDL();
                this.WriteExistingData();
                this.FinishStream();
            }
            catch (IOException)
            {
                throw Error.GetError(0x1c4);
            }
        }

        public abstract void WriteCommitStatement(Session session);
        protected abstract void WriteDataTerm();
        protected void WriteDDL()
        {
            Result script = this._database.GetScript(!this._includeCachedData);
            this.WriteSingleColumnResult(script);
        }

        public abstract void WriteDeleteStatement(Session session, Table table, object[] data);
        protected void WriteExistingData()
        {
            this.CurrentSession.LoggedSchema = null;
            Iterator<string> iterator = this._database.schemaManager.AllSchemaNameIterator();
            while (iterator.HasNext())
            {
                string schemaName = iterator.Next();
                Iterator<object> iterator2 = this._database.schemaManager.DatabaseObjectIterator(schemaName, 3);
                while (iterator2.HasNext())
                {
                    Table t = (Table) iterator2.Next();
                    bool flag = false;
                    switch (t.GetTableType())
                    {
                        case 4:
                            flag = true;
                            break;

                        case 5:
                            flag = this._includeCachedData;
                            break;
                    }
                    try
                    {
                        if (flag)
                        {
                            this.SchemaToLog = t.GetName().schema;
                            this.WriteTableInit(t);
                            IRowIterator rowIterator = t.GetRowIterator(this.CurrentSession);
                            while (rowIterator.HasNext())
                            {
                                this.WriteRow(this.CurrentSession, t, rowIterator.GetNextRow().RowData);
                            }
                            WriteTableTerm(t);
                        }
                        continue;
                    }
                    catch (Exception exception)
                    {
                        throw Error.GetError(0x1c4, exception.ToString());
                    }
                }
            }
            this.WriteDataTerm();
        }

        public abstract void WriteInsertStatement(Session session, Table table, object[] data);
        public abstract void WriteLogStatement(Session session, string s);
        public abstract void WriteRow(Session session, Table table, object[] data);
        public abstract void WriteSequenceStatement(Session session, NumberSequence seq);
        protected abstract void WriteSessionIdAndSchema(Session session);
        protected void WriteSingleColumnResult(Result r)
        {
            RowSetNavigator navigator = r.InitialiseNavigator();
            while (navigator.HasNext())
            {
                object[] next = navigator.GetNext();
                this.WriteLogStatement(this.CurrentSession, (string) next[0]);
            }
        }

        protected virtual void WriteTableInit(Table t)
        {
        }

        protected static void WriteTableTerm(Table t)
        {
        }
    }
}

