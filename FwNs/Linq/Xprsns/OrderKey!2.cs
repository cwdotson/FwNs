namespace FwNs.Linq.Xprsns
{
    using FwNs.Core;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    internal class OrderKey<T, TKey> : OrderKey<T>
    {
        internal OrderKey(Function<T, TKey> keySelector, bool ascending) : base(OrderKey<T, TKey>.GetLambda(keySelector), ascending)
        {
            this.KeySelectorFunc = keySelector;
        }

        public OrderKey(Expression<Func<T, TKey>> keySelector, bool ascending) : base(keySelector, ascending)
        {
            this.KeySelectorFunc = new Function<T, TKey>(keySelector);
        }

        internal override IKeyComponent Clone(LambdaExpression keySelector, bool ascending)
        {
            Function<T, TKey> keySelectorFunc;
            if (keySelector == this.KeySelectorFunc.Expression)
            {
                if (ascending == base.Ascending)
                {
                    return this;
                }
                keySelectorFunc = this.KeySelectorFunc;
            }
            else
            {
                keySelectorFunc = new Function<T, TKey>(keySelector);
            }
            return new OrderKey<T, TKey>(keySelectorFunc, ascending);
        }

        internal override OrderKey<T2> Create<T2>(LambdaExpression keySelector)
        {
            return new OrderKey<T2, TKey>(keySelector as Expression<Func<T2, TKey>>, base.Ascending);
        }

        public override IComparer<T> CreateComparer(CultureInfo locale)
        {
            return Cmprsn.CreateItemComparer<T, TKey>(this.KeySelectorFunc, base.Ascending, locale);
        }

        internal override IIndexScanner<T> FindIndex(IIndexedSource<T> source)
        {
            throw new Exception();
        }

        private static LambdaExpression GetLambda(Function<T, TKey> keySelector)
        {
            Argument.NotNull<Function<T, TKey>>(keySelector, "keySelector");
            return keySelector.Expression;
        }

        public override IOrderedEnumerable<T> OrderBy(IEnumerable<T> source)
        {
            if (base.Ascending)
            {
                return Enumerable.OrderBy<T, TKey>(source, this.KeySelectorFunc.AsDelegate());
            }
            return Enumerable.OrderByDescending<T, TKey>(source, this.KeySelectorFunc.AsDelegate());
        }

        internal override OrderKey<T2> Project<T2>(Function<T2, T> function)
        {
            return new OrderKey<T2, TKey>(this.KeySelectorFunc.Substitute<T2, T>(function), base.Ascending);
        }

        public override IOrderedEnumerable<T> ThenBy(IOrderedEnumerable<T> source)
        {
            if (base.Ascending)
            {
                return Enumerable.ThenBy<T, TKey>(source, this.KeySelectorFunc.AsDelegate());
            }
            return Enumerable.ThenByDescending<T, TKey>(source, this.KeySelectorFunc.AsDelegate());
        }

        internal Function<T, TKey> KeySelectorFunc { get; private set; }

        public Expression<Func<T, TKey>> KeySelector
        {
            get
            {
                return this.KeySelectorFunc.Expression;
            }
        }
    }
}

