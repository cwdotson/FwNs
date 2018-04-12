namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;

    internal class ExpressionVisitor<TResult>
    {
        protected ExpressionVisitor()
        {
        }

        protected virtual TResult DefaultVisit(Expression exp)
        {
            throw new NotSupportedException(Xtnz.InvariantFormat(EMExpressions.Visitor_ExpressionTypeNotSupported, exp.GetType(), base.GetType()));
        }

        protected virtual TResult Visit(Expression expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException("expr");
            }
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.VisitBinary(expr as BinaryExpression);

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary(expr as UnaryExpression);

                case ExpressionType.ArrayIndex:
                    if (expr is BinaryExpression)
                    {
                        return this.VisitBinary(expr as BinaryExpression);
                    }
                    return this.VisitMethodCall(expr as MethodCallExpression);

                case ExpressionType.Call:
                    return this.VisitMethodCall(expr as MethodCallExpression);

                case ExpressionType.Conditional:
                    return this.VisitConditional(expr as ConditionalExpression);

                case ExpressionType.Constant:
                    return this.VisitConstant(expr as ConstantExpression);

                case ExpressionType.Invoke:
                    return this.VisitInvocation(expr as InvocationExpression);

                case ExpressionType.Lambda:
                    return this.VisitLambda(expr as LambdaExpression);

                case ExpressionType.ListInit:
                    return this.VisitListInit(expr as ListInitExpression);

                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess(expr as MemberExpression);

                case ExpressionType.MemberInit:
                    return this.VisitMemberInit(expr as MemberInitExpression);

                case ExpressionType.New:
                    return this.VisitNew(expr as NewExpression);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray(expr as NewArrayExpression);

                case ExpressionType.Parameter:
                    return this.VisitParameter(expr as ParameterExpression);

                case ExpressionType.TypeIs:
                    return this.VisitTypeIs(expr as TypeBinaryExpression);
            }
            return this.DefaultVisit(expr);
        }

        protected virtual TResult VisitBinary(BinaryExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitConditional(ConditionalExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitConstant(ConstantExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitInvocation(InvocationExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitLambda(LambdaExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitListInit(ListInitExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitMemberAccess(MemberExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitMemberInit(MemberInitExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitMethodCall(MethodCallExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitNew(NewExpression nex)
        {
            return this.DefaultVisit(nex);
        }

        protected virtual TResult VisitNewArray(NewArrayExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitParameter(ParameterExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitTypeIs(TypeBinaryExpression exp)
        {
            return this.DefaultVisit(exp);
        }

        protected virtual TResult VisitUnary(UnaryExpression exp)
        {
            return this.DefaultVisit(exp);
        }
    }
}

