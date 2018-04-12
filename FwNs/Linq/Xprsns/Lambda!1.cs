namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;

    public class Lambda<TDelegate> : IEquatable<Lambda<TDelegate>> where TDelegate: class
    {
        protected TDelegate _compiled;

        public Lambda(Expression<TDelegate> lambda)
        {
            this.Expression = lambda;
        }

        public TDelegate AsDelegate()
        {
            if (this._compiled == null)
            {
                this._compiled = this.Compile();
            }
            return this._compiled;
        }

        protected virtual TDelegate Compile()
        {
            return ((Expression<TDelegate>) Xtnz.EliminateNulls(this.Expression)).Compile();
        }

        internal bool DeepEquals(Expression<TDelegate> func)
        {
            return this.DeepEquals(func, ExpressionComparisonOptions.None);
        }

        internal bool DeepEquals(Expression<TDelegate> func, ExpressionComparisonOptions options)
        {
            return Xtnz.DeepEquals(this.Expression, func, options);
        }

        public bool Equals(Lambda<TDelegate> func)
        {
            if (func == null)
            {
                return false;
            }
            if (this != func)
            {
                return Xtnz.DeepEquals(this.Expression, func.Expression, ExpressionComparisonOptions.ParametersByIndex);
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return ((this == obj) || ((obj.GetType() == typeof(Lambda<TDelegate>)) && this.Equals((Lambda<TDelegate>) obj)));
        }

        public override int GetHashCode()
        {
            return Xtnz.DeepGetHashCode(this.Expression, ExpressionComparisonOptions.ParametersByIndex);
        }

        public static bool operator ==(Lambda<TDelegate> left, Lambda<TDelegate> right)
        {
            return object.Equals(left, right);
        }

        public static implicit operator TDelegate(Lambda<TDelegate> lambda)
        {
            if (lambda != null)
            {
                return lambda.AsDelegate();
            }
            return default(TDelegate);
        }

        public static bool operator !=(Lambda<TDelegate> left, Lambda<TDelegate> right)
        {
            return !object.Equals(left, right);
        }

        public override string ToString()
        {
            return this.Expression.ToString();
        }

        internal XElement ToXml()
        {
            throw new Exception();
        }

        public string ToXmlFriendlyString()
        {
            return Lambda.ToXmlFriendlyString(this.Expression);
        }

        public Expression<TDelegate> Expression { get; private set; }

        public System.Linq.Expressions.Expression Body
        {
            get
            {
                return this.Expression.Body;
            }
        }
    }
}

