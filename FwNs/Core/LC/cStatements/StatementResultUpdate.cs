namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class StatementResultUpdate : StatementDML
    {
        public int ActionType;
        private Result _result;
        public SqlType[] Types;

        public StatementResultUpdate()
        {
            base.isTransactionStatement = true;
        }

        public override void CheckAccessRights(Session session)
        {
            switch (this.ActionType)
            {
                case 0x12:
                    session.GetGrantee().CheckDelete(base.BaseTable);
                    return;

                case 50:
                    session.GetGrantee().CheckInsert(base.BaseTable, base.InsertCheckColumns);
                    return;

                case 0x51:
                    session.GetGrantee().CheckUpdate(base.BaseTable, base.UpdateCheckColumns);
                    break;
            }
        }

        public override string Describe(Session session)
        {
            return "";
        }

        public override Result Execute(Session session)
        {
            try
            {
                return this.GetResult(session);
            }
            catch (Exception exception1)
            {
                return Result.NewErrorResult(exception1, null);
            }
        }

        public override Result GetResult(Session session)
        {
            this.CheckAccessRights(session);
            object[] dynamicArguments = session.sessionContext.DynamicArguments;
            IPersistentStore rowStore = base.BaseTable.GetRowStore(session);
            int actionType = this.ActionType;
            switch (actionType)
            {
                case 0x12:
                {
                    Row row = this.GetRow(session, dynamicArguments);
                    if ((row == null) || row.IsDeleted(session, rowStore))
                    {
                        throw Error.GetError(0xe25);
                    }
                    RowSetNavigatorDataChange navigator = new RowSetNavigatorDataChange();
                    navigator.AddRow(row);
                    StatementDML.Delete(session, base.BaseTable, navigator);
                    break;
                }
                case 50:
                {
                    object[] newRowData = base.BaseTable.GetNewRowData(session);
                    for (int i = 0; i < newRowData.Length; i++)
                    {
                        newRowData[base.BaseColumnMap[i]] = dynamicArguments[i];
                    }
                    return base.InsertSingleRow(session, rowStore, newRowData);
                }
                default:
                    if (actionType == 0x51)
                    {
                        Row row = this.GetRow(session, dynamicArguments);
                        if ((row == null) || row.IsDeleted(session, rowStore))
                        {
                            throw Error.GetError(0xe25);
                        }
                        RowSetNavigatorDataChange navigator = new RowSetNavigatorDataChange();
                        object[] data = (object[]) row.RowData.Clone();
                        bool[] newColumnCheckList = base.BaseTable.GetNewColumnCheckList();
                        for (int i = 0; i < base.BaseColumnMap.Length; i++)
                        {
                            if (this.Types[i] != SqlType.SqlAllTypes)
                            {
                                data[base.BaseColumnMap[i]] = dynamicArguments[i];
                                newColumnCheckList[base.BaseColumnMap[i]] = true;
                            }
                        }
                        int[] columnMap = ArrayUtil.BooleanArrayToIntIndexes(newColumnCheckList);
                        navigator.AddRow(session, row, data, base.BaseTable.GetColumnTypes(), columnMap);
                        base.Update(session, base.BaseTable, navigator);
                    }
                    break;
            }
            return Result.UpdateOneResult;
        }

        private Row GetRow(Session session, object[] args)
        {
            int columnCount = this._result.MetaData.GetColumnCount();
            long rowId = Convert.ToInt64(args[columnCount]);
            IPersistentStore rowStore = base.BaseTable.GetRowStore(session);
            Row row = null;
            if ((columnCount + 2) == this._result.MetaData.GetExtendedColumnCount())
            {
                object[] data = ((RowSetNavigatorData) this._result.GetNavigator()).GetData(rowId);
                if (data != null)
                {
                    row = (Row) data[columnCount + 1];
                }
            }
            else
            {
                int key = (int) rowId;
                row = (Row) rowStore.Get(key, false);
            }
            this._result = null;
            return row;
        }

        public void SetRowActionProperties(Result result, int action, Table table, SqlType[] types, int[] columnMap)
        {
            this._result = result;
            this.ActionType = action;
            base.BaseTable = table;
            this.Types = types;
            base.BaseColumnMap = columnMap;
        }
    }
}

