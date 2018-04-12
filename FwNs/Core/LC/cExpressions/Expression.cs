namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Expression : IEquatable<Expression>
    {
        public const int Left = 0;
        public const int Right = 1;
        public const int Unary = 1;
        public const int Binary = 2;
        public static Expression[] emptyArray = new Expression[0];
        public static Expression ExprTrue = new ExpressionLogical(true);
        public static Expression ExprFalse = new ExpressionLogical(false);
        public static OrderedIntHashSet AggregateFunctionSet = GetAggregateFunctionSet();
        public static OrderedIntHashSet ColumnExpressionSet = GetColumnExpressionSet();
        public static OrderedIntHashSet SubqueryExpressionSet = GetSubqueryExpressionSet();
        public static OrderedIntHashSet SubqueryAggregateExpressionSet = GetSubqueryAggregateExpressionSet();
        public static OrderedIntHashSet EmptyExpressionSet = new OrderedIntHashSet();
        public static OrderedIntHashSet FunctionExpressionSet = GetFunctionExpressionSet();
        public bool isCorrelated;
        protected byte Nullability;
        public QNameManager.SimpleName Alias;
        public int ColumnIndex;
        public SqlType DataType;
        public int ExprSubType;
        public bool isAggregate;
        public bool IsColumnEqual;
        public SqlType[] NodeDataTypes;
        public Expression[] nodes;
        public int OpType;
        public int[] OpTypeChain;
        public bool NoBreak;
        public int ParameterIndex;
        public int QueryTableColumnIndex;
        public int RangePosition;
        public SubQuery subQuery;
        public object ValueData;
        public bool IsDistinctAggregate;
        public int FuncType;

        public Expression(int type)
        {
            this.Nullability = 1;
            this.ColumnIndex = -1;
            this.ParameterIndex = -1;
            this.QueryTableColumnIndex = -1;
            this.RangePosition = -1;
            this.OpType = type;
            this.nodes = emptyArray;
        }

        public Expression(int exprType, SubQuery sq)
        {
            this.Nullability = 1;
            this.ColumnIndex = -1;
            this.ParameterIndex = -1;
            this.QueryTableColumnIndex = -1;
            this.RangePosition = -1;
            if ((exprType - 0x15) > 1)
            {
                if (exprType == 0x17)
                {
                    this.OpType = 0x17;
                }
                else
                {
                    if (exprType != 0x6a)
                    {
                        if (exprType != 0x6b)
                        {
                            throw Error.RuntimeError(0xc9, "Expression");
                        }
                    }
                    else
                    {
                        this.OpType = 0x6a;
                        goto Label_007B;
                    }
                    this.OpType = 0x6b;
                }
            }
            else
            {
                this.OpType = 0x16;
            }
        Label_007B:
            this.nodes = emptyArray;
            this.subQuery = sq;
        }

        public Expression(int type, Expression[] list) : this(type)
        {
            this.nodes = list;
        }

        public void CheckValidCheckConstraint()
        {
            OrderedHashSet<Expression> set = null;
            set = this.CollectAllExpressions(set, SubqueryAggregateExpressionSet, EmptyExpressionSet);
            if ((set != null) && !set.IsEmpty())
            {
                throw Error.GetError(0x5dc, "subquery in check constraint");
            }
        }

        public OrderedHashSet<Expression> CollectAllExpressions(OrderedHashSet<Expression> set, OrderedIntHashSet typeSet, OrderedIntHashSet stopAtTypeSet)
        {
            if (!stopAtTypeSet.Contains(this.OpType))
            {
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    if (this.nodes[i] != null)
                    {
                        set = this.nodes[i].CollectAllExpressions(set, typeSet, stopAtTypeSet);
                    }
                }
                if (typeSet.Contains(this.OpType))
                {
                    if (set == null)
                    {
                        set = new OrderedHashSet<Expression>();
                    }
                    set.Add(this);
                }
                if ((this.subQuery != null) && (this.subQuery.queryExpression != null))
                {
                    set = this.subQuery.queryExpression.CollectAllExpressions(set, typeSet, stopAtTypeSet);
                }
            }
            return set;
        }

        public OrderedHashSet<SubQuery> CollectAllSubqueries(OrderedHashSet<SubQuery> set)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    set = this.nodes[i].CollectAllSubqueries(set);
                }
            }
            if (this.subQuery != null)
            {
                if (set == null)
                {
                    set = new OrderedHashSet<SubQuery>();
                }
                set.Add(this.subQuery);
                set = OrderedHashSet<SubQuery>.AddAll(set, this.subQuery.GetExtraSubqueries());
                if (this.subQuery.queryExpression != null)
                {
                    OrderedHashSet<SubQuery> subqueries = this.subQuery.queryExpression.GetSubqueries();
                    set = OrderedHashSet<SubQuery>.AddAll(set, subqueries);
                }
            }
            return set;
        }

        public virtual void CollectObjectNames(FwNs.Core.LC.cLib.ISet<QNameManager.QName> set)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    this.nodes[i].CollectObjectNames(set);
                }
            }
            if ((this.subQuery != null) && (this.subQuery.queryExpression != null))
            {
                this.subQuery.queryExpression.CollectObjectNames(set);
            }
        }

        public virtual void CollectRangeVariables(RangeVariable[] rangeVariables, FwNs.Core.LC.cLib.ISet<RangeVariable> set)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    this.nodes[i].CollectRangeVariables(rangeVariables, set);
                }
            }
            if ((this.subQuery != null) && (this.subQuery.queryExpression != null))
            {
                List<Expression> unresolvedExpressions = this.subQuery.queryExpression.GetUnresolvedExpressions();
                if (unresolvedExpressions != null)
                {
                    for (int j = 0; j < unresolvedExpressions.Count; j++)
                    {
                        unresolvedExpressions[j].CollectRangeVariables(rangeVariables, set);
                    }
                }
            }
        }

        public void ConvertToSimpleColumn(OrderedHashSet<Expression> expressions, OrderedHashSet<Expression> replacements)
        {
            if (this.OpType != 1)
            {
                int index = expressions.GetIndex(this);
                if (index != -1)
                {
                    Expression expression = replacements.Get(index);
                    this.nodes = emptyArray;
                    this.OpType = 5;
                    this.ColumnIndex = expression.ColumnIndex;
                    this.RangePosition = expression.RangePosition;
                }
                else
                {
                    for (int i = 0; i < this.nodes.Length; i++)
                    {
                        if (this.nodes[i] != null)
                        {
                            this.nodes[i].ConvertToSimpleColumn(expressions, replacements);
                        }
                    }
                }
            }
        }

        public static void ConvertToType(Session session, object[] data, SqlType[] dataType, SqlType[] newType)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = newType[i].ConvertToType(session, data[i], dataType[i]);
            }
        }

        public static int CountNulls(object[] a)
        {
            int num = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == null)
                {
                    num++;
                }
            }
            return num;
        }

        public virtual string Describe(Session session, int blanks)
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append('\n');
            for (int i = 0; i < blanks; i++)
            {
                builder.Append(' ');
            }
            int opType = this.OpType;
            switch (opType)
            {
                case 0x16:
                case 0x17:
                    builder.Append("QUERY ");
                    builder.Append(this.subQuery.queryExpression.Describe(session));
                    return builder.ToString();

                case 0x18:
                    break;

                case 0x19:
                    builder.Append("ROW = ");
                    for (int j = 0; j < this.nodes.Length; j++)
                    {
                        builder.Append(this.nodes[j].Describe(session, blanks + 1));
                        builder.Append(' ');
                    }
                    break;

                case 0x1a:
                    builder.Append("VALUELIST ");
                    for (int j = 0; j < this.nodes.Length; j++)
                    {
                        builder.Append(this.nodes[j].Describe(session, blanks + 1));
                        builder.Append(' ');
                    }
                    break;

                case 1:
                    builder.Append("VALUE = ").Append(this.ValueData);
                    builder.Append(", TYPE = ").Append(this.DataType.GetNameString());
                    return builder.ToString();

                default:
                    if (opType != 0x6a)
                    {
                        if (opType == 0x6b)
                        {
                            builder.Append("ARRAY ");
                            return builder.ToString();
                        }
                        break;
                    }
                    builder.Append("ARRAY SUBQUERY");
                    return builder.ToString();
            }
            return builder.ToString();
        }

        public Expression Duplicate()
        {
            Expression expression;
            try
            {
                expression = (Expression) base.MemberwiseClone();
                expression.nodes = (Expression[]) this.nodes.Clone();
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    if (this.nodes[i] != null)
                    {
                        expression.nodes[i] = this.nodes[i].Duplicate();
                    }
                }
            }
            catch (Exception)
            {
                throw Error.RuntimeError(0xc9, "Expression");
            }
            return expression;
        }

        public virtual bool Equals(Expression other)
        {
            if (other == this)
            {
                return true;
            }
            if (other == null)
            {
                return false;
            }
            if (((this.OpType != other.OpType) || (this.ExprSubType != other.ExprSubType)) || !Equals(this.DataType, other.DataType))
            {
                return false;
            }
            int opType = this.OpType;
            if (opType > 5)
            {
                if (((opType - 0x16) <= 1) || ((opType - 0x6a) <= 1))
                {
                    return this.subQuery.queryExpression.IsEquivalent(other.subQuery.queryExpression);
                }
            }
            else
            {
                switch (opType)
                {
                    case 1:
                        return Equals(this.ValueData, other.ValueData);

                    case 5:
                        return (this.ColumnIndex == other.ColumnIndex);
                }
            }
            return (Equals(this.nodes, other.nodes) && Equals(this.subQuery, other.subQuery));
        }

        public override bool Equals(object other)
        {
            if (other == this)
            {
                return true;
            }
            Expression expression = other as Expression;
            return ((expression != null) && this.Equals(expression));
        }

        public static bool Equals(object o1, object o2)
        {
            if (o1 == o2)
            {
                return true;
            }
            if (o1 != null)
            {
                return o1.Equals(o2);
            }
            return (o2 == null);
        }

        private static bool Equals(Expression[] row1, Expression[] row2)
        {
            if (row1 != row2)
            {
                if (row1.Length != row2.Length)
                {
                    return false;
                }
                int length = row1.Length;
                for (int i = 0; i < length; i++)
                {
                    Expression expression = row1[i];
                    Expression other = row2[i];
                    if (!((expression == null) ? (other == null) : expression.Equals(other)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual int FindMatchingRangeVariableIndex(RangeVariable[] rangeVarArray)
        {
            return -1;
        }

        public virtual object GetAggregatedValue(Session session, object currValue)
        {
            throw Error.RuntimeError(0xc9, "Expression");
        }

        private static OrderedIntHashSet GetAggregateFunctionSet()
        {
            OrderedIntHashSet set1 = new OrderedIntHashSet();
            set1.Add(0x47);
            set1.Add(0x48);
            set1.Add(0x49);
            set1.Add(0x4a);
            set1.Add(0x4b);
            set1.Add(0x4c);
            set1.Add(0x4d);
            set1.Add(0x4e);
            set1.Add(0x4f);
            set1.Add(80);
            set1.Add(0x51);
            set1.Add(0x68);
            return set1;
        }

        public virtual string GetAlias()
        {
            if (this.Alias != null)
            {
                return this.Alias.Name;
            }
            return "";
        }

        public static QuerySpecification GetCheckSelect(Session session, Table t, Expression e)
        {
            ParserDQL.CompileContext compileContext = new ParserDQL.CompileContext(session, null);
            compileContext.Reset(0);
            QuerySpecification specification = new QuerySpecification(compileContext);
            RangeVariable[] ranges = new RangeVariable[] { new RangeVariable(t, null, null, null, compileContext) };
            e.ResolveCheckOrGenExpression(session, ranges, true);
            specification.ExprColumns = new Expression[] { new ExpressionLogical(true) };
            specification.RangeVariables = ranges;
            if (SqlType.SqlBoolean != e.GetDataType())
            {
                throw Error.GetError(0x15c0);
            }
            Expression expression = new ExpressionLogical(0x30, e);
            specification.QueryCondition = expression;
            specification.ResolveReferences(session, RangeVariable.EmptyArray);
            specification.ResolveTypes(session);
            return specification;
        }

        public virtual ColumnSchema GetColumn()
        {
            return null;
        }

        private static OrderedIntHashSet GetColumnExpressionSet()
        {
            OrderedIntHashSet set1 = new OrderedIntHashSet();
            set1.Add(2);
            return set1;
        }

        public int GetColumnIndex()
        {
            return this.ColumnIndex;
        }

        public virtual string GetColumnName()
        {
            return this.GetAlias();
        }

        public object GetConstantValueNoCheck(Session session)
        {
            try
            {
                return this.GetValue(session);
            }
            catch (CoreException)
            {
                return null;
            }
        }

        public static string GetContextSql(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            string sql = expression.GetSql();
            int opType = expression.OpType;
            if (opType <= 0x1c)
            {
                if ((opType - 1) <= 1)
                {
                    return sql;
                }
                switch (opType)
                {
                    case 0x19:
                    case 0x1b:
                    case 0x1c:
                        return sql;
                }
            }
            else
            {
                switch (opType)
                {
                    case 0x5b:
                    case 0x5d:
                        return sql;

                    case 0x60:
                        return sql;
                }
            }
            return new StringBuilder().Append('(').Append(sql).Append(')').ToString();
        }

        public virtual SqlType GetDataType()
        {
            return this.DataType;
        }

        public int GetDegree()
        {
            int opType = this.OpType;
            if ((opType - 0x16) > 1)
            {
                if (opType == 0x19)
                {
                    return this.nodes.Length;
                }
                return 1;
            }
            SubQuery subQuery = this.subQuery;
            return this.subQuery.queryExpression.GetColumnCount();
        }

        public int GetExprType()
        {
            return this.OpType;
        }

        private static OrderedIntHashSet GetFunctionExpressionSet()
        {
            OrderedIntHashSet set1 = new OrderedIntHashSet();
            set1.Add(0x1c);
            set1.Add(0x1b);
            return set1;
        }

        public override int GetHashCode()
        {
            int num = this.OpType + this.ExprSubType;
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    num += this.nodes[i].GetHashCode();
                }
            }
            return num;
        }

        public virtual Expression GetIndexableExpression(RangeVariable rangeVar)
        {
            return null;
        }

        public Expression GetLeftNode()
        {
            if (this.nodes.Length == 0)
            {
                return null;
            }
            return this.nodes[0];
        }

        public SqlType GetNodeDataType(int i)
        {
            if (this.NodeDataTypes != null)
            {
                return this.NodeDataTypes[i];
            }
            if (i > 0)
            {
                throw Error.RuntimeError(0xc9, "Expression");
            }
            return this.DataType;
        }

        public SqlType[] GetNodeDataTypes()
        {
            if (this.NodeDataTypes == null)
            {
                return new SqlType[] { this.DataType };
            }
            return this.NodeDataTypes;
        }

        public virtual byte GetNullability()
        {
            return this.Nullability;
        }

        public virtual RangeVariable GetRangeVariable()
        {
            return null;
        }

        public virtual Result GetResult(Session session)
        {
            switch (this.OpType)
            {
                case 0x17:
                {
                    this.subQuery.MaterialiseCorrelated(session);
                    Result result1 = Result.NewResult(this.subQuery.GetNavigator(session));
                    result1.MetaData = this.subQuery.queryExpression.GetMetaData();
                    return result1;
                }
                case 0x6b:
                {
                    RowSetNavigatorData navigator = this.subQuery.GetNavigator(session);
                    object[] objArray = new object[navigator.GetSize()];
                    navigator.BeforeFirst();
                    for (int i = 0; navigator.HasNext(); i++)
                    {
                        object[] next = navigator.GetNext();
                        objArray[i] = next[0];
                    }
                    return Result.NewPsmResult(objArray);
                }
            }
            return Result.NewPsmResult(this.GetValue(session));
        }

        public Expression GetRightNode()
        {
            if (this.nodes.Length <= 1)
            {
                return null;
            }
            return this.nodes[1];
        }

        public virtual object[] GetRowValue(Session session)
        {
            int opType = this.OpType;
            if ((opType - 0x16) > 1)
            {
                if (opType != 0x19)
                {
                    throw Error.RuntimeError(0xc9, "Expression");
                }
            }
            else
            {
                return this.subQuery.queryExpression.GetValues(session);
            }
            object[] objArray = new object[this.nodes.Length];
            for (int i = 0; i < this.nodes.Length; i++)
            {
                objArray[i] = this.nodes[i].GetValue(session);
            }
            return objArray;
        }

        public virtual QNameManager.SimpleName GetSimpleName()
        {
            if (this.Alias != null)
            {
                return this.Alias;
            }
            return null;
        }

        public virtual string GetSql()
        {
            StringBuilder builder = new StringBuilder(0x40);
            switch (this.OpType)
            {
                case 1:
                    if (this.ValueData == null)
                    {
                        return "NULL";
                    }
                    return this.DataType.ConvertToSQLString(this.ValueData);

                case 0x19:
                    builder.Append('(');
                    for (int i = 0; i < this.nodes.Length; i++)
                    {
                        if (i > 0)
                        {
                            builder.Append(',');
                        }
                        builder.Append(this.nodes[i].GetSql());
                    }
                    builder.Append(')');
                    return builder.ToString();

                case 0x1a:
                    for (int i = 0; i < this.nodes.Length; i++)
                    {
                        if (i > 0)
                        {
                            builder.Append(',');
                        }
                        builder.Append(this.nodes[i].GetSql());
                    }
                    return builder.ToString();
            }
            int opType = this.OpType;
            if (((opType - 0x16) > 1) && (opType != 0x6a))
            {
                if (opType != 0x6b)
                {
                    throw Error.RuntimeError(0xc9, "Expression");
                }
                builder.Append("ARRAY").Append('[');
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(',');
                    }
                    builder.Append(this.nodes[i].GetSql());
                }
                builder.Append(']');
            }
            else
            {
                builder.Append('(');
                builder.Append(')');
            }
            return builder.ToString();
        }

        public OrderedHashSet<SubQuery> GetSubqueries()
        {
            return this.CollectAllSubqueries(null);
        }

        private static OrderedIntHashSet GetSubqueryAggregateExpressionSet()
        {
            OrderedIntHashSet set1 = new OrderedIntHashSet();
            set1.Add(0x47);
            set1.Add(0x48);
            set1.Add(0x49);
            set1.Add(0x4a);
            set1.Add(0x4b);
            set1.Add(0x4c);
            set1.Add(0x4d);
            set1.Add(0x4e);
            set1.Add(0x4f);
            set1.Add(80);
            set1.Add(0x51);
            set1.Add(0x68);
            set1.Add(0x17);
            set1.Add(0x16);
            return set1;
        }

        private static OrderedIntHashSet GetSubqueryExpressionSet()
        {
            OrderedIntHashSet set1 = new OrderedIntHashSet();
            set1.Add(0x16);
            set1.Add(0x17);
            return set1;
        }

        public Table GetTable()
        {
            if (this.subQuery != null)
            {
                return this.subQuery.GetTable();
            }
            return null;
        }

        public virtual OrderedHashSet<Expression> GetUnkeyedColumns(OrderedHashSet<Expression> unresolvedSet)
        {
            if (this.OpType != 1)
            {
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    if (this.nodes[i] != null)
                    {
                        unresolvedSet = this.nodes[i].GetUnkeyedColumns(unresolvedSet);
                    }
                }
                int opType = this.OpType;
                if (((opType - 0x16) > 1) && ((opType - 0x6a) > 1))
                {
                    return unresolvedSet;
                }
                if (this.subQuery != null)
                {
                    if (unresolvedSet == null)
                    {
                        unresolvedSet = new OrderedHashSet<Expression>();
                    }
                    unresolvedSet.Add(this);
                }
            }
            return unresolvedSet;
        }

        public virtual object GetValue(Session session)
        {
            int opType = this.OpType;
            if (opType <= 5)
            {
                switch (opType)
                {
                    case 1:
                        return this.ValueData;

                    case 5:
                        return session.sessionContext.RangeIterators[this.RangePosition].GetCurrent()[this.ColumnIndex];
                }
            }
            else
            {
                switch (opType)
                {
                    case 0x16:
                    case 0x17:
                    {
                        this.subQuery.MaterialiseCorrelated(session);
                        object[] values = this.subQuery.GetValues(session);
                        if (values.Length != 1)
                        {
                            return values;
                        }
                        return values[0];
                    }
                    case 0x18:
                        break;

                    case 0x19:
                        if (this.nodes.Length != 1)
                        {
                            object[] objArray2 = new object[this.nodes.Length];
                            for (int i = 0; i < this.nodes.Length; i++)
                            {
                                objArray2[i] = this.nodes[i].GetValue(session);
                            }
                            return objArray2;
                        }
                        return this.nodes[0].GetValue(session);

                    default:
                    {
                        if (opType != 0x6a)
                        {
                            if (opType == 0x6b)
                            {
                                object[] objArray5 = new object[this.nodes.Length];
                                for (int j = 0; j < this.nodes.Length; j++)
                                {
                                    objArray5[j] = this.nodes[j].GetValue(session);
                                }
                                return objArray5;
                            }
                            break;
                        }
                        this.subQuery.MaterialiseCorrelated(session);
                        RowSetNavigatorData navigator = this.subQuery.GetNavigator(session);
                        object[] objArray3 = new object[navigator.GetSize()];
                        navigator.BeforeFirst();
                        for (int i = 0; navigator.HasNext(); i++)
                        {
                            object[] nextRowData = navigator.GetNextRowData();
                            objArray3[i] = nextRowData[0];
                        }
                        return objArray3;
                    }
                }
            }
            throw Error.RuntimeError(0xc9, "Expression");
        }

        public virtual object GetValue(Session session, SqlType type)
        {
            object a = this.GetValue(session);
            if ((a == null) || (this.DataType == type))
            {
                return a;
            }
            return type.ConvertToType(session, a, this.DataType);
        }

        public bool HasNonDeterministicFunction()
        {
            OrderedHashSet<Expression> set = null;
            set = this.CollectAllExpressions(set, FunctionExpressionSet, EmptyExpressionSet);
            if (set != null)
            {
                for (int i = 0; i < set.Size(); i++)
                {
                    Expression expression = set.Get(i);
                    if (expression.OpType == 0x1b)
                    {
                        if (!((FunctionSQLInvoked) expression).IsDeterministic())
                        {
                            return true;
                        }
                    }
                    else if ((expression.OpType == 0x1c) && !((FunctionSQL) expression).IsDeterministic())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool HasReference(RangeVariable range)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if ((this.nodes[i] != null) && this.nodes[i].HasReference(range))
                {
                    return true;
                }
            }
            return (((this.subQuery != null) && (this.subQuery.queryExpression != null)) && this.subQuery.queryExpression.HasReference(range));
        }

        public virtual void InsertValuesIntoSubqueryTable(Session session, IPersistentStore store)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                object[] rowValue = this.nodes[i].GetRowValue(session);
                for (int j = 0; j < this.NodeDataTypes.Length; j++)
                {
                    rowValue[j] = this.NodeDataTypes[j].ConvertToType(session, rowValue[j], this.nodes[i].nodes[j].DataType);
                }
                Row newCachedObject = store.GetNewCachedObject(session, rowValue);
                try
                {
                    store.IndexRow(session, newCachedObject);
                }
                catch (CoreException)
                {
                }
            }
        }

        public bool IsAggregate()
        {
            return this.isAggregate;
        }

        public bool IsComposedOf(OrderedHashSet<Expression> expressions, OrderedIntHashSet excludeSet)
        {
            if (((this.OpType == 1) || (this.OpType == 8)) || ((this.OpType == 7) || (this.OpType == 6)))
            {
                return true;
            }
            if (excludeSet.Contains(this.OpType))
            {
                return true;
            }
            for (int i = 0; i < expressions.Size(); i++)
            {
                if (this.Equals(expressions.Get(i)))
                {
                    return true;
                }
            }
            if ((this.OpType - 0x47) <= 10)
            {
                return false;
            }
            if (this.nodes.Length == 0)
            {
                return false;
            }
            bool flag2 = true;
            for (int j = 0; j < this.nodes.Length; j++)
            {
                flag2 &= (this.nodes[j] == null) || this.nodes[j].IsComposedOf(expressions, excludeSet);
            }
            return flag2;
        }

        public bool IsComposedOf(Expression[] exprList, int start, int end, OrderedIntHashSet excludeSet)
        {
            if (this.OpType == 1)
            {
                return true;
            }
            if (excludeSet.Contains(this.OpType))
            {
                return true;
            }
            for (int i = start; i < end; i++)
            {
                if (this.Equals(exprList[i]))
                {
                    return true;
                }
            }
            int opType = this.OpType;
            if ((opType - 0x16) <= 1)
            {
                return false;
            }
            switch (opType)
            {
                case 0x35:
                case 0x37:
                case 0x39:
                case 0x3b:
                case 60:
                case 0x3d:
                case 0x3e:
                case 0x3f:
                case 0x40:
                case 0x47:
                case 0x48:
                case 0x49:
                case 0x4a:
                case 0x4b:
                case 0x4c:
                case 0x4d:
                case 0x4e:
                case 0x4f:
                case 80:
                case 0x51:
                    return false;

                case 0x36:
                case 0x38:
                case 0x3a:
                case 0x41:
                case 0x42:
                case 0x43:
                case 0x44:
                case 0x45:
                case 70:
                    break;

                default:
                    if ((opType - 0x6a) <= 1)
                    {
                        return false;
                    }
                    break;
            }
            if (this.nodes.Length == 0)
            {
                return false;
            }
            bool flag2 = true;
            for (int j = 0; j < this.nodes.Length; j++)
            {
                flag2 &= (this.nodes[j] == null) || this.nodes[j].IsComposedOf(exprList, start, end, excludeSet);
            }
            return flag2;
        }

        public bool IsCorrelated()
        {
            return ((this.subQuery != null) && this.subQuery.IsCorrelated());
        }

        public virtual bool IsDynamicParam()
        {
            return false;
        }

        public virtual bool IsIndexable(RangeVariable range)
        {
            return false;
        }

        public virtual bool IsParameter()
        {
            return false;
        }

        public virtual bool IsSelfAggregate()
        {
            return false;
        }

        public virtual bool IsUnresolvedParam()
        {
            return false;
        }

        public void Materialise(Session session)
        {
            if (this.subQuery != null)
            {
                this.subQuery.Materialise(session);
            }
        }

        public void PrepareTable(Session session, Expression row, int degree)
        {
            if (this.NodeDataTypes == null)
            {
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    Expression expression = this.nodes[i];
                    if (expression.OpType == 0x19)
                    {
                        if (degree != expression.nodes.Length)
                        {
                            throw Error.GetError(0x15bc);
                        }
                    }
                    else
                    {
                        if (degree != 1)
                        {
                            throw Error.GetError(0x15bc);
                        }
                        Expression expression2 = new Expression(0x19);
                        expression2.nodes = new Expression[] { expression };
                        this.nodes[i] = expression2;
                    }
                }
                this.NodeDataTypes = new SqlType[degree];
                for (int j = 0; j < degree; j++)
                {
                    SqlType existing = (row == null) ? null : row.nodes[j].DataType;
                    for (int k = 0; k < this.nodes.Length; k++)
                    {
                        existing = SqlType.GetAggregateType(this.nodes[k].nodes[j].DataType, existing);
                    }
                    if (existing == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    this.NodeDataTypes[j] = existing;
                    if ((row != null) && row.nodes[j].IsUnresolvedParam())
                    {
                        row.nodes[j].DataType = existing;
                    }
                    for (int m = 0; m < this.nodes.Length; m++)
                    {
                        if (this.nodes[m].nodes[j].IsUnresolvedParam())
                        {
                            this.nodes[m].nodes[j].DataType = this.NodeDataTypes[j];
                        }
                        else if ((this.nodes[m].nodes[j].OpType == 1) && (this.nodes[m].nodes[j].ValueData == null))
                        {
                            this.nodes[m].nodes[j].DataType = this.NodeDataTypes[j];
                        }
                    }
                    if (this.NodeDataTypes[j].IsCharacterType())
                    {
                        ((CharacterType) this.NodeDataTypes[j]).IsEqualIdentical();
                    }
                }
            }
        }

        public virtual Expression ReplaceAliasInOrderBy(Expression[] columns, int length)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    this.nodes[i] = this.nodes[i].ReplaceAliasInOrderBy(columns, length);
                }
            }
            return this;
        }

        public virtual Expression ReplaceColumnReferences(RangeVariable range, Expression[] list)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    this.nodes[i] = this.nodes[i].ReplaceColumnReferences(range, list);
                }
            }
            if ((this.subQuery != null) && (this.subQuery.queryExpression != null))
            {
                this.subQuery.queryExpression.ReplaceColumnReference(range, list);
            }
            return this;
        }

        public void ReplaceNode(Expression existing, Expression replacement)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] == existing)
                {
                    replacement.Alias = this.nodes[i].Alias;
                    this.nodes[i] = replacement;
                    return;
                }
            }
            throw Error.RuntimeError(0xc9, "Expression");
        }

        public virtual void ReplaceRangeVariables(RangeVariable[] ranges, RangeVariable[] newRanges)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    this.nodes[i].ReplaceRangeVariables(ranges, newRanges);
                }
            }
            if ((this.subQuery != null) && (this.subQuery.queryExpression != null))
            {
                this.subQuery.queryExpression.ReplaceRangeVariables(ranges, newRanges);
            }
        }

        public virtual void ResetColumnReferences()
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    this.nodes[i].ResetColumnReferences();
                }
            }
        }

        public void ResolveCheckOrGenExpression(Session session, RangeVariable[] ranges, bool isCheck)
        {
            bool flag = false;
            OrderedHashSet<Expression> set = new OrderedHashSet<Expression>();
            ExpressionColumn.CheckColumnsResolved(this.ResolveColumnReferences(ranges, null));
            this.ResolveTypes(session, null);
            this.CollectAllExpressions(set, SubqueryAggregateExpressionSet, EmptyExpressionSet);
            if (!set.IsEmpty())
            {
                throw Error.GetError(0x1588);
            }
            this.CollectAllExpressions(set, FunctionExpressionSet, EmptyExpressionSet);
            for (int i = 0; i < set.Size(); i++)
            {
                Expression expression = set.Get(i);
                if ((expression.OpType == 0x1b) && !((FunctionSQLInvoked) expression).IsDeterministic())
                {
                    throw Error.GetError(0x1588);
                }
                if ((expression.OpType == 0x1c) && !((FunctionSQL) expression).IsDeterministic())
                {
                    if (!isCheck)
                    {
                        throw Error.GetError(0x1588);
                    }
                    flag = true;
                }
            }
            if (isCheck & flag)
            {
                List<Expression> conditions = new List<Expression>();
                RangeVariableResolver.DecomposeAndConditions(this, conditions);
                for (int k = 0; k < conditions.Count; k++)
                {
                    flag = true;
                    Expression leftNode = conditions[k];
                    ExpressionLogical logical = leftNode as ExpressionLogical;
                    if ((logical != null) && logical.ConvertToSmaller())
                    {
                        Expression rightNode = leftNode.GetRightNode();
                        leftNode = leftNode.GetLeftNode();
                        if (!leftNode.DataType.IsDateTimeType())
                        {
                            flag = true;
                        }
                        else if (leftNode.HasNonDeterministicFunction())
                        {
                            flag = true;
                        }
                        else
                        {
                            if (rightNode is ExpressionArithmetic)
                            {
                                if (this.OpType == 0x20)
                                {
                                    if (rightNode.GetRightNode().HasNonDeterministicFunction())
                                    {
                                        rightNode.SwapLeftAndRightNodes();
                                    }
                                }
                                else if (this.OpType != 0x21)
                                {
                                    break;
                                }
                                if (rightNode.GetRightNode().HasNonDeterministicFunction())
                                {
                                    break;
                                }
                                rightNode = rightNode.GetLeftNode();
                            }
                            if (rightNode.OpType == 0x1c)
                            {
                                switch (((FunctionSQL) rightNode).FuncType)
                                {
                                    case 0x29:
                                    case 0x2b:
                                    case 50:
                                        goto Label_01BB;
                                }
                            }
                        }
                    }
                    break;
                Label_01BB:
                    flag = false;
                }
                if (flag)
                {
                    throw Error.GetError(0x1588);
                }
            }
            set.Clear();
            OrderedHashSet<QNameManager.QName> set2 = new OrderedHashSet<QNameManager.QName>();
            this.CollectObjectNames(set2);
            for (int j = 0; j < set2.Size(); j++)
            {
                QNameManager.QName name = set2.Get(j);
                int type = name.type;
                switch (type)
                {
                    case 7:
                        throw Error.GetError(0x1588);

                    case 8:
                        break;

                    case 9:
                        if (!isCheck)
                        {
                            int num6 = ranges[0].RangeTable.FindColumn(name.Name);
                            if (ranges[0].RangeTable.GetColumn(num6).IsGenerated())
                            {
                                throw Error.GetError(0x1588);
                            }
                        }
                        break;

                    default:
                        if (type == 0x18)
                        {
                            Routine schemaObject = (Routine) session.database.schemaManager.GetSchemaObject(name);
                            if (!schemaObject.IsDeterministic())
                            {
                                throw Error.GetError(0x1588);
                            }
                            switch (schemaObject.GetDataImpact())
                            {
                                case 3:
                                case 4:
                                    throw Error.GetError(0x1588);
                            }
                        }
                        break;
                }
            }
            set2.Clear();
        }

        public virtual List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, List<Expression> unresolvedSet)
        {
            return this.ResolveColumnReferences(rangeVarArray, rangeVarArray.Length, unresolvedSet, true);
        }

        public virtual List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            switch (this.OpType)
            {
                case 0x1a:
                {
                    List<Expression> list2 = null;
                    for (int j = 0; j < this.nodes.Length; j++)
                    {
                        if (this.nodes[j] != null)
                        {
                            list2 = this.nodes[j].ResolveColumnReferences(RangeVariable.EmptyArray, list2);
                        }
                    }
                    if (list2 != null)
                    {
                        this.isCorrelated = true;
                        if (this.subQuery != null)
                        {
                            this.subQuery.SetCorrelated();
                        }
                        for (int k = 0; k < list2.Count; k++)
                        {
                            unresolvedSet = list2[k].ResolveColumnReferences(rangeVarArray, unresolvedSet);
                        }
                        unresolvedSet = ResolveColumnSet(rangeVarArray, rangeVarArray.Length, list2, unresolvedSet);
                    }
                    return unresolvedSet;
                }
                case 0x5d:
                    acceptsSequences = false;
                    break;

                case 1:
                    return unresolvedSet;
            }
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    unresolvedSet = this.nodes[i].ResolveColumnReferences(rangeVarArray, rangeCount, unresolvedSet, acceptsSequences);
                }
            }
            int opType = this.OpType;
            if (((opType - 0x16) <= 1) || (opType == 0x6a))
            {
                QueryExpression queryExpression = this.subQuery.queryExpression;
                if (queryExpression.AreColumnsResolved())
                {
                    return unresolvedSet;
                }
                this.isCorrelated = true;
                this.subQuery.SetCorrelated();
                if (unresolvedSet == null)
                {
                    unresolvedSet = new List<Expression>();
                }
                unresolvedSet.AddRange(queryExpression.GetUnresolvedExpressions());
            }
            return unresolvedSet;
        }

        public static List<Expression> ResolveColumnSet(RangeVariable[] rangeVars, int rangeCount, List<Expression> sourceSet, List<Expression> targetSet)
        {
            if (sourceSet != null)
            {
                for (int i = 0; i < sourceSet.Count; i++)
                {
                    targetSet = sourceSet[i].ResolveColumnReferences(rangeVars, rangeCount, targetSet, true);
                }
            }
            return targetSet;
        }

        public virtual void ResolveTypes(Session session, Expression parent)
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    this.nodes[i].ResolveTypes(session, this);
                }
            }
            int opType = this.OpType;
            switch (opType)
            {
                case 0x16:
                case 0x17:
                {
                    QueryExpression queryExpression = this.subQuery.queryExpression;
                    queryExpression.ResolveTypes(session);
                    this.subQuery.PrepareTable(session);
                    this.NodeDataTypes = queryExpression.GetColumnTypes();
                    this.DataType = this.NodeDataTypes[0];
                    return;
                }
                case 0x18:
                    break;

                case 0x19:
                    this.NodeDataTypes = new SqlType[this.nodes.Length];
                    for (int j = 0; j < this.nodes.Length; j++)
                    {
                        if (this.nodes[j] != null)
                        {
                            this.NodeDataTypes[j] = this.nodes[j].DataType;
                        }
                    }
                    return;

                case 0x1a:
                    return;

                case 1:
                    return;

                default:
                {
                    if (opType != 0x6a)
                    {
                        if (opType == 0x6b)
                        {
                            bool flag = false;
                            for (int j = 0; j < this.nodes.Length; j++)
                            {
                                if (this.nodes[j].DataType == null)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    this.DataType = SqlType.GetAggregateType(this.DataType, this.nodes[j].DataType);
                                }
                            }
                            if (flag)
                            {
                                for (int k = 0; k < this.nodes.Length; k++)
                                {
                                    if (this.nodes[k].DataType == null)
                                    {
                                        this.nodes[k].DataType = this.DataType;
                                    }
                                }
                            }
                            this.DataType = new ArrayType(this.DataType, this.nodes.Length);
                            return;
                        }
                        break;
                    }
                    QueryExpression queryExpression = this.subQuery.queryExpression;
                    queryExpression.ResolveTypes(session);
                    this.subQuery.PrepareTable(session);
                    this.NodeDataTypes = queryExpression.GetColumnTypes();
                    this.DataType = this.NodeDataTypes[0];
                    if (this.NodeDataTypes.Length > 1)
                    {
                        throw Error.GetError(0x15bc);
                    }
                    this.DataType = new ArrayType(this.DataType, this.nodes.Length);
                    return;
                }
            }
            throw Error.RuntimeError(0xc9, "Expression");
        }

        public void SetAggregate()
        {
            this.isAggregate = true;
        }

        public virtual void SetAlias(QNameManager.SimpleName name)
        {
            this.Alias = name;
        }

        public void SetAsConstantValue(Session session)
        {
            this.ValueData = this.GetValue(session);
            this.OpType = 1;
            this.nodes = emptyArray;
        }

        public void SetAsConstantValue(object value)
        {
            this.ValueData = value;
            this.OpType = 1;
            this.nodes = emptyArray;
        }

        public virtual void SetAttributesAsColumn(ColumnSchema column, bool isWritable)
        {
            throw Error.RuntimeError(0xc9, "Expression");
        }

        public virtual void SetDataType(Session session, SqlType type)
        {
            if (this.OpType == 1)
            {
                this.ValueData = type.ConvertToType(session, this.ValueData, this.DataType);
            }
            this.DataType = type;
        }

        public void SetLeftNode(Expression e)
        {
            this.nodes[0] = e;
        }

        public void SetNullability(byte nullability)
        {
            this.Nullability = nullability;
        }

        public void SetRightNode(Expression e)
        {
            this.nodes[1] = e;
        }

        public virtual void SetSubType(int i)
        {
            this.ExprSubType = i;
        }

        private void SwapLeftAndRightNodes()
        {
            Expression expression = this.nodes[0];
            this.nodes[0] = this.nodes[1];
            this.nodes[1] = expression;
        }

        public virtual bool TestCondition(Session session)
        {
            object obj2 = this.GetValue(session);
            return ((obj2 != null) && ((bool) obj2));
        }

        public virtual object UpdateAggregatingValue(Session session, object currValue)
        {
            throw Error.RuntimeError(0xc9, "Expression");
        }
    }
}

