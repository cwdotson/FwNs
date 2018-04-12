namespace FwNs.Linq.Xprsns
{
    using FwNs.Core.Typs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    [Extension]
    internal static class Lambda
    {
        [Extension]
        internal static Expression<Func<T, TKey>> AsLambdaSafe<T, TKey>(Function<T, TKey> func, string argName)
        {
            Argument.NotNull<Function<T, TKey>>(func, argName);
            return func.Expression;
        }

        internal static LambdaExpression CreateIdentityFunction(Type argType)
        {
            ParameterExpression body = Expression.Parameter(argType, "x");
            ParameterExpression[] parameters = new ParameterExpression[] { body };
            return Expression.Lambda(body, parameters);
        }

        [Extension]
        internal static bool DependsOnParameter(LambdaExpression lambda, int paramOrdinal)
        {
            throw new Exception();
        }

        public static Expression ExtractBody<TResult>(Expression<Func<TResult>> lambda)
        {
            Argument.NotNull<Expression<Func<TResult>>>(lambda, "lambda");
            return lambda.Body;
        }

        public static Expression ExtractBody<T1, TResult>(Expression<Func<T1, TResult>> lambda, Expression param)
        {
            Argument.NotNull<Expression<Func<T1, TResult>>>(lambda, "lambda");
            Expression[] newParams = new Expression[] { param };
            return ExtractBody(lambda, newParams);
        }

        [Extension]
        public static Expression ExtractBody(LambdaExpression lambda, Expression newParameter)
        {
            Argument.NotNull<LambdaExpression>(lambda, "lambda");
            Argument.NotNull<Expression>(newParameter, "newParameter");
            if (lambda.Parameters.Count != 1)
            {
                throw new ArgumentException(EMExpressions.InvalidParameterCount);
            }
            throw new Exception();
        }

        [Extension]
        public static Expression ExtractBody(LambdaExpression lambda, params Expression[] newParams)
        {
            Argument.NotNull<LambdaExpression>(lambda, "lambda");
            Argument.NotNull<Expression[]>(newParams, "newParams");
            if (lambda.Parameters.Count != newParams.Length)
            {
                throw new ArgumentException(EMExpressions.InvalidParameterCount);
            }
            throw new Exception();
        }

        public static Expression ExtractBody<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> lambda, Expression param1, Expression param2)
        {
            Argument.NotNull<Expression<Func<T1, T2, TResult>>>(lambda, "lambda");
            Expression[] newParams = new Expression[] { param1, param2 };
            return ExtractBody(lambda, newParams);
        }

        [Extension]
        public static Expression ExtractBody(LambdaExpression lambda, Expression newParameter1, Expression newParameter2)
        {
            Argument.NotNull<LambdaExpression>(lambda, "lambda");
            Argument.NotNull<Expression>(newParameter1, "newParameter1");
            Argument.NotNull<Expression>(newParameter2, "newParameter2");
            if (lambda.Parameters.Count != 2)
            {
                throw new ArgumentException(EMExpressions.InvalidParameterCount);
            }
            throw new Exception();
        }

        public static Expression ExtractBody<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> lambda, Expression param1, Expression param2, Expression param3)
        {
            Argument.NotNull<Expression<Func<T1, T2, T3, TResult>>>(lambda, "lambda");
            Expression[] newParams = new Expression[] { param1, param2, param3 };
            return ExtractBody(lambda, newParams);
        }

        public static Expression ExtractBody<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> lambda, Expression param1, Expression param2, Expression param3, Expression param4)
        {
            Argument.NotNull<Expression<Func<T1, T2, T3, T4, TResult>>>(lambda, "lambda");
            Expression[] newParams = new Expression[] { param1, param2, param3, param4 };
            return ExtractBody(lambda, newParams);
        }

        public static Expression<Func<TResult>> From<TResult>(Expression<Func<TResult>> expression)
        {
            return expression;
        }

        public static Expression<Func<T1, TResult>> From<T1, TResult>(Expression<Func<T1, TResult>> expression)
        {
            return expression;
        }

        public static Expression<Func<T1, T2, TResult>> From<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression)
        {
            return expression;
        }

        public static Expression<Func<T1, T2, T3, TResult>> From<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression)
        {
            return expression;
        }

        public static Expression<Func<T1, T2, T3, T4, TResult>> From<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression)
        {
            return expression;
        }

        public static Expression<Func<TResult>> Function<TResult>(Expression body)
        {
            return Expression.Lambda<Func<TResult>>(body, new ParameterExpression[0]);
        }

        public static Expression<Func<T1, TResult>> Function<T1, TResult>(Expression body, ParameterExpression param)
        {
            ParameterExpression[] parameters = new ParameterExpression[] { param };
            return Expression.Lambda<Func<T1, TResult>>(body, parameters);
        }

        public static LambdaExpression Function(Expression body, params ParameterExpression[] parameters)
        {
            return Expression.Lambda(body, parameters);
        }

        public static Expression<Func<T1, T2, TResult>> Function<T1, T2, TResult>(Expression body, ParameterExpression param1, ParameterExpression param2)
        {
            ParameterExpression[] parameters = new ParameterExpression[] { param1, param2 };
            return Expression.Lambda<Func<T1, T2, TResult>>(body, parameters);
        }

        public static Expression<Func<T1, T2, T3, TResult>> Function<T1, T2, T3, TResult>(Expression body, ParameterExpression param1, ParameterExpression param2, ParameterExpression param3)
        {
            ParameterExpression[] parameters = new ParameterExpression[] { param1, param2, param3 };
            return Expression.Lambda<Func<T1, T2, T3, TResult>>(body, parameters);
        }

        public static Expression<Func<T1, T2, T3, T4, TResult>> Function<T1, T2, T3, T4, TResult>(Expression body, ParameterExpression param1, ParameterExpression param2, ParameterExpression param3, ParameterExpression param4)
        {
            ParameterExpression[] parameters = new ParameterExpression[] { param1, param2, param3, param4 };
            return Expression.Lambda<Func<T1, T2, T3, T4, TResult>>(body, parameters);
        }

        [Extension]
        internal static Dictionary<ParameterExpression, int> GetParametersMap(LambdaExpression lambda)
        {
            Dictionary<ParameterExpression, int> dictionary = new Dictionary<ParameterExpression, int>(lambda.Parameters.Count);
            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                dictionary.Add(lambda.Parameters[i], i);
            }
            return dictionary;
        }

        [Extension]
        internal static bool HasOneParameter(LambdaExpression lambda)
        {
            return (lambda.Parameters.Count == 1);
        }

        [Extension]
        internal static LambdaExpression Invert(LambdaExpression func)
        {
            return Invert(func, 0);
        }

        [Extension]
        internal static LambdaExpression Invert(LambdaExpression func, int paramIndex)
        {
            return new FunctionInverter().Invert(func, paramIndex);
        }

        [Extension]
        internal static LambdaExpression Invert(LambdaExpression func, int paramIndex, LambdaExpression selector)
        {
            LambdaExpression expression = InvertSubstitution(func, selector, paramIndex, null);
            if (expression == null)
            {
                return null;
            }
            return Invert(expression, paramIndex);
        }

        [Extension]
        internal static LambdaExpression InvertSubstitution(LambdaExpression sourceFunc, LambdaExpression substitutedFunc)
        {
            return InvertSubstitution(sourceFunc, substitutedFunc, "x");
        }

        [Extension]
        internal static LambdaExpression InvertSubstitution(LambdaExpression sourceFunc, LambdaExpression substitutedFunc, string newParamName)
        {
            return InvertSubstitution(sourceFunc, substitutedFunc, 0, newParamName);
        }

        [Extension]
        internal static LambdaExpression InvertSubstitution(LambdaExpression sourceFunc, LambdaExpression substitutedFunc, Dictionary<ParameterExpression, ParameterExpression> sourceToSubstituedParameterMap, string newParamName)
        {
            return new SubstitutionInverter().Invert(sourceFunc, substitutedFunc, sourceToSubstituedParameterMap, newParamName);
        }

        [Extension]
        internal static LambdaExpression InvertSubstitution(LambdaExpression sourceFunc, LambdaExpression substitutedFunc, int sourceParamOrdinal, string newParamName)
        {
            Dictionary<ParameterExpression, ParameterExpression> sourceToSubstituedParameterMap = new Dictionary<ParameterExpression, ParameterExpression> {
                { 
                    sourceFunc.Parameters[sourceParamOrdinal],
                    substitutedFunc.Parameters[0]
                }
            };
            return InvertSubstitution(sourceFunc, substitutedFunc, sourceToSubstituedParameterMap, newParamName ?? sourceFunc.Parameters[sourceParamOrdinal].Name);
        }

        [Extension]
        internal static bool IsDependentOnParameters(LambdaExpression lambda)
        {
            Argument.NotNull<LambdaExpression>(lambda, "lambda");
            if (lambda.Parameters.Count != 0)
            {
                throw new Exception();
            }
            return false;
        }

        [Extension]
        internal static bool IsPredicate(LambdaExpression lambda)
        {
            return (HasOneParameter(lambda) && (lambda.Body.Type == typeof(bool)));
        }

        [Extension]
        internal static LambdaExpression MakeParameterTypes(LambdaExpression lambda, Type desiredParameterType)
        {
            <>c__DisplayClass40_0 class_;
            if (Enumerable.All<ParameterExpression>(lambda.Parameters, new Func<ParameterExpression, bool>(class_, this.<MakeParameterTypes>b__0)))
            {
                return lambda;
            }
            Enumerable.ToArray(Enumerable.Select(Enumerable.Select(lambda.Parameters, <>c.<>9__40_1 ?? (<>c.<>9__40_1 = new Func<ParameterExpression, <>f__AnonymousType6<ParameterExpression, ParameterExpression>>(<>c.<>9, this.<MakeParameterTypes>b__40_1))), <>c.<>9__40_2 ?? (<>c.<>9__40_2 = new Func<<>f__AnonymousType6<ParameterExpression, ParameterExpression>, <>f__AnonymousType7<ParameterExpression, ParameterExpression, UnaryExpression>>(<>c.<>9, this.<MakeParameterTypes>b__40_2))));
            throw new Exception();
        }

        [Extension]
        public static LambdaExpression MakeReturnType(LambdaExpression func, Type desiredReturnType)
        {
            if (func.Body.Type != desiredReturnType)
            {
                func = Expression.Lambda(Expression.Convert(func.Body, desiredReturnType), Enumerable.ToArray<ParameterExpression>(func.Parameters));
            }
            return func;
        }

        public static Expression<Func<T, bool>> Predicate<T>(Expression body, ParameterExpression parameter)
        {
            Argument.NotNull<Expression>(body, "body");
            Argument.NotNull<ParameterExpression>(parameter, "parameter");
            ParameterExpression[] parameters = new ParameterExpression[] { parameter };
            return Expression.Lambda<Func<T, bool>>(body, parameters);
        }

        public static LambdaExpression Predicate(Expression body, ParameterExpression parameter)
        {
            Argument.NotNull<Expression>(body, "body");
            Argument.NotNull<ParameterExpression>(parameter, "parameter");
            ParameterExpression[] parameters = new ParameterExpression[] { parameter };
            return Expression.Lambda(body, parameters);
        }

        [Extension]
        internal static LambdaExpression RemoveResultConversion(LambdaExpression function)
        {
            throw new Exception();
        }

        [Extension]
        public static LambdaExpression ReplaceParameters(LambdaExpression function, params ParameterExpression[] newParameters)
        {
            throw new Exception();
        }

        [Extension]
        internal static Expression<Func<T2, TResult1>> Substitute<T1, TResult1, T2, TResult2>(Expression<Func<T1, TResult1>> sourceFunc, Expression<Func<T2, TResult2>> funcToSubstitute) where TResult2: T1
        {
            return (Substitute(sourceFunc, 0, funcToSubstitute) as Expression<Func<T2, TResult1>>);
        }

        [Extension]
        internal static LambdaExpression Substitute(LambdaExpression sourceFunc, int parameterIndex, LambdaExpression funcToSubstitute)
        {
            ParameterExpression local1 = sourceFunc.Parameters[parameterIndex];
            Enumerable.ToArray<ParameterExpression>(sourceFunc.Parameters)[parameterIndex] = funcToSubstitute.Parameters[0];
            throw new Exception();
        }

        [Extension]
        public static Function<T, TResult> ToFunc<T, TResult>(Func<T, TResult> @delegate)
        {
            Type[] types = new Type[] { typeof(Func<T, TResult>) };
            object[] parameters = new object[] { @delegate };
            return (Function<T, TResult>) typeof(Function<T, TResult>).GetMethod("CreateDelegateBased", types).Invoke(null, parameters);
        }

        [Extension]
        public static Function<T, T2, TResult> ToFunc<T, T2, TResult>(Func<T, T2, TResult> @delegate)
        {
            Type[] types = new Type[] { typeof(Func<T, TResult>) };
            object[] parameters = new object[] { @delegate };
            return (Function<T, T2, TResult>) typeof(Function<T, T2, TResult>).GetMethod("CreateDelegateBased", types).Invoke(null, parameters);
        }

        [Extension]
        internal static Function<T, TResult> ToFunc<T, TResult>(Expression<Func<T, TResult>> lambda)
        {
            return new Function<T, TResult>(lambda);
        }

        [Extension]
        internal static Function<T1, T2, TResult> ToFunc<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> lambda)
        {
            return new Function<T1, T2, TResult>(lambda);
        }

        [Extension]
        public static string ToXmlFriendlyString(LambdaExpression lambda)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("|");
            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(lambda.Parameters[i].Name);
            }
            builder.Append("| ").Append(lambda.Body);
            return builder.ToString();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly Lambda.<>c <>9 = new Lambda.<>c();
            public static Func<ParameterExpression, <>f__AnonymousType6<ParameterExpression, ParameterExpression>> <>9__40_1;
            public static Func<<>f__AnonymousType6<ParameterExpression, ParameterExpression>, <>f__AnonymousType7<ParameterExpression, ParameterExpression, UnaryExpression>> <>9__40_2;

            internal <>f__AnonymousType6<ParameterExpression, ParameterExpression> <MakeParameterTypes>b__40_1(ParameterExpression p)
            {
                return new { 
                    p = p,
                    NewParam = Expression.Parameter(typeof(object), p.Name)
                };
            }

            internal <>f__AnonymousType7<ParameterExpression, ParameterExpression, UnaryExpression> <MakeParameterTypes>b__40_2(<>f__AnonymousType6<ParameterExpression, ParameterExpression> <>h__TransparentIdentifier0)
            {
                return new { 
                    OriginalParam = <>h__TransparentIdentifier0.p,
                    NewParam = <>h__TransparentIdentifier0.NewParam,
                    Converted = Expression.Convert(<>h__TransparentIdentifier0.NewParam, <>h__TransparentIdentifier0.p.Type)
                };
            }
        }

        private class FunctionInverter : ExpressionVisitor<Func<Expression, Expression>>
        {
            private ParameterExpression _param;

            protected override Func<Expression, Expression> DefaultVisit(Expression exp)
            {
                return null;
            }

            public LambdaExpression Invert(LambdaExpression func, int parameterIndex)
            {
                this._param = func.Parameters[parameterIndex];
                Func<Expression, Expression> func2 = this.Visit(func.Body);
                if (func2 == null)
                {
                    return null;
                }
                ParameterExpression expression = Expression.Parameter(func.Body.Type, "z");
                Expression expression2 = func2.Invoke(expression);
                if (expression2.Type != this._param.Type)
                {
                    expression2 = Expression.Convert(expression2, this._param.Type);
                }
                ParameterExpression[] parameters = new ParameterExpression[] { expression };
                return Expression.Lambda(expression2, parameters);
            }

            protected override Func<Expression, Expression> VisitMemberInit(MemberInitExpression exp)
            {
                for (int i = 0; i < exp.Bindings.Count; i++)
                {
                    MemberAssignment assignment = exp.Bindings[i] as MemberAssignment;
                    if (assignment != null)
                    {
                        Func<Expression, Expression> innerRule = this.Visit(assignment.Expression);
                        if (innerRule != null)
                        {
                            <>c__DisplayClass5_1 class_1;
                            MemberInfo member = TypsFw.Normalize(assignment.Member);
                            return new Func<Expression, Expression>(class_1, this.<VisitMemberInit>b__0);
                        }
                    }
                }
                return null;
            }

            protected override Func<Expression, Expression> VisitNew(NewExpression nex)
            {
                if (nex.Members != null)
                {
                    for (int i = 0; i < nex.Members.Count; i++)
                    {
                        Func<Expression, Expression> innerRule = this.Visit(nex.Arguments[i]);
                        if (innerRule != null)
                        {
                            <>c__DisplayClass4_1 class_1;
                            MemberInfo member = TypsFw.Normalize(nex.Members[i]);
                            return new Func<Expression, Expression>(class_1, this.<VisitNew>b__0);
                        }
                    }
                }
                return null;
            }

            protected override Func<Expression, Expression> VisitParameter(ParameterExpression exp)
            {
                if (exp != this._param)
                {
                    return null;
                }
                return FwNs.Linq.Xprsns.IdentityFunction<Expression>.Delegate;
            }

            protected override Func<Expression, Expression> VisitUnary(UnaryExpression exp)
            {
                <>c__DisplayClass6_0 class_;
                if (exp.NodeType != ExpressionType.Not)
                {
                    return base.VisitUnary(exp);
                }
                Func<Expression, Expression> innerRule = this.Visit(exp.Operand);
                if (innerRule == null)
                {
                    return null;
                }
                return new Func<Expression, Expression>(class_, this.<VisitUnary>b__0);
            }
        }

        private class SubstitutionInverter : ExpressionRewriter
        {
            private Dictionary<ParameterExpression, ParameterExpression> _sourceToSubstituedParameterMap;
            private Dictionary<ParameterExpression, int> _sourceParamMap;
            private Dictionary<ParameterExpression, int> _sbstParamMap;
            private readonly ExpressionEqualityComparer _comparer = new ExpressionEqualityComparer(ExpressionComparisonOptions.ParametersByIndex | ExpressionComparisonOptions.IgnoreOrder);
            private LambdaExpression _sourceFunc;
            private LambdaExpression _sbstFunc;
            private string _newParamName;
            private ParameterExpression _newParam;

            private ParameterExpression[] CreateNewParameters()
            {
                int num = 0;
                bool flag = false;
                ParameterExpression[] expressionArray = new ParameterExpression[(this._sourceFunc.Parameters.Count - this._sourceToSubstituedParameterMap.Count) + 1];
                foreach (ParameterExpression expression in this._sourceFunc.Parameters)
                {
                    if (!this._sourceToSubstituedParameterMap.ContainsKey(expression))
                    {
                        expressionArray[num++] = expression;
                    }
                    else if (!flag)
                    {
                        expressionArray[num++] = this._newParam;
                        flag = true;
                    }
                }
                return expressionArray;
            }

            private ParameterExpression GetNewParameter()
            {
                if (this._newParam == null)
                {
                    this._newParam = Expression.Parameter(this._sbstFunc.Body.Type, this._newParamName);
                }
                return this._newParam;
            }

            public LambdaExpression Invert(LambdaExpression sourceFunc, LambdaExpression substitutedFunc, Dictionary<ParameterExpression, ParameterExpression> sourceToSubstituedParameterMap, string newParamName)
            {
                this._newParam = null;
                this._newParamName = newParamName;
                this._sourceFunc = sourceFunc;
                this._sbstFunc = substitutedFunc;
                this._sourceParamMap = Lambda.GetParametersMap(sourceFunc);
                this._sbstParamMap = Lambda.GetParametersMap(substitutedFunc);
                this._sourceToSubstituedParameterMap = sourceToSubstituedParameterMap;
                Expression body = this.Rewrite(sourceFunc.Body);
                object.Equals(body == null, base._stopVisiting);
                if (body == null)
                {
                    return null;
                }
                if (this._newParam == null)
                {
                    return null;
                }
                return Expression.Lambda(body, this.CreateNewParameters());
            }

            protected override Expression Visit(Expression expr)
            {
                if (this._comparer.Equals(expr, this._sourceParamMap, this._sbstFunc.Body, this._sbstParamMap))
                {
                    return this.GetNewParameter();
                }
                return base.Visit(expr);
            }

            protected override Expression VisitLambda(LambdaExpression lambda)
            {
                return lambda;
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                if (this._sourceToSubstituedParameterMap.ContainsKey(p))
                {
                    base._stopVisiting = true;
                    return null;
                }
                return p;
            }
        }
    }
}

