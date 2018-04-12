namespace FwNs.Data.FyleDbNs
{
    using FwNs.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class FyleDb : FileDb
    {
        private JsonSerializer _jsonSerializer;
        private Table _wurds;
        private Table _itymz;
        private object obj;
        private StringWriter _stringWriter;
        private StringBuilder _sb;
        public static Func<KV<int, KV<int, string>>, int> FunkFsoInt;
        public static Func<IEnumerable<KV<int, KV<int, string>>>, int> FunkFsoIntMax;
        public static Func<string, int, int, KV<int, KV<int, string>>> FunkStrKV;
        public static Func<FileSystemInfo, int, int, KV<int, KV<int, string>>> FunkFsoKV;
        public static Func<IEnumerable<KV<int, KV<int, string>>>, IEnumerable<int>> FunkFsoInts;
        public static Func<IEnumerable<FileSystemInfo>, int, IEnumerable<KV<int, KV<int, string>>>> FunkFsoKVs;
        public static Func<string, Func<string, string[]>> StrFuncSplitRmvEmpty;
        public static Expression<Func<string, string[], Func<string, string>, bool>> ExprStrAryStrAll;
        public static Expression<Func<Func<FileSystemInfo, string>, Func<string, string, bool>, Expression<Func<string, Expression<Func<FileSystemInfo, bool>>>>>> esff;
        public static Expression<Func<string, Expression<Func<FileSystemInfo, bool>>>> ef;
        public static Func<Expression<Func<FileSystemInfo, string>>, Expression<Func<string, string, bool>>, string, Func<FileSystemInfo, bool>> tests010ff;
        public static Expression<Func<FileSystemInfo, string>> XprsnFileNm;
        public static Expression<Func<FileSystemInfo, string>> FuncFileNm;
        public static Func<string, Expression<Func<string, string, bool>>, Func<FileSystemInfo, bool>> tests010f;
        public static Func<string, Func<FileSystemInfo, bool>> tests101;
        public static Func<string, Func<FileSystemInfo, bool>> tests102;
        public static Func<string, Func<FileSystemInfo, bool>> tests103;
        public static Func<string, Func<FileSystemInfo, bool>> tests104;
        public static Func<FileSystemInfo, bool>[] tests01Ary;
        public static Func<FileSystemInfo, bool>[] tests105;

        static FyleDb()
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            ParameterExpression expression3;
            ParameterExpression expression4;
            FunkFsoInt = new Func<KV<int, KV<int, string>>, int>(<>c.<>9, this.<.cctor>b__53_0);
            FunkFsoIntMax = new Func<IEnumerable<KV<int, KV<int, string>>>, int>(<>c.<>9, this.<.cctor>b__53_1);
            FunkStrKV = null;
            FunkFsoKV = new Func<FileSystemInfo, int, int, KV<int, KV<int, string>>>(<>c.<>9, this.<.cctor>b__53_2);
            FunkFsoInts = new Func<IEnumerable<KV<int, KV<int, string>>>, IEnumerable<int>>(<>c.<>9, this.<.cctor>b__53_3);
            FunkFsoKVs = new Func<IEnumerable<FileSystemInfo>, int, IEnumerable<KV<int, KV<int, string>>>>(<>c.<>9, this.<.cctor>b__53_4);
            StrFuncSplitRmvEmpty = new Func<string, Func<string, string[]>>(<>c.<>9, this.<.cctor>b__53_6);
            Expression[] arguments = new Expression[2];
            arguments[0] = expression2 = Expression.Parameter(typeof(string[]), "strAry");
            Expression[] expressionArray2 = new Expression[] { expression = Expression.Parameter(typeof(string), "s") };
            Expression[] expressionArray3 = new Expression[1];
            Expression[] expressionArray4 = new Expression[] { expression4 = Expression.Parameter(typeof(string), "x") };
            expressionArray3[0] = Expression.Call(expression3, (MethodInfo) methodof(Func<string, string>.Invoke, Func<string, string>), expressionArray4);
            ParameterExpression[] parameters = new ParameterExpression[] { expression4 };
            arguments[1] = Expression.Lambda<Func<string, bool>>(Expression.Call(Expression.Call(expression3 = Expression.Parameter(typeof(Func<string, string>), "F"), (MethodInfo) methodof(Func<string, string>.Invoke, Func<string, string>), expressionArray2), (MethodInfo) methodof(string.Contains), expressionArray3), parameters);
            ParameterExpression[] expressionArray6 = new ParameterExpression[] { expression, expression2, expression3 };
            ExprStrAryStrAll = Expression.Lambda<Func<string, string[], Func<string, string>, bool>>(Expression.Call(null, (MethodInfo) methodof(Enumerable.All), arguments), expressionArray6);
            Expression[] expressionArray7 = new Expression[2];
            Expression[] expressionArray8 = new Expression[] { expression4 = Expression.Parameter(typeof(FileSystemInfo), "fso") };
            expressionArray7[0] = Expression.Invoke(expression3 = Expression.Parameter(typeof(Func<FileSystemInfo, string>), "f2s"), expressionArray8);
            expressionArray7[1] = expression = Expression.Parameter(typeof(string), "s");
            ParameterExpression[] expressionArray9 = new ParameterExpression[] { expression4 };
            ParameterExpression[] expressionArray10 = new ParameterExpression[] { expression };
            ParameterExpression[] expressionArray11 = new ParameterExpression[] { expression3, expression2 };
            esff = Expression.Lambda<Func<Func<FileSystemInfo, string>, Func<string, string, bool>, Expression<Func<string, Expression<Func<FileSystemInfo, bool>>>>>>(Expression.Quote(Expression.Lambda<Func<string, Expression<Func<FileSystemInfo, bool>>>>(Expression.Quote(Expression.Lambda<Func<FileSystemInfo, bool>>(Expression.Invoke(expression2 = Expression.Parameter(typeof(Func<string, string, bool>), "ss2b"), expressionArray7), expressionArray9)), expressionArray10)), expressionArray11);
            ef = esff.Compile().Invoke(new Func<FileSystemInfo, string>(<>c.<>9, this.<.cctor>b__53_8), new Func<string, string, bool>(<>c.<>9, this.<.cctor>b__53_9));
            tests010ff = new Func<Expression<Func<FileSystemInfo, string>>, Expression<Func<string, string, bool>>, string, Func<FileSystemInfo, bool>>(<>c.<>9, this.<.cctor>b__53_10);
            Expression[] expressionArray12 = new Expression[] { Expression.Property(expression2 = Expression.Parameter(typeof(FileSystemInfo), "x"), (MethodInfo) methodof(FileSystemInfo.get_FullName)) };
            ParameterExpression[] expressionArray13 = new ParameterExpression[] { expression2 };
            XprsnFileNm = Expression.Lambda<Func<FileSystemInfo, string>>(Expression.Call(null, (MethodInfo) methodof(Path.GetFileName), expressionArray12), expressionArray13);
            FuncFileNm = XprsnFileNm;
            tests010f = new Func<string, Expression<Func<string, string, bool>>, Func<FileSystemInfo, bool>>(<>c.<>9, this.<.cctor>b__53_11);
            tests101 = new Func<string, Func<FileSystemInfo, bool>>(<>c.<>9, this.<.cctor>b__53_12);
            tests102 = new Func<string, Func<FileSystemInfo, bool>>(<>c.<>9, this.<.cctor>b__53_13);
            tests103 = new Func<string, Func<FileSystemInfo, bool>>(<>c.<>9, this.<.cctor>b__53_14);
            tests104 = new Func<string, Func<FileSystemInfo, bool>>(<>c.<>9, this.<.cctor>b__53_15);
            tests01Ary = new Func<FileSystemInfo, bool>[] { tests101.Invoke("index"), tests102.Invoke("-"), tests103.Invoke("ml"), tests104.Invoke(@"(ht|f)tps?:\/\/") };
            Func<FileSystemInfo, bool>[] funcArray2 = new Func<FileSystemInfo, bool>[4];
            funcArray2[0] = ef.Compile().Invoke("index").Compile();
            funcArray2[1] = ef.Compile().Invoke("publishers").Compile();
            Func<FileSystemInfo, bool>[][] source = new Func<FileSystemInfo, bool>[][] { tests01Ary };
            funcArray2[2] = IsTypAndAll<FileSystemInfo>(typeof(DirectoryInfo), Enumerable.Cast<Expression<Func<FileSystemInfo, bool>>>(source)).Compile();
            Func<FileSystemInfo, bool>[][] funcArrayArray2 = new Func<FileSystemInfo, bool>[][] { tests01Ary };
            funcArray2[3] = IsTypAndAll<FileSystemInfo>(typeof(FileInfo), Enumerable.Cast<Expression<Func<FileSystemInfo, bool>>>(funcArrayArray2)).Compile();
            tests105 = funcArray2;
        }

        public FyleDb()
        {
            this._sb = new StringBuilder();
            this._stringWriter = new StringWriter(this._sb);
            this._jsonSerializer = new JsonSerializer();
            new Field("Id", DataTypeEnum.Int, 0).IsPrimaryKey = true;
            new Field("Idp", DataTypeEnum.Int);
            new Field("Idw", DataTypeEnum.Int);
            new Field("Idc", DataTypeEnum.Int);
            new Field("Idx", DataTypeEnum.Int);
            new Field("Val", DataTypeEnum.String);
        }

        public FyleDb(object[][] wurds) : this()
        {
            this.Wurds.AddRange(Enumerable.Select<object[], Record>(wurds, new Func<object[], Record>(this, this.<.ctor>b__36_0)));
        }

        public string[] GetFsoArry(FileSystemInfo nfo)
        {
            char[] separator = new char[] { '\\', '.' };
            return nfo.FullName.Split(separator);
        }

        public Dictionary<int, string> GetFsoDyct(FileSystemInfo nfo)
        {
            return Enumerable.ToDictionary(Enumerable.Select(this.GetFsoArry(nfo), <>c.<>9__40_0 ?? (<>c.<>9__40_0 = new Func<string, int, <>f__AnonymousType4<int, string>>(<>c.<>9, this.<GetFsoDyct>b__40_0))), <>c.<>9__40_1 ?? (<>c.<>9__40_1 = new Func<<>f__AnonymousType4<int, string>, int>(<>c.<>9, this.<GetFsoDyct>b__40_1)), <>c.<>9__40_2 ?? (<>c.<>9__40_2 = new Func<<>f__AnonymousType4<int, string>, string>(<>c.<>9, this.<GetFsoDyct>b__40_2)));
        }

        public string GetFsoJson(FileSystemInfo nfo)
        {
            this._jsonSerializer.Serialize(this._stringWriter, this.GetFsoDyct(nfo));
            this._stringWriter.Flush();
            this._stringWriter.Close();
            return this._sb.ToString();
        }

        public IEnumerable<object> GetFsoLyst(DirectoryInfo nfo)
        {
            List<object> list = new List<object>();
            this.GetFsoDyct(nfo);
            IEnumerable<FileSystemInfo> source = null;
            try
            {
                source = Enumerable.OrderBy<FileSystemInfo, string>(nfo.GetFileSystemInfos(), <>c.<>9__42_0 ?? (<>c.<>9__42_0 = new Func<FileSystemInfo, string>(<>c.<>9, this.<GetFsoLyst>b__42_0)));
            }
            catch (Exception)
            {
            }
            if ((source != null) && Enumerable.Any<FileSystemInfo>(source))
            {
                throw new Exception();
            }
            return list;
        }

        public static Func<T, bool> GetFuncAll<T>(IEnumerable<Expression<Func<T, bool>>> args)
        {
            <>c__DisplayClass45_0<T> class_1;
            return new Func<T, bool>(class_1, this.<GetFuncAll>b__0);
        }

        public string GetJsonStrng<T>(IEnumerable<T> ts)
        {
            this._jsonSerializer.Serialize(this._stringWriter, ts);
            this._stringWriter.Flush();
            this._stringWriter.Close();
            return this._sb.ToString();
        }

        public Record GetWurd(string s)
        {
            if (Xtnz.IsNullOrEmpty<Table>(this.Wurds.SelectRecords(new FilterExpression("Val", s, EqualityEnum.Equal))))
            {
                object[] values = new object[] { this._wurds.Count, s };
                Record item = new Record(this._wurds.Fields, values);
                this._wurds.Add(item);
                return item;
            }
            return this._wurds[0];
        }

        public static Expression<Func<T, bool>> IsTruAndTru<T>(Expression<Func<T, bool>> e1, Func<T, bool> e2)
        {
            <>c__DisplayClass52_0<T> class_;
            ParameterExpression expression;
            Expression[] arguments = new Expression[] { expression = Expression.Parameter(typeof(T), "nfo") };
            Expression[] expressionArray2 = new Expression[] { expression };
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(Expression.Invoke(Expression.Call(Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass52_0<T>)), fieldof(<>c__DisplayClass52_0<T>.e1, <>c__DisplayClass52_0<T>)), (MethodInfo) methodof(Expression<Func<T, bool>>.Compile, Expression<Func<T, bool>>), new Expression[0]), arguments), Expression.Invoke(Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass52_0<T>)), fieldof(<>c__DisplayClass52_0<T>.e2, <>c__DisplayClass52_0<T>)), expressionArray2)), parameters);
        }

        public static Expression<Func<T, bool>> IsTyp<T>(Type typ)
        {
            return IsTypExprT<T>().Compile().Invoke(typ);
        }

        public static Expression<Func<T, bool>> IsTypAndAll<T>(Type typ, IEnumerable<Expression<Func<T, bool>>> args)
        {
            return IsTypAndFF<T>(typ).Compile().Invoke(GetFuncAll<T>(args));
        }

        public static Expression<Func<T, bool>> IsTypAndAll<T>(Type typ, params Expression<Func<T, bool>>[] args)
        {
            return IsTypAndFF<T>(typ).Compile().Invoke(GetFuncAll<T>(args));
        }

        public static Expression<Func<Func<T, bool>, Expression<Func<T, bool>>>> IsTypAndFF<T>(Type typ)
        {
            <>c__DisplayClass48_0<T> class_;
            ParameterExpression expression;
            Expression[] arguments = new Expression[] { Expression.Field(Expression.Constant(class_, typeof(<>c__DisplayClass48_0<T>)), fieldof(<>c__DisplayClass48_0<T>.typ, <>c__DisplayClass48_0<T>)), expression = Expression.Parameter(typeof(Func<T, bool>), "e1") };
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return Expression.Lambda<Func<Func<T, bool>, Expression<Func<T, bool>>>>(Expression.Invoke(Expression.Call(Expression.Call(null, (MethodInfo) methodof(FyleDb.XprsnIsTypAnd), new Expression[0]), (MethodInfo) methodof(Expression<Func<Type, Func<T, bool>, Expression<Func<T, bool>>>>.Compile, Expression<Func<Type, Func<T, bool>, Expression<Func<T, bool>>>>), new Expression[0]), arguments), parameters);
        }

        public static Expression<Func<Type, Expression<Func<T, bool>>>> IsTypExprT<T>()
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            ParameterExpression[] parameters = new ParameterExpression[] { expression2 };
            ParameterExpression[] expressionArray2 = new ParameterExpression[] { expression };
            return Expression.Lambda<Func<Type, Expression<Func<T, bool>>>>(Expression.Quote(Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Call(expression2 = Expression.Parameter(typeof(T), "t"), (MethodInfo) methodof(object.GetType), new Expression[0]), expression = Expression.Parameter(typeof(Type), "typ"), false, (MethodInfo) methodof(Type.op_Equality)), parameters)), expressionArray2);
        }

        public void Save(FileInfo nfo)
        {
            if (!nfo.Exists)
            {
                base.Create(nfo.FullName, Enumerable.ToArray<Field>(Enumerable.Concat<Field>(new Field[0], new Field[0])));
            }
            this._wurds.SaveToDb(nfo.FullName);
        }

        public static Expression<Func<Type, Func<T, bool>, Expression<Func<T, bool>>>> XprsnIsTypAnd<T>()
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            Expression[] arguments = new Expression[2];
            Expression[] expressionArray2 = new Expression[] { expression = Expression.Parameter(typeof(Type), "t") };
            arguments[0] = Expression.Call(null, (MethodInfo) methodof(FyleDb.IsTyp), expressionArray2);
            arguments[1] = expression2 = Expression.Parameter(typeof(Func<T, bool>), "e1");
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
            return Expression.Lambda<Func<Type, Func<T, bool>, Expression<Func<T, bool>>>>(Expression.Call(null, (MethodInfo) methodof(FyleDb.IsTruAndTru), arguments), parameters);
        }

        public static Expression<Func<Func<T, string>, Func<string, string, bool>, string, Func<T, bool>>> XprsnObjTestr<T>()
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            ParameterExpression expression3;
            ParameterExpression expression4;
            Expression[] arguments = new Expression[2];
            Expression[] expressionArray2 = new Expression[] { expression4 = Expression.Parameter(typeof(T), "x") };
            arguments[0] = Expression.Invoke(expression = Expression.Parameter(typeof(Func<T, string>), "fCnvrt"), expressionArray2);
            arguments[1] = expression3 = Expression.Parameter(typeof(string), "s");
            ParameterExpression[] parameters = new ParameterExpression[] { expression4 };
            ParameterExpression[] expressionArray4 = new ParameterExpression[] { expression, expression2, expression3 };
            return Expression.Lambda<Func<Func<T, string>, Func<string, string, bool>, string, Func<T, bool>>>(Expression.Lambda<Func<T, bool>>(Expression.Invoke(expression2 = Expression.Parameter(typeof(Func<string, string, bool>), "fTstr"), arguments), parameters), expressionArray4);
        }

        public Table Wurds
        {
            get
            {
                if (this._wurds == null)
                {
                    Fields fields = new Fields();
                    Field field = new Field("Id", DataTypeEnum.Int, 0) {
                        IsPrimaryKey = true,
                        AutoIncStart = 0
                    };
                    fields.Add(field);
                    fields.Add(new Field("Val", DataTypeEnum.String));
                    this._wurds = new Table(fields, new object[0][], true);
                }
                return this._wurds;
            }
            set
            {
                this._wurds = value;
            }
        }

        public Table Itymz
        {
            get
            {
                if (this._itymz == null)
                {
                    Fields fields = new Fields();
                    fields.Add(new Field("Id", DataTypeEnum.Int));
                    fields.Add(new Field("Idc", DataTypeEnum.Int));
                    fields.Add(new Field("Idw", DataTypeEnum.Int));
                    fields.Add(new Field("Idx", DataTypeEnum.Int));
                    fields.Add(new Field("nfo", DataTypeEnum.String));
                    this._wurds = new Table(fields, new object[0][], true);
                }
                return this._itymz;
            }
            set
            {
                this._itymz = value;
            }
        }

        public object Obj
        {
            get
            {
                return this.obj;
            }
            set
            {
                this.obj = value;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FyleDb.<>c <>9 = new FyleDb.<>c();
            public static Func<string, int, <>f__AnonymousType4<int, string>> <>9__40_0;
            public static Func<<>f__AnonymousType4<int, string>, int> <>9__40_1;
            public static Func<<>f__AnonymousType4<int, string>, string> <>9__40_2;
            public static Func<FileSystemInfo, string> <>9__42_0;

            internal int <.cctor>b__53_0(KV<int, KV<int, string>> o)
            {
                return o().Kee;
            }

            internal int <.cctor>b__53_1(IEnumerable<KV<int, KV<int, string>>> q)
            {
                return Enumerable.Max<KV<int, KV<int, string>>>(q, FyleDb.FunkFsoInt);
            }

            internal Func<FileSystemInfo, bool> <.cctor>b__53_10(Expression<Func<FileSystemInfo, string>> fcnvrt, Expression<Func<string, string, bool>> fcmpr, string ss)
            {
                return FyleDb.XprsnObjTestr<FileSystemInfo>().Compile().Invoke(fcnvrt.Compile(), fcmpr.Compile(), ss);
            }

            internal Func<FileSystemInfo, bool> <.cctor>b__53_11(string ss, Expression<Func<string, string, bool>> c)
            {
                return FyleDb.tests010ff.Invoke(FyleDb.FuncFileNm, c, ss);
            }

            internal Func<FileSystemInfo, bool> <.cctor>b__53_12(string ss)
            {
                ParameterExpression expression;
                ParameterExpression expression2;
                Expression[] arguments = new Expression[] { expression2 = Expression.Parameter(typeof(string), "s1") };
                ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
                return FyleDb.tests010f.Invoke(ss, Expression.Lambda<Func<string, string, bool>>(Expression.Call(expression = Expression.Parameter(typeof(string), "s"), (MethodInfo) methodof(string.StartsWith), arguments), parameters));
            }

            internal Func<FileSystemInfo, bool> <.cctor>b__53_13(string ss)
            {
                ParameterExpression expression;
                ParameterExpression expression2;
                Expression[] arguments = new Expression[] { expression2 = Expression.Parameter(typeof(string), "s1") };
                ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
                return FyleDb.tests010f.Invoke(ss, Expression.Lambda<Func<string, string, bool>>(Expression.Call(expression = Expression.Parameter(typeof(string), "s"), (MethodInfo) methodof(string.Contains), arguments), parameters));
            }

            internal Func<FileSystemInfo, bool> <.cctor>b__53_14(string ss)
            {
                ParameterExpression expression;
                ParameterExpression expression2;
                Expression[] arguments = new Expression[] { expression2 = Expression.Parameter(typeof(string), "s1") };
                ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
                return FyleDb.tests010f.Invoke(ss, Expression.Lambda<Func<string, string, bool>>(Expression.Call(expression = Expression.Parameter(typeof(string), "s"), (MethodInfo) methodof(string.EndsWith), arguments), parameters));
            }

            internal Func<FileSystemInfo, bool> <.cctor>b__53_15(string ss)
            {
                ParameterExpression expression;
                ParameterExpression expression2;
                Expression[] arguments = new Expression[] { expression = Expression.Parameter(typeof(string), "s"), expression2 = Expression.Parameter(typeof(string), "s1") };
                ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
                return FyleDb.tests010f.Invoke(ss, Expression.Lambda<Func<string, string, bool>>(Expression.Call(null, (MethodInfo) methodof(Regex.IsMatch), arguments), parameters));
            }

            internal KV<int, KV<int, string>> <.cctor>b__53_2(FileSystemInfo o, int pi, int i)
            {
                return FyleDb.FunkStrKV.Invoke(Path.GetFileName(o.FullName), pi, i);
            }

            internal IEnumerable<int> <.cctor>b__53_3(IEnumerable<KV<int, KV<int, string>>> q)
            {
                return Enumerable.Select<KV<int, KV<int, string>>, int>(q, FyleDb.FunkFsoInt);
            }

            internal IEnumerable<KV<int, KV<int, string>>> <.cctor>b__53_4(IEnumerable<FileSystemInfo> q, int i)
            {
                FyleDb.<>c__DisplayClass53_0 class_;
                return Enumerable.Select<FileSystemInfo, KV<int, KV<int, string>>>(q, new Func<FileSystemInfo, int, KV<int, KV<int, string>>>(class_, this.<.cctor>b__5));
            }

            internal Func<string, string[]> <.cctor>b__53_6(string cb)
            {
                FyleDb.<>c__DisplayClass53_1 class_1;
                return new Func<string, string[]>(class_1, this.<.cctor>b__7);
            }

            internal string <.cctor>b__53_8(FileSystemInfo info)
            {
                return Path.GetFileNameWithoutExtension(info.FullName);
            }

            internal bool <.cctor>b__53_9(string s1, string s2)
            {
                return s1.Equals(s2, StringComparison.OrdinalIgnoreCase);
            }

            internal <>f__AnonymousType4<int, string> <GetFsoDyct>b__40_0(string o, int i)
            {
                return new { 
                    i = i,
                    o = o
                };
            }

            internal int <GetFsoDyct>b__40_1(<>f__AnonymousType4<int, string> o)
            {
                return o.i;
            }

            internal string <GetFsoDyct>b__40_2(<>f__AnonymousType4<int, string> o)
            {
                return o.o;
            }

            internal string <GetFsoLyst>b__42_0(FileSystemInfo d)
            {
                return d.FullName;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__45<T>
        {
            public static readonly FyleDb.<>c__45<T> <>9;
            public static Func<Expression<Func<T, bool>>, int, <>f__AnonymousType5<Expression<Func<T, bool>>, int>> <>9__45_1;

            static <>c__45()
            {
                FyleDb.<>c__45<T>.<>9 = new FyleDb.<>c__45<T>();
            }

            internal <>f__AnonymousType5<Expression<Func<T, bool>>, int> <GetFuncAll>b__45_1(Expression<Func<T, bool>> t, int i)
            {
                return new { 
                    t = t,
                    i = i
                };
            }
        }
    }
}

