namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class ExpressionAggregate : Expression
    {
        public ExpressionAggregate(int type, bool distinct, Expression e) : base(type)
        {
            base.nodes = new Expression[1];
            base.IsDistinctAggregate = distinct;
            base.nodes[0] = e;
        }

        public override string Describe(Session session, int blanks)
        {
            return string.Empty;
        }

        public override bool Equals(Expression other)
        {
            return ((((other != null) && (base.OpType == other.OpType)) && ((base.ExprSubType == other.ExprSubType) && (base.IsDistinctAggregate == other.IsDistinctAggregate))) && Expression.Equals(base.nodes, other.nodes));
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }
            ExpressionAggregate aggregate = other as ExpressionAggregate;
            return ((aggregate != null) && this.Equals((Expression) aggregate));
        }

        public override object GetAggregatedValue(Session session, object currValue)
        {
            if (currValue != null)
            {
                return ((SetFunction) currValue).GetValue(session);
            }
            if (base.OpType == 0x47)
            {
                return 0;
            }
            return null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string GetSql()
        {
            StringBuilder builder = new StringBuilder(0x40);
            string contextSql = Expression.GetContextSql((base.nodes.Length != 0) ? base.nodes[0] : null);
            switch (base.OpType)
            {
                case 0x47:
                    builder.Append(' ').Append("COUNT").Append('(');
                    break;

                case 0x48:
                    builder.Append(' ').Append("SUM").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 0x49:
                    builder.Append(' ').Append("MIN").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 0x4a:
                    builder.Append(' ').Append("MAX").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 0x4b:
                    builder.Append(' ').Append("AVG").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 0x4c:
                    builder.Append(' ').Append("EVERY").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 0x4d:
                    builder.Append(' ').Append("SOME").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 0x4e:
                    builder.Append(' ').Append("STDDEV_POP").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 0x4f:
                    builder.Append(' ').Append("STDDEV_SAMP").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 80:
                    builder.Append(' ').Append("VAR_POP").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                case 0x51:
                    builder.Append(' ').Append("VAR_SAMP").Append('(');
                    builder.Append(contextSql).Append(')');
                    break;

                default:
                    throw Error.RuntimeError(0xc9, "ExpressionAggregate");
            }
            return builder.ToString();
        }

        public override bool IsSelfAggregate()
        {
            return true;
        }

        public override List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            if (unresolvedSet == null)
            {
                unresolvedSet = new List<Expression>();
            }
            unresolvedSet.Add(this);
            return unresolvedSet;
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
            if (base.nodes[0].IsUnresolvedParam())
            {
                throw Error.GetError(0x15bf);
            }
            if (base.IsDistinctAggregate && base.nodes[0].DataType.IsLobType())
            {
                throw Error.GetError(0x159e);
            }
            base.DataType = SetFunction.GetType(base.OpType, base.nodes[0].DataType);
        }

        public override object UpdateAggregatingValue(Session session, object currValue)
        {
            if (currValue == null)
            {
                currValue = new SetFunction(base.OpType, base.nodes[0].DataType, base.IsDistinctAggregate);
            }
            object item = (base.nodes[0].OpType == 9) ? 1 : base.nodes[0].GetValue(session);
            ((SetFunction) currValue).Add(session, item);
            return currValue;
        }
    }
}

