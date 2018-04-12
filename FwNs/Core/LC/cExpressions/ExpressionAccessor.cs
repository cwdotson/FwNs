namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class ExpressionAccessor : Expression
    {
        public ExpressionAccessor(Expression left, Expression right) : base(0x69)
        {
            base.nodes = new Expression[] { left, right };
        }

        public override string Describe(Session session, int blanks)
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append('\n');
            for (int i = 0; i < blanks; i++)
            {
                builder.Append(' ');
            }
            builder.Append("ARRAY ACCESS");
            if (base.GetLeftNode() != null)
            {
                builder.Append(" array=[");
                builder.Append(base.nodes[0].Describe(session, blanks + 1));
                builder.Append(']');
            }
            if (base.GetRightNode() != null)
            {
                builder.Append(" array_index=[");
                builder.Append(base.nodes[1].Describe(session, blanks + 1));
                builder.Append(']');
            }
            return builder.ToString();
        }

        public override ColumnSchema GetColumn()
        {
            return base.nodes[0].GetColumn();
        }

        public override string GetSql()
        {
            string contextSql = Expression.GetContextSql(base.nodes[0]);
            StringBuilder builder1 = new StringBuilder(0x40);
            builder1.Append(contextSql).Append('[');
            builder1.Append(base.nodes[1].GetSql()).Append(']');
            return builder1.ToString();
        }

        public object[] GetUpdatedArray(Session session, object[] array, object value, bool copy)
        {
            if (array == null)
            {
                throw Error.GetError(0xd55);
            }
            object obj1 = base.nodes[1].GetValue(session);
            if (obj1 == null)
            {
                throw Error.GetError(0xda2);
            }
            int index = ((int) obj1) - 1;
            if (index < 0)
            {
                throw Error.GetError(0xda2);
            }
            if (index >= base.nodes[0].DataType.ArrayLimitCardinality())
            {
                throw Error.GetError(0xda2);
            }
            object[] destinationArray = array;
            if (index >= array.Length)
            {
                destinationArray = new object[index + 1];
                Array.Copy(array, 0, destinationArray, 0, array.Length);
            }
            else if (copy)
            {
                destinationArray = new object[array.Length];
                Array.Copy(array, 0, destinationArray, 0, array.Length);
            }
            destinationArray[index] = value;
            return destinationArray;
        }

        public override object GetValue(Session session)
        {
            object[] objArray = (object[]) base.nodes[0].GetValue(session);
            if (objArray == null)
            {
                return null;
            }
            object obj3 = base.nodes[1].GetValue(session);
            if (obj3 == null)
            {
                return null;
            }
            int num = (int) obj3;
            if ((num < 1) || (num > objArray.Length))
            {
                throw Error.GetError(0xda2);
            }
            return objArray[num - 1];
        }

        public override List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    unresolvedSet = base.nodes[i].ResolveColumnReferences(rangeVarArray, rangeCount, unresolvedSet, acceptsSequences);
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
            if (base.nodes[0].DataType == null)
            {
                throw Error.GetError(0x15bf);
            }
            if (!base.nodes[0].DataType.IsArrayType())
            {
                throw Error.GetError(0x15bb);
            }
            base.DataType = base.nodes[0].DataType.CollectionBaseType();
            if (base.nodes[1].OpType == 8)
            {
                base.nodes[1].DataType = SqlType.SqlInteger;
            }
        }
    }
}

