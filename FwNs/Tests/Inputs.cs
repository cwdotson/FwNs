namespace FwNs.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    public static class Inputs
    {
        public static bool GetBoolF()
        {
            return !GetBoolT();
        }

        public static bool GetBoolFalse()
        {
            return false;
        }

        public static bool GetBoolT()
        {
            return !GetBoolFalse();
        }

        public static KeyValuePair<int, XNoadItm>[] XNoadKeyValus()
        {
            return new KeyValuePair<int, XNoadItm>[] { new KeyValuePair<int, XNoadItm>(0, null), new KeyValuePair<int, XNoadItm>(0, null) };
        }

        private class KydKlktn : Inputs.KydKlktn<IComparable>
        {
            public KydKlktn(Func<object, IComparable> val2KeyFunc) : base(val2KeyFunc, <>c.<>9__0_0 ?? (<>c.<>9__0_0 = new Func<IComparable, IComparable, int>(<>c.<>9, this.<.ctor>b__0_0)))
            {
            }

            [Serializable, CompilerGenerated]
            private sealed class <>c
            {
                public static readonly Inputs.KydKlktn.<>c <>9 = new Inputs.KydKlktn.<>c();
                public static Func<IComparable, IComparable, int> <>9__0_0;

                internal int <.ctor>b__0_0(IComparable tki, IComparable tkj)
                {
                    return tki.CompareTo(tkj);
                }
            }
        }

        private class KydKlktn<TK> : Inputs.KydKlktn<TK, object>
        {
            public KydKlktn(Func<object, TK> val2KeyFunc, Func<TK, TK, int> cmprFunc) : base(val2KeyFunc, cmprFunc)
            {
            }

            protected override TK GetKeyForItem(object item)
            {
                return base.GetKeyForItem(item ?? null);
            }
        }

        private class KydKlktn<TK, TV> : Inputs.KydKlktnBase<TK, TV>
        {
            public KydKlktn(Func<TV, TK> val2KeyFunc, Func<TK, TK, int> cmprFunc) : base(val2KeyFunc, cmprFunc)
            {
            }
        }

        public abstract class KydKlktnBase<TK, TV> : KeyedCollection<TK, TV>
        {
            public KydKlktnBase(Func<TV, TK> val2KeyFunc, Func<TK, TK, int> cmprFunc)
            {
                this.Val2KeyFunc = val2KeyFunc;
                this.CmprFunc = cmprFunc;
            }

            protected override TK GetKeyForItem(TV item)
            {
                if (this.Val2KeyFunc == null)
                {
                    throw new Exception();
                }
                return this.Val2KeyFunc.Invoke(item);
            }

            public Func<TK, TK, int> CmprFunc { get; set; }

            public Func<TV, TK> Val2KeyFunc { get; set; }
        }

        [DataContract(Name="XNoad")]
        public class XNoadItm : IExtensibleDataObject
        {
            private KeyedCollection<int, Inputs.XNoadItm> itms;

            public XNoadItm() : this(null)
            {
            }

            public XNoadItm(int? id) : this(nullable.HasValue ? nullable.GetValueOrDefault() : 0, null)
            {
                int? nullable = id;
            }

            public XNoadItm(int id, string nm) : this(id, nm, null)
            {
            }

            public XNoadItm(object id, string nm, string prfx)
            {
                prfx = prfx ?? "";
                this.IdXn = (id == null) ? ((long) 0) : ((long) Convert.ToInt32(id));
                this.Nm = nm ?? "";
            }

            [DataMember(Name="Id")]
            public long IdXn { get; set; }

            [DataMember(Name="Prfx")]
            public string Prfx { get; set; }

            [DataMember(Name="Nm")]
            public string Nm { get; set; }

            public ExtensionDataObject ExtensionData
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public virtual KeyedCollection<int, Inputs.XNoadItm> Itms
            {
                get
                {
                    if (this.itms == null)
                    {
                        KeyedCollection<IComparable, object> keyeds = new Inputs.KydKlktn(<>c.<>9__21_0 ?? (<>c.<>9__21_0 = new Func<object, IComparable>(<>c.<>9, this.<get_Itms>b__21_0)));
                        this.itms = (KeyedCollection<int, Inputs.XNoadItm>) keyeds;
                    }
                    return this.itms;
                }
                set
                {
                    this.itms = value;
                }
            }

            [Serializable, CompilerGenerated]
            private sealed class <>c
            {
                public static readonly Inputs.XNoadItm.<>c <>9 = new Inputs.XNoadItm.<>c();
                public static Func<object, IComparable> <>9__21_0;

                internal IComparable <get_Itms>b__21_0(object xnd)
                {
                    return (int) ((Inputs.XNoadItm) xnd).IdXn;
                }
            }
        }
    }
}

