namespace FwNs.Txt
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    internal static class Statk
    {
        public static Func<string, string> SnglToDblQuotXprsn;
        public static Func<string, string> Kill_MultiLnBrks;
        public static Func<string, string> Kill_LnBrks;
        public static Expression<Func<string, string, bool>> WrdMtchWrdCs;
        public static Expression<Func<string, string, bool>> WrdMtchWrdIs;
        public static Expression<Func<string, string, bool>> WrdMtchWrd;
        public static Func<string, string> QuotdFunc;
        public static Func<string, string, string> NmValFunc;
        public static string[] Splttrs;

        static Statk()
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            SnglToDblQuotXprsn = new Func<string, string>(<>c.<>9, this.<.cctor>b__9_0);
            Kill_MultiLnBrks = new Func<string, string>(<>c.<>9, this.<.cctor>b__9_1);
            Kill_LnBrks = new Func<string, string>(<>c.<>9, this.<.cctor>b__9_2);
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
            WrdMtchWrdCs = Expression.Lambda<Func<string, string, bool>>(Expression.Equal(expression = Expression.Parameter(typeof(string), "w"), expression2 = Expression.Parameter(typeof(string), "s")), parameters);
            ParameterExpression[] expressionArray2 = new ParameterExpression[] { expression2, expression };
            WrdMtchWrdIs = Expression.Lambda<Func<string, string, bool>>(Expression.Equal(Expression.Call(expression2 = Expression.Parameter(typeof(string), "w"), (MethodInfo) methodof(string.ToLower), new Expression[0]), Expression.Call(expression = Expression.Parameter(typeof(string), "s"), (MethodInfo) methodof(string.ToLower), new Expression[0])), expressionArray2);
            Expression[] arguments = new Expression[] { expression = Expression.Parameter(typeof(string), "w"), expression2 = Expression.Parameter(typeof(string), "s") };
            Expression[] expressionArray4 = new Expression[] { expression, expression2 };
            ParameterExpression[] expressionArray5 = new ParameterExpression[] { expression, expression2 };
            WrdMtchWrd = Expression.Lambda<Func<string, string, bool>>(Expression.OrElse(Expression.Invoke(Expression.Call(Expression.Field(null, fieldof(Statk.WrdMtchWrdCs)), (MethodInfo) methodof(Expression<Func<string, string, bool>>.Compile, Expression<Func<string, string, bool>>), new Expression[0]), arguments), Expression.Invoke(Expression.Call(Expression.Field(null, fieldof(Statk.WrdMtchWrdIs)), (MethodInfo) methodof(Expression<Func<string, string, bool>>.Compile, Expression<Func<string, string, bool>>), new Expression[0]), expressionArray4)), expressionArray5);
            QuotdFunc = new Func<string, string>(<>c.<>9, this.<.cctor>b__9_3);
            NmValFunc = new Func<string, string, string>(<>c.<>9, this.<.cctor>b__9_4);
            Splttrs = new string[0];
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly Statk.<>c <>9 = new Statk.<>c();

            internal string <.cctor>b__9_0(string s)
            {
                char ch = '"';
                return s.Replace("'", ch.ToString());
            }

            internal string <.cctor>b__9_1(string s)
            {
                return Regex.Replace(s, @"(\r|\n)+\s*", "");
            }

            internal string <.cctor>b__9_2(string s)
            {
                return Regex.Replace(s, @"(\r|\n)+\s*", "");
            }

            internal string <.cctor>b__9_3(string val)
            {
                object[] a = new object[] { val };
                return Xtnz.Frmt("\"{0}\"", a);
            }

            internal string <.cctor>b__9_4(string sNm, string sVal)
            {
                object[] a = new object[] { "{", sNm, sVal, "}" };
                return Xtnz.Frmt("{0}\"{1}\":{2}{3}", a);
            }
        }
    }
}

