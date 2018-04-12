namespace FwNs.Core.LC.cTables
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Text;

    public sealed class View : TableDerived
    {
        public SubQuery ViewSubQuery;
        public string statement;
        public QNameManager.QName[] ColumnNames;
        public SubQuery[] ViewSubqueries;
        private OrderedHashSet<QNameManager.QName> _schemaObjectNames;
        private readonly int _check;
        private Table _baseTable;
        private bool _isTriggerInsertable;
        private bool _isTriggerUpdatable;
        private bool _isTriggerDeletable;

        public View(Database db, QNameManager.QName name, QNameManager.QName[] columnNames, int check) : base(db, name, 8)
        {
            this.ColumnNames = columnNames;
            this._check = check;
        }

        public override void AddTrigger(TriggerDef td, QNameManager.QName otherName)
        {
            int operationType = td.OperationType;
            switch (operationType)
            {
                case 0x13:
                    if (this._isTriggerDeletable)
                    {
                        throw Error.GetError(0x15a9);
                    }
                    this._isTriggerDeletable = true;
                    break;

                case 50:
                    if (this._isTriggerInsertable)
                    {
                        throw Error.GetError(0x15a9);
                    }
                    this._isTriggerInsertable = true;
                    break;

                default:
                    if (operationType != 0x52)
                    {
                        throw Error.RuntimeError(0xc9, "View");
                    }
                    if (this._isTriggerUpdatable)
                    {
                        throw Error.GetError(0x15a9);
                    }
                    this._isTriggerUpdatable = true;
                    break;
            }
            base.AddTrigger(td, otherName);
        }

        public override void Compile(Session session, ISchemaObject parentObject)
        {
            using (Scanner scanner = new Scanner(this.statement))
            {
                ParserDQL rdql = new ParserDQL(session, scanner);
                rdql.Read();
                this.ViewSubQuery = rdql.XreadViewSubquery(this);
                base.queryExpression = this.ViewSubQuery.queryExpression;
                if (base.GetColumnCount() == 0)
                {
                    if (this.ColumnNames == null)
                    {
                        this.ColumnNames = this.ViewSubQuery.queryExpression.GetResultColumnNames();
                    }
                    if (this.ColumnNames.Length != this.ViewSubQuery.queryExpression.GetColumnCount())
                    {
                        throw Error.GetError(0x15d9, this.GetName().StatementName);
                    }
                    TableUtil.SetColumnsInSchemaTable(this, this.ColumnNames, base.queryExpression.GetColumnTypes(), base.queryExpression.GetColumnNullability());
                }
                OrderedHashSet<SubQuery> set = OrderedHashSet<SubQuery>.AddAll(OrderedHashSet<SubQuery>.Add(base.queryExpression.GetSubqueries(), this.ViewSubQuery), this.ViewSubQuery.GetExtraSubqueries());
                this.ViewSubqueries = new SubQuery[set.Size()];
                set.ToArray(this.ViewSubqueries);
                ArraySort.Sort<SubQuery>(this.ViewSubqueries, 0, this.ViewSubqueries.Length, this.ViewSubqueries[0]);
                foreach (SubQuery query in this.ViewSubqueries)
                {
                    if (query.ParentView == null)
                    {
                        query.ParentView = this;
                    }
                    query.PrepareTable(session);
                }
                this.ViewSubQuery.GetTable().view = this;
                this.ViewSubQuery.GetTable().ColumnList = base.ColumnList;
                this._schemaObjectNames = rdql.compileContext.GetSchemaObjectNames();
                this._baseTable = base.queryExpression.GetBaseTable();
            }
            if (this._baseTable != null)
            {
                switch (this._check)
                {
                    case 0:
                    case 2:
                        return;

                    case 1:
                        base.queryExpression.GetCheckCondition();
                        return;
                }
                throw Error.RuntimeError(0xc9, "View");
            }
        }

        public override long GetChangeTimestamp()
        {
            return base.ChangeTimestamp;
        }

        public int GetCheckOption()
        {
            return this._check;
        }

        public override OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public override OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return this._schemaObjectNames;
        }

        public override int GetSchemaObjectType()
        {
            return 4;
        }

        public override string GetSql()
        {
            StringBuilder builder = new StringBuilder(0x80);
            builder.Append("CREATE").Append(' ').Append("VIEW");
            builder.Append(' ');
            builder.Append(this.GetName().GetSchemaQualifiedStatementName()).Append(' ');
            builder.Append('(');
            int columnCount = base.GetColumnCount();
            for (int i = 0; i < columnCount; i++)
            {
                builder.Append(base.GetColumn(i).GetName().StatementName);
                if (i < (columnCount - 1))
                {
                    builder.Append(',');
                }
            }
            builder.Append(')').Append(' ').Append("AS").Append(' ');
            builder.Append(this.GetStatement());
            return builder.ToString();
        }

        public string GetStatement()
        {
            return this.statement;
        }

        public SubQuery[] GetSubqueries()
        {
            return this.ViewSubqueries;
        }

        public Table GetSubqueryTable()
        {
            return this.ViewSubQuery.GetTable();
        }

        public override int[] GetUpdatableColumns()
        {
            return base.queryExpression.GetBaseTableColumnMap();
        }

        public override bool IsTriggerDeletable()
        {
            return this._isTriggerDeletable;
        }

        public override bool IsTriggerInsertable()
        {
            return this._isTriggerInsertable;
        }

        public override bool IsTriggerUpdatable()
        {
            return this._isTriggerUpdatable;
        }

        public override void RemoveTrigger(TriggerDef td)
        {
            int operationType = td.OperationType;
            switch (operationType)
            {
                case 0x13:
                    this._isTriggerDeletable = false;
                    break;

                case 50:
                    this._isTriggerInsertable = false;
                    break;

                default:
                    if (operationType != 0x52)
                    {
                        throw Error.RuntimeError(0xc9, "View");
                    }
                    this._isTriggerInsertable = false;
                    break;
            }
            base.RemoveTrigger(td);
        }

        public override void SetDataReadOnly(bool value)
        {
            throw Error.GetError(0xfa0);
        }

        public void SetStatement(string sql)
        {
            this.statement = sql;
        }
    }
}

