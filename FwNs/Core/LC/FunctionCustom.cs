namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class FunctionCustom : FunctionSQL
    {
        private const int FUNC_ISAUTOCOMMIT = 0x47;
        private const int FUNC_ISREADONLYSESSION = 0x48;
        private const int FUNC_ISREADONLYDATABASE = 0x49;
        private const int FUNC_ISREADONLYDATABASEFILES = 0x4a;
        private const int FUNC_DATABASE = 0x4b;
        private const int FUNC_IDENTITY = 0x4c;
        private const int FUNC_SYSDATE = 0x4d;
        private const int FUNC_TIMESTAMPADD = 0x4e;
        private const int FUNC_TIMESTAMPDIFF = 0x4f;
        private const int FUNC_TRUNCATE = 80;
        private const int FUNC_TO_CHAR = 0x51;
        private const int FUNC_TIMESTAMP = 0x52;
        private const int FUNC_CRYPT_KEY = 0x53;
        private const int FUNC_CRYPT_IV = 0x54;
        private const int FUNC_TO_DATE = 0x55;
        private const int FUNC_TRANSACTION_ID = 0x5f;
        private const int FUNC_ACOS = 0x65;
        private const int FUNC_ASIN = 0x66;
        private const int FUNC_ATAN = 0x67;
        private const int FUNC_ATAN2 = 0x68;
        private const int FUNC_COS = 0x69;
        private const int FUNC_COT = 0x6a;
        private const int FUNC_DEGREES = 0x6b;
        private const int FUNC_LOG10 = 110;
        private const int FUNC_PI = 0x6f;
        private const int FUNC_RADIANS = 0x70;
        private const int FUNC_RAND = 0x71;
        private const int FUNC_ROUND = 0x72;
        private const int FUNC_SIGN = 0x73;
        private const int FUNC_SIN = 0x74;
        private const int FUNC_TAN = 0x75;
        private const int FUNC_BITAND = 0x76;
        private const int FUNC_BITOR = 0x77;
        private const int FUNC_BITXOR = 120;
        private const int FUNC_ASCII = 0x7a;
        private const int FUNC_CHAR = 0x7b;
        private const int FUNC_CONCAT = 0x7c;
        private const int FUNC_DIFFERENCE = 0x7d;
        private const int FUNC_HEXTORAW = 0x7e;
        private const int FUNC_LEFT = 0x80;
        private const int FUNC_INSTR = 130;
        private const int FUNC_LTRIM = 0x83;
        private const int FUNC_RAWTOHEX = 0x84;
        private const int FUNC_REPEAT = 0x85;
        private const int FUNC_REPLACE = 0x86;
        private const int FUNC_REVERSE = 0x87;
        private const int FUNC_RIGHT = 0x88;
        private const int FUNC_RTRIM = 0x89;
        private const int FUNC_SOUNDEX = 0x8a;
        private const int FUNC_SPACE = 0x8b;
        private const int FUNC_SUBSTR = 140;
        private const int FUNC_DATEADD = 0x8d;
        private const int FUNC_DATEDIFF = 0x8e;
        private const int FUNC_SECONDS_MIDNIGHT = 0x8f;
        private const int FUNC_TRUNC = 0x90;
        private const int FUNC_LEAST = 0x91;
        private const int FUNC_GREATEST = 0x92;
        private const int FUNC_ADD_MONTHS = 0x93;
        private const int FUNC_NEXT_DAY = 0x94;
        private const int FUNC_LAST_DAY = 0x95;
        private const int FUNC_MONTHS_BETWEEN = 150;
        private const int FUNC_NEW_TIME = 0x97;
        public static string[] OpenGroupNumericFunctions = new string[] { 
            "ABS", "ACOS", "ASIN", "ATAN", "ATAN2", "BITAND", "BITOR", "BITXOR", "CEILING", "COS", "COT", "DEGREES", "EXP", "FLOOR", "LOG", "LOG10",
            "MOD", "PI", "POWER", "RADIANS", "RAND", "ROUND", "SIGN", "SIN", "SQRT", "TAN", "TRUNC"
        };
        public static string[] OpenGroupStringFunctions = new string[] { 
            "ASCII", "CHAR", "CONCAT", "DIFFERENCE", "HEXTORAW", "INSERT", "LCASE", "LEFT", "LENGTH", "LOCATE", "LTRIM", "RAWTOHEX", "REPEAT", "REPLACE", "RIGHT", "RTRIM",
            "SOUNDEX", "SPACE", "SUBSTR", "UCASE"
        };
        public static string[] OpenGroupDateTimeFunctions = new string[] { 
            "CURDATE", "CURTIME", "DATEDIFF", "DAYNAME", "DAYOFMONTH", "DAYOFWEEK", "DAYOFYEAR", "HOUR", "MINUTE", "MONTH", "MONTHNAME", "NOW", "QUARTER", "SECOND", "SECONDS_SINCE_MIDNIGHT", "TIMESTAMPADD",
            "TIMESTAMPDIFF", "TO_CHAR", "TO_DATE", "WEEK", "YEAR"
        };
        public static string[] OpenGroupSystemFunctions = new string[] { "DATABASE", "IFNULL", "USER" };
        private static readonly Dictionary<int, int> CustomRegularFuncMap = GetCustomRegularFuncMap();
        private static readonly Dictionary<int, int> CustomValueFuncMap = GetCustomValueFuncMap();
        private int _extractSpec;

        private FunctionCustom(int id)
        {
            base.FuncType = id;
            if (id == 5)
            {
                base.Name = "EXTRACT";
                base.ParseList = FunctionSQL.SingleParamList;
            }
            else
            {
                if (id != 0x1d)
                {
                    switch (id)
                    {
                        case 0x47:
                        case 0x48:
                        case 0x49:
                        case 0x4a:
                            base.ParseList = FunctionSQL.EmptyParamList;
                            return;

                        case 0x4b:
                            base.ParseList = FunctionSQL.EmptyParamList;
                            return;

                        case 0x4c:
                            base.Name = "IDENTITY";
                            base.ParseList = FunctionSQL.EmptyParamList;
                            return;

                        case 0x4e:
                            base.Name = "TIMESTAMPADD";
                            base.ParseList = new short[] { 
                                0x2b7, 0x2e5, 9, 0x2db, 0x2dc, 0x2dd, 0x2de, 0x2df, 0x2e0, 0x2e1, 0x2e2, 0x2e3, 0x2ac, 0x2b9, 0x2ac, 0x2b9,
                                0x2aa
                            };
                            return;

                        case 0x4f:
                            base.Name = "TIMESTAMPDIFF";
                            base.ParseList = new short[] { 
                                0x2b7, 0x2e5, 9, 0x2db, 0x2dc, 0x2dd, 0x2de, 0x2df, 0x2e0, 0x2e1, 0x2e2, 0x2e3, 0x2ac, 0x2b9, 0x2ac, 0x2b9,
                                0x2aa
                            };
                            return;

                        case 80:
                            base.ParseList = FunctionSQL.DoubleParamList;
                            return;

                        case 0x51:
                            base.ParseList = FunctionSQL.DoubleParamList;
                            base.ParseListAlt = FunctionSQL.SingleParamList;
                            return;

                        case 0x52:
                            base.Name = "TIMESTAMP";
                            base.ParseList = new short[] { 0x2b7, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2aa };
                            return;

                        case 0x53:
                        case 0x54:
                            base.ParseList = FunctionSQL.SingleParamList;
                            return;

                        case 0x55:
                            base.ParseList = FunctionSQL.DoubleParamList;
                            base.ParseListAlt = FunctionSQL.SingleParamList;
                            return;

                        case 0x5f:
                            base.ParseList = FunctionSQL.OptionalNoParamList;
                            return;

                        case 0x65:
                        case 0x66:
                        case 0x67:
                        case 0x68:
                        case 0x69:
                        case 0x6a:
                        case 0x6b:
                        case 110:
                        case 0x70:
                        case 0x73:
                        case 0x74:
                        case 0x75:
                        case 0x7a:
                        case 0x7b:
                        case 0x7e:
                        case 0x84:
                        case 0x87:
                        case 0x8a:
                        case 0x8b:
                            base.ParseList = FunctionSQL.SingleParamList;
                            return;

                        case 0x6f:
                            base.ParseList = FunctionSQL.EmptyParamList;
                            return;

                        case 0x71:
                            base.ParseList = FunctionSQL.OptionalIntegerParamList;
                            return;

                        case 0x72:
                        case 0x90:
                            base.ParseList = FunctionSQL.DoubleParamList;
                            base.ParseListAlt = FunctionSQL.SingleParamList;
                            return;

                        case 0x76:
                        case 0x77:
                        case 120:
                        case 0x7d:
                        case 0x85:
                        case 0x88:
                            base.ParseList = FunctionSQL.DoubleParamList;
                            return;

                        case 0x7c:
                            base.ParseList = FunctionSQL.ManyParamList;
                            return;

                        case 0x80:
                            base.ParseList = FunctionSQL.DoubleParamList;
                            return;

                        case 130:
                            base.ParseList = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2e6, 6, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2aa };
                            return;

                        case 0x86:
                            base.ParseList = FunctionSQL.TripleParamList;
                            base.ParseListAlt = FunctionSQL.DoubleParamList;
                            return;

                        case 0x8d:
                        case 0x97:
                            base.ParseList = FunctionSQL.TripleParamList;
                            return;

                        case 0x8e:
                            base.ParseList = FunctionSQL.TripleParamList;
                            base.ParseListAlt = FunctionSQL.DoubleParamList;
                            return;

                        case 0x91:
                        case 0x92:
                            base.ParseList = FunctionSQL.ManyParamList;
                            return;

                        case 0x93:
                        case 150:
                            base.ParseList = FunctionSQL.DoubleParamList;
                            return;

                        case 0x94:
                            base.ParseList = FunctionSQL.DoubleParamList;
                            return;

                        case 0x95:
                            base.ParseList = FunctionSQL.SingleParamList;
                            return;

                        case 30:
                            base.Name = "OVERLAY";
                            base.ParseList = FunctionSQL.QuadParamList;
                            return;
                    }
                }
                else
                {
                    base.Name = "TRIM";
                    base.ParseList = FunctionSQL.SingleParamList;
                    return;
                }
                throw Error.RuntimeError(0xc9, "FunctionCustom");
            }
        }

        private static Dictionary<int, int> GetCustomRegularFuncMap()
        {
            return new Dictionary<int, int> { 
                { 
                    0x1af,
                    7
                },
                { 
                    0x260,
                    6
                },
                { 
                    0x287,
                    8
                },
                { 
                    0x27c,
                    0x18
                },
                { 
                    0x29f,
                    0x19
                },
                { 
                    0x2a3,
                    0x3e
                },
                { 
                    0x142,
                    0x3f
                },
                { 
                    0x143,
                    0x40
                },
                { 
                    0x27e,
                    0x41
                },
                { 
                    0x267,
                    0x29
                },
                { 
                    0x268,
                    0x2c
                },
                { 
                    0x297,
                    0x15
                },
                { 
                    0x266,
                    0x53
                },
                { 
                    0x32f,
                    0x54
                },
                { 
                    0x141,
                    5
                },
                { 
                    0xab,
                    5
                },
                { 
                    0x48,
                    5
                },
                { 
                    0x7e,
                    5
                },
                { 
                    0xa7,
                    5
                },
                { 
                    0xf8,
                    5
                },
                { 
                    0x26f,
                    5
                },
                { 
                    0x282,
                    5
                },
                { 
                    0x270,
                    5
                },
                { 
                    0x271,
                    5
                },
                { 
                    0x272,
                    5
                },
                { 
                    0x289,
                    5
                },
                { 
                    0x2a0,
                    5
                },
                { 
                    0x292,
                    5
                },
                { 
                    640,
                    0x1d
                },
                { 
                    0x291,
                    0x1d
                },
                { 
                    0x97,
                    0x80
                },
                { 
                    0x7f,
                    0x4c
                },
                { 
                    0x29a,
                    0x4e
                },
                { 
                    0x29b,
                    0x4f
                },
                { 
                    0x125,
                    80
                },
                { 
                    0x29c,
                    0x51
                },
                { 
                    280,
                    0x52
                },
                { 
                    0x2a2,
                    0x55
                },
                { 
                    0x27d,
                    130
                },
                { 
                    0x85,
                    30
                },
                { 
                    0x28e,
                    0x87
                },
                { 
                    0x234,
                    0x4b
                },
                { 
                    0x278,
                    0x47
                },
                { 
                    0x27b,
                    0x48
                },
                { 
                    0x279,
                    0x49
                },
                { 
                    0x27a,
                    0x4a
                },
                { 
                    0x259,
                    0x65
                },
                { 
                    0x25b,
                    0x66
                },
                { 
                    0x25c,
                    0x67
                },
                { 
                    0x25d,
                    0x68
                },
                { 
                    0x264,
                    0x69
                },
                { 
                    0x265,
                    0x6a
                },
                { 
                    0x273,
                    0x6b
                },
                { 
                    0x27f,
                    110
                },
                { 
                    0x288,
                    0x6f
                },
                { 
                    650,
                    0x70
                },
                { 
                    0x28b,
                    0x71
                },
                { 
                    0x28f,
                    0x72
                },
                { 
                    0x2a4,
                    0x90
                },
                { 
                    0x293,
                    0x73
                },
                { 
                    660,
                    0x74
                },
                { 
                    0x299,
                    0x75
                },
                { 
                    0x25f,
                    0x76
                },
                { 
                    0x261,
                    0x77
                },
                { 
                    610,
                    120
                },
                { 
                    0x25a,
                    0x7a
                },
                { 
                    0x20,
                    0x7b
                },
                { 
                    0x263,
                    0x7c
                },
                { 
                    0x274,
                    0x7d
                },
                { 
                    630,
                    0x7e
                },
                { 
                    0x28c,
                    0x84
                },
                { 
                    0xe8,
                    0x85
                },
                { 
                    0x28d,
                    0x86
                },
                { 
                    0xee,
                    0x88
                },
                { 
                    0x295,
                    0x8a
                },
                { 
                    0x200,
                    0x8b
                },
                { 
                    0x269,
                    0x8d
                },
                { 
                    0x26a,
                    0x8e
                },
                { 
                    0x2a6,
                    0x91
                },
                { 
                    0x2a5,
                    0x92
                },
                { 
                    0x2a7,
                    0x93
                },
                { 
                    0x325,
                    0x94
                },
                { 
                    0x326,
                    0x95
                },
                { 
                    0x327,
                    150
                },
                { 
                    0x328,
                    0x97
                },
                { 
                    0x33b,
                    0x5f
                }
            };
        }

        private static Dictionary<int, int> GetCustomValueFuncMap()
        {
            return new Dictionary<int, int> { 
                { 
                    0x298,
                    50
                },
                { 
                    0x336,
                    50
                },
                { 
                    0x29d,
                    0x29
                },
                { 
                    0x286,
                    50
                }
            };
        }

        public override Expression GetFunctionExpression()
        {
            if (base.FuncType == 0x7c)
            {
                Expression left = base.nodes[0];
                for (int i = 1; i < base.nodes.Length; i++)
                {
                    left = new ExpressionArithmetic(0x24, left, base.nodes[i]);
                }
                return left;
            }
            return base.GetFunctionExpression();
        }

        public override string GetSql()
        {
            int funcType = base.FuncType;
            if (funcType <= 30)
            {
                if (funcType != 5)
                {
                    int num1 = funcType - 0x1d;
                }
                return base.GetSql();
            }
            switch (funcType)
            {
                case 0x47:
                case 0x48:
                case 0x49:
                case 0x4a:
                case 0x4b:
                    return new StringBuilder(base.Name).Append("(").Append(")").ToString();

                case 0x4c:
                    return new StringBuilder("IDENTITY").Append("(").Append(")").ToString();

                case 0x4d:
                case 0x52:
                case 0x56:
                case 0x57:
                case 0x58:
                case 0x59:
                case 90:
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x5e:
                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                case 100:
                case 0x6c:
                case 0x6d:
                    break;

                case 0x4e:
                    return new StringBuilder("TIMESTAMPADD").Append("(").Append(base.nodes[0].GetSql()).Append(",").Append(base.nodes[1].GetSql()).Append(",").Append(base.nodes[2].GetSql()).Append(")").ToString();

                case 0x4f:
                    return new StringBuilder("TIMESTAMPDIFF").Append("(").Append(base.nodes[0].GetSql()).Append(",").Append(base.nodes[1].GetSql()).Append(",").Append(base.nodes[2].GetSql()).Append(")").ToString();

                case 80:
                {
                    StringBuilder builder = new StringBuilder("TRUNCATE").Append('(').Append(base.nodes[0].GetSql());
                    if (base.nodes.Length > 1)
                    {
                        builder.Append(",").Append(base.nodes[1].GetSql());
                    }
                    return builder.Append(')').ToString();
                }
                case 0x51:
                    return new StringBuilder("TO_CHAR").Append('(').Append(base.nodes[0].GetSql()).Append(",").Append(base.nodes[1].GetSql()).Append(')').ToString();

                case 0x53:
                    return new StringBuilder("CRYPT_KEY").Append('(').Append(base.nodes[0].GetSql()).ToString();

                case 0x54:
                    return new StringBuilder("CRYPT_IV").Append('(').Append(base.nodes[0].GetSql()).ToString();

                case 0x55:
                {
                    StringBuilder builder2 = new StringBuilder("TO_DATE").Append('(').Append(base.nodes[0].GetSql());
                    if (base.nodes.Length > 1)
                    {
                        builder2.Append(",").Append(base.nodes[1].GetSql());
                    }
                    return builder2.Append(')').ToString();
                }
                case 0x5f:
                    return new StringBuilder("TRANSACTION_ID").Append("(").Append(")").ToString();

                case 0x65:
                case 0x66:
                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6b:
                case 110:
                case 0x70:
                case 0x73:
                case 0x74:
                case 0x75:
                    return new StringBuilder(base.Name).Append('(').Append(base.nodes[0].GetSql()).Append(')').ToString();

                case 0x6f:
                case 0x71:
                    return new StringBuilder(base.Name).Append('(').Append(')').ToString();

                case 0x72:
                {
                    StringBuilder builder3 = new StringBuilder("ROUND").Append('(').Append(base.nodes[0].GetSql());
                    if (base.nodes.Length > 1)
                    {
                        builder3.Append(",").Append(base.nodes[1].GetSql());
                    }
                    return builder3.Append(')').ToString();
                }
                case 0x90:
                {
                    StringBuilder builder4 = new StringBuilder("TRUNC").Append('(').Append(base.nodes[0].GetSql());
                    if (base.nodes.Length > 1)
                    {
                        builder4.Append(",").Append(base.nodes[1].GetSql());
                    }
                    return builder4.Append(')').ToString();
                }
                default:
                    if ((funcType - 0x91) <= 1)
                    {
                        StringBuilder builder5 = new StringBuilder(base.Name).Append('(').Append(base.nodes[0].GetSql());
                        for (int i = 1; i < base.nodes.Length; i++)
                        {
                            builder5.Append(",").Append(base.nodes[i].GetSql());
                        }
                        return builder5.Append(")").ToString();
                    }
                    break;
            }
            return base.GetSql();
        }

        public override object GetValue(Session session, object[] data)
        {
            long num6;
            int funcType = base.FuncType;
            if ((funcType == 5) || ((funcType - 0x1d) <= 1))
            {
                return base.GetValue(session, data);
            }
            switch (funcType)
            {
                case 0x47:
                    return session.IsAutoCommit();

                case 0x48:
                    return session.IsReadOnlyDefault();

                case 0x49:
                    return session.GetDatabase().DatabaseReadOnly;

                case 0x4a:
                    return session.GetDatabase().IsFilesReadOnly();

                case 0x4b:
                    return session.GetDatabase().GetPath();

                case 0x4c:
                {
                    object lastIdentity = session.GetLastIdentity();
                    if (lastIdentity is long)
                    {
                        return lastIdentity;
                    }
                    return (long) ((int) lastIdentity);
                }
                case 0x4e:
                    return this.ProcessTimeStampAdd(session, data);

                case 0x4f:
                    return this.ProcessTimeStampDiff(session, data);

                case 80:
                    goto Label_0D50;

                case 0x51:
                    if (data[0] != null)
                    {
                        if (base.nodes.Length == 1)
                        {
                            return base.DataType.ConvertToType(session, data[0], base.nodes[0].DataType);
                        }
                        if (data[1] == null)
                        {
                            return null;
                        }
                        string format = UtlDateTime.ToCSharpDatePattern((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType));
                        DateTime time = (DateTime) ((DateTimeType) base.nodes[0].DataType).ConvertSqltoCSharpGmt(session, data[0]);
                        return time.ToString(format);
                    }
                    return null;

                case 0x52:
                {
                    bool flag = base.nodes[1] == null;
                    if (data[0] != null)
                    {
                        if (flag)
                        {
                            return SqlType.SqlTimestamp.ConvertToType(session, data[0], base.nodes[0].DataType);
                        }
                        if (data[1] == null)
                        {
                            return null;
                        }
                        TimeData data2 = (TimeData) SqlType.SqlTime.ConvertToType(session, data[1], base.nodes[1].DataType);
                        return new TimestampData(((TimestampData) SqlType.SqlDate.ConvertToType(session, data[0], base.nodes[0].DataType)).GetSeconds() + data2.GetSeconds(), data2.GetNanos());
                    }
                    return null;
                }
                case 0x53:
                    return Crypto.GetNewStrKey((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType));

                case 0x54:
                    return Crypto.GetNewStrIv((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType));

                case 0x55:
                    if (data[0] != null)
                    {
                        if (base.nodes[0].DataType.IsNumberType() || base.nodes[0].DataType.IsDateTimeType())
                        {
                            data[0] = SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        }
                        if (base.nodes.Length > 1)
                        {
                            string fmt = UtlDateTime.ToCSharpDatePattern((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType));
                            return SqlType.SqlDate.ConvertToType(session, data[0], base.nodes[0].DataType, fmt);
                        }
                        return SqlType.SqlDate.ConvertToType(session, data[0], base.nodes[0].DataType);
                    }
                    return null;

                case 0x5f:
                    return session.TransactionTimestamp;

                case 0x65:
                    if (data[0] != null)
                    {
                        double d = Math.Acos(NumberType.ToDouble(data[0]));
                        if (double.IsNaN(d))
                        {
                            throw Error.GetError(0xd80, "ACOS");
                        }
                        return d;
                    }
                    return null;

                case 0x66:
                    if (data[0] != null)
                    {
                        double d = Math.Asin(NumberType.ToDouble(data[0]));
                        if (double.IsNaN(d))
                        {
                            throw Error.GetError(0xd80, "ASIN");
                        }
                        return d;
                    }
                    return null;

                case 0x67:
                    if (data[0] != null)
                    {
                        double d = Math.Atan(NumberType.ToDouble(data[0]));
                        if (double.IsNaN(d))
                        {
                            throw Error.GetError(0xd80, "ATAN");
                        }
                        return d;
                    }
                    return null;

                case 0x68:
                    if (data[0] != null)
                    {
                        return Math.Atan2(NumberType.ToDouble(data[0]), NumberType.ToDouble(data[1]));
                    }
                    return null;

                case 0x69:
                    if (data[0] != null)
                    {
                        return Math.Cos(NumberType.ToDouble(data[0]));
                    }
                    return null;

                case 0x6a:
                    if (data[0] != null)
                    {
                        double a = NumberType.ToDouble(data[0]);
                        return (1.0 / Math.Tan(a));
                    }
                    return null;

                case 0x6b:
                    if (data[0] != null)
                    {
                        return ((NumberType.ToDouble(data[0]) * 180.0) / 3.1415926535897931);
                    }
                    return null;

                case 110:
                    if (data[0] != null)
                    {
                        return Math.Log10(NumberType.ToDouble(data[0]));
                    }
                    return null;

                case 0x6f:
                    return 3.1415926535897931;

                case 0x70:
                    if (data[0] != null)
                    {
                        return ((NumberType.ToDouble(data[0]) * 3.1415926535897931) / 180.0);
                    }
                    return null;

                case 0x71:
                    if (base.nodes[0] != null)
                    {
                        int seed = Convert.ToInt32(data[0]);
                        return session.Random(seed);
                    }
                    return session.Random();

                case 0x72:
                    if (data[0] != null)
                    {
                        if (base.nodes[0].DataType.IsDateTimeType())
                        {
                            return ((DateTimeType) base.nodes[0].DataType).Round(session, (TimestampData) data[0]);
                        }
                        if (data[1] == null)
                        {
                            return null;
                        }
                        int p = (int) data[1];
                        return Library.Round(NumberType.ToDouble(data[0]), p);
                    }
                    return null;

                case 0x73:
                    if (data[0] != null)
                    {
                        return ((NumberType) base.nodes[0].DataType).CompareToZero(data[0]);
                    }
                    return null;

                case 0x74:
                    if (data[0] != null)
                    {
                        return Math.Sin(NumberType.ToDouble(data[0]));
                    }
                    return null;

                case 0x75:
                    if (data[0] != null)
                    {
                        return Math.Tan(NumberType.ToDouble(data[0]));
                    }
                    return null;

                case 0x76:
                case 0x77:
                case 120:
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] == null)
                        {
                            return null;
                        }
                    }
                    num6 = 0L;
                    long num7 = Convert.ToInt64(data[0]);
                    long num8 = Convert.ToInt64(data[1]);
                    switch (base.FuncType)
                    {
                        case 0x76:
                            num6 = num7 & num8;
                            goto Label_06DE;

                        case 0x77:
                            num6 = num7 | num8;
                            goto Label_06DE;

                        case 120:
                            num6 = num7 ^ num8;
                            goto Label_06DE;
                    }
                    break;
                }
                case 0x7a:
                    if (data[0] != null)
                    {
                        string str3;
                        if (base.nodes[0].DataType.IsLobType())
                        {
                            str3 = ((IClobData) data[0]).GetSubString(session, 0L, 1);
                        }
                        else
                        {
                            str3 = (string) data[0];
                        }
                        if (str3.Length == 0)
                        {
                            return null;
                        }
                        return (int) str3[0];
                    }
                    return null;

                case 0x7b:
                    if (data[0] != null)
                    {
                        return Convert.ToChar((int) data[0]).ToString();
                    }
                    return null;

                case 0x7d:
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[0] == null)
                        {
                            return null;
                        }
                    }
                    string str4 = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
                    return Library.Difference((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType), str4);
                }
                case 0x7e:
                    if (data[0] != null)
                    {
                        return base.DataType.ConvertToType(session, data[0], base.nodes[0].DataType);
                    }
                    return null;

                case 0x80:
                case 0x88:
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[0] == null)
                        {
                            return null;
                        }
                    }
                    int num12 = (int) data[1];
                    return ((CharacterType) base.DataType).Substring(session, data[0], 0L, (long) num12, true, base.FuncType == 0x88);
                }
                case 130:
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[0] == null)
                        {
                            return null;
                        }
                    }
                    string search = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
                    int num14 = (int) SqlType.SqlInteger.ConvertToType(session, data[2], base.nodes[2].DataType);
                    int num15 = (int) SqlType.SqlInteger.ConvertToType(session, data[3], base.nodes[3].DataType);
                    return Library.Locate((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType), search, new int?(num14), new int?(num15));
                }
                case 0x84:
                    if (data[0] != null)
                    {
                        return base.nodes[0].DataType.ConvertToString(data[0]);
                    }
                    return null;

                case 0x85:
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[0] == null)
                        {
                            return null;
                        }
                    }
                    int num17 = (int) SqlType.SqlInteger.ConvertToType(session, data[1], base.nodes[1].DataType);
                    return Library.Repeat((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType), new int?(num17));
                }
                case 0x86:
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[0] == null)
                        {
                            return null;
                        }
                    }
                    string replace = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
                    string with = string.Empty;
                    if (data.Length > 2)
                    {
                        with = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[2], base.nodes[2].DataType);
                    }
                    return Library.Replace((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType), replace, with);
                }
                case 0x87:
                    if (data[0] != null)
                    {
                        char[] array = new StringBuilder((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType)).ToString().ToCharArray();
                        Array.Reverse(array);
                        return new string(array);
                    }
                    return null;

                case 0x8a:
                    if (data[0] != null)
                    {
                        string s = string.Empty;
                        if (base.nodes[0].DataType.IsLobType())
                        {
                            s = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        }
                        else
                        {
                            s = (string) data[0];
                        }
                        return Library.Soundex(s);
                    }
                    return null;

                case 0x8b:
                    if (data[0] != null)
                    {
                        char[] array = new char[(int) data[0]];
                        ArrayUtil.FillArray(array, 0, ' ');
                        return new string(array);
                    }
                    return null;

                case 0x8f:
                    if (data[0] != null)
                    {
                        goto Label_0D50;
                    }
                    return null;

                case 0x90:
                    if (data[0] != null)
                    {
                        if (base.nodes[0].DataType.IsDateTimeType())
                        {
                            return ((DateTimeType) base.nodes[0].DataType).Trunc(session, (TimestampData) data[0]);
                        }
                        if (data[1] == null)
                        {
                            return null;
                        }
                        int s = (int) SqlType.SqlInteger.ConvertToDefaultType(session, data[1]);
                        object o = ((NumberType) base.nodes[0].DataType).Truncate(data[0], s);
                        return base.nodes[0].DataType.ConvertToDefaultType(session, o);
                    }
                    return null;

                case 0x91:
                    if (data[0] != null)
                    {
                        object a = data[0];
                        for (int i = 1; i < data.Length; i++)
                        {
                            if (data[i] == null)
                            {
                                return null;
                            }
                            object b = base.DataType.ConvertToType(session, data[i], base.nodes[i].DataType);
                            if (base.DataType.Compare(session, a, b, null, false) > 0)
                            {
                                a = b;
                            }
                        }
                        return a;
                    }
                    return null;

                case 0x92:
                    if (data[0] != null)
                    {
                        object a = data[0];
                        for (int i = 1; i < data.Length; i++)
                        {
                            if (data[i] == null)
                            {
                                return null;
                            }
                            object b = base.DataType.ConvertToType(session, data[i], base.nodes[i].DataType);
                            if (base.DataType.Compare(session, a, b, null, false) < 0)
                            {
                                a = b;
                            }
                        }
                        return a;
                    }
                    return null;

                case 0x94:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        string weekDay = (string) SqlType.SqlVarcharDefault.ConvertToDefaultType(session, data[1]);
                        return ((DateTimeType) base.nodes[0].DataType).GetNextDay(session, (TimestampData) data[0], weekDay);
                    }
                    return null;

                case 0x95:
                    if (data[0] != null)
                    {
                        return ((DateTimeType) base.nodes[0].DataType).GetLastDayOfMonth(session, (TimestampData) data[0]);
                    }
                    return null;

                case 0x97:
                    if (((data[0] != null) && (data[1] != null)) && (data[2] != null))
                    {
                        string thisZone = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
                        string newZone = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[2], base.nodes[2].DataType);
                        return ((DateTimeType) base.nodes[0].DataType).ConvertBetweenTimeZones(session, (TimestampData) data[0], thisZone, newZone);
                    }
                    return null;

                default:
                    throw Error.RuntimeError(0xc9, "FunctionCustom");
            }
        Label_06DE:
            switch (base.DataType.TypeCode)
            {
                case -6:
                    return (((int) num6) & 0xff);

                case 4:
                    return (int) num6;

                case 5:
                    return (((int) num6) & 0xffff);

                case 0x19:
                    return num6;

                default:
                    throw Error.GetError(0x15b9);
            }
        Label_0D50:
            if ((data[0] != null) && (data[1] != null))
            {
                return ((NumberType) base.DataType).Truncate(data[0], (int) data[1]);
            }
            return null;
        }

        public static bool IsRegularFunction(int tokenType)
        {
            return CustomRegularFuncMap.ContainsKey(tokenType);
        }

        public static bool IsValueFunction(int tokenType)
        {
            return CustomValueFuncMap.ContainsKey(tokenType);
        }

        public static FunctionSQL NewCustomFunction(string token, int tokenType)
        {
            int num;
            FunctionCustom custom;
            if (!CustomRegularFuncMap.TryGetValue(tokenType, out num) && !CustomValueFuncMap.TryGetValue(tokenType, out num))
            {
                return null;
            }
            if (tokenType <= 0x268)
            {
                if (tokenType > 0x143)
                {
                    if ((tokenType != 0x1af) && (tokenType != 0x260))
                    {
                        if ((tokenType - 0x267) <= 1)
                        {
                            return new FunctionSQL(num) { ParseList = FunctionSQL.EmptyParamList };
                        }
                        goto Label_014B;
                    }
                    goto Label_0142;
                }
                if (tokenType == 0x9a)
                {
                    goto Label_0142;
                }
                if ((tokenType - 0x142) <= 1)
                {
                    return new FunctionSQL(num) { 
                        ParseList = FunctionSQL.TripleParamList,
                        ParseListAlt = FunctionSQL.DoubleParamList
                    };
                }
                goto Label_014B;
            }
            if (tokenType <= 0x298)
            {
                switch (tokenType)
                {
                    case 0x27c:
                    case 0x27e:
                    case 0x287:
                        goto Label_0142;

                    case 0x27d:
                        goto Label_014B;

                    case 0x286:
                        return new FunctionSQL(num) { ParseList = FunctionSQL.OptionalNoParamList };

                    case 0x297:
                        return new FunctionSQL(num) { 
                            ParseList = FunctionSQL.TripleParamList,
                            ParseListAlt = FunctionSQL.DoubleParamList
                        };
                }
                if (tokenType != 0x298)
                {
                    goto Label_014B;
                }
            }
            else
            {
                switch (tokenType)
                {
                    case 0x29d:
                    case 0x29f:
                    case 0x2a3:
                        goto Label_0142;

                    case 670:
                        goto Label_014B;
                }
                if (tokenType != 0x336)
                {
                    goto Label_014B;
                }
            }
        Label_0142:
            return new FunctionSQL(num);
        Label_014B:
            custom = new FunctionCustom(num);
            switch (num)
            {
                case 0x1d:
                    if (tokenType != 640)
                    {
                        if (tokenType == 0x291)
                        {
                            custom._extractSpec = 0x11c;
                            custom.ParseListAlt = FunctionSQL.DoubleParamList;
                        }
                    }
                    else
                    {
                        custom._extractSpec = 0x95;
                        custom.ParseListAlt = FunctionSQL.DoubleParamList;
                    }
                    break;

                case 5:
                    switch (tokenType)
                    {
                        case 0x26f:
                            custom._extractSpec = 0x26b;
                            goto Label_0207;

                        case 0x270:
                            custom._extractSpec = 620;
                            goto Label_0207;

                        case 0x271:
                            custom._extractSpec = 0x26d;
                            goto Label_0207;

                        case 0x272:
                            custom._extractSpec = 0x26e;
                            goto Label_0207;
                    }
                    if (tokenType != 0x282)
                    {
                        custom._extractSpec = tokenType;
                    }
                    else
                    {
                        custom._extractSpec = 0x281;
                    }
                    break;
            }
        Label_0207:
            if (custom.Name == null)
            {
                custom.Name = token;
            }
            return custom;
        }

        private object ProcessTimeStampAdd(Session session, object[] data)
        {
            if ((data[1] == null) || (data[2] == null))
            {
                return null;
            }
            int valueData = (int) base.nodes[0].ValueData;
            long seconds = (long) SqlType.SqlBigint.ConvertToDefaultType(session, data[1]);
            TimestampData a = (TimestampData) data[2];
            switch (valueData)
            {
                case 0x2db:
                {
                    long num3 = seconds / 0x3b9aca00L;
                    int nanos = (int) (seconds % 0x3b9aca00L);
                    IntervalType sqlIntervalSecondMaxFraction = SqlType.SqlIntervalSecondMaxFraction;
                    object b = new IntervalSecondData(num3, nanos, sqlIntervalSecondMaxFraction);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalSecondMaxFraction);
                }
                case 0x2dc:
                {
                    IntervalType sqlIntervalSecond = SqlType.SqlIntervalSecond;
                    object b = IntervalSecondData.NewIntervalSeconds(seconds, sqlIntervalSecond);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalSecond);
                }
                case 0x2dd:
                {
                    IntervalType sqlIntervalMinute = SqlType.SqlIntervalMinute;
                    object b = IntervalSecondData.NewIntervalMinute(seconds, sqlIntervalMinute);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalMinute);
                }
                case 0x2de:
                {
                    IntervalType sqlIntervalHour = SqlType.SqlIntervalHour;
                    object b = IntervalSecondData.NewIntervalHour(seconds, sqlIntervalHour);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalHour);
                }
                case 0x2df:
                {
                    IntervalType sqlIntervalDay = SqlType.SqlIntervalDay;
                    object b = IntervalSecondData.NewIntervalDay(seconds, sqlIntervalDay);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalDay);
                }
                case 0x2e0:
                {
                    IntervalType sqlIntervalDay = SqlType.SqlIntervalDay;
                    object b = IntervalSecondData.NewIntervalDay(seconds * 7L, sqlIntervalDay);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalDay);
                }
                case 0x2e1:
                {
                    IntervalType sqlIntervalMonth = SqlType.SqlIntervalMonth;
                    object b = IntervalMonthData.NewIntervalMonth(seconds, sqlIntervalMonth);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalMonth);
                }
                case 0x2e2:
                {
                    IntervalType sqlIntervalMonth = SqlType.SqlIntervalMonth;
                    object b = IntervalMonthData.NewIntervalMonth(seconds * 3L, sqlIntervalMonth);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalMonth);
                }
                case 0x2e3:
                {
                    IntervalType sqlIntervalYear = SqlType.SqlIntervalYear;
                    object b = IntervalMonthData.NewIntervalYear(seconds, sqlIntervalYear);
                    return base.DataType.Add(a, b, base.nodes[2].DataType, sqlIntervalYear);
                }
            }
            throw Error.RuntimeError(0xc9, "FunctionCustom");
        }

        private object ProcessTimeStampDiff(Session session, object[] data)
        {
            TimestampData data2;
            TimestampData data3;
            if ((data[1] == null) || (data[2] == null))
            {
                return null;
            }
            int valueData = (int) base.nodes[0].ValueData;
            if (base.nodes[1].DataType.IsCharacterType())
            {
                try
                {
                    data2 = (TimestampData) SqlType.SqlTimestamp.ConvertToType(session, data[1], base.nodes[1].DataType);
                }
                catch (Exception)
                {
                    data2 = (TimestampData) SqlType.SqlDate.ConvertToType(session, data[1], base.nodes[1].DataType);
                }
            }
            else
            {
                data2 = (TimestampData) data[1];
            }
            if (base.nodes[2].DataType.IsCharacterType())
            {
                try
                {
                    data3 = (TimestampData) SqlType.SqlTimestamp.ConvertToType(session, data[2], base.nodes[2].DataType);
                }
                catch (Exception)
                {
                    data3 = (TimestampData) SqlType.SqlDate.ConvertToType(session, data[2], base.nodes[2].DataType);
                }
            }
            else
            {
                data3 = (TimestampData) data[2];
            }
            if (base.nodes[2].DataType.IsDateTimeTypeWithZone())
            {
                data3 = (TimestampData) SqlType.SqlTimestamp.ConvertToType(session, data3, SqlType.SqlTimestampWithTimeZone);
            }
            if (base.nodes[1].DataType.IsDateTimeTypeWithZone())
            {
                data2 = (TimestampData) SqlType.SqlTimestamp.ConvertToType(session, data2, SqlType.SqlTimestampWithTimeZone);
            }
            switch (valueData)
            {
                case 0x2db:
                {
                    IntervalSecondData data4 = (IntervalSecondData) SqlType.SqlIntervalSecondMaxPrecision.Subtract(data3, data2, null, null);
                    return ((0x3b9aca00L * data4.GetSeconds()) + data4.GetNanos());
                }
                case 0x2dc:
                    return SqlType.SqlIntervalSecondMaxPrecision.ConvertToLong(SqlType.SqlIntervalSecondMaxPrecision.Subtract(data3, data2, null, null));

                case 0x2dd:
                    return SqlType.SqlIntervalMinuteMaxPrecision.ConvertToLong(SqlType.SqlIntervalMinuteMaxPrecision.Subtract(data3, data2, null, null));

                case 0x2de:
                    return SqlType.SqlIntervalHourMaxPrecision.ConvertToLong(SqlType.SqlIntervalHourMaxPrecision.Subtract(data3, data2, null, null));

                case 0x2df:
                    return SqlType.SqlIntervalDayMaxPrecision.ConvertToLong(SqlType.SqlIntervalDayMaxPrecision.Subtract(data3, data2, null, null));

                case 0x2e0:
                    return (SqlType.SqlIntervalDayMaxPrecision.ConvertToLong(SqlType.SqlIntervalDayMaxPrecision.Subtract(data3, data2, null, null)) / 7L);

                case 0x2e1:
                    return SqlType.SqlIntervalMonthMaxPrecision.ConvertToLong(SqlType.SqlIntervalMonthMaxPrecision.Subtract(data3, data2, null, null));

                case 0x2e2:
                    return (SqlType.SqlIntervalMonthMaxPrecision.ConvertToLong(SqlType.SqlIntervalMonthMaxPrecision.Subtract(data3, data2, null, null)) / 3L);

                case 0x2e3:
                    return SqlType.SqlIntervalYearMaxPrecision.ConvertToLong(SqlType.SqlIntervalYearMaxPrecision.Subtract(data3, data2, null, null));
            }
            throw Error.RuntimeError(0xc9, "FunctionCustom");
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            int num3;
            int num4;
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i].ResolveTypes(session, this);
                }
            }
            int funcType = base.FuncType;
            if ((funcType == 5) || ((funcType - 0x1d) <= 1))
            {
                base.ResolveTypes(session, parent);
                return;
            }
            switch (funcType)
            {
                case 0x47:
                case 0x48:
                case 0x49:
                case 0x4a:
                    base.DataType = SqlType.SqlBoolean;
                    return;

                case 0x4b:
                    base.DataType = SqlType.SqlVarcharDefault;
                    return;

                case 0x4c:
                    base.DataType = SqlType.SqlBigint;
                    return;

                case 0x4e:
                    goto Label_1220;

                case 0x4f:
                    goto Label_1468;

                case 80:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlInteger;
                    }
                    else if (!base.nodes[1].DataType.IsIntegralType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if (!base.nodes[0].DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    base.DataType = base.nodes[0].DataType;
                    return;

                case 0x51:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    base.DataType = SqlType.SqlVarcharDefault;
                    if (base.nodes.Length > 1)
                    {
                        if (!base.nodes[0].DataType.IsDateTimeType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        if ((base.nodes[1].DataType == null) || !base.nodes[1].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15bf);
                        }
                        if (base.nodes[1].OpType == 1)
                        {
                            base.nodes[1].SetAsConstantValue(session);
                        }
                    }
                    return;

                case 0x52:
                {
                    SqlType dataType = base.nodes[0].DataType;
                    if (base.nodes[1] == null)
                    {
                        if (dataType == null)
                        {
                            dataType = base.nodes[0].DataType = SqlType.SqlVarcharDefault;
                        }
                        if ((!dataType.IsCharacterType() && (dataType.TypeCode != 0x5d)) && (dataType.TypeCode != 0x5f))
                        {
                            throw Error.GetError(0x15b9);
                        }
                        break;
                    }
                    if (dataType == null)
                    {
                        if (base.nodes[1].DataType == null)
                        {
                            dataType = base.nodes[0].DataType = base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                        }
                        else if (base.nodes[1].DataType.IsCharacterType())
                        {
                            dataType = base.nodes[0].DataType = SqlType.SqlVarcharDefault;
                        }
                        else
                        {
                            dataType = base.nodes[0].DataType = SqlType.SqlDate;
                        }
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        if (dataType.IsCharacterType())
                        {
                            base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                        }
                        else if (dataType.TypeCode == 0x5b)
                        {
                            base.nodes[1].DataType = SqlType.SqlTime;
                        }
                    }
                    if (((dataType.TypeCode != 0x5b) || (base.nodes[1].DataType.TypeCode != 0x5c)) && (!dataType.IsCharacterType() || !base.nodes[1].DataType.IsCharacterType()))
                    {
                        throw Error.GetError(0x15b9);
                    }
                    break;
                }
                case 0x53:
                case 0x54:
                    for (int j = 0; j < base.nodes.Length; j++)
                    {
                        if (base.nodes[j].DataType == null)
                        {
                            base.nodes[j].DataType = SqlType.SqlVarchar;
                        }
                        else if (!base.nodes[j].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15b9);
                        }
                    }
                    base.DataType = SqlType.SqlVarcharDefault;
                    return;

                case 0x55:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if ((!base.nodes[0].DataType.IsCharacterType() && !base.nodes[0].DataType.IsNumberType()) && !base.nodes[0].DataType.IsDateTimeType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    base.DataType = DateTimeType.GetDateTimeType(0x5b, 0);
                    if (base.nodes.Length > 1)
                    {
                        if ((base.nodes[1].DataType == null) || !base.nodes[1].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15bf);
                        }
                        if (base.nodes[1].OpType == 1)
                        {
                            base.nodes[1].SetAsConstantValue(session);
                        }
                    }
                    return;

                case 0x5f:
                    base.DataType = SqlType.SqlBigint;
                    return;

                case 0x65:
                case 0x66:
                case 0x67:
                case 0x69:
                case 0x6a:
                case 0x6b:
                case 110:
                case 0x70:
                case 0x74:
                case 0x75:
                    goto Label_15FA;

                case 0x68:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlDouble;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlDouble;
                    }
                    if (!base.nodes[0].DataType.IsNumberType() || !base.nodes[1].DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = SqlType.SqlDouble;
                    return;

                case 0x6f:
                    base.DataType = SqlType.SqlDouble;
                    return;

                case 0x71:
                    if (base.nodes[0] != null)
                    {
                        if (base.nodes[0].DataType != null)
                        {
                            if (!base.nodes[0].DataType.IsExactNumberType())
                            {
                                throw Error.GetError(0x15bd);
                            }
                        }
                        else
                        {
                            base.nodes[0].DataType = SqlType.SqlBigint;
                        }
                    }
                    base.DataType = SqlType.SqlDouble;
                    return;

                case 0x72:
                case 0x90:
                    if ((base.nodes[0].DataType == null) || !base.nodes[0].DataType.IsDateTimeType())
                    {
                        if (base.nodes.Length < 2)
                        {
                            Expression addition = new ExpressionValue(0, SqlType.SqlInteger);
                            base.nodes = ArrayUtil.ToAdjustedArray<Expression>(base.nodes, addition, 1, 1);
                        }
                        if (base.nodes[1].DataType == null)
                        {
                            base.nodes[1].DataType = SqlType.SqlInteger;
                        }
                        if (!base.nodes[1].DataType.IsExactNumberType())
                        {
                            throw Error.GetError(0x15b9);
                        }
                        goto Label_15FA;
                    }
                    base.DataType = base.nodes[0].DataType;
                    return;

                case 0x73:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlDouble;
                    }
                    if (!base.nodes[0].DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = SqlType.SqlInteger;
                    return;

                case 0x76:
                case 0x77:
                case 120:
                {
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = base.nodes[1].DataType;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = base.nodes[0].DataType;
                    }
                    for (int j = 0; j < base.nodes.Length; j++)
                    {
                        if (base.nodes[j].DataType == null)
                        {
                            base.nodes[j].DataType = SqlType.SqlInteger;
                        }
                    }
                    base.DataType = base.nodes[0].DataType.GetAggregateType(base.nodes[1].DataType);
                    int typeCode = base.DataType.TypeCode;
                    if (typeCode != -6)
                    {
                        if ((typeCode - 4) <= 1)
                        {
                            return;
                        }
                        if (typeCode != 0x19)
                        {
                            throw Error.GetError(0x15b9);
                        }
                    }
                    return;
                }
                case 0x7a:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarchar;
                    }
                    if (!base.nodes[0].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = SqlType.SqlInteger;
                    return;

                case 0x7b:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlInteger;
                    }
                    if (!base.nodes[0].DataType.IsExactNumberType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = SqlType.GetDataType(12, 0, 1L, 0);
                    return;

                case 0x7d:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarchar;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarchar;
                    }
                    base.DataType = SqlType.SqlInteger;
                    return;

                case 0x7e:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarchar;
                    }
                    if (!base.nodes[0].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = (base.nodes[0].DataType.Precision == 0) ? SqlType.SqlVarbinaryDefault : SqlType.GetDataType(0x3d, 0, base.nodes[0].DataType.Precision / 2L, 0);
                    return;

                case 0x80:
                case 0x88:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarchar;
                    }
                    if (!base.nodes[0].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlInteger;
                    }
                    if (!base.nodes[1].DataType.IsExactNumberType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = (base.nodes[0].DataType.Precision == 0) ? SqlType.SqlVarcharDefault : SqlType.GetDataType(12, 0, base.nodes[0].DataType.Precision, 0);
                    return;

                case 130:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarchar;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarchar;
                    }
                    if (base.nodes[2] == null)
                    {
                        base.nodes[2] = new ExpressionValue(0, SqlType.SqlInteger);
                    }
                    if (base.nodes[3] == null)
                    {
                        base.nodes[3] = new ExpressionValue(1, SqlType.SqlInteger);
                    }
                    if (base.nodes[2].DataType == null)
                    {
                        base.nodes[2].DataType = SqlType.SqlInteger;
                    }
                    if (base.nodes[3].DataType == null)
                    {
                        base.nodes[3].DataType = SqlType.SqlInteger;
                    }
                    if ((!(base.nodes[0].DataType.IsCharacterType() && base.nodes[1].DataType.IsCharacterType()) || !base.nodes[2].DataType.IsExactNumberType()) || !base.nodes[3].DataType.IsExactNumberType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = SqlType.SqlInteger;
                    return;

                case 0x84:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarbinary;
                    }
                    if (!base.nodes[0].DataType.IsBinaryType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = (base.nodes[0].DataType.Precision == 0) ? SqlType.SqlVarcharDefault : SqlType.GetDataType(12, 0, base.nodes[0].DataType.Precision * 2L, 0);
                    return;

                case 0x85:
                {
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarchar;
                    }
                    bool flag = base.nodes[0].DataType.IsCharacterType();
                    if (!flag && !base.nodes[0].DataType.IsBinaryType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = flag ? ((SqlType) SqlType.SqlVarcharDefault) : ((SqlType) SqlType.SqlVarbinaryDefault);
                    return;
                }
                case 0x86:
                    for (int j = 0; j < base.nodes.Length; j++)
                    {
                        if (base.nodes[j].DataType == null)
                        {
                            base.nodes[j].DataType = SqlType.SqlVarchar;
                        }
                        else if (!base.nodes[j].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15b9);
                        }
                    }
                    base.DataType = SqlType.SqlVarcharDefault;
                    return;

                case 0x87:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarcharDefault;
                    }
                    base.DataType = base.nodes[0].DataType;
                    if (!base.DataType.IsCharacterType() || base.DataType.IsLobType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    return;

                case 0x8a:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarchar;
                    }
                    if (!base.nodes[0].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = SqlType.GetDataType(12, 0, 4L, 0);
                    return;

                case 0x8b:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlInteger;
                    }
                    if (!base.nodes[0].DataType.IsIntegralType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = SqlType.SqlVarcharDefault;
                    return;

                case 0x8d:
                    goto Label_10AF;

                case 0x8e:
                    goto Label_12E2;

                case 0x91:
                case 0x92:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = base.nodes[0].DataType;
                    for (int j = 1; j < base.nodes.Length; j++)
                    {
                        if (base.nodes[j].DataType == null)
                        {
                            throw Error.GetError(0x15b9);
                        }
                        base.DataType = base.DataType.GetCombinedType(base.nodes[j].DataType, 0x2b);
                        if (base.DataType == null)
                        {
                            throw Error.GetError(0x15b9);
                        }
                    }
                    return;

                case 0x93:
                {
                    Expression addition = new ExpressionValue("mm", SqlType.SqlVarcharDefault);
                    base.nodes = ArrayUtil.ToAdjustedArray<Expression>(base.nodes, addition, 0, 1);
                    Expression expression3 = base.nodes[1];
                    base.nodes[1] = base.nodes[2];
                    base.nodes[2] = expression3;
                    goto Label_10AF;
                }
                case 0x94:
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    if ((!base.nodes[1].DataType.IsCharacterType() || (base.nodes[0].DataType == null)) || !base.nodes[0].DataType.IsDateTimeType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = base.nodes[0].DataType;
                    return;

                case 0x95:
                    if ((base.nodes[0].DataType == null) || !base.nodes[0].DataType.IsDateTimeType())
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = base.nodes[0].DataType;
                    return;

                case 150:
                {
                    Expression addition = new ExpressionValue("mm", SqlType.SqlVarcharDefault);
                    base.nodes = ArrayUtil.ToAdjustedArray<Expression>(base.nodes, addition, 0, 1);
                    Expression expression5 = base.nodes[1];
                    base.nodes[1] = base.nodes[2];
                    base.nodes[2] = expression5;
                    goto Label_12E2;
                }
                case 0x97:
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    if (base.nodes[2].DataType == null)
                    {
                        base.nodes[2].DataType = SqlType.SqlVarcharDefault;
                    }
                    if ((!base.nodes[1].DataType.IsCharacterType() || !base.nodes[2].DataType.IsCharacterType()) || ((base.nodes[0].DataType == null) || !base.nodes[0].DataType.IsDateTimeType()))
                    {
                        throw Error.GetError(0x15b9);
                    }
                    base.DataType = base.nodes[0].DataType;
                    return;

                default:
                    throw Error.RuntimeError(0xc9, "FunctionCustom");
            }
            base.DataType = SqlType.SqlTimestamp;
            return;
        Label_10AF:
            if (!base.nodes[0].DataType.IsCharacterType() || base.nodes[0].DataType.IsLobType())
            {
                throw Error.GetError(0x15b9);
            }
            if ("yy".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num3 = 0x2e3;
            }
            else if ("mm".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num3 = 0x2e1;
            }
            else if ("dd".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num3 = 0x2df;
            }
            else if ("hh".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num3 = 0x2de;
            }
            else if ("mi".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num3 = 0x2dd;
            }
            else if ("ss".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num3 = 0x2dc;
            }
            else
            {
                if (!"ms".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
                {
                    throw Error.GetError(0x15b9);
                }
                num3 = 0x2db;
            }
            base.nodes[0].ValueData = num3;
            base.FuncType = 0x4e;
        Label_1220:
            if (base.nodes[1].DataType == null)
            {
                base.nodes[1].DataType = SqlType.SqlBigint;
            }
            if (base.nodes[2].DataType == null)
            {
                base.nodes[2].DataType = SqlType.SqlTimestamp;
            }
            if (!base.nodes[1].DataType.IsIntegralType())
            {
                throw Error.GetError(0x15b9);
            }
            if (((base.nodes[2].DataType.TypeCode != 0x5b) && (base.nodes[2].DataType.TypeCode != 0x5d)) && (base.nodes[2].DataType.TypeCode != 0x5f))
            {
                throw Error.GetError(0x15b9);
            }
            base.DataType = base.nodes[2].DataType;
            return;
        Label_12E2:
            if (!base.nodes[0].DataType.IsCharacterType() || base.nodes[0].DataType.IsLobType())
            {
                throw Error.GetError(0x15bd);
            }
            if ("yy".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num4 = 0x2e3;
            }
            else if ("mm".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num4 = 0x2e1;
            }
            else if ("dd".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num4 = 0x2df;
            }
            else if ("hh".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num4 = 0x2de;
            }
            else if ("mi".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num4 = 0x2dd;
            }
            else if ("ss".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
            {
                num4 = 0x2dc;
            }
            else
            {
                if (!"ms".Equals((string) base.nodes[0].ValueData, StringComparison.OrdinalIgnoreCase))
                {
                    throw Error.GetError(0xd90, (string) base.nodes[0].ValueData);
                }
                num4 = 0x2db;
            }
            base.nodes[0].ValueData = num4;
            base.FuncType = 0x4f;
        Label_1468:
            if (base.nodes[1].DataType == null)
            {
                base.nodes[1].DataType = base.nodes[2].DataType;
            }
            if (base.nodes[2].DataType == null)
            {
                base.nodes[2].DataType = base.nodes[1].DataType;
            }
            if (base.nodes[1].DataType == null)
            {
                base.nodes[1].DataType = SqlType.SqlTimestamp;
                base.nodes[2].DataType = SqlType.SqlTimestamp;
            }
            switch (base.nodes[1].DataType.TypeCode)
            {
                case 0x5b:
                {
                    if ((base.nodes[2].DataType.TypeCode != 0x5b) && (base.nodes[2].DataType.TypeCode != 0x5d))
                    {
                        throw Error.GetError(0x15bd);
                    }
                    int valueData = (int) base.nodes[0].ValueData;
                    if ((valueData - 0x2df) > 4)
                    {
                        throw Error.GetError(0x15bd);
                    }
                    break;
                }
                case 0x5d:
                case 0x5f:
                    if (((base.nodes[2].DataType.TypeCode != 0x5d) && (base.nodes[2].DataType.TypeCode != 0x5f)) && (base.nodes[2].DataType.TypeCode != 0x5b))
                    {
                        throw Error.GetError(0x15bd);
                    }
                    break;

                case 12:
                case 1:
                    break;

                default:
                    throw Error.GetError(0x15bd);
            }
            base.DataType = SqlType.SqlBigint;
            return;
        Label_15FA:
            if (base.nodes[0].DataType == null)
            {
                base.nodes[0].DataType = SqlType.SqlDouble;
            }
            if (!base.nodes[0].DataType.IsNumberType())
            {
                throw Error.GetError(0x15b9);
            }
            base.DataType = SqlType.SqlDouble;
        }

        public override void SetArguments(Expression[] nodes)
        {
            int funcType = base.FuncType;
            switch (funcType)
            {
                case 5:
                    nodes = new Expression[] { new ExpressionValue(this._extractSpec, SqlType.SqlInteger), nodes[0] };
                    break;

                case 0x1d:
                {
                    Expression[] expressionArray = new Expression[3];
                    expressionArray[0] = new ExpressionValue(this._extractSpec, SqlType.SqlInteger);
                    if ((nodes.Length > 1) && (nodes[1] != null))
                    {
                        expressionArray[1] = nodes[1];
                    }
                    else
                    {
                        expressionArray[1] = new ExpressionValue(" ", SqlType.SqlChar);
                    }
                    expressionArray[2] = nodes[0];
                    nodes = expressionArray;
                    break;
                }
                case 30:
                {
                    Expression expression = nodes[1];
                    Expression expression2 = nodes[2];
                    nodes[1] = nodes[3];
                    nodes[2] = expression;
                    nodes[3] = expression2;
                    break;
                }
                default:
                    if ((funcType == 0x8e) && (nodes.Length == 2))
                    {
                        nodes = new Expression[] { new ExpressionValue("dd", SqlType.SqlVarcharDefault), nodes[1], nodes[0] };
                    }
                    break;
            }
            base.SetArguments(nodes);
        }
    }
}

