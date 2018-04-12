namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Text;

    public sealed class BlobType : BinaryType
    {
        public const long MaxBlobPrecision = 0x10000000000L;
        public const int DefaultBlobSize = 0x1000000;

        public BlobType() : base(30, 0x1000000L)
        {
        }

        public BlobType(long precision) : base(30, precision)
        {
        }

        public override bool AcceptsPrecision()
        {
            return true;
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
            if (b is BinaryData)
            {
                return session.database.lobManager.Compare((IBlobData) a, ((BinaryData) b).GetBytes());
            }
            return session.database.lobManager.Compare((IBlobData) a, (IBlobData) b);
        }

        public override object ConvertSQLToCSharp(ISessionInterface session, object a)
        {
            IBlobData data1 = (IBlobData) a;
            int length = (int) data1.Length(session);
            return data1.GetBytes(session, 0L, length);
        }

        public override object ConvertToDefaultType(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return a;
            }
            byte[] data = a as byte[];
            if (data == null)
            {
                throw Error.GetError(0x15b9);
            }
            return new BinaryData(data, false);
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            return StringConverter.ByteArrayToSqlHexString(((IBlobData) a).GetBytes());
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            return StringConverter.ByteArrayToHexString(((IBlobData) a).GetBytes());
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            if (a == null)
            {
                return null;
            }
            if (othType.TypeCode == 30)
            {
                return a;
            }
            if ((othType.TypeCode == 60) || (othType.TypeCode == 0x3d))
            {
                IBlobData data = (IBlobData) a;
                BlobDataId id1 = session.CreateBlob(data.Length(session));
                id1.SetBytes(session, 0L, data.GetBytes());
                return id1;
            }
            if ((othType.TypeCode != 12) && (othType.TypeCode != 1))
            {
                throw Error.GetError(0x15b9);
            }
            IBlobData data2 = session.GetScanner().ConvertToBinary((string) a);
            BlobDataId id2 = session.CreateBlob(data2.Length(session));
            id2.SetBytes(session, 0L, data2.GetBytes());
            return id2;
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            return a;
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
            return 0x16;
        }

        public override Type GetCSharpClass()
        {
            return typeof(byte[]);
        }

        public override string GetCSharpClassName()
        {
            return "System.Byte[]";
        }

        public override string GetDefinition()
        {
            long precision = base.Precision;
            string str = null;
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
            else if ((base.Precision % 0x400L) == 0)
            {
                precision = base.Precision / 0x400L;
                str = "K";
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

        public override string GetFullNameString()
        {
            return "BINARY LARGE OBJECT";
        }

        public override long GetMaxPrecision()
        {
            return 0x10000000000L;
        }

        public override string GetNameString()
        {
            return "BLOB";
        }

        public override bool IsBinaryType()
        {
            return true;
        }

        public override bool IsLobType()
        {
            return true;
        }

        public override bool RequiresPrecision()
        {
            return false;
        }
    }
}

