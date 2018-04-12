namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;

    public sealed class NullType : SqlType
    {
        public static readonly NullType nullType = new NullType();

        private NullType() : base(0, 0, 0L, 0)
        {
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            return true;
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            throw Error.RuntimeError(0xc9, "NullType");
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            return null;
        }

        public override string ConvertToSQLString(object a)
        {
            throw Error.RuntimeError(0xc9, "NullType");
        }

        public override string ConvertToString(object a)
        {
            throw Error.RuntimeError(0xc9, "NullType");
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            return null;
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            return null;
        }

        public override int DisplaySize()
        {
            return 4;
        }

        public override int GetAdoTypeCode()
        {
            return 0;
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            return other;
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            return other;
        }

        public override Type GetCSharpClass()
        {
            return typeof(object);
        }

        public override string GetCSharpClassName()
        {
            return "System.Void";
        }

        public override string GetDefinition()
        {
            return "NULL";
        }

        public override string GetNameString()
        {
            return "NULL";
        }

        public static SqlType GetNullType()
        {
            return nullType;
        }
    }
}

