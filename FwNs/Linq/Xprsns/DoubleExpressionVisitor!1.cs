namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Linq.Expressions;

    internal class DoubleExpressionVisitor<TResult>
    {
        protected DoubleExpressionVisitor()
        {
        }

        protected virtual TResult DefaultVisit(Expression x, Expression y)
        {
            throw new NotSupportedException(Xtnz.InvariantFormat(EMExpressions.Visitor_ExpressionTypePairNotSupportedByVisitor, x.GetType(), y.GetType(), base.GetType()));
        }

        protected virtual TResult Visit(Expression x, Expression y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            if (x.NodeType == y.NodeType)
            {
                switch (x.NodeType)
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
                    case ExpressionType.RightShift:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        return this.VisitBinary(x as BinaryExpression, y as BinaryExpression);

                    case ExpressionType.ArrayLength:
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                    case ExpressionType.Not:
                    case ExpressionType.Quote:
                    case ExpressionType.TypeAs:
                        return this.VisitUnary(x as UnaryExpression, y as UnaryExpression);

                    case ExpressionType.ArrayIndex:
                        if (!(x is BinaryExpression) || !(y is BinaryExpression))
                        {
                            if ((x is MethodCallExpression) && (y is MethodCallExpression))
                            {
                                return this.VisitMethodCall(x as MethodCallExpression, y as MethodCallExpression);
                            }
                            break;
                        }
                        return this.VisitBinary(x as BinaryExpression, y as BinaryExpression);

                    case ExpressionType.Call:
                        return this.VisitMethodCall(x as MethodCallExpression, y as MethodCallExpression);

                    case ExpressionType.Conditional:
                        return this.VisitConditional(x as ConditionalExpression, y as ConditionalExpression);

                    case ExpressionType.Constant:
                        return this.VisitConstant(x as ConstantExpression, y as ConstantExpression);

                    case ExpressionType.Invoke:
                        return this.VisitInvocation(x as InvocationExpression, y as InvocationExpression);

                    case ExpressionType.Lambda:
                        return this.VisitLambda(x as LambdaExpression, y as LambdaExpression);

                    case ExpressionType.ListInit:
                        return this.VisitListInit(x as ListInitExpression, y as ListInitExpression);

                    case ExpressionType.MemberAccess:
                        return this.VisitMemberAccess(x as MemberExpression, y as MemberExpression);

                    case ExpressionType.MemberInit:
                        return this.VisitMemberInit(x as MemberInitExpression, y as MemberInitExpression);

                    case ExpressionType.New:
                        return this.VisitNew(x as NewExpression, y as NewExpression);

                    case ExpressionType.NewArrayInit:
                    case ExpressionType.NewArrayBounds:
                        return this.VisitNewArray(x as NewArrayExpression, y as NewArrayExpression);

                    case ExpressionType.Parameter:
                        return this.VisitParameter(x as ParameterExpression, y as ParameterExpression);

                    case ExpressionType.TypeIs:
                        return this.VisitTypeIs(x as TypeBinaryExpression, y as TypeBinaryExpression);
                }
            }
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitBinary(BinaryExpression x, BinaryExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitConditional(ConditionalExpression x, ConditionalExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitConstant(ConstantExpression x, ConstantExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitInvocation(InvocationExpression x, InvocationExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitLambda(LambdaExpression x, LambdaExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitListInit(ListInitExpression x, ListInitExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitMemberAccess(MemberExpression x, MemberExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitMemberInit(MemberInitExpression x, MemberInitExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitMethodCall(MethodCallExpression x, MethodCallExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitNew(NewExpression x, NewExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitNewArray(NewArrayExpression x, NewArrayExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitParameter(ParameterExpression x, ParameterExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitTypeIs(TypeBinaryExpression x, TypeBinaryExpression y)
        {
            return this.DefaultVisit(x, y);
        }

        protected virtual TResult VisitUnary(UnaryExpression x, UnaryExpression y)
        {
            return this.DefaultVisit(x, y);
        }
    }
}

