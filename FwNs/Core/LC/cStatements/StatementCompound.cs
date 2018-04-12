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
    using System.Collections.Generic;

    public sealed class StatementCompound : Statement
    {
        public static StatementCompound[] EmptyStatementArray = new StatementCompound[0];
        public StatementExpression condition;
        public StatementHandler[] Handlers;
        public bool HasUndoHandler;
        public bool IsAtomic;
        public bool IsLoop;
        public QNameManager.QName label;
        public StatementQuery LoopCursor;
        public RangeVariable[] RangeVariables;
        public HashMappedList<string, ColumnSchema> ScopeVariables;
        public Table[] Tables;
        public HashMappedList<string, Table> ScopeTables;
        public Statement[] Statements;
        public ColumnSchema[] Variables;

        public StatementCompound(int type, QNameManager.QName label) : base(type, 0x7d7)
        {
            this.Handlers = StatementHandler.EmptyExceptionHandlerArray;
            this.RangeVariables = RangeVariable.EmptyArray;
            this.Tables = Table.EmptyArray;
            this.Variables = ColumnSchema.EmptyArray;
            this.label = label;
            base.isTransactionStatement = false;
            if (type <= 0x2e)
            {
                if (type == 12)
                {
                    goto Label_008B;
                }
                if (type == 0x2e)
                {
                    goto Label_0083;
                }
            }
            else
            {
                switch (type)
                {
                    case 0x58:
                        goto Label_008B;

                    case 90:
                    case 0x5f:
                    case 0x61:
                        goto Label_0083;
                }
            }
            throw Error.RuntimeError(0xc9, "StatementCompound");
        Label_0083:
            this.IsLoop = true;
            return;
        Label_008B:
            this.IsLoop = false;
        }

        public override string Describe(Session session)
        {
            return "";
        }

        public override Result Execute(Session session)
        {
            Result result;
            int type = base.type;
            if (type > 0x2e)
            {
                switch (type)
                {
                    case 0x58:
                        result = this.ExecuteIf(session);
                        goto Label_0083;

                    case 90:
                    case 0x5f:
                    case 0x61:
                        result = this.ExecuteLoop(session);
                        goto Label_0083;
                }
            }
            else
            {
                switch (type)
                {
                    case 12:
                        this.InitialiseVariables(session);
                        result = this.ExecuteBlock(session);
                        goto Label_0083;

                    case 0x2e:
                        result = this.ExecuteForLoop(session);
                        goto Label_0083;
                }
            }
            throw Error.RuntimeError(0xc9, "StatementCompound");
        Label_0083:
            if (result.IsError())
            {
                result.GetException().SetStatementType(base.Group, base.type);
            }
            return result;
        }

        private Result ExecuteBlock(Session session)
        {
            Result updateZeroResult = Result.UpdateZeroResult;
            bool flag = !base.Root.IsTrigger();
            if (flag)
            {
                session.sessionContext.Push(false);
                session.sessionContext.PushRoutineTables(this.ScopeTables);
                if (this.HasUndoHandler)
                {
                    string autoSavepointNameString = QNameManager.GetAutoSavepointNameString(session.ActionTimestamp, session.sessionContext.Depth);
                    session.Savepoint(autoSavepointNameString);
                }
            }
            for (int i = 0; i < this.Statements.Length; i++)
            {
                Result result = this.Statements[i].Execute(session);
                updateZeroResult = this.HandleCondition(session, result);
                if (updateZeroResult.IsError() || (updateZeroResult.GetResultType() == 0x2a))
                {
                    break;
                }
                if (updateZeroResult.IsData() && !base.Root.IsFunction())
                {
                    session.AddResultSet(result);
                    updateZeroResult = Result.UpdateZeroResult;
                }
            }
            if ((updateZeroResult.GetResultType() == 0x2a) && (updateZeroResult.GetErrorCode() == 0x59))
            {
                if (updateZeroResult.GetMainString() == null)
                {
                    updateZeroResult = Result.UpdateZeroResult;
                }
                else if ((this.label != null) && this.label.Name.Equals(updateZeroResult.GetMainString()))
                {
                    updateZeroResult = Result.UpdateZeroResult;
                }
            }
            if (flag)
            {
                session.sessionContext.PopRoutineTables();
                session.sessionContext.Pop(false);
            }
            return updateZeroResult;
        }

        private Result ExecuteForLoop(Session session)
        {
            Result result = this.LoopCursor.GetResult(session);
            if (result.IsError())
            {
                return result;
            }
            Result updateZeroResult = Result.UpdateZeroResult;
            while (result.navigator.HasNext())
            {
                result.navigator.Next();
                object[] current = result.navigator.GetCurrent();
                this.InitialiseVariables(session, current);
                for (int i = 0; i < this.Statements.Length; i++)
                {
                    updateZeroResult = this.Statements[i].Execute(session);
                    if ((updateZeroResult.IsError() || (updateZeroResult.GetResultType() == 0x2a)) || (updateZeroResult.GetResultType() == 3))
                    {
                        break;
                    }
                }
                if (updateZeroResult.IsError())
                {
                    break;
                }
                if (updateZeroResult.GetResultType() == 0x2a)
                {
                    if (updateZeroResult.GetErrorCode() == 0x66)
                    {
                        if ((updateZeroResult.GetMainString() == null) || ((this.label != null) && this.label.Name.Equals(updateZeroResult.GetMainString())))
                        {
                            continue;
                        }
                    }
                    else if (updateZeroResult.GetErrorCode() == 0x59)
                    {
                    }
                    break;
                }
                if (updateZeroResult.GetResultType() == 3)
                {
                    break;
                }
            }
            result.navigator.Close();
            return updateZeroResult;
        }

        private Result ExecuteIf(Session session)
        {
            Result updateZeroResult = Result.UpdateZeroResult;
            bool flag = false;
            for (int i = 0; i < this.Statements.Length; i++)
            {
                if (this.Statements[i].GetStatementType() == 0x44d)
                {
                    if (flag)
                    {
                        return updateZeroResult;
                    }
                    updateZeroResult = this.Statements[i].Execute(session);
                    if (updateZeroResult.IsError())
                    {
                        return updateZeroResult;
                    }
                    object valueObject = updateZeroResult.GetValueObject();
                    flag = (valueObject != null) && ((bool) valueObject);
                    i++;
                }
                updateZeroResult = Result.UpdateZeroResult;
                if (flag)
                {
                    updateZeroResult = this.Statements[i].Execute(session);
                    if (updateZeroResult.IsError() || (updateZeroResult.GetResultType() == 0x2a))
                    {
                        return updateZeroResult;
                    }
                    if (updateZeroResult.IsData())
                    {
                        session.AddResultSet(updateZeroResult);
                        updateZeroResult = Result.UpdateZeroResult;
                    }
                }
            }
            return updateZeroResult;
        }

        private Result ExecuteLoop(Session session)
        {
            Result updateZeroResult = Result.UpdateZeroResult;
        Label_0006:
            if (base.type == 0x61)
            {
                updateZeroResult = this.condition.Execute(session);
                if (updateZeroResult.IsError())
                {
                    return updateZeroResult;
                }
                object valueObject = updateZeroResult.GetValueObject();
                if ((valueObject == null) || !((bool) valueObject))
                {
                    return Result.UpdateZeroResult;
                }
            }
            for (int i = 0; i < this.Statements.Length; i++)
            {
                updateZeroResult = this.Statements[i].Execute(session);
                if (updateZeroResult.IsError() || (updateZeroResult.GetResultType() == 0x2a))
                {
                    break;
                }
                if (updateZeroResult.IsData())
                {
                    session.AddResultSet(updateZeroResult);
                    updateZeroResult = Result.UpdateZeroResult;
                }
            }
            if (!updateZeroResult.IsError())
            {
                if (updateZeroResult.GetResultType() == 0x2a)
                {
                    if (updateZeroResult.GetErrorCode() == 0x66)
                    {
                        if ((updateZeroResult.GetMainString() != null) && ((this.label == null) || !this.label.Name.Equals(updateZeroResult.GetMainString())))
                        {
                            return updateZeroResult;
                        }
                        goto Label_0006;
                    }
                }
                else
                {
                    if (base.type != 0x5f)
                    {
                        goto Label_0006;
                    }
                    updateZeroResult = this.condition.Execute(session);
                    if (updateZeroResult.IsError())
                    {
                        return updateZeroResult;
                    }
                    object valueObject = updateZeroResult.GetValueObject();
                    if ((valueObject == null) || !((bool) valueObject))
                    {
                        goto Label_0006;
                    }
                    return Result.UpdateZeroResult;
                }
                if (updateZeroResult.GetErrorCode() == 0x59)
                {
                    if (updateZeroResult.GetMainString() == null)
                    {
                        updateZeroResult = Result.UpdateZeroResult;
                    }
                    if ((this.label != null) && this.label.Name.Equals(updateZeroResult.GetMainString()))
                    {
                        return Result.UpdateZeroResult;
                    }
                }
            }
            return updateZeroResult;
        }

        private bool FindLabel(StatementSimple statement)
        {
            if ((this.label != null) && statement.label.Name.Equals(this.label.Name))
            {
                return (this.IsLoop || (statement.GetStatementType() != 0x66));
            }
            return ((base.Parent != null) && base.Parent.FindLabel(statement));
        }

        public override RangeVariable[] GetRangeVariables()
        {
            return this.RangeVariables;
        }

        public override OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return base.References;
        }

        public override string GetSql()
        {
            return base.Sql;
        }

        private Result HandleCondition(Session session, Result result)
        {
            string subString;
            int errorCode;
            Result r = result;
            if (result.IsError())
            {
                subString = result.GetSubString();
                errorCode = result.GetErrorCode();
            }
            else
            {
                if (session.GetLastWarning() == null)
                {
                    return result;
                }
                CoreException lastWarning = session.GetLastWarning();
                subString = lastWarning.GetSqlState();
                errorCode = lastWarning.GetErrorCode();
            }
            if ((subString != null) || (errorCode != 0))
            {
                for (int i = 0; i < this.Handlers.Length; i++)
                {
                    StatementHandler handler = this.Handlers[i];
                    session.ClearWarnings();
                    if (handler.HandlesCondition(subString, errorCode))
                    {
                        Result result3;
                        session.ResetSchema();
                        switch (handler.HandlerType)
                        {
                            case 5:
                                result = Result.UpdateZeroResult;
                                break;

                            case 6:
                                result = Result.NewPsmResult(0x59, null, null);
                                break;

                            case 7:
                                session.RollbackToSavepoint();
                                result = Result.NewPsmResult(0x59, this.label.Name, null);
                                break;
                        }
                        session.sessionContext.PushHandlerContext(r);
                        try
                        {
                            result3 = handler.Execute(session);
                        }
                        finally
                        {
                            session.sessionContext.PopHandlerContext();
                        }
                        if (result3.IsError())
                        {
                            result = result3;
                            break;
                        }
                        return result;
                    }
                }
                if (base.Parent != null)
                {
                    return base.Parent.HandleCondition(session, result);
                }
            }
            return result;
        }

        private void InitialiseVariables(Session session)
        {
            object[] routineVariables = session.sessionContext.RoutineVariables;
            int num = ((base.Parent == null) || (base.Parent.ScopeVariables == null)) ? 0 : base.Parent.ScopeVariables.Size();
            for (int i = 0; i < this.Variables.Length; i++)
            {
                try
                {
                    object defaultValue = this.Variables[i].GetDefaultValue(session);
                    Cursor cursor = defaultValue as Cursor;
                    if (cursor != null)
                    {
                        routineVariables[num + i] = cursor.Clone();
                    }
                    else
                    {
                        routineVariables[num + i] = defaultValue;
                    }
                }
                catch (CoreException)
                {
                }
            }
        }

        private void InitialiseVariables(Session session, object[] data)
        {
            object[] routineVariables = session.sessionContext.RoutineVariables;
            int num = ((base.Parent == null) || (base.Parent.ScopeVariables == null)) ? 0 : base.Parent.ScopeVariables.Size();
            for (int i = 0; i < data.Length; i++)
            {
                try
                {
                    routineVariables[num + i] = data[i];
                }
                catch (CoreException)
                {
                }
            }
        }

        public override void Resolve(Session session)
        {
            this.Resolve(session, null);
        }

        public override void Resolve(Session session, RangeVariable[] rangeVars)
        {
            for (int i = 0; i < this.Statements.Length; i++)
            {
                if ((this.Statements[i].GetStatementType() == 0x59) || (this.Statements[i].GetStatementType() == 0x66))
                {
                    if (!this.FindLabel((StatementSimple) this.Statements[i]))
                    {
                        throw Error.GetError(0x1584, ((StatementSimple) this.Statements[i]).label.Name);
                    }
                }
                else if ((this.Statements[i].GetStatementType() == 0x3a) && !base.Root.IsFunction())
                {
                    throw Error.GetError(0x15e2, "RETURN");
                }
            }
            for (int j = 0; j < this.Statements.Length; j++)
            {
                this.Statements[j].Resolve(session, this.RangeVariables);
            }
            for (int k = 0; k < this.Handlers.Length; k++)
            {
                this.Handlers[k].Resolve(session, this.RangeVariables);
            }
            OrderedHashSet<QNameManager.QName> c = new OrderedHashSet<QNameManager.QName>();
            OrderedHashSet<QNameManager.QName> set2 = new OrderedHashSet<QNameManager.QName>();
            OrderedHashSet<QNameManager.QName> set3 = new OrderedHashSet<QNameManager.QName>();
            for (int m = 0; m < this.Variables.Length; m++)
            {
                set3.AddAll(this.Variables[m].GetReferences());
            }
            for (int n = 0; n < this.Statements.Length; n++)
            {
                set3.AddAll(this.Statements[n].GetReferences());
                set2.AddAll(this.Statements[n].GetTableNamesForRead());
                c.AddAll(this.Statements[n].GetTableNamesForWrite());
            }
            for (int num6 = 0; num6 < this.Handlers.Length; num6++)
            {
                set3.AddAll(this.Handlers[num6].GetReferences());
                set2.AddAll(this.Handlers[num6].GetTableNamesForRead());
                c.AddAll(this.Handlers[num6].GetTableNamesForWrite());
            }
            set2.RemoveAll(c);
            base.ReadTableNames = new QNameManager.QName[set2.Size()];
            set2.ToArray(base.ReadTableNames);
            base.WriteTableNames = new QNameManager.QName[c.Size()];
            c.ToArray(base.WriteTableNames);
            base.References = set3;
        }

        public void SetAtomic(bool atomic)
        {
            this.IsAtomic = atomic;
        }

        public void SetCondition(StatementExpression condition)
        {
            this.condition = condition;
        }

        private void SetHandlers()
        {
            if (this.Handlers.Length != 0)
            {
                HashSet<string> set = new HashSet<string>();
                HashSet<int> set2 = new HashSet<int>();
                for (int i = 0; i < this.Handlers.Length; i++)
                {
                    int[] conditionTypes = this.Handlers[i].GetConditionTypes();
                    for (int j = 0; j < conditionTypes.Length; j++)
                    {
                        if (!set2.Add(conditionTypes[j]))
                        {
                            throw Error.GetError(0x15e1);
                        }
                    }
                    string[] conditionStates = this.Handlers[i].GetConditionStates();
                    for (int k = 0; k < conditionStates.Length; k++)
                    {
                        if (!set.Add(conditionStates[k]))
                        {
                            throw Error.GetError(0x15e1);
                        }
                    }
                }
            }
        }

        public void SetLocalDeclarations(object[] declarations)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < declarations.Length; i++)
            {
                if (((declarations[i] is ColumnSchema) || (declarations[i] is StatementQuery)) || (declarations[i] is Condition))
                {
                    num++;
                }
                else if (declarations[i] is Table)
                {
                    num3++;
                }
                else
                {
                    num2++;
                }
            }
            this.Variables = new ColumnSchema[num];
            this.Handlers = new StatementHandler[num2];
            if (num3 > 0)
            {
                this.Tables = new Table[num3];
            }
            num = 0;
            num2 = 0;
            num3 = 0;
            for (int j = 0; j < declarations.Length; j++)
            {
                if (declarations[j] is ColumnSchema)
                {
                    this.Variables[num++] = (ColumnSchema) declarations[j];
                }
                else if (declarations[j] is StatementQuery)
                {
                    StatementQuery queryStatement = (StatementQuery) declarations[j];
                    Cursor o = new Cursor(queryStatement);
                    this.Variables[num++] = new ColumnSchema(queryStatement.CursorName, SqlType.SqlAllTypes, false, false, new ExpressionValue(o, SqlType.SqlAllTypes));
                }
                else if (declarations[j] is Condition)
                {
                    Condition o = (Condition) declarations[j];
                    this.Variables[num++] = new ColumnSchema(o.Name, SqlType.SqlAllTypes, false, false, new ExpressionValue(o, SqlType.SqlAllTypes));
                }
                else if (declarations[j] is Table)
                {
                    Table table = (Table) declarations[j];
                    this.Tables[num3++] = table;
                }
                else
                {
                    StatementHandler handler = (StatementHandler) declarations[j];
                    handler.SetParent(this);
                    this.Handlers[num2++] = handler;
                    if (handler.HandlerType == 7)
                    {
                        this.HasUndoHandler = true;
                    }
                }
            }
            this.SetVariables();
            this.SetHandlers();
            this.SetTables();
        }

        public void SetLoopStatement(StatementQuery cursorStatement)
        {
            this.LoopCursor = cursorStatement;
            QNameManager.QName[] resultColumnNames = cursorStatement.queryExpression.GetResultColumnNames();
            SqlType[] columnTypes = cursorStatement.queryExpression.GetColumnTypes();
            ColumnSchema[] declarations = new ColumnSchema[resultColumnNames.Length];
            for (int i = 0; i < resultColumnNames.Length; i++)
            {
                declarations[i] = new ColumnSchema(resultColumnNames[i], columnTypes[i], false, false, null);
                declarations[i].SetParameterMode(1);
            }
            this.SetLocalDeclarations(declarations);
        }

        public override void SetRoot(Routine routine)
        {
            base.Root = routine;
        }

        public void SetStatements(Statement[] statements)
        {
            for (int i = 0; i < statements.Length; i++)
            {
                statements[i].SetParent(this);
            }
            this.Statements = statements;
        }

        private void SetTables()
        {
            if (((this.Tables.Length != 0) || (base.Parent != null)) || (base.Root.ScopeTables != null))
            {
                HashMappedList<string, Table> list = new HashMappedList<string, Table>();
                if ((base.Parent == null) && (base.Root.ScopeTables != null))
                {
                    for (int j = 0; j < base.Root.ScopeTables.Size(); j++)
                    {
                        list.Add(base.Root.ScopeTables.GetKey(j), base.Root.ScopeTables.Get(j));
                    }
                }
                else if ((base.Parent != null) && (base.Parent.ScopeTables != null))
                {
                    for (int j = 0; j < base.Parent.ScopeTables.Size(); j++)
                    {
                        list.Add(base.Parent.ScopeTables.GetKey(j), base.Parent.ScopeTables.Get(j));
                    }
                }
                for (int i = 0; i < this.Tables.Length; i++)
                {
                    string name = this.Tables[i].GetName().Name;
                    if (!list.Add(name, this.Tables[i]))
                    {
                        throw Error.GetError(0x15e6, name);
                    }
                }
                this.ScopeTables = list;
            }
        }

        private void SetVariables()
        {
            if (this.Variables.Length == 0)
            {
                this.RangeVariables = (base.Parent == null) ? base.Root.GetParameterRangeVariables() : base.Parent.RangeVariables;
            }
            else
            {
                HashMappedList<string, ColumnSchema> variables = new HashMappedList<string, ColumnSchema>();
                if ((base.Parent != null) && (base.Parent.ScopeVariables != null))
                {
                    for (int j = 0; j < base.Parent.ScopeVariables.Size(); j++)
                    {
                        variables.Add(base.Parent.ScopeVariables.GetKey(j), base.Parent.ScopeVariables.Get(j));
                    }
                }
                for (int i = 0; i < this.Variables.Length; i++)
                {
                    string name = this.Variables[i].GetName().Name;
                    if (!variables.Add(name, this.Variables[i]))
                    {
                        throw Error.GetError(0x15e6, name);
                    }
                    if (base.Root.GetParameterIndex(name) != -1)
                    {
                        throw Error.GetError(0x15e6, name);
                    }
                }
                this.ScopeVariables = variables;
                RangeVariable variable = new RangeVariable(variables, true);
                if (!base.Root.IsTrigger())
                {
                    this.RangeVariables = new RangeVariable[] { base.Root.GetParameterRangeVariables()[0], variable };
                }
                else
                {
                    this.RangeVariables = new RangeVariable[base.Root.GetParameterRangeVariables().Length + 1];
                    Array.Copy(base.Root.GetParameterRangeVariables(), this.RangeVariables, (int) (this.RangeVariables.Length - 1));
                    this.RangeVariables[this.RangeVariables.Length - 1] = variable;
                }
                base.Root.VariableCount = variables.Size();
            }
        }
    }
}

