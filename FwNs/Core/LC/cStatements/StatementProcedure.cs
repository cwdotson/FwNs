namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public sealed class StatementProcedure : StatementDMQL
    {
        private readonly Expression[] _arguments;
        private readonly Expression _expression;
        private ResultMetaData _resultMetaData;
        public Routine Procedure;

        public StatementProcedure(Session session, Expression expression) : base(7, 0x7d3, session.GetCurrentSchemaQName())
        {
            this._arguments = Expression.emptyArray;
            if (expression.OpType == 0x1b)
            {
                FunctionSQLInvoked invoked = (FunctionSQLInvoked) expression;
                if (invoked.routine.ReturnsTable())
                {
                    this.Procedure = invoked.routine;
                    this._arguments = invoked.nodes;
                }
                else
                {
                    this._expression = expression;
                }
            }
            else
            {
                this._expression = expression;
            }
            if (this.Procedure != null)
            {
                session.GetGrantee().CheckAccess(this.Procedure);
            }
        }

        public StatementProcedure(Session session, Routine procedure, Expression[] arguments) : base(7, 0x7d3, session.GetCurrentSchemaQName())
        {
            this._arguments = Expression.emptyArray;
            this.Procedure = procedure;
            this._arguments = arguments;
            if (procedure != null)
            {
                session.GetGrantee().CheckAccess(procedure);
            }
        }

        public override void CollectTableNamesForRead(OrderedHashSet<QNameManager.QName> set)
        {
            if (this._expression == null)
            {
                set.AddAll(this.Procedure.GetTableNamesForRead());
            }
            else
            {
                for (int i = 0; i < base.Subqueries.Length; i++)
                {
                    if (base.Subqueries[i].queryExpression != null)
                    {
                        base.Subqueries[i].queryExpression.GetBaseTableNames(set);
                    }
                }
                for (int j = 0; j < base.Routines.Length; j++)
                {
                    set.AddAll(base.Routines[j].GetTableNamesForRead());
                }
            }
        }

        public override void CollectTableNamesForWrite(OrderedHashSet<QNameManager.QName> set)
        {
            if (this._expression == null)
            {
                set.AddAll(this.Procedure.GetTableNamesForWrite());
            }
        }

        private Result ExecuteClrProcedure(Session session)
        {
            bool cSharpMethodWithConnection = this.Procedure.CSharpMethodWithConnection;
            object[] routineArguments = session.sessionContext.RoutineArguments;
            object[] data = this.Procedure.ConvertArgsToCSharp(session, routineArguments);
            if (this.Procedure.CSharpMethodWithConnection)
            {
                data[0] = session.GetInternalConnection();
            }
            this.Procedure.ConvertArgsToSql(session, routineArguments, data);
            return this.Procedure.InvokeClrMethod(session, data);
        }

        private Result ExecutePsmProcedure(Session session)
        {
            int variableCount = this.Procedure.GetVariableCount();
            if (variableCount > 0)
            {
                session.sessionContext.RoutineVariables = new object[variableCount];
            }
            Result result = this.Procedure.statement.Execute(session);
            if (result.IsError() && (result.GetException().GetSqlState() != "00000"))
            {
                return result;
            }
            return Result.UpdateZeroResult;
        }

        private IRowIterator GetArgumentNavigator(Session session, string tableName)
        {
            Table table1 = session.database.schemaManager.GetTable(session, tableName, null);
            if (table1 == null)
            {
                throw Error.GetError(0x15b9);
            }
            return table1.GetRowIterator(session);
        }

        public Result GetExpressionResult(Session session)
        {
            object[] objArray;
            session.sessionData.StartRowProcessing();
            object obj2 = this._expression.GetValue(session);
            Result result = obj2 as Result;
            if (result != null)
            {
                return result;
            }
            if (this._resultMetaData == null)
            {
                this.GetResultMetaData();
            }
            Result result3 = Result.NewSingleColumnResult(this._resultMetaData);
            if (this._expression.GetDataType().IsArrayType())
            {
                objArray = new object[] { obj2 };
            }
            else
            {
                object[] objArray2 = obj2 as object[];
                if (objArray2 != null)
                {
                    objArray = objArray2;
                }
                else
                {
                    objArray = new object[] { obj2 };
                }
            }
            result3.GetNavigator().Add(objArray);
            return result3;
        }

        public Result GetProcedureResult(Session session)
        {
            Result updateZeroResult;
            object[] objArray = new object[0];
            if (this._arguments.Length != 0)
            {
                objArray = new object[this._arguments.Length];
            }
            Dictionary<string, IRowIterator> dictionary = new Dictionary<string, IRowIterator>();
            for (int i = 0; i < this._arguments.Length; i++)
            {
                Expression expression = this._arguments[i];
                object a = expression.GetValue(session);
                if (expression != null)
                {
                    ColumnSchema parameter = this.Procedure.GetParameter(i);
                    SqlType dataType = parameter.GetDataType();
                    objArray[i] = dataType.ConvertToType(session, a, expression.GetDataType());
                    if (dataType.IsTableType())
                    {
                        IRowIterator argumentNavigator = this.GetArgumentNavigator(session, expression.GetColumnName());
                        dictionary.Add(parameter.GetName().Name, argumentNavigator);
                    }
                }
            }
            session.sessionContext.Push(true);
            session.sessionContext.PushRoutineTables(this.Procedure.ScopeTables);
            foreach (KeyValuePair<string, IRowIterator> pair in dictionary)
            {
                Table parameterTable = this.Procedure.GetParameterTable(pair.Key);
                if (parameterTable != null)
                {
                    IPersistentStore rowStore = session.sessionData.GetRowStore(parameterTable);
                    parameterTable.ClearAllData(rowStore);
                    IRowIterator iterator2 = pair.Value;
                    while (iterator2.HasNext())
                    {
                        object[] next = iterator2.GetNext();
                        parameterTable.InsertData(session, rowStore, next);
                    }
                }
            }
            session.sessionContext.RoutineArguments = objArray;
            session.sessionContext.RoutineVariables = new object[0];
            if (this.Procedure.IsPsm())
            {
                updateZeroResult = this.ExecutePsmProcedure(session);
            }
            else
            {
                updateZeroResult = this.ExecuteClrProcedure(session);
            }
            object[] routineArguments = session.sessionContext.RoutineArguments;
            session.sessionContext.PopRoutineTables();
            session.sessionContext.Pop(true);
            bool flag = false;
            for (int j = 0; j < this.Procedure.GetParameterCount(); j++)
            {
                if (this.Procedure.GetParameter(j).GetParameterMode() != 1)
                {
                    if (this._arguments[j].IsDynamicParam())
                    {
                        int parameterIndex = this._arguments[j].ParameterIndex;
                        session.sessionContext.DynamicArguments[parameterIndex] = routineArguments[j];
                        flag = true;
                    }
                    else if (this._arguments[j].IsParameter())
                    {
                        int columnIndex = this._arguments[j].GetColumnIndex();
                        session.sessionContext.RoutineArguments[columnIndex] = routineArguments[j];
                    }
                    else
                    {
                        int columnIndex = this._arguments[j].GetColumnIndex();
                        session.sessionContext.RoutineVariables[columnIndex] = routineArguments[j];
                    }
                }
            }
            if (!updateZeroResult.IsError())
            {
                if (updateZeroResult.IsSimpleValue())
                {
                    updateZeroResult = Result.UpdateZeroResult;
                }
                if (flag)
                {
                    updateZeroResult = Result.NewCallResponse(this.GetParametersMetaData().GetParameterTypes(), base.Id, session.sessionContext.DynamicArguments);
                }
            }
            return updateZeroResult;
        }

        public override Result GetResult(Session session)
        {
            if (this._expression != null)
            {
                return this.GetExpressionResult(session);
            }
            return this.GetProcedureResult(session);
        }

        public override ResultMetaData GetResultMetaData()
        {
            if (this._resultMetaData != null)
            {
                return this._resultMetaData;
            }
            if (base.type != 7)
            {
                throw Error.RuntimeError(0xc9, "StatementProcedure");
            }
            if (this._expression == null)
            {
                return ResultMetaData.EmptyResultMetaData;
            }
            ResultMetaData data2 = ResultMetaData.NewResultMetaData(1);
            ColumnBase base2 = new ColumnBase(null, null, null, StatementDMQL.ReturnColumnName);
            base2.SetType(this._expression.GetDataType());
            data2.columns[0] = base2;
            data2.PrepareData();
            this._resultMetaData = data2;
            return data2;
        }

        public override SubQuery[] GetSubqueries(Session session)
        {
            OrderedHashSet<SubQuery> set = null;
            if (this._expression != null)
            {
                set = this._expression.CollectAllSubqueries(set);
            }
            for (int i = 0; i < this._arguments.Length; i++)
            {
                set = this._arguments[i].CollectAllSubqueries(set);
            }
            if ((set == null) || (set.Size() == 0))
            {
                return SubQuery.EmptySubqueryArray;
            }
            SubQuery[] a = new SubQuery[set.Size()];
            set.ToArray(a);
            ArraySort.Sort<SubQuery>(a, 0, a.Length, a[0]);
            for (int j = 0; j < base.Subqueries.Length; j++)
            {
                a[j].PrepareTable(session);
            }
            return a;
        }
    }
}

