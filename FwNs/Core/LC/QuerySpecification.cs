namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class QuerySpecification : QueryExpression
    {
        private static readonly int[] DefaultLimits;
        private readonly List<Expression> _exprColumnList;
        private readonly List<RangeVariable> _rangeVariableList;
        private readonly OrderedHashSet<object> _tempSet;
        private readonly OrderedHashSet<Expression> _tempSetExpr;
        public bool IsAggregated;
        public bool IsDistinctSelect;
        public bool IsGrouped;
        public int ResultRangePosition;
        private bool[] _aggregateCheck;
        private List<Expression> _aggregateSet;
        private Table _baseTable;
        private int[] _columnMap;
        private SqlType[] _columnTypes;
        private int _groupByColumnCount;
        private bool _hasMemoryRow;
        private int _havingColumnCount;
        private Expression _havingCondition;
        private int _indexLimitExpressions;
        private int _indexLimitRowId;
        private int _indexStartHaving;
        public int IndexStartOrderBy;
        private List<Expression> _resolvedSubqueryExpressions;
        private bool _simpleLimit;
        public Expression CheckQueryCondition;
        public Expression[] ExprColumns;
        public Index GroupIndex;
        public int IndexLimitData;
        public int IndexLimitVisible;
        public int IndexStartAggregates;
        private bool _isSimpleCount;
        public Expression QueryCondition;
        public RangeVariable[] RangeVariables;
        public Expression RowExpression;

        static QuerySpecification()
        {
            int[] numArray1 = new int[3];
            numArray1[1] = 0x7fffffff;
            numArray1[2] = 0x7fffffff;
            DefaultLimits = numArray1;
        }

        public QuerySpecification(ParserDQL.CompileContext compileContext) : base(compileContext)
        {
            this._tempSet = new OrderedHashSet<object>();
            this._tempSetExpr = new OrderedHashSet<Expression>();
            this._simpleLimit = true;
            base.compileContext = compileContext;
            this.ResultRangePosition = compileContext.GetNextRangeVarIndex();
            this._rangeVariableList = new List<RangeVariable>();
            this._exprColumnList = new List<Expression>();
            base.ExprSortAndSlice = new SortAndSlice();
            base.IsMergeable = true;
        }

        public QuerySpecification(Session session, Table table, ParserDQL.CompileContext compileContext) : this(compileContext)
        {
            RangeVariable rangeVar = new RangeVariable(table, null, null, null, compileContext);
            rangeVar.AddTableColumns(this._exprColumnList, 0, null);
            this.IndexLimitVisible = this._exprColumnList.Count;
            this.AddRangeVariable(rangeVar);
            base.IsMergeable = false;
            base.IsFullOrder = true;
            base.ExprSortAndSlice = new SortAndSlice();
        }

        private void AddAllJoinedColumns(Expression e)
        {
            List<Expression> exprList = new List<Expression>();
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                this.RangeVariables[i].AddTableColumns(exprList);
            }
            Expression[] expressionArray = exprList.ToArray();
            e.nodes = expressionArray;
        }

        public void AddGroupByColumnExpression(Expression e)
        {
            if (e.GetExprType() == 0x19)
            {
                throw Error.GetError(0x15bc);
            }
            this._exprColumnList.Add(e);
            this.IsGrouped = true;
            this._groupByColumnCount++;
        }

        public void AddHavingExpression(Expression e)
        {
            this._exprColumnList.Add(e);
            this._havingCondition = e;
            this._havingColumnCount = 1;
        }

        public void AddQueryCondition(Expression e)
        {
            this.QueryCondition = e;
        }

        public void AddRangeVariable(RangeVariable rangeVar)
        {
            this._rangeVariableList.Add(rangeVar);
        }

        public void AddSelectColumnExpression(Expression e)
        {
            if (e.GetExprType() == 0x19)
            {
                throw Error.GetError(0x15bc);
            }
            if (this.IndexLimitVisible > 0)
            {
                if ((e.OpType == 0x61) && (((ExpressionColumn) e).GetTableName() == null))
                {
                    throw Error.GetError(0x15ca);
                }
                Expression expression = this._exprColumnList[0];
                if ((expression.OpType == 0x61) && (((ExpressionColumn) expression).GetTableName() == null))
                {
                    throw Error.GetError(0x15ca);
                }
            }
            this._exprColumnList.Add(e);
            this.IndexLimitVisible++;
        }

        public override void AddSortAndSlice(SortAndSlice sortAndSlice)
        {
            base.ExprSortAndSlice = sortAndSlice;
        }

        public override bool AreColumnsResolved()
        {
            if (base.UnresolvedExpressions != null)
            {
                return (base.UnresolvedExpressions.Count == 0);
            }
            return true;
        }

        private Result BuildResult(Session session, int limitcount)
        {
            RowSetNavigatorData nav = new RowSetNavigatorData(session, this);
            Result result = Result.NewResult(nav);
            result.MetaData = base.resultMetaData;
            if (base.IsUpdatable)
            {
                result.RsProperties = ResultProperties.UpdatablePropsValue;
            }
            if (this._isSimpleCount)
            {
                object[] data = new object[this.IndexLimitData];
                Table table = this.RangeVariables[0].GetTable();
                IPersistentStore rowStore = table.GetRowStore(session);
                int num = table.GetIndex(0).Size(session, rowStore);
                data[0] = data[this.IndexStartAggregates] = num;
                nav.Add(data);
                return result;
            }
            int num2 = 0;
            IRangeIterator[] iteratorArray = new IRangeIterator[this.RangeVariables.Length];
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                iteratorArray[i] = this.RangeVariables[i].GetIterator(session);
            }
            int index = 0;
        Label_00CD:
            if (index < num2)
            {
                bool flag = true;
                for (int k = num2 + 1; k < this.RangeVariables.Length; k++)
                {
                    if (this.RangeVariables[k].IsRightJoin)
                    {
                        num2 = k;
                        index = k;
                        flag = false;
                        ((RangeVariable.RangeIteratorRight) iteratorArray[k]).SetOnOuterRows();
                        break;
                    }
                }
                if (flag)
                {
                    goto Label_02CB;
                }
            }
            IRangeIterator iterator = iteratorArray[index];
            if (iterator.Next())
            {
                if (index < (this.RangeVariables.Length - 1))
                {
                    index++;
                    goto Label_00CD;
                }
                session.sessionData.StartRowProcessing();
                object[] data = new object[this.IndexLimitData];
                for (int k = 0; k < this.IndexStartAggregates; k++)
                {
                    if (!this.IsAggregated || !this._aggregateCheck[k])
                    {
                        data[k] = this.ExprColumns[k].GetValue(session);
                    }
                }
                for (int m = this.IndexLimitVisible; m < this._indexLimitRowId; m++)
                {
                    if (m == this.IndexLimitVisible)
                    {
                        data[m] = iterator.GetRowidObject();
                    }
                    else
                    {
                        data[m] = iterator.GetCurrentRow();
                    }
                }
                object[] oldData = null;
                if (this.IsAggregated || this.IsGrouped)
                {
                    oldData = nav.GetGroupData(data);
                    if (oldData != null)
                    {
                        data = oldData;
                    }
                }
                for (int n = this.IndexStartAggregates; n < this._indexLimitExpressions; n++)
                {
                    data[n] = this.ExprColumns[n].UpdateAggregatingValue(session, data[n]);
                }
                if (oldData == null)
                {
                    nav.Add(data);
                }
                else if (this.IsAggregated)
                {
                    nav.Update(oldData, data);
                }
                int size = nav.GetSize();
                if (((size == session.ResultMaxMemoryRows) && !this.IsAggregated) && !this._hasMemoryRow)
                {
                    RowSetNavigatorDataTable table2 = new RowSetNavigatorDataTable(session, this, nav);
                    table2.Initialize(nav);
                    nav = table2;
                    result.SetNavigator(nav);
                }
                if (((this.IsAggregated || this.IsGrouped) && !base.ExprSortAndSlice.IsGenerated) || (size < limitcount))
                {
                    goto Label_00CD;
                }
            }
            else
            {
                iterator.Reset();
                index--;
                goto Label_00CD;
            }
        Label_02CB:
            nav.Reset();
            for (int j = 0; j < this.RangeVariables.Length; j++)
            {
                iteratorArray[j].Reset();
            }
            if (this.IsGrouped || this.IsAggregated)
            {
                if (this.IsAggregated)
                {
                    if (!this.IsGrouped && (nav.GetSize() == 0))
                    {
                        object[] data = new object[this.ExprColumns.Length];
                        nav.Add(data);
                    }
                    nav.Reset();
                    session.sessionContext.SetRangeIterator(nav);
                    while (nav.Next())
                    {
                        object[] current = nav.GetCurrent();
                        for (int k = this.IndexStartAggregates; k < this._indexLimitExpressions; k++)
                        {
                            current[k] = this.ExprColumns[k].GetAggregatedValue(session, current[k]);
                        }
                        for (int m = 0; m < this.IndexStartAggregates; m++)
                        {
                            if (this._aggregateCheck[m])
                            {
                                current[m] = this.ExprColumns[m].GetValue(session);
                            }
                        }
                    }
                }
                nav.Reset();
                if (this._havingCondition != null)
                {
                    while (nav.HasNext())
                    {
                        object[] next = nav.GetNext();
                        bool flag2 = true;
                        if (!flag2.Equals(next[this.IndexLimitVisible + this._groupByColumnCount]))
                        {
                            nav.Remove();
                        }
                    }
                    nav.Reset();
                }
            }
            return result;
        }

        private void CheckLobUsage()
        {
            if (this.IsDistinctSelect || this.IsGrouped)
            {
                for (int i = 0; i < this._indexStartHaving; i++)
                {
                    if (this.ExprColumns[i].DataType.IsLobType())
                    {
                        throw Error.GetError(0x159e);
                    }
                }
            }
        }

        public override OrderedHashSet<Expression> CollectAllExpressions(OrderedHashSet<Expression> set, OrderedIntHashSet typeSet, OrderedIntHashSet stopAtTypeSet)
        {
            for (int i = 0; i < this.IndexStartAggregates; i++)
            {
                set = this.ExprColumns[i].CollectAllExpressions(set, typeSet, stopAtTypeSet);
            }
            if (this.QueryCondition != null)
            {
                set = this.QueryCondition.CollectAllExpressions(set, typeSet, stopAtTypeSet);
            }
            if (this._havingCondition != null)
            {
                set = this._havingCondition.CollectAllExpressions(set, typeSet, stopAtTypeSet);
            }
            return set;
        }

        public override void CollectObjectNames(FwNs.Core.LC.cLib.ISet<QNameManager.QName> set)
        {
            for (int i = 0; i < this.IndexStartAggregates; i++)
            {
                this.ExprColumns[i].CollectObjectNames(set);
            }
            if (this.QueryCondition != null)
            {
                this.QueryCondition.CollectObjectNames(set);
            }
            if (this._havingCondition != null)
            {
                this._havingCondition.CollectObjectNames(set);
            }
            int index = 0;
            int length = this.RangeVariables.Length;
            while (index < length)
            {
                QNameManager.QName o = this.RangeVariables[index].GetTable().GetName();
                set.Add(o);
                index++;
            }
        }

        public void CollectRangeVariables(RangeVariable[] rangeVars, FwNs.Core.LC.cLib.ISet<RangeVariable> set)
        {
            for (int i = 0; i < this.IndexStartAggregates; i++)
            {
                this.ExprColumns[i].CollectRangeVariables(rangeVars, set);
            }
            if (this.QueryCondition != null)
            {
                this.QueryCondition.CollectRangeVariables(rangeVars, set);
            }
            if (this._havingCondition != null)
            {
                this._havingCondition.CollectRangeVariables(rangeVars, set);
            }
        }

        public static void CollectSubQueriesAndReferences(OrderedHashSet<object> set, Expression expression)
        {
            OrderedHashSet<Expression> set2 = new OrderedHashSet<Expression>();
            expression.CollectAllExpressions(set2, Expression.SubqueryExpressionSet, Expression.EmptyExpressionSet);
            set.AddAll<Expression>(set2);
            int num = set.Size();
            OrderedHashSet<QNameManager.QName> set3 = new OrderedHashSet<QNameManager.QName>();
            for (int i = 0; i < num; i++)
            {
                set3.Clear();
                ((Expression) set.Get(i)).CollectObjectNames(set3);
                set.AddAll<QNameManager.QName>(set3);
            }
        }

        public void CreateFullIndex(Session session)
        {
            int[] colindex = new int[this.IndexLimitVisible];
            ArrayUtil.FillSequence(colindex);
            base.FullIndex = base.ResultTable.CreateAndAddIndexStructure(session, null, colindex, null, null, false, false, false);
            base.ResultTable.FullIndex = base.FullIndex;
        }

        private void CreateResultMetaData()
        {
            this._columnTypes = new SqlType[this.IndexLimitData];
            for (int i = 0; i < this.IndexStartAggregates; i++)
            {
                this._columnTypes[i] = this.ExprColumns[i].GetDataType();
            }
            for (int j = this.IndexLimitVisible; j < this._indexLimitRowId; j++)
            {
                if (j == this.IndexLimitVisible)
                {
                    this._columnTypes[j] = SqlType.SqlBigint;
                }
                else
                {
                    this._columnTypes[j] = SqlType.SqlAllTypes;
                }
            }
            for (int k = this._indexLimitRowId; k < this.IndexLimitData; k++)
            {
                this._columnTypes[k] = this.ExprColumns[k].GetDataType();
            }
            base.resultMetaData = ResultMetaData.NewResultMetaData(this._columnTypes, this._columnMap, this.IndexLimitVisible, this._indexLimitRowId);
            for (int m = 0; m < this.IndexLimitVisible; m++)
            {
                Expression expression3 = this.ExprColumns[m];
                base.resultMetaData.ColumnTypes[m] = expression3.GetDataType();
                if (m < this.IndexLimitVisible)
                {
                    ColumnBase column = expression3.GetColumn();
                    if (column != null)
                    {
                        base.resultMetaData.columns[m] = column;
                        base.resultMetaData.ColumnLabels[m] = expression3.GetAlias();
                    }
                    else
                    {
                        column = new ColumnBase();
                        column.SetType(expression3.GetDataType());
                        base.resultMetaData.columns[m] = column;
                        base.resultMetaData.ColumnLabels[m] = expression3.GetAlias();
                    }
                }
            }
        }

        public override void CreateResultTable(Session session)
        {
            QNameManager.QName subqueryTableName = session.database.NameManager.GetSubqueryTableName();
            int type = (base.PersistenceScope == 0x15) ? 2 : 9;
            HashMappedList<string, ColumnSchema> columnList = new HashMappedList<string, ColumnSchema>();
            byte[] columnNullability = new byte[this.IndexLimitVisible];
            for (int i = 0; i < this.IndexLimitVisible; i++)
            {
                Expression expression = this.ExprColumns[i];
                QNameManager.SimpleName simpleName = expression.GetSimpleName();
                string name = simpleName.Name;
                if (!base.AccessibleColumns[i])
                {
                    name = QNameManager.GetAutoNoNameColumnString(i);
                }
                columnNullability[i] = expression.GetNullability();
                ColumnSchema schema = new ColumnSchema(session.database.NameManager.NewColumnSchemaQName(subqueryTableName, simpleName), expression.DataType, expression.GetNullability() > 0, false, null);
                columnList.Add(name, schema);
            }
            try
            {
                base.ResultTable = new TableDerived(session.database, subqueryTableName, type, this._columnTypes, columnNullability, columnList, null, null);
            }
            catch (Exception)
            {
            }
        }

        public override void CreateTable(Session session)
        {
            this.CreateResultTable(session);
            base.MainIndex = base.ResultTable.GetPrimaryIndex();
            if (base.ExprSortAndSlice.HasOrder() && !base.ExprSortAndSlice.SkipSort)
            {
                base.OrderIndex = base.ResultTable.CreateAndAddIndexStructure(session, null, base.ExprSortAndSlice.SortOrder, base.ExprSortAndSlice.SortDescending, base.ExprSortAndSlice.SortNullsLast, false, false, false);
            }
            if (this.IsDistinctSelect || base.IsFullOrder)
            {
                this.CreateFullIndex(session);
            }
            if (this.IsGrouped)
            {
                int[] columns = new int[this._groupByColumnCount];
                for (int i = 0; i < this._groupByColumnCount; i++)
                {
                    columns[i] = this.IndexLimitVisible + i;
                }
                this.GroupIndex = base.ResultTable.CreateAndAddIndexStructure(session, null, columns, null, null, false, false, false);
            }
            else if (this.IsAggregated)
            {
                this.GroupIndex = base.MainIndex;
            }
            if (base.IsUpdatable && (base.view == null))
            {
                int[] columns = new int[] { this.IndexLimitVisible };
                base.IdIndex = base.ResultTable.CreateAndAddIndexStructure(session, null, columns, null, null, false, false, false);
            }
        }

        public override string Describe(Session session)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString()).Append("[\n");
            if (base.ExprSortAndSlice.LimitCondition != null)
            {
                builder.Append("offset=[").Append(base.ExprSortAndSlice.LimitCondition.GetLeftNode().Describe(session, 0)).Append("]\n");
                builder.Append("limit=[").Append(base.ExprSortAndSlice.LimitCondition.GetRightNode().Describe(session, 0)).Append("]\n");
            }
            builder.Append("isDistinctSelect=[").Append(this.IsDistinctSelect).Append("]\n");
            builder.Append("isGrouped=[").Append(this.IsGrouped).Append("]\n");
            builder.Append("isAggregated=[").Append(this.IsAggregated).Append("]\n");
            builder.Append("columns=[");
            for (int i = 0; i < this.IndexLimitVisible; i++)
            {
                int index = i;
                if (this.ExprColumns[i].GetExprType() == 5)
                {
                    index = this.ExprColumns[i].ColumnIndex;
                }
                builder.Append(this.ExprColumns[index].Describe(session, 0));
            }
            builder.Append("\n]\n");
            builder.Append("range variables=[\n");
            for (int j = 0; j < this.RangeVariables.Length; j++)
            {
                builder.Append("[\n");
                builder.Append(this.RangeVariables[j].Describe(session));
                builder.Append("\n]");
            }
            builder.Append("]\n");
            if (this.QueryCondition != null)
            {
                this.QueryCondition.Describe(session, 0);
            }
            builder.Append("groupColumns=[");
            for (int k = this._indexLimitRowId; k < (this._indexLimitRowId + this._groupByColumnCount); k++)
            {
                int index = k;
                if (this.ExprColumns[k].GetExprType() == 5)
                {
                    index = this.ExprColumns[k].ColumnIndex;
                }
                builder.Append(this.ExprColumns[index].Describe(session, 0));
            }
            builder.Append("]\n");
            string str = (this._havingCondition == null) ? "null" : this._havingCondition.Describe(session, 0);
            builder.Append("havingCondition=[").Append(str).Append("]\n");
            return builder.ToString();
        }

        private void FinaliseColumns()
        {
            this._indexLimitRowId = this.IndexLimitVisible;
            this._indexStartHaving = this._indexLimitRowId + this._groupByColumnCount;
            this.IndexStartOrderBy = this._indexStartHaving + this._havingColumnCount;
            this.IndexStartAggregates = this.IndexStartOrderBy + base.ExprSortAndSlice.GetOrderLength();
            this.IndexLimitData = this._indexLimitExpressions = this.IndexStartAggregates;
            this.ExprColumns = new Expression[this._indexLimitExpressions];
            this._exprColumnList.CopyTo(this.ExprColumns);
            for (int i = 0; i < this.IndexLimitVisible; i++)
            {
                this.ExprColumns[i].QueryTableColumnIndex = i;
            }
            if (base.ExprSortAndSlice.HasOrder())
            {
                for (int j = 0; j < base.ExprSortAndSlice.GetOrderLength(); j++)
                {
                    this.ExprColumns[this.IndexStartOrderBy + j] = base.ExprSortAndSlice.ExprList[j];
                }
            }
            this.RowExpression = new Expression(0x19, this.ExprColumns);
        }

        private static byte GetAggregateNullability(int lNullability, int rNullability)
        {
            if ((lNullability == 0) && (rNullability == 0))
            {
                return 0;
            }
            if ((lNullability == 2) || (rNullability == 2))
            {
                return 2;
            }
            return 1;
        }

        private HashSet<string> GetAllNamedJoinColumns()
        {
            HashSet<string> set = null;
            for (int i = 0; i < this._rangeVariableList.Count; i++)
            {
                RangeVariable variable = this._rangeVariableList[i];
                if (variable.NamedJoinColumns != null)
                {
                    if (set == null)
                    {
                        set = new HashSet<string>();
                    }
                    Iterator<string> iterator = variable.NamedJoinColumns.GetIterator();
                    while (iterator.HasNext())
                    {
                        set.Add(iterator.Next());
                    }
                }
            }
            return set;
        }

        public override Table GetBaseTable()
        {
            return this._baseTable;
        }

        public override int[] GetBaseTableColumnMap()
        {
            return this._columnMap;
        }

        public override void GetBaseTableNames(OrderedHashSet<QNameManager.QName> set)
        {
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                Table rangeTable = this.RangeVariables[i].RangeTable;
                QNameManager.QName key = rangeTable.GetName();
                if ((!rangeTable.IsReadOnly() && !rangeTable.IsTemp()) && (key.schema != SqlInvariants.SystemSchemaQname))
                {
                    set.Add(key);
                }
            }
        }

        public override Expression GetCheckCondition()
        {
            return this.QueryCondition;
        }

        public override int GetColumnCount()
        {
            return this.IndexLimitVisible;
        }

        public override string[] GetColumnNames()
        {
            string[] strArray = new string[this.IndexLimitVisible];
            for (int i = 0; i < this.IndexLimitVisible; i++)
            {
                strArray[i] = this.ExprColumns[i].GetAlias();
            }
            return strArray;
        }

        public override SqlType[] GetColumnTypes()
        {
            if (this._columnTypes.Length == this.IndexLimitVisible)
            {
                return this._columnTypes;
            }
            SqlType[] destinationArray = new SqlType[this.IndexLimitVisible];
            Array.Copy(this._columnTypes, destinationArray, destinationArray.Length);
            return destinationArray;
        }

        public Expression GetEquiJoinExpressions(OrderedHashSet<string> nameSet, RangeVariable rightRange, bool fullList)
        {
            HashSet<string> set = new HashSet<string>();
            Expression expression = null;
            OrderedHashSet<string> columns = new OrderedHashSet<string>();
            for (int i = 0; i < this._rangeVariableList.Count; i++)
            {
                RangeVariable leftRangeVar = this._rangeVariableList[i];
                HashMappedList<string, ColumnSchema> columnList = leftRangeVar.RangeTable.ColumnList;
                for (int j = 0; j < columnList.Size(); j++)
                {
                    ColumnSchema schema = columnList.Get(j);
                    string columnAlias = leftRangeVar.GetColumnAlias(j);
                    bool flag = nameSet.Contains(columnAlias);
                    if ((((leftRangeVar.NamedJoinColumns == null) || !leftRangeVar.NamedJoinColumns.Contains(columnAlias)) && !set.Add(columnAlias)) && (!fullList | flag))
                    {
                        throw Error.GetError(0x15ca, columnAlias);
                    }
                    if (flag)
                    {
                        columns.Add(columnAlias);
                        int columnIndex = leftRangeVar.RangeTable.GetColumnIndex(schema.GetNameString());
                        int colIndexRight = rightRange.RangeTable.GetColumnIndex(rightRange.GetColumnNameForAlias(columnAlias));
                        Expression expression2 = new ExpressionLogical(leftRangeVar, columnIndex, rightRange, colIndexRight);
                        expression = ExpressionLogical.AndExpressions(expression, expression2);
                        ExpressionColumn columnExpression = leftRangeVar.GetColumnExpression(columnAlias);
                        if (columnExpression == null)
                        {
                            Expression[] nodes = new Expression[] { expression2.GetLeftNode(), expression2.GetRightNode() };
                            columnExpression = new ExpressionColumn(nodes, columnAlias);
                            leftRangeVar.AddNamedJoinColumnExpression(columnAlias, columnExpression);
                        }
                        else
                        {
                            columnExpression.nodes = ArrayUtil.ResizeArray<Expression>(columnExpression.nodes, columnExpression.nodes.Length + 1);
                            columnExpression.nodes[columnExpression.nodes.Length - 1] = expression2.GetRightNode();
                        }
                        rightRange.AddNamedJoinColumnExpression(columnAlias, columnExpression);
                    }
                }
            }
            if (fullList && !columns.ContainsAll(nameSet))
            {
                throw Error.GetError(0x157d);
            }
            rightRange.AddNamedJoinColumns(columns);
            return expression;
        }

        private int[] GetLimits(Session session, int maxRows)
        {
            int num = 0;
            int num2 = 0x7fffffff;
            int num3 = 0x7fffffff;
            bool flag = false;
            if (base.ExprSortAndSlice.HasLimit())
            {
                int? nullable = (int?) base.ExprSortAndSlice.LimitCondition.GetLeftNode().GetValue(session);
                if (nullable.HasValue)
                {
                    int? nullable2 = nullable;
                    int num4 = 0;
                    if (!((nullable2.GetValueOrDefault() < num4) ? nullable2.HasValue : false))
                    {
                        num = nullable.Value;
                        flag = num > 0;
                        if (base.ExprSortAndSlice.LimitCondition.GetRightNode() != null)
                        {
                            nullable = (int?) base.ExprSortAndSlice.LimitCondition.GetRightNode().GetValue(session);
                            if (nullable.HasValue)
                            {
                                nullable2 = nullable;
                                num4 = 0;
                                if (!((nullable2.GetValueOrDefault() <= num4) ? nullable2.HasValue : false))
                                {
                                    nullable2 = nullable;
                                    num4 = 0;
                                    if (!((nullable2.GetValueOrDefault() == num4) ? !nullable2.HasValue : true))
                                    {
                                        num2 = 0x7fffffff;
                                    }
                                    else
                                    {
                                        num2 = nullable.Value;
                                        flag = true;
                                    }
                                    goto Label_011B;
                                }
                            }
                            throw Error.GetError(0xd7c);
                        }
                        goto Label_011B;
                    }
                }
                throw Error.GetError(0xd7d);
            }
        Label_011B:
            if (maxRows != 0)
            {
                if (maxRows < num2)
                {
                    num2 = maxRows;
                }
                flag = true;
            }
            if ((((flag && this._simpleLimit) && (!base.ExprSortAndSlice.HasOrder() || base.ExprSortAndSlice.SkipSort)) && (!base.ExprSortAndSlice.HasLimit() || base.ExprSortAndSlice.SkipFullResult)) && ((num3 - num) > num2))
            {
                num3 = num + num2;
            }
            if (!flag)
            {
                return DefaultLimits;
            }
            return new int[] { num, num2, num3 };
        }

        public override ResultMetaData GetMetaData()
        {
            return base.resultMetaData;
        }

        public override Result GetResult(Session session, int maxrows)
        {
            Result singleResult = this.GetSingleResult(session, maxrows);
            singleResult.GetNavigator().Reset();
            return singleResult;
        }

        private Result GetSingleResult(Session session, int maxRows)
        {
            int[] limits = this.GetLimits(session, maxRows);
            Result result1 = this.BuildResult(session, limits[2]);
            RowSetNavigatorData navigator = (RowSetNavigatorData) result1.GetNavigator();
            if (this.IsDistinctSelect)
            {
                navigator.RemoveDuplicates();
            }
            if (base.ExprSortAndSlice.HasOrder())
            {
                navigator.SortOrder();
            }
            if (limits != DefaultLimits)
            {
                navigator.Trim(limits[0], limits[1]);
            }
            return result1;
        }

        public string GetSQL()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT").Append(' ');
            int indexLimitVisible = this.IndexLimitVisible;
            for (int i = 0; i < indexLimitVisible; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(this.ExprColumns[i].GetSql());
            }
            builder.Append("FROM");
            indexLimitVisible = this.RangeVariables.Length;
            for (int j = 0; j < indexLimitVisible; j++)
            {
                RangeVariable variable = this.RangeVariables[j];
                if (j > 0)
                {
                    if (variable.IsLeftJoin && variable.IsRightJoin)
                    {
                        builder.Append("FULL").Append(' ');
                    }
                    else if (variable.IsLeftJoin)
                    {
                        builder.Append("LEFT").Append(' ');
                    }
                    else if (variable.IsRightJoin)
                    {
                        builder.Append("RIGHT").Append(' ');
                    }
                    builder.Append("JOIN").Append(' ');
                }
                builder.Append(variable.GetTable().GetName().StatementName);
            }
            if (this.IsGrouped)
            {
                builder.Append(' ').Append("GROUP").Append(' ').Append("BY");
                indexLimitVisible = this.IndexLimitVisible + this._groupByColumnCount;
                for (int k = this.IndexLimitVisible; k < indexLimitVisible; k++)
                {
                    builder.Append(this.ExprColumns[k].GetSql());
                    if (k < (indexLimitVisible - 1))
                    {
                        builder.Append(',');
                    }
                }
            }
            if (this._havingCondition != null)
            {
                builder.Append(' ').Append("HAVING").Append(' ');
                builder.Append(this._havingCondition.GetSql());
            }
            if (base.ExprSortAndSlice.HasOrder())
            {
                indexLimitVisible = this.IndexStartOrderBy + base.ExprSortAndSlice.GetOrderLength();
                builder.Append(' ').Append("ORDER").Append("BY").Append(' ');
                for (int k = this.IndexStartOrderBy; k < indexLimitVisible; k++)
                {
                    builder.Append(this.ExprColumns[k].GetSql());
                    if (k < (indexLimitVisible - 1))
                    {
                        builder.Append(',');
                    }
                }
            }
            if (base.ExprSortAndSlice.HasLimit())
            {
                builder.Append(base.ExprSortAndSlice.LimitCondition.GetLeftNode().GetSql());
            }
            return builder.ToString();
        }

        public override OrderedHashSet<SubQuery> GetSubqueries()
        {
            OrderedHashSet<SubQuery> set = null;
            for (int i = 0; i < this.IndexStartAggregates; i++)
            {
                set = this.ExprColumns[i].CollectAllSubqueries(set);
            }
            if (this.QueryCondition != null)
            {
                set = this.QueryCondition.CollectAllSubqueries(set);
            }
            if (this._havingCondition != null)
            {
                set = this._havingCondition.CollectAllSubqueries(set);
            }
            for (int j = 0; j < this.RangeVariables.Length; j++)
            {
                OrderedHashSet<SubQuery> subqueries = this.RangeVariables[j].GetSubqueries();
                set = OrderedHashSet<SubQuery>.AddAll(set, subqueries);
            }
            return set;
        }

        public override bool HasReference(RangeVariable range)
        {
            if (base.UnresolvedExpressions != null)
            {
                for (int i = 0; i < base.UnresolvedExpressions.Count; i++)
                {
                    if (base.UnresolvedExpressions[i].HasReference(range))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool IsEquivalent(QueryExpression other)
        {
            QuerySpecification specification = other as QuerySpecification;
            if (specification != null)
            {
                return false;
            }
            if (!Expression.Equals(this.ExprColumns, specification.ExprColumns))
            {
                return false;
            }
            if (!Expression.Equals(this.QueryCondition, specification.QueryCondition))
            {
                return false;
            }
            if (this.RangeVariables.Length != specification.RangeVariables.Length)
            {
                return false;
            }
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                if (this.RangeVariables[i].GetTable() != specification.RangeVariables[i].GetTable())
                {
                    return false;
                }
            }
            return true;
        }

        public override bool IsSingleColumn()
        {
            return (this.IndexLimitVisible == 1);
        }

        public void MergeQuery()
        {
            RangeVariable range = this.RangeVariables[0];
            Table table = range.GetTable();
            Expression queryCondition = this.QueryCondition;
            if (table is TableDerived)
            {
                QueryExpression queryExpression = table.GetQueryExpression();
                if ((queryExpression == null) || !queryExpression.IsMergeable)
                {
                    base.IsMergeable = false;
                    return;
                }
                QuerySpecification mainSelect = queryExpression.GetMainSelect();
                if (queryExpression.view == null)
                {
                    this.RangeVariables[0] = mainSelect.RangeVariables[0];
                    this.RangeVariables[0].ResetConditions();
                    Expression[] expressionArray = new Expression[this.IndexLimitData];
                    for (int i = 0; i < this.IndexLimitData; i++)
                    {
                        expressionArray[i] = this.ExprColumns[i].ReplaceColumnReferences(range, mainSelect.ExprColumns);
                    }
                    this.ExprColumns = expressionArray;
                    if (queryCondition != null)
                    {
                        queryCondition = queryCondition.ReplaceColumnReferences(range, mainSelect.ExprColumns);
                    }
                    Expression expression3 = mainSelect.QueryCondition;
                    this.CheckQueryCondition = mainSelect.CheckQueryCondition;
                    this.QueryCondition = ExpressionLogical.AndExpressions(expression3, queryCondition);
                }
                else
                {
                    RangeVariable[] newRanges = new RangeVariable[] { mainSelect.RangeVariables[0].Duplicate() };
                    Expression[] list = new Expression[mainSelect.IndexLimitData];
                    for (int i = 0; i < mainSelect.IndexLimitData; i++)
                    {
                        Expression expression6 = mainSelect.ExprColumns[i].Duplicate();
                        list[i] = expression6;
                        expression6.ReplaceRangeVariables(mainSelect.RangeVariables, newRanges);
                    }
                    for (int j = 0; j < this.IndexLimitData; j++)
                    {
                        this.ExprColumns[j] = this.ExprColumns[j].ReplaceColumnReferences(range, list);
                    }
                    Expression expression5 = mainSelect.QueryCondition;
                    if (expression5 != null)
                    {
                        expression5 = expression5.Duplicate();
                        expression5.ReplaceRangeVariables(mainSelect.RangeVariables, newRanges);
                    }
                    if (queryCondition != null)
                    {
                        queryCondition = queryCondition.ReplaceColumnReferences(range, list);
                    }
                    this.CheckQueryCondition = mainSelect.CheckQueryCondition;
                    if (this.CheckQueryCondition != null)
                    {
                        this.CheckQueryCondition = this.CheckQueryCondition.Duplicate();
                        this.CheckQueryCondition.ReplaceRangeVariables(mainSelect.RangeVariables, newRanges);
                    }
                    this.QueryCondition = ExpressionLogical.AndExpressions(expression5, queryCondition);
                    this.RangeVariables = newRanges;
                }
            }
            if (base.view != null)
            {
                switch (base.view.GetCheckOption())
                {
                    case 1:
                        if (!base.IsUpdatable)
                        {
                            throw Error.GetError(0x15a1);
                        }
                        this.CheckQueryCondition = queryCondition;
                        break;

                    case 2:
                        if (!base.IsUpdatable)
                        {
                            throw Error.GetError(0x15a1);
                        }
                        this.CheckQueryCondition = this.QueryCondition;
                        break;
                }
            }
            if (this.IsAggregated)
            {
                base.IsMergeable = false;
            }
        }

        private void ReplaceColumnIndexInOrderBy(Expression orderBy)
        {
            Expression leftNode = orderBy.GetLeftNode();
            if (leftNode.GetExprType() == 1)
            {
                if (leftNode.GetDataType().TypeCode == 4)
                {
                    int num = (int) leftNode.GetValue(null);
                    if ((0 < num) && (num <= this.IndexLimitVisible))
                    {
                        orderBy.SetLeftNode(this.ExprColumns[num - 1]);
                        return;
                    }
                }
                throw Error.GetError(0x15c8);
            }
        }

        public override void ReplaceColumnReference(RangeVariable range, Expression[] list)
        {
            for (int i = 0; i < this.IndexStartAggregates; i++)
            {
                this.ExprColumns[i].ReplaceColumnReferences(range, list);
            }
            if (this.QueryCondition != null)
            {
                this.QueryCondition.ReplaceColumnReferences(range, list);
            }
            if (this._havingCondition != null)
            {
                this._havingCondition.ReplaceColumnReferences(range, list);
            }
            int num = 0;
            int length = this.RangeVariables.Length;
            while (num < length)
            {
                num++;
            }
        }

        public override void ReplaceRangeVariables(RangeVariable[] ranges, RangeVariable[] newRanges)
        {
            for (int i = 0; i < this.IndexStartAggregates; i++)
            {
                this.ExprColumns[i].ReplaceRangeVariables(ranges, newRanges);
            }
            if (this.QueryCondition != null)
            {
                this.QueryCondition.ReplaceRangeVariables(ranges, newRanges);
            }
            if (this._havingCondition != null)
            {
                this._havingCondition.ReplaceRangeVariables(ranges, newRanges);
            }
            int index = 0;
            int length = this.RangeVariables.Length;
            while (index < length)
            {
                this.RangeVariables[index].GetSubqueries();
                index++;
            }
        }

        private void ResolveAggregates()
        {
            this._tempSetExpr.Clear();
            if (this.IsAggregated)
            {
                this._aggregateCheck = new bool[this.IndexStartAggregates];
                this._tempSetExpr.AddAll(this._aggregateSet);
                this.IndexLimitData = this._indexLimitExpressions = this.ExprColumns.Length + this._tempSetExpr.Size();
                this.ExprColumns = ArrayUtil.ResizeArray<Expression>(this.ExprColumns, this._indexLimitExpressions);
                int indexStartAggregates = this.IndexStartAggregates;
                for (int i = 0; indexStartAggregates < this._indexLimitExpressions; i++)
                {
                    Expression expression = this._tempSetExpr.Get(i);
                    this.ExprColumns[indexStartAggregates] = expression.Duplicate();
                    this.ExprColumns[indexStartAggregates].nodes = expression.nodes;
                    this.ExprColumns[indexStartAggregates].DataType = expression.DataType;
                    indexStartAggregates++;
                }
                this._tempSetExpr.Clear();
            }
        }

        private void ResolveColumnReferences()
        {
            if (this.IsDistinctSelect || this.IsGrouped)
            {
                base.AcceptsSequences = false;
            }
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                Expression joinCondition = this.RangeVariables[i].GetJoinCondition();
                if (joinCondition != null)
                {
                    this.ResolveColumnReferencesAndAllocate(joinCondition, i + 1, false);
                }
            }
            this.ResolveColumnReferencesAndAllocate(this.QueryCondition, this.RangeVariables.Length, false);
            if (this._resolvedSubqueryExpressions != null)
            {
                this._resolvedSubqueryExpressions.Clear();
            }
            for (int j = 0; j < this.IndexLimitVisible; j++)
            {
                this.ResolveColumnReferencesAndAllocate(this.ExprColumns[j], this.RangeVariables.Length, base.AcceptsSequences);
            }
            for (int k = this.IndexLimitVisible; k < this._indexStartHaving; k++)
            {
                this.ExprColumns[k] = this.ResolveColumnReferencesInGroupBy(this.ExprColumns[k]);
            }
            for (int m = this._indexStartHaving; m < this.IndexStartOrderBy; m++)
            {
                this.ResolveColumnReferencesAndAllocate(this.ExprColumns[m], this.RangeVariables.Length, false);
            }
            this.ResolveColumnRefernecesInOrderBy(base.ExprSortAndSlice);
        }

        private bool ResolveColumnReferences(Expression e, int rangeCount, bool withSequences)
        {
            if (e == null)
            {
                return true;
            }
            base.UnresolvedExpressions = e.ResolveColumnReferences(this.RangeVariables, rangeCount, base.UnresolvedExpressions, withSequences);
            int num = (base.UnresolvedExpressions == null) ? 0 : base.UnresolvedExpressions.Count;
            return (((base.UnresolvedExpressions == null) ? 0 : base.UnresolvedExpressions.Count) == num);
        }

        public void ResolveColumnReferencesAndAllocate(Expression expression, int count, bool withSequences)
        {
            if (expression != null)
            {
                List<Expression> list = expression.ResolveColumnReferences(this.RangeVariables, count, null, withSequences);
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Expression e = list[i];
                        bool flag = false;
                        if (e.IsSelfAggregate())
                        {
                            for (int j = 0; j < e.nodes.Length; j++)
                            {
                                flag = this.ResolveColumnReferences(e.nodes[j], count, false);
                            }
                        }
                        else
                        {
                            flag = this.ResolveColumnReferences(e, count, withSequences);
                        }
                        if (flag)
                        {
                            if (e.IsSelfAggregate())
                            {
                                if (this._aggregateSet == null)
                                {
                                    this._aggregateSet = new List<Expression>();
                                }
                                this._aggregateSet.Add(e);
                                this.IsAggregated = true;
                                expression.SetAggregate();
                            }
                            if (this._resolvedSubqueryExpressions == null)
                            {
                                this._resolvedSubqueryExpressions = new List<Expression>();
                            }
                            this._resolvedSubqueryExpressions.Add(e);
                        }
                        else
                        {
                            if (base.UnresolvedExpressions == null)
                            {
                                base.UnresolvedExpressions = new List<Expression>();
                            }
                            base.UnresolvedExpressions.Add(e);
                        }
                    }
                }
            }
        }

        private void ResolveColumnReferencesForAsterisk()
        {
            int index = 0;
            while (index < this.IndexLimitVisible)
            {
                Expression e = this._exprColumnList[index];
                if (e.GetExprType() == 0x61)
                {
                    this._exprColumnList.RemoveAt(index);
                    string tableName = ((ExpressionColumn) e).GetTableName();
                    if (tableName == null)
                    {
                        this.AddAllJoinedColumns(e);
                    }
                    else
                    {
                        int num2 = e.FindMatchingRangeVariableIndex(this.RangeVariables);
                        if (num2 == -1)
                        {
                            throw Error.GetError(0x157d, tableName);
                        }
                        HashSet<string> allNamedJoinColumns = this.GetAllNamedJoinColumns();
                        this.RangeVariables[num2].AddTableColumns(e, allNamedJoinColumns);
                    }
                    for (int i = 0; i < e.nodes.Length; i++)
                    {
                        this._exprColumnList.Insert(index, e.nodes[i]);
                        index++;
                    }
                    this.IndexLimitVisible += e.nodes.Length - 1;
                }
                else
                {
                    index++;
                }
            }
        }

        private Expression ResolveColumnReferencesInGroupBy(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            if (expression.ResolveColumnReferences(this.RangeVariables, this.RangeVariables.Length, null, false) != null)
            {
                if (expression.GetExprType() == 2)
                {
                    Expression expression3 = expression.ReplaceAliasInOrderBy(this.ExprColumns, this.IndexLimitVisible);
                    if (expression3 != expression)
                    {
                        return expression3;
                    }
                }
                this.ResolveColumnReferencesAndAllocate(expression, this.RangeVariables.Length, false);
            }
            return expression;
        }

        public void ResolveColumnRefernecesInOrderBy(SortAndSlice sortAndSlice)
        {
            int orderLength = sortAndSlice.GetOrderLength();
            for (int i = 0; i < orderLength; i++)
            {
                ExpressionOrderBy orderBy = sortAndSlice.ExprList[i];
                this.ReplaceColumnIndexInOrderBy(orderBy);
                if (orderBy.GetLeftNode().QueryTableColumnIndex == -1)
                {
                    if (sortAndSlice.SortUnion && (orderBy.GetLeftNode().GetExprType() != 2))
                    {
                        throw Error.GetError(0x15c8);
                    }
                    orderBy.ReplaceAliasInOrderBy(this.ExprColumns, this.IndexLimitVisible);
                    this.ResolveColumnReferencesAndAllocate(orderBy, this.RangeVariables.Length, false);
                    if ((this.IsAggregated || this.IsGrouped) && !orderBy.GetLeftNode().IsComposedOf(this.ExprColumns, 0, this.IndexLimitVisible + this._groupByColumnCount, Expression.AggregateFunctionSet))
                    {
                        throw Error.GetError(0x15c8);
                    }
                }
            }
            sortAndSlice.Prepare(this);
        }

        public void ResolveExpressionTypes(Session session, Expression parent)
        {
            for (int i = 0; i < this.IndexStartAggregates; i++)
            {
                Expression expression1 = this.ExprColumns[i];
                expression1.ResolveTypes(session, parent);
                if (expression1.GetExprType() == 0x19)
                {
                    throw Error.GetError(0x15bc);
                }
            }
            int index = 0;
            int length = this.RangeVariables.Length;
            while (index < length)
            {
                Expression joinCondition = this.RangeVariables[index].GetJoinCondition();
                if (joinCondition != null)
                {
                    joinCondition.ResolveTypes(session, null);
                    if (joinCondition.GetDataType() != SqlType.SqlBoolean)
                    {
                        throw Error.GetError(0x15c0);
                    }
                }
                index++;
            }
            if (this.QueryCondition != null)
            {
                this.QueryCondition.ResolveTypes(session, null);
                if (this.QueryCondition.GetDataType() != SqlType.SqlBoolean)
                {
                    throw Error.GetError(0x15c0);
                }
            }
            if (this._havingCondition != null)
            {
                this._havingCondition.ResolveTypes(session, null);
                if (this._havingCondition.GetDataType() != SqlType.SqlBoolean)
                {
                    throw Error.GetError(0x15c0);
                }
            }
        }

        public bool ResolveForGroupBy(IUtlList<Expression> unresolvedSet)
        {
            for (int i = this.IndexLimitVisible; i < (this.IndexLimitVisible + this._groupByColumnCount); i++)
            {
                Expression expression = this.ExprColumns[i];
                if (expression.GetExprType() == 2)
                {
                    RangeVariable rangeVariable = expression.GetRangeVariable();
                    int columnIndex = expression.GetColumnIndex();
                    rangeVariable.ColumnsInGroupBy[columnIndex] = true;
                }
            }
            for (int j = 0; j < this.RangeVariables.Length; j++)
            {
                RangeVariable variable2 = this.RangeVariables[j];
                variable2.HasKeyedColumnInGroupBy = variable2.RangeTable.GetUniqueNotNullColumnGroup(variable2.ColumnsInGroupBy) > null;
            }
            OrderedHashSet<Expression> unkeyedColumns = null;
            for (int k = 0; k < unresolvedSet.Size(); k++)
            {
                unkeyedColumns = unresolvedSet.Get(k).GetUnkeyedColumns(unkeyedColumns);
            }
            return (unkeyedColumns == null);
        }

        private void ResolveGroups()
        {
            this._tempSetExpr.Clear();
            if (this.IsGrouped)
            {
                for (int i = this.IndexLimitVisible; i < (this.IndexLimitVisible + this._groupByColumnCount); i++)
                {
                    this.ExprColumns[i].CollectAllExpressions(this._tempSetExpr, Expression.AggregateFunctionSet, Expression.SubqueryExpressionSet);
                    if (!this._tempSetExpr.IsEmpty())
                    {
                        throw Error.GetError(0x15c4, this._tempSetExpr.Get(0).GetSql());
                    }
                }
                for (int j = 0; j < this.IndexLimitVisible; j++)
                {
                    if (!this.ExprColumns[j].IsComposedOf(this.ExprColumns, this.IndexLimitVisible, this.IndexLimitVisible + this._groupByColumnCount, Expression.SubqueryAggregateExpressionSet))
                    {
                        this._tempSetExpr.Add(this.ExprColumns[j]);
                    }
                }
                if (!this._tempSetExpr.IsEmpty() && !this.ResolveForGroupBy(this._tempSetExpr))
                {
                    throw Error.GetError(0x15c6, this._tempSetExpr.Get(0).GetSql());
                }
            }
            else if (this.IsAggregated)
            {
                for (int i = 0; i < this.IndexLimitVisible; i++)
                {
                    this.ExprColumns[i].CollectAllExpressions(this._tempSetExpr, Expression.ColumnExpressionSet, Expression.AggregateFunctionSet);
                    if (!this._tempSetExpr.IsEmpty())
                    {
                        throw Error.GetError(0x15c6, this._tempSetExpr.Get(0).GetSql());
                    }
                }
            }
            this._tempSetExpr.Clear();
            if (this._havingCondition != null)
            {
                if (base.UnresolvedExpressions != null)
                {
                    this._tempSetExpr.AddAll(base.UnresolvedExpressions);
                }
                for (int i = this.IndexLimitVisible; i < (this.IndexLimitVisible + this._groupByColumnCount); i++)
                {
                    this._tempSetExpr.Add(this.ExprColumns[i]);
                }
                if (!this._havingCondition.IsComposedOf(this._tempSetExpr, Expression.SubqueryAggregateExpressionSet))
                {
                    throw Error.GetError(0x15c5);
                }
                this._tempSetExpr.Clear();
            }
            if (this.IsDistinctSelect)
            {
                int orderLength = base.ExprSortAndSlice.GetOrderLength();
                for (int i = 0; i < orderLength; i++)
                {
                    ExpressionOrderBy by = base.ExprSortAndSlice.ExprList[i];
                    if ((by.QueryTableColumnIndex == -1) && !by.IsComposedOf(this.ExprColumns, 0, this.IndexLimitVisible, Expression.EmptyExpressionSet))
                    {
                        throw Error.GetError(0x15c8);
                    }
                }
            }
            if (this.IsGrouped)
            {
                int orderLength = base.ExprSortAndSlice.GetOrderLength();
                for (int i = 0; i < orderLength; i++)
                {
                    ExpressionOrderBy by2 = base.ExprSortAndSlice.ExprList[i];
                    if (((by2.QueryTableColumnIndex == -1) && !by2.IsAggregate()) && !by2.IsComposedOf(this.ExprColumns, 0, this.IndexLimitVisible + this._groupByColumnCount, Expression.EmptyExpressionSet))
                    {
                        throw Error.GetError(0x15c8);
                    }
                }
            }
            if (this.IsDistinctSelect || this.IsGrouped)
            {
                this._simpleLimit = false;
            }
            if (this.IsAggregated)
            {
                OrderedHashSet<Expression> expressions = new OrderedHashSet<Expression>();
                OrderedHashSet<Expression> replacements = new OrderedHashSet<Expression>();
                for (int i = this.IndexStartAggregates; i < this._indexLimitExpressions; i++)
                {
                    Expression e = this.ExprColumns[i];
                    Expression key = new ExpressionColumn(e, i, this.ResultRangePosition);
                    expressions.Add(e);
                    replacements.Add(key);
                }
                for (int j = 0; j < this._indexStartHaving; j++)
                {
                    if (!this.ExprColumns[j].IsAggregate())
                    {
                        Expression key = this.ExprColumns[j];
                        if (expressions.Add(key))
                        {
                            Expression expression4 = new ExpressionColumn(key, j, this.ResultRangePosition);
                            replacements.Add(expression4);
                        }
                    }
                }
                int orderLength = base.ExprSortAndSlice.GetOrderLength();
                for (int k = 0; k < orderLength; k++)
                {
                    ExpressionOrderBy by3 = base.ExprSortAndSlice.ExprList[k];
                    if (by3.GetLeftNode().IsAggregate())
                    {
                        by3.SetAggregate();
                    }
                }
                for (int m = this.IndexStartOrderBy; m < this.IndexStartAggregates; m++)
                {
                    if (this.ExprColumns[m].GetLeftNode().isAggregate)
                    {
                        this.ExprColumns[m].SetAggregate();
                    }
                }
                for (int n = 0; n < this.IndexStartAggregates; n++)
                {
                    Expression expression5 = this.ExprColumns[n];
                    if (expression5.IsAggregate() || expression5.IsCorrelated())
                    {
                        this._aggregateCheck[n] = true;
                        if (expression5.IsAggregate())
                        {
                            expression5.ConvertToSimpleColumn(expressions, replacements);
                        }
                    }
                }
                for (int num15 = 0; num15 < this._aggregateSet.Count; num15++)
                {
                    this._aggregateSet[num15].ConvertToSimpleColumn(expressions, replacements);
                }
                if (this._resolvedSubqueryExpressions != null)
                {
                    for (int num16 = 0; num16 < this._resolvedSubqueryExpressions.Count; num16++)
                    {
                        this._resolvedSubqueryExpressions[num16].ConvertToSimpleColumn(expressions, replacements);
                    }
                }
            }
        }

        private void ResolveRangeVariables(Session session, RangeVariable[] outerRanges)
        {
            if ((this.RangeVariables == null) || (this.RangeVariables.Length < this._rangeVariableList.Count))
            {
                this.RangeVariables = this._rangeVariableList.ToArray();
            }
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                this.RangeVariables[i].ResolveRangeTable(session, this.RangeVariables, i, outerRanges);
            }
        }

        public override void ResolveReferences(Session session, RangeVariable[] outerRanges)
        {
            this.ResolveRangeVariables(session, outerRanges);
            this.ResolveColumnReferencesForAsterisk();
            this.FinaliseColumns();
            this.ResolveColumnReferences();
            base.UnionColumnTypes = new SqlType[this.IndexLimitVisible];
            base.UnionColumnNullability = new byte[this.IndexLimitVisible];
            this.SetReferenceableColumns();
        }

        public override void ResolveTypes(Session session)
        {
            if (!base.IsResolved)
            {
                this.ResolveTypesPartOne(session);
                this.ResolveTypesPartTwo(session);
                Array.Copy(base.ResultTable.ColTypes, base.UnionColumnTypes, base.UnionColumnTypes.Length);
                for (int i = 0; i < base.ResultTable.ColNotNull.Length; i++)
                {
                    base.ResultTable.ColNotNull[i] = base.UnionColumnNullability[i] == 0;
                }
                for (int j = 0; j < this._indexStartHaving; j++)
                {
                    if (this.ExprColumns[j].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                }
            }
        }

        public override void ResolveTypesPartOne(Session session)
        {
            this.ResolveExpressionTypes(session, this.RowExpression);
            this.ResolveAggregates();
            for (int i = 0; i < base.UnionColumnTypes.Length; i++)
            {
                base.UnionColumnTypes[i] = SqlType.GetAggregateType(base.UnionColumnTypes[i], this.ExprColumns[i].GetDataType());
                base.UnionColumnNullability[i] = GetAggregateNullability(base.UnionColumnNullability[i], this.ExprColumns[i].GetNullability());
            }
        }

        public override void ResolveTypesPartTwo(Session session)
        {
            this.ResolveGroups();
            for (int i = 0; i < base.UnionColumnTypes.Length; i++)
            {
                SqlType type = base.UnionColumnTypes[i];
                byte nullability = base.UnionColumnNullability[i];
                if (type == null)
                {
                    throw Error.GetError(0x15bf);
                }
                this.ExprColumns[i].SetDataType(session, type);
                this.ExprColumns[i].SetNullability(nullability);
            }
            for (int j = 0; j < this._indexStartHaving; j++)
            {
                if (this.ExprColumns[j].DataType == null)
                {
                    throw Error.GetError(0x15bf);
                }
            }
            this.CheckLobUsage();
            this.SetMergeability();
            this.SetUpdatability();
            this.CreateResultMetaData();
            this.CreateTable(session);
            if (base.IsMergeable)
            {
                this.MergeQuery();
            }
            this.SetRangeVariableConditions(session);
            if (((this.IsAggregated && !this.IsGrouped) && (!base.ExprSortAndSlice.HasOrder() && !base.ExprSortAndSlice.HasLimit())) && ((this._aggregateSet.Count == 1) && (this.IndexLimitVisible == 1)))
            {
                Expression expression = this.ExprColumns[this.IndexStartAggregates];
                int exprType = expression.GetExprType();
                if (exprType == 0x47)
                {
                    if (((this.RangeVariables.Length == 1) && (this.QueryCondition == null)) && (expression.GetLeftNode().GetExprType() == 9))
                    {
                        this._isSimpleCount = true;
                    }
                }
                else if ((exprType - 0x49) <= 1)
                {
                    SortAndSlice slice = new SortAndSlice {
                        IsGenerated = true
                    };
                    slice.AddLimitCondition(ExpressionOp.LimitOneExpression);
                    if (slice.PrepareSpecial(session, this))
                    {
                        base.ExprSortAndSlice = slice;
                    }
                }
            }
            base.ExprSortAndSlice.SetSortRange(this);
            base.IsResolved = true;
        }

        public void SetColumnAliases(QNameManager.SimpleName[] names)
        {
            if (names.Length != this.IndexLimitVisible)
            {
                throw Error.GetError(0x15d9);
            }
            for (int i = 0; i < this.IndexLimitVisible; i++)
            {
                this.ExprColumns[i].SetAlias(names[i]);
            }
        }

        public void SetMergeability()
        {
            if (this.IsGrouped || this.IsDistinctSelect)
            {
                base.IsMergeable = false;
            }
            else if (base.ExprSortAndSlice.HasLimit() || base.ExprSortAndSlice.HasOrder())
            {
                base.IsMergeable = false;
            }
            else if (this.RangeVariables.Length != 1)
            {
                base.IsMergeable = false;
            }
        }

        private void SetRangeVariableConditions(Session session)
        {
            RangeVariableResolver resolver = new RangeVariableResolver(this.RangeVariables, this.QueryCondition, base.compileContext);
            resolver.ProcessConditions(session);
            this.RangeVariables = resolver.GetRangeVariables();
        }

        public void SetReferenceableColumns()
        {
            base.AccessibleColumns = new bool[this.IndexLimitVisible];
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < this.IndexLimitVisible; i++)
            {
                Expression expression = this.ExprColumns[i];
                string alias = expression.GetAlias();
                if (alias.Length == 0)
                {
                    QNameManager.SimpleName autoColumnName = QNameManager.GetAutoColumnName(i);
                    expression.SetAlias(autoColumnName);
                }
                else
                {
                    int num2;
                    if (!dictionary.TryGetValue(alias, out num2))
                    {
                        dictionary.Add(alias, i);
                        base.AccessibleColumns[i] = true;
                    }
                    else
                    {
                        base.AccessibleColumns[num2] = false;
                    }
                }
            }
        }

        public override void SetReturningResult()
        {
            this.SetReturningResultSet();
            base.AcceptsSequences = true;
            base.IsTopLevel = true;
        }

        public override void SetReturningResultSet()
        {
            base.PersistenceScope = 0x17;
        }

        public void SetUpdatability()
        {
            if (base.IsUpdatable)
            {
                base.IsUpdatable = false;
                if (((base.IsMergeable && base.IsTopLevel) && (!this.IsAggregated && !base.ExprSortAndSlice.HasLimit())) && !base.ExprSortAndSlice.HasOrder())
                {
                    Table table = this.RangeVariables[0].GetTable();
                    Table baseTable = table.GetBaseTable();
                    if (baseTable != null)
                    {
                        base.IsInsertable = table.IsInsertable();
                        base.IsUpdatable = table.IsUpdatable();
                        if (base.IsInsertable || base.IsUpdatable)
                        {
                            Dictionary<string, int> dictionary = new Dictionary<string, int>();
                            int[] baseTableColumnMap = table.GetBaseTableColumnMap();
                            int[] columnIndexes = new int[this.IndexLimitVisible];
                            if (this.QueryCondition != null)
                            {
                                this._tempSet.Clear();
                                CollectSubQueriesAndReferences(this._tempSet, this.QueryCondition);
                                if (this._tempSet.Contains(table.GetName()) || this._tempSet.Contains(baseTable.GetName()))
                                {
                                    base.IsUpdatable = false;
                                    base.IsInsertable = false;
                                    return;
                                }
                            }
                            for (int i = 0; i < this.IndexLimitVisible; i++)
                            {
                                Expression expression = this.ExprColumns[i];
                                if (expression.GetExprType() == 2)
                                {
                                    string name = expression.GetColumn().GetName().Name;
                                    if (dictionary.ContainsKey(name))
                                    {
                                        dictionary[name] = 1;
                                    }
                                    else
                                    {
                                        dictionary[name] = 0;
                                    }
                                }
                                else
                                {
                                    this._tempSet.Clear();
                                    CollectSubQueriesAndReferences(this._tempSet, expression);
                                    if (this._tempSet.Contains(table.GetName()))
                                    {
                                        base.IsUpdatable = false;
                                        base.IsInsertable = false;
                                        return;
                                    }
                                }
                            }
                            base.IsUpdatable = false;
                            int index = 0;
                            while (index < this.IndexLimitVisible)
                            {
                                if (!base.AccessibleColumns[index])
                                {
                                    goto Label_021C;
                                }
                                Expression expression2 = this.ExprColumns[index];
                                if (expression2.GetExprType() != 2)
                                {
                                    goto Label_021C;
                                }
                                string name = expression2.GetColumn().GetName().Name;
                                if (dictionary[name] != 0)
                                {
                                    goto Label_021C;
                                }
                                int num3 = table.FindColumn(name);
                                columnIndexes[index] = baseTableColumnMap[num3];
                                if (columnIndexes[index] != -1)
                                {
                                    base.IsUpdatable = true;
                                }
                            Label_0214:
                                index++;
                                continue;
                            Label_021C:
                                columnIndexes[index] = -1;
                                base.IsInsertable = false;
                                goto Label_0214;
                            }
                            if (base.IsInsertable)
                            {
                                bool[] columnCheckList = baseTable.GetColumnCheckList(columnIndexes);
                                for (int j = 0; j < columnCheckList.Length; j++)
                                {
                                    if (!columnCheckList[j])
                                    {
                                        ColumnSchema column = baseTable.GetColumn(j);
                                        if ((!column.IsIdentity() && !column.IsGenerated()) && (!column.HasDefault && !column.IsNullable()))
                                        {
                                            base.IsInsertable = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!base.IsUpdatable)
                            {
                                base.IsInsertable = false;
                            }
                            if (base.IsUpdatable)
                            {
                                this._columnMap = columnIndexes;
                                this._baseTable = baseTable;
                                if (base.view == null)
                                {
                                    this._indexLimitRowId++;
                                    if (!baseTable.IsFileBased())
                                    {
                                        this._indexLimitRowId++;
                                        this._hasMemoryRow = true;
                                    }
                                    this.IndexLimitData = this._indexLimitRowId;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

