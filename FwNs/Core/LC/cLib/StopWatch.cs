namespace FwNs.Core.LC.cLib
{
    using System;

    public sealed class StopWatch
    {
        private bool _running;
        private long _startTime;
        private long _total;

        public StopWatch() : this(true)
        {
        }

        public StopWatch(bool start)
        {
            if (start)
            {
                this.Start();
            }
        }

        public long ElapsedTime()
        {
            if (this._running)
            {
                return ((this._total + (DateTime.UtcNow.Ticks / 0x186a0L)) - this._startTime);
            }
            return this._total;
        }

        public void Start()
        {
            this._startTime = DateTime.UtcNow.Ticks / 0x186a0L;
            this._running = true;
        }

        public void Stop()
        {
            if (this._running)
            {
                this._total += (DateTime.UtcNow.Ticks / 0x186a0L) - this._startTime;
                this._running = false;
            }
        }
    }
}

