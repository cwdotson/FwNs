namespace FwNs.Linq.Xprsns
{
    using FwNs.Core.Typs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml.Linq;

    [Extension]
    public static class Xtnz
    {
        public static readonly Expression Nothing = Expression.Constant("Nothing");

        [Extension]
        internal static Function<T, bool> AndAlso<T>(Function<T, bool> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                if (right == null)
                {
                    return null;
                }
                return new Function<T, bool>(right);
            }
            if (right == null)
            {
                return left;
            }
            return new Function<T, bool>(Expression.AndAlso(left.Body, Lambda.ExtractBody(right, left.Parameter)), left.Parameter);
        }

        [Extension]
        internal static LambdaExpression AndAlso(LambdaExpression left, LambdaExpression right)
        {
            if (left == null)
            {
                if (right == null)
                {
                    return null;
                }
                return right;
            }
            if (right == null)
            {
                return left;
            }
            ParameterExpression[] parameters = new ParameterExpression[] { left.Parameters[0] };
            return Expression.Lambda(Expression.AndAlso(left.Body, Lambda.ExtractBody(right, left.Parameters[0])), parameters);
        }

        public static void CheckCopyToArgs(Array array, int arrayIndex, int actualCount)
        {
            Argument.NotNull<Array>(array, "array");
            Argument.NotLess(arrayIndex, 0, "arrayIndex");
            if ((array.Length - arrayIndex) < actualCount)
            {
                throw new OverflowException(Errorz.Argument_ArrayIsTooSmall);
            }
            if ((actualCount != 0) && (array.Length > 0))
            {
                Argument.NotGreater(arrayIndex, array.Length - 1, "arrayIndex");
            }
        }

        [Extension]
        public static bool Contains(Expression expression, Expression value)
        {
            return Contains(expression, value, ExpressionEqualityComparison.Reference);
        }

        [Extension]
        public static bool Contains(Expression expression, ExpressionType nodeType)
        {
            <>c__DisplayClass22_0 class_;
            return Enumerable.Any<Expression>(Expand(expression), new Func<Expression, bool>(class_, this.<Contains>b__0));
        }

        [Extension]
        public static bool Contains(Expression expression, Expression value, ExpressionEqualityComparison comparison)
        {
            return Contains(expression, value, GetComparer(comparison));
        }

        [Extension]
        public static bool Contains(Expression expression, Expression value, IEqualityComparer<Expression> comparer)
        {
            Argument.NotNull<Expression>(expression, "expression");
            Argument.NotNull<IEqualityComparer<Expression>>(comparer, "comparer");
            return ((value != null) && Enumerable.Contains<Expression>(Expand(expression), value, comparer));
        }

        [Extension]
        public static bool DeepEquals(Expression left, Expression right, ExpressionComparisonOptions options)
        {
            if (left == right)
            {
                return true;
            }
            if ((left == null) || (right == null))
            {
                return false;
            }
            return ((options == ExpressionComparisonOptions.None) ? ExpressionEqualityComparer.Instance : new ExpressionEqualityComparer(options)).Equals(left, right);
        }

        [Extension]
        public static int DeepGetHashCode(Expression exp, ExpressionComparisonOptions options)
        {
            return new ExpressionEqualityComparer(options).GetHashCode(exp);
        }

        [Extension]
        internal static bool DoesReturn(Expression containerExpr, Expression subExpr, bool allowConversion)
        {
            Expression operand = containerExpr;
            if (allowConversion)
            {
                while ((operand.NodeType == ExpressionType.Convert) || (operand.NodeType == ExpressionType.ConvertChecked))
                {
                    operand = ((UnaryExpression) operand).Operand;
                }
            }
            return (operand == subExpr);
        }

        [Extension]
        internal static Expression EliminateNulls(Expression expression)
        {
            return new NullEliminator().Rewrite(expression);
        }

        [Extension]
        internal static Expression EnsureType(Expression expr, Type type)
        {
            if (expr.Type != type)
            {
                expr = Expression.Convert(expr, type);
            }
            return expr;
        }

        [Extension]
        public static T Eval<T>(Expression expression)
        {
            T local;
            Argument.NotNull<Expression>(expression, "expression");
            ConstantExpression expression2 = expression as ConstantExpression;
            if (expression2 != null)
            {
                return (T) expression2.Value;
            }
            try
            {
                local = new Constant<T>(expression).Eval();
            }
            catch (Exception exception)
            {
                throw new ArgumentException(InvariantFormat(EMExpressions.Eval_Cannot, expression), exception);
            }
            return local;
        }

        [Extension]
        public static IEnumerable<Expression> Expand(Expression expression)
        {
            return Expand(expression, true);
        }

        [Extension]
        public static IEnumerable<Expression> Expand(Expression expression, bool recursive)
        {
            Argument.NotNull<Expression>(expression, "expression");
            if (!recursive)
            {
                return ExpandIterator(expression);
            }
            return Expand(expression, <>c.<>9__17_0 ?? (<>c.<>9__17_0 = new Func<Expression, bool>(<>c.<>9, this.<Expand>b__17_0)));
        }

        [IteratorStateMachine(typeof(<Expand>d__15)), Extension]
        public static IEnumerable<Expression> Expand(Expression expression, Func<Expression, bool> goDeeper)
        {
            return new <Expand>d__15(-2) { 
                <>3__expression = expression,
                <>3__goDeeper = goDeeper
            };
        }

        [IteratorStateMachine(typeof(<ExpandIterator>d__16))]
        private static IEnumerable<Expression> ExpandIterator(Expression expression)
        {
            return new <ExpandIterator>d__16(-2) { <>3__expression = expression };
        }

        [IteratorStateMachine(typeof(<FindPropertyDependencies>d__46))]
        internal static IEnumerable<PropertyInfo[]> FindPropertyDependencies(LambdaExpression expression)
        {
            return new <FindPropertyDependencies>d__46(-2) { <>3__expression = expression };
        }

        [Extension]
        public static IEqualityComparer<Expression> GetComparer(ExpressionEqualityComparison comparison)
        {
            if (comparison == ExpressionEqualityComparison.Content)
            {
                return ExpressionEqualityComparer.Instance;
            }
            return EqualityComparer<Expression>.Default;
        }

        [Extension]
        public static string InvariantFormat(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        [Extension]
        public static string InvariantFormat(string format, object arg1)
        {
            object[] args = new object[] { arg1 };
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        [Extension]
        public static string InvariantFormat(string format, object arg1, object arg2)
        {
            object[] args = new object[] { arg1, arg2 };
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        [Extension]
        public static string InvariantFormat(string format, object arg1, object arg2, object arg3)
        {
            object[] args = new object[] { arg1, arg2, arg3 };
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        [Extension]
        public static string InvariantFormat(string format, object arg1, object arg2, object arg3, object arg4)
        {
            object[] args = new object[] { arg1, arg2, arg3, arg4 };
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        [Extension]
        internal static bool IsAnd(ExpressionType type)
        {
            if (type != ExpressionType.AndAlso)
            {
                return (type == ExpressionType.And);
            }
            return true;
        }

        [Extension]
        internal static bool IsAssociative(BinaryExpression expr)
        {
            if (expr.Method != null)
            {
                return false;
            }
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return TypsFw.IsNumeric(expr.Type);

                case ExpressionType.And:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Or:
                    return IsLogical(expr);

                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.NotEqual:
                case ExpressionType.OrElse:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return false;
            }
            throw new Exception();
        }

        [Extension]
        internal static bool IsAssociative(Expression expr)
        {
            BinaryExpression expression = expr as BinaryExpression;
            return ((expression != null) && IsAssociative(expression));
        }

        [Extension]
        internal static bool IsCnf(Expression expression)
        {
            Argument.NotNull<Expression>(expression, "expression");
            Stack<Expression> stack = new Stack<Expression>();
            stack.Push(expression);
            while (stack.Count > 0)
            {
                Expression expression2 = stack.Pop();
                if (IsAnd(expression2.NodeType))
                {
                    BinaryExpression expression3 = expression2 as BinaryExpression;
                    stack.Push(expression3.Left);
                    stack.Push(expression3.Right);
                }
                else if (Enumerable.Any<Expression>(Expand(expression2), <>c.<>9__49_0 ?? (<>c.<>9__49_0 = new Func<Expression, bool>(<>c.<>9, this.<IsCnf>b__49_0))))
                {
                    return false;
                }
            }
            return true;
        }

        [Extension]
        internal static bool IsCommutative(BinaryExpression expr)
        {
            if (expr.Method != null)
            {
                return false;
            }
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return TypsFw.IsNumeric(expr.Type);

                case ExpressionType.And:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Or:
                    return IsLogical(expr);

                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.OrElse:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return false;

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return true;
            }
            throw new Exception();
        }

        [Extension]
        internal static bool IsCommutative(Expression expr)
        {
            BinaryExpression expression = expr as BinaryExpression;
            return ((expression != null) && IsCommutative(expression));
        }

        [Extension]
        internal static bool IsIdentityFunction(LambdaExpression lambdaExpression)
        {
            return ((lambdaExpression.Parameters.Count == 1) && (lambdaExpression.Body == lambdaExpression.Parameters[0]));
        }

        [Extension]
        internal static bool IsLogical(BinaryExpression binary)
        {
            if (binary.Method != null)
            {
                return false;
            }
            if (binary.Type != typeof(bool))
            {
                return (binary.Type == typeof(bool?));
            }
            return true;
        }

        [Extension]
        internal static bool IsLogical(Expression expr)
        {
            BinaryExpression binary = expr as BinaryExpression;
            return ((binary != null) && IsLogical(binary));
        }

        [Extension]
        internal static bool IsLogicalAnd(Expression expr)
        {
            return (IsAnd(expr.NodeType) && IsLogical(expr));
        }

        [Extension]
        internal static bool IsLogicalOr(Expression expr)
        {
            return (IsOr(expr.NodeType) && IsLogical(expr));
        }

        [Extension]
        internal static bool IsOr(ExpressionType type)
        {
            if (type != ExpressionType.OrElse)
            {
                return (type == ExpressionType.Or);
            }
            return true;
        }

        [Extension]
        public static Expression Replace(Expression expression, Func<Expression, Expression> evaluator)
        {
            Argument.NotNull<Expression>(expression, "expression");
            Argument.NotNull<Func<Expression, Expression>>(evaluator, "evaluator");
            return new ExpressionReplacer().Replace(expression, evaluator);
        }

        [Extension]
        public static Expression Replace(Expression expression, Func<Expression, Func<Expression, Expression>, Expression> evaluator)
        {
            Argument.NotNull<Expression>(expression, "expression");
            Argument.NotNull<Func<Expression, Func<Expression, Expression>, Expression>>(evaluator, "evaluator");
            return new ExpressionReplacer().Replace(expression, evaluator);
        }

        [Extension]
        public static Expression Replace(Expression expression, Func<Expression, bool> predicate, Func<Expression, Expression> evaluator)
        {
            <>c__DisplayClass52_0 class_;
            Argument.NotNull<Func<Expression, bool>>(predicate, "predicate");
            Argument.NotNull<Func<Expression, Expression>>(evaluator, "evaluator");
            return Replace(expression, new Func<Expression, Expression>(class_, this.<Replace>b__0));
        }

        [Extension]
        public static Expression Replace(Expression expression, Func<Expression, bool> predicate, Expression newExpression)
        {
            <>c__DisplayClass53_0 class_;
            Argument.NotNull<Expression>(expression, "expression");
            return Replace(expression, new Func<Expression, Expression>(class_, this.<Replace>b__0));
        }

        [Extension]
        public static Expression Replace(Expression expression, Expression oldValue, Expression newValue)
        {
            return Replace(expression, oldValue, newValue, ExpressionEqualityComparison.Reference);
        }

        [Extension]
        public static Expression Replace(Expression expression, Expression oldValue, Expression newValue, ExpressionEqualityComparison comparison)
        {
            return Replace(expression, oldValue, newValue, GetComparer(comparison));
        }

        [Extension]
        public static Expression Replace(Expression expression, Expression oldValue, Expression newValue, IEqualityComparer<Expression> comparer)
        {
            <>c__DisplayClass54_0 class_;
            Argument.NotNull<Expression>(expression, "expression");
            Argument.NotNull<Expression>(oldValue, "oldValue");
            Argument.NotNull<Expression>(newValue, "newValue");
            Argument.NotNull<IEqualityComparer<Expression>>(comparer, "comparer");
            return Replace(expression, new Func<Expression, bool>(class_, this.<Replace>b__0), newValue);
        }

        [Extension]
        internal static Expression SafeAndAlso(Expression left, Expression right)
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }
            return Expression.AndAlso(left, right);
        }

        [Extension]
        internal static Expression<Func<T, bool>> SafeAndAlso<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }
            ParameterExpression[] parameters = Enumerable.ToArray<ParameterExpression>(left.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, Lambda.ExtractBody(right, parameters)), parameters);
        }

        [Extension]
        internal static LambdaExpression SafeAndAlso(LambdaExpression left, LambdaExpression right)
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }
            ParameterExpression[] parameters = Enumerable.ToArray<ParameterExpression>(left.Parameters);
            return Expression.Lambda(Expression.AndAlso(left.Body, Lambda.ExtractBody(right, parameters)), parameters);
        }

        [Extension]
        internal static Expression SafeOrElse(Expression left, Expression right)
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }
            return Expression.OrElse(left, right);
        }

        [Extension]
        internal static Expression StripConversion(Expression expr)
        {
            while (expr.NodeType == ExpressionType.Convert)
            {
                expr = ((UnaryExpression) expr).Operand;
            }
            return expr;
        }

        [Extension]
        internal static Expression ToCanonicalForm(Expression expression)
        {
            return new CanonicalExpressionRewriter().Rewrite(expression);
        }

        [Extension]
        internal static Expression<Func<T, TResult>> ToCanonicalForm<T, TResult>(Expression<Func<T, TResult>> lambda)
        {
            return ToCanonicalForm<T, TResult>(lambda);
        }

        [Extension]
        internal static Expression<Func<T1, T2, TResult>> ToCanonicalForm<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> lambda)
        {
            return ToCanonicalForm<T1, T2, TResult>(lambda);
        }

        [Extension]
        internal static LambdaExpression ToCanonicalForm(LambdaExpression lambda)
        {
            Expression body = ToCanonicalForm(lambda.Body);
            if (body == lambda.Body)
            {
                return lambda;
            }
            return Expression.Lambda(lambda.Type, body, lambda.Parameters);
        }

        [Extension]
        internal static Expression ToCnf(Expression expr)
        {
            ParameterExpression[] parameters = new ParameterExpression[] { Expression.Parameter(typeof(int), "p") };
            return ToCnf(Expression.Lambda(expr, parameters)).Body;
        }

        [Extension]
        internal static LambdaExpression ToCnf(LambdaExpression expr)
        {
            return new ExpressionNormalizer().Normalize(expr);
        }

        [Extension]
        internal static Constant<T> ToConst<T>(Expression expr)
        {
            return new Constant<T>(expr);
        }

        [Extension]
        internal static XElement ToXml(Expression expression)
        {
            return new ExpressionToXml().ToXml(expression);
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly Xtnz.<>c <>9 = new Xtnz.<>c();
            public static Func<Expression, bool> <>9__17_0;
            public static Func<Expression, bool> <>9__49_0;

            internal bool <Expand>b__17_0(Expression x)
            {
                return true;
            }

            internal bool <IsCnf>b__49_0(Expression x)
            {
                if (x.NodeType != ExpressionType.AndAlso)
                {
                    return (x.NodeType == ExpressionType.And);
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <Expand>d__15 : IEnumerable<Expression>, IEnumerable, IEnumerator<Expression>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private Expression <>2__current;
            private int <>l__initialThreadId;
            private Expression expression;
            public Expression <>3__expression;
            private ExpressionStack <expressionStack>5__1;
            private Func<Expression, bool> goDeeper;
            public Func<Expression, bool> <>3__goDeeper;
            private Expression <expression2>5__2;

            [DebuggerHidden]
            public <Expand>d__15(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.get_CurrentManagedThreadId();
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    this.<expressionStack>5__1 = new ExpressionStack();
                    this.<expressionStack>5__1.Push(this.expression);
                    while (this.<expressionStack>5__1.Count > 0)
                    {
                        this.<expression2>5__2 = this.<expressionStack>5__1.Pop();
                        this.<>2__current = this.<expression2>5__2;
                        this.<>1__state = 1;
                        return true;
                    Label_005B:
                        this.<>1__state = -1;
                        if (this.goDeeper.Invoke(this.<expression2>5__2))
                        {
                            this.<expressionStack>5__1.PushContent(this.<expression2>5__2);
                        }
                        this.<expression2>5__2 = null;
                    }
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_005B;
            }

            [DebuggerHidden]
            IEnumerator<Expression> IEnumerable<Expression>.GetEnumerator()
            {
                Xtnz.<Expand>d__15 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.get_CurrentManagedThreadId()))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new Xtnz.<Expand>d__15(0);
                }
                d__.expression = this.<>3__expression;
                d__.goDeeper = this.<>3__goDeeper;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            Expression IEnumerator<Expression>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ExpandIterator>d__16 : IEnumerable<Expression>, IEnumerable, IEnumerator<Expression>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private Expression <>2__current;
            private int <>l__initialThreadId;
            private Expression expression;
            public Expression <>3__expression;
            private ExpressionStack <expressionStack>5__1;

            [DebuggerHidden]
            public <ExpandIterator>d__16(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.get_CurrentManagedThreadId();
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    this.<expressionStack>5__1 = new ExpressionStack();
                    this.<expressionStack>5__1.PushContent(this.expression);
                    while (this.<expressionStack>5__1.Count > 0)
                    {
                        this.<>2__current = this.<expressionStack>5__1.Pop();
                        this.<>1__state = 1;
                        return true;
                    Label_004F:
                        this.<>1__state = -1;
                    }
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_004F;
            }

            [DebuggerHidden]
            IEnumerator<Expression> IEnumerable<Expression>.GetEnumerator()
            {
                Xtnz.<ExpandIterator>d__16 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.get_CurrentManagedThreadId()))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new Xtnz.<ExpandIterator>d__16(0);
                }
                d__.expression = this.<>3__expression;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            Expression IEnumerator<Expression>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FindPropertyDependencies>d__46 : IEnumerable<PropertyInfo[]>, IEnumerable, IEnumerator<PropertyInfo[]>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private PropertyInfo[] <>2__current;
            private int <>l__initialThreadId;
            private LambdaExpression expression;
            public LambdaExpression <>3__expression;
            private ExpressionStack <expressionStack>5__1;
            private Stack<PropertyInfo> <stack>5__2;

            [DebuggerHidden]
            public <FindPropertyDependencies>d__46(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.get_CurrentManagedThreadId();
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    this.<expressionStack>5__1 = new ExpressionStack();
                    this.<expressionStack>5__1.Push(this.expression);
                    this.<stack>5__2 = new Stack<PropertyInfo>();
                    while (this.<expressionStack>5__1.Count > 0)
                    {
                        Expression exp = this.<expressionStack>5__1.Pop();
                        this.<expressionStack>5__1.PushContent(exp);
                        MemberExpression expression = exp as MemberExpression;
                        if (expression != null)
                        {
                            this.<stack>5__2.Clear();
                            do
                            {
                                PropertyInfo member = expression.Member as PropertyInfo;
                                if (member == null)
                                {
                                    break;
                                }
                                this.<stack>5__2.Push(member);
                                if (expression.Expression == Enumerable.Single<ParameterExpression>(this.expression.Parameters))
                                {
                                    goto Label_00BD;
                                }
                                expression = expression.Expression as MemberExpression;
                            }
                            while (expression != null);
                        }
                        continue;
                    Label_00BD:
                        this.<>2__current = this.<stack>5__2.ToArray();
                        this.<>1__state = 1;
                        return true;
                    Label_00D7:
                        this.<>1__state = -1;
                    }
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_00D7;
            }

            [DebuggerHidden]
            IEnumerator<PropertyInfo[]> IEnumerable<PropertyInfo[]>.GetEnumerator()
            {
                Xtnz.<FindPropertyDependencies>d__46 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.get_CurrentManagedThreadId()))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new Xtnz.<FindPropertyDependencies>d__46(0);
                }
                d__.expression = this.<>3__expression;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo[]>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            PropertyInfo[] IEnumerator<PropertyInfo[]>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }

        private class ExpressionNormalizer : ExpressionRewriter
        {
            private BinaryExpression MakeAnd(bool shortCircuit, Expression left, Expression right)
            {
                if (shortCircuit)
                {
                    return Expression.AndAlso(left, right);
                }
                return Expression.And(left, right);
            }

            private Expression MakeNormalizedNot(Expression op)
            {
                Expression expression = this.NormalizeNot(op);
                if (expression == null)
                {
                    return expression;
                }
                return Expression.Not(op);
            }

            private BinaryExpression MakeOr(bool shortCircuit, Expression left, Expression right)
            {
                if (shortCircuit)
                {
                    return Expression.OrElse(left, right);
                }
                return Expression.Or(left, right);
            }

            public LambdaExpression Normalize(LambdaExpression expr)
            {
                return Expression.Lambda(this.Rewrite(expr.Body), Enumerable.ToArray<ParameterExpression>(expr.Parameters));
            }

            private Expression NormalizeNot(Expression op)
            {
                if (!Xtnz.IsLogicalAnd(op))
                {
                    return null;
                }
                bool shortCircuit = op.NodeType == ExpressionType.AndAlso;
                BinaryExpression expression = op as BinaryExpression;
                return this.MakeOr(shortCircuit, this.MakeNormalizedNot(expression.Left), this.MakeNormalizedNot(expression.Right));
            }

            private Expression NormalizeOr(bool shortOr, Expression left, Expression right)
            {
                bool flag;
                if (this.NormalizeOr(shortOr, ref left, ref right, out flag))
                {
                    return this.MakeAnd(flag, left, right);
                }
                return this.MakeOr(shortOr, left, right);
            }

            private bool NormalizeOr(bool shortOr, ref Expression left, ref Expression right, out bool shortAnd)
            {
                if (Xtnz.IsLogicalAnd(left))
                {
                    BinaryExpression expression = left as BinaryExpression;
                    shortAnd = expression.NodeType == ExpressionType.AndAlso;
                    left = this.NormalizeOr(shortOr, expression.Left, right);
                    right = this.NormalizeOr(shortOr, expression.Right, right);
                    return true;
                }
                if (Xtnz.IsLogicalAnd(right))
                {
                    BinaryExpression expression2 = right as BinaryExpression;
                    shortAnd = expression2.NodeType == ExpressionType.AndAlso;
                    right = this.NormalizeOr(shortOr, left, expression2.Right);
                    left = this.NormalizeOr(shortOr, left, expression2.Left);
                    return true;
                }
                shortAnd = false;
                return false;
            }

            protected override Expression VisitBinary(BinaryExpression b)
            {
                bool flag;
                if (!Xtnz.IsLogicalOr(b))
                {
                    return base.VisitBinary(b);
                }
                Expression left = this.Visit(b.Left);
                Expression right = this.Visit(b.Right);
                this.NormalizeOr(b.NodeType == ExpressionType.OrElse, ref left, ref right, out flag);
                if ((left == b.Left) && (right == b.Right))
                {
                    return b;
                }
                return Expression.And(left, right);
            }

            protected override Expression VisitUnary(UnaryExpression u)
            {
                if ((u.NodeType != ExpressionType.Not) || (u.Method != null))
                {
                    return base.VisitUnary(u);
                }
                Expression op = this.Visit(u.Operand);
                Expression expression2 = this.NormalizeNot(op);
                if (expression2 != null)
                {
                    return expression2;
                }
                if (op == u.Operand)
                {
                    return u;
                }
                return Expression.Not(op);
            }
        }

        private sealed class ExpressionReplacer : ExpressionRewriter
        {
            private Func<Expression, Expression> _evaluator;

            public Expression Replace(Expression expression, Func<Expression, Expression> evaluator)
            {
                this._evaluator = evaluator;
                return this.Rewrite(expression);
            }

            public Expression Replace(Expression expression, Func<Expression, Func<Expression, Expression>, Expression> evaluator)
            {
                <>c__DisplayClass1_0 class_;
                return this.Replace(expression, new Func<Expression, Expression>(class_, this.<Replace>b__0));
            }

            protected override Expression Visit(Expression expr)
            {
                Expression expression = this._evaluator.Invoke(expr);
                if (expression != Xtnz.Nothing)
                {
                    return expression;
                }
                return base.Visit(expr);
            }
        }

        internal class ExpressionToXml : ExpressionVisitor<XElement>
        {
            protected override XElement DefaultVisit(Expression exp)
            {
                return new XElement(exp.NodeType.ToString(), Enumerable.Select<Expression, XElement>(Xtnz.Expand(exp, false), new Func<Expression, XElement>(this, this.<DefaultVisit>b__11_0)));
            }

            private XAttribute ToAttribute(MemberInfo member)
            {
                return this.ToAttribute("Member", member);
            }

            private XAttribute ToAttribute(string name, MemberInfo member)
            {
                return new XAttribute(name, this.ToString(member));
            }

            private string ToString(MemberInfo member)
            {
                return string.Format("{0}.{1}", member.DeclaringType.Name, member.Name);
            }

            private IEnumerable<XElement> ToXml(IEnumerable<ElementInit> bindings)
            {
                return Enumerable.Select<ElementInit, XElement>(bindings, new Func<ElementInit, XElement>(this, this.<ToXml>b__7_0));
            }

            private IEnumerable<XElement> ToXml(IEnumerable<MemberBinding> bindings)
            {
                return Enumerable.Select<MemberBinding, XElement>(bindings, new Func<MemberBinding, XElement>(this, this.<ToXml>b__5_0));
            }

            private IEnumerable<XElement> ToXml<TExpression>(IEnumerable<TExpression> expressions) where TExpression: Expression
            {
                return Enumerable.Select<TExpression, XElement>(expressions, new Func<TExpression, XElement>(this, this.<ToXml>b__2_0<TExpression>));
            }

            internal XElement ToXml(Expression expression)
            {
                return this.Visit(expression);
            }

            private XElement ToXml(string containerName, IEnumerable<ElementInit> bindings)
            {
                return new XElement(containerName, this.ToXml(bindings));
            }

            private XElement ToXml(string containerName, IEnumerable<MemberBinding> bindings)
            {
                return new XElement(containerName, this.ToXml(bindings));
            }

            private XElement ToXml<TExpression>(string containerName, IEnumerable<TExpression> expressions) where TExpression: Expression
            {
                return new XElement(containerName, this.ToXml<TExpression>(expressions));
            }

            private XElement ToXml(string containerName, Expression expression)
            {
                return new XElement(containerName, this.ToXml(expression));
            }

            private XElement VisitAssignment(MemberAssignment binding)
            {
                return new XElement("Assignment", new object[] { this.ToAttribute(binding.Member), this.ToXml(binding.Expression) });
            }

            private XElement VisitBinding(MemberBinding binding)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        return this.VisitAssignment((MemberAssignment) binding);

                    case MemberBindingType.MemberBinding:
                        return this.VisitMemberBinding((MemberMemberBinding) binding);

                    case MemberBindingType.ListBinding:
                        return this.VisitListBinding((MemberListBinding) binding);
                }
                return null;
            }

            protected override XElement VisitConditional(ConditionalExpression exp)
            {
                return new XElement("Conditional", new object[] { this.ToXml("Test", exp.Test), this.ToXml("IfTrue", exp.IfTrue), this.ToXml("IfFalse", exp.IfFalse) });
            }

            protected override XElement VisitConstant(ConstantExpression exp)
            {
                return new XElement("Constant", Convert.ToString(exp.Value));
            }

            private XElement VisitElementInit(ElementInit init)
            {
                return new XElement("ElementInit", new object[] { this.ToAttribute("AddMethod", init.AddMethod), this.ToXml<Expression>(init.Arguments) });
            }

            protected override XElement VisitInvocation(InvocationExpression exp)
            {
                return new XElement("Invocation", new object[] { this.ToXml<Expression>("Arguments", exp.Arguments), this.ToXml("Expression", exp.Expression) });
            }

            protected override XElement VisitLambda(LambdaExpression exp)
            {
                return new XElement("Lambda", new object[] { this.ToXml<ParameterExpression>("Parameters", exp.Parameters), this.ToXml("Body", exp.Body) });
            }

            private XElement VisitListBinding(MemberListBinding binding)
            {
                return new XElement("ListBinding", new object[] { this.ToAttribute(binding.Member), this.ToXml(binding.Initializers) });
            }

            protected override XElement VisitListInit(ListInitExpression exp)
            {
                return new XElement("ListInit", new object[] { this.ToXml(exp.NewExpression), this.ToXml("Initializers", exp.Initializers) });
            }

            protected override XElement VisitMemberAccess(MemberExpression exp)
            {
                return new XElement("MemberAccess", new object[] { this.ToAttribute(exp.Member), this.ToXml(exp.Expression) });
            }

            private XElement VisitMemberBinding(MemberMemberBinding binding)
            {
                return new XElement("MemberBinding", new object[] { this.ToAttribute(binding.Member), this.ToXml(binding.Bindings) });
            }

            protected override XElement VisitMemberInit(MemberInitExpression exp)
            {
                return new XElement("MemberInit", new object[] { this.ToXml(exp.NewExpression), this.ToXml("Bindings", exp.Bindings) });
            }

            protected override XElement VisitMethodCall(MethodCallExpression exp)
            {
                XElement element = new XElement("MethodCall", this.ToAttribute("Method", exp.Method));
                if (exp.Object != null)
                {
                    element.Add(this.ToXml("Object", exp.Object));
                }
                element.Add(this.ToXml<Expression>("Arguments", exp.Arguments));
                return element;
            }

            protected override XElement VisitNew(NewExpression nex)
            {
                object[] content = new object[] { new XAttribute("Type", nex.Type), this.ToXml<Expression>("Arguments", nex.Arguments) };
                XElement element = new XElement("New", content);
                if (nex.Members != null)
                {
                    element.Add(new XElement("Members", Enumerable.Select<MemberInfo, XElement>(nex.Members, new Func<MemberInfo, XElement>(this, this.<VisitNew>b__26_0))));
                }
                return element;
            }

            protected override XElement VisitNewArray(NewArrayExpression exp)
            {
                return new XElement("NewArray", new object[] { new XAttribute("Type", exp.Type), this.ToXml<Expression>(exp.Expressions) });
            }

            protected override XElement VisitParameter(ParameterExpression exp)
            {
                return new XElement("Parameter", new object[] { new XAttribute("Type", exp.Type), new XAttribute("Name", exp.Name) });
            }

            protected override XElement VisitTypeIs(TypeBinaryExpression exp)
            {
                return new XElement(exp.NodeType.ToString(), new object[] { new XAttribute("TypeOperand", exp.TypeOperand), this.ToXml(exp.Expression) });
            }
        }

        private class NullEliminator : ExpressionRewriter
        {
            private Expression DefaultIfEmpty(Expression source)
            {
                Type[] typeArguments = new Type[] { TypsFw.GetSequenceElementType(source.Type) };
                Expression[] arguments = new Expression[] { source };
                return Expression.Call(typeof(Enumerable), "DefaultIfEmpty", typeArguments, arguments);
            }

            private Expression EliminateEmptyAggregationValueAccess(MemberExpression expr)
            {
                throw new Exception();
            }

            private Expression EliminateNonNullableLinqAggregators(MethodCallExpression expr)
            {
                MethodInfo method = expr.Method;
                if (method.DeclaringType == typeof(Enumerable))
                {
                    Expression[] arguments = Enumerable.ToArray<Expression>(expr.Arguments);
                    if ((method.Name == "Aggregate") && (expr.Arguments.Count == 2))
                    {
                        arguments[0] = this.DefaultIfEmpty(arguments[0]);
                        return Expression.Call(method, arguments);
                    }
                    if ((((method.Name == "Average") || (method.Name == "Max")) || (method.Name == "Min")) && !TypsFw.IsOfGenericType(method.ReturnType, typeof(Nullable<>)))
                    {
                        if (arguments.Length == 1)
                        {
                            arguments[0] = this.DefaultIfEmpty(arguments[0]);
                            return Expression.Call(method, arguments);
                        }
                        if (arguments.Length == 2)
                        {
                            ParameterExpression expression = Expression.Parameter(TypsFw.GetSequenceElementType(arguments[0].Type), "x");
                            Expression[] expressionArray1 = new Expression[] { expression };
                            Type[] typeArguments = new Type[] { expr.Type };
                            ParameterExpression[] parameters = new ParameterExpression[] { expression };
                            arguments[1] = Expression.Lambda(Expression.Convert(Expression.Invoke(arguments[1], expressionArray1), typeof(Nullable<>).MakeGenericType(typeArguments)), parameters);
                            Type[] typeArray2 = new Type[] { expression.Type };
                            return Expression.Coalesce(Expression.Call(method.DeclaringType, method.Name, typeArray2, arguments), Expression.Constant(TypsFw.GetDefaultValue(expr.Type)));
                        }
                    }
                    else if (Enumerable.Any<MemberInfo>(method.DeclaringType.GetMember(method.Name + "OrDefault")))
                    {
                        return Expression.Call(method.DeclaringType, method.Name + "OrDefault", method.GetGenericArguments(), arguments);
                    }
                }
                return null;
            }

            private MethodCallExpression EliminateNullEnumerableArgs(MethodCallExpression expr)
            {
                ParameterInfo[] parameters = expr.Method.GetParameters();
                List<Expression> arguments = new List<Expression>();
                bool flag = false;
                for (int i = 0; i < expr.Arguments.Count; i++)
                {
                    ParameterInfo info = parameters[i];
                    Expression left = expr.Arguments[i];
                    if (TypsFw.IsOfGenericType(info.ParameterType, typeof(IEnumerable<>)))
                    {
                        Type type = Enumerable.Single<Type>(info.ParameterType.GetGenericArguments());
                        Type[] typeArguments = new Type[] { type };
                        MethodInfo method = typeof(Enumerable).GetMethod("Empty", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeArguments);
                        left = Expression.Coalesce(left, Expression.Call(method, new Expression[0]));
                        flag = true;
                    }
                    arguments.Add(left);
                }
                if (!flag)
                {
                    return null;
                }
                return Expression.Call(expr.Object, expr.Method, arguments);
            }

            private Expression EliminateNullInstance(Expression expr, Expression instance)
            {
                if ((instance != null) && !instance.Type.IsValueType)
                {
                    <>c__DisplayClass5_0 class_;
                    return this.EvaluateOnce(instance, new Func<Expression, Expression>(class_, this.<EliminateNullInstance>b__0));
                }
                return expr;
            }

            private Expression EvaluateOnce(Expression expr, Func<Expression, Expression> evaluate)
            {
                if ((expr.NodeType == ExpressionType.Parameter) || (expr.NodeType == ExpressionType.Constant))
                {
                    return evaluate.Invoke(expr);
                }
                ParameterExpression left = Expression.Variable(expr.Type);
                BinaryExpression expression2 = Expression.Assign(left, expr);
                ParameterExpression[] variables = new ParameterExpression[] { left };
                Expression[] expressions = new Expression[] { expression2, evaluate.Invoke(left) };
                return Expression.Block(variables, expressions);
            }

            private Expression VisitConversion(MethodInfo method, Expression operand)
            {
                <>c__DisplayClass7_0 class_;
                if (((method == null) || (method.Name != "op_Explicit")) || !method.IsSpecialName)
                {
                    return null;
                }
                Type returnType = method.ReturnType;
                if (!returnType.IsValueType || TypsFw.IsOfGenericType(returnType, typeof(Nullable<>)))
                {
                    return null;
                }
                ParameterInfo[] parameters = method.GetParameters();
                if ((parameters.Length > 1) || (Enumerable.Single<ParameterInfo>(parameters).ParameterType != operand.Type))
                {
                    return null;
                }
                BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
                if (!method.IsPublic)
                {
                    flags |= BindingFlags.NonPublic;
                }
                Type[] typeArguments = new Type[] { returnType };
                Type nullableType = typeof(Nullable<>).MakeGenericType(typeArguments);
                MethodInfo info = Enumerable.FirstOrDefault<MethodInfo>(Enumerable.OfType<MethodInfo>(method.DeclaringType.GetMember(method.Name)), new Func<MethodInfo, bool>(class_, this.<VisitConversion>b__0));
                if (info == null)
                {
                    return null;
                }
                DefaultExpression right = Expression.Default(returnType);
                Expression ifFalse = Expression.Coalesce(Expression.Convert(operand, nullableType, info), right);
                if ((operand.Type != typeof(XAttribute)) && (operand.Type != typeof(XElement)))
                {
                    return ifFalse;
                }
                return Expression.Condition(Expression.OrElse(Expression.Equal(operand, Expression.Constant(null, operand.Type)), Expression.Equal(Expression.Property(operand, "Value"), Expression.Constant(string.Empty))), right, ifFalse);
            }

            protected override Expression VisitMemberAccess(MemberExpression m)
            {
                Expression expression = this.EliminateEmptyAggregationValueAccess(m);
                if (expression != null)
                {
                    return expression;
                }
                m = (MemberExpression) base.VisitMemberAccess(m);
                return (this.EliminateNullInstance(m, m.Expression) ?? m);
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                if ((m.Object == null) && (m.Arguments.Count == 1))
                {
                    Expression expression = this.VisitConversion(m.Method, Enumerable.Single<Expression>(m.Arguments));
                    if (expression != null)
                    {
                        return expression;
                    }
                }
                m = (MethodCallExpression) base.VisitMethodCall(m);
                m = this.EliminateNullEnumerableArgs(m) ?? m;
                if (m.Object == null)
                {
                    return (this.EliminateNonNullableLinqAggregators(m) ?? m);
                }
                return (this.EliminateNullInstance(m, m.Object) ?? m);
            }

            protected override Expression VisitUnary(UnaryExpression u)
            {
                if (u.NodeType == ExpressionType.Quote)
                {
                    return u;
                }
                if (u.NodeType == ExpressionType.Convert)
                {
                    Expression expression = this.VisitConversion(u.Method, u.Operand);
                    if (expression != null)
                    {
                        return expression;
                    }
                }
                return base.VisitUnary(u);
            }
        }
    }
}

