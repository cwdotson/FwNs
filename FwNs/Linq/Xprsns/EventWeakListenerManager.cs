namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Xml.Linq;

    internal class EventWeakListenerManager
    {
        private readonly List<IListener> _listeners = new List<IListener>(2);

        private void AddAndListen<T>(T source, IListener<T> listener)
        {
            this._listeners.Add(listener);
            listener.StartListening(source);
        }

        public void Clear()
        {
            using (List<IListener>.Enumerator enumerator = this._listeners.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Clear();
                }
            }
            this._listeners.Clear();
        }

        public void OnBindingListChanged(IBindingList source, EventHandler<ListChangedEventArgs> handler)
        {
            this.AddAndListen<IBindingList>(source, EventWeakListener.BindingListChanged(handler));
        }

        public void OnCollectionChanged(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler)
        {
            this.AddAndListen<INotifyCollectionChanged>(source, EventWeakListener.NotifyCollectionChanged(handler));
        }

        public void OnObservableSourceChanged<T>(IObservableSource<T> source, EventHandler<SourceChangeEventArgs<T>> handler)
        {
            this.AddAndListen<IObservableSource<T>>(source, EventWeakListener.ObservableSourceChanged<T>(handler));
        }

        public void OnPropertyChanged(INotifyPropertyChanged source, EventHandler<PropertyChangedEventArgs> handler)
        {
            this.AddAndListen<INotifyPropertyChanged>(source, EventWeakListener.NotifyPropertyChanged(handler));
        }

        public void OnPropertyChanging(INotifyPropertyChanging source, EventHandler<PropertyChangingEventArgs> handler)
        {
            this.AddAndListen<INotifyPropertyChanging>(source, EventWeakListener.NotifyPropertyChanging(handler));
        }

        public void OnXObjectChanged(XObject source, EventHandler<XObjectChangeEventArgs> handler)
        {
            this.AddAndListen<XObject>(source, EventWeakListener.XObjectChanged(handler));
        }

        public void StartListening<T, THandler, TEventArgs>(T source, EventWeakListener.Factory<T, TEventArgs> listenerFactory, EventHandler<TEventArgs> handler) where THandler: class where TEventArgs: EventArgs
        {
            this.AddAndListen<T>(source, listenerFactory(handler));
        }

        public void StartListening<T, THandler, TEventArgs>(T source, Action<T, THandler> addHandler, Action<T, THandler> removeHandler, EventHandler<TEventArgs> handler, Func<EventHandler<TEventArgs>, THandler> handlerConverter) where T: class where THandler: class where TEventArgs: EventArgs
        {
            this.AddAndListen<T>(source, EventWeakListener.Create<T, THandler, TEventArgs>(addHandler, removeHandler, handler, handlerConverter));
        }
    }
}

