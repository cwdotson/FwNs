namespace System.Data.LibCore
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cResources;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using System.Threading;

    public sealed class UtlTransaction : DbTransaction
    {
        public UtlConnection Conn;
        private System.Data.IsolationLevel _isolationLevel;
        private bool _readOnly;
        private bool _oldAutoCommit;
        public bool IsCompleted;

        public UtlTransaction(UtlConnection connection, System.Data.IsolationLevel isolationLevel, bool readOnly)
        {
            this.BeginTransaction(connection, isolationLevel, readOnly);
        }

        public void BeginTransaction(UtlConnection connection, System.Data.IsolationLevel isolationLevel, bool readOnly)
        {
            this.Conn = connection;
            this._isolationLevel = isolationLevel;
            this._readOnly = readOnly;
            this.IsValid(true);
            this._oldAutoCommit = this.Conn.AutoCommit;
            this.Conn.AutoCommit = false;
            this.SetTransactionCharacteristics();
        }

        public override void Commit()
        {
            this.IsValid(true);
            UtlConnection conn = Interlocked.Exchange<UtlConnection>(ref this.Conn, null);
            try
            {
                conn.InnerConnection.SessionProxy.Commit(true);
            }
            catch (CoreException exception1)
            {
                UtlException.ThrowError(exception1);
            }
            finally
            {
                this.RestoreAutoCommit(conn);
            }
            this.IsCompleted = true;
        }

        public void CreateSavePoint(string savePoint)
        {
            if (string.IsNullOrEmpty(savePoint))
            {
                throw new ArgumentException("Required argumet missing : SavePoint");
            }
            this.IsValid(true);
            try
            {
                this.Conn.InnerConnection.SessionProxy.Savepoint(savePoint);
            }
            catch (CoreException exception1)
            {
                UtlException.ThrowError(exception1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.IsValid(false))
                {
                    this.IssueRollback();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public void IssueRollback()
        {
            UtlConnection conn = Interlocked.Exchange<UtlConnection>(ref this.Conn, null);
            if (conn != null)
            {
                try
                {
                    conn.InnerConnection.SessionProxy.Rollback(true);
                }
                catch (CoreException exception1)
                {
                    UtlException.ThrowError(exception1);
                }
                finally
                {
                    this.RestoreAutoCommit(conn);
                }
            }
        }

        public bool IsValid(bool throwError)
        {
            if (this.Conn == null)
            {
                if (throwError)
                {
                    throw new ArgumentNullException(FwNs.Core.LC.cResources.SR.UtlTransaction_IsValid_No_connection_associated_with_this_transaction);
                }
                return false;
            }
            if (this.Conn.State != ConnectionState.Open)
            {
                if (throwError)
                {
                    throw new UtlException(FwNs.Core.LC.cResources.SR.UtlTransaction_IsValid_Connection_was_closed);
                }
                return false;
            }
            if (!this.IsCompleted)
            {
                return true;
            }
            if (throwError)
            {
                throw new UtlException(FwNs.Core.LC.cResources.SR.UtlTransaction_IsValid_No_transaction_is_active_on_this_connection);
            }
            return false;
        }

        public bool PrepareCommit()
        {
            if (!this.IsValid(false))
            {
                return false;
            }
            try
            {
                this.Conn.InnerConnection.SessionProxy.PrepareCommit();
            }
            catch (CoreException)
            {
                return false;
            }
            return true;
        }

        public void ReleaseSavePoint(string savePoint)
        {
            try
            {
                this.Conn.InnerConnection.SessionProxy.ReleaseSavepoint(savePoint);
            }
            catch (CoreException exception1)
            {
                UtlException.ThrowError(exception1);
            }
        }

        private void RestoreAutoCommit(UtlConnection conn)
        {
            conn.AutoCommit = this._oldAutoCommit;
        }

        public override void Rollback()
        {
            this.IsValid(true);
            this.IssueRollback();
            this.IsCompleted = true;
        }

        public void RollbackToSavePoint(string savePoint)
        {
            this.IsValid(true);
            try
            {
                this.Conn.InnerConnection.SessionProxy.RollbackToSavepoint(savePoint);
            }
            catch (CoreException exception1)
            {
                UtlException.ThrowError(exception1);
            }
        }

        private void SetTransactionCharacteristics()
        {
            if ((this.Conn.ReadOnly == this.ReadOnly) && (this.Conn.IsolationLevel == this.IsolationLevel))
            {
                this.Conn.InnerConnection.ExecuteDirect("START TRANSACTION;");
            }
            else
            {
                this.Conn.InnerConnection.SessionProxy.Rollback(false);
                StringBuilder builder = new StringBuilder();
                builder.Append("START TRANSACTION ISOLATION LEVEL ");
                System.Data.IsolationLevel isolationLevel = this.IsolationLevel;
                switch (isolationLevel)
                {
                    case System.Data.IsolationLevel.ReadUncommitted:
                        builder.Append("READ UNCOMMITTED ");
                        break;

                    case System.Data.IsolationLevel.RepeatableRead:
                        builder.Append("REPEATABLE READ ");
                        break;

                    default:
                        if (isolationLevel != System.Data.IsolationLevel.Serializable)
                        {
                            builder.Append("READ COMMITTED ");
                        }
                        else
                        {
                            builder.Append("SERIALIZABLE  ");
                        }
                        break;
                }
                builder.Append(this.ReadOnly ? "READ ONLY " : "READ WRITE ");
                builder.Append(" ;");
                string sql = builder.ToString();
                this.Conn.InnerConnection.ExecuteDirect(sql);
            }
        }

        public UtlConnection Connection
        {
            get
            {
                return this.Conn;
            }
        }

        public override System.Data.IsolationLevel IsolationLevel
        {
            get
            {
                return this._isolationLevel;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return this._readOnly;
            }
        }

        protected override System.Data.Common.DbConnection DbConnection
        {
            get
            {
                return this.Connection;
            }
        }
    }
}

