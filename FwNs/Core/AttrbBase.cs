namespace FwNs.Core
{
    using FwNs.Txt.JSon;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class AttrbBase : Attribute
    {
        private string _nm;

        public AttrbBase(string name)
        {
            this._nm = name;
        }

        public string Get(object enm)
        {
            if (enm == null)
            {
                MemberInfo[] member = enm.GetType().GetMember(enm.ToString());
                if ((member != null) && (member.Length != 0))
                {
                    AttrbBase customAttribute = Attribute.GetCustomAttribute(member[0], typeof(AttrbBase)) as AttrbBase;
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
                AttrbBase customAttribute = Attribute.GetCustomAttribute(member[0], typeof(AttrbBase)) as AttrbBase;
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

        public virtual KeyValuePair<string, object>[] Ary
        {
            get
            {
                return new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Nm", this.Nm) };
            }
        }

        public virtual object[,] AryObj
        {
            get
            {
                return new string[,] { { "Nm", this.Nm } };
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
    }
}

