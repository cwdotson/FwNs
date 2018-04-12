namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;

    public abstract class SqlType : ISchemaObject, IEquatable<SqlType>, ICloneable
    {
        public const int DefaultArrayCardinality = 0x400;
        public int TypeComparisonGroup;
        public int TypeCode;
        public long Precision;
        public int Scale;
        public UserTypeModifier userTypeModifier;
        public static SqlType[] EmptyArray = new SqlType[0];
        public static SqlType SqlAllTypes = NullType.GetNullType();
        public static readonly CharacterType SqlChar = new CharacterType(1, 1L);
        public static readonly CharacterType SqlCharDefault = new CharacterType(1, 0x8000L);
        public static readonly CharacterType SqlVarchar = new CharacterType(12, 0L);
        public static readonly CharacterType SqlVarcharDefault = new CharacterType(12, 0x8000L);
        public static readonly ClobType SqlClob = new ClobType();
        public static readonly CharacterType VarcharIgnorecase = new CharacterType(100, 0L);
        public static readonly CharacterType VarcharIgnorecaseDefault = new CharacterType(100, 0x8000L);
        public static readonly BinaryType SqlBinary = new BinaryType(60, 1L);
        public static readonly BinaryType SqlBinaryDefault = new BinaryType(60, 0x8000L);
        public static readonly BinaryType SqlVarbinary = new BinaryType(0x3d, 0L);
        public static readonly BinaryType SqlVarbinaryDefault = new BinaryType(0x3d, 0x8000L);
        public static readonly BlobType SqlBlob = new BlobType();
        public static readonly OtherType Other = OtherType.GetOtherType();
        public static readonly BooleanType SqlBoolean = BooleanType.GetBooleanType();
        public static readonly UniqueIdentifierType SqlUniqueIdentifier = UniqueIdentifierType.GetUniqueIdentifierType();
        public static readonly NumberType SqlNumeric = new NumberType(2, 0x7fffffffL, 0x7fff);
        public static readonly NumberType SqlDecimal = new NumberType(3, 0x7fffffffL, 0x7fff);
        public static readonly NumberType SqlDecimalDefault = new NumberType(3, 0x7fffffffL, 0x7fff);
        public static readonly NumberType SqlDecimalBigintSqr = new NumberType(3, 0x1cL, 0x7fff);
        public static readonly NumberType SqlDouble = new NumberType(8, 0L, 0xff);
        public static readonly NumberType Tinyint = new NumberType(-6, 3L, 0);
        public static readonly NumberType SqlSmallint = new NumberType(5, 5L, 0);
        public static readonly NumberType SqlInteger = new NumberType(4, 10L, 0);
        public static readonly NumberType SqlBigint = new NumberType(0x19, 0x13L, 0);
        public static readonly DateTimeType SqlDate = new DateTimeType(0x5d, 0x5b, 0);
        public static readonly DateTimeType SqlTime = new DateTimeType(0x5c, 0x5c, DTIType.DefaultTimeFractionPrecision);
        public static readonly DateTimeType SqlTimeWithTimeZone = new DateTimeType(0x5c, 0x5e, DTIType.DefaultTimeFractionPrecision);
        public static readonly DateTimeType SqlTimestamp = new DateTimeType(0x5d, 0x5d, 6);
        public static readonly DateTimeType SqlTimestampWithTimeZone = new DateTimeType(0x5d, 0x5f, 6);
        public static readonly DateTimeType SqlTimestampNoFraction = new DateTimeType(0x5d, 0x5d, 0);
        public static readonly IntervalType SqlIntervalYear = IntervalType.NewIntervalType(0x65, 3L, 0);
        public static readonly IntervalType SqlIntervalMonth = IntervalType.NewIntervalType(0x66, 3L, 0);
        public static readonly IntervalType SqlIntervalDay = IntervalType.NewIntervalType(0x67, 3L, 0);
        public static readonly IntervalType SqlIntervalHour = IntervalType.NewIntervalType(0x68, 3L, 0);
        public static readonly IntervalType SqlIntervalMinute = IntervalType.NewIntervalType(0x69, 3L, 0);
        public static readonly IntervalType SqlIntervalSecond = IntervalType.NewIntervalType(0x6a, 3L, 6);
        public static readonly IntervalType SqlIntervalSecondMaxFraction = IntervalType.NewIntervalType(0x6a, 3L, 9);
        public static readonly IntervalType SqlIntervalYearToMonth = IntervalType.NewIntervalType(0x6b, 3L, 0);
        public static readonly IntervalType SqlIntervalDayToHour = IntervalType.NewIntervalType(0x6c, 3L, 0);
        public static readonly IntervalType SqlIntervalDayToMinute = IntervalType.NewIntervalType(0x6d, 3L, 0);
        public static readonly IntervalType SqlIntervalDayToSecond = IntervalType.NewIntervalType(110, 3L, 6);
        public static readonly IntervalType SqlIntervalHourToMinute = IntervalType.NewIntervalType(0x6f, 3L, 0);
        public static readonly IntervalType SqlIntervalHourToSecond = IntervalType.NewIntervalType(0x70, 3L, 6);
        public static readonly IntervalType SqlIntervalMinuteToSecond = IntervalType.NewIntervalType(0x71, 3L, 6);
        public static readonly IntervalType SqlIntervalYearMaxPrecision = IntervalType.NewIntervalType(0x65, 9L, 0);
        public static readonly IntervalType SqlIntervalMonthMaxPrecision = IntervalType.NewIntervalType(0x66, 9L, 0);
        public static IntervalType SqlIntervalDayMaxPrecision = IntervalType.NewIntervalType(0x67, 9L, 0);
        public static readonly IntervalType SqlIntervalHourMaxPrecision = IntervalType.NewIntervalType(0x68, 9L, 0);
        public static readonly IntervalType SqlIntervalMinuteMaxPrecision = IntervalType.NewIntervalType(0x69, 9L, 0);
        public static readonly IntervalType SqlIntervalSecondMaxPrecision = IntervalType.NewIntervalType(0x6a, 9L, 6);
        public static readonly IntervalType SqlIntervalSecondMaxFractionMaxPrecision = IntervalType.NewIntervalType(0x6a, 9L, 9);
        public static readonly ArrayType SqlArrayAllTypes = new ArrayType(SqlAllTypes, 0);
        public static Dictionary<string, int> TypeAliases;
        public static Dictionary<string, int> TypeNames;

        static SqlType()
        {
            Dictionary<string, int> dictionary1 = new Dictionary<string, int>(0x40) {
                { 
                    "INT",
                    4
                },
                { 
                    "DEC",
                    3
                },
                { 
                    "LONGVARCHAR",
                    -1
                },
                { 
                    "TEXT",
                    -1
                },
                { 
                    "LONGVARBINARY",
                    -4
                },
                { 
                    "OBJECT",
                    0x457
                }
            };
            TypeAliases = dictionary1;
            Dictionary<string, int> dictionary2 = new Dictionary<string, int>(0x25) {
                { 
                    "CHARACTER",
                    1
                },
                { 
                    "VARCHAR",
                    12
                },
                { 
                    "VARCHAR_IGNORECASE",
                    100
                },
                { 
                    "DATE",
                    0x5b
                },
                { 
                    "TIME",
                    0x5c
                },
                { 
                    "TIMESTAMP",
                    0x5d
                },
                { 
                    "INTERVAL",
                    10
                },
                { 
                    "TINYINT",
                    -6
                },
                { 
                    "SMALLINT",
                    5
                },
                { 
                    "INTEGER",
                    4
                },
                { 
                    "BIGINT",
                    0x19
                },
                { 
                    "REAL",
                    7
                },
                { 
                    "FLOAT",
                    6
                },
                { 
                    "DOUBLE",
                    8
                },
                { 
                    "NUMERIC",
                    2
                },
                { 
                    "DECIMAL",
                    3
                },
                { 
                    "NUMBER",
                    3
                },
                { 
                    "BOOLEAN",
                    0x10
                },
                { 
                    "BINARY",
                    60
                },
                { 
                    "VARBINARY",
                    0x3d
                },
                { 
                    "CLOB",
                    40
                },
                { 
                    "BLOB",
                    30
                },
                { 
                    "OTHER",
                    0x457
                },
                { 
                    "UNIQUEIDENTIFIER",
                    -11
                },
                { 
                    "CHAR",
                    1
                },
                { 
                    "NCHAR",
                    1
                },
                { 
                    "VARCHAR2",
                    12
                },
                { 
                    "NVARCHAR",
                    12
                },
                { 
                    "NVARCHAR2",
                    12
                },
                { 
                    "NCLOB",
                    40
                },
                { 
                    "DATETIME",
                    0x5b
                },
                { 
                    "MONEY",
                    3
                },
                { 
                    "BIT",
                    0x10
                }
            };
            TypeNames = dictionary2;
        }

        protected SqlType(int typeGroup, int type, long precision, int scale)
        {
            this.TypeComparisonGroup = typeGroup;
            this.TypeCode = type;
            this.Precision = precision;
            this.Scale = scale;
        }

        public virtual object Absolute(object a)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual bool AcceptsFractionalPrecision()
        {
            return false;
        }

        public virtual bool AcceptsPrecision()
        {
            return false;
        }

        public virtual bool AcceptsScale()
        {
            return false;
        }

        public virtual object Add(object a, object b, SqlType aType, SqlType bType)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual int ArrayLimitCardinality()
        {
            return 0;
        }

        public virtual bool CanBeAssignedFrom(SqlType otherType)
        {
            if (otherType != null)
            {
                return (this.TypeComparisonGroup == otherType.TypeComparisonGroup);
            }
            return true;
        }

        public abstract bool CanConvertFrom(SqlType othType);
        public virtual int Cardinality(Session session, object a)
        {
            return 0;
        }

        public virtual object CastToType(ISessionInterface session, object a, SqlType type)
        {
            return this.ConvertToType(session, a, type);
        }

        public virtual bool CheckEquals()
        {
            return true;
        }

        public object Clone()
        {
            return (base.MemberwiseClone() as SqlType);
        }

        public virtual SqlType CollectionBaseType()
        {
            return null;
        }

        public abstract int Compare(Session session, object a, object b, SqlType otherType, bool forEquality);
        public virtual void Compile(Session session, ISchemaObject parentObject)
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            this.userTypeModifier.Compile(session);
        }

        public virtual object Concat(Session session, object a, object b)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual object ConvertCSharpToSQL(ISessionInterface session, object a)
        {
            return a;
        }

        public virtual object ConvertSQLToCSharp(ISessionInterface session, object a)
        {
            return a;
        }

        public abstract object ConvertToDefaultType(ISessionInterface sessionInterface, object o);
        public abstract string ConvertToSQLString(object a);
        public abstract string ConvertToString(object a);
        public abstract object ConvertToType(ISessionInterface session, object a, SqlType type);
        public virtual object ConvertToTypeAdo(ISessionInterface session, object a, SqlType otherType)
        {
            if (otherType.IsLobType())
            {
                throw Error.GetError(0x15b9);
            }
            return this.ConvertToType(session, a, otherType);
        }

        public abstract object ConvertToTypeLimits(ISessionInterface session, object a);
        public abstract int DisplaySize();
        public virtual object Divide(object a, object b, SqlType aType, SqlType bType)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public SqlType Duplicate()
        {
            SqlType type;
            try
            {
                type = (SqlType) this.Clone();
            }
            catch (NotSupportedException)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return type;
        }

        public bool Equals(SqlType other)
        {
            return ((((other != null) && (other.TypeCode == this.TypeCode)) && ((other.Precision == this.Precision) && (other.Scale == this.Scale))) && (other.userTypeModifier == this.userTypeModifier));
        }

        public override bool Equals(object other)
        {
            if (other == this)
            {
                return true;
            }
            SqlType type = other as SqlType;
            return ((type != null) && this.Equals(type));
        }

        public virtual int GetAdoPrecision()
        {
            if (this.Precision <= 0x7fffffffL)
            {
                return (int) this.Precision;
            }
            return 0x7fffffff;
        }

        public virtual int GetAdoScale()
        {
            if (this.AcceptsScale())
            {
                return this.Scale;
            }
            return 0;
        }

        public abstract int GetAdoTypeCode();
        public abstract SqlType GetAggregateType(SqlType other);
        public static SqlType GetAggregateType(SqlType add, SqlType existing)
        {
            if ((add == null) && (existing == null))
            {
                return SqlAllTypes;
            }
            if ((existing == null) || (existing.TypeCode == 0))
            {
                return add;
            }
            if ((add == null) || (add.TypeCode == 0))
            {
                return existing;
            }
            return existing.GetAggregateType(add);
        }

        public virtual QNameManager.QName GetCatalogName()
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return this.userTypeModifier.GetSchemaName().schema;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        public abstract SqlType GetCombinedType(SqlType other, int operation);
        public virtual OrderedHashSet<ISchemaObject> GetComponents()
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return this.userTypeModifier.GetComponents();
        }

        public abstract Type GetCSharpClass();
        public abstract string GetCSharpClassName();
        public static SqlType GetDataType(int type, int collation, long precision, int scale)
        {
            if (type <= 0x19)
            {
                if (type > 12)
                {
                    if (type == 0x10)
                    {
                        return SqlBoolean;
                    }
                    if (type == 0x19)
                    {
                        return SqlBigint;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case -11:
                            return SqlUniqueIdentifier;

                        case -6:
                            return Tinyint;

                        case 0:
                            return SqlAllTypes;

                        case 1:
                        case 12:
                            goto Label_0191;

                        case 2:
                        case 3:
                            if (precision == 0)
                            {
                                precision = 0x7fffffffL;
                            }
                            return NumberType.GetNumberType(type, precision, scale);

                        case 4:
                            return SqlInteger;

                        case 5:
                            return SqlSmallint;

                        case 6:
                            if (precision > 0x35L)
                            {
                                throw Error.GetError(0x15d8, precision);
                            }
                            goto Label_0175;

                        case 7:
                        case 8:
                            goto Label_0175;
                    }
                }
                goto Label_0199;
            }
            if (type <= 40)
            {
                if (type == 30)
                {
                    goto Label_00B5;
                }
                if (type == 40)
                {
                    goto Label_0191;
                }
                goto Label_0199;
            }
            if ((type - 60) > 1)
            {
                switch (type)
                {
                    case 0x5b:
                    case 0x5c:
                    case 0x5d:
                    case 0x5e:
                    case 0x5f:
                        return DateTimeType.GetDateTimeType(type, scale);

                    case 100:
                        goto Label_0191;

                    case 0x65:
                    case 0x66:
                    case 0x67:
                    case 0x68:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                    case 110:
                    case 0x6f:
                    case 0x70:
                    case 0x71:
                        return IntervalType.GetIntervalType(type, precision, scale);

                    case 0x457:
                        return Other;
                }
                goto Label_0199;
            }
        Label_00B5:
            return BinaryType.GetBinaryType(type, precision);
        Label_0175:
            return SqlDouble;
        Label_0191:
            return CharacterType.GetCharacterType(type, precision);
        Label_0199:
            throw Error.RuntimeError(0xc9, "Type");
        }

        public static ArrayType GetDefaultArrayType(int type)
        {
            return new ArrayType(GetDefaultType(type), 0x400);
        }

        public static SqlType GetDefaultType(int type)
        {
            try
            {
                return GetDataType(type, 0, 0L, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static SqlType GetDefaultTypeWithSize(int type)
        {
            if (type <= 0x19)
            {
                if (type > 12)
                {
                    if (type == 0x10)
                    {
                        return SqlBoolean;
                    }
                    if (type == 0x19)
                    {
                        return SqlBigint;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case -11:
                            return SqlUniqueIdentifier;

                        case -6:
                            return Tinyint;

                        case 0:
                            return SqlAllTypes;

                        case 1:
                            return SqlCharDefault;

                        case 2:
                            return SqlNumeric;

                        case 3:
                            return SqlDecimal;

                        case 4:
                            return SqlInteger;

                        case 5:
                            return SqlSmallint;

                        case 6:
                        case 7:
                        case 8:
                            return SqlDouble;

                        case 12:
                            return SqlVarcharDefault;
                    }
                }
            }
            else if (type <= 40)
            {
                if (type == 30)
                {
                    return SqlBlob;
                }
                if (type == 40)
                {
                    return SqlClob;
                }
            }
            else
            {
                if (type != 60)
                {
                    if (type == 0x3d)
                    {
                        return SqlVarbinaryDefault;
                    }
                }
                else
                {
                    return SqlBinaryDefault;
                }
                switch (type)
                {
                    case 0x5b:
                        return SqlDate;

                    case 0x5c:
                        return SqlTime;

                    case 0x5d:
                        return SqlTimestamp;

                    case 0x5e:
                        return SqlTimeWithTimeZone;

                    case 0x5f:
                        return SqlTimestampWithTimeZone;

                    case 100:
                        return VarcharIgnorecaseDefault;

                    case 0x65:
                        return SqlIntervalYear;

                    case 0x66:
                        return SqlIntervalMonth;

                    case 0x67:
                        return SqlIntervalDay;

                    case 0x68:
                        return SqlIntervalHour;

                    case 0x69:
                        return SqlIntervalMinute;

                    case 0x6a:
                        return SqlIntervalSecond;

                    case 0x6b:
                        return SqlIntervalYearToMonth;

                    case 0x6c:
                        return SqlIntervalDayToHour;

                    case 0x6d:
                        return SqlIntervalDayToMinute;

                    case 110:
                        return SqlIntervalDayToSecond;

                    case 0x6f:
                        return SqlIntervalHourToMinute;

                    case 0x70:
                        return SqlIntervalHourToSecond;

                    case 0x71:
                        return SqlIntervalMinuteToSecond;

                    case 0x457:
                        return Other;
                }
            }
            return null;
        }

        public abstract string GetDefinition();
        public virtual string GetFullNameString()
        {
            return this.GetNameString();
        }

        public override int GetHashCode()
        {
            return (((this.TypeCode + ((int) this.Precision)) << ((8 + this.Scale) & 0x1f)) << 0x10);
        }

        public static int GetJdbcTypeCode(int type)
        {
            if (type <= 30)
            {
                if (type == 0x19)
                {
                    return -5;
                }
                if (type != 30)
                {
                    return type;
                }
                return 0x7d4;
            }
            if (type == 40)
            {
                return 0x7d5;
            }
            if (type != 60)
            {
                if (type == 0x3d)
                {
                    return -3;
                }
                return type;
            }
            return -2;
        }

        public virtual long GetMaxPrecision()
        {
            return 0L;
        }

        public virtual int GetMaxScale()
        {
            return 0;
        }

        public virtual QNameManager.QName GetName()
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return this.userTypeModifier.GetName();
        }

        public abstract string GetNameString();
        public virtual Grantee GetOwner()
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return this.userTypeModifier.GetOwner();
        }

        public virtual int GetPrecisionRadix()
        {
            return 0;
        }

        public virtual OrderedHashSet<QNameManager.QName> GetReferences()
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return this.userTypeModifier.GetReferences();
        }

        public virtual QNameManager.QName GetSchemaName()
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return this.userTypeModifier.GetSchemaName();
        }

        public virtual int GetSchemaObjectType()
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return this.userTypeModifier.GetSchemaObjectType();
        }

        public virtual string GetSql()
        {
            if (this.userTypeModifier == null)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return this.userTypeModifier.GetSQL();
        }

        public virtual int GetSqlGenericTypeCode()
        {
            return this.TypeCode;
        }

        public string GetTypeDefinition()
        {
            if (this.userTypeModifier == null)
            {
                return this.GetDefinition();
            }
            return this.GetName().GetSchemaQualifiedStatementName();
        }

        public static int GetTypeNr(string name)
        {
            int num;
            if (!TypeNames.TryGetValue(name, out num) && !TypeAliases.TryGetValue(name, out num))
            {
                num = -2147483648;
            }
            return num;
        }

        public static SqlType GetUtlType(UtlType dbTypeNumber)
        {
            switch (dbTypeNumber)
            {
                case UtlType.Null:
                    return SqlAllTypes;

                case UtlType.Boolean:
                    return SqlBoolean;

                case UtlType.TinyInt:
                    return Tinyint;

                case UtlType.SmallInt:
                    return SqlSmallint;

                case UtlType.Int:
                    return SqlInteger;

                case UtlType.BigInt:
                    return SqlBigint;

                case UtlType.Decimal:
                case UtlType.Money:
                    return SqlDecimalDefault;

                case UtlType.Double:
                    return SqlDouble;

                case UtlType.Char:
                    return SqlChar;

                case UtlType.VarChar:
                case UtlType.VarCharIngnoreCase:
                    return SqlVarchar;

                case UtlType.UniqueIdentifier:
                    return SqlUniqueIdentifier;

                case UtlType.Variant:
                    return Other;

                case UtlType.Time:
                    return SqlTime;

                case UtlType.TimeTZ:
                    return SqlTimeWithTimeZone;

                case UtlType.TimeStamp:
                    return SqlTimestamp;

                case UtlType.TimeStampTZ:
                    return SqlTimestampWithTimeZone;

                case UtlType.Date:
                    return SqlDate;

                case UtlType.IntervalDS:
                    return SqlIntervalDayToSecond;

                case UtlType.IntervalYM:
                    return SqlIntervalYearToMonth;

                case UtlType.Binary:
                    return SqlBinary;

                case UtlType.VarBinary:
                    return SqlVarbinary;

                case UtlType.Blob:
                    return SqlBlob;

                case UtlType.Clob:
                    return SqlClob;
            }
            throw new NotSupportedException(string.Format("DbType :{0}", dbTypeNumber));
        }

        public virtual bool IsArrayType()
        {
            return false;
        }

        public virtual bool IsBinaryType()
        {
            return false;
        }

        public virtual bool IsBooleanType()
        {
            return false;
        }

        public virtual bool IsCharacterType()
        {
            return false;
        }

        public virtual bool IsDateTimeType()
        {
            return false;
        }

        public virtual bool IsDateTimeTypeWithZone()
        {
            return false;
        }

        public virtual bool IsDistinctType()
        {
            return ((this.userTypeModifier != null) && (this.userTypeModifier.GetSchemaObjectType() == 12));
        }

        public virtual bool IsDomainType()
        {
            return ((this.userTypeModifier != null) && (this.userTypeModifier.GetSchemaObjectType() == 13));
        }

        public virtual bool IsExactNumberType()
        {
            return false;
        }

        public virtual bool IsGuidType()
        {
            return false;
        }

        public virtual bool IsIntegralType()
        {
            return false;
        }

        public virtual bool IsIntervalType()
        {
            return false;
        }

        public virtual bool IsLobType()
        {
            return false;
        }

        public virtual bool IsMultisetType()
        {
            return false;
        }

        public virtual bool IsNumberType()
        {
            return false;
        }

        public virtual bool IsObjectType()
        {
            return false;
        }

        public virtual bool IsRowType()
        {
            return false;
        }

        public static bool IsSupportedSqlType(int typeNumber)
        {
            return (GetDefaultType(typeNumber) > null);
        }

        public virtual bool IsTableType()
        {
            return false;
        }

        public static bool Matches(SqlType[] one, SqlType[] other)
        {
            for (int i = 0; i < one.Length; i++)
            {
                if (one[i].TypeCode != other[i].TypeCode)
                {
                    return false;
                }
            }
            return true;
        }

        public virtual object Mod(object a, object b, SqlType aType, SqlType bType)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual object Multiply(object a, object b, SqlType aType, SqlType bType)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual object Negate(object a)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual int PrecedenceDegree(SqlType other)
        {
            if (other.TypeCode == this.TypeCode)
            {
                return 0;
            }
            return -2147483648;
        }

        public virtual bool RequiresPrecision()
        {
            return false;
        }

        public virtual object Subtract(object a, object b, SqlType aType, SqlType bType)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual decimal ToDecimal(object a)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual int ToInt32(object a)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual long ToInt64(object a)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }

        public virtual double ToReal(object a)
        {
            throw Error.RuntimeError(0xc9, "Type");
        }
    }
}

