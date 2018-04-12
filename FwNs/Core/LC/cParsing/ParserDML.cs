namespace FwNs.Core.LC.cParsing
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public class ParserDML : ParserDQL
    {
        public ParserDML(Session session, Scanner t) : base(session, t)
        {
        }

        public StatementDMQL CompileCallStatement(RangeVariable[] outerRanges, bool isStrictlyProcedure)
        {
            Expression[] expressionArray;
            Expression expression2;
            base.Read();
            if (!base.IsIdentifier())
            {
                goto Label_01E4;
            }
            base.CheckValidCatalogName(base.token.NamePrePrefix);
            RoutineSchema schema = (RoutineSchema) base.database.schemaManager.FindSchemaObject(base.token.TokenString, base.session.GetSchemaName(base.token.NamePrefix), 0x11);
            if (schema == null)
            {
                goto Label_01E4;
            }
            base.Read();
            List<Expression> list = new List<Expression>();
            base.ReadThis(0x2b7);
            if (base.token.TokenType == 0x2aa)
            {
                base.Read();
                goto Label_00C7;
            }
        Label_0092:
            expression2 = base.XreadValueExpression();
            list.Add(expression2);
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_0092;
            }
            base.ReadThis(0x2aa);
        Label_00C7:
            expressionArray = list.ToArray();
            Routine specificRoutine = schema.GetSpecificRoutine(expressionArray.Length);
            base.compileContext.AddProcedureCall(specificRoutine);
            List<Expression> unresolvedSet = null;
            for (int i = 0; i < expressionArray.Length; i++)
            {
                Expression expression3 = expressionArray[i];
                if (expression3.IsUnresolvedParam())
                {
                    expression3.SetAttributesAsColumn(specificRoutine.GetParameter(i), specificRoutine.GetParameter(i).IsWriteable());
                }
                else
                {
                    int parameterMode = specificRoutine.GetParameter(i).GetParameterMode();
                    unresolvedSet = expression3.ResolveColumnReferences(outerRanges, unresolvedSet);
                    if (parameterMode != 1)
                    {
                        if (expression3.GetExprType() == 7)
                        {
                            ExpressionColumn column = (ExpressionColumn) expression3;
                            if ((column.GetParameterMode() == parameterMode) || (column.GetParameterMode() == 2))
                            {
                                goto Label_017E;
                            }
                        }
                        if (expression3.GetExprType() != 6)
                        {
                            throw Error.GetError(0x15e3);
                        }
                    }
                Label_017E:;
                }
            }
            ExpressionColumn.CheckColumnsResolved(unresolvedSet);
            for (int j = 0; j < expressionArray.Length; j++)
            {
                expressionArray[j].ResolveTypes(base.session, null);
            }
            StatementProcedure procedure1 = new StatementProcedure(base.session, specificRoutine, expressionArray);
            procedure1.SetDatabseObjects(base.session, base.compileContext);
            procedure1.CheckAccessRights(base.session);
            return procedure1;
        Label_01E4:
            if (isStrictlyProcedure)
            {
                throw Error.GetError(0x157d, base.token.TokenString);
            }
            Expression expression = base.XreadValueExpression();
            ExpressionColumn.CheckColumnsResolved(expression.ResolveColumnReferences(outerRanges, null));
            expression.ResolveTypes(base.session, null);
            StatementProcedure procedure2 = new StatementProcedure(base.session, expression);
            procedure2.SetDatabseObjects(base.session, base.compileContext);
            procedure2.CheckAccessRights(base.session);
            return procedure2;
        }

        public StatementDMQL CompileDeleteStatement(RangeVariable[] outerRanges)
        {
            Expression expression = null;
            int num2;
            bool flag = false;
            bool restartIdentity = false;
            int tokenType = base.token.TokenType;
            if (tokenType != 0x4e)
            {
                if (tokenType != 0x125)
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                base.ReadThis(0x114);
                flag = true;
                num2 = 0x451;
            }
            else
            {
                base.Read();
                base.ReadThis(0x72);
                num2 = 0x13;
            }
            RangeVariable[] rangeVars = new RangeVariable[] { base.ReadSimpleRangeVariable(num2) };
            Table targetTable = rangeVars[0].GetTable();
            Table baseTable = targetTable.GetBaseTable();
            if (flag)
            {
                if (targetTable != baseTable)
                {
                    throw Error.GetError(0x15a9);
                }
                if (targetTable.IsTriggerDeletable())
                {
                    throw Error.GetError(0x15a9);
                }
                switch (base.token.TokenType)
                {
                    case 0x178:
                        base.Read();
                        base.ReadThis(0x7f);
                        break;

                    case 0x1e3:
                        base.Read();
                        base.ReadThis(0x7f);
                        restartIdentity = true;
                        break;
                }
                if (targetTable.FkMainConstraints.Length != 0)
                {
                    throw Error.GetError(8);
                }
            }
            if (!flag && (base.token.TokenType == 0x13a))
            {
                base.Read();
                expression = base.XreadBooleanValueExpression();
                List<Expression> sourceSet = expression.ResolveColumnReferences(outerRanges, null);
                if (outerRanges.Length != 0)
                {
                    sourceSet = Expression.ResolveColumnSet(outerRanges, outerRanges.Length, sourceSet, null);
                }
                ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVars, rangeVars.Length, sourceSet, null));
                expression.ResolveTypes(base.session, null);
                if (expression.IsUnresolvedParam())
                {
                    expression.DataType = SqlType.SqlBoolean;
                }
                if (expression.GetDataType() != SqlType.SqlBoolean)
                {
                    throw Error.GetError(0x15c0);
                }
            }
            if (targetTable != baseTable)
            {
                QuerySpecification mainSelect = targetTable.GetQueryExpression().GetMainSelect();
                RangeVariable[] newRanges = (RangeVariable[]) mainSelect.RangeVariables.Clone();
                newRanges[0] = mainSelect.RangeVariables[0].Duplicate();
                Expression[] list = new Expression[mainSelect.IndexLimitData];
                for (int i = 0; i < mainSelect.IndexLimitData; i++)
                {
                    Expression expression3 = mainSelect.ExprColumns[i].Duplicate();
                    list[i] = expression3;
                    expression3.ReplaceRangeVariables(mainSelect.RangeVariables, newRanges);
                }
                Expression queryCondition = mainSelect.QueryCondition;
                if (queryCondition != null)
                {
                    queryCondition = queryCondition.Duplicate();
                    queryCondition.ReplaceRangeVariables(rangeVars, newRanges);
                }
                if (expression != null)
                {
                    expression = expression.ReplaceColumnReferences(rangeVars[0], list);
                }
                rangeVars = newRanges;
                expression = ExpressionLogical.AndExpressions(queryCondition, expression);
            }
            if (expression != null)
            {
                rangeVars[0].AddJoinCondition(expression);
                RangeVariableResolver resolver1 = new RangeVariableResolver(rangeVars, null, base.compileContext);
                resolver1.ProcessConditions(base.session);
                rangeVars = resolver1.RangeVariables;
            }
            StatementDML tdml1 = new StatementDML(base.session, targetTable, rangeVars, base.compileContext, restartIdentity, num2);
            tdml1.CheckAccessRights(base.session);
            return tdml1;
        }

        public StatementDMQL CompileInsertStatement(RangeVariable[] outerRanges)
        {
            bool[] columnCheckList;
            base.Read();
            base.ReadThis(0x8b);
            Table targetTable = base.ReadTableName();
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            int[] columnMap = targetTable.GetColumnMap();
            int columnCount = targetTable.GetColumnCount();
            int position = base.GetPosition();
            if ((!targetTable.IsInsertable() && !targetTable.IsTriggerInsertable()) && !base.session.IsProcessingScript())
            {
                throw Error.GetError(0x15a9);
            }
            Table table2 = targetTable.IsTriggerInsertable() ? targetTable : targetTable.GetBaseTable();
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x114)
            {
                switch (tokenType)
                {
                    case 0xf9:
                    case 0x114:
                        goto Label_046B;

                    case 0x4d:
                    {
                        base.Read();
                        base.ReadThis(0x132);
                        Expression insertExpression = new Expression(0x19, new Expression[0]);
                        Expression[] list = new Expression[] { insertExpression };
                        insertExpression = new Expression(0x1a, list);
                        columnCheckList = targetTable.GetNewColumnCheckList();
                        for (int j = 0; j < targetTable.ColDefaults.Length; j++)
                        {
                            if (((targetTable.ColDefaults[j] == null) && (targetTable.IdentityColumn != j)) && !targetTable.GetColumn(j).IsGenerated())
                            {
                                throw Error.GetError(0x15a8);
                            }
                        }
                        StatementInsert insert2 = new StatementInsert(base.session, targetTable, columnMap, insertExpression, columnCheckList);
                        insert2.SetDatabseObjects(base.session, base.compileContext);
                        insert2.CheckAccessRights(base.session);
                        insert2.SetupChecks();
                        insert2.InitSimpleInsert();
                        return insert2;
                    }
                }
                goto Label_0464;
            }
            if (tokenType <= 0x13d)
            {
                switch (tokenType)
                {
                    case 0x132:
                        goto Label_01E7;

                    case 0x13d:
                        goto Label_046B;
                }
                goto Label_0464;
            }
            if (tokenType != 460)
            {
                if (tokenType != 0x2b7)
                {
                    goto Label_0464;
                }
                if (base.ReadOpenBrackets() != 1)
                {
                    this.Rewind(position);
                    goto Label_046B;
                }
                bool flag4 = false;
                switch (base.token.TokenType)
                {
                    case 0xf9:
                    case 0x114:
                    case 0x13d:
                        this.Rewind(position);
                        flag4 = true;
                        break;
                }
                if (flag4)
                {
                    goto Label_046B;
                }
                OrderedHashSet<string> columns = new OrderedHashSet<string>();
                base.ReadSimpleColumnNames(columns, targetTable);
                base.ReadThis(0x2aa);
                columnCount = columns.Size();
                columnMap = targetTable.GetColumnIndexes(columns);
                if ((base.token.TokenType != 0x132) && (base.token.TokenType != 460))
                {
                    goto Label_046B;
                }
            }
            if (base.token.TokenType == 460)
            {
                base.Read();
                if (base.token.TokenType == 0x12f)
                {
                    base.Read();
                    flag = true;
                }
                else if (base.token.TokenType == 0x112)
                {
                    base.Read();
                    flag2 = true;
                }
                else
                {
                    base.UnexpectedToken();
                }
                base.ReadThis(0x131);
                if (base.token.TokenType != 0x132)
                {
                    goto Label_046B;
                }
            }
        Label_01E7:
            base.Read();
            columnCheckList = targetTable.GetColumnCheckList(columnMap);
            Expression tableExpression = base.XreadContextuallyTypedTable(columnCount);
            ExpressionColumn.CheckColumnsResolved(tableExpression.ResolveColumnReferences(outerRanges, null));
            tableExpression.ResolveTypes(base.session, null);
            SetParameterTypes(tableExpression, targetTable, columnMap);
            if (targetTable != table2)
            {
                int[] numArray2 = new int[columnMap.Length];
                ArrayUtil.ProjectRow(targetTable.GetBaseTableColumnMap(), columnMap, numArray2);
                columnMap = numArray2;
            }
            Expression[] nodes = tableExpression.nodes;
            for (int i = 0; i < nodes.Length; i++)
            {
                Expression[] expressionArray2 = nodes[i].nodes;
                for (int j = 0; j < expressionArray2.Length; j++)
                {
                    Expression expression3 = expressionArray2[j];
                    ColumnSchema column = table2.GetColumn(columnMap[j]);
                    if (column.IsIdentity())
                    {
                        flag3 = true;
                        if (expression3.GetExprType() != 4)
                        {
                            if ((column.GetIdentitySequence().IsAlways() && !flag) && !flag2)
                            {
                                throw Error.GetError(0x15a7);
                            }
                            if (flag)
                            {
                                expressionArray2[j] = new ExpressionColumn(4);
                            }
                        }
                    }
                    else if (!column.HasDefault)
                    {
                        if (column.IsGenerated())
                        {
                            if (expression3.GetExprType() != 4)
                            {
                                throw Error.GetError(0x15a5);
                            }
                        }
                        else if (expression3.GetExprType() == 4)
                        {
                            throw Error.GetError(0x15a8);
                        }
                    }
                    if (expression3.IsUnresolvedParam())
                    {
                        expression3.SetAttributesAsColumn(column, true);
                    }
                }
            }
            if (!flag3 && (flag | flag2))
            {
                base.UnexpectedTokenRequire("OVERRIDING");
            }
            StatementInsert insert1 = new StatementInsert(base.session, targetTable, columnMap, tableExpression, columnCheckList);
            insert1.SetDatabseObjects(base.session, base.compileContext);
            insert1.CheckAccessRights(base.session);
            insert1.SetupChecks();
            insert1.InitSimpleInsert();
            return insert1;
        Label_0464:
            throw base.UnexpectedToken();
        Label_046B:
            columnCheckList = targetTable.GetColumnCheckList(columnMap);
            if ((targetTable != table2) && (targetTable != table2))
            {
                int[] numArray3 = new int[columnMap.Length];
                ArrayUtil.ProjectRow(targetTable.GetBaseTableColumnMap(), columnMap, numArray3);
                columnMap = numArray3;
            }
            int identityColumnIndex = targetTable.GetIdentityColumnIndex();
            int overrid = -1;
            if ((identityColumnIndex != -1) && (ArrayUtil.Find(columnMap, identityColumnIndex) > -1))
            {
                if ((targetTable.IdentitySequence.IsAlways() && !flag) && !flag2)
                {
                    throw Error.GetError(0x15a7);
                }
                if (flag)
                {
                    overrid = identityColumnIndex;
                }
            }
            else if (flag | flag2)
            {
                base.UnexpectedTokenRequire("OVERRIDING");
            }
            SqlType[] newRow = new SqlType[columnMap.Length];
            ArrayUtil.ProjectRow(table2.GetColumnTypes(), columnMap, newRow);
            QueryExpression queryExpression = base.XreadQueryExpression();
            queryExpression.SetReturningResult();
            queryExpression.Resolve(base.session, outerRanges, newRow);
            if (columnCount != queryExpression.GetColumnCount())
            {
                throw Error.GetError(0x15aa);
            }
            StatementInsert insert3 = new StatementInsert(base.session, targetTable, columnMap, columnCheckList, queryExpression, overrid);
            insert3.SetDatabseObjects(base.session, base.compileContext);
            insert3.CheckAccessRights(base.session);
            insert3.SetupChecks();
            return insert3;
        }

        public StatementDMQL CompileMergeStatement(RangeVariable[] outerRanges)
        {
            int[] array = null;
            Expression[] a = null;
            List<Expression> updateExpressions = new List<Expression>();
            Expression[] emptyArray = Expression.emptyArray;
            List<Expression> insertExpressions = new List<Expression>();
            Expression tableExpression = null;
            base.Read();
            base.ReadThis(0x8b);
            RangeVariable variable = base.ReadSimpleRangeVariable(0x80);
            Table rangeTable = variable.RangeTable;
            base.ReadThis(0x130);
            RangeVariable sourceRangeVar = base.ReadTableOrSubquery();
            base.ReadThis(0xc0);
            Expression conditions = base.XreadBooleanValueExpression();
            if (conditions.GetDataType() != SqlType.SqlBoolean)
            {
                throw Error.GetError(0x15c0);
            }
            RangeVariable[] rangeVarArray = new RangeVariable[] { sourceRangeVar, variable };
            RangeVariable[] rangeVariables = new RangeVariable[] { sourceRangeVar };
            RangeVariable[] targetRangeVars = new RangeVariable[] { variable };
            int[] columnMap = rangeTable.GetColumnMap();
            bool[] newColumnCheckList = rangeTable.GetNewColumnCheckList();
            OrderedHashSet<Expression> updateTargetSet = new OrderedHashSet<Expression>();
            OrderedHashSet<string> insertColumnNames = new OrderedHashSet<string>();
            LongDeque updateColIndexList = new LongDeque();
            this.ReadMergeWhen(updateColIndexList, insertColumnNames, updateTargetSet, insertExpressions, updateExpressions, targetRangeVars, sourceRangeVar);
            if (insertExpressions.Count > 0)
            {
                if (insertColumnNames.Size() != 0)
                {
                    columnMap = rangeTable.GetColumnIndexes(insertColumnNames);
                    newColumnCheckList = rangeTable.GetColumnCheckList(columnMap);
                }
                tableExpression = insertExpressions[0];
                SetParameterTypes(tableExpression, rangeTable, columnMap);
            }
            if (updateExpressions.Count > 0)
            {
                a = new Expression[updateTargetSet.Size()];
                updateTargetSet.ToArray(a);
                for (int i = 0; i < a.Length; i++)
                {
                    this.ResolveOuterReferencesAndTypes(outerRanges, a[i]);
                }
                emptyArray = updateExpressions.ToArray();
                array = new int[updateColIndexList.Size()];
                updateColIndexList.ToArray(array);
            }
            if (emptyArray.Length != 0)
            {
                Table baseTable = rangeTable.GetBaseTable();
                if (rangeTable != baseTable)
                {
                    int[] newRow = new int[array.Length];
                    ArrayUtil.ProjectRow(rangeTable.GetBaseTableColumnMap(), array, newRow);
                }
                this.ResolveUpdateExpressions(rangeTable, rangeVariables, array, emptyArray, outerRanges);
            }
            List<Expression> set = conditions.ResolveColumnReferences(rangeVarArray, null);
            ExpressionColumn.CheckColumnsResolved(set);
            conditions.ResolveTypes(base.session, null);
            if (conditions.IsUnresolvedParam())
            {
                conditions.DataType = SqlType.SqlBoolean;
            }
            if (conditions.GetDataType() != SqlType.SqlBoolean)
            {
                throw Error.GetError(0x15c0);
            }
            RangeVariableResolver resolver1 = new RangeVariableResolver(rangeVarArray, conditions, base.compileContext);
            resolver1.ProcessConditions(base.session);
            rangeVarArray = resolver1.GetRangeVariables();
            if (tableExpression != null)
            {
                ExpressionColumn.CheckColumnsResolved(tableExpression.ResolveColumnReferences(rangeVariables, set));
                tableExpression.ResolveTypes(base.session, null);
            }
            StatementDML tdml1 = new StatementDML(base.session, a, rangeVarArray, columnMap, array, newColumnCheckList, conditions, tableExpression, emptyArray, base.compileContext);
            tdml1.CheckAccessRights(base.session);
            return tdml1;
        }

        public StatementDMQL CompileUpdateStatement(RangeVariable[] outerRanges)
        {
            base.Read();
            OrderedHashSet<Expression> targets = new OrderedHashSet<Expression>();
            LongDeque colIndexList = new LongDeque();
            List<Expression> expressions = new List<Expression>();
            RangeVariable[] rangeVars = new RangeVariable[] { base.ReadSimpleRangeVariable(0x52) };
            Table rangeTable = rangeVars[0].RangeTable;
            Table baseTable = rangeTable.GetBaseTable();
            base.ReadThis(0xfc);
            this.ReadSetClauseList(rangeVars, targets, colIndexList, expressions);
            int[] array = new int[colIndexList.Size()];
            colIndexList.ToArray(array);
            Expression[] a = new Expression[targets.Size()];
            targets.ToArray(a);
            for (int i = 0; i < a.Length; i++)
            {
                this.ResolveOuterReferencesAndTypes(outerRanges, a[i]);
            }
            bool[] columnCheckList = rangeTable.GetColumnCheckList(array);
            Expression[] colExpressions = expressions.ToArray();
            Expression expression = null;
            if (base.token.TokenType == 0x13a)
            {
                base.Read();
                expression = base.XreadBooleanValueExpression();
                List<Expression> sourceSet = expression.ResolveColumnReferences(outerRanges, null);
                if (outerRanges.Length != 0)
                {
                    sourceSet = Expression.ResolveColumnSet(outerRanges, outerRanges.Length, sourceSet, null);
                }
                ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVars, rangeVars.Length, sourceSet, null));
                expression.ResolveTypes(base.session, null);
                if (expression.IsUnresolvedParam())
                {
                    expression.DataType = SqlType.SqlBoolean;
                }
                if (expression.GetDataType() != SqlType.SqlBoolean)
                {
                    throw Error.GetError(0x15c0);
                }
            }
            this.ResolveUpdateExpressions(rangeTable, rangeVars, array, colExpressions, outerRanges);
            if (rangeTable != baseTable)
            {
                QuerySpecification mainSelect = rangeTable.GetQueryExpression().GetMainSelect();
                RangeVariable[] newRanges = (RangeVariable[]) mainSelect.RangeVariables.Clone();
                newRanges[0] = mainSelect.RangeVariables[0].Duplicate();
                Expression[] list = new Expression[mainSelect.IndexLimitData];
                for (int j = 0; j < mainSelect.IndexLimitData; j++)
                {
                    Expression expression3 = mainSelect.ExprColumns[j].Duplicate();
                    list[j] = expression3;
                    expression3.ReplaceRangeVariables(mainSelect.RangeVariables, newRanges);
                }
                Expression queryCondition = mainSelect.QueryCondition;
                if (queryCondition != null)
                {
                    queryCondition = queryCondition.Duplicate();
                    queryCondition.ReplaceRangeVariables(rangeVars, newRanges);
                }
                if (expression != null)
                {
                    expression = expression.ReplaceColumnReferences(rangeVars[0], list);
                }
                for (int k = 0; k < colExpressions.Length; k++)
                {
                    colExpressions[k] = colExpressions[k].ReplaceColumnReferences(rangeVars[0], list);
                }
                rangeVars = newRanges;
                expression = ExpressionLogical.AndExpressions(queryCondition, expression);
            }
            if (expression != null)
            {
                rangeVars[0].AddJoinCondition(expression);
                RangeVariableResolver resolver1 = new RangeVariableResolver(rangeVars, null, base.compileContext);
                resolver1.ProcessConditions(base.session);
                rangeVars = resolver1.RangeVariables;
            }
            if ((baseTable != null) && (rangeTable != baseTable))
            {
                int[] newRow = new int[array.Length];
                ArrayUtil.ProjectRow(rangeTable.GetBaseTableColumnMap(), array, newRow);
                array = newRow;
                for (int j = 0; j < array.Length; j++)
                {
                    if (baseTable.ColGenerated[array[j]])
                    {
                        throw Error.GetError(0x1589);
                    }
                }
            }
            StatementDML tdml1 = new StatementDML(base.session, a, rangeTable, rangeVars, array, colExpressions, columnCheckList, base.compileContext);
            tdml1.CheckAccessRights(base.session);
            return tdml1;
        }

        private void ReadMergeWhen(LongDeque updateColIndexList, OrderedHashSet<string> insertColumnNames, OrderedHashSet<Expression> updateTargetSet, List<Expression> insertExpressions, List<Expression> updateExpressions, RangeVariable[] targetRangeVars, RangeVariable sourceRangeVar)
        {
            int columnCount = targetRangeVars[0].RangeTable.GetColumnCount();
            base.ReadThis(0x138);
            if (base.token.TokenType == 0x1b4)
            {
                if (updateExpressions.Count != 0)
                {
                    throw Error.GetError(0x15ab);
                }
                base.Read();
                base.ReadThis(0x116);
                base.ReadThis(0x12d);
                base.ReadThis(0xfc);
                this.ReadSetClauseList(targetRangeVars, updateTargetSet, updateColIndexList, updateExpressions);
            }
            else
            {
                if (base.token.TokenType != 0xb5)
                {
                    throw base.UnexpectedToken();
                }
                if (insertExpressions.Count != 0)
                {
                    throw Error.GetError(0x15ac);
                }
                base.Read();
                base.ReadThis(0x1b4);
                base.ReadThis(0x116);
                base.ReadThis(0x85);
                if (base.ReadOpenBrackets() == 1)
                {
                    base.ReadSimpleColumnNames(insertColumnNames, targetRangeVars[0]);
                    columnCount = insertColumnNames.Size();
                    base.ReadThis(0x2aa);
                }
                base.ReadThis(0x132);
                Expression item = base.XreadContextuallyTypedTable(columnCount);
                if (item.nodes.Length != 1)
                {
                    throw Error.GetError(0xc81);
                }
                insertExpressions.Add(item);
            }
            if (base.token.TokenType == 0x138)
            {
                this.ReadMergeWhen(updateColIndexList, insertColumnNames, updateTargetSet, insertExpressions, updateExpressions, targetRangeVars, sourceRangeVar);
            }
        }

        public void ReadSetClauseList(RangeVariable[] rangeVars, OrderedHashSet<Expression> targets, LongDeque colIndexList, List<Expression> expressions)
        {
            Expression expression;
            while (true)
            {
                int num;
                if (base.token.TokenType == 0x2b7)
                {
                    base.Read();
                    int num4 = targets.Size();
                    base.ReadTargetSpecificationList(targets, rangeVars, colIndexList);
                    num = targets.Size() - num4;
                    base.ReadThis(0x2aa);
                }
                else
                {
                    expression = base.XreadTargetSpecification(rangeVars, colIndexList);
                    if (!targets.Add(expression))
                    {
                        break;
                    }
                    num = 1;
                }
                if (base.token.TokenType == 0x18c)
                {
                    base.ReadThis(0x18c);
                }
                else
                {
                    base.ReadThis(0x223);
                }
                int position = base.GetPosition();
                int num3 = base.ReadOpenBrackets();
                if (base.token.TokenType == 0xf9)
                {
                    this.Rewind(position);
                    SubQuery sq = base.XreadSubqueryBody(false, 0x16);
                    if (num != sq.queryExpression.GetColumnCount())
                    {
                        throw Error.GetError(0x15aa);
                    }
                    Expression item = new Expression(0x16, sq);
                    expressions.Add(item);
                    if (base.token.TokenType != 0x2ac)
                    {
                        return;
                    }
                    base.Read();
                }
                else
                {
                    if (num3 > 0)
                    {
                        this.Rewind(position);
                    }
                    if (num > 1)
                    {
                        base.ReadThis(0x2b7);
                        Expression item = base.ReadRow();
                        base.ReadThis(0x2aa);
                        int num5 = (item.GetExprType() == 0x19) ? item.nodes.Length : 1;
                        if (num != num5)
                        {
                            throw Error.GetError(0x15aa);
                        }
                        expressions.Add(item);
                    }
                    else
                    {
                        Expression item = base.XreadValueExpressionWithContext();
                        expressions.Add(item);
                    }
                    if (base.token.TokenType != 0x2ac)
                    {
                        return;
                    }
                    base.Read();
                }
            }
            ColumnSchema column = expression.GetColumn();
            throw Error.GetError(0x15cb, column.GetName().Name);
        }

        public void ResolveOuterReferencesAndTypes(RangeVariable[] rangeVars, Expression e)
        {
            List<Expression> sourceSet = e.ResolveColumnReferences(rangeVars, rangeVars.Length, null, false);
            ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVars, rangeVars.Length, sourceSet, null));
            e.ResolveTypes(base.session, null);
        }

        public void ResolveUpdateExpressions(Table targetTable, RangeVariable[] rangeVariables, int[] columnMap, Expression[] colExpressions, RangeVariable[] outerRanges)
        {
            int identityColumnIndex = -1;
            if (targetTable.HasIdentityColumn() && targetTable.IdentitySequence.IsAlways())
            {
                identityColumnIndex = targetTable.GetIdentityColumnIndex();
            }
            int index = 0;
            for (int i = 0; index < columnMap.Length; i++)
            {
                Expression expression = colExpressions[i];
                if (targetTable.ColGenerated[columnMap[index]])
                {
                    throw Error.GetError(0x1589);
                }
                if (expression.GetExprType() == 0x19)
                {
                    Expression[] nodes = expression.nodes;
                    int num4 = 0;
                    while (num4 < nodes.Length)
                    {
                        Expression expression2 = nodes[num4];
                        if ((identityColumnIndex == columnMap[index]) && (expression2.GetExprType() != 4))
                        {
                            throw Error.GetError(0x15a5);
                        }
                        if (expression2.IsUnresolvedParam())
                        {
                            expression2.SetAttributesAsColumn(targetTable.GetColumn(columnMap[index]), true);
                        }
                        else if (expression2.GetExprType() == 4)
                        {
                            if ((targetTable.ColDefaults[columnMap[index]] == null) && (targetTable.IdentityColumn != columnMap[index]))
                            {
                                throw Error.GetError(0x15a8);
                            }
                        }
                        else
                        {
                            List<Expression> sourceSet = expression.ResolveColumnReferences(outerRanges, null);
                            if (outerRanges.Length != 0)
                            {
                                sourceSet = Expression.ResolveColumnSet(outerRanges, outerRanges.Length, sourceSet, null);
                            }
                            ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVariables, rangeVariables.Length, sourceSet, null));
                            expression2.ResolveTypes(base.session, null);
                        }
                        num4++;
                        index++;
                    }
                }
                else if (expression.GetExprType() == 0x16)
                {
                    List<Expression> sourceSet = expression.ResolveColumnReferences(outerRanges, null);
                    if (outerRanges.Length != 0)
                    {
                        sourceSet = Expression.ResolveColumnSet(outerRanges, outerRanges.Length, sourceSet, null);
                    }
                    ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVariables, rangeVariables.Length, sourceSet, null));
                    expression.ResolveTypes(base.session, null);
                    int columnCount = expression.subQuery.queryExpression.GetColumnCount();
                    int num6 = 0;
                    while (num6 < columnCount)
                    {
                        if (identityColumnIndex == columnMap[index])
                        {
                            throw Error.GetError(0x15a5);
                        }
                        num6++;
                        index++;
                    }
                }
                else
                {
                    Expression expression3 = expression;
                    if ((identityColumnIndex == columnMap[index]) && (expression3.GetExprType() != 4))
                    {
                        throw Error.GetError(0x15a5);
                    }
                    if (expression3.IsUnresolvedParam())
                    {
                        expression3.SetAttributesAsColumn(targetTable.GetColumn(columnMap[index]), true);
                    }
                    else if (expression3.GetExprType() == 4)
                    {
                        if ((targetTable.ColDefaults[columnMap[index]] == null) && (targetTable.IdentityColumn != columnMap[index]))
                        {
                            throw Error.GetError(0x15a8);
                        }
                    }
                    else
                    {
                        List<Expression> sourceSet = expression.ResolveColumnReferences(outerRanges, null);
                        if (outerRanges.Length != 0)
                        {
                            sourceSet = Expression.ResolveColumnSet(outerRanges, outerRanges.Length, sourceSet, null);
                        }
                        ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVariables, rangeVariables.Length, sourceSet, null));
                        expression3.ResolveTypes(base.session, null);
                    }
                    index++;
                }
            }
        }

        private static void SetParameterTypes(Expression tableExpression, Table table, int[] columnMap)
        {
            for (int i = 0; i < tableExpression.nodes.Length; i++)
            {
                Expression[] nodes = tableExpression.nodes[i].nodes;
                for (int j = 0; j < nodes.Length; j++)
                {
                    if (nodes[j].IsUnresolvedParam())
                    {
                        nodes[j].SetAttributesAsColumn(table.GetColumn(columnMap[j]), true);
                    }
                }
            }
        }
    }
}

