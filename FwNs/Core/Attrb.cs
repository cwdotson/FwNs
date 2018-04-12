namespace FwNs.Core
{
    using FwNs.Txt.JSon;
    using System;
    using System.Collections.Generic;

    public class Attrb : AttrbBase
    {
        private string _Val;

        public Attrb(string name) : this(name, null)
        {
        }

        public Attrb(string name, string val) : base(name)
        {
            this._Val = val;
        }

        public override string ToString()
        {
            try
            {
                return FwNs.Txt.JSon.Xtnz.ToJsonStr(this.Ary);
            }
            catch (Exception)
            {
                return base.ToString();
            }
        }

        public override KeyValuePair<string, object>[] Ary
        {
            get
            {
                return new KeyValuePair<string, object>[] { base.Ary[0], new KeyValuePair<string, object>("Val", this.Val) };
            }
        }

        public string Val
        {
            get
            {
                return this._Val;
            }
            set
            {
                this._Val = value;
            }
        }
    }
}

