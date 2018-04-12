namespace FwNs.Core
{
    using FwNs.Core.Typs;
    using FwNs.Linq.Xprsns;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class Cmprsn
    {
        [Extension]
        public static IComparer<T> Add<T>(IComparer<T> first, IComparer<T> second)
        {
            CompositeComparer<T> comparer = first as CompositeComparer<T>;
            if (comparer != null)
            {
                return comparer.Add(second, false);
            }
            CompositeComparer<T> comparer2 = first as CompositeComparer<T>;
            if (comparer2 != null)
            {
                return comparer2.Add(first, true);
            }
            return new CompositeComparer<T>(new IComparer<T>[] { first, second });
        }

        [Extension]
        public static IComparer<T> Add<T, TKey>(IComparer<T> first, Func<T, TKey> second, CultureInfo locale)
        {
            return Add<T, TKey>(first, second, true, locale);
        }

        [Extension]
        public static IComparer<T> Add<T, TKey>(IComparer<T> first, Func<T, TKey> second, bool sign, CultureInfo locale)
        {
            return Add<T>(first, CreateItemComparer<T, TKey>(second, sign, locale));
        }

        [Extension]
        internal static IComparer<T> CreateComparer<T>(Comparison<T> func)
        {
            return new CustomComparer<T>(func);
        }

        [Extension]
        public static IComparer<T> CreateItemComparer<T, TKey>(Function<T, TKey> lambda, CultureInfo locale)
        {
            return CreateItemComparer<T, TKey>(lambda, true, locale);
        }

        [Extension]
        public static IComparer<T> CreateItemComparer<T, TKey>(Func<T, TKey> lambda, CultureInfo locale)
        {
            return CreateItemComparer<T, TKey>(lambda, true, locale);
        }

        [Extension]
        public static IComparer<T> CreateItemComparer<T, TKey>(Function<T, TKey> func, bool sign, CultureInfo locale)
        {
            Argument.NotNull<Function<T, TKey>>(func, "func");
            Argument.NotNull<CultureInfo>(locale, "locale");
            return Project<T, TKey>(CreateKeyComparer<T, TKey>(func.Expression, sign, locale), func.AsDelegate());
        }

        [Extension]
        public static IComparer<T> CreateItemComparer<T, TKey>(Func<T, TKey> lambda, bool sign, CultureInfo locale)
        {
            return Project<T, TKey>(GetDefaultComparer<TKey>(locale), lambda, sign);
        }

        [Extension]
        public static IComparer<TResult> CreateKeyComparer<TSource, TResult>(Expression<Func<TSource, TResult>> lambda, CultureInfo locale)
        {
            return CreateKeyComparer<TSource, TResult>(lambda, true, locale);
        }

        internal static IComparer<T> CreateKeyComparer<T>(Expression expression, bool sign, CultureInfo locale)
        {
            Argument.NotNull<Expression>(expression, "expression");
            Argument.ThrowIf(!expression.Type.IsAssignableFrom(typeof(T)), Errorz.KeyComparer_InvalidTypes);
            switch (expression.NodeType)
            {
                case ExpressionType.MemberInit:
                {
                    MemberInitExpression expression1 = (MemberInitExpression) expression;
                    List<Expression> expressions = new List<Expression>(expression1.Bindings.Count);
                    List<MemberInfo> members = new List<MemberInfo>(expression1.Bindings.Count);
                    foreach (MemberAssignment assignment in Enumerable.OfType<MemberAssignment>(expression1.Bindings))
                    {
                        expressions.Add(assignment.Expression);
                        members.Add(assignment.Member);
                    }
                    return new CompositeComparer<T>(expressions, members, sign, locale);
                }
                case ExpressionType.New:
                {
                    NewExpression expression2 = (NewExpression) expression;
                    if (expression2.Members != null)
                    {
                        return new CompositeComparer<T>(expression2.Arguments, expression2.Members, sign, locale);
                    }
                    break;
                }
            }
            return GetDefaultComparer<T>(sign, locale);
        }

        [Extension]
        public static IComparer<TKey> CreateKeyComparer<T, TKey>(Expression<Func<T, TKey>> lambda, bool sign, CultureInfo locale)
        {
            Argument.NotNull<Expression<Func<T, TKey>>>(lambda, "lambda");
            Argument.NotNull<CultureInfo>(locale, "locale");
            return CreateKeyComparer<TKey>(lambda.Body, sign, locale);
        }

        internal static IComparer<T> CreateProjectionComparer<T>(LambdaExpression selector, CultureInfo locale, bool ascending)
        {
            Type[] typeArguments = new Type[] { typeof(T), selector.Body.Type };
            object[] args = new object[] { selector, locale, ascending };
            return (IComparer<T>) Activator.CreateInstance(typeof(ProjectionComparer).MakeGenericType(typeArguments), args);
        }

        internal static IComparer<T> CreateProjectionComparer<T>(Expression expr, MemberInfo member, CultureInfo locale, bool ascending)
        {
            Type[] typeArguments = new Type[] { typeof(T), expr.Type };
            object[] args = new object[] { member, expr, locale, ascending };
            return (IComparer<T>) Activator.CreateInstance(typeof(ProjectionComparer).MakeGenericType(typeArguments), args);
        }

        [Extension]
        public static bool Equal<T>(IComparer<T> comparer, T x, T y)
        {
            return (comparer.Compare(x, y) == 0);
        }

        private static IComparer<T> GetDefaultComparer<T>(CultureInfo locale)
        {
            if (typeof(T) == typeof(string))
            {
                return (StringComparer.Create(locale, false) as IComparer<T>);
            }
            return Comparer<T>.Default;
        }

        public static IComparer<T> GetDefaultComparer<T>(bool sign, CultureInfo locale)
        {
            IComparer<T> defaultComparer = GetDefaultComparer<T>(locale);
            if (!sign)
            {
                defaultComparer = Negate<T>(defaultComparer);
            }
            return defaultComparer;
        }

        [Extension]
        public static bool IsGreater<T>(IComparer<T> comparer, T x, T y, bool including)
        {
            int num = comparer.Compare(x, y);
            if (including)
            {
                return (num >= 0);
            }
            return (num > 0);
        }

        [Extension]
        public static bool IsLess<T>(IComparer<T> comparer, T x, T y, bool including)
        {
            int num = comparer.Compare(x, y);
            if (including)
            {
                return (num <= 0);
            }
            return (num < 0);
        }

        [Extension]
        internal static IComparer<T> Negate<T>(IComparer<T> comparer)
        {
            return new CustomComparer<T>((x, y) => -comparer.Compare(x, y));
        }

        [Extension]
        public static IComparer<TSource> Project<TSource, TResult>(IComparer<TResult> comparer, Func<TSource, TResult> selector)
        {
            return Project<TSource, TResult>(comparer, selector, true);
        }

        [Extension]
        public static IComparer<TSource> Project<TSource, TResult>(IComparer<TResult> comparer, Func<TSource, TResult> selector, bool ascending)
        {
            return new ProjectionComparer<TSource, TResult>(selector, comparer, ascending);
        }

        internal class CompositeComparer<T> : IComparer<T>
        {
            internal readonly IComparer<T>[] _subComparers;
            private readonly int _sign;

            public CompositeComparer(params IComparer<T>[] subComparers) : this(true, subComparers)
            {
            }

            public CompositeComparer(bool sign, params IComparer<T>[] subComparers)
            {
                this._subComparers = subComparers.Clone() as IComparer<T>[];
                for (int i = 0; i < this._subComparers.Length; i++)
                {
                }
                this._sign = sign ? 1 : -1;
            }

            public CompositeComparer(IList<Expression> expressions, IList<MemberInfo> members, bool sign, CultureInfo locale)
            {
                this._subComparers = new IComparer<T>[expressions.Count];
                for (int i = 0; i < this._subComparers.Length; i++)
                {
                    Expression expr = expressions[i];
                    bool ascending = !Cmprsn.CompositeComparer<T>.IsDescending(ref expr);
                    this._subComparers[i] = Cmprsn.CreateProjectionComparer<T>(expr, members[i], locale, ascending);
                }
                this._sign = sign ? 1 : -1;
            }

            public Cmprsn.CompositeComparer<T> Add(IComparer<T> comparer, bool first)
            {
                IComparer<T>[] comparerArray;
                Cmprsn.CompositeComparer<T> comparer2 = comparer as Cmprsn.CompositeComparer<T>;
                if (comparer2 == null)
                {
                    comparerArray = new IComparer<T>[this._subComparers.Length + 1];
                    if (first)
                    {
                        comparerArray[0] = comparer;
                        this._subComparers.CopyTo(comparerArray, 1);
                    }
                    else
                    {
                        this._subComparers.CopyTo(comparerArray, 0);
                        comparerArray[comparerArray.Length - 1] = comparer;
                    }
                }
                else
                {
                    comparerArray = new IComparer<T>[this._subComparers.Length + comparer2._subComparers.Length];
                    if (first)
                    {
                        comparer2._subComparers.CopyTo(comparerArray, 0);
                        this._subComparers.CopyTo(comparerArray, comparer2._subComparers.Length);
                    }
                    else
                    {
                        this._subComparers.CopyTo(comparerArray, 0);
                        comparer2._subComparers.CopyTo(comparerArray, this._subComparers.Length);
                    }
                }
                return new Cmprsn.CompositeComparer<T>(comparerArray);
            }

            public int Compare(T x, T y)
            {
                for (int i = 0; i < this._subComparers.Length; i++)
                {
                    int num2 = this._sign * this._subComparers[i].Compare(x, y);
                    if (num2 != 0)
                    {
                        return num2;
                    }
                }
                return 0;
            }

            private static bool IsDescending(ref Expression expr)
            {
                MethodCallExpression expression = expr as MethodCallExpression;
                if ((expression != null) && ((expression.Method.DeclaringType == typeof(Hints)) && (expression.Method.Name == "Descending")))
                {
                    expr = expression.Arguments[0];
                    return true;
                }
                return false;
            }
        }

        private class CustomComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> _func;

            public CustomComparer(Comparison<T> func)
            {
                this._func = func;
            }

            public int Compare(T x, T y)
            {
                return this._func(x, y);
            }
        }

        internal class ProjectionComparer<T, TKey> : IComparer<T>
        {
            private readonly Func<T, TKey> _keySelector;
            private readonly IComparer<TKey> _subComparer;
            private readonly int _ascending;

            public ProjectionComparer(Function<T, TKey> selector, CultureInfo locale, bool ascending)
            {
                this._keySelector = selector.AsDelegate();
                this._subComparer = Cmprsn.CreateKeyComparer<T, TKey>(selector.Expression, locale);
                this._ascending = ascending ? 1 : -1;
            }

            public ProjectionComparer(Func<T, TKey> keySelector, IComparer<TKey> subComparer, bool ascending)
            {
                this._keySelector = keySelector;
                this._subComparer = subComparer;
                this._ascending = ascending ? 1 : -1;
            }

            public ProjectionComparer(MemberInfo member, Expression value, CultureInfo locale, bool ascending)
            {
                PropertyInfo info = member as PropertyInfo;
                if (info != null)
                {
                    member = info.GetGetMethod(true);
                }
                MethodInfo method = member as MethodInfo;
                if ((method != null) && TypsFw.CanBeDelegateSource(method))
                {
                    this._keySelector = TypsFw.CreateDelegate<Func<T, TKey>>(method);
                }
                else
                {
                    ParameterExpression expression = null;
                    ParameterExpression[] parameters = new ParameterExpression[] { expression };
                    this._keySelector = Expression.Lambda<Func<T, TKey>>(Expression.MakeMemberAccess(expression, member), parameters).Compile();
                }
                this._subComparer = Cmprsn.CreateKeyComparer<TKey>(value, true, locale);
                this._ascending = ascending ? 1 : -1;
            }

            public int Compare(T x, T y)
            {
                return (this._ascending * this._subComparer.Compare(this._keySelector.Invoke(x), this._keySelector.Invoke(y)));
            }
        }
    }
}

