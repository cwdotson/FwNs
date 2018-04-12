namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public class StatementDML : StatementDMQL
    {
        private readonly bool _isTruncate;
        private readonly Expression[] _targets;
        private RangeVariable _checkRangeVariable;
        public Expression UpdatableTableCheck;

        public StatementDML() : base(0x51, 0x7d4, null)
        {
        }

        public StatementDML(int type, int group, QNameManager.QName schemaName) : base(type, group, schemaName)
        {
        }

        public StatementDML(Session session, Table targetTable, RangeVariable[] rangeVars, ParserDQL.CompileContext compileContext, bool restartIdentity, int type) : base(0x13, 0x7d4, session.GetCurrentSchemaQName())
        {
            base.TargetTable = targetTable;
            base.BaseTable = targetTable.GetBaseTable() ?? targetTable;
            base.TargetRangeVariables = rangeVars;
            base.RestartIdentity = restartIdentity;
            base.SetDatabseObjects(session, compileContext);
            if (type == 0x451)
            {
                this._isTruncate = true;
            }
        }

        public StatementDML(Session session, Expression[] targets, Table targetTable, RangeVariable[] rangeVars, int[] updateColumnMap, Expression[] colExpressions, bool[] checkColumns, ParserDQL.CompileContext compileContext) : base(0x52, 0x7d4, session.GetCurrentSchemaQName())
        {
            this._targets = targets;
            base.TargetTable = targetTable;
            base.BaseTable = targetTable.GetBaseTable() ?? targetTable;
            base.UpdateColumnMap = updateColumnMap;
            base.UpdateExpressions = colExpressions;
            base.UpdateCheckColumns = checkColumns;
            base.TargetRangeVariables = rangeVars;
            base.SetDatabseObjects(session, compileContext);
            this.SetupChecks();
        }

        public StatementDML(Session session, Expression[] targets, RangeVariable[] targetRangeVars, int[] insertColMap, int[] updateColMap, bool[] checkColumns, Expression mergeCondition, Expression insertExpr, Expression[] updateExpr, ParserDQL.CompileContext compileContext) : base(0x80, 0x7d4, session.GetCurrentSchemaQName())
        {
            this._targets = targets;
            base.SourceTable = targetRangeVars[0].RangeTable;
            base.TargetTable = targetRangeVars[1].RangeTable;
            base.BaseTable = base.TargetTable.GetBaseTable() ?? base.TargetTable;
            base.InsertCheckColumns = checkColumns;
            base.InsertColumnMap = insertColMap;
            base.UpdateColumnMap = updateColMap;
            base.InsertExpression = insertExpr;
            base.UpdateExpressions = updateExpr;
            base.TargetRangeVariables = targetRangeVars;
            base.condition = mergeCondition;
            base.SetDatabseObjects(session, compileContext);
            this.SetupChecks();
        }

        public override void CollectTableNamesForRead(OrderedHashSet<QNameManager.QName> set)
        {
            if (base.BaseTable.IsView())
            {
                this.GetTriggerTableNames(set, false);
            }
            else if (!base.BaseTable.IsTemp())
            {
                for (int m = 0; m < base.BaseTable.FkConstraints.Length; m++)
                {
                    Constraint constraint = base.BaseTable.FkConstraints[m];
                    if ((base.type == 0x52) || (base.type == 0x80))
                    {
                        if (ArrayUtil.HaveCommonElement(constraint.GetRefColumns(), base.UpdateColumnMap))
                        {
                            set.Add(base.BaseTable.FkConstraints[m].GetMain().GetName());
                        }
                    }
                    else if (base.type == 50)
                    {
                        set.Add(base.BaseTable.FkConstraints[m].GetMain().GetName());
                    }
                }
                if ((base.type == 0x52) || (base.type == 0x80))
                {
                    base.BaseTable.CollectFkReadLocks(base.UpdateColumnMap, set);
                }
                else if (base.type == 0x13)
                {
                    base.BaseTable.CollectFkReadLocks(null, set);
                }
                this.GetTriggerTableNames(set, false);
            }
            for (int i = 0; i < base.RangeVariables.Length; i++)
            {
                Table rangeTable = base.RangeVariables[i].RangeTable;
                QNameManager.QName key = rangeTable.GetName();
                if ((!rangeTable.IsReadOnly() && !rangeTable.IsTemp()) && (key.schema != SqlInvariants.SystemSchemaQname))
                {
                    set.Add(key);
                }
            }
            for (int j = 0; j < base.Subqueries.Length; j++)
            {
                if (base.Subqueries[j].queryExpression != null)
                {
                    base.Subqueries[j].queryExpression.GetBaseTableNames(set);
                }
            }
            for (int k = 0; k < base.Routines.Length; k++)
            {
                set.AddAll(base.Routines[k].GetTableNamesForRead());
            }
        }

        public override void CollectTableNamesForWrite(OrderedHashSet<QNameManager.QName> set)
        {
            if (base.BaseTable.IsView())
            {
                this.GetTriggerTableNames(set, true);
            }
            else if (!base.BaseTable.IsTemp())
            {
                set.Add(base.BaseTable.GetName());
                if ((base.type == 0x52) || (base.type == 0x80))
                {
                    base.BaseTable.CollectFkWriteLocks(base.UpdateColumnMap, set);
                }
                else if (base.type == 0x13)
                {
                    base.BaseTable.CollectFkWriteLocks(null, set);
                }
                this.GetTriggerTableNames(set, true);
            }
        }

        public static int Delete(Session session, Table table, RowSetNavigatorDataChange navigator)
        {
            int size = navigator.GetSize();
            navigator.BeforeFirst();
            if (table.FkMainConstraints.Length != 0)
            {
                HashSet<object> constraintPath = session.sessionContext.GetConstraintPath();
                for (int i = 0; i < size; i++)
                {
                    navigator.Next();
                    Row currentRow = navigator.GetCurrentRow();
                    PerformReferentialActions(session, table, navigator, currentRow, null, null, constraintPath);
                    constraintPath.Clear();
                }
                navigator.BeforeFirst();
            }
            while (navigator.HasNext())
            {
                navigator.Next();
                Row currentRow = navigator.GetCurrentRow();
                object[] currentChangedData = navigator.GetCurrentChangedData();
                int[] currentChangedColumns = navigator.GetCurrentChangedColumns();
                Table table2 = (Table) currentRow.GetTable();
                if (currentChangedData == null)
                {
                    table2.FireTriggers(session, 7, currentRow.RowData, null, null);
                }
                else
                {
                    table2.FireTriggers(session, 8, currentRow.RowData, currentChangedData, currentChangedColumns);
                }
            }
            if (!table.IsView())
            {
                navigator.BeforeFirst();
                bool flag = false;
                for (int i = 0; i < navigator.GetSize(); i++)
                {
                    Row nextRow = navigator.GetNextRow();
                    Table table3 = (Table) nextRow.GetTable();
                    session.AddDeleteAction(table3, nextRow, null);
                    if (navigator.GetCurrentChangedData() != null)
                    {
                        flag = true;
                    }
                }
                navigator.BeforeFirst();
                if (flag)
                {
                    for (int j = 0; j < navigator.GetSize(); j++)
                    {
                        object[] currentChangedData = navigator.GetCurrentChangedData();
                        Table table4 = (Table) navigator.GetNextRow().GetTable();
                        int[] currentChangedColumns = navigator.GetCurrentChangedColumns();
                        IPersistentStore rowStore = table4.GetRowStore(session);
                        if (currentChangedData != null)
                        {
                            table4.InsertSingleRow(session, rowStore, currentChangedData, currentChangedColumns);
                        }
                    }
                    navigator.BeforeFirst();
                }
                OrderedHashSet<Table> set2 = null;
                OrderedHashSet<Table> set3 = null;
                bool flag2 = table.TriggerLists[4].Length > 0;
                if (size != navigator.GetSize())
                {
                    while (navigator.HasNext())
                    {
                        navigator.Next();
                        Row currentRow = navigator.GetCurrentRow();
                        object[] currentChangedData = navigator.GetCurrentChangedData();
                        int[] currentChangedColumns = navigator.GetCurrentChangedColumns();
                        Table table5 = (Table) currentRow.GetTable();
                        if (currentChangedData != null)
                        {
                            PerformIntegrityChecks(session, table5, currentRow.RowData, currentChangedData, currentChangedColumns);
                        }
                        if (table5 != table)
                        {
                            if (currentChangedData == null)
                            {
                                if (table5.TriggerLists[4].Length != 0)
                                {
                                    flag2 = true;
                                }
                                if (set3 == null)
                                {
                                    set3 = new OrderedHashSet<Table>();
                                }
                                set3.Add(table5);
                            }
                            else
                            {
                                if (table5.TriggerLists[5].Length != 0)
                                {
                                    flag2 = true;
                                }
                                if (set2 == null)
                                {
                                    set2 = new OrderedHashSet<Table>();
                                }
                                set2.Add(table5);
                            }
                        }
                    }
                    navigator.BeforeFirst();
                }
                if (flag2)
                {
                    while (navigator.HasNext())
                    {
                        navigator.Next();
                        Row currentRow = navigator.GetCurrentRow();
                        object[] currentChangedData = navigator.GetCurrentChangedData();
                        Table table6 = (Table) currentRow.GetTable();
                        if (currentChangedData == null)
                        {
                            table6.FireTriggers(session, 4, currentRow.RowData, null, null);
                        }
                        else
                        {
                            table6.FireTriggers(session, 5, currentRow.RowData, currentChangedData, null);
                        }
                    }
                    navigator.BeforeFirst();
                }
                table.FireTriggers(session, 1, navigator);
                if (set2 != null)
                {
                    for (int j = 0; j < set2.Size(); j++)
                    {
                        set2.Get(j).FireTriggers(session, 2, navigator);
                    }
                }
                if (set3 != null)
                {
                    for (int j = 0; j < set3.Size(); j++)
                    {
                        set3.Get(j).FireTriggers(session, 1, navigator);
                    }
                }
            }
            return size;
        }

        public Result ExecuteDeleteStatement(Session session)
        {
            IRangeIterator iterator = RangeVariable.GetIterator(session, base.TargetRangeVariables);
            RowSetNavigatorDataChange navigator = new RowSetNavigatorDataChange();
            while (iterator.Next())
            {
                Row currentRow = iterator.GetCurrentRow();
                navigator.AddRow(currentRow);
            }
            iterator.Release();
            if (navigator.GetSize() <= 0)
            {
                return Result.UpdateZeroResult;
            }
            int count = Delete(session, base.BaseTable, navigator);
            if (count == 1)
            {
                return Result.UpdateOneResult;
            }
            return new Result(1, count);
        }

        public Result ExecuteDeleteTruncateStatement(Session session)
        {
            IPersistentStore rowStore = base.TargetTable.GetRowStore(session);
            IRowIterator iterator = base.TargetTable.GetPrimaryIndex().FirstRow(rowStore);
            try
            {
                while (iterator.HasNext())
                {
                    Row nextRow = iterator.GetNextRow();
                    session.AddDeleteAction((Table) nextRow.GetTable(), nextRow, null);
                }
                if (base.RestartIdentity && (base.TargetTable.IdentitySequence != null))
                {
                    base.TargetTable.IdentitySequence.Reset();
                }
            }
            finally
            {
                iterator.Release();
            }
            return Result.UpdateOneResult;
        }

        public Result ExecuteMergeStatement(Session session)
        {
            SqlType[] columnTypes = base.BaseTable.GetColumnTypes();
            Result result = null;
            RowSetNavigator generatedNavigator = null;
            if (base.GeneratedIndexes != null)
            {
                result = Result.NewUpdateCountResult(base.generatedResultMetaData, 0);
                generatedNavigator = result.GetChainedResult().GetNavigator();
            }
            int count = 0;
            RowSetNavigatorClient newData = new RowSetNavigatorClient(8);
            RowSetNavigatorDataChange navigator = new RowSetNavigatorDataChange();
            RangeVariable[] targetRangeVariables = base.TargetRangeVariables;
            IRangeIterator[] iteratorArray = new IRangeIterator[targetRangeVariables.Length];
            for (int i = 0; i < targetRangeVariables.Length; i++)
            {
                iteratorArray[i] = targetRangeVariables[i].GetIterator(session);
            }
            int index = 0;
            while (index >= 0)
            {
                IRangeIterator iterator = iteratorArray[index];
                bool flag = iterator.IsBeforeFirst();
                if (iterator.Next())
                {
                    if (index < (targetRangeVariables.Length - 1))
                    {
                        index++;
                        continue;
                    }
                    if (base.UpdateExpressions.Length == 0)
                    {
                        continue;
                    }
                    Row currentRow = iterator.GetCurrentRow();
                    object[] data = GetUpdatedData(session, this._targets, base.BaseTable, base.UpdateColumnMap, base.UpdateExpressions, columnTypes, currentRow.RowData);
                    try
                    {
                        navigator.AddRow(session, currentRow, data, columnTypes, base.UpdateColumnMap);
                        continue;
                    }
                    catch (CoreException)
                    {
                        for (int k = 0; k < targetRangeVariables.Length; k++)
                        {
                            iteratorArray[k].Reset();
                        }
                        throw Error.GetError(0xc81);
                    }
                }
                if (((index == 1) & flag) && (base.InsertExpression != null))
                {
                    object[] data = this.GetInsertData(session, columnTypes, base.InsertExpression.nodes[0].nodes);
                    if (data != null)
                    {
                        newData.Add(data);
                    }
                }
                iterator.Reset();
                index--;
            }
            for (int j = 0; j < targetRangeVariables.Length; j++)
            {
                iteratorArray[j].Reset();
            }
            if (base.UpdateExpressions.Length != 0)
            {
                count = this.Update(session, base.BaseTable, navigator);
            }
            if (newData.GetSize() > 0)
            {
                this.InsertRowSet(session, generatedNavigator, newData);
                count += newData.GetSize();
            }
            if ((base.InsertExpression != null) && (base.BaseTable.TriggerLists[0].Length != 0))
            {
                base.BaseTable.FireTriggers(session, 0, newData);
            }
            if (result != null)
            {
                result.SetUpdateCount(count);
                return result;
            }
            if (count == 1)
            {
                return Result.UpdateOneResult;
            }
            return new Result(1, count);
        }

        public Result ExecuteUpdateStatement(Session session)
        {
            Expression[] updateExpressions = base.UpdateExpressions;
            RowSetNavigatorDataChange navigator = new RowSetNavigatorDataChange();
            SqlType[] columnTypes = base.BaseTable.GetColumnTypes();
            IRangeIterator iterator = RangeVariable.GetIterator(session, base.TargetRangeVariables);
            while (iterator.Next())
            {
                session.sessionData.StartRowProcessing();
                Row currentRow = iterator.GetCurrentRow();
                object[] rowData = currentRow.RowData;
                object[] data = GetUpdatedData(session, this._targets, base.BaseTable, base.UpdateColumnMap, updateExpressions, columnTypes, rowData);
                if (this.UpdatableTableCheck != null)
                {
                    iterator.SetCurrent(data);
                    if (!this.UpdatableTableCheck.TestCondition(session))
                    {
                        iterator.Release();
                        throw Error.GetError(0x1644);
                    }
                }
                navigator.AddRow(session, currentRow, data, columnTypes, base.UpdateColumnMap);
            }
            iterator.Release();
            navigator.BeforeFirst();
            int count = this.Update(session, base.BaseTable, navigator);
            switch (count)
            {
                case 1:
                    return Result.UpdateOneResult;

                case 0:
                    return Result.UpdateZeroResult;
            }
            return new Result(1, count);
        }

        public object[] GetInsertData(Session session, SqlType[] colTypes, Expression[] rowArgs)
        {
            object[] newRowData = base.BaseTable.GetNewRowData(session);
            session.sessionData.StartRowProcessing();
            for (int i = 0; i < rowArgs.Length; i++)
            {
                Expression expression = rowArgs[i];
                int index = base.InsertColumnMap[i];
                if (expression.OpType == 4)
                {
                    if ((base.BaseTable.IdentityColumn != index) && (base.BaseTable.ColDefaults[index] != null))
                    {
                        newRowData[index] = base.BaseTable.ColDefaults[index].GetValue(session);
                    }
                }
                else
                {
                    object a = expression.GetValue(session);
                    SqlType type = colTypes[index];
                    if (colTypes[index] != expression.DataType)
                    {
                        a = type.ConvertToType(session, a, expression.DataType);
                    }
                    newRowData[index] = a;
                }
            }
            return newRowData;
        }

        public override Result GetResult(Session session)
        {
            int type = base.type;
            switch (type)
            {
                case 0x13:
                    if (this._isTruncate)
                    {
                        return this.ExecuteDeleteTruncateStatement(session);
                    }
                    return this.ExecuteDeleteStatement(session);

                case 0x52:
                    return this.ExecuteUpdateStatement(session);
            }
            if (type != 0x80)
            {
                throw Error.RuntimeError(0xc9, "StatementDML");
            }
            return this.ExecuteMergeStatement(session);
        }

        public void GetTriggerTableNames(OrderedHashSet<QNameManager.QName> set, bool write)
        {
            int index = 0;
            while (index < base.BaseTable.TriggerList.Length)
            {
                TriggerDef def = base.BaseTable.TriggerList[index];
                int type = base.type;
                if (type <= 50)
                {
                    if (type == 0x13)
                    {
                        if (def.GetStatementType() != 0x13)
                        {
                            goto Label_0070;
                        }
                    }
                    else
                    {
                        if (type != 50)
                        {
                            goto Label_00A9;
                        }
                        if (def.GetStatementType() != 50)
                        {
                            goto Label_0070;
                        }
                    }
                    goto Label_0076;
                }
                if (type != 0x52)
                {
                    if (type != 0x80)
                    {
                        goto Label_00A9;
                    }
                    if ((def.GetStatementType() != 50) && (def.GetStatementType() != 0x52))
                    {
                        goto Label_0070;
                    }
                    goto Label_0076;
                }
                if (def.GetStatementType() == 0x52)
                {
                    goto Label_0076;
                }
            Label_0070:
                index++;
                continue;
            Label_0076:
                if (def.routine != null)
                {
                    if (write)
                    {
                        set.AddAll(def.routine.GetTableNamesForWrite());
                    }
                    else
                    {
                        set.AddAll(def.routine.GetTableNamesForRead());
                    }
                }
                goto Label_0070;
            Label_00A9:
                throw Error.RuntimeError(0xc9, "StatementDML");
            }
        }

        public static object[] GetUpdatedData(Session session, Expression[] targets, Table targetTable, int[] columnMap, Expression[] colExpressions, SqlType[] colTypes, object[] oldData)
        {
            object[] emptyRowData = targetTable.GetEmptyRowData();
            Array.Copy(oldData, 0, emptyRowData, 0, emptyRowData.Length);
            int index = 0;
            int num2 = 0;
            while (index < columnMap.Length)
            {
                Expression expression = colExpressions[num2++];
                if (expression.GetExprType() == 0x19)
                {
                    object[] rowValue = expression.GetRowValue(session);
                    int num3 = 0;
                    while (num3 < rowValue.Length)
                    {
                        int num4 = columnMap[index];
                        Expression expression2 = expression.nodes[num3];
                        if (((targetTable.IdentityColumn != num4) || (expression2.GetExprType() != 1)) || (expression2.ValueData != null))
                        {
                            if (expression2.GetExprType() == 4)
                            {
                                if (targetTable.IdentityColumn != num4)
                                {
                                    emptyRowData[num4] = targetTable.ColDefaults[num4].GetValue(session);
                                }
                            }
                            else
                            {
                                emptyRowData[num4] = colTypes[num4].ConvertToType(session, rowValue[num3], expression2.DataType);
                            }
                        }
                        num3++;
                        index++;
                    }
                }
                else
                {
                    if (expression.GetExprType() == 0x16)
                    {
                        object[] rowValue = expression.GetRowValue(session);
                        if (rowValue.Length == 0)
                        {
                            index = columnMap.Length;
                        }
                        int num5 = 0;
                        while (num5 < rowValue.Length)
                        {
                            int num6 = columnMap[index];
                            SqlType type = expression.subQuery.queryExpression.GetMetaData().ColumnTypes[num5];
                            emptyRowData[num6] = colTypes[num6].ConvertToType(session, rowValue[num5], type);
                            num5++;
                            index++;
                        }
                        continue;
                    }
                    int num7 = columnMap[index];
                    if (expression.GetExprType() == 4)
                    {
                        if (targetTable.IdentityColumn == num7)
                        {
                            index++;
                        }
                        else
                        {
                            emptyRowData[num7] = targetTable.ColDefaults[num7].GetValue(session);
                            index++;
                        }
                        continue;
                    }
                    object obj2 = expression.GetValue(session);
                    if (targets[index].GetExprType() == 0x69)
                    {
                        emptyRowData[num7] = ((ExpressionAccessor) targets[index]).GetUpdatedArray(session, (object[]) emptyRowData[num7], obj2, true);
                    }
                    else
                    {
                        emptyRowData[num7] = colTypes[num7].ConvertToType(session, obj2, expression.DataType);
                    }
                    index++;
                }
            }
            return emptyRowData;
        }

        public void InsertRowSet(Session session, RowSetNavigator generatedNavigator, RowSetNavigator newData)
        {
            IPersistentStore rowStore = base.BaseTable.GetRowStore(session);
            IRangeIterator iterator = null;
            if (this.UpdatableTableCheck != null)
            {
                iterator = this._checkRangeVariable.GetIterator(session);
            }
            newData.BeforeFirst();
            if (base.BaseTable.TriggerLists[6].Length != 0)
            {
                while (newData.HasNext())
                {
                    object[] next = newData.GetNext();
                    base.BaseTable.FireTriggers(session, 6, null, next, null);
                }
                newData.BeforeFirst();
            }
            while (newData.HasNext())
            {
                object[] next = newData.GetNext();
                base.BaseTable.InsertSingleRow(session, rowStore, next, null);
                if (iterator != null)
                {
                    iterator.SetCurrent(next);
                    if (!this.UpdatableTableCheck.TestCondition(session))
                    {
                        throw Error.GetError(0x1644);
                    }
                }
                if (generatedNavigator != null)
                {
                    object[] generatedColumns = base.GetGeneratedColumns(next);
                    generatedNavigator.Add(generatedColumns);
                }
            }
            newData.BeforeFirst();
            while (newData.HasNext())
            {
                object[] next = newData.GetNext();
                PerformIntegrityChecks(session, base.BaseTable, null, next, null);
            }
            newData.BeforeFirst();
            if (base.BaseTable.TriggerLists[3].Length != 0)
            {
                while (newData.HasNext())
                {
                    object[] next = newData.GetNext();
                    base.BaseTable.FireTriggers(session, 3, null, next, null);
                }
                newData.BeforeFirst();
            }
        }

        public Result InsertSingleRow(Session session, IPersistentStore store, object[] data)
        {
            if (base.BaseTable.TriggerLists[6].Length != 0)
            {
                base.BaseTable.FireTriggers(session, 6, null, data, null);
            }
            base.BaseTable.InsertSingleRow(session, store, data, null);
            PerformIntegrityChecks(session, base.BaseTable, null, data, null);
            if (session.database.IsReferentialIntegrity())
            {
                int index = 0;
                int length = base.BaseTable.FkConstraints.Length;
                while (index < length)
                {
                    base.BaseTable.FkConstraints[index].CheckInsert(session, base.BaseTable, data, true);
                    index++;
                }
            }
            if (base.BaseTable.TriggerLists[3].Length != 0)
            {
                base.BaseTable.FireTriggers(session, 3, null, data, null);
            }
            if (base.BaseTable.TriggerLists[0].Length != 0)
            {
                base.BaseTable.FireTriggers(session, 0, null);
            }
            return Result.UpdateOneResult;
        }

        public static void PerformIntegrityChecks(Session session, Table table, object[] oldData, object[] newData, int[] updatedColumns)
        {
            if (newData != null)
            {
                int index = 0;
                int length = table.CheckConstraints.Length;
                while (index < length)
                {
                    table.CheckConstraints[index].CheckInsert(session, table, newData, oldData == null);
                    index++;
                }
                if (session.database.IsReferentialIntegrity())
                {
                    int num3 = 0;
                    int num4 = table.FkConstraints.Length;
                    while (num3 < num4)
                    {
                        bool flag = oldData == null;
                        Constraint constraint = table.FkConstraints[num3];
                        if (!flag)
                        {
                            flag = ArrayUtil.HaveCommonElement(constraint.GetRefColumns(), updatedColumns);
                        }
                        if (flag)
                        {
                            constraint.CheckInsert(session, table, newData, oldData == null);
                        }
                        num3++;
                    }
                }
            }
        }

        private static void PerformReferentialActions(Session session, Table table, RowSetNavigatorDataChange navigator, Row row, object[] data, int[] changedCols, HashSet<object> path)
        {
            if (session.database.IsReferentialIntegrity())
            {
                bool flag = data == null;
                int index = 0;
                int length = table.FkMainConstraints.Length;
                while (index < length)
                {
                    Constraint item = table.FkMainConstraints[index];
                    int num3 = flag ? item.Core.DeleteAction : item.Core.UpdateAction;
                    if (flag || (ArrayUtil.HaveCommonElement(changedCols, item.Core.MainCols) && (item.Core.MainIndex.CompareRowNonUnique(session, row.RowData, data, item.Core.MainCols) != 0)))
                    {
                        IRowIterator iterator = item.FindFkRef(session, row.RowData);
                        if (iterator.HasNext())
                        {
                            while (iterator.HasNext())
                            {
                                object[] emptyRowData;
                                int num6;
                                int num7;
                                ColumnSchema schema;
                                Row nextRow = iterator.GetNextRow();
                                if (item.Core.RefIndex.CompareRowNonUnique(session, nextRow.RowData, row.RowData, item.Core.MainCols) != 0)
                                {
                                    break;
                                }
                                if (flag && (nextRow.GetId() == row.GetId()))
                                {
                                    iterator.Release();
                                }
                                else
                                {
                                    switch (num3)
                                    {
                                        case 0:
                                            if (!flag)
                                            {
                                                goto Label_0151;
                                            }
                                            if (navigator.AddRow(nextRow))
                                            {
                                                PerformReferentialActions(session, item.Core.RefTable, navigator, nextRow, null, null, path);
                                            }
                                            break;

                                        case 1:
                                        case 3:
                                            if (!navigator.ContainsDeletedRow(nextRow))
                                            {
                                                int code = (item.Core.DeleteAction == 3) ? 8 : 0xdad;
                                                string[] add = new string[] { item.Core.RefName.Name, item.Core.RefTable.GetName().Name };
                                                iterator.Release();
                                                throw Error.GetError(null, code, 2, add);
                                            }
                                            break;

                                        case 2:
                                            emptyRowData = item.Core.RefTable.GetEmptyRowData();
                                            Array.Copy(nextRow.RowData, 0, emptyRowData, 0, emptyRowData.Length);
                                            num6 = 0;
                                            goto Label_0268;

                                        case 4:
                                            emptyRowData = item.Core.RefTable.GetEmptyRowData();
                                            Array.Copy(nextRow.RowData, 0, emptyRowData, 0, emptyRowData.Length);
                                            num7 = 0;
                                            goto Label_02E5;
                                    }
                                }
                                continue;
                            Label_0151:
                                emptyRowData = item.Core.RefTable.GetEmptyRowData();
                                Array.Copy(nextRow.RowData, 0, emptyRowData, 0, emptyRowData.Length);
                                for (int i = 0; i < item.Core.RefCols.Length; i++)
                                {
                                    emptyRowData[item.Core.RefCols[i]] = data[item.Core.MainCols[i]];
                                }
                                goto Label_02F6;
                            Label_0250:
                                emptyRowData[item.Core.RefCols[num6]] = null;
                                num6++;
                            Label_0268:
                                if (num6 < item.Core.RefCols.Length)
                                {
                                    goto Label_0250;
                                }
                                goto Label_02F6;
                            Label_02A6:
                                schema = item.Core.RefTable.GetColumn(item.Core.RefCols[num7]);
                                emptyRowData[item.Core.RefCols[num7]] = schema.GetDefaultValue(session);
                                num7++;
                            Label_02E5:
                                if (num7 < item.Core.RefCols.Length)
                                {
                                    goto Label_02A6;
                                }
                            Label_02F6:
                                emptyRowData = navigator.AddRow(session, nextRow, emptyRowData, table.GetColumnTypes(), item.Core.RefCols);
                                if (path.Add(item))
                                {
                                    PerformReferentialActions(session, item.Core.RefTable, navigator, nextRow, emptyRowData, item.Core.RefCols, path);
                                    path.Remove(item);
                                }
                            }
                            iterator.Release();
                        }
                    }
                    index++;
                }
            }
        }

        public void SetupChecks()
        {
            if (base.TargetTable != base.BaseTable)
            {
                QuerySpecification mainSelect = base.TargetTable.GetQueryExpression().GetMainSelect();
                this.UpdatableTableCheck = mainSelect.CheckQueryCondition;
                this._checkRangeVariable = mainSelect.RangeVariables[0];
            }
        }

        public int Update(Session session, Table table, RowSetNavigatorDataChange navigator)
        {
            int size = navigator.GetSize();
            for (int i = 0; i < size; i++)
            {
                navigator.Next();
                object[] currentChangedData = navigator.GetCurrentChangedData();
                table.SetIdentityColumn(session, currentChangedData);
                table.SetGeneratedColumns(session, currentChangedData);
            }
            navigator.BeforeFirst();
            if (table.FkMainConstraints.Length != 0)
            {
                HashSet<object> constraintPath = session.sessionContext.GetConstraintPath();
                for (int k = 0; k < size; k++)
                {
                    Row nextRow = navigator.GetNextRow();
                    object[] currentChangedData = navigator.GetCurrentChangedData();
                    PerformReferentialActions(session, table, navigator, nextRow, currentChangedData, base.UpdateColumnMap, constraintPath);
                    constraintPath.Clear();
                }
                navigator.BeforeFirst();
            }
            for (int j = 0; j < navigator.GetSize(); j++)
            {
                Row nextRow = navigator.GetNextRow();
                object[] currentChangedData = navigator.GetCurrentChangedData();
                int[] currentChangedColumns = navigator.GetCurrentChangedColumns();
                Table table2 = (Table) nextRow.GetTable();
                if (table2.TriggerLists[8].Length != 0)
                {
                    table2.FireTriggers(session, 8, nextRow.RowData, currentChangedData, currentChangedColumns);
                    table2.EnforceRowConstraints(session, currentChangedData);
                }
            }
            if (!table.IsView())
            {
                navigator.BeforeFirst();
                for (int k = 0; k < navigator.GetSize(); k++)
                {
                    Row nextRow = navigator.GetNextRow();
                    Table table3 = (Table) nextRow.GetTable();
                    int[] currentChangedColumns = navigator.GetCurrentChangedColumns();
                    session.AddDeleteAction(table3, nextRow, currentChangedColumns);
                }
                navigator.BeforeFirst();
                for (int m = 0; m < navigator.GetSize(); m++)
                {
                    object[] currentChangedData = navigator.GetCurrentChangedData();
                    Table table4 = (Table) navigator.GetNextRow().GetTable();
                    int[] currentChangedColumns = navigator.GetCurrentChangedColumns();
                    IPersistentStore rowStore = table4.GetRowStore(session);
                    if (currentChangedData != null)
                    {
                        table4.InsertSingleRow(session, rowStore, currentChangedData, currentChangedColumns);
                    }
                }
                navigator.BeforeFirst();
                OrderedHashSet<Table> set2 = null;
                bool flag = table.TriggerLists[5].Length > 0;
                for (int n = 0; n < navigator.GetSize(); n++)
                {
                    Row nextRow = navigator.GetNextRow();
                    Table table5 = (Table) nextRow.GetTable();
                    object[] currentChangedData = navigator.GetCurrentChangedData();
                    int[] currentChangedColumns = navigator.GetCurrentChangedColumns();
                    PerformIntegrityChecks(session, table5, nextRow.RowData, currentChangedData, currentChangedColumns);
                    if (table5 != table)
                    {
                        if (set2 == null)
                        {
                            set2 = new OrderedHashSet<Table>();
                        }
                        set2.Add(table5);
                        if (table5.TriggerLists[5].Length != 0)
                        {
                            flag = true;
                        }
                    }
                }
                navigator.BeforeFirst();
                if (flag)
                {
                    for (int num9 = 0; num9 < navigator.GetSize(); num9++)
                    {
                        Row nextRow = navigator.GetNextRow();
                        object[] currentChangedData = navigator.GetCurrentChangedData();
                        int[] currentChangedColumns = navigator.GetCurrentChangedColumns();
                        ((Table) nextRow.GetTable()).FireTriggers(session, 5, nextRow.RowData, currentChangedData, currentChangedColumns);
                    }
                    navigator.BeforeFirst();
                }
                base.BaseTable.FireTriggers(session, 2, navigator);
                if (set2 != null)
                {
                    for (int num10 = 0; num10 < set2.Size(); num10++)
                    {
                        set2.Get(num10).FireTriggers(session, 2, navigator);
                    }
                }
            }
            return size;
        }
    }
}

