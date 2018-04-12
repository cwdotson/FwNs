namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using System;

    public sealed class RecursiveSubQuery : SubQuery
    {
        public const int MaxRecursions = 0x2710;
        public QueryExpression RecursiveQueryExpression;
        public QueryExpression AnchorQueryExpression;
        public int UnionType;
        public SqlType[] UnionDataTypes;
        public OrderedHashSet<SubQuery> ExtraSubQueries;
        private bool _matrializationOnProgress;

        public RecursiveSubQuery(Database database, int level, QueryExpression queryExpression, int mode) : base(database, level, queryExpression, mode)
        {
        }

        protected override OrderedHashSet<SubQuery> GetDependents()
        {
            OrderedHashSet<SubQuery> first = new OrderedHashSet<SubQuery>();
            OrderedHashSet<SubQuery>.AddAll(first, base.queryExpression.GetSubqueries());
            if (this.RecursiveQueryExpression != null)
            {
                OrderedHashSet<SubQuery>.AddAll(first, this.RecursiveQueryExpression.GetSubqueries());
            }
            first.Remove(this);
            return first;
        }

        public override OrderedHashSet<SubQuery> GetExtraSubqueries()
        {
            return this.ExtraSubQueries;
        }

        private void InsertResult(Session session, IPersistentStore store, RowSetNavigatorData nav, SqlType[] fromDataTypes, SqlType[] toDataTypes)
        {
            while (nav.HasNext())
            {
                object[] next = nav.GetNext();
                for (int i = 0; i < next.Length; i++)
                {
                    if (fromDataTypes[i].TypeCode != toDataTypes[i].TypeCode)
                    {
                        next[i] = fromDataTypes[i].ConvertToType(session, next[i], toDataTypes[i]);
                    }
                }
                base._table.InsertData(session, store, next);
            }
        }

        public override void Materialise(Session session)
        {
            if (!this._matrializationOnProgress)
            {
                if (this.RecursiveQueryExpression == null)
                {
                    base.Materialise(session);
                }
                else
                {
                    this._matrializationOnProgress = true;
                    IRangeIterator[] iteratorArray = (IRangeIterator[]) session.sessionContext.RangeIterators.Clone();
                    RowSetNavigatorData navigator = (RowSetNavigatorData) this.AnchorQueryExpression.GetResult(session, 0).GetNavigator();
                    IPersistentStore subqueryRowStore = session.sessionData.GetSubqueryRowStore(base._table);
                    base._table.ClearAllData(subqueryRowStore);
                    base._table.InsertFromNavigator(session, subqueryRowStore, navigator);
                    int num = 0;
                    SqlType[] columnTypes = this.AnchorQueryExpression.GetColumnTypes();
                    SqlType[] fromDataTypes = this.RecursiveQueryExpression.GetColumnTypes();
                    while (num++ < 0x2710)
                    {
                        RowSetNavigatorData other = (RowSetNavigatorData) this.RecursiveQueryExpression.GetResult(session, 0).GetNavigator();
                        if (other.GetSize() == 0)
                        {
                            other.Close();
                            break;
                        }
                        int size = navigator.GetSize();
                        int unionType = this.UnionType;
                        if (unionType == 1)
                        {
                            navigator.Union(other);
                        }
                        else
                        {
                            if (unionType != 2)
                            {
                                throw Error.RuntimeError(0xc9, "QueryExpression");
                            }
                            navigator.UnionAll(other);
                        }
                        base._table.ClearAllData(subqueryRowStore);
                        other.BeforeFirst();
                        this.InsertResult(session, subqueryRowStore, other, fromDataTypes, columnTypes);
                        other.Close();
                        if (size == navigator.GetSize())
                        {
                            break;
                        }
                    }
                    session.sessionContext.RangeIterators = iteratorArray;
                    if (base._uniqueRows)
                    {
                        navigator.RemoveDuplicates();
                    }
                    base._table.ClearAllData(subqueryRowStore);
                    this.InsertResult(session, subqueryRowStore, navigator, fromDataTypes, this.UnionDataTypes);
                    navigator.Close();
                    this._matrializationOnProgress = false;
                }
            }
        }

        public void PrepareTable2()
        {
            if (base._table != null)
            {
                base._table.SetQueryExpressionRecursive(this.RecursiveQueryExpression);
            }
        }
    }
}

