namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cErrors;
    using System;

    public sealed class IntervalMonthData : IEquatable<IntervalMonthData>
    {
        public long Units;

        public IntervalMonthData(long months)
        {
            this.Units = months;
        }

        public IntervalMonthData(long months, IntervalType type)
        {
            if (months >= type.GetIntervalValueLimit())
            {
                throw Error.GetError(0xd4e);
            }
            if (type.TypeCode == 0x65)
            {
                months -= months % 12L;
            }
            this.Units = months;
        }

        public int CompareTo(IntervalMonthData b)
        {
            long num = this.Units - b.Units;
            if (num == 0)
            {
                return 0;
            }
            if (num <= 0L)
            {
                return -1;
            }
            return 1;
        }

        public bool Equals(IntervalMonthData other)
        {
            return ((other != null) && (this.Units == other.Units));
        }

        public override bool Equals(object other)
        {
            IntervalMonthData data = other as IntervalMonthData;
            return ((data != null) && this.Equals(data));
        }

        public override int GetHashCode()
        {
            return (int) this.Units;
        }

        public static IntervalMonthData NewIntervalMonth(long months, IntervalType type)
        {
            return new IntervalMonthData(months, type);
        }

        public static IntervalMonthData NewIntervalYear(long years, IntervalType type)
        {
            return new IntervalMonthData(years * 12L, type);
        }

        public override string ToString()
        {
            return SqlType.SqlIntervalMonthMaxPrecision.ConvertToString(this);
        }
    }
}

