namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Text;

    public class ExpressionLogical : Expression
    {
        public bool IsQuantified;
        public bool NoOptimisation;
        private bool _hasRowArgument;

        public ExpressionLogical(ColumnSchema column) : base(0x30)
        {
            base.nodes = new Expression[1];
            base.DataType = SqlType.SqlBoolean;
            Expression e = new ExpressionColumn(column);
            e = new ExpressionLogical(0x2f, e);
            base.nodes[0] = e;
        }

        public ExpressionLogical(bool b) : base(1)
        {
            base.DataType = SqlType.SqlBoolean;
            base.ValueData = b;
        }

        public ExpressionLogical(int type) : base(type)
        {
            base.DataType = SqlType.SqlBoolean;
        }

        public ExpressionLogical(Expression left, Expression right) : base(0x29)
        {
            base.nodes = new Expression[] { left, right };
            if ((left.OpType == 2) && (right.OpType == 2))
            {
                base.IsColumnEqual = true;
            }
            base.DataType = SqlType.SqlBoolean;
        }

        public ExpressionLogical(int type, Expression e) : base(type)
        {
            base.nodes = new Expression[] { e };
            int opType = base.OpType;
            if ((((opType - 0x2f) > 1) && (opType != 0x37)) && (opType != 0x39))
            {
                throw Error.RuntimeError(0xc9, "ExpressionLogical");
            }
            base.DataType = SqlType.SqlBoolean;
        }

        public ExpressionLogical(int type, Expression left, Expression right) : base(type)
        {
            base.nodes = new Expression[] { left, right };
            switch (base.OpType)
            {
                case 0x29:
                    if ((left.OpType == 2) && (right.OpType == 2))
                    {
                        base.IsColumnEqual = true;
                    }
                    break;

                case 0x2a:
                case 0x2b:
                case 0x2c:
                case 0x2d:
                case 0x2e:
                case 0x31:
                case 50:
                case 0x36:
                case 0x38:
                case 0x3a:
                case 0x3b:
                case 60:
                case 0x3d:
                case 0x3e:
                case 0x3f:
                case 0x40:
                    break;

                default:
                    throw Error.RuntimeError(0xc9, "ExpressionLogical");
            }
            base.DataType = SqlType.SqlBoolean;
        }

        public ExpressionLogical(RangeVariable leftRangeVar, int colIndexLeft, RangeVariable rightRangeVar, int colIndexRight) : base(0x29)
        {
            ExpressionColumn column = new ExpressionColumn(leftRangeVar, colIndexLeft);
            ExpressionColumn column2 = new ExpressionColumn(rightRangeVar, colIndexRight);
            base.nodes = new Expression[2];
            base.DataType = SqlType.SqlBoolean;
            base.nodes[0] = column;
            base.nodes[1] = column2;
        }

        public void AddLeftColumnsForAllAny(RangeVariable range, OrderedIntHashSet set)
        {
            if (base.nodes.Length != 0)
            {
                for (int i = 0; i < base.nodes[0].nodes.Length; i++)
                {
                    int columnIndex = base.nodes[0].nodes[i].GetColumnIndex();
                    if ((columnIndex < 0) || (base.nodes[0].nodes[i].GetRangeVariable() != range))
                    {
                        set.Clear();
                        return;
                    }
                    set.Add(columnIndex);
                }
            }
        }

        public static Expression AndExpressions(Expression e1, Expression e2)
        {
            if (e1 == null)
            {
                return e2;
            }
            if (e2 == null)
            {
                return e1;
            }
            if (Expression.ExprFalse.Equals(e1) || Expression.ExprFalse.Equals(e2))
            {
                return new ExpressionLogical(false);
            }
            return new ExpressionLogical(0x31, e1, e2);
        }

        private void CheckRowComparison()
        {
            if ((base.OpType != 0x29) && (base.OpType != 0x2e))
            {
                for (int i = 0; i < base.nodes[0].NodeDataTypes.Length; i++)
                {
                    SqlType type = base.nodes[0].NodeDataTypes[i];
                    SqlType type2 = base.nodes[1].NodeDataTypes[i];
                    if ((type.IsArrayType() || type.IsLobType()) || type2.IsLobType())
                    {
                        throw Error.GetError(0x159e);
                    }
                }
            }
        }

        private object CompareValues(Session session, object left, object right)
        {
            if (((left == null) || (right == null)) && (base.OpType != 0x3a))
            {
                return null;
            }
            return this.CompareValuesCore(session, left, right);
        }

        private object CompareValues(Session session, object[] left, object[] right)
        {
            int num = 0;
            bool flag = false;
            if ((left == null) || (right == null))
            {
                return null;
            }
            int index = 0;
            while (index < base.nodes[0].nodes.Length)
            {
                if (left[index] != null)
                {
                    goto Label_0037;
                }
                if ((base.OpType != 60) && (base.OpType != 0x3f))
                {
                    flag = true;
                    goto Label_0037;
                }
            Label_0031:
                index++;
                continue;
            Label_0037:
                if (right[index] == null)
                {
                    flag = true;
                }
                object a = left[index];
                object b = right[index];
                num = base.nodes[0].NodeDataTypes[index].Compare(session, a, b, base.nodes[1].NodeDataTypes[index], false);
                if (num != 0)
                {
                    break;
                }
                goto Label_0031;
            }
            switch (base.OpType)
            {
                case 0x29:
                case 0x36:
                    if (!flag)
                    {
                        return (num == 0);
                    }
                    return null;

                case 0x2a:
                    if (!flag)
                    {
                        return (num >= 0);
                    }
                    return null;

                case 0x2b:
                    if (!flag)
                    {
                        return (num > 0);
                    }
                    return null;

                case 0x2c:
                    if (!flag)
                    {
                        return (num < 0);
                    }
                    return null;

                case 0x2d:
                    if (!flag)
                    {
                        return (num <= 0);
                    }
                    return null;

                case 0x2e:
                    if (!flag)
                    {
                        return (num > 0);
                    }
                    return null;

                case 0x3a:
                case 0x3b:
                case 60:
                case 0x3d:
                case 0x3e:
                case 0x3f:
                case 0x40:
                    return (num == 0);
            }
            throw Error.RuntimeError(0xc9, "ExpressionLogical");
        }

        private bool CompareValuesBool(Session session, object left, object right)
        {
            if (((left == null) || (right == null)) && (base.OpType != 0x3a))
            {
                return false;
            }
            return this.CompareValuesCore(session, left, right);
        }

        private bool CompareValuesCore(Session session, object left, object right)
        {
            int opType = base.OpType;
            switch (opType)
            {
                case 0x29:
                    return (base.nodes[0].DataType.Compare(session, left, right, base.nodes[1].DataType, true) == 0);

                case 0x2a:
                    return (base.nodes[0].DataType.Compare(session, left, right, base.nodes[1].DataType, false) >= 0);

                case 0x2b:
                    return (base.nodes[0].DataType.Compare(session, left, right, base.nodes[1].DataType, false) > 0);

                case 0x2c:
                    return (base.nodes[0].DataType.Compare(session, left, right, base.nodes[1].DataType, false) < 0);

                case 0x2d:
                    return (base.nodes[0].DataType.Compare(session, left, right, base.nodes[1].DataType, false) <= 0);

                case 0x2e:
                    return (base.nodes[0].DataType.Compare(session, left, right, base.nodes[1].DataType, true) > 0);
            }
            if (opType != 0x3a)
            {
                throw Error.RuntimeError(0xc9, "ExpressionLogical");
            }
            return (base.nodes[0].DataType.Compare(session, left, right, base.nodes[1].DataType, false) == 0);
        }

        private static bool ConvertDateTimeLiteral(Session session, Expression a, Expression b)
        {
            if (!a.DataType.IsDateTimeType())
            {
                if (!b.DataType.IsDateTimeType())
                {
                    return false;
                }
                a = b;
                b = a;
            }
            if (!a.DataType.IsDateTimeTypeWithZone() && ((b.OpType == 1) && b.DataType.IsCharacterType()))
            {
                b.ValueData = a.DataType.CastToType(session, b.ValueData, b.DataType);
                b.DataType = a.DataType;
                return true;
            }
            return false;
        }

        public bool ConvertToSmaller()
        {
            int opType = base.OpType;
            if ((opType - 0x2a) > 1)
            {
                if ((opType - 0x2c) <= 1)
                {
                    return true;
                }
            }
            else
            {
                this.SwapCondition();
                return true;
            }
            return false;
        }

        public override string Describe(Session session, int blanks)
        {
            return string.Empty;
        }

        private object GetAllAnyValue(Session session, object[] data, SubQuery subquery)
        {
            Table table = subquery.GetTable();
            bool flag = table.IsEmpty(session);
            Index fullIndex = table.GetFullIndex();
            IPersistentStore rowStore = session.sessionData.GetRowStore(table);
            Row nextRow = fullIndex.LastRow(session, rowStore).GetNextRow();
            switch (base.ExprSubType)
            {
                case 0x33:
                {
                    if (flag)
                    {
                        return true;
                    }
                    if (Expression.CountNulls(data) == data.Length)
                    {
                        return null;
                    }
                    object[] rowData = fullIndex.FirstRow(session, rowStore).GetNextRow().RowData;
                    if (Expression.CountNulls(rowData) == data.Length)
                    {
                        return null;
                    }
                    Expression.ConvertToType(session, data, base.nodes[0].NodeDataTypes, base.nodes[1].NodeDataTypes);
                    IRowIterator iterator = fullIndex.FindFirstRow(session, rowStore, data);
                    if (base.OpType != 0x29)
                    {
                        if (base.OpType == 0x2e)
                        {
                            return !iterator.HasNext();
                        }
                        object[] right = nextRow.RowData;
                        object obj2 = this.CompareValues(session, data, rowData);
                        object obj3 = this.CompareValues(session, data, right);
                        switch (base.OpType)
                        {
                            case 0x2a:
                                return obj3;

                            case 0x2b:
                                return obj3;

                            case 0x2c:
                                return obj2;

                            case 0x2d:
                                return obj2;
                        }
                        break;
                    }
                    return (iterator.HasNext() && (subquery.GetTable().GetRowCount(rowStore) == 1));
                }
                case 0x34:
                {
                    if (flag)
                    {
                        return false;
                    }
                    if (Expression.CountNulls(data) == data.Length)
                    {
                        return null;
                    }
                    object[] rowData = nextRow.RowData;
                    if (Expression.CountNulls(rowData) == data.Length)
                    {
                        return null;
                    }
                    Expression.ConvertToType(session, data, base.nodes[0].NodeDataTypes, base.nodes[1].NodeDataTypes);
                    if (base.OpType == 0x29)
                    {
                        return fullIndex.FindFirstRow(session, rowStore, data).HasNext();
                    }
                    object[] right = fullIndex.FindFirstRowNotNull(session, rowStore).GetNextRow().RowData;
                    object obj4 = this.CompareValues(session, data, right);
                    object obj5 = this.CompareValues(session, data, rowData);
                    switch (base.OpType)
                    {
                        case 0x2a:
                            return obj4;

                        case 0x2b:
                            return obj4;

                        case 0x2c:
                            return obj5;

                        case 0x2d:
                            return obj5;

                        case 0x2e:
                        {
                            bool flag2 = true;
                            return (flag2.Equals(obj4) ? ((object) 1) : ((object) (flag2 = true).Equals(obj5)));
                        }
                    }
                    break;
                }
            }
            return null;
        }

        public override Expression GetIndexableExpression(RangeVariable rangeVar)
        {
            switch (base.OpType)
            {
                case 0x29:
                    if (base.ExprSubType != 0x34)
                    {
                        break;
                    }
                    if (!base.nodes[1].IsCorrelated())
                    {
                        for (int i = 0; i < base.nodes[0].nodes.Length; i++)
                        {
                            if ((base.nodes[0].nodes[i].OpType == 2) && base.nodes[0].nodes[i].IsIndexable(rangeVar))
                            {
                                return this;
                            }
                        }
                        return null;
                    }
                    return null;

                case 0x2a:
                case 0x2b:
                case 0x2c:
                case 0x2d:
                    break;

                case 0x2f:
                    if ((base.nodes[0].OpType == 2) && base.nodes[0].IsIndexable(rangeVar))
                    {
                        return this;
                    }
                    return null;

                case 0x30:
                    if (((base.nodes[0].OpType == 0x2f) && (base.nodes[0].nodes[0].OpType == 2)) && base.nodes[0].nodes[0].IsIndexable(rangeVar))
                    {
                        return this;
                    }
                    return null;

                case 50:
                    if (!this.IsIndexable(rangeVar))
                    {
                        return null;
                    }
                    return this;

                default:
                    return null;
            }
            if (base.ExprSubType == 0)
            {
                if ((base.nodes[0].OpType == 2) && base.nodes[0].IsIndexable(rangeVar))
                {
                    if (base.nodes[1].HasReference(rangeVar))
                    {
                        return null;
                    }
                    return this;
                }
                if (!base.nodes[0].HasReference(rangeVar) && ((base.nodes[1].OpType == 2) && base.nodes[1].IsIndexable(rangeVar)))
                {
                    this.SwapCondition();
                    return this;
                }
            }
            return null;
        }

        public override string GetSql()
        {
            StringBuilder builder = new StringBuilder(0x40);
            if (base.OpType == 1)
            {
                return base.GetSql();
            }
            string contextSql = Expression.GetContextSql(base.nodes[0]);
            string str3 = Expression.GetContextSql((base.nodes.Length > 1) ? base.nodes[1] : null);
            switch (base.OpType)
            {
                case 0x29:
                    builder.Append(contextSql).Append('=').Append(str3);
                    return builder.ToString();

                case 0x2a:
                    builder.Append(contextSql).Append(">=").Append(str3);
                    return builder.ToString();

                case 0x2b:
                    builder.Append(contextSql).Append('>').Append(str3);
                    return builder.ToString();

                case 0x2c:
                    builder.Append(contextSql).Append('<').Append(str3);
                    return builder.ToString();

                case 0x2d:
                    builder.Append(contextSql).Append("<=").Append(str3);
                    return builder.ToString();

                case 0x2e:
                    if (!"NULL".Equals(str3))
                    {
                        builder.Append(contextSql).Append("!=").Append(str3);
                        break;
                    }
                    builder.Append(contextSql).Append(" IS NOT ").Append(str3);
                    break;

                case 0x2f:
                    builder.Append(contextSql).Append(' ').Append("IS").Append(' ').Append("NULL");
                    return builder.ToString();

                case 0x30:
                    if (base.nodes[0].OpType != 0x2f)
                    {
                        if (base.nodes[0].OpType == 0x3a)
                        {
                            builder.Append(Expression.GetContextSql(base.nodes[0].nodes[0])).Append(' ').Append("IS").Append(' ').Append("DISTINCT").Append(' ').Append("FROM").Append(' ');
                            if (base.nodes[0].nodes.Length > 1)
                            {
                                builder.Append(Expression.GetContextSql(base.nodes[0].nodes[1]));
                            }
                            else
                            {
                                builder.Append(Expression.GetContextSql(base.nodes[1]));
                            }
                            return builder.ToString();
                        }
                        builder.Append("NOT").Append(' ').Append(contextSql);
                        return builder.ToString();
                    }
                    builder.Append(Expression.GetContextSql(base.nodes[0].nodes[0])).Append(' ').Append("IS").Append(' ').Append("NOT").Append(' ').Append("NULL");
                    return builder.ToString();

                case 0x31:
                    builder.Append(contextSql).Append(' ').Append("AND").Append(' ').Append(str3);
                    return builder.ToString();

                case 50:
                    builder.Append(contextSql).Append(' ').Append("OR").Append(' ').Append(str3);
                    return builder.ToString();

                case 0x36:
                    builder.Append(contextSql).Append(' ').Append("IN").Append(' ').Append(str3);
                    return builder.ToString();

                case 0x37:
                    builder.Append(' ').Append("EXISTS").Append(' ');
                    goto Label_0609;

                case 0x39:
                    builder.Append(' ').Append("UNIQUE").Append(' ');
                    goto Label_0609;

                case 0x3a:
                    builder.Append("NOT").Append(' ').Append(Expression.GetContextSql(base.nodes[0].nodes[0])).Append(' ').Append("IS").Append(' ').Append("DISTINCT").Append(' ').Append("FROM").Append(' ');
                    if (base.nodes[0].nodes.Length <= 1)
                    {
                        builder.Append(Expression.GetContextSql(base.nodes[1]));
                    }
                    else
                    {
                        builder.Append(Expression.GetContextSql(base.nodes[0].nodes[1]));
                    }
                    return builder.ToString();

                case 0x3b:
                    builder.Append(contextSql).Append(' ').Append("MATCH").Append(' ').Append(str3);
                    return builder.ToString();

                case 60:
                    builder.Append(contextSql).Append(' ').Append("MATCH").Append(' ').Append(0x1d5).Append(str3);
                    return builder.ToString();

                case 0x3d:
                    builder.Append(contextSql).Append(' ').Append("MATCH").Append(' ').Append(0x73).Append(str3);
                    return builder.ToString();

                case 0x3e:
                    builder.Append(contextSql).Append(' ').Append("MATCH").Append(' ').Append(0x129).Append(str3);
                    return builder.ToString();

                case 0x3f:
                    builder.Append(contextSql).Append(' ').Append("MATCH").Append(' ').Append(0x129).Append(' ').Append(0x1d5).Append(str3);
                    return builder.ToString();

                case 0x40:
                    builder.Append(contextSql).Append(' ').Append("MATCH").Append(' ').Append(0x129).Append(' ').Append(0x73).Append(str3);
                    return builder.ToString();

                default:
                    throw Error.RuntimeError(0xc9, "ExpressionLogical");
            }
            return builder.ToString();
        Label_0609:
            return builder.ToString();
        }

        public override object GetValue(Session session)
        {
            int opType = base.OpType;
            if (opType <= 5)
            {
                switch (opType)
                {
                    case 1:
                        return base.ValueData;

                    case 5:
                        return session.sessionContext.RangeIterators[base.RangePosition].GetCurrent()[base.ColumnIndex];
                }
            }
            else
            {
                switch (opType)
                {
                    case 0x1f:
                        return base.DataType.Negate(base.nodes[0].GetValue(session, base.nodes[0].DataType));

                    case 0x20:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 40:
                    case 0x33:
                    case 0x34:
                    case 0x35:
                        break;

                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x3a:
                        if ((base.ExprSubType != 0x34) && (base.ExprSubType != 0x33))
                        {
                            object left = base.nodes[0].GetValue(session);
                            object right = base.nodes[1].GetValue(session);
                            if (this._hasRowArgument)
                            {
                                object[] objArray = left as object[];
                                object[] objArray2 = right as object[];
                                if (objArray != null)
                                {
                                    if (objArray2 == null)
                                    {
                                        throw Error.RuntimeError(0xc9, "ExpressionLogical");
                                    }
                                    return this.CompareValues(session, objArray, objArray2);
                                }
                                if (objArray2 != null)
                                {
                                    return this.CompareValues(session, left, objArray2[0]);
                                }
                            }
                            return this.CompareValues(session, left, right);
                        }
                        return this.TestAllAnyCondition(session, base.nodes[0].GetRowValue(session));

                    case 0x2f:
                        return (base.nodes[0].GetValue(session) == null);

                    case 0x30:
                    {
                        object obj4 = base.nodes[0].GetValue(session);
                        if (obj4 != null)
                        {
                            return !((bool) obj4);
                        }
                        return null;
                    }
                    case 0x31:
                    {
                        object obj5 = base.nodes[0].GetValue(session);
                        if (obj5 != null)
                        {
                            if (!((bool) obj5))
                            {
                                return false;
                            }
                            return base.nodes[1].GetValue(session);
                        }
                        return null;
                    }
                    case 50:
                    {
                        object obj6 = base.nodes[0].GetValue(session);
                        if ((obj6 == null) || !((bool) obj6))
                        {
                            object obj7 = base.nodes[1].GetValue(session);
                            if ((obj7 != null) && ((bool) obj7))
                            {
                                return true;
                            }
                            if ((obj6 != null) && (obj7 != null))
                            {
                                return false;
                            }
                            return null;
                        }
                        return true;
                    }
                    case 0x36:
                        return this.TestInCondition(session, base.nodes[0].GetRowValue(session));

                    case 0x37:
                        return this.TestExistsCondition(session);

                    case 0x38:
                    {
                        object[] rowValue = base.nodes[0].GetRowValue(session);
                        object[] b = base.nodes[1].GetRowValue(session);
                        return DateTimeType.Overlaps(session, rowValue, base.nodes[0].NodeDataTypes, b, base.nodes[1].NodeDataTypes);
                    }
                    case 0x39:
                        base.nodes[0].Materialise(session);
                        return base.nodes[0].subQuery.HasUniqueNotNullRows(session);

                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x40:
                        return this.TestMatchCondition(session, base.nodes[0].GetRowValue(session));

                    default:
                        if (opType == 0x6f)
                        {
                            object obj8 = base.nodes[0].GetValue(session);
                            for (int i = 1; i < base.nodes.Length; i++)
                            {
                                if (obj8 == null)
                                {
                                    return null;
                                }
                                if (base.OpTypeChain[i - 1] == 50)
                                {
                                    if ((bool) obj8)
                                    {
                                        return true;
                                    }
                                    obj8 = base.nodes[i].GetValue(session);
                                }
                                else
                                {
                                    if (!((bool) obj8))
                                    {
                                        return false;
                                    }
                                    obj8 = base.nodes[i].GetValue(session);
                                }
                            }
                            return obj8;
                        }
                        break;
                }
            }
            throw Error.RuntimeError(0xc9, "ExpressionLogical");
        }

        public override bool IsIndexable(RangeVariable rangeVar)
        {
            switch (base.OpType)
            {
                case 0x31:
                    return (base.nodes[0].IsIndexable(rangeVar) || base.nodes[1].IsIndexable(rangeVar));

                case 50:
                    return (base.nodes[0].IsIndexable(rangeVar) && base.nodes[1].IsIndexable(rangeVar));
            }
            return (this.GetIndexableExpression(rangeVar) > null);
        }

        public static Expression OrExpressions(Expression e1, Expression e2)
        {
            if (e1 == null)
            {
                return e2;
            }
            if (e2 == null)
            {
                return e1;
            }
            return new ExpressionLogical(50, e1, e2);
        }

        private void ResolveRowTypes()
        {
            for (int i = 0; i < base.nodes[0].NodeDataTypes.Length; i++)
            {
                SqlType type = base.nodes[0].NodeDataTypes[i];
                SqlType type2 = base.nodes[1].NodeDataTypes[i];
                if (type == null)
                {
                    type = base.nodes[0].NodeDataTypes[i] = type2;
                }
                else if (base.nodes[1].DataType == null)
                {
                    type2 = base.nodes[1].NodeDataTypes[i] = type;
                }
                if ((type == null) || (type2 == null))
                {
                    throw Error.GetError(0x15bf);
                }
                if (type.TypeComparisonGroup != type2.TypeComparisonGroup)
                {
                    throw Error.GetError(0x15ba);
                }
                if (type.IsDateTimeType() && (type.IsDateTimeTypeWithZone() ^ type2.IsDateTimeTypeWithZone()))
                {
                    base.nodes[0].nodes[i] = new ExpressionOp(base.nodes[0].nodes[i]);
                    base.nodes[0].NodeDataTypes[i] = base.nodes[0].nodes[i].DataType;
                }
            }
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            bool flag;
            if ((this.IsQuantified && (base.nodes[1].OpType == 0x1a)) && ((base.nodes[1] is ExpressionTable) && (base.nodes[1].nodes[0].OpType == 8)))
            {
                base.nodes[0].ResolveTypes(session, this);
                base.nodes[1].nodes[0].DataType = SqlType.GetDefaultArrayType(base.nodes[0].DataType.TypeCode);
            }
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i].ResolveTypes(session, this);
                }
            }
            switch (base.OpType)
            {
                case 0x29:
                case 0x2a:
                case 0x2b:
                case 0x2c:
                case 0x2d:
                case 0x2e:
                case 0x3a:
                    this.ResolveTypesForComparison(session, parent);
                    return;

                case 0x2f:
                    if (base.nodes[0].IsUnresolvedParam())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if (base.nodes[0].OpType == 1)
                    {
                        base.SetAsConstantValue(session);
                        return;
                    }
                    return;

                case 0x30:
                    if (!base.nodes[0].IsUnresolvedParam())
                    {
                        if (base.nodes[0].OpType == 1)
                        {
                            if (!base.nodes[0].DataType.IsBooleanType())
                            {
                                throw Error.GetError(0x15bd);
                            }
                            base.SetAsConstantValue(session);
                            return;
                        }
                        if ((base.nodes[0].DataType == null) || !base.nodes[0].DataType.IsBooleanType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.DataType = SqlType.SqlBoolean;
                        return;
                    }
                    base.nodes[0].DataType = SqlType.SqlBoolean;
                    return;

                case 0x31:
                    this.ResolveTypesForLogicalOp();
                    if (base.nodes[0].OpType != 1)
                    {
                        if (base.nodes[1].OpType == 1)
                        {
                            object obj3 = base.nodes[1].GetValue(session);
                            if (obj3 != null)
                            {
                                flag = false;
                                if (!flag.Equals(obj3))
                                {
                                    return;
                                }
                            }
                            base.SetAsConstantValue(false);
                        }
                        return;
                    }
                    if (base.nodes[1].OpType != 1)
                    {
                        object obj2 = base.nodes[0].GetValue(session);
                        if (obj2 != null)
                        {
                            flag = false;
                            if (!flag.Equals(obj2))
                            {
                                return;
                            }
                        }
                        base.SetAsConstantValue(false);
                        return;
                    }
                    base.SetAsConstantValue(session);
                    return;

                case 50:
                    this.ResolveTypesForLogicalOp();
                    if (base.nodes[0].OpType != 1)
                    {
                        if (base.nodes[1].OpType == 1)
                        {
                            object obj5 = base.nodes[1].GetValue(session);
                            flag = true;
                            if (flag.Equals(obj5))
                            {
                                base.SetAsConstantValue(true);
                                return;
                            }
                        }
                        return;
                    }
                    if (base.nodes[1].OpType != 1)
                    {
                        object obj4 = base.nodes[0].GetValue(session);
                        flag = true;
                        if (flag.Equals(obj4))
                        {
                            base.SetAsConstantValue(true);
                            return;
                        }
                        return;
                    }
                    base.SetAsConstantValue(session);
                    return;

                case 0x36:
                    this.ResolveTypesForIn(session);
                    return;

                case 0x37:
                case 0x39:
                    return;

                case 0x38:
                    this.ResolveTypesForOverlaps();
                    return;

                case 0x3b:
                case 60:
                case 0x3d:
                case 0x3e:
                case 0x3f:
                case 0x40:
                    this.ResolveTypesForAllAny(session);
                    return;

                case 1:
                    return;
            }
            throw Error.RuntimeError(0xc9, "ExpressionLogical");
        }

        private void ResolveTypesForAllAny(Session session)
        {
            int degree = base.nodes[0].GetDegree();
            if ((degree == 1) && (base.nodes[0].OpType != 0x19))
            {
                Expression[] list = new Expression[] { base.nodes[0] };
                base.nodes[0] = new Expression(0x19, list);
                base.nodes[0].DataType = base.nodes[0].nodes[0].DataType;
            }
            if (base.nodes[1].OpType == 0x1a)
            {
                base.nodes[1].PrepareTable(session, base.nodes[0], degree);
                base.nodes[1].subQuery.PrepareTable(session);
                if (base.nodes[1].isCorrelated)
                {
                    base.nodes[1].subQuery.SetCorrelated();
                }
            }
            if (base.nodes[1].NodeDataTypes == null)
            {
                base.nodes[1].NodeDataTypes = new SqlType[] { base.nodes[1].DataType };
            }
            if (degree != base.nodes[1].NodeDataTypes.Length)
            {
                throw Error.GetError(0x15bc);
            }
            int opType = base.nodes[1].OpType;
            base.nodes[0].NodeDataTypes = new SqlType[base.nodes[0].nodes.Length];
            for (int i = 0; i < base.nodes[0].NodeDataTypes.Length; i++)
            {
                SqlType type = base.nodes[0].nodes[i].DataType ?? base.nodes[1].NodeDataTypes[i];
                if (type == null)
                {
                    throw Error.GetError(0x15bf);
                }
                base.nodes[0].NodeDataTypes[i] = type;
                base.nodes[0].nodes[i].DataType = type;
            }
            this._hasRowArgument = (base.nodes[0].OpType == 0x19) || (base.nodes[1].OpType == 0x19);
        }

        private void ResolveTypesForComparison(Session session, Expression parent)
        {
            if (((base.OpType == 0x3a) || (base.ExprSubType == 0x33)) || (base.ExprSubType == 0x34))
            {
                this.ResolveTypesForAllAny(session);
            }
            else if ((base.nodes[0].OpType == 0x19) || (base.nodes[1].OpType == 0x19))
            {
                this._hasRowArgument = true;
                if (((base.nodes[0].OpType != 0x19) || (base.nodes[1].OpType != 0x19)) || (base.nodes[0].nodes.Length != base.nodes[1].nodes.Length))
                {
                    throw Error.GetError(0x15bc);
                }
                this.ResolveRowTypes();
                this.CheckRowComparison();
            }
            else
            {
                if (base.nodes[0].IsUnresolvedParam())
                {
                    base.nodes[0].DataType = base.nodes[1].DataType;
                }
                else if (base.nodes[1].IsUnresolvedParam())
                {
                    base.nodes[1].DataType = base.nodes[0].DataType;
                }
                if (base.nodes[0].DataType == null)
                {
                    base.nodes[0].DataType = base.nodes[1].DataType;
                }
                else if (base.nodes[1].DataType == null)
                {
                    base.nodes[1].DataType = base.nodes[0].DataType;
                }
                if ((base.nodes[0].DataType == null) || (base.nodes[1].DataType == null))
                {
                    throw Error.GetError(0x15bf);
                }
                if (base.nodes[0].DataType.TypeComparisonGroup != base.nodes[1].DataType.TypeComparisonGroup)
                {
                    if (!ConvertDateTimeLiteral(session, base.nodes[0], base.nodes[1]))
                    {
                        if (base.nodes[0].DataType.IsBooleanType())
                        {
                            if (!base.nodes[0].DataType.CanConvertFrom(base.nodes[1].DataType))
                            {
                                throw Error.GetError(0x15ba);
                            }
                            base.nodes[1] = ExpressionOp.GetCastExpression(session, base.nodes[1], base.nodes[0].DataType);
                        }
                        else
                        {
                            if (!base.nodes[1].DataType.IsBooleanType())
                            {
                                throw Error.GetError(0x15ba);
                            }
                            if (!base.nodes[1].DataType.CanConvertFrom(base.nodes[0].DataType))
                            {
                                throw Error.GetError(0x15ba);
                            }
                            base.nodes[0] = ExpressionOp.GetCastExpression(session, base.nodes[0], base.nodes[1].DataType);
                        }
                    }
                }
                else if (base.nodes[0].DataType.IsDateTimeType() && (base.nodes[0].DataType.IsDateTimeTypeWithZone() ^ base.nodes[1].DataType.IsDateTimeTypeWithZone()))
                {
                    base.nodes[0] = new ExpressionOp(base.nodes[0]);
                }
                if (((base.OpType != 0x29) && (base.OpType != 0x2e)) && ((base.nodes[0].DataType.IsArrayType() || base.nodes[0].DataType.IsLobType()) || base.nodes[1].DataType.IsLobType()))
                {
                    throw Error.GetError(0x159e);
                }
                if ((base.nodes[0].OpType == 1) && (base.nodes[1].OpType == 1))
                {
                    base.SetAsConstantValue(session);
                }
            }
        }

        public void ResolveTypesForIn(Session session)
        {
            this.ResolveTypesForAllAny(session);
        }

        private void ResolveTypesForLogicalOp()
        {
            if (base.nodes[0].IsUnresolvedParam())
            {
                base.nodes[0].DataType = SqlType.SqlBoolean;
            }
            if (base.nodes[1].IsUnresolvedParam())
            {
                base.nodes[1].DataType = SqlType.SqlBoolean;
            }
            if ((base.nodes[0].DataType == null) || (base.nodes[1].DataType == null))
            {
                throw Error.GetError(0x15c3);
            }
            if ((base.nodes[0].OpType == 0x19) || (base.nodes[1].OpType == 0x19))
            {
                throw Error.GetError(0x15bd);
            }
            if ((SqlType.SqlBoolean != base.nodes[0].DataType) || (SqlType.SqlBoolean != base.nodes[1].DataType))
            {
                throw Error.GetError(0x15c0);
            }
        }

        private void ResolveTypesForOverlaps()
        {
            if (base.nodes[0].nodes[0].IsUnresolvedParam())
            {
                base.nodes[0].nodes[0].DataType = base.nodes[1].nodes[0].DataType;
            }
            if (base.nodes[1].nodes[0].IsUnresolvedParam())
            {
                base.nodes[1].nodes[0].DataType = base.nodes[0].nodes[0].DataType;
            }
            if (base.nodes[0].nodes[0].DataType == null)
            {
                base.nodes[0].nodes[0].DataType = base.nodes[1].nodes[0].DataType = SqlType.SqlTimestamp;
            }
            if (base.nodes[0].nodes[1].IsUnresolvedParam())
            {
                base.nodes[0].nodes[1].DataType = base.nodes[1].nodes[0].DataType;
            }
            if (base.nodes[1].nodes[1].IsUnresolvedParam())
            {
                base.nodes[1].nodes[1].DataType = base.nodes[0].nodes[0].DataType;
            }
            if (!DTIType.IsValidDatetimeRange(base.nodes[0].nodes[0].DataType, base.nodes[0].nodes[1].DataType) || !DTIType.IsValidDatetimeRange(base.nodes[1].nodes[0].DataType, base.nodes[1].nodes[1].DataType))
            {
                throw Error.GetError(0x15bd);
            }
            if (!DTIType.IsValidDatetimeRange(base.nodes[0].nodes[0].DataType, base.nodes[0].nodes[1].DataType))
            {
                throw Error.GetError(0x15bb);
            }
            base.nodes[0].NodeDataTypes[0] = base.nodes[0].nodes[0].DataType;
            base.nodes[0].NodeDataTypes[1] = base.nodes[0].nodes[1].DataType;
            base.nodes[1].NodeDataTypes[0] = base.nodes[1].nodes[0].DataType;
            base.nodes[1].NodeDataTypes[1] = base.nodes[1].nodes[1].DataType;
        }

        public override void SetSubType(int type)
        {
            base.ExprSubType = type;
            if ((base.ExprSubType == 0x33) || (base.ExprSubType == 0x34))
            {
                this.IsQuantified = true;
            }
        }

        public void SwapCondition()
        {
            int num = 0x29;
            int opType = base.OpType;
            switch (opType)
            {
                case 0x29:
                    break;

                case 0x2a:
                    num = 0x2d;
                    break;

                case 0x2b:
                    num = 0x2c;
                    break;

                case 0x2c:
                    num = 0x2b;
                    break;

                case 0x2d:
                    num = 0x2a;
                    break;

                default:
                    if (opType != 0x3a)
                    {
                        throw Error.RuntimeError(0xc9, "ExpressionLogical");
                    }
                    num = 0x3a;
                    break;
            }
            base.OpType = num;
            Expression expression = base.nodes[0];
            base.nodes[0] = base.nodes[1];
            base.nodes[1] = expression;
        }

        private object TestAllAnyCondition(Session session, object[] o)
        {
            SubQuery subQuery = base.nodes[1].subQuery;
            subQuery.MaterialiseCorrelated(session);
            return this.GetAllAnyValue(session, o, subQuery);
        }

        public override bool TestCondition(Session session)
        {
            int opType = base.OpType;
            if (opType <= 5)
            {
                switch (opType)
                {
                    case 1:
                        return ((base.ValueData != null) && ((bool) base.ValueData));

                    case 5:
                    {
                        object obj2 = session.sessionContext.RangeIterators[base.RangePosition].GetCurrent()[base.ColumnIndex];
                        return ((obj2 != null) && ((bool) obj2));
                    }
                }
            }
            else
            {
                switch (opType)
                {
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x3a:
                    {
                        if ((base.ExprSubType != 0x34) && (base.ExprSubType != 0x33))
                        {
                            object left = base.nodes[0].GetValue(session);
                            object right = base.nodes[1].GetValue(session);
                            if (this._hasRowArgument)
                            {
                                object[] objArray = left as object[];
                                object[] objArray2 = right as object[];
                                if (objArray != null)
                                {
                                    if (objArray2 == null)
                                    {
                                        throw Error.RuntimeError(0xc9, "ExpressionLogical");
                                    }
                                    object obj6 = this.CompareValues(session, objArray, objArray2);
                                    return ((obj6 != null) && ((bool) obj6));
                                }
                                if (objArray2 != null)
                                {
                                    return this.CompareValuesBool(session, left, objArray2[0]);
                                }
                            }
                            return this.CompareValuesBool(session, left, right);
                        }
                        object obj5 = this.TestAllAnyCondition(session, base.nodes[0].GetRowValue(session));
                        return ((obj5 != null) && ((bool) obj5));
                    }
                    case 0x2f:
                        return (base.nodes[0].GetValue(session) == null);

                    case 0x30:
                    {
                        object obj7 = base.nodes[0].GetValue(session);
                        if (obj7 == null)
                        {
                            return false;
                        }
                        return !((bool) obj7);
                    }
                    case 0x31:
                        if (!base.nodes[0].TestCondition(session))
                        {
                            return false;
                        }
                        return base.nodes[1].TestCondition(session);

                    case 50:
                        if (!base.nodes[0].TestCondition(session))
                        {
                            return base.nodes[1].TestCondition(session);
                        }
                        return true;

                    case 0x33:
                    case 0x34:
                    case 0x35:
                        break;

                    case 0x36:
                        return this.TestInConditionBool(session, base.nodes[0].GetRowValue(session));

                    case 0x37:
                        return this.TestExistsCondition(session);

                    case 0x38:
                    {
                        object[] rowValue = base.nodes[0].GetRowValue(session);
                        object[] b = base.nodes[1].GetRowValue(session);
                        object obj8 = DateTimeType.Overlaps(session, rowValue, base.nodes[0].NodeDataTypes, b, base.nodes[1].NodeDataTypes);
                        if (obj8 == null)
                        {
                            return false;
                        }
                        return (bool) obj8;
                    }
                    case 0x39:
                        base.nodes[0].Materialise(session);
                        return base.nodes[0].subQuery.HasUniqueNotNullRows(session);

                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x40:
                        return this.TestMatchCondition(session, base.nodes[0].GetRowValue(session));

                    default:
                        if (opType == 0x6f)
                        {
                            bool flag = base.nodes[0].TestCondition(session);
                            for (int i = 1; i < base.nodes.Length; i++)
                            {
                                if (base.OpTypeChain[i - 1] == 50)
                                {
                                    if (flag)
                                    {
                                        return true;
                                    }
                                    flag = base.nodes[i].TestCondition(session);
                                }
                                else
                                {
                                    if (!flag)
                                    {
                                        return false;
                                    }
                                    flag = base.nodes[i].TestCondition(session);
                                }
                            }
                            return flag;
                        }
                        break;
                }
            }
            throw Error.RuntimeError(0xc9, "ExpressionLogical");
        }

        private bool TestExistsCondition(Session session)
        {
            base.nodes[0].Materialise(session);
            return !base.nodes[0].GetTable().IsEmpty(session);
        }

        private object TestInCondition(Session session, object[] data)
        {
            if ((data == null) || (Expression.CountNulls(data) != 0))
            {
                return null;
            }
            return this.TestInConditionCore(session, data);
        }

        private bool TestInConditionBool(Session session, object[] data)
        {
            return (((data != null) && (Expression.CountNulls(data) == 0)) && this.TestInConditionCore(session, data));
        }

        private bool TestInConditionCore(Session session, object[] data)
        {
            if (base.nodes[1].OpType != 0x1a)
            {
                throw Error.RuntimeError(0xc9, "ExpressionLogical");
            }
            int length = base.nodes[1].nodes.Length;
            for (int i = 0; i < length; i++)
            {
                object[] rowValue = base.nodes[1].nodes[i].GetRowValue(session);
                object obj2 = this.CompareValues(session, data, rowValue);
                if ((obj2 != null) && ((bool) obj2))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TestMatchCondition(Session session, object[] data)
        {
            if (data == null)
            {
                return true;
            }
            int num = Expression.CountNulls(data);
            if (num != 0)
            {
                switch (base.OpType)
                {
                    case 0x3b:
                    case 0x3e:
                        return true;

                    case 60:
                    case 0x3f:
                        if (num != data.Length)
                        {
                            break;
                        }
                        return true;

                    case 0x3d:
                    case 0x40:
                        return (num == data.Length);
                }
            }
            if (base.nodes[1].OpType == 0x1a)
            {
                int length = base.nodes[1].nodes.Length;
                bool flag2 = false;
                for (int i = 0; i < length; i++)
                {
                    object[] rowValue = base.nodes[1].nodes[i].GetRowValue(session);
                    object obj2 = this.CompareValues(session, data, rowValue);
                    if ((obj2 == null) || !((bool) obj2))
                    {
                        continue;
                    }
                    int opType = base.OpType;
                    if ((opType - 0x3b) > 2)
                    {
                        if ((opType - 0x3e) <= 2)
                        {
                            goto Label_00CD;
                        }
                        continue;
                    }
                    return true;
                Label_00CD:
                    if (flag2)
                    {
                        return false;
                    }
                    flag2 = true;
                }
                return flag2;
            }
            if (base.nodes[1].OpType != 0x17)
            {
                throw Error.GetError(0x15bc);
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(base.nodes[1].GetTable());
            base.nodes[1].Materialise(session);
            Expression.ConvertToType(session, data, base.nodes[0].NodeDataTypes, base.nodes[1].NodeDataTypes);
            if ((num != 0) && ((base.OpType == 60) || (base.OpType == 0x3f)))
            {
                bool flag3 = false;
                IRowIterator rowIterator = base.nodes[1].GetTable().GetRowIterator(session);
                while (rowIterator.HasNext())
                {
                    object[] right = rowIterator.GetNextRow().RowData;
                    object obj3 = this.CompareValues(session, data, right);
                    if ((obj3 != null) && ((bool) obj3))
                    {
                        if (base.OpType == 60)
                        {
                            return true;
                        }
                        if (flag3)
                        {
                            return false;
                        }
                        flag3 = true;
                    }
                }
                return flag3;
            }
            IRowIterator iterator2 = base.nodes[1].GetTable().GetPrimaryIndex().FindFirstRow(session, rowStore, data);
            if (!iterator2.HasNext())
            {
                return false;
            }
            if ((base.OpType - 0x3b) <= 2)
            {
                return true;
            }
            iterator2.GetNextRow();
            if (!iterator2.HasNext())
            {
                return true;
            }
            object[] rowData = iterator2.GetNextRow().RowData;
            bool flag4 = true;
            return !flag4.Equals(this.CompareValues(session, data, rowData));
        }
    }
}

