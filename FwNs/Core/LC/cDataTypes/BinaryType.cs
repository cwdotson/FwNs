namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Text;

    public class BinaryType : SqlType
    {
        private const long MaxBinaryPrecision = 0x7fffffffL;

        public BinaryType(int type, long precision) : base(0x3d, type, precision, 0)
        {
        }

        public override bool AcceptsPrecision()
        {
            return true;
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            if ((othType.TypeCode != 0) && !othType.IsBinaryType())
            {
                return othType.IsCharacterType();
            }
            return true;
        }

        private object CastOrConvertToType(ISessionInterface session, object a, SqlType otherType, bool cast)
        {
            if (a == null)
            {
                return null;
            }
            int typeCode = otherType.TypeCode;
            if (typeCode > 12)
            {
                if ((typeCode != 30) && ((typeCode - 60) > 1))
                {
                    goto Label_005A;
                }
                IBlobData data1 = (IBlobData) a;
                goto Label_0065;
            }
            switch (typeCode)
            {
                case 1:
                case 12:
                {
                    IBlobData data = session.GetScanner().ConvertToBinary((string) a);
                    otherType = GetBinaryType(0x3d, data.Length(session));
                    goto Label_0065;
                }
            }
        Label_005A:
            throw Error.GetError(0xd8f);
        Label_0065:
            if (base.Precision == 0)
            {
                return null;
            }
            IBlobData data2 = null;
            if ((data2.Length(session) > base.Precision) && (data2.NonZeroLength(session) > base.Precision))
            {
                if (!cast)
                {
                    throw Error.GetError(0xd49);
                }
                session.AddWarning(Error.GetError(0x3ec));
            }
            if (otherType.TypeCode == 30)
            {
                long num2 = data2.Length(session);
                if (num2 > base.Precision)
                {
                    throw Error.GetError(0xd8f);
                }
                data2 = new BinaryData(data2.GetBytes(session, 0L, (int) num2), false);
            }
            int num3 = base.TypeCode;
            if (num3 == 60)
            {
                if (data2.Length(session) > base.Precision)
                {
                    return new BinaryData(data2.GetBytes(session, 0L, (int) base.Precision), false);
                }
                if (data2.Length(session) < base.Precision)
                {
                    data2 = new BinaryData(ArrayUtil.ResizeArray<byte>(data2.GetBytes(), (int) base.Precision), false);
                }
                return data2;
            }
            if (num3 != 0x3d)
            {
                throw Error.GetError(0xd8f);
            }
            if (data2.Length(session) > base.Precision)
            {
                data2 = new BinaryData(data2.GetBytes(session, 0L, (int) base.Precision), false);
            }
            return data2;
        }

        public override object CastToType(ISessionInterface session, object a, SqlType otherType)
        {
            return this.CastOrConvertToType(session, a, otherType, true);
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            if (a == b)
            {
                return 0;
            }
            if (a == null)
            {
                return -1;
            }
            if (b != null)
            {
                BinaryData data = b as BinaryData;
                BinaryData data1 = a as BinaryData;
                if ((data1 == null) || (data == null))
                {
                    throw Error.RuntimeError(0xc9, "BinaryType");
                }
                byte[] bytes = data1.GetBytes();
                byte[] buffer2 = data.GetBytes();
                int num2 = (bytes.Length > buffer2.Length) ? buffer2.Length : bytes.Length;
                for (int i = 0; i < num2; i++)
                {
                    if (bytes[i] != buffer2[i])
                    {
                        if (bytes[i] <= buffer2[i])
                        {
                            return -1;
                        }
                        return 1;
                    }
                }
                if (bytes.Length == buffer2.Length)
                {
                    return 0;
                }
                if (bytes.Length <= buffer2.Length)
                {
                    return -1;
                }
            }
            return 1;
        }

        public override object Concat(Session session, object a, object b)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            BinaryData data = a as BinaryData;
            BinaryData data2 = b as BinaryData;
            long length = data.Length(session) + data2.Length(session);
            if (length > base.Precision)
            {
                throw Error.GetError(0xd49);
            }
            if (base.TypeCode == 30)
            {
                BlobDataId id1 = session.CreateBlob(length);
                id1.SetBytes(session, 0L, data2.GetBytes());
                id1.SetBytes(session, data.Length(session), data2.GetBytes());
                return id1;
            }
            return new BinaryData(session, data, data2);
        }

        public override object ConvertCSharpToSQL(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            byte[] data = a as byte[];
            if (data == null)
            {
                throw Error.GetError(0x15b9);
            }
            return new BinaryData(data, true);
        }

        public override object ConvertSQLToCSharp(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            return ((IBlobData) a).GetBytes();
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return a;
            }
            byte[] data = a as byte[];
            if (data != null)
            {
                return new BinaryData(data, false);
            }
            if (a is BinaryData)
            {
                return a;
            }
            if (!(a is string))
            {
                throw Error.GetError(0xd8f);
            }
            return this.CastOrConvertToType(session, a, SqlType.SqlVarchar, false);
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            return StringConverter.ByteArrayToSqlHexString(((BinaryData) a).GetBytes());
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            return StringConverter.ByteArrayToHexString(((IBlobData) a).GetBytes());
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            return this.CastOrConvertToType(session, a, othType, false);
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            return this.CastOrConvertToType(session, a, this, false);
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
            if (base.TypeCode != 60)
            {
                return 0x15;
            }
            return 20;
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            if (base.TypeCode == other.TypeCode)
            {
                if (base.Precision < other.Precision)
                {
                    return other;
                }
                return this;
            }
            if (other.IsCharacterType())
            {
                return other.GetAggregateType(this);
            }
            switch (other.TypeCode)
            {
                case 30:
                    if (other.Precision < base.Precision)
                    {
                        return GetBinaryType(other.TypeCode, base.Precision);
                    }
                    return other;

                case 60:
                    if (base.Precision < other.Precision)
                    {
                        return GetBinaryType(base.TypeCode, other.Precision);
                    }
                    return this;

                case 0x3d:
                    if (base.TypeCode != 30)
                    {
                        if (other.Precision < base.Precision)
                        {
                            return GetBinaryType(other.TypeCode, base.Precision);
                        }
                        return other;
                    }
                    if (base.Precision < other.Precision)
                    {
                        return GetBinaryType(base.TypeCode, other.Precision);
                    }
                    return this;

                case -11:
                    if (base.Precision < 0x10L)
                    {
                        return GetBinaryType(base.TypeCode, 0x10L);
                    }
                    return this;

                case 0:
                    return this;
            }
            throw Error.GetError(0x15ba);
        }

        public static BinaryType GetBinaryType(int type, long precision)
        {
            if (type == 30)
            {
                return new BlobType(precision);
            }
            if ((type - 60) > 1)
            {
                throw Error.RuntimeError(0xc9, "BinaryType");
            }
            return new BinaryType(type, precision);
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            if (operation != 0x24)
            {
                return this.GetAggregateType(other);
            }
            long precision = base.Precision + other.Precision;
            int typeCode = other.TypeCode;
            if (typeCode <= 0)
            {
                switch (typeCode)
                {
                    case -11:
                        return this;

                    case 0:
                        return this;
                }
            }
            else
            {
                SqlType type;
                switch (typeCode)
                {
                    case 30:
                        type = other;
                        break;

                    case 60:
                        type = this;
                        break;

                    case 0x3d:
                        type = (base.TypeCode == 30) ? this : other;
                        break;

                    default:
                        goto Label_0091;
                }
                if (precision > 0x7fffffffL)
                {
                    if (base.TypeCode == 60)
                    {
                        throw Error.GetError(0x15c2);
                    }
                    if (base.TypeCode == 0x3d)
                    {
                        precision = 0x7fffffffL;
                    }
                }
                return GetBinaryType(type.TypeCode, precision);
            }
        Label_0091:
            throw Error.GetError(0x15b9);
        }

        public override Type GetCSharpClass()
        {
            return typeof(byte[]);
        }

        public override string GetCSharpClassName()
        {
            return "System.Byte[]";
        }

        public override string GetDefinition()
        {
            if (base.Precision == 0)
            {
                return this.GetNameString();
            }
            StringBuilder builder1 = new StringBuilder(0x10);
            builder1.Append(this.GetNameString());
            builder1.Append('(');
            builder1.Append(base.Precision);
            builder1.Append(')');
            return builder1.ToString();
        }

        public override long GetMaxPrecision()
        {
            return 0x7fffffffL;
        }

        public string GetNameFullString()
        {
            if (base.TypeCode != 60)
            {
                return "BINARY VARYING";
            }
            return "BINARY";
        }

        public override string GetNameString()
        {
            if (base.TypeCode != 60)
            {
                return "VARBINARY";
            }
            return "BINARY";
        }

        public override bool IsBinaryType()
        {
            return true;
        }

        public virtual IBlobData Overlay(Session session, IBlobData data, IBlobData overlay, long offset, long length, bool hasLength)
        {
            if ((data == null) || (overlay == null))
            {
                return null;
            }
            if (!hasLength)
            {
                length = overlay.Length(session);
            }
            int typeCode = base.TypeCode;
            if (typeCode == 30)
            {
                byte[] bytes = this.Substring(session, data, 0L, offset, false).GetBytes();
                long num2 = (data.Length(session) + overlay.Length(session)) - length;
                IBlobData data3 = session.CreateBlob(num2);
                data3.SetBytes(session, 0L, bytes);
                data3.SetBytes(session, data3.Length(session), overlay.GetBytes());
                bytes = this.Substring(session, data, offset + length, 0L, false).GetBytes();
                data3.SetBytes(session, data3.Length(session), bytes);
                return data3;
            }
            if ((typeCode - 60) > 1)
            {
                throw Error.RuntimeError(0xc9, "BinaryType");
            }
            return new BinaryData(session, new BinaryData(session, this.Substring(session, data, 0L, offset, true), overlay), this.Substring(session, data, offset + length, 0L, false));
        }

        public static long Position(ISessionInterface session, IBlobData data, IBlobData otherData, SqlType otherType, long offset)
        {
            if ((data == null) || (otherData == null))
            {
                return -1L;
            }
            long num2 = data.Length(session);
            if ((offset + num2) > data.Length(session))
            {
                return -1L;
            }
            return data.Position(session, otherData, offset);
        }

        public override int PrecedenceDegree(SqlType other)
        {
            if (other.TypeCode == base.TypeCode)
            {
                return 0;
            }
            if (!other.IsBinaryType())
            {
                return -2147483648;
            }
            switch (base.TypeCode)
            {
                case 30:
                    if (other.TypeCode != 60)
                    {
                        return -2;
                    }
                    return -4;

                case 60:
                    if (other.TypeCode != 30)
                    {
                        return 2;
                    }
                    return 4;

                case 0x3d:
                    if (other.TypeCode != 30)
                    {
                        return 2;
                    }
                    return 4;
            }
            throw Error.RuntimeError(0xc9, "BinaryType");
        }

        public override bool RequiresPrecision()
        {
            return (base.TypeCode == 0x3d);
        }

        public virtual IBlobData Substring(ISessionInterface session, IBlobData data, long offset, long length, bool hasLength)
        {
            long num2;
            long num = data.Length(session);
            if (hasLength)
            {
                num2 = offset + length;
            }
            else
            {
                num2 = (num > offset) ? num : offset;
            }
            if (offset > num2)
            {
                throw Error.GetError(0xd67);
            }
            if ((offset > num2) || (num2 < 0L))
            {
                offset = 0L;
                num2 = 0L;
            }
            if (offset < 0L)
            {
                offset = 0L;
            }
            if (num2 > num)
            {
                num2 = num;
            }
            length = num2 - offset;
            return new BinaryData(data.GetBytes(session, offset, (int) length), false);
        }

        public IBlobData Trim(Session session, IBlobData data, int trim, bool leading, bool trailing)
        {
            if (data == null)
            {
                return null;
            }
            byte[] bytes = data.GetBytes();
            int length = bytes.Length;
            if (trailing)
            {
                length--;
                while ((length >= 0) && (bytes[length] == trim))
                {
                    length--;
                }
                length++;
            }
            int index = 0;
            if (leading)
            {
                while ((index < length) && (bytes[index] == trim))
                {
                    index++;
                }
            }
            byte[] destinationArray = bytes;
            if ((index != 0) || (length != bytes.Length))
            {
                destinationArray = new byte[length - index];
                Array.Copy(bytes, index, destinationArray, 0, length - index);
            }
            if (base.TypeCode == 30)
            {
                BlobDataId id1 = session.CreateBlob((long) destinationArray.Length);
                id1.SetBytes(session, 0L, destinationArray);
                return id1;
            }
            return new BinaryData(destinationArray, destinationArray == bytes);
        }
    }
}

