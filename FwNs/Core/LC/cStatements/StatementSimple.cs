namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public sealed class StatementSimple : Statement
    {
        private readonly string _sqlState;
        private readonly Dictionary<int, Expression> _conditionInformationItems;
        private readonly QNameManager.QName _conditionName;
        private int _conditionIndex;
        private string cursorVariableName;
        private int cursorVariableNameIndex;
        public QNameManager.QName label;
        private ColumnSchema variable;

        public StatementSimple(int type, QNameManager.QName label) : base(type, 0x7d7)
        {
            this._conditionIndex = -1;
            this.cursorVariableNameIndex = -1;
            base.References = new OrderedHashSet<QNameManager.QName>();
            base.isTransactionStatement = false;
            this.label = label;
        }

        public StatementSimple(int type, QNameManager.QName conditionName, Dictionary<int, Expression> conditionInformationItems) : base(type, 0x7d7)
        {
            this._conditionIndex = -1;
            this.cursorVariableNameIndex = -1;
            base.References = new OrderedHashSet<QNameManager.QName>();
            base.isTransactionStatement = false;
            this._conditionName = conditionName;
            this._conditionInformationItems = conditionInformationItems;
        }

        public StatementSimple(int type, QNameManager.QName label, string cursorVariableName) : base(type, 0x7d3)
        {
            this._conditionIndex = -1;
            this.cursorVariableNameIndex = -1;
            base.References = new OrderedHashSet<QNameManager.QName>();
            this.label = label;
            this.cursorVariableName = cursorVariableName;
        }

        public StatementSimple(int type, string sqlState, Dictionary<int, Expression> conditionInformationItems) : base(type, 0x7d7)
        {
            this._conditionIndex = -1;
            this.cursorVariableNameIndex = -1;
            base.References = new OrderedHashSet<QNameManager.QName>();
            base.isTransactionStatement = false;
            this._sqlState = sqlState;
            this._conditionInformationItems = conditionInformationItems;
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

        public Result GetResult(Session session)
        {
            string str3;
            int num3;
            string sqlState;
            int type = base.type;
            if (type <= 0x35)
            {
                switch (type)
                {
                    case 9:
                    {
                        Cursor cursor2 = session.sessionContext.RoutineVariables[this.cursorVariableNameIndex] as Cursor;
                        if (cursor2 == null)
                        {
                            throw Error.GetError(0xe10);
                        }
                        cursor2.Close(session);
                        return Result.UpdateZeroResult;
                    }
                    case 0x35:
                    {
                        Cursor cursor1 = session.sessionContext.RoutineVariables[this.cursorVariableNameIndex] as Cursor;
                        if (cursor1 == null)
                        {
                            throw Error.GetError(0xe10);
                        }
                        cursor1.Open(session, this.cursorVariableName);
                        return Result.UpdateZeroResult;
                    }
                }
                goto Label_0291;
            }
            switch (type)
            {
                case 0x59:
                    goto Label_0207;

                case 90:
                    goto Label_0291;

                case 0x5b:
                {
                    if (!session.sessionContext.ActiveHandlerContextPresent())
                    {
                        throw Error.GetError(0x76c);
                    }
                    Result result = session.sessionContext.PeekHandlerContext();
                    string message = FwNs.Core.LC.cResources.SR.StatementSimple_GetResult_sql_routine_error;
                    int i = -1;
                    string sqlState = this._sqlState;
                    if ((sqlState == null) && (this._conditionInformationItems == null))
                    {
                        return result;
                    }
                    if (this._conditionInformationItems != null)
                    {
                        Expression expression;
                        if (this._conditionInformationItems.TryGetValue(440, out expression))
                        {
                            message = (string) expression.GetValue(session, SqlType.SqlVarcharDefault);
                        }
                        else
                        {
                            message = result.GetMainString();
                        }
                        if (this._conditionInformationItems.TryGetValue(0x331, out expression))
                        {
                            i = (int) expression.GetValue(session, SqlType.SqlInteger);
                        }
                        else
                        {
                            i = result.GetErrorCode();
                        }
                    }
                    if (sqlState == null)
                    {
                        sqlState = result.GetSubString();
                    }
                    CoreException exception = Error.GetError(message, sqlState, i);
                    if (ErrorCode.IsWarning(sqlState))
                    {
                        session.AddWarning(exception);
                        return Result.UpdateZeroResult;
                    }
                    return Result.NewErrorResult(exception);
                }
                case 0x5c:
                    str3 = FwNs.Core.LC.cResources.SR.StatementSimple_GetResult_sql_routine_error;
                    num3 = -1;
                    sqlState = this._sqlState;
                    if (this._conditionName != null)
                    {
                        Condition condition = (Condition) session.sessionContext.RoutineVariables[this._conditionIndex];
                        if (!condition.IsSqlState)
                        {
                            num3 = (int) condition.ErrNo.GetValue(session, SqlType.SqlInteger);
                            break;
                        }
                        sqlState = condition.SqlState;
                    }
                    break;

                default:
                    if (type != 0x66)
                    {
                        goto Label_0291;
                    }
                    goto Label_0207;
            }
            if (this._conditionInformationItems != null)
            {
                Expression expression2;
                if (this._conditionInformationItems.TryGetValue(440, out expression2))
                {
                    str3 = (string) expression2.GetValue(session, SqlType.SqlVarcharDefault);
                }
                else
                {
                    str3 = FwNs.Core.LC.cResources.SR.StatementSimple_GetResult_sql_routine_error;
                }
                if (this._conditionInformationItems.TryGetValue(0x331, out expression2))
                {
                    num3 = (int) expression2.GetValue(session, SqlType.SqlInteger);
                }
                else
                {
                    num3 = -1;
                }
            }
            CoreException warning = Error.GetError(str3, sqlState, num3);
            if (ErrorCode.IsWarning(sqlState))
            {
                session.AddWarning(warning);
                return Result.UpdateZeroResult;
            }
            return Result.NewErrorResult(warning);
        Label_0207:
            return Result.NewPsmResult(base.type, this.label.Name, null);
        Label_0291:
            throw Error.RuntimeError(0xc9, "");
        }

        public override string GetSql()
        {
            return base.Sql;
        }

        public override bool IsCatalogChange()
        {
            return false;
        }

        public override void Resolve(Session session, RangeVariable[] rangeVars)
        {
            bool flag = false;
            int type = base.type;
            if (type <= 0x35)
            {
                if ((type != 9) && (type != 0x35))
                {
                    goto Label_01D3;
                }
                OrderedHashSet<string> colNames = new OrderedHashSet<string>();
                colNames.Add(this.cursorVariableName);
                int[] indexes = new int[colNames.Size()];
                ColumnSchema[] variables = new ColumnSchema[colNames.Size()];
                StatementSet.SetVariables(rangeVars, colNames, indexes, variables);
                this.cursorVariableNameIndex = indexes[0];
                this.variable = variables[0];
                if (this.cursorVariableNameIndex < 0)
                {
                    throw Error.GetError(0xe10);
                }
                if (this.variable == null)
                {
                    throw Error.GetError(0xe10);
                }
                flag = true;
                goto Label_01E3;
            }
            switch (type)
            {
                case 0x59:
                    flag = true;
                    goto Label_01E3;

                case 90:
                    break;

                case 0x5b:
                case 0x5c:
                    if (this._conditionName != null)
                    {
                        OrderedHashSet<string> colNames = new OrderedHashSet<string>();
                        colNames.Add(this._conditionName.Name);
                        int[] indexes = new int[colNames.Size()];
                        ColumnSchema[] variables = new ColumnSchema[colNames.Size()];
                        StatementSet.SetVariables(rangeVars, colNames, indexes, variables);
                        this._conditionIndex = indexes[0];
                        if (this._conditionIndex == -1)
                        {
                            throw Error.GetError(0x19e0, this._conditionName.Name);
                        }
                    }
                    if (this._conditionInformationItems != null)
                    {
                        foreach (Expression local1 in this._conditionInformationItems.Values)
                        {
                            ExpressionColumn.CheckColumnsResolved(local1.ResolveColumnReferences(rangeVars, null));
                            local1.ResolveTypes(session, null);
                        }
                    }
                    flag = true;
                    goto Label_01E3;

                default:
                    if (type == 0x66)
                    {
                        for (StatementCompound compound = base.Parent; compound != null; compound = compound.Parent)
                        {
                            if ((compound.IsLoop && (compound.label != null)) && this.label.Name.Equals(compound.label.Name))
                            {
                                flag = true;
                                break;
                            }
                        }
                        goto Label_01E3;
                    }
                    break;
            }
        Label_01D3:
            throw Error.RuntimeError(0xc9, "");
        Label_01E3:
            if (!flag)
            {
                throw Error.GetError(0x15e2);
            }
        }
    }
}

