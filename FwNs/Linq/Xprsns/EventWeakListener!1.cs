namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Reflection;

    internal abstract class EventWeakListener<T> : WeakListener<T>
    {
        protected EventWeakListener()
        {
        }

        internal sealed class Impl<THandler, TEventArgs> : EventWeakListener<T> where THandler: class where TEventArgs: EventArgs
        {
            private readonly EventHandler<TEventArgs> _handler;
            private readonly Func<EventHandler<TEventArgs>, THandler> _handlerConverter;
            private readonly WeakReference _weakHandler;
            private Mediator<T, THandler, TEventArgs> _mediator;
            private readonly Action<T, THandler> _addHandler;
            private readonly Action<T, THandler> _removeHandler;

            public Impl(Action<T, THandler> addHandler, Action<T, THandler> removeHandler, EventHandler<TEventArgs> handler, Func<EventHandler<TEventArgs>, THandler> handlerConverter)
            {
                TypeArgument.IsDelegate<THandler>("THandler");
                Argument.NotNull<EventHandler<TEventArgs>>(handler, "handler");
                this._handler = handler;
                this._handlerConverter = handlerConverter;
                this._addHandler = addHandler;
                this._removeHandler = removeHandler;
                this._weakHandler = new WeakReference(this._handler);
                this._mediator = new Mediator<T, THandler, TEventArgs>(this._weakHandler, this._removeHandler, handlerConverter);
            }

            public override void Clear()
            {
                this._mediator.Dispose();
                this._mediator = new Mediator<T, THandler, TEventArgs>(this._weakHandler, this._removeHandler, this._handlerConverter);
            }

            protected override void Dispose(bool disposing)
            {
                if (this._mediator != null)
                {
                    this._mediator.Dispose();
                }
            }

            public override void StartListening(T source)
            {
                base.CheckDisposed();
                this._addHandler.Invoke(source, this._mediator._selfHandleDelegate);
            }

            public override void StopListening(T source)
            {
                base.CheckDisposed();
                this._removeHandler.Invoke(source, this._mediator._selfHandleDelegate);
            }

            internal class Mediator : IDisposable
            {
                private WeakReference _weakRealHandler;
                public readonly THandler _selfHandleDelegate;
                private readonly Action<T, THandler> _removeListener;
                private static readonly MethodInfo miHandle;

                static Mediator()
                {
                    EventWeakListener<T>.Impl<THandler, TEventArgs>.Mediator.miHandle = typeof(EventWeakListener<T>.Impl<THandler, TEventArgs>.Mediator).GetMethod("Handle");
                }

                public Mediator(WeakReference handler, Action<T, THandler> removeListener, Func<EventHandler<TEventArgs>, THandler> handlerConverter)
                {
                    this._weakRealHandler = handler;
                    this._removeListener = removeListener;
                    this._selfHandleDelegate = handlerConverter.Invoke(new EventHandler<TEventArgs>(this.Handle));
                }

                public void Dispose()
                {
                    this._weakRealHandler = null;
                }

                public void Handle(object sender, TEventArgs e)
                {
                    WeakReference reference = this._weakRealHandler;
                    if (reference != null)
                    {
                        EventHandler<TEventArgs> target = reference.Target as EventHandler<TEventArgs>;
                        if (target != null)
                        {
                            target(sender, e);
                            return;
                        }
                        this.Dispose();
                    }
                    if (sender is T)
                    {
                        this._removeListener.Invoke((T) sender, this._selfHandleDelegate);
                    }
                }
            }
        }
    }
}

