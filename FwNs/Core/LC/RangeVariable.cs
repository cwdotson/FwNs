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
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class RangeVariable
    {
        public static RangeVariable[] EmptyArray = new RangeVariable[0];
        private readonly QNameManager.SimpleName[] _columnAliasNames;
        private readonly OrderedHashSet<string> _columnAliases;
        private OrderedHashSet<QNameManager.QName> _columnNames;
        public bool[] ColumnsInGroupBy;
        private object[] _emptyData;
        public bool HasKeyedColumnInGroupBy;
        public bool IsBoundary;
        public bool IsGenerated;
        public bool IsLeftJoin;
        public bool IsRightJoin;
        public bool IsVariable;
        public Expression JoinCondition;
        public RangeVariableConditions[] JoinConditions;
        public int Level;
        public HashMap<string, Expression> NamedJoinColumnExpressions;
        public OrderedHashSet<string> NamedJoinColumns;
        public int ParsePosition;
        public int RangePosition;
        public Table RangeTable;
        public QNameManager.SimpleName TableAlias;
        public bool[] UpdatedColumns;
        public bool[] UsedColumns;
        public HashMappedList<string, ColumnSchema> Variables;
        public RangeVariableConditions[] WhereConditions;

        public RangeVariable(HashMappedList<string, ColumnSchema> variables, bool isVariable)
        {
            this.Variables = variables;
            this.IsVariable = isVariable;
            this.RangeTable = null;
            this.TableAlias = null;
            this._emptyData = null;
            this.ColumnsInGroupBy = null;
            this.UsedColumns = null;
            this.JoinConditions = new RangeVariableConditions[] { new RangeVariableConditions(this, true) };
            this.WhereConditions = new RangeVariableConditions[] { new RangeVariableConditions(this, false) };
        }

        public RangeVariable(Table table, int position)
        {
            this.RangeTable = table;
            this.TableAlias = null;
            this._emptyData = this.RangeTable.GetEmptyRowData();
            this.ColumnsInGroupBy = this.RangeTable.GetNewColumnCheckList();
            this.UsedColumns = this.RangeTable.GetNewColumnCheckList();
            this.RangePosition = position;
            this.JoinConditions = new RangeVariableConditions[] { new RangeVariableConditions(this, true) };
            this.WhereConditions = new RangeVariableConditions[] { new RangeVariableConditions(this, false) };
        }

        public RangeVariable(Table table, QNameManager.SimpleName alias, OrderedHashSet<string> columnList, QNameManager.SimpleName[] columnNameList, ParserDQL.CompileContext compileContext)
        {
            this.RangeTable = table;
            this.TableAlias = alias;
            this._columnAliases = columnList;
            this._columnAliasNames = columnNameList;
            this.JoinConditions = new RangeVariableConditions[] { new RangeVariableConditions(this, true) };
            this.JoinConditions[0].RangeIndex = this.RangeTable.GetPrimaryIndex();
            this.WhereConditions = new RangeVariableConditions[] { new RangeVariableConditions(this, false) };
            compileContext.RegisterRangeVariable(this);
            SubQuery subQuery = this.RangeTable.GetSubQuery();
            if ((subQuery == null) || subQuery.IsResolved())
            {
                this.SetRangeTableVariables();
            }
        }

        public void AddColumn(int columnIndex)
        {
            this.UsedColumns[columnIndex] = true;
        }

        public void AddJoinCondition(Expression e)
        {
            this.JoinCondition = ExpressionLogical.AndExpressions(this.JoinCondition, e);
        }

        public void AddNamedJoinColumnExpression(string name, Expression e)
        {
            if (this.NamedJoinColumnExpressions == null)
            {
                this.NamedJoinColumnExpressions = new HashMap<string, Expression>();
            }
            this.NamedJoinColumnExpressions.Put(name, e);
        }

        public void AddNamedJoinColumns(OrderedHashSet<string> columns)
        {
            this.NamedJoinColumns = columns;
        }

        public void AddTableColumns(List<Expression> exprList)
        {
            if (this.NamedJoinColumns != null)
            {
                int count = exprList.Count;
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    Expression item = exprList[i];
                    string columnName = item.GetColumnName();
                    if (this.NamedJoinColumns.Contains(columnName))
                    {
                        if (index != i)
                        {
                            exprList.RemoveAt(i);
                            exprList.Insert(index, item);
                        }
                        item = this.GetColumnExpression(columnName);
                        exprList[index] = item;
                        index++;
                    }
                }
            }
            this.AddTableColumns(exprList, exprList.Count, this.NamedJoinColumns);
        }

        public void AddTableColumns(Expression expression, HashSet<string> exclude)
        {
            List<Expression> list = new List<Expression>();
            Table table = this.GetTable();
            int columnCount = table.GetColumnCount();
            for (int i = 0; i < columnCount; i++)
            {
                ColumnSchema column = table.GetColumn(i);
                string item = (this._columnAliases == null) ? column.GetName().Name : this._columnAliases.Get(i);
                if ((exclude == null) || !exclude.Contains(item))
                {
                    Expression expression2 = new ExpressionColumn(this, i);
                    list.Add(expression2);
                }
            }
            Expression[] expressionArray = list.ToArray();
            expression.nodes = expressionArray;
        }

        public int AddTableColumns(List<Expression> expList, int position, UtlHashSet<string> exclude)
        {
            Table table = this.GetTable();
            int columnCount = table.GetColumnCount();
            for (int i = 0; i < columnCount; i++)
            {
                ColumnSchema column = table.GetColumn(i);
                string key = (this._columnAliases == null) ? column.GetName().Name : this._columnAliases.Get(i);
                if ((exclude == null) || !exclude.Contains(key))
                {
                    Expression item = new ExpressionColumn(this, i);
                    expList.Insert(position++, item);
                }
            }
            return position;
        }

        public string Describe(Session session)
        {
            RangeVariableConditions[] joinConditions = this.JoinConditions;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < joinConditions.Length; i++)
            {
                RangeVariableConditions conditions = this.JoinConditions[i];
                if (i > 0)
                {
                    builder.Append("\nOR condition = [");
                    builder.Append(conditions.Describe(session)).Append("]\n");
                }
                else
                {
                    bool flag = !conditions.HasIndexCondition();
                    builder.Append("table=[").Append(this.RangeTable.GetName().Name).Append("]\n");
                    if (this.TableAlias != null)
                    {
                        builder.Append("alias=[").Append(this.TableAlias.Name).Append("]\n");
                    }
                    builder.Append("access=[").Append(flag ? "FULL SCAN" : "INDEX PRED").Append("]\n");
                    string str = "INNER";
                    if (this.IsLeftJoin)
                    {
                        str = "LEFT OUTER";
                        if (this.IsRightJoin)
                        {
                            str = "FULL";
                        }
                    }
                    else if (this.IsRightJoin)
                    {
                        str = "RIGHT OUTER";
                    }
                    builder.Append("join type=[").Append(str).Append("]\n");
                    builder.Append(conditions.Describe(session));
                }
            }
            return builder.ToString();
        }

        public RangeVariable Duplicate()
        {
            RangeVariable variable;
            try
            {
                variable = (RangeVariable) base.MemberwiseClone();
            }
            catch (Exception)
            {
                throw Error.RuntimeError(0xc9, "RangeVariable");
            }
            variable.ResetConditions();
            return variable;
        }

        public int FindColumn(string columnName)
        {
            if ((this.NamedJoinColumnExpressions != null) && this.NamedJoinColumnExpressions.ContainsKey(columnName))
            {
                return -1;
            }
            if (this.Variables != null)
            {
                return this.Variables.GetIndex(columnName);
            }
            if (this._columnAliases != null)
            {
                return this._columnAliases.GetIndex(columnName);
            }
            return this.RangeTable.FindColumn(columnName);
        }

        public ColumnSchema GetColumn(int i)
        {
            if (this.Variables != null)
            {
                return this.Variables.Get(i);
            }
            return this.RangeTable.GetColumn(i);
        }

        public ColumnSchema GetColumn(string columnName)
        {
            int i = this.FindColumn(columnName);
            if (i >= 0)
            {
                return this.RangeTable.GetColumn(i);
            }
            return null;
        }

        public string GetColumnAlias(int i)
        {
            return this.GetColumnAliasName(i).Name;
        }

        public QNameManager.SimpleName GetColumnAliasName(int i)
        {
            if (this._columnAliases != null)
            {
                return this._columnAliasNames[i];
            }
            return this.RangeTable.GetColumn(i).GetName();
        }

        public ExpressionColumn GetColumnExpression(string name)
        {
            if (this.NamedJoinColumnExpressions != null)
            {
                return (ExpressionColumn) this.NamedJoinColumnExpressions.Get(name);
            }
            return null;
        }

        public string GetColumnNameForAlias(string alias)
        {
            if (this._columnAliases == null)
            {
                return alias;
            }
            int index = this._columnAliases.GetIndex(alias);
            if (index == -1)
            {
                return alias;
            }
            return this.RangeTable.GetColumn(index).GetName().Name;
        }

        public OrderedHashSet<QNameManager.QName> GetColumnNames()
        {
            if (this._columnNames == null)
            {
                this._columnNames = new OrderedHashSet<QNameManager.QName>();
                this.RangeTable.GetColumnNames(this.UsedColumns, this._columnNames);
            }
            return this._columnNames;
        }

        public RangeIteratorMain GetIterator(Session session)
        {
            RangeIteratorMain iterator = this.IsRightJoin ? new RangeIteratorRight(session, this, null) : new RangeIteratorMain(session, this);
            session.sessionContext.SetRangeIterator(iterator);
            return iterator;
        }

        public static RangeIteratorMain GetIterator(Session session, RangeVariable[] rangeVars)
        {
            if (rangeVars.Length == 1)
            {
                return rangeVars[0].GetIterator(session);
            }
            RangeIteratorMain[] rangeIterators = new RangeIteratorMain[rangeVars.Length];
            for (int i = 0; i < rangeVars.Length; i++)
            {
                rangeIterators[i] = rangeVars[i].GetIterator(session);
            }
            return new RangeIteratorJoined(rangeIterators);
        }

        public Expression GetJoinCondition()
        {
            return this.JoinCondition;
        }

        public Index GetSortIndex()
        {
            if (this.JoinConditions.Length == 1)
            {
                return this.JoinConditions[0].RangeIndex;
            }
            return null;
        }

        public OrderedHashSet<SubQuery> GetSubqueries()
        {
            OrderedHashSet<SubQuery> set = null;
            if (this.JoinCondition != null)
            {
                set = this.JoinCondition.CollectAllSubqueries(set);
            }
            if (!(this.RangeTable is TableDerived))
            {
                return set;
            }
            QueryExpression queryExpression = this.RangeTable.GetQueryExpression();
            if (((TableDerived) this.RangeTable).view != null)
            {
                if (set == null)
                {
                    set = new OrderedHashSet<SubQuery>();
                }
                set.AddAll(((TableDerived) this.RangeTable).view.GetSubqueries());
                return set;
            }
            if (queryExpression == null)
            {
                return OrderedHashSet<SubQuery>.Add(set, this.RangeTable.GetSubQuery());
            }
            OrderedHashSet<SubQuery> subqueries = queryExpression.GetSubqueries();
            set = OrderedHashSet<SubQuery>.AddAll(set, subqueries);
            SubQuery subQuery = this.RangeTable.GetSubQuery();
            return OrderedHashSet<SubQuery>.AddAll(OrderedHashSet<SubQuery>.Add(set, subQuery), subQuery.GetExtraSubqueries());
        }

        public Table GetTable()
        {
            return this.RangeTable;
        }

        public string GetTableAlias()
        {
            return this.GetTableAliasName().Name;
        }

        public QNameManager.SimpleName GetTableAliasName()
        {
            return (this.TableAlias ?? this.RangeTable.GetName());
        }

        public OrderedHashSet<string> GetUniqueColumnNameSet()
        {
            OrderedHashSet<string> set = new OrderedHashSet<string>();
            if (this._columnAliases != null)
            {
                set.AddAll(this._columnAliases);
                return set;
            }
            for (int i = 0; i < this.RangeTable.ColumnList.Size(); i++)
            {
                string name = this.RangeTable.GetColumn(i).GetName().Name;
                if (!set.Add(name))
                {
                    throw Error.GetError(0x15ca, name);
                }
            }
            return set;
        }

        public bool HasColumnAlias()
        {
            return (this._columnAliases > null);
        }

        public bool HasIndexCondition()
        {
            return ((this.JoinConditions.Length == 1) && (this.JoinConditions[0].IndexedColumnCount > 0));
        }

        public void ReplaceColumnReference(RangeVariable range, Expression[] list)
        {
            if (this.JoinCondition != null)
            {
                this.JoinCondition.ReplaceColumnReferences(range, list);
            }
        }

        public void ReplaceRangeVariables(RangeVariable[] ranges, RangeVariable[] newRanges)
        {
            if (this.JoinCondition != null)
            {
                this.JoinCondition.ReplaceRangeVariables(ranges, newRanges);
            }
        }

        public void ResetConditions()
        {
            Index rangeIndex = this.JoinConditions[0].RangeIndex;
            this.JoinConditions = new RangeVariableConditions[] { new RangeVariableConditions(this, true) };
            this.JoinConditions[0].RangeIndex = rangeIndex;
            this.WhereConditions = new RangeVariableConditions[] { new RangeVariableConditions(this, false) };
        }

        public void ResolveRangeTable(Session session, RangeVariable[] rangeVariables, int rangeCount, RangeVariable[] outerRanges)
        {
            SubQuery subQuery = this.RangeTable.GetSubQuery();
            if ((subQuery != null) && !subQuery.IsResolved())
            {
                if (subQuery.DataExpression != null)
                {
                    List<Expression> list = subQuery.DataExpression.ResolveColumnReferences(EmptyArray, null);
                    if (list != null)
                    {
                        list = subQuery.DataExpression.ResolveColumnReferences(rangeVariables, rangeCount, null, true);
                    }
                    if (list != null)
                    {
                        list = subQuery.DataExpression.ResolveColumnReferences(outerRanges, null);
                    }
                    if (list != null)
                    {
                        throw Error.GetError(0x157d, list[0].GetSql());
                    }
                    subQuery.DataExpression.ResolveTypes(session, null);
                    this.SetRangeTableVariables();
                }
                if (subQuery.queryExpression != null)
                {
                    subQuery.queryExpression.ResolveReferences(session, outerRanges);
                    List<Expression> unresolvedExpressions = subQuery.queryExpression.GetUnresolvedExpressions();
                    List<Expression> list3 = Expression.ResolveColumnSet(rangeVariables, rangeCount, unresolvedExpressions, null);
                    if (list3 != null)
                    {
                        throw Error.GetError(0x157d, list3[0].GetSql());
                    }
                    subQuery.queryExpression.ResolveTypes(session);
                    subQuery.PrepareTable(session);
                    subQuery.SetCorrelated();
                    this.SetRangeTableVariables();
                }
            }
        }

        public bool ResolvesSchemaName(string name)
        {
            return ((name == null) || ((this.TableAlias == null) && name.Equals(this.RangeTable.GetSchemaName().Name)));
        }

        public bool ResolvesTableName(ExpressionColumn e)
        {
            if (e.TableName == null)
            {
                return true;
            }
            if (this.RangeTable != null)
            {
                if (e.schema == null)
                {
                    if (this.TableAlias != null)
                    {
                        if (e.TableName.Equals(this.TableAlias.Name))
                        {
                            return true;
                        }
                    }
                    else if (e.TableName.Equals(this.RangeTable.GetName().Name))
                    {
                        return true;
                    }
                }
                else if (((this.TableAlias == null) && e.TableName.Equals(this.RangeTable.GetName().Name)) && e.schema.Equals(this.RangeTable.GetSchemaName().Name))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ResolvesTableName(string name)
        {
            if (name == null)
            {
                return true;
            }
            if (this.TableAlias == null)
            {
                if (name.Equals(this.RangeTable.GetName().Name))
                {
                    return true;
                }
            }
            else if (name.Equals(this.TableAlias.Name))
            {
                return true;
            }
            return false;
        }

        public bool ReverseOrder()
        {
            this.JoinConditions[0].ReverseIndexCondition();
            return true;
        }

        public void SetForCheckConstraint()
        {
            this.JoinConditions[0].RangeIndex = null;
        }

        public void SetJoinType(bool isLeft, bool isRight)
        {
            this.IsLeftJoin = isLeft;
            this.IsRightJoin = isRight;
            if (this.IsRightJoin)
            {
                this.WhereConditions[0].RangeIndex = this.RangeTable.GetPrimaryIndex();
            }
        }

        public void SetRangeTableVariables()
        {
            if ((this._columnAliasNames != null) && (this.RangeTable.GetColumnCount() != this._columnAliasNames.Length))
            {
                throw Error.GetError(0x15d9);
            }
            this._emptyData = this.RangeTable.GetEmptyRowData();
            this.ColumnsInGroupBy = this.RangeTable.GetNewColumnCheckList();
            this.UsedColumns = this.RangeTable.GetNewColumnCheckList();
            this.JoinConditions[0].RangeIndex = this.RangeTable.GetPrimaryIndex();
        }

        public bool SetSortIndex(Index index, bool reversed)
        {
            if ((this.JoinConditions.Length == 1) && (this.JoinConditions[0].IndexedColumnCount == 0))
            {
                this.JoinConditions[0].RangeIndex = index;
                this.JoinConditions[0].Reversed = reversed;
                return true;
            }
            return false;
        }

        public class RangeIteratorBase : IRangeIterator, IRowIterator
        {
            public bool isBeforeFirst;
            public object[] CurrentData;
            public Row CurrentRow;
            public IRowIterator It;
            public int rangePosition;
            public RangeVariable RangeVar;
            public Session session;
            public IPersistentStore Store;

            public RangeIteratorBase()
            {
            }

            public RangeIteratorBase(Session session, IPersistentStore store, TableBase t, int position)
            {
                this.session = session;
                this.rangePosition = position;
                this.Store = store;
                this.It = t.GetRowIterator(store);
                this.isBeforeFirst = true;
            }

            public object[] GetCurrent()
            {
                return this.CurrentData;
            }

            public Row GetCurrentRow()
            {
                return this.CurrentRow;
            }

            public object[] GetNext()
            {
                throw Error.RuntimeError(0xc9, "RangeVariable");
            }

            public Row GetNextRow()
            {
                throw Error.RuntimeError(0xc9, "RangeVariable");
            }

            public virtual RangeVariable GetRange()
            {
                return this.RangeVar;
            }

            public virtual int GetRangePosition()
            {
                return this.rangePosition;
            }

            public long GetRowId()
            {
                if (this.CurrentRow != null)
                {
                    return ((this.RangeVar.RangeTable.GetId() << 0x20) + this.CurrentRow.GetPos());
                }
                return 0L;
            }

            public object GetRowidObject()
            {
                if (this.CurrentRow != null)
                {
                    return this.GetRowId();
                }
                return null;
            }

            public bool HasNext()
            {
                throw Error.RuntimeError(0xc9, "RangeVariable");
            }

            public virtual bool IsBeforeFirst()
            {
                return this.isBeforeFirst;
            }

            public virtual bool Next()
            {
                if (this.isBeforeFirst)
                {
                    this.isBeforeFirst = false;
                }
                else if (this.It == null)
                {
                    return false;
                }
                this.CurrentRow = this.It.GetNextRow();
                if (this.CurrentRow == null)
                {
                    return false;
                }
                this.CurrentData = this.CurrentRow.RowData;
                return true;
            }

            public virtual void Release()
            {
                if (this.It != null)
                {
                    this.It.Release();
                }
            }

            public virtual void Remove()
            {
            }

            public virtual void Reset()
            {
                if (this.It != null)
                {
                    this.It.Release();
                }
                this.It = null;
                this.CurrentRow = null;
                this.isBeforeFirst = true;
            }

            public void SetCurrent(object[] data)
            {
                this.CurrentData = data;
            }

            public bool SetRowColumns(bool[] columns)
            {
                throw Error.RuntimeError(0xc9, "RangeVariable");
            }
        }

        public class RangeIteratorJoined : RangeVariable.RangeIteratorMain
        {
            private readonly RangeVariable.RangeIteratorMain[] _rangeIterators;
            private int _currentIndex;

            public RangeIteratorJoined(RangeVariable.RangeIteratorMain[] rangeIterators)
            {
                this._rangeIterators = rangeIterators;
            }

            public override RangeVariable GetRange()
            {
                return null;
            }

            public override int GetRangePosition()
            {
                return 0;
            }

            public override bool IsBeforeFirst()
            {
                return base.isBeforeFirst;
            }

            public override bool Next()
            {
                while (this._currentIndex >= 0)
                {
                    RangeVariable.RangeIteratorMain main = this._rangeIterators[this._currentIndex];
                    if (main.Next())
                    {
                        if (this._currentIndex >= (this._rangeIterators.Length - 1))
                        {
                            base.CurrentRow = this._rangeIterators[this._currentIndex].CurrentRow;
                            base.CurrentData = base.CurrentRow.RowData;
                            return true;
                        }
                        this._currentIndex++;
                    }
                    else
                    {
                        main.Reset();
                        this._currentIndex--;
                    }
                }
                base.CurrentData = this._rangeIterators[this._rangeIterators.Length - 1].RangeVar._emptyData;
                base.CurrentRow = null;
                for (int i = 0; i < this._rangeIterators.Length; i++)
                {
                    this._rangeIterators[i].Reset();
                }
                return false;
            }

            public override void Release()
            {
                if (base.It != null)
                {
                    base.It.Release();
                }
                for (int i = 0; i < this._rangeIterators.Length; i++)
                {
                    this._rangeIterators[i].Reset();
                }
            }

            public override void Remove()
            {
            }

            public override void Reset()
            {
                base.Reset();
                for (int i = 0; i < this._rangeIterators.Length; i++)
                {
                    this._rangeIterators[i].Reset();
                }
            }
        }

        public class RangeIteratorMain : RangeVariable.RangeIteratorBase
        {
            protected int CondIndex;
            protected RangeVariable.RangeVariableConditions[] Conditions;
            protected object[] CurrentJoinData;
            public bool HasLeftOuterRow;
            public bool IsFullIterator;
            protected RangeVariable.RangeVariableConditions[] joinConditions;
            protected OrderedIntHashSet Lookup;
            protected RangeVariable.RangeVariableConditions[] whereConditions;

            public RangeIteratorMain()
            {
            }

            public RangeIteratorMain(Session session, RangeVariable rangeVar)
            {
                base.rangePosition = rangeVar.RangePosition;
                base.Store = rangeVar.RangeTable.GetRowStore(session);
                base.session = session;
                base.RangeVar = rangeVar;
                base.CurrentData = rangeVar._emptyData;
                base.isBeforeFirst = true;
                if (rangeVar.IsRightJoin)
                {
                    this.Lookup = new OrderedIntHashSet();
                }
                this.Conditions = rangeVar.JoinConditions;
                if (rangeVar.WhereConditions[0].HasIndexCondition())
                {
                    this.Conditions = rangeVar.WhereConditions;
                }
                this.whereConditions = rangeVar.WhereConditions;
                this.joinConditions = rangeVar.JoinConditions;
            }

            protected void AddFoundRow()
            {
                if (base.RangeVar.IsRightJoin)
                {
                    this.Lookup.Add(base.CurrentRow.GetPos());
                }
            }

            protected virtual bool FindNext()
            {
                bool flag = false;
            Label_0002:
                base.CurrentRow = base.It.GetNextRow();
                if (base.CurrentRow != null)
                {
                    base.CurrentData = base.CurrentRow.RowData;
                    RangeVariable.RangeVariableConditions conditions = this.Conditions[this.CondIndex];
                    if ((conditions.IndexEndCondition == null) || conditions.IndexEndCondition.TestCondition(base.session))
                    {
                        RangeVariable.RangeVariableConditions conditions2 = this.joinConditions[this.CondIndex];
                        if ((conditions2.NonIndexCondition == null) || conditions2.NonIndexCondition.TestCondition(base.session))
                        {
                            RangeVariable.RangeVariableConditions conditions3 = this.whereConditions[this.CondIndex];
                            if ((conditions3.NonIndexCondition == null) || conditions3.NonIndexCondition.TestCondition(base.session))
                            {
                                Expression excludeConditions = conditions.ExcludeConditions;
                                if ((excludeConditions == null) || !excludeConditions.TestCondition(base.session))
                                {
                                    this.AddFoundRow();
                                    this.HasLeftOuterRow = false;
                                    return true;
                                }
                            }
                            else
                            {
                                this.HasLeftOuterRow = false;
                                this.AddFoundRow();
                            }
                        }
                        goto Label_0002;
                    }
                    if (!conditions.IsJoin)
                    {
                        this.HasLeftOuterRow = false;
                    }
                }
                base.It.Release();
                base.CurrentRow = null;
                base.CurrentData = base.RangeVar._emptyData;
                if (this.HasLeftOuterRow && (this.CondIndex == (this.Conditions.Length - 1)))
                {
                    RangeVariable.RangeVariableConditions conditions4 = this.whereConditions[this.CondIndex];
                    flag = (conditions4.NonIndexCondition == null) || conditions4.NonIndexCondition.TestCondition(base.session);
                    this.HasLeftOuterRow = false;
                }
                return flag;
            }

            private void GetFirstRow()
            {
                if ((this.CurrentJoinData == null) || (this.CurrentJoinData.Length < this.Conditions[this.CondIndex].IndexedColumnCount))
                {
                    this.CurrentJoinData = new object[this.Conditions[this.CondIndex].IndexedColumnCount];
                }
                for (int i = 0; i < this.Conditions[this.CondIndex].IndexedColumnCount; i++)
                {
                    int num2 = 0;
                    switch (((i == (this.Conditions[this.CondIndex].IndexedColumnCount - 1)) ? this.Conditions[this.CondIndex].OpType : this.Conditions[this.CondIndex].IndexCond[i].GetExprType()))
                    {
                        case 0x2f:
                        case 0x30:
                        case 0x4a:
                            this.CurrentJoinData[i] = null;
                            break;

                        default:
                        {
                            SqlType dataType = this.Conditions[this.CondIndex].IndexCond[i].GetRightNode().GetDataType();
                            object a = this.Conditions[this.CondIndex].IndexCond[i].GetRightNode().GetValue(base.session);
                            SqlType type2 = this.Conditions[this.CondIndex].IndexCond[i].GetLeftNode().GetDataType();
                            if (((type2 != dataType) && (num2 == 0)) && (type2.TypeComparisonGroup != dataType.TypeComparisonGroup))
                            {
                                a = type2.ConvertToType(base.session, a, dataType);
                            }
                            if (i == 0)
                            {
                                int exprType = this.Conditions[this.CondIndex].IndexCond[0].GetExprType();
                                if (num2 < 0)
                                {
                                    if ((exprType - 0x2a) > 1)
                                    {
                                        base.It = this.Conditions[this.CondIndex].RangeIndex.GetEmptyIterator();
                                        return;
                                    }
                                    a = null;
                                }
                                else if (num2 > 0)
                                {
                                    if (exprType != 0x30)
                                    {
                                        base.It = this.Conditions[this.CondIndex].RangeIndex.GetEmptyIterator();
                                        return;
                                    }
                                    a = null;
                                }
                            }
                            this.CurrentJoinData[i] = a;
                            break;
                        }
                    }
                }
                base.It = this.Conditions[this.CondIndex].RangeIndex.FindFirstRow(base.session, base.Store, this.CurrentJoinData, this.Conditions[this.CondIndex].IndexedColumnCount, this.Conditions[this.CondIndex].OpType, this.Conditions[this.CondIndex].Reversed, null);
            }

            public override int GetRangePosition()
            {
                return base.RangeVar.RangePosition;
            }

            protected virtual void InitialiseIterator()
            {
                if (this.CondIndex == 0)
                {
                    this.HasLeftOuterRow = base.RangeVar.IsLeftJoin;
                }
                if (this.Conditions[this.CondIndex].IsFalse)
                {
                    base.It = this.Conditions[this.CondIndex].RangeIndex.GetEmptyIterator();
                }
                else
                {
                    SubQuery subQuery = base.RangeVar.RangeTable.GetSubQuery();
                    if (subQuery != null)
                    {
                        subQuery.MaterialiseCorrelated(base.session);
                    }
                    if (this.Conditions[this.CondIndex].IndexCond == null)
                    {
                        base.It = this.Conditions[this.CondIndex].Reversed ? this.Conditions[this.CondIndex].RangeIndex.LastRow(base.session, base.Store) : this.Conditions[this.CondIndex].RangeIndex.FirstRow(base.session, base.Store);
                    }
                    else
                    {
                        this.GetFirstRow();
                        if (!this.Conditions[this.CondIndex].IsJoin)
                        {
                            this.HasLeftOuterRow = false;
                        }
                    }
                }
            }

            public override bool IsBeforeFirst()
            {
                return base.isBeforeFirst;
            }

            public override bool Next()
            {
                while (this.CondIndex < this.Conditions.Length)
                {
                    if (base.isBeforeFirst)
                    {
                        base.isBeforeFirst = false;
                        this.InitialiseIterator();
                    }
                    if (this.FindNext())
                    {
                        return true;
                    }
                    this.Reset();
                    this.CondIndex++;
                }
                this.CondIndex = 0;
                return false;
            }

            public override void Remove()
            {
            }

            public override void Reset()
            {
                if (base.It != null)
                {
                    base.It.Release();
                }
                base.It = null;
                base.CurrentData = base.RangeVar._emptyData;
                base.CurrentRow = null;
                base.isBeforeFirst = true;
            }
        }

        public class RangeIteratorRight : RangeVariable.RangeIteratorMain
        {
            private bool _isOnRightOuterRows;

            public RangeIteratorRight(Session session, RangeVariable rangeVar, RangeVariable.RangeIteratorMain main) : base(session, rangeVar)
            {
                base.IsFullIterator = true;
            }

            protected bool FindNextRight()
            {
                bool flag = false;
                do
                {
                    base.CurrentRow = base.It.GetNextRow();
                    if (base.CurrentRow == null)
                    {
                        goto Label_00A5;
                    }
                    base.CurrentData = base.CurrentRow.RowData;
                    if ((base.Conditions[base.CondIndex].IndexEndCondition != null) && !base.Conditions[base.CondIndex].IndexEndCondition.TestCondition(base.session))
                    {
                        goto Label_00A5;
                    }
                }
                while (((base.Conditions[base.CondIndex].NonIndexCondition != null) && !base.Conditions[base.CondIndex].NonIndexCondition.TestCondition(base.session)) || !this.LookupAndTest());
                flag = true;
            Label_00A5:
                if (flag)
                {
                    return true;
                }
                base.It.Release();
                base.CurrentRow = null;
                base.CurrentData = base.RangeVar._emptyData;
                return false;
            }

            private bool LookupAndTest()
            {
                bool flag = !base.Lookup.Contains(base.CurrentRow.GetPos());
                if (flag)
                {
                    base.CurrentData = base.CurrentRow.RowData;
                    if ((base.Conditions[base.CondIndex].NonIndexCondition != null) && !base.Conditions[base.CondIndex].NonIndexCondition.TestCondition(base.session))
                    {
                        flag = false;
                    }
                }
                return flag;
            }

            public override bool Next()
            {
                if (this._isOnRightOuterRows)
                {
                    return ((base.It != null) && this.FindNextRight());
                }
                return base.Next();
            }

            public void SetOnOuterRows()
            {
                base.Conditions = base.RangeVar.WhereConditions;
                this._isOnRightOuterRows = true;
                base.HasLeftOuterRow = false;
                base.CondIndex = 0;
                this.InitialiseIterator();
            }
        }

        public class RangeVariableConditions
        {
            public Expression ExcludeConditions;
            public Expression[] IndexCond;
            public Expression[] IndexEndCond;
            public Expression IndexEndCondition;
            public int IndexedColumnCount;
            public bool IsFalse;
            public bool IsJoin;
            public Expression NonIndexCondition;
            public int OpType;
            public int OpTypeEnd;
            public Index RangeIndex;
            public RangeVariable RangeVar;
            public bool Reversed;

            public RangeVariableConditions(RangeVariable.RangeVariableConditions other)
            {
                this.RangeVar = other.RangeVar;
                this.IsJoin = other.IsJoin;
                this.NonIndexCondition = other.NonIndexCondition;
            }

            public RangeVariableConditions(RangeVariable rangeVar, bool isJoin)
            {
                this.RangeVar = rangeVar;
                this.IsJoin = isJoin;
            }

            public void AddCondition(Expression e)
            {
                if (e != null)
                {
                    this.NonIndexCondition = ExpressionLogical.AndExpressions(this.NonIndexCondition, e);
                    if (Expression.ExprFalse.Equals(this.NonIndexCondition))
                    {
                        this.IsFalse = true;
                    }
                    if (((this.RangeIndex != null) && (this.RangeIndex.GetColumnCount() != 0)) && ((this.IndexedColumnCount != 0) && (e.GetIndexableExpression(this.RangeVar) != null)))
                    {
                        int columnIndex = e.GetLeftNode().GetColumnIndex();
                        int exprType = e.GetExprType();
                        if ((exprType - 0x2a) > 1)
                        {
                            if ((exprType - 0x2c) > 1)
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (this.OpType != 0x30)
                            {
                                this.AddToIndexConditions(e);
                                return;
                            }
                            if (this.RangeIndex.GetColumns()[this.IndexedColumnCount - 1] == columnIndex)
                            {
                                this.NonIndexCondition = ExpressionLogical.AndExpressions(this.NonIndexCondition, this.IndexCond[this.IndexedColumnCount - 1]);
                                this.IndexCond[this.IndexedColumnCount - 1] = e;
                                this.OpType = e.OpType;
                            }
                            return;
                        }
                        if ((((this.OpType == 0x2b) || (this.OpType == 0x2a)) || (this.OpType == 0x30)) && ((this.OpTypeEnd == 0x4a) && (this.RangeIndex.GetColumns()[this.IndexedColumnCount - 1] == columnIndex)))
                        {
                            this.IndexEndCond[this.IndexedColumnCount - 1] = e;
                            this.IndexEndCondition = ExpressionLogical.AndExpressions(this.IndexEndCondition, e);
                            this.OpTypeEnd = e.GetExprType();
                        }
                    }
                }
            }

            public void AddIndexCondition(Expression[] exprList, Index index, int colCount)
            {
                this.RangeIndex = index;
                this.OpType = exprList[0].GetExprType();
                switch (this.OpType)
                {
                    case 0x29:
                    case 0x2f:
                        this.IndexCond = exprList;
                        this.IndexEndCond = new Expression[exprList.Length];
                        for (int i = 0; i < colCount; i++)
                        {
                            Expression expression = exprList[i];
                            this.IndexEndCond[i] = expression;
                            this.IndexEndCondition = ExpressionLogical.AndExpressions(this.IndexEndCondition, expression);
                            this.OpType = expression.GetExprType();
                        }
                        this.OpTypeEnd = this.OpType;
                        break;

                    case 0x2a:
                    case 0x2b:
                        this.IndexCond = exprList;
                        this.IndexEndCond = new Expression[exprList.Length];
                        this.OpTypeEnd = 0x4a;
                        break;

                    case 0x2c:
                    case 0x2d:
                    {
                        Expression leftNode = exprList[0].GetLeftNode();
                        leftNode = new ExpressionLogical(0x2f, leftNode);
                        leftNode = new ExpressionLogical(0x30, leftNode);
                        this.IndexCond = new Expression[] { leftNode };
                        this.IndexEndCond = new Expression[exprList.Length];
                        this.IndexEndCond[0] = this.IndexEndCondition = exprList[0];
                        this.OpTypeEnd = this.OpType;
                        this.OpType = 0x30;
                        break;
                    }
                    case 0x30:
                        this.IndexCond = exprList;
                        this.IndexEndCond = new Expression[exprList.Length];
                        this.OpTypeEnd = 0x4a;
                        break;

                    default:
                        throw Error.RuntimeError(0xc9, "RangeVariable");
                }
                this.IndexedColumnCount = colCount;
            }

            public bool AddToIndexConditions(Expression e)
            {
                if (((this.OpType == 0x29) || (this.OpType == 0x2f)) && ((this.IndexedColumnCount < this.RangeIndex.GetColumnCount()) && (this.RangeIndex.GetColumns()[this.IndexedColumnCount] == e.GetLeftNode().GetColumnIndex())))
                {
                    this.IndexCond[this.IndexedColumnCount] = e;
                    this.IndexedColumnCount++;
                    this.OpType = e.OpType;
                    this.OpTypeEnd = 0x4a;
                    return true;
                }
                return false;
            }

            public string Describe(Session session)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("index=[").Append(this.RangeIndex.GetName().Name).Append("]\n");
                if (this.HasIndexCondition())
                {
                    if (this.IndexedColumnCount > 0)
                    {
                        builder.Append("start conditions=[");
                        for (int i = 0; i < this.IndexedColumnCount; i++)
                        {
                            if (this.IndexCond[i] != null)
                            {
                                builder.Append(this.IndexCond[i].Describe(session, 0));
                            }
                        }
                        builder.Append("]\n");
                    }
                    if (this.IndexEndCondition != null)
                    {
                        string str = this.IndexEndCondition.Describe(session, 0);
                        builder.Append("end condition=[").Append(str).Append("]\n");
                    }
                }
                if (this.NonIndexCondition != null)
                {
                    string str2 = this.NonIndexCondition.Describe(session, 0);
                    builder.Append("other condition=[").Append(str2).Append("]\n");
                }
                return builder.ToString();
            }

            public bool HasIndexCondition()
            {
                return (this.IndexedColumnCount > 0);
            }

            public void ReverseIndexCondition()
            {
                if ((this.OpType != 0x29) && (this.OpType != 0x2f))
                {
                    this.IndexEndCondition = null;
                    for (int i = 0; i < this.IndexedColumnCount; i++)
                    {
                        Expression expression = this.IndexCond[i];
                        this.IndexCond[i] = this.IndexEndCond[i];
                        this.IndexEndCond[i] = expression;
                        this.IndexEndCondition = ExpressionLogical.AndExpressions(this.IndexEndCondition, expression);
                    }
                    this.OpType = this.OpTypeEnd;
                    this.Reversed = true;
                }
            }
        }
    }
}

