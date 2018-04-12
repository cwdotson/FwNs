namespace System.Data.LibCore
{
    using FwNs.Core.LC.cParsing;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class UtlMetaData
    {
        private const string Selstar = "SELECT * FROM INFORMATION_SCHEMA.";
        private const string WhereTrue = " WHERE 1=1";
        private const string MetaDataCollectionsXml = "LibCore.Resources.MetaDataCollections.xml";
        private readonly UtlConnection _cnn;

        public UtlMetaData(UtlConnection cnn)
        {
            this._cnn = cnn;
        }

        private static string And(string id, string op, object val)
        {
            if (val == null)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            string str = val as string;
            bool flag = str > null;
            if (flag && (str.Length == 0))
            {
                return builder.Append(" AND ").Append(id).Append(" IS NULL").ToString();
            }
            string str2 = flag ? ("'" + val + "'") : val.ToString();
            builder.Append(" AND ").Append(id).Append(' ');
            if (flag && "LIKE".Equals(op, StringComparison.OrdinalIgnoreCase))
            {
                if ((str2.IndexOf('_') < 0) && (str2.IndexOf('%') < 0))
                {
                    builder.Append("=").Append(' ').Append(str2);
                }
                else
                {
                    builder.Append("LIKE").Append(' ').Append(str2);
                    if ((str2.IndexOf(@"\_") >= 0) || (str2.IndexOf(@"\%") >= 0))
                    {
                        builder.Append(@" ESCAPE '\'");
                    }
                }
            }
            else
            {
                builder.Append(op).Append(' ').Append(str2);
            }
            return builder.ToString();
        }

        private DbDataReader Execute(string select)
        {
            UtlCommand command1 = this._cnn.CreateCommand();
            command1.CommandText = select;
            return command1.ExecuteReader();
        }

        private DbDataReader ExecuteSelect(string table, string where)
        {
            string select = "SELECT * FROM INFORMATION_SCHEMA." + table;
            if (where != null)
            {
                select = select + " WHERE " + where;
            }
            return this.Execute(select);
        }

        public DataTable GetCharacterSets(string catalogPattern, string schemaPattern, string namePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("CHARACTER_SETS").Append(And("CHARACTER_SET_CATALOG", "LIKE", catalogPattern)).Append(And("CHARACTER_SET_SCHEMA", "LIKE", schemaPattern)).Append(And("CHARACTER_SET_NAME", "LIKE", namePattern)).Append(" ORDER BY CHARACTER_SET_NAME").ToString());
            DataTable t = new DataTable("CharacterSets");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetCheckConstraints(string catalogPattern, string schemaPattern, string namePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("CHECK_CONSTRAINTS").Append(And("CONSTRAINT_CATALOG", "LIKE", catalogPattern)).Append(And("CONSTRAINT_SCHEMA", "LIKE", schemaPattern)).Append(And("CONSTRAINT_NAME", "LIKE", namePattern)).Append(" ORDER BY CONSTRAINT_NAME").ToString());
            DataTable t = new DataTable("CheckConstraints");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetCollations(string catalogPattern, string schemaPattern, string namePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("COLLATIONS").Append(And("COLLATION_CATALOG", "LIKE", catalogPattern)).Append(And("COLLATION_SCHEMA", "LIKE", schemaPattern)).Append(And("COLLATION_NAME", "LIKE", namePattern)).Append(" ORDER BY COLLATION_NAME").ToString());
            DataTable t = new DataTable("Collations");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetColumnPrivilages(string catalogPattern, string schemaPattern, string tableNamePattern, string columnNamePattern, string grantor, string grantee)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("ROLE_COLUMN_GRANTS").Append(And("TABLE_CATALOG", "LIKE", catalogPattern)).Append(And("TABLE_SCHEMA", "LIKE", schemaPattern)).Append(And("TABLE_NAME", "LIKE", tableNamePattern)).Append(And("COLUMN_NAME", "LIKE", columnNamePattern)).Append(And("GRANTOR", "LIKE", grantor)).Append(And("GRANTEE", "LIKE", grantee)).Append(" ORDER BY TABLE_NAME").ToString());
            DataTable t = new DataTable("ColumnPrivilages");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetColumns(string catalogPattern, string schemaPattern, string tableNamePattern, string columnNamePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_COLUMNS").Append(And("TABLE_CAT", "LIKE", catalogPattern)).Append(And("TABLE_SCHEM", "LIKE", schemaPattern)).Append(And("TABLE_NAME", "LIKE", tableNamePattern)).Append(And("COLUMN_NAME", "LIKE", columnNamePattern)).Append("ORDER BY TABLE_NAME,ORDINAL_POSITION ").ToString());
            DataTable t = new DataTable("Columns");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        private static DataTable GetDataSourceInformation()
        {
            DataTable table = new DataTable("DataSourceInformation") {
                Locale = CultureInfo.InvariantCulture
            };
            table.Columns.Add(DbMetaDataColumnNames.CompositeIdentifierSeparatorPattern, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.DataSourceProductName, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.DataSourceProductVersion, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.DataSourceProductVersionNormalized, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.GroupByBehavior, typeof(int));
            table.Columns.Add(DbMetaDataColumnNames.IdentifierPattern, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.IdentifierCase, typeof(int));
            table.Columns.Add(DbMetaDataColumnNames.OrderByColumnsInSelect, typeof(bool));
            table.Columns.Add(DbMetaDataColumnNames.ParameterMarkerFormat, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.ParameterMarkerPattern, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.ParameterNameMaxLength, typeof(int));
            table.Columns.Add(DbMetaDataColumnNames.ParameterNamePattern, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.QuotedIdentifierPattern, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.QuotedIdentifierCase, typeof(int));
            table.Columns.Add(DbMetaDataColumnNames.StatementSeparatorPattern, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.StringLiteralPattern, typeof(string));
            table.Columns.Add(DbMetaDataColumnNames.SupportedJoinOperators, typeof(int));
            table.BeginLoadData();
            DataRow row = table.NewRow();
            object[] objArray1 = new object[0x11];
            objArray1[1] = "LibCore";
            objArray1[2] = "Alpha-Testing";
            objArray1[3] = "Alpha-Testing";
            objArray1[4] = 3;
            objArray1[5] = "(^\\[\\p{Lo}\\p{Lu}\\p{Ll}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Nd}@$#_]*$)|(^\\\"[^\\\"\\0]|\\\"\\\"+\\]$)|(^\\\"[^\\\"\\0]|\\\"\\\"+\\\"$)";
            objArray1[6] = 0;
            objArray1[7] = false;
            objArray1[8] = "{0}";
            objArray1[9] = @"@[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            objArray1[10] = 0xff;
            objArray1[11] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            objArray1[12] = "(([^\\\"]|\\\"\\\")*)";
            objArray1[13] = 0;
            objArray1[14] = ";";
            objArray1[15] = "'(([^']|'')*)'";
            objArray1[0x10] = 15;
            row.ItemArray = objArray1;
            table.Rows.Add(row);
            table.AcceptChanges();
            table.EndLoadData();
            return table;
        }

        public DataTable GetDataTypes(string typeName)
        {
            DataTable table = new DataTable("DataTypes");
            try
            {
                table.Locale = CultureInfo.InvariantCulture;
                table.Columns.Add("TypeName", typeof(string));
                table.Columns.Add("ProviderDbType", typeof(int));
                table.Columns.Add("DataType", typeof(string));
                table.Columns.Add("ColumnSize", typeof(long));
                table.Columns.Add("CreateFormat", typeof(string));
                table.Columns.Add("CreateParameters", typeof(string));
                table.Columns.Add("IsAutoIncrementable", typeof(bool));
                table.Columns.Add("IsBestMatch", typeof(bool));
                table.Columns.Add("IsCaseSensitive", typeof(bool));
                table.Columns.Add("IsFixedLength", typeof(bool));
                table.Columns.Add("IsFixedPrecisionScale", typeof(bool));
                table.Columns.Add("IsLong", typeof(bool));
                table.Columns.Add("IsNullable", typeof(bool));
                table.Columns.Add("IsSearchable", typeof(bool));
                table.Columns.Add("IsSearchableWithLike", typeof(bool));
                table.Columns.Add("IsLiteralSupported", typeof(bool));
                table.Columns.Add("LiteralPrefix", typeof(string));
                table.Columns.Add("LiteralSuffix", typeof(string));
                table.Columns.Add("IsUnsigned", typeof(bool));
                table.Columns.Add("MaximumScale", typeof(short));
                table.Columns.Add("MinimumScale", typeof(short));
                table.Columns.Add("IsConcurrencyType", typeof(bool));
                DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_TYPEINFO").Append(And("TYPE_NAME", "LIKE", typeName)).ToString());
                table.BeginLoadData();
                while (reader.Read())
                {
                    DataRow row = table.NewRow();
                    row["TypeName"] = reader["TYPE_NAME"];
                    row["ProviderDbType"] = reader["SQL_DATA_TYPE"];
                    row["DataType"] = reader["LOCAL_TYPE_NAME"];
                    row["ColumnSize"] = reader["PRECISION"];
                    row["CreateFormat"] = reader["CREATE_FORMAT"];
                    row["CreateParameters"] = reader["CREATE_PARAMS"];
                    row["IsAutoIncrementable"] = reader["AUTO_INCREMENT"];
                    row["IsBestMatch"] = true;
                    row["IsFixedLength"] = reader["FIXED_PREC_SCALE"];
                    row["IsFixedPrecisionScale"] = reader["FIXED_PREC_SCALE"];
                    row["IsLong"] = false;
                    row["IsNullable"] = Convert.ToInt32(reader["NULLABLE"]) > 0;
                    row["IsSearchable"] = Convert.ToInt32(reader["SEARCHABLE"]) > 0;
                    row["IsLiteralSupported"] = DBNull.Value;
                    row["LiteralPrefix"] = reader["LITERAL_PREFIX"];
                    row["LiteralSuffix"] = reader["LITERAL_SUFFIX"];
                    row["IsUnsigned"] = reader["UNSIGNED_ATTRIBUTE"];
                    row["MaximumScale"] = reader["MAXIMUM_SCALE"];
                    row["MinimumScale"] = reader["MINIMUM_SCALE"];
                    row["IsConcurrencyType"] = false;
                    table.Rows.Add(row);
                }
                table.AcceptChanges();
                table.EndLoadData();
            }
            catch (Exception)
            {
                table.Dispose();
                throw;
            }
            return table;
        }

        public DataTable GetDomainConstraints(string schemaPattern, string domainNamePattern, string namePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("DOMAIN_CONSTRAINTS").Append(And("DOMAIN_SCHEMA", "LIKE", schemaPattern)).Append(And("DOMAIN_NAME", "LIKE", domainNamePattern)).Append(And("CONSTRAINT_NAME", "LIKE", namePattern)).Append(" ORDER BY DOMAIN_NAME").ToString());
            DataTable t = new DataTable("Domains");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetDomains(string catalogPattern, string schemaPattern, string namePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("DOMAINS").Append(And("DOMAIN_CATALOG", "LIKE", catalogPattern)).Append(And("DOMAIN_SCHEMA", "LIKE", schemaPattern)).Append(And("DOMAIN_NAME", "LIKE", namePattern)).Append(" ORDER BY DOMAIN_NAME").ToString());
            DataTable t = new DataTable("Domains");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetExportedKeys(string catalogPattern, string schemaPattern, string tableNamePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_CROSSREFERENCE").Append(And("PKTABLE_CAT", "LIKE", catalogPattern)).Append(And("PKTABLE_SCHEM", "LIKE", schemaPattern)).Append(And("PKTABLE_NAME", "LIKE", tableNamePattern)).ToString());
            DataTable t = new DataTable("ExportedKeys");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetForeignKeyColumns(string catalogPattern, string schemaPattern, string tableNamePattern, string namePattern, string columPattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_CROSSREFERENCE").Append(And("FKTABLE_CAT", "LIKE", catalogPattern)).Append(And("FKTABLE_SCHEM", "LIKE", schemaPattern)).Append(And("FKTABLE_NAME", "LIKE", tableNamePattern)).Append(And("FK_NAME", "LIKE", namePattern)).Append(And("FKCOLUMN_NAME", "LIKE", columPattern)).Append("ORDER BY FK_NAME,KEY_SEQ").ToString());
            DataTable t = new DataTable("ForeignKeyColumns");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetForeignKeys(string catalogPattern, string schemaPattern, string tableNamePattern, string namePattern)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("SELECT DISTINCT FKTABLE_CAT,FKTABLE_SCHEM,FKTABLE_NAME,FK_NAME, PKTABLE_CAT,PKTABLE_SCHEM,PKTABLE_NAME, PK_NAME,UPDATE_RULE,DELETE_RULE,DEFERRABILITY FROM INFORMATION_SCHEMA.SYSTEM_CROSSREFERENCE ");
            DbDataReader reader = this.Execute(builder1.Append(" WHERE 1=1").Append(And("FKTABLE_CAT", "LIKE", catalogPattern)).Append(And("FKTABLE_SCHEM", "LIKE", schemaPattern)).Append(And("FKTABLE_NAME", "LIKE", tableNamePattern)).Append(And("FK_NAME", "LIKE", namePattern)).Append(" ORDER BY FKTABLE_NAME,FK_NAME").ToString());
            DataTable t = new DataTable("ForeignKeys");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetFunctionParameters(string catalogPattern, string schemaPattern, string functionNamePattern, string parameterNamePattern)
        {
            StringBuilder builder = new StringBuilder(0xff);
            builder.Append("select pc.procedure_cat as FUNCTION_CAT,").Append("pc.procedure_schem as FUNCTION_SCHEM,").Append("pc.procedure_name as FUNCTION_NAME,").Append("pc.column_name as COLUMN_NAME,").Append("case pc.column_type").Append(" when 3 then 5").Append(" when 4 then 3").Append(" when 5 then 4").Append(" else pc.column_type").Append(" end as COLUMN_TYPE,").Append("pc.DATA_TYPE,").Append("pc.TYPE_NAME,").Append("pc.PRECISION,").Append("pc.LENGTH,").Append("pc.SCALE,").Append("pc.RADIX,").Append("pc.NULLABLE,").Append("pc.REMARKS,").Append("pc.CHAR_OCTET_LENGTH,").Append("pc.ORDINAL_POSITION,").Append("pc.IS_NULLABLE,").Append("pc.SPECIFIC_NAME,").Append("case pc.column_type").Append(" when 3 then 1").Append(" else 0").Append(" end AS COLUMN_GROUP ").Append("from information_schema.system_procedurecolumns pc ").Append("join (select procedure_schem,").Append("procedure_name,").Append("specific_name ").Append("from information_schema.system_procedures ").Append("where procedure_type = 2) p ").Append("on pc.procedure_schem = p.procedure_schem ").Append("and pc.procedure_name = p.procedure_name ").Append("and pc.specific_name = p.specific_name ").Append("and ((pc.column_type = 3 and pc.column_name = '@p0') ").Append("or ").Append("(pc.column_type <> 3)) ");
            builder.Append("where 1=1 ").Append(And("pc.procedure_cat", "LIKE", catalogPattern)).Append(And("pc.procedure_schem", "LIKE", schemaPattern)).Append(And("pc.procedure_name", "LIKE", functionNamePattern)).Append(And("pc.column_name", "LIKE", parameterNamePattern)).Append("order by 1, 2, 3, 17, 18 , 15");
            DbDataReader reader = this.Execute(builder.ToString());
            DataTable t = new DataTable("Functions");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetFunctions(string catalogPattern, string schemaPattern, string namePattern)
        {
            DbDataReader reader = this.Execute(new StringBuilder(0xff).Append("SELECT PROCEDURE_CAT AS FUNCTION_CAT,PROCEDURE_SCHEM AS FUNCTION_SCHEM, PROCEDURE_NAME AS FUNCTION_NAME,REMARKS,SPECIFIC_NAME FROM INFORMATION_SCHEMA.SYSTEM_PROCEDURES WHERE").Append(" PROCEDURE_TYPE = 2 ").Append(And("PROCEDURE_CAT", "LIKE", catalogPattern)).Append(And("PROCEDURE_SCHEM", "LIKE", schemaPattern)).Append(And("PROCEDURE_NAME", "LIKE", namePattern)).ToString());
            DataTable t = new DataTable("Functions");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetImportedKeys(string catalogPattern, string schemaPattern, string tableNamePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_CROSSREFERENCE").Append(And("FKTABLE_CAT", "LIKE", catalogPattern)).Append(And("FKTABLE_SCHEM", "LIKE", schemaPattern)).Append(And("FKTABLE_NAME", "LIKE", tableNamePattern)).Append(" ORDER BY PKTABLE_CAT, PKTABLE_SCHEM, PKTABLE_NAME, KEY_SEQ").ToString());
            DataTable t = new DataTable("ImportedKeys");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetIndexColumns(string catalog, string schema, string table, string index, string name, string unique)
        {
            StringBuilder builder = null;
            if (!string.IsNullOrEmpty(unique))
            {
                bool? val = Convert.ToBoolean(unique, CultureInfo.CurrentCulture) ? ((bool?) false) : null;
                builder = ToQueryPrefix("SYSTEM_INDEXINFO").Append(And("TABLE_CAT", "LIKE", catalog)).Append(And("TABLE_SCHEM", "LIKE", schema)).Append(And("TABLE_NAME", "LIKE", table)).Append(And("INDEX_NAME", "LIKE", index)).Append(And("COLUMN_NAME", "LIKE", name)).Append(And("NON_UNIQUE", "=", val));
            }
            else
            {
                builder = ToQueryPrefix("SYSTEM_INDEXINFO").Append(And("TABLE_CAT", "LIKE", catalog)).Append(And("TABLE_SCHEM", "LIKE", schema)).Append(And("TABLE_NAME", "LIKE", table)).Append(And("INDEX_NAME", "LIKE", index)).Append(And("COLUMN_NAME", "LIKE", name));
            }
            DbDataReader reader = this.Execute(builder.ToString());
            DataTable t = new DataTable("IndexeColumns");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetIndexes(string catalog, string schema, string table, string index, string unique)
        {
            if (table == null)
            {
                throw UtlException.GetException(0x1a7);
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT DISTINCT TABLE_CAT,TABLE_SCHEM,TABLE_NAME,NON_UNIQUE, INDEX_QUALIFIER,INDEX_NAME,UNIQUE_INDEX as \"UNIQUE\",PRIMARY_INDEX as\"PRIMARY_KEY\" FROM INFORMATION_SCHEMA.SYSTEM_INDEXINFO ");
            StringBuilder builder2 = null;
            if (!string.IsNullOrEmpty(unique))
            {
                bool? val = Convert.ToBoolean(unique, CultureInfo.CurrentCulture) ? ((bool?) false) : null;
                builder2 = builder.Append(" WHERE 1=1").Append(And("TABLE_CAT", "LIKE", catalog)).Append(And("TABLE_SCHEM", "LIKE", schema)).Append(And("TABLE_NAME", "LIKE", table)).Append(And("INDEX_NAME", "LIKE", index)).Append(And("NON_UNIQUE", "=", val));
            }
            else
            {
                builder2 = builder.Append(" WHERE 1=1").Append(And("TABLE_CAT", "LIKE", catalog)).Append(And("TABLE_SCHEM", "LIKE", schema)).Append(And("TABLE_NAME", "LIKE", table)).Append(And("INDEX_NAME", "LIKE", index));
            }
            DbDataReader reader = this.Execute(builder2.ToString());
            DataTable t = new DataTable("Indexes");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetMetaData(string collectionName, string[] restrictionValues)
        {
            string[] array = new string[6];
            if (restrictionValues == null)
            {
                restrictionValues = new string[0];
            }
            restrictionValues.CopyTo(array, 0);
            string s = collectionName.ToUpper(CultureInfo.InvariantCulture);
            switch (<PrivateImplementationDetails>.ComputeStringHash(s))
            {
                case 0x8c2feb3:
                    if (s == "USERS")
                    {
                        return this.GetUsers(array[0]);
                    }
                    goto Label_07A4;

                case 0xb672eed:
                    if (s == "COLLATIONS")
                    {
                        return this.GetCollations(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0x2720b35:
                    if (s == "PROCEDURECOLUMNS")
                    {
                        return this.GetProcedureColumns(array[0], array[1], array[2], array[3]);
                    }
                    goto Label_07A4;

                case 0x8bca585:
                    if (s == "TABLECONSTRAINTS")
                    {
                        return this.GetTableConstraints(array[0], array[1], array[2], array[3]);
                    }
                    goto Label_07A4;

                case 0x1a5775c6:
                    if (s == "INDEXCOLUMNS")
                    {
                        return this.GetIndexColumns(array[0], array[1], array[2], array[3], array[4], array[5]);
                    }
                    goto Label_07A4;

                case 0x1cfef9d3:
                    if (s == "COLUMNPRIVILAGES")
                    {
                        return this.GetColumnPrivilages(array[0], array[1], array[2], array[3], array[4], array[5]);
                    }
                    goto Label_07A4;

                case 0x240979a7:
                    if (s == "TABLECHECKCONSTRAINTS")
                    {
                        return this.GetTableCheckConstraints(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0x25a2363d:
                    if (s == "CHECKCONSTRAINTS")
                    {
                        return this.GetCheckConstraints(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0x26e9688d:
                    if (s == "IMPORTEDKEYS")
                    {
                        return this.GetImportedKeys(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0x2e31c1db:
                    if (s == "INDEXES")
                    {
                        return this.GetIndexes(array[0], array[1], array[2], array[3], array[4]);
                    }
                    goto Label_07A4;

                case 0x2fbd0bae:
                    if (s == "PROCEDUREPARAMETERS")
                    {
                        return this.GetProcedureParameters(array[0], array[1], array[2], array[3]);
                    }
                    goto Label_07A4;

                case 0x29835559:
                    if (s == "PRIMARYKEYS")
                    {
                        return this.GetPrimaryKeys(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0x2c7ab01a:
                    if (s == "TABLECOLUMNS")
                    {
                        break;
                    }
                    goto Label_07A4;

                case 0x32d9a99c:
                    if (s == "COLUMNS")
                    {
                        break;
                    }
                    goto Label_07A4;

                case 0x330168f1:
                    if (s == "VIEWS")
                    {
                        return this.GetViews(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0x605ef30d:
                    if (s == "FUNCTIONPARAMETERS")
                    {
                        return this.GetFunctionParameters(array[0], array[1], array[2], array[3]);
                    }
                    goto Label_07A4;

                case 0x66d47ea3:
                    if (s == "VIEWCOLUMNS")
                    {
                        return this.GetViewColumns(array[0], array[1], array[2], array[3]);
                    }
                    goto Label_07A4;

                case 0x755e2a9d:
                    if (s == "METADATACOLLECTIONS")
                    {
                        return GetMetaDataCollections();
                    }
                    goto Label_07A4;

                case 0xa33a8561:
                    if (s == "SEQUENCES")
                    {
                        return this.GetSequences(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0xa56a027a:
                    if (s == "TYPES")
                    {
                        return this.GetTypes();
                    }
                    goto Label_07A4;

                case 0x76300d04:
                    if (s == "TABLES")
                    {
                        return this.GetTables(array[0], array[1], array[2], array[3]);
                    }
                    goto Label_07A4;

                case 0x9aafcbde:
                    if (s == "EXPORTEDKEYS")
                    {
                        return this.GetExportedKeys(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0xaade11f1:
                    if (s == "FOREIGNKEYCOLUMNS")
                    {
                        return this.GetForeignKeyColumns(array[0], array[1], array[2], array[3], array[4]);
                    }
                    goto Label_07A4;

                case 0xaf5f733a:
                    if (s == "RESTRICTIONS")
                    {
                        return GetRestrictions();
                    }
                    goto Label_07A4;

                case 0xb3f9a3a2:
                    if (s == "RESERVEDWORDS")
                    {
                        return GetReservedWords();
                    }
                    goto Label_07A4;

                case 0xb553d310:
                    if (s == "TRIGGERS")
                    {
                        return this.GetTriggers(array[0], array[1], array[2], array[3]);
                    }
                    goto Label_07A4;

                case 0xb90fceff:
                    if (s == "SCHEMAS")
                    {
                        return this.GetSchemas();
                    }
                    goto Label_07A4;

                case 0xc053666f:
                    if (s == "TABLEPRIVILEGES")
                    {
                        return this.GetTablePrivileges(array[0], array[1], array[2], array[3], array[4]);
                    }
                    goto Label_07A4;

                case 0xd2acc9c6:
                    if (s == "DOMAINS")
                    {
                        return this.GetDomains(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0xb95a02fb:
                    if (s == "FOREIGNKEYS")
                    {
                        return this.GetForeignKeys(array[0], array[1], array[2], array[3]);
                    }
                    goto Label_07A4;

                case 0xbe9dd7c7:
                    if (s == "PROCEDURES")
                    {
                        return this.GetProcedures(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0xe600d006:
                    if (s == "DATASOURCEINFORMATION")
                    {
                        return GetDataSourceInformation();
                    }
                    goto Label_07A4;

                case 0xe8a96b27:
                    if (s == "DOMAINCONSTRAINTS")
                    {
                        return this.GetDomainConstraints(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0xeda1cb5a:
                    if (s == "DATATYPES")
                    {
                        return this.GetDataTypes(array[0]);
                    }
                    goto Label_07A4;

                case 0xf4fa63a9:
                    if (s == "CHARACTERSETS")
                    {
                        return this.GetCharacterSets(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                case 0xfdefadae:
                    if (s == "FUNCTIONS")
                    {
                        return this.GetFunctions(array[0], array[1], array[2]);
                    }
                    goto Label_07A4;

                default:
                    goto Label_07A4;
            }
            return this.GetColumns(array[0], array[1], array[2], array[3]);
        Label_07A4:
            throw new NotSupportedException();
        }

        public static DataTable GetMetaDataCollections()
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LibCore.Resources.MetaDataCollections.xml");
            DataSet set1 = new DataSet();
            set1.ReadXml(manifestResourceStream);
            return set1.Tables[DbMetaDataCollectionNames.MetaDataCollections];
        }

        public DataTable GetPrimaryKeys(string catalog, string schema, string table)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_PRIMARYKEYS").Append(And("TABLE_CAT", "LIKE", catalog)).Append(And("TABLE_SCHEM", "LIKE", schema)).Append(And("TABLE_NAME", "LIKE", table)).ToString());
            DataTable t = new DataTable("PrimaryKeys");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetProcedureColumns(string catalogPattern, string schemaPattern, string procedureNamePattern, string columnNamePattern)
        {
            DbDataReader reader = this.Execute("SELECT * FROM INFORMATION_SCHEMA.SYSTEM_PROCEDURECOLUMNS WHERE 1=0");
            DataTable t = new DataTable("ProcedureColumns");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetProcedureParameters(string catalogPattern, string schemaPattern, string procedureNamePattern, string columnNamePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_PROCEDURECOLUMNS").Append(And("PROCEDURE_CAT", "LIKE", catalogPattern)).Append(And("PROCEDURE_SCHEM", "LIKE", schemaPattern)).Append(And("PROCEDURE_NAME", "LIKE", procedureNamePattern)).Append(And("COLUMN_NAME", "LIKE", columnNamePattern)).ToString());
            DataTable t = new DataTable("ProcedureParameters");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetProcedures(string catalogPattern, string schemaPattern, string procedureNamePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_PROCEDURES").Append(" AND PROCEDURE_TYPE = 1 ").Append(And("PROCEDURE_CAT", "LIKE", catalogPattern)).Append(And("PROCEDURE_SCHEM", "LIKE", schemaPattern)).Append(And("PROCEDURE_NAME", "LIKE", procedureNamePattern)).ToString());
            DataTable t = new DataTable("Procedures");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        private static DataTable GetReservedWords()
        {
            DataTable table = new DataTable("MetaDataCollections");
            try
            {
                table.Locale = CultureInfo.InvariantCulture;
                table.Columns.Add("ReservedWord", typeof(string));
                table.Columns.Add("MaximumVersion", typeof(string));
                table.Columns.Add("MinimumVersion", typeof(string));
                table.BeginLoadData();
                foreach (string str in FwNs.Core.LC.cParsing.Tokens.GetReservedWords())
                {
                    DataRow row = table.NewRow();
                    row[0] = str;
                    table.Rows.Add(row);
                    row[1] = DBNull.Value;
                    row[2] = DBNull.Value;
                }
                table.AcceptChanges();
                table.EndLoadData();
            }
            catch (Exception)
            {
                table.Dispose();
                throw;
            }
            return table;
        }

        public static DataTable GetRestrictions()
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LibCore.Resources.MetaDataCollections.xml");
            DataSet set1 = new DataSet();
            set1.ReadXml(manifestResourceStream);
            return set1.Tables[DbMetaDataCollectionNames.Restrictions];
        }

        public DataTable GetSchemas()
        {
            DbDataReader reader = this.ExecuteSelect("SYSTEM_SCHEMAS", null);
            DataTable t = new DataTable("Schemas");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetSequences(string catalogPattern, string schemaPattern, string namePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_SEQUENCES").Append(And("SEQUENCE_CATALOG", "LIKE", catalogPattern)).Append(And("SEQUENCE_SCHEMA", "LIKE", schemaPattern)).Append(And("SEQUENCE_NAME", "LIKE", namePattern)).Append(" ORDER BY SEQUENCE_NAME").ToString());
            DataTable t = new DataTable("Sequences");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetTableCheckConstraints(string catalogPattern, string schemaPattern, string namePattern)
        {
            DbDataReader reader = this.Execute(new StringBuilder("SELECT cc.CONSTRAINT_NAME,cc.CHECK_CLAUSE FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS as cc\r\n                        INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS as tc ON\r\n                        tc.CONSTRAINT_CATALOG = cc.CONSTRAINT_CATALOG AND tc.CONSTRAINT_SCHEMA = cc.CONSTRAINT_SCHEMA\r\n                          AND tc.CONSTRAINT_NAME = cc.CONSTRAINT_NAME   WHERE 1=1 ").Append(And("tc.TABLE_CATALOG", "LIKE", catalogPattern)).Append(And("tc.TABLE_SCHEMA", "LIKE", schemaPattern)).Append(And("tc.TABLE_NAME", "LIKE", namePattern)).Append(" ORDER BY CONSTRAINT_NAME").ToString());
            DataTable t = new DataTable("TableCheckConstraints");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetTableConstraints(string tableCatalog, string tableSchema, string tableName, string constraintName)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("TABLE_CONSTRAINTS").Append(And("TABLE_CATALOG", "LIKE", tableCatalog)).Append(And("TABLE_SCHEMA", "LIKE", tableSchema)).Append(And("TABLE_NAME", "LIKE", tableName)).Append(And("CONSTRAINT_NAME", "LIKE", constraintName)).ToString());
            DataTable t = new DataTable("TablesConstraints");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetTablePrivileges(string catalogPattern, string schemaPattern, string tableNamePattern, string grantorPattern, string granteePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("TABLE_PRIVILEGES").Append(And("TABLE_CATALOG", "LIKE", catalogPattern)).Append(And("TABLE_SCHEMA", "LIKE", schemaPattern)).Append(And("TABLE_NAME", "LIKE", tableNamePattern)).Append(And("GRANTOR", "LIKE", grantorPattern)).Append(And("GRANTEE", "LIKE", granteePattern)).ToString());
            DataTable t = new DataTable("TablePrivileges");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetTables(string catalogPattern, string schemaPattern, string tableNamePattern, string tableType)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_TABLES").Append(And("TABLE_CAT", "LIKE", catalogPattern)).Append(And("TABLE_SCHEM", "LIKE", schemaPattern)).Append(And("TABLE_NAME", "LIKE", tableNamePattern)).Append(And("TABLE_TYPE", "LIKE", tableType)).ToString());
            DataTable t = new DataTable("Tables");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetTriggers(string catalog, string schema, string table, string trigger)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("TRIGGERS").Append(And("EVENT_OBJECT_CATALOG", "LIKE", catalog)).Append(And("EVENT_OBJECT_SCHEMA", "LIKE", schema)).Append(And("EVENT_OBJECT_TABLE", "LIKE", table)).Append(And("TRIGGER_NAME", "LIKE", trigger)).Append(" ORDER BY EVENT_OBJECT_TABLE,TRIGGER_NAME").ToString());
            DataTable t = new DataTable("Triggers");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetTypes()
        {
            DbDataReader reader = this.ExecuteSelect("SYSTEM_TYPEINFO", null);
            DataTable t = new DataTable("Types");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetUsers(string userName)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_USERS").Append(And("USER_NAME", "LIKE", userName)).ToString());
            DataTable t = new DataTable("Users");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetViewColumns(string catalogPattern, string schemaPattern, string namePattern, string columnNamePattern)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("SYSTEM_COLUMNS").Append(And("TABLE_CAT", "LIKE", catalogPattern)).Append(And("TABLE_SCHEM", "LIKE", schemaPattern)).Append(And("TABLE_NAME", "LIKE", namePattern)).Append(And("COLUMN_NAME", "LIKE", columnNamePattern)).Append("ORDER BY TABLE_NAME,ORDINAL_POSITION ").ToString());
            DataTable t = new DataTable("ViewColumns");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public DataTable GetViews(string catalog, string schema, string view)
        {
            DbDataReader reader = this.Execute(ToQueryPrefix("VIEWS").Append(And("TABLE_CATALOG", "LIKE", catalog)).Append(And("TABLE_SCHEMA", "LIKE", schema)).Append(And("TABLE_NAME", "LIKE", view)).Append(" AND view_definition IS NOT NULL").Append(" ORDER BY TABLE_NAME ").ToString());
            DataTable t = new DataTable("Views");
            try
            {
                PopulateDataTable(t, reader);
            }
            catch (Exception)
            {
                t.Dispose();
                throw;
            }
            return t;
        }

        public static void PopulateDataTable(DataTable t, DbDataReader reader)
        {
            DataTable schemaTable = reader.GetSchemaTable();
            int count = schemaTable.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = schemaTable.Rows[i];
                t.Columns.Add((string) row[SchemaTableColumn.ColumnName], (Type) row[SchemaTableColumn.DataType]);
            }
            t.BeginLoadData();
            while (reader.Read())
            {
                DataRow row = t.NewRow();
                for (int j = 0; j < count; j++)
                {
                    row[j] = reader.GetValue(j);
                }
                t.Rows.Add(row);
            }
            t.AcceptChanges();
            t.EndLoadData();
        }

        private static StringBuilder ToQueryPrefix(string t)
        {
            return new StringBuilder(0xff).Append("SELECT * FROM INFORMATION_SCHEMA.").Append(t).Append(" WHERE 1=1");
        }
    }
}

