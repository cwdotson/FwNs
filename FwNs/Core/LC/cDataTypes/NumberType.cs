namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class NumberType : SqlType
    {
        public const int TinyintPrecision = 3;
        public const int SmallintPrecision = 5;
        public const int IntegerPrecision = 10;
        public const int BigintPrecision = 0x13;
        public const int DoublePrecision = 0x10;
        public const int DefaultNumericPrecision = 0x7fffffff;
        public const int DefaultNumericScale = 0x7fff;
        public const int MaxNumericPrecision = 0x1c;
        public const int MaxNumericScale = 0x1c;
        public const int BigintSquareNumericPrecision = 0x1c;
        public const int TinyintWidth = 8;
        public const int SmallintWidth = 0x10;
        public const int IntegerWidth = 0x20;
        public const int BigintWidth = 0x40;
        public const int FloatWidth = 0x20;
        public const int DoubleWidth = 0x40;
        public const int DecimalWidth = 0x80;
        [DecimalConstant(0, 0, 0, 0x7fffffff, uint.MaxValue)]
        public static readonly decimal MaxLong = 9223372036854775807M;
        [DecimalConstant(0, 0x80, (uint) 0, (uint) 0x80000000, (uint) 0)]
        public static readonly decimal MinLong = -9223372036854775808M;
        [DecimalConstant(0, 0, (uint) 0, (uint) 0, (uint) 0x7fffffff)]
        public static readonly decimal MaxInt = 2147483647M;
        [DecimalConstant(0, 0x80, (uint) 0, (uint) 0, (uint) 0x80000000)]
        public static readonly decimal MinInt = -2147483648M;
        private readonly int _typeWidth;

        public NumberType(int type, long precision, int scale) : base(2, type, precision, scale)
        {
            switch (type)
            {
                case -6:
                    this._typeWidth = 8;
                    return;

                case 2:
                case 3:
                    this._typeWidth = 0x80;
                    return;

                case 4:
                    this._typeWidth = 0x20;
                    return;

                case 5:
                    this._typeWidth = 0x10;
                    return;

                case 6:
                    this._typeWidth = 0x20;
                    return;

                case 7:
                case 8:
                    this._typeWidth = 0x40;
                    return;

                case 0x19:
                    this._typeWidth = 0x40;
                    return;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override object Absolute(object a)
        {
            if (!this.IsNegative(a))
            {
                return a;
            }
            return this.Negate(a);
        }

        public override bool AcceptsPrecision()
        {
            int typeCode = base.TypeCode;
            if (((typeCode - 2) > 1) && (typeCode != 6))
            {
                return false;
            }
            return true;
        }

        public override bool AcceptsScale()
        {
            return ((base.TypeCode - 2) <= 1);
        }

        public override object Add(object a, object b, SqlType aType, SqlType bType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case -6:
                case 4:
                case 5:
                    return (aType.ToInt32(a) + bType.ToInt32(b));

                case 2:
                case 3:
                {
                    decimal num3 = aType.ToDecimal(a);
                    decimal num4 = bType.ToDecimal(b);
                    num3 += num4;
                    return this.ConvertToTypeLimits(null, num3);
                }
                case 6:
                case 7:
                case 8:
                    return (aType.ToReal(a) + bType.ToReal(b));

                case 0x19:
                    return (aType.ToInt64(a) + bType.ToInt64(b));
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        private static void CalcRealPrecisionScale(decimal dec, out uint precision, out uint scale)
        {
            string str = dec.ToString(CultureInfo.InvariantCulture);
            str = str.EndsWith(".0") ? str.Substring(0, str.Length - 2) : str;
            precision = 0;
            scale = 0;
            int num = 0;
            bool flag = false;
            bool flag2 = false;
            foreach (char ch in str)
            {
                if (flag)
                {
                    if (ch == '0')
                    {
                        num++;
                    }
                    else
                    {
                        flag2 = true;
                        num = 0;
                    }
                    precision++;
                    scale++;
                }
                else if (ch == '.')
                {
                    flag = true;
                }
                else if ((ch != '-') && ((ch != '0') | flag2))
                {
                    flag2 = true;
                    precision++;
                }
            }
            if (!flag2)
            {
                precision++;
            }
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            if (((othType.TypeCode != 0) && !othType.IsNumberType()) && !othType.IsIntervalType())
            {
                return othType.IsCharacterType();
            }
            return true;
        }

        public object Ceiling(object a)
        {
            if (a == null)
            {
                return null;
            }
            int typeCode = base.TypeCode;
            if ((typeCode - 2) > 1)
            {
                if ((typeCode - 6) > 2)
                {
                    return a;
                }
            }
            else
            {
                return Math.Ceiling((decimal) a);
            }
            double d = Math.Ceiling(Convert.ToDouble(a));
            if (double.IsInfinity(d))
            {
                throw Error.GetError(0xd4b);
            }
            return d;
        }

        public int Compare(Session session, object a, object b)
        {
            if (a == b)
            {
                return 0;
            }
            if (a != null)
            {
                long num22;
                if (b == null)
                {
                    return 1;
                }
                int typeCode = base.TypeCode;
                switch (typeCode)
                {
                    case -6:
                    case 4:
                    case 5:
                    {
                        int num2;
                        if (!(b is int))
                        {
                            if (b is long)
                            {
                                long num4;
                                try
                                {
                                    num4 = (int) a;
                                }
                                catch (Exception)
                                {
                                    num4 = Convert.ToInt64(a);
                                }
                                long num5 = (long) b;
                                if (num4 > num5)
                                {
                                    return 1;
                                }
                                if (num5 <= num4)
                                {
                                    return 0;
                                }
                                return -1;
                            }
                            if (b is double)
                            {
                                double num6;
                                try
                                {
                                    num6 = (int) a;
                                }
                                catch (Exception)
                                {
                                    num6 = Convert.ToDouble(a);
                                }
                                double num7 = (double) b;
                                if (num6 > num7)
                                {
                                    return 1;
                                }
                                if (num7 <= num6)
                                {
                                    return 0;
                                }
                                return -1;
                            }
                            if (b is decimal)
                            {
                                decimal num8;
                                try
                                {
                                    num8 = (int) a;
                                }
                                catch (Exception)
                                {
                                    num8 = Convert.ToDecimal(a);
                                }
                                int num9 = num8.CompareTo((decimal) b);
                                if (num9 == 0)
                                {
                                    return 0;
                                }
                                if (num9 >= 0)
                                {
                                    return 1;
                                }
                                return -1;
                            }
                            a = (int) a;
                            break;
                        }
                        try
                        {
                            num2 = (int) a;
                        }
                        catch (Exception)
                        {
                            num2 = Convert.ToInt32(a);
                        }
                        int num3 = (int) b;
                        if (num2 > num3)
                        {
                            return 1;
                        }
                        if (num3 <= num2)
                        {
                            return 0;
                        }
                        return -1;
                    }
                    case -5:
                    case -4:
                    case -3:
                    case -2:
                    case -1:
                    case 0:
                    case 1:
                        goto Label_0291;

                    case 2:
                    case 3:
                    {
                        decimal num10 = ConvertToDecimal(a);
                        decimal num11 = ConvertToDecimal(b);
                        int num12 = num10.CompareTo(num11);
                        if (num12 == 0)
                        {
                            return 0;
                        }
                        if (num12 >= 0)
                        {
                            return 1;
                        }
                        return -1;
                    }
                    case 6:
                    case 7:
                    case 8:
                    {
                        double num13 = Convert.ToDouble(a);
                        double num14 = Convert.ToDouble(b);
                        if (num13 <= num14)
                        {
                            if (num14 <= num13)
                            {
                                return 0;
                            }
                            return -1;
                        }
                        return 1;
                    }
                    default:
                        if (typeCode != 0x19)
                        {
                            goto Label_0291;
                        }
                        break;
                }
                if (b is long)
                {
                    long num15;
                    try
                    {
                        num15 = (long) a;
                    }
                    catch (Exception)
                    {
                        num15 = Convert.ToInt64(a);
                    }
                    long num16 = (long) b;
                    if (num15 > num16)
                    {
                        return 1;
                    }
                    if (num16 <= num15)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (b is double)
                {
                    double num17;
                    try
                    {
                        num17 = (long) a;
                    }
                    catch (Exception)
                    {
                        num17 = Convert.ToDouble(a);
                    }
                    double num18 = (double) b;
                    int num19 = num17.CompareTo(num18);
                    if (num19 == 0)
                    {
                        return 0;
                    }
                    if (num19 >= 0)
                    {
                        return 1;
                    }
                    return -1;
                }
                if (b is decimal)
                {
                    decimal num20;
                    try
                    {
                        num20 = (long) a;
                    }
                    catch (Exception)
                    {
                        num20 = Convert.ToDecimal(a);
                    }
                    int num21 = num20.CompareTo((decimal) b);
                    if (num21 == 0)
                    {
                        return 0;
                    }
                    if (num21 >= 0)
                    {
                        return 1;
                    }
                    return -1;
                }
                int num1 = b as int;
                try
                {
                    num22 = (long) a;
                }
                catch (Exception)
                {
                    num22 = Convert.ToInt64(a);
                }
                int num23 = (int) b;
                if (num22 > num23)
                {
                    return 1;
                }
                if (num23 <= num22)
                {
                    return 0;
                }
            }
            return -1;
        Label_0291:
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            if (otherType == null)
            {
                return this.Compare(session, a, b);
            }
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
            int typeCode = base.TypeCode;
            switch (typeCode)
            {
                case -6:
                case 4:
                case 5:
                {
                    long num13;
                    long num14;
                    int num5 = otherType.TypeCode;
                    switch (num5)
                    {
                        case 3:
                        {
                            decimal num8;
                            decimal num9;
                            try
                            {
                                num8 = (int) a;
                            }
                            catch (Exception)
                            {
                                num8 = Convert.ToDecimal(a);
                            }
                            try
                            {
                                num9 = (decimal) b;
                            }
                            catch (Exception)
                            {
                                num9 = Convert.ToDecimal(b);
                            }
                            int num10 = num8.CompareTo(num9);
                            if (num10 == 0)
                            {
                                return 0;
                            }
                            if (num10 >= 0)
                            {
                                return 1;
                            }
                            return -1;
                        }
                        case 4:
                        case 5:
                        case -6:
                            int num6;
                            int num7;
                            try
                            {
                                num6 = (int) a;
                            }
                            catch (Exception)
                            {
                                num6 = Convert.ToInt32(a);
                            }
                            try
                            {
                                num7 = (int) b;
                            }
                            catch (Exception)
                            {
                                num7 = Convert.ToInt32(b);
                            }
                            if (num6 > num7)
                            {
                                return 1;
                            }
                            if (num7 <= num6)
                            {
                                return 0;
                            }
                            return -1;

                        case 8:
                            double num11;
                            double num12;
                            try
                            {
                                num11 = (int) a;
                            }
                            catch (Exception)
                            {
                                num11 = Convert.ToDouble(a);
                            }
                            try
                            {
                                num12 = (double) b;
                            }
                            catch (Exception)
                            {
                                num12 = Convert.ToDouble(b);
                            }
                            if (num11 > num12)
                            {
                                return 1;
                            }
                            if (num12 <= num11)
                            {
                                return 0;
                            }
                            return -1;
                    }
                    if (num5 != 0x19)
                    {
                        break;
                    }
                    try
                    {
                        num13 = (int) a;
                    }
                    catch (Exception)
                    {
                        num13 = Convert.ToInt64(a);
                    }
                    try
                    {
                        num14 = (long) b;
                    }
                    catch (Exception)
                    {
                        num14 = Convert.ToInt64(b);
                    }
                    if (num13 > num14)
                    {
                        return 1;
                    }
                    if (num14 <= num13)
                    {
                        return 0;
                    }
                    return -1;
                }
                case -5:
                case -4:
                case -3:
                case -2:
                case -1:
                case 0:
                case 1:
                    goto Label_036B;

                case 2:
                case 3:
                {
                    decimal num15;
                    try
                    {
                        num15 = (decimal) a;
                    }
                    catch (Exception)
                    {
                        num15 = Convert.ToDecimal(a);
                    }
                    decimal num16 = Convert.ToDecimal(b);
                    int num17 = num15.CompareTo(num16);
                    if (num17 == 0)
                    {
                        return 0;
                    }
                    if (num17 >= 0)
                    {
                        return 1;
                    }
                    return -1;
                }
                case 6:
                case 7:
                case 8:
                {
                    double num18;
                    try
                    {
                        num18 = (double) a;
                    }
                    catch (Exception)
                    {
                        num18 = Convert.ToDouble(a);
                    }
                    double num19 = Convert.ToDouble(b);
                    if (num18 > num19)
                    {
                        return 1;
                    }
                    if (num19 <= num18)
                    {
                        return 0;
                    }
                    return -1;
                }
                default:
                    if (typeCode != 0x19)
                    {
                        goto Label_036B;
                    }
                    break;
            }
            switch (otherType.TypeCode)
            {
                case 3:
                {
                    decimal num20;
                    try
                    {
                        num20 = (long) a;
                    }
                    catch (Exception)
                    {
                        num20 = Convert.ToDecimal(a);
                    }
                    try
                    {
                        decimal decimal1 = (decimal) b;
                    }
                    catch (Exception)
                    {
                        Convert.ToDecimal(b);
                    }
                    int num21 = num20.CompareTo((decimal) b);
                    if (num21 == 0)
                    {
                        return 0;
                    }
                    if (num21 >= 0)
                    {
                        return 1;
                    }
                    return -1;
                }
                case 4:
                case 5:
                case 6:
                case 7:
                case -6:
                    long num3;
                    int num4;
                    try
                    {
                        num3 = (long) a;
                    }
                    catch (Exception)
                    {
                        num3 = Convert.ToInt64(a);
                    }
                    try
                    {
                        num4 = (int) b;
                    }
                    catch (Exception)
                    {
                        num4 = Convert.ToInt32(b);
                    }
                    if (num3 > num4)
                    {
                        return 1;
                    }
                    if (num4 <= num3)
                    {
                        return 0;
                    }
                    return -1;

                case 8:
                {
                    double num22;
                    double num23;
                    try
                    {
                        num22 = (long) a;
                    }
                    catch (Exception)
                    {
                        num22 = Convert.ToDouble(a);
                    }
                    try
                    {
                        num23 = (double) b;
                    }
                    catch (Exception)
                    {
                        num23 = Convert.ToDouble(b);
                    }
                    int num24 = num22.CompareTo(num23);
                    if (num24 == 0)
                    {
                        return 0;
                    }
                    if (num24 >= 0)
                    {
                        return 1;
                    }
                    return -1;
                }
                default:
                    long num25;
                    long num26;
                    try
                    {
                        num25 = (long) a;
                    }
                    catch (Exception)
                    {
                        num25 = Convert.ToInt64(a);
                    }
                    try
                    {
                        num26 = (long) b;
                    }
                    catch (Exception)
                    {
                        num26 = Convert.ToInt64(b);
                    }
                    if (num25 > num26)
                    {
                        return 1;
                    }
                    if (num26 <= num25)
                    {
                        return 0;
                    }
                    return -1;
            }
        Label_036B:
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public int CompareToZero(object a)
        {
            if (a == null)
            {
                return 0;
            }
            int typeCode = base.TypeCode;
            switch (typeCode)
            {
                case -6:
                case 4:
                case 5:
                {
                    int num2 = Convert.ToInt32(a, CultureInfo.InvariantCulture);
                    if (num2 == 0)
                    {
                        return 0;
                    }
                    if (num2 >= 0)
                    {
                        return 1;
                    }
                    return -1;
                }
                case -5:
                case -4:
                case -3:
                case -2:
                case -1:
                case 0:
                case 1:
                    break;

                case 2:
                case 3:
                {
                    decimal num3 = Convert.ToDecimal(a, CultureInfo.InvariantCulture);
                    if (!(num3 == decimal.Zero))
                    {
                        if (num3 >= decimal.Zero)
                        {
                            return 1;
                        }
                        return -1;
                    }
                    return 0;
                }
                case 6:
                case 7:
                case 8:
                {
                    double num4 = Convert.ToDouble(a, CultureInfo.InvariantCulture);
                    if (num4 == 0.0)
                    {
                        return 0;
                    }
                    if (num4 >= 0.0)
                    {
                        return 1;
                    }
                    return -1;
                }
                default:
                    if (typeCode == 0x19)
                    {
                        long num5 = Convert.ToInt64(a, CultureInfo.InvariantCulture);
                        if (num5 == 0)
                        {
                            return 0;
                        }
                        if (num5 >= 0L)
                        {
                            return 1;
                        }
                        return -1;
                    }
                    break;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override object ConvertCSharpToSQL(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case -6:
                    return Convert.ToByte(a, CultureInfo.InvariantCulture);

                case -5:
                case -4:
                case -3:
                case -2:
                case -1:
                case 0:
                case 1:
                    return a;

                case 2:
                case 3:
                    return Convert.ToDecimal(a, CultureInfo.InvariantCulture);

                case 4:
                    return Convert.ToInt32(a, CultureInfo.InvariantCulture);

                case 5:
                    return Convert.ToInt16(a, CultureInfo.InvariantCulture);

                case 6:
                    return Convert.ToSingle(a, CultureInfo.InvariantCulture);

                case 7:
                case 8:
                    return Convert.ToDouble(a, CultureInfo.InvariantCulture);

                case 0x19:
                    return Convert.ToInt64(a, CultureInfo.InvariantCulture);
            }
            return a;
        }

        public override object ConvertSQLToCSharp(ISessionInterface session, object a)
        {
            switch (base.TypeCode)
            {
                case -6:
                    return Convert.ToByte(a, CultureInfo.InvariantCulture);

                case -5:
                case -4:
                case -3:
                case -2:
                case -1:
                case 0:
                case 1:
                    return a;

                case 2:
                case 3:
                    return Convert.ToDecimal(a, CultureInfo.InvariantCulture);

                case 4:
                    return Convert.ToInt32(a, CultureInfo.InvariantCulture);

                case 5:
                    return Convert.ToInt16(a, CultureInfo.InvariantCulture);

                case 6:
                    return Convert.ToSingle(a, CultureInfo.InvariantCulture);

                case 7:
                case 8:
                    return Convert.ToDouble(a, CultureInfo.InvariantCulture);

                case 0x19:
                    return Convert.ToInt64(a, CultureInfo.InvariantCulture);
            }
            return a;
        }

        private static decimal ConvertToDecimal(object a)
        {
            decimal num;
            try
            {
                num = Convert.ToDecimal(a);
            }
            catch (Exception exception)
            {
                throw Error.GetError(0xd4b, exception);
            }
            return num;
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            SqlType sqlInteger;
            if (a == null)
            {
                return a;
            }
            if (a is float)
            {
                a = (float) a;
            }
            else if (a is byte)
            {
                a = (byte) a;
            }
            else if (a is short)
            {
                a = (short) a;
            }
            if (a is int)
            {
                sqlInteger = SqlType.SqlInteger;
            }
            else if (a is long)
            {
                sqlInteger = SqlType.SqlBigint;
            }
            else if (a is double)
            {
                sqlInteger = SqlType.SqlDouble;
            }
            else if (a is decimal)
            {
                if ((base.TypeCode == 3) || (base.TypeCode == 2))
                {
                    return a;
                }
                sqlInteger = SqlType.SqlDecimalDefault;
            }
            else if (a is string)
            {
                sqlInteger = SqlType.SqlVarchar;
            }
            else if (a is Enum)
            {
                a = Convert.ToInt32(a);
                sqlInteger = SqlType.SqlInteger;
            }
            else if (a is ulong)
            {
                a = (ulong) a;
                sqlInteger = SqlType.SqlDecimal;
            }
            else if (a is uint)
            {
                a = (uint) a;
                sqlInteger = SqlType.SqlBigint;
            }
            else
            {
                if (!(a is ushort))
                {
                    throw Error.GetError(0x15b9);
                }
                a = (ushort) a;
                sqlInteger = SqlType.SqlInteger;
            }
            return this.ConvertToType(session, a, sqlInteger);
        }

        private static double ConvertToDouble(object a)
        {
            double num;
            try
            {
                num = Convert.ToDouble(a);
            }
            catch (Exception exception)
            {
                throw Error.GetError(0xd4b, exception);
            }
            return num;
        }

        private static double ConvertToDouble(object a, SqlType otherType)
        {
            if (otherType.TypeCode == 0x5b)
            {
                return (double) (((TimestampData) a).GetSeconds() / 0x15180L);
            }
            return ConvertToDouble(a);
        }

        private static int ConvertToInt(object a, int type)
        {
            int num;
            try
            {
                num = Convert.ToInt32(a);
            }
            catch (Exception exception)
            {
                throw Error.GetError(0xd4b, exception);
            }
            if (type == -6)
            {
                if ((0xff < num) || (num < 0))
                {
                    throw Error.GetError(0xd4b);
                }
                return num;
            }
            if ((type == 5) && ((0x7fff < num) || (num < -32768)))
            {
                throw Error.GetError(0xd4b);
            }
            return num;
        }

        private static long ConvertToLong(object a)
        {
            long num;
            try
            {
                num = Convert.ToInt64(a);
            }
            catch (Exception exception)
            {
                throw Error.GetError(0xd4b, exception);
            }
            return num;
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            return this.ConvertToString(a);
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            int typeCode = base.TypeCode;
            switch (typeCode)
            {
                case 2:
                case 3:
                {
                    decimal num2 = (decimal) a;
                    return num2.ToString(CultureInfo.InvariantCulture);
                }
                case 4:
                case 5:
                case -6:
                {
                    int num5 = (int) a;
                    return num5.ToString(CultureInfo.InvariantCulture);
                }
                case 6:
                    break;

                case 7:
                case 8:
                {
                    double d = (double) a;
                    if (!double.IsNegativeInfinity(d))
                    {
                        if (double.IsPositiveInfinity(d))
                        {
                            return "1E0/0";
                        }
                        if (double.IsNaN(d))
                        {
                            return "0E0/0E0";
                        }
                        string str = d.ToString(CultureInfo.InvariantCulture);
                        if (str.IndexOf('E') < 0)
                        {
                            str = str + "E0";
                        }
                        return str;
                    }
                    return "-1E0/0";
                }
                default:
                    if (typeCode == 0x19)
                    {
                        long num4 = (long) a;
                        return num4.ToString(CultureInfo.InvariantCulture);
                    }
                    break;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            string str;
            if (a == null)
            {
                return a;
            }
            if (othType.TypeCode == base.TypeCode)
            {
                if ((base.TypeCode - 2) > 1)
                {
                    return a;
                }
                if ((othType.Scale == base.Scale) && (othType.Precision <= base.Precision))
                {
                    return a;
                }
            }
            if (othType.IsIntervalType())
            {
                IntervalType type = (IntervalType) othType;
                int endIntervalType = type.EndIntervalType;
                if ((endIntervalType - 0x65) <= 4)
                {
                    long num4 = type.ConvertToLong(a);
                    return this.ConvertToType(session, num4, SqlType.SqlBigint);
                }
                if (endIntervalType == 0x6a)
                {
                    IntervalSecondData data1 = (IntervalSecondData) a;
                    long units = data1.Units;
                    long nanos = data1.Nanos;
                    return ((DTIType) othType).GetSecondPart(units, nanos);
                }
            }
            int typeCode = othType.TypeCode;
            if (typeCode <= 0x10)
            {
                switch (typeCode)
                {
                    case -6:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 0x10:
                        goto Label_01EA;

                    case 1:
                    case 12:
                        goto Label_0151;
                }
                goto Label_01DF;
            }
            if (typeCode == 0x19)
            {
                goto Label_01EA;
            }
            if (typeCode != 40)
            {
                if (typeCode == 100)
                {
                    goto Label_0151;
                }
                goto Label_01DF;
            }
            IClobData data = (IClobData) a;
            a = data.GetSubString(session, 0L, (int) data.Length(session));
        Label_0151:
            str = ((string) a).Trim();
            if (base.TypeCode == 8)
            {
                if ("NaN".Equals(str, StringComparison.OrdinalIgnoreCase))
                {
                    return (double) 1.0 / (double) 0.0;
                }
                if ("Infinity".Equals(str, StringComparison.OrdinalIgnoreCase))
                {
                    return (double) 1.0 / (double) 0.0;
                }
                if ("-Infinity".Equals(str, StringComparison.OrdinalIgnoreCase))
                {
                    return (double) -1.0 / (double) 0.0;
                }
            }
            a = session.GetScanner().ConvertToNumber(str, this);
            a = this.ConvertToDefaultType(session, a);
            return this.ConvertToTypeLimits(session, a);
        Label_01DF:
            throw Error.GetError(0x15b9);
        Label_01EA:
            switch (base.TypeCode)
            {
                case -6:
                case 4:
                case 5:
                    return ConvertToInt(a, base.TypeCode);

                case 2:
                case 3:
                {
                    decimal num7 = ConvertToDecimal(a);
                    return this.ConvertToTypeLimits(session, num7);
                }
                case 6:
                case 7:
                case 8:
                    return ConvertToDouble(a);

                case 0x19:
                    return ConvertToLong(a);
            }
            throw Error.GetError(0x15b9);
        }

        public override object ConvertToTypeAdo(ISessionInterface session, object a, SqlType otherType)
        {
            if (a == null)
            {
                return a;
            }
            if (otherType.IsLobType())
            {
                throw Error.GetError(0x15b9);
            }
            if (otherType.TypeCode == 0x10)
            {
                a = Convert.ToBoolean(a, CultureInfo.InvariantCulture) ? 1 : 0;
                otherType = SqlType.SqlInteger;
            }
            return this.ConvertToType(session, a, otherType);
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            int typeCode = base.TypeCode;
            switch (typeCode)
            {
                case -6:
                case 4:
                case 5:
                    return a;

                case -5:
                case -4:
                case -3:
                case -2:
                case -1:
                case 0:
                case 1:
                    break;

                case 2:
                case 3:
                {
                    uint num3;
                    uint num4;
                    decimal dec = Convert.ToDecimal(a, CultureInfo.InvariantCulture);
                    CalcRealPrecisionScale(dec, out num3, out num4);
                    if ((base.Precision <= num3) || (base.Scale <= num4))
                    {
                        if (base.Scale < num4)
                        {
                            dec = Math.Round(dec, base.Scale);
                        }
                        else if (((base.Scale == 0x7fff) && (base.Precision < num3)) && (num4 >= (num3 - base.Precision)))
                        {
                            return Math.Round(dec, (int) (num4 - (num3 - base.Precision)));
                        }
                        if ((base.Precision < num3) && (((base.Scale >= num4) || (base.Scale == 0x7fff)) || ((num4 - base.Scale) < (num3 - base.Precision))))
                        {
                            throw Error.GetError(0xd4b);
                        }
                        return dec;
                    }
                    if ((base.Precision - base.Scale) < (num3 - num4))
                    {
                        if ((dec == decimal.Zero) || (Math.Abs(dec) < decimal.One))
                        {
                            num3--;
                        }
                        if ((base.Precision - base.Scale) < (num3 - num4))
                        {
                            throw Error.GetError(0xd4b);
                        }
                    }
                    return dec;
                }
                case 6:
                case 7:
                case 8:
                    return a;

                default:
                    if (typeCode == 0x19)
                    {
                        return a;
                    }
                    break;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override int DisplaySize()
        {
            int typeCode = base.TypeCode;
            switch (typeCode)
            {
                case -6:
                    return 4;

                case -5:
                case -4:
                case -3:
                case -2:
                case -1:
                case 0:
                case 1:
                    break;

                case 2:
                case 3:
                    if (base.Precision != 0x7fffffffL)
                    {
                        if (base.Scale == 0)
                        {
                            long precision = base.Precision;
                            return (((int) base.Precision) + 1);
                        }
                        if (base.Precision == base.Scale)
                        {
                            return (((int) base.Precision) + 3);
                        }
                        return (((int) base.Precision) + 2);
                    }
                    return 0x1f;

                case 4:
                    return 11;

                case 5:
                    return 6;

                case 6:
                case 7:
                case 8:
                    return 0x17;

                default:
                    if (typeCode == 0x19)
                    {
                        return 20;
                    }
                    break;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override object Divide(object a, object b, SqlType aType, SqlType bType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            int typeCode = base.TypeCode;
            switch (typeCode)
            {
                case -6:
                case 4:
                case 5:
                {
                    int num2 = bType.ToInt32(b);
                    if (num2 == 0)
                    {
                        throw Error.GetError(0xd68);
                    }
                    return (aType.ToInt32(a) / num2);
                }
                case -5:
                case -4:
                case -3:
                case -2:
                case -1:
                case 0:
                case 1:
                    break;

                case 2:
                case 3:
                {
                    decimal num3 = bType.ToDecimal(b);
                    if (num3 == decimal.Zero)
                    {
                        throw Error.GetError(0xd68);
                    }
                    decimal num4 = aType.ToDecimal(a) / num3;
                    return this.ConvertToTypeLimits(null, num4);
                }
                case 6:
                case 7:
                case 8:
                {
                    double num5 = bType.ToReal(b);
                    if (num5 == 0.0)
                    {
                        throw Error.GetError(0xd68);
                    }
                    return (aType.ToReal(a) / num5);
                }
                default:
                    if (typeCode == 0x19)
                    {
                        long num6 = bType.ToInt64(b);
                        if (num6 == 0)
                        {
                            throw Error.GetError(0xd68);
                        }
                        return (aType.ToInt64(a) / num6);
                    }
                    break;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public object Floor(object a)
        {
            if (a == null)
            {
                return null;
            }
            int typeCode = base.TypeCode;
            if ((typeCode - 2) > 1)
            {
                if ((typeCode - 6) > 2)
                {
                    return a;
                }
            }
            else
            {
                return Math.Floor(Convert.ToDecimal(a));
            }
            double d = Math.Floor(Convert.ToDouble(a));
            if (double.IsInfinity(d))
            {
                throw Error.GetError(0xd4b);
            }
            return d;
        }

        public override int GetAdoPrecision()
        {
            return this.GetNumericPrecisionInRadix();
        }

        public override int GetAdoScale()
        {
            if ((base.TypeCode != 2) && (base.TypeCode != 3))
            {
                return 0;
            }
            if (base.Scale != 0x7fff)
            {
                return base.Scale;
            }
            return 0x1c;
        }

        public override int GetAdoTypeCode()
        {
            switch (base.TypeCode)
            {
                case -6:
                    return 2;

                case 2:
                case 3:
                    return 6;

                case 4:
                    return 4;

                case 5:
                    return 3;

                case 6:
                case 7:
                case 8:
                    return 7;

                case 0x19:
                    return 5;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            if (this == other)
            {
                return this;
            }
            if (other.IsCharacterType())
            {
                return other.GetAggregateType(this);
            }
            switch (other.TypeCode)
            {
                case -6:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 0x19:
                    if (this._typeWidth != 0x40)
                    {
                        NumberType type = (NumberType) other;
                        if (type._typeWidth == 0x40)
                        {
                            return other;
                        }
                        if ((this._typeWidth > 0x40) || (type._typeWidth > 0x40))
                        {
                            int num2 = (base.Scale > other.Scale) ? base.Scale : other.Scale;
                            long num3 = ((base.Precision - base.Scale) > (other.Precision - other.Scale)) ? (base.Precision - base.Scale) : (other.Precision - other.Scale);
                            return GetNumberType(3, ((num3 + num2) > 0x1cL) ? 0x7fffffffL : (num3 + num2), (num2 > 0x1c) ? 0x7fff : num2);
                        }
                        if (this._typeWidth <= type._typeWidth)
                        {
                            return other;
                        }
                    }
                    return this;

                case 0:
                    return this;

                case 0x10:
                    return this;
            }
            throw Error.GetError(0x15ba);
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            int num2;
            long num3;
            if (other.TypeCode == 0)
            {
                other = this;
            }
            switch (operation)
            {
                case 0x20:
                    if (other.TypeCode != 0x5b)
                    {
                        break;
                    }
                    return SqlType.SqlDate;

                case 0x22:
                    if (!other.IsIntervalType())
                    {
                        break;
                    }
                    return other.GetCombinedType(this, 0x22);

                case 0x23:
                    if (this._typeWidth == 0x80)
                    {
                        break;
                    }
                    return SqlType.SqlDouble;

                default:
                    return this.GetAggregateType(other);
            }
            if (!other.IsNumberType())
            {
                throw Error.GetError(0x15ba);
            }
            NumberType type = (NumberType) other;
            if ((this._typeWidth == 0x40) || (type._typeWidth == 0x40))
            {
                return SqlType.SqlDouble;
            }
            int num = this._typeWidth + type._typeWidth;
            if (num <= 0x20)
            {
                return SqlType.SqlInteger;
            }
            if (num <= 0x40)
            {
                return SqlType.SqlBigint;
            }
            switch (operation)
            {
                case 0x20:
                    num2 = (base.Scale > other.Scale) ? base.Scale : other.Scale;
                    num3 = ((base.Precision - base.Scale) > (other.Precision - other.Scale)) ? (base.Precision - base.Scale) : (other.Precision - other.Scale);
                    num3 += 1L;
                    break;

                case 0x22:
                    num3 = ((base.Precision - base.Scale) + other.Precision) - other.Scale;
                    num2 = base.Scale + other.Scale;
                    break;

                case 0x23:
                    num3 = (base.Precision - base.Scale) + other.Scale;
                    num2 = (base.Scale > other.Scale) ? base.Scale : other.Scale;
                    break;

                default:
                    throw Error.RuntimeError(0xc9, "NumberType");
            }
            return GetNumberType(3, ((num3 + num2) > 0x1cL) ? 0x7fffffffL : (num3 + num2), (num2 > 0x1c) ? 0x7fff : num2);
        }

        public override Type GetCSharpClass()
        {
            switch (base.TypeCode)
            {
                case -6:
                    return typeof(byte);

                case 2:
                case 3:
                    return typeof(decimal);

                case 4:
                    return typeof(int);

                case 5:
                    return typeof(short);

                case 6:
                case 7:
                case 8:
                    return typeof(double);

                case 0x19:
                    return typeof(long);
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override string GetCSharpClassName()
        {
            switch (base.TypeCode)
            {
                case -6:
                    return "System.Byte";

                case 2:
                case 3:
                    return "System.Decimal";

                case 4:
                    return "System.Int32";

                case 5:
                    return "System.Int16";

                case 6:
                case 7:
                case 8:
                    return "System.Double";

                case 0x19:
                    return "System.Int64";
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override string GetDefinition()
        {
            if ((base.TypeCode - 2) > 1)
            {
                return this.GetNameString();
            }
            StringBuilder builder = new StringBuilder(0x10);
            builder.Append(this.GetNameString());
            if (base.Precision != 0x7fffffffL)
            {
                builder.Append('(');
                builder.Append(base.Precision);
                if (base.Scale != 0x7fff)
                {
                    builder.Append(',');
                    builder.Append(base.Scale);
                }
                builder.Append(')');
            }
            return builder.ToString();
        }

        public override string GetFullNameString()
        {
            if (base.TypeCode == 8)
            {
                return "DOUBLE PRECISION";
            }
            return this.GetNameString();
        }

        public SqlType GetIntegralType()
        {
            int typeCode = base.TypeCode;
            if ((typeCode - 2) > 1)
            {
                if ((typeCode - 6) <= 2)
                {
                    return new NumberType(2, 0x7fffffffL, 0);
                }
                return this;
            }
            if (base.Scale != 0)
            {
                return new NumberType(base.TypeCode, base.Precision, 0);
            }
            return this;
        }

        public override long GetMaxPrecision()
        {
            if ((base.TypeCode - 2) <= 1)
            {
                return 0x1cL;
            }
            return (long) this.GetNumericPrecisionInRadix();
        }

        public override int GetMaxScale()
        {
            if ((base.TypeCode - 2) <= 1)
            {
                return 0x7fff;
            }
            return 0;
        }

        public override string GetNameString()
        {
            switch (base.TypeCode)
            {
                case -6:
                    return "TINYINT";

                case 2:
                    return "NUMERIC";

                case 3:
                    return "DECIMAL";

                case 4:
                    return "INTEGER";

                case 5:
                    return "SMALLINT";

                case 6:
                    return "FLOAT";

                case 7:
                    return "REAL";

                case 8:
                    return "DOUBLE";

                case 0x19:
                    return "BIGINT";
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public int GetNominalWidth()
        {
            return this._typeWidth;
        }

        public static NumberType GetNumberType(int type, long precision, int scale)
        {
            switch (type)
            {
                case 2:
                case 3:
                    return new NumberType(type, precision, scale);

                case 4:
                    return SqlType.SqlInteger;

                case 5:
                    return SqlType.SqlSmallint;

                case 7:
                case 8:
                    return SqlType.SqlDouble;

                case -6:
                    return SqlType.Tinyint;

                case 0x19:
                    return SqlType.SqlBigint;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public int GetNumericPrecisionInRadix()
        {
            switch (base.TypeCode)
            {
                case -6:
                    return 8;

                case 2:
                case 3:
                    if (base.Precision == 0x7fffffffL)
                    {
                        return 0x1c;
                    }
                    return (int) base.Precision;

                case 4:
                    return 0x20;

                case 5:
                    return 0x10;

                case 6:
                case 7:
                case 8:
                    return 0x40;

                case 0x19:
                    return 0x40;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public int GetPrecision()
        {
            switch (base.TypeCode)
            {
                case -6:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 0x19:
                    return this._typeWidth;

                case 2:
                case 3:
                    return (int) base.Precision;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override int GetPrecisionRadix()
        {
            if ((base.TypeCode != 3) && (base.TypeCode != 2))
            {
                return 2;
            }
            return 10;
        }

        public override bool IsExactNumberType()
        {
            if ((base.TypeCode - 6) <= 2)
            {
                return false;
            }
            return true;
        }

        public static bool IsInLongLimits(decimal result)
        {
            decimal num = -9223372036854775808M;
            if (num.CompareTo(result) <= 0)
            {
                num = 9223372036854775807M;
                return (num.CompareTo(result) >= 0);
            }
            return false;
        }

        public static bool IsInLongLimits(long result)
        {
            long num = 0x7fffffffffffffffL;
            if (num.CompareTo(result) >= 0)
            {
                ulong num2 = 9223372036854775808L;
                return (-num2.CompareTo(result) <= 0);
            }
            return false;
        }

        public override bool IsIntegralType()
        {
            int typeCode = base.TypeCode;
            if ((typeCode - 2) > 1)
            {
                if ((typeCode - 6) <= 2)
                {
                    return false;
                }
                return true;
            }
            return (base.Scale == 0);
        }

        public bool IsNegative(object a)
        {
            if (a == null)
            {
                return false;
            }
            switch (base.TypeCode)
            {
                case -6:
                case 4:
                case 5:
                    return (Convert.ToInt32(a, CultureInfo.InvariantCulture) < 0);

                case 2:
                case 3:
                    return (((decimal) a) < decimal.Zero);

                case 6:
                case 7:
                case 8:
                    return (Convert.ToDouble(a, CultureInfo.InvariantCulture) < 0.0);

                case 0x19:
                    return (Convert.ToInt64(a, CultureInfo.InvariantCulture) < 0L);
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override bool IsNumberType()
        {
            return true;
        }

        public static bool IsZero(object a)
        {
            if (a is decimal)
            {
                return (((decimal) a) == decimal.Zero);
            }
            if (!(a is double))
            {
                return (Convert.ToInt64(a, CultureInfo.InvariantCulture) == 0L);
            }
            if (((double) a) != 0.0)
            {
                return double.IsNaN((double) a);
            }
            return true;
        }

        public override object Mod(object a, object b, SqlType aType, SqlType bType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            int typeCode = base.TypeCode;
            switch (typeCode)
            {
                case -6:
                case 4:
                case 5:
                {
                    int num2 = bType.ToInt32(b);
                    if (num2 == 0)
                    {
                        throw Error.GetError(0xd68);
                    }
                    return (aType.ToInt32(a) % num2);
                }
                case -5:
                case -4:
                case -3:
                case -2:
                case -1:
                case 0:
                case 1:
                    break;

                case 2:
                case 3:
                {
                    decimal num3 = aType.ToDecimal(a);
                    decimal num4 = bType.ToDecimal(b);
                    if (num4 == decimal.Zero)
                    {
                        throw Error.GetError(0xd68);
                    }
                    num3 = num3 % num4;
                    return this.ConvertToTypeLimits(null, num3);
                }
                case 6:
                case 7:
                case 8:
                {
                    double num5 = bType.ToReal(b);
                    if (num5 == 0.0)
                    {
                        throw Error.GetError(0xd68);
                    }
                    return (aType.ToReal(a) % num5);
                }
                default:
                    if (typeCode == 0x19)
                    {
                        long num6 = bType.ToInt64(b);
                        if (num6 == 0)
                        {
                            throw Error.GetError(0xd68);
                        }
                        return (aType.ToInt64(a) % num6);
                    }
                    break;
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override object Multiply(object a, object b, SqlType aType, SqlType bType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case -6:
                case 4:
                case 5:
                    return (aType.ToInt32(a) * bType.ToInt32(b));

                case 2:
                case 3:
                {
                    decimal num4 = aType.ToDecimal(a) * bType.ToDecimal(b);
                    return this.ConvertToTypeLimits(null, num4);
                }
                case 6:
                case 7:
                case 8:
                    return (aType.ToReal(a) * bType.ToReal(b));

                case 0x19:
                    return (aType.ToInt64(a) * bType.ToInt64(b));
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override object Negate(object a)
        {
            if (a == null)
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case -6:
                {
                    byte num1 = Convert.ToByte(a, CultureInfo.InvariantCulture);
                    if (num1 == 0)
                    {
                        throw Error.GetError(0xd4b);
                    }
                    return (int) -num1;
                }
                case 2:
                case 3:
                    return -Convert.ToDecimal(a);

                case 4:
                {
                    int num2 = Convert.ToInt32(a, CultureInfo.InvariantCulture);
                    if (num2 == -2147483648)
                    {
                        throw Error.GetError(0xd4b);
                    }
                    return -num2;
                }
                case 5:
                {
                    short num3 = Convert.ToInt16(a, CultureInfo.InvariantCulture);
                    if (num3 == -32768)
                    {
                        throw Error.GetError(0xd4b);
                    }
                    return (int) -num3;
                }
                case 6:
                case 7:
                case 8:
                    return -Convert.ToDouble(a, CultureInfo.InvariantCulture);

                case 0x19:
                {
                    long num4 = Convert.ToInt64(a, CultureInfo.InvariantCulture);
                    if (num4 == -9223372036854775808L)
                    {
                        throw Error.GetError(0xd4b);
                    }
                    return -num4;
                }
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override int PrecedenceDegree(SqlType other)
        {
            if (other.IsNumberType())
            {
                return (((NumberType) other)._typeWidth - this._typeWidth);
            }
            return -2147483648;
        }

        public static long ScaledDecimal(object a, int scale)
        {
            if (a == null)
            {
                return 0L;
            }
            if (scale == 0)
            {
                return 0L;
            }
            decimal d = (decimal) a;
            d = Math.Round(d, 0);
            d = ((decimal) a) - d;
            return (long) (d * ((decimal) Math.Pow(10.0, (double) scale)));
        }

        public override object Subtract(object a, object b, SqlType aType, SqlType bType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case -6:
                case 4:
                case 5:
                    return (aType.ToInt32(a) - bType.ToInt32(b));

                case 2:
                case 3:
                {
                    decimal num3 = aType.ToDecimal(a);
                    decimal num4 = bType.ToDecimal(b);
                    num3 -= num4;
                    return this.ConvertToTypeLimits(null, num3);
                }
                case 6:
                case 7:
                case 8:
                    return (aType.ToReal(a) - bType.ToReal(b));

                case 0x19:
                    return (aType.ToInt64(a) - bType.ToInt64(b));
            }
            throw Error.RuntimeError(0xc9, "NumberType");
        }

        public override decimal ToDecimal(object a)
        {
            try
            {
                switch (base.TypeCode)
                {
                    case 2:
                    case 3:
                        return (decimal) a;

                    case 4:
                    case 5:
                    case -6:
                        break;

                    case 7:
                    case 8:
                        return (decimal) ((double) a);

                    case 0x19:
                        return (long) a;

                    default:
                        return Convert.ToDecimal(a);
                }
                return (int) a;
            }
            catch (Exception)
            {
                return Convert.ToDecimal(a);
            }
        }

        public static double ToDouble(object a)
        {
            return Convert.ToDouble(a);
        }

        public override int ToInt32(object a)
        {
            try
            {
                switch (base.TypeCode)
                {
                    case 2:
                    case 3:
                        return (int) ((decimal) a);

                    case 4:
                    case 5:
                    case -6:
                        break;

                    case 7:
                    case 8:
                        return (int) ((double) a);

                    case 0x19:
                        return (int) ((long) a);

                    default:
                        return Convert.ToInt32(a);
                }
                return (int) a;
            }
            catch (Exception)
            {
                return Convert.ToInt32(a);
            }
        }

        public override long ToInt64(object a)
        {
            try
            {
                switch (base.TypeCode)
                {
                    case 2:
                    case 3:
                        return (long) ((decimal) a);

                    case 4:
                    case 5:
                    case -6:
                        break;

                    case 7:
                    case 8:
                        return (long) ((double) a);

                    case 0x19:
                        return (long) a;

                    default:
                        return Convert.ToInt64(a);
                }
                return (int) a;
            }
            catch (Exception)
            {
                return Convert.ToInt64(a);
            }
        }

        public override double ToReal(object a)
        {
            try
            {
                switch (base.TypeCode)
                {
                    case 2:
                    case 3:
                        return (double) ((decimal) a);

                    case 4:
                    case 5:
                    case -6:
                        break;

                    case 7:
                    case 8:
                        return (double) a;

                    case 0x19:
                        return (double) ((long) a);

                    default:
                        return Convert.ToDouble(a);
                }
                return (int) a;
            }
            catch (Exception)
            {
                return Convert.ToDouble(a);
            }
        }

        public object Truncate(object a, int s)
        {
            if (a == null)
            {
                return null;
            }
            if (s >= base.Scale)
            {
                return a;
            }
            if (s == 0)
            {
                if (base.TypeCode == 8)
                {
                    return Math.Truncate(ConvertToDouble(a));
                }
                decimal num = Math.Truncate(ConvertToDecimal(a));
                return this.ConvertToType(null, num, SqlType.SqlDecimalDefault);
            }
            decimal num2 = ConvertToDecimal(a);
            if (s < 0)
            {
                double num3 = Math.Pow(10.0, -((double) s));
                return ConvertToDecimal(Math.Truncate((double) (((double) num2) / num3)) * num3);
            }
            StringBuilder builder1 = new StringBuilder("{0:0.");
            builder1.Append("".PadRight(s + 1, '0'));
            builder1.Append("}");
            string str = string.Format(builder1.ToString(), num2);
            a = ConvertToDecimal(str.Substring(0, str.Length - 1));
            return this.ConvertToTypeLimits(null, a);
        }
    }
}

