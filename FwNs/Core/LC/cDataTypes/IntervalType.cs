namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cParsing;
    using System;
    using System.Data.LibCore;
    using System.Text;

    public sealed class IntervalType : DTIType
    {
        private readonly bool _isYearMonth;
        public readonly bool DefaultPrecision;

        private IntervalType(int typeGroup, int type, long precision, int scale, int startIntervalType, int endIntervalType, bool defaultPrecision) : base(typeGroup, type, precision, scale, startIntervalType, endIntervalType)
        {
            if ((startIntervalType - 0x65) <= 1)
            {
                this._isYearMonth = true;
            }
            else
            {
                this._isYearMonth = false;
            }
            this.DefaultPrecision = defaultPrecision;
        }

        public override object Absolute(object a)
        {
            if (a == null)
            {
                return null;
            }
            IntervalMonthData data = a as IntervalMonthData;
            if (data != null)
            {
                if (data.Units < 0L)
                {
                    return this.Negate(a);
                }
            }
            else
            {
                IntervalSecondData data2 = (IntervalSecondData) a;
                if ((data2.Units < 0L) || (data2.Nanos < 0))
                {
                    return this.Negate(a);
                }
            }
            return a;
        }

        public override bool AcceptsFractionalPrecision()
        {
            return (base.EndIntervalType == 0x6a);
        }

        public override bool AcceptsPrecision()
        {
            return true;
        }

        public override object Add(object a, object b, SqlType aType, SqlType otherType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case 0x65:
                case 0x66:
                case 0x6b:
                    return new IntervalMonthData(((IntervalMonthData) a).Units + ((IntervalMonthData) b).Units, this);

                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                {
                    IntervalSecondData data = (IntervalSecondData) b;
                    IntervalSecondData data1 = (IntervalSecondData) a;
                    long seconds = data1.Units + data.Units;
                    return new IntervalSecondData(seconds, data1.Nanos + data.Nanos, this, true);
                }
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            return ((((othType.TypeCode == 0) || othType.IsCharacterType()) || othType.IsNumberType()) || (othType.IsIntervalType() && (this.IsYearMonthIntervalType() == ((IntervalType) othType).IsYearMonthIntervalType())));
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
            if (b == null)
            {
                return 1;
            }
            switch (base.TypeCode)
            {
                case 0x65:
                case 0x66:
                case 0x6b:
                    return ((IntervalMonthData) a).CompareTo((IntervalMonthData) b);

                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                    return ((IntervalSecondData) a).CompareTo((IntervalSecondData) b);
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override object ConvertCSharpToSQL(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            int typeCode = base.TypeCode;
            if (((typeCode - 0x67) <= 3) || ((typeCode - 0x6c) <= 5))
            {
                TimeSpan? nullable = a as TimeSpan?;
                if (nullable.HasValue)
                {
                    return new IntervalSecondData(((long) ((int) nullable.Value.Ticks)) / 0x989680L, (int) ((nullable.Value.Ticks % 0x989680L) * 1000000000.0));
                }
            }
            return base.ConvertCSharpToSQL(session, a);
        }

        public override object ConvertSQLToCSharp(ISessionInterface session, object a)
        {
            switch (base.TypeCode)
            {
                case 0x65:
                case 0x66:
                case 0x6b:
                    return new MonthSpan((int) ((IntervalMonthData) a).Units);

                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                {
                    IntervalSecondData data = (IntervalSecondData) a;
                    return new TimeSpan((((int) data.Units) * 0x989680L) + (data.Nanos / 100));
                }
            }
            return base.ConvertSQLToCSharp(session, a);
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            if (!(a is string))
            {
                throw Error.GetError(0x15b9);
            }
            return this.ConvertToType(null, a, SqlType.SqlVarchar);
        }

        public long ConvertToLong(object interval)
        {
            int endIntervalType = base.EndIntervalType;
            if ((endIntervalType - 0x65) > 1)
            {
                if ((endIntervalType - 0x67) > 3)
                {
                    throw Error.RuntimeError(0xc9, "IntervalType");
                }
            }
            else
            {
                return (((IntervalMonthData) interval).Units / ((long) DTIType.YearToSecondFactors[base.EndPartIndex]));
            }
            return (((IntervalSecondData) interval).Units / ((long) DTIType.YearToSecondFactors[base.EndPartIndex]));
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            StringBuilder builder1 = new StringBuilder(0x20);
            builder1.Append("INTERVAL").Append(' ');
            builder1.Append('\'').Append(this.ConvertToString(a)).Append('\'').Append(' ');
            builder1.Append(Tokens.SQL_INTERVAL_FIELD_NAMES[base.StartPartIndex]);
            builder1.Append(' ');
            builder1.Append("TO");
            builder1.Append(' ');
            builder1.Append(Tokens.SQL_INTERVAL_FIELD_NAMES[base.EndPartIndex]);
            return builder1.ToString();
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case 0x65:
                case 0x66:
                case 0x6b:
                    return this.IntervalMonthToString(a);

                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                    return this.IntervalSecondToString(a);
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            long units;
            int nanos;
            if (a == null)
            {
                return null;
            }
            int typeCode = othType.TypeCode;
            if (typeCode > 0x19)
            {
                switch (typeCode)
                {
                    case 100:
                        goto Label_023D;

                    case 0x65:
                        return new IntervalMonthData((((IntervalMonthData) a).Units / 12L) * 12L, this);

                    case 0x66:
                    case 0x6b:
                        return new IntervalMonthData(((IntervalMonthData) a).Units, this);

                    case 0x67:
                        return new IntervalSecondData((((IntervalSecondData) a).Units / ((long) DTIType.YearToSecondFactors[2])) * DTIType.YearToSecondFactors[2], 0, this);

                    case 0x68:
                    case 0x69:
                    case 0x6c:
                    case 0x6d:
                    case 0x6f:
                        return new IntervalSecondData((((IntervalSecondData) a).Units / ((long) DTIType.YearToSecondFactors[base.EndPartIndex])) * DTIType.YearToSecondFactors[base.EndPartIndex], 0, this);

                    case 0x6a:
                    case 110:
                    case 0x70:
                    case 0x71:
                    {
                        IntervalSecondData data1 = (IntervalSecondData) a;
                        units = data1.Units;
                        nanos = data1.Nanos;
                        if (base.Scale != 0)
                        {
                            nanos = (nanos / DTIType.NanoScaleFactors[base.Scale]) * DTIType.NanoScaleFactors[base.Scale];
                        }
                        else
                        {
                            nanos = 0;
                        }
                        goto Label_022A;
                    }
                    case 40:
                        a = a.ToString();
                        goto Label_023D;
                }
                goto Label_0251;
            }
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
                case 0x19:
                {
                    if ((a is decimal) && !NumberType.IsInLongLimits((decimal) a))
                    {
                        throw Error.GetError(0xd6b);
                    }
                    long years = Convert.ToInt64(a);
                    switch (base.EndIntervalType)
                    {
                        case 0x65:
                            return IntervalMonthData.NewIntervalYear(years, this);

                        case 0x66:
                            return IntervalMonthData.NewIntervalMonth(years, this);

                        case 0x67:
                            return IntervalSecondData.NewIntervalDay(years, this);

                        case 0x68:
                            return IntervalSecondData.NewIntervalHour(years, this);

                        case 0x69:
                            return IntervalSecondData.NewIntervalMinute(years, this);

                        case 0x6a:
                        {
                            int nanos = 0;
                            if ((base.Scale > 0) && (a is decimal))
                            {
                                nanos = (int) NumberType.ScaledDecimal(a, 9);
                            }
                            return new IntervalSecondData(years, nanos, this);
                        }
                    }
                    throw Error.GetError(0x15b9);
                }
                case 1:
                case 12:
                    goto Label_023D;

                default:
                    goto Label_0251;
            }
        Label_022A:
            return new IntervalSecondData(units, nanos, this);
        Label_023D:
            return session.GetScanner().ConvertToDatetimeInterval(session, (string) a, this);
        Label_0251:
            throw Error.GetError(0x15b9);
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            IntervalMonthData data = a as IntervalMonthData;
            if (data != null)
            {
                if (data.Units > this.GetIntervalValueLimit())
                {
                    throw Error.GetError(0xd6b);
                }
            }
            else
            {
                IntervalSecondData data2 = a as IntervalSecondData;
                if ((data2 != null) && (data2.Units > this.GetIntervalValueLimit()))
                {
                    throw Error.GetError(0xd6b);
                }
            }
            return a;
        }

        public override int DisplaySize()
        {
            switch (base.TypeCode)
            {
                case 0x65:
                    return (((int) base.Precision) + 1);

                case 0x66:
                    return (((int) base.Precision) + 1);

                case 0x67:
                    return (((int) base.Precision) + 1);

                case 0x68:
                    return (((int) base.Precision) + 1);

                case 0x69:
                    return (((int) base.Precision) + 1);

                case 0x6a:
                    return ((((int) base.Precision) + 1) + ((base.Scale == 0) ? 0 : (base.Scale + 1)));

                case 0x6b:
                    return (((int) base.Precision) + 4);

                case 0x6c:
                    return (((int) base.Precision) + 4);

                case 0x6d:
                    return (((int) base.Precision) + 7);

                case 110:
                    return ((((int) base.Precision) + 10) + ((base.Scale == 0) ? 0 : (base.Scale + 1)));

                case 0x6f:
                    return (((int) base.Precision) + 4);

                case 0x70:
                    return ((((int) base.Precision) + 7) + ((base.Scale == 0) ? 0 : (base.Scale + 1)));

                case 0x71:
                    return ((((int) base.Precision) + 4) + ((base.Scale == 0) ? 0 : (base.Scale + 1)));
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override object Divide(object a, object b, SqlType aType, SqlType bType)
        {
            return this.MultiplyOrDivide(a, b, true);
        }

        public override int GetAdoPrecision()
        {
            return this.DisplaySize();
        }

        public override int GetAdoTypeCode()
        {
            switch (base.TypeCode)
            {
                case 0x65:
                case 0x66:
                case 0x6b:
                    return 0x13;

                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                    return 0x12;
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            SqlType type3;
            if (base.TypeCode == other.TypeCode)
            {
                if ((base.Precision >= other.Precision) && (base.Scale >= other.Scale))
                {
                    return this;
                }
                if ((base.Precision <= other.Precision) && (base.Scale <= other.Scale))
                {
                    return other;
                }
            }
            if (other == SqlType.SqlAllTypes)
            {
                return this;
            }
            if (other.IsCharacterType())
            {
                return other.GetAggregateType(this);
            }
            if (!other.IsIntervalType())
            {
                throw Error.GetError(0x15ba);
            }
            IntervalType type2 = (IntervalType) other;
            int startType = (type2.StartIntervalType > base.StartIntervalType) ? base.StartIntervalType : type2.StartIntervalType;
            int endType = (type2.EndIntervalType > base.EndIntervalType) ? type2.EndIntervalType : base.EndIntervalType;
            int combinedIntervalType = GetCombinedIntervalType(startType, endType);
            long precision = (base.Precision > other.Precision) ? base.Precision : other.Precision;
            int fractionPrecision = (base.Scale > other.Scale) ? base.Scale : other.Scale;
            try
            {
                type3 = GetIntervalType(combinedIntervalType, startType, endType, precision, fractionPrecision, false);
            }
            catch (Exception)
            {
                throw Error.GetError(0x15ba);
            }
            return type3;
        }

        public static SqlType GetCombinedIntervalType(IntervalType type1, IntervalType type2)
        {
            int startType = (type2.StartIntervalType > type1.StartIntervalType) ? type1.StartIntervalType : type2.StartIntervalType;
            int endType = (type2.EndIntervalType > type1.EndIntervalType) ? type2.EndIntervalType : type1.EndIntervalType;
            long precision = (type1.Precision > type2.Precision) ? type1.Precision : type2.Precision;
            int fractionPrecision = (type1.Scale > type2.Scale) ? type1.Scale : type2.Scale;
            return GetIntervalType(GetCombinedIntervalType(startType, endType), startType, endType, precision, fractionPrecision, false);
        }

        public static int GetCombinedIntervalType(int startType, int endType)
        {
            if (startType == endType)
            {
                return startType;
            }
            switch (startType)
            {
                case 0x65:
                    if (endType != 0x66)
                    {
                        break;
                    }
                    return 0x6b;

                case 0x67:
                    switch (endType)
                    {
                        case 0x68:
                            return 0x6c;

                        case 0x69:
                            return 0x6d;

                        case 0x6a:
                            return 110;
                    }
                    break;

                case 0x68:
                    if (endType == 0x69)
                    {
                        return 0x6f;
                    }
                    if (endType == 0x6a)
                    {
                        return 0x70;
                    }
                    break;

                case 0x69:
                    if (endType != 0x6a)
                    {
                        break;
                    }
                    return 0x71;
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            switch (operation)
            {
                case 0x20:
                    if (!other.IsDateTimeType())
                    {
                        if (other.IsIntervalType())
                        {
                            IntervalType aggregateType = (IntervalType) this.GetAggregateType(other);
                            return GetIntervalType(aggregateType, 9L, aggregateType.Scale);
                        }
                        break;
                    }
                    return other.GetCombinedType(this, operation);

                case 0x22:
                    if (!other.IsNumberType())
                    {
                        break;
                    }
                    return GetIntervalType(this, 9L, base.Scale);

                case 0x23:
                    if (!other.IsNumberType())
                    {
                        break;
                    }
                    return this;

                default:
                    return this.GetAggregateType(other);
            }
            throw Error.GetError(0x15ba);
        }

        public override Type GetCSharpClass()
        {
            switch (base.TypeCode)
            {
                case 0x65:
                case 0x66:
                case 0x6b:
                    return typeof(IntervalMonthData);

                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                    return typeof(IntervalSecondData);
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override string GetCSharpClassName()
        {
            switch (base.TypeCode)
            {
                case 0x65:
                case 0x66:
                case 0x6b:
                    return typeof(IntervalMonthData).FullName;

                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                    return typeof(IntervalSecondData).FullName;
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override string GetDefinition()
        {
            if ((base.Precision == 3L) && ((base.EndIntervalType != 0x6a) || (base.Scale == 6)))
            {
                return this.GetNameString();
            }
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("INTERVAL").Append(' ');
            builder.Append(GetQualifier(base.StartIntervalType));
            if (base.TypeCode == 0x6a)
            {
                builder.Append('(');
                builder.Append(base.Precision);
                if (base.Scale != 6)
                {
                    builder.Append(',');
                    builder.Append(base.Scale);
                }
                builder.Append(')');
                return builder.ToString();
            }
            if (base.Precision != 3L)
            {
                builder.Append('(');
                builder.Append(base.Precision);
                builder.Append(')');
            }
            if (base.StartIntervalType != base.EndIntervalType)
            {
                builder.Append(' ');
                builder.Append("TO");
                builder.Append(' ');
                builder.Append(Tokens.SQL_INTERVAL_FIELD_NAMES[base.EndPartIndex]);
                if ((base.EndIntervalType == 0x6a) && (base.Scale != 6))
                {
                    builder.Append('(');
                    builder.Append(base.Scale);
                    builder.Append(')');
                }
            }
            return builder.ToString();
        }

        public override int GetEndIntervalType()
        {
            return base.EndIntervalType;
        }

        public static int GetEndIntervalType(int type)
        {
            switch (type)
            {
                case 0x65:
                    return 0x65;

                case 0x66:
                    return 0x66;

                case 0x67:
                    return 0x67;

                case 0x68:
                    return 0x68;

                case 0x69:
                    return 0x69;

                case 0x6a:
                    return 0x6a;

                case 0x6b:
                    return 0x66;

                case 0x6c:
                    return 0x68;

                case 0x6d:
                    return 0x69;

                case 110:
                    return 0x6a;

                case 0x6f:
                    return 0x69;

                case 0x70:
                    return 0x6a;

                case 0x71:
                    return 0x6a;
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public static IntervalType GetIntervalType(IntervalType type, long precision, int fractionalPrecision)
        {
            if ((type.Precision >= precision) && (type.Scale >= fractionalPrecision))
            {
                return type;
            }
            return GetIntervalType(type.TypeCode, precision, fractionalPrecision);
        }

        public static IntervalType GetIntervalType(int type, long precision, int fractionPrecision)
        {
            int startIntervalType = GetStartIntervalType(type);
            int endIntervalType = GetEndIntervalType(type);
            return GetIntervalType(type, startIntervalType, endIntervalType, precision, fractionPrecision, false);
        }

        public static IntervalType GetIntervalType(int startIndex, int endIndex, long precision, int fractionPrecision)
        {
            bool defaultPrecision = precision == -1L;
            if ((startIndex == -1) || (endIndex == -1))
            {
                throw Error.GetError(0xd4e);
            }
            if (startIndex > endIndex)
            {
                throw Error.GetError(0xd4e);
            }
            if ((startIndex <= DTIType.IntervalMonthIndex) && (endIndex > DTIType.IntervalMonthIndex))
            {
                throw Error.GetError(0xd4e);
            }
            int startType = DTIType.IntervalParts[startIndex];
            int endType = DTIType.IntervalParts[endIndex];
            if (((precision == 0) || (precision > 9L)) || (fractionPrecision > 9))
            {
                throw Error.GetError(0x15d8);
            }
            if (precision == -1L)
            {
                precision = 3L;
            }
            if (fractionPrecision == -1)
            {
                fractionPrecision = (endType == 0x6a) ? 6 : 0;
            }
            return GetIntervalType(DTIType.IntervalTypes[startIndex][endIndex], startType, endType, precision, fractionPrecision, defaultPrecision);
        }

        public static IntervalType GetIntervalType(int type, int startType, int endType, long precision, int fractionPrecision, bool defaultPrecision)
        {
            int typeGroup = (startType > 0x66) ? 0x6a : 0x66;
            if (!defaultPrecision)
            {
                switch (type)
                {
                    case 0x65:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalYear;

                    case 0x66:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalMonth;

                    case 0x67:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalDay;

                    case 0x68:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalHour;

                    case 0x69:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalMinute;

                    case 0x6a:
                        if ((precision != 3L) || (fractionPrecision != 6))
                        {
                            break;
                        }
                        return SqlType.SqlIntervalSecond;

                    case 0x6b:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalYearToMonth;

                    case 0x6c:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalDayToHour;

                    case 0x6d:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalDayToMinute;

                    case 110:
                        if ((precision != 3L) || (fractionPrecision != 6))
                        {
                            break;
                        }
                        return SqlType.SqlIntervalDayToSecond;

                    case 0x6f:
                        if (precision != 3L)
                        {
                            break;
                        }
                        return SqlType.SqlIntervalHourToMinute;

                    case 0x70:
                        if ((precision != 3L) || (fractionPrecision != 6))
                        {
                            break;
                        }
                        return SqlType.SqlIntervalHourToSecond;

                    case 0x71:
                        if ((precision != 3L) || (fractionPrecision != 6))
                        {
                            break;
                        }
                        return SqlType.SqlIntervalMinuteToSecond;

                    default:
                        throw Error.RuntimeError(0xc9, "IntervalType");
                }
            }
            return new IntervalType(typeGroup, type, precision, fractionPrecision, startType, endType, defaultPrecision);
        }

        public long GetIntervalValueLimit()
        {
            switch (base.TypeCode)
            {
                case 0x65:
                    return (DTIType.PrecisionLimits[(int) base.Precision] * 12L);

                case 0x66:
                    return DTIType.PrecisionLimits[(int) base.Precision];

                case 0x67:
                    return (((DTIType.PrecisionLimits[(int) base.Precision] * 0x18L) * 60L) * 60L);

                case 0x68:
                    return ((DTIType.PrecisionLimits[(int) base.Precision] * 60L) * 60L);

                case 0x69:
                    return (DTIType.PrecisionLimits[(int) base.Precision] * 60L);

                case 0x6a:
                    return DTIType.PrecisionLimits[(int) base.Precision];

                case 0x6b:
                {
                    long num = DTIType.PrecisionLimits[(int) base.Precision] * 12L;
                    return (num + 12L);
                }
                case 0x6c:
                    return (((DTIType.PrecisionLimits[(int) base.Precision] * 0x18L) * 60L) * 60L);

                case 0x6d:
                    return (((DTIType.PrecisionLimits[(int) base.Precision] * 0x18L) * 60L) * 60L);

                case 110:
                    return (((DTIType.PrecisionLimits[(int) base.Precision] * 0x18L) * 60L) * 60L);

                case 0x6f:
                    return ((DTIType.PrecisionLimits[(int) base.Precision] * 60L) * 60L);

                case 0x70:
                    return ((DTIType.PrecisionLimits[(int) base.Precision] * 60L) * 60L);

                case 0x71:
                    return (DTIType.PrecisionLimits[(int) base.Precision] * 60L);
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override string GetNameString()
        {
            return ("INTERVAL " + GetQualifier(base.TypeCode));
        }

        public override int GetPart(Session session, object interval, int part)
        {
            switch (part)
            {
                case 0x65:
                    return (int) (((IntervalMonthData) interval).Units / 12L);

                case 0x66:
                {
                    long units = ((IntervalMonthData) interval).Units;
                    if (part == base.StartIntervalType)
                    {
                        return (int) units;
                    }
                    return (int) (units % 12L);
                }
                case 0x67:
                    return (int) (((IntervalSecondData) interval).Units / 0x15180L);

                case 0x68:
                {
                    long num3 = ((IntervalSecondData) interval).Units / 0xe10L;
                    if (part == base.StartIntervalType)
                    {
                        return (int) num3;
                    }
                    return (int) (num3 % 0x18L);
                }
                case 0x69:
                {
                    long num4 = ((IntervalSecondData) interval).Units / 60L;
                    if (part == base.StartIntervalType)
                    {
                        return (int) num4;
                    }
                    return (int) (num4 % 60L);
                }
                case 0x6a:
                {
                    long units = ((IntervalSecondData) interval).Units;
                    if (part == base.StartIntervalType)
                    {
                        return (int) units;
                    }
                    return (int) (units % 60L);
                }
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public static string GetQualifier(int type)
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

                case 0x6b:
                    return "YEAR TO MONTH";

                case 0x6c:
                    return "DAY TO HOUR";

                case 0x6d:
                    return "DAY TO MINUTE";

                case 110:
                    return "DAY TO SECOND";

                case 0x6f:
                    return "HOUR TO MINUTE";

                case 0x70:
                    return "HOUR TO SECOND";

                case 0x71:
                    return "MINUTE TO SECOND";
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        public override decimal GetSecondPart(object interval)
        {
            IntervalSecondData data1 = (IntervalSecondData) interval;
            long units = data1.Units;
            if (base.TypeCode != 0x6a)
            {
                units = units % 60L;
            }
            int nanos = data1.Nanos;
            return base.GetSecondPart(units, (long) nanos);
        }

        public static long GetSeconds(object interval)
        {
            return ((IntervalSecondData) interval).Units;
        }

        public override int GetSqlGenericTypeCode()
        {
            return 10;
        }

        public override int GetStartIntervalType()
        {
            return base.StartIntervalType;
        }

        public static int GetStartIntervalType(int type)
        {
            switch (type)
            {
                case 0x65:
                case 0x6b:
                    return 0x65;

                case 0x66:
                    return 0x66;

                case 0x67:
                case 0x6c:
                case 0x6d:
                case 110:
                    return 0x67;

                case 0x68:
                case 0x6f:
                case 0x70:
                    return 0x68;

                case 0x69:
                case 0x71:
                    return 0x69;

                case 0x6a:
                    return 0x6a;
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }

        private string IntervalMonthToString(object a)
        {
            StringBuilder builder = new StringBuilder(8);
            long units = ((IntervalMonthData) a).Units;
            if (units < 0L)
            {
                units = -units;
                builder.Append('-');
            }
            for (int i = base.StartPartIndex; i <= base.EndPartIndex; i++)
            {
                int num3 = DTIType.YearToSecondFactors[i];
                long num4 = units / ((long) num3);
                if ((i != base.StartPartIndex) && (num4 < 10L))
                {
                    builder.Append('0');
                }
                builder.Append(num4);
                units = units % ((long) num3);
                if (i < base.EndPartIndex)
                {
                    builder.Append((char) DTIType.YearToSecondSeparators[i]);
                }
            }
            return builder.ToString();
        }

        public string IntervalSecondToString(object a)
        {
            IntervalSecondData data1 = (IntervalSecondData) a;
            long units = data1.Units;
            int nanos = data1.Nanos;
            return base.IntervalSecondToString(units, nanos, false);
        }

        public bool IsDaySecondIntervalType()
        {
            int typeCode = base.TypeCode;
            if (((typeCode - 0x67) > 3) && ((typeCode - 0x6c) > 5))
            {
                return false;
            }
            return true;
        }

        public override bool IsIntervalType()
        {
            return true;
        }

        public bool IsYearMonthIntervalType()
        {
            int typeCode = base.TypeCode;
            if (((typeCode - 0x65) > 1) && (typeCode != 0x6b))
            {
                return false;
            }
            return true;
        }

        public override object Multiply(object a, object b, SqlType aType, SqlType bType)
        {
            return this.MultiplyOrDivide(a, b, false);
        }

        private object MultiplyOrDivide(object a, object b, bool divide)
        {
            decimal units;
            if ((a == null) || (b == null))
            {
                return null;
            }
            if (a is ValueType)
            {
                a = b;
                b = a;
            }
            if (divide && NumberType.IsZero(b))
            {
                throw Error.GetError(0xd68);
            }
            NumberType type = NumberType.GetNumberType(3, 40L, 9);
            decimal num = Convert.ToDecimal(type.ConvertToDefaultType(null, b));
            if (this._isYearMonth)
            {
                units = ((IntervalMonthData) a).Units;
            }
            else
            {
                IntervalSecondData data = (IntervalSecondData) a;
                units = data.Units + (data.Nanos / DTIType.NanoScaleFactors[0]);
            }
            decimal result = divide ? ((decimal) type.Divide(units, num, SqlType.SqlDecimal, SqlType.SqlDecimal)) : ((decimal) type.Multiply(units, num, SqlType.SqlDecimal, SqlType.SqlDecimal));
            if (!NumberType.IsInLongLimits(result))
            {
                throw Error.GetError(0xd6b);
            }
            if (this._isYearMonth)
            {
                return new IntervalMonthData((long) result, this);
            }
            int num4 = (int) NumberType.ScaledDecimal(result, 9);
            return new IntervalSecondData((long) result, (long) num4, this, true);
        }

        public override object Negate(object a)
        {
            if (a == null)
            {
                return null;
            }
            IntervalMonthData data = a as IntervalMonthData;
            if (data != null)
            {
                return new IntervalMonthData(-data.Units, this);
            }
            IntervalSecondData data1 = (IntervalSecondData) a;
            long units = data1.Units;
            int nanos = data1.Nanos;
            return new IntervalSecondData(-units, -((long) nanos), this, true);
        }

        public static IntervalType NewIntervalType(int type, long precision, int fractionPrecision)
        {
            int startIntervalType = GetStartIntervalType(type);
            return new IntervalType((startIntervalType > 0x66) ? 0x6a : 0x66, type, precision, fractionPrecision, startIntervalType, GetEndIntervalType(type), false);
        }

        public override int PrecedenceDegree(SqlType other)
        {
            if (other.IsIntervalType())
            {
                return (((IntervalType) other).EndPartIndex - base.EndPartIndex);
            }
            return -2147483648;
        }

        public override object Subtract(object a, object b, SqlType aType, SqlType otherType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case 0x65:
                case 0x66:
                case 0x6b:
                {
                    IntervalMonthData data = a as IntervalMonthData;
                    IntervalMonthData data2 = b as IntervalMonthData;
                    if ((data == null) || (data2 == null))
                    {
                        TimestampData data3 = a as TimestampData;
                        TimestampData data4 = b as TimestampData;
                        if ((data3 == null) || (data4 == null))
                        {
                            throw Error.RuntimeError(0xc9, "IntervalType");
                        }
                        bool isYear = base.TypeCode == 0x65;
                        return new IntervalMonthData((long) DateTimeType.SubtractMonths(data3, data4, isYear), this);
                    }
                    return new IntervalMonthData(data.Units - data2.Units, this);
                }
                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6c:
                case 0x6d:
                case 110:
                case 0x6f:
                case 0x70:
                case 0x71:
                {
                    IntervalSecondData data5 = a as IntervalSecondData;
                    IntervalSecondData data6 = b as IntervalSecondData;
                    if ((data5 == null) || (data6 == null))
                    {
                        TimeData data7 = a as TimeData;
                        TimeData data8 = b as TimeData;
                        if ((data7 != null) && (data8 != null))
                        {
                            long num4 = data7.GetSeconds() - data8.GetSeconds();
                            return new IntervalSecondData(num4, data7.GetNanos() - data8.GetNanos(), this, true);
                        }
                        TimestampData data9 = a as TimestampData;
                        TimestampData data10 = b as TimestampData;
                        if ((data9 != null) && (data10 != null))
                        {
                            long num6 = data9.GetSeconds() - data10.GetSeconds();
                            return new IntervalSecondData(num6, data9.GetNanos() - data10.GetNanos(), this, true);
                        }
                        break;
                    }
                    long seconds = data5.Units - data6.Units;
                    return new IntervalSecondData(seconds, data5.Nanos - data6.Nanos, this, true);
                }
            }
            throw Error.RuntimeError(0xc9, "IntervalType");
        }
    }
}

