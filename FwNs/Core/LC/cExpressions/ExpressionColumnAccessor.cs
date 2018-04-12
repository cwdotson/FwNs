namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public sealed class ExpressionColumnAccessor : Expression
    {
        public ColumnSchema column;

        public ExpressionColumnAccessor(ColumnSchema column) : base(2)
        {
            this.column = column;
            base.DataType = column.GetDataType();
        }

        public override void CollectObjectNames(FwNs.Core.LC.cLib.ISet<QNameManager.QName> set)
        {
            set.Add(this.column.GetName());
            if (this.column.GetName().Parent != null)
            {
                set.Add(this.column.GetName().Parent);
            }
        }

        public override void CollectRangeVariables(RangeVariable[] rangeVariables, FwNs.Core.LC.cLib.ISet<RangeVariable> set)
        {
        }

        public override string Describe(Session session, int blanks)
        {
            return this.column.GetName().Name;
        }

        public override bool Equals(Expression other)
        {
            return ((other == this) || (((other != null) && (base.OpType == other.OpType)) && (this.column == other.GetColumn())));
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }
            ExpressionColumnAccessor accessor = other as ExpressionColumnAccessor;
            return ((accessor != null) && this.Equals((Expression) accessor));
        }

        public override int FindMatchingRangeVariableIndex(RangeVariable[] rangeVarArray)
        {
            return -1;
        }

        public override string GetAlias()
        {
            return this.column.GetNameString();
        }

        public override ColumnSchema GetColumn()
        {
            return this.column;
        }

        public override string GetColumnName()
        {
            return this.column.GetNameString();
        }

        public override SqlType GetDataType()
        {
            return this.column.GetDataType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override RangeVariable GetRangeVariable()
        {
            return null;
        }

        public override string GetSql()
        {
            return this.column.GetName().StatementName;
        }

        public override OrderedHashSet<Expression> GetUnkeyedColumns(OrderedHashSet<Expression> unresolvedSet)
        {
            return unresolvedSet;
        }

        public override object GetValue(Session session)
        {
            return null;
        }

        public override bool HasReference(RangeVariable range)
        {
            return false;
        }

        public override bool IsDynamicParam()
        {
            return false;
        }

        public override bool IsIndexable(RangeVariable range)
        {
            return false;
        }

        public override bool IsUnresolvedParam()
        {
            return false;
        }

        public override Expression ReplaceAliasInOrderBy(Expression[] columns, int length)
        {
            return this;
        }

        public override Expression ReplaceColumnReferences(RangeVariable range, Expression[] list)
        {
            return this;
        }

        public override void ReplaceRangeVariables(RangeVariable[] ranges, RangeVariable[] newRanges)
        {
        }

        public override void ResetColumnReferences()
        {
        }

        public override List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            return unresolvedSet;
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
        }
    }
}

