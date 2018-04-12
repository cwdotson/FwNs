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
    public class CTypsTableAdapter : Component
    {
        private SqlDataAdapter _adapter;
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private SqlCommand[] _commandCollection;
        private bool _clearBeforeFill;

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public CTypsTableAdapter()
        {
            this.ClearBeforeFill = true;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Delete, true)]
        public virtual int Delete(int Original_Id, int? Original_IdR, string Original_Nm, string Original_Fldr)
        {
            int num;
            this.Adapter.DeleteCommand.Parameters[0].Value = Original_Id;
            if (Original_IdR.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[1].Value = 0;
                this.Adapter.DeleteCommand.Parameters[2].Value = Original_IdR.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[1].Value = 1;
                this.Adapter.DeleteCommand.Parameters[2].Value = DBNull.Value;
            }
            if (Original_Nm == null)
            {
                this.Adapter.DeleteCommand.Parameters[3].Value = 1;
                this.Adapter.DeleteCommand.Parameters[4].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[3].Value = 0;
                this.Adapter.DeleteCommand.Parameters[4].Value = Original_Nm;
            }
            if (Original_Fldr == null)
            {
                this.Adapter.DeleteCommand.Parameters[5].Value = 1;
                this.Adapter.DeleteCommand.Parameters[6].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[5].Value = 0;
                this.Adapter.DeleteCommand.Parameters[6].Value = Original_Fldr;
            }
            ConnectionState state = this.Adapter.DeleteCommand.Connection.State;
            if ((this.Adapter.DeleteCommand.Connection.State & ConnectionState.Open) != ConnectionState.Open)
            {
                this.Adapter.DeleteCommand.Connection.Open();
            }
            try
            {
                num = this.Adapter.DeleteCommand.ExecuteNonQuery();
            }
            finally
            {
                if (state == ConnectionState.Closed)
                {
                    this.Adapter.DeleteCommand.Connection.Close();
                }
            }
            return num;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Fill, true)]
        public virtual int Fill(DbRootsVII0004DataSet.CTypsDataTable dataTable)
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            if (this.ClearBeforeFill)
            {
                dataTable.Clear();
            }
            return this.Adapter.Fill(dataTable);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Select, true)]
        public virtual DbRootsVII0004DataSet.CTypsDataTable GetData()
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            DbRootsVII0004DataSet.CTypsDataTable dataTable = new DbRootsVII0004DataSet.CTypsDataTable();
            this.Adapter.Fill(dataTable);
            return dataTable;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitAdapter()
        {
            this._adapter = new SqlDataAdapter();
            DataTableMapping mapping = new DataTableMapping {
                SourceTable = "Table",
                DataSetTable = "CTyps",
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
                    },
                    { 
                        "Fldr",
                        "Fldr"
                    }
                }
            };
            this._adapter.TableMappings.Add(mapping);
            this._adapter.DeleteCommand = new SqlCommand();
            this._adapter.DeleteCommand.Connection = this.Connection;
            this._adapter.DeleteCommand.CommandText = "DELETE FROM [dbo].[CTyps] WHERE (([Id] = @Original_Id) AND ((@IsNull_IdR = 1 AND [IdR] IS NULL) OR ([IdR] = @Original_IdR)) AND ((@IsNull_Nm = 1 AND [Nm] IS NULL) OR ([Nm] = @Original_Nm)) AND ((@IsNull_Fldr = 1 AND [Fldr] IS NULL) OR ([Fldr] = @Original_Fldr)))";
            this._adapter.DeleteCommand.CommandType = CommandType.Text;
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IdR", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdR", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IdR", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdR", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Nm", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Nm", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Nm", SqlDbType.NVarChar, 0, ParameterDirection.Input, 0, 0, "Nm", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Fldr", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Fldr", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Fldr", SqlDbType.NVarChar, 0, ParameterDirection.Input, 0, 0, "Fldr", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.InsertCommand = new SqlCommand();
            this._adapter.InsertCommand.Connection = this.Connection;
            this._adapter.InsertCommand.CommandText = "INSERT INTO [dbo].[CTyps] ([Id], [IdR], [Nm], [Fldr]) VALUES (@Id, @IdR, @Nm, @Fldr);\r\nSELECT Id, IdR, Nm, Fldr FROM CTyps WHERE (Id = @Id)";
            this._adapter.InsertCommand.CommandType = CommandType.Text;
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IdR", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdR", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Nm", SqlDbType.NVarChar, 0, ParameterDirection.Input, 0, 0, "Nm", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Fldr", SqlDbType.NVarChar, 0, ParameterDirection.Input, 0, 0, "Fldr", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand = new SqlCommand();
            this._adapter.UpdateCommand.Connection = this.Connection;
            this._adapter.UpdateCommand.CommandText = "UPDATE [dbo].[CTyps] SET [Id] = @Id, [IdR] = @IdR, [Nm] = @Nm, [Fldr] = @Fldr WHERE (([Id] = @Original_Id) AND ((@IsNull_IdR = 1 AND [IdR] IS NULL) OR ([IdR] = @Original_IdR)) AND ((@IsNull_Nm = 1 AND [Nm] IS NULL) OR ([Nm] = @Original_Nm)) AND ((@IsNull_Fldr = 1 AND [Fldr] IS NULL) OR ([Fldr] = @Original_Fldr)));\r\nSELECT Id, IdR, Nm, Fldr FROM CTyps WHERE (Id = @Id)";
            this._adapter.UpdateCommand.CommandType = CommandType.Text;
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IdR", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdR", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Nm", SqlDbType.NVarChar, 0, ParameterDirection.Input, 0, 0, "Nm", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Fldr", SqlDbType.NVarChar, 0, ParameterDirection.Input, 0, 0, "Fldr", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IdR", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdR", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IdR", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdR", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Nm", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Nm", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Nm", SqlDbType.NVarChar, 0, ParameterDirection.Input, 0, 0, "Nm", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Fldr", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Fldr", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Fldr", SqlDbType.NVarChar, 0, ParameterDirection.Input, 0, 0, "Fldr", DataRowVersion.Original, false, null, "", "", ""));
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitCommandCollection()
        {
            this._commandCollection = new SqlCommand[] { new SqlCommand() };
            this._commandCollection[0].Connection = this.Connection;
            this._commandCollection[0].CommandText = "SELECT Id, IdR, Nm, Fldr FROM dbo.CTyps";
            this._commandCollection[0].CommandType = CommandType.Text;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitConnection()
        {
            this._connection = new SqlConnection();
            this._connection.ConnectionString = Settings.Default.DbRootsVII0004ConnectionString;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Insert, true)]
        public virtual int Insert(int Id, int? IdR, string Nm, string Fldr)
        {
            int num;
            this.Adapter.InsertCommand.Parameters[0].Value = Id;
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
            if (Fldr == null)
            {
                this.Adapter.InsertCommand.Parameters[3].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[3].Value = Fldr;
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
            return this.Adapter.Update(dataSet, "CTyps");
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter")]
        public virtual int Update(DbRootsVII0004DataSet.CTypsDataTable dataTable)
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

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Update, true)]
        public virtual int Update(int? IdR, string Nm, string Fldr, int Original_Id, int? Original_IdR, string Original_Nm, string Original_Fldr)
        {
            return this.Update(Original_Id, IdR, Nm, Fldr, Original_Id, Original_IdR, Original_Nm, Original_Fldr);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Update, true)]
        public virtual int Update(int Id, int? IdR, string Nm, string Fldr, int Original_Id, int? Original_IdR, string Original_Nm, string Original_Fldr)
        {
            int num;
            this.Adapter.UpdateCommand.Parameters[0].Value = Id;
            if (IdR.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[1].Value = IdR.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[1].Value = DBNull.Value;
            }
            if (Nm == null)
            {
                this.Adapter.UpdateCommand.Parameters[2].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[2].Value = Nm;
            }
            if (Fldr == null)
            {
                this.Adapter.UpdateCommand.Parameters[3].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[3].Value = Fldr;
            }
            this.Adapter.UpdateCommand.Parameters[4].Value = Original_Id;
            if (Original_IdR.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[5].Value = 0;
                this.Adapter.UpdateCommand.Parameters[6].Value = Original_IdR.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[5].Value = 1;
                this.Adapter.UpdateCommand.Parameters[6].Value = DBNull.Value;
            }
            if (Original_Nm == null)
            {
                this.Adapter.UpdateCommand.Parameters[7].Value = 1;
                this.Adapter.UpdateCommand.Parameters[8].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[7].Value = 0;
                this.Adapter.UpdateCommand.Parameters[8].Value = Original_Nm;
            }
            if (Original_Fldr == null)
            {
                this.Adapter.UpdateCommand.Parameters[9].Value = 1;
                this.Adapter.UpdateCommand.Parameters[10].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[9].Value = 0;
                this.Adapter.UpdateCommand.Parameters[10].Value = Original_Fldr;
            }
            ConnectionState state = this.Adapter.UpdateCommand.Connection.State;
            if ((this.Adapter.UpdateCommand.Connection.State & ConnectionState.Open) != ConnectionState.Open)
            {
                this.Adapter.UpdateCommand.Connection.Open();
            }
            try
            {
                num = this.Adapter.UpdateCommand.ExecuteNonQuery();
            }
            finally
            {
                if (state == ConnectionState.Closed)
                {
                    this.Adapter.UpdateCommand.Connection.Close();
                }
            }
            return num;
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

