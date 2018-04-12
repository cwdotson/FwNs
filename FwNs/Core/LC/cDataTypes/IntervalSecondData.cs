namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cErrors;
    using System;

    public sealed class IntervalSecondData : IEquatable<IntervalSecondData>
    {
        public int Nanos;
        public long Units;

        public IntervalSecondData(long seconds, int nanos)
        {
            this.Units = seconds;
            this.Nanos = nanos;
        }

        public IntervalSecondData(long seconds, int nanos, IntervalType type)
        {
            if (seconds >= type.GetIntervalValueLimit())
            {
                throw Error.GetError(0xd6b);
            }
            this.Units = seconds;
            this.Nanos = nanos;
        }

        public IntervalSecondData(long seconds, long nanos, IntervalType type, bool normalise)
        {
            if (nanos >= 0x3b9aca00L)
            {
                long num2 = nanos / 0x3b9aca00L;
                nanos = nanos % 0x3b9aca00L;
                seconds += num2;
            }
            else if (nanos <= -1000000000L)
            {
                long num3 = -nanos / 0x3b9aca00L;
                nanos = -(-nanos % 0x3b9aca00L);
                seconds -= num3;
            }
            int num = DTIType.NanoScaleFactors[type.Scale];
            nanos /= (long) num;
            nanos *= num;
            if ((seconds > 0L) && (nanos < 0L))
            {
                nanos += 0x3b9aca00L;
                seconds -= 1L;
            }
            else if ((seconds < 0L) && (nanos > 0L))
            {
                nanos -= 0x3b9aca00L;
                seconds += 1L;
            }
            num = DTIType.YearToSecondFactors[type.EndPartIndex];
            seconds /= (long) num;
            seconds *= num;
            if (seconds >= type.GetIntervalValueLimit())
            {
                throw Error.GetError(0xd6b);
            }
            this.Units = seconds;
            this.Nanos = (int) nanos;
        }

        public int CompareTo(IntervalSecondData b)
        {
            long num = this.Units - b.Units;
            if (num == 0)
            {
                num = this.Nanos - b.Nanos;
                if (num == 0)
                {
                    return 0;
                }
            }
            if (num <= 0L)
            {
                return -1;
            }
            return 1;
        }

        public bool Equals(IntervalSecondData other)
        {
            return (((other != null) && (this.Units == other.Units)) && (this.Nanos == other.Nanos));
        }

        public override bool Equals(object other)
        {
            IntervalSecondData data = other as IntervalSecondData;
            return ((data != null) && this.Equals(data));
        }

        public override int GetHashCode()
        {
            return (((int) this.Units) ^ this.Nanos);
        }

        public int GetNanos()
        {
            return this.Nanos;
        }

        public long GetSeconds()
        {
            return this.Units;
        }

        public static IntervalSecondData NewIntervalDay(long days, IntervalType type)
        {
            return new IntervalSecondData(((days * 0x18L) * 60L) * 60L, 0, type);
        }

        public static IntervalSecondData NewIntervalHour(long hours, IntervalType type)
        {
            return new IntervalSecondData((hours * 60L) * 60L, 0, type);
        }

        public static IntervalSecondData NewIntervalMinute(long minutes, IntervalType type)
        {
            return new IntervalSecondData(minutes * 60L, 0, type);
        }

        public static IntervalSecondData NewIntervalSeconds(long seconds, IntervalType type)
        {
            return new IntervalSecondData(seconds, 0, type);
        }

        public override string ToString()
        {
            return SqlType.SqlIntervalSecondMaxFractionMaxPrecision.ConvertToString(this);
        }
    }
}

