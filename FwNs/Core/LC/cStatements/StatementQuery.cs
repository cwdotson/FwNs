namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using System;

    public class StatementQuery : StatementDMQL
    {
        public QNameManager.QName CursorName;

        public StatementQuery(Session session, QueryExpression queryExpression) : base(0x55, 0x7d3, session.GetCurrentSchemaQName())
        {
            base.queryExpression = queryExpression;
        }

        public override void CollectTableNamesForRead(OrderedHashSet<QNameManager.QName> set)
        {
            base.queryExpression.GetBaseTableNames(set);
            for (int i = 0; i < base.Subqueries.Length; i++)
            {
                if (base.Subqueries[i].queryExpression != null)
                {
                    base.Subqueries[i].queryExpression.GetBaseTableNames(set);
                }
            }
            for (int j = 0; j < base.Routines.Length; j++)
            {
                set.AddAll(base.Routines[j].GetTableNamesForRead());
            }
        }

        public override void CollectTableNamesForWrite(OrderedHashSet<QNameManager.QName> set)
        {
        }

        public override Result GetResult(Session session)
        {
            Result result = base.queryExpression.GetResult(session, session.GetMaxRows());
            result.SetStatement(this);
            return result;
        }

        public override ResultMetaData GetResultMetaData()
        {
            int type = base.type;
            if ((type != 0x41) && (type != 0x55))
            {
                throw Error.RuntimeError(0xc9, "CompiledStatement.getResultMetaData()");
            }
            return base.queryExpression.GetMetaData();
        }
    }
}

