namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConcurrency;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public sealed class Logger : IDisposable
    {
        private const string BackupFileFormat = "yyyyMMdd'T'HHmmss";
        private const char RuntimeFileDelim = '\n';
        public SimpleLog AppLog;
        public Log log;
        private readonly Database _database;
        private LockFile _lockFile;
        public bool CheckpointRequired;
        public bool CheckpointDue;
        public bool CheckpointDisabled;
        private bool _logsStatements;
        private bool _loggingEnabled;
        private bool _syncFile = true;
        public bool PropFilesReadOnly;
        public bool PropDatabaseReadOnly;
        public bool PropIncrementBackup;
        public bool PropNioDataFile;
        public int PropMaxFreeBlocks;
        public int PropCacheMaxRows;
        public int PropCacheMaxSize;
        public int PropCacheFileScale;
        public int PropCacheDefragLimit;
        public int PropWriteDelay;
        public int PropLogSize;
        public bool PropLogData = true;
        private int _propEventLogLevel;
        private int _propGc;
        private int _propTxMode = 2;
        private int _propLobBlockSize = 0x8000;
        private bool _propRefIntegrity = true;
        private Crypto _crypto;
        public UtlFileAccess fileAccess;
        public bool isStoredFileAccess;
        private string _tempDirectoryPath;
        public bool IsNewDatabase;
        private List<RowStoreAVLDisk> _createdStores = new List<RowStoreAVLDisk>();

        public Logger(Database database)
        {
            this._database = database;
        }

        public void AcquireLock(string path)
        {
            if (this._lockFile == null)
            {
                this._lockFile = LockFile.NewLockFileLock(path, this.fileAccess);
            }
        }

        public void Backup(string destPath, string dbPath, bool script, bool blocking, bool compressed)
        {
            lock (this)
            {
                string fullName = new FileInfo(dbPath).FullName;
                if (string.IsNullOrEmpty(destPath))
                {
                    throw Error.GetError(0xd56, "0-length destination path");
                }
                char ch = destPath[destPath.Length - 1];
                DirectoryInfo archiveDir = ((ch == '/') || (ch == '\n')) ? new DirectoryInfo(fullName + "-" + DateTime.Now.ToString("yyyyMMdd'T'HHmmss")) : new DirectoryInfo(destPath);
                this.log.CheckpointClose();
                try
                {
                    this.LogInfoEvent(FwNs.Core.LC.cResources.SR.Logger_Backup_Initiating_backup_of_instance__ + fullName + FwNs.Core.LC.cResources.SR.Logger_Backup__Single_Quote);
                    DbBackup backup1 = new DbBackup(archiveDir, dbPath);
                    backup1.SetAbortUponModify(false);
                    backup1.Write();
                    this.LogInfoEvent(FwNs.Core.LC.cResources.SR.Logger_Backup_Successfully_backed_up_instance__ + fullName + FwNs.Core.LC.cResources.SR.Logger_Backup___to__ + destPath + FwNs.Core.LC.cResources.SR.Logger_Backup__Single_Quote);
                }
                catch (ArgumentException exception)
                {
                    throw Error.GetError(0x19d1, exception.Message);
                }
                catch (IOException exception2)
                {
                    throw Error.GetError(0x1c4, exception2.Message);
                }
                catch (Exception exception3)
                {
                    throw Error.GetError(0x1c4, exception3.Message);
                }
                finally
                {
                    this.log.CheckpointReopen();
                }
            }
        }

        public void Checkpoint(bool mode)
        {
            lock (this)
            {
                this._database.lobManager.DeleteUnusedLobs();
                if (this._logsStatements)
                {
                    this.LogInfoEvent(FwNs.Core.LC.cResources.SR.Logger_Checkpoint_Checkpoint_start);
                    this.log.Checkpoint(mode);
                    this._database.sessionManager.ResetLoggedSchemas();
                    this.LogInfoEvent(FwNs.Core.LC.cResources.SR.Logger_Checkpoint_Checkpoint_end);
                }
                this.CheckpointDue = false;
            }
        }

        private static void CheckPower(int n, int limit)
        {
            for (int i = 0; i < limit; i++)
            {
                if ((n & 1) != 0)
                {
                    if ((n | 1) != 1)
                    {
                        throw Error.GetError(0x15b4);
                    }
                    return;
                }
                n = n >> 1;
            }
            throw Error.GetError(0x15b4);
        }

        public bool ClosePersistence(int closemode)
        {
            if (this.log == null)
            {
                return true;
            }
            this._database.lobManager.DeleteUnusedLobs();
            try
            {
                switch (closemode)
                {
                    case -1:
                        this.log.Shutdown();
                        goto Label_0076;

                    case 0:
                        this.log.Close(false);
                        goto Label_0076;

                    case 1:
                    case 2:
                        this.log.Close(true);
                        goto Label_0076;
                }
            }
            catch (Exception exception)
            {
                this.LogSevereEvent(FwNs.Core.LC.cResources.SR.Logger_ClosePersistence_error_closing_log, exception);
                return false;
            }
        Label_0076:
            this.LogInfoEvent(FwNs.Core.LC.cResources.SR.Logger_ClosePersistence_Database_closed);
            this.AppLog.Close();
            this._logsStatements = false;
            this._loggingEnabled = false;
            return true;
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
                if (this.log != null)
                {
                    this.log.Dispose();
                }
                this.AppLog.Dispose();
                if (this.HasLockFile())
                {
                    this._lockFile.Dispose();
                }
                using (List<RowStoreAVLDisk>.Enumerator enumerator = this._createdStores.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Dispose();
                    }
                }
            }
        }

        public DataFileCache GetCache()
        {
            if (this.log == null)
            {
                return null;
            }
            return this.log.GetCache();
        }

        public int GetCacheFileScale()
        {
            return this.PropCacheFileScale;
        }

        public int GetCacheRowsDefault()
        {
            return this.PropCacheMaxRows;
        }

        public int GetCacheSize()
        {
            return this.PropCacheMaxSize;
        }

        public Crypto GetCrypto()
        {
            return this._crypto;
        }

        public int GetDefragLimit()
        {
            return this.PropCacheDefragLimit;
        }

        public UtlFileAccess GetFileAccess()
        {
            return this.fileAccess;
        }

        public int GetLobBlockSize()
        {
            return this._propLobBlockSize;
        }

        public int GetLobFileScale()
        {
            return (this._propLobBlockSize / 0x400);
        }

        public int GetLogSize()
        {
            return this.PropLogSize;
        }

        public string[] GetPropertiesSql()
        {
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            builder.Append("SET DATABASE ").Append("UNIQUE").Append(' ');
            builder.Append("NAME").Append(' ').Append(this._database.GetUniqueName());
            list.Add(builder.ToString());
            builder.Length = 0;
            builder.Append("SET DATABASE ").Append("GC").Append(' ');
            builder.Append(this._propGc);
            list.Add(builder.ToString());
            builder.Length = 0;
            builder.Append("SET DATABASE ").Append("DEFAULT").Append(' ');
            builder.Append("RESULT").Append(' ').Append("MEMORY");
            builder.Append(' ').Append("ROWS").Append(' ');
            builder.Append(this._database.GetResultMaxMemoryRows());
            list.Add(builder.ToString());
            builder.Length = 0;
            builder.Append("SET DATABASE ").Append("EVENT").Append(' ');
            builder.Append("LOG").Append(' ').Append("LEVEL");
            builder.Append(' ').Append(this._propEventLogLevel);
            list.Add(builder.ToString());
            builder.Length = 0;
            builder.Append("SET DATABASE ").Append("SQL").Append(' ');
            builder.Append("REFERENCES").Append(' ');
            builder.Append(this._database.SqlEnforceRefs ? "TRUE" : "FALSE");
            list.Add(builder.ToString());
            builder.Length = 0;
            builder.Append("SET DATABASE ").Append("SQL").Append(' ');
            builder.Append("SIZE").Append(' ');
            builder.Append(this._database.SqlEnforceSize ? "TRUE" : "FALSE");
            list.Add(builder.ToString());
            builder.Length = 0;
            builder.Append("SET DATABASE ").Append("SQL").Append(' ');
            builder.Append("NAMES").Append(' ');
            builder.Append(this._database.SqlEnforceNames ? "TRUE" : "FALSE");
            list.Add(builder.ToString());
            builder.Length = 0;
            builder.Append("SET DATABASE ").Append("TRANSACTION");
            builder.Append(' ').Append("CONTROL").Append(' ');
            switch (this._database.TxManager.GetTransactionControl())
            {
                case 0:
                    builder.Append("LOCKS");
                    break;

                case 1:
                    builder.Append("MVLOCKS");
                    break;

                case 2:
                    builder.Append("MVCC");
                    break;
            }
            list.Add(builder.ToString());
            builder.Length = 0;
            builder.Append("SET DATABASE ").Append("DEFAULT").Append(' ');
            builder.Append("ISOLATION").Append(' ').Append("LEVEL");
            builder.Append(' ');
            switch (this._database.GetDefaultIsolationLevel())
            {
                case 0x1000:
                    builder.Append("READ").Append(' ').Append("COMMITTED");
                    break;

                case 0x100000:
                    builder.Append("SERIALIZABLE");
                    break;
            }
            list.Add(builder.ToString());
            builder.Length = 0;
            if (this.HasPersistence())
            {
                if (this._database.schemaManager.GetDefaultTableType() == 5)
                {
                    list.Add("SET DATABASE DEFAULT TABLE TYPE CACHED");
                }
                int propWriteDelay = this.PropWriteDelay;
                bool flag1 = (propWriteDelay > 0) && (propWriteDelay < 0x3e8);
                if (flag1)
                {
                    if (propWriteDelay < 20)
                    {
                        propWriteDelay = 20;
                    }
                }
                else
                {
                    propWriteDelay /= 0x3e8;
                }
                builder.Length = 0;
                builder.Append("SET FILES ").Append("WRITE").Append(' ');
                builder.Append("DELAY").Append(' ').Append(propWriteDelay);
                if (flag1)
                {
                    builder.Append(' ').Append("MILLIS");
                }
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("BACKUP");
                builder.Append(' ').Append("INCREMENT").Append(' ');
                builder.Append(this.PropIncrementBackup ? "TRUE" : "FALSE");
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("CACHE");
                builder.Append(' ').Append("SIZE").Append(' ');
                builder.Append((int) (this.PropCacheMaxSize / 0x400));
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("CACHE");
                builder.Append(' ').Append("ROWS").Append(' ');
                builder.Append(this.PropCacheMaxRows);
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("SCALE");
                builder.Append(' ').Append(this.PropCacheFileScale);
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("LOB").Append(' ').Append("SCALE");
                builder.Append(' ').Append(this.GetLobFileScale());
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("DEFRAG");
                builder.Append(' ').Append(this.PropCacheDefragLimit);
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("NIO");
                builder.Append(' ').Append(this.PropNioDataFile ? "TRUE" : "FALSE");
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("LOG").Append(' ');
                builder.Append(this.PropLogData ? "TRUE" : "FALSE");
                list.Add(builder.ToString());
                builder.Length = 0;
                builder.Append("SET FILES ").Append("LOG").Append(' ');
                builder.Append("SIZE").Append(' ').Append(this.PropLogSize);
                list.Add(builder.ToString());
                builder.Length = 0;
            }
            return list.ToArray();
        }

        public int GetScriptType()
        {
            if (this.log == null)
            {
                return 0;
            }
            return this.log.GetScriptType();
        }

        public string GetTempDirectoryPath()
        {
            return this._tempDirectoryPath;
        }

        public int GetWriteDelay()
        {
            return this.PropWriteDelay;
        }

        public bool HasLockFile()
        {
            return (this._lockFile > null);
        }

        public bool HasPersistence()
        {
            return (this.log > null);
        }

        public bool IsLogged()
        {
            return (DatabaseUrl.IsFileBasedDatabaseType(this._database.GetDatabaseType()) && !this._database.IsFilesReadOnly());
        }

        public bool IsStoredFileAccess()
        {
            return this.isStoredFileAccess;
        }

        public void LogInfoEvent(string message)
        {
            if (this.AppLog != null)
            {
                this.AppLog.LogContext(2, message);
            }
        }

        public void LogSevereEvent(string message, Exception t)
        {
            if (this.AppLog != null)
            {
                this.AppLog.LogContext(t, message);
            }
        }

        public void LogSevereEvent(string sql, string message, Exception t)
        {
            if (this.AppLog != null)
            {
                this.AppLog.LogContext(t, message, sql);
            }
        }

        public void LogWarningEvent(string message, Exception t)
        {
            if (this.AppLog != null)
            {
                this.AppLog.LogContext(t, message);
            }
        }

        public bool NeedsCheckpointReset()
        {
            lock (this)
            {
                if ((this.CheckpointRequired && !this.CheckpointDue) && !this.CheckpointDisabled)
                {
                    this.CheckpointDue = true;
                    this.CheckpointRequired = false;
                    return true;
                }
                this.CheckpointRequired = false;
                return false;
            }
        }

        public static Index NewIndex(QNameManager.QName name, long id, TableBase table, int[] columns, bool[] descending, bool[] nullsLast, SqlType[] colTypes, bool pk, bool unique, bool constraint, bool forward)
        {
            if ((table.database.DatabaseProperties != null) && (table.database.DatabaseProperties.GetIntegerProperty("LibCore.result_max_memory_rows") == 0))
            {
                switch (table.GetTableType())
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        return new IndexAVLMemory(name, id, table, columns, descending, nullsLast, colTypes, pk, unique, constraint, forward);

                    case 5:
                        return new IndexAVL(name, id, table, columns, descending, nullsLast, colTypes, pk, unique, constraint, forward);
                }
            }
            else
            {
                switch (table.GetTableType())
                {
                    case 1:
                    case 4:
                        return new IndexAVLMemory(name, id, table, columns, descending, nullsLast, colTypes, pk, unique, constraint, forward);

                    case 2:
                    case 3:
                    case 5:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        return new IndexAVL(name, id, table, columns, descending, nullsLast, colTypes, pk, unique, constraint, forward);
                }
            }
            throw Error.RuntimeError(0xc9, "Logger");
        }

        public IPersistentStore NewStore(Session session, IPersistentStoreCollection collection, TableBase table, bool diskBased)
        {
            switch (table.GetTableType())
            {
                case 1:
                case 4:
                case 11:
                    return new RowStoreAVLMemory(collection, (Table) table);

                case 2:
                case 8:
                case 10:
                    break;

                case 3:
                    diskBased = false;
                    break;

                case 5:
                {
                    DataFileCache cache = this.GetCache();
                    if (cache == null)
                    {
                        goto Label_0093;
                    }
                    RowStoreAVLDisk item = new RowStoreAVLDisk(collection, cache, (Table) table);
                    this._createdStores.Add(item);
                    return item;
                }
                case 9:
                    if (session != null)
                    {
                        return new RowStoreAVLHybrid(session, collection, table);
                    }
                    return null;

                default:
                    goto Label_0093;
            }
            if (session == null)
            {
                return null;
            }
            return new RowStoreAVLHybrid(session, collection, table, diskBased);
        Label_0093:
            throw Error.RuntimeError(0xc9, "Logger");
        }

        private static string NewUniqueName()
        {
            string str = StringUtil.ToPaddedString(string.Format("{0:x}", DateTime.Now.Ticks), 0x10, '0', false);
            return ("LibCore" + str.Substring(6).ToUpper(CultureInfo.InvariantCulture));
        }

        public void OpenPersistence()
        {
            if (this._database.IsFilesInAssembly())
            {
                this.fileAccess = FileUtil.GetFileAccessResource();
            }
            else
            {
                this.fileAccess = FileUtil.GetDefaultInstance();
            }
            this.isStoredFileAccess = true;
            bool flag = DatabaseUrl.IsFileBasedDatabaseType(this._database.GetDatabaseType());
            this._database.DatabaseProperties = new LibCoreDatabaseProperties(this._database);
            this.IsNewDatabase = !flag || !this.fileAccess.IsStreamElement(this._database.GetPath() + ".script");
            if (this.IsNewDatabase)
            {
                string name = NewUniqueName();
                this._database.SetUniqueName(name);
                if (this._database.UrlProperties.IsPropertyTrue("ifexists"))
                {
                    throw Error.GetError(0x1d1, this._database.GetPath());
                }
                this._database.DatabaseProperties.SetUrlProperties(this._database.UrlProperties);
            }
            else
            {
                this._database.DatabaseProperties.Load(this.fileAccess);
                if (this._database.UrlProperties.IsPropertyTrue("LibCore.files_readonly"))
                {
                    this._database.DatabaseProperties.SetProperty("LibCore.files_readonly", true);
                }
                if (this._database.UrlProperties.IsPropertyTrue("readonly"))
                {
                    this._database.DatabaseProperties.SetProperty("readonly", true);
                }
            }
            this.SetVariables();
            string path = null;
            if (DatabaseUrl.IsFileBasedDatabaseType(this._database.GetDatabaseType()) && !this._database.IsFilesReadOnly())
            {
                path = this._database.GetPath() + ".app.log";
            }
            this.AppLog = new SimpleLog(path, this._propEventLogLevel, this.fileAccess);
            this._database.SetReferentialIntegrity(this._propRefIntegrity);
            if (flag)
            {
                this.CheckpointRequired = false;
                this._logsStatements = false;
                if (this._database.GetProperties().IsPropertyTrue("LibCore.lock_file") && !this._database.IsFilesReadOnly())
                {
                    this.AcquireLock(this._database.GetPath());
                }
                this.log = new Log(this._database);
                this.log.Open();
                this._logsStatements = true;
                this._loggingEnabled = this.PropLogData && !this._database.IsFilesReadOnly();
                if (this._database.GetUniqueName() == null)
                {
                    this._database.SetUniqueName(NewUniqueName());
                }
            }
        }

        public void ReleaseLock()
        {
            try
            {
                if (this._lockFile != null)
                {
                    this._lockFile.TryRelease();
                }
            }
            catch (Exception)
            {
            }
            this._lockFile = null;
        }

        public void RestartLogging()
        {
            this._loggingEnabled = this._logsStatements;
        }

        public void SetCacheFileScale(int value)
        {
            if (this.PropCacheFileScale != value)
            {
                CheckPower(value, 8);
                if ((1 < value) && (value < 8))
                {
                    throw Error.GetError(0x15b4);
                }
                if (this.GetCache() != null)
                {
                    throw Error.GetError(0x1d5);
                }
                this.PropCacheFileScale = value;
            }
        }

        public void SetCacheFileScaleNoCheck(int value)
        {
            CheckPower(value, 8);
            if ((1 < value) && (value < 8))
            {
                throw Error.GetError(0x15b4);
            }
            this.PropCacheFileScale = value;
        }

        public void SetCacheMaxRows(int value)
        {
            this.PropCacheMaxRows = value;
        }

        public void SetCacheSize(int value)
        {
            this.PropCacheMaxSize = value * 0x400;
        }

        public void SetDefagLimit(int value)
        {
            this.PropCacheDefragLimit = value;
        }

        public void SetEventLogLevel(int level)
        {
            this._propEventLogLevel = level;
            if (this.AppLog != null)
            {
                this.AppLog.SetLevel(level);
            }
        }

        public void SetIncrementBackup(bool val)
        {
            lock (this)
            {
                if (val != this.PropIncrementBackup)
                {
                    if (this.log != null)
                    {
                        this.log.SetIncrementBackup(val);
                        if (this.log.HasCache())
                        {
                            this._database.logger.CheckpointRequired = true;
                        }
                    }
                    this.PropIncrementBackup = val;
                }
            }
        }

        public void SetLobFileScale(int value)
        {
            if (this._propLobBlockSize != (value * 0x400))
            {
                CheckPower(value, 6);
                if (this._database.lobManager.GetLobCount() > 0)
                {
                    throw Error.GetError(0x1d5);
                }
                this._propLobBlockSize = value * 0x400;
                this._database.lobManager.Close();
                this._database.lobManager.Open();
            }
        }

        public void SetLobFileScaleNoCheck(int value)
        {
            CheckPower(value, 6);
            this._propLobBlockSize = value * 0x400;
        }

        public void SetLogData(bool mode)
        {
            lock (this)
            {
                this.PropLogData = mode;
                this._loggingEnabled = this.PropLogData && !this._database.IsFilesReadOnly();
                this._loggingEnabled &= this._logsStatements;
            }
        }

        public void SetLogSize(int megas)
        {
            lock (this)
            {
                this.PropLogSize = megas;
                if (this.log != null)
                {
                    this.log.SetLogSize(this.PropLogSize);
                }
            }
        }

        public void SetNioDataFile(bool value)
        {
            this.PropNioDataFile = value;
        }

        private void SetVariables()
        {
            string property = this._database.UrlProperties.GetProperty("crypt_key");
            if (property != null)
            {
                string cipherName = this._database.UrlProperties.GetProperty("crypt_type");
                string iVString = this._database.UrlProperties.GetProperty("crypt_iv");
                this._crypto = new Crypto(property, iVString, cipherName);
            }
            if (this._database.DatabaseProperties.IsPropertyTrue("readonly"))
            {
                this._database.SetReadOnly();
            }
            if (this._database.DatabaseProperties.IsPropertyTrue("LibCore.files_readonly"))
            {
                this._database.SetFilesReadOnly();
            }
            if (!this._database.IsFilesReadOnly())
            {
                if ((this._database.GetDatabaseType() == "mem:") || this.isStoredFileAccess)
                {
                    this._tempDirectoryPath = this._database.GetProperties().GetStringProperty("LibCore.temp_directory");
                }
                else
                {
                    this._tempDirectoryPath = this._database.GetPath() + ".tmp";
                }
                if (this._tempDirectoryPath != null)
                {
                    this._tempDirectoryPath = FileUtil.GetDefaultInstance().MakeDirectories(this._tempDirectoryPath);
                }
            }
            if (this._tempDirectoryPath != null)
            {
                int integerProperty = this._database.DatabaseProperties.GetIntegerProperty("LibCore.result_max_memory_rows");
                this._database.SetResultMaxMemoryRows(integerProperty);
            }
            string stringProperty = this._database.DatabaseProperties.GetStringProperty("LibCore.default_table_type");
            if ("CACHED".Equals(stringProperty, StringComparison.OrdinalIgnoreCase))
            {
                this._database.schemaManager.SetDefaultTableType(5);
            }
            string str3 = this._database.DatabaseProperties.GetStringProperty("LibCore.tx");
            if ("MVCC".Equals(str3, StringComparison.OrdinalIgnoreCase))
            {
                this._propTxMode = 2;
            }
            else if ("MVLOCKS".Equals(str3, StringComparison.OrdinalIgnoreCase))
            {
                this._propTxMode = 1;
            }
            else if ("LOCKS".Equals(str3, StringComparison.OrdinalIgnoreCase))
            {
                this._propTxMode = 0;
            }
            switch (this._propTxMode)
            {
                case 0:
                    this._database.TxManager = new TransactionManager2PL(this._database);
                    break;

                case 1:
                    this._database.TxManager = new TransactionManagerMV2PL(this._database);
                    break;

                case 2:
                    this._database.TxManager = new TransactionManagerMvcc(this._database);
                    break;
            }
            string str4 = this._database.DatabaseProperties.GetStringProperty("LibCore.tx_level");
            if ("SERIALIZABLE".Equals(str4, StringComparison.OrdinalIgnoreCase))
            {
                this._database.DefaultIsolationLevel = 0x100000;
            }
            else
            {
                this._database.DefaultIsolationLevel = 0x1000;
            }
            this._database.SqlEnforceRefs = this._database.DatabaseProperties.IsPropertyTrue("sql.enforce_refs");
            this._database.SqlEnforceSize = this._database.DatabaseProperties.IsPropertyTrue("sql.enforce_strict_size");
            this._database.SqlEnforceSize = this._database.DatabaseProperties.IsPropertyTrue("sql.enforce_size");
            this._database.SqlEnforceNames = this._database.DatabaseProperties.IsPropertyTrue("sql.enforce_names");
            if (this._database.DatabaseProperties.IsPropertyTrue("sql.compare_in_locale"))
            {
                this._database.collation.SetCollationAsLocale();
            }
            this._propEventLogLevel = this._database.DatabaseProperties.GetIntegerProperty("LibCore.applog");
            this.PropFilesReadOnly = this._database.DatabaseProperties.IsPropertyTrue("LibCore.files_readonly");
            this.PropDatabaseReadOnly = this._database.DatabaseProperties.IsPropertyTrue("readonly");
            this.PropIncrementBackup = this._database.DatabaseProperties.IsPropertyTrue("LibCore.incremental_backup");
            this.PropNioDataFile = this._database.DatabaseProperties.IsPropertyTrue("LibCore.nio_data_file");
            this.PropCacheMaxRows = this._database.DatabaseProperties.GetIntegerProperty("LibCore.cache_rows");
            this.PropCacheMaxSize = this._database.DatabaseProperties.GetIntegerProperty("LibCore.cache_size") * 0x400;
            this.SetLobFileScaleNoCheck(this._database.DatabaseProperties.GetIntegerProperty("LibCore.lob_file_scale"));
            this.SetCacheFileScaleNoCheck(this._database.DatabaseProperties.GetIntegerProperty("LibCore.cache_file_scale"));
            this.PropCacheFileScale = this._database.DatabaseProperties.GetIntegerProperty("LibCore.cache_file_scale");
            this.PropCacheDefragLimit = this._database.DatabaseProperties.GetIntegerProperty("LibCore.defrag_limit");
            this.PropMaxFreeBlocks = this._database.DatabaseProperties.GetIntegerProperty("LibCore.cache_free_count_scale");
            this.PropMaxFreeBlocks = ((int) 1) << this.PropMaxFreeBlocks;
            this.PropWriteDelay = this._database.DatabaseProperties.GetIntegerProperty("LibCore.write_delay_millis");
            if (!this._database.DatabaseProperties.IsPropertyTrue("LibCore.write_delay"))
            {
                this.PropWriteDelay = 0;
            }
            this.PropLogSize = this._database.DatabaseProperties.GetIntegerProperty("LibCore.log_size");
            this.PropLogData = this._database.DatabaseProperties.IsPropertyTrue("LibCore.log_data");
            this._propGc = this._database.DatabaseProperties.GetIntegerProperty("runtime.gc_interval");
            this._propRefIntegrity = this._database.DatabaseProperties.IsPropertyTrue("sql.ref_integrity");
        }

        public void SetWriteDelay(int delay)
        {
            lock (this)
            {
                this.PropWriteDelay = delay;
                if (this.log != null)
                {
                    this._syncFile = delay == 0;
                    this.log.SetWriteDelay(delay);
                }
            }
        }

        public void StopLogging()
        {
            this._loggingEnabled = false;
        }

        public void SynchLog()
        {
            lock (this)
            {
                if (this._loggingEnabled && this._syncFile)
                {
                    this.log.SynchLog();
                }
            }
        }

        public void WriteCommitStatement(Session session)
        {
            lock (this)
            {
                if (this._loggingEnabled)
                {
                    this.log.WriteCommitStatement(session);
                    this.SynchLog();
                }
            }
        }

        public void WriteDeleteStatement(Session session, Table t, object[] row)
        {
            lock (this)
            {
                if (this._loggingEnabled && !t.isSessionBased)
                {
                    this.log.WriteDeleteStatement(session, t, row);
                }
            }
        }

        public void WriteInsertStatement(Session session, Table table, object[] row)
        {
            lock (this)
            {
                if (this._loggingEnabled && !table.isSessionBased)
                {
                    this.log.WriteInsertStatement(session, table, row);
                }
            }
        }

        public void WriteSequenceStatement(Session session, NumberSequence s)
        {
            lock (this)
            {
                if (this._loggingEnabled)
                {
                    this.log.WriteSequenceStatement(session, s);
                }
            }
        }

        public void WriteToLog(Session session, string statement)
        {
            lock (this)
            {
                if (this._loggingEnabled)
                {
                    this.log.WriteStatement(session, statement);
                }
            }
        }
    }
}

