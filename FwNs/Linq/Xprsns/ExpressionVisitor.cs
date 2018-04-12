namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;

    internal class ExpressionVisitor
    {
        protected ExpressionVisitor()
        {
        }

        protected virtual void DefaultVisit(Expression exp)
        {
            throw new NotSupportedException(Xtnz.InvariantFormat(EMExpressions.Visitor_ExpressionTypeNotSupported, exp.GetType(), base.GetType()));
        }

        protected virtual void Visit(Expression exp)
        {
            if (exp == null)
            {
                throw new ArgumentNullException("exp");
            }
            switch (exp.NodeType)
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
                    this.VisitBinary(exp as BinaryExpression);
                    return;

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    this.VisitUnary(exp as UnaryExpression);
                    return;

                case ExpressionType.ArrayIndex:
                    if (!(exp is BinaryExpression))
                    {
                        this.VisitMethodCall(exp as MethodCallExpression);
                        return;
                    }
                    this.VisitBinary(exp as BinaryExpression);
                    return;

                case ExpressionType.Call:
                    this.VisitMethodCall(exp as MethodCallExpression);
                    return;

                case ExpressionType.Conditional:
                    this.VisitConditional(exp as ConditionalExpression);
                    return;

                case ExpressionType.Constant:
                    this.VisitConstant(exp as ConstantExpression);
                    return;

                case ExpressionType.Invoke:
                    this.VisitInvocation(exp as InvocationExpression);
                    return;

                case ExpressionType.Lambda:
                    this.VisitLambda(exp as LambdaExpression);
                    return;

                case ExpressionType.ListInit:
                    this.VisitListInit(exp as ListInitExpression);
                    return;

                case ExpressionType.MemberAccess:
                    this.VisitMemberAccess(exp as MemberExpression);
                    return;

                case ExpressionType.MemberInit:
                    this.VisitMemberInit(exp as MemberInitExpression);
                    return;

                case ExpressionType.New:
                    this.VisitNew(exp as NewExpression);
                    return;

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    this.VisitNewArray(exp as NewArrayExpression);
                    return;

                case ExpressionType.Parameter:
                    this.VisitParameter(exp as ParameterExpression);
                    return;

                case ExpressionType.TypeIs:
                    this.VisitTypeIs(exp as TypeBinaryExpression);
                    return;
            }
            this.DefaultVisit(exp);
        }

        protected virtual void VisitBinary(BinaryExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitConditional(ConditionalExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitConstant(ConstantExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitInvocation(InvocationExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitLambda(LambdaExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitListInit(ListInitExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitMemberAccess(MemberExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitMemberInit(MemberInitExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitMethodCall(MethodCallExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitNew(NewExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitNewArray(NewArrayExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitParameter(ParameterExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitTypeIs(TypeBinaryExpression exp)
        {
            this.DefaultVisit(exp);
        }

        protected virtual void VisitUnary(UnaryExpression exp)
        {
            this.DefaultVisit(exp);
        }
    }
}

