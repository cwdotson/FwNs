namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;

    internal class IdentityFunction<T>
    {
        public static readonly Function<T, T> Function;

        static IdentityFunction()
        {
            ParameterExpression expression;
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            IdentityFunction<T>.Function = new Function<T, T>(Expression.Lambda<Func<T, T>>(expression = Expression.Parameter(typeof(T), "x"), parameters));
        }

        public static Func<T, T> Delegate
        {
            get
            {
                return IdentityFunction<T>.Function.AsDelegate();
            }
        }

        public static Expression<Func<T, T>> Lambda
        {
            get
            {
                return IdentityFunction<T>.Function.Expression;
            }
        }
    }
}

