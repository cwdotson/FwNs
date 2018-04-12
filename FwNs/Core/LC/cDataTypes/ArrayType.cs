namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Text;

    public sealed class ArrayType : SqlType
    {
        private readonly SqlType _dataType;
        private readonly int _maxCardinality;

        public ArrayType(SqlType dataType, int cardinality) : base(50, 50, 0L, 0)
        {
            this._dataType = dataType;
            this._maxCardinality = cardinality;
        }

        public override int ArrayLimitCardinality()
        {
            return this._maxCardinality;
        }

        public override bool CanBeAssignedFrom(SqlType otherType)
        {
            if (otherType == null)
            {
                return true;
            }
            SqlType type = otherType.CollectionBaseType();
            return ((type != null) && this._dataType.CanBeAssignedFrom(type));
        }

        public override bool CanConvertFrom(SqlType othType)
        {
            if (othType == null)
            {
                return true;
            }
            if (!othType.IsArrayType())
            {
                return false;
            }
            SqlType type = othType.CollectionBaseType();
            return this._dataType.CanConvertFrom(type);
        }

        public override int Cardinality(Session session, object a)
        {
            if (a == null)
            {
                return 0;
            }
            return ((object[]) a).Length;
        }

        public override SqlType CollectionBaseType()
        {
            return this._dataType;
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
                    int num4 = this._dataType.Compare(session, objArray[i], objArray2[i], null, false);
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

        public override object Concat(Session session, object a, object b)
        {
            if ((a == null) || (b == null))
            {
                return null;
            }
            object[] sourceArray = (object[]) a;
            object[] objArray2 = (object[]) b;
            object[] destinationArray = new object[sourceArray.Length + objArray2.Length];
            Array.Copy(sourceArray, 0, destinationArray, 0, sourceArray.Length);
            Array.Copy(objArray2, 0, destinationArray, sourceArray.Length, objArray2.Length);
            return destinationArray;
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
            builder.Append("ARRAY");
            builder.Append('[');
            for (int i = 0; i < objArray.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(this._dataType.ConvertToSQLString(objArray[i]));
            }
            builder.Append(']');
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
            if (!othType.IsArrayType())
            {
                throw Error.GetError(0x15ba);
            }
            object[] objArray = (object[]) a;
            if (objArray.Length > this._maxCardinality)
            {
                throw Error.GetError(0xda3);
            }
            SqlType other = othType.CollectionBaseType();
            if (this._dataType.Equals(other))
            {
                return a;
            }
            object[] objArray2 = new object[objArray.Length];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray2[i] = this._dataType.ConvertToType(session, objArray[i], other);
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
            if (objArray.Length > this._maxCardinality)
            {
                throw Error.GetError(0xda3);
            }
            object[] objArray2 = new object[objArray.Length];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray2[i] = this._dataType.ConvertToTypeLimits(session, objArray[i]);
            }
            return objArray2;
        }

        public override int DisplaySize()
        {
            return (7 + ((this._dataType.DisplaySize() + 1) * this._maxCardinality));
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
            return 0x7d3;
        }

        public override SqlType GetAggregateType(SqlType otherType)
        {
            if (otherType == null)
            {
                return this;
            }
            if (!otherType.IsArrayType())
            {
                throw Error.GetError(0x15ba);
            }
            SqlType other = otherType.CollectionBaseType();
            if (this._dataType.Equals(other))
            {
                return this;
            }
            return new ArrayType(this._dataType.GetAggregateType(other), this._maxCardinality);
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
            if (!otherType.IsArrayType())
            {
                throw Error.GetError(0x15ba);
            }
            SqlType other = otherType.CollectionBaseType();
            return new ArrayType(this._dataType.GetAggregateType(other), this._maxCardinality + otherType.ArrayLimitCardinality());
        }

        public override Type GetCSharpClass()
        {
            return typeof(Array);
        }

        public override string GetCSharpClassName()
        {
            return "System.Array";
        }

        public override string GetDefinition()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this._dataType.GetDefinition()).Append(' ');
            builder.Append("ARRAY");
            if (this._maxCardinality != 0x400)
            {
                builder.Append('[').Append(this._maxCardinality).Append(']');
            }
            return builder.ToString();
        }

        public override string GetNameString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this._dataType.GetNameString()).Append(' ');
            builder.Append("ARRAY");
            if (this._maxCardinality != 0x400)
            {
                builder.Append('[').Append(this._maxCardinality).Append(']');
            }
            return builder.ToString();
        }

        public override int GetSqlGenericTypeCode()
        {
            return 0;
        }

        public override bool IsArrayType()
        {
            return true;
        }
    }
}

