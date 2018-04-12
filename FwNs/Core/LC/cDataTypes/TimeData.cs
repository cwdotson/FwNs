namespace FwNs.Core.LC.cDataTypes
{
    using System;

    public sealed class TimeData : IEquatable<TimeData>
    {
        private readonly int _nanos;
        private readonly int _seconds;
        private readonly int _zone;

        public TimeData(long seconds, int nanos) : this(seconds, nanos, 0)
        {
        }

        public TimeData(long seconds, int nanos, int zoneSeconds)
        {
            while (seconds < 0L)
            {
                seconds += 0x15180L;
            }
            if (seconds > 0x15180L)
            {
                seconds = seconds % 0x15180L;
            }
            this._zone = zoneSeconds;
            this._seconds = (int) seconds;
            this._nanos = nanos;
        }

        public int CompareTo(TimeData b)
        {
            long num = this._seconds - b._seconds;
            if (num == 0)
            {
                num = this._nanos - b._nanos;
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

        public bool Equals(TimeData other)
        {
            return ((((other != null) && (this._seconds == other._seconds)) && (this._nanos == other._nanos)) && (this._zone == other._zone));
        }

        public override bool Equals(object other)
        {
            TimeData data = other as TimeData;
            return ((data != null) && this.Equals(data));
        }

        public override int GetHashCode()
        {
            return (this._seconds ^ this._nanos);
        }

        public int GetNanos()
        {
            return this._nanos;
        }

        public int GetSeconds()
        {
            return this._seconds;
        }

        public int GetZone()
        {
            return this._zone;
        }
    }
}

