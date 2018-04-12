namespace System.Data.LibCore
{
    using FwNs.Core;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;

    public sealed class UtlCommandBuilder : DbCommandBuilder
    {
        public UtlCommandBuilder() : this(null)
        {
        }

        public UtlCommandBuilder(UtlDataAdapter adp)
        {
            this.QuotePrefix = "\"";
            this.QuoteSuffix = "\"";
            this.DataAdapter = adp;
        }

        protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause)
        {
            ((UtlParameter) parameter).UtlType = (UtlType) row[SchemaTableColumn.ProviderType];
        }

        public UtlCommand GetDeleteCommand()
        {
            return (UtlCommand) base.GetDeleteCommand();
        }

        public UtlCommand GetDeleteCommand(bool useColumnsForParameterNames)
        {
            return (UtlCommand) base.GetDeleteCommand(useColumnsForParameterNames);
        }

        public UtlCommand GetInsertCommand()
        {
            return (UtlCommand) base.GetInsertCommand();
        }

        public UtlCommand GetInsertCommand(bool useColumnsForParameterNames)
        {
            return (UtlCommand) base.GetInsertCommand(useColumnsForParameterNames);
        }

        protected override string GetParameterName(int parameterOrdinal)
        {
            object[] args = new object[] { parameterOrdinal };
            return string.Format(CultureInfo.InvariantCulture, "@p{0}", args);
        }

        protected override string GetParameterName(string parameterName)
        {
            object[] args = new object[] { parameterName };
            return string.Format(CultureInfo.InvariantCulture, "@{0}", args);
        }

        protected override string GetParameterPlaceholder(int parameterOrdinal)
        {
            return this.GetParameterName(parameterOrdinal);
        }

        protected override DataTable GetSchemaTable(DbCommand sourceCommand)
        {
            using (IDataReader reader = sourceCommand.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly))
            {
                DataTable schemaTable = reader.GetSchemaTable();
                if (HasSchemaPrimaryKey(schemaTable))
                {
                    ResetIsUniqueSchemaColumn(schemaTable);
                }
                return schemaTable;
            }
        }

        public UtlCommand GetUpdateCommand()
        {
            return (UtlCommand) base.GetUpdateCommand();
        }

        public UtlCommand GetUpdateCommand(bool useColumnsForParameterNames)
        {
            return (UtlCommand) base.GetUpdateCommand(useColumnsForParameterNames);
        }

        private static bool HasSchemaPrimaryKey(DataTable schema)
        {
            DataColumn column = schema.Columns[SchemaTableColumn.IsKey];
            using (IEnumerator enumerator = schema.Rows.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if ((bool) ((DataRow) enumerator.Current)[column])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string QuoteIdentifier(string unquotedIdentifier)
        {
            if ((string.IsNullOrEmpty(this.QuotePrefix) || string.IsNullOrEmpty(this.QuoteSuffix)) || string.IsNullOrEmpty(unquotedIdentifier))
            {
                return unquotedIdentifier;
            }
            return (this.QuotePrefix + unquotedIdentifier.Replace(this.QuoteSuffix, this.QuoteSuffix + this.QuoteSuffix) + this.QuoteSuffix);
        }

        private static void ResetIsUniqueSchemaColumn(DataTable schema)
        {
            DataColumn column = schema.Columns[SchemaTableColumn.IsUnique];
            DataColumn column2 = schema.Columns[SchemaTableColumn.IsKey];
            foreach (DataRow row in schema.Rows)
            {
                if (!((bool) row[column2]))
                {
                    row[column] = false;
                }
            }
            schema.AcceptChanges();
        }

        private void RowUpdatingEventHandler(object sender, RowUpdatingEventArgs e)
        {
            base.RowUpdatingHandler(e);
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
        {
            UtlDataAdapter adapter2 = (UtlDataAdapter) adapter;
            if (adapter == base.DataAdapter)
            {
                adapter2.RowUpdating -= new EventHandler<RowUpdatingEventArgs>(this.RowUpdatingEventHandler);
            }
            else
            {
                adapter2.RowUpdating += new EventHandler<RowUpdatingEventArgs>(this.RowUpdatingEventHandler);
            }
        }

        public override string UnquoteIdentifier(string quotedIdentifier)
        {
            if ((string.IsNullOrEmpty(this.QuotePrefix) || string.IsNullOrEmpty(this.QuoteSuffix)) || string.IsNullOrEmpty(quotedIdentifier))
            {
                return quotedIdentifier;
            }
            if (!quotedIdentifier.StartsWith(this.QuotePrefix, StringComparison.OrdinalIgnoreCase) || !quotedIdentifier.EndsWith(this.QuoteSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return quotedIdentifier;
            }
            return quotedIdentifier.Substring(this.QuotePrefix.Length, quotedIdentifier.Length - (this.QuotePrefix.Length + this.QuoteSuffix.Length)).Replace(this.QuoteSuffix + this.QuoteSuffix, this.QuoteSuffix);
        }

        public UtlDataAdapter DataAdapter
        {
            get
            {
                return (UtlDataAdapter) base.DataAdapter;
            }
            set
            {
                base.DataAdapter = value;
            }
        }

        [Browsable(false)]
        public override System.Data.Common.CatalogLocation CatalogLocation
        {
            get
            {
                return base.CatalogLocation;
            }
            set
            {
                base.CatalogLocation = value;
            }
        }

        [Browsable(false)]
        public override string CatalogSeparator
        {
            get
            {
                return base.CatalogSeparator;
            }
            set
            {
                base.CatalogSeparator = value;
            }
        }

        [Browsable(false), DefaultValue("\"")]
        public override string QuotePrefix
        {
            get
            {
                return base.QuotePrefix;
            }
            set
            {
                base.QuotePrefix = value;
            }
        }

        [Browsable(false), DefaultValue("\"")]
        public override string QuoteSuffix
        {
            get
            {
                return base.QuoteSuffix;
            }
            set
            {
                base.QuoteSuffix = value;
            }
        }

        [Browsable(false)]
        public override string SchemaSeparator
        {
            get
            {
                return base.SchemaSeparator;
            }
            set
            {
                base.SchemaSeparator = value;
            }
        }
    }
}

