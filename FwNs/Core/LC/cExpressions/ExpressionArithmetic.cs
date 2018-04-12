namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class ExpressionArithmetic : Expression
    {
        public ExpressionArithmetic(int type, Expression e) : base(type)
        {
            base.nodes = new Expression[] { e };
            if (base.OpType != 0x1f)
            {
                throw Error.RuntimeError(0xc9, "Expression");
            }
        }

        public ExpressionArithmetic(int type, Expression left, Expression right) : base(type)
        {
            base.nodes = new Expression[] { left, right };
            int opType = base.OpType;
            if (((opType - 0x20) > 4) && (opType != 0x65))
            {
                throw Error.RuntimeError(0xc9, "Expression");
            }
        }

        public override string Describe(Session session, int blanks)
        {
            return string.Empty;
        }

        public override string GetSql()
        {
            StringBuilder builder = new StringBuilder(0x40);
            if (base.OpType == 1)
            {
                if (base.ValueData == null)
                {
                    return "NULL";
                }
                if (base.DataType == null)
                {
                    throw Error.RuntimeError(0xc9, "Expression");
                }
                return base.DataType.ConvertToSQLString(base.ValueData);
            }
            string contextSql = Expression.GetContextSql((base.nodes.Length != 0) ? base.nodes[0] : null);
            string str3 = Expression.GetContextSql((base.nodes.Length > 1) ? base.nodes[1] : null);
            int opType = base.OpType;
            switch (opType)
            {
                case 0x1f:
                    builder.Append('-').Append(contextSql);
                    break;

                case 0x20:
                    builder.Append(contextSql).Append('+').Append(str3);
                    break;

                case 0x21:
                    builder.Append(contextSql).Append('-').Append(str3);
                    break;

                case 0x22:
                    builder.Append(contextSql).Append('*').Append(str3);
                    break;

                case 0x23:
                    builder.Append(contextSql).Append('/').Append(str3);
                    break;

                case 0x24:
                    builder.Append(contextSql).Append("||").Append(str3);
                    break;

                case 0x5b:
                    builder.Append(' ').Append("CAST").Append('(');
                    builder.Append(contextSql).Append(' ').Append("AS").Append(' ');
                    builder.Append(base.DataType.GetTypeDefinition());
                    builder.Append(')');
                    break;

                default:
                    if (opType != 0x65)
                    {
                        throw Error.RuntimeError(0xc9, "Expression");
                    }
                    builder.Append(contextSql).Append('%').Append(str3);
                    break;
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

                case 0x1f:
                    return base.DataType.Negate(base.nodes[0].GetValue(session, base.nodes[0].DataType));
            }
            object a = base.nodes[0].GetValue(session);
            object b = base.nodes[1].GetValue(session);
            int opType = base.OpType;
            switch (opType)
            {
                case 0x20:
                    return base.DataType.Add(a, b, base.nodes[0].DataType, base.nodes[1].DataType);

                case 0x21:
                    return base.DataType.Subtract(a, b, base.nodes[0].DataType, base.nodes[1].DataType);

                case 0x22:
                    return base.DataType.Multiply(a, b, base.nodes[0].DataType, base.nodes[1].DataType);

                case 0x23:
                    return base.DataType.Divide(a, b, base.nodes[0].DataType, base.nodes[1].DataType);

                case 0x24:
                    return base.DataType.Concat(session, a, b);
            }
            if (opType != 0x65)
            {
                throw Error.RuntimeError(0xc9, "Expression");
            }
            return base.DataType.Mod(a, b, base.nodes[0].DataType, base.nodes[1].DataType);
        }

        public override List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            if (base.OpType != 1)
            {
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
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i].ResolveTypes(session, this);
                }
            }
            int opType = base.OpType;
            switch (opType)
            {
                case 0x1f:
                    if (base.nodes[0].IsUnresolvedParam() || (base.nodes[0].DataType == null))
                    {
                        throw Error.GetError(0x15bf);
                    }
                    base.DataType = base.nodes[0].DataType;
                    if (!base.DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if (base.nodes[0].OpType == 1)
                    {
                        base.SetAsConstantValue(session);
                        return;
                    }
                    return;

                case 0x20:
                    if (((base.nodes[0].DataType == null) || !base.nodes[0].DataType.IsCharacterType()) && ((base.nodes[1].DataType == null) || !base.nodes[1].DataType.IsCharacterType()))
                    {
                        break;
                    }
                    base.OpType = 0x24;
                    this.ResolveTypesForConcat(session, parent);
                    return;

                case 0x21:
                case 0x22:
                case 0x23:
                    break;

                case 0x24:
                    this.ResolveTypesForConcat(session, parent);
                    return;

                case 1:
                    return;

                default:
                    if (opType != 0x65)
                    {
                        throw Error.RuntimeError(0xc9, "Expression");
                    }
                    break;
            }
            this.ResolveTypesForArithmetic(session);
        }

        public void ResolveTypesForArithmetic(Session session)
        {
            if (base.nodes[0].IsUnresolvedParam() && base.nodes[1].IsUnresolvedParam())
            {
                throw Error.GetError(0x15bf);
            }
            if (base.nodes[0].IsUnresolvedParam())
            {
                base.nodes[0].DataType = base.nodes[1].DataType;
            }
            else if (base.nodes[1].IsUnresolvedParam())
            {
                base.nodes[1].DataType = base.nodes[0].DataType;
            }
            if ((base.nodes[0].DataType == null) || (base.nodes[1].DataType == null))
            {
                throw Error.GetError(0x15bf);
            }
            if (base.nodes[0].DataType.IsDateTimeType() && base.nodes[1].DataType.IsDateTimeType())
            {
                if (base.DataType != null)
                {
                    if (!base.DataType.IsIntervalType() || (base.nodes[0].DataType.TypeCode != base.nodes[1].DataType.TypeCode))
                    {
                        throw Error.GetError(0x15ba);
                    }
                }
                else
                {
                    if (((base.nodes[0].DataType.TypeCode != 0x5b) || (base.nodes[1].DataType.TypeCode != 0x5b)) || (base.OpType != 0x21))
                    {
                        throw Error.GetError(0x15be);
                    }
                    base.DataType = SqlType.SqlDouble;
                }
            }
            else
            {
                base.DataType = base.nodes[0].DataType.GetCombinedType(base.nodes[1].DataType, base.OpType);
                if (base.DataType.IsDateTimeType() && base.nodes[0].DataType.IsIntervalType())
                {
                    if (base.OpType != 0x20)
                    {
                        throw Error.GetError(0x15bd);
                    }
                    Expression expression = base.nodes[0];
                    base.nodes[0] = base.nodes[1];
                    base.nodes[1] = expression;
                }
            }
            if ((base.nodes[0].OpType == 1) && (base.nodes[1].OpType == 1))
            {
                base.SetAsConstantValue(session);
            }
        }

        private void ResolveTypesForConcat(Session session, Expression parent)
        {
            if (base.DataType == null)
            {
                if (base.nodes[0].IsUnresolvedParam())
                {
                    base.nodes[0].DataType = base.nodes[1].DataType;
                }
                else if (base.nodes[1].IsUnresolvedParam())
                {
                    base.nodes[1].DataType = base.nodes[0].DataType;
                }
                if ((base.nodes[0].DataType == null) || (base.nodes[1].DataType == null))
                {
                    throw Error.GetError(0x15bf);
                }
                if (base.nodes[0].DataType.IsBinaryType() ^ base.nodes[1].DataType.IsBinaryType())
                {
                    throw Error.GetError(0x15bd);
                }
                if (base.nodes[0].DataType.IsArrayType())
                {
                    Expression replacement = base.nodes[1];
                    if (replacement.OpType == 0x69)
                    {
                        if (parent == null)
                        {
                            throw Error.GetError(0x15bb);
                        }
                        base.nodes[1] = replacement.GetLeftNode();
                        replacement.nodes[0] = this;
                        parent.ReplaceNode(this, replacement);
                    }
                }
                if (base.nodes[0].DataType.IsArrayType() ^ base.nodes[1].DataType.IsArrayType())
                {
                    throw Error.GetError(0x15bb);
                }
                if (base.nodes[0].DataType.IsCharacterType() && !base.nodes[1].DataType.IsCharacterType())
                {
                    SqlType characterType = CharacterType.GetCharacterType(12, (long) base.nodes[1].DataType.DisplaySize());
                    base.nodes[1] = ExpressionOp.GetCastExpression(session, base.nodes[1], characterType);
                }
                if (base.nodes[1].DataType.IsCharacterType() && !base.nodes[0].DataType.IsCharacterType())
                {
                    SqlType characterType = CharacterType.GetCharacterType(12, (long) base.nodes[0].DataType.DisplaySize());
                    base.nodes[0] = ExpressionOp.GetCastExpression(session, base.nodes[0], characterType);
                }
                base.DataType = base.nodes[0].DataType.GetCombinedType(base.nodes[1].DataType, 0x24);
                if ((base.nodes[0].OpType == 1) && (base.nodes[1].OpType == 1))
                {
                    base.SetAsConstantValue(session);
                }
            }
        }
    }
}

