namespace FwNs.DbRootsVII0004DataSetTableAdapters
{
    using FwNs;
    using FwNs.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;

    [DesignerCategory("code"), ToolboxItem(true), DataObject(true), Designer("Microsoft.VSDesigner.DataSource.Design.TableAdapterDesigner, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), HelpKeyword("vs.data.TableAdapter")]
    public class TblIdsTableAdapter : Component
    {
        private SqlDataAdapter _adapter;
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private SqlCommand[] _commandCollection;
        private bool _clearBeforeFill;

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public TblIdsTableAdapter()
        {
            this.ClearBeforeFill = true;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Fill, true)]
        public virtual int Fill(DbRootsVII0004DataSet.TblIdsDataTable dataTable)
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            if (this.ClearBeforeFill)
            {
                dataTable.Clear();
            }
            return this.Adapter.Fill(dataTable);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Select, true)]
        public virtual DbRootsVII0004DataSet.TblIdsDataTable GetData()
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            DbRootsVII0004DataSet.TblIdsDataTable dataTable = new DbRootsVII0004DataSet.TblIdsDataTable();
            this.Adapter.Fill(dataTable);
            return dataTable;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitAdapter()
        {
            this._adapter = new SqlDataAdapter();
            DataTableMapping mapping = new DataTableMapping {
                SourceTable = "Table",
                DataSetTable = "TblIds",
                ColumnMappings = { 
                    { 
                        "Id",
                        "Id"
                    },
                    { 
                        "IdR",
                        "IdR"
                    },
                    { 
                        "Nm",
                        "Nm"
                    }
                }
            };
            this._adapter.TableMappings.Add(mapping);
            this._adapter.InsertCommand = new SqlCommand();
            this._adapter.InsertCommand.Connection = this.Connection;
            this._adapter.InsertCommand.CommandText = "INSERT INTO [dbo].[TblIds] ([Id], [IdR], [Nm]) VALUES (@Id, @IdR, @Nm)";
            this._adapter.InsertCommand.CommandType = CommandType.Text;
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IdR", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdR", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Nm", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Nm", DataRowVersion.Current, false, null, "", "", ""));
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitCommandCollection()
        {
            this._commandCollection = new SqlCommand[] { new SqlCommand() };
            this._commandCollection[0].Connection = this.Connection;
            this._commandCollection[0].CommandText = "SELECT Id, IdR, Nm FROM dbo.TblIds";
            this._commandCollection[0].CommandType = CommandType.Text;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitConnection()
        {
            this._connection = new SqlConnection();
            this._connection.ConnectionString = Settings.Default.DbRootsVII0004ConnectionString;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Insert, true)]
        public virtual int Insert(int? Id, int? IdR, string Nm)
        {
            int num;
            if (Id.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[0].Value = Id.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[0].Value = DBNull.Value;
            }
            if (IdR.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[1].Value = IdR.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[1].Value = DBNull.Value;
            }
            if (Nm == null)
            {
                this.Adapter.InsertCommand.Parameters[2].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[2].Value = Nm;
            }
            ConnectionState state = this.Adapter.InsertCommand.Connection.State;
            if ((this.Adapter.InsertCommand.Connection.State & ConnectionState.Open) != ConnectionState.Open)
            {
                this.Adapter.InsertCommand.Connection.Open();
            }
            try
            {
                num = this.Adapter.InsertCommand.ExecuteNonQuery();
            }
            finally
            {
                if (state == ConnectionState.Closed)
                {
                    this.Adapter.InsertCommand.Connection.Close();
                }
            }
            return num;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter")]
        public virtual int Update(DbRootsVII0004DataSet dataSet)
        {
            return this.Adapter.Update(dataSet, "TblIds");
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter")]
        public virtual int Update(DbRootsVII0004DataSet.TblIdsDataTable dataTable)
        {
            return this.Adapter.Update(dataTable);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter")]
        public virtual int Update(DataRow dataRow)
        {
            DataRow[] dataRows = new DataRow[] { dataRow };
            return this.Adapter.Update(dataRows);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter")]
        public virtual int Update(DataRow[] dataRows)
        {
            return this.Adapter.Update(dataRows);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected internal SqlDataAdapter Adapter
        {
            get
            {
                if (this._adapter == null)
                {
                    this.InitAdapter();
                }
                return this._adapter;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        internal SqlConnection Connection
        {
            get
            {
                if (this._connection == null)
                {
                    this.InitConnection();
                }
                return this._connection;
            }
            set
            {
                this._connection = value;
                if (this.Adapter.InsertCommand != null)
                {
                    this.Adapter.InsertCommand.Connection = value;
                }
                if (this.Adapter.DeleteCommand != null)
                {
                    this.Adapter.DeleteCommand.Connection = value;
                }
                if (this.Adapter.UpdateCommand != null)
                {
                    this.Adapter.UpdateCommand.Connection = value;
                }
                for (int i = 0; i < this.CommandCollection.Length; i++)
                {
                    if (this.CommandCollection[i] != null)
                    {
                        this.CommandCollection[i].Connection = value;
                    }
                }
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        internal SqlTransaction Transaction
        {
            get
            {
                return this._transaction;
            }
            set
            {
                this._transaction = value;
                for (int i = 0; i < this.CommandCollection.Length; i++)
                {
                    this.CommandCollection[i].Transaction = this._transaction;
                }
                if ((this.Adapter != null) && (this.Adapter.DeleteCommand != null))
                {
                    this.Adapter.DeleteCommand.Transaction = this._transaction;
                }
                if ((this.Adapter != null) && (this.Adapter.InsertCommand != null))
                {
                    this.Adapter.InsertCommand.Transaction = this._transaction;
                }
                if ((this.Adapter != null) && (this.Adapter.UpdateCommand != null))
                {
                    this.Adapter.UpdateCommand.Transaction = this._transaction;
                }
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected SqlCommand[] CommandCollection
        {
            get
            {
                if (this._commandCollection == null)
                {
                    this.InitCommandCollection();
                }
                return this._commandCollection;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public bool ClearBeforeFill
        {
            get
            {
                return this._clearBeforeFill;
            }
            set
            {
                this._clearBeforeFill = value;
            }
        }
    }
}

