namespace FwNs.Txt
{
    using FwNs.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    [Extension]
    public static class Xtnz
    {
        [Extension]
        public static string Frmt(string Pttrn_, params object[] A)
        {
            return string.Format(Pttrn_, A);
        }

        public static Func<string, string, string, string, char, string> FrmtType()
        {
            return (<>c.<>9__28_0 ?? (<>c.<>9__28_0 = new Func<string, string, string, string, char, string>(<>c.<>9, this.<FrmtType>b__28_0)));
        }

        [Extension]
        public static Expression<Func<TA, string, string, string, string>> FrmtXprsn<TA, T>(Func<T, string> t2sf, char c0) where TA: IEnumerable<T>
        {
            return FrmtXprsn<TA, T>(t2sf, "{0}{1}{2}.{3}", c0);
        }

        [Extension]
        public static Expression<Func<TA, string, string, string, string>> FrmtXprsn<TA, T>(Func<T, string> t2sf, string pttrn, char c0) where TA: IEnumerable<T>
        {
            <>c__DisplayClass26_0<TA, T> class_;
            ParameterExpression expression;
            ParameterExpression expression2;
            ParameterExpression expression3;
            ParameterExpression expression4;
            Func<T, string> ffd = <>c__26<TA, T>.<>9__26_0 ?? (<>c__26<TA, T>.<>9__26_0 = new Func<T, string>(<>c__26<TA, T>.<>9, this.<FrmtXprsn>b__26_0));
            Expression[] arguments = new Expression[2];
            arguments[0] = Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass26_0<TA, T>)), fieldof(<>c__DisplayClass26_0<TA, T>.pttrn, <>c__DisplayClass26_0<TA, T>));
            Expression[] initializers = new Expression[4];
            Expression[] expressionArray3 = new Expression[2];
            expressionArray3[0] = Expression.Call(Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass26_0<TA, T>)), fieldof(<>c__DisplayClass26_0<TA, T>.c0, <>c__DisplayClass26_0<TA, T>)), (MethodInfo) methodof(char.ToString), new Expression[0]);
            Expression[] expressionArray4 = new Expression[1];
            Expression[] expressionArray5 = new Expression[] { Expression.Convert(expression = Expression.Parameter(typeof(TA), "tarray"), typeof(IEnumerable<T>)), Expression.Coalesce(Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass26_0<TA, T>)), fieldof(<>c__DisplayClass26_0<TA, T>.t2sf, <>c__DisplayClass26_0<TA, T>)), Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass26_0<TA, T>)), fieldof(<>c__DisplayClass26_0<TA, T>.ffd, <>c__DisplayClass26_0<TA, T>))) };
            expressionArray4[0] = Expression.Call(null, (MethodInfo) methodof(Enumerable.Select), expressionArray5);
            expressionArray3[1] = Expression.Call(null, (MethodInfo) methodof(Enumerable.ToArray), expressionArray4);
            initializers[0] = Expression.Call(null, (MethodInfo) methodof(string.Join), expressionArray3);
            initializers[1] = expression2 = Expression.Parameter(typeof(string), "prfxx");
            initializers[2] = expression3 = Expression.Parameter(typeof(string), "namex");
            initializers[3] = expression4 = Expression.Parameter(typeof(string), "sffxx");
            arguments[1] = Expression.NewArrayInit(typeof(object), initializers);
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2, expression3, expression4 };
            return Expression.Lambda<Func<TA, string, string, string, string>>(Expression.Call(null, (MethodInfo) methodof(string.Format), arguments), parameters);
        }

        [Extension]
        public static Expression<Func<TA, TB, string>> FrmtXprsn<TAA, TA, TAB, TB>(string pttrn, char c0, char c1) where TAA: IEnumerable<TA>
        {
            throw new Exception();
        }

        public static Func<string, string, string, string, string, string, string> FullTypNmFunc()
        {
            return (<>c.<>9__1_0 ?? (<>c.<>9__1_0 = new Func<string, string, string, string, string, string, string>(<>c.<>9, this.<FullTypNmFunc>b__1_0)));
        }

        [Extension]
        public static string GetFullTypNm(Type typ, string vrsn, string cltr, string kyTkn)
        {
            string fullName = typ.Assembly.FullName;
            fullName = typ.Assembly.GetName().Name;
            return GetFullTypNm(typ.Name, typ.Namespace, fullName, vrsn, cltr, kyTkn);
        }

        public static string GetFullTypNm(string typNm, string nmspc, string assmbly, string vrsn, string cltr, string kyTkn)
        {
            return FullTypNmFunc().Invoke(typNm, nmspc, assmbly, vrsn, cltr, kyTkn);
        }

        [Extension]
        public static string Joyn<T>(T tA) where T: IEnumerable<object>
        {
            return Joyn<T>(tA, "");
        }

        [Extension]
        public static string Joyn<TA, TL>(TA tA) where TA: IEnumerable<TL>
        {
            return Joyn<TA, TL>(tA, <>c__21<TA, TL>.<>9__21_0 ?? (<>c__21<TA, TL>.<>9__21_0 = new Func<TL, string>(<>c__21<TA, TL>.<>9, this.<Joyn>b__21_0)), null);
        }

        [Extension]
        public static string Joyn<T>(T tA, Func<object, string> o2sf) where T: IEnumerable<object>
        {
            return Joyn<T>(tA, o2sf ?? (<>c__18<T>.<>9__18_0 ?? (<>c__18<T>.<>9__18_0 = new Func<object, string>(<>c__18<T>.<>9, this.<Joyn>b__18_0))));
        }

        [Extension]
        public static string Joyn<T>(T tA, string btwn) where T: IEnumerable<object>
        {
            return Joyn<T>(tA, null, btwn);
        }

        [Extension]
        public static string Joyn<T>(T tA, Func<object, string> o2sf, string btwn) where T: IEnumerable<object>
        {
            <>c__DisplayClass20_0<T> class_;
            return Joyn<T, object>(tA, o2sf ?? (<>c__20<T>.<>9__20_0 ?? (<>c__20<T>.<>9__20_0 = new Func<object, string>(<>c__20<T>.<>9, this.<Joyn>b__20_0))), new Func<string, string, string>(class_, this.<Joyn>b__1));
        }

        [Extension]
        public static string Joyn<TA, TL>(TA tA, Func<TL, string> o2sf, Func<string, string, string> ss2sf) where TA: IEnumerable<TL>
        {
            if (!FwNs.Core.Xtnz.IsNullOrEmpty<TA>(tA))
            {
                throw new Exception();
            }
            if (FwNs.Core.Xtnz.IsNull<Func<string, string, string>>(ss2sf))
            {
                return string.Join("", Enumerable.ToArray<string>(Enumerable.Select<TL, string>(Enumerable.Cast<TL>(FwNs.Core.Xtnz.IsNull(tA) ? ((IEnumerable) new TA[0]) : ((IEnumerable) tA)), o2sf ?? (<>c__22<TA, TL>.<>9__22_0 ?? (<>c__22<TA, TL>.<>9__22_0 = new Func<TL, string>(<>c__22<TA, TL>.<>9, this.<Joyn>b__22_0))))));
            }
            return Enumerable.Aggregate<string, string>(Enumerable.Select<TL, string>(Enumerable.Cast<TL>(tA), o2sf ?? (<>c__22<TA, TL>.<>9__22_1 ?? (<>c__22<TA, TL>.<>9__22_1 = new Func<TL, string>(<>c__22<TA, TL>.<>9, this.<Joyn>b__22_1)))), "", ss2sf);
        }

        [Extension]
        public static string[] Split(string arg, string spltr)
        {
            return Split(arg, spltr, StringSplitOptions.RemoveEmptyEntries);
        }

        [Extension]
        public static string[] Split(string arg, string spltr, StringSplitOptions so)
        {
            string[] separator = new string[] { spltr };
            return arg.Split(separator, so);
        }

        [Extension]
        public static string[] Splyt(string S)
        {
            return Splyt(S, Splytrs(true, true, true, true));
        }

        [Extension]
        public static string[] Splyt<T>(T strng0)
        {
            throw new Exception();
        }

        [Extension]
        public static string[] Splyt(string arg, char c)
        {
            return arg.Split(Enumerable.ToArray<char>(c.ToString()), StringSplitOptions.RemoveEmptyEntries);
        }

        [Extension]
        public static string[] Splyt(string strngIn, IEnumerable<string> splttrs)
        {
            return Splyt(strngIn, splttrs, StringSplitOptions.RemoveEmptyEntries);
        }

        [Extension]
        public static string[] Splyt<T>(string strngIn, IEnumerable<T> splttrs)
        {
            return Splyt(strngIn, Enumerable.Select<T, string>(splttrs, typeof(T).GetMethod("ToString") as Func<T, string>), StringSplitOptions.RemoveEmptyEntries);
        }

        [Extension]
        public static string[] Splyt(string arg, string spltr)
        {
            return Splyt(arg, spltr, StringSplitOptions.RemoveEmptyEntries);
        }

        [Extension]
        public static string[] Splyt(string arg, params char[] cc)
        {
            return arg.Split(cc, StringSplitOptions.RemoveEmptyEntries);
        }

        [Extension]
        public static string[] Splyt(string strngIn, IEnumerable<string> splttrs, StringSplitOptions optn)
        {
            return strngIn.Split(Enumerable.ToArray<string>(splttrs ?? Splytrs(true, true, true, true)), optn);
        }

        [Extension]
        public static string[] Splyt(string arg, string spltr, StringSplitOptions so)
        {
            string[] args = new string[] { spltr };
            return Splyt(arg, so, args);
        }

        [Extension]
        private static string[] Splyt(string arg, StringSplitOptions so, params string[] args)
        {
            if (FwNs.Core.Xtnz.IsNotNullOrEmpty<string[]>(args))
            {
                if (!FwNs.Core.Xtnz.IsMltple<string[]>(args) && (1 >= Enumerable.Count<string>(args)))
                {
                    return Splyt(arg, args[0], so);
                }
                return arg.Split(args, so);
            }
            try
            {
                return arg.Split(new string[0], so);
            }
            catch (Exception)
            {
                return Splyt(arg, "", so);
            }
        }

        [Extension]
        public static string[] Splyt(string S, char[] C, StringSplitOptions so)
        {
            return S.Split(C, so);
        }

        [Extension]
        public static string[] Splytrs(bool onComma44)
        {
            return Splytrs(onComma44, false, false, false);
        }

        [Extension]
        public static string[] Splytrs(IEnumerable<int> splttrs)
        {
            Func<int, bool> predicate = <>c.<>9__32_0 ?? (<>c.<>9__32_0 = new Func<int, bool>(<>c.<>9, this.<Splytrs>b__32_0));
            return Enumerable.ToArray<string>(Enumerable.Select<int, string>(Enumerable.Where<int>(splttrs, predicate), <>c.<>9__32_1 ?? (<>c.<>9__32_1 = new Func<int, string>(<>c.<>9, this.<Splytrs>b__32_1))));
        }

        [Extension]
        public static string[] Splytrs(bool onComma044, bool onTab009, bool onCr013, bool onLf010)
        {
            object[] source = new object[,] { { 0x2c, onComma044 }, { 9, onTab009 }, { 13, onCr013 }, { 10, onLf010 } };
            return Splytrs(Enumerable.Select(Enumerable.Select(Enumerable.Select(Enumerable.Cast<object[]>(source), <>c.<>9__31_0 ?? (<>c.<>9__31_0 = new Func<object[], <>f__AnonymousType20<object[], KeyValuePair<int, bool>>>(<>c.<>9, this.<Splytrs>b__31_0))), <>c.<>9__31_1 ?? (<>c.<>9__31_1 = new Func<<>f__AnonymousType20<object[], KeyValuePair<int, bool>>, <>f__AnonymousType21<<>f__AnonymousType20<object[], KeyValuePair<int, bool>>, int>>(<>c.<>9, this.<Splytrs>b__31_1))), <>c.<>9__31_2 ?? (<>c.<>9__31_2 = new Func<<>f__AnonymousType21<<>f__AnonymousType20<object[], KeyValuePair<int, bool>>, int>, int>(<>c.<>9, this.<Splytrs>b__31_2))));
        }

        [Extension]
        public static string SubStrRgt(string strngIn, string aftr)
        {
            return null;
        }

        public static Func<string, string, string, bool> FuncCriteriaText
        {
            get
            {
                return (<>c.<>9__24_0 ?? (<>c.<>9__24_0 = new Func<string, string, string, bool>(<>c.<>9, this.<get_FuncCriteriaText>b__24_0)));
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FwNs.Txt.Xtnz.<>c <>9 = new FwNs.Txt.Xtnz.<>c();
            public static Func<string, string, string, string, string, string, string> <>9__1_0;
            public static Func<string, string, string, bool> <>9__24_0;
            public static Func<string, string, string, string, char, string> <>9__28_0;
            public static Func<object[], <>f__AnonymousType20<object[], KeyValuePair<int, bool>>> <>9__31_0;
            public static Func<<>f__AnonymousType20<object[], KeyValuePair<int, bool>>, <>f__AnonymousType21<<>f__AnonymousType20<object[], KeyValuePair<int, bool>>, int>> <>9__31_1;
            public static Func<<>f__AnonymousType21<<>f__AnonymousType20<object[], KeyValuePair<int, bool>>, int>, int> <>9__31_2;
            public static Func<int, bool> <>9__32_0;
            public static Func<int, string> <>9__32_1;

            internal string <FrmtType>b__28_0(string dnmx, string nmx, string ndxx, string x10fxx, char dscx)
            {
                return string.Format("{0}{4}{1}{2}.{3}", new object[] { dnmx, nmx, ndxx, x10fxx, dscx });
            }

            internal string <FullTypNmFunc>b__1_0(string typNm, string nmspc, string assmbly, string vrsn, string cltr, string kyTkn)
            {
                throw new Exception();
            }

            internal bool <get_FuncCriteriaText>b__24_0(string enm, string test4, string testOn)
            {
                bool flag = enm.StartsWith("Not");
                string str = test4;
                object obj2 = testOn;
                Type type = typeof(string);
                string name = flag ? enm.Substring(3) : enm;
                if (enm.Contains("Regex"))
                {
                    type = typeof(Regex);
                    name = name.Substring("Regex".Length);
                    obj2 = new Regex(test4);
                    str = testOn;
                }
                Type[] types = new Type[] { typeof(string) };
                string[] parameters = new string[] { str };
                bool flag2 = (bool) type.GetMethod(name, types).Invoke(obj2, parameters);
                if (flag)
                {
                    return !flag2;
                }
                return flag2;
            }

            internal <>f__AnonymousType20<object[], KeyValuePair<int, bool>> <Splytrs>b__31_0(object[] ov)
            {
                return new { 
                    ov = ov,
                    kv = new KeyValuePair<int, bool>((int) ov[0], (bool) ov[1])
                };
            }

            internal <>f__AnonymousType21<<>f__AnonymousType20<object[], KeyValuePair<int, bool>>, int> <Splytrs>b__31_1(<>f__AnonymousType20<object[], KeyValuePair<int, bool>> <>h__TransparentIdentifier0)
            {
                return new { 
                    <>h__TransparentIdentifier0 = <>h__TransparentIdentifier0,
                    ndx = <>h__TransparentIdentifier0.kv.Value ? <>h__TransparentIdentifier0.kv.Key : -1
                };
            }

            internal int <Splytrs>b__31_2(<>f__AnonymousType21<<>f__AnonymousType20<object[], KeyValuePair<int, bool>>, int> <>h__TransparentIdentifier1)
            {
                return <>h__TransparentIdentifier1.ndx;
            }

            internal bool <Splytrs>b__32_0(int ii)
            {
                return (0 < ii);
            }

            internal string <Splytrs>b__32_1(int splttr)
            {
                return string.Format("{0}", (char) splttr);
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__18<T> where T: IEnumerable<object>
        {
            public static readonly FwNs.Txt.Xtnz.<>c__18<T> <>9;
            public static Func<object, string> <>9__18_0;

            static <>c__18()
            {
                FwNs.Txt.Xtnz.<>c__18<T>.<>9 = new FwNs.Txt.Xtnz.<>c__18<T>();
            }

            internal string <Joyn>b__18_0(object oo)
            {
                return oo.ToString();
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__20<T> where T: IEnumerable<object>
        {
            public static readonly FwNs.Txt.Xtnz.<>c__20<T> <>9;
            public static Func<object, string> <>9__20_0;

            static <>c__20()
            {
                FwNs.Txt.Xtnz.<>c__20<T>.<>9 = new FwNs.Txt.Xtnz.<>c__20<T>();
            }

            internal string <Joyn>b__20_0(object oo)
            {
                return oo.ToString();
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__21<TA, TL> where TA: IEnumerable<TL>
        {
            public static readonly FwNs.Txt.Xtnz.<>c__21<TA, TL> <>9;
            public static Func<TL, string> <>9__21_0;

            static <>c__21()
            {
                FwNs.Txt.Xtnz.<>c__21<TA, TL>.<>9 = new FwNs.Txt.Xtnz.<>c__21<TA, TL>();
            }

            internal string <Joyn>b__21_0(TL tl)
            {
                return null;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__22<TA, TL> where TA: IEnumerable<TL>
        {
            public static readonly FwNs.Txt.Xtnz.<>c__22<TA, TL> <>9;
            public static Func<TL, string> <>9__22_0;
            public static Func<TL, string> <>9__22_1;

            static <>c__22()
            {
                FwNs.Txt.Xtnz.<>c__22<TA, TL>.<>9 = new FwNs.Txt.Xtnz.<>c__22<TA, TL>();
            }

            internal string <Joyn>b__22_0(TL oo)
            {
                return oo.ToString();
            }

            internal string <Joyn>b__22_1(TL oo)
            {
                return oo.ToString();
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__26<TA, T> where TA: IEnumerable<T>
        {
            public static readonly FwNs.Txt.Xtnz.<>c__26<TA, T> <>9;
            public static Func<T, string> <>9__26_0;

            static <>c__26()
            {
                FwNs.Txt.Xtnz.<>c__26<TA, T>.<>9 = new FwNs.Txt.Xtnz.<>c__26<TA, T>();
            }

            internal string <FrmtXprsn>b__26_0(T tt)
            {
                return tt.ToString();
            }
        }
    }
}

