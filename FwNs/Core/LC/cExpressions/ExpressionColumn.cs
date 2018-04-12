namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public sealed class ExpressionColumn : Expression
    {
        public static ExpressionColumn[] EmptyArray = new ExpressionColumn[0];
        private readonly bool _isParam;
        private readonly bool _strictReference;
        public ColumnSchema column;
        public string ColumnName;
        public bool IsWritable;
        public RangeVariable rangeVariable;
        public string schema;
        public NumberSequence Sequence;
        public string TableName;
        private bool _needConversion;

        public ExpressionColumn() : base(9)
        {
        }

        public ExpressionColumn(NumberSequence sequence) : base(10)
        {
            this.Sequence = sequence;
            base.DataType = sequence.GetDataType();
        }

        public ExpressionColumn(ColumnSchema column) : base(2)
        {
            this.column = column;
            base.DataType = column.GetDataType();
            this.ColumnName = column.GetName().Name;
        }

        public ExpressionColumn(int type) : base(type)
        {
            if (type == 8)
            {
                this._isParam = true;
            }
        }

        public ExpressionColumn(NumberSequence sequence, int opType) : base(opType)
        {
            this.Sequence = sequence;
            base.DataType = sequence.GetDataType();
        }

        public ExpressionColumn(RangeVariable rangeVar, int index) : base(2)
        {
            base.ColumnIndex = index;
            this.SetAutoAttributesAsColumn(rangeVar, base.ColumnIndex);
        }

        public ExpressionColumn(int type, string paramName) : base(type)
        {
            if (type == 0x62)
            {
                this._isParam = true;
                this.ColumnName = paramName;
            }
        }

        public ExpressionColumn(Expression[] nodes, string name) : base(3)
        {
            base.nodes = nodes;
            this.ColumnName = name;
        }

        public ExpressionColumn(string schema, string table) : base(0x61)
        {
            this.schema = schema;
            this.TableName = table;
        }

        public ExpressionColumn(Expression e, int colIndex, int rangePosition) : base(5)
        {
            base.DataType = e.DataType;
            base.ColumnIndex = colIndex;
            base.Alias = e.Alias;
            base.RangePosition = rangePosition;
        }

        public ExpressionColumn(string schema, string table, string column, bool strictReference) : base(2)
        {
            this.schema = schema;
            this.TableName = table;
            this.ColumnName = column;
            this._strictReference = strictReference;
        }

        public static void CheckColumnsResolved(List<Expression> set)
        {
            if ((set != null) && (set.Count != 0))
            {
                Expression expression = set[0];
                StringBuilder builder = new StringBuilder();
                ExpressionColumn column = expression as ExpressionColumn;
                if (column == null)
                {
                    throw Error.GetError(0x157d, builder.ToString());
                }
                if (column.schema != null)
                {
                    builder.Append(column.schema + ".");
                }
                if (column.TableName != null)
                {
                    builder.Append(column.TableName + ".");
                }
                builder.Append(expression.GetColumn());
                throw Error.GetError(0x157d, builder + column.GetColumnName());
            }
        }

        public override void CollectObjectNames(FwNs.Core.LC.cLib.ISet<QNameManager.QName> set)
        {
            int opType = base.OpType;
            switch (opType)
            {
                case 2:
                    set.Add(this.column.GetName());
                    if (this.column.GetName().Parent != null)
                    {
                        set.Add(this.column.GetName().Parent);
                    }
                    return;

                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return;

                case 10:
                    break;

                default:
                    if ((opType - 0x61) <= 1)
                    {
                        return;
                    }
                    if ((opType - 0x6d) > 1)
                    {
                        return;
                    }
                    break;
            }
            QNameManager.QName o = this.Sequence.GetName();
            set.Add(o);
        }

        public override void CollectRangeVariables(RangeVariable[] rangeVariables, FwNs.Core.LC.cLib.ISet<RangeVariable> set)
        {
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i].CollectRangeVariables(rangeVariables, set);
                }
            }
            if (this.rangeVariable != null)
            {
                for (int j = 0; j < rangeVariables.Length; j++)
                {
                    if (rangeVariables[j] == this.rangeVariable)
                    {
                        set.Add(this.rangeVariable);
                        return;
                    }
                }
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
            int opType = base.OpType;
            switch (opType)
            {
                case 2:
                    builder.Append("COLUMN").Append(": ");
                    builder.Append(this.column.GetName().GetSchemaQualifiedStatementName());
                    if (base.Alias != null)
                    {
                        builder.Append(" AS ").Append(base.Alias.Name);
                    }
                    goto Label_0221;

                case 3:
                    builder.Append("COLUMN").Append(": ");
                    builder.Append(this.ColumnName);
                    if (base.Alias != null)
                    {
                        builder.Append(" AS ").Append(base.Alias.Name);
                    }
                    goto Label_0221;

                case 4:
                    builder.Append("DEFAULT");
                    goto Label_0221;

                case 5:
                case 0x61:
                    goto Label_0221;

                case 6:
                    builder.Append("VARIABLE: ");
                    builder.Append(this.column.GetName().Name);
                    goto Label_0221;

                case 7:
                    builder.Append("PARAMETER").Append(": ");
                    builder.Append(this.column.GetName().Name);
                    goto Label_0221;

                case 8:
                    builder.Append("DYNAMIC PARAM: ");
                    builder.Append(", TYPE = ").Append(base.DataType.GetNameString());
                    goto Label_0221;

                case 9:
                    builder.Append("OpTypes.ASTERISK ");
                    goto Label_0221;

                case 10:
                    break;

                case 0x62:
                    builder.Append("NAMED PARAM: ");
                    builder.Append(this.ColumnName);
                    builder.Append(", TYPE = ").Append(base.DataType.GetNameString());
                    goto Label_0221;

                default:
                    if ((opType - 0x6d) > 1)
                    {
                        goto Label_0221;
                    }
                    break;
            }
            builder.Append("SEQUENCE").Append(": ");
            builder.Append(this.Sequence.GetName().Name);
        Label_0221:
            return builder.ToString();
        }

        public override bool Equals(Expression other)
        {
            if (other == this)
            {
                return true;
            }
            if (other != null)
            {
                if (base.OpType != other.OpType)
                {
                    return false;
                }
                switch (base.OpType)
                {
                    case 2:
                        return (this.column == other.GetColumn());

                    case 3:
                        return (base.nodes == other.nodes);

                    case 5:
                        return (base.ColumnIndex == other.ColumnIndex);
                }
            }
            return false;
        }

        public override bool Equals(object other)
        {
            if (other == this)
            {
                return true;
            }
            ExpressionColumn column = other as ExpressionColumn;
            return ((column != null) && this.Equals((Expression) column));
        }

        public override int FindMatchingRangeVariableIndex(RangeVariable[] rangeVarArray)
        {
            for (int i = 0; i < rangeVarArray.Length; i++)
            {
                if (rangeVarArray[i].ResolvesTableName(this))
                {
                    return i;
                }
            }
            return -1;
        }

        public override string GetAlias()
        {
            if (base.Alias != null)
            {
                return base.Alias.Name;
            }
            if (((base.OpType == 2) || (base.OpType == 6)) || ((base.OpType == 7) || (base.OpType == 0x62)))
            {
                return this.ColumnName;
            }
            if (base.OpType == 3)
            {
                return this.ColumnName;
            }
            return "";
        }

        public string GetBaseColumnName()
        {
            if ((base.OpType == 2) && (this.rangeVariable != null))
            {
                return this.rangeVariable.GetTable().GetColumn(base.ColumnIndex).GetName().Name;
            }
            return null;
        }

        public QNameManager.QName GetBaseColumnQName()
        {
            return this.column.GetName();
        }

        public override ColumnSchema GetColumn()
        {
            return this.column;
        }

        public override string GetColumnName()
        {
            if ((base.OpType == 2) && (this.column != null))
            {
                return this.column.GetName().Name;
            }
            return this.GetAlias();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override byte GetNullability()
        {
            if ((base.OpType == 2) && (this.column != null))
            {
                return this.column.GetNullability();
            }
            return base.Nullability;
        }

        public byte GetParameterMode()
        {
            return this.column.ParameterMode;
        }

        public override RangeVariable GetRangeVariable()
        {
            return this.rangeVariable;
        }

        public string GetSchemaName()
        {
            return this.schema;
        }

        public override QNameManager.SimpleName GetSimpleName()
        {
            if (base.Alias != null)
            {
                return base.Alias;
            }
            if (base.OpType == 2)
            {
                return new QNameManager.SimpleName(this.ColumnName, this.ColumnName.ToUpper(CultureInfo.InvariantCulture) != this.ColumnName);
            }
            if (this.column != null)
            {
                return this.column.GetName();
            }
            if (base.OpType == 3)
            {
                return base.nodes[0].GetSimpleName();
            }
            return null;
        }

        public override string GetSql()
        {
            int opType = base.OpType;
            switch (opType)
            {
                case 2:
                case 6:
                case 7:
                    if (this.column != null)
                    {
                        if (this.rangeVariable.TableAlias == null)
                        {
                            return this.column.GetName().GetSchemaQualifiedStatementName();
                        }
                        StringBuilder builder1 = new StringBuilder();
                        builder1.Append(this.rangeVariable.TableAlias.GetStatementName());
                        builder1.Append('.');
                        builder1.Append(this.column.GetName().StatementName);
                        return builder1.ToString();
                    }
                    if (base.Alias == null)
                    {
                        return this.ColumnName;
                    }
                    return base.Alias.GetStatementName();

                case 3:
                    return base.Alias.GetStatementName();

                case 4:
                    return "DEFAULT";

                case 5:
                    goto Label_01C0;

                case 8:
                    return "?";

                case 9:
                    return "*";

                case 10:
                    break;

                default:
                    switch (opType)
                    {
                        case 110:
                        {
                            StringBuilder builder2 = new StringBuilder();
                            builder2.Append(this.Sequence.GetName().GetStatementName());
                            builder2.Append('.');
                            builder2.Append("CURRVAL");
                            return builder2.ToString();
                        }
                        case 0x62:
                            return ("@" + this.ColumnName);

                        case 0x61:
                        {
                            if (base.nodes.Length == 0)
                            {
                                return "*";
                            }
                            StringBuilder builder = new StringBuilder();
                            for (int i = 0; i < base.nodes.Length; i++)
                            {
                                if (i > 0)
                                {
                                    builder.Append(',');
                                }
                                string sql = base.nodes[i].GetSql();
                                builder.Append(sql);
                            }
                            return builder.ToString();
                        }
                    }
                    goto Label_01C0;
            }
            StringBuilder builder3 = new StringBuilder();
            builder3.Append(this.Sequence.GetName().GetStatementName());
            builder3.Append('.');
            builder3.Append("NEXTVAL");
            return builder3.ToString();
        Label_01C0:
            throw Error.RuntimeError(0xc9, "ExpressionColumn");
        }

        public string GetTableName()
        {
            if (base.OpType == 0x61)
            {
                return this.TableName;
            }
            if (base.OpType != 2)
            {
                return "";
            }
            if (this.rangeVariable == null)
            {
                return this.TableName;
            }
            return this.rangeVariable.GetTable().GetName().Name;
        }

        public override OrderedHashSet<Expression> GetUnkeyedColumns(OrderedHashSet<Expression> unresolvedSet)
        {
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    unresolvedSet = base.nodes[i].GetUnkeyedColumns(unresolvedSet);
                }
            }
            if ((base.OpType == 2) && !this.rangeVariable.HasKeyedColumnInGroupBy)
            {
                if (unresolvedSet == null)
                {
                    unresolvedSet = new OrderedHashSet<Expression>();
                }
                unresolvedSet.Add(this);
            }
            return unresolvedSet;
        }

        public override object GetValue(Session session)
        {
            int opType = base.OpType;
            switch (opType)
            {
                case 2:
                {
                    object a = session.sessionContext.RangeIterators[this.rangeVariable.RangePosition].GetCurrent()[base.ColumnIndex];
                    if (this._needConversion)
                    {
                        a = base.DataType.ConvertToType(session, a, this.column.DataType);
                    }
                    return a;
                }
                case 3:
                {
                    object obj3 = null;
                    for (int i = 0; i < base.nodes.Length; i++)
                    {
                        obj3 = base.nodes[i].GetValue(session, base.DataType);
                        if (obj3 != null)
                        {
                            return obj3;
                        }
                    }
                    return obj3;
                }
                case 4:
                    return null;

                case 5:
                    return session.sessionContext.RangeIterators[base.RangePosition].GetCurrent()[base.ColumnIndex];

                case 6:
                    return session.sessionContext.RoutineVariables[base.ColumnIndex];

                case 7:
                    return session.sessionContext.RoutineArguments[base.ColumnIndex];

                case 8:
                    break;

                case 9:
                    goto Label_0185;

                case 10:
                    goto Label_0173;

                default:
                    switch (opType)
                    {
                        case 0x61:
                            goto Label_0185;

                        case 0x62:
                        {
                            int index = session.sessionContext.SessionVariablesRange[0].FindColumn(this.ColumnName);
                            if (index != -1)
                            {
                                return session.sessionContext.SessionVariableValues[index];
                            }
                            goto Label_013B;
                        }
                    }
                    if ((opType - 0x6d) <= 1)
                    {
                        goto Label_0173;
                    }
                    goto Label_0185;
            }
        Label_013B:
            if (base.ParameterIndex >= session.sessionContext.DynamicArguments.Length)
            {
                throw Error.RuntimeError(0xc9, "ExpressionColumn");
            }
            return session.sessionContext.DynamicArguments[base.ParameterIndex];
        Label_0173:
            return session.sessionData.GetSequenceValue(this.Sequence);
        Label_0185:
            throw Error.RuntimeError(0xc9, "ExpressionColumn");
        }

        public override bool HasReference(RangeVariable range)
        {
            if (range == this.rangeVariable)
            {
                return true;
            }
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if ((base.nodes[i] != null) && base.nodes[i].HasReference(range))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsDynamicParam()
        {
            return this._isParam;
        }

        public override bool IsIndexable(RangeVariable range)
        {
            return ((base.OpType == 2) && (this.rangeVariable == range));
        }

        public override bool IsParameter()
        {
            return (base.OpType == 7);
        }

        public override bool IsUnresolvedParam()
        {
            return (this._isParam && (base.DataType == null));
        }

        public override Expression ReplaceAliasInOrderBy(Expression[] columns, int length)
        {
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i] = base.nodes[i].ReplaceAliasInOrderBy(columns, length);
                }
            }
            if ((base.OpType - 2) <= 1)
            {
                for (int j = 0; j < length; j++)
                {
                    QNameManager.SimpleName alias = columns[j].Alias;
                    string str = (alias == null) ? null : alias.Name;
                    if (((this.schema == null) && (this.TableName == null)) && this.ColumnName.Equals(str))
                    {
                        return columns[j];
                    }
                }
                for (int k = 0; k < length; k++)
                {
                    if (columns[k] is ExpressionColumn)
                    {
                        if (this.Equals(columns[k]))
                        {
                            return columns[k];
                        }
                        if (((this.TableName == null) && (this.schema == null)) && this.ColumnName.Equals(((ExpressionColumn) columns[k]).ColumnName))
                        {
                            return columns[k];
                        }
                    }
                }
            }
            return this;
        }

        public override Expression ReplaceColumnReferences(RangeVariable range, Expression[] list)
        {
            if ((base.OpType == 2) && (this.rangeVariable == range))
            {
                return list[base.ColumnIndex];
            }
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i] = base.nodes[i].ReplaceColumnReferences(range, list);
                }
            }
            return this;
        }

        public override void ResetColumnReferences()
        {
            this.rangeVariable = null;
            base.ColumnIndex = -1;
            this.TableName = null;
        }

        public bool ResolveColumnReference(RangeVariable rangeVar)
        {
            if ((this.TableName == null) || rangeVar.IsVariable)
            {
                Expression columnExpression = rangeVar.GetColumnExpression(this.ColumnName);
                if (columnExpression != null)
                {
                    base.OpType = columnExpression.OpType;
                    base.nodes = columnExpression.nodes;
                    base.DataType = columnExpression.DataType;
                    return true;
                }
                if (rangeVar.Variables != null)
                {
                    string columnName = (this.TableName != null) ? string.Format("{0}.{1}", this.TableName, this.ColumnName) : this.ColumnName;
                    int i = rangeVar.FindColumn(columnName);
                    if (i == -1)
                    {
                        return false;
                    }
                    if (rangeVar.GetColumn(i).GetParameterMode() == 4)
                    {
                        return false;
                    }
                    base.OpType = rangeVar.IsVariable ? 6 : 7;
                    this.SetAttributesAsColumn(rangeVar, i);
                    return true;
                }
            }
            if (rangeVar.ResolvesTableName(this))
            {
                int i = rangeVar.FindColumn(this.ColumnName);
                if (i != -1)
                {
                    this.SetAttributesAsColumn(rangeVar, i);
                    return true;
                }
            }
            return false;
        }

        public override List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            int opType = base.OpType;
            switch (opType)
            {
                case 2:
                case 6:
                case 7:
                {
                    bool flag = false;
                    bool flag2 = this.TableName > null;
                    if (this.rangeVariable == null)
                    {
                        for (int i = 0; i < rangeCount; i++)
                        {
                            RangeVariable rangeVar = rangeVarArray[i];
                            if (rangeVar != null)
                            {
                                if (flag)
                                {
                                    if (this.ResolvesDuplicateColumnReference(rangeVar) && this._strictReference)
                                    {
                                        string columnName = this.GetColumnName();
                                        if (base.Alias != null)
                                        {
                                            StringBuilder builder1 = new StringBuilder(columnName);
                                            builder1.Append(' ').Append("AS").Append(' ').Append(base.Alias.GetStatementName());
                                            columnName = builder1.ToString();
                                        }
                                        throw Error.GetError(0x15cc, columnName);
                                    }
                                }
                                else if (this.ResolveColumnReference(rangeVar))
                                {
                                    if (flag2)
                                    {
                                        return unresolvedSet;
                                    }
                                    flag = true;
                                }
                            }
                        }
                        if (!flag)
                        {
                            if (unresolvedSet == null)
                            {
                                unresolvedSet = new List<Expression>();
                            }
                            unresolvedSet.Add(this);
                        }
                        return unresolvedSet;
                    }
                    return unresolvedSet;
                }
                case 3:
                case 4:
                case 5:
                case 8:
                case 9:
                    return unresolvedSet;

                case 10:
                    break;

                default:
                    if ((opType - 0x61) <= 1)
                    {
                        return unresolvedSet;
                    }
                    if ((opType - 0x6d) > 1)
                    {
                        return unresolvedSet;
                    }
                    break;
            }
            if (!acceptsSequences)
            {
                throw Error.GetError(0x15de);
            }
            return unresolvedSet;
        }

        private bool ResolvesDuplicateColumnReference(RangeVariable rangeVar)
        {
            if (this.TableName == null)
            {
                if (rangeVar.GetColumnExpression(this.ColumnName) != null)
                {
                    return false;
                }
                if (rangeVar.Variables != null)
                {
                    int i = rangeVar.FindColumn(this.ColumnName);
                    if (i == -1)
                    {
                        return false;
                    }
                    return (rangeVar.GetColumn(i).GetParameterMode() != 4);
                }
            }
            if (!rangeVar.ResolvesTableName(this))
            {
                return false;
            }
            return (rangeVar.FindColumn(this.ColumnName) != -1);
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            int opType = base.OpType;
            switch (opType)
            {
                case 2:
                    this._needConversion = !base.DataType.Equals(this.column.DataType);
                    return;

                case 3:
                {
                    SqlType existing = null;
                    for (int i = 0; i < base.nodes.Length; i++)
                    {
                        existing = SqlType.GetAggregateType(base.nodes[i].DataType, existing);
                    }
                    base.DataType = existing;
                    return;
                }
                case 4:
                    if ((parent != null) && (parent.OpType != 0x19))
                    {
                        throw Error.GetError(0x15a8);
                    }
                    break;

                default:
                    if ((opType == 0x62) && session.sessionContext.SessionVariables.ContainsKey(this.ColumnName))
                    {
                        ColumnSchema schema = session.sessionContext.SessionVariables.Get(this.ColumnName);
                        base.DataType = schema.DataType;
                    }
                    break;
            }
        }

        public override void SetAttributesAsColumn(ColumnSchema column, bool isWritable)
        {
            this.column = column;
            base.DataType = column.GetDataType();
            this.IsWritable = isWritable;
        }

        public void SetAttributesAsColumn(RangeVariable range, int i)
        {
            if (range.Variables != null)
            {
                base.ColumnIndex = i;
                this.column = range.GetColumn(i);
                base.DataType = this.column.GetDataType();
                this.rangeVariable = range;
            }
            else
            {
                base.ColumnIndex = i;
                this.column = range.GetColumn(i);
                base.DataType = this.column.GetDataType();
                this.rangeVariable = range;
                this.rangeVariable.AddColumn(base.ColumnIndex);
            }
        }

        private void SetAutoAttributesAsColumn(RangeVariable range, int i)
        {
            base.ColumnIndex = i;
            this.column = range.GetColumn(i);
            base.DataType = this.column.GetDataType();
            this.ColumnName = range.GetColumnAlias(i);
            this.TableName = range.GetTableAlias();
            this.rangeVariable = range;
            this.rangeVariable.AddColumn(base.ColumnIndex);
        }

        public override void SetDataType(Session session, SqlType type)
        {
            base.SetDataType(session, type);
            if (base.OpType == 2)
            {
                this._needConversion = !base.DataType.Equals(this.column.DataType);
            }
        }
    }
}

