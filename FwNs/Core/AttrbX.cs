namespace FwNs.Core
{
    using FwNs.Txt.JSon;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Field)]
    public class AttrbX : Attribute
    {
        private string _nm;
        private string _Val;

        public AttrbX(string name, string val_)
        {
            this._nm = name;
            this._Val = val_;
        }

        public string Get(object enm)
        {
            if (enm == null)
            {
                MemberInfo[] member = enm.GetType().GetMember(enm.ToString());
                if ((member != null) && (member.Length != 0))
                {
                    Attrb customAttribute = Attribute.GetCustomAttribute(member[0], typeof(Attrb)) as Attrb;
                    if (customAttribute != null)
                    {
                        return customAttribute.Nm;
                    }
                }
            }
            return null;
        }

        public string Get(Type tp, string name)
        {
            MemberInfo[] member = tp.GetMember(name);
            if ((member != null) && (member.Length != 0))
            {
                Attrb customAttribute = Attribute.GetCustomAttribute(member[0], typeof(Attrb)) as Attrb;
                if (customAttribute != null)
                {
                    return customAttribute.Nm;
                }
            }
            return null;
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

        public KeyValuePair<string, object>[] Ary
        {
            get
            {
                return new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Nm", this.Nm), new KeyValuePair<string, object>("Val", this.Val) };
            }
        }

        public string Nm
        {
            get
            {
                return this._nm;
            }
            set
            {
                this._nm = value;
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

