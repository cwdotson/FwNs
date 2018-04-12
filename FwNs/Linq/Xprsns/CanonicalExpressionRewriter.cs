namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class CanonicalExpressionRewriter : ExpressionRewriter
    {
        protected override Expression VisitBinary(BinaryExpression b)
        {
            Expression expression = this.VisitVBOperator(b);
            if (expression != null)
            {
                return expression;
            }
            if (!Xtnz.IsAssociative(b))
            {
                return base.VisitBinary(b);
            }
            Expression left = this.Visit(b.Left);
            Expression expr = this.Visit(b.Right);
            if (((expr.NodeType == b.NodeType) && Xtnz.IsAssociative(expr)) && !b.IsLifted)
            {
                BinaryExpression item = expr as BinaryExpression;
                Stack<BinaryExpression> stack = null;
                if (item.Left.NodeType == b.NodeType)
                {
                    stack = new Stack<BinaryExpression>();
                    do
                    {
                        item = item.Left as BinaryExpression;
                        stack.Push(item);
                    }
                    while ((item.Left.NodeType == b.NodeType) && Xtnz.IsAssociative(item.Left));
                }
                BinaryExpression expression5 = Expression.MakeBinary(b.NodeType, Expression.MakeBinary(b.NodeType, left, item.Left), item.Right);
                if (stack != null)
                {
                    while (stack.Count > 0)
                    {
                        item = stack.Pop();
                        expression5 = Expression.MakeBinary(b.NodeType, expression5, item.Right);
                    }
                }
                return expression5;
            }
            if ((left == b.Left) && (expr == b.Right))
            {
                return b;
            }
            return Expression.MakeBinary(b.NodeType, left, expr);
        }

        private Expression VisitVBOperator(BinaryExpression b)
        {
            if (((b.NodeType == ExpressionType.Equal) && (b.Left.NodeType == ExpressionType.Call)) && (b.Right.NodeType == ExpressionType.Constant))
            {
                ConstantExpression right = b.Right as ConstantExpression;
                if ((right.Type != typeof(int)) || (((int) right.Value) != 0))
                {
                    return null;
                }
                MethodCallExpression left = (MethodCallExpression) b.Left;
                Type declaringType = left.Method.DeclaringType;
                if ((((declaringType == null) || !declaringType.Assembly.FullName.StartsWith("Microsoft.VisualBasic")) || ((declaringType.Namespace != "Microsoft.VisualBasic.CompilerServices") || (declaringType.Name != "Operators"))) || ((left.Method.Name != "CompareString") || (left.Arguments.Count != 3)))
                {
                    return null;
                }
                ConstantExpression expression3 = left.Arguments[2] as ConstantExpression;
                if (((expression3 != null) && (expression3.Type == typeof(bool))) && !((bool) expression3.Value))
                {
                    return Expression.Equal(this.Visit(left.Arguments[0]), this.Visit(left.Arguments[1]));
                }
            }
            return null;
        }
    }
}

