namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public sealed class Collation : ISchemaObject
    {
        public static Dictionary<string, string> NameToCSharpName = GetCollationMap();
        private readonly QNameManager.QName _name;
        public CompareInfo Collator = CompareInfo.GetCompareInfo("en-US");
        private CultureInfo _locale = CultureInfo.GetCultureInfo("en-US");
        private static Collation DefaultCollation = null;

        public Collation()
        {
            this._name = QNameManager.NewInfoSchemaObjectName(this._locale.EnglishName, true, 15);
        }

        public int Compare(string a, string b)
        {
            return this.Collator.Compare(a, b);
        }

        public int CompareForEquality(string a, string b)
        {
            return string.CompareOrdinal(a, b);
        }

        public int CompareIgnoreCase(string a, string b)
        {
            return this.Collator.Compare(a, b, CompareOptions.IgnoreCase);
        }

        public int CompareIgnoreCaseForEquality(string a, string b)
        {
            if (!string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
            {
                return -1;
            }
            return 0;
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public QNameManager.QName GetCatalogName()
        {
            return null;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        private static Dictionary<string, string> GetCollationMap()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>(0x65, StringComparer.InvariantCultureIgnoreCase);
            foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                dictionary[info.EnglishName] = info.Name;
            }
            return dictionary;
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public static Collation GetDefaultInstance()
        {
            if (DefaultCollation == null)
            {
                DefaultCollation = new Collation();
            }
            return DefaultCollation;
        }

        public QNameManager.QName GetName()
        {
            return this._name;
        }

        public Grantee GetOwner()
        {
            return SqlInvariants.InformationSchemaQname.Owner;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return new OrderedHashSet<QNameManager.QName>();
        }

        public QNameManager.QName GetSchemaName()
        {
            return SqlInvariants.InformationSchemaQname;
        }

        public int GetSchemaObjectType()
        {
            return 15;
        }

        public string GetSql()
        {
            return "";
        }

        public bool IsEqualAlwaysIdentical()
        {
            return (this.Collator == null);
        }

        public void SetCollation(string newName)
        {
            string name = NameToCSharpName[newName];
            if (name == null)
            {
                throw Error.GetError(0x157d, newName);
            }
            this._name.Rename(newName, true);
            this._locale = CultureInfo.GetCultureInfo(name);
            this.Collator = CompareInfo.GetCompareInfo(name);
        }

        public void SetCollationAsLocale()
        {
            this._locale = CultureInfo.CurrentCulture;
            this.Collator = CultureInfo.CurrentCulture.CompareInfo;
        }

        public string ToInitcap(string s)
        {
            return this._locale.TextInfo.ToTitleCase(s);
        }

        public string ToLowerCase(string s)
        {
            return s.ToLower(this._locale);
        }

        public string ToUpperCase(string s)
        {
            return s.ToUpper(this._locale);
        }
    }
}

