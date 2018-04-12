namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cScriptIO;
    using FwNs.Core.LC.cTables;
    using System;
    using System.IO;

    public sealed class Log : IDisposable
    {
        private readonly Database _database;
        private readonly UtlFileAccess _fa;
        private readonly string _fileName;
        private readonly LibCoreDatabaseProperties _properties;
        private DataFileCache _cache;
        private bool _filesReadOnly;
        private string _logFileName;
        private long _maxLogSize;
        private string _scriptFileName;
        private int _scriptFormat;
        private int _writeDelay;
        public ScriptWriterBase DbLogWriter;

        public Log(Database db)
        {
            this._database = db;
            this._fa = db.logger.GetFileAccess();
            this._fileName = db.GetPath();
            this._properties = db.GetProperties();
        }

        public void BackupData()
        {
            if (this._database.logger.PropIncrementBackup)
            {
                this._fa.RemoveElement(this._fileName + ".backup");
            }
            else if (this._fa.IsStreamElement(this._fileName + ".data"))
            {
                FileArchiver.Archive(this._fileName + ".data", this._fileName + ".backup.new", this._database.logger.GetFileAccess(), 1);
            }
        }

        public void Checkpoint(bool defr)
        {
            if (!this._filesReadOnly)
            {
                if (this._cache == null)
                {
                    defr = false;
                }
                else if (this.ForceDefrag())
                {
                    defr = true;
                }
                if (defr)
                {
                    try
                    {
                        this.Defrag();
                    }
                    catch (Exception)
                    {
                        this._database.logger.CheckpointDisabled = true;
                    }
                }
                else if (this.CheckpointClose())
                {
                    this.CheckpointReopen();
                }
            }
        }

        public bool CheckpointClose()
        {
            if (!this._filesReadOnly)
            {
                this.DeleteNewAndOldFiles();
                try
                {
                    this.WriteScript(false);
                }
                catch (CoreException)
                {
                    this.DeleteNewScript();
                    return false;
                }
                try
                {
                    if (this._cache != null)
                    {
                        this._cache.Close(true);
                        this._cache.BackupFile();
                    }
                }
                catch (Exception)
                {
                    this.DeleteNewScript();
                    this.DeleteNewBackup();
                    try
                    {
                        if (!this._cache.IsFileOpen())
                        {
                            this._cache.Open(false);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return false;
                }
                this._properties.SetDbModified(2);
                this.CloseLog();
                this.DeleteLog();
                this.RenameNewScript();
                this.RenameNewBackup();
                try
                {
                    this._properties.SetDbModified(0);
                }
                catch (Exception)
                {
                }
            }
            return true;
        }

        public bool CheckpointReopen()
        {
            if (!this._filesReadOnly)
            {
                try
                {
                    if (this._cache != null)
                    {
                        this._cache.Open(false);
                    }
                    if (this.DbLogWriter != null)
                    {
                        this.OpenLog();
                    }
                    this._properties.SetDbModified(1);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public void Close(bool script)
        {
            this.CloseLog();
            this.DeleteNewAndOldFiles();
            this.WriteScript(script);
            if (this._cache != null)
            {
                this._cache.Close(true);
            }
            this._properties.SetDbModified(2);
            this.DeleteLog();
            if (this._cache != null)
            {
                if (script)
                {
                    this.DeleteBackup();
                    this.DeleteData();
                }
                else
                {
                    this.BackupData();
                    this.RenameNewBackup();
                }
            }
            this.RenameNewScript();
            this._properties.SetDbModified(0);
        }

        public void CloseLog()
        {
            lock (this)
            {
                if (this.DbLogWriter != null)
                {
                    this.DbLogWriter.Close();
                }
            }
        }

        public void Defrag()
        {
            if (this._cache.FileFreePosition != this._cache.InitialFreePos)
            {
                this._database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.Log_Defrag_defrag_start);
                try
                {
                    this._cache.Defrag();
                }
                catch (CoreException exception)
                {
                    this._database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.Log_Defrag_defrag_failure, exception);
                    throw;
                }
                catch (Exception exception2)
                {
                    this._database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.Log_Defrag_defrag_failure, exception2);
                    throw Error.GetError(0x1d2, exception2);
                }
                this._database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.Log_Defrag_defrag_end);
            }
        }

        public void DeleteBackup()
        {
            this._fa.RemoveElement(this._fileName + ".backup");
        }

        public void DeleteData()
        {
            this._fa.RemoveElement(this._fileName + ".data");
        }

        public void DeleteLog()
        {
            this._fa.RemoveElement(this._logFileName);
        }

        public void DeleteNewAndOldFiles()
        {
            this._fa.RemoveElement(this._fileName + ".data.old");
            this._fa.RemoveElement(this._fileName + ".data.new");
            this._fa.RemoveElement(this._fileName + ".backup.new");
            this._fa.RemoveElement(this._scriptFileName + ".new");
        }

        public void DeleteNewBackup()
        {
            this._fa.RemoveElement(this._fileName + ".backup.new");
        }

        public void DeleteNewScript()
        {
            this._fa.RemoveElement(this._scriptFileName + ".new");
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
                if (this.DbLogWriter != null)
                {
                    this.DbLogWriter.Dispose();
                }
                if (this._cache != null)
                {
                    this._cache.Dispose();
                }
            }
        }

        public bool ForceDefrag()
        {
            long num = (this._database.logger.PropCacheDefragLimit * this._cache.GetFileFreePos()) / 100L;
            long lostBlocksSize = this._cache.FreeBlocks.GetLostBlocksSize();
            return ((num > 0L) && (lostBlocksSize > num));
        }

        public DataFileCache GetCache()
        {
            if (this._cache == null)
            {
                this._cache = new DataFileCache();
                this._cache.InitParams(this._database, this._fileName);
                this._cache.Open(this._filesReadOnly);
            }
            return this._cache;
        }

        public int GetScriptType()
        {
            return this._scriptFormat;
        }

        public int GetWriteDelay()
        {
            return this._writeDelay;
        }

        public bool HasCache()
        {
            return (this._cache > null);
        }

        public void InitParams()
        {
            this._maxLogSize = (this._database.logger.PropLogSize * 0x400L) * 0x400L;
            this._scriptFormat = 0;
            this._writeDelay = this._database.logger.PropWriteDelay;
            this._filesReadOnly = this._database.IsFilesReadOnly();
            this._scriptFileName = this._fileName + ".script";
            this._logFileName = this._fileName + ".log";
        }

        public bool IsAnyCacheModified()
        {
            return ((this._cache != null) && this._cache.IsFileModified());
        }

        public void Open()
        {
            this.InitParams();
            switch (this._properties.GetDbModified())
            {
                case 0:
                    break;

                case 1:
                    this.DeleteNewAndOldFiles();
                    this.ProcessScript();
                    this.ProcessLog();
                    this.Close(false);
                    if (this._cache != null)
                    {
                        this._cache.Open(this._filesReadOnly);
                    }
                    goto Label_00BB;

                case 2:
                    this.RenameNewDataFile();
                    this.RenameNewBackup();
                    this.RenameNewScript();
                    this.DeleteLog();
                    this._properties.SetDbModified(0);
                    break;

                default:
                    goto Label_00BB;
            }
            this.ProcessScript();
            if (this.IsAnyCacheModified())
            {
                this._properties.SetDbModified(1);
                this.Close(false);
                if (this._cache != null)
                {
                    this._cache.Open(this._filesReadOnly);
                }
            }
        Label_00BB:
            this.OpenLog();
            if (!this._filesReadOnly)
            {
                this._properties.SetDbModified(1);
            }
        }

        public void OpenLog()
        {
            if (!this._filesReadOnly)
            {
                Crypto crypto = this._database.logger.GetCrypto();
                try
                {
                    if (crypto == null)
                    {
                        this.DbLogWriter = new ScriptWriterText(this._database, this._logFileName, false, false, false);
                    }
                    else
                    {
                        this.DbLogWriter = new ScriptWriterEncode(this._database, this._logFileName, crypto);
                    }
                    this.DbLogWriter.SetWriteDelay(this._writeDelay);
                    this.DbLogWriter.Start();
                }
                catch (Exception)
                {
                    throw Error.GetError(0x1c4, this._logFileName);
                }
            }
        }

        private void ProcessLog()
        {
            if (this._fa.IsStreamElement(this._logFileName))
            {
                ScriptRunner.RunScript(this._database, this._logFileName);
            }
        }

        private void ProcessScript()
        {
            try
            {
                if (this._fa.IsStreamElement(this._scriptFileName))
                {
                    Crypto crypto = this._database.logger.GetCrypto();
                    if (crypto == null)
                    {
                        using (ScriptReaderText text = new ScriptReaderText(this._database, this._scriptFileName))
                        {
                            this.ProcessScriptHelper(text);
                            return;
                        }
                    }
                    using (ScriptReaderText text2 = new ScriptReaderDecode(this._database, this._scriptFileName, crypto))
                    {
                        this.ProcessScriptHelper(text2);
                    }
                }
            }
            catch (Exception exception)
            {
                if (this._cache != null)
                {
                    this._cache.Close(false);
                }
                this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.Log_ProcessScript_Script_processing_failure, exception);
                if (exception is CoreException)
                {
                    throw;
                }
                if (exception is IOException)
                {
                    throw Error.GetError(0x1c4, exception);
                }
                if (exception is OutOfMemoryException)
                {
                    throw Error.GetError(460);
                }
                throw Error.GetError(0x1ca, exception);
            }
        }

        private void ProcessScriptHelper(ScriptReaderText scr)
        {
            using (Session session = SessionManager.GetSysSessionForScript(this._database))
            {
                scr.ReadAll(session);
            }
        }

        public void RenameNewBackup()
        {
            this._fa.RemoveElement(this._fileName + ".backup");
            if (this._fa.IsStreamElement(this._fileName + ".backup.new"))
            {
                this._fa.RenameElement(this._fileName + ".backup.new", this._fileName + ".backup");
            }
        }

        public void RenameNewDataFile()
        {
            if (this._fa.IsStreamElement(this._fileName + ".data.new"))
            {
                this._fa.RenameElement(this._fileName + ".data.new", this._fileName + ".data");
            }
        }

        public void RenameNewScript()
        {
            if (this._fa.IsStreamElement(this._scriptFileName + ".new"))
            {
                this._fa.RenameElement(this._scriptFileName + ".new", this._scriptFileName);
            }
        }

        public void SetIncrementBackup(bool val)
        {
            if (this._cache != null)
            {
                this._cache.SetIncrementBackup(val);
                this._cache.FileModified = true;
            }
        }

        public void SetLogSize(int megas)
        {
            this._maxLogSize = (megas * 0x400L) * 0x400L;
        }

        public void SetWriteDelay(int delay)
        {
            this._writeDelay = delay;
            if (this.DbLogWriter != null)
            {
                this.DbLogWriter.ForceSync();
                this.DbLogWriter.Stop();
                this.DbLogWriter.SetWriteDelay(delay);
                this.DbLogWriter.Start();
            }
        }

        public void Shutdown()
        {
            this.SynchLog();
            if (this._cache != null)
            {
                this._cache.Close(false);
            }
            this.CloseLog();
        }

        public void SynchLog()
        {
            if (this.DbLogWriter != null)
            {
                this.DbLogWriter.Sync();
            }
        }

        public void WriteCommitStatement(Session session)
        {
            try
            {
                this.DbLogWriter.WriteCommitStatement(session);
            }
            catch (IOException)
            {
                throw Error.GetError(0x1c4, this._logFileName);
            }
            if ((this._maxLogSize > 0L) && (this.DbLogWriter.Size() > this._maxLogSize))
            {
                this._database.logger.CheckpointRequired = true;
            }
        }

        public void WriteDeleteStatement(Session session, Table t, object[] row)
        {
            try
            {
                this.DbLogWriter.WriteDeleteStatement(session, t, row);
            }
            catch (IOException)
            {
                throw Error.GetError(0x1c4, this._logFileName);
            }
            if ((this._maxLogSize > 0L) && (this.DbLogWriter.Size() > this._maxLogSize))
            {
                this._database.logger.CheckpointRequired = true;
            }
        }

        public void WriteInsertStatement(Session session, Table t, object[] row)
        {
            try
            {
                this.DbLogWriter.WriteInsertStatement(session, t, row);
            }
            catch (IOException)
            {
                throw Error.GetError(0x1c4, this._logFileName);
            }
            if ((this._maxLogSize > 0L) && (this.DbLogWriter.Size() > this._maxLogSize))
            {
                this._database.logger.CheckpointRequired = true;
            }
        }

        public void WriteScript(bool full)
        {
            this.DeleteNewScript();
            Crypto crypto = this._database.logger.GetCrypto();
            if (crypto == null)
            {
                using (ScriptWriterText text = new ScriptWriterText(this._database, this._scriptFileName + ".new", full, true, false))
                {
                    text.WriteAll();
                    return;
                }
            }
            using (ScriptWriterEncode encode = new ScriptWriterEncode(this._database, this._scriptFileName + ".new", full, crypto))
            {
                encode.WriteAll();
            }
        }

        public void WriteSequenceStatement(Session session, NumberSequence s)
        {
            try
            {
                this.DbLogWriter.WriteSequenceStatement(session, s);
            }
            catch (IOException)
            {
                throw Error.GetError(0x1c4, this._logFileName);
            }
            if ((this._maxLogSize > 0L) && (this.DbLogWriter.Size() > this._maxLogSize))
            {
                this._database.logger.CheckpointRequired = true;
            }
        }

        public void WriteStatement(Session session, string s)
        {
            try
            {
                this.DbLogWriter.WriteLogStatement(session, s);
            }
            catch (IOException)
            {
                throw Error.GetError(0x1c4, this._logFileName);
            }
            if ((this._maxLogSize > 0L) && (this.DbLogWriter.Size() > this._maxLogSize))
            {
                this._database.logger.CheckpointRequired = true;
            }
        }
    }
}

