namespace FwNs.Linq.Xprsns
{
    using FwNs.Core.Typs;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    internal class EventWeakListener
    {
        private static int _handlerConverterCounter;
        private static MethodInfo _prepareMethod = typeof(EventWeakListener).GetMethod("PrepareCore", BindingFlags.NonPublic | BindingFlags.Static);
        public static readonly Factory<INotifyPropertyChanged, PropertyChangedEventArgs> NotifyPropertyChanged = Prepare<INotifyPropertyChanged, PropertyChangedEventArgs>("PropertyChanged");
        public static readonly Factory<INotifyPropertyChanging, PropertyChangingEventArgs> NotifyPropertyChanging = Prepare<INotifyPropertyChanging, PropertyChangingEventArgs>("PropertyChanging");
        public static readonly Factory<IBindingList, ListChangedEventArgs> BindingListChanged = Prepare<IBindingList, ListChangedEventArgs>("ListChanged");
        public static readonly Factory<INotifyCollectionChanged, NotifyCollectionChangedEventArgs> NotifyCollectionChanged = Prepare<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>("CollectionChanged");
        public static readonly Factory<XObject, XObjectChangeEventArgs> XObjectChanged = Prepare<XObject, XObjectChangeEventArgs>("Changed");

        public static EventWeakListener<TTarget> Create<TTarget, THandler, TEventArgs>(Action<TTarget, THandler> addHandler, Action<TTarget, THandler> removeHandler, EventHandler<TEventArgs> handler, Func<EventHandler<TEventArgs>, THandler> handlerConverter) where THandler: class where TEventArgs: EventArgs
        {
            return new EventWeakListener<TTarget>.Impl<THandler, TEventArgs>(addHandler, removeHandler, handler, handlerConverter);
        }

        public static EventWeakListener<IObservableSource<T>> ObservableSourceChanged<T>(EventHandler<SourceChangeEventArgs<T>> handler)
        {
            return ObservableSourceChanged<IObservableSource<T>, T>(handler);
        }

        public static EventWeakListener<TSource> ObservableSourceChanged<TSource, TElement>(EventHandler<SourceChangeEventArgs<TElement>> handler) where TSource: IObservableSource<TElement>
        {
            Action<TSource, EventHandler<SourceChangeEventArgs<TElement>>> removeHandler = <>c__13<TSource, TElement>.<>9__13_1 ?? (<>c__13<TSource, TElement>.<>9__13_1 = new Action<TSource, EventHandler<SourceChangeEventArgs<TElement>>>(<>c__13<TSource, TElement>.<>9, this.<ObservableSourceChanged>b__13_1));
            Func<EventHandler<SourceChangeEventArgs<TElement>>, EventHandler<SourceChangeEventArgs<TElement>>> handlerConverter = <>c__13<TSource, TElement>.<>9__13_2 ?? (<>c__13<TSource, TElement>.<>9__13_2 = new Func<EventHandler<SourceChangeEventArgs<TElement>>, EventHandler<SourceChangeEventArgs<TElement>>>(<>c__13<TSource, TElement>.<>9, this.<ObservableSourceChanged>b__13_2));
            return Create<TSource, EventHandler<SourceChangeEventArgs<TElement>>, SourceChangeEventArgs<TElement>>(<>c__13<TSource, TElement>.<>9__13_0 ?? (<>c__13<TSource, TElement>.<>9__13_0 = new Action<TSource, EventHandler<SourceChangeEventArgs<TElement>>>(<>c__13<TSource, TElement>.<>9, this.<ObservableSourceChanged>b__13_0)), removeHandler, handler, handlerConverter);
        }

        public static Factory<TTarget, TEventArgs> Prepare<TTarget, TEventArgs>(EventInfo @event) where TEventArgs: EventArgs
        {
            Type[] typeArguments = new Type[] { typeof(TTarget), @event.EventHandlerType, typeof(TEventArgs) };
            object[] parameters = new object[] { @event };
            return (Factory<TTarget, TEventArgs>) _prepareMethod.MakeGenericMethod(typeArguments).Invoke(null, parameters);
        }

        public static Factory<TTarget, TEventArgs> Prepare<TTarget, TEventArgs>(string eventName) where TEventArgs: EventArgs
        {
            return Prepare<TTarget, TEventArgs>(typeof(TTarget).GetEvent(eventName));
        }

        internal static Factory<TTarget, TEventArgs> PrepareCore<TTarget, THandler, TEventArgs>(EventInfo @event) where THandler: class where TEventArgs: EventArgs
        {
            // This item is obfuscated and can not be translated.
            throw new Exception();
        }

        private static Func<EventHandler<TEventArgs>, THandler> PrepareHandlerConverter<TEventArgs, THandler>() where TEventArgs: EventArgs
        {
            ParameterExpression expression = Expression.Parameter(typeof(EventHandler<TEventArgs>));
            ParameterExpression expression2 = Expression.Parameter(typeof(object));
            ParameterExpression expression3 = Expression.Parameter(typeof(TEventArgs));
            Expression[] arguments = new Expression[] { expression2, expression3 };
            ParameterExpression[] parameters = new ParameterExpression[] { expression2, expression3 };
            ParameterExpression[] expressionArray3 = new ParameterExpression[] { expression };
            Expression<Func<EventHandler<TEventArgs>, THandler>> expression4 = Expression.Lambda<Func<EventHandler<TEventArgs>, THandler>>(Expression.Lambda<THandler>(Expression.Invoke(expression, arguments), parameters), expressionArray3);
            TypeBuilder builder1 = TypeGeneration.Module.DefineType("HandlerConverters.Converter" + _handlerConverterCounter++, TypeAttributes.Public);
            MethodBuilder method = builder1.DefineMethod("Convert", MethodAttributes.Static | MethodAttributes.Public);
            expression4.CompileToMethod(method);
            return TypsFw.CreateDelegate<Func<EventHandler<TEventArgs>, THandler>>(builder1.CreateType().GetMethod("Convert", BindingFlags.Public | BindingFlags.Static));
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__13<TSource, TElement> where TSource: IObservableSource<TElement>
        {
            public static readonly EventWeakListener.<>c__13<TSource, TElement> <>9;
            public static Action<TSource, EventHandler<SourceChangeEventArgs<TElement>>> <>9__13_0;
            public static Action<TSource, EventHandler<SourceChangeEventArgs<TElement>>> <>9__13_1;
            public static Func<EventHandler<SourceChangeEventArgs<TElement>>, EventHandler<SourceChangeEventArgs<TElement>>> <>9__13_2;

            static <>c__13()
            {
                EventWeakListener.<>c__13<TSource, TElement>.<>9 = new EventWeakListener.<>c__13<TSource, TElement>();
            }

            internal void <ObservableSourceChanged>b__13_0(TSource x, EventHandler<SourceChangeEventArgs<TElement>> h)
            {
                x.Changed += h;
            }

            internal void <ObservableSourceChanged>b__13_1(TSource x, EventHandler<SourceChangeEventArgs<TElement>> h)
            {
                x.Changed -= h;
            }

            internal EventHandler<SourceChangeEventArgs<TElement>> <ObservableSourceChanged>b__13_2(EventHandler<SourceChangeEventArgs<TElement>> h)
            {
                return delegate (object s, SourceChangeEventArgs<TElement> e) {
                    h(s, e);
                };
            }
        }

        public delegate EventWeakListener<TTarget> Factory<TTarget, TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs: EventArgs;
    }
}

