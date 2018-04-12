namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class ExpressionOp : Expression
    {
        public static ExpressionOp LimitOneExpression = new ExpressionOp(0x5f, new ExpressionValue(0, SqlType.SqlInteger), new ExpressionValue(1, SqlType.SqlInteger));

        public ExpressionOp(Expression e) : base(e.DataType.IsDateTimeTypeWithZone() ? 0x5b : 0x5c)
        {
            switch (e.DataType.TypeCode)
            {
                case 0x5c:
                    base.nodes = new Expression[2];
                    base.nodes[0] = e;
                    base.nodes[0].DataType = e.DataType;
                    base.DataType = DateTimeType.GetDateTimeType(0x5e, e.DataType.Scale);
                    break;

                case 0x5d:
                    base.nodes = new Expression[2];
                    base.nodes[0] = e;
                    base.nodes[0].DataType = e.DataType;
                    base.DataType = DateTimeType.GetDateTimeType(0x5f, e.DataType.Scale);
                    break;

                case 0x5e:
                {
                    base.nodes = new Expression[1];
                    ExpressionOp op1 = new ExpressionOp(0x5c, e, null) {
                        DataType = e.DataType
                    };
                    base.nodes[0] = op1;
                    base.DataType = DateTimeType.GetDateTimeType(0x5c, e.DataType.Scale);
                    break;
                }
                case 0x5f:
                {
                    base.nodes = new Expression[1];
                    ExpressionOp op2 = new ExpressionOp(0x5c, e, null) {
                        DataType = e.DataType
                    };
                    base.nodes[0] = op2;
                    base.DataType = DateTimeType.GetDateTimeType(0x5d, e.DataType.Scale);
                    break;
                }
                default:
                    throw Error.RuntimeError(0xc9, "ExpressionOp");
            }
            base.Alias = e.Alias;
        }

        public ExpressionOp(Expression e, SqlType dataType) : base(0x5b)
        {
            base.nodes = new Expression[] { e };
            base.DataType = dataType;
            base.Alias = e.Alias;
        }

        public ExpressionOp(int type, Expression left, Expression right) : base(type)
        {
            base.nodes = new Expression[] { left, right };
            int opType = base.OpType;
            if (((opType - 0x5c) > 1) && ((opType - 0x5f) > 1))
            {
                throw Error.RuntimeError(0xc9, "ExpressionOp");
            }
        }

        public override string Describe(Session session, int blanks)
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append('\n');
            for (int i = 0; i < blanks; i++)
            {
                builder.Append(' ');
            }
            switch (base.OpType)
            {
                case 1:
                    builder.Append("VALUE = ").Append(base.ValueData);
                    builder.Append(", TYPE = ").Append(base.DataType.GetNameString());
                    return builder.ToString();

                case 0x1a:
                    builder.Append("VALUELIST ");
                    for (int j = 0; j < base.nodes.Length; j++)
                    {
                        builder.Append(base.nodes[j].Describe(session, blanks + 1));
                        builder.Append(' ');
                    }
                    break;

                case 0x5b:
                    builder.Append("CAST ");
                    builder.Append(base.DataType.GetTypeDefinition());
                    builder.Append(' ');
                    break;

                case 0x5d:
                    builder.Append("CASEWHEN ");
                    break;
            }
            if (base.GetLeftNode() != null)
            {
                builder.Append(" arg_left=[");
                builder.Append(base.nodes[0].Describe(session, blanks + 1));
                builder.Append(']');
            }
            if (base.GetRightNode() != null)
            {
                builder.Append(" arg_right=[");
                builder.Append(base.nodes[1].Describe(session, blanks + 1));
                builder.Append(']');
            }
            return builder.ToString();
        }

        public static Expression GetCastExpression(Session session, Expression e, SqlType dataType)
        {
            if (e.GetExprType() == 1)
            {
                return new ExpressionValue(dataType.CastToType(session, e.GetValue(session), e.GetDataType()), dataType);
            }
            return new ExpressionOp(e, dataType);
        }

        public override string GetSql()
        {
            StringBuilder builder = new StringBuilder(0x40);
            string contextSql = Expression.GetContextSql((base.nodes.Length != 0) ? base.nodes[0] : null);
            string str2 = Expression.GetContextSql((base.nodes.Length > 1) ? base.nodes[1] : null);
            switch (base.OpType)
            {
                case 0x5b:
                    builder.Append(' ').Append("CAST").Append('(');
                    builder.Append(contextSql).Append(' ').Append("AS").Append(' ');
                    builder.Append(base.DataType.GetTypeDefinition());
                    builder.Append(')');
                    return builder.ToString();

                case 0x5c:
                    builder.Append(contextSql).Append(' ').Append("AT").Append(' ');
                    if (base.nodes[1] != null)
                    {
                        builder.Append(str2);
                    }
                    else
                    {
                        builder.Append("LOCAL").Append(' ');
                    }
                    break;

                case 0x5d:
                    builder.Append(' ').Append("CASEWHEN").Append('(');
                    builder.Append(contextSql).Append(',').Append(str2).Append(')');
                    return builder.ToString();

                case 0x5f:
                    if (contextSql != null)
                    {
                        builder.Append(' ').Append("OFFSET").Append(' ');
                        builder.Append(contextSql).Append(' ');
                    }
                    if (str2 != null)
                    {
                        builder.Append(' ').Append("FETCH").Append(' ');
                        builder.Append("FIRST");
                        builder.Append(str2).Append(' ').Append(str2).Append(' ');
                        builder.Append("ROWS").Append(' ').Append("ONLY");
                        builder.Append(' ');
                    }
                    break;

                case 0x60:
                    builder.Append(contextSql).Append(',').Append(str2);
                    return builder.ToString();

                case 1:
                    if (base.ValueData == null)
                    {
                        return "NULL";
                    }
                    if (base.DataType == null)
                    {
                        throw Error.RuntimeError(0xc9, "ExpressionOp");
                    }
                    return base.DataType.ConvertToSQLString(base.ValueData);

                default:
                    throw Error.RuntimeError(0xc9, "ExpressionOp");
            }
            return builder.ToString();
        }

        public override object GetValue(Session session)
        {
            switch (base.OpType)
            {
                case 1:
                    return base.ValueData;

                case 5:
                    return session.sessionContext.RangeIterators[base.RangePosition].GetCurrent()[base.ColumnIndex];

                case 0x5b:
                {
                    object data = base.DataType.CastToType(session, base.nodes[0].GetValue(session), base.nodes[0].DataType);
                    if (base.DataType.userTypeModifier != null)
                    {
                        Constraint[] constraints = base.DataType.userTypeModifier.GetConstraints();
                        for (int i = 0; i < constraints.Length; i++)
                        {
                            constraints[i].CheckCheckConstraint(session, null, data);
                        }
                    }
                    return data;
                }
                case 0x5c:
                {
                    object a = base.nodes[0].GetValue(session);
                    object interval = (base.nodes[1] == null) ? null : base.nodes[1].GetValue(session);
                    if (a != null)
                    {
                        if ((base.nodes[1] != null) && (interval == null))
                        {
                            return null;
                        }
                        long num3 = (base.nodes[1] == null) ? ((long) session.GetZoneSeconds()) : IntervalType.GetSeconds(interval);
                        return ((DateTimeType) base.DataType).ChangeZone(a, base.nodes[0].DataType, (int) num3, session.GetZoneSeconds());
                    }
                    return null;
                }
                case 0x5d:
                    if (!base.nodes[0].TestCondition(session))
                    {
                        return base.nodes[1].nodes[1].GetValue(session, base.DataType);
                    }
                    return base.nodes[1].nodes[0].GetValue(session, base.DataType);

                case 0x5e:
                    return base.nodes[0].GetValue(session);
            }
            throw Error.RuntimeError(0xc9, "ExpressionOp");
        }

        public override List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            if (base.OpType != 1)
            {
                if (base.OpType == 0x5d)
                {
                    acceptsSequences = false;
                }
                for (int i = 0; i < base.nodes.Length; i++)
                {
                    if (base.nodes[i] != null)
                    {
                        unresolvedSet = base.nodes[i].ResolveColumnReferences(rangeVarArray, rangeCount, unresolvedSet, acceptsSequences);
                    }
                }
            }
            return unresolvedSet;
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            switch (base.OpType)
            {
                case 0x5b:
                {
                    base.nodes[0].ResolveTypes(session, this);
                    SqlType dataType = base.nodes[0].DataType;
                    if ((dataType != null) && !base.DataType.CanConvertFrom(dataType))
                    {
                        throw Error.GetError(0x15b9);
                    }
                    if (base.nodes[0].OpType == 1)
                    {
                        Expression replacement = base.nodes[0];
                        base.SetAsConstantValue(session);
                        replacement.DataType = base.DataType;
                        replacement.ValueData = base.ValueData;
                        if (parent != null)
                        {
                            parent.ReplaceNode(this, replacement);
                            return;
                        }
                        return;
                    }
                    if (base.nodes[0].OpType == 8)
                    {
                        base.nodes[0].DataType = base.DataType;
                        if (parent != null)
                        {
                            parent.IsSelfAggregate();
                            return;
                        }
                        return;
                    }
                    base.DataType.Equals(base.nodes[0].DataType);
                    return;
                }
                case 0x5c:
                    base.nodes[0].ResolveTypes(session, this);
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (base.nodes[1] != null)
                    {
                        base.nodes[1].ResolveTypes(session, this);
                        if (base.nodes[1].DataType == null)
                        {
                            base.nodes[1].DataType = SqlType.SqlIntervalHourToMinute;
                        }
                        if (base.nodes[1].DataType.TypeCode != 0x6f)
                        {
                            throw Error.GetError(0x15bd);
                        }
                    }
                    switch (base.nodes[0].DataType.TypeCode)
                    {
                        case 0x5c:
                            base.DataType = DateTimeType.GetDateTimeType(0x5e, base.nodes[0].DataType.Scale);
                            return;

                        case 0x5d:
                            base.DataType = DateTimeType.GetDateTimeType(0x5f, base.nodes[0].DataType.Scale);
                            return;

                        case 0x5e:
                        case 0x5f:
                            base.DataType = base.nodes[0].DataType;
                            return;
                    }
                    throw Error.GetError(0x15bd);

                case 0x5d:
                    this.ResolveTypesForCaseWhen(session);
                    return;

                case 0x5f:
                    for (int i = 0; i < base.nodes.Length; i++)
                    {
                        if (base.nodes[i] != null)
                        {
                            base.nodes[i].ResolveTypes(session, this);
                            if (base.nodes[i].DataType == null)
                            {
                                base.nodes[i].DataType = SqlType.SqlInteger;
                            }
                        }
                    }
                    return;

                case 0x60:
                    return;

                case 1:
                    return;
            }
            throw Error.RuntimeError(0xc9, "ExpressionOp");
        }

        private void ResolveTypesForCaseWhen(Session session)
        {
            if (base.DataType == null)
            {
                Expression expression;
                for (expression = this; expression.OpType == 0x5d; expression = expression.nodes[1].nodes[1])
                {
                    expression.nodes[0].ResolveTypes(session, expression);
                    if (expression.nodes[0].IsUnresolvedParam())
                    {
                        expression.nodes[0].DataType = SqlType.SqlBoolean;
                    }
                    expression.nodes[1].nodes[0].ResolveTypes(session, base.nodes[1]);
                    expression.nodes[1].nodes[1].ResolveTypes(session, base.nodes[1]);
                }
                for (expression = this; expression.OpType == 0x5d; expression = expression.nodes[1].nodes[1])
                {
                    base.DataType = SqlType.GetAggregateType(expression.nodes[1].nodes[0].DataType, base.DataType);
                    base.DataType = SqlType.GetAggregateType(expression.nodes[1].nodes[1].DataType, base.DataType);
                }
                for (expression = this; expression.OpType == 0x5d; expression = expression.nodes[1].nodes[1])
                {
                    if (expression.nodes[1].nodes[0].DataType == null)
                    {
                        expression.nodes[1].nodes[0].DataType = base.DataType;
                    }
                    if (expression.nodes[1].nodes[1].DataType == null)
                    {
                        expression.nodes[1].nodes[1].DataType = base.DataType;
                    }
                    if (expression.nodes[1].DataType == null)
                    {
                        expression.nodes[1].DataType = base.DataType;
                    }
                }
                if (base.DataType == null)
                {
                    throw Error.GetError(0x15bf);
                }
            }
        }
    }
}

