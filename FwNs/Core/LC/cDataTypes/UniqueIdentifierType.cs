namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;

    public sealed class UniqueIdentifierType : SqlType
    {
        private static readonly UniqueIdentifierType uniqueIdentifierType = new UniqueIdentifierType();

        private UniqueIdentifierType() : base(-11, -11, 0L, 0)
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
            return ((((othType.TypeCode == 0) || othType.IsGuidType()) || othType.IsCharacterType()) || (othType.IsBinaryType() && (othType.Precision == 0x10L)));
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            if (a != b)
            {
                if (a == null)
                {
                    return -1;
                }
                if (b == null)
                {
                    return 1;
                }
                Guid guid = (Guid) a;
                Guid guid2 = (Guid) b;
                byte[] buffer = guid.ToByteArray();
                byte[] buffer2 = guid2.ToByteArray();
                int num2 = buffer[10].CompareTo(buffer2[10]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[11].CompareTo(buffer2[11]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[12].CompareTo(buffer2[12]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[13].CompareTo(buffer2[13]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[14].CompareTo(buffer2[14]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[15].CompareTo(buffer2[15]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[8].CompareTo(buffer2[8]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[9].CompareTo(buffer2[9]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[6].CompareTo(buffer2[6]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[7].CompareTo(buffer2[7]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[4].CompareTo(buffer2[4]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[5].CompareTo(buffer2[5]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[0].CompareTo(buffer2[0]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[1].CompareTo(buffer2[1]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[2].CompareTo(buffer2[2]);
                if (num2 != 0)
                {
                    return num2;
                }
                num2 = buffer[3].CompareTo(buffer2[3]);
                if (num2 != 0)
                {
                    return num2;
                }
            }
            return 0;
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            if (a is Guid)
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
                return "NULL";
            }
            Guid guid = (Guid) a;
            return guid.ToString();
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            Guid guid = (Guid) a;
            return guid.ToString();
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            byte[] buffer;
            if (a == null)
            {
                return a;
            }
            int typeCode = othType.TypeCode;
            if (typeCode <= 12)
            {
                switch (typeCode)
                {
                    case -11:
                        return a;

                    case 1:
                    case 12:
                        goto Label_0049;
                }
                goto Label_00B3;
            }
            if (typeCode != 40)
            {
                if ((typeCode - 60) <= 1)
                {
                    goto Label_0083;
                }
                if (typeCode == 100)
                {
                    goto Label_0049;
                }
                goto Label_00B3;
            }
            a = SqlType.SqlVarchar.ConvertToType(session, a, othType);
        Label_0049:
            a = ((CharacterType) othType).Trim(session, a, " ", true, true);
            try
            {
                return new Guid((string) a);
            }
            catch (Exception)
            {
                throw Error.RuntimeError(0xc9, "UniqueIdentifier Type");
            }
        Label_0083:
            buffer = (byte[]) a;
            if (buffer.Length != 0x10)
            {
                return null;
            }
            try
            {
                return new Guid(buffer);
            }
            catch (Exception)
            {
                throw Error.RuntimeError(0xc9, "UniqueIdentifier Type");
            }
        Label_00B3:
            throw Error.GetError(0xd6e);
        }

        public override object ConvertToTypeAdo(ISessionInterface session, object a, SqlType otherType)
        {
            if (a == null)
            {
                return a;
            }
            if (otherType.TypeCode == -11)
            {
                return a;
            }
            if (otherType.IsLobType())
            {
                throw Error.GetError(0x15b9);
            }
            return this.ConvertToType(session, a, otherType);
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            return a;
        }

        public override int DisplaySize()
        {
            return 0x26;
        }

        public override int GetAdoTypeCode()
        {
            return 11;
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            if (base.TypeCode == other.TypeCode)
            {
                return this;
            }
            if (!other.IsCharacterType() && !other.IsBinaryType())
            {
                throw Error.GetError(0x15ba);
            }
            return other.GetAggregateType(this);
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            if ((operation != 0x29) || !other.IsGuidType())
            {
                throw Error.GetError(0x15ba);
            }
            return this;
        }

        public override Type GetCSharpClass()
        {
            return typeof(Guid);
        }

        public override string GetCSharpClassName()
        {
            return "System.Guid";
        }

        public override string GetDefinition()
        {
            return "UNIQUEIDENTIFIER";
        }

        public override string GetNameString()
        {
            return "UNIQUEIDENTIFIER";
        }

        public static UniqueIdentifierType GetUniqueIdentifierType()
        {
            return uniqueIdentifierType;
        }

        public override bool IsGuidType()
        {
            return true;
        }
    }
}

