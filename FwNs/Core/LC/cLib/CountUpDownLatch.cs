namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Threading;

    public class CountUpDownLatch : IDisposable
    {
        private readonly ManualResetEvent _latch = new ManualResetEvent(false);
        private int _count;

        public void Await()
        {
            if (this._count != 0)
            {
                this._latch.WaitOne();
            }
        }

        public void CountDown()
        {
            if (this._count == 0)
            {
                throw new InvalidOperationException();
            }
            this._count--;
            if (this._count == 0)
            {
                this._latch.Set();
            }
        }

        public void CountUp()
        {
            this._latch.Reset();
            this._count++;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._latch.Close();
            }
        }

        public long GetCount()
        {
            return (long) this._count;
        }

        public void SetCount(int count)
        {
            if (count == 0)
            {
                this._latch.Set();
            }
            else
            {
                this._latch.Reset();
            }
            this._count = count;
        }
    }
}

