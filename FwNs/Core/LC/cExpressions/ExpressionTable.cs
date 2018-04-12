namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRows;
    using System;
    using System.Text;

    public sealed class ExpressionTable : Expression
    {
        private readonly bool _ordinality;
        private bool _isTable;

        public ExpressionTable(Expression e, SubQuery sq, bool ordinality) : base(0x1a)
        {
            base.nodes = new Expression[] { e };
            base.subQuery = sq;
            this._ordinality = ordinality;
        }

        public override string Describe(Session session, int blanks)
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append('\n');
            for (int i = 0; i < blanks; i++)
            {
                builder.Append(' ');
            }
            if (this._isTable)
            {
                builder.Append("TABLE").Append(' ');
            }
            else
            {
                builder.Append("UNNEST").Append(' ');
            }
            builder.Append(base.nodes[0].Describe(session, blanks));
            return builder.ToString();
        }

        public override Result GetResult(Session session)
        {
            if (base.OpType != 0x1a)
            {
                throw Error.RuntimeError(0xc9, "ExpressionTable");
            }
            Result result1 = Result.NewResult(base.subQuery.GetNavigator(session));
            result1.MetaData = base.subQuery.queryExpression.GetMetaData();
            return result1;
        }

        public override object[] GetRowValue(Session session)
        {
            if (base.OpType != 0x1a)
            {
                throw Error.RuntimeError(0xc9, "Expression");
            }
            return base.subQuery.queryExpression.GetValues(session);
        }

        public override string GetSql()
        {
            if (this._isTable)
            {
                return "TABLE";
            }
            return "UNNEST";
        }

        public override object GetValue(Session session)
        {
            return base.ValueData;
        }

        public override object GetValue(Session session, SqlType type)
        {
            if (base.OpType != 0x1a)
            {
                throw Error.RuntimeError(0xc9, "Expression");
            }
            base.Materialise(session);
            object[] values = base.subQuery.GetValues(session);
            if (values.Length == 1)
            {
                return values[0];
            }
            return values;
        }

        public override void InsertValuesIntoSubqueryTable(Session session, IPersistentStore store)
        {
            if (!this._isTable)
            {
                object[] objArray2 = (object[]) base.nodes[0].GetValue(session);
                for (int i = 0; i < objArray2.Length; i++)
                {
                    object[] objArray3;
                    if (this._ordinality)
                    {
                        objArray3 = new object[] { objArray2[i], i };
                    }
                    else
                    {
                        objArray3 = new object[] { objArray2[i] };
                    }
                    Row newCachedObject = store.GetNewCachedObject(session, objArray3);
                    try
                    {
                        store.IndexRow(session, newCachedObject);
                    }
                    catch (CoreException)
                    {
                    }
                }
            }
            else
            {
                RowSetNavigator navigator = base.nodes[0].GetResult(session).GetNavigator();
                while (navigator.HasNext())
                {
                    object[] next = navigator.GetNext();
                    Row newCachedObject = store.GetNewCachedObject(session, next);
                    try
                    {
                        store.IndexRow(session, newCachedObject);
                        continue;
                    }
                    catch (CoreException)
                    {
                        continue;
                    }
                }
            }
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i].ResolveTypes(session, this);
                }
            }
            if (base.nodes[0].DataType.IsRowType())
            {
                this._isTable = true;
                base.NodeDataTypes = ((RowType) base.nodes[0].DataType).GetTypesArray();
                base.subQuery.PrepareTable(session);
                base.subQuery.GetTable().ColumnList = ((FunctionSQLInvoked) base.nodes[0]).routine.GetTable().ColumnList;
            }
            else
            {
                this._isTable = false;
                int num2 = this._ordinality ? 2 : 1;
                base.NodeDataTypes = new SqlType[num2];
                base.NodeDataTypes[0] = base.nodes[0].DataType.CollectionBaseType();
                if (this._ordinality)
                {
                    base.NodeDataTypes[1] = SqlType.SqlInteger;
                }
                base.subQuery.PrepareTable(session);
            }
        }
    }
}

