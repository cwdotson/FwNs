namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Text;

    public sealed class ExpressionValue : Expression
    {
        public ExpressionValue(object o, SqlType datatype) : base(1)
        {
            base.nodes = Expression.emptyArray;
            base.DataType = datatype;
            base.ValueData = o;
        }

        public override string Describe(Session session, int blanks)
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append('\n');
            for (int i = 0; i < blanks; i++)
            {
                builder.Append(' ');
            }
            if (base.OpType != 1)
            {
                throw Error.RuntimeError(0xc9, "ExpressionValue");
            }
            builder.Append("VALUE = ").Append(base.ValueData);
            builder.Append(", TYPE = ").Append(base.DataType.GetNameString());
            return builder.ToString();
        }

        public override string GetSql()
        {
            if (base.OpType != 1)
            {
                throw Error.RuntimeError(0xc9, "ExpressionValue");
            }
            if (base.ValueData == null)
            {
                return "NULL";
            }
            return base.DataType.ConvertToSQLString(base.ValueData);
        }

        public override object GetValue(Session session)
        {
            return base.ValueData;
        }

        public override object GetValue(Session session, SqlType type)
        {
            if ((base.DataType == type) || (base.ValueData == null))
            {
                return base.ValueData;
            }
            return type.ConvertToType(session, base.ValueData, base.DataType);
        }

        public override bool TestCondition(Session session)
        {
            return (((base.DataType.TypeCode == 0x10) && (base.ValueData != null)) && ((bool) base.ValueData));
        }
    }
}

