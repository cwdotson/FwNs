namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class StatementInsert : StatementDML
    {
        public ResultMetaData GeneratedInputMetaData;
        public int GeneratedType;
        public bool IsSimpleInsert;
        public int OverrideUserValue;

        public StatementInsert(Session session, Table targetTable, int[] columnMap, Expression insertExpression, bool[] checkColumns) : base(50, 0x7d4, session.GetCurrentSchemaQName())
        {
            this.OverrideUserValue = -1;
            base.TargetTable = targetTable;
            base.BaseTable = targetTable.IsTriggerInsertable() ? targetTable : targetTable.GetBaseTable();
            base.InsertColumnMap = columnMap;
            base.InsertCheckColumns = checkColumns;
            base.InsertExpression = insertExpression;
        }

        public StatementInsert(Session session, Table targetTable, int[] columnMap, bool[] checkColumns, QueryExpression queryExpression, int overrid) : base(50, 0x7d4, session.GetCurrentSchemaQName())
        {
            this.OverrideUserValue = -1;
            base.TargetTable = targetTable;
            base.BaseTable = targetTable.IsTriggerInsertable() ? targetTable : targetTable.GetBaseTable();
            base.InsertColumnMap = columnMap;
            base.InsertCheckColumns = checkColumns;
            base.queryExpression = queryExpression;
            this.OverrideUserValue = overrid;
        }

        public RowSetNavigator GetInsertSelectNavigator(Session session)
        {
            SqlType[] columnTypes = base.BaseTable.GetColumnTypes();
            int[] insertColumnMap = base.InsertColumnMap;
            Result result = base.queryExpression.GetResult(session, 0);
            RowSetNavigator navigator = result.InitialiseNavigator();
            SqlType[] typeArray2 = result.MetaData.ColumnTypes;
            RowSetNavigatorClient client = new RowSetNavigatorClient(2);
            while (navigator.HasNext())
            {
                object[] newRowData = base.BaseTable.GetNewRowData(session);
                object[] next = navigator.GetNext();
                for (int i = 0; i < insertColumnMap.Length; i++)
                {
                    int index = insertColumnMap[i];
                    if (index != this.OverrideUserValue)
                    {
                        SqlType type = typeArray2[i];
                        newRowData[index] = columnTypes[index].ConvertToType(session, next[i], type);
                    }
                }
                client.Add(newRowData);
            }
            return client;
        }

        public RowSetNavigator GetInsertValuesNavigator(Session session)
        {
            SqlType[] columnTypes = base.BaseTable.GetColumnTypes();
            Expression[] nodes = base.InsertExpression.nodes;
            RowSetNavigatorClient client = new RowSetNavigatorClient(nodes.Length);
            for (int i = 0; i < nodes.Length; i++)
            {
                Expression[] rowArgs = nodes[i].nodes;
                object[] data = base.GetInsertData(session, columnTypes, rowArgs);
                client.Add(data);
            }
            return client;
        }

        public override Result GetResult(Session session)
        {
            Result result = null;
            RowSetNavigator generatedNavigator = null;
            IPersistentStore rowStore = base.BaseTable.GetRowStore(session);
            if (base.GeneratedIndexes != null)
            {
                result = Result.NewUpdateCountResult(base.generatedResultMetaData, 0);
                generatedNavigator = result.GetChainedResult().GetNavigator();
            }
            if (this.IsSimpleInsert)
            {
                SqlType[] columnTypes = base.BaseTable.GetColumnTypes();
                object[] data = base.GetInsertData(session, columnTypes, base.InsertExpression.nodes[0].nodes);
                return base.InsertSingleRow(session, rowStore, data);
            }
            RowSetNavigator newData = (base.queryExpression == null) ? this.GetInsertValuesNavigator(session) : this.GetInsertSelectNavigator(session);
            if (newData.GetSize() > 0)
            {
                base.InsertRowSet(session, generatedNavigator, newData);
            }
            if (base.BaseTable.TriggerLists[0].Length != 0)
            {
                base.BaseTable.FireTriggers(session, 0, newData);
            }
            if (result == null)
            {
                result = new Result(1, newData.GetSize());
            }
            else
            {
                result.SetUpdateCount(newData.GetSize());
            }
            return result;
        }

        public void InitSimpleInsert()
        {
            this.IsSimpleInsert = ((base.InsertExpression != null) && (base.InsertExpression.nodes.Length == 1)) && (base.UpdatableTableCheck == null);
        }

        public override void SetGeneratedColumnInfo(int generate, ResultMetaData meta)
        {
            if (base.type == 50)
            {
                int identityColumnIndex = base.BaseTable.GetIdentityColumnIndex();
                if (identityColumnIndex != -1)
                {
                    this.GeneratedType = generate;
                    this.GeneratedInputMetaData = meta;
                    if (generate != 1)
                    {
                        if (generate == 2)
                        {
                            return;
                        }
                    }
                    else
                    {
                        base.GeneratedIndexes = new int[] { identityColumnIndex };
                    }
                    base.generatedResultMetaData = ResultMetaData.NewResultMetaData(base.GeneratedIndexes.Length);
                    for (int i = 0; i < base.GeneratedIndexes.Length; i++)
                    {
                        base.generatedResultMetaData.columns[i] = base.BaseTable.GetColumn(base.GeneratedIndexes[i]);
                    }
                    base.generatedResultMetaData.PrepareData();
                    this.IsSimpleInsert = false;
                }
            }
        }
    }
}

