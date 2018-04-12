namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Text;

    public sealed class RowType : SqlType
    {
        private readonly SqlType[] _dataTypes;

        public RowType(SqlType[] dataTypes) : base(0x13, 0x13, 0L, 0)
        {
            this._dataTypes = dataTypes;
        }

        public override bool CanBeAssignedFrom(SqlType otherType)
        {
            if (otherType != null)
            {
                if (!otherType.IsRowType())
                {
                    return false;
                }
                SqlType[] typesArray = ((RowType) otherType).GetTypesArray();
                if (this._dataTypes.Length != typesArray.Length)
                {
                    return false;
                }
                for (int i = 0; i < this._dataTypes.Length; i++)
                {
                    if (!this._dataTypes[i].CanBeAssignedFrom(typesArray[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            if (othType != null)
            {
                if (!othType.IsRowType())
                {
                    return false;
                }
                SqlType[] typesArray = ((RowType) othType).GetTypesArray();
                if (this._dataTypes.Length != typesArray.Length)
                {
                    return false;
                }
                for (int i = 0; i < this._dataTypes.Length; i++)
                {
                    if (!this._dataTypes[i].CanConvertFrom(typesArray[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int Compare(Session session, object a, object b, SqlType otherType, bool forEquality)
        {
            if (a != b)
            {
                if (a == null)
                {
                    return -1;
                }
                if (b == null)
                {
                    return 1;
                }
                object[] objArray = (object[]) a;
                object[] objArray2 = (object[]) b;
                int length = objArray.Length;
                if (objArray2.Length < length)
                {
                    length = objArray2.Length;
                }
                for (int i = 0; i < length; i++)
                {
                    int num4 = this._dataTypes[i].Compare(session, objArray[i], objArray2[i], null, false);
                    if (num4 != 0)
                    {
                        return num4;
                    }
                }
                if (objArray.Length > objArray2.Length)
                {
                    return 1;
                }
                if (objArray.Length < objArray2.Length)
                {
                    return -1;
                }
            }
            return 0;
        }

        public override object ConvertToDefaultType(ISessionInterface sessionInterface, object o)
        {
            return o;
        }

        public override string ConvertToSQLString(object a)
        {
            if (a == null)
            {
                return "NULL";
            }
            object[] objArray = (object[]) a;
            StringBuilder builder = new StringBuilder();
            builder.Append("ROW");
            builder.Append('(');
            for (int i = 0; i < objArray.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(this._dataTypes[i].ConvertToSQLString(objArray[i]));
            }
            builder.Append(')');
            return builder.ToString();
        }

        public override string ConvertToString(object a)
        {
            if (a == null)
            {
                return null;
            }
            return this.ConvertToSQLString(a);
        }

        public override object ConvertToType(ISessionInterface session, object a, SqlType othType)
        {
            if (a == null)
            {
                return null;
            }
            if (othType == null)
            {
                return a;
            }
            if (!othType.IsRowType())
            {
                throw Error.GetError(0x15ba);
            }
            SqlType[] typesArray = ((RowType) othType).GetTypesArray();
            if (this._dataTypes.Length != typesArray.Length)
            {
                throw Error.GetError(0x15bc);
            }
            object[] objArray = (object[]) a;
            object[] objArray2 = new object[objArray.Length];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray2[i] = this._dataTypes[i].ConvertToType(session, objArray[i], typesArray[i]);
            }
            return objArray2;
        }

        public override object ConvertToTypeLimits(ISessionInterface session, object a)
        {
            if (a == null)
            {
                return null;
            }
            object[] objArray = (object[]) a;
            object[] objArray2 = new object[objArray.Length];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray2[i] = this._dataTypes[i].ConvertToTypeLimits(session, objArray[i]);
            }
            return objArray2;
        }

        public override int DisplaySize()
        {
            return 0;
        }

        public override int GetAdoPrecision()
        {
            return 0;
        }

        public override int GetAdoScale()
        {
            return 0;
        }

        public override int GetAdoTypeCode()
        {
            return 0;
        }

        public override SqlType GetAggregateType(SqlType otherType)
        {
            if (otherType == null)
            {
                return this;
            }
            if (otherType == this)
            {
                return this;
            }
            if (!otherType.IsRowType())
            {
                throw Error.GetError(0x15ba);
            }
            SqlType[] dataTypes = new SqlType[this._dataTypes.Length];
            SqlType[] typesArray = ((RowType) otherType).GetTypesArray();
            if (this._dataTypes.Length != typesArray.Length)
            {
                throw Error.GetError(0x15bc);
            }
            for (int i = 0; i < this._dataTypes.Length; i++)
            {
                dataTypes[i] = this._dataTypes[i].GetAggregateType(typesArray[i]);
            }
            return new RowType(dataTypes);
        }

        public override SqlType GetCombinedType(SqlType otherType, int operation)
        {
            if (operation != 0x24)
            {
                return this.GetAggregateType(otherType);
            }
            if (otherType == null)
            {
                return this;
            }
            if (!otherType.IsRowType())
            {
                throw Error.GetError(0x15ba);
            }
            SqlType[] dataTypes = new SqlType[this._dataTypes.Length];
            SqlType[] typesArray = ((RowType) otherType).GetTypesArray();
            if (this._dataTypes.Length != typesArray.Length)
            {
                throw Error.GetError(0x15bc);
            }
            for (int i = 0; i < this._dataTypes.Length; i++)
            {
                dataTypes[i] = this._dataTypes[i].GetAggregateType(typesArray[i]);
            }
            return new RowType(dataTypes);
        }

        public override Type GetCSharpClass()
        {
            return typeof(object);
        }

        public override string GetCSharpClassName()
        {
            return "System.Object";
        }

        public override string GetDefinition()
        {
            return this.GetNameString();
        }

        public override string GetNameString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ROW");
            builder.Append('(');
            for (int i = 0; i < this._dataTypes.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(this._dataTypes[i].GetNameString());
            }
            builder.Append(')');
            return builder.ToString();
        }

        public override int GetSqlGenericTypeCode()
        {
            return 0x13;
        }

        public SqlType[] GetTypesArray()
        {
            return this._dataTypes;
        }

        public override bool IsRowType()
        {
            return true;
        }
    }
}

