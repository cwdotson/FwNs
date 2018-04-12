namespace System.Data.LibCore
{
    using System;
    using System.Data;
    using System.Transactions;

    public class UtlEnlistment : IEnlistmentNotification
    {
        private UtlConnectionProxy _connectionProxy;
        private Transaction _scope;
        private UtlTransaction _transaction;
        public bool DisposeConnection;

        public UtlEnlistment(UtlConnectionProxy connectionProxy, Transaction scope)
        {
            this._scope = scope;
            this._connectionProxy = connectionProxy;
            System.Data.IsolationLevel readCommitted = System.Data.IsolationLevel.ReadCommitted;
            switch (this._scope.IsolationLevel)
            {
                case System.Transactions.IsolationLevel.Serializable:
                    readCommitted = System.Data.IsolationLevel.Serializable;
                    break;

                case System.Transactions.IsolationLevel.RepeatableRead:
                    readCommitted = System.Data.IsolationLevel.RepeatableRead;
                    break;

                case System.Transactions.IsolationLevel.ReadCommitted:
                    readCommitted = System.Data.IsolationLevel.ReadCommitted;
                    break;

                case System.Transactions.IsolationLevel.ReadUncommitted:
                    readCommitted = System.Data.IsolationLevel.ReadUncommitted;
                    break;

                case System.Transactions.IsolationLevel.Snapshot:
                    readCommitted = System.Data.IsolationLevel.Snapshot;
                    break;

                case System.Transactions.IsolationLevel.Chaos:
                    readCommitted = System.Data.IsolationLevel.Chaos;
                    break;

                case System.Transactions.IsolationLevel.Unspecified:
                    readCommitted = System.Data.IsolationLevel.Unspecified;
                    break;
            }
            this._transaction = this._connectionProxy.BeginTransaction(readCommitted);
            this._scope = scope;
            this._scope.EnlistVolatile(this, EnlistmentOptions.None);
        }

        private void Cleanup(UtlConnectionProxy cnn)
        {
            if (this.DisposeConnection || (!cnn.IsPooled && ((cnn.Owner == null) || cnn.Owner.IsClosed)))
            {
                cnn.Dispose();
            }
            this._transaction = null;
            this._scope = null;
            this._connectionProxy = null;
        }

        public void Commit(Enlistment enlistment)
        {
            if ((this._transaction != null) && !this._transaction.IsCompleted)
            {
                this._connectionProxy.Enlistment = null;
                try
                {
                    this._transaction.IsValid(true);
                    this._transaction.Commit();
                    enlistment.Done();
                }
                finally
                {
                    this.Cleanup(this._connectionProxy);
                }
            }
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            if ((this._transaction == null) || this._transaction.IsCompleted)
            {
                preparingEnlistment.ForceRollback();
            }
            else if (!this._transaction.PrepareCommit())
            {
                preparingEnlistment.ForceRollback();
            }
            else
            {
                preparingEnlistment.Prepared();
            }
        }

        public void Rollback(Enlistment enlistment)
        {
            if ((this._transaction != null) && !this._transaction.IsCompleted)
            {
                this._connectionProxy.Enlistment = null;
                try
                {
                    this._transaction.Rollback();
                    enlistment.Done();
                }
                finally
                {
                    this.Cleanup(this._connectionProxy);
                }
            }
        }

        public UtlTransaction DbTransaction
        {
            get
            {
                return this._transaction;
            }
            set
            {
                this._transaction = value;
            }
        }

        public Transaction Scope
        {
            get
            {
                return this._scope;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return (this._transaction == null);
            }
        }
    }
}

