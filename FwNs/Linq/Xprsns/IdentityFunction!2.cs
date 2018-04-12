namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;

    internal class IdentityFunction<T1, T2>
    {
        public static readonly Function<T1, T2, T1> Function1;
        public static readonly Function<T1, T2, T2> Function2;

        static IdentityFunction()
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 = Expression.Parameter(typeof(T2), "y") };
            IdentityFunction<T1, T2>.Function1 = new Function<T1, T2, T1>(Expression.Lambda<Func<T1, T2, T1>>(expression = Expression.Parameter(typeof(T1), "x"), parameters));
            ParameterExpression[] expressionArray2 = new ParameterExpression[] { expression2 = Expression.Parameter(typeof(T1), "x"), expression };
            IdentityFunction<T1, T2>.Function2 = new Function<T1, T2, T2>(Expression.Lambda<Func<T1, T2, T2>>(expression = Expression.Parameter(typeof(T2), "y"), expressionArray2));
        }

        public static Func<T1, T2, T1> Delegate1
        {
            get
            {
                return IdentityFunction<T1, T2>.Function1.AsDelegate();
            }
        }

        public static Expression<Func<T1, T2, T1>> Lambda1
        {
            get
            {
                return IdentityFunction<T1, T2>.Function1.Expression;
            }
        }

        public static Func<T1, T2, T2> Delegate2
        {
            get
            {
                return IdentityFunction<T1, T2>.Function2.AsDelegate();
            }
        }

        public static Expression<Func<T1, T2, T2>> Lambda2
        {
            get
            {
                return IdentityFunction<T1, T2>.Function2.Expression;
            }
        }
    }
}

