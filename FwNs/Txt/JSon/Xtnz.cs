namespace FwNs.Txt.JSon
{
    using fastJSON;
    using FwNs;
    using FwNs.Core;
    using FwNs.Core.Typs;
    using FwNs.Linq.Xprsns;
    using FwNs.Txt;
    using JsonFx.Json;
    using Microsoft.CSharp.RuntimeBinder;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    [Extension]
    public static class Xtnz
    {
        public static Func<Type, Func<object, string>> ObjJp2sFuncFunc;
        public static Func<object, JSONParameters, string> ObjJp2sFunc;
        public const string DblQtT = "\"\"{0}\"\"";

        static Xtnz()
        {
            ObjJp2sFuncFunc = new Func<Type, Func<object, string>>(<>c.<>9, this.<.cctor>b__28_0);
            ObjJp2sFunc = new Func<object, JSONParameters, string>(<>c.<>9, this.<.cctor>b__28_14);
        }

        [Extension]
        public static Func<object, string> GetFastJsonFunc(Type type)
        {
            <>c__DisplayClass1_1 class_;
            if (TypsFw.IsAnonymous(type))
            {
                return (<>c.<>9__1_0 ?? (<>c.<>9__1_0 = new Func<object, string>(<>c.<>9, this.<GetFastJsonFunc>b__1_0)));
            }
            TypeCode typCode = FwNs.Core.Xtnz.GetTypCode(type);
            Func<object, JSONParameters, string> func = null;
            if (FwNs.Core.Xtnz.IsNotNull<Func<object, JSONParameters, string>>(func))
            {
                <>c__DisplayClass1_0 class_2;
                return new Func<object, string>(class_2, this.<GetFastJsonFunc>b__1);
            }
            Func<object, JSONParameters, string> ojp2sf = new Func<object, JSONParameters, string>(class_, this.<GetFastJsonFunc>b__2);
            Func<object, string> func = new Func<object, string>(class_, this.<GetFastJsonFunc>b__3);
            if ((typCode != TypeCode.Empty) && (TypeCode.Object != typCode))
            {
                throw new Exception();
            }
            return func;
        }

        public static Func<TypeCode, Func<object, JSONParameters, string>> GetFastJsonFuncFunc()
        {
            return (<>c.<>9__4_0 ?? (<>c.<>9__4_0 = new Func<TypeCode, Func<object, JSONParameters, string>>(<>c.<>9, this.<GetFastJsonFuncFunc>b__4_0)));
        }

        public static JSONParameters jSonPrmtrs(bool annymsTyps, bool readOnlyProps, bool xtns)
        {
            return new JSONParameters { 
                EnableAnonymousTypes = annymsTyps,
                IgnoreCaseOnDeserialize = false,
                SerializeNullValues = true,
                ShowReadOnlyProperties = readOnlyProps,
                UseExtensions = xtns,
                UseFastGuid = true,
                UseOptimizedDatasetSchema = true,
                UseUTCDateTime = false,
                UsingGlobalTypes = false,
                UseEscapedUnicode = false
            };
        }

        public static string PrepJSonTxt(string txt)
        {
            string str = '"'.ToString();
            txt = Regex.Replace(txt, "([^" + str + "]|" + str + @",)(\\" + str + ")([^" + str + "])", "${1}" + str + "${3}");
            txt = Regex.Replace(txt, "([^" + str + "]|" + str + @",)(\\" + str + ")([^" + str + "])", "${1}" + str + "${3}");
            txt = Regex.Replace(txt, @"\s+", "");
            return txt;
        }

        [Extension]
        public static string ToFastJson(Array ary)
        {
            if (ary is string[])
            {
                return ToFastJson<string>((string[]) ary);
            }
            if (ary is object[])
            {
                return ToFastJson((object[]) ary);
            }
            return ToFastJson<List<string>>(Enumerable.ToList<string>(Enumerable.Select<object, string>(Sequence.SmartCast<object>(ary), new Func<object, string>(null, ToFastJson<object>))));
        }

        [Extension]
        public static string ToFastJson(ICollection objs)
        {
            throw new Exception();
        }

        [Extension]
        public static string ToFastJson(IDictionary idyct)
        {
            Dictionary<string, string> dyct = null;
            dyct = new Dictionary<string, string>();
            foreach (object obj2 in idyct.Keys)
            {
                string key = ToFastJson<object>(obj2);
                string str2 = ToFastJson<object>(idyct[obj2]);
                dyct.Add(key, str2);
            }
            return ToFastJson<string, string>(dyct);
        }

        [Extension]
        public static string ToFastJson(IEnumerable enmrbl)
        {
            return ToFastJson(enmrbl, jSonPrmtrs(true, true, false));
        }

        [Extension]
        public static string ToFastJson(Expression xprsn)
        {
            if (xprsn.NodeType == ExpressionType.Lambda)
            {
                LambdaExpression expression = (LambdaExpression) xprsn;
                IEnumerable<string> t = Enumerable.Select<Expression, string>(Enumerable.Cast<Expression>(expression.Parameters), new Func<Expression, string>(null, ToFastJson));
                return ToFastJson(new { 
                    Name = expression.Name,
                    NodeType = expression.NodeType,
                    prmtrs = ToFastJson<IEnumerable<string>>(t),
                    type = ToFastJson(expression.Type),
                    body = expression.Body.ToString(),
                    typeReturn = ToFastJson(expression.ReturnType)
                });
            }
            if (xprsn is MethodCallExpression)
            {
                MethodCallExpression expression2 = (MethodCallExpression) xprsn;
                return ToFastJson(new { 
                    NodeType = xprsn.NodeType,
                    type = ToFastJson(xprsn.Type),
                    mthd = expression2.Method.Name
                });
            }
            if (xprsn is ParameterExpression)
            {
                ParameterExpression expression3 = (ParameterExpression) xprsn;
                return ToFastJson(new { 
                    Name = expression3.Name,
                    type = ToFastJson(expression3.Type)
                });
            }
            return ToFastJson(new { 
                NodeType = xprsn.NodeType,
                strng = xprsn.ToString(),
                type = ToFastJson(xprsn.Type)
            });
        }

        [Extension]
        public static string ToFastJson(Type typ)
        {
            return ToFastJson(new { nm = TypsFw.GetTypNm(typ) });
        }

        [Extension]
        public static string ToFastJson(object[] objs)
        {
            Dictionary<int, string> source = new Dictionary<int, string>();
            object[] objArray = objs;
            for (int i = 0; i < objArray.Length; i++)
            {
                string str = ToFastJson<object>(objArray[i]);
                source.Add(source.Count, str);
            }
            return ToFastJson<KeyValuePair<int, string>>(Enumerable.ToArray<KeyValuePair<int, string>>(Enumerable.Select<KeyValuePair<int, string>, KeyValuePair<int, string>>(source, <>c.<>9__21_0 ?? (<>c.<>9__21_0 = new Func<KeyValuePair<int, string>, KeyValuePair<int, string>>(<>c.<>9, this.<ToFastJson>b__21_0)))));
        }

        [Extension]
        public static string ToFastJson<T>(T t)
        {
            return ToFastJsonFunc<T>().Invoke(t);
        }

        [Extension]
        public static string ToFastJson<TK, TV>(Dictionary<TK, TV> dyct)
        {
            if (TypsFw.IsTypeOrDerivedFrom<IEnumerable>(typeof(TV)))
            {
                TypsFw.IsTypeOrDerivedFrom<IEnumerable>(typeof(TV));
            }
            return ToFastJson<TK, TV>((IDictionary<TK, TV>) dyct);
        }

        [Extension]
        public static string ToFastJson<TK, TV>(IDictionary<TK, TV> idyct)
        {
            <>c__DisplayClass10_0<TK, TV> class_;
            Func<TK, string> elementSelector = new Func<TK, string>(class_, this.<ToFastJson>b__0);
            if (!TypsFw.IsComparable(typeof(TK)))
            {
                return ToFastJson<int, string>(Enumerable.ToDictionary<TK, int, string>(idyct.Keys, new Func<TK, int>(class_, this.<ToFastJson>b__2), elementSelector));
            }
            if ((TypsFw.IsComparable(typeof(TV)) || typeof(TV).IsValueType) || (typeof(TV).IsEnum || (TypeCode.Empty < FwNs.Core.Xtnz.GetTypCode(typeof(TV)))))
            {
                return ObjJp2sFunc.Invoke(idyct, null);
            }
            return ToFastJson<IComparable, string>(Enumerable.ToDictionary<TK, IComparable, string>(idyct.Keys, <>c__10<TK, TV>.<>9__10_1 ?? (<>c__10<TK, TV>.<>9__10_1 = new Func<TK, IComparable>(<>c__10<TK, TV>.<>9, this.<ToFastJson>b__10_1)), elementSelector));
        }

        [Extension]
        public static string ToFastJson<T>(T[] ta)
        {
            <>c__DisplayClass18_0<T> class_;
            if (typeof(T) != typeof(object))
            {
                if (typeof(T) == typeof(string))
                {
                    return FwNs.Txt.Xtnz.Joyn<IEnumerable<string>>(Enumerable.Cast<string>(ta), ",");
                }
                if (typeof(IKeyVal<T>).IsAssignableFrom(typeof(T).BaseType))
                {
                    ParameterExpression expression;
                    Expression[] arguments = new Expression[2];
                    Expression[] expressionArray2 = new Expression[] { Expression.Call(expression = Expression.Parameter(typeof(object), "vx"), (MethodInfo) methodof(object.GetType), new Expression[0]) };
                    arguments[0] = Expression.Call(null, (MethodInfo) methodof(FwNs.Txt.JSon.Xtnz.ToFastJson), expressionArray2);
                    arguments[1] = expression;
                    MemberInfo[] members = new MemberInfo[] { (MethodInfo) methodof(<>f__AnonymousType9<string, object>.get_typx, <>f__AnonymousType9<string, object>), (MethodInfo) methodof(<>f__AnonymousType9<string, object>.get_valx, <>f__AnonymousType9<string, object>) };
                    ParameterExpression[] parameters = new ParameterExpression[] { expression };
                    Expression.Lambda<Func<object, object>>(Expression.New((ConstructorInfo) methodof(<>f__AnonymousType9<string, object>..ctor, <>f__AnonymousType9<string, object>), arguments, members), parameters);
                    throw new Exception();
                }
                throw new Exception();
            }
            return ToFastJson<int, string>(Enumerable.ToDictionary<T, int, string>(ta, new Func<T, int>(class_, this.<ToFastJson>b__0), new Func<T, string>(class_, this.<ToFastJson>b__1)));
        }

        [Extension]
        public static string ToFastJson(IEnumerable enmrbl, JSONParameters jp)
        {
            Type[] types;
            <>c__DisplayClass11_0 class_;
            int num2;
            MethodInfo mthd = null;
            Type type = enmrbl.GetType();
            Func<object, object> o2of = <>c.<>9__11_0 ?? (<>c.<>9__11_0 = new Func<object, object>(<>c.<>9, this.<ToFastJson>b__11_0));
            if (!type.IsGenericType && !Enumerable.Any<Type>(type.GetGenericArguments()))
            {
                if (!type.IsGenericType)
                {
                    types = new Type[] { typeof(ICollection), typeof(Array) };
                }
                else
                {
                    types = new Type[0];
                }
            }
            else if (Enumerable.Count<Type>(type.GetGenericArguments()) == 1)
            {
                types = new Type[] { typeof(ICollection<>), typeof(IEnumerable<>), typeof(IQueryable<>), typeof(object[]), typeof(List<>) };
            }
            else if (Enumerable.Count<Type>(type.GetGenericArguments()) == 2)
            {
                types = new Type[] { typeof(IDictionary<,>), typeof(IGrouping<,>), typeof(object[,]) };
            }
            else
            {
                Enumerable.Count<Type>(type.GetGenericArguments());
                throw new Exception();
            }
            if (Enumerable.Any<Type>(types, new Func<Type, bool>(class_, this.<ToFastJson>b__1)))
            {
                Type[] typeArray4 = new Type[] { type };
                mthd = typeof(FwNs.Txt.JSon.Xtnz).GetMethod("ToFastJson", typeArray4);
                IEnumerable[] parameters = new IEnumerable[] { enmrbl };
                return (mthd.Invoke(null, parameters) as string);
            }
            for (int ndx = 0; ndx < Enumerable.Count<Type>(types); ndx = num2 + 1)
            {
                Type typAncstr = null;
                bool flag = false;
                if (Enumerable.Any<Type>(type.GetGenericArguments()) && types[ndx].IsGenericType)
                {
                    typAncstr = types[ndx].MakeGenericType(type.GetGenericArguments());
                    flag = TypsFw.IsTypeOrDerivedFrom(type, typAncstr);
                }
                if (flag || TypsFw.IsTypeOrDerivedFrom(type, types[ndx]))
                {
                    <>c__DisplayClass11_1 class_2;
                    Func<object, string> <>9__3;
                    Type[] typeArray5 = new Type[] { typAncstr ?? types[ndx] };
                    mthd = typeof(FwNs.Txt.JSon.Xtnz).GetMethod("ToFastJson", typeArray5);
                    if (types[ndx].IsEquivalentTo(type))
                    {
                        Func<object, object> <>9__2;
                        o2of = <>9__2 ?? (<>9__2 = new Func<object, object>(class_2, this.<ToFastJson>b__2));
                    }
                    return (<>9__3 ?? (<>9__3 = new Func<object, string>(class_2, this.<ToFastJson>b__3))).Invoke(enmrbl);
                }
                num2 = ndx;
            }
            throw new Exception();
        }

        [Extension]
        public static string ToFastJson(ValueType vt, JSONParameters jp)
        {
            <>c__DisplayClass16_0 class_;
            Func<ValueType, string> func = null;
            if (vt.GetType().GetGenericTypeDefinition() != typeof(KeyValuePair<,>))
            {
                throw new Exception();
            }
            func = new Func<ValueType, string>(class_, this.<ToFastJson>b__0);
            return func.Invoke(vt);
        }

        [Extension]
        public static string ToFastJson<T, TV>(T enmrbl, JSONParameters jp) where T: IEnumerable<TV>
        {
            return ObjJp2sFuncFunc.Invoke(typeof(T)).Invoke(enmrbl);
        }

        [Extension]
        public static string ToFastJson<T>(T t, bool annymsTyps, bool readOnlyProps, bool xtns)
        {
            JSONParameters parameters = new JSONParameters {
                EnableAnonymousTypes = annymsTyps,
                IgnoreCaseOnDeserialize = false,
                SerializeNullValues = true,
                ShowReadOnlyProperties = readOnlyProps,
                UseExtensions = xtns,
                UseFastGuid = true,
                UseOptimizedDatasetSchema = true,
                UseUTCDateTime = false,
                UsingGlobalTypes = false,
                UseEscapedUnicode = false
            };
            return ObjJp2sFunc.Invoke(typeof(T), parameters);
        }

        public static string ToFastJsonDynmk([Dynamic] object dynmk)
        {
            throw new Exception();
        }

        [Extension]
        public static string ToFastJsonDynmk(string dynmkStrng)
        {
            if (<>o__14.<>p__1 == null)
            {
                <>o__14.<>p__1 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(FwNs.Txt.JSon.Xtnz)));
            }
            if (<>o__14.<>p__0 == null)
            {
                CSharpArgumentInfo[] argumentInfo = new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.IsStaticType | CSharpArgumentInfoFlags.UseCompileTimeType, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };
                <>o__14.<>p__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToFastJsonDynmk", null, typeof(FwNs.Txt.JSon.Xtnz), argumentInfo));
            }
            return <>o__14.<>p__1.Target.Invoke(<>o__14.<>p__1, <>o__14.<>p__0.Target.Invoke(<>o__14.<>p__0, typeof(FwNs.Txt.JSon.Xtnz), JSON.Instance.ToDynamic(dynmkStrng)));
        }

        public static Func<T, string> ToFastJsonFunc<T>()
        {
            return (<>c__5<T>.<>9__5_0 ?? (<>c__5<T>.<>9__5_0 = new Func<T, string>(<>c__5<T>.<>9, this.<ToFastJsonFunc>b__5_0)));
        }

        [Extension]
        public static string ToFastJsonT<T>(T tt, JSONParameters jp)
        {
            <>c__DisplayClass15_0<T> class_;
            Func<T, string> func = null;
            Func<object, JSONParameters, string> oJp2sf = <>c__15<T>.<>9__15_0 ?? (<>c__15<T>.<>9__15_0 = new Func<object, JSONParameters, string>(<>c__15<T>.<>9, this.<ToFastJsonT>b__15_0));
            if (typeof(T).IsArray)
            {
                if (typeof(T) == typeof(string[]))
                {
                    func = new Func<T, string>(class_, this.<ToFastJsonT>b__1);
                    func = new Func<T, string>(class_, this.<ToFastJsonT>b__2);
                }
                else
                {
                    func = new Func<T, string>(class_, this.<ToFastJsonT>b__3);
                    func = new Func<T, string>(class_, this.<ToFastJsonT>b__4);
                }
            }
            else if (typeof(T).IsValueType)
            {
                func = new Func<T, string>(class_, this.<ToFastJsonT>b__5);
            }
            else if (TypsFw.IsTypeOrDerivedFrom<IKeyVal>(typeof(T)))
            {
                func = new Func<T, string>(class_, this.<ToFastJsonT>b__6);
            }
            else
            {
                if (typeof(T) != typeof(object))
                {
                    throw new Exception();
                }
                if (tt is Type)
                {
                    return ToFastJson((Type) tt);
                }
                if (tt is Expression)
                {
                    return ToFastJson((Expression) tt);
                }
                if (!(tt is IEnumerable))
                {
                    throw new Exception();
                }
                return ToFastJson((IEnumerable) tt);
            }
            return func.Invoke(tt);
        }

        [Extension]
        public static string ToJson(object obj)
        {
            return new JsonFx.Json.JsonWriter().Write(obj);
        }

        [Extension]
        public static string ToJsonStr(object arg)
        {
            string str = null;
            if (arg == null)
            {
                throw new Exception();
            }
            StringBuilder sb = new StringBuilder();
            StringWriter textWriter = new StringWriter(sb);
            new JsonSerializer().Serialize(textWriter, arg);
            textWriter.Flush();
            textWriter.Dispose();
            str = sb.ToString();
            if (str == null)
            {
                str = arg.ToString();
            }
            return str;
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FwNs.Txt.JSon.Xtnz.<>c <>9 = new FwNs.Txt.JSon.Xtnz.<>c();
            public static Func<object, string> <>9__1_0;
            public static Func<object, JSONParameters, string> <>9__4_1;
            public static Func<object, JSONParameters, string> <>9__4_2;
            public static Func<object, JSONParameters, string> <>9__4_3;
            public static Func<object, JSONParameters, string> <>9__4_4;
            public static Func<object, JSONParameters, string> <>9__4_5;
            public static Func<object, JSONParameters, string> <>9__4_6;
            public static Func<object, JSONParameters, string> <>9__4_7;
            public static Func<object, JSONParameters, string> <>9__4_8;
            public static Func<TypeCode, Func<object, JSONParameters, string>> <>9__4_0;
            public static Func<object, object> <>9__11_0;
            public static Func<KeyValuePair<int, string>, KeyValuePair<int, string>> <>9__21_0;
            public static Func<object, object> <>9__28_1;
            public static Func<object, string> <>9__28_4;
            public static Func<object, string> <>9__28_13;

            internal Func<object, string> <.cctor>b__28_0(Type type)
            {
                FwNs.Txt.JSon.Xtnz.<>c__DisplayClass28_0 class_;
                Type[] types = new Type[] { type };
                MethodInfo mthd = null;
                Func<object, object> o2of = <>9__28_1 ?? (<>9__28_1 = new Func<object, object>(<>9, this.<.cctor>b__28_1));
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    types[0] = typeof(IEnumerable);
                    if (TypsFw.IsTypeOrDerivedFrom<IDictionary>(type))
                    {
                        types[0] = typeof(IDictionary);
                        if (type.IsGenericType)
                        {
                            if (!Enumerable.Any<Type>(type.GetGenericArguments()))
                            {
                                throw new Exception();
                            }
                            types[0] = typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
                            if (FwNs.Core.Xtnz.IsNull<MethodInfo>(typeof(FwNs.Txt.JSon.Xtnz).GetMethod("ToFastJson", types)))
                            {
                                throw new Exception();
                            }
                        }
                    }
                    else if ((type.IsGenericType && Enumerable.Any<Type>(type.GetGenericArguments())) && (Enumerable.Count<Type>(type.GetGenericArguments()) == 1))
                    {
                        types[0] = typeof(IEnumerable<>).MakeGenericType(type.GetGenericArguments());
                        o2of = new Func<object, object>(class_, this.<.cctor>b__2);
                    }
                    mthd = typeof(FwNs.Txt.JSon.Xtnz).GetMethod("ToFastJson", types);
                    return new Func<object, string>(class_, this.<.cctor>b__3);
                }
                if (type.IsGenericType || Enumerable.Any<Type>(type.GetGenericArguments()))
                {
                    if (Enumerable.Count<Type>(type.GetGenericArguments()) >= 3)
                    {
                        int count = Enumerable.Count<Type>(type.GetGenericArguments());
                        Type[] typeArray4 = new Type[] { typeof(object) };
                        Enumerable.ToArray<Type>(Enumerable.Repeat<Type>(typeof(object), count));
                        throw new Exception();
                    }
                    if (!Enumerable.Any<Type>(type.GetGenericArguments()))
                    {
                        throw new Exception();
                    }
                    if (Enumerable.Count<Type>(type.GetGenericArguments()) != 2)
                    {
                        if (Enumerable.Count<Type>(type.GetGenericArguments()) != 1)
                        {
                            throw new Exception();
                        }
                        types = new Type[] { typeof(IComparable<>), typeof(Expression<>), typeof(IKeyVal<>), typeof(Action<>), typeof(Func<>), typeof(Dlgt<>), typeof(Type) };
                    }
                    else
                    {
                        types = new Type[] { typeof(KeyValuePair<,>), typeof(IKeyVal<,>), typeof(Func<,>), typeof(IKv<,>), typeof(Dlgt<,>) };
                    }
                }
                else
                {
                    int num2;
                    FwNs.Txt.JSon.Xtnz.<>c__DisplayClass28_0 CS$<>8__locals1;
                    if ((TypsFw.IsComparable(type) || type.IsEnum) || (TypeCode.Empty < FwNs.Core.Xtnz.GetTypCode(type)))
                    {
                        return (<>9__28_4 ?? (<>9__28_4 = new Func<object, string>(<>9, this.<.cctor>b__28_4)));
                    }
                    types = new Type[] { typeof(IComparable), typeof(Expression), typeof(ValueType), typeof(IKeyVal), typeof(Enum), typeof(Type) };
                    if (Enumerable.Any<Type>(types, new Func<Type, bool>(class_, this.<.cctor>b__5)))
                    {
                        Type[] typeArray6 = new Type[] { type };
                        mthd = typeof(FwNs.Txt.JSon.Xtnz).GetMethod("ToFastJson", typeArray6);
                        return new Func<object, string>(class_, this.<.cctor>b__6);
                    }
                    types = new Type[] { typeof(IComparable), typeof(Expression), typeof(ValueType), typeof(IKeyVal), typeof(Enum), typeof(Type) };
                    if (Enumerable.Any<Type>(types, new Func<Type, bool>(class_, this.<.cctor>b__7)))
                    {
                        Type[] typeArray8 = new Type[] { type };
                        mthd = typeof(FwNs.Txt.JSon.Xtnz).GetMethod("ToFastJson", typeArray8);
                        return new Func<object, string>(class_, this.<.cctor>b__8);
                    }
                    for (int ndx = 0; ndx < Enumerable.Count<Type>(types); ndx = num2 + 1)
                    {
                        if (TypsFw.IsTypeOrDerivedFrom(type, types[ndx]))
                        {
                            Type[] typeArray10;
                            Type[] typeArray9 = new Type[] { type };
                            mthd = typeof(FwNs.Txt.JSon.Xtnz).GetMethod("ToFastJson", typeArray9);
                            mthd = new Type[] { types[ndx] } ?? typeof(FwNs.Txt.JSon.Xtnz).GetMethod("ToFastJson", typeArray10);
                            if (FwNs.Core.Xtnz.IsNotNull<MethodInfo>(mthd))
                            {
                                Func<object, string> <>9__12;
                                if (!types[ndx].IsEquivalentTo(type))
                                {
                                    FwNs.Txt.JSon.Xtnz.<>c__DisplayClass28_1 class_2;
                                    FwNs.Txt.JSon.Xtnz.<>c__DisplayClass28_2 class_3;
                                    Func<object, object> <>9__10;
                                    Func<object, object> <>9__9;
                                    Func<object, object> o2of2 = <>9__9 ?? (<>9__9 = new Func<object, object>(class_2, this.<.cctor>b__9));
                                    o2of = <>9__10 ?? (<>9__10 = new Func<object, object>(class_2, this.<.cctor>b__10));
                                    o2of = new Func<object, object>(class_3, this.<.cctor>b__11);
                                }
                                return (<>9__12 ?? (<>9__12 = new Func<object, string>(CS$<>8__locals1, this.<.cctor>b__12)));
                            }
                        }
                        num2 = ndx;
                    }
                }
                return (<>9__28_13 ?? (<>9__28_13 = new Func<object, string>(<>9, this.<.cctor>b__28_13)));
            }

            internal object <.cctor>b__28_1(object ox)
            {
                return ox;
            }

            internal string <.cctor>b__28_13(object ox)
            {
                return FwNs.Txt.JSon.Xtnz.ObjJp2sFunc.Invoke(ox, null);
            }

            internal string <.cctor>b__28_14(object ox, JSONParameters jpx)
            {
                return JSON.Instance.ToJSON(ox, jpx ?? FwNs.Txt.JSon.Xtnz.jSonPrmtrs(true, true, false));
            }

            internal string <.cctor>b__28_4(object ox)
            {
                return ox.ToString();
            }

            internal string <GetFastJsonFunc>b__1_0(object ox)
            {
                return JSON.Instance.ToJSON(ox, FwNs.Txt.JSon.Xtnz.jSonPrmtrs(true, true, false));
            }

            internal Func<object, JSONParameters, string> <GetFastJsonFuncFunc>b__4_0(TypeCode typeCode)
            {
                switch (typeCode)
                {
                    case TypeCode.DBNull:
                        throw new Exception();

                    case TypeCode.Boolean:
                        return (<>9__4_3 ?? (<>9__4_3 = new Func<object, JSONParameters, string>(<>9, this.<GetFastJsonFuncFunc>b__4_3)));

                    case TypeCode.Char:
                        return (<>9__4_1 ?? (<>9__4_1 = new Func<object, JSONParameters, string>(<>9, this.<GetFastJsonFuncFunc>b__4_1)));

                    case TypeCode.SByte:
                        return FwNs.Txt.JSon.Xtnz.GetFastJsonFuncFunc().Invoke(TypeCode.Byte);

                    case TypeCode.Byte:
                        return (<>9__4_2 ?? (<>9__4_2 = new Func<object, JSONParameters, string>(<>9, this.<GetFastJsonFuncFunc>b__4_2)));

                    case TypeCode.Int16:
                        return FwNs.Txt.JSon.Xtnz.GetFastJsonFuncFunc().Invoke(TypeCode.Int32);

                    case TypeCode.UInt16:
                        return FwNs.Txt.JSon.Xtnz.GetFastJsonFuncFunc().Invoke(TypeCode.UInt32);

                    case TypeCode.Int32:
                        return (<>9__4_4 ?? (<>9__4_4 = new Func<object, JSONParameters, string>(<>9, this.<GetFastJsonFuncFunc>b__4_4)));

                    case TypeCode.UInt32:
                        return FwNs.Txt.JSon.Xtnz.GetFastJsonFuncFunc().Invoke(TypeCode.Int32);

                    case TypeCode.Int64:
                        return FwNs.Txt.JSon.Xtnz.GetFastJsonFuncFunc().Invoke(TypeCode.Int32);

                    case TypeCode.UInt64:
                        return FwNs.Txt.JSon.Xtnz.GetFastJsonFuncFunc().Invoke(TypeCode.UInt32);

                    case TypeCode.Single:
                        return (<>9__4_5 ?? (<>9__4_5 = new Func<object, JSONParameters, string>(<>9, this.<GetFastJsonFuncFunc>b__4_5)));

                    case TypeCode.Double:
                        return (<>9__4_7 ?? (<>9__4_7 = new Func<object, JSONParameters, string>(<>9, this.<GetFastJsonFuncFunc>b__4_7)));

                    case TypeCode.Decimal:
                        return (<>9__4_6 ?? (<>9__4_6 = new Func<object, JSONParameters, string>(<>9, this.<GetFastJsonFuncFunc>b__4_6)));

                    case TypeCode.DateTime:
                        return FwNs.Txt.JSon.Xtnz.ObjJp2sFunc;

                    case TypeCode.String:
                        return (<>9__4_8 ?? (<>9__4_8 = new Func<object, JSONParameters, string>(<>9, this.<GetFastJsonFuncFunc>b__4_8)));
                }
                return null;
            }

            internal string <GetFastJsonFuncFunc>b__4_1(object ox, JSONParameters jpx)
            {
                return ox.ToString();
            }

            internal string <GetFastJsonFuncFunc>b__4_2(object ox, JSONParameters jpx)
            {
                return ox.ToString();
            }

            internal string <GetFastJsonFuncFunc>b__4_3(object ox, JSONParameters jpx)
            {
                bool flag = (bool) ox;
                return flag.ToString();
            }

            internal string <GetFastJsonFuncFunc>b__4_4(object ox, JSONParameters jpx)
            {
                int num = (int) ox;
                return num.ToString();
            }

            internal string <GetFastJsonFuncFunc>b__4_5(object ox, JSONParameters jpx)
            {
                return ox.ToString();
            }

            internal string <GetFastJsonFuncFunc>b__4_6(object ox, JSONParameters jpx)
            {
                return ox.ToString();
            }

            internal string <GetFastJsonFuncFunc>b__4_7(object ox, JSONParameters jpx)
            {
                return ox.ToString();
            }

            internal string <GetFastJsonFuncFunc>b__4_8(object ox, JSONParameters jpx)
            {
                return (string) ox;
            }

            internal object <ToFastJson>b__11_0(object ox)
            {
                return ox;
            }

            internal KeyValuePair<int, string> <ToFastJson>b__21_0(KeyValuePair<int, string> kv)
            {
                return kv;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__10<TK, TV>
        {
            public static readonly FwNs.Txt.JSon.Xtnz.<>c__10<TK, TV> <>9;
            public static Func<TK, IComparable> <>9__10_1;

            static <>c__10()
            {
                FwNs.Txt.JSon.Xtnz.<>c__10<TK, TV>.<>9 = new FwNs.Txt.JSon.Xtnz.<>c__10<TK, TV>();
            }

            internal IComparable <ToFastJson>b__10_1(TK ko)
            {
                return (IComparable) ko;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__15<T>
        {
            public static readonly FwNs.Txt.JSon.Xtnz.<>c__15<T> <>9;
            public static Func<object, JSONParameters, string> <>9__15_0;

            static <>c__15()
            {
                FwNs.Txt.JSon.Xtnz.<>c__15<T>.<>9 = new FwNs.Txt.JSon.Xtnz.<>c__15<T>();
            }

            internal string <ToFastJsonT>b__15_0(object ox, JSONParameters jpx)
            {
                return Frmttr.PrettyPrint(JSON.Instance.ToJSON(ox, jpx));
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__5<T>
        {
            public static readonly FwNs.Txt.JSon.Xtnz.<>c__5<T> <>9;
            public static Func<T, string> <>9__5_0;

            static <>c__5()
            {
                FwNs.Txt.JSon.Xtnz.<>c__5<T>.<>9 = new FwNs.Txt.JSon.Xtnz.<>c__5<T>();
            }

            internal string <ToFastJsonFunc>b__5_0(T oo)
            {
                return FwNs.Txt.JSon.Xtnz.ObjJp2sFuncFunc.Invoke(typeof(T)).Invoke(oo);
            }
        }

        [CompilerGenerated]
        private static class <>o__14
        {
            public static CallSite<Func<CallSite, Type, object, object>> <>p__0;
            public static CallSite<Func<CallSite, object, string>> <>p__1;
        }
    }
}

