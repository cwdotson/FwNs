namespace FwNs.Core.LC.cTables
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using System;

    public sealed class TableUtil
    {
        public static void AddAutoColumns(Table table, SqlType[] colTypes)
        {
            for (int i = 0; i < colTypes.Length; i++)
            {
                ColumnSchema column = new ColumnSchema(QNameManager.GetAutoColumnName(i), colTypes[i], true, false, null);
                table.AddColumnNoCheck(column);
            }
        }

        public static Table NewLookupTable(Database database)
        {
            Table table;
            QNameManager.QName subqueryTableName = database.NameManager.GetSubqueryTableName();
            ColumnSchema column = new ColumnSchema(QNameManager.GetAutoColumnName(0), SqlType.SqlInteger, false, true, null);
            TableDerived derived1 = new TableDerived(database, subqueryTableName, 2, null, null);
            derived1.AddColumn(column);
            TableDerived derived2 = table = derived1;
            QNameManager.QName name = derived2.GetName();
            int[] columns = new int[1];
            table.CreatePrimaryKeyConstraint(name, columns, true);
            return derived2;
        }

        public static Table NewLookupTable(Database database, QNameManager.QName tableName, int tableType, QNameManager.QName colName, SqlType colType)
        {
            Table table;
            ColumnSchema column = new ColumnSchema(colName, colType, false, true, null);
            TableDerived derived1 = new TableDerived(database, tableName, tableType);
            derived1.AddColumn(column);
            TableDerived derived2 = table = derived1;
            QNameManager.QName indexName = derived2.GetName();
            int[] columns = new int[1];
            table.CreatePrimaryKeyConstraint(indexName, columns, true);
            return derived2;
        }

        public static Table NewTable(Database database, int type, QNameManager.QName tableHsqlName)
        {
            return new Table(database, tableHsqlName, type);
        }

        public static void SetColumnsInSchemaTable(Table table, QNameManager.QName[] columnNames, SqlType[] columnTypes, byte[] columnNullability)
        {
            for (int i = 0; i < columnNames.Length; i++)
            {
                QNameManager.QName name = columnNames[i];
                ColumnSchema column = new ColumnSchema(table.database.NameManager.NewColumnSchemaQName(table.GetName(), name), columnTypes[i], (columnNullability == null) || (columnNullability[i] > 0), false, null);
                table.AddColumn(column);
            }
            table.SetColumnStructures();
        }

        public static void SetTableIndexesForSubquery(Table table, bool fullIndex, bool uniqueRows)
        {
            int[] colindex = null;
            if (fullIndex)
            {
                colindex = new int[table.GetColumnCount()];
                ArrayUtil.FillSequence(colindex);
            }
            table.CreatePrimaryKey(null, uniqueRows ? colindex : null, false);
            if (uniqueRows)
            {
                table.FullIndex = table.GetPrimaryIndex();
            }
            else if (fullIndex)
            {
                table.FullIndex = table.CreateIndexForColumns(null, colindex);
            }
        }
    }
}

