namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Text;

    public class CharacterType : SqlType
    {
        public const int DefaultCharPrecision = 0x8000;
        public const long MaxCharPrecision = 0x7fffffffL;
        private readonly bool _isEqualIdentical;
        private readonly Charset _charset;
        private readonly Collation _collation;

        public CharacterType(int type, long precision) : base(12, type, precision, 0)
        {
            this._collation = Collation.GetDefaultInstance();
            this._charset = Charset.GetDefaultInstance();
            this._isEqualIdentical = type != 100;
        }

        public CharacterType(Collation collation, int type, long precision) : base(12, type, precision, 0)
        {
            this._collation = collation;
            this._charset = Charset.GetDefaultInstance();
            this._isEqualIdentical = this._collation.IsEqualAlwaysIdentical() && (type != 100);
        }

        public override bool AcceptsPrecision()
        {
            return true;
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            return !othType.IsObjectType();
        }

        public object CastOrConvertToType(ISessionInterface session, object a, SqlType otherType, bool cast)
        {
            switch (otherType.TypeCode)
            {
                case 40:
                {
                    IClobData data = (IClobData) a;
                    long precision = data.Length(session);
                    if ((base.Precision != 0) && (precision > base.Precision))
                    {
                        if (data.NonSpaceLength(session) > base.Precision)
                        {
                            if (!cast)
                            {
                                throw Error.GetError(0xd49);
                            }
                            session.AddWarning(Error.GetError(0x3ec));
                        }
                        precision = base.Precision;
                    }
                    switch (base.TypeCode)
                    {
                        case 40:
                            if ((base.Precision != 0) && (precision > base.Precision))
                            {
                                return data.GetClob(session, 0L, base.Precision);
                            }
                            return a;

                        case 100:
                        case 1:
                        case 12:
                            if (precision > 0x7fffffffL)
                            {
                                if (!cast)
                                {
                                    throw Error.GetError(0xd49);
                                }
                                precision = 0x7fffffffL;
                            }
                            a = data.GetSubString(session, 0L, (int) precision);
                            return this.ConvertToTypeLimits(session, a);
                    }
                    throw Error.RuntimeError(0xc9, "CharacterType");
                }
                case 100:
                case 1:
                case 12:
                {
                    string s = (string) a;
                    int length = s.Length;
                    if ((base.Precision != 0) && (length > base.Precision))
                    {
                        if (StringUtil.RightTrimSize(s) > base.Precision)
                        {
                            if (!cast)
                            {
                                throw Error.GetError(0xd49);
                            }
                            session.AddWarning(Error.GetError(0x3ec));
                        }
                        a = s.Substring(0, (int) base.Precision);
                    }
                    int typeCode = base.TypeCode;
                    if (typeCode <= 12)
                    {
                        switch (typeCode)
                        {
                            case 1:
                                return this.ConvertToTypeLimits(session, a);

                            case 12:
                                return a;
                        }
                        break;
                    }
                    if (typeCode == 40)
                    {
                        ClobDataId id1 = session.CreateClob((long) s.Length);
                        id1.SetString(session, 0L, s);
                        return id1;
                    }
                    if (typeCode != 100)
                    {
                        break;
                    }
                    return a;
                }
                case 0x457:
                    throw Error.GetError(0x15b9);

                default:
                {
                    string str2 = otherType.ConvertToString(a);
                    if ((base.Precision != 0) && (str2.Length > base.Precision))
                    {
                        throw Error.GetError(0xd49);
                    }
                    a = str2;
                    return this.ConvertToTypeLimits(session, a);
                }
            }
            throw Error.RuntimeError(0xc9, "CharacterType");
        }

        public override object CastToType(ISessionInterface session, object a, SqlType otherType)
        {
            if (a == null)
            {
                return a;
            }
            return this.CastOrConvertToType(session, a, otherType, true);
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            string str = (string) a;
            string str2 = (string) b;
            if (base.TypeCode == 12)
            {
                if (!forEquality)
                {
                    return this._collation.Compare(str, str2);
                }
                return string.CompareOrdinal(str, str2);
            }
            if (base.TypeCode == 1)
            {
                if (a == b)
                {
                    return 0;
                }
                if (a == null)
                {
                    return -1;
                }
                if (b == null)
                {
                    return 1;
                }
                int length = str.Length;
                int count = str2.Length;
                if (length != count)
                {
                    if (length > count)
                    {
                        char[] destination = new char[length];
                        str2.CopyTo(0, destination, 0, count);
                        ArrayUtil.FillArray(destination, count, ' ');
                        str2 = new string(destination);
                    }
                    else
                    {
                        char[] destination = new char[count];
                        str.CopyTo(0, destination, 0, length);
                        ArrayUtil.FillArray(destination, length, ' ');
                        str = new string(destination);
                    }
                }
            }
            else if (base.TypeCode == 100)
            {
                if (!forEquality)
                {
                    return this._collation.CompareIgnoreCase(str, str2);
                }
                return this._collation.CompareIgnoreCaseForEquality(str, str2);
            }
            if (!forEquality)
            {
                return this._collation.Compare(str, str2);
            }
            return string.CompareOrdinal(str, str2);
        }

        public override object Concat(Session session, object a, object b)
        {
            string str;
            string str2;
            if ((a == null) || (b == null))
            {
                return null;
            }
            IClobData data = a as IClobData;
            if (data != null)
            {
                str = data.GetSubString(session, 0L, (int) data.Length(session));
            }
            else
            {
                str = a.ToString();
            }
            IClobData data2 = b as IClobData;
            if (data2 != null)
            {
                str2 = data2.GetSubString(session, 0L, (int) data2.Length(session));
            }
            else
            {
                str2 = b.ToString();
            }
            if (base.TypeCode == 40)
            {
                ClobDataId id1 = session.CreateClob((long) (str.Length + str2.Length));
                id1.SetString(session, 0L, str);
                id1.SetString(session, (long) str.Length, str2);
                return id1;
            }
            return (str + str2);
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return a;
            }
            string str = a.ToString();
            return this.ConvertToType(session, str, SqlType.SqlVarchar);
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            return StringConverter.ToQuotedString(this.ConvertToString(a), '\'', true);
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            int typeCode = base.TypeCode;
            if (typeCode == 1)
            {
                string str2 = (string) a;
                int length = str2.Length;
                if ((base.Precision == 0) || (length == base.Precision))
                {
                    return str2;
                }
                char[] destination = new char[(int) base.Precision];
                str2.CopyTo(0, destination, 0, length);
                for (int i = length; i < base.Precision; i++)
                {
                    destination[i] = ' ';
                }
                return new string(destination);
            }
            if ((typeCode != 12) && (typeCode != 100))
            {
                throw Error.RuntimeError(0xc9, "CharacterType");
            }
            return (string) a;
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            if (a == null)
            {
                return a;
            }
            return this.CastOrConvertToType(session, a, othType, false);
        }

        public override object ConvertToTypeAdo(ISessionInterface session, object a, SqlType otherType)
        {
            if (a == null)
            {
                return a;
            }
            if (otherType.TypeCode == 30)
            {
                throw Error.GetError(0x15b9);
            }
            return this.ConvertToType(session, a, otherType);
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return a;
            }
            if (base.Precision == 0)
            {
                return a;
            }
            int typeCode = base.TypeCode;
            if (typeCode > 12)
            {
                if (typeCode == 40)
                {
                    return a;
                }
                if (typeCode != 100)
                {
                    goto Label_0122;
                }
            }
            else
            {
                switch (typeCode)
                {
                    case 1:
                    {
                        string str2 = (string) a;
                        int length = str2.Length;
                        if (length == base.Precision)
                        {
                            return a;
                        }
                        if (length > base.Precision)
                        {
                            if (GetRightTrimSize(str2, ' ') > base.Precision)
                            {
                                throw Error.GetError(0xd49);
                            }
                            return str2.Substring(0, (int) base.Precision);
                        }
                        char[] destination = new char[(int) base.Precision];
                        str2.CopyTo(0, destination, 0, length);
                        for (int i = length; i < base.Precision; i++)
                        {
                            destination[i] = ' ';
                        }
                        return new string(destination);
                    }
                    case 12:
                        break;

                    default:
                        goto Label_0122;
                }
            }
            string s = (string) a;
            if (s.Length <= base.Precision)
            {
                return a;
            }
            if (GetRightTrimSize(s, ' ') <= base.Precision)
            {
                return s.Substring(0, (int) base.Precision);
            }
            throw Error.GetError(0xd49);
        Label_0122:
            throw Error.RuntimeError(0xc9, "CharacterType");
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
            switch (base.TypeCode)
            {
                case 1:
                    return 8;

                case 12:
                    return 9;

                case 40:
                    return 0x17;

                case 100:
                    return 10;
            }
            throw Error.RuntimeError(0xc9, "CharacterType");
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            int num2;
            if (base.TypeCode == other.TypeCode)
            {
                if (base.Precision < other.Precision)
                {
                    return other;
                }
                return this;
            }
            int typeCode = other.TypeCode;
            if (typeCode <= 30)
            {
                switch (typeCode)
                {
                    case 0:
                        return this;

                    case 1:
                        if (base.Precision < other.Precision)
                        {
                            return GetCharacterType(base.TypeCode, other.Precision);
                        }
                        return this;

                    case 12:
                        if ((base.TypeCode == 40) || (base.TypeCode == 100))
                        {
                            if (base.Precision < other.Precision)
                            {
                                return GetCharacterType(base.TypeCode, other.Precision);
                            }
                            return this;
                        }
                        if (other.Precision < base.Precision)
                        {
                            return this;
                        }
                        return GetCharacterType(base.TypeCode, other.Precision);
                }
                if (typeCode != 30)
                {
                    goto Label_0174;
                }
            }
            else
            {
                if (typeCode <= 0x3d)
                {
                    if (typeCode == 40)
                    {
                        if (other.Precision < base.Precision)
                        {
                            return GetCharacterType(other.TypeCode, base.Precision);
                        }
                        return other;
                    }
                    if ((typeCode - 60) <= 1)
                    {
                        goto Label_0169;
                    }
                }
                else
                {
                    switch (typeCode)
                    {
                        case 100:
                            if (base.TypeCode == 40)
                            {
                                if (base.Precision < other.Precision)
                                {
                                    return GetCharacterType(base.TypeCode, other.Precision);
                                }
                                return this;
                            }
                            if (other.Precision < base.Precision)
                            {
                                return GetCharacterType(other.TypeCode, base.Precision);
                            }
                            return other;

                        case 0x457:
                            goto Label_0169;
                    }
                }
                goto Label_0174;
            }
        Label_0169:
            throw Error.GetError(0x15ba);
        Label_0174:
            num2 = other.DisplaySize();
            return GetCharacterType(12, (long) num2).GetAggregateType(this);
        }

        public Charset GetCharacterSet()
        {
            return this._charset;
        }

        public static SqlType GetCharacterType(int type, long precision)
        {
            if (type <= 12)
            {
                if ((type == 1) || (type == 12))
                {
                    goto Label_0021;
                }
                goto Label_002B;
            }
            if (type == 40)
            {
                return new ClobType(precision);
            }
            if (type != 100)
            {
                goto Label_002B;
            }
        Label_0021:
            return new CharacterType(type, (long) ((int) precision));
        Label_002B:
            throw Error.RuntimeError(0xc9, "CharacterType");
        }

        public Collation GetCollation()
        {
            return this._collation;
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            SqlType type2;
            if (operation != 0x24)
            {
                return this.GetAggregateType(other);
            }
            long precision = base.Precision + other.Precision;
            int typeCode = other.TypeCode;
            if (typeCode <= 12)
            {
                switch (typeCode)
                {
                    case 0:
                        return this;

                    case 1:
                        type2 = this;
                        goto Label_0087;
                }
                if (typeCode != 12)
                {
                    goto Label_007C;
                }
                type2 = ((base.TypeCode == 40) || (base.TypeCode == 100)) ? this : other;
                goto Label_0087;
            }
            switch (typeCode)
            {
                case 40:
                    type2 = other;
                    goto Label_0087;

                case 100:
                    type2 = (base.TypeCode == 40) ? this : other;
                    goto Label_0087;
            }
        Label_007C:
            throw Error.GetError(0x15ba);
        Label_0087:
            if (precision > 0x7fffffffL)
            {
                if (base.TypeCode == 60)
                {
                    throw Error.GetError(0x15c2);
                }
                if (base.TypeCode == 1)
                {
                    precision = 0x7fffffffL;
                }
            }
            return GetCharacterType(type2.TypeCode, precision);
        }

        public override Type GetCSharpClass()
        {
            return typeof(string);
        }

        public override string GetCSharpClassName()
        {
            return "System.String";
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

        public override string GetFullNameString()
        {
            switch (base.TypeCode)
            {
                case 1:
                    return "CHARACTER";

                case 12:
                    return "CHARACTER VARYING";

                case 40:
                    return "CHARACTER LARGE OBJECT";

                case 100:
                    return "VARCHAR_IGNORECASE";
            }
            throw Error.RuntimeError(0xc9, "CharacterType");
        }

        public override long GetMaxPrecision()
        {
            return 0x7fffffffL;
        }

        public override string GetNameString()
        {
            switch (base.TypeCode)
            {
                case 1:
                    return "CHAR";

                case 12:
                    return "VARCHAR";

                case 40:
                    return "CLOB";

                case 100:
                    return "VARCHAR_IGNORECASE";
            }
            throw Error.RuntimeError(0xc9, "CharacterType");
        }

        public static int GetRightTrimSize(string s, char trim)
        {
            int num = s.Length - 1;
            while ((num >= 0) && (s[num] == trim))
            {
                num--;
            }
            return (num + 1);
        }

        public override int GetSqlGenericTypeCode()
        {
            if (base.TypeCode != 1)
            {
                return 12;
            }
            return base.TypeCode;
        }

        public object Initcap(Session session, object data)
        {
            if (data == null)
            {
                return null;
            }
            if (base.TypeCode == 40)
            {
                IClobData data2 = (IClobData) data;
                string s = data2.GetSubString(session, 0L, (int) data2.Length(session));
                s = this._collation.ToInitcap(this._collation.ToLowerCase(s));
                ClobDataId id1 = session.CreateClob((long) s.Length);
                id1.SetString(session, 0L, s);
                return id1;
            }
            return this._collation.ToInitcap(this._collation.ToLowerCase((string) data));
        }

        public bool IsCaseInsensitive()
        {
            return (base.TypeCode == 100);
        }

        public override bool IsCharacterType()
        {
            return true;
        }

        public bool IsEqualIdentical()
        {
            return this._isEqualIdentical;
        }

        public object Lower(Session session, object data)
        {
            if (data == null)
            {
                return null;
            }
            if (base.TypeCode == 40)
            {
                IClobData data2 = (IClobData) data;
                string s = data2.GetSubString(session, 0L, (int) data2.Length(session));
                s = this._collation.ToLowerCase(s);
                ClobDataId id1 = session.CreateClob((long) s.Length);
                id1.SetString(session, 0L, s);
                return id1;
            }
            return this._collation.ToLowerCase((string) data);
        }

        public object LPad(Session session, object data, int width, string paddingChar)
        {
            if ((data == null) || (width < 0))
            {
                return null;
            }
            if (string.IsNullOrEmpty(paddingChar))
            {
                return data;
            }
            if (base.TypeCode == 40)
            {
                IClobData data2 = (IClobData) data;
                string str = data2.GetSubString(session, 0L, (int) data2.Length(session));
                if (str.Length > width)
                {
                    return str.Substring(0, width);
                }
                str = (paddingChar.Length > 1) ? PadLeft(str, width, paddingChar) : str.PadLeft(width, paddingChar[0]);
                ClobDataId id1 = session.CreateClob((long) str.Length);
                id1.SetString(session, 0L, str);
                return id1;
            }
            string str2 = (string) data;
            if (str2.Length > width)
            {
                return str2.Substring(0, width);
            }
            if (paddingChar.Length > 1)
            {
                return PadLeft(str2, width, paddingChar);
            }
            return str2.PadLeft(width, paddingChar[0]);
        }

        public object Overlay(ISessionInterface session, object data, object overlay, long offset, long length, bool hasLength)
        {
            if ((data == null) || (overlay == null))
            {
                return null;
            }
            if (!hasLength)
            {
                length = (base.TypeCode == 40) ? ((IClobData) overlay).Length(session) : ((long) ((string) overlay).Length);
            }
            object a = this.Concat(null, this.Substring(session, data, 0L, offset, true, false), overlay);
            return this.Concat(null, a, this.Substring(session, data, offset + length, 0L, false, false));
        }

        private static string PadLeft(string str, int width, string pad)
        {
            StringBuilder builder = new StringBuilder();
            if ((str.Length + pad.Length) <= width)
            {
                int num2 = (width - str.Length) / pad.Length;
                for (int i = 0; i < num2; i++)
                {
                    builder.Append(pad);
                }
            }
            int count = (width - str.Length) % pad.Length;
            builder.Append(pad, 0, count);
            builder.Append(str);
            return builder.ToString();
        }

        private static string PadRight(string str, int width, string pad)
        {
            StringBuilder builder = new StringBuilder(str);
            if ((str.Length + pad.Length) <= width)
            {
                int num2 = (width - str.Length) / pad.Length;
                for (int i = 0; i < num2; i++)
                {
                    builder.Append(pad);
                }
            }
            int count = (width - str.Length) % pad.Length;
            builder.Append(pad, 0, count);
            return builder.ToString();
        }

        public virtual long Position(ISessionInterface session, object data, object otherData, SqlType otherType, long offset)
        {
            if ((data == null) || (otherData == null))
            {
                return -1L;
            }
            string str = (string) data;
            if (otherType.TypeCode == 40)
            {
                IClobData data2 = (IClobData) otherData;
                long num2 = data2.Length(session);
                if ((offset + num2) > str.Length)
                {
                    return -1L;
                }
                string str2 = data2.GetSubString(session, 0L, (int) num2);
                return (long) str.IndexOf(str2, (int) offset);
            }
            if (!otherType.IsCharacterType())
            {
                throw Error.RuntimeError(0xc9, "CharacterType");
            }
            string str3 = (string) otherData;
            long length = str3.Length;
            if ((offset + length) > str.Length)
            {
                return -1L;
            }
            return str.IndexOf(str3, (int) offset);
        }

        public override int PrecedenceDegree(SqlType other)
        {
            if (other.TypeCode == base.TypeCode)
            {
                return 0;
            }
            if (!other.IsCharacterType())
            {
                return -2147483648;
            }
            switch (base.TypeCode)
            {
                case 40:
                    if (other.TypeCode != 1)
                    {
                        return -2;
                    }
                    return -4;

                case 100:
                case 12:
                    if ((other.TypeCode == 12) || (other.TypeCode == 100))
                    {
                        return 0;
                    }
                    if (other.TypeCode != 40)
                    {
                        return 2;
                    }
                    return 4;

                case 1:
                    if (other.TypeCode != 40)
                    {
                        return 2;
                    }
                    return 4;
            }
            throw Error.RuntimeError(0xc9, "CharacterType");
        }

        public override bool RequiresPrecision()
        {
            if (base.TypeCode != 12)
            {
                return (base.TypeCode == 100);
            }
            return true;
        }

        public object RPad(Session session, object data, int width, string paddingChar)
        {
            if ((data == null) || (width < 0))
            {
                return null;
            }
            if (string.IsNullOrEmpty(paddingChar))
            {
                return data;
            }
            if (base.TypeCode == 40)
            {
                IClobData data2 = (IClobData) data;
                string str = data2.GetSubString(session, 0L, (int) data2.Length(session));
                if (str.Length > width)
                {
                    return str.Substring(0, width);
                }
                str = (paddingChar.Length > 1) ? PadRight(str, width, paddingChar) : str.PadRight(width, paddingChar[0]);
                ClobDataId id1 = session.CreateClob((long) str.Length);
                id1.SetString(session, 0L, str);
                return id1;
            }
            string str2 = (string) data;
            if (str2.Length > width)
            {
                return str2.Substring(0, width);
            }
            if (paddingChar.Length > 1)
            {
                return PadRight(str2, width, paddingChar);
            }
            return str2.PadRight(width, paddingChar[0]);
        }

        public long Size(ISessionInterface session, object data)
        {
            if (base.TypeCode == 40)
            {
                return ((IClobData) data).Length(session);
            }
            return ((string) data).Length;
        }

        public object Substring(ISessionInterface session, object data, long offset, long length, bool hasLength, bool trailing)
        {
            IClobData data2 = null;
            string str = null;
            long num;
            long num2;
            if (base.TypeCode == 40)
            {
                data2 = (IClobData) data;
                num = data2.Length(session);
            }
            else
            {
                str = (string) data;
                num = str.Length;
            }
            if (offset < 0L)
            {
                offset += num + 1L;
            }
            if (trailing)
            {
                num2 = num;
                if (length > num)
                {
                    offset = 0L;
                    length = num;
                }
                else
                {
                    offset = num - length;
                }
            }
            else if (hasLength)
            {
                num2 = offset + length;
            }
            else
            {
                num2 = (num > offset) ? num : offset;
            }
            if (num2 < offset)
            {
                throw Error.GetError(0xd67);
            }
            if (((offset > num2) || (num2 < 0L)) || (offset > num))
            {
                offset = 0L;
                return string.Empty;
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
            if (base.TypeCode == 40)
            {
                string str2 = data2.GetSubString(session, offset, (int) length);
                ClobDataId id1 = session.CreateClob(length);
                id1.SetString(session, 0L, str2);
                return id1;
            }
            return str.Substring((int) offset, (int) length);
        }

        public object Trim(ISessionInterface session, object data, string trim, bool leading, bool trailing)
        {
            string str;
            if (data == null)
            {
                return null;
            }
            IClobData data2 = data as IClobData;
            if (data2 != null)
            {
                str = data2.GetSubString(session, 0L, (int) data2.Length(session));
            }
            else
            {
                str = (string) data;
            }
            if (leading & trailing)
            {
                str = str.Trim(trim.ToCharArray());
            }
            else if (leading)
            {
                str = str.TrimStart(trim.ToCharArray());
            }
            else if (trailing)
            {
                str = str.TrimEnd(trim.ToCharArray());
            }
            if (base.TypeCode == 40)
            {
                ClobDataId id1 = session.CreateClob((long) str.Length);
                id1.SetString(session, 0L, str);
                return id1;
            }
            return str;
        }

        public object Upper(Session session, object data)
        {
            if (data == null)
            {
                return null;
            }
            if (base.TypeCode == 40)
            {
                IClobData data2 = (IClobData) data;
                string s = data2.GetSubString(session, 0L, (int) data2.Length(session));
                s = this._collation.ToUpperCase(s);
                ClobDataId id1 = session.CreateClob((long) s.Length);
                id1.SetString(session, 0L, s);
                return id1;
            }
            return this._collation.ToUpperCase((string) data);
        }
    }
}

