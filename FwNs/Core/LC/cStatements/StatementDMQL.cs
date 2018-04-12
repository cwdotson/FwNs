namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class StatementDMQL : Statement
    {
        public static readonly string PcolPrefix = "@p";
        public static readonly string ReturnColumnName = "@p0";
        public ResultMetaData generatedResultMetaData;
        public int[] BaseColumnMap;
        public Table BaseTable;
        public int[] BaseUpdateColumnMap;
        public Expression condition;
        public int[] GeneratedIndexes;
        public bool[] InsertCheckColumns;
        public int[] InsertColumnMap;
        public Expression InsertExpression;
        public Expression[][] MultiColumnValues;
        public ResultMetaData ParameterMetaData;
        private QNameManager.SimpleName cursorName;
        public ExpressionColumn[] Parameters;
        public QueryExpression queryExpression;
        public int RangeIteratorCount;
        public RangeVariable[] RangeVariables;
        public bool RestartIdentity;
        public Routine[] Routines;
        public NumberSequence[] Sequences;
        public Table SourceTable;
        public SubQuery[] Subqueries;
        public RangeVariable[] TargetRangeVariables;
        public Table TargetTable;
        public bool[] UpdateCheckColumns;
        public int[] UpdateColumnMap;
        public Expression[] UpdateExpressions;

        protected StatementDMQL(int type, int group, QNameManager.QName schemaName) : base(type, group)
        {
            this.BaseUpdateColumnMap = new int[0];
            this.InsertColumnMap = new int[0];
            this.Subqueries = SubQuery.EmptySubqueryArray;
            this.TargetRangeVariables = RangeVariable.EmptyArray;
            this.UpdateColumnMap = new int[0];
            this.UpdateExpressions = Expression.emptyArray;
            base.SchemaName = schemaName;
            base.isTransactionStatement = true;
        }

        private StringBuilder AppendColumns(StringBuilder sb, int[] columnMap)
        {
            if ((columnMap != null) && (this.UpdateExpressions.Length != 0))
            {
                sb.Append("COLUMNS=[");
                for (int i = 0; i < columnMap.Length; i++)
                {
                    sb.Append('\n').Append(columnMap[i]).Append(':').Append(' ').Append(this.TargetTable.GetColumn(columnMap[i]).GetNameString()).Append('[').Append(this.UpdateExpressions[i]).Append(']');
                }
                sb.Append(']');
            }
            return sb;
        }

        private StringBuilder AppendCondition(Session session, StringBuilder sb)
        {
            if (this.condition != null)
            {
                return sb.Append("CONDITION[").Append(this.condition.Describe(session, 0)).Append("]\n");
            }
            return sb.Append("CONDITION[]\n");
        }

        private StringBuilder AppendMultiColumns(StringBuilder sb, int[] columnMap)
        {
            if ((columnMap != null) && (this.MultiColumnValues != null))
            {
                sb.Append("COLUMNS=[");
                for (int i = 0; i < this.MultiColumnValues.Length; i++)
                {
                    for (int j = 0; j < columnMap.Length; j++)
                    {
                        sb.Append('\n').Append(columnMap[j]).Append(':').Append(' ').Append(this.TargetTable.GetColumn(columnMap[j]).GetName().Name).Append('[').Append(this.MultiColumnValues[i][j]).Append(']');
                    }
                }
                sb.Append(']');
            }
            return sb;
        }

        private StringBuilder AppendParms(StringBuilder sb)
        {
            sb.Append("PARAMETERS=[");
            for (int i = 0; i < this.Parameters.Length; i++)
            {
                sb.Append('\n').Append('@').Append(i).Append('[').Append(this.Parameters[i]).Append(']');
            }
            sb.Append(']');
            return sb;
        }

        private StringBuilder AppendSubqueries(Session session, StringBuilder sb)
        {
            sb.Append("SUBQUERIES[");
            for (int i = 0; i < this.Subqueries.Length; i++)
            {
                sb.Append("\n[level=").Append(this.Subqueries[i].Level).Append('\n');
                if (this.Subqueries[i].queryExpression != null)
                {
                    sb.Append(this.Subqueries[i].queryExpression.Describe(session));
                }
                sb.Append("]");
            }
            sb.Append(']');
            return sb;
        }

        private StringBuilder AppendTable(StringBuilder sb)
        {
            sb.Append("TABLE[").Append(this.TargetTable.GetName().Name).Append(']');
            return sb;
        }

        public virtual void CheckAccessRights(Session session)
        {
            if ((this.TargetTable != null) && !this.TargetTable.IsTemp())
            {
                if (((this.TargetTable.GetOwner() != null) && this.TargetTable.GetOwner().IsSystem) && !session.GetUser().IsSystem)
                {
                    throw Error.GetError(0x157d, this.TargetTable.GetName().Name);
                }
                if (!session.IsProcessingScript())
                {
                    this.TargetTable.CheckDataReadOnly();
                }
                session.CheckReadWrite();
            }
            if (!session.IsAdmin())
            {
                for (int i = 0; i < this.Sequences.Length; i++)
                {
                    session.GetGrantee().CheckAccess(this.Sequences[i]);
                }
                for (int j = 0; j < this.Routines.Length; j++)
                {
                    if (!this.Routines[j].IsLibraryRoutine())
                    {
                        session.GetGrantee().CheckAccess(this.Routines[j]);
                    }
                }
                for (int k = 0; k < this.RangeVariables.Length; k++)
                {
                    RangeVariable variable = this.RangeVariables[k];
                    if (variable.RangeTable.GetSchemaName() != SqlInvariants.SystemSchemaQname)
                    {
                        session.GetGrantee().CheckSelect(variable.RangeTable, variable.UsedColumns);
                    }
                }
                int type = base.type;
                if (type <= 50)
                {
                    switch (type)
                    {
                        case 0x13:
                            session.GetGrantee().CheckDelete(this.TargetTable);
                            return;

                        case 50:
                            session.GetGrantee().CheckInsert(this.TargetTable, this.InsertCheckColumns);
                            return;
                    }
                }
                else if (type == 0x52)
                {
                    session.GetGrantee().CheckUpdate(this.TargetTable, this.UpdateCheckColumns);
                }
                else if ((type != 0x55) && (type == 0x80))
                {
                    session.GetGrantee().CheckInsert(this.TargetTable, this.InsertCheckColumns);
                    session.GetGrantee().CheckUpdate(this.TargetTable, this.UpdateCheckColumns);
                }
            }
        }

        public abstract void CollectTableNamesForRead(OrderedHashSet<QNameManager.QName> set);
        public abstract void CollectTableNamesForWrite(OrderedHashSet<QNameManager.QName> set);
        public override string Describe(Session session)
        {
            try
            {
                return this.DescribeImpl(session);
            }
            catch (Exception exception1)
            {
                return exception1.ToString();
            }
        }

        public virtual string DescribeImpl(Session session)
        {
            StringBuilder sb = new StringBuilder();
            switch (base.type)
            {
                case 7:
                    sb.Append("CALL");
                    sb.Append('[').Append(']');
                    return sb.ToString();

                case 0x13:
                    sb.Append("DELETE");
                    sb.Append('[').Append('\n');
                    this.AppendTable(sb).Append('\n');
                    this.AppendCondition(session, sb);
                    for (int i = 0; i < this.TargetRangeVariables.Length; i++)
                    {
                        sb.Append(this.TargetRangeVariables[i].Describe(session)).Append('\n');
                    }
                    this.AppendParms(sb).Append('\n');
                    this.AppendSubqueries(session, sb).Append(']');
                    return sb.ToString();

                case 50:
                    if (this.queryExpression == null)
                    {
                        sb.Append("INSERT VALUES");
                        sb.Append('[').Append('\n');
                        this.AppendMultiColumns(sb, this.InsertColumnMap).Append('\n');
                        this.AppendTable(sb).Append('\n');
                        this.AppendParms(sb).Append('\n');
                        this.AppendSubqueries(session, sb).Append(']');
                        return sb.ToString();
                    }
                    sb.Append("INSERT SELECT");
                    sb.Append('[').Append('\n');
                    this.AppendColumns(sb, this.InsertColumnMap).Append('\n');
                    this.AppendTable(sb).Append('\n');
                    sb.Append(this.queryExpression.Describe(session)).Append('\n');
                    this.AppendParms(sb).Append('\n');
                    this.AppendSubqueries(session, sb).Append(']');
                    return sb.ToString();

                case 0x52:
                    sb.Append("UPDATE");
                    sb.Append('[').Append('\n');
                    this.AppendColumns(sb, this.UpdateColumnMap).Append('\n');
                    this.AppendTable(sb).Append('\n');
                    this.AppendCondition(session, sb);
                    for (int i = 0; i < this.TargetRangeVariables.Length; i++)
                    {
                        sb.Append(this.TargetRangeVariables[i].Describe(session)).Append('\n');
                    }
                    this.AppendParms(sb).Append('\n');
                    this.AppendSubqueries(session, sb).Append(']');
                    return sb.ToString();

                case 0x55:
                    sb.Append(this.queryExpression.Describe(session));
                    this.AppendParms(sb).Append('\n');
                    this.AppendSubqueries(session, sb);
                    return sb.ToString();

                case 0x80:
                    sb.Append("MERGE");
                    sb.Append('[').Append('\n');
                    this.AppendMultiColumns(sb, this.InsertColumnMap).Append('\n');
                    this.AppendColumns(sb, this.UpdateColumnMap).Append('\n');
                    this.AppendTable(sb).Append('\n');
                    this.AppendCondition(session, sb);
                    for (int i = 0; i < this.TargetRangeVariables.Length; i++)
                    {
                        sb.Append(this.TargetRangeVariables[i].Describe(session)).Append('\n');
                    }
                    this.AppendParms(sb).Append('\n');
                    this.AppendSubqueries(session, sb).Append(']');
                    return sb.ToString();
            }
            return "UNKNOWN";
        }

        public override Result Execute(Session session)
        {
            Result result;
            if (((this.TargetTable != null) && session.IsReadOnly()) && !this.TargetTable.IsTemp())
            {
                return Result.NewErrorResult(Error.GetError(0xe7a));
            }
            if (base.IsExplain)
            {
                return this.GetExplainResult(session);
            }
            try
            {
                if (this.Subqueries.Length != 0)
                {
                    this.MaterializeSubQueries(session);
                }
                result = this.GetResult(session);
            }
            catch (Exception exception1)
            {
                result = Result.NewErrorResult(exception1, null);
                result.GetException().SetStatementType(base.Group, base.type);
            }
            session.sessionContext.ClearStructures(this);
            return result;
        }

        public override ResultMetaData GeneratedResultMetaData()
        {
            return this.generatedResultMetaData;
        }

        public QNameManager.SimpleName GetCursorName()
        {
            return this.cursorName;
        }

        private Result GetExplainResult(Session session)
        {
            Result result = Result.NewSingleColumnStringResult("OPERATION", this.Describe(session));
            OrderedHashSet<QNameManager.QName> references = this.GetReferences();
            object[] data = new object[] { "Object References" };
            result.navigator.Add(data);
            for (int i = 0; i < references.Size(); i++)
            {
                QNameManager.QName name = references.Get(i);
                object[] objArray2 = new object[] { name.GetSchemaQualifiedStatementName() };
                result.navigator.Add(objArray2);
            }
            object[] objArray3 = new object[] { "Read Locks" };
            result.navigator.Add(objArray3);
            for (int j = 0; j < base.ReadTableNames.Length; j++)
            {
                QNameManager.QName name2 = base.ReadTableNames[j];
                object[] objArray4 = new object[] { name2.GetSchemaQualifiedStatementName() };
                result.navigator.Add(objArray4);
            }
            object[] objArray5 = new object[] { "WriteLocks" };
            result.navigator.Add(objArray5);
            for (int k = 0; k < base.WriteTableNames.Length; k++)
            {
                QNameManager.QName name3 = base.WriteTableNames[k];
                object[] objArray6 = new object[] { name3.GetSchemaQualifiedStatementName() };
                result.navigator.Add(objArray6);
            }
            return result;
        }

        public object[] GetGeneratedColumns(object[] data)
        {
            if (this.GeneratedIndexes == null)
            {
                return null;
            }
            object[] objArray2 = new object[this.GeneratedIndexes.Length];
            for (int i = 0; i < this.GeneratedIndexes.Length; i++)
            {
                objArray2[i] = data[this.GeneratedIndexes[i]];
            }
            return objArray2;
        }

        public override ResultMetaData GetParametersMetaData()
        {
            return this.ParameterMetaData;
        }

        public override RangeVariable[] GetRangeVariables()
        {
            return this.RangeVariables;
        }

        public abstract Result GetResult(Session session);
        public override ResultMetaData GetResultMetaData()
        {
            switch (base.type)
            {
                case 0x52:
                case 0x80:
                case 0x13:
                case 50:
                    return ResultMetaData.EmptyResultMetaData;
            }
            throw Error.RuntimeError(0xc9, "StatementDMQL");
        }

        public virtual SubQuery[] GetSubqueries(Session session)
        {
            OrderedHashSet<SubQuery> first = null;
            for (int i = 0; i < this.TargetRangeVariables.Length; i++)
            {
                if (this.TargetRangeVariables[i] != null)
                {
                    OrderedHashSet<SubQuery> subqueries = this.TargetRangeVariables[i].GetSubqueries();
                    first = OrderedHashSet<SubQuery>.AddAll(first, subqueries);
                }
            }
            for (int j = 0; j < this.UpdateExpressions.Length; j++)
            {
                first = this.UpdateExpressions[j].CollectAllSubqueries(first);
            }
            if (this.InsertExpression != null)
            {
                first = this.InsertExpression.CollectAllSubqueries(first);
            }
            if (this.condition != null)
            {
                first = this.condition.CollectAllSubqueries(first);
            }
            if (this.queryExpression != null)
            {
                OrderedHashSet<SubQuery> subqueries = this.queryExpression.GetSubqueries();
                first = OrderedHashSet<SubQuery>.AddAll(first, subqueries);
            }
            if ((first == null) || (first.Size() == 0))
            {
                return SubQuery.EmptySubqueryArray;
            }
            SubQuery[] a = new SubQuery[first.Size()];
            first.ToArray(a);
            ArraySort.Sort<SubQuery>(a, 0, a.Length, a[0]);
            for (int k = 0; k < a.Length; k++)
            {
                a[k].PrepareTable(session);
            }
            return a;
        }

        public override bool HasGeneratedColumns()
        {
            return (this.GeneratedIndexes > null);
        }

        public override bool IsCatalogChange()
        {
            return false;
        }

        public void MaterializeSubQueries(Session session)
        {
            HashSet<SubQuery> set = new HashSet<SubQuery>();
            for (int i = 0; i < this.Subqueries.Length; i++)
            {
                SubQuery item = this.Subqueries[i];
                if (set.Add(item) && !item.IsCorrelated())
                {
                    item.Materialise(session);
                }
            }
        }

        public override void Resolve(Session session, RangeVariable[] rangeVars)
        {
        }

        public void SetCursorName(QNameManager.SimpleName name)
        {
            this.cursorName = name;
        }

        public void SetDatabseObjects(Session session, ParserDQL.CompileContext compileContext)
        {
            this.Parameters = compileContext.GetParameters();
            this.SetParameters();
            this.SetParameterMetaData();
            this.Subqueries = this.GetSubqueries(session);
            this.RangeIteratorCount = compileContext.GetRangeVarCount();
            this.RangeVariables = compileContext.GetRangeVariables();
            this.Sequences = compileContext.GetSequences();
            this.Routines = compileContext.GetRoutines();
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            this.CollectTableNamesForWrite(set);
            if (set.Size() > 0)
            {
                base.WriteTableNames = new QNameManager.QName[set.Size()];
                set.ToArray(base.WriteTableNames);
                set.Clear();
            }
            this.CollectTableNamesForRead(set);
            set.RemoveAll(base.WriteTableNames);
            if (set.Size() > 0)
            {
                base.ReadTableNames = new QNameManager.QName[set.Size()];
                set.ToArray(base.ReadTableNames);
            }
            base.References = compileContext.GetSchemaObjectNames();
            if (this.TargetTable != null)
            {
                base.References.Add(this.TargetTable.GetName());
            }
        }

        public void SetParameterMetaData()
        {
            int num = 0;
            if (this.Parameters.Length == 0)
            {
                this.ParameterMetaData = ResultMetaData.EmptyParamMetaData;
            }
            else
            {
                this.ParameterMetaData = ResultMetaData.NewParameterMetaData(this.Parameters.Length);
                for (int i = 0; i < this.Parameters.Length; i++)
                {
                    int index = i + num;
                    if (this.Parameters[i].OpType == 0x62)
                    {
                        if (!this.ParameterMetaData.IsNamedParameters)
                        {
                            if (i != 0)
                            {
                                throw Error.GetError(0x1a91);
                            }
                            this.ParameterMetaData.IsNamedParameters = true;
                        }
                        this.ParameterMetaData.ColumnLabels[index] = this.Parameters[i].ColumnName;
                    }
                    else
                    {
                        if (this.ParameterMetaData.IsNamedParameters)
                        {
                            throw Error.GetError(0x1a91);
                        }
                        this.ParameterMetaData.ColumnLabels[index] = PcolPrefix + (i + 1);
                    }
                    this.ParameterMetaData.ColumnTypes[index] = this.Parameters[i].DataType;
                    byte parameterMode = 1;
                    if ((this.Parameters[i].column != null) && (this.Parameters[i].column.GetParameterMode() != 0))
                    {
                        parameterMode = this.Parameters[i].column.GetParameterMode();
                    }
                    this.ParameterMetaData.ParamModes[index] = parameterMode;
                    this.ParameterMetaData.ParamNullable[index] = (this.Parameters[i].column == null) ? ((byte) 1) : this.Parameters[i].column.GetNullability();
                }
            }
        }

        private void SetParameters()
        {
            for (int i = 0; i < this.Parameters.Length; i++)
            {
                this.Parameters[i].ParameterIndex = i;
            }
        }
    }
}

