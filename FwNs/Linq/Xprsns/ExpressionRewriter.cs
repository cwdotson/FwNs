namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    internal class ExpressionRewriter : ExpressionVisitor<Expression>
    {
        protected bool _stopVisiting;

        protected override Expression DefaultVisit(Expression exp)
        {
            return exp;
        }

        public virtual Expression Rewrite(Expression expression)
        {
            Argument.NotNull<Expression>(expression, "expression");
            this._stopVisiting = false;
            return this.Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            Expression expression3 = (b.Conversion == null) ? null : this.VisitLambda(b.Conversion);
            if (this._stopVisiting)
            {
                return null;
            }
            if (((left == b.Left) && (right == b.Right)) && (expression3 == b.Conversion))
            {
                return b;
            }
            if ((b.NodeType == ExpressionType.Coalesce) && (b.Conversion != null))
            {
                return Expression.Coalesce(left, right, expression3 as LambdaExpression);
            }
            return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment(binding as MemberAssignment);

                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding(binding as MemberMemberBinding);

                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding(binding as MemberListBinding);
            }
            return null;
        }

        protected virtual IList<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            return this.VisitList<MemberBinding>(original, new Func<MemberBinding, MemberBinding>(this, this.VisitBinding));
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            if (this._stopVisiting)
            {
                return null;
            }
            if (((test == c.Test) && (ifTrue == c.IfTrue)) && (ifFalse == c.IfFalse))
            {
                return c;
            }
            return Expression.Condition(test, ifTrue, ifFalse);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            IList<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (this._stopVisiting)
            {
                return null;
            }
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        protected virtual IList<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            return this.VisitList<ElementInit>(original, new Func<ElementInit, ElementInit>(this, this.VisitElementInitializer));
        }

        protected virtual IList<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            return this.VisitList<Expression>(original, new Func<Expression, Expression>(this, this.Visit));
        }

        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            IList<Expression> arguments = this.VisitExpressionList(iv.Arguments);
            Expression expression = this.Visit(iv.Expression);
            if (this._stopVisiting)
            {
                return null;
            }
            if ((arguments == iv.Arguments) && (expression == iv.Expression))
            {
                return iv;
            }
            return Expression.Invoke(expression, arguments);
        }

        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);
            if (this._stopVisiting)
            {
                return null;
            }
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        protected virtual IList<T> VisitList<T>(IList<T> original, Func<T, T> visitor) where T: class
        {
            List<T> list = null;
            int count = original.Count;
            for (int i = 0; i < count; i++)
            {
                T item = visitor.Invoke(original[i]);
                if (this._stopVisiting)
                {
                    return null;
                }
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[i])
                {
                    list = new List<T>(count);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(item);
                }
            }
            IList<T> list2 = list;
            return (list2 ?? original);
        }

        protected override Expression VisitListInit(ListInitExpression init)
        {
            NewExpression newExpression = (NewExpression) this.VisitNew(init.NewExpression);
            IList<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            if (this._stopVisiting)
            {
                return null;
            }
            if ((newExpression == init.NewExpression) && (initializers == init.Initializers))
            {
                return init;
            }
            return Expression.ListInit(newExpression, initializers);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null)
            {
                Expression expression = this.Visit(m.Expression);
                if (this._stopVisiting)
                {
                    return null;
                }
                if (expression != m.Expression)
                {
                    return Expression.MakeMemberAccess(expression, m.Member);
                }
            }
            return m;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression expression = this.Visit(assignment.Expression);
            if (this._stopVisiting)
            {
                return null;
            }
            if (expression != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, expression);
            }
            return assignment;
        }

        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression newExpression = (NewExpression) this.VisitNew(init.NewExpression);
            IList<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if (this._stopVisiting)
            {
                return null;
            }
            if ((newExpression == init.NewExpression) && (bindings == init.Bindings))
            {
                return init;
            }
            return Expression.MemberInit(newExpression, bindings);
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            if (this._stopVisiting)
            {
                return null;
            }
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            if (this._stopVisiting)
            {
                return null;
            }
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression instance = (m.Object == null) ? null : this.Visit(m.Object);
            IList<Expression> arguments = this.VisitExpressionList(m.Arguments);
            if (this._stopVisiting)
            {
                return null;
            }
            if ((instance == m.Object) && (arguments == m.Arguments))
            {
                return m;
            }
            return Expression.Call(instance, m.Method, arguments);
        }

        protected override Expression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> arguments = this.VisitExpressionList(nex.Arguments);
            if (this._stopVisiting)
            {
                return null;
            }
            if (arguments == nex.Arguments)
            {
                return nex;
            }
            if (nex.Members != null)
            {
                return Expression.New(nex.Constructor, arguments, nex.Members);
            }
            return Expression.New(nex.Constructor, arguments);
        }

        protected override Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> initializers = this.VisitExpressionList(na.Expressions);
            if (this._stopVisiting)
            {
                return null;
            }
            if (initializers == na.Expressions)
            {
                return na;
            }
            if (na.NodeType == ExpressionType.NewArrayInit)
            {
                return Expression.NewArrayInit(na.Type.GetElementType(), initializers);
            }
            return Expression.NewArrayBounds(na.Type.GetElementType(), initializers);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expression = this.Visit(b.Expression);
            if (this._stopVisiting)
            {
                return null;
            }
            if (expression != b.Expression)
            {
                return Expression.TypeIs(expression, b.TypeOperand);
            }
            return b;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            if (this._stopVisiting)
            {
                return null;
            }
            if (operand != u.Operand)
            {
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;
        }
    }
}

