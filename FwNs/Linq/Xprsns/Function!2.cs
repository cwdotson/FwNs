namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;

    public class Function<T, TResult> : Lambda<Func<T, TResult>>
    {
        public Function(Expression<Func<T, TResult>> lambda) : base(Xtnz.ToCanonicalForm<T, TResult>(lambda))
        {
        }

        public Function(LambdaExpression lambda) : this((Expression<Func<T, TResult>>) lambda)
        {
        }

        public Function(Expression body, ParameterExpression parameter) : base(Lambda.Function<T, TResult>(Xtnz.ToCanonicalForm(body), parameter))
        {
        }

        public static Function<T, TResult> CreateDelegateBased(Func<T, TResult> @delegate)
        {
            <>c__DisplayClass9_0<T, TResult> class_;
            ParameterExpression expression;
            Expression[] arguments = new Expression[] { expression = Expression.Parameter(typeof(T), "x") };
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return new Function<T, TResult>(Expression.Lambda<Func<T, TResult>>(Expression.Invoke(Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass9_0<T, TResult>)), fieldof(<>c__DisplayClass9_0<T, TResult>.delegate, <>c__DisplayClass9_0<T, TResult>)), arguments), parameters)) { _compiled = @delegate };
        }

        internal Function<TResult, T> Invert()
        {
            LambdaExpression lambda = Lambda.Invert(base.Expression);
            if (lambda == null)
            {
                return null;
            }
            return new Function<TResult, T>(lambda);
        }

        public TResult Invoke(T arg)
        {
            return base.AsDelegate().Invoke(arg);
        }

        internal bool IsIdentityFunction()
        {
            return this.IsIdentityFunction(false);
        }

        public virtual bool IsIdentityFunction(bool allowConversion)
        {
            return Xtnz.DoesReturn(base.Body, this.Parameter, allowConversion);
        }

        internal Function<T2, TResult> Substitute<T2, TResult2>(Function<T2, TResult2> function) where TResult2: T
        {
            return new SubstitutedFunction<T, TResult, T2, TResult2>((Function<T, TResult>) this, function);
        }

        public ParameterExpression Parameter
        {
            get
            {
                return base.Expression.Parameters[0];
            }
        }

        public bool IsTuple
        {
            get
            {
                NewExpression body = base.Body as NewExpression;
                return (((body != null) && (body.Members != null)) && (body.Arguments[0] == this.Parameter));
            }
        }
    }
}

