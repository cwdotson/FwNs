namespace FwNs.Core.LC.cDataTypes
{
    using System;

    public sealed class TimestampData : IEquatable<TimestampData>
    {
        private readonly int _nanos;
        private readonly long _seconds;
        private readonly int _zone;

        public TimestampData(long seconds)
        {
            this._seconds = seconds;
            this._nanos = 0;
            this._zone = 0;
        }

        public TimestampData(long seconds, int nanos)
        {
            this._seconds = seconds;
            this._nanos = nanos;
            this._zone = 0;
        }

        public TimestampData(long seconds, int nanos, int zoneSeconds)
        {
            this._seconds = seconds;
            this._nanos = nanos;
            this._zone = zoneSeconds;
        }

        public int CompareTo(TimestampData b)
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

        public bool Equals(TimestampData other)
        {
            return ((((other != null) && (this._seconds == other._seconds)) && (this._nanos == other._nanos)) && (this._zone == other._zone));
        }

        public override bool Equals(object other)
        {
            TimestampData data = other as TimestampData;
            return ((data != null) && this.Equals(data));
        }

        public override int GetHashCode()
        {
            return (((int) this._seconds) ^ this._nanos);
        }

        public int GetNanos()
        {
            return this._nanos;
        }

        public long GetSeconds()
        {
            return this._seconds;
        }

        public int GetZone()
        {
            return this._zone;
        }
    }
}

