namespace FwNs.Core.LC.cEngine
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConcurrency;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cDbInfos;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;

    public sealed class Database : IDisposable
    {
        public const int DatabaseOnline = 1;
        public const int DatabaseOpening = 4;
        public const int DatabaseClosing = 8;
        public const int DatabaseShutdown = 0x10;
        public const int ClosemodeImmediately = -1;
        public const int ClosemodeNormal = 0;
        public const int ClosemodeCompact = 1;
        public const int ClosemodeScript = 2;
        public int DatabaseId;
        private string _databaseUniqueName;
        private readonly string _databaseType;
        private readonly string _canonicalPath;
        public LibCoreProperties UrlProperties;
        private readonly string _path;
        public DatabaseInformation DbInfo;
        private int _dbState;
        public Logger logger;
        public bool DatabaseReadOnly;
        private bool _filesReadOnly;
        public bool FilesInAssembly;
        public bool SqlEnforceRefs;
        public bool SqlEnforceSize;
        public bool SqlEnforceNames;
        private bool _isReferentialIntegrity;
        public LibCoreDatabaseProperties DatabaseProperties;
        private readonly bool _shutdownOnNoConnection;
        private int _resultMaxMemoryRows;
        public UserManager userManager;
        public GranteeManager granteeManager;
        public QNameManager NameManager;
        public SessionManager sessionManager;
        public ITransactionManager TxManager;
        public int DefaultIsolationLevel = 0x1000;
        public SchemaManager schemaManager;
        public PersistentStoreCollectionDatabase persistentStoreCollection;
        public LobManager lobManager;
        public Collation collation;

        public Database(string type, string path, string canonicalPath, LibCoreProperties props)
        {
            this.SetState(0x10);
            this._databaseType = type;
            this._path = path;
            this._canonicalPath = canonicalPath;
            this.UrlProperties = props;
            if (this._databaseType == "res:")
            {
                this.FilesInAssembly = true;
                this._filesReadOnly = true;
            }
            this.logger = new Logger(this);
            this._shutdownOnNoConnection = this.UrlProperties.IsPropertyTrue("shutdown");
            this.lobManager = new LobManager(this);
        }

        private static void AddRows(Result r, string[] sql)
        {
            if (sql != null)
            {
                for (int i = 0; i < sql.Length; i++)
                {
                    string[] data = new string[] { sql[i] };
                    r.InitialiseNavigator().Add(data);
                }
            }
        }

        private void ClearStructures()
        {
            if (this.schemaManager != null)
            {
                this.schemaManager.ClearStructures();
            }
            this.granteeManager = null;
            this.userManager = null;
            this.NameManager = null;
            this.schemaManager = null;
            this.DbInfo = null;
        }

        public void Close(int closemode)
        {
            CoreException error = null;
            this.SetState(8);
            this.sessionManager.CloseAllSessions();
            this.sessionManager.ClearAll();
            if (this._filesReadOnly)
            {
                closemode = -1;
            }
            this.logger.ClosePersistence(closemode);
            this.lobManager.Close();
            try
            {
                if (closemode == 1)
                {
                    this.ClearStructures();
                    this.Reopen();
                    this.SetState(8);
                    this.logger.ClosePersistence(0);
                }
            }
            catch (Exception exception2)
            {
                CoreException exception3 = exception2 as CoreException;
                if (exception3 != null)
                {
                    error = exception3;
                }
                else
                {
                    error = Error.GetError(0x1ca, exception2.Message);
                }
            }
            this.logger.ReleaseLock();
            this.SetState(0x10);
            this.ClearStructures();
            DatabaseManager.RemoveDatabase(this);
            if (error != null)
            {
                throw error;
            }
            this.Dispose();
        }

        public void CloseIfLast()
        {
            if ((this._shutdownOnNoConnection && this.sessionManager.IsEmpty()) && (this._dbState == 1))
            {
                try
                {
                    this.Close(0);
                }
                catch (CoreException)
                {
                }
            }
        }

        public Session Connect(string username, string password, string zoneString, int timeZoneSeconds)
        {
            lock (this)
            {
                if (username.Equals("SA", StringComparison.OrdinalIgnoreCase))
                {
                    username = "SA";
                }
                User user = this.userManager.GetUser(username, password);
                return this.sessionManager.NewSession(this, user, this.DatabaseReadOnly, false, zoneString, timeZoneSeconds);
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
                this.sessionManager.Dispose();
                this.logger.Dispose();
                ((TransactionManagerCommon) this.TxManager).Dispose();
            }
            if (this.GetState() == 1)
            {
                try
                {
                    this.Close(-1);
                }
                catch (Exception)
                {
                }
            }
        }

        ~Database()
        {
            this.Dispose(false);
        }

        public string GetCanonicalPath()
        {
            return this._canonicalPath;
        }

        public QNameManager.QName GetCatalogName()
        {
            return this.NameManager.GetCatalogName();
        }

        public int GetDatabaseId()
        {
            return this.DatabaseId;
        }

        public string GetDatabaseType()
        {
            return this._databaseType;
        }

        public int GetDefaultIsolationLevel()
        {
            return this.DefaultIsolationLevel;
        }

        public GranteeManager GetGranteeManager()
        {
            return this.granteeManager;
        }

        public string GetPath()
        {
            return this._path;
        }

        public LibCoreDatabaseProperties GetProperties()
        {
            return this.DatabaseProperties;
        }

        public int GetResultMaxMemoryRows()
        {
            return this._resultMaxMemoryRows;
        }

        public Result GetScript(bool indexRoots)
        {
            Result r = Result.NewSingleColumnResult("COMMAND", SqlType.SqlVarchar);
            string[] propertiesSql = this.logger.GetPropertiesSql();
            AddRows(r, propertiesSql);
            propertiesSql = this.GetSettingsSql();
            AddRows(r, propertiesSql);
            propertiesSql = this.GetGranteeManager().GetSQL();
            AddRows(r, propertiesSql);
            propertiesSql = this.schemaManager.GetSqlArray();
            AddRows(r, propertiesSql);
            propertiesSql = this.schemaManager.GetCommentsArray();
            AddRows(r, propertiesSql);
            propertiesSql = this.GetUserManager().GetInitialSchemaSQL();
            AddRows(r, propertiesSql);
            propertiesSql = this.GetGranteeManager().GetRightstSQL();
            AddRows(r, propertiesSql);
            if (indexRoots)
            {
                propertiesSql = this.schemaManager.GetIndexRootsSql();
                AddRows(r, propertiesSql);
            }
            propertiesSql = this.schemaManager.GetTablePropsSql(!indexRoots);
            AddRows(r, propertiesSql);
            return r;
        }

        public SessionManager GetSessionManager()
        {
            return this.sessionManager;
        }

        public string[] GetSettingsSql()
        {
            List<string> list = new List<string>();
            if (!this.GetCatalogName().Name.Equals("PUBLIC"))
            {
                list.Add("ALTER CATALOG PUBLIC RENAME TO " + this.GetCatalogName().StatementName);
            }
            if (this.collation.Collator != null)
            {
                list.Add("SET DATABASE COLLATION " + this.collation.GetName().StatementName);
            }
            return list.ToArray();
        }

        public int GetState()
        {
            lock (this)
            {
                return this._dbState;
            }
        }

        public string GetUniqueName()
        {
            return this._databaseUniqueName;
        }

        public string GetUri()
        {
            return (this._databaseType + this._canonicalPath);
        }

        public LibCoreProperties GetUrlProperties()
        {
            return this.UrlProperties;
        }

        public UserManager GetUserManager()
        {
            return this.userManager;
        }

        public bool IsFilesInAssembly()
        {
            return this.FilesInAssembly;
        }

        public bool IsFilesReadOnly()
        {
            return this._filesReadOnly;
        }

        public bool IsReadOnly()
        {
            return this.DatabaseReadOnly;
        }

        public bool IsReferentialIntegrity()
        {
            return this._isReferentialIntegrity;
        }

        public bool IsShutdown()
        {
            lock (this)
            {
                return (this._dbState == 0x10);
            }
        }

        public void Open()
        {
            lock (this)
            {
                if (this.IsShutdown())
                {
                    this.Reopen();
                }
            }
        }

        private void Reopen()
        {
            this.SetState(4);
            try
            {
                this.NameManager = new QNameManager(this);
                this.granteeManager = new GranteeManager(this);
                this.userManager = new UserManager(this);
                this.schemaManager = new SchemaManager(this);
                this.persistentStoreCollection = new PersistentStoreCollectionDatabase();
                this._isReferentialIntegrity = true;
                this.sessionManager = new SessionManager(this);
                this.collation = Collation.GetDefaultInstance();
                this.DbInfo = DatabaseInformation.NewDatabaseInformation(this);
                this.TxManager = new TransactionManagerMvcc(this);
                this.lobManager.CreateSchema();
                this.sessionManager.GetSysLobSession().SetSchema("SYSTEM_LOBS");
                this.schemaManager.SetSchemaChangeTimestamp();
                this.logger.OpenPersistence();
                if (this.logger.IsNewDatabase)
                {
                    string property = this.UrlProperties.GetProperty("user", "SA");
                    string password = this.UrlProperties.GetProperty("password", "");
                    this.userManager.CreateFirstUser(property, password);
                    this.schemaManager.CreatePublicSchema();
                    this.lobManager.InitialiseLobSpace();
                    this.logger.Checkpoint(false);
                }
                this.lobManager.Open();
                this.DbInfo.SetWithContent(true);
            }
            catch (Exception error)
            {
                this.logger.ClosePersistence(-1);
                this.logger.ReleaseLock();
                this.SetState(0x10);
                this.ClearStructures();
                DatabaseManager.RemoveDatabase(this);
                if (!(error is CoreException))
                {
                    error = Error.GetError(0x1ca, error);
                }
                this.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.Database_Reopen_could_not_reopen_database, error);
                throw;
            }
            this.SetState(1);
        }

        public void SetFilesReadOnly()
        {
            this._filesReadOnly = true;
        }

        public void SetReadOnly()
        {
            this.DatabaseReadOnly = true;
            this._filesReadOnly = true;
        }

        public void SetReferentialIntegrity(bool reference)
        {
            this._isReferentialIntegrity = reference;
        }

        public void SetResultMaxMemoryRows(int size)
        {
            this._resultMaxMemoryRows = size;
        }

        private void SetState(int state)
        {
            lock (this)
            {
                this._dbState = state;
            }
        }

        public void SetStrictColumnSize(bool mode)
        {
            this.SqlEnforceSize = mode;
        }

        public void SetStrictNames(bool mode)
        {
            this.SqlEnforceNames = mode;
        }

        public void SetStrictReferences(bool mode)
        {
            this.SqlEnforceRefs = mode;
        }

        public void SetUniqueName(string name)
        {
            this._databaseUniqueName = name;
        }
    }
}

