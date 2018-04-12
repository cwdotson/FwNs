namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;

    internal class BindingObserver : DependencyObject, IDisposable
    {
        private bool _disposed;
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BindingObserver), new PropertyMetadata(new PropertyChangedCallback(BindingObserver.OnChanged)));

        [field: CompilerGenerated]
        public event EventHandler Changed;

        public BindingObserver(Bndng binding)
        {
            throw new Exception();
        }

        public void Dispose()
        {
            this._disposed = true;
            base.ClearValue(ValueProperty);
        }

        private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BindingObserver observer = sender as BindingObserver;
            if ((observer != null) && !observer._disposed)
            {
                observer.Changed(observer, EventArgs.Empty);
            }
        }

        public static BindingObserver Subscribe(Bndng binding, Action onChanged)
        {
            BindingObserver observer1 = new BindingObserver(binding);
            observer1.Changed += delegate (object <p0>, EventArgs <p1>) {
                onChanged.Invoke();
            };
            return observer1;
        }

        public static BindingObserver Subscribe<T>(Bndng binding, Action<T> onNext)
        {
            BindingObserver observer = new BindingObserver(binding);
            observer.Changed += delegate (object <p0>, EventArgs <p1>) {
                onNext((T) observer.Value);
            };
            return observer;
        }

        public static BindingObserver Subscribe(DependencyObject source, string propertyPath, Action onChanged)
        {
            Bndng binding = new Bndng(propertyPath) {
                Source = source
            };
            return Subscribe(binding, onChanged);
        }

        public static BindingObserver Subscribe<T>(DependencyObject source, string propertyPath, Action<T> onNext)
        {
            Bndng binding = new Bndng(propertyPath) {
                Source = source
            };
            return Subscribe<T>(binding, onNext);
        }

        public object Value
        {
            get
            {
                return base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly BindingObserver.<>c <>9 = new BindingObserver.<>c();
            public static EventHandler <>9__8_0;

            internal void <.ctor>b__8_0(object <p0>, EventArgs <p1>)
            {
            }
        }
    }
}

