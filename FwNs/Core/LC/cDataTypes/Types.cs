namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Collections.Generic;

    public static class Types
    {
        public const int SqlChar = 1;
        public const int SqlNumeric = 2;
        public const int SqlDecimal = 3;
        public const int SqlInteger = 4;
        public const int SqlSmallint = 5;
        public const int SqlFloat = 6;
        public const int SqlReal = 7;
        public const int SqlDouble = 8;
        public const int SqlVarchar = 12;
        public const int SqlBoolean = 0x10;
        public const int SqlUserDefinedType = 0x11;
        public const int SqlRow = 0x13;
        public const int SqlRef = 20;
        public const int SqlTable = 0x15;
        public const int SqlBigint = 0x19;
        public const int SqlBlob = 30;
        public const int SqlClob = 40;
        public const int SqlArray = 50;
        public const int SqlMultiset = 0x37;
        public const int SqlBinary = 60;
        public const int SqlVarbinary = 0x3d;
        public const int SqlDate = 0x5b;
        public const int SqlTime = 0x5c;
        public const int SqlTimestamp = 0x5d;
        public const int SqlTimeWithTimeZone = 0x5e;
        public const int SqlTimestampWithTimeZone = 0x5f;
        public const int SqlIntervalYear = 0x65;
        public const int SqlIntervalMonth = 0x66;
        public const int SqlIntervalDay = 0x67;
        public const int SqlIntervalHour = 0x68;
        public const int SqlIntervalMinute = 0x69;
        public const int SqlIntervalSecond = 0x6a;
        public const int SqlIntervalYearToMonth = 0x6b;
        public const int SqlIntervalDayToHour = 0x6c;
        public const int SqlIntervalDayToMinute = 0x6d;
        public const int SqlIntervalDayToSecond = 110;
        public const int SqlIntervalHourToMinute = 0x6f;
        public const int SqlIntervalHourToSecond = 0x70;
        public const int SqlIntervalMinuteToSecond = 0x71;
        public const int SqlTypeNumberLimit = 0x100;
        public const int SqlDatalink = 70;
        public const int SqlUdt = 0x11;
        public const int SqlUdtLocator = 0x12;
        public const int SqlBlobLocator = 0x1f;
        public const int SqlClobLocator = 0x29;
        public const int SqlArrayLocator = 0x33;
        public const int SqlMultisetLocator = 0x38;
        public const int SqlAllTypes = 0;
        public const int SqlDatetime = 9;
        public const int SqlInterval = 10;
        public const int SqlXml = 0x89;
        public const int SqlNchar = -8;
        public const int SqlWchar = -8;
        public const int SqlWvarchar = -9;
        public const int SqlNvarchar = -9;
        public const int SqlWlongvarchar = -10;
        public const int SqlNtext = -10;
        public const int SqlLongvarbinary = -4;
        public const int SqlImage = -4;
        public const int SqlGuid = -11;
        public const int SqlVariant = -150;
        public const int SqlSubDistinct = 1;
        public const int SqlSubStructured = 2;
        public const int VarcharIgnorecase = 100;
        public const int Array = 0x7d3;
        public const int Bigint = -5;
        public const int Binary = -2;
        public const int Bit = -7;
        public const int Blob = 0x7d4;
        public const int Boolean = 0x10;
        public const int CHAR = 1;
        public const int Clob = 0x7d5;
        public const int Datalink = 70;
        public const int Date = 0x5b;
        public const int DECIMAL = 3;
        public const int Distinct = 0x7d1;
        public const int DOUBLE = 8;
        public const int FLOAT = 6;
        public const int Integer = 4;
        public const int CsharpObject = 0x7d0;
        public const int Longvarbinary = -4;
        public const int Longvarchar = -1;
        public const int Multiset = 0;
        public const int NULL = 0;
        public const int Numeric = 2;
        public const int Other = 0x457;
        public const int Real = 7;
        public const int REF = 0x7d6;
        public const int Smallint = 5;
        public const int STRUCT = 0x7d2;
        public const int Time = 0x5c;
        public const int Timestamp = 0x5d;
        public const int Tinyint = -6;
        public const int Varbinary = -3;
        public const int Varchar = 12;
        public const int Rowid = 0x7d8;
        public const int Nchar = -8;
        public const int Nvarchar = -9;
        public const int Longnvarchar = -10;
        public const int Nclob = 0x7d7;
        public const int Sqlxml = 0x7d9;
        public const int TypeSubDefault = 1;
        public const int TypeSubIgnorecase = 4;
        private static readonly Dictionary<string, int> DotNetTypeNumbers;
        public static int[][] AllTypes;

        static Types()
        {
            Dictionary<string, int> dictionary1 = new Dictionary<string, int>(0x20) {
                { 
                    "int",
                    4
                },
                { 
                    "System.Int32",
                    4
                },
                { 
                    "double",
                    8
                },
                { 
                    "System.Double",
                    8
                },
                { 
                    "System.String",
                    12
                },
                { 
                    "Datetime",
                    0x5b
                },
                { 
                    "System.DateTime",
                    0x5b
                },
                { 
                    "decimal",
                    3
                },
                { 
                    "System.Decimal",
                    3
                },
                { 
                    "bool",
                    0x10
                },
                { 
                    "System.Boolean",
                    0x10
                },
                { 
                    "byte",
                    -6
                },
                { 
                    "System.Byte",
                    -6
                },
                { 
                    "short",
                    5
                },
                { 
                    "System.Int16",
                    5
                },
                { 
                    "long",
                    0x19
                },
                { 
                    "System.Int64",
                    0x19
                },
                { 
                    "byte[]",
                    60
                },
                { 
                    "System.Object",
                    0x457
                },
                { 
                    "System.Void",
                    0
                },
                { 
                    "System.Guid",
                    -11
                }
            };
            DotNetTypeNumbers = dictionary1;
            int[][] numArrayArray1 = new int[0x22][];
            numArrayArray1[0] = new int[] { 50, 1 };
            numArrayArray1[1] = new int[] { 0x19, 1 };
            numArrayArray1[2] = new int[] { 60, 1 };
            numArrayArray1[3] = new int[] { 0x3d, 1 };
            numArrayArray1[4] = new int[] { 30, 1 };
            numArrayArray1[5] = new int[] { 0x10, 1 };
            numArrayArray1[6] = new int[] { 1, 1 };
            numArrayArray1[7] = new int[] { 40, 1 };
            numArrayArray1[8] = new int[] { 70, 1 };
            numArrayArray1[9] = new int[] { 0x5b, 1 };
            numArrayArray1[10] = new int[] { 3, 1 };
            numArrayArray1[11] = new int[] { 0x7d1, 1 };
            numArrayArray1[12] = new int[] { 8, 1 };
            numArrayArray1[13] = new int[] { 6, 1 };
            numArrayArray1[14] = new int[] { 4, 1 };
            numArrayArray1[15] = new int[] { 0x7d0, 1 };
            numArrayArray1[0x10] = new int[] { -8, 1 };
            numArrayArray1[0x11] = new int[] { 0x7d7, 1 };
            int[] numArray19 = new int[2];
            numArray19[1] = 1;
            numArrayArray1[0x12] = numArray19;
            numArrayArray1[0x13] = new int[] { 2, 1 };
            numArrayArray1[20] = new int[] { -9, 1 };
            numArrayArray1[0x15] = new int[] { 0x457, 1 };
            numArrayArray1[0x16] = new int[] { 7, 1 };
            numArrayArray1[0x17] = new int[] { 20, 1 };
            numArrayArray1[0x18] = new int[] { 0x7d8, 1 };
            numArrayArray1[0x19] = new int[] { 5, 1 };
            numArrayArray1[0x1a] = new int[] { 0x7d2, 1 };
            numArrayArray1[0x1b] = new int[] { 0x5c, 1 };
            numArrayArray1[0x1c] = new int[] { 0x5d, 1 };
            numArrayArray1[0x1d] = new int[] { -6, 1 };
            numArrayArray1[30] = new int[] { 12, 1 };
            numArrayArray1[0x1f] = new int[] { 12, 4 };
            numArrayArray1[0x20] = new int[] { 0x89, 1 };
            numArrayArray1[0x21] = new int[] { -11, 1 };
            AllTypes = numArrayArray1;
        }

        public static bool AcceptsPrecision(int type)
        {
            if (type <= 30)
            {
                if (type <= 6)
                {
                    switch (type)
                    {
                        case -9:
                        case -8:
                        case -4:
                        case -1:
                        case 1:
                        case 2:
                        case 3:
                        case 6:
                            goto Label_00EC;

                        case -7:
                        case -6:
                        case -5:
                        case -3:
                        case -2:
                        case 0:
                            return false;
                    }
                    return false;
                }
                if ((type != 12) && (type != 30))
                {
                    return false;
                }
            }
            else if (type <= 50)
            {
                if ((type != 40) && (type != 50))
                {
                    return false;
                }
            }
            else if ((type - 60) > 1)
            {
                switch (type)
                {
                    case 0x5c:
                    case 0x5d:
                    case 100:
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
                        goto Label_00EC;

                    case 0x5e:
                    case 0x5f:
                    case 0x60:
                    case 0x61:
                    case 0x62:
                    case 0x63:
                        return false;
                }
                if (type != 0x7d7)
                {
                    return false;
                }
            }
        Label_00EC:
            return true;
        }

        public static bool AcceptsScaleCreateParam(int type)
        {
            return (((type - 2) <= 1) || (type == 0x6a));
        }

        public static bool AcceptsZeroPrecision(int type)
        {
            return ((type - 0x5c) <= 1);
        }

        public static SqlType GetParameterSqlType(Type c)
        {
            string fullName;
            int num;
            if (c == null)
            {
                throw Error.RuntimeError(0xc9, "Types");
            }
            if (c.Equals(typeof(void)))
            {
                return SqlType.SqlAllTypes;
            }
            try
            {
                if (c.IsGenericType && (c.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    fullName = c.GetGenericArguments()[0].FullName;
                }
                else if (c.IsByRef)
                {
                    fullName = c.GetElementType().FullName;
                }
                else
                {
                    fullName = c.FullName;
                }
            }
            catch (Exception)
            {
                fullName = c.FullName;
            }
            if (!DotNetTypeNumbers.TryGetValue(fullName, out num))
            {
                num = -2147483648;
                if (c.IsArray)
                {
                    while (c.IsArray)
                    {
                        c = c.GetElementType();
                    }
                    if (c.IsPrimitive || c.IsSerializable)
                    {
                        num = 0x457;
                    }
                }
                else if (c.IsSerializable)
                {
                    num = 0x457;
                }
            }
            return SqlType.GetDefaultTypeWithSize(num);
        }

        public static string GetTypeName(int type)
        {
            if (type <= 50)
            {
                if (type > 0x19)
                {
                    if (type == 30)
                    {
                        return "BLOB";
                    }
                    if (type == 40)
                    {
                        return "CLOB";
                    }
                    if (type == 50)
                    {
                        return "ARRAY";
                    }
                }
                else
                {
                    switch (type)
                    {
                        case -11:
                            return "UNIQUEIDENTIFIER";

                        case -9:
                            return "NVARCHAR";

                        case -8:
                            return "NCHAR";

                        case -6:
                            return "TINYINT";

                        case 0:
                            return "NULL";

                        case 1:
                            return "CHAR";

                        case 2:
                            return "NUMERIC";

                        case 3:
                            return "DECIMAL";

                        case 4:
                            return "INTEGER";

                        case 5:
                            return "SMALLINT";

                        case 6:
                            return "FLOAT";

                        case 7:
                            return "REAL";

                        case 8:
                            return "DOUBLE";

                        case 12:
                            return "VARCHAR";

                        case 0x10:
                            return "BOOLEAN";

                        case 20:
                            return "REF";

                        case 0x19:
                            return "BIGINT";
                    }
                }
            }
            else if (type <= 0x5d)
            {
                if (type != 60)
                {
                    if (type == 0x3d)
                    {
                        return "VARBINARY";
                    }
                }
                else
                {
                    return "BINARY";
                }
                switch (type)
                {
                    case 0x5b:
                        return "DATE";

                    case 0x5c:
                        return "TIME";

                    case 0x5d:
                        return "TIMESTAMP";

                    case 70:
                        return "DATALINK";
                }
            }
            else if (type <= 0x89)
            {
                if (type == 100)
                {
                    return "VARCHAR_IGNORECASE";
                }
                if (type == 0x89)
                {
                    return "XML";
                }
            }
            else
            {
                switch (type)
                {
                    case 0x7d0:
                        return "CSHARP_OBJECT";

                    case 0x7d1:
                        return "DISTINCT";

                    case 0x7d2:
                        return "STRUCT";

                    case 0x7d7:
                        return "NCLOB";

                    case 0x7d8:
                        return "ROWID";

                    case 0x457:
                        return "OTHER";
                }
            }
            return null;
        }

        public static bool IsSearchable(int type)
        {
            if (type <= 40)
            {
                if ((type != 30) && (type != 40))
                {
                    return true;
                }
            }
            else
            {
                switch (type)
                {
                    case 0x7d0:
                    case 0x7d2:
                    case 0x457:
                        goto Label_0041;

                    case 0x7d1:
                        return true;
                }
                if ((type - 0x7d7) > 1)
                {
                    return true;
                }
            }
        Label_0041:
            return false;
        }

        public static bool RequiresPrecision(int type)
        {
            return (type == 100);
        }
    }
}

