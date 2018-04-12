namespace System.Data.LibCore
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cPersist;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Transactions;

    public sealed class UtlConnection : DbConnection, ICloneable
    {
        private ConnectionState _connectionState;
        private string _connectionString;
        private UtlConnectionOptions _connectionOptions;
        public long Version;
        public bool IsInternal;
        private int _poolVersion;
        public UtlConnectionProxy InnerConnection;

        [field: CompilerGenerated]
        public event StateChangeEventHandler StateChange;

        public UtlConnection() : this(string.Empty)
        {
        }

        public UtlConnection(ISessionInterface sessionProxy)
        {
            this._connectionState = ConnectionState.Closed;
            this._connectionOptions = new UtlConnectionOptions();
            this._connectionString = string.Empty;
            this.Version += 1L;
            this.InnerConnection = new UtlConnectionProxy(sessionProxy, this._connectionOptions, this);
            this._connectionState = ConnectionState.Open;
        }

        public UtlConnection(UtlConnection connection) : this(connection.ConnectionString)
        {
            if (connection.State == ConnectionState.Open)
            {
                this.Open();
            }
        }

        public UtlConnection(string connectionString)
        {
            this._connectionState = ConnectionState.Closed;
            this._connectionOptions = new UtlConnectionOptions();
            this._connectionString = string.Empty;
            if (!string.IsNullOrEmpty(connectionString))
            {
                this.ConnectionString = connectionString;
            }
        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return this.BeginTransaction(isolationLevel, false);
        }

        public UtlTransaction BeginTransaction()
        {
            return this.BeginTransaction(this.IsolationLevel, this.ReadOnly);
        }

        public UtlTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return this.BeginTransaction(isolationLevel, false);
        }

        public UtlTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel, bool readOnly)
        {
            this.CheckOpen();
            if (isolationLevel == System.Data.IsolationLevel.Unspecified)
            {
                isolationLevel = this.IsolationLevel;
            }
            if ((isolationLevel == System.Data.IsolationLevel.Snapshot) || (isolationLevel == System.Data.IsolationLevel.Chaos))
            {
                throw new ArgumentException("Invalid IsolationLevel");
            }
            return this.InnerConnection.BeginTransaction(isolationLevel, readOnly);
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string newPassword)
        {
            this.CheckOpen();
            string sql = "SET IDENTIFIED BY \"" + newPassword + "\";";
            this.InnerConnection.ExecuteDirect(sql);
        }

        public void CheckOpen()
        {
            if (this._connectionState != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection is not open.");
            }
        }

        public static void ClearAllPools()
        {
            UtlConnectionPool.ClearAllPools();
        }

        public static void ClearPool(UtlConnection connection)
        {
            UtlConnectionPool.ClearPool(connection.ConnectionString);
        }

        public override void Close()
        {
            if ((this.State != ConnectionState.Closed) && (this.InnerConnection != null))
            {
                if (this.InnerConnection.IsEnlisted)
                {
                    UtlConnection connection = new UtlConnection {
                        InnerConnection = this.InnerConnection
                    };
                    connection.InnerConnection.Owner = connection;
                    connection.ConnectionString = this.ConnectionString;
                    connection._connectionOptions = this._connectionOptions;
                    connection.Version = this.Version;
                    connection._connectionState = this._connectionState;
                    connection.InnerConnection.Enlistment.DbTransaction.Conn = connection;
                    connection.InnerConnection.Enlistment.DisposeConnection = true;
                    this.InnerConnection = null;
                }
                try
                {
                    if (this.InnerConnection != null)
                    {
                        lock (this.InnerConnection)
                        {
                            this.InnerConnection.DisposeTransaction();
                            this.InnerConnection.Owner = null;
                            if (this._connectionOptions.Pooling)
                            {
                                UtlConnectionPool.Add(this._connectionString, this.InnerConnection, this._poolVersion);
                            }
                            else
                            {
                                this.InnerConnection.Dispose();
                            }
                        }
                    }
                    this.InnerConnection = null;
                }
                catch (Exception)
                {
                }
                finally
                {
                    this.OnStateChange(ConnectionState.Closed);
                }
            }
        }

        public UtlCommand CreateCommand()
        {
            return new UtlCommand(this);
        }

        protected override DbCommand CreateDbCommand()
        {
            return this.CreateCommand();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.Close();
                this._connectionString = string.Empty;
                this._connectionOptions = null;
            }
        }

        public override void EnlistTransaction(Transaction transaction)
        {
            this.CheckOpen();
            this.InnerConnection.EnlistTransaction(transaction);
        }

        public override DataTable GetSchema()
        {
            return this.GetSchema("MetaDataCollections", null);
        }

        public override DataTable GetSchema(string collectionName)
        {
            return this.GetSchema(collectionName, new string[0]);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            this.CheckOpen();
            return new UtlMetaData(this).GetMetaData(collectionName, restrictionValues);
        }

        public void OnStateChange(ConnectionState newState)
        {
            ConnectionState originalState = this._connectionState;
            this._connectionState = newState;
            if ((this.StateChange != null) && (originalState != newState))
            {
                StateChangeEventArgs e = new StateChangeEventArgs(originalState, newState);
                this.StateChange(this, e);
            }
        }

        public override void Open()
        {
            if (this._connectionState != ConnectionState.Closed)
            {
                throw new InvalidOperationException("Connection already open.");
            }
            if (this._connectionOptions.Enlist && (Transaction.Current == null))
            {
                throw new InvalidOperationException("No active Transaction to enlist.");
            }
            try
            {
                if (this._connectionOptions.Pooling)
                {
                    this.InnerConnection = UtlConnectionPool.Remove(this._connectionString, this._connectionOptions.MaxPoolSize, this._connectionOptions.MinPoolSize, out this._poolVersion);
                    if ((this.InnerConnection != null) && this.InnerConnection.SessionProxy.IsClosed())
                    {
                        this.InnerConnection = null;
                    }
                }
                if (this.InnerConnection == null)
                {
                    this.InnerConnection = new UtlConnectionProxy(this._connectionOptions, this);
                    this.InnerConnection.Open();
                }
                else
                {
                    this.InnerConnection.Owner = this;
                }
                this.Version += 1L;
                ConnectionState state = this._connectionState;
                this._connectionState = ConnectionState.Open;
                if ((Transaction.Current != null) && this._connectionOptions.Enlist)
                {
                    this.EnlistTransaction(Transaction.Current);
                }
                this._connectionState = state;
                this.OnStateChange(ConnectionState.Open);
            }
            catch (UtlException)
            {
                this.Close();
                throw;
            }
        }

        public void Shutdown()
        {
            this.CheckOpen();
            string sql = "SHUTDOWN;";
            this.InnerConnection.ExecuteDirect(sql);
        }

        object ICloneable.Clone()
        {
            return new UtlConnection(this.ConnectionString);
        }

        public bool IsNetConnection
        {
            get
            {
                return ((this._connectionOptions.ConnectionType != "mem:") && (this._connectionOptions.ConnectionType != "file:"));
            }
        }

        public System.Data.IsolationLevel IsolationLevel
        {
            get
            {
                this.CheckOpen();
                return this.InnerConnection.GetIsolationLevel();
            }
            set
            {
                this.CheckOpen();
                this.InnerConnection.SetIsolationLevel(value);
            }
        }

        public bool ReadOnly
        {
            get
            {
                this.CheckOpen();
                return this.InnerConnection.IsReadOnly();
            }
            set
            {
                this.CheckOpen();
                this.InnerConnection.SetReadOnly(value);
            }
        }

        public bool AutoCommit
        {
            get
            {
                this.CheckOpen();
                return this.InnerConnection.IsAutoCommit();
            }
            set
            {
                this.CheckOpen();
                this.InnerConnection.SetAutoCommit(value);
            }
        }

        public bool IsClosed
        {
            get
            {
                bool flag1 = (this.InnerConnection != null) && this.InnerConnection.VerifyConnection();
                if (flag1 && (this._connectionState != ConnectionState.Closed))
                {
                    this._connectionState = ConnectionState.Closed;
                }
                return flag1;
            }
        }

        public override string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (this._connectionState != ConnectionState.Closed)
                {
                    throw new InvalidOperationException();
                }
                this._connectionOptions.ParseConnectionString(value);
                if (!this._connectionOptions.ContextConnection)
                {
                    this._connectionOptions.Validate();
                }
                this._connectionString = value;
            }
        }

        public override string DataSource
        {
            get
            {
                this.CheckOpen();
                return this._connectionOptions.Database;
            }
        }

        public override string Database
        {
            get
            {
                return this._connectionOptions.Database;
            }
        }

        public override string ServerVersion
        {
            get
            {
                this.CheckOpen();
                return LibCoreDatabaseProperties.ThisFullVersion;
            }
        }

        public override ConnectionState State
        {
            get
            {
                return this._connectionState;
            }
        }

        protected override System.Data.Common.DbProviderFactory DbProviderFactory
        {
            get
            {
                return UtlFactory.Instance;
            }
        }
    }
}

