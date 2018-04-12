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
    public class ItmsTableAdapter : Component
    {
        private SqlDataAdapter _adapter;
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private SqlCommand[] _commandCollection;
        private bool _clearBeforeFill;

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public ItmsTableAdapter()
        {
            this.ClearBeforeFill = true;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Delete, true)]
        public virtual int Delete(int Original_Id, int? Original_IdC, int? Original_IdW, int? Original_IdX, DateTime? Original_Cr8D8, DateTime? Original_ChkD8)
        {
            int num;
            this.Adapter.DeleteCommand.Parameters[0].Value = Original_Id;
            if (Original_IdC.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[1].Value = 0;
                this.Adapter.DeleteCommand.Parameters[2].Value = Original_IdC.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[1].Value = 1;
                this.Adapter.DeleteCommand.Parameters[2].Value = DBNull.Value;
            }
            if (Original_IdW.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[3].Value = 0;
                this.Adapter.DeleteCommand.Parameters[4].Value = Original_IdW.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[3].Value = 1;
                this.Adapter.DeleteCommand.Parameters[4].Value = DBNull.Value;
            }
            if (Original_IdX.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[5].Value = 0;
                this.Adapter.DeleteCommand.Parameters[6].Value = Original_IdX.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[5].Value = 1;
                this.Adapter.DeleteCommand.Parameters[6].Value = DBNull.Value;
            }
            if (Original_Cr8D8.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[7].Value = 0;
                this.Adapter.DeleteCommand.Parameters[8].Value = Original_Cr8D8.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[7].Value = 1;
                this.Adapter.DeleteCommand.Parameters[8].Value = DBNull.Value;
            }
            if (Original_ChkD8.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[9].Value = 0;
                this.Adapter.DeleteCommand.Parameters[10].Value = Original_ChkD8.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[9].Value = 1;
                this.Adapter.DeleteCommand.Parameters[10].Value = DBNull.Value;
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
        public virtual int Fill(DbRootsVII0004DataSet.ItmsDataTable dataTable)
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            if (this.ClearBeforeFill)
            {
                dataTable.Clear();
            }
            return this.Adapter.Fill(dataTable);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Select, true)]
        public virtual DbRootsVII0004DataSet.ItmsDataTable GetData()
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            DbRootsVII0004DataSet.ItmsDataTable dataTable = new DbRootsVII0004DataSet.ItmsDataTable();
            this.Adapter.Fill(dataTable);
            return dataTable;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitAdapter()
        {
            this._adapter = new SqlDataAdapter();
            DataTableMapping mapping = new DataTableMapping {
                SourceTable = "Table",
                DataSetTable = "Itms",
                ColumnMappings = { 
                    { 
                        "Id",
                        "Id"
                    },
                    { 
                        "IdC",
                        "IdC"
                    },
                    { 
                        "IdW",
                        "IdW"
                    },
                    { 
                        "IdX",
                        "IdX"
                    },
                    { 
                        "Cr8D8",
                        "Cr8D8"
                    },
                    { 
                        "ChkD8",
                        "ChkD8"
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
            this._adapter.DeleteCommand.CommandText = "DELETE FROM [dbo].[Itms] WHERE (([Id] = @Original_Id) AND ((@IsNull_IdC = 1 AND [IdC] IS NULL) OR ([IdC] = @Original_IdC)) AND ((@IsNull_IdW = 1 AND [IdW] IS NULL) OR ([IdW] = @Original_IdW)) AND ((@IsNull_IdX = 1 AND [IdX] IS NULL) OR ([IdX] = @Original_IdX)) AND ((@IsNull_Cr8D8 = 1 AND [Cr8D8] IS NULL) OR ([Cr8D8] = @Original_Cr8D8)) AND ((@IsNull_ChkD8 = 1 AND [ChkD8] IS NULL) OR ([ChkD8] = @Original_ChkD8)))";
            this._adapter.DeleteCommand.CommandType = CommandType.Text;
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IdC", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdC", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IdC", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdC", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IdW", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdW", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IdW", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdW", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IdX", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdX", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IdX", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdX", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Cr8D8", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Cr8D8", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Cr8D8", SqlDbType.DateTime, 0, ParameterDirection.Input, 0, 0, "Cr8D8", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_ChkD8", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "ChkD8", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_ChkD8", SqlDbType.DateTime, 0, ParameterDirection.Input, 0, 0, "ChkD8", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.InsertCommand = new SqlCommand();
            this._adapter.InsertCommand.Connection = this.Connection;
            this._adapter.InsertCommand.CommandText = "INSERT INTO [dbo].[Itms] ([Id], [IdC], [IdW], [IdX], [Cr8D8], [ChkD8], [Nfo]) VALUES (@Id, @IdC, @IdW, @IdX, @Cr8D8, @ChkD8, @Nfo);\r\nSELECT Id, IdC, IdW, IdX, Cr8D8, ChkD8, Nfo FROM Itms WHERE (Id = @Id)";
            this._adapter.InsertCommand.CommandType = CommandType.Text;
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IdC", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdC", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IdW", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdW", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IdX", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdX", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Cr8D8", SqlDbType.DateTime, 0, ParameterDirection.Input, 0, 0, "Cr8D8", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@ChkD8", SqlDbType.DateTime, 0, ParameterDirection.Input, 0, 0, "ChkD8", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Nfo", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Nfo", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand = new SqlCommand();
            this._adapter.UpdateCommand.Connection = this.Connection;
            this._adapter.UpdateCommand.CommandText = "UPDATE [dbo].[Itms] SET [Id] = @Id, [IdC] = @IdC, [IdW] = @IdW, [IdX] = @IdX, [Cr8D8] = @Cr8D8, [ChkD8] = @ChkD8, [Nfo] = @Nfo WHERE (([Id] = @Original_Id) AND ((@IsNull_IdC = 1 AND [IdC] IS NULL) OR ([IdC] = @Original_IdC)) AND ((@IsNull_IdW = 1 AND [IdW] IS NULL) OR ([IdW] = @Original_IdW)) AND ((@IsNull_IdX = 1 AND [IdX] IS NULL) OR ([IdX] = @Original_IdX)) AND ((@IsNull_Cr8D8 = 1 AND [Cr8D8] IS NULL) OR ([Cr8D8] = @Original_Cr8D8)) AND ((@IsNull_ChkD8 = 1 AND [ChkD8] IS NULL) OR ([ChkD8] = @Original_ChkD8)));\r\nSELECT Id, IdC, IdW, IdX, Cr8D8, ChkD8, Nfo FROM Itms WHERE (Id = @Id)";
            this._adapter.UpdateCommand.CommandType = CommandType.Text;
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IdC", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdC", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IdW", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdW", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IdX", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdX", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Cr8D8", SqlDbType.DateTime, 0, ParameterDirection.Input, 0, 0, "Cr8D8", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@ChkD8", SqlDbType.DateTime, 0, ParameterDirection.Input, 0, 0, "ChkD8", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Nfo", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Nfo", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IdC", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdC", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IdC", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdC", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IdW", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdW", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IdW", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdW", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IdX", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdX", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IdX", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IdX", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Cr8D8", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Cr8D8", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Cr8D8", SqlDbType.DateTime, 0, ParameterDirection.Input, 0, 0, "Cr8D8", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_ChkD8", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "ChkD8", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_ChkD8", SqlDbType.DateTime, 0, ParameterDirection.Input, 0, 0, "ChkD8", DataRowVersion.Original, false, null, "", "", ""));
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitCommandCollection()
        {
            this._commandCollection = new SqlCommand[] { new SqlCommand() };
            this._commandCollection[0].Connection = this.Connection;
            this._commandCollection[0].CommandText = "SELECT Id, IdC, IdW, IdX, Cr8D8, ChkD8, Nfo FROM dbo.Itms";
            this._commandCollection[0].CommandType = CommandType.Text;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitConnection()
        {
            this._connection = new SqlConnection();
            this._connection.ConnectionString = Settings.Default.DbRootsVII0004ConnectionString;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Insert, true)]
        public virtual int Insert(int Id, int? IdC, int? IdW, int? IdX, DateTime? Cr8D8, DateTime? ChkD8, string Nfo)
        {
            int num;
            this.Adapter.InsertCommand.Parameters[0].Value = Id;
            if (IdC.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[1].Value = IdC.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[1].Value = DBNull.Value;
            }
            if (IdW.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[2].Value = IdW.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[2].Value = DBNull.Value;
            }
            if (IdX.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[3].Value = IdX.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[3].Value = DBNull.Value;
            }
            if (Cr8D8.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[4].Value = Cr8D8.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[4].Value = DBNull.Value;
            }
            if (ChkD8.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[5].Value = ChkD8.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[5].Value = DBNull.Value;
            }
            if (Nfo == null)
            {
                this.Adapter.InsertCommand.Parameters[6].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[6].Value = Nfo;
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
            return this.Adapter.Update(dataSet, "Itms");
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter")]
        public virtual int Update(DbRootsVII0004DataSet.ItmsDataTable dataTable)
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
        public virtual int Update(int? IdC, int? IdW, int? IdX, DateTime? Cr8D8, DateTime? ChkD8, string Nfo, int Original_Id, int? Original_IdC, int? Original_IdW, int? Original_IdX, DateTime? Original_Cr8D8, DateTime? Original_ChkD8)
        {
            return this.Update(Original_Id, IdC, IdW, IdX, Cr8D8, ChkD8, Nfo, Original_Id, Original_IdC, Original_IdW, Original_IdX, Original_Cr8D8, Original_ChkD8);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Update, true)]
        public virtual int Update(int Id, int? IdC, int? IdW, int? IdX, DateTime? Cr8D8, DateTime? ChkD8, string Nfo, int Original_Id, int? Original_IdC, int? Original_IdW, int? Original_IdX, DateTime? Original_Cr8D8, DateTime? Original_ChkD8)
        {
            int num;
            this.Adapter.UpdateCommand.Parameters[0].Value = Id;
            if (IdC.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[1].Value = IdC.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[1].Value = DBNull.Value;
            }
            if (IdW.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[2].Value = IdW.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[2].Value = DBNull.Value;
            }
            if (IdX.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[3].Value = IdX.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[3].Value = DBNull.Value;
            }
            if (Cr8D8.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[4].Value = Cr8D8.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[4].Value = DBNull.Value;
            }
            if (ChkD8.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[5].Value = ChkD8.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[5].Value = DBNull.Value;
            }
            if (Nfo == null)
            {
                this.Adapter.UpdateCommand.Parameters[6].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[6].Value = Nfo;
            }
            this.Adapter.UpdateCommand.Parameters[7].Value = Original_Id;
            if (Original_IdC.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[8].Value = 0;
                this.Adapter.UpdateCommand.Parameters[9].Value = Original_IdC.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[8].Value = 1;
                this.Adapter.UpdateCommand.Parameters[9].Value = DBNull.Value;
            }
            if (Original_IdW.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[10].Value = 0;
                this.Adapter.UpdateCommand.Parameters[11].Value = Original_IdW.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[10].Value = 1;
                this.Adapter.UpdateCommand.Parameters[11].Value = DBNull.Value;
            }
            if (Original_IdX.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[12].Value = 0;
                this.Adapter.UpdateCommand.Parameters[13].Value = Original_IdX.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[12].Value = 1;
                this.Adapter.UpdateCommand.Parameters[13].Value = DBNull.Value;
            }
            if (Original_Cr8D8.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[14].Value = 0;
                this.Adapter.UpdateCommand.Parameters[15].Value = Original_Cr8D8.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[14].Value = 1;
                this.Adapter.UpdateCommand.Parameters[15].Value = DBNull.Value;
            }
            if (Original_ChkD8.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x10].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x11].Value = Original_ChkD8.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x10].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x11].Value = DBNull.Value;
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

