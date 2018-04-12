namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class Constant<T>
    {
        private Expression<Func<T>> _lambda;
        private Func<T> _eval;
        private bool _hasValue;
        private T _value;

        public Constant(System.Linq.Expressions.Expression expression)
        {
            Argument.NotNull<System.Linq.Expressions.Expression>(expression, "expression");
            this.Expression = Xtnz.ToCanonicalForm(expression);
        }

        public Constant(T value) : this(System.Linq.Expressions.Expression.Constant(value))
        {
        }

        public Expression<Func<T>> AsLambda()
        {
            if (this._lambda == null)
            {
                System.Linq.Expressions.Expression expression = this.Expression;
                if (typeof(T) != expression.Type)
                {
                    expression = System.Linq.Expressions.Expression.Convert(expression, typeof(T));
                }
                this._lambda = System.Linq.Expressions.Expression.Lambda<Func<T>>(expression, new ParameterExpression[0]);
            }
            return this._lambda;
        }

        private Func<T> Compile()
        {
            ConstantExpression constant = this.Expression as ConstantExpression;
            if (constant != null)
            {
                <>c__DisplayClass11_0<T> class_;
                return new Func<T>(class_, this.<Compile>b__0);
            }
            MemberExpression expression = this.Expression as MemberExpression;
            if (expression != null)
            {
                constant = expression.Expression as ConstantExpression;
                if (constant != null)
                {
                    switch (expression.Member.MemberType)
                    {
                        case MemberTypes.Field:
                        {
                            <>c__DisplayClass11_1<T> class_1;
                            FieldInfo field = (FieldInfo) expression.Member;
                            return new Func<T>(class_1, this.<Compile>b__1);
                        }
                        case MemberTypes.Method:
                        {
                            <>c__DisplayClass11_2<T> class_2;
                            MethodInfo method = (MethodInfo) expression.Member;
                            return new Func<T>(class_2, this.<Compile>b__2);
                        }
                        case MemberTypes.Property:
                        {
                            <>c__DisplayClass11_3<T> class_3;
                            PropertyInfo property = (PropertyInfo) expression.Member;
                            return new Func<T>(class_3, this.<Compile>b__3);
                        }
                    }
                }
            }
            return this.AsLambda().Compile();
        }

        public T Eval()
        {
            if (this._hasValue)
            {
                return this._value;
            }
            if (this._eval == null)
            {
                this._eval = this.Compile();
                if (Xtnz.StripConversion(this.Expression) is ConstantExpression)
                {
                    this._hasValue = true;
                    this._value = this._eval.Invoke();
                    return this._value;
                }
            }
            return this._eval.Invoke();
        }

        public override string ToString()
        {
            return this.Expression.ToString();
        }

        public System.Linq.Expressions.Expression Expression { get; private set; }
    }
}

