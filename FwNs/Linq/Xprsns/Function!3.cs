namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;

    internal class Function<T1, T2, TResult> : Lambda<Func<T1, T2, TResult>>
    {
        private Function<T2, T1, TResult> _reversed;

        public Function(Expression<Func<T1, T2, TResult>> lambda) : base(Xtnz.ToCanonicalForm<T1, T2, TResult>(lambda))
        {
        }

        public Function(LambdaExpression lambda) : this((Expression<Func<T1, T2, TResult>>) lambda)
        {
        }

        public Function(Expression body, ParameterExpression parameter1, ParameterExpression parameter2) : base(Lambda.Function<T1, T2, TResult>(Xtnz.ToCanonicalForm(body), parameter1, parameter2))
        {
        }

        internal Function<T2, T1, TResult> AsReversed()
        {
            if (this._reversed == null)
            {
                this._reversed = new Reversed<T1, T2, TResult>((Function<T1, T2, TResult>) this);
            }
            return this._reversed;
        }

        public static Function<T1, T2, TResult> CreateDelegateBased(Func<T1, T2, TResult> @delegate)
        {
            <>c__DisplayClass15_0<T1, T2, TResult> class_;
            ParameterExpression expression;
            ParameterExpression expression2;
            Expression[] arguments = new Expression[] { expression = Expression.Parameter(typeof(T1), "x"), expression2 = Expression.Parameter(typeof(T2), "y") };
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
            return new Function<T1, T2, TResult>(Expression.Lambda<Func<T1, T2, TResult>>(Expression.Invoke(Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass15_0<T1, T2, TResult>)), fieldof(<>c__DisplayClass15_0<T1, T2, TResult>.delegate, <>c__DisplayClass15_0<T1, T2, TResult>)), arguments), parameters)) { _compiled = @delegate };
        }

        internal virtual Function<TResult, T1> InvertParameter1()
        {
            LambdaExpression lambdaExpression = Lambda.Invert(base.Expression, 0);
            Assert.IsNullOrIs<Expression<Func<TResult, T1>>>(lambdaExpression);
            if (lambdaExpression == null)
            {
                return null;
            }
            return new Function<TResult, T1>(lambdaExpression as Expression<Func<TResult, T1>>);
        }

        internal virtual Function<TResult, T2> InvertParameter2()
        {
            LambdaExpression expression = Lambda.Invert(base.Expression, 1);
            if (expression == null)
            {
                return null;
            }
            return new Function<TResult, T2>(expression as Expression<Func<TResult, T2>>);
        }

        public TResult Invoke(T1 arg1, T2 arg2)
        {
            return base.AsDelegate().Invoke(arg1, arg2);
        }

        public bool ReturnsParameter1()
        {
            return this.ReturnsParameter1(false);
        }

        public bool ReturnsParameter1(bool allowConversion)
        {
            return Xtnz.DoesReturn(base.Body, this.Parameter1, allowConversion);
        }

        public bool ReturnsParameter2()
        {
            return this.ReturnsParameter2(false);
        }

        public bool ReturnsParameter2(bool allowConversion)
        {
            return Xtnz.DoesReturn(base.Body, this.Parameter2, allowConversion);
        }

        public ParameterExpression Parameter1
        {
            get
            {
                return base.Expression.Parameters[0];
            }
        }

        public ParameterExpression Parameter2
        {
            get
            {
                return base.Expression.Parameters[1];
            }
        }

        internal bool CreatesPair
        {
            get
            {
                NewExpression body = base.Expression.Body as NewExpression;
                if ((body == null) || (body.Members == null))
                {
                    return false;
                }
                return (((body.Arguments[0] == this.Parameter1) && (body.Arguments[1] == this.Parameter2)) || ((body.Arguments[1] == this.Parameter1) && (body.Arguments[0] == this.Parameter2)));
            }
        }

        private class Reversed : Function<T2, T1, TResult>
        {
            public Reversed(Function<T1, T2, TResult> owner) : base(Expression.Lambda<Func<T2, T1, TResult>>(owner.Body, expressionArray1))
            {
                ParameterExpression[] expressionArray1 = new ParameterExpression[] { owner.Parameter2, owner.Parameter1 };
                base._reversed = owner;
            }

            protected override Func<T2, T1, TResult> Compile()
            {
                <>c__DisplayClass3_0<T1, T2, TResult> class_1;
                Func<T1, T2, TResult> compiled = base._reversed.AsDelegate();
                return new Func<T2, T1, TResult>(class_1, this.<Compile>b__0);
            }

            internal override Function<TResult, T2> InvertParameter1()
            {
                return base._reversed.InvertParameter2();
            }

            internal override Function<TResult, T1> InvertParameter2()
            {
                return base._reversed.InvertParameter1();
            }
        }
    }
}

