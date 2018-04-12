namespace System.Data.LibCore
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using System;
    using System.Data;
    using System.Data.LibCore.Clr;
    using System.Transactions;

    public class UtlConnectionProxy : IDisposable
    {
        public ISessionInterface SessionProxy;
        public UtlConnection Owner;
        public UtlConnectionOptions ConnectionOptions;
        public UtlTransaction Transaction;
        public bool IsPooled;
        public long Created;
        public UtlEnlistment Enlistment;
        private bool _disposed;
        private bool _isClrSpConnection;

        public UtlConnectionProxy(UtlConnectionOptions connectionOptions) : this(connectionOptions, null)
        {
        }

        public UtlConnectionProxy(UtlConnectionOptions connectionOptions, UtlConnection owner)
        {
            this.ConnectionOptions = connectionOptions;
            this.Owner = owner;
            this.Created = DateTime.UtcNow.Ticks;
            this.IsPooled = false;
        }

        public UtlConnectionProxy(ISessionInterface sessionProxy, UtlConnectionOptions connectionOptions, UtlConnection owner)
        {
            this.SessionProxy = sessionProxy;
            this.ConnectionOptions = connectionOptions;
            this.Owner = owner;
            this.Created = DateTime.UtcNow.Ticks;
            this.IsPooled = false;
        }

        public UtlTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            if (isolationLevel <= System.Data.IsolationLevel.ReadCommitted)
            {
                if (isolationLevel == System.Data.IsolationLevel.ReadUncommitted)
                {
                    return this.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted, false);
                }
                return this.BeginTransaction(System.Data.IsolationLevel.ReadCommitted, false);
            }
            if (isolationLevel == System.Data.IsolationLevel.RepeatableRead)
            {
                return this.BeginTransaction(System.Data.IsolationLevel.RepeatableRead, false);
            }
            if (isolationLevel == System.Data.IsolationLevel.Serializable)
            {
                return this.BeginTransaction(System.Data.IsolationLevel.Serializable, false);
            }
            return this.BeginTransaction(System.Data.IsolationLevel.ReadCommitted, false);
        }

        public UtlTransaction BeginTransaction(System.Data.IsolationLevel level, bool readOnly)
        {
            lock (this)
            {
                if (this.TransactionActive)
                {
                    throw new InvalidOperationException("Transaction is already active.");
                }
                try
                {
                    this.Transaction = new UtlTransaction(this.Owner, level, readOnly);
                }
                catch (CoreException exception1)
                {
                    throw UtlException.GetException(exception1);
                }
            }
            return this.Transaction;
        }

        public void Close()
        {
            if (!this._isClrSpConnection && (this.SessionProxy != null))
            {
                try
                {
                    this.SessionProxy.Close();
                }
                catch (Exception)
                {
                }
                finally
                {
                    this.SessionProxy = null;
                    this.Owner = null;
                    this.IsPooled = false;
                    this.Created = 0L;
                    this.ConnectionOptions = null;
                }
            }
        }

        private LibCoreProperties CreateDbProperties()
        {
            LibCoreProperties properties = new LibCoreProperties();
            if (this.ConnectionOptions.CryptoType != null)
            {
                properties.SetProperty("crypt_type", this.ConnectionOptions.CryptoType);
                properties.SetProperty("crypt_key", this.ConnectionOptions.CryptoKey);
                properties.SetProperty("crypt_iv", this.ConnectionOptions.CryptoIv);
            }
            return properties;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                this.Close();
            }
            this._disposed = true;
        }

        public void DisposeTransaction()
        {
            if ((this.Transaction != null) && !this.IsEnlisted)
            {
                this.Transaction.Dispose();
                this.Transaction = null;
            }
            try
            {
                this.SessionProxy.Rollback(false);
            }
            catch (Exception)
            {
            }
        }

        public void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            if ((this.Owner != null) && this.ConnectionOptions.Enlist)
            {
                if (this.TransactionActive && (transaction != null))
                {
                    throw new ArgumentException("Unable to enlisttransaction (local transaction already exists)");
                }
                if ((this.Enlistment == null) || (transaction != this.Enlistment.Scope))
                {
                    if (this.Enlistment != null)
                    {
                        throw new ArgumentException("Already enlisted in a transaction");
                    }
                    this.Enlistment = new UtlEnlistment(this, transaction);
                }
            }
        }

        public Result ExecuteDirect(string sql)
        {
            Result result2;
            Result r = Result.NewExecuteDirectRequest();
            r.SetMainString(sql);
            try
            {
                result2 = this.SessionProxy.Execute(r);
                if (result2.IsError())
                {
                    throw result2.GetException();
                }
            }
            catch (CoreException exception1)
            {
                throw UtlException.GetException(exception1);
            }
            return result2;
        }

        public System.Data.IsolationLevel GetIsolationLevel()
        {
            return (System.Data.IsolationLevel) this.SessionProxy.GetIsolation();
        }

        public bool IsAutoCommit()
        {
            return this.SessionProxy.IsAutoCommit();
        }

        public bool IsReadOnly()
        {
            return this.SessionProxy.IsReadOnlyDefault();
        }

        public void Open()
        {
            LibCoreProperties props = this.CreateDbProperties();
            if (this.ConnectionOptions.ContextConnection)
            {
                this.SessionProxy = UtlContext.session;
                this._isClrSpConnection = true;
            }
            else
            {
                if (this.ConnectionOptions.SlOob)
                {
                    props.SetProperty("url_sl_oob", this.ConnectionOptions.SlOob);
                }
                props.SetProperty("shutdown", this.ConnectionOptions.AutoShutdown);
                if (!string.IsNullOrEmpty(this.ConnectionOptions.CryptoType))
                {
                    props.SetProperty("crypt_type", this.ConnectionOptions.CryptoType);
                    props.SetProperty("crypt_key", this.ConnectionOptions.CryptoKey);
                    props.SetProperty("crypt_iv", this.ConnectionOptions.CryptoIv);
                }
                try
                {
                    if (!DatabaseUrl.IsInProcessDatabaseType(this.ConnectionOptions.ConnectionType))
                    {
                        throw UtlException.GetException(0x1a7, this.ConnectionOptions.ConnectionType);
                    }
                    this.SessionProxy = DatabaseManager.NewSession(this.ConnectionOptions.ConnectionType, this.ConnectionOptions.Database, this.ConnectionOptions.User, this.ConnectionOptions.Password, props, "", 0);
                    if (!this.ConnectionOptions.AutoCommit)
                    {
                        this.SessionProxy.SetAutoCommit(this.ConnectionOptions.AutoCommit);
                    }
                    if (this.ConnectionOptions.Readonly)
                    {
                        this.SessionProxy.SetReadOnlyDefault(this.ConnectionOptions.Readonly);
                    }
                    if (this.ConnectionOptions.IsolationLevel != System.Data.IsolationLevel.ReadCommitted)
                    {
                        this.SessionProxy.SetIsolationDefault((int) this.ConnectionOptions.IsolationLevel);
                    }
                }
                catch (CoreException exception1)
                {
                    throw UtlException.GetException(exception1);
                }
            }
        }

        public void SetAutoCommit(bool autoCommit)
        {
            this.SessionProxy.SetAutoCommit(autoCommit);
        }

        public void SetIsolationLevel(System.Data.IsolationLevel isolationLevel)
        {
            this.SessionProxy.SetIsolationDefault((int) isolationLevel);
        }

        public void SetReadOnly(bool readOnly)
        {
            this.SessionProxy.SetReadOnlyDefault(readOnly);
        }

        public bool VerifyConnection()
        {
            try
            {
                return this.SessionProxy.IsClosed();
            }
            catch
            {
                return false;
            }
        }

        public long LifeTime
        {
            get
            {
                return (this.ConnectionOptions.ConnectionLifeTime * 0x989680L);
            }
        }

        public bool TransactionActive
        {
            get
            {
                return ((this.Transaction != null) && !this.Transaction.IsCompleted);
            }
        }

        public bool IsEnlisted
        {
            get
            {
                return ((this.Enlistment != null) && !this.Enlistment.IsCompleted);
            }
        }
    }
}

