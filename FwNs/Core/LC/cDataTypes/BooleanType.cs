namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;

    public sealed class BooleanType : SqlType
    {
        private static readonly BooleanType booleanType = new BooleanType();

        private BooleanType() : base(0x10, 0x10, 0L, 0)
        {
        }

        public override bool CanBeAssignedFrom(SqlType otherType)
        {
            if (otherType != null)
            {
                return this.CanConvertFrom(otherType);
            }
            return true;
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            if (((othType.TypeCode != 0) && !othType.IsBooleanType()) && !othType.IsCharacterType())
            {
                return othType.IsIntegralType();
            }
            return true;
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            if (a == b)
            {
                return 0;
            }
            if (a != null)
            {
                if (b == null)
                {
                    return 1;
                }
                bool flag = (bool) b;
                if (((bool) a) == flag)
                {
                    return 0;
                }
                if (!flag)
                {
                    return 1;
                }
            }
            return -1;
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            if (a is bool)
            {
                return a;
            }
            if (!(a is string))
            {
                throw Error.GetError(0x15b9);
            }
            return this.ConvertToType(session, a, SqlType.SqlVarchar);
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "UNKNOWN";
            }
            if (!((bool) a))
            {
                return "FALSE";
            }
            return "TRUE";
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            if (!((bool) a))
            {
                return "FALSE";
            }
            return "TRUE";
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            if (a == null)
            {
                return a;
            }
            int typeCode = othType.TypeCode;
            if (typeCode <= 12)
            {
                switch (typeCode)
                {
                    case 1:
                    case 12:
                        goto Label_0088;

                    case 2:
                    case 3:
                        return !NumberType.IsZero(a);

                    case 4:
                    case 5:
                    case -6:
                        goto Label_016B;
                }
                goto Label_0181;
            }
            if (typeCode <= 0x19)
            {
                switch (typeCode)
                {
                    case 0x10:
                        return a;

                    case 0x19:
                        goto Label_016B;
                }
                goto Label_0181;
            }
            if (typeCode != 40)
            {
                if (typeCode == 100)
                {
                    goto Label_0088;
                }
                goto Label_0181;
            }
            a = SqlType.SqlVarchar.ConvertToType(session, a, othType);
        Label_0088:
            a = ((CharacterType) othType).Trim(session, a, " ", true, true);
            string str = (string) a;
            if (((str.Equals("TRUE", StringComparison.OrdinalIgnoreCase) || str.Equals("ON", StringComparison.OrdinalIgnoreCase)) || (str.Equals("T", StringComparison.OrdinalIgnoreCase) || str.Equals("YES", StringComparison.OrdinalIgnoreCase))) || (str.Equals("1", StringComparison.OrdinalIgnoreCase) || str.Equals("Y", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            if (((str.Equals("FALSE", StringComparison.OrdinalIgnoreCase) || str.Equals("OFF", StringComparison.OrdinalIgnoreCase)) || (str.Equals("F", StringComparison.OrdinalIgnoreCase) || str.Equals("NO", StringComparison.OrdinalIgnoreCase))) || (str.Equals("0", StringComparison.OrdinalIgnoreCase) || str.Equals("N", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            if (!str.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase))
            {
                goto Label_0181;
            }
            return null;
        Label_016B:
            if (Convert.ToInt64(a) == 0)
            {
                return false;
            }
            return true;
        Label_0181:
            throw Error.GetError(0xd6e);
        }

        public override object ConvertToTypeAdo(ISessionInterface session, object a, SqlType otherType)
        {
            if (a == null)
            {
                return a;
            }
            if (otherType.TypeCode == 0x10)
            {
                return a;
            }
            if (otherType.IsLobType())
            {
                throw Error.GetError(0x15b9);
            }
            if (otherType.IsCharacterType())
            {
                if ("0".Equals(a))
                {
                    return true;
                }
                if ("1".Equals(a))
                {
                    return false;
                }
            }
            return this.ConvertToType(session, a, otherType);
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            return a;
        }

        public override int DisplaySize()
        {
            return 5;
        }

        public override int GetAdoTypeCode()
        {
            return 1;
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            if (base.TypeCode == other.TypeCode)
            {
                return this;
            }
            if (!other.IsCharacterType() && !other.IsNumberType())
            {
                throw Error.GetError(0x15ba);
            }
            return other.GetAggregateType(this);
        }

        public static BooleanType GetBooleanType()
        {
            return booleanType;
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            if ((operation != 0x29) || !other.IsBooleanType())
            {
                throw Error.GetError(0x15ba);
            }
            return this;
        }

        public override Type GetCSharpClass()
        {
            return typeof(byte[]);
        }

        public override string GetCSharpClassName()
        {
            return "System.Boolean";
        }

        public override string GetDefinition()
        {
            return "BOOLEAN";
        }

        public override string GetNameString()
        {
            return "BOOLEAN";
        }

        public override bool IsBooleanType()
        {
            return true;
        }
    }
}

