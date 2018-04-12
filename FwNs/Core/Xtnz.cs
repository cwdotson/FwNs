namespace FwNs.Core
{
    using FwNs.Core.Typs;
    using FwNs.Txt;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Extension]
    public static class Xtnz
    {
        public static MethodInfo AsMthd<TO>()
        {
            return AsMthd<TO>("Invoke", new Type[0]);
        }

        public static MethodInfo AsMthd<TO, TA>()
        {
            return AsMthd<TO>(typeof(Func<TA>));
        }

        public static MethodInfo AsMthd<TO, TA, TB>()
        {
            return AsMthd<TO>(typeof(Func<TA>), typeof(Func<TB>));
        }

        public static MethodInfo AsMthd<TO, TA, TB, TC>()
        {
            return AsMthd<TO>("Invoke", typeof(Func<TA>), typeof(Func<TB>), typeof(Func<TC>));
        }

        [Extension]
        public static MethodInfo AsMthd<TR>(Func<TR> func)
        {
            return typeof(Func<TR>).GetMethod("Invoke");
        }

        [Extension]
        public static MethodInfo AsMthd<TA>(Func<TA, object> func)
        {
            return AsMthd<TA, object>(func);
        }

        [Extension]
        public static MethodInfo AsMthd<TA, TR>(Func<TA, TR> func)
        {
            return AsMthd<Func<TA, object>, TA>();
        }

        [Extension]
        public static MethodInfo AsMthd<TA, TB>(Func<TA, TB, object> func)
        {
            return AsMthd<TA, TB, object>(func);
        }

        [Extension]
        public static MethodInfo AsMthd<TA, TB, TR>(Func<TA, TB, TR> func)
        {
            return AsMthd<Func<TA, TB, object>, TA>();
        }

        [Extension]
        public static MethodInfo AsMthd<TA, TB, TC>(Func<TA, TB, TC, object> func)
        {
            return AsMthd<TA, TB, TC, object>(func);
        }

        [Extension]
        public static MethodInfo AsMthd<TA, TB, TC, TR>(Func<TA, TB, TC, TR> func)
        {
            return AsMthd<Func<TA, TB, TC, object>, TA, TB, TC>();
        }

        public static MethodInfo AsMthd<TO>(string mthdnm)
        {
            return AsMthd<TO>(mthdnm, new Type[0]);
        }

        public static MethodInfo AsMthd<TO, TA>(string mthdnm)
        {
            return AsMthd<TO>(mthdnm, typeof(Func<TA>));
        }

        public static MethodInfo AsMthd<TO, TA, TB>(string mthdnm)
        {
            return AsMthd<TO>(mthdnm, typeof(Func<TA>), typeof(Func<TB>));
        }

        public static MethodInfo AsMthd<TO, TA, TB, TC>(string mthdnm)
        {
            return AsMthd<TO>(mthdnm, typeof(Func<TA>), typeof(Func<TB>), typeof(Func<TC>));
        }

        public static MethodInfo AsMthd<TO>(Type typA)
        {
            return AsMthd<TO>("Invoke", typA);
        }

        public static MethodInfo AsMthd<TO>(string mthdnm, Type[] typs)
        {
            return AsMthd(typeof(TO), mthdnm, typs);
        }

        public static MethodInfo AsMthd<TO>(string mthdnm, Type typA)
        {
            Type[] typs = new Type[] { typA };
            return AsMthd<TO>(mthdnm, typs);
        }

        [Extension]
        public static MethodInfo AsMthd(Type type, string mthdnm)
        {
            return AsMthd(type, mthdnm, new Type[0]);
        }

        public static MethodInfo AsMthd<TO>(Type typA, Type typB)
        {
            Type[] typs = new Type[] { typA, typB };
            return AsMthd<TO>("Invoke", typs);
        }

        public static MethodInfo AsMthd<TO>(string mthdnm, Type typA, Type typB)
        {
            Type[] typs = new Type[] { typA, typB };
            return AsMthd<TO>(mthdnm, typs);
        }

        [Extension]
        public static MethodInfo AsMthd(Type type, string mthdnm, params object[] obj)
        {
            if (IsNotNullOrEmpty<object[]>(obj))
            {
                return type.GetMethod(mthdnm);
            }
            Type[] types = Enumerable.ToArray<Type>(Enumerable.Select<object, Type>(obj, <>c.<>9__94_0 ?? (<>c.<>9__94_0 = new Func<object, Type>(<>c.<>9, this.<AsMthd>b__94_0))));
            return type.GetMethod(mthdnm, types);
        }

        [Extension]
        public static MethodInfo AsMthd(Type type, string mthdnm, Type[] typs)
        {
            if (IsNotNullOrEmpty<Type[]>(typs))
            {
                return type.GetMethod(mthdnm);
            }
            return type.GetMethod(mthdnm, typs);
        }

        public static MethodInfo AsMthd<TO>(string mthdnm, Type typA, Type typB, Type typC)
        {
            Type[] typs = new Type[] { typA, typB, typC };
            return AsMthd<TO>(mthdnm, typs);
        }

        [Extension]
        public static MethodInfo AsMthdInvk(Type type, params object[] obj)
        {
            return AsMthd(type, "Invoke", obj);
        }

        [Extension]
        public static TR AsTry<TR>(Dlgt<TR> func) where TR: class
        {
            return AsTry<TR>((Func<TR>) func);
        }

        [Extension]
        public static TR AsTry<TR>(Func<TR> func) where TR: class
        {
            return AsTry<TR>(func, default(TR));
        }

        [Extension]
        public static Func<TA, TR> AsTry<TA, TR>(Func<TA, TR> func) where TR: class
        {
            <>c__DisplayClass92_0<TA, TR> class_;
            return new Func<TA, TR>(class_, this.<AsTry>b__0);
        }

        [Extension]
        public static Func<TA, TB, TR> AsTry<TA, TB, TR>(Func<TA, TB, TR> func) where TR: class
        {
            <>c__DisplayClass87_0<TA, TB, TR> class_1;
            return new Func<TA, TB, TR>(class_1, this.<AsTry>b__0);
        }

        [Extension]
        public static Func<TA, TB, TC, TR> AsTry<TA, TB, TC, TR>(Func<TA, TB, TC, TR> func) where TR: class
        {
            <>c__DisplayClass83_0<TA, TB, TC, TR> class_1;
            return new Func<TA, TB, TC, TR>(class_1, this.<AsTry>b__0);
        }

        [Extension]
        public static object AsTry(MethodInfo mthdNfo)
        {
            return AsTry(mthdNfo, null);
        }

        [Extension]
        public static TR AsTry<TR>(MethodInfo mthdNfo) where TR: class
        {
            return AsTry<TR>(mthdNfo, default(TR), null);
        }

        [Extension]
        public static TR AsTry<TR>(Func<TR> func, TR dflt) where TR: class
        {
            return TryFunk<TR>(func, dflt);
        }

        [Extension]
        public static Func<TA, TR> AsTry<TA, TR>(Func<TA, TR> func, TR dflt)
        {
            <>c__DisplayClass91_0<TA, TR> class_1;
            return new Func<TA, TR>(class_1, this.<AsTry>b__0);
        }

        [Extension]
        public static Func<TA, TB, TR> AsTry<TA, TB, TR>(Func<TA, TB, TR> func, TR dflt)
        {
            <>c__DisplayClass86_0<TA, TB, TR> class_1;
            return new Func<TA, TB, TR>(class_1, this.<AsTry>b__0);
        }

        [Extension]
        public static Func<TA, TB, TC, TR> AsTry<TA, TB, TC, TR>(Func<TA, TB, TC, TR> func, TR dflt)
        {
            <>c__DisplayClass82_0<TA, TB, TC, TR> class_1;
            return new Func<TA, TB, TC, TR>(class_1, this.<AsTry>b__0);
        }

        [Extension]
        public static object AsTry(MethodInfo mthdNfo, params object[] prms)
        {
            return AsTry(mthdNfo, null, null, prms);
        }

        [Extension]
        public static T AsTry<T>(MethodInfo mthdNfo, out Exception excptn)
        {
            throw new Exception();
        }

        [Extension]
        public static TR AsTry<TR>(MethodInfo mthdNfo, object[] args) where TR: class
        {
            return AsTry<TR>(mthdNfo, default(TR), args);
        }

        [Extension]
        public static object AsTry<TA, TR>(Func<TA, TR> obj, TR dflt, TA tA)
        {
            Type[] types = new Type[] { typeof(TA) };
            object[] args = new object[] { tA };
            return AsTry<TR>(typeof(Func<,>).GetMethod("Invoke", types), obj, dflt, args);
        }

        [Extension]
        public static object AsTry(MethodInfo mthdNfo, object objA, object[] args)
        {
            if (IsNotNull<object>(objA))
            {
                if (mthdNfo.DeclaringType == objA.GetType())
                {
                    return AsTry(mthdNfo, objA, null, args);
                }
                if (mthdNfo.ReturnType.IsAssignableFrom(objA.GetType()))
                {
                    return AsTry<MethodInfo, object>(mthdNfo, null, objA, args);
                }
                if (mthdNfo.ReturnType.IsAssignableFrom(objA.GetType()))
                {
                    return AsTry<MethodInfo, object>(mthdNfo, null, objA, args);
                }
            }
            return AsTry(mthdNfo, null, null, args);
        }

        [Extension]
        public static TR AsTry<TR>(MethodInfo mthdNfo, TR dflt, object[] args)
        {
            return AsTry<MethodInfo, TR>(mthdNfo, null, dflt, args);
        }

        [Extension]
        public static object AsTry(MethodInfo mthdNfo, object ownr, object dflt, object[] args)
        {
            try
            {
                return mthdNfo.Invoke(ownr, null);
            }
            catch (Exception)
            {
                return dflt;
            }
        }

        [Extension]
        public static TR AsTry<TR>(MethodInfo mthdNfo, object ownr, TR dflt, object[] args)
        {
            try
            {
                return (TR) mthdNfo.Invoke(ownr, args);
            }
            catch (Exception)
            {
                return dflt;
            }
        }

        [Extension]
        public static TR AsTry<TO, TR>(TO ownr, string mthdnm, TR dflt, object[] args)
        {
            if (IsNotNullOrEmpty<object[]>(args))
            {
                return AsTry<TR>(typeof(TO).GetMethod(mthdnm), ownr, dflt, new object[0]);
            }
            Type[] types = Enumerable.ToArray<Type>(Enumerable.Select<object, Type>(args, <>c__48<TO, TR>.<>9__48_0 ?? (<>c__48<TO, TR>.<>9__48_0 = new Func<object, Type>(<>c__48<TO, TR>.<>9, this.<AsTry>b__48_0))));
            return AsTry<TR>(typeof(TO).GetMethod(mthdnm, types), ownr, dflt, args);
        }

        [Extension]
        public static Func<TA, TR> AsTryFunc<TO, TA, TR>(TO ownr, string mthdnm) where TR: class
        {
            <>c__DisplayClass51_0<TO, TA, TR> class_1;
            return new Func<TA, TR>(class_1, this.<AsTryFunc>b__0);
        }

        [Extension]
        public static Func<TA, TB, TR> AsTryFunc<TA, TB, TR>(Func<TA, TB, TR> func, TR dflt)
        {
            <>c__DisplayClass53_0<TA, TB, TR> class_;
            return new Func<TA, TB, TR>(class_, this.<AsTryFunc>b__0);
        }

        [Extension]
        public static Func<TA, TB, TR> AsTryFunc<TA, TB, TR>(Func<TA, TB, TR> func, object[] args) where TR: class
        {
            <>c__DisplayClass54_0<TA, TB, TR> class_1;
            return new Func<TA, TB, TR>(class_1, this.<AsTryFunc>b__0);
        }

        [Extension]
        public static Func<TA, TB, TR> AsTryFunc<TO, TA, TB, TR>(TO ownr, string mthdnm) where TR: class
        {
            <>c__DisplayClass52_0<TO, TA, TB, TR> class_;
            return new Func<TA, TB, TR>(class_, this.<AsTryFunc>b__0);
        }

        [Extension]
        public static Func<TR> AsTryFunc<TO, TR>(TO ownr, string mthdnm, TR dflt)
        {
            <>c__DisplayClass49_0<TO, TR> class_1;
            return new Func<TR>(class_1, this.<AsTryFunc>b__0);
        }

        [Extension]
        public static Func<TA, TR> AsTryFunc<TO, TA, TR>(TO ownr, string mthdnm, TR dflt)
        {
            <>c__DisplayClass50_0<TO, TA, TR> class_1;
            return new Func<TA, TR>(class_1, this.<AsTryFunc>b__0);
        }

        [Extension]
        public static T CatchErr<T>(Func<T> func, out Exception excptn) where T: class
        {
            try
            {
                excptn = null;
                return func.Invoke();
            }
            catch (Exception exception)
            {
                excptn = exception;
                return default(T);
            }
        }

        [Extension]
        public static bool CatchErr<T, TA>(T actn, TA tA, out Exception excptn)
        {
            try
            {
                if (actn is Action<TA>)
                {
                    ((Action<TA>) actn)(tA);
                }
                else if (actn is Func<TA>)
                {
                    ((Action<TA>) actn)(tA);
                }
                excptn = null;
                return false;
            }
            catch (Exception exception)
            {
                excptn = exception;
                return true;
            }
        }

        public static object DoAndRtrn(object obj, Action<object> aktn)
        {
            aktn(obj);
            return obj;
        }

        [Extension]
        public static TT DoAndRtrn<TT>(TT tt, Action<TT> aktn)
        {
            return (TT) DoAndRtrn(tt, delegate (object oo) {
                aktn((TT) oo);
            });
        }

        [Extension]
        public static TT DoAndRtrn<TT, TU>(TT tt, Action<TU> aktn, TU uu)
        {
            return (TT) DoAndRtrn(tt, delegate (object oo) {
                aktn(uu);
            });
        }

        [Extension]
        public static TT DoAndRtrn<TT, TU>(TT tt, Action<TT, TU> aktn, TU uu)
        {
            return (TT) DoAndRtrn(tt, delegate (object oo) {
                aktn.Invoke((TT) oo, uu);
            });
        }

        [Extension]
        public static object EvalEx(MethodInfo mthd)
        {
            return EvalEx(mthd, (object[]) null);
        }

        [Extension]
        public static object EvalEx(MethodInfo mthdNfo, object[] args)
        {
            return EvalEx(mthdNfo, null, args);
        }

        [Extension]
        public static object EvalEx(MethodInfo mthd, object ownr)
        {
            return EvalEx(mthd, (object[]) null);
        }

        [Extension]
        public static object EvalEx<TA>(MethodInfo mthd, TA tA)
        {
            return EvalEx<TA>(mthd, null, tA);
        }

        [Extension]
        public static object EvalEx(MethodInfo mthdNfo, object ownr, object[] args)
        {
            try
            {
                return mthdNfo.Invoke(ownr, args);
            }
            catch (Exception exception1)
            {
                return exception1;
            }
        }

        [Extension]
        public static object EvalEx<TA>(MethodInfo mthd, object ownr, TA tA)
        {
            object[] args = new object[] { tA };
            return EvalEx(mthd, ownr, args);
        }

        [Extension]
        public static object EvalEx<TA, TB>(MethodInfo mthd, TA tA, TB tb)
        {
            return EvalEx<TA, TB>(mthd, null, tA, tb);
        }

        [Extension]
        public static object EvalEx<TA, TB>(MethodInfo mthd, object ownr, TA tA, TB tb)
        {
            object[] args = new object[] { tA, tb };
            return EvalEx(mthd, ownr, args);
        }

        [Extension]
        public static object EvalEx<TA, TB, TC>(MethodInfo mthd, TA tA, TB tb, TC tc)
        {
            return EvalEx<TA, TB, TC>(mthd, null, tA, tb, tc);
        }

        [Extension]
        public static object EvalEx<TA, TB, TC>(MethodInfo mthd, object ownr, TA tA, TB tb, TC tc)
        {
            object[] args = new object[] { tA, tb, tc };
            return EvalEx(mthd, ownr, args);
        }

        [Extension]
        public static Func<TA, TR> ExFunc<TA, TR>(Func<TA, TR> targ, TR dflt)
        {
            throw new Exception();
        }

        public static TypeCode GetTypCode(string typName)
        {
            TypeCode empty;
            if (!Enum.TryParse<TypeCode>(typName, false, ref empty))
            {
                empty = TypeCode.Empty;
            }
            return empty;
        }

        [Extension]
        public static TypeCode GetTypCode(Type typ)
        {
            return GetTypCode(typ.Name);
        }

        [Extension]
        public static object GetValu(object obj, string nm)
        {
            // This item is obfuscated and can not be translated.
            throw new Exception();
        }

        [Extension]
        public static object GetValu<TT>(TT tt, string nmProp)
        {
            return GetValu<TT, object>(tt, nmProp);
        }

        [Extension]
        public static TP GetValu<TT, TP>(TT tt, string nmProp)
        {
            return (TP) GetValu<TT>(tt, nmProp, typeof(TP));
        }

        [Extension]
        public static object GetValu<TT>(TT tt, string nmProp, Type tp)
        {
            return GetValuFunc<TT>(nmProp, tp).Invoke(tt);
        }

        public static Func<TT, object> GetValuFunc<TT, TP>(string nmProp)
        {
            return GetValuFunc<TT>(nmProp, typeof(TP));
        }

        public static Func<TT, object> GetValuFunc<TT>(string nmProp, Type tp)
        {
            return GetValuFuncFunc<TT>().Invoke(nmProp, tp);
        }

        public static Func<string, Type, Func<T, object>> GetValuFuncFunc<T>()
        {
            return (<>c__99<T>.<>9__99_0 ?? (<>c__99<T>.<>9__99_0 = new Func<string, Type, Func<T, object>>(<>c__99<T>.<>9, this.<GetValuFuncFunc>b__99_0)));
        }

        public static Expression<Func<string, Func<T, TP>>> GetValuFuncXprsn<T, TP>()
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            Expression[] arguments = new Expression[] { expression = Expression.Parameter(typeof(string), "nm"), Expression.Constant(typeof(TP), typeof(Type)) };
            Expression[] expressionArray2 = new Expression[] { expression2 = Expression.Parameter(typeof(T), "tt") };
            ParameterExpression[] parameters = new ParameterExpression[] { expression2 };
            ParameterExpression[] expressionArray4 = new ParameterExpression[] { expression };
            return Expression.Lambda<Func<string, Func<T, TP>>>(Expression.Lambda<Func<T, TP>>(Expression.Convert(Expression.Call(Expression.Invoke(Expression.Call(null, (MethodInfo) methodof(FwNs.Core.Xtnz.GetValuFuncFunc), new Expression[0]), arguments), (MethodInfo) methodof(Func<T, object>.Invoke, Func<T, object>), expressionArray2), typeof(TP)), parameters), expressionArray4);
        }

        [Extension]
        public static byte[] HexStringToBytes(string hex)
        {
            byte[] buffer = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(hex.Substring(i, 2), 0x10);
            }
            return buffer;
        }

        [Extension]
        public static bool IsEmpty<T>(T tt) where T: IEnumerable
        {
            return !Enumerable.Any<object>(Enumerable.Cast<object>(tt));
        }

        [Extension]
        public static bool IsMltple<T>(T tt) where T: IEnumerable
        {
            return ((!IsNull<object>(tt) && !IsEmpty<T>(tt)) && (1 < Enumerable.Count<object>(Enumerable.Cast<object>(tt))));
        }

        [Extension]
        public static bool IsMoreThanOne<T>(T tt) where T: IEnumerable
        {
            return !IsNullOrEmpty<T>(tt);
        }

        [Extension]
        public static bool IsNotEmpty<T>(T tt) where T: IEnumerable
        {
            return !IsEmpty<T>(tt);
        }

        [Extension]
        public static bool IsNotNull<T>(T tt) where T: class
        {
            return !IsNull<T>(tt);
        }

        [Extension]
        public static bool IsNotNullOrEmpty<T>(T tt) where T: IEnumerable
        {
            return !IsNullOrEmpty<T>(tt);
        }

        [Extension]
        public static bool IsNull(IEnumerable tt)
        {
            return (tt == null);
        }

        [Extension]
        public static bool IsNull<T>(T tt) where T: class
        {
            return (tt == null);
        }

        [Extension]
        public static bool IsNullOrEmpty<T>(T tt) where T: IEnumerable
        {
            if (!IsNull<object>(tt))
            {
                return IsEmpty<T>(tt);
            }
            return true;
        }

        [Extension]
        public static T Katch<T>(Func<T> func, out Exception excptn) where T: class
        {
            T local = default(T);
            throw new Exception();
        }

        [Extension]
        public static Func<TR> KatchFunc<TR>(Func<TR> func) where TR: class
        {
            <>c__DisplayClass25_0<TR> class_1;
            return new Func<TR>(class_1, this.<KatchFunc>b__0);
        }

        [Extension]
        public static Func<TA, TR> MakeSafe<TA, TR>(Func<TA, TR> func) where TR: class
        {
            <>c__DisplayClass61_0<TA, TR> class_1;
            AsTryFunc<Func<TA, TR>, TR>(func, "Invoke", default(TR));
            return new Func<TA, TR>(class_1, this.<MakeSafe>b__0);
        }

        [Extension]
        public static Expression<Func<TA, TR>> MakeSafe<TA, TR>(Func<TA, TR> func, TR dflt)
        {
            <>c__DisplayClass60_0<TA, TR> class_;
            ParameterExpression expression;
            Expression[] arguments = new Expression[] { Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass60_0<TA, TR>)), fieldof(<>c__DisplayClass60_0<TA, TR>.func, <>c__DisplayClass60_0<TA, TR>)), expression = Expression.Parameter(typeof(TA), "ta"), Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass60_0<TA, TR>)), fieldof(<>c__DisplayClass60_0<TA, TR>.dflt, <>c__DisplayClass60_0<TA, TR>)) };
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return Expression.Lambda<Func<TA, TR>>(Expression.Call(null, (MethodInfo) methodof(FwNs.Core.Xtnz.TryFunk), arguments), parameters);
        }

        [Extension]
        public static Expression<Func<TR>> MakeXprsn<TR>(Func<TR> func)
        {
            <>c__DisplayClass64_0<TR> class_1;
            return Expression.Lambda<Func<TR>>(Expression.Call(Expression.Field(Expression.Constant(class_1, typeof(<>c__DisplayClass64_0<TR>)), fieldof(<>c__DisplayClass64_0<TR>.func, <>c__DisplayClass64_0<TR>)), (MethodInfo) methodof(Func<TR>.Invoke, Func<TR>), new Expression[0]), new ParameterExpression[0]);
        }

        [Extension]
        public static Expression<Func<TR>> MakeXprsn<TR>(Func<TR> func, TR dflt)
        {
            <>c__DisplayClass63_0<TR> class_;
            Expression[] arguments = new Expression[] { Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass63_0<TR>)), fieldof(<>c__DisplayClass63_0<TR>.func, <>c__DisplayClass63_0<TR>)), Expression.Constant("Invoke", typeof(string)), Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass63_0<TR>)), fieldof(<>c__DisplayClass63_0<TR>.dflt, <>c__DisplayClass63_0<TR>)) };
            return Expression.Lambda<Func<TR>>(Expression.Call(Expression.Call(null, (MethodInfo) methodof(FwNs.Core.Xtnz.AsTryFunc), arguments), (MethodInfo) methodof(Func<TR>.Invoke, Func<TR>), new Expression[0]), new ParameterExpression[0]);
        }

        [Extension]
        public static Expression<Func<TA, TR>> MakeXprsn<TA, TR>(Func<TA, TR> func, TR dflt)
        {
            <>c__DisplayClass62_0<TA, TR> class_;
            ParameterExpression expression;
            Expression[] arguments = new Expression[] { Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass62_0<TA, TR>)), fieldof(<>c__DisplayClass62_0<TA, TR>.func, <>c__DisplayClass62_0<TA, TR>)), expression = Expression.Parameter(typeof(TA), "ta"), Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass62_0<TA, TR>)), fieldof(<>c__DisplayClass62_0<TA, TR>.dflt, <>c__DisplayClass62_0<TA, TR>)) };
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return Expression.Lambda<Func<TA, TR>>(Expression.Call(null, (MethodInfo) methodof(FwNs.Core.Xtnz.TryFunk), arguments), parameters);
        }

        public static Func<Type, string, Type, MemberInfo, bool> MemberInfo2ValuFuncFunc<T>()
        {
            return (<>c__98<T>.<>9__98_0 ?? (<>c__98<T>.<>9__98_0 = new Func<Type, string, Type, MemberInfo, bool>(<>c__98<T>.<>9, this.<MemberInfo2ValuFuncFunc>b__98_0)));
        }

        public static Func<MemberInfo, bool> Mmbr2BoolFunc<T, TP>(string nm)
        {
            <>c__DisplayClass97_0<T, TP> class_1;
            return new Func<MemberInfo, bool>(class_1, this.<Mmbr2BoolFunc>b__0);
        }

        [Extension]
        public static Type MmbrValTyp(MemberInfo mi)
        {
            Type tt = Type.GetType("System." + FwNs.Txt.Xtnz.Splyt(mi.ToString(), " ")[0]);
            if (IsNull<Type>(tt))
            {
                tt = typeof(object);
                if (IsNull<Type>(tt))
                {
                    throw new Exception();
                }
            }
            return tt;
        }

        [Extension]
        public static void SetValu<TT, TP>(TT tt, string nmProp, TP val)
        {
            MemberInfo[] members = typeof(TT).GetMembers();
            if (!Enumerable.Any<MemberInfo>(members))
            {
                throw new Exception();
            }
            IEnumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberInfo current = (MemberInfo) enumerator.Current;
                if (current.Name == nmProp)
                {
                    if (current.MemberType == MemberTypes.Property)
                    {
                        typeof(TT).GetProperty(nmProp).SetValue(tt, val, new object[0]);
                        if (!val.Equals((TP) GetValu<TT>(tt, nmProp)))
                        {
                            throw new Exception();
                        }
                        return;
                    }
                    if (current.MemberType != MemberTypes.Field)
                    {
                        throw new Exception();
                    }
                    typeof(TT).GetField(nmProp).SetValue(tt, val);
                    if (!val.Equals((TP) GetValu<TT>(tt, nmProp)))
                    {
                        throw new Exception();
                    }
                    return;
                }
            }
        }

        [Extension]
        public static bool TryDo(Action actn, object errAktn)
        {
            throw new Exception();
        }

        [Extension]
        public static bool TryDo<TA>(Action<TA> actn, TA tA, object errAktn)
        {
            <>c__DisplayClass29_0<TA> class_;
            Action action = null;
            if ((IsNotNull<object>(errAktn) && TypsFw.IsAction(errAktn.GetType())) && (errAktn is Action<TA>))
            {
                action = new Action(class_, this.<TryDo>b__1);
            }
            return TryDo(new Action(class_, this.<TryDo>b__0), action ?? errAktn);
        }

        [Extension]
        public static bool TryDo<TA, TB>(Action<TA, TB> actn, TA tA, TB tB, object errAktn)
        {
            object tt = errAktn;
            if (IsNotNull<object>(tt) && TypsFw.IsAction(tt.GetType()))
            {
                if (errAktn is Action<TA, TB>)
                {
                    tt = delegate (TA ta) {
                        ((Action<TA, TB>) errAktn).Invoke(ta, tB);
                    };
                }
                else if (errAktn is Action<TB>)
                {
                    <>c__DisplayClass30_0<TA, TB> class_;
                    tt = new Action(class_, this.<TryDo>b__2);
                }
            }
            return TryDo<TA>(delegate (TA ta) {
                actn.Invoke(ta, tB);
            }, tA, tt);
        }

        [Extension]
        public static bool TryDo<TA, TB, TC>(Action<TA, TB, TC> actn, TA tA, TB tB, TC tC, object errAktn)
        {
            <>c__DisplayClass31_0<TA, TB, TC> class_;
            object tt = errAktn;
            if (IsNotNull<object>(tt) && TypsFw.IsAction(tt.GetType()))
            {
                if (errAktn is Action<TA, TB, TC>)
                {
                    tt = new Action<TA, TB>(class_, this.<TryDo>b__1);
                }
                else if (errAktn is Action<TB, TC>)
                {
                    tt = delegate (TB tb) {
                        ((Action<TB, TC>) errAktn).Invoke(tb, tC);
                    };
                }
                else if (errAktn is Action<TC>)
                {
                    tt = new Action(class_, this.<TryDo>b__3);
                }
            }
            return TryDo<TA, TB>(new Action<TA, TB>(class_, this.<TryDo>b__0), tA, tB, tt);
        }

        [Extension]
        public static bool TryDo<TA, TB, TC, TD>(Action<TA, TB, TC, TD> actn, TA tA, TB tB, TC tC, TD tD, object errAktn)
        {
            <>c__DisplayClass32_0<TA, TB, TC, TD> class_;
            object tt = errAktn;
            if (IsNotNull<object>(tt) && TypsFw.IsAction(tt.GetType()))
            {
                if (errAktn is Action<TA, TB, TC, TD>)
                {
                    tt = new Action<TA, TB, TC>(class_, this.<TryDo>b__1);
                }
                else if (errAktn is Action<TB, TC, TD>)
                {
                    tt = new Action<TB, TC>(class_, this.<TryDo>b__2);
                }
                else if (errAktn is Action<TC, TD>)
                {
                    tt = delegate (TC tc) {
                        ((Action<TC, TD>) errAktn).Invoke(tc, tD);
                    };
                }
                else if (errAktn is Action<TD>)
                {
                    tt = new Action(class_, this.<TryDo>b__4);
                }
            }
            return TryDo<TA, TB, TC>(new Action<TA, TB, TC>(class_, this.<TryDo>b__0), tA, tB, tC, tt);
        }

        [Extension]
        public static TR TryFunk<TR>(Func<TR> funk) where TR: class
        {
            TR tr = default(TR);
            Action<TR> aktn = delegate (TR trx) {
                tr = funk.Invoke();
            };
            return DoAndRtrn<TR>(tr, delegate (TR trx) {
                TryDo<TR>(aktn, trx, null);
            });
        }

        [Extension]
        public static TR TryFunk<TR>(Func<TR> funk, TR dflt)
        {
            <>c__DisplayClass38_0<TR> class_1;
            TR tr = dflt;
            Action actn = new Action(class_1, this.<TryFunk>b__0);
            Action errAktn = new Action(class_1, this.<TryFunk>b__1);
            TryDo(actn, errAktn);
            return tr;
        }

        [Extension]
        public static TR TryFunk<TA, TR>(Func<TA, TR> funk, TA tA) where TR: class
        {
            TR tr = default(TR);
            Action<TR> aktn = delegate (TR trx) {
                tr = funk.Invoke(tA);
            };
            return DoAndRtrn<TR>(tr, delegate (TR trx) {
                TryDo<TR>(aktn, trx, null);
            });
        }

        [Extension]
        public static TR TryFunk<TA, TR>(Func<TA, TR> funk, TA tA, TR dflt)
        {
            TryDo<TA>(delegate (TA ta) {
                dflt = funk.Invoke(ta);
            }, tA, null);
            return dflt;
        }

        [Extension]
        public static TR TryFunk<TA, TR>(Func<TA, TR> funk, TA tA, Func<TA, TR> funk2) where TR: class
        {
            Action<TR> aktn = delegate (TR trx) {
                funk.Invoke(tA);
            };
            Action<TR> aktn2 = delegate (TR trx) {
                funk2.Invoke(tA);
            };
            return DoAndRtrn<TR>(default(TR), delegate (TR trx) {
                TryDo<TR>(aktn, trx, aktn2);
            });
        }

        [Extension]
        public static TR TryFunk<TA, TB, TR>(Func<TA, TB, TR> funk, TA tA, TB tB, TR dflt)
        {
            <>c__DisplayClass34_0<TA, TB, TR> class_1;
            TryDo<TA, TB>(new Action<TA, TB>(class_1, this.<TryFunk>b__0), tA, tB, null);
            return dflt;
        }

        [Extension]
        public static TR TryFunk<TA, TB, TC, TR>(Func<TA, TB, TC, TR> funk, TA tA, TB tB, TC tC, TR dflt)
        {
            <>c__DisplayClass33_0<TA, TB, TC, TR> class_;
            TR tD = default(TR);
            TryDo<TA, TB, TC, TR>(new Action<TA, TB, TC, TR>(class_, this.<TryFunk>b__0), tA, tB, tC, tD, delegate (TR tr) {
                tr = dflt;
            });
            return tD;
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FwNs.Core.Xtnz.<>c <>9 = new FwNs.Core.Xtnz.<>c();
            public static Func<object, Type> <>9__94_0;
            public static Func<object, Type> <>9__95_0;

            internal Type <AsMthd>b__94_0(object oo)
            {
                return oo.GetType();
            }

            internal Type <AsMthdInvk>b__95_0(object oo)
            {
                return oo.GetType();
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__48<TO, TR>
        {
            public static readonly FwNs.Core.Xtnz.<>c__48<TO, TR> <>9;
            public static Func<object, Type> <>9__48_0;

            static <>c__48()
            {
                FwNs.Core.Xtnz.<>c__48<TO, TR>.<>9 = new FwNs.Core.Xtnz.<>c__48<TO, TR>();
            }

            internal Type <AsTry>b__48_0(object rg)
            {
                return rg.GetType();
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__98<T>
        {
            public static readonly FwNs.Core.Xtnz.<>c__98<T> <>9;
            public static Func<Type, string, Type, MemberInfo, bool> <>9__98_0;
            public static Func<Type, string, Type, Func<MemberInfo, bool>> <>9__98_1;

            static <>c__98()
            {
                FwNs.Core.Xtnz.<>c__98<T>.<>9 = new FwNs.Core.Xtnz.<>c__98<T>();
            }

            internal bool <MemberInfo2ValuFuncFunc>b__98_0(Type ot, string nm, Type pt, MemberInfo mi)
            {
                bool flag = false;
                if (mi.Name == nm)
                {
                    Type propertyType;
                    if (mi.MemberType == MemberTypes.Property)
                    {
                        propertyType = typeof(T).GetProperty(mi.Name).PropertyType;
                    }
                    else if (mi.MemberType == MemberTypes.Field)
                    {
                        propertyType = typeof(T).GetField(mi.Name).FieldType;
                    }
                    else if (mi.MemberType == MemberTypes.Method)
                    {
                        propertyType = typeof(MethodInfo);
                    }
                    else if (mi.MemberType == MemberTypes.Constructor)
                    {
                        propertyType = typeof(ConstructorInfo);
                    }
                    else if (mi.MemberType == MemberTypes.NestedType)
                    {
                        propertyType = typeof(T).GetNestedType(mi.Name);
                    }
                    else
                    {
                        MemberTypes memberType = mi.MemberType;
                        throw new Exception();
                    }
                    if (FwNs.Core.Xtnz.IsNotNull<Type>(propertyType))
                    {
                        flag = pt.IsAssignableFrom(propertyType);
                    }
                }
                return flag;
            }

            internal Func<MemberInfo, bool> <MemberInfo2ValuFuncFunc>b__98_1(Type otc, string nmc, Type ptc)
            {
                throw new Exception();
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__99<T>
        {
            public static readonly FwNs.Core.Xtnz.<>c__99<T> <>9;
            public static Func<string, Type, Func<T, object>> <>9__99_0;

            static <>c__99()
            {
                FwNs.Core.Xtnz.<>c__99<T>.<>9 = new FwNs.Core.Xtnz.<>c__99<T>();
            }

            internal Func<T, object> <GetValuFuncFunc>b__99_0(string nm, Type pt)
            {
                Func<MemberInfo, bool> predicate = FwNs.Core.Xtnz.Mmbr2BoolFunc<T, object>(nm);
                if (!Enumerable.Any<MemberInfo>(typeof(T).GetMember(nm), predicate))
                {
                    throw new Exception();
                }
                MemberInfo info = Enumerable.Single<MemberInfo>(typeof(T).GetMembers(), predicate);
                if (info.MemberType == MemberTypes.Property)
                {
                    new FwNs.Core.Xtnz.<>c__DisplayClass99_0<T>().pi = typeof(T).GetProperty(info.Name);
                    return new Func<T, object>(new FwNs.Core.Xtnz.<>c__DisplayClass99_0<T>(), this.<GetValuFuncFunc>b__1);
                }
                if (info.MemberType == MemberTypes.Field)
                {
                    new FwNs.Core.Xtnz.<>c__DisplayClass99_1<T>().fi = typeof(T).GetField(info.Name);
                    return new Func<T, object>(new FwNs.Core.Xtnz.<>c__DisplayClass99_1<T>(), this.<GetValuFuncFunc>b__2);
                }
                if (info.MemberType != MemberTypes.All)
                {
                    throw new Exception();
                }
                return null;
            }
        }
    }
}

