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
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public class QueryExpression
    {
        public const int Nounion = 0;
        public const int Union = 1;
        public const int UnionAll = 2;
        public const int Intersect = 3;
        public const int IntersectAll = 4;
        public const int ExceptAll = 5;
        public const int Except = 6;
        public const int UnionTerm = 7;
        private readonly QueryExpression _leftQueryExpression;
        private int _columnCount;
        public bool IsInsertable;
        public bool IsUpdatable;
        private QueryExpression _rightQueryExpression;
        public SortAndSlice ExprSortAndSlice;
        private bool _unionCorresponding;
        private OrderedHashSet<string> _unionCorrespondingColumns;
        private int _unionType;
        public bool AcceptsSequences;
        public bool[] AccessibleColumns;
        public ParserDQL.CompileContext compileContext;
        public Index FullIndex;
        public Index IdIndex;
        public bool IsCheckable;
        public bool IsFullOrder;
        public bool IsMergeable;
        public bool IsResolved;
        public bool IsTopLevel;
        public Index MainIndex;
        public Index OrderIndex;
        public int PersistenceScope;
        public ResultMetaData resultMetaData;
        public TableBase ResultTable;
        public int[] UnionColumnMap;
        public byte[] UnionColumnNullability;
        public SqlType[] UnionColumnTypes;
        public List<Expression> UnresolvedExpressions;
        public View view;

        public QueryExpression(ParserDQL.CompileContext compileContext)
        {
            this.PersistenceScope = 0x15;
            this.compileContext = compileContext;
            this.ExprSortAndSlice = new SortAndSlice();
        }

        public QueryExpression(ParserDQL.CompileContext compileContext, QueryExpression leftQueryExpression) : this(compileContext)
        {
            this.ExprSortAndSlice = new SortAndSlice();
            this._leftQueryExpression = leftQueryExpression;
        }

        public virtual void AddSortAndSlice(SortAndSlice sortAndSlice)
        {
            this.ExprSortAndSlice = sortAndSlice;
            sortAndSlice.SortUnion = true;
        }

        public void AddUnion(QueryExpression queryExpression, int unionType)
        {
            this.ExprSortAndSlice = new SortAndSlice();
            this._rightQueryExpression = queryExpression;
            this._unionType = unionType;
            this.SetFullOrder();
        }

        private void AddUnresolvedExpressions(List<Expression> expressions)
        {
            if (expressions != null)
            {
                if (this.UnresolvedExpressions == null)
                {
                    this.UnresolvedExpressions = new List<Expression>();
                }
                this.UnresolvedExpressions.AddRange(expressions);
            }
        }

        public virtual bool AreColumnsResolved()
        {
            if (this.UnresolvedExpressions != null)
            {
                return (this.UnresolvedExpressions.Count == 0);
            }
            return true;
        }

        public virtual OrderedHashSet<Expression> CollectAllExpressions(OrderedHashSet<Expression> set, OrderedIntHashSet typeSet, OrderedIntHashSet stopAtTypeSet)
        {
            set = this._leftQueryExpression.CollectAllExpressions(set, typeSet, stopAtTypeSet);
            if (this._rightQueryExpression != null)
            {
                set = this._rightQueryExpression.CollectAllExpressions(set, typeSet, stopAtTypeSet);
            }
            return set;
        }

        public virtual void CollectObjectNames(FwNs.Core.LC.cLib.ISet<QNameManager.QName> set)
        {
            this._leftQueryExpression.CollectObjectNames(set);
            if (this._rightQueryExpression != null)
            {
                this._rightQueryExpression.CollectObjectNames(set);
            }
        }

        public virtual void CreateResultTable(Session session)
        {
            QNameManager.QName subqueryTableName = session.database.NameManager.GetSubqueryTableName();
            int type = (this.PersistenceScope == 0x15) ? 2 : 9;
            HashMappedList<string, ColumnSchema> unionColumns = this._leftQueryExpression.GetUnionColumns();
            try
            {
                this.ResultTable = new TableDerived(session.database, subqueryTableName, type, this.UnionColumnTypes, this.UnionColumnNullability, unionColumns, null, null);
            }
            catch (Exception)
            {
            }
        }

        public virtual void CreateTable(Session session)
        {
            this.CreateResultTable(session);
            this.MainIndex = this.ResultTable.GetPrimaryIndex();
            if (this.ExprSortAndSlice.HasOrder())
            {
                this.OrderIndex = this.ResultTable.CreateAndAddIndexStructure(session, null, this.ExprSortAndSlice.SortOrder, this.ExprSortAndSlice.SortDescending, this.ExprSortAndSlice.SortNullsLast, false, false, false);
            }
            int[] colindex = new int[this._columnCount];
            ArrayUtil.FillSequence(colindex);
            this.FullIndex = this.ResultTable.CreateAndAddIndexStructure(session, null, colindex, null, null, false, false, false);
            this.ResultTable.FullIndex = this.FullIndex;
        }

        public virtual string Describe(Session session)
        {
            return this._leftQueryExpression.Describe(session);
        }

        public virtual Table GetBaseTable()
        {
            return null;
        }

        public virtual int[] GetBaseTableColumnMap()
        {
            return null;
        }

        public virtual void GetBaseTableNames(OrderedHashSet<QNameManager.QName> set)
        {
            this._leftQueryExpression.GetBaseTableNames(set);
            this._rightQueryExpression.GetBaseTableNames(set);
        }

        public virtual Expression GetCheckCondition()
        {
            return null;
        }

        public virtual int GetColumnCount()
        {
            if (this._unionCorrespondingColumns != null)
            {
                return this._unionCorrespondingColumns.Size();
            }
            int columnCount = this._rightQueryExpression.GetColumnCount();
            int num1 = this._leftQueryExpression.GetColumnCount();
            if (num1 != columnCount)
            {
                throw Error.GetError(0x15da);
            }
            return num1;
        }

        public virtual string[] GetColumnNames()
        {
            if (this._unionCorrespondingColumns == null)
            {
                return this._leftQueryExpression.GetColumnNames();
            }
            string[] a = new string[this._unionCorrespondingColumns.Size()];
            this._unionCorrespondingColumns.ToArray(a);
            return a;
        }

        public virtual byte[] GetColumnNullability()
        {
            return this.UnionColumnNullability;
        }

        public HashMappedList<string, ColumnSchema> GetColumns()
        {
            this.GetResultTable();
            return ((TableDerived) this.GetResultTable()).ColumnList;
        }

        public virtual SqlType[] GetColumnTypes()
        {
            return this.UnionColumnTypes;
        }

        public QuerySpecification GetMainSelect()
        {
            if (this._leftQueryExpression == null)
            {
                return (QuerySpecification) this;
            }
            return this._leftQueryExpression.GetMainSelect();
        }

        public virtual ResultMetaData GetMetaData()
        {
            if (this.resultMetaData != null)
            {
                return this.resultMetaData;
            }
            return this._leftQueryExpression.GetMetaData();
        }

        public virtual Result GetResult(Session session, int maxRows)
        {
            int num = (this._unionType == 2) ? maxRows : 0;
            Result result = this._leftQueryExpression.GetResult(session, num);
            RowSetNavigatorData navigator = (RowSetNavigatorData) result.GetNavigator();
            RowSetNavigatorData other = (RowSetNavigatorData) this._rightQueryExpression.GetResult(session, num).GetNavigator();
            if (this._unionCorresponding)
            {
                bool flag1 = (session.ResultMaxMemoryRows == 0) || ((navigator.GetSize() < session.ResultMaxMemoryRows) && (other.GetSize() < session.ResultMaxMemoryRows));
                RowSetNavigatorData data1 = flag1 ? new RowSetNavigatorData(session, this) : new RowSetNavigatorDataTable(session, this);
                data1.Copy(navigator, this._leftQueryExpression.UnionColumnMap);
                navigator = data1;
                result.SetNavigator(navigator);
                result.MetaData = this.GetMetaData();
                RowSetNavigatorData data3 = flag1 ? new RowSetNavigatorData(session, this) : new RowSetNavigatorDataTable(session, this);
                data3.Copy(other, this._rightQueryExpression.UnionColumnMap);
                other = data3;
                navigator.Reset();
                other.Reset();
            }
            switch (this._unionType)
            {
                case 1:
                    navigator.Union(other);
                    break;

                case 2:
                    navigator.UnionAll(other);
                    break;

                case 3:
                    navigator.Intersect(other);
                    break;

                case 4:
                    navigator.IntersectAll(other);
                    break;

                case 5:
                    navigator.ExceptAll(other);
                    break;

                case 6:
                    navigator.Except(other);
                    break;

                default:
                    throw Error.RuntimeError(0xc9, "QueryExpression");
            }
            if (this.ExprSortAndSlice.HasOrder())
            {
                navigator.SortUnion(this.ExprSortAndSlice);
            }
            if (this.ExprSortAndSlice.HasLimit())
            {
                navigator.Trim(this.ExprSortAndSlice.GetLimitStart(session), this.ExprSortAndSlice.GetLimitCount(session, maxRows));
            }
            navigator.Reset();
            return result;
        }

        public QNameManager.QName[] GetResultColumnNames()
        {
            if (this.ResultTable == null)
            {
                return this._leftQueryExpression.GetResultColumnNames();
            }
            HashMappedList<string, ColumnSchema> columnList = ((TableDerived) this.ResultTable).ColumnList;
            QNameManager.QName[] nameArray2 = new QNameManager.QName[columnList.Size()];
            for (int i = 0; i < nameArray2.Length; i++)
            {
                nameArray2[i] = columnList.Get(i).GetName();
            }
            return nameArray2;
        }

        public TableBase GetResultTable()
        {
            if (this.ResultTable != null)
            {
                return this.ResultTable;
            }
            if (this._leftQueryExpression != null)
            {
                return this._leftQueryExpression.GetResultTable();
            }
            return null;
        }

        public object[] GetSingleRowValues(Session session)
        {
            Result result = this.GetResult(session, 2);
            int size = result.GetNavigator().GetSize();
            if (size == 0)
            {
                return null;
            }
            if (size != 1)
            {
                throw Error.GetError(0xc81);
            }
            return result.GetSingleRowData();
        }

        public virtual OrderedHashSet<SubQuery> GetSubqueries()
        {
            return OrderedHashSet<SubQuery>.AddAll(this._leftQueryExpression.GetSubqueries(), this._rightQueryExpression.GetSubqueries());
        }

        private HashMappedList<string, ColumnSchema> GetUnionColumns()
        {
            if (this._unionCorresponding || (this._leftQueryExpression == null))
            {
                HashMappedList<string, ColumnSchema> columnList = ((TableDerived) this.ResultTable).ColumnList;
                HashMappedList<string, ColumnSchema> list3 = new HashMappedList<string, ColumnSchema>();
                for (int i = 0; i < this.UnionColumnMap.Length; i++)
                {
                    ColumnSchema schema = columnList.Get(i);
                    list3.Add(schema.GetName().Name, schema);
                }
                return list3;
            }
            return this._leftQueryExpression.GetUnionColumns();
        }

        public List<Expression> GetUnresolvedExpressions()
        {
            return this.UnresolvedExpressions;
        }

        public object[] GetValues(Session session)
        {
            Result result = this.GetResult(session, 2);
            int size = result.GetNavigator().GetSize();
            if (size == 0)
            {
                return new object[0];
            }
            if (size != 1)
            {
                throw Error.GetError(0xc81);
            }
            return result.GetSingleRowData();
        }

        public virtual bool HasReference(RangeVariable range)
        {
            if (!this._leftQueryExpression.HasReference(range))
            {
                return this._rightQueryExpression.HasReference(range);
            }
            return true;
        }

        public virtual bool IsEquivalent(QueryExpression other)
        {
            if (!this._leftQueryExpression.IsEquivalent(other._leftQueryExpression) || (this._unionType != other._unionType))
            {
                return false;
            }
            if (this._rightQueryExpression != null)
            {
                return this._rightQueryExpression.IsEquivalent(other._rightQueryExpression);
            }
            return (other._rightQueryExpression == null);
        }

        public virtual bool IsSingleColumn()
        {
            return this._leftQueryExpression.IsSingleColumn();
        }

        public virtual void ReplaceColumnReference(RangeVariable range, Expression[] list)
        {
            this._leftQueryExpression.ReplaceColumnReference(range, list);
            this._rightQueryExpression.ReplaceColumnReference(range, list);
        }

        public virtual void ReplaceRangeVariables(RangeVariable[] ranges, RangeVariable[] newRanges)
        {
            this._leftQueryExpression.ReplaceRangeVariables(ranges, newRanges);
            this._rightQueryExpression.ReplaceRangeVariables(ranges, newRanges);
        }

        public void Resolve(Session session)
        {
            this.ResolveReferences(session, RangeVariable.EmptyArray);
            ExpressionColumn.CheckColumnsResolved(this.UnresolvedExpressions);
            this.ResolveTypes(session);
        }

        public void Resolve(Session session, RangeVariable[] outerRanges, SqlType[] targetTypes)
        {
            this.ResolveReferences(session, outerRanges);
            if (this.UnresolvedExpressions != null)
            {
                for (int i = 0; i < this.UnresolvedExpressions.Count; i++)
                {
                    ExpressionColumn.CheckColumnsResolved(this.UnresolvedExpressions[i].ResolveColumnReferences(outerRanges, null));
                }
            }
            this.ResolveTypesPartOne(session);
            if (targetTypes != null)
            {
                for (int i = 0; (i < this.UnionColumnTypes.Length) && (i < targetTypes.Length); i++)
                {
                    if (this.UnionColumnTypes[i] == null)
                    {
                        this.UnionColumnTypes[i] = targetTypes[i];
                    }
                }
            }
            this.ResolveTypesPartTwo(session);
        }

        private void ResolveColumnRefernecesInUnionOrderBy()
        {
            int orderLength = this.ExprSortAndSlice.GetOrderLength();
            if (orderLength != 0)
            {
                string[] columnNames = this.GetColumnNames();
                int num2 = 0;
                while (num2 < orderLength)
                {
                    ExpressionOrderBy by = this.ExprSortAndSlice.ExprList[num2];
                    Expression leftNode = by.GetLeftNode();
                    if (leftNode.GetExprType() == 1)
                    {
                        if (leftNode.GetDataType().TypeCode != 4)
                        {
                            goto Label_00B0;
                        }
                        int num3 = Convert.ToInt32(leftNode.GetValue(null));
                        if ((0 >= num3) || (num3 > columnNames.Length))
                        {
                            goto Label_00B0;
                        }
                        by.GetLeftNode().QueryTableColumnIndex = num3 - 1;
                    }
                    else
                    {
                        if (leftNode.GetExprType() != 2)
                        {
                            goto Label_00B0;
                        }
                        int num4 = ArrayUtil.Find(columnNames, leftNode.GetColumnName());
                        if (num4 < 0)
                        {
                            goto Label_00B0;
                        }
                        by.GetLeftNode().QueryTableColumnIndex = num4;
                    }
                    num2++;
                    continue;
                Label_00B0:
                    throw Error.GetError(0x15c8);
                }
                this.ExprSortAndSlice.Prepare(null);
            }
        }

        public virtual void ResolveReferences(Session session, RangeVariable[] outerRanges)
        {
            this._leftQueryExpression.ResolveReferences(session, outerRanges);
            this._rightQueryExpression.ResolveReferences(session, outerRanges);
            this.AddUnresolvedExpressions(this._leftQueryExpression.UnresolvedExpressions);
            this.AddUnresolvedExpressions(this._rightQueryExpression.UnresolvedExpressions);
            if (this._unionCorresponding)
            {
                string[] columnNames = this._leftQueryExpression.GetColumnNames();
                string[] array = this._rightQueryExpression.GetColumnNames();
                if (this._unionCorrespondingColumns == null)
                {
                    this._unionCorrespondingColumns = new OrderedHashSet<string>();
                    OrderedIntHashSet set = new OrderedIntHashSet();
                    OrderedIntHashSet set2 = new OrderedIntHashSet();
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        string str = columnNames[i];
                        int index = ArrayUtil.Find(array, str);
                        if ((str.Length > 0) && (index != -1))
                        {
                            if (!this._leftQueryExpression.AccessibleColumns[i])
                            {
                                throw Error.GetError(0x15ca);
                            }
                            if (!this._rightQueryExpression.AccessibleColumns[index])
                            {
                                throw Error.GetError(0x15ca);
                            }
                            set.Add(i);
                            set2.Add(index);
                            this._unionCorrespondingColumns.Add(str);
                        }
                    }
                    if (this._unionCorrespondingColumns.IsEmpty())
                    {
                        throw Error.GetError(0x15ca);
                    }
                    this._leftQueryExpression.UnionColumnMap = set.ToArray();
                    this._rightQueryExpression.UnionColumnMap = set2.ToArray();
                }
                else
                {
                    this._leftQueryExpression.UnionColumnMap = new int[this._unionCorrespondingColumns.Size()];
                    this._rightQueryExpression.UnionColumnMap = new int[this._unionCorrespondingColumns.Size()];
                    for (int i = 0; i < this._unionCorrespondingColumns.Size(); i++)
                    {
                        string str2 = this._unionCorrespondingColumns.Get(i);
                        int index = ArrayUtil.Find(columnNames, str2);
                        if (index == -1)
                        {
                            throw Error.GetError(0x157d);
                        }
                        if (!this._leftQueryExpression.AccessibleColumns[index])
                        {
                            throw Error.GetError(0x15ca);
                        }
                        this._leftQueryExpression.UnionColumnMap[i] = index;
                        index = ArrayUtil.Find(array, str2);
                        if (index == -1)
                        {
                            throw Error.GetError(0x157d);
                        }
                        if (!this._rightQueryExpression.AccessibleColumns[index])
                        {
                            throw Error.GetError(0x15ca);
                        }
                        this._rightQueryExpression.UnionColumnMap[i] = index;
                    }
                }
                this._columnCount = this._unionCorrespondingColumns.Size();
                this.UnionColumnTypes = new SqlType[this._columnCount];
                this.UnionColumnNullability = new byte[this._columnCount];
                this.ResolveColumnRefernecesInUnionOrderBy();
            }
            else
            {
                this._columnCount = this._leftQueryExpression.GetColumnCount();
                int columnCount = this._rightQueryExpression.GetColumnCount();
                if (this._columnCount != columnCount)
                {
                    throw Error.GetError(0x15da);
                }
                this.UnionColumnTypes = new SqlType[this._columnCount];
                this.UnionColumnNullability = new byte[this._columnCount];
                this._leftQueryExpression.UnionColumnMap = this._rightQueryExpression.UnionColumnMap = new int[this._columnCount];
                ArrayUtil.FillSequence(this._leftQueryExpression.UnionColumnMap);
                this.ResolveColumnRefernecesInUnionOrderBy();
            }
        }

        public virtual void ResolveTypes(Session session)
        {
            if (!this.IsResolved)
            {
                this.ResolveTypesPartOne(session);
                this.ResolveTypesPartTwo(session);
                this.IsResolved = true;
            }
        }

        public virtual void ResolveTypesPartOne(Session session)
        {
            ArrayUtil.ProjectRowReverse(this._leftQueryExpression.UnionColumnTypes, this._leftQueryExpression.UnionColumnMap, this.UnionColumnTypes);
            ArrayUtil.ProjectRowReverse(this._leftQueryExpression.UnionColumnNullability, this._leftQueryExpression.UnionColumnMap, this.UnionColumnNullability);
            this._leftQueryExpression.ResolveTypesPartOne(session);
            ArrayUtil.ProjectRow(this._leftQueryExpression.UnionColumnTypes, this._leftQueryExpression.UnionColumnMap, this.UnionColumnTypes);
            ArrayUtil.ProjectRow(this._leftQueryExpression.UnionColumnNullability, this._leftQueryExpression.UnionColumnMap, this.UnionColumnNullability);
            ArrayUtil.ProjectRowReverse(this._rightQueryExpression.UnionColumnTypes, this._rightQueryExpression.UnionColumnMap, this.UnionColumnTypes);
            ArrayUtil.ProjectRowReverse(this._rightQueryExpression.UnionColumnNullability, this._rightQueryExpression.UnionColumnMap, this.UnionColumnNullability);
            this._rightQueryExpression.ResolveTypesPartOne(session);
            ArrayUtil.ProjectRow(this._rightQueryExpression.UnionColumnTypes, this._rightQueryExpression.UnionColumnMap, this.UnionColumnTypes);
            ArrayUtil.ProjectRow(this._rightQueryExpression.UnionColumnNullability, this._rightQueryExpression.UnionColumnMap, this.UnionColumnNullability);
        }

        public virtual void ResolveTypesPartTwo(Session session)
        {
            ArrayUtil.ProjectRowReverse(this._leftQueryExpression.UnionColumnTypes, this._leftQueryExpression.UnionColumnMap, this.UnionColumnTypes);
            ArrayUtil.ProjectRowReverse(this._leftQueryExpression.UnionColumnNullability, this._leftQueryExpression.UnionColumnMap, this.UnionColumnNullability);
            this._leftQueryExpression.ResolveTypesPartTwo(session);
            ArrayUtil.ProjectRowReverse(this._rightQueryExpression.UnionColumnTypes, this._rightQueryExpression.UnionColumnMap, this.UnionColumnTypes);
            ArrayUtil.ProjectRowReverse(this._rightQueryExpression.UnionColumnNullability, this._rightQueryExpression.UnionColumnMap, this.UnionColumnNullability);
            this._rightQueryExpression.ResolveTypesPartTwo(session);
            if (this._unionCorresponding)
            {
                this.resultMetaData = this._leftQueryExpression.GetMetaData().GetNewMetaData(this._leftQueryExpression.UnionColumnMap);
                this.CreateTable(session);
            }
            if (this.ExprSortAndSlice.HasOrder())
            {
                QueryExpression expression = this;
                while ((expression._leftQueryExpression != null) && !expression._unionCorresponding)
                {
                    expression = expression._leftQueryExpression;
                }
                this.ExprSortAndSlice.SetIndex(null, expression.ResultTable);
            }
        }

        public void SetColumnsDefined()
        {
            if (this._leftQueryExpression != null)
            {
                this._leftQueryExpression.SetColumnsDefined();
            }
        }

        public void SetFullOrder()
        {
            this.IsFullOrder = true;
            if (this._leftQueryExpression == null)
            {
                if (this.IsResolved)
                {
                    ((QuerySpecification) this).CreateFullIndex(null);
                }
            }
            else
            {
                this._leftQueryExpression.SetFullOrder();
                this._rightQueryExpression.SetFullOrder();
            }
        }

        public virtual void SetReturningResult()
        {
            if (this.compileContext.GetSequences().Length != 0)
            {
                throw Error.GetError(0x15de);
            }
            this.IsTopLevel = true;
            this.SetReturningResultSet();
        }

        public virtual void SetReturningResultSet()
        {
            if (this._unionCorresponding)
            {
                this.PersistenceScope = 0x17;
            }
            else if (this._leftQueryExpression != null)
            {
                this._leftQueryExpression.SetReturningResultSet();
            }
        }

        public void SetUnionCorresoponding()
        {
            this._unionCorresponding = true;
        }

        public void SetUnionCorrespondingColumns(OrderedHashSet<string> names)
        {
            this._unionCorrespondingColumns = names;
        }

        public void SetView(View view)
        {
            this.IsUpdatable = true;
            this.view = view;
            this.AcceptsSequences = true;
            this.IsTopLevel = true;
        }
    }
}

