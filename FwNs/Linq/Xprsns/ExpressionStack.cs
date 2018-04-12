namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    internal class ExpressionStack : FwNs.Linq.Xprsns.ExpressionVisitor
    {
        private readonly Stack<Expression> _data = new Stack<Expression>();

        protected override void DefaultVisit(Expression exp)
        {
        }

        public Expression Pop()
        {
            return this._data.Pop();
        }

        public void Push(Expression exp)
        {
            this._data.Push(exp);
        }

        public void PushContent(Expression exp)
        {
            this.Visit(exp);
        }

        public void PushIfNotNull(Expression exp)
        {
            if (exp != null)
            {
                this._data.Push(exp);
            }
        }

        private void PushList(ReadOnlyCollection<Expression> expressions)
        {
            for (int i = expressions.Count - 1; i >= 0; i--)
            {
                this.PushIfNotNull(expressions[i]);
            }
        }

        private void VisitAssignment(MemberAssignment mem)
        {
            this.PushIfNotNull(mem.Expression);
        }

        protected override void VisitBinary(BinaryExpression exp)
        {
            this.PushIfNotNull(exp.Conversion);
            this.PushIfNotNull(exp.Right);
            this.PushIfNotNull(exp.Left);
        }

        private void VisitBinding(MemberBinding mem)
        {
            switch (mem.BindingType)
            {
                case MemberBindingType.Assignment:
                    this.VisitAssignment((MemberAssignment) mem);
                    return;

                case MemberBindingType.MemberBinding:
                    this.VisitMemberBinding((MemberMemberBinding) mem);
                    return;

                case MemberBindingType.ListBinding:
                    this.VisitListBinding((MemberListBinding) mem);
                    return;
            }
        }

        private void VisitBindings(ReadOnlyCollection<MemberBinding> bindings)
        {
            for (int i = bindings.Count - 1; i >= 0; i--)
            {
                this.VisitBinding(bindings[i]);
            }
        }

        protected override void VisitConditional(ConditionalExpression exp)
        {
            this.PushIfNotNull(exp.IfFalse);
            this.PushIfNotNull(exp.IfTrue);
            this.PushIfNotNull(exp.Test);
        }

        private void VisitInitializers(ReadOnlyCollection<ElementInit> initilizers)
        {
            for (int i = initilizers.Count - 1; i >= 0; i--)
            {
                this.PushList(initilizers[i].Arguments);
            }
        }

        protected override void VisitInvocation(InvocationExpression exp)
        {
            this.PushList(exp.Arguments);
            this.PushIfNotNull(exp.Expression);
        }

        protected override void VisitLambda(LambdaExpression exp)
        {
            this.PushIfNotNull(exp.Body);
            for (int i = exp.Parameters.Count - 1; i >= 0; i--)
            {
                this.PushIfNotNull(exp.Parameters[i]);
            }
        }

        private void VisitListBinding(MemberListBinding mem)
        {
            this.VisitInitializers(mem.Initializers);
        }

        protected override void VisitListInit(ListInitExpression exp)
        {
            this.VisitInitializers(exp.Initializers);
            this.PushIfNotNull(exp.NewExpression);
        }

        protected override void VisitMemberAccess(MemberExpression exp)
        {
            this.PushIfNotNull(exp.Expression);
        }

        private void VisitMemberBinding(MemberMemberBinding mem)
        {
            this.VisitBindings(mem.Bindings);
        }

        protected override void VisitMemberInit(MemberInitExpression exp)
        {
            this.VisitBindings(exp.Bindings);
            this.PushIfNotNull(exp.NewExpression);
        }

        protected override void VisitMethodCall(MethodCallExpression exp)
        {
            this.PushList(exp.Arguments);
            this.PushIfNotNull(exp.Object);
        }

        protected override void VisitNew(NewExpression exp)
        {
            this.PushList(exp.Arguments);
        }

        protected override void VisitNewArray(NewArrayExpression exp)
        {
            this.PushList(exp.Expressions);
        }

        protected override void VisitTypeIs(TypeBinaryExpression exp)
        {
            this.PushIfNotNull(exp.Expression);
        }

        protected override void VisitUnary(UnaryExpression exp)
        {
            this.PushIfNotNull(exp.Operand);
        }

        public int Count
        {
            get
            {
                return this._data.Count;
            }
        }
    }
}

