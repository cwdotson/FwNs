namespace FwNs.Core.LC.cSchemas
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using System;
    using System.Text;

    public sealed class QNameManager
    {
        public const string DefaultCatalogName = "PUBLIC";
        private static readonly QNameManager StaticManager = GetStaticManager();
        private static readonly QName[] AutoColumnNames = GetAutoColumnNames();
        private static readonly string[] AutoNoNameColumnNames = GetAutoNoNameColumnNames();
        private readonly QName _catalogName;
        private int _serialNumber = 1;
        private int _sysNumber;

        public QNameManager(Database database)
        {
            this._catalogName = new QName(this, "PUBLIC", 1, false);
        }

        public static QName GetAutoColumnName(int i)
        {
            if (i < AutoColumnNames.Length)
            {
                return AutoColumnNames[i];
            }
            return new QName(StaticManager, "C_" + (i + 1), 0, false);
        }

        private static QName[] GetAutoColumnNames()
        {
            QName[] nameArray = new QName[0x20];
            for (int i = 0; i < nameArray.Length; i++)
            {
                nameArray[i] = new QName(StaticManager, "C" + (i + 1), 0, false);
            }
            return nameArray;
        }

        private static string[] GetAutoNoNameColumnNames()
        {
            string[] strArray = new string[0x20];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = i.ToString();
            }
            return strArray;
        }

        public static string GetAutoNoNameColumnString(int i)
        {
            if (i < AutoColumnNames.Length)
            {
                return AutoNoNameColumnNames[i];
            }
            return i.ToString();
        }

        public static string GetAutoSavepointNameString(long i, int j)
        {
            StringBuilder builder1 = new StringBuilder("S");
            builder1.Append(i).Append('_').Append(j);
            return builder1.ToString();
        }

        public QName GetCatalogName()
        {
            return this._catalogName;
        }

        public static SimpleName GetSimpleName(string name, bool isNameQuoted)
        {
            return new SimpleName(name, isNameQuoted);
        }

        private static QNameManager GetStaticManager()
        {
            return new QNameManager(null) { _serialNumber = -2147483648 };
        }

        public QName GetSubqueryTableName()
        {
            return new QName(this, "SYSTEM_SUBQUERY", false, 3) { schema = SqlInvariants.SystemSchemaQname };
        }

        public QName NewAutoName(string prefix, QName schema, QName parent, int type)
        {
            return this.NewAutoName(prefix, null, schema, parent, type);
        }

        public QName NewAutoName(string prefix, string namepart, QName schema, QName parent, int type)
        {
            StringBuilder builder = new StringBuilder();
            if (prefix != null)
            {
                if (prefix.Length != 0)
                {
                    builder.Append("SYS_");
                    builder.Append(prefix);
                    builder.Append('_');
                    if (namepart != null)
                    {
                        builder.Append(namepart);
                        builder.Append('_');
                    }
                    int num = this._sysNumber + 1;
                    this._sysNumber = num;
                    builder.Append(num);
                }
            }
            else
            {
                builder.Append(namepart);
            }
            return new QName(this, builder.ToString(), type, false) { 
                schema = schema,
                Parent = parent
            };
        }

        public QName NewColumnQName(QName table, string name, bool isquoted)
        {
            return new QName(this, name, isquoted, 9) { 
                schema = table.schema,
                Parent = table
            };
        }

        public QName NewColumnSchemaQName(QName table, SimpleName name)
        {
            return this.NewColumnQName(table, name.Name, name.IsNameQuoted);
        }

        public static QName NewInfoSchemaColumnName(string name, QName table)
        {
            return new QName(StaticManager, name, false, 9) { 
                schema = SqlInvariants.InformationSchemaQname,
                Parent = table
            };
        }

        public static QName NewInfoSchemaObjectName(string name, bool isQuoted, int type)
        {
            return new QName(StaticManager, name, type, isQuoted) { schema = SqlInvariants.InformationSchemaQname };
        }

        public static QName NewInfoSchemaTableName(string name)
        {
            return new QName(StaticManager, name, 3, false) { schema = SqlInvariants.InformationSchemaQname };
        }

        public QName NewQName(QName schema, string name, int type)
        {
            return new QName(this, name, type, false) { schema = schema };
        }

        public QName NewQName(string name, bool isquoted, int type)
        {
            return new QName(this, name, isquoted, type);
        }

        public QName NewQName(QName schema, string name, bool isquoted, int type)
        {
            return new QName(this, name, isquoted, type) { schema = schema };
        }

        public QName NewQName(QName schema, string name, bool isquoted, int type, QName parent)
        {
            return new QName(this, name, isquoted, type) { 
                schema = schema,
                Parent = parent
            };
        }

        public QName NewSpecificRoutineName(QName name)
        {
            StringBuilder builder = new StringBuilder();
            int num = this._sysNumber + 1;
            this._sysNumber = num;
            builder.Append(name.Name).Append('_').Append(num);
            return new QName(this, builder.ToString(), 0x18, name.IsNameQuoted) { 
                Parent = name,
                schema = name.schema
            };
        }

        public static QName NewSystemObjectName(string name, int type)
        {
            return new QName(StaticManager, name, type, false);
        }

        public sealed class QName : QNameManager.SimpleName, IEquatable<QNameManager.QName>
        {
            public static QNameManager.QName[] EmptyArray = new QNameManager.QName[0];
            private static readonly string[] SysPrefixes = new string[] { "SYS_IDX_", "SYS_PK_", "SYS_REF_", "SYS_CT_", "SYS_FK_" };
            private readonly int _hashCode;
            private readonly QNameManager _manager;
            public string Comment;
            public Grantee Owner;
            public QNameManager.QName Parent;
            public QNameManager.QName schema;
            public string StatementName;
            public int type;

            private QName(QNameManager man, int type)
            {
                this._manager = man;
                this.type = type;
                int num = this._manager._serialNumber;
                this._manager._serialNumber = num + 1;
                this._hashCode = num;
            }

            public QName(QNameManager man, string name, bool isquoted, int type) : this(man, type)
            {
                this.Rename(name, isquoted);
            }

            public QName(QNameManager man, string name, int type, bool isquoted) : this(man, type)
            {
                this.StatementName = name;
                base.Name = name;
                base.IsNameQuoted = isquoted;
                if (base.IsNameQuoted)
                {
                    this.StatementName = StringConverter.ToQuotedString(name, '"', true);
                }
            }

            public int CompareTo(object o)
            {
                return (this._hashCode - o.GetHashCode());
            }

            public bool Equals(QNameManager.QName other)
            {
                return ((other != null) && (this._hashCode == other.GetHashCode()));
            }

            public override bool Equals(object other)
            {
                QNameManager.QName name = other as QNameManager.QName;
                return ((name != null) && this.Equals(name));
            }

            public override int GetHashCode()
            {
                return this._hashCode;
            }

            public string GetSchemaQualifiedStatementName()
            {
                if (this.type == 9)
                {
                    if ((this.Parent == null) || "SYSTEM_SUBQUERY".Equals(this.Parent.Name))
                    {
                        return this.StatementName;
                    }
                    StringBuilder builder = new StringBuilder();
                    if ((this.schema != null) && !SqlInvariants.IsSystemSchemaName(this.schema))
                    {
                        builder.Append(this.schema.GetStatementName());
                        builder.Append('.');
                    }
                    builder.Append(this.Parent.GetStatementName());
                    builder.Append('.');
                    builder.Append(this.StatementName);
                    return builder.ToString();
                }
                if ((this.schema == null) || (this.schema == SqlInvariants.ModuleQname))
                {
                    return this.StatementName;
                }
                StringBuilder builder1 = new StringBuilder();
                builder1.Append(this.schema.GetStatementName());
                builder1.Append('.');
                builder1.Append(this.StatementName);
                return builder1.ToString();
            }

            public bool IsReservedName()
            {
                return IsReservedName(base.Name);
            }

            public static bool IsReservedName(string name)
            {
                return (SysPrefixLength(name) > 0);
            }

            public void Rename(QNameManager.QName name)
            {
                this.Rename(name.Name, name.IsNameQuoted);
            }

            public void Rename(string name, bool isquoted)
            {
                if (name.Length > 0x80)
                {
                    throw Error.GetError(0x157d, name);
                }
                base.Name = name;
                this.StatementName = name;
                base.IsNameQuoted = isquoted;
                if (base.IsNameQuoted)
                {
                    this.StatementName = StringConverter.ToQuotedString(name, '"', true);
                }
                if (name.StartsWith("SYS_"))
                {
                    int startIndex = name.LastIndexOf('_') + 1;
                    try
                    {
                        int num2 = int.Parse(name.Substring(startIndex));
                        if (num2 > this._manager._sysNumber)
                        {
                            this._manager._sysNumber = num2;
                        }
                    }
                    catch (FormatException)
                    {
                    }
                }
            }

            public void SetSchemaIfNull(QNameManager.QName schema)
            {
                if (this.schema == null)
                {
                    this.schema = schema;
                }
            }

            public static int SysPrefixLength(string name)
            {
                for (int i = 0; i < SysPrefixes.Length; i++)
                {
                    if (name.StartsWith(SysPrefixes[i]))
                    {
                        return SysPrefixes[i].Length;
                    }
                }
                return 0;
            }

            public override string ToString()
            {
                object[] objArray1 = new object[] { base.GetType().Name, base.GetHashCode(), "[this.GetHashCode()=", this.GetHashCode(), ", name=", base.Name, ", name.GetHashCode()=", base.Name.GetHashCode(), ", isNameQuoted=", base.IsNameQuoted, "]" };
                return string.Concat(objArray1);
            }
        }

        public class SimpleName : IEquatable<QNameManager.SimpleName>
        {
            public bool IsNameQuoted;
            public string Name;

            protected SimpleName()
            {
            }

            public SimpleName(string name, bool isNameQuoted)
            {
                this.Name = name;
                this.IsNameQuoted = isNameQuoted;
            }

            public virtual bool Equals(QNameManager.SimpleName other)
            {
                return (((other != null) && (other.IsNameQuoted == this.IsNameQuoted)) && other.Name.Equals(this.Name));
            }

            public override bool Equals(object other)
            {
                QNameManager.SimpleName name = other as QNameManager.SimpleName;
                return ((name != null) && this.Equals(name));
            }

            public override int GetHashCode()
            {
                return this.Name.GetHashCode();
            }

            public string GetStatementName()
            {
                if (!this.IsNameQuoted)
                {
                    return this.Name;
                }
                return StringConverter.ToQuotedString(this.Name, '"', true);
            }
        }
    }
}

