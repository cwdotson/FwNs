namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public class SubQuery : IComparer<SubQuery>
    {
        public static readonly SubQuery[] EmptySubqueryArray = new SubQuery[0];
        private readonly bool _isExistsPredicate;
        private readonly bool _fullOrder;
        private bool _isCorrelated;
        protected TableDerived _table;
        protected bool _uniqueRows;
        private QNameManager.SimpleName[] _columnNames;
        public Expression DataExpression;
        public Database database;
        public bool IsDataExpression;
        private bool _isResolved;
        public int Level;
        public View ParentView;
        public int ParsePosition;
        public QueryExpression queryExpression;
        public string Sql;
        public View view;
        public bool IsNamed;
        public SubQuery NamedParentSubQuery;

        public SubQuery(Database database, int level, Expression dataExpression, int mode)
        {
            this.Level = level;
            this.database = database;
            this.DataExpression = dataExpression;
            dataExpression.subQuery = this;
            this.IsDataExpression = true;
            if (mode == 0x36)
            {
                this._uniqueRows = true;
            }
        }

        public SubQuery(Database database, int level, QueryExpression queryExpression, View view)
        {
            this.Level = level;
            this.queryExpression = queryExpression;
            this.database = database;
            this.view = view;
        }

        public SubQuery(Database database, int level, QueryExpression queryExpression, int mode)
        {
            this.Level = level;
            this.queryExpression = queryExpression;
            this.database = database;
            switch (mode)
            {
                case 0x36:
                    this._uniqueRows = true;
                    if (queryExpression == null)
                    {
                        break;
                    }
                    queryExpression.SetFullOrder();
                    return;

                case 0x37:
                    this._isExistsPredicate = true;
                    return;

                case 0x38:
                    break;

                case 0x39:
                    this._fullOrder = true;
                    if (queryExpression != null)
                    {
                        queryExpression.SetFullOrder();
                    }
                    break;

                default:
                    return;
            }
        }

        public int Compare(SubQuery sqa, SubQuery sqb)
        {
            if ((sqa.ParentView == null) && (sqb.ParentView == null))
            {
                return this.GetLevelDiff(sqa, sqb);
            }
            if ((sqa.ParentView != null) && (sqb.ParentView != null))
            {
                int tableIndex = this.database.schemaManager.GetTableIndex(sqa.ParentView);
                int num3 = this.database.schemaManager.GetTableIndex(sqb.ParentView);
                if (tableIndex == -1)
                {
                    tableIndex = this.database.schemaManager.GetTables(sqa.ParentView.GetSchemaName().Name).Size();
                }
                if (num3 == -1)
                {
                    num3 = this.database.schemaManager.GetTables(sqb.ParentView.GetSchemaName().Name).Size();
                }
                int num4 = tableIndex - num3;
                if (num4 != 0)
                {
                    return num4;
                }
                return this.GetLevelDiff(sqa, sqb);
            }
            if (sqa.ParentView != null)
            {
                return -1;
            }
            return 1;
        }

        public void CreateTable()
        {
            QNameManager.QName subqueryTableName = this.database.NameManager.GetSubqueryTableName();
            this._table = new TableDerived(this.database, subqueryTableName, 2, this.queryExpression, this);
        }

        public QNameManager.SimpleName[] GetColumnNames()
        {
            return this._columnNames;
        }

        protected virtual OrderedHashSet<SubQuery> GetDependents()
        {
            OrderedHashSet<SubQuery> first = new OrderedHashSet<SubQuery>();
            OrderedHashSet<SubQuery>.AddAll(first, this.queryExpression.GetSubqueries());
            first.Remove(this);
            return first;
        }

        public virtual OrderedHashSet<SubQuery> GetExtraSubqueries()
        {
            return null;
        }

        private int GetLevelDiff(SubQuery sqa, SubQuery sqb)
        {
            if (sqa.IsNamed && sqb.IsNamed)
            {
                return (sqa.Level - sqb.Level);
            }
            if (sqa.IsNamed)
            {
                if (sqa == sqb.NamedParentSubQuery)
                {
                    return (sqb.Level - sqa.Level);
                }
                return -1;
            }
            if (!sqb.IsNamed)
            {
                return (sqb.Level - sqa.Level);
            }
            if (sqb == sqa.NamedParentSubQuery)
            {
                return (sqb.Level - sqa.Level);
            }
            return 1;
        }

        public RowSetNavigatorData GetNavigator(Session session)
        {
            RowSetNavigatorDataTable table1 = new RowSetNavigatorDataTable(session, this._table);
            table1.Reset();
            return table1;
        }

        public TableDerived GetTable()
        {
            return this._table;
        }

        public object GetValue(Session session)
        {
            return this.GetValues(session)[0];
        }

        public object[] GetValues(Session session)
        {
            IRowIterator rowIterator = this._table.GetRowIterator(session);
            if (!rowIterator.HasNext())
            {
                return new object[this._table.GetColumnCount()];
            }
            Row nextRow = rowIterator.GetNextRow();
            if (rowIterator.HasNext())
            {
                throw Error.GetError(0xc81);
            }
            return nextRow.RowData;
        }

        public bool HasUniqueNotNullRows(Session session)
        {
            RowSetNavigatorDataTable table1 = new RowSetNavigatorDataTable(session, this._table);
            table1.Reset();
            return table1.HasUniqueNotNullRows();
        }

        public bool IsCorrelated()
        {
            return this._isCorrelated;
        }

        public bool IsResolved()
        {
            return this._isResolved;
        }

        public virtual void Materialise(Session session)
        {
            if (this.IsDataExpression)
            {
                IPersistentStore subqueryRowStore = session.sessionData.GetSubqueryRowStore(this._table);
                this.DataExpression.InsertValuesIntoSubqueryTable(session, subqueryRowStore);
            }
            else
            {
                Result ins = this.queryExpression.GetResult(session, this._isExistsPredicate ? 1 : 0);
                if (this._uniqueRows)
                {
                    ((RowSetNavigatorData) ins.GetNavigator()).RemoveDuplicates();
                }
                IPersistentStore subqueryRowStore = session.sessionData.GetSubqueryRowStore(this._table);
                this._table.InsertResult(session, subqueryRowStore, ins);
                ins.GetNavigator().Close();
            }
        }

        public void MaterialiseCorrelated(Session session)
        {
            if (this._isCorrelated)
            {
                IRangeIterator[] iteratorArray = (IRangeIterator[]) session.sessionContext.RangeIterators.Clone();
                this.Materialise(session);
                session.sessionContext.RangeIterators = iteratorArray;
            }
        }

        public void PrepareTable(Session session)
        {
            if (!this._isResolved)
            {
                if (this.view == null)
                {
                    if (this._table == null)
                    {
                        QNameManager.QName subqueryTableName = this.database.NameManager.GetSubqueryTableName();
                        this._table = new TableDerived(this.database, subqueryTableName, 2, this.queryExpression, this);
                    }
                    if (this.IsDataExpression)
                    {
                        TableUtil.AddAutoColumns(this._table, this.DataExpression.NodeDataTypes);
                        TableUtil.SetTableIndexesForSubquery(this._table, this._uniqueRows || this._fullOrder, this._uniqueRows);
                    }
                    else
                    {
                        this._table.ColumnList = this.queryExpression.GetColumns();
                        this._table.ColumnCount = this.queryExpression.GetColumnCount();
                        TableUtil.SetTableIndexesForSubquery(this._table, this._uniqueRows || this._fullOrder, this._uniqueRows);
                    }
                }
                else
                {
                    TableDerived derived1 = new TableDerived(this.database, this.view.GetName(), 8, this.queryExpression, this) {
                        ColumnList = this.view.ColumnList
                    };
                    this._table = derived1;
                    this._table.ColumnCount = this._table.ColumnList.Size();
                    this._table.CreatePrimaryKey();
                }
                this._isResolved = true;
            }
        }

        public void PrepareTable(Session session, QNameManager.QName name, QNameManager.QName[] columns)
        {
            if (!this._isResolved)
            {
                if (this._table == null)
                {
                    this._table = new TableDerived(this.database, name, 2, this.queryExpression, this);
                }
                bool flag1 = (columns != null) && (this.queryExpression > null);
                if (flag1)
                {
                    this.queryExpression.GetMainSelect().SetColumnAliases(columns);
                }
                this._table.ColumnList = this.queryExpression.GetColumns();
                this._table.ColumnCount = this.queryExpression.GetColumnCount();
                if (flag1 && (columns.Length <= this._table.ColumnCount))
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        ColumnSchema schema = this._table.ColumnList.Get(i);
                        schema.ColumnName = columns[i];
                        this._table.ColumnList.Set(i, columns[i].Name, schema);
                    }
                }
                TableUtil.SetTableIndexesForSubquery(this._table, this._uniqueRows || this._fullOrder, this._uniqueRows);
                this._isResolved = true;
            }
        }

        public void SetColumnNames(QNameManager.SimpleName[] names)
        {
            this._columnNames = names;
        }

        public void SetCorrelated()
        {
            this._isCorrelated = true;
        }

        protected void SetDependentProperties(OrderedHashSet<SubQuery> set)
        {
            Iterator<SubQuery> iterator = set.GetIterator();
            bool flag = false;
            while (iterator.HasNext())
            {
                SubQuery query = iterator.Next();
                if (query.IsNamed)
                {
                    flag = true;
                }
                if (query.NamedParentSubQuery == null)
                {
                    query.NamedParentSubQuery = this;
                }
            }
            if (flag)
            {
                iterator = set.GetIterator();
                while (iterator.HasNext())
                {
                    iterator.Next().SetCorrelated();
                }
            }
        }

        public void SetDependents()
        {
            OrderedHashSet<SubQuery> dependents = this.GetDependents();
            this.SetDependentProperties(dependents);
        }

        public void SetUniqueRows()
        {
            this._uniqueRows = true;
        }

        public bool IsRecursive
        {
            get
            {
                return false;
            }
        }
    }
}

