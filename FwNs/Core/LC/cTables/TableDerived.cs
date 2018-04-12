namespace FwNs.Core.LC.cTables
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using System;

    public class TableDerived : Table
    {
        public QueryExpression queryExpression;
        public QueryExpression queryExpressionRecursive;
        public View view;
        private readonly SubQuery _subQuery;

        public TableDerived(Database database, QNameManager.QName name, int type) : base(database, name, type)
        {
            if (((type != 1) && (type != 8)) && (type != 11))
            {
                throw Error.RuntimeError(0xc9, "Table");
            }
        }

        public TableDerived(Database database, QNameManager.QName name, int type, QueryExpression queryExpression, SubQuery subQuery) : base(database, name, type)
        {
            if ((type != 2) && ((type - 8) > 1))
            {
                throw Error.RuntimeError(0xc9, "Table");
            }
            this.queryExpression = queryExpression;
            this._subQuery = subQuery;
        }

        public TableDerived(Database database, QNameManager.QName name, int type, SqlType[] columnTypes, byte[] columnNullability, HashMappedList<string, ColumnSchema> columnList, QueryExpression queryExpression, SubQuery subQuery) : this(database, name, type, queryExpression, subQuery)
        {
            base.ColTypes = columnTypes;
            base.ColumnList = columnList;
            base.ColumnCount = columnList.Size();
            base.PrimaryKeyCols = new int[0];
            base.PrimaryKeyTypes = SqlType.EmptyArray;
            base.PrimaryKeyColsSequence = new int[0];
            base.ColDefaults = new Expression[base.ColumnCount];
            base.ColNotNull = new bool[base.ColumnCount];
            for (int i = 0; i < base.ColNotNull.Length; i++)
            {
                base.ColNotNull[i] = columnNullability[i] == 0;
            }
            base.DefaultColumnMap = new int[base.ColumnCount];
            ArrayUtil.FillSequence(base.DefaultColumnMap);
            base.BestIndexForColumn = new int[base.ColTypes.Length];
            ArrayUtil.FillArray(base.BestIndexForColumn, -1);
            base.CreatePrimaryIndex(base.PrimaryKeyCols, base.PrimaryKeyTypes, null);
        }

        public override Table GetBaseTable()
        {
            if (this.queryExpression != null)
            {
                return this.queryExpression.GetBaseTable();
            }
            return this;
        }

        public override int[] GetBaseTableColumnMap()
        {
            if (this.queryExpression != null)
            {
                return this.queryExpression.GetBaseTableColumnMap();
            }
            return null;
        }

        public override int GetId()
        {
            return 0;
        }

        public override QueryExpression GetQueryExpression()
        {
            return this.queryExpression;
        }

        public override QueryExpression GetQueryExpressionRecursive()
        {
            return this.queryExpressionRecursive;
        }

        public override SubQuery GetSubQuery()
        {
            return this._subQuery;
        }

        public override int[] GetUpdatableColumns()
        {
            return base.DefaultColumnMap;
        }

        public override bool IsInsertable()
        {
            return ((this.queryExpression != null) && this.queryExpression.IsInsertable);
        }

        public override bool IsUpdatable()
        {
            return ((this.queryExpression != null) && this.queryExpression.IsUpdatable);
        }

        public override bool IsWritable()
        {
            return true;
        }

        public void SetQueryExpression(QueryExpression qe)
        {
            this.queryExpression = qe;
        }

        public void SetQueryExpressionRecursive(QueryExpression qe)
        {
            this.queryExpressionRecursive = qe;
        }
    }
}

