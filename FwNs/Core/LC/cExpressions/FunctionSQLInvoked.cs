namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class FunctionSQLInvoked : Expression
    {
        public Routine routine;
        public RoutineSchema routineSchema;
        private static object[] EmptyObjectArray = new object[0];

        public FunctionSQLInvoked(RoutineSchema routineSchema) : base(routineSchema.IsAggregate() ? 0x68 : 0x1b)
        {
            this.routineSchema = routineSchema;
        }

        public FunctionSQLInvoked(int type, RoutineSchema routineSchema) : base(type)
        {
            this.routineSchema = routineSchema;
        }

        public override void CollectObjectNames(FwNs.Core.LC.cLib.ISet<QNameManager.QName> set)
        {
            set.Add(this.routine.GetSpecificName());
        }

        public override object GetAggregatedValue(Session session, object currValue)
        {
            object[] aggregateData = ((object[]) currValue) ?? new object[3];
            aggregateData[0] = true;
            Result valueInternal = this.GetValueInternal(session, aggregateData);
            if (valueInternal.IsError())
            {
                throw valueInternal.GetException();
            }
            return valueInternal.GetValueObject();
        }

        public override Result GetResult(Session session)
        {
            object valueInternal = this.GetValueInternal(session, null);
            Result result = valueInternal as Result;
            if (result != null)
            {
                return result;
            }
            return Result.NewPsmResult(valueInternal);
        }

        public override string GetSql()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.routineSchema.GetName().GetSchemaQualifiedStatementName());
            builder.Append('(');
            int length = base.nodes.Length;
            if (base.OpType == 0x68)
            {
                length = 1;
            }
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(base.nodes[i].GetSql());
                builder.Append(')');
            }
            return builder.ToString();
        }

        public override object GetValue(Session session)
        {
            if (base.OpType == 5)
            {
                return session.sessionContext.RangeIterators[base.RangePosition].GetCurrent()[base.ColumnIndex];
            }
            Result valueInternal = this.GetValueInternal(session, null);
            object valueObject = valueInternal;
            if (valueInternal != null)
            {
                if (valueInternal.IsError())
                {
                    throw valueInternal.GetException();
                }
                if (valueInternal.IsSimpleValue())
                {
                    valueObject = valueInternal.GetValueObject();
                }
                else
                {
                    if (!valueInternal.IsData())
                    {
                        throw Error.GetError(0x11fd, this.routine.GetName().Name);
                    }
                    valueObject = valueInternal;
                }
            }
            return valueObject;
        }

        public Result GetValueInternal(Session session, object[] aggregateData)
        {
            Result result;
            int variableCount = this.routine.GetVariableCount();
            int num2 = this.routine.CSharpMethodWithConnection ? 1 : 0;
            object[] emptyObjectArray = EmptyObjectArray;
            if ((num2 + base.nodes.Length) > 0)
            {
                if (base.OpType == 0x68)
                {
                    emptyObjectArray = new object[this.routine.GetParameterCount()];
                    for (int j = 0; j < aggregateData.Length; j++)
                    {
                        emptyObjectArray[(emptyObjectArray.Length - 3) + j] = aggregateData[j];
                    }
                }
                else
                {
                    emptyObjectArray = new object[base.nodes.Length + num2];
                }
                if (num2 > 0)
                {
                    emptyObjectArray[0] = session.GetInternalConnection();
                }
            }
            SqlType[] parameterTypes = this.routine.GetParameterTypes();
            for (int i = 0; i < base.nodes.Length; i++)
            {
                Expression expression = base.nodes[i];
                object a = expression.GetValue(session, parameterTypes[i]);
                if (a == null)
                {
                    if (this.routine.IsNullInputOutput())
                    {
                        return null;
                    }
                    if (!this.routine.GetParameter(i).IsNullable())
                    {
                        throw Error.GetError(0x12cb);
                    }
                }
                if (this.routine.IsPsm())
                {
                    emptyObjectArray[i] = a;
                }
                else
                {
                    emptyObjectArray[i + num2] = (a == null) ? null : expression.DataType.ConvertSQLToCSharp(session, a);
                }
            }
            session.sessionContext.Push(true);
            if (this.routine.IsPsm())
            {
                try
                {
                    session.sessionContext.RoutineArguments = emptyObjectArray;
                    if (variableCount > 0)
                    {
                        session.sessionContext.RoutineVariables = new object[variableCount];
                    }
                    else
                    {
                        session.sessionContext.RoutineVariables = EmptyObjectArray;
                    }
                    result = this.routine.statement.Execute(session);
                    if (aggregateData != null)
                    {
                        for (int j = 0; j < aggregateData.Length; j++)
                        {
                            aggregateData[j] = emptyObjectArray[j + 1];
                        }
                    }
                }
                catch (Exception exception1)
                {
                    result = Result.NewErrorResult(exception1);
                }
            }
            else
            {
                if (base.OpType == 0x68)
                {
                    emptyObjectArray = this.routine.ConvertArgsToCSharp(session, emptyObjectArray);
                }
                result = this.routine.InvokeClrMethod(session, emptyObjectArray);
                if (base.OpType == 0x68)
                {
                    object[] callArguments = new object[emptyObjectArray.Length];
                    this.routine.ConvertArgsToSql(session, callArguments, emptyObjectArray);
                    for (int j = 0; j < aggregateData.Length; j++)
                    {
                        aggregateData[j] = callArguments[(emptyObjectArray.Length - 3) + j];
                    }
                }
            }
            session.sessionContext.Pop(true);
            if (result.IsError())
            {
                throw result.GetException();
            }
            return result;
        }

        public bool IsDeterministic()
        {
            return this.routine.IsDeterministic();
        }

        public override bool IsSelfAggregate()
        {
            return this.routineSchema.IsAggregate();
        }

        public override List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            if (this.IsSelfAggregate())
            {
                if (unresolvedSet == null)
                {
                    unresolvedSet = new List<Expression>();
                }
                unresolvedSet.Add(this);
                return unresolvedSet;
            }
            return base.ResolveColumnReferences(rangeVarArray, rangeCount, unresolvedSet, acceptsSequences);
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            SqlType[] types = new SqlType[base.nodes.Length];
            for (int i = 0; i < base.nodes.Length; i++)
            {
                Expression expression = base.nodes[i];
                expression.ResolveTypes(session, this);
                types[i] = expression.DataType;
            }
            this.routine = this.routineSchema.GetSpecificRoutine(types);
            for (int j = 0; j < base.nodes.Length; j++)
            {
                if (base.nodes[j].DataType == null)
                {
                    base.nodes[j].DataType = this.routine.GetParameterTypes()[j];
                }
            }
            base.DataType = this.routine.GetReturnType();
        }

        public void SetArguments(Expression[] newNodes)
        {
            base.nodes = newNodes;
        }

        public override object UpdateAggregatingValue(Session session, object currValue)
        {
            object[] aggregateData = ((object[]) currValue) ?? new object[3];
            aggregateData[0] = false;
            this.GetValueInternal(session, aggregateData);
            return aggregateData;
        }
    }
}

