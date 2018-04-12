namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using System;

    public sealed class StatementExpression : StatementDMQL
    {
        private readonly Expression _expression;
        private readonly SqlType _returnType;

        public StatementExpression(Session session, int type, Expression expression) : base(type, 0x7d7, null)
        {
            if ((type != 0x3a) && (type != 0x44d))
            {
                throw Error.RuntimeError(0xc9, "");
            }
            base.isTransactionStatement = false;
            this._expression = expression;
        }

        public StatementExpression(Session session, int type, Expression expression, SqlType returnType) : this(session, type, expression)
        {
            this._returnType = returnType;
        }

        public override void CollectTableNamesForRead(OrderedHashSet<QNameManager.QName> set)
        {
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

        public override string DescribeImpl(Session session)
        {
            return this.GetSql();
        }

        public override Result Execute(Session session)
        {
            Result result;
            try
            {
                if (base.Subqueries.Length != 0)
                {
                    base.MaterializeSubQueries(session);
                }
                result = this.GetResult(session);
            }
            catch (Exception exception1)
            {
                result = Result.NewErrorResult(exception1, null);
            }
            if (result.IsError())
            {
                result.GetException().SetStatementType(base.Group, base.type);
            }
            return result;
        }

        public override Result GetResult(Session session)
        {
            int type = base.type;
            if (type == 0x3a)
            {
                Result result = this._expression.GetResult(session);
                if (result.GetResultType() == 0x2a)
                {
                    object valueObject = result.GetValueObject();
                    valueObject = this._returnType.ConvertToType(session, valueObject, this._expression.DataType);
                    result.SetValueObject(valueObject);
                }
                return result;
            }
            if (type != 0x44d)
            {
                throw Error.RuntimeError(0xc9, "");
            }
            return this._expression.GetResult(session);
        }

        public override string GetSql()
        {
            return base.Sql;
        }

        public override SubQuery[] GetSubqueries(Session session)
        {
            OrderedHashSet<SubQuery> set = null;
            if (this._expression != null)
            {
                set = this._expression.CollectAllSubqueries(set);
            }
            if ((set == null) || (set.Size() == 0))
            {
                return SubQuery.EmptySubqueryArray;
            }
            SubQuery[] a = new SubQuery[set.Size()];
            set.ToArray(a);
            ArraySort.Sort<SubQuery>(a, 0, a.Length, a[0]);
            for (int i = 0; i < base.Subqueries.Length; i++)
            {
                a[i].PrepareTable(session);
            }
            return a;
        }

        public override void Resolve(Session session)
        {
        }
    }
}

