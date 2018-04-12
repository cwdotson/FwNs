namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Text;

    public sealed class ClobType : CharacterType
    {
        private const long MaxClobPrecision = 0x10000000000L;
        private const int DefaultClobSize = 0x1000000;

        public ClobType() : base(40, 0x1000000L)
        {
        }

        public ClobType(long precision) : base(40, precision)
        {
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            if (a == b)
            {
                return 0;
            }
            if (a == null)
            {
                return -1;
            }
            if (b == null)
            {
                return 1;
            }
            IClobData data = (IClobData) a;
            string str = b as string;
            if (str != null)
            {
                return session.database.lobManager.Compare(data, str);
            }
            return session.database.lobManager.Compare(data, (IClobData) b);
        }

        public override object ConvertSQLToCSharp(ISessionInterface session, object a)
        {
            IClobData data1 = (IClobData) a;
            int length = (int) data1.Length(session);
            return new string(data1.GetChars(session, 0L, length));
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            if (a is IClobData)
            {
                return a;
            }
            string str = a as string;
            if (str == null)
            {
                throw Error.GetError(0x15b9);
            }
            ClobDataId id1 = session.CreateClob((long) str.Length);
            id1.SetString(session, 0L, str);
            return id1;
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            return StringConverter.ToQuotedString(this.ConvertToString(a), '\'', true);
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            return a.ToString();
        }

        public override int DisplaySize()
        {
            if (base.Precision <= 0x7fffffffL)
            {
                return (int) base.Precision;
            }
            return 0x7fffffff;
        }

        public override int GetAdoTypeCode()
        {
            return 0x17;
        }

        public override Type GetCSharpClass()
        {
            return typeof(char[]);
        }

        public override string GetCSharpClassName()
        {
            return "System.Char[]";
        }

        public override string GetDefinition()
        {
            long precision = base.Precision;
            string str = null;
            if ((base.Precision % 0x400L) == 0)
            {
                if ((base.Precision % 0x40000000L) == 0)
                {
                    precision = base.Precision / 0x40000000L;
                    str = "G";
                }
                else if ((base.Precision % 0x100000L) == 0)
                {
                    precision = base.Precision / 0x100000L;
                    str = "M";
                }
                else
                {
                    precision = base.Precision / 0x400L;
                    str = "K";
                }
            }
            StringBuilder builder = new StringBuilder(0x10);
            builder.Append(this.GetNameString());
            builder.Append('(');
            builder.Append(precision);
            if (str != null)
            {
                builder.Append(str);
            }
            builder.Append(')');
            return builder.ToString();
        }

        public override long GetMaxPrecision()
        {
            return 0x10000000000L;
        }

        public override int GetSqlGenericTypeCode()
        {
            return base.TypeCode;
        }

        public override bool IsLobType()
        {
            return true;
        }

        public override long Position(ISessionInterface session, object data, object otherData, SqlType otherType, long start)
        {
            IClobData data2 = (IClobData) data;
            if (otherType.TypeCode == 40)
            {
                return data2.Position(session, (IClobData) otherData, start);
            }
            if (!otherType.IsCharacterType())
            {
                throw Error.RuntimeError(0xc9, "ClobType");
            }
            return data2.Position(session, (string) otherData, start);
        }
    }
}

