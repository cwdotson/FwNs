namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class ExpressionEqualityComparer : IEqualityComparer<Expression>, IEqualityComparer<LambdaExpression>, IEqualityComparer<MemberBinding>, IEqualityComparer<ElementInit>
    {
        private static readonly IDictionary<ParameterExpression, int> EmptyMap = new Dictionary<ParameterExpression, int>();
        public static readonly ExpressionEqualityComparer Instance = new ExpressionEqualityComparer(ExpressionComparisonOptions.None);
        private readonly Equality eq;
        private readonly HashcodeCalc hash;

        public ExpressionEqualityComparer(ExpressionComparisonOptions options)
        {
            this.eq = new Equality(options);
            this.hash = new HashcodeCalc(options);
        }

        public bool Equals(ElementInit x, ElementInit y)
        {
            return this.eq.Equals(x, y);
        }

        public bool Equals(Expression x, Expression y)
        {
            return this.Equals(x, null, y, null);
        }

        public bool Equals(LambdaExpression x, LambdaExpression y)
        {
            return this.eq.Equals(x, y);
        }

        public bool Equals(MemberBinding x, MemberBinding y)
        {
            return this.eq.Equals(x, y);
        }

        public bool Equals(Expression x, IDictionary<ParameterExpression, int> xParameterMap, Expression y, IDictionary<ParameterExpression, int> yParameterMap)
        {
            return this.eq.Equals(x, xParameterMap, y, yParameterMap);
        }

        public int GetHashCode(ElementInit init)
        {
            return this.hash.GetHashCode(init);
        }

        public int GetHashCode(Expression expr)
        {
            return this.hash.GetHashCode(expr);
        }

        public int GetHashCode(LambdaExpression lambda)
        {
            return this.hash.GetHashCode(lambda);
        }

        public int GetHashCode(MemberBinding binding)
        {
            return this.hash.GetHashCode(binding);
        }

        private sealed class Equality : DoubleExpressionVisitor<bool>
        {
            private readonly ExpressionComparisonOptions _options;
            private readonly Stack<ExpressionEqualityComparer.ParameterMaps> _paramMaps;

            public Equality(ExpressionComparisonOptions options)
            {
                this._options = options;
                if (this.ParametersByIndex)
                {
                    this._paramMaps = new Stack<ExpressionEqualityComparer.ParameterMaps>();
                }
            }

            protected override bool DefaultVisit(Expression x, Expression y)
            {
                return ((x.NodeType == y.NodeType) && (x.Type == y.Type));
            }

            public bool Equals(ElementInit x, ElementInit y)
            {
                return ((x == y) || ((((x != null) && (y != null)) && (x.AddMethod == y.AddMethod)) && this.VisitLists(x.Arguments, y.Arguments)));
            }

            public bool Equals(LambdaExpression x, LambdaExpression y)
            {
                if (x == y)
                {
                    return true;
                }
                if ((x != null) && (y != null))
                {
                    return this.VisitLambda(x, y);
                }
                return false;
            }

            public bool Equals(MemberBinding x, MemberBinding y)
            {
                if (x == y)
                {
                    return true;
                }
                if ((x != null) && (y != null))
                {
                    if ((x.BindingType != y.BindingType) || (x.Member != y.Member))
                    {
                        return false;
                    }
                    switch (y.BindingType)
                    {
                        case MemberBindingType.Assignment:
                            return this.VisitMemberAssignment(x as MemberAssignment, y as MemberAssignment);

                        case MemberBindingType.MemberBinding:
                            return this.VisitMemberMemberBinding(x as MemberMemberBinding, y as MemberMemberBinding);

                        case MemberBindingType.ListBinding:
                            return this.VisitMemberListBinding(x as MemberListBinding, y as MemberListBinding);
                    }
                }
                return false;
            }

            public bool Equals(Expression x, IDictionary<ParameterExpression, int> xParameterIndexMap, Expression y, IDictionary<ParameterExpression, int> yParameterIndexMap)
            {
                bool flag;
                if (x == y)
                {
                    return true;
                }
                if ((x == null) || (y == null))
                {
                    return false;
                }
                if (!this.ParametersByIndex)
                {
                    return this.Visit(x, y);
                }
                ExpressionEqualityComparer.ParameterMaps item = new ExpressionEqualityComparer.ParameterMaps(xParameterIndexMap ?? ExpressionEqualityComparer.EmptyMap, yParameterIndexMap ?? ExpressionEqualityComparer.EmptyMap);
                this._paramMaps.Push(item);
                try
                {
                    flag = this.Visit(x, y);
                }
                finally
                {
                    this._paramMaps.Pop();
                }
                return flag;
            }

            private int? FindIndex(ParameterExpression param, Func<ExpressionEqualityComparer.ParameterMaps, IDictionary<ParameterExpression, int>> mapSelector)
            {
                foreach (ExpressionEqualityComparer.ParameterMaps maps in this._paramMaps)
                {
                    int num;
                    if (mapSelector.Invoke(maps).TryGetValue(param, out num))
                    {
                        return new int?(num);
                    }
                }
                return null;
            }

            private static bool ListsEqual<T>(ReadOnlyCollection<T> x, ReadOnlyCollection<T> y) where T: class
            {
                if (x != y)
                {
                    if ((x == null) || (y == null))
                    {
                        return false;
                    }
                    if (x.Count != y.Count)
                    {
                        return false;
                    }
                    for (int i = 0; i < x.Count; i++)
                    {
                        if (x[i] != y[i])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            protected override bool Visit(Expression x, Expression y)
            {
                if (x == y)
                {
                    return true;
                }
                if ((x != null) && (y != null))
                {
                    return base.Visit(x, y);
                }
                return false;
            }

            protected override bool VisitBinary(BinaryExpression x, BinaryExpression y)
            {
                if ((this.DefaultVisit(x, y) && (x.Method == y.Method)) && this.Equals(x.Conversion, y.Conversion))
                {
                    if (this.IgnoreOrder && Xtnz.IsCommutative(x))
                    {
                        if (this.Visit(x.Left, y.Left) && this.Visit(x.Right, y.Right))
                        {
                            return true;
                        }
                        if (this.Visit(x.Left, y.Right))
                        {
                            return this.Visit(x.Right, y.Left);
                        }
                        return false;
                    }
                    if (this.Visit(x.Left, y.Left))
                    {
                        return this.Visit(x.Right, y.Right);
                    }
                }
                return false;
            }

            private bool VisitBindings(ReadOnlyCollection<MemberBinding> x, ReadOnlyCollection<MemberBinding> y)
            {
                return VisitLists<MemberBinding>(x, y, new Func<MemberBinding, MemberBinding, bool>(this, this.Equals));
            }

            protected override bool VisitConditional(ConditionalExpression x, ConditionalExpression y)
            {
                if ((this.DefaultVisit(x, y) && this.Visit(x.Test, y.Test)) && this.Visit(x.IfTrue, y.IfTrue))
                {
                    return this.Visit(y.IfTrue, y.IfTrue);
                }
                return false;
            }

            protected override bool VisitConstant(ConstantExpression x, ConstantExpression y)
            {
                return (this.DefaultVisit(x, y) && object.Equals(x.Value, y.Value));
            }

            protected override bool VisitInvocation(InvocationExpression x, InvocationExpression y)
            {
                return ((this.DefaultVisit(x, y) && this.Visit(x.Expression, y.Expression)) && this.VisitLists(x.Arguments, y.Arguments));
            }

            protected override bool VisitLambda(LambdaExpression x, LambdaExpression y)
            {
                if (this.DefaultVisit(x, y))
                {
                    if (x.Parameters.Count != y.Parameters.Count)
                    {
                        return false;
                    }
                    if (this.ParametersByIndex)
                    {
                        ExpressionEqualityComparer.ParameterMaps item = new ExpressionEqualityComparer.ParameterMaps(Lambda.GetParametersMap(x), Lambda.GetParametersMap(y));
                        this._paramMaps.Push(item);
                        try
                        {
                            return this.Visit(x.Body, y.Body);
                        }
                        finally
                        {
                            this._paramMaps.Pop();
                        }
                    }
                    if (VisitLists<ParameterExpression>(x.Parameters, y.Parameters, new Func<ParameterExpression, ParameterExpression, bool>(this, this.VisitParameter)))
                    {
                        return this.Visit(x.Body, y.Body);
                    }
                }
                return false;
            }

            protected override bool VisitListInit(ListInitExpression x, ListInitExpression y)
            {
                if (this.DefaultVisit(x, y) && VisitLists<ElementInit>(x.Initializers, y.Initializers, new Func<ElementInit, ElementInit, bool>(this, this.Equals)))
                {
                    return this.VisitNew(x.NewExpression, y.NewExpression);
                }
                return false;
            }

            private bool VisitLists(ReadOnlyCollection<Expression> x, ReadOnlyCollection<Expression> y)
            {
                return VisitLists<Expression>(x, y, new Func<Expression, Expression, bool>(this, this.Visit));
            }

            private static bool VisitLists<T>(ReadOnlyCollection<T> x, ReadOnlyCollection<T> y, Func<T, T, bool> visitor)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }
                for (int i = 0; i < x.Count; i++)
                {
                    if (!visitor.Invoke(x[i], y[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            protected override bool VisitMemberAccess(MemberExpression x, MemberExpression y)
            {
                if (this.DefaultVisit(x, y) && (x.Member == y.Member))
                {
                    return this.Visit(x.Expression, y.Expression);
                }
                return false;
            }

            private bool VisitMemberAssignment(MemberAssignment x, MemberAssignment y)
            {
                return this.Visit(x.Expression, y.Expression);
            }

            protected override bool VisitMemberInit(MemberInitExpression x, MemberInitExpression y)
            {
                if (this.DefaultVisit(x, y) && this.VisitBindings(x.Bindings, y.Bindings))
                {
                    return this.VisitNew(x.NewExpression, y.NewExpression);
                }
                return false;
            }

            private bool VisitMemberListBinding(MemberListBinding x, MemberListBinding y)
            {
                return VisitLists<ElementInit>(x.Initializers, y.Initializers, new Func<ElementInit, ElementInit, bool>(this, this.Equals));
            }

            private bool VisitMemberMemberBinding(MemberMemberBinding x, MemberMemberBinding y)
            {
                return this.VisitBindings(x.Bindings, y.Bindings);
            }

            protected override bool VisitMethodCall(MethodCallExpression x, MethodCallExpression y)
            {
                return (((this.DefaultVisit(x, y) && (x.Method == y.Method)) && this.Visit(x.Object, y.Object)) && this.VisitLists(x.Arguments, y.Arguments));
            }

            protected override bool VisitNew(NewExpression x, NewExpression y)
            {
                return (((this.DefaultVisit(x, y) && (x.Constructor == y.Constructor)) && ListsEqual<MemberInfo>(x.Members, y.Members)) && this.VisitLists(x.Arguments, y.Arguments));
            }

            protected override bool VisitNewArray(NewArrayExpression x, NewArrayExpression y)
            {
                return this.VisitLists(x.Expressions, y.Expressions);
            }

            protected override bool VisitParameter(ParameterExpression x, ParameterExpression y)
            {
                if (this.ParametersByIndex)
                {
                    int? nullable = this.FindIndex(x, <>c.<>9__32_0 ?? (<>c.<>9__32_0 = new Func<ExpressionEqualityComparer.ParameterMaps, IDictionary<ParameterExpression, int>>(<>c.<>9, this.<VisitParameter>b__32_0)));
                    int? nullable2 = this.FindIndex(y, <>c.<>9__32_1 ?? (<>c.<>9__32_1 = new Func<ExpressionEqualityComparer.ParameterMaps, IDictionary<ParameterExpression, int>>(<>c.<>9, this.<VisitParameter>b__32_1)));
                    if (nullable.HasValue && nullable2.HasValue)
                    {
                        int? nullable3 = nullable;
                        int? nullable4 = nullable2;
                        if (nullable3.GetValueOrDefault() != nullable4.GetValueOrDefault())
                        {
                            return false;
                        }
                        return (nullable3.HasValue == nullable4.HasValue);
                    }
                }
                return ((x.Type == y.Type) && (x.Name == y.Name));
            }

            protected override bool VisitTypeIs(TypeBinaryExpression x, TypeBinaryExpression y)
            {
                if (this.DefaultVisit(x, y) && (x.TypeOperand == y.TypeOperand))
                {
                    return this.Visit(x.Expression, y.Expression);
                }
                return false;
            }

            protected override bool VisitUnary(UnaryExpression x, UnaryExpression y)
            {
                if (this.DefaultVisit(x, y) && (x.Method == y.Method))
                {
                    return this.Visit(x.Operand, y.Operand);
                }
                return false;
            }

            private bool ParametersByIndex
            {
                get
                {
                    return ((this._options & ExpressionComparisonOptions.ParametersByIndex) == ExpressionComparisonOptions.ParametersByIndex);
                }
            }

            private bool IgnoreOrder
            {
                get
                {
                    return ((this._options & ExpressionComparisonOptions.IgnoreOrder) == ExpressionComparisonOptions.IgnoreOrder);
                }
            }

            [Serializable, CompilerGenerated]
            private sealed class <>c
            {
                public static readonly ExpressionEqualityComparer.Equality.<>c <>9 = new ExpressionEqualityComparer.Equality.<>c();
                public static Func<ExpressionEqualityComparer.ParameterMaps, IDictionary<ParameterExpression, int>> <>9__32_0;
                public static Func<ExpressionEqualityComparer.ParameterMaps, IDictionary<ParameterExpression, int>> <>9__32_1;

                internal IDictionary<ParameterExpression, int> <VisitParameter>b__32_0(ExpressionEqualityComparer.ParameterMaps p)
                {
                    return p.X;
                }

                internal IDictionary<ParameterExpression, int> <VisitParameter>b__32_1(ExpressionEqualityComparer.ParameterMaps p)
                {
                    return p.Y;
                }
            }
        }

        private sealed class HashcodeCalc : ExpressionVisitor<int>
        {
            private readonly ExpressionComparisonOptions _options;
            private readonly Stack<IDictionary<ParameterExpression, int>> _paramMaps;

            public HashcodeCalc(ExpressionComparisonOptions options)
            {
                this._options = options;
                if (this.ParametersByIndex)
                {
                    this._paramMaps = new Stack<IDictionary<ParameterExpression, int>>();
                }
            }

            protected override int DefaultVisit(Expression exp)
            {
                return (exp.NodeType.GetHashCode() ^ exp.Type.GetHashCode());
            }

            public int GetHashCode(ElementInit init)
            {
                if (init == null)
                {
                    return 0;
                }
                return (ghc<MethodInfo>(init.AddMethod) ^ this.VisitList(init.Arguments));
            }

            public int GetHashCode(Expression expr)
            {
                return this.Visit(expr);
            }

            public int GetHashCode(MemberBinding b)
            {
                if (b == null)
                {
                    return 0;
                }
                int num = b.BindingType.GetHashCode() ^ ghc<MemberInfo>(b.Member);
                switch (b.BindingType)
                {
                    case MemberBindingType.Assignment:
                        return (num ^ this.VisitMemberAssignment(b as MemberAssignment));

                    case MemberBindingType.MemberBinding:
                        return (num ^ this.VisitMemberMemberBinding(b as MemberMemberBinding));

                    case MemberBindingType.ListBinding:
                        return (num ^ this.VisitMemberListBinding(b as MemberListBinding));
                }
                return num;
            }

            private static int ghc<T>(T obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                return obj.GetHashCode();
            }

            private static int ghcList<T>(IEnumerable<T> seq)
            {
                int num = 0;
                if (seq != null)
                {
                    foreach (T local in seq)
                    {
                        num ^= ghc<T>(local);
                    }
                }
                return num;
            }

            protected override int Visit(Expression expr)
            {
                if (expr == null)
                {
                    return 0;
                }
                return base.Visit(expr);
            }

            protected override int VisitBinary(BinaryExpression exp)
            {
                return ((((this.DefaultVisit(exp) ^ this.Visit(exp.Conversion)) ^ this.Visit(exp.Left)) ^ this.Visit(exp.Right)) ^ ghc<MethodInfo>(exp.Method));
            }

            protected override int VisitConditional(ConditionalExpression exp)
            {
                return (((this.DefaultVisit(exp) ^ this.Visit(exp.Test)) ^ this.Visit(exp.IfTrue)) ^ this.Visit(exp.IfFalse));
            }

            protected override int VisitConstant(ConstantExpression exp)
            {
                return (this.DefaultVisit(exp) ^ ghc<object>(exp.Value));
            }

            protected override int VisitInvocation(InvocationExpression exp)
            {
                return ((this.DefaultVisit(exp) ^ this.Visit(exp.Expression)) ^ this.VisitList(exp.Arguments));
            }

            protected override int VisitLambda(LambdaExpression exp)
            {
                int num = this.DefaultVisit(exp);
                if (this.ParametersByIndex)
                {
                    Dictionary<ParameterExpression, int> parametersMap = Lambda.GetParametersMap(exp);
                    this._paramMaps.Push(parametersMap);
                    try
                    {
                        num ^= exp.Parameters.Count;
                        return (num ^ this.Visit(exp.Body));
                    }
                    finally
                    {
                        this._paramMaps.Pop();
                    }
                }
                return (num ^ VisitList<ParameterExpression>(exp.Parameters, new Func<ParameterExpression, int>(this, this.VisitParameter)));
            }

            private int VisitList(IEnumerable<ElementInit> inits)
            {
                return VisitList<ElementInit>(inits, new Func<ElementInit, int>(this, this.GetHashCode));
            }

            private int VisitList(IEnumerable<Expression> elements)
            {
                return VisitList<Expression>(elements, new Func<Expression, int>(this, this.Visit));
            }

            private int VisitList(IEnumerable<MemberBinding> original)
            {
                return VisitList<MemberBinding>(original, new Func<MemberBinding, int>(this, this.GetHashCode));
            }

            private static int VisitList<T>(IEnumerable<T> elements, Func<T, int> visitor)
            {
                int num = 0;
                foreach (T local in elements)
                {
                    num ^= visitor.Invoke(local);
                }
                return num;
            }

            protected override int VisitListInit(ListInitExpression exp)
            {
                return ((this.DefaultVisit(exp) ^ this.VisitList(exp.Initializers)) ^ this.VisitNew(exp.NewExpression));
            }

            protected override int VisitMemberAccess(MemberExpression exp)
            {
                return ((this.DefaultVisit(exp) ^ ghc<MemberInfo>(exp.Member)) ^ this.Visit(exp.Expression));
            }

            private int VisitMemberAssignment(MemberAssignment assignment)
            {
                return this.Visit(assignment.Expression);
            }

            protected override int VisitMemberInit(MemberInitExpression exp)
            {
                return ((this.DefaultVisit(exp) ^ this.VisitList(exp.Bindings)) ^ this.VisitNew(exp.NewExpression));
            }

            private int VisitMemberListBinding(MemberListBinding binding)
            {
                return this.VisitList(binding.Initializers);
            }

            private int VisitMemberMemberBinding(MemberMemberBinding binding)
            {
                return this.VisitList(binding.Bindings);
            }

            protected override int VisitMethodCall(MethodCallExpression exp)
            {
                return (((this.DefaultVisit(exp) ^ exp.Method.GetHashCode()) ^ this.Visit(exp.Object)) ^ this.VisitList(exp.Arguments));
            }

            protected override int VisitNew(NewExpression nex)
            {
                return (((this.DefaultVisit(nex) ^ nex.Constructor.GetHashCode()) ^ ghcList<MemberInfo>(nex.Members)) ^ this.VisitList(nex.Arguments));
            }

            protected override int VisitNewArray(NewArrayExpression exp)
            {
                return (this.DefaultVisit(exp) ^ this.VisitList(exp.Expressions));
            }

            protected override int VisitParameter(ParameterExpression exp)
            {
                if (this.ParametersByIndex)
                {
                    using (Stack<IDictionary<ParameterExpression, int>>.Enumerator enumerator = this._paramMaps.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            int num;
                            if (enumerator.Current.TryGetValue(exp, out num))
                            {
                                return num;
                            }
                        }
                    }
                }
                return (this.DefaultVisit(exp) ^ exp.Name.GetHashCode());
            }

            protected override int VisitTypeIs(TypeBinaryExpression exp)
            {
                return ((this.DefaultVisit(exp) ^ exp.TypeOperand.GetHashCode()) ^ this.Visit(exp.Expression));
            }

            protected override int VisitUnary(UnaryExpression exp)
            {
                return ((this.DefaultVisit(exp) ^ ghc<MethodInfo>(exp.Method)) ^ this.Visit(exp.Operand));
            }

            private bool ParametersByIndex
            {
                get
                {
                    return ((this._options & ExpressionComparisonOptions.ParametersByIndex) == ExpressionComparisonOptions.ParametersByIndex);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ParameterMaps
        {
            public readonly IDictionary<ParameterExpression, int> X;
            public readonly IDictionary<ParameterExpression, int> Y;
            public ParameterMaps(IDictionary<ParameterExpression, int> x, IDictionary<ParameterExpression, int> y)
            {
                this.X = x;
                this.Y = y;
            }
        }
    }
}

