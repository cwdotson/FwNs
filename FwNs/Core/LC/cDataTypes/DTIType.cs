namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class DTIType : SqlType
    {
        public const int TimezoneHour = 0x101;
        public const int TimezoneMinute = 0x102;
        public const int DayOfWeek = 0x103;
        public const int DayOfMonth = 260;
        public const int DayOfYear = 0x105;
        public const int WeekOfYear = 0x106;
        public const int Quarter = 0x107;
        public const int DayName = 0x108;
        public const int MonthName = 0x109;
        public const int SecondsMidnight = 0x10a;
        public const int DefaultTimestampFractionPrecision = 6;
        public const int DefaultIntervalPrecision = 3;
        public const int DefaultIntervalFractionPrecision = 6;
        public const int MaxIntervalPrecision = 9;
        public const int MaxFractionPrecision = 9;
        public const int LimitNanoseconds = 0x3b9aca00;
        public static byte[] YearToSecondSeparators = new byte[] { 0x2d, 0x2d, 0x20, 0x3a, 0x3a, 0x2e };
        public static int[] YearToSecondFactors = new int[] { 12, 1, 0x15180, 0xe10, 60, 1, 0 };
        public static int[] YearToSecondLimits = new int[] { 0, 12, 0, 0x18, 60, 60, 0x3b9aca00 };
        public static int IntervalMonthIndex = 1;
        public static int IntervalFractionPartIndex = 6;
        public static long[] PrecisionLimits = new long[] { 1L, 10L, 100L, 0x3e8L, 0x2710L, 0x186a0L, 0xf4240L, 0x989680L, 0x5f5e100L, 0x3b9aca00L, 0x2540be400L, 0x174876e800L, 0xe8d4a51000L };
        public static int[] PrecisionFactors = new int[] { 0x5f5e100, 0x989680, 0xf4240, 0x186a0, 0x2710, 0x3e8, 100, 10, 1 };
        public static int[] NanoScaleFactors = new int[] { 0x3b9aca00, 0x5f5e100, 0x989680, 0xf4240, 0x186a0, 0x2710, 0x3e8, 100, 10, 1 };
        public static int TimezoneSecondsLimit = 0xc4e0;
        protected static int[] IntervalParts = new int[] { 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a };
        protected static int[][] IntervalTypes;
        private static readonly Dictionary<int, int> IntervalIndexMap;
        public static int DefaultTimeFractionPrecision;
        public int EndIntervalType;
        public int EndPartIndex;
        public int StartIntervalType;
        public int StartPartIndex;

        static DTIType()
        {
            int[][] numArray = new int[6][];
            int index = 0;
            int[] numArray2 = new int[6];
            numArray2[0] = 0x65;
            numArray2[1] = 0x6b;
            numArray[index] = numArray2;
            int num2 = 1;
            int[] numArray3 = new int[6];
            numArray3[1] = 0x66;
            numArray[num2] = numArray3;
            numArray[2] = new int[] { 0, 0, 0x67, 0x6c, 0x6d, 110 };
            numArray[3] = new int[] { 0, 0, 0, 0x68, 0x6f, 0x70 };
            int[] numArray1 = new int[6];
            numArray1[4] = 0x69;
            numArray1[5] = 0x71;
            numArray[4] = numArray1;
            int[] numArray4 = new int[6];
            numArray4[5] = 0x6a;
            numArray[5] = numArray4;
            IntervalTypes = numArray;
            IntervalIndexMap = GetIntervalIndexMap();
        }

        protected DTIType(int typeGroup, int type, long precision, int scale) : base(typeGroup, type, precision, scale)
        {
            switch (type)
            {
                case 0x5b:
                    this.StartIntervalType = 0x65;
                    this.EndIntervalType = 0x6a;
                    break;

                case 0x5c:
                case 0x5e:
                    this.StartIntervalType = 0x68;
                    this.EndIntervalType = 0x6a;
                    break;

                case 0x5d:
                case 0x5f:
                    this.StartIntervalType = 0x65;
                    this.EndIntervalType = 0x6a;
                    break;

                default:
                    throw Error.RuntimeError(0xc9, "DTIType");
            }
            this.StartPartIndex = IntervalIndexMap[this.StartIntervalType];
            this.EndPartIndex = IntervalIndexMap[this.EndIntervalType];
        }

        protected DTIType(int typeGroup, int type, long precision, int scale, int startIntervalType, int endIntervalType) : base(typeGroup, type, precision, scale)
        {
            this.StartIntervalType = startIntervalType;
            this.EndIntervalType = endIntervalType;
            this.StartPartIndex = IntervalIndexMap[startIntervalType];
            this.EndPartIndex = IntervalIndexMap[endIntervalType];
        }

        public virtual int GetEndIntervalType()
        {
            return this.EndIntervalType;
        }

        public SqlType GetExtractType(int part)
        {
            if ((part - 0x65) > 4)
            {
                switch (part)
                {
                    case 0x101:
                    case 0x102:
                        if ((base.TypeCode != 0x5f) && (base.TypeCode != 0x5e))
                        {
                            throw Error.GetError(0x15b9);
                        }
                        return SqlType.SqlInteger;

                    case 0x103:
                    case 260:
                    case 0x105:
                    case 0x106:
                    case 0x107:
                    case 0x108:
                    case 0x109:
                        if (!this.IsDateTimeType() || (this.StartIntervalType != 0x65))
                        {
                            throw Error.GetError(0x15b9);
                        }
                        if ((part != 0x108) && (part != 0x109))
                        {
                            return SqlType.SqlInteger;
                        }
                        return SqlType.SqlVarchar;

                    case 0x10a:
                        if (!this.IsDateTimeType() || (this.EndIntervalType < 0x6a))
                        {
                            throw Error.GetError(0x15b9);
                        }
                        return SqlType.SqlInteger;

                    case 0x6a:
                        if (part != this.StartIntervalType)
                        {
                            if ((part == this.EndIntervalType) && (base.Scale != 0))
                            {
                                return new NumberType(3, (long) (9 + base.Scale), base.Scale);
                            }
                        }
                        else if (base.Scale != 0)
                        {
                            return new NumberType(3, base.Precision + base.Scale, base.Scale);
                        }
                        break;

                    default:
                        throw Error.RuntimeError(0xc9, "DTIType");
                }
            }
            if ((part < this.StartIntervalType) || (part > this.EndIntervalType))
            {
                throw Error.GetError(0x15b9);
            }
            return SqlType.SqlInteger;
        }

        public static string GetFieldNameTokenForType(int type)
        {
            switch (type)
            {
                case 0x65:
                    return "YEAR";

                case 0x66:
                    return "MONTH";

                case 0x67:
                    return "DAY";

                case 0x68:
                    return "HOUR";

                case 0x69:
                    return "MINUTE";

                case 0x6a:
                    return "SECOND";

                case 0x101:
                    return "TIMEZONE_HOUR";

                case 0x102:
                    return "TIMEZONE_MINUTE";

                case 0x103:
                    return "DAY_OF_WEEK";

                case 260:
                    return "DAY_OF_MONTH";

                case 0x105:
                    return "DAY_OF_YEAR";

                case 0x106:
                    return "WEEK_OF_YEAR";

                case 0x107:
                    return "QUARTER";

                case 0x108:
                    return "DAY_NAME";

                case 0x109:
                    return "MONTH_NAME";

                case 0x10a:
                    return "SECONDS_SINCE_MIDNIGHT";
            }
            throw Error.RuntimeError(0xc9, "DTIType");
        }

        public static int GetFieldNameTypeForToken(int token)
        {
            if (token <= 0x11a)
            {
                if (token > 0xa7)
                {
                    if (token == 0xab)
                    {
                        return 0x66;
                    }
                    if (token == 0xf8)
                    {
                        return 0x6a;
                    }
                    if (token == 0x119)
                    {
                        return 0x101;
                    }
                    if (token == 0x11a)
                    {
                        return 0x102;
                    }
                }
                else
                {
                    if (token == 0x48)
                    {
                        return 0x67;
                    }
                    if (token == 0x7e)
                    {
                        return 0x68;
                    }
                    if (token == 0xa7)
                    {
                        return 0x69;
                    }
                }
            }
            else if (token <= 0x281)
            {
                switch (token)
                {
                    case 0x26b:
                        return 0x108;

                    case 620:
                        return 260;

                    case 0x26d:
                        return 0x103;

                    case 0x26e:
                        return 0x105;

                    case 0x141:
                        return 0x65;

                    case 0x281:
                        return 0x109;
                }
            }
            else
            {
                if (token == 0x289)
                {
                    return 0x107;
                }
                if (token == 0x292)
                {
                    return 0x10a;
                }
                if (token == 0x2a1)
                {
                    return 0x106;
                }
            }
            throw Error.RuntimeError(0xc9, "DTIType");
        }

        private static Dictionary<int, int> GetIntervalIndexMap()
        {
            return new Dictionary<int, int> { 
                { 
                    0x65,
                    0
                },
                { 
                    0x66,
                    1
                },
                { 
                    0x67,
                    2
                },
                { 
                    0x68,
                    3
                },
                { 
                    0x69,
                    4
                },
                { 
                    0x6a,
                    5
                }
            };
        }

        public abstract int GetPart(Session session, object dateTime, int part);
        public static int GetPrecisionExponent(long value)
        {
            int index = 1;
            while ((index < PrecisionLimits.Length) && (value >= PrecisionLimits[index]))
            {
                index++;
            }
            return index;
        }

        public abstract decimal GetSecondPart(object dateTime);
        public decimal GetSecondPart(long seconds, long nanos)
        {
            seconds *= PrecisionLimits[base.Scale];
            seconds += nanos / ((long) NanoScaleFactors[base.Scale]);
            return Math.Round((decimal) seconds, base.Scale);
        }

        public virtual int GetStartIntervalType()
        {
            return this.StartIntervalType;
        }

        public string IntervalSecondToString(long seconds, int nanos, bool signed)
        {
            StringBuilder builder = new StringBuilder(0x40);
            if (seconds < 0L)
            {
                seconds = -seconds;
                builder.Append('-');
            }
            else if (signed)
            {
                builder.Append('+');
            }
            for (int i = this.StartPartIndex; i <= this.EndPartIndex; i++)
            {
                int num2 = YearToSecondFactors[i];
                long num3 = seconds / ((long) num2);
                if ((i != this.StartPartIndex) && (num3 < 10L))
                {
                    builder.Append('0');
                }
                builder.Append(num3);
                seconds = seconds % ((long) num2);
                if (i < this.EndPartIndex)
                {
                    builder.Append((char) YearToSecondSeparators[i]);
                }
            }
            if (base.Scale != 0)
            {
                builder.Append((char) YearToSecondSeparators[IntervalFractionPartIndex - 1]);
            }
            if (nanos < 0)
            {
                nanos = -nanos;
            }
            for (int j = 0; j < base.Scale; j++)
            {
                int num5 = nanos / PrecisionFactors[j];
                nanos -= num5 * PrecisionFactors[j];
                builder.Append(num5);
            }
            return builder.ToString();
        }

        public static bool IsValidDatetimeRange(SqlType a, SqlType b)
        {
            if (!a.IsDateTimeType())
            {
                return false;
            }
            if (b.IsDateTimeType())
            {
                return (((a.TypeCode != 0x5c) || (b.TypeCode != 0x5b)) && ((a.TypeCode != 0x5b) || (b.TypeCode != 0x5c)));
            }
            return (b.IsIntervalType() && ((DateTimeType) a).CanAdd((IntervalType) b));
        }

        public static int NormaliseFraction(int fraction, int precision)
        {
            return ((fraction / NanoScaleFactors[precision]) * NanoScaleFactors[precision]);
        }
    }
}

