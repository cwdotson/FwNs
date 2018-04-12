namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class StatementSet : StatementDMQL
    {
        public const int TriggerSet = 1;
        public const int SelectInto = 2;
        public const int VariableSet = 3;
        public const int CursorFetch = 4;
        private readonly Expression _expression;
        private readonly int _operationType;
        private readonly SqlType[] _sourceTypes;
        private readonly Expression[] _targets;
        private readonly int[] _variableIndexes;
        private int cursorVariableNameIndex;

        public StatementSet(Session session, Expression[] targets, Expression e, int[] indexes) : base(5, 0x7d7, null)
        {
            this.cursorVariableNameIndex = -1;
            this._operationType = 3;
            this._targets = targets;
            this._expression = e;
            this._variableIndexes = indexes;
            this._sourceTypes = this._expression.GetNodeDataTypes();
            base.isTransactionStatement = false;
        }

        public StatementSet(Session session, Expression[] targets, QueryExpression query, int[] indexes) : base(5, 0x7d7, null)
        {
            this.cursorVariableNameIndex = -1;
            this._operationType = 2;
            base.queryExpression = query;
            this._targets = targets;
            this._variableIndexes = indexes;
            this._sourceTypes = query.GetColumnTypes();
            base.isTransactionStatement = false;
        }

        public StatementSet(Session session, Expression[] targets, int[] indexes, Expression cursorAccessor) : base(5, 0x7d7, null)
        {
            this.cursorVariableNameIndex = -1;
            this._operationType = 4;
            this._targets = targets;
            this._variableIndexes = indexes;
            this._expression = cursorAccessor;
            base.isTransactionStatement = false;
        }

        public StatementSet(Session session, Expression[] targets, Table table, RangeVariable[] rangeVars, int[] indexes, Expression[] colExpressions) : base(5, 0x7d4, session.GetCurrentSchemaQName())
        {
            this.cursorVariableNameIndex = -1;
            this._operationType = 1;
            this._targets = targets;
            base.TargetTable = table;
            base.BaseTable = base.TargetTable.GetBaseTable();
            base.UpdateColumnMap = indexes;
            base.UpdateExpressions = colExpressions;
            base.UpdateCheckColumns = base.TargetTable.GetColumnCheckList(indexes);
            base.TargetRangeVariables = rangeVars;
            base.isTransactionStatement = false;
        }

        public override void CollectTableNamesForRead(OrderedHashSet<QNameManager.QName> set)
        {
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
        }

        public override string Describe(Session session)
        {
            return "";
        }

        public override Result Execute(Session session)
        {
            Result result;
            try
            {
                if (base.Subqueries.Length != 0)
                {
                    base.MaterializeSubQueries(session);
                }
                result = this.GetResult(session);
            }
            catch (Exception exception1)
            {
                result = Result.NewErrorResult(exception1, null);
            }
            if (result.IsError())
            {
                result.GetException().SetStatementType(base.Group, base.type);
            }
            return result;
        }

        private Result ExecuteAssignment(Session session, object[] values)
        {
            for (int i = 0; i < this._variableIndexes.Length; i++)
            {
                object[] routineVariables = new object[0];
                switch (this._targets[i].GetColumn().GetSchemaObjectType())
                {
                    case 0x16:
                        routineVariables = session.sessionContext.RoutineVariables;
                        break;

                    case 0x17:
                        routineVariables = session.sessionContext.RoutineArguments;
                        break;

                    default:
                        throw Error.GetError(0x157d, this._targets[i].GetColumn().GetNameString());
                }
                int index = this._variableIndexes[i];
                if (this._targets[i].GetExprType() == 0x69)
                {
                    routineVariables[index] = ((ExpressionAccessor) this._targets[i]).GetUpdatedArray(session, (object[]) routineVariables[index], values[i], true);
                }
                else
                {
                    routineVariables[index] = values[i];
                }
            }
            return Result.UpdateZeroResult;
        }

        private Result ExecuteSetStatement(Session session)
        {
            Table targetTable = base.TargetTable;
            int[] updateColumnMap = base.UpdateColumnMap;
            Expression[] updateExpressions = base.UpdateExpressions;
            SqlType[] columnTypes = targetTable.GetColumnTypes();
            int rangePosition = base.TargetRangeVariables[1].RangePosition;
            object[] current = session.sessionContext.RangeIterators[rangePosition].GetCurrent();
            object[] sourceArray = StatementDML.GetUpdatedData(session, this._targets, targetTable, updateColumnMap, updateExpressions, columnTypes, current);
            Array.Copy(sourceArray, current, sourceArray.Length);
            return Result.UpdateOneResult;
        }

        public object[] GetExpressionValues(Session session)
        {
            if (this._expression.GetExprType() == 0x19)
            {
                return this._expression.GetRowValue(session);
            }
            if (this._expression.GetExprType() == 0x16)
            {
                object[] singleRowValues = this._expression.subQuery.queryExpression.GetSingleRowValues(session);
                if (singleRowValues == null)
                {
                    return null;
                }
                return singleRowValues;
            }
            return new object[] { this._expression.GetValue(session) };
        }

        public override Result GetResult(Session session)
        {
            switch (this._operationType)
            {
                case 1:
                    return this.ExecuteSetStatement(session);

                case 2:
                {
                    object[] singleRowValues = base.queryExpression.GetSingleRowValues(session);
                    if (singleRowValues != null)
                    {
                        for (int i = 0; i < singleRowValues.Length; i++)
                        {
                            singleRowValues[i] = this._targets[i].GetColumn().GetDataType().ConvertToType(session, singleRowValues[i], this._sourceTypes[i]);
                        }
                        return this.ExecuteAssignment(session, singleRowValues);
                    }
                    return Result.UpdateZeroResult;
                }
                case 3:
                {
                    object[] expressionValues = this.GetExpressionValues(session);
                    if (expressionValues != null)
                    {
                        for (int i = 0; i < expressionValues.Length; i++)
                        {
                            SqlType dataType;
                            if (this._targets[i].GetExprType() == 0x69)
                            {
                                dataType = this._targets[i].GetLeftNode().GetColumn().GetDataType().CollectionBaseType();
                            }
                            else
                            {
                                dataType = this._targets[i].GetColumn().GetDataType();
                            }
                            expressionValues[i] = dataType.ConvertToType(session, expressionValues[i], this._sourceTypes[i]);
                        }
                        return this.ExecuteAssignment(session, expressionValues);
                    }
                    return Result.UpdateZeroResult;
                }
                case 4:
                {
                    Cursor cursor1 = session.sessionContext.RoutineVariables[this.cursorVariableNameIndex] as Cursor;
                    if (cursor1 == null)
                    {
                        throw Error.GetError(0xe10);
                    }
                    object[] values = cursor1.Fetch(session);
                    if (values == null)
                    {
                        return Result.UpdateZeroResult;
                    }
                    return this.ExecuteAssignment(session, values);
                }
            }
            throw Error.RuntimeError(0xc9, "StatementSet");
        }

        public string GetSQL()
        {
            return base.Sql;
        }

        public override SubQuery[] GetSubqueries(Session session)
        {
            OrderedHashSet<SubQuery> set = null;
            if (this._expression != null)
            {
                set = this._expression.CollectAllSubqueries(set);
            }
            if ((set == null) || (set.Size() == 0))
            {
                return SubQuery.EmptySubqueryArray;
            }
            SubQuery[] a = new SubQuery[set.Size()];
            set.ToArray(a);
            ArraySort.Sort<SubQuery>(a, 0, a.Length, a[0]);
            for (int i = 0; i < base.Subqueries.Length; i++)
            {
                a[i].PrepareTable(session);
            }
            return a;
        }

        public override void Resolve(Session session, RangeVariable[] rangeVars)
        {
            base.References = new OrderedHashSet<QNameManager.QName>();
            switch (this._operationType)
            {
                case 1:
                    for (int i = 0; i < base.UpdateExpressions.Length; i++)
                    {
                        base.UpdateExpressions[i].CollectObjectNames(base.References);
                    }
                    return;

                case 2:
                case 3:
                    if (this._expression != null)
                    {
                        this._expression.CollectObjectNames(base.References);
                    }
                    if (base.queryExpression != null)
                    {
                        base.queryExpression.CollectObjectNames(base.References);
                    }
                    return;

                case 4:
                {
                    OrderedHashSet<string> colNames = new OrderedHashSet<string>();
                    string name = this._expression.GetColumn().ColumnName.Name;
                    colNames.Add(name);
                    int[] indexes = new int[colNames.Size()];
                    ColumnSchema[] variables = new ColumnSchema[colNames.Size()];
                    SetVariables(rangeVars, colNames, indexes, variables);
                    this.cursorVariableNameIndex = indexes[0];
                    if (this.cursorVariableNameIndex < 0)
                    {
                        throw Error.GetError(0xe10);
                    }
                    if (variables[0] == null)
                    {
                        throw Error.GetError(0xe10);
                    }
                    Cursor defaultValue = this._expression.GetColumn().GetDefaultValue(session) as Cursor;
                    if (defaultValue == null)
                    {
                        throw Error.GetError(0xe10);
                    }
                    defaultValue.CollectObjectNames(base.References);
                    return;
                }
            }
            throw Error.RuntimeError(0xc9, "StatementSet");
        }

        public static void SetVariables(RangeVariable[] rangeVars, OrderedHashSet<string> colNames, int[] indexes, ColumnSchema[] variables)
        {
            int i = -1;
            for (int j = 0; j < variables.Length; j++)
            {
                string columnName = colNames.Get(j);
                for (int k = 0; k < rangeVars.Length; k++)
                {
                    if (rangeVars[k].IsVariable)
                    {
                        i = rangeVars[k].FindColumn(columnName);
                        if (i > -1)
                        {
                            indexes[j] = i;
                            variables[j] = rangeVars[k].GetColumn(i);
                            break;
                        }
                    }
                }
                if (i == -1)
                {
                    throw Error.GetError(0x157d, columnName);
                }
            }
        }
    }
}

