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
    public class FTypsTableAdapter : Component
    {
        private SqlDataAdapter _adapter;
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private SqlCommand[] _commandCollection;
        private bool _clearBeforeFill;

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public FTypsTableAdapter()
        {
            this.ClearBeforeFill = true;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Delete, true)]
        public virtual int Delete(int Original_Id, int? Original_Idp, short? Original_IsApp, short? Original_IsData, short? Original_IsDoc, short? Original_IsGeo, short? Original_IsCode, short? Original_IsGrp, short? Original_IsProj, short? Original_Ef0, short? Original_Ef1)
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
            if (Original_IsApp.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[3].Value = 0;
                this.Adapter.DeleteCommand.Parameters[4].Value = Original_IsApp.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[3].Value = 1;
                this.Adapter.DeleteCommand.Parameters[4].Value = DBNull.Value;
            }
            if (Original_IsData.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[5].Value = 0;
                this.Adapter.DeleteCommand.Parameters[6].Value = Original_IsData.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[5].Value = 1;
                this.Adapter.DeleteCommand.Parameters[6].Value = DBNull.Value;
            }
            if (Original_IsDoc.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[7].Value = 0;
                this.Adapter.DeleteCommand.Parameters[8].Value = Original_IsDoc.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[7].Value = 1;
                this.Adapter.DeleteCommand.Parameters[8].Value = DBNull.Value;
            }
            if (Original_IsGeo.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[9].Value = 0;
                this.Adapter.DeleteCommand.Parameters[10].Value = Original_IsGeo.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[9].Value = 1;
                this.Adapter.DeleteCommand.Parameters[10].Value = DBNull.Value;
            }
            if (Original_IsCode.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[11].Value = 0;
                this.Adapter.DeleteCommand.Parameters[12].Value = Original_IsCode.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[11].Value = 1;
                this.Adapter.DeleteCommand.Parameters[12].Value = DBNull.Value;
            }
            if (Original_IsGrp.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[13].Value = 0;
                this.Adapter.DeleteCommand.Parameters[14].Value = Original_IsGrp.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[13].Value = 1;
                this.Adapter.DeleteCommand.Parameters[14].Value = DBNull.Value;
            }
            if (Original_IsProj.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[15].Value = 0;
                this.Adapter.DeleteCommand.Parameters[0x10].Value = Original_IsProj.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[15].Value = 1;
                this.Adapter.DeleteCommand.Parameters[0x10].Value = DBNull.Value;
            }
            if (Original_Ef0.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[0x11].Value = 0;
                this.Adapter.DeleteCommand.Parameters[0x12].Value = Original_Ef0.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[0x11].Value = 1;
                this.Adapter.DeleteCommand.Parameters[0x12].Value = DBNull.Value;
            }
            if (Original_Ef1.HasValue)
            {
                this.Adapter.DeleteCommand.Parameters[0x13].Value = 0;
                this.Adapter.DeleteCommand.Parameters[20].Value = Original_Ef1.Value;
            }
            else
            {
                this.Adapter.DeleteCommand.Parameters[0x13].Value = 1;
                this.Adapter.DeleteCommand.Parameters[20].Value = DBNull.Value;
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
        public virtual int Fill(DbRootsVII0004DataSet.FTypsDataTable dataTable)
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            if (this.ClearBeforeFill)
            {
                dataTable.Clear();
            }
            return this.Adapter.Fill(dataTable);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Select, true)]
        public virtual DbRootsVII0004DataSet.FTypsDataTable GetData()
        {
            this.Adapter.SelectCommand = this.CommandCollection[0];
            DbRootsVII0004DataSet.FTypsDataTable dataTable = new DbRootsVII0004DataSet.FTypsDataTable();
            this.Adapter.Fill(dataTable);
            return dataTable;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitAdapter()
        {
            this._adapter = new SqlDataAdapter();
            DataTableMapping mapping = new DataTableMapping {
                SourceTable = "Table",
                DataSetTable = "FTyps",
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
                        "Extn",
                        "Extn"
                    },
                    { 
                        "Applctn",
                        "Applctn"
                    },
                    { 
                        "Dscrptn",
                        "Dscrptn"
                    },
                    { 
                        "IsApp",
                        "IsApp"
                    },
                    { 
                        "IsData",
                        "IsData"
                    },
                    { 
                        "IsDoc",
                        "IsDoc"
                    },
                    { 
                        "IsGeo",
                        "IsGeo"
                    },
                    { 
                        "IsCode",
                        "IsCode"
                    },
                    { 
                        "IsGrp",
                        "IsGrp"
                    },
                    { 
                        "IsProj",
                        "IsProj"
                    },
                    { 
                        "Ef0",
                        "Ef0"
                    },
                    { 
                        "Ef1",
                        "Ef1"
                    }
                }
            };
            this._adapter.TableMappings.Add(mapping);
            this._adapter.DeleteCommand = new SqlCommand();
            this._adapter.DeleteCommand.Connection = this.Connection;
            this._adapter.DeleteCommand.CommandText = "DELETE FROM [dbo].[FTyps] WHERE (([Id] = @Original_Id) AND ((@IsNull_Idp = 1 AND [Idp] IS NULL) OR ([Idp] = @Original_Idp)) AND ((@IsNull_IsApp = 1 AND [IsApp] IS NULL) OR ([IsApp] = @Original_IsApp)) AND ((@IsNull_IsData = 1 AND [IsData] IS NULL) OR ([IsData] = @Original_IsData)) AND ((@IsNull_IsDoc = 1 AND [IsDoc] IS NULL) OR ([IsDoc] = @Original_IsDoc)) AND ((@IsNull_IsGeo = 1 AND [IsGeo] IS NULL) OR ([IsGeo] = @Original_IsGeo)) AND ((@IsNull_IsCode = 1 AND [IsCode] IS NULL) OR ([IsCode] = @Original_IsCode)) AND ((@IsNull_IsGrp = 1 AND [IsGrp] IS NULL) OR ([IsGrp] = @Original_IsGrp)) AND ((@IsNull_IsProj = 1 AND [IsProj] IS NULL) OR ([IsProj] = @Original_IsProj)) AND ((@IsNull_Ef0 = 1 AND [Ef0] IS NULL) OR ([Ef0] = @Original_Ef0)) AND ((@IsNull_Ef1 = 1 AND [Ef1] IS NULL) OR ([Ef1] = @Original_Ef1)))";
            this._adapter.DeleteCommand.CommandType = CommandType.Text;
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IsApp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsApp", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IsApp", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsApp", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IsData", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsData", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IsData", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsData", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IsDoc", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsDoc", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IsDoc", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsDoc", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IsGeo", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsGeo", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IsGeo", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsGeo", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IsCode", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsCode", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IsCode", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsCode", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IsGrp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsGrp", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IsGrp", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsGrp", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_IsProj", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsProj", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_IsProj", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsProj", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Ef0", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ef0", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Ef0", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "Ef0", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@IsNull_Ef1", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ef1", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.DeleteCommand.Parameters.Add(new SqlParameter("@Original_Ef1", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "Ef1", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.InsertCommand = new SqlCommand();
            this._adapter.InsertCommand.Connection = this.Connection;
            this._adapter.InsertCommand.CommandText = "INSERT INTO [dbo].[FTyps] ([Id], [Idp], [Extn], [Applctn], [Dscrptn], [IsApp], [IsData], [IsDoc], [IsGeo], [IsCode], [IsGrp], [IsProj], [Ef0], [Ef1]) VALUES (@Id, @Idp, @Extn, @Applctn, @Dscrptn, @IsApp, @IsData, @IsDoc, @IsGeo, @IsCode, @IsGrp, @IsProj, @Ef0, @Ef1);\r\nSELECT Id, Idp, Extn, Applctn, Dscrptn, IsApp, IsData, IsDoc, IsGeo, IsCode, IsGrp, IsProj, Ef0, Ef1 FROM FTyps WHERE (Id = @Id)";
            this._adapter.InsertCommand.CommandType = CommandType.Text;
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Extn", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Extn", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Applctn", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Applctn", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Dscrptn", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Dscrptn", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IsApp", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsApp", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IsData", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsData", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IsDoc", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsDoc", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IsGeo", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsGeo", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IsCode", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsCode", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IsGrp", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsGrp", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@IsProj", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsProj", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Ef0", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "Ef0", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.InsertCommand.Parameters.Add(new SqlParameter("@Ef1", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "Ef1", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand = new SqlCommand();
            this._adapter.UpdateCommand.Connection = this.Connection;
            this._adapter.UpdateCommand.CommandText = "UPDATE [dbo].[FTyps] SET [Id] = @Id, [Idp] = @Idp, [Extn] = @Extn, [Applctn] = @Applctn, [Dscrptn] = @Dscrptn, [IsApp] = @IsApp, [IsData] = @IsData, [IsDoc] = @IsDoc, [IsGeo] = @IsGeo, [IsCode] = @IsCode, [IsGrp] = @IsGrp, [IsProj] = @IsProj, [Ef0] = @Ef0, [Ef1] = @Ef1 WHERE (([Id] = @Original_Id) AND ((@IsNull_Idp = 1 AND [Idp] IS NULL) OR ([Idp] = @Original_Idp)) AND ((@IsNull_IsApp = 1 AND [IsApp] IS NULL) OR ([IsApp] = @Original_IsApp)) AND ((@IsNull_IsData = 1 AND [IsData] IS NULL) OR ([IsData] = @Original_IsData)) AND ((@IsNull_IsDoc = 1 AND [IsDoc] IS NULL) OR ([IsDoc] = @Original_IsDoc)) AND ((@IsNull_IsGeo = 1 AND [IsGeo] IS NULL) OR ([IsGeo] = @Original_IsGeo)) AND ((@IsNull_IsCode = 1 AND [IsCode] IS NULL) OR ([IsCode] = @Original_IsCode)) AND ((@IsNull_IsGrp = 1 AND [IsGrp] IS NULL) OR ([IsGrp] = @Original_IsGrp)) AND ((@IsNull_IsProj = 1 AND [IsProj] IS NULL) OR ([IsProj] = @Original_IsProj)) AND ((@IsNull_Ef0 = 1 AND [Ef0] IS NULL) OR ([Ef0] = @Original_Ef0)) AND ((@IsNull_Ef1 = 1 AND [Ef1] IS NULL) OR ([Ef1] = @Original_Ef1)));\r\nSELECT Id, Idp, Extn, Applctn, Dscrptn, IsApp, IsData, IsDoc, IsGeo, IsCode, IsGrp, IsProj, Ef0, Ef1 FROM FTyps WHERE (Id = @Id)";
            this._adapter.UpdateCommand.CommandType = CommandType.Text;
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Extn", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Extn", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Applctn", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Applctn", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Dscrptn", SqlDbType.NText, 0, ParameterDirection.Input, 0, 0, "Dscrptn", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsApp", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsApp", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsData", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsData", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsDoc", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsDoc", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsGeo", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsGeo", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsCode", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsCode", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsGrp", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsGrp", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsProj", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsProj", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Ef0", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "Ef0", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Ef1", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "Ef1", DataRowVersion.Current, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Id", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Id", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Idp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Idp", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IsApp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsApp", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IsApp", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsApp", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IsData", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsData", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IsData", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsData", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IsDoc", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsDoc", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IsDoc", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsDoc", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IsGeo", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsGeo", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IsGeo", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsGeo", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IsCode", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsCode", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IsCode", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsCode", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IsGrp", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsGrp", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IsGrp", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsGrp", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_IsProj", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "IsProj", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_IsProj", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "IsProj", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Ef0", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ef0", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Ef0", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "Ef0", DataRowVersion.Original, false, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@IsNull_Ef1", SqlDbType.Int, 0, ParameterDirection.Input, 0, 0, "Ef1", DataRowVersion.Original, true, null, "", "", ""));
            this._adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Original_Ef1", SqlDbType.SmallInt, 0, ParameterDirection.Input, 0, 0, "Ef1", DataRowVersion.Original, false, null, "", "", ""));
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitCommandCollection()
        {
            this._commandCollection = new SqlCommand[] { new SqlCommand() };
            this._commandCollection[0].Connection = this.Connection;
            this._commandCollection[0].CommandText = "SELECT Id, Idp, Extn, Applctn, Dscrptn, IsApp, IsData, IsDoc, IsGeo, IsCode, IsGrp, IsProj, Ef0, Ef1 FROM dbo.FTyps";
            this._commandCollection[0].CommandType = CommandType.Text;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitConnection()
        {
            this._connection = new SqlConnection();
            this._connection.ConnectionString = Settings.Default.DbRootsVII0004ConnectionString;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Insert, true)]
        public virtual int Insert(int Id, int? Idp, string Extn, string Applctn, string Dscrptn, short? IsApp, short? IsData, short? IsDoc, short? IsGeo, short? IsCode, short? IsGrp, short? IsProj, short? Ef0, short? Ef1)
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
            if (Extn == null)
            {
                this.Adapter.InsertCommand.Parameters[2].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[2].Value = Extn;
            }
            if (Applctn == null)
            {
                this.Adapter.InsertCommand.Parameters[3].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[3].Value = Applctn;
            }
            if (Dscrptn == null)
            {
                this.Adapter.InsertCommand.Parameters[4].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[4].Value = Dscrptn;
            }
            if (IsApp.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[5].Value = IsApp.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[5].Value = DBNull.Value;
            }
            if (IsData.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[6].Value = IsData.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[6].Value = DBNull.Value;
            }
            if (IsDoc.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[7].Value = IsDoc.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[7].Value = DBNull.Value;
            }
            if (IsGeo.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[8].Value = IsGeo.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[8].Value = DBNull.Value;
            }
            if (IsCode.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[9].Value = IsCode.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[9].Value = DBNull.Value;
            }
            if (IsGrp.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[10].Value = IsGrp.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[10].Value = DBNull.Value;
            }
            if (IsProj.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[11].Value = IsProj.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[11].Value = DBNull.Value;
            }
            if (Ef0.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[12].Value = Ef0.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[12].Value = DBNull.Value;
            }
            if (Ef1.HasValue)
            {
                this.Adapter.InsertCommand.Parameters[13].Value = Ef1.Value;
            }
            else
            {
                this.Adapter.InsertCommand.Parameters[13].Value = DBNull.Value;
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
            return this.Adapter.Update(dataSet, "FTyps");
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter")]
        public virtual int Update(DbRootsVII0004DataSet.FTypsDataTable dataTable)
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
        public virtual int Update(int? Idp, string Extn, string Applctn, string Dscrptn, short? IsApp, short? IsData, short? IsDoc, short? IsGeo, short? IsCode, short? IsGrp, short? IsProj, short? Ef0, short? Ef1, int Original_Id, int? Original_Idp, short? Original_IsApp, short? Original_IsData, short? Original_IsDoc, short? Original_IsGeo, short? Original_IsCode, short? Original_IsGrp, short? Original_IsProj, short? Original_Ef0, short? Original_Ef1)
        {
            return this.Update(Original_Id, Idp, Extn, Applctn, Dscrptn, IsApp, IsData, IsDoc, IsGeo, IsCode, IsGrp, IsProj, Ef0, Ef1, Original_Id, Original_Idp, Original_IsApp, Original_IsData, Original_IsDoc, Original_IsGeo, Original_IsCode, Original_IsGrp, Original_IsProj, Original_Ef0, Original_Ef1);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), HelpKeyword("vs.data.TableAdapter"), DataObjectMethod(DataObjectMethodType.Update, true)]
        public virtual int Update(int Id, int? Idp, string Extn, string Applctn, string Dscrptn, short? IsApp, short? IsData, short? IsDoc, short? IsGeo, short? IsCode, short? IsGrp, short? IsProj, short? Ef0, short? Ef1, int Original_Id, int? Original_Idp, short? Original_IsApp, short? Original_IsData, short? Original_IsDoc, short? Original_IsGeo, short? Original_IsCode, short? Original_IsGrp, short? Original_IsProj, short? Original_Ef0, short? Original_Ef1)
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
            if (Extn == null)
            {
                this.Adapter.UpdateCommand.Parameters[2].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[2].Value = Extn;
            }
            if (Applctn == null)
            {
                this.Adapter.UpdateCommand.Parameters[3].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[3].Value = Applctn;
            }
            if (Dscrptn == null)
            {
                this.Adapter.UpdateCommand.Parameters[4].Value = DBNull.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[4].Value = Dscrptn;
            }
            if (IsApp.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[5].Value = IsApp.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[5].Value = DBNull.Value;
            }
            if (IsData.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[6].Value = IsData.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[6].Value = DBNull.Value;
            }
            if (IsDoc.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[7].Value = IsDoc.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[7].Value = DBNull.Value;
            }
            if (IsGeo.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[8].Value = IsGeo.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[8].Value = DBNull.Value;
            }
            if (IsCode.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[9].Value = IsCode.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[9].Value = DBNull.Value;
            }
            if (IsGrp.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[10].Value = IsGrp.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[10].Value = DBNull.Value;
            }
            if (IsProj.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[11].Value = IsProj.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[11].Value = DBNull.Value;
            }
            if (Ef0.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[12].Value = Ef0.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[12].Value = DBNull.Value;
            }
            if (Ef1.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[13].Value = Ef1.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[13].Value = DBNull.Value;
            }
            this.Adapter.UpdateCommand.Parameters[14].Value = Original_Id;
            if (Original_Idp.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[15].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x10].Value = Original_Idp.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[15].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x10].Value = DBNull.Value;
            }
            if (Original_IsApp.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x11].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x12].Value = Original_IsApp.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x11].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x12].Value = DBNull.Value;
            }
            if (Original_IsData.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x13].Value = 0;
                this.Adapter.UpdateCommand.Parameters[20].Value = Original_IsData.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x13].Value = 1;
                this.Adapter.UpdateCommand.Parameters[20].Value = DBNull.Value;
            }
            if (Original_IsDoc.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x15].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x16].Value = Original_IsDoc.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x15].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x16].Value = DBNull.Value;
            }
            if (Original_IsGeo.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x17].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x18].Value = Original_IsGeo.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x17].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x18].Value = DBNull.Value;
            }
            if (Original_IsCode.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x19].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x1a].Value = Original_IsCode.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x19].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x1a].Value = DBNull.Value;
            }
            if (Original_IsGrp.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x1b].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x1c].Value = Original_IsGrp.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x1b].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x1c].Value = DBNull.Value;
            }
            if (Original_IsProj.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x1d].Value = 0;
                this.Adapter.UpdateCommand.Parameters[30].Value = Original_IsProj.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x1d].Value = 1;
                this.Adapter.UpdateCommand.Parameters[30].Value = DBNull.Value;
            }
            if (Original_Ef0.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x1f].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x20].Value = Original_Ef0.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x1f].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x20].Value = DBNull.Value;
            }
            if (Original_Ef1.HasValue)
            {
                this.Adapter.UpdateCommand.Parameters[0x21].Value = 0;
                this.Adapter.UpdateCommand.Parameters[0x22].Value = Original_Ef1.Value;
            }
            else
            {
                this.Adapter.UpdateCommand.Parameters[0x21].Value = 1;
                this.Adapter.UpdateCommand.Parameters[0x22].Value = DBNull.Value;
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

