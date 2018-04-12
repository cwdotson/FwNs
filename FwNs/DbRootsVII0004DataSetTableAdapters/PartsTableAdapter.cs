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
    public class PartsTableAdapter : Component
    {
        private SqlDataAdapter _adapter;
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private SqlCommand[] _commandCollection;
        private bool _clearBeforeFill;

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public PartsTableAdapter()
        {
            this.ClearBeforeFill = true;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Delete, true)]
        public virtual int Delete(int Original_Id, int? Original_Idp, int? Original_Typ, int? Original_Ndx)
        {
            int num;
            this.Adapter.DeleteCommand.Parameters[0].Value = Original_Id;
            if (Original_Idp.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[1].Value = 0;
                this.Adapter.DeleteCommand.Parameters[2].Value = Original_Idp.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[1].Value = 1;
                this.Adapter.DeleteCommand.Parameters[2].Value = DBNull.Value;
            }
            if (Original_Typ.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[3].Value = 0;
                this.Adapter.DeleteCommand.Parameters[4].Value = Original_Typ.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[3].Value = 1;
                this.Adapter.DeleteCommand.Parameters[4].Value = DBNull.Value;
            }
            if (Original_Ndx.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[5].Value = 0;
                this.Adapter.DeleteCommand.Parameters[6].Value = Original_Ndx.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[5].Value = 1;
                this.Adapter.DeleteCommand.Parameters[6].Value = DBNull.Value;
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
        public virtual int Fill(DbRootsVII0004DataSet.PartsDataTable dataTable)
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            if (this.ClearBeforeFill)
            {
                dataTable.Clear();
            }
            return this.Adapter.Fill(dataTable);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Select, true)]
        public virtual DbRootsVII0004DataSet.PartsDataTable GetData()
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            DbRootsVII0004DataSet.PartsDataTable dataTable = new DbRootsVII0004DataSet.PartsDataTable();
            this.Adapter.Fill(dataTable);
            return dataTable;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitAdapter()
        {
            this._adapter = new SqlDataAdapter();
            DataTableMapping mapping = new DataTableMapping {
                SourceTable = "Table",
                DataSetTable = "Parts",
                ColumnMappings = { 
                    { 
                        "Id",
                        "Id"
                    },
                    { 
                        "Idp",
                        "Idp"
                    },
                    { 
                        "Typ",
                        "Typ"
                    },
                    { 
                        "Ndx",
                        "Ndx"
                    },
                    { 
                        "Val",
                        "Val"
                    },
                    { 
                        "Nfo",
                        "Nfo"
                    }
                }
            };
            this._adapter.TableMappings.Add(mapping);
            this._adapter.DeleteCommand = new SqlCommand();
            this._adapter.DeleteCommand.Connection = this.Connection;
            this._adapter.DeleteCommand.CommandText = "DELETE FROM [dbo].[Parts] WHERE (([Id] = @Original_Id) AND ((@IsNull_Idp = 1 AND [Idp] IS NULL) OR ([Idp] = @Original_Idp)) AND ((@IsNull_Typ = 1 AND [Typ] IS NULL) OR ([Typ] = @Original_Typ)) AND ((@IsNull_Ndx = 1 AND [Ndx] IS NULL) OR ([Ndx] = @Original_Ndx)))";
            this._adapter.DeleteCommand.CommandType = CommandType.Text;
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Typ", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Typ", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Typ", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Typ", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Ndx", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ndx", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Ndx", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ndx", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.InsertCommand = new SqlCommand();
            this._adapter.InsertCommand.Connection = this.Connection;
            this._adapter.InsertCommand.CommandText = "INSERT INTO [dbo].[Parts] ([Id], [Idp], [Typ], [Ndx], [Val], [Nfo]) VALUES (@Id, @Idp, @Typ, @Ndx, @Val, @Nfo);\r\nSELECT Id, Idp, Typ, Ndx, Val, Nfo FROM Parts WHERE (Id = @Id)";
            this._adapter.InsertCommand.CommandType = CommandType.Text;
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Typ", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Typ", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Ndx", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ndx", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Val", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Val", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Nfo", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Nfo", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand = new SqlCommand();
            this._adapter.UpdateCommand.Connection = this.Connection;
            this._adapter.UpdateCommand.CommandText = "UPDATE [dbo].[Parts] SET [Id] = @Id, [Idp] = @Idp, [Typ] = @Typ, [Ndx] = @Ndx, [Val] = @Val, [Nfo] = @Nfo WHERE (([Id] = @Original_Id) AND ((@IsNull_Idp = 1 AND [Idp] IS NULL) OR ([Idp] = @Original_Idp)) AND ((@IsNull_Typ = 1 AND [Typ] IS NULL) OR ([Typ] = @Original_Typ)) AND ((@IsNull_Ndx = 1 AND [Ndx] IS NULL) OR ([Ndx] = @Original_Ndx)));\r\nSELECT Id, Idp, Typ, Ndx, Val, Nfo FROM Parts WHERE (Id = @Id)";
            this._adapter.UpdateCommand.CommandType = CommandType.Text;
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Typ", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Typ", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Ndx", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ndx", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Val", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Val", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Nfo", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Nfo", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Typ", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Typ", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Typ", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Typ", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Ndx", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ndx", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Ndx", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ndx", DataRowVersion.Original, false, null, "", "", ""));
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitCommandCollection()
        {
            this._commandCollection = new SqlCommand[] { new SqlCommand() };
            this._commandCollection[0].Connection = this.Connection;
            this._commandCollection[0].CommandText = "SELECT Id, Idp, Typ, Ndx, Val, Nfo FROM dbo.Parts";
            this._commandCollection[0].CommandType = CommandType.Text;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitConnection()
        {
            this._connection = new SqlConnection();
            this._connection.ConnectionString = Settings.Default.DbRootsVII0004ConnectionString;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Insert, true)]
        public virtual int Insert(int Id, int? Idp, int? Typ, int? Ndx, string Val, string Nfo)
        {
            int num;
            this.Adapter.InsertCommand.Parameters[0].Value = Id;
            if (Idp.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[1].Value = Idp.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[1].Value = DBNull.Value;
            }
            if (Typ.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[2].Value = Typ.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[2].Value = DBNull.Value;
            }
            if (Ndx.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[3].Value = Ndx.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[3].Value = DBNull.Value;
            }
            if (Val == null)
            {
                this.Adapter.InsertCommand.Parameters[4].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[4].Value = Val;
            }
            if (Nfo == null)
            {
                this.Adapter.InsertCommand.Parameters[5].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[5].Value = Nfo;
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
            return this.Adapter.Update(dataSet, "Parts");
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter")]
        public virtual int Update(DbRootsVII0004DataSet.PartsDataTable dataTable)
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
        public virtual int Update(int? Idp, int? Typ, int? Ndx, string Val, string Nfo, int Original_Id, int? Original_Idp, int? Original_Typ, int? Original_Ndx)
        {
            return this.Update(Original_Id, Idp, Typ, Ndx, Val, Nfo, Original_Id, Original_Idp, Original_Typ, Original_Ndx);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Update, true)]
        public virtual int Update(int Id, int? Idp, int? Typ, int? Ndx, string Val, string Nfo, int Original_Id, int? Original_Idp, int? Original_Typ, int? Original_Ndx)
        {
            int num;
            this.Adapter.UpdateCommand.Parameters[0].Value = Id;
            if (Idp.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[1].Value = Idp.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[1].Value = DBNull.Value;
            }
            if (Typ.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[2].Value = Typ.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[2].Value = DBNull.Value;
            }
            if (Ndx.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[3].Value = Ndx.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[3].Value = DBNull.Value;
            }
            if (Val == null)
            {
                this.Adapter.UpdateCommand.Parameters[4].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[4].Value = Val;
            }
            if (Nfo == null)
            {
                this.Adapter.UpdateCommand.Parameters[5].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[5].Value = Nfo;
            }
            this.Adapter.UpdateCommand.Parameters[6].Value = Original_Id;
            if (Original_Idp.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[7].Value = 0;
                this.Adapter.UpdateCommand.Parameters[8].Value = Original_Idp.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[7].Value = 1;
                this.Adapter.UpdateCommand.Parameters[8].Value = DBNull.Value;
            }
            if (Original_Typ.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[9].Value = 0;
                this.Adapter.UpdateCommand.Parameters[10].Value = Original_Typ.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[9].Value = 1;
                this.Adapter.UpdateCommand.Parameters[10].Value = DBNull.Value;
            }
            if (Original_Ndx.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[11].Value = 0;
                this.Adapter.UpdateCommand.Parameters[12].Value = Original_Ndx.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[11].Value = 1;
                this.Adapter.UpdateCommand.Parameters[12].Value = DBNull.Value;
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

