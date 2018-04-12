namespace FwNs
{
    using FwNs.Txt.JSon;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Xml.Linq;

    public class KeyVal<TK, TV, TC> : IKeyVal<TK, TV, TC>, IEquatable<KeyValuePair<TK, TV>>, IComparable<KeyValuePair<TK, TV>>, IEquatable<IKeyVal<TK, TV, TC>>, IComparable<IKeyVal<TK, TV, TC>>, IEquatable<Tuple<TK, TV, TC>>, IComparable<Tuple<TK, TV, TC>>, IComparable where TC: IComparable
    {
        private KeyValuePair<TK, TV> _kvp;

        [field: CompilerGenerated]
        public event EventHandler ValChnged;

        public KeyVal(KeyValuePair<TK, TV> kvp) : this(kvp, (Func<TK, TC>) null)
        {
        }

        public KeyVal(KeyValuePair<TK, TV> kvp, Func<TK, TC> key2CmprbleFunc)
        {
            this._kvp = kvp;
            if (key2CmprbleFunc == null)
            {
                this.Key2CmprbleFunc = <>c<TK, TV, TC>.<>9__35_0 ?? (<>c<TK, TV, TC>.<>9__35_0 = new Func<TK, TC>(<>c<TK, TV, TC>.<>9, this.<.ctor>b__35_0));
            }
            this.Key2CmprbleFunc = key2CmprbleFunc;
            this.tK2Stck = new Stack<Func<TK, string>>();
            this.tK2Stck.Push(<>c<TK, TV, TC>.<>9__35_1 ?? (<>c<TK, TV, TC>.<>9__35_1 = new Func<TK, string>(<>c<TK, TV, TC>.<>9, this.<.ctor>b__35_1)));
            this.tV2Stck = new Stack<Func<TV, string>>();
            this.tV2Stck.Push(<>c<TK, TV, TC>.<>9__35_2 ?? (<>c<TK, TV, TC>.<>9__35_2 = new Func<TV, string>(<>c<TK, TV, TC>.<>9, this.<.ctor>b__35_2)));
            this.ToStrngStck = new Stack<Func<IKeyVal<TK, TV, TC>, string>>();
            this.ToStrngStck.Push(<>c<TK, TV, TC>.<>9__35_3 ?? (<>c<TK, TV, TC>.<>9__35_3 = new Func<IKeyVal<TK, TV, TC>, string>(<>c<TK, TV, TC>.<>9, this.<.ctor>b__35_3)));
            this.ToStrngStck.Push(<>c<TK, TV, TC>.<>9__35_4 ?? (<>c<TK, TV, TC>.<>9__35_4 = new Func<IKeyVal<TK, TV, TC>, string>(<>c<TK, TV, TC>.<>9, this.<.ctor>b__35_4)));
            this.ToStrngStck.Push(<>c<TK, TV, TC>.<>9__35_5 ?? (<>c<TK, TV, TC>.<>9__35_5 = new Func<IKeyVal<TK, TV, TC>, string>(<>c<TK, TV, TC>.<>9, this.<.ctor>b__35_5)));
            this.ToStrngStck.Push(<>c<TK, TV, TC>.<>9__35_6 ?? (<>c<TK, TV, TC>.<>9__35_6 = new Func<IKeyVal<TK, TV, TC>, string>(<>c<TK, TV, TC>.<>9, this.<.ctor>b__35_6)));
        }

        public KeyVal(KeyValuePair<TK, TV> kvp, TC tC) : this(kvp, new Func<TK, TC>(class_, this.<.ctor>b__0))
        {
        }

        public KeyVal(TK tK, TV tV) : this(new KeyValuePair<TK, TV>(tK, tV))
        {
        }

        public KeyVal(TK tK, TV tV, Func<TK, TC> key2CmprbleFunc) : this(new KeyValuePair<TK, TV>(tK, tV), key2CmprbleFunc)
        {
        }

        public KeyVal(TK tK, TV tV, TC tC) : this(new KeyValuePair<TK, TV>(tK, tV), new Func<TK, TC>(class_, this.<.ctor>b__0))
        {
        }

        public Dictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object> { 
                { 
                    "Tc",
                    ((TC) this)
                },
                { 
                    "Key",
                    this.Key
                },
                { 
                    "Value",
                    this.Value
                }
            };
        }

        public KeyValuePair<TC, TX> AsKeyValuePair<TX>(Func<IKeyVal<TK, TV, TC>, TX> kv2txf)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<TA, TB> AsKvp<TA, TB, TP>() where TP: KeyVal<TK, TV, TC>, TA, TB
        {
            return this.AsKvp<TA, TB, KeyVal<TK, TV, TC>>(<>c__56<TK, TV, TC, TA, TB, TP>.<>9__56_0 ?? (<>c__56<TK, TV, TC, TA, TB, TP>.<>9__56_0 = new Func<KeyVal<TK, TV, TC>, TA>(<>c__56<TK, TV, TC, TA, TB, TP>.<>9, this.<AsKvp>b__56_0)), <>c__56<TK, TV, TC, TA, TB, TP>.<>9__56_1 ?? (<>c__56<TK, TV, TC, TA, TB, TP>.<>9__56_1 = new Func<KeyVal<TK, TV, TC>, TB>(<>c__56<TK, TV, TC, TA, TB, TP>.<>9, this.<AsKvp>b__56_1)));
        }

        public KeyValuePair<TC, TX> AsKvp<TX>() where TX: IKeyVal<TK, TV, TC>
        {
            throw new Exception();
        }

        public KeyValuePair<TK, TV> AsKvp()
        {
            return (KeyValuePair<TK, TV>) this;
        }

        public KeyValuePair<TK, TB> AsKvp<TB>(Func<KeyVal<TK, TV, TC>, TB> ftP2tB)
        {
            return this.AsKvp<TK, TB, KeyVal<TK, TV, TC>>(<>c__55<TK, TV, TC, TB>.<>9__55_0 ?? (<>c__55<TK, TV, TC, TB>.<>9__55_0 = new Func<KeyVal<TK, TV, TC>, TK>(<>c__55<TK, TV, TC, TB>.<>9, this.<AsKvp>b__55_0)), ftP2tB);
        }

        public KeyValuePair<TA, TB> AsKvp<TA, TB, TP>(Func<TP, TA> ftP2tA, Func<TP, TB> ftP2tB) where TP: KeyVal<TK, TV, TC>
        {
            return new KeyValuePair<TA, TB>(ftP2tA.Invoke((TP) this), ftP2tB.Invoke((TP) this));
        }

        public KeyValuePair<TC, Dictionary<string, object>> AsKvpDict()
        {
            throw new Exception();
        }

        public KeyValuePair<TC, KeyValuePair<TK, TV>> AsKvpKvp()
        {
            return new KeyValuePair<TC, KeyValuePair<TK, TV>>((TC) this, (KeyValuePair<TK, TV>) this);
        }

        public XElement AsXElement()
        {
            XElement element = new XElement("KeyVal", new XAttribute("TC", this));
            try
            {
                element.SetValue(new XElement("TC", Enumerable.Select<KeyValuePair<string, object>, XElement>(this.AsDictionary(), <>c<TK, TV, TC>.<>9__50_1 ?? (<>c<TK, TV, TC>.<>9__50_1 = new Func<KeyValuePair<string, object>, XElement>(<>c<TK, TV, TC>.<>9, this.<AsXElement>b__50_1)))));
            }
            catch (Exception)
            {
                XElement element2 = new XElement("Key", this.Key);
                XElement element3 = new XElement("Value", this.Value);
                object[] content = new object[] { element2, element3 };
                element.Add(content);
            }
            return element;
        }

        public int CompareTo(TC other)
        {
            TC local = (TC) this;
            return local.CompareTo(other);
        }

        public int CompareTo(IKeyVal<TK, TV, TC> other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(KeyValuePair<TK, TV> other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new Exception();
            }
            Func<object, int> func = null;
            if (obj.GetType().IsGenericType)
            {
                <>c__DisplayClass60_2<TK, TV, TC> class_;
                <>c__DisplayClass60_0<TK, TV, TC> class_2;
                MethodInfo mthd = null;
                Type[] source = obj.GetType().get_GenericTypeArguments();
                Type genericTypeDefinition = obj.GetType().GetGenericTypeDefinition();
                Func<object, Type> o2tf = <>c<TK, TV, TC>.<>9__60_0 ?? (<>c<TK, TV, TC>.<>9__60_0 = new Func<object, Type>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_0));
                Func<object, Func<Type, Type, bool>, Func<Type, bool>> func2 = new Func<object, Func<Type, Type, bool>, Func<Type, bool>>(class_2, this.<CompareTo>b__1);
                Func<Type, Type, bool> func3 = null;
                if (obj.GetType().get_IsConstructedGenericType())
                {
                    func3 = <>c<TK, TV, TC>.<>9__60_3 ?? (<>c<TK, TV, TC>.<>9__60_3 = new Func<Type, Type, bool>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_3));
                    if (3 < Enumerable.Count<Type>(source))
                    {
                        if (!Enumerable.Any<Type>(source, <>c<TK, TV, TC>.<>9__60_4 ?? (<>c<TK, TV, TC>.<>9__60_4 = new Func<Type, bool>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_4))))
                        {
                            throw new Exception();
                        }
                        Enumerable.ToArray<Type>(Enumerable.OrderBy<Type, string>(Enumerable.Where<Type>(source, <>c<TK, TV, TC>.<>9__60_5 ?? (<>c<TK, TV, TC>.<>9__60_5 = new Func<Type, bool>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_5))), <>c<TK, TV, TC>.<>9__60_6 ?? (<>c<TK, TV, TC>.<>9__60_6 = new Func<Type, string>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_6))));
                        throw new Exception();
                    }
                    if (Enumerable.Count<Type>(source) == 3)
                    {
                        Type[] types = new Type[] { obj.GetType() };
                        mthd = base.GetType().GetMethod("CompareTo", types);
                    }
                    else if (Enumerable.Count<Type>(source) == 2)
                    {
                        if (obj is IComparable)
                        {
                            Type type2 = genericTypeDefinition.MakeGenericType(source);
                            Func<Type, bool> func4 = func2.Invoke(type2, func3);
                            if ((func4.Invoke(typeof(IKeyVal<,,>)) || func4.Invoke(typeof(KeyVal<,,>))) || func4.Invoke(typeof(IKv<,>)))
                            {
                                if (!func4.Invoke(typeof(IKv<,>)) && func4.Invoke(typeof(IKeyVal<,>)))
                                {
                                }
                            }
                            else
                            {
                                func = new Func<object, int>(class_, this.<CompareTo>b__7);
                            }
                        }
                        else if (!typeof(TC).IsAssignableFrom(source[0]))
                        {
                            if (typeof(TC).IsAssignableFrom(source[0]))
                            {
                                return ((KeyVal<TK, TV, TC>) obj).CompareTo((KeyValuePair<TK, TV>) this);
                            }
                            if (!(obj is IComparable))
                            {
                                throw new Exception();
                            }
                        }
                        else if (genericTypeDefinition.IsAssignableFrom(typeof(IKv<,>)) || !genericTypeDefinition.IsAssignableFrom(typeof(KeyValuePair<,>)))
                        {
                        }
                    }
                    else if (Enumerable.Count<Type>(source) == 1)
                    {
                        if (genericTypeDefinition.IsAssignableFrom(typeof(IKv<,>)))
                        {
                            func = new Func<object, int>(this, this.<CompareTo>b__60_8);
                            throw new Exception();
                        }
                        if (!(obj is IComparable))
                        {
                            throw new Exception();
                        }
                        func = new Func<object, int>(class_, this.<CompareTo>b__9);
                    }
                    if (mthd != null)
                    {
                        func = new Func<object, int>(class_2, this.<CompareTo>b__10);
                    }
                }
                else
                {
                    func3 = <>c<TK, TV, TC>.<>9__60_11 ?? (<>c<TK, TV, TC>.<>9__60_11 = new Func<Type, Type, bool>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_11));
                    if (3 < Enumerable.Count<Type>(source))
                    {
                        if (!Enumerable.Any<Type>(source, <>c<TK, TV, TC>.<>9__60_12 ?? (<>c<TK, TV, TC>.<>9__60_12 = new Func<Type, bool>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_12))))
                        {
                            throw new Exception();
                        }
                        Enumerable.ToArray<Type>(Enumerable.OrderBy<Type, string>(Enumerable.Where<Type>(source, <>c<TK, TV, TC>.<>9__60_13 ?? (<>c<TK, TV, TC>.<>9__60_13 = new Func<Type, bool>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_13))), <>c<TK, TV, TC>.<>9__60_14 ?? (<>c<TK, TV, TC>.<>9__60_14 = new Func<Type, string>(<>c<TK, TV, TC>.<>9, this.<CompareTo>b__60_14))));
                        throw new Exception();
                    }
                    if (Enumerable.Count<Type>(source) == 3)
                    {
                        Type[] types = new Type[] { obj.GetType() };
                        mthd = base.GetType().GetMethod("CompareTo", types);
                    }
                    else if (Enumerable.Count<Type>(source) == 2)
                    {
                        if (obj is IComparable)
                        {
                            if ((obj is IKeyVal<TK, TV, TC>) || genericTypeDefinition.IsAssignableFrom(typeof(IKv<,>)))
                            {
                                if (!genericTypeDefinition.IsAssignableFrom(typeof(IKv<,,>)))
                                {
                                    genericTypeDefinition.IsAssignableFrom(typeof(IKeyVal<,>));
                                }
                                return ((KeyVal<TK, TV, TC>) obj).CompareTo((KeyValuePair<TK, TV>) this);
                            }
                            func = new Func<object, int>(class_, this.<CompareTo>b__15);
                        }
                        else if (!typeof(TC).IsAssignableFrom(source[0]))
                        {
                            if (typeof(TC).IsAssignableFrom(source[0]))
                            {
                                return ((KeyVal<TK, TV, TC>) obj).CompareTo((KeyValuePair<TK, TV>) this);
                            }
                        }
                        else
                        {
                            if (genericTypeDefinition.IsAssignableFrom(typeof(IKv<,>)))
                            {
                                return ((KeyVal<TK, TV, TC>) obj).CompareTo((KeyValuePair<TK, TV>) this);
                            }
                            if (genericTypeDefinition.IsAssignableFrom(typeof(KeyValuePair<,>)))
                            {
                                return ((KeyVal<TK, TV, TC>) obj).CompareTo((KeyValuePair<TK, TV>) this);
                            }
                        }
                    }
                    else if (Enumerable.Count<Type>(source) == 1)
                    {
                        if (genericTypeDefinition.IsAssignableFrom(typeof(IKv<,>)))
                        {
                            return ((KeyVal<TK, TV, TC>) obj).CompareTo((KeyValuePair<TK, TV>) this);
                        }
                    }
                    else
                    {
                        func = new Func<object, int>(this, this.<CompareTo>b__60_16);
                    }
                    if (mthd != null)
                    {
                        func = new Func<object, int>(class_2, this.<CompareTo>b__17);
                    }
                }
                return func.Invoke(obj);
            }
            TC local = (TC) this;
            return obj.ToString().CompareTo(local.ToString());
        }

        public int CompareTo(Tuple<TK, TV, TC> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IKeyVal<TK, TV, TC> other)
        {
            if (!this.tKEqF.Invoke(other.Key, this.Key))
            {
                return false;
            }
            return this.tVEqF.Invoke(other.Value, this.Value);
        }

        public bool Equals(KeyValuePair<TK, TV> other)
        {
            if (!this.tKEqF.Invoke(other.Key, this.Key))
            {
                return false;
            }
            return this.tVEqF.Invoke(other.Value, this.Value);
        }

        public bool Equals(Tuple<TK, TV, TC> other)
        {
            throw new NotImplementedException();
        }

        public static implicit operator KeyValuePair<TK, TV>(KeyVal<TK, TV, TC> kv)
        {
            return new KeyValuePair<TK, TV>(kv.Key, kv.Value);
        }

        public static implicit operator Tuple<TC, TK, TV>(KeyVal<TK, TV, TC> kv)
        {
            return new Tuple<TC, TK, TV>((TC) kv, kv.Key, kv.Value);
        }

        public static implicit operator TC(KeyVal<TK, TV, TC> kv)
        {
            return kv.Key2CmprbleFunc.Invoke(kv.Key);
        }

        public static implicit operator KeyVal<TK, TV, TC>(KeyValuePair<TK, TV> kvp)
        {
            return new KeyVal<TK, TV, TC>(kvp);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void vChngedHndlr()
        {
            if (this.ValChnged != null)
            {
                this.ValChnged(this, new EventArgs());
            }
        }

        private Func<KeyVal<TK, TV, TC>, TK, TV, bool> KvKVEqF
        {
            get
            {
                return new Func<KeyVal<TK, TV, TC>, TK, TV, bool>(this, this.<get_KvKVEqF>b__1_0);
            }
        }

        private Func<KeyVal<TK, TV, TC>, KeyVal<TK, TV, TC>, bool> KvKvEqF
        {
            get
            {
                return new Func<KeyVal<TK, TV, TC>, KeyVal<TK, TV, TC>, bool>(this, this.<get_KvKvEqF>b__3_0);
            }
        }

        private Stack<Func<TK, string>> tK2Stck { get; set; }

        private Func<TK, TK, bool> tKEqF { get; set; }

        private Func<TV, TV, bool> tVEqF { get; set; }

        public Stack<Func<IKeyVal<TK, TV, TC>, string>> ToStrngStck { get; set; }

        public Stack<Func<TV, string>> tV2Stck { get; set; }

        public Func<TK, TC> Key2CmprbleFunc { get; set; }

        public Func<DataTable, DataRow> Kv2Drf
        {
            get
            {
                return new Func<DataTable, DataRow>(this, this.<get_Kv2Drf>b__33_0);
            }
        }

        public TK Key
        {
            get
            {
                return this._kvp.Key;
            }
        }

        public TV Value
        {
            get
            {
                return this._kvp.Value;
            }
            set
            {
                if (!this._kvp.Value.Equals(value))
                {
                    KeyVal<TK, TV, TC> val = new KeyVal<TK, TV, TC>(this.Key, value);
                    this._kvp = (KeyValuePair<TK, TV>) val;
                }
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly KeyVal<TK, TV, TC>.<>c <>9;
            public static Func<Type, Func<DataColumn, bool>> <>9__33_4;
            public static Func<string[], Type, Func<DataColumn, bool>> <>9__33_3;
            public static Func<TK, TC> <>9__35_0;
            public static Func<TK, string> <>9__35_1;
            public static Func<TV, string> <>9__35_2;
            public static Func<IKeyVal<TK, TV, TC>, string> <>9__35_3;
            public static Func<IKeyVal<TK, TV, TC>, string> <>9__35_4;
            public static Func<IKeyVal<TK, TV, TC>, string> <>9__35_5;
            public static Func<IKeyVal<TK, TV, TC>, string> <>9__35_6;
            public static Func<IKeyVal<TK, TV, TC>, string> <>9__35_7;
            public static Func<IKeyVal<TK, TV, TC>, string> <>9__35_8;
            public static Func<IKeyVal<TK, TV, TC>, string> <>9__35_9;
            public static Func<IKeyVal<TK, TV, TC>, string> <>9__35_10;
            public static Func<TK, TK, bool> <>9__35_11;
            public static Func<TV, TV, bool> <>9__35_12;
            public static Func<KeyValuePair<string, object>, XElement> <>9__50_0;
            public static Func<KeyValuePair<string, object>, XElement> <>9__50_1;
            public static Func<object, Type> <>9__60_0;
            public static Func<Type, Type, bool> <>9__60_3;
            public static Func<Type, bool> <>9__60_4;
            public static Func<Type, bool> <>9__60_5;
            public static Func<Type, string> <>9__60_6;
            public static Func<Type, Type, bool> <>9__60_11;
            public static Func<Type, bool> <>9__60_12;
            public static Func<Type, bool> <>9__60_13;
            public static Func<Type, string> <>9__60_14;

            static <>c()
            {
                KeyVal<TK, TV, TC>.<>c.<>9 = new KeyVal<TK, TV, TC>.<>c();
            }

            internal TC <.ctor>b__35_0(TK tk)
            {
                TC local;
                try
                {
                    object[] parameters = new object[] { tk };
                    local = (TC) typeof(TC).TypeInitializer.Invoke(parameters);
                }
                catch (Exception)
                {
                    throw;
                }
                return local;
            }

            internal string <.ctor>b__35_1(TK k)
            {
                return k.ToString();
            }

            internal string <.ctor>b__35_10(IKeyVal<TK, TV, TC> kv)
            {
                throw new Exception();
            }

            internal bool <.ctor>b__35_11(TK keyM, TK keyO)
            {
                return keyM.Equals(keyO);
            }

            internal bool <.ctor>b__35_12(TV valM, TV valO)
            {
                return valM.Equals(valO);
            }

            internal string <.ctor>b__35_2(TV v)
            {
                return v.ToString();
            }

            internal string <.ctor>b__35_3(IKeyVal<TK, TV, TC> kv)
            {
                return kv.ToString();
            }

            internal string <.ctor>b__35_4(IKeyVal<TK, TV, TC> kv)
            {
                return Xtnz.ToJson(kv);
            }

            internal string <.ctor>b__35_5(IKeyVal<TK, TV, TC> kv)
            {
                return (kv.Key.ToString() + kv.Value.ToString());
            }

            internal string <.ctor>b__35_6(IKeyVal<TK, TV, TC> kv)
            {
                // This item is obfuscated and can not be translated.
                object[] objArray1 = new object[2, 2];
                objArray1[0, 0] = "Key";
                throw new Exception();
            }

            internal string <.ctor>b__35_7(IKeyVal<TK, TV, TC> kv)
            {
                string[] textArray1 = new string[] { "" };
                return ("{\r\n\t\t\"" + string.Join("\";\"", textArray1) + "\"\r\n\t}");
            }

            internal string <.ctor>b__35_8(IKeyVal<TK, TV, TC> kv)
            {
                object[] objArray1 = new object[5];
                objArray1[0] = "\t{\r\n\t\t\"";
                objArray1[2] = "};\r\n\t\t{";
                objArray1[4] = "\r\n\t}";
                return string.Concat(objArray1);
            }

            internal string <.ctor>b__35_9(IKeyVal<TK, TV, TC> kv)
            {
                // This item is obfuscated and can not be translated.
                throw new Exception();
            }

            internal XElement <AsXElement>b__50_0(KeyValuePair<string, object> kvx)
            {
                return new XElement(kvx.Key, kvx.Value);
            }

            internal XElement <AsXElement>b__50_1(KeyValuePair<string, object> kvx)
            {
                return new XElement(kvx.Key, kvx.Value);
            }

            internal Type <CompareTo>b__60_0(object oo)
            {
                throw new Exception();
            }

            internal bool <CompareTo>b__60_11(Type txx, Type tyy)
            {
                return txx.IsAssignableFrom(tyy);
            }

            internal bool <CompareTo>b__60_12(Type ga)
            {
                return typeof(TC).IsAssignableFrom(ga);
            }

            internal bool <CompareTo>b__60_13(Type ga)
            {
                return typeof(TC).IsAssignableFrom(ga);
            }

            internal string <CompareTo>b__60_14(Type pi)
            {
                return pi.Name;
            }

            internal bool <CompareTo>b__60_3(Type txx, Type tyy)
            {
                return txx.IsAssignableFrom(tyy);
            }

            internal bool <CompareTo>b__60_4(Type ga)
            {
                return typeof(TC).IsAssignableFrom(ga);
            }

            internal bool <CompareTo>b__60_5(Type ga)
            {
                return typeof(TC).IsAssignableFrom(ga);
            }

            internal string <CompareTo>b__60_6(Type pi)
            {
                return pi.Name;
            }

            internal Func<DataColumn, bool> <get_Kv2Drf>b__33_3(string[] nms, Type typ)
            {
                KeyVal<TK, TV, TC>.<>c__DisplayClass33_3 class_1;
                Func<DataColumn, bool> chkTypf = (KeyVal<TK, TV, TC>.<>c.<>9__33_4 ?? (KeyVal<TK, TV, TC>.<>c.<>9__33_4 = new Func<Type, Func<DataColumn, bool>>(KeyVal<TK, TV, TC>.<>c.<>9, this.<get_Kv2Drf>b__33_4))).Invoke(typ);
                return new Func<DataColumn, bool>(class_1, this.<get_Kv2Drf>b__6);
            }

            internal Func<DataColumn, bool> <get_Kv2Drf>b__33_4(Type typx)
            {
                KeyVal<TK, TV, TC>.<>c__DisplayClass33_2 class_1;
                return new Func<DataColumn, bool>(class_1, this.<get_Kv2Drf>b__5);
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__53<TX> where TX: IKeyVal<TK, TV, TC>
        {
            public static readonly KeyVal<TK, TV, TC>.<>c__53<TX> <>9;
            public static Func<Type, Type> <>9__53_0;

            static <>c__53()
            {
                KeyVal<TK, TV, TC>.<>c__53<TX>.<>9 = new KeyVal<TK, TV, TC>.<>c__53<TX>();
            }

            internal Type <AsKvp>b__53_0(Type ii)
            {
                if (!ii.IsGenericType)
                {
                    return null;
                }
                return ii.GetGenericTypeDefinition();
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__55<TB>
        {
            public static readonly KeyVal<TK, TV, TC>.<>c__55<TB> <>9;
            public static Func<KeyVal<TK, TV, TC>, TK> <>9__55_0;

            static <>c__55()
            {
                KeyVal<TK, TV, TC>.<>c__55<TB>.<>9 = new KeyVal<TK, TV, TC>.<>c__55<TB>();
            }

            internal TK <AsKvp>b__55_0(KeyVal<TK, TV, TC> xx)
            {
                return xx.Key;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c__56<TA, TB, TP> where TP: KeyVal<TK, TV, TC>, TA, TB
        {
            public static readonly KeyVal<TK, TV, TC>.<>c__56<TA, TB, TP> <>9;
            public static Func<KeyVal<TK, TV, TC>, TA> <>9__56_0;
            public static Func<KeyVal<TK, TV, TC>, TB> <>9__56_1;

            static <>c__56()
            {
                KeyVal<TK, TV, TC>.<>c__56<TA, TB, TP>.<>9 = new KeyVal<TK, TV, TC>.<>c__56<TA, TB, TP>();
            }

            internal TA <AsKvp>b__56_0(KeyVal<TK, TV, TC> xx)
            {
                return (TA) ((TP) xx);
            }

            internal TB <AsKvp>b__56_1(KeyVal<TK, TV, TC> xx)
            {
                return (TB) ((TP) xx);
            }
        }
    }
}

