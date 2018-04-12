namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Runtime.CompilerServices;

    internal class Disposable
    {
        public static readonly IDisposable Empty;

        static Disposable()
        {
            Empty = Create(new Action(<>c.<>9, this.<.cctor>b__5_0));
        }

        public static IDisposable Concat(IDisposable a, IDisposable b)
        {
            <>c__DisplayClass3_0 class_;
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }
            return Create(new Action(class_, this.<Concat>b__0));
        }

        public static IDisposable Create(Action dispose)
        {
            return new Anonymous { _dispose = dispose };
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly Disposable.<>c <>9 = new Disposable.<>c();

            internal void <.cctor>b__5_0()
            {
            }
        }

        private class Anonymous : IDisposable
        {
            internal Action _dispose;

            public void Dispose()
            {
                if (this._dispose != null)
                {
                    this._dispose = null;
                    this._dispose.Invoke();
                }
            }
        }
    }
}

