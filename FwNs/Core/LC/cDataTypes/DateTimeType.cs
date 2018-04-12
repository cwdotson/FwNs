namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Globalization;
    using System.Text;

    public sealed class DateTimeType : DTIType, IEquatable<DateTimeType>
    {
        private readonly bool _withTimeZone;

        public DateTimeType(int typeGroup, int type, int scale) : base(typeGroup, type, 0L, scale)
        {
            this._withTimeZone = (type == 0x5e) || (type == 0x5f);
        }

        public override bool AcceptsFractionalPrecision()
        {
            return (base.TypeCode != 0x5b);
        }

        public override object Add(object a, object b, SqlType aType, SqlType otherType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case 0x5b:
                {
                    if (!otherType.IsNumberType())
                    {
                        if (otherType.TypeCode != 0x5b)
                        {
                            break;
                        }
                        double num3 = ((double) SqlType.SqlDouble.ConvertToDefaultType(null, a)) * 86400.0;
                        return AddSeconds((TimestampData) b, (int) num3, 0);
                    }
                    double num2 = ((double) SqlType.SqlDouble.ConvertToDefaultType(null, b)) * 86400.0;
                    return AddSeconds((TimestampData) a, (int) num2, 0);
                }
                case 0x5c:
                case 0x5e:
                {
                    if (b is IntervalMonthData)
                    {
                        throw Error.RuntimeError(0xc9, "DateTimeType");
                    }
                    IntervalSecondData data4 = b as IntervalSecondData;
                    if (data4 == null)
                    {
                        goto Label_011F;
                    }
                    return AddSeconds((TimeData) a, (int) data4.Units, data4.Nanos);
                }
                case 0x5d:
                case 0x5f:
                    break;

                default:
                    goto Label_011F;
            }
            TimestampData source = (TimestampData) a;
            IntervalMonthData data2 = b as IntervalMonthData;
            if (data2 != null)
            {
                return AddMonths(source, (int) data2.Units);
            }
            IntervalSecondData data3 = b as IntervalSecondData;
            if (data3 != null)
            {
                return AddSeconds(source, (int) data3.Units, data3.Nanos);
            }
        Label_011F:
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public static TimestampData AddMonths(TimestampData source, int months)
        {
            int nanos = source.GetNanos();
            DateTime time = new DateTime(source.GetSeconds() * 0x989680L, DateTimeKind.Utc);
            return new TimestampData(time.AddMonths(months).Ticks / 0x989680L, nanos, source.GetZone());
        }

        public static TimeData AddSeconds(TimeData source, int seconds, int nanos)
        {
            nanos += source.GetNanos();
            seconds += nanos / 0x3b9aca00;
            nanos = nanos % 0x3b9aca00;
            if (nanos < 0)
            {
                nanos += 0x3b9aca00;
                seconds--;
            }
            seconds += source.GetSeconds();
            seconds = seconds % 0x15180;
            return new TimeData((long) seconds, nanos, source.GetZone());
        }

        public static TimestampData AddSeconds(TimestampData source, int seconds, int nanos)
        {
            nanos += source.GetNanos();
            seconds += nanos / 0x3b9aca00;
            nanos = nanos % 0x3b9aca00;
            if (nanos < 0)
            {
                nanos += 0x3b9aca00;
                seconds--;
            }
            return new TimestampData(source.GetSeconds() + seconds, nanos, source.GetZone());
        }

        public bool CanAdd(IntervalType other)
        {
            return ((other.StartPartIndex >= base.StartPartIndex) && (other.EndPartIndex <= base.EndPartIndex));
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            if (othType.TypeCode == 0)
            {
                return true;
            }
            if (othType.IsCharacterType())
            {
                return true;
            }
            if (!othType.IsDateTimeType())
            {
                return false;
            }
            if (othType.TypeCode == 0x5b)
            {
                return (base.TypeCode != 0x5c);
            }
            return ((othType.TypeCode != 0x5c) || (base.TypeCode != 0x5b));
        }

        public object ChangeZone(object a, SqlType otherType, int targetZone, int localZone)
        {
            if (a == null)
            {
                return null;
            }
            if ((otherType.TypeCode == 0x5f) || (otherType.TypeCode == 0x5e))
            {
                localZone = 0;
            }
            if ((targetZone > DTIType.TimezoneSecondsLimit) || (-targetZone > DTIType.TimezoneSecondsLimit))
            {
                throw Error.GetError(0xd51);
            }
            switch (base.TypeCode)
            {
                case 0x5e:
                {
                    TimeData data = (TimeData) a;
                    if ((localZone == 0) && (data.GetZone() == targetZone))
                    {
                        return a;
                    }
                    return new TimeData((long) (data.GetSeconds() - localZone), data.GetNanos(), targetZone);
                }
                case 0x5f:
                {
                    TimestampData data2 = (TimestampData) a;
                    if ((localZone == 0) && (data2.GetZone() == targetZone))
                    {
                        return a;
                    }
                    return new TimestampData(data2.GetSeconds() - localZone, data2.GetNanos(), targetZone);
                }
            }
            return a;
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
                case 0x5b:
                case 0x5d:
                case 0x5f:
                {
                    TimestampData data = (TimestampData) a;
                    TimestampData data2 = (TimestampData) b;
                    long num3 = data.GetSeconds() - data2.GetSeconds();
                    if (num3 == 0)
                    {
                        num3 = data.GetNanos() - data2.GetNanos();
                    }
                    if (num3 == 0)
                    {
                        return 0;
                    }
                    if (num3 <= 0L)
                    {
                        return -1;
                    }
                    return 1;
                }
                case 0x5c:
                case 0x5e:
                {
                    TimeData data3 = (TimeData) a;
                    TimeData data4 = (TimeData) b;
                    long num4 = data3.GetSeconds() - data4.GetSeconds();
                    if (num4 == 0)
                    {
                        num4 = data3.GetNanos() - data4.GetNanos();
                    }
                    if (num4 == 0)
                    {
                        return 0;
                    }
                    if (num4 <= 0L)
                    {
                        return -1;
                    }
                    return 1;
                }
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public TimestampData ConvertBetweenTimeZones(Session session, TimestampData source, string thisZone, string newZone)
        {
            DateTime a = (DateTime) this.ConvertSQLToCSharp(session, source);
            TimeZoneInfo timeZoneInfo = GetTimeZoneInfo(thisZone);
            TimeZoneInfo info2 = GetTimeZoneInfo(newZone);
            if ((thisZone == null) || (info2 == null))
            {
                throw Error.GetError(0x15b9);
            }
            a = TimeZoneInfo.ConvertTime(a, timeZoneInfo, info2);
            return (TimestampData) this.ConvertCSharpToSQL(session, a);
        }

        public override object ConvertCSharpToSQL(ISessionInterface session, object a)
        {
            int zoneSeconds;
            long normalisedTime;
            if (a == null)
            {
                return null;
            }
            DateTime? nullable = a as DateTime?;
            if (!nullable.HasValue)
            {
                throw Error.GetError(0x15b9);
            }
            DateTime time = nullable.Value;
            switch (base.TypeCode)
            {
                case 0x5b:
                    return new TimestampData(UtlDateTime.GetNormalisedDate(UtlDateTime.ConvertTicksFromTimeZone(session.GetTimeZone(), time.Ticks) / 0x989680L));

                case 0x5c:
                case 0x5e:
                    zoneSeconds = 0;
                    if (base.TypeCode != 0x5c)
                    {
                        normalisedTime = time.Ticks / 0x989680L;
                        zoneSeconds = session.GetZoneSeconds();
                        break;
                    }
                    normalisedTime = UtlDateTime.ConvertTicksFromTimeZone(session.GetTimeZone(), time.Ticks) / 0x989680L;
                    break;

                case 0x5d:
                case 0x5f:
                {
                    long ticks;
                    int zoneSeconds = 0;
                    if (base.TypeCode != 0x5d)
                    {
                        ticks = time.Ticks;
                        zoneSeconds = session.GetZoneSeconds();
                    }
                    else
                    {
                        ticks = UtlDateTime.ConvertTicksFromTimeZone(session.GetTimeZone(), time.Ticks);
                    }
                    long num7 = ticks / 0x989680L;
                    int num8 = ((int) (ticks - (num7 * 0x989680L))) * 100;
                    return new TimestampData(ticks / 0x989680L, DTIType.NormaliseFraction(num8, base.Scale), zoneSeconds);
                }
                default:
                    throw Error.GetError(0x15b9);
            }
            normalisedTime = UtlDateTime.GetNormalisedTime(normalisedTime);
            int fraction = (int) ((time.Ticks * 100L) - (normalisedTime * 0x3b9aca00L));
            return new TimeData((long) ((int) normalisedTime), DTIType.NormaliseFraction(fraction, base.Scale), zoneSeconds);
        }

        public override object ConvertSQLToCSharp(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case 0x5b:
                {
                    long seconds = ((TimestampData) a).GetSeconds();
                    seconds = UtlDateTime.ConvertTicksToTimeZone(session.GetTimeZone(), seconds * 0x989680L) / 0x989680L;
                    return new DateTime(UtlDateTime.GetNormalisedDate(seconds) * 0x989680L);
                }
                case 0x5c:
                {
                    long seconds = ((TimeData) a).GetSeconds();
                    seconds = UtlDateTime.ConvertTicksToTimeZone(session.GetTimeZone(), seconds * 0x989680L) / 0x989680L;
                    return new DateTime(UtlDateTime.GetNormalisedTime(seconds) * 0x989680L);
                }
                case 0x5d:
                {
                    TimestampData data = (TimestampData) a;
                    long seconds = data.GetSeconds();
                    seconds = UtlDateTime.ConvertTicksToTimeZone(session.GetTimeZone(), seconds * 0x989680L) / 0x989680L;
                    return new DateTime((seconds * 0x989680L) + (data.GetNanos() / 100));
                }
                case 0x5e:
                {
                    TimeData data2 = (TimeData) a;
                    return new DateTime(UtlDateTime.GetNormalisedTime((long) (data2.GetSeconds() + data2.GetZone())) * 0x989680L);
                }
                case 0x5f:
                {
                    TimestampData data3 = (TimestampData) a;
                    return new DateTime((data3.GetSeconds() * 0x989680L) + (data3.GetNanos() / 100));
                }
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public object ConvertSqltoCSharpGmt(ISessionInterface session, object a)
        {
            switch (base.TypeCode)
            {
                case 0x5b:
                    return new DateTime(((TimestampData) a).GetSeconds() * 0x989680L);

                case 0x5c:
                case 0x5e:
                    return new DateTime(((TimeData) a).GetSeconds() * 0x989680L);

                case 0x5d:
                case 0x5f:
                {
                    TimestampData data = (TimestampData) a;
                    return new DateTime((data.GetSeconds() * 0x989680L) + (data.GetNanos() / 100));
                }
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            throw Error.GetError(0x15b9);
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            StringBuilder builder = new StringBuilder(0x20);
            switch (base.TypeCode)
            {
                case 0x5b:
                    builder.Append("DATE");
                    break;

                case 0x5c:
                case 0x5e:
                    builder.Append("TIME");
                    break;

                case 0x5d:
                case 0x5f:
                    builder.Append("TIMESTAMP");
                    break;
            }
            builder.Append(StringConverter.ToQuotedString(this.ConvertToString(a), '\'', false));
            return builder.ToString();
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case 0x5b:
                    return UtlDateTime.GetDateString(((TimestampData) a).GetSeconds());

                case 0x5c:
                case 0x5e:
                {
                    TimeData data = (TimeData) a;
                    int num2 = NormaliseTime(data.GetSeconds() + data.GetZone());
                    string str2 = base.IntervalSecondToString((long) num2, data.GetNanos(), false);
                    if (this._withTimeZone)
                    {
                        str2 = SqlType.SqlIntervalHourToMinute.IntervalSecondToString((long) data.GetZone(), 0, true);
                        StringBuilder builder1 = new StringBuilder(str2);
                        builder1.Append(str2);
                        return builder1.ToString();
                    }
                    return str2;
                }
                case 0x5d:
                case 0x5f:
                {
                    TimestampData data2 = (TimestampData) a;
                    StringBuilder sb = new StringBuilder();
                    UtlDateTime.GetTimestampString(sb, data2.GetSeconds() + data2.GetZone(), data2.GetNanos(), base.Scale);
                    if (this._withTimeZone)
                    {
                        string str3 = SqlType.SqlIntervalHourToMinute.IntervalSecondToString((long) data2.GetZone(), 0, true);
                        sb.Append(str3);
                        return sb.ToString();
                    }
                    return sb.ToString();
                }
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            if (a == null)
            {
                return a;
            }
            switch (othType.TypeCode)
            {
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x5e:
                case 0x5f:
                    goto Label_0126;

                case 100:
                case 1:
                case 12:
                    break;

                case 40:
                case 0x19:
                case 3:
                case 4:
                case 8:
                    a = a.ToString();
                    break;

                default:
                    throw Error.GetError(0x15b9);
            }
            int typeCode = base.TypeCode;
            if (typeCode != 0x5b)
            {
                if ((typeCode - 0x5c) <= 3)
                {
                    goto Label_0105;
                }
                goto Label_0126;
            }
            try
            {
                DateTime time = DateTime.Parse((string) a, CultureInfo.InvariantCulture);
                long seconds = time.Ticks / 0x989680L;
                long num4 = (time.Ticks - (seconds * 0x989680L)) * 100L;
                return new TimestampData(seconds, (int) num4);
            }
            catch (Exception)
            {
                return session.GetScanner().ConvertToDatetimeInterval(session, (string) a, this);
            }
        Label_0105:
            return session.GetScanner().ConvertToDatetimeInterval(session, (string) a, this);
        Label_0126:
            switch (base.TypeCode)
            {
                case 0x5b:
                    switch (othType.TypeCode)
                    {
                        case 0x5b:
                            return a;

                        case 0x5d:
                            return new TimestampData(UtlDateTime.GetNormalisedDate(((TimestampData) a).GetSeconds()));

                        case 0x5f:
                            return new TimestampData(UtlDateTime.GetNormalisedDate(((TimestampData) a).GetSeconds() + ((TimestampData) a).GetZone()));
                    }
                    break;

                case 0x5c:
                    switch (othType.TypeCode)
                    {
                        case 0x5c:
                            return this.ConvertToTypeLimits(session, a);

                        case 0x5d:
                        {
                            TimestampData data = (TimestampData) a;
                            return new TimeData((long) ((int) data.GetSeconds()), this.ScaleNanos(data.GetNanos()));
                        }
                        case 0x5e:
                        {
                            TimeData data2 = (TimeData) a;
                            return new TimeData((long) (data2.GetSeconds() + data2.GetZone()), this.ScaleNanos(data2.GetNanos()), 0);
                        }
                        case 0x5f:
                        {
                            TimestampData data3 = (TimestampData) a;
                            return new TimeData((long) (((int) data3.GetSeconds()) + data3.GetZone()), this.ScaleNanos(data3.GetNanos()), 0);
                        }
                    }
                    throw Error.GetError(0x15b9);

                case 0x5d:
                    switch (othType.TypeCode)
                    {
                        case 0x5b:
                            return a;

                        case 0x5c:
                        {
                            TimeData data4 = (TimeData) a;
                            return new TimestampData(session.GetCurrentDate().GetSeconds() + data4.GetSeconds(), this.ScaleNanos(data4.GetNanos()));
                        }
                        case 0x5d:
                            return this.ConvertToTypeLimits(session, a);

                        case 0x5e:
                        {
                            TimeData data5 = (TimeData) a;
                            return new TimestampData((session.GetCurrentDate().GetSeconds() + data5.GetSeconds()) - session.GetZoneSeconds(), this.ScaleNanos(data5.GetNanos()), session.GetZoneSeconds());
                        }
                        case 0x5f:
                        {
                            TimestampData data6 = (TimestampData) a;
                            return new TimestampData(data6.GetSeconds() + data6.GetZone(), this.ScaleNanos(data6.GetNanos()));
                        }
                    }
                    throw Error.GetError(0x15b9);

                case 0x5e:
                    switch (othType.TypeCode)
                    {
                        case 0x5c:
                        {
                            TimeData data7 = (TimeData) a;
                            return new TimeData((long) (data7.GetSeconds() - session.GetZoneSeconds()), this.ScaleNanos(data7.GetNanos()), session.GetZoneSeconds());
                        }
                        case 0x5d:
                        {
                            TimestampData data8 = (TimestampData) a;
                            return new TimeData((long) (((int) data8.GetSeconds()) - session.GetZoneSeconds()), this.ScaleNanos(data8.GetNanos()), session.GetZoneSeconds());
                        }
                        case 0x5e:
                            return this.ConvertToTypeLimits(session, a);

                        case 0x5f:
                        {
                            TimestampData data9 = (TimestampData) a;
                            return new TimeData((long) ((int) data9.GetSeconds()), this.ScaleNanos(data9.GetNanos()), data9.GetZone());
                        }
                    }
                    throw Error.GetError(0x15b9);

                case 0x5f:
                    switch (othType.TypeCode)
                    {
                        case 0x5b:
                            return new TimestampData(((TimestampData) a).GetSeconds(), 0, session.GetZoneSeconds());

                        case 0x5c:
                        {
                            TimeData data10 = (TimeData) a;
                            return new TimestampData((session.GetCurrentDate().GetSeconds() + data10.GetSeconds()) - session.GetZoneSeconds(), this.ScaleNanos(data10.GetNanos()), session.GetZoneSeconds());
                        }
                        case 0x5d:
                        {
                            TimestampData data11 = (TimestampData) a;
                            return new TimestampData(data11.GetSeconds() - session.GetZoneSeconds(), this.ScaleNanos(data11.GetNanos()), session.GetZoneSeconds());
                        }
                        case 0x5e:
                        {
                            TimeData data12 = (TimeData) a;
                            return new TimestampData(session.GetCurrentDate().GetSeconds() + data12.GetSeconds(), this.ScaleNanos(data12.GetNanos()), data12.GetZone());
                        }
                        case 0x5f:
                            return this.ConvertToTypeLimits(session, a);
                    }
                    throw Error.GetError(0x15b9);

                default:
                    throw Error.RuntimeError(0xc9, "DateTimeType");
            }
            throw Error.GetError(0x15b9);
        }

        public object ConvertToType(ISessionInterface session, object a, SqlType othType, string fmt)
        {
            if (a == null)
            {
                return a;
            }
            switch (othType.TypeCode)
            {
                case 0x19:
                case 40:
                case 3:
                case 4:
                case 8:
                    a = a.ToString();
                    break;

                case 100:
                case 1:
                case 12:
                    break;

                default:
                    goto Label_00C1;
            }
            if ((base.TypeCode - 0x5b) <= 2)
            {
                DateTime time;
                string s = (string) a;
                if (DateTime.TryParseExact(s, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                {
                    long seconds = time.Ticks / 0x989680L;
                    long num4 = (time.Ticks - (seconds * 0x989680L)) * 100L;
                    return new TimestampData(seconds, (int) num4);
                }
                return session.GetScanner().ConvertToDatetimeInterval(session, s, this);
            }
        Label_00C1:
            return this.ConvertToType(session, a, othType);
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            if (base.Scale == 9)
            {
                return a;
            }
            switch (base.TypeCode)
            {
                case 0x5b:
                    return a;

                case 0x5c:
                case 0x5e:
                {
                    TimeData data = (TimeData) a;
                    int nanos = data.GetNanos();
                    int num3 = this.ScaleNanos(nanos);
                    if (num3 != nanos)
                    {
                        return new TimeData((long) data.GetSeconds(), num3, data.GetZone());
                    }
                    return data;
                }
                case 0x5d:
                case 0x5f:
                {
                    TimestampData data2 = (TimestampData) a;
                    int nanos = data2.GetNanos();
                    int num5 = this.ScaleNanos(nanos);
                    if (num5 != nanos)
                    {
                        return new TimestampData(data2.GetSeconds(), num5, data2.GetZone());
                    }
                    return data2;
                }
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public override int DisplaySize()
        {
            switch (base.TypeCode)
            {
                case 0x5b:
                    return 10;

                case 0x5c:
                    return (8 + ((base.Scale == 0) ? 0 : (base.Scale + 1)));

                case 0x5d:
                    return (0x13 + ((base.Scale == 0) ? 0 : (base.Scale + 1)));

                case 0x5e:
                    return ((8 + ((base.Scale == 0) ? 0 : (base.Scale + 1))) + 6);

                case 0x5f:
                    return ((0x13 + ((base.Scale == 0) ? 0 : (base.Scale + 1))) + 6);
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public bool Equals(DateTimeType other)
        {
            return (((other != null) && base.Equals((SqlType) other)) && (other._withTimeZone == this._withTimeZone));
        }

        public override bool Equals(object other)
        {
            DateTimeType type = other as DateTimeType;
            return ((other != null) && this.Equals(type));
        }

        public override int GetAdoPrecision()
        {
            return this.DisplaySize();
        }

        public override int GetAdoTypeCode()
        {
            switch (base.TypeCode)
            {
                case 0x5b:
                    return 0x11;

                case 0x5c:
                    return 13;

                case 0x5d:
                    return 15;

                case 0x5e:
                    return 14;

                case 0x5f:
                    return 0x10;
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public override SqlType GetAggregateType(SqlType other)
        {
            int num2;
            if (base.TypeCode == other.TypeCode)
            {
                if (base.Scale < other.Scale)
                {
                    return other;
                }
                return this;
            }
            if (other.TypeCode == 0)
            {
                return this;
            }
            if (other.IsCharacterType())
            {
                return other.GetAggregateType(this);
            }
            if (!other.IsDateTimeType())
            {
                throw Error.GetError(0x15ba);
            }
            DateTimeType type2 = (DateTimeType) other;
            if ((type2.StartIntervalType > base.EndIntervalType) || (base.StartIntervalType > type2.EndIntervalType))
            {
                throw Error.GetError(0x15ba);
            }
            int scale = (base.Scale > type2.Scale) ? base.Scale : type2.Scale;
            bool flag = this._withTimeZone || type2._withTimeZone;
            if (((type2.StartIntervalType > base.StartIntervalType) ? base.StartIntervalType : type2.StartIntervalType) == 0x68)
            {
                num2 = flag ? 0x5e : 0x5c;
            }
            else
            {
                num2 = flag ? 0x5f : 0x5d;
            }
            return GetDateTimeType(num2, scale);
        }

        public override SqlType GetCombinedType(SqlType other, int operation)
        {
            if ((operation - 0x20) > 1)
            {
                if ((operation - 0x29) <= 5)
                {
                    int num2;
                    if (base.TypeCode == other.TypeCode)
                    {
                        return this;
                    }
                    if (other.TypeCode == 0)
                    {
                        return this;
                    }
                    if (!other.IsDateTimeType())
                    {
                        throw Error.GetError(0x15ba);
                    }
                    DateTimeType type = (DateTimeType) other;
                    if ((type.StartIntervalType > base.EndIntervalType) || (base.StartIntervalType > type.EndIntervalType))
                    {
                        throw Error.GetError(0x15ba);
                    }
                    int scale = (base.Scale > type.Scale) ? base.Scale : type.Scale;
                    bool flag = this._withTimeZone || type._withTimeZone;
                    if (((type.StartIntervalType > base.StartIntervalType) ? base.StartIntervalType : type.StartIntervalType) == 0x68)
                    {
                        num2 = flag ? 0x5e : 0x5c;
                    }
                    else
                    {
                        num2 = flag ? 0x5f : 0x5d;
                    }
                    return GetDateTimeType(num2, scale);
                }
            }
            else
            {
                if (other.IsIntervalType())
                {
                    if ((base.TypeCode != 0x5b) && (other.Scale > base.Scale))
                    {
                        return GetDateTimeType(base.TypeCode, other.Scale);
                    }
                    return this;
                }
                if ((base.TypeCode == 0x5b) && other.IsNumberType())
                {
                    return SqlType.SqlDate;
                }
            }
            throw Error.GetError(0x15ba);
        }

        public override Type GetCSharpClass()
        {
            return typeof(DateTime);
        }

        public override string GetCSharpClassName()
        {
            switch (base.TypeCode)
            {
                case 0x5b:
                    return "System.DateTime";

                case 0x5c:
                case 0x5e:
                    return "System.DateTime";

                case 0x5d:
                case 0x5f:
                    return "System.DateTime";
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public static DateTimeType GetDateTimeType(int type, int scale)
        {
            if (scale > 9)
            {
                throw Error.GetError(0x15d8);
            }
            switch (type)
            {
                case 0x5b:
                    return SqlType.SqlDate;

                case 0x5c:
                    if (scale == DTIType.DefaultTimeFractionPrecision)
                    {
                        return SqlType.SqlTime;
                    }
                    return new DateTimeType(0x5c, type, scale);

                case 0x5d:
                    if (scale == 6)
                    {
                        return SqlType.SqlTimestamp;
                    }
                    return new DateTimeType(0x5d, type, scale);

                case 0x5e:
                    if (scale == DTIType.DefaultTimeFractionPrecision)
                    {
                        return SqlType.SqlTimeWithTimeZone;
                    }
                    return new DateTimeType(0x5c, type, scale);

                case 0x5f:
                    if (scale == 6)
                    {
                        return SqlType.SqlTimestampWithTimeZone;
                    }
                    return new DateTimeType(0x5d, type, scale);
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public DateTimeType GetDateTimeTypeWithoutZone()
        {
            if (!this._withTimeZone)
            {
                return this;
            }
            switch (base.TypeCode)
            {
                case 0x5e:
                    if (base.Scale != DTIType.DefaultTimeFractionPrecision)
                    {
                        return new DateTimeType(0x5c, 0x5c, base.Scale);
                    }
                    return SqlType.SqlTime;

                case 0x5f:
                    if (base.Scale != 6)
                    {
                        return new DateTimeType(0x5d, 0x5d, base.Scale);
                    }
                    return SqlType.SqlTimestamp;
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public override string GetDefinition()
        {
            string str2;
            if (base.Scale == DTIType.DefaultTimeFractionPrecision)
            {
                return this.GetNameString();
            }
            switch (base.TypeCode)
            {
                case 0x5b:
                    return "DATE";

                case 0x5c:
                case 0x5e:
                    if (base.Scale != DTIType.DefaultTimeFractionPrecision)
                    {
                        str2 = "TIME";
                        break;
                    }
                    return this.GetNameString();

                case 0x5d:
                case 0x5f:
                    if (base.Scale != 6)
                    {
                        str2 = "TIMESTAMP";
                        break;
                    }
                    return this.GetNameString();

                default:
                    throw Error.RuntimeError(0xc9, "DateTimeType");
            }
            StringBuilder builder = new StringBuilder(0x10);
            builder.Append(str2);
            builder.Append('(');
            builder.Append(base.Scale);
            builder.Append(')');
            if (this._withTimeZone)
            {
                builder.Append(string.Concat(new object[] { ' ', "WITH", ' ', "TIME", ' ', "ZONE" }));
            }
            return builder.ToString();
        }

        public override int GetHashCode()
        {
            return (base.GetHashCode() ^ this._withTimeZone.GetHashCode());
        }

        public TimestampData GetLastDayOfMonth(Session session, TimestampData source)
        {
            DateTime a = (DateTime) this.ConvertSQLToCSharp(session, source);
            a = a.AddMonths(1);
            a = a.AddDays(-((double) a.Day));
            return (TimestampData) this.ConvertCSharpToSQL(session, a);
        }

        public override string GetNameString()
        {
            switch (base.TypeCode)
            {
                case 0x5b:
                    return "DATE";

                case 0x5c:
                    return "TIME";

                case 0x5d:
                    return "TIMESTAMP";

                case 0x5e:
                {
                    object[] objArray1 = new object[] { "TIME", ' ', "WITH", ' ', "TIME", ' ', "ZONE" };
                    return string.Concat(objArray1);
                }
                case 0x5f:
                {
                    object[] objArray2 = new object[] { "TIMESTAMP", ' ', "WITH", ' ', "TIME", ' ', "ZONE" };
                    return string.Concat(objArray2);
                }
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public TimestampData GetNextDay(Session session, TimestampData source, string weekDay)
        {
            int num = -1;
            string[] strArray = new string[] { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].IndexOf(weekDay, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    num = i;
                    break;
                }
            }
            if (num == -1)
            {
                throw Error.GetError(0x15b9);
            }
            DateTime a = ((DateTime) this.ConvertSQLToCSharp(session, source)).AddDays(1.0);
            int num2 = ((num - a.DayOfWeek) >= 0) ? (num - a.DayOfWeek) : ((num - a.DayOfWeek) + 7);
            a = a.AddDays((double) num2);
            return (TimestampData) this.ConvertCSharpToSQL(session, a);
        }

        public override int GetPart(Session session, object dateTime, int part)
        {
            long num;
            if ((base.TypeCode == 0x5c) || (base.TypeCode == 0x5e))
            {
                TimeData data = (TimeData) dateTime;
                num = (data.GetSeconds() + data.GetZone()) * 0x989680L;
            }
            else
            {
                TimestampData data2 = (TimestampData) dateTime;
                num = (data2.GetSeconds() + data2.GetZone()) * 0x989680L;
            }
            DateTime time = new DateTime(num);
            switch (part)
            {
                case 0x65:
                    return time.Year;

                case 0x66:
                    return time.Month;

                case 0x67:
                case 260:
                    return time.Day;

                case 0x68:
                    return time.Hour;

                case 0x69:
                    return time.Minute;

                case 0x6a:
                    return time.Second;

                case 0x101:
                    if (base.TypeCode != 0x5f)
                    {
                        return (((TimeData) dateTime).GetZone() / 0xe10);
                    }
                    return (((TimestampData) dateTime).GetZone() / 0xe10);

                case 0x102:
                    if (base.TypeCode != 0x5f)
                    {
                        return ((((TimeData) dateTime).GetZone() / 60) % 60);
                    }
                    return ((((TimestampData) dateTime).GetZone() / 60) % 60);

                case 0x103:
                    return (((int) time.DayOfWeek) + 1);

                case 0x105:
                    return time.DayOfYear;

                case 0x106:
                    return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                case 0x107:
                    return (((time.Month - 1) / 3) + 1);

                case 0x10a:
                    if ((base.TypeCode != 0x5c) && (base.TypeCode != 0x5e))
                    {
                        try
                        {
                            dateTime = (this._withTimeZone ? SqlType.SqlTimeWithTimeZone : SqlType.SqlTime).CastToType(session, dateTime, this);
                        }
                        catch (CoreException)
                        {
                        }
                    }
                    return ((TimeData) dateTime).GetSeconds();
            }
            throw Error.RuntimeError(0xc9, "DateTimeType - " + part);
        }

        public string GetPartString(Session session, object dateTime, int part)
        {
            string format = "";
            if (part != 0x108)
            {
                if (part == 0x109)
                {
                    format = "MMMM";
                }
            }
            else
            {
                format = "dddd";
            }
            DateTime time = (DateTime) this.ConvertSqltoCSharpGmt(session, dateTime);
            return time.ToString(format);
        }

        public override decimal GetSecondPart(object dateTime)
        {
            long seconds = this.GetPart(null, dateTime, 0x6a);
            int nanos = 0;
            if (base.TypeCode == 0x5d)
            {
                nanos = ((TimestampData) dateTime).GetNanos();
            }
            else if (base.TypeCode == 0x5c)
            {
                nanos = ((TimeData) dateTime).GetNanos();
            }
            return base.GetSecondPart(seconds, (long) nanos);
        }

        public int GetSqlDateTimeSub()
        {
            switch (base.TypeCode)
            {
                case 0x5b:
                    return 1;

                case 0x5c:
                    return 2;

                case 0x5d:
                    return 3;
            }
            return 0;
        }

        public override int GetSqlGenericTypeCode()
        {
            return 9;
        }

        private static TimeZoneInfo GetTimeZoneInfo(string name)
        {
            foreach (TimeZoneInfo info in TimeZoneInfo.GetSystemTimeZones())
            {
                if (info.get_DisplayName().Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return info;
                }
                if (info.get_DaylightName().Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return info;
                }
                if (info.get_Id().Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return info;
                }
                if (info.get_StandardName().Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return info;
                }
            }
            return null;
        }

        public object GetValue(long seconds, int nanos, int zoneSeconds)
        {
            switch (base.TypeCode)
            {
                case 0x5b:
                    seconds = UtlDateTime.GetNormalisedDate(seconds + zoneSeconds);
                    return new TimestampData(seconds);

                case 0x5c:
                    seconds = UtlDateTime.GetNormalisedTime(seconds + zoneSeconds);
                    return new TimeData(seconds, nanos);

                case 0x5d:
                    return new TimestampData(seconds + zoneSeconds, nanos);

                case 0x5e:
                    seconds = UtlDateTime.GetNormalisedTime(seconds);
                    return new TimeData((long) ((int) seconds), nanos, zoneSeconds);

                case 0x5f:
                    return new TimestampData(seconds, nanos, zoneSeconds);
            }
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public override bool IsDateTimeType()
        {
            return true;
        }

        public override bool IsDateTimeTypeWithZone()
        {
            return this._withTimeZone;
        }

        public static int NormaliseTime(int seconds)
        {
            while (seconds < 0)
            {
                seconds += 0x15180;
            }
            if (seconds > 0x15180)
            {
                seconds = seconds % 0x15180;
            }
            return seconds;
        }

        public static bool? Overlaps(Session session, object[] a, SqlType[] ta, object[] b, SqlType[] tb)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            if ((a[0] == null) || (b[0] == null))
            {
                return null;
            }
            if (a[1] == null)
            {
                a[1] = a[0];
            }
            if (b[1] == null)
            {
                b[1] = b[0];
            }
            SqlType combinedType = ta[0].GetCombinedType(tb[0], 0x29);
            a[0] = combinedType.CastToType(session, a[0], ta[0]);
            b[0] = combinedType.CastToType(session, b[0], tb[0]);
            if (ta[1].IsIntervalType())
            {
                a[1] = combinedType.Add(a[0], a[1], ta[0], ta[1]);
            }
            else
            {
                a[1] = combinedType.CastToType(session, a[1], ta[1]);
            }
            if (tb[1].IsIntervalType())
            {
                b[1] = combinedType.Add(b[0], b[1], tb[0], tb[1]);
            }
            else
            {
                b[1] = combinedType.CastToType(session, b[1], tb[1]);
            }
            if (combinedType.Compare(session, a[0], a[1], null, false) > 0)
            {
                object obj2 = a[0];
                a[0] = a[1];
                a[1] = obj2;
            }
            if (combinedType.Compare(session, b[0], b[1], null, false) > 0)
            {
                object obj3 = b[0];
                b[0] = b[1];
                b[1] = obj3;
            }
            if (combinedType.Compare(session, a[0], b[0], null, false) > 0)
            {
                a = b;
                b = a;
            }
            return (combinedType.Compare(session, a[1], b[0], null, false) > 0);
        }

        public TimestampData Round(Session session, TimestampData source)
        {
            DateTime a = (DateTime) this.ConvertSQLToCSharp(session, source);
            DateTime time2 = a.Date.AddHours(12.0);
            DateTime time3 = (a > time2) ? a.Date.AddDays(1.0) : time2;
            a = time3.Date;
            return (TimestampData) this.ConvertCSharpToSQL(session, a);
        }

        private int ScaleNanos(int nanos)
        {
            int num = DTIType.NanoScaleFactors[base.Scale];
            return ((nanos / num) * num);
        }

        public override object Subtract(object a, object b, SqlType aType, SqlType otherType)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            switch (base.TypeCode)
            {
                case 0x5b:
                {
                    if (!otherType.IsNumberType())
                    {
                        break;
                    }
                    double num2 = ((double) SqlType.SqlDouble.ConvertToDefaultType(null, b)) * 86400.0;
                    return AddSeconds((TimestampData) a, -((int) num2), 0);
                }
                case 0x5c:
                case 0x5e:
                {
                    if (b is IntervalMonthData)
                    {
                        throw Error.RuntimeError(0xc9, "DateTimeType");
                    }
                    IntervalSecondData data4 = b as IntervalSecondData;
                    if (data4 == null)
                    {
                        goto Label_00ED;
                    }
                    return AddSeconds((TimeData) a, -((int) data4.Units), -data4.Nanos);
                }
                case 0x5d:
                case 0x5f:
                    break;

                default:
                    goto Label_00ED;
            }
            TimestampData source = (TimestampData) a;
            IntervalMonthData data2 = b as IntervalMonthData;
            if (data2 != null)
            {
                return AddMonths(source, -((int) data2.Units));
            }
            IntervalSecondData data3 = b as IntervalSecondData;
            if (data3 != null)
            {
                return AddSeconds(source, -((int) data3.Units), -data3.Nanos);
            }
        Label_00ED:
            throw Error.RuntimeError(0xc9, "DateTimeType");
        }

        public static int SubtractMonths(TimestampData a, TimestampData b, bool isYear)
        {
            bool flag = false;
            if (b.GetSeconds() > a.GetSeconds())
            {
                flag = true;
                a = b;
                b = a;
            }
            DateTime time = new DateTime(b.GetSeconds() * 0x989680L, DateTimeKind.Utc);
            int month = time.Month;
            int year = time.Year;
            DateTime time2 = new DateTime(a.GetSeconds() * 0x989680L, DateTimeKind.Utc);
            try
            {
                DateTime time3 = time2.AddMonths(-1 * month).AddYears(-1 * year);
                month = time3.Month;
                year = time3.Year;
            }
            catch (Exception)
            {
                month = time2.Subtract(time).Days / 30;
                year = 0;
            }
            if (isYear)
            {
                return (year * 12);
            }
            if (month < 0)
            {
                month += 12;
                year--;
            }
            month += year * 12;
            if (flag)
            {
                month = -month;
            }
            return month;
        }

        public override double ToReal(object a)
        {
            if (base.TypeCode == 0x5b)
            {
                return (double) (((TimestampData) a).GetSeconds() / 0x15180L);
            }
            return base.ToReal(a);
        }

        public TimestampData Trunc(Session session, TimestampData source)
        {
            DateTime time2 = (DateTime) this.ConvertSQLToCSharp(session, source);
            DateTime date = time2.Date;
            return (TimestampData) this.ConvertCSharpToSQL(session, date);
        }

        public enum CalendarPart
        {
            Year,
            Month,
            DayOfMonth,
            HourOfDay,
            Minute,
            Second,
            DayOfWeek,
            DayOfYear,
            WeekOfYear
        }
    }
}

