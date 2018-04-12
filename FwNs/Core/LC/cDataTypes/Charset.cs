namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Text;

    public sealed class Charset : ISchemaObject
    {
        public static readonly int[][] UppercaseLetters;
        public static readonly int[][] UnquotedIdentifier;
        public static int[][] BasicIdentifier;
        private readonly QNameManager.QName _name;
        public QNameManager.QName BaseName;
        public int[][] Ranges;

        static Charset()
        {
            int[][] numArrayArray1 = new int[1][];
            numArrayArray1[0] = new int[] { 0x41, 90 };
            UppercaseLetters = numArrayArray1;
            int[][] numArrayArray2 = new int[3][];
            numArrayArray2[0] = new int[] { 0x30, 0x39 };
            numArrayArray2[1] = new int[] { 0x41, 90 };
            numArrayArray2[2] = new int[] { 0x5f, 0x5f };
            UnquotedIdentifier = numArrayArray2;
            int[][] numArrayArray3 = new int[4][];
            numArrayArray3[0] = new int[] { 0x30, 0x39 };
            numArrayArray3[1] = new int[] { 0x41, 90 };
            numArrayArray3[2] = new int[] { 0x5f, 0x5f };
            numArrayArray3[3] = new int[] { 0x61, 0x7a };
            BasicIdentifier = numArrayArray3;
        }

        public Charset(QNameManager.QName name)
        {
            this._name = name;
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public QNameManager.QName GetCatalogName()
        {
            return this._name.schema.schema;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public static Charset GetDefaultInstance()
        {
            return SqlInvariants.Utf8;
        }

        public QNameManager.QName GetName()
        {
            return this._name;
        }

        public Grantee GetOwner()
        {
            return this._name.schema.Owner;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            OrderedHashSet<QNameManager.QName> set1 = new OrderedHashSet<QNameManager.QName>();
            set1.Add(this.BaseName);
            return set1;
        }

        public QNameManager.QName GetSchemaName()
        {
            return this._name.schema;
        }

        public int GetSchemaObjectType()
        {
            return 14;
        }

        public string GetSql()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("CREATE").Append(' ').Append("CHARACTER").Append(' ').Append("SET").Append(' ');
            builder1.Append(this._name.GetSchemaQualifiedStatementName());
            builder1.Append(' ').Append("AS").Append(' ').Append("GET");
            builder1.Append(' ').Append(this.BaseName.GetSchemaQualifiedStatementName());
            return builder1.ToString();
        }

        public static bool IsInSet(string value, int[][] ranges)
        {
            int length = value.Length;
            int num2 = 0;
            while (num2 < length)
            {
                int num3 = value[num2];
                for (int i = 0; i < ranges.GetLength(0); i++)
                {
                    if (num3 <= ranges[i][1])
                    {
                        if (num3 < ranges[i][0])
                        {
                            return false;
                        }
                        num2++;
                        continue;
                    }
                }
                return false;
            }
            return true;
        }

        public static bool StartsWith(string value, int[][] ranges)
        {
            int num = value[0];
            for (int i = 0; i < ranges.Length; i++)
            {
                if (num <= ranges[i][1])
                {
                    return (num >= ranges[i][0]);
                }
            }
            return false;
        }
    }
}

