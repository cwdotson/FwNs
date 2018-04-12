namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;

    public sealed class OtherType : SqlType
    {
        private static readonly OtherType otherType = new OtherType();

        private OtherType() : base(0x457, 0x457, 0L, 0)
        {
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            if (othType.TypeCode != base.TypeCode)
            {
                return (othType.TypeCode == 0);
            }
            return true;
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            if (a == null)
            {
                return -1;
            }
            if (b == null)
            {
                return 1;
            }
            return 0;
        }

        public override object ConvertSQLToCSharp(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            return ((OtherData) a).GetObject();
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            if (!a.GetType().IsSerializable)
            {
                throw Error.GetError(0x15b9);
            }
            return a;
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            return StringConverter.ByteArrayToSqlHexString(((OtherData) a).GetBytes());
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            return StringConverter.ByteArrayToHexString(((OtherData) a).GetBytes());
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            object obj2 = ((OtherData) a).GetObject();
            if (obj2 == null)
            {
                return null;
            }
            int typeCode = othType.TypeCode;
            if (typeCode <= 8)
            {
                switch (typeCode)
                {
                    case -11:
                        return SqlType.SqlBoolean.ConvertSQLToCSharp(session, obj2);

                    case -6:
                        return SqlType.Tinyint.ConvertSQLToCSharp(session, obj2);

                    case -2:
                        return SqlType.SqlVarchar.ConvertSQLToCSharp(session, obj2);

                    case -1:
                    case 0:
                    case 2:
                    case 3:
                    case 7:
                        return a;

                    case 1:
                        return SqlType.SqlChar.ConvertSQLToCSharp(session, obj2);

                    case 4:
                        return SqlType.SqlInteger.ConvertSQLToCSharp(session, obj2);

                    case 5:
                        return SqlType.SqlSmallint.ConvertSQLToCSharp(session, obj2);

                    case 6:
                        return SqlType.SqlDouble.ConvertSQLToCSharp(session, obj2);

                    case 8:
                        return SqlType.SqlDouble.ConvertSQLToCSharp(session, obj2);
                }
                return a;
            }
            if (typeCode <= 0x10)
            {
                switch (typeCode)
                {
                    case 12:
                        return SqlType.SqlVarchar.ConvertSQLToCSharp(session, obj2);

                    case 0x10:
                        return SqlType.SqlBoolean.ConvertSQLToCSharp(session, obj2);
                }
                return a;
            }
            switch (typeCode)
            {
                case 0x5b:
                    return SqlType.SqlDate.ConvertSQLToCSharp(session, obj2);

                case 0x5c:
                    return SqlType.SqlTime.ConvertSQLToCSharp(session, obj2);

                case 0x5d:
                    return SqlType.SqlTimestamp.ConvertSQLToCSharp(session, obj2);

                case 0x19:
                    return SqlType.SqlBigint.ConvertSQLToCSharp(session, obj2);
            }
            return a;
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            return a;
        }

        public override int DisplaySize()
        {
            if (base.Precision <= 0x7fffffffL)
            {
                return (int) base.Precision;
            }
            return 0x7fffffff;
        }

        public override int GetAdoTypeCode()
        {
            return 12;
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            if ((base.TypeCode != other.TypeCode) && (other != SqlType.SqlAllTypes))
            {
                throw Error.GetError(0x15ba);
            }
            return this;
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            return this;
        }

        public override Type GetCSharpClass()
        {
            return typeof(object);
        }

        public override string GetCSharpClassName()
        {
            return "System.Object";
        }

        public override string GetDefinition()
        {
            return "OTHER";
        }

        public override string GetNameString()
        {
            return "OTHER";
        }

        public static OtherType GetOtherType()
        {
            return otherType;
        }

        public override int GetSqlGenericTypeCode()
        {
            return base.TypeCode;
        }

        public override bool IsObjectType()
        {
            return true;
        }
    }
}

