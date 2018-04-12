namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Runtime.CompilerServices;

    internal abstract class WeakListener<T> : IListener<T>, IListener, IDisposable
    {
        protected WeakListener()
        {
        }

        internal void CheckDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException("WeakListener");
            }
        }

        public abstract void Clear();
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            this.IsDisposed = true;
        }

        ~WeakListener()
        {
            this.Dispose(false);
        }

        public abstract void StartListening(T source);
        public abstract void StopListening(T source);

        public bool IsDisposed { get; private set; }
    }
}

