namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    internal abstract class OrderKey<T> : IEquatable<OrderKey<T>>, IKeyComponent<T>, IKeyComponent
    {
        internal OrderKey(LambdaExpression keySelector, bool ascending)
        {
            Argument.NotNull<LambdaExpression>(keySelector, "keySelector");
            this.KeySelector = keySelector;
            this.Ascending = ascending;
        }

        internal abstract IKeyComponent Clone(LambdaExpression keySelector, bool ascending);
        internal abstract OrderKey<T2> Create<T2>(LambdaExpression keySelector);
        public abstract IComparer<T> CreateComparer(CultureInfo locale);
        public bool Equals(OrderKey<T> other)
        {
            if (other == null)
            {
                return false;
            }
            return ((this == other) || ((this.Ascending == other.Ascending) && Xtnz.DeepEquals(this.KeySelector, other.KeySelector, ExpressionComparisonOptions.ParametersByIndex)));
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }
            if (this == other)
            {
                return true;
            }
            OrderKey<T> key = other as OrderKey<T>;
            return ((key != null) && this.Equals(key));
        }

        internal abstract IIndexScanner<T> FindIndex(IIndexedSource<T> source);
        IKeyComponent IKeyComponent.Clone(LambdaExpression keySelector, FwNs.Linq.Xprsns.Order order)
        {
            return this.Clone(keySelector, order == FwNs.Linq.Xprsns.Order.Ascending);
        }

        object IKeyComponent.ToOrderKey()
        {
            return this;
        }

        public override int GetHashCode()
        {
            return ((Xtnz.DeepGetHashCode(this.KeySelector, ExpressionComparisonOptions.ParametersByIndex) * 0x18d) ^ this.Ascending.GetHashCode());
        }

        public static bool operator ==(OrderKey<T> left, OrderKey<T> right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(OrderKey<T> left, OrderKey<T> right)
        {
            return !object.Equals(left, right);
        }

        public abstract IOrderedEnumerable<T> OrderBy(IEnumerable<T> source);
        internal abstract OrderKey<T2> Project<T2>(Function<T2, T> function);
        public abstract IOrderedEnumerable<T> ThenBy(IOrderedEnumerable<T> source);

        public LambdaExpression KeySelector { get; private set; }

        public bool Ascending { get; private set; }

        internal FwNs.Linq.Xprsns.Order Order
        {
            get
            {
                if (!this.Ascending)
                {
                    return FwNs.Linq.Xprsns.Order.Descending;
                }
                return FwNs.Linq.Xprsns.Order.Ascending;
            }
        }

        FwNs.Linq.Xprsns.Order IKeyComponent.Order
        {
            get
            {
                return this.Order;
            }
        }
    }
}

