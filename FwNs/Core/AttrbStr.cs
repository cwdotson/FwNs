namespace FwNs.Core
{
    using FwNs.Txt.JSon;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class AttrbStr : Attrb, IComparable
    {
        private object _itm;
        private Func<object, bool> _enumItmFunk;

        [field: CompilerGenerated]
        public event EventHandler OnEvnt;

        public AttrbStr(string val) : this(-1, "", "", val)
        {
        }

        public AttrbStr(string nm, string val) : this(-1, "{0},{1}", nm, val)
        {
        }

        public AttrbStr(int ndx, string nm, string val) : this(ndx, "{0},{1}", nm, val)
        {
        }

        public AttrbStr(string frmtStrng, string nm, string val) : this(-1, frmtStrng, nm, val)
        {
        }

        public AttrbStr(int ndx, string frmtStrng, string nm, string val) : base(nm, val)
        {
            this.Ndx = ndx;
            this.Delimtr = ":";
            this.FrmtStrng = frmtStrng;
            this.OnEvnt += new EventHandler(this.AttrbStrOnEvnt);
            this.FrmtFunk = new Func<string>(this, this.<.ctor>b__44_0);
            throw new Exception();
        }

        public void AttrbStrOnEvnt(object sender, EventArgs e)
        {
            if (((AttrbEvntArgs) e).EvntNm != "Cr8td")
            {
                throw new Exception();
            }
            if (((base.Val == "") || (base.Nm == "")) && ((base.Val + base.Nm) != ""))
            {
                if ((this.FrmtStrng == "{0},{1}") || (this.FrmtStrng == ""))
                {
                    this.FrmtStrng = "{0}";
                    if (base.Val == "")
                    {
                        this.FrmtFunk = new Func<string>(this, this.<AttrbStrOnEvnt>b__50_0);
                    }
                    else
                    {
                        this.FrmtFunk = new Func<string>(this, this.<AttrbStrOnEvnt>b__50_1);
                    }
                }
            }
            else if (this.FrmtStrng == "")
            {
                if ((base.Val != "") || (base.Nm != ""))
                {
                    this.FrmtStrng = "{0}" + this.Delimtr + "{1}";
                    this.FrmtFunk = new Func<string>(this, this.<AttrbStrOnEvnt>b__50_2);
                }
                else
                {
                    this.FrmtStrng = "{0}";
                    if ((base.Nm == "") && (base.Val != ""))
                    {
                        this.FrmtFunk = new Func<string>(this, this.<AttrbStrOnEvnt>b__50_3);
                    }
                    else
                    {
                        if ((base.Nm == "") || (base.Val != ""))
                        {
                            throw new Exception();
                        }
                        this.FrmtFunk = new Func<string>(this, this.<AttrbStrOnEvnt>b__50_4);
                    }
                }
            }
        }

        public int CompareTo(object obj)
        {
            try
            {
                return this.ToString().CompareTo(obj.ToString());
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public override string ToString()
        {
            string str;
            try
            {
                str = this.FrmtFunk.Invoke();
            }
            catch (Exception)
            {
                try
                {
                    str = FwNs.Txt.JSon.Xtnz.ToJsonStr(this.Ary);
                }
                catch (Exception)
                {
                    str = base.ToString();
                }
            }
            return str;
        }

        public int Ndx { get; set; }

        public Type EnmTarg { get; set; }

        public Type AttrbTarg { get; set; }

        public string Delimtr { get; set; }

        public string FrmtStrng { get; set; }

        public Func<object> ItmFunk { get; set; }

        public object Itm
        {
            get
            {
                if ((this._itm == null) && (this.ItmFunk != null))
                {
                    this._itm = this.ItmFunk.Invoke();
                }
                return this._itm;
            }
        }

        public Func<string> FrmtFunk { get; set; }

        public Func<AttrbStr> AttrbFunk
        {
            get
            {
                throw new Exception();
            }
        }

        public Func<string, bool> EnumItmMtch { get; set; }

        public Func<object, string> FrmtItmFunk { get; set; }

        public KeyValuePair<Type, Type> TypesKV
        {
            get
            {
                return new KeyValuePair<Type, Type>(this.EnmTarg, this.AttrbTarg);
            }
        }

        public Func<object, bool> EnumItmFunk
        {
            get
            {
                if (this._enumItmFunk == null)
                {
                    if (this.FrmtItmFunk == null)
                    {
                        this.FrmtItmFunk = <>c.<>9__49_0 ?? (<>c.<>9__49_0 = new Func<object, string>(<>c.<>9, this.<get_EnumItmFunk>b__49_0));
                    }
                    if (this.EnumItmMtch == null)
                    {
                        this.EnumItmMtch = new Func<string, bool>(this, this.<get_EnumItmFunk>b__49_1);
                    }
                    this._enumItmFunk = new Func<object, bool>(this, this.<get_EnumItmFunk>b__49_2);
                }
                return this._enumItmFunk;
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly AttrbStr.<>c <>9 = new AttrbStr.<>c();
            public static Func<object, string> <>9__49_0;

            internal string <get_EnumItmFunk>b__49_0(object u)
            {
                return u.ToString();
            }
        }
    }
}

