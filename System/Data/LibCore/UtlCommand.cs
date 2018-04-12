namespace System.Data.LibCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;

    [Designer("LibCore.Designer.UtlCommandDesigner, FwNs.Core.Designer, Version=1.5.0.0, Culture=neutral, PublicKeyToken=9c147f7358eea142"), ToolboxItem(true)]
    public sealed class UtlCommand : DbCommand, ICloneable
    {
        private System.Data.CommandType _commandType;
        private string _commandText;
        private UtlConnection _cnn;
        private long _version;
        private WeakReference _activeReader;
        private int _commandTimeout;
        private bool _designTimeVisible;
        private UpdateRowSource _updateRowSource;
        private readonly UtlParameterCollection _parameterCollection;
        public UtlStatement Statement;
        private UtlTransaction _transaction;
        private List<UtlException> _warnings;
        public int MaxRows;
        public int FetchSize;
        public bool FetchGeneratedResults;

        public UtlCommand() : this(null, null, null)
        {
        }

        private UtlCommand(UtlCommand source) : this(source.CommandText, source.Connection, source.Transaction)
        {
            this.CommandTimeout = source.CommandTimeout;
            this._commandType = source.CommandType;
            this.DesignTimeVisible = source.DesignTimeVisible;
            this.UpdatedRowSource = source.UpdatedRowSource;
            this.FetchGeneratedResults = source.FetchGeneratedResults;
            foreach (UtlParameter parameter in source._parameterCollection)
            {
                this.Parameters.Add((UtlParameter) parameter.Clone());
            }
        }

        public UtlCommand(UtlConnection connection) : this(null, connection, null)
        {
        }

        public UtlCommand(string commandText) : this(commandText, null, null)
        {
        }

        public UtlCommand(string commandText, UtlConnection connection) : this(commandText, connection, null)
        {
        }

        public UtlCommand(string commandText, UtlConnection connection, UtlTransaction transaction)
        {
            this._commandTimeout = 30;
            this._commandType = System.Data.CommandType.Text;
            this._parameterCollection = new UtlParameterCollection(this);
            this._designTimeVisible = true;
            this._updateRowSource = UpdateRowSource.None;
            if (commandText != null)
            {
                this.CommandText = commandText;
            }
            if (connection != null)
            {
                this.DbConnection = connection;
            }
            if (transaction != null)
            {
                this.Transaction = transaction;
            }
            this.FetchGeneratedResults = false;
        }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        public void ClearCommands()
        {
            if (this._activeReader != null)
            {
                UtlDataReader target = null;
                try
                {
                    target = this._activeReader.Target as UtlDataReader;
                }
                catch (InvalidOperationException)
                {
                }
                if (target != null)
                {
                    target.Close();
                }
                this._activeReader = null;
            }
            if (this.Statement != null)
            {
                this.Statement.Dispose();
            }
            this.Statement = null;
        }

        public void ClearDataReader()
        {
            this._activeReader = null;
        }

        public object Clone()
        {
            return new UtlCommand(this);
        }

        protected override DbParameter CreateDbParameter()
        {
            return this.CreateParameter();
        }

        public UtlParameter CreateParameter()
        {
            return new UtlParameter();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                UtlDataReader target = null;
                if (this._activeReader != null)
                {
                    try
                    {
                        target = this._activeReader.Target as UtlDataReader;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
                if (target != null)
                {
                    target.DisposeCommand = true;
                    this._activeReader = null;
                }
                else
                {
                    this.Connection = null;
                }
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return this.ExecuteReader(behavior);
        }

        public override int ExecuteNonQuery()
        {
            using (UtlDataReader reader = this.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
            {
                while (reader.NextResult())
                {
                }
                return reader.RecordsAffected;
            }
        }

        public UtlDataReader ExecuteReader()
        {
            return this.ExecuteReader(CommandBehavior.Default);
        }

        public UtlDataReader ExecuteReader(CommandBehavior behavior)
        {
            this.InitializeForReader();
            UtlDataReader target = new UtlDataReader(this, behavior);
            this._activeReader = new WeakReference(target, false);
            return target;
        }

        public override object ExecuteScalar()
        {
            this.InitializeForReader();
            using (UtlDataReader reader = this.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
            {
                if (reader.Read())
                {
                    return reader[0];
                }
            }
            return null;
        }

        private void InitializeForReader()
        {
            if ((this._activeReader != null) && this._activeReader.IsAlive)
            {
                this.ClearDataReader();
            }
            if (this._cnn == null)
            {
                throw new InvalidOperationException("No connection associated with this command");
            }
            if (this._cnn.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Database is not open");
            }
            if (this._cnn.Version != this._version)
            {
                this._version = this._cnn.Version;
                this.ClearCommands();
            }
        }

        public override void Prepare()
        {
            if (this.Statement != null)
            {
                this.Statement.Dispose();
            }
            this.Statement = new UtlStatement(this, this._commandText, true);
        }

        public void SetWarnings(List<UtlException> warnings)
        {
            this._warnings = warnings;
        }

        public List<UtlException> Warnings
        {
            get
            {
                return this._warnings;
            }
        }

        [DefaultValue(""), Editor("Microsoft.VSDesigner.Data.SQL.Design.SqlCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), RefreshProperties(RefreshProperties.All)]
        public override string CommandText
        {
            get
            {
                return this._commandText;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (this._commandText != value)
                {
                    if ((this._activeReader != null) && this._activeReader.IsAlive)
                    {
                        throw new InvalidOperationException("Cannot set CommandText while a DataReader is active");
                    }
                    this.ClearCommands();
                    this._commandText = value;
                    this.Statement = null;
                }
            }
        }

        [DefaultValue(30)]
        public override int CommandTimeout
        {
            get
            {
                return this._commandTimeout;
            }
            set
            {
                this._commandTimeout = value;
            }
        }

        [DefaultValue(1), RefreshProperties(RefreshProperties.All)]
        public override System.Data.CommandType CommandType
        {
            get
            {
                return this._commandType;
            }
            set
            {
                if ((value != System.Data.CommandType.Text) && (value != System.Data.CommandType.StoredProcedure))
                {
                    throw new NotSupportedException();
                }
                this._commandType = value;
            }
        }

        [DefaultValue((string) null), Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public UtlConnection Connection
        {
            get
            {
                return this._cnn;
            }
            set
            {
                if ((this._activeReader != null) && this._activeReader.IsAlive)
                {
                    throw new InvalidOperationException("Cannot set Connection while a DataReader is active");
                }
                if (this._cnn != null)
                {
                    this.ClearCommands();
                }
                this._cnn = value;
                if (this._cnn != null)
                {
                    this._version = this._cnn.Version;
                }
            }
        }

        protected override System.Data.Common.DbConnection DbConnection
        {
            get
            {
                return this.Connection;
            }
            set
            {
                this.Connection = (UtlConnection) value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UtlParameterCollection Parameters
        {
            get
            {
                return this._parameterCollection;
            }
        }

        protected override System.Data.Common.DbParameterCollection DbParameterCollection
        {
            get
            {
                return this.Parameters;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UtlTransaction Transaction
        {
            get
            {
                return this._transaction;
            }
            set
            {
                if (this._cnn == null)
                {
                    if (value != null)
                    {
                        this.Connection = value.Connection;
                    }
                    this._transaction = value;
                }
                else
                {
                    if ((this._activeReader != null) && this._activeReader.IsAlive)
                    {
                        throw new InvalidOperationException("Cannot set Transaction while a DataReader is active");
                    }
                    if ((value != null) && (value.Conn != this._cnn))
                    {
                        throw new ArgumentException("Transaction is not associated with the command's connection");
                    }
                    this._transaction = value;
                }
            }
        }

        protected override System.Data.Common.DbTransaction DbTransaction
        {
            get
            {
                return this.Transaction;
            }
            set
            {
                this.Transaction = (UtlTransaction) value;
            }
        }

        [DefaultValue(0)]
        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this._updateRowSource;
            }
            set
            {
                this._updateRowSource = value;
            }
        }

        [Browsable(false), DefaultValue(true), DesignOnly(true), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool DesignTimeVisible
        {
            get
            {
                return this._designTimeVisible;
            }
            set
            {
                this._designTimeVisible = value;
            }
        }
    }
}

