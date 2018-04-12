namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cParsing;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class FunctionSQL : Expression
    {
        private const int FUNC_POSITION_CHAR = 1;
        private const int FUNC_POSITION_BINARY = 2;
        private const int FUNC_REGEXP_REPLACE = 4;
        protected const int FUNC_EXTRACT = 5;
        protected const int FUNC_BIT_LENGTH = 6;
        protected const int FUNC_CHAR_LENGTH = 7;
        protected const int FUNC_OCTET_LENGTH = 8;
        private const int FUNC_CARDINALITY = 9;
        private const int FUNC_ABS = 10;
        private const int FUNC_MOD = 11;
        protected const int FUNC_LN = 12;
        private const int FUNC_EXP = 13;
        private const int FUNC_POWER = 14;
        private const int FUNC_SQRT = 15;
        private const int FUNC_FLOOR = 0x10;
        private const int FUNC_CEILING = 0x11;
        protected const int FUNC_SUBSTRING_CHAR = 0x15;
        private const int FUNC_REGEXP_INSTR = 0x16;
        private const int FUNC_REGEXP_SUBSTR = 0x17;
        protected const int FUNC_FOLD_LOWER = 0x18;
        protected const int FUNC_FOLD_UPPER = 0x19;
        private const int FUNC_TRANSCODING = 0x1a;
        private const int FUNC_TRANSLITERATION = 0x1b;
        private const int FUNC_REGEX_TRANSLITERATION = 0x1c;
        protected const int FUNC_TRIM_CHAR = 0x1d;
        public const int FUNC_OVERLAY_CHAR = 30;
        private const int FUNC_CHAR_NORMALIZE = 0x1f;
        private const int FUNC_SUBSTRING_BINARY = 0x20;
        private const int FUNC_TRIM_BINARY = 0x21;
        private const int FUNC_OVERLAY_BINARY = 40;
        public const int FUNC_CURRENT_DATE = 0x29;
        public const int FUNC_CURRENT_TIME = 0x2a;
        public const int FUNC_CURRENT_TIMESTAMP = 0x2b;
        protected const int FUNC_LOCALTIME = 0x2c;
        public const int FUNC_LOCALTIMESTAMP = 50;
        private const int FUNC_CURRENT_CATALOG = 0x33;
        private const int FUNC_CURRENT_DEFAULT_TRANSFORM_GROUP = 0x34;
        private const int FUNC_CURRENT_PATH = 0x35;
        private const int FUNC_CURRENT_ROLE = 0x36;
        private const int FUNC_CURRENT_SCHEMA = 0x37;
        private const int FUNC_CURRENT_TRANSFORM_GROUP_FOR_TYPE = 0x38;
        private const int FUNC_CURRENT_USER = 0x39;
        private const int FUNC_SESSION_USER = 0x3a;
        private const int FUNC_SYSTEM_USER = 0x3b;
        protected const int FUNC_USER = 60;
        private const int FUNC_VALUE = 0x3d;
        protected const int FUNC_FOLD_INITCAP = 0x3e;
        protected const int FUNC_LPAD = 0x3f;
        protected const int FUNC_RPAD = 0x40;
        protected const int FUNC_LOG = 0x41;
        protected const int FUNC_REGEXP_LIKE = 0x42;
        protected const int FUNC_TRANSLATE = 0x43;
        protected const int FUNC_NUMTOYMINTERVAL = 0x45;
        protected const int FUNC_TO_YMINTERVAL = 70;
        protected const int FUNC_NUMTODSINTERVAL = 0x47;
        protected const int FUNC_TO_DSINTERVAL = 0x48;
        private const int FUNC_MAX_CARDINALITY = 0x49;
        private const int FUNC_TRIM_ARRAY = 0x4a;
        private const int FUNC_NVL2 = 0x4b;
        private const int FUNC_NEWID = 0x4c;
        public static short[] NoParamList = new short[0];
        public static short[] EmptyParamList = new short[] { 0x2b7, 0x2aa };
        public static short[] OptionalNoParamList = new short[] { 0x2e6, 2, 0x2b7, 0x2aa };
        public static short[] SingleParamList = new short[] { 0x2b7, 0x2b9, 0x2aa };
        public static short[] OptionalIntegerParamList = new short[] { 0x2e6, 5, 0x2b7, 0x2e6, 1, 0x2e8, 0x2aa };
        public static short[] DoubleParamList = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2aa };
        public static short[] TripleParamList = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2ac, 0x2b9, 0x2aa };
        public static short[] QuadParamList = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2ac, 0x2b9, 0x2ac, 0x2b9, 0x2aa };
        public static short[] ManyParamList = new short[] { 0x2b7, 740, 0x2aa };
        public static Dictionary<string, int> RegularFuncMap = GetRegularFuncMap();
        public static Dictionary<string, int> ValueFuncMap = GetValueFuncMap();
        public static OrderedIntHashSet NonDeterministicFuncSet = GetNonDeterministicFuncSet();
        private readonly bool _isDeterministic;
        private bool _isValueFunction;
        public string Name;
        public short[] ParseList;
        public short[] ParseListAlt;

        protected FunctionSQL() : base(0x1c)
        {
            base.nodes = Expression.emptyArray;
        }

        public FunctionSQL(int id) : this()
        {
            base.FuncType = id;
            this._isDeterministic = !NonDeterministicFuncSet.Contains(id);
            switch (id)
            {
                case 1:
                case 2:
                    this.Name = "POSITION";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x80, 0x2b9, 0x2e6, 5, 0x130, 0x2e5, 2, 0x163, 0x1c5, 0x2aa };
                    return;

                case 4:
                    this.Name = "REGEXP_REPLACE";
                    this.ParseList = new short[] { 
                        0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9,
                        0x2e6, 2, 0x2ac, 0x2b9, 0x2aa
                    };
                    return;

                case 5:
                    this.Name = "EXTRACT";
                    this.ParseList = new short[] { 
                        0x2b7, 0x2e5, 0x10, 0x141, 0xab, 0x48, 0x7e, 0xa7, 0xf8, 0x26d, 0x2a1, 0x289, 0x26e, 620, 0x26b, 0x281,
                        0x292, 0x119, 0x11a, 0x72, 0x2b9, 0x2aa
                    };
                    return;

                case 6:
                    this.Name = "BIT_LENGTH";
                    this.ParseList = SingleParamList;
                    return;

                case 7:
                    this.Name = "CHAR_LENGTH";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x2e6, 5, 0x130, 0x2e5, 2, 0x163, 0x1c5, 0x2aa };
                    return;

                case 8:
                    this.Name = "OCTET_LENGTH";
                    this.ParseList = SingleParamList;
                    return;

                case 9:
                    this.Name = "CARDINALITY";
                    this.ParseList = SingleParamList;
                    return;

                case 10:
                    this.Name = "ABS";
                    this.ParseList = SingleParamList;
                    return;

                case 11:
                    this.Name = "MOD";
                    this.ParseList = DoubleParamList;
                    return;

                case 12:
                    this.Name = "LN";
                    this.ParseList = SingleParamList;
                    return;

                case 13:
                    this.Name = "EXP";
                    this.ParseList = SingleParamList;
                    return;

                case 14:
                    this.Name = "POW";
                    this.ParseList = DoubleParamList;
                    return;

                case 15:
                    this.Name = "SQRT";
                    this.ParseList = SingleParamList;
                    return;

                case 0x10:
                    this.Name = "FLOOR";
                    this.ParseList = SingleParamList;
                    return;

                case 0x11:
                    this.Name = "CEILING";
                    this.ParseList = SingleParamList;
                    return;

                case 0x15:
                case 0x20:
                    this.Name = "SUBSTRING";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x72, 0x2b9, 0x2e6, 2, 0x6f, 0x2b9, 0x2e6, 5, 0x130, 0x2e5, 2, 0x163, 0x1c5, 0x2aa };
                    this.ParseListAlt = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2ac, 0x2b9, 0x2aa };
                    return;

                case 0x16:
                    this.Name = "REGEXP_INSTR";
                    this.ParseList = new short[] { 
                        0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9,
                        0x2e6, 2, 0x2ac, 0x2b9, 0x2aa
                    };
                    return;

                case 0x17:
                    this.Name = "REGEXP_SUBSTR";
                    this.ParseList = new short[] { 
                        0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9,
                        0x2aa
                    };
                    return;

                case 0x18:
                    this.Name = "LOWER";
                    this.ParseList = SingleParamList;
                    return;

                case 0x19:
                    this.Name = "UPPER";
                    this.ParseList = SingleParamList;
                    return;

                case 0x1d:
                case 0x21:
                    this.Name = "TRIM";
                    this.ParseList = new short[] { 0x2b7, 0x2e6, 11, 0x2e6, 5, 0x2e5, 3, 0x95, 0x11c, 0x16, 0x2e6, 1, 0x2b9, 0x72, 0x2b9, 0x2aa };
                    return;

                case 30:
                case 40:
                    this.Name = "OVERLAY";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x1d8, 0x2b9, 0x72, 0x2b9, 0x2e6, 2, 0x6f, 0x2b9, 0x2e6, 2, 0x130, 0x163, 0x2aa };
                    return;

                case 0x29:
                    this.Name = "CURRENT_DATE";
                    this.ParseList = NoParamList;
                    return;

                case 0x2a:
                    this.Name = "CURRENT_TIME";
                    this.ParseList = OptionalIntegerParamList;
                    return;

                case 0x2b:
                    this.Name = "CURRENT_TIMESTAMP";
                    this.ParseList = OptionalIntegerParamList;
                    return;

                case 0x2c:
                    this.Name = "LOCALTIME";
                    this.ParseList = OptionalIntegerParamList;
                    return;

                case 50:
                    this.Name = "LOCALTIMESTAMP";
                    this.ParseList = OptionalIntegerParamList;
                    return;

                case 0x33:
                    this.Name = "CURRENT_CATALOG";
                    this.ParseList = NoParamList;
                    return;

                case 0x36:
                    this.Name = "CURRENT_ROLE";
                    this.ParseList = NoParamList;
                    return;

                case 0x37:
                    this.Name = "CURRENT_SCHEMA";
                    this.ParseList = NoParamList;
                    return;

                case 0x39:
                    this.Name = "CURRENT_USER";
                    this.ParseList = NoParamList;
                    return;

                case 0x3a:
                    this.Name = "SESSION_USER";
                    this.ParseList = NoParamList;
                    return;

                case 0x3b:
                    this.Name = "SYSTEM_USER";
                    this.ParseList = NoParamList;
                    return;

                case 60:
                    this.Name = "USER";
                    this.ParseList = OptionalNoParamList;
                    return;

                case 0x3d:
                    this.Name = "VALUE";
                    this.ParseList = NoParamList;
                    return;

                case 0x3e:
                    this.Name = "INITCAP";
                    this.ParseList = SingleParamList;
                    return;

                case 0x3f:
                    this.Name = "LPAD";
                    this.ParseList = TripleParamList;
                    return;

                case 0x40:
                    this.Name = "RPAD";
                    this.ParseList = TripleParamList;
                    return;

                case 0x41:
                    this.Name = "LOG";
                    this.ParseList = DoubleParamList;
                    return;

                case 0x42:
                    this.Name = "REGEXP_LIKE";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2e6, 2, 0x2ac, 0x2b9, 0x2aa };
                    return;

                case 0x43:
                    this.Name = "TRANSLATE";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2ac, 0x2b9, 0x2aa };
                    return;

                case 0x45:
                    this.Name = "NUMTOYMINTERVAL";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2aa };
                    return;

                case 70:
                    this.Name = "TO_YMINTERVAL";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x2aa };
                    return;

                case 0x47:
                    this.Name = "NUMTODSINTERVAL";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x2ac, 0x2b9, 0x2aa };
                    return;

                case 0x48:
                    this.Name = "TO_DSINTERVAL";
                    this.ParseList = new short[] { 0x2b7, 0x2b9, 0x2aa };
                    return;

                case 0x49:
                    this.Name = "MAX_CARDINALITY";
                    this.ParseList = SingleParamList;
                    return;

                case 0x4a:
                    this.Name = "TRIM_ARRAY";
                    this.ParseList = DoubleParamList;
                    return;

                case 0x4b:
                    this.Name = "NVL2";
                    this.ParseList = TripleParamList;
                    return;

                case 0x4c:
                    this.Name = "NEWID";
                    this.ParseList = OptionalNoParamList;
                    return;
            }
            throw Error.RuntimeError(0xc9, "FunctionSQL");
        }

        public override string Describe(Session session, int blanks)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('\n');
            for (int i = 0; i < blanks; i++)
            {
                builder.Append(' ');
            }
            builder.Append("FUNCTION ").Append("=[\n");
            builder.Append(this.Name).Append("(");
            for (int j = 0; j < base.nodes.Length; j++)
            {
                if (base.nodes[j] != null)
                {
                    builder.Append("[").Append(base.nodes[j].Describe(session, blanks)).Append("]");
                }
            }
            builder.Append(") returns ").Append(base.DataType.GetNameString());
            builder.Append("]\n");
            return builder.ToString();
        }

        public override bool Equals(Expression fsql)
        {
            return (((fsql != null) && (base.FuncType == fsql.FuncType)) && base.Equals(fsql));
        }

        public override bool Equals(object other)
        {
            FunctionSQL nsql = other as FunctionSQL;
            return ((nsql != null) && this.Equals((Expression) nsql));
        }

        public virtual Expression GetFunctionExpression()
        {
            return this;
        }

        public override int GetHashCode()
        {
            return (base.OpType + base.FuncType);
        }

        private static OrderedIntHashSet GetNonDeterministicFuncSet()
        {
            OrderedIntHashSet set = new OrderedIntHashSet();
            foreach (int num in ValueFuncMap.Values)
            {
                set.Add(num);
            }
            return set;
        }

        private static Dictionary<string, int> GetRegularFuncMap()
        {
            return new Dictionary<string, int> { 
                { 
                    "POSITION",
                    1
                },
                { 
                    "REGEXP_REPLACE",
                    4
                },
                { 
                    "EXTRACT",
                    5
                },
                { 
                    "BIT_LENGTH",
                    6
                },
                { 
                    "CHAR_LENGTH",
                    7
                },
                { 
                    "CHARACTER_LENGTH",
                    7
                },
                { 
                    "OCTET_LENGTH",
                    8
                },
                { 
                    "CARDINALITY",
                    9
                },
                { 
                    "MAX_CARDINALITY",
                    0x49
                },
                { 
                    "TRIM_ARRAY",
                    0x4a
                },
                { 
                    "ABS",
                    10
                },
                { 
                    "MOD",
                    11
                },
                { 
                    "LN",
                    12
                },
                { 
                    "EXP",
                    13
                },
                { 
                    "POW",
                    14
                },
                { 
                    "POWER",
                    14
                },
                { 
                    "SQRT",
                    15
                },
                { 
                    "FLOOR",
                    0x10
                },
                { 
                    "CEILING",
                    0x11
                },
                { 
                    "CEIL",
                    0x11
                },
                { 
                    "SUBSTRING",
                    0x15
                },
                { 
                    "REGEXP_LIKE",
                    0x42
                },
                { 
                    "REGEXP_INSTR",
                    0x16
                },
                { 
                    "REGEXP_SUBSTR",
                    0x17
                },
                { 
                    "LOWER",
                    0x18
                },
                { 
                    "UPPER",
                    0x19
                },
                { 
                    "INITCAP",
                    0x3e
                },
                { 
                    "LPAD",
                    0x3f
                },
                { 
                    "RPAD",
                    0x40
                },
                { 
                    "TRANSLATE",
                    0x43
                },
                { 
                    "NUMTOYMINTERVAL",
                    0x45
                },
                { 
                    "TO_YMINTERVAL",
                    70
                },
                { 
                    "NUMTODSINTERVAL",
                    0x47
                },
                { 
                    "TO_DSINTERVAL",
                    0x48
                },
                { 
                    "TRIM",
                    0x1d
                },
                { 
                    "OVERLAY",
                    30
                },
                { 
                    "NVL2",
                    0x4b
                }
            };
        }

        public override string GetSql()
        {
            bool flag;
            StringBuilder builder = new StringBuilder();
            switch (base.FuncType)
            {
                case 1:
                case 2:
                    builder.Append("POSITION").Append('(').Append(base.nodes[0].GetSql()).Append(' ').Append("IN").Append(' ').Append(base.nodes[1].GetSql());
                    if (base.nodes[2] != null)
                    {
                        flag = true;
                        if (flag.Equals(base.nodes[2].ValueData))
                        {
                            builder.Append(' ').Append("USING").Append(' ').Append("OCTETS");
                        }
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 4:
                    builder.Append("REGEXP_REPLACE").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql());
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        builder.Append(',').Append(base.nodes[2].GetSql());
                    }
                    if ((base.nodes.Length > 3) && (base.nodes[3] != null))
                    {
                        builder.Append(',').Append(base.nodes[3].GetSql());
                    }
                    if ((base.nodes.Length > 4) && (base.nodes[4] != null))
                    {
                        builder.Append(',').Append(base.nodes[4].GetSql());
                    }
                    if ((base.nodes.Length > 5) && (base.nodes[5] != null))
                    {
                        builder.Append(',').Append(base.nodes[5].GetSql());
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 5:
                {
                    string fieldNameTokenForType = DTIType.GetFieldNameTokenForType(DTIType.GetFieldNameTypeForToken((int) base.nodes[0].ValueData));
                    builder.Append("EXTRACT").Append('(').Append(fieldNameTokenForType).Append(' ').Append("FROM").Append(' ').Append(base.nodes[1].GetSql()).Append(')');
                    goto Label_0FDF;
                }
                case 6:
                    builder.Append("BIT_LENGTH").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 7:
                    builder.Append("CHAR_LENGTH").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 8:
                    builder.Append("OCTET_LENGTH").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 9:
                    builder.Append("CARDINALITY").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 10:
                    builder.Append("ABS").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 11:
                    builder.Append("MOD").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql()).Append(')');
                    goto Label_0FDF;

                case 12:
                    builder.Append("LN").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 13:
                    builder.Append("EXP").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 14:
                    builder.Append("POW").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql()).Append(')');
                    goto Label_0FDF;

                case 15:
                    builder.Append("SQRT").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x10:
                    builder.Append("FLOOR").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x11:
                    builder.Append("CEILING").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x15:
                case 0x20:
                    builder.Append("SUBSTRING").Append('(').Append(base.nodes[0].GetSql()).Append(' ').Append("FROM").Append(' ').Append(base.nodes[1].GetSql());
                    if (base.nodes[2] != null)
                    {
                        builder.Append(' ').Append("FOR").Append(' ').Append(base.nodes[2].GetSql());
                    }
                    if ((base.nodes.Length > 3) && (base.nodes[3] != null))
                    {
                        flag = true;
                        if (flag.Equals(base.nodes[3].ValueData))
                        {
                            builder.Append(' ').Append("USING").Append(' ').Append("OCTETS");
                        }
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 0x16:
                    builder.Append("REGEXP_INSTR").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql());
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        builder.Append(',').Append(base.nodes[2].GetSql());
                    }
                    if ((base.nodes.Length > 3) && (base.nodes[3] != null))
                    {
                        builder.Append(',').Append(base.nodes[3].GetSql());
                    }
                    if ((base.nodes.Length > 4) && (base.nodes[4] != null))
                    {
                        builder.Append(',').Append(base.nodes[4].GetSql());
                    }
                    if ((base.nodes.Length > 5) && (base.nodes[5] != null))
                    {
                        builder.Append(',').Append(base.nodes[5].GetSql());
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 0x17:
                    builder.Append("REGEXP_SUBSTR").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql());
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        builder.Append(',').Append(base.nodes[2].GetSql());
                    }
                    if ((base.nodes.Length > 3) && (base.nodes[3] != null))
                    {
                        builder.Append(',').Append(base.nodes[3].GetSql());
                    }
                    if ((base.nodes.Length > 4) && (base.nodes[4] != null))
                    {
                        builder.Append(',').Append(base.nodes[4].GetSql());
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 0x18:
                    builder.Append("LOWER").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x19:
                    builder.Append("UPPER").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x1d:
                case 0x21:
                {
                    string str2 = null;
                    switch (((int) base.nodes[0].ValueData))
                    {
                        case 0x16:
                            str2 = "BOTH";
                            break;

                        case 0x95:
                            str2 = "LEADING";
                            break;

                        case 0x11c:
                            str2 = "TRAILING";
                            break;
                    }
                    builder.Append("TRIM").Append('(').Append(str2).Append(' ').Append(base.nodes[1].GetSql()).Append(' ').Append("FROM").Append(' ').Append(base.nodes[2].GetSql()).Append(')');
                    goto Label_0FDF;
                }
                case 30:
                case 40:
                    builder.Append("OVERLAY").Append('(').Append(base.nodes[0].GetSql()).Append(' ').Append("PLACING").Append(' ').Append(base.nodes[1].GetSql()).Append(' ').Append("FROM").Append(' ').Append(base.nodes[2].GetSql());
                    if (base.nodes[3] != null)
                    {
                        builder.Append(' ').Append("FOR").Append(' ').Append(base.nodes[3].GetSql());
                    }
                    if (base.nodes[4] != null)
                    {
                        flag = true;
                        if (flag.Equals(base.nodes[4].ValueData))
                        {
                            builder.Append(' ').Append("USING").Append(' ').Append("OCTETS");
                        }
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 0x29:
                case 0x33:
                case 0x34:
                case 0x35:
                case 0x36:
                case 0x37:
                case 0x38:
                case 0x39:
                case 0x3a:
                case 0x3b:
                case 60:
                case 0x3d:
                case 0x4c:
                    return this.Name;

                case 0x2a:
                case 0x2c:
                {
                    int defaultTimeFractionPrecision = DTIType.DefaultTimeFractionPrecision;
                    if ((base.nodes.Length != 0) && (base.nodes[0] != null))
                    {
                        defaultTimeFractionPrecision = (int) base.nodes[0].ValueData;
                    }
                    if (defaultTimeFractionPrecision == DTIType.DefaultTimeFractionPrecision)
                    {
                        return this.Name;
                    }
                    builder.Append(this.Name).Append("(").Append(defaultTimeFractionPrecision);
                    builder.Append(")");
                    return builder.ToString();
                }
                case 0x2b:
                case 50:
                {
                    int valueData = 6;
                    if ((base.nodes.Length != 0) && (base.nodes[0] != null))
                    {
                        valueData = (int) base.nodes[0].ValueData;
                    }
                    if (valueData == 6)
                    {
                        return this.Name;
                    }
                    builder.Append(this.Name).Append("(").Append(valueData);
                    builder.Append(")");
                    return builder.ToString();
                }
                case 0x3e:
                    builder.Append("INITCAP").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x3f:
                    builder.Append("LPAD").Append('(').Append(base.nodes[0].GetSql()).Append(0x2ac).Append(base.nodes[1].GetSql());
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        builder.Append(base.nodes[2].GetSql());
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 0x40:
                    builder.Append("RPAD").Append('(').Append(base.nodes[0].GetSql()).Append(0x2ac).Append(base.nodes[1].GetSql());
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        builder.Append(base.nodes[2].GetSql());
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 0x41:
                    builder.Append("LOG").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x42:
                    builder.Append("REGEXP_LIKE").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql());
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        builder.Append(',').Append(base.nodes[2].GetSql());
                    }
                    builder.Append(')');
                    goto Label_0FDF;

                case 0x43:
                    builder.Append("TRANSLATE").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[0].GetSql()).Append(',').Append(')');
                    goto Label_0FDF;

                case 0x45:
                    builder.Append("NUMTOYMINTERVAL").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[0].GetSql()).Append(',').Append(')');
                    goto Label_0FDF;

                case 70:
                    builder.Append(0x32b).Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x47:
                    builder.Append("NUMTODSINTERVAL").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[0].GetSql()).Append(',').Append(')');
                    goto Label_0FDF;

                case 0x48:
                    builder.Append(0x32d).Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x49:
                    builder.Append("MAX_CARDINALITY").Append('(').Append(base.nodes[0].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x4a:
                    builder.Append("TRIM_ARRAY").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql()).Append(')');
                    goto Label_0FDF;

                case 0x4b:
                    builder.Append("NVL2").Append('(').Append(base.nodes[0].GetSql()).Append(',').Append(base.nodes[1].GetSql()).Append(',').Append(base.nodes[2].GetSql()).Append(')');
                    goto Label_0FDF;
            }
            throw Error.RuntimeError(0xc9, "FunctionSQL");
        Label_0FDF:
            return builder.ToString();
        }

        public override object GetValue(Session session)
        {
            object[] data = new object[base.nodes.Length];
            for (int i = 0; i < base.nodes.Length; i++)
            {
                Expression expression = base.nodes[i];
                if (expression != null)
                {
                    data[i] = expression.GetValue(session, expression.DataType);
                }
            }
            return this.GetValue(session, data);
        }

        public virtual object GetValue(Session session, object[] data)
        {
            IBlobData data1;
            bool flag;
            bool flag2;
            bool flag3;
            bool flag4;
            switch (base.FuncType)
            {
                case 1:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        long num5 = ((CharacterType) base.nodes[1].DataType).Position(session, data[1], data[0], base.nodes[0].DataType, 0L) + 1L;
                        if ((base.nodes[2] != null) && (((int) base.nodes[2].ValueData) == 0x1c5))
                        {
                            num5 *= 2L;
                        }
                        return num5;
                    }
                    return null;

                case 2:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        long num6 = BinaryType.Position(session, (IBlobData) data[1], (IBlobData) data[0], base.nodes[0].DataType, 0L) + 1L;
                        if ((base.nodes[2] != null) && (((int) base.nodes[2].ValueData) == 0x1c5))
                        {
                            num6 *= 2L;
                        }
                        return num6;
                    }
                    return null;

                case 4:
                    return this.ProcessRegExpReplace(session, data);

                case 5:
                    if (data[1] != null)
                    {
                        int valueData = (int) base.nodes[0].ValueData;
                        valueData = DTIType.GetFieldNameTypeForToken(valueData);
                        int num8 = valueData;
                        if (num8 == 0x6a)
                        {
                            return ((DTIType) base.nodes[1].DataType).GetSecondPart(data[1]);
                        }
                        if ((num8 - 0x108) <= 1)
                        {
                            return ((DateTimeType) base.nodes[1].DataType).GetPartString(session, data[1], valueData);
                        }
                        return ((DTIType) base.nodes[1].DataType).GetPart(session, data[1], valueData);
                    }
                    return null;

                case 6:
                    if (data[0] != null)
                    {
                        if (base.nodes[0].DataType.IsBinaryType())
                        {
                            return ((IBlobData) data[0]).BitLength(session);
                        }
                        return (0x10L * ((CharacterType) base.nodes[0].DataType).Size(session, data[0]));
                    }
                    return null;

                case 7:
                    if (data[0] != null)
                    {
                        return ((CharacterType) base.nodes[0].DataType).Size(session, data[0]);
                    }
                    return null;

                case 8:
                    if (data[0] != null)
                    {
                        if (base.nodes[0].DataType.IsBinaryType())
                        {
                            return ((IBlobData) data[0]).Length(session);
                        }
                        return (2L * ((CharacterType) base.nodes[0].DataType).Size(session, data[0]));
                    }
                    return null;

                case 9:
                    if (data[0] != null)
                    {
                        return base.nodes[0].DataType.Cardinality(session, data[0]);
                    }
                    return null;

                case 10:
                    if (data[0] != null)
                    {
                        return base.DataType.Absolute(data[0]);
                    }
                    return null;

                case 11:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        object a = base.nodes[0].DataType.Mod(data[0], data[1], base.nodes[0].DataType, base.nodes[1].DataType);
                        return base.DataType.ConvertToTypeLimits(session, a);
                    }
                    return null;

                case 12:
                    if (data[0] != null)
                    {
                        double d = Convert.ToDouble(data[0]);
                        if (d <= 0.0)
                        {
                            throw Error.GetError(0xd74);
                        }
                        return Math.Log(d);
                    }
                    return null;

                case 13:
                    if (data[0] != null)
                    {
                        return Math.Exp(Convert.ToDouble(data[0]));
                    }
                    return null;

                case 14:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        double x = Convert.ToDouble(data[0]);
                        double y = Convert.ToDouble(data[1]);
                        if (x == 0.0)
                        {
                            if (y < 0.0)
                            {
                                throw Error.GetError(0xd75);
                            }
                            return ((y == 0.0) ? 1.0 : 0.0);
                        }
                        return Math.Pow(x, y);
                    }
                    return null;

                case 15:
                    if (data[0] != null)
                    {
                        return Math.Sqrt(Convert.ToDouble(data[0]));
                    }
                    return null;

                case 0x10:
                    if (data[0] != null)
                    {
                        if (base.nodes[0].DataType.IsCharacterType())
                        {
                            data[0] = base.DataType.ConvertToDefaultType(session, data[0]);
                        }
                        return ((NumberType) base.DataType).Floor(data[0]);
                    }
                    return null;

                case 0x11:
                    if (data[0] != null)
                    {
                        if (base.nodes[0].DataType.IsCharacterType())
                        {
                            data[0] = base.DataType.ConvertToDefaultType(session, data[0]);
                        }
                        return ((NumberType) base.DataType).Ceiling(data[0]);
                    }
                    return null;

                case 0x15:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        long offset = ((long) SqlType.SqlBigint.ConvertToType(session, data[1], base.nodes[1].DataType)) - 1L;
                        offset = (offset == -1L) ? 0L : offset;
                        long length = 0L;
                        if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                        {
                            if (data[2] == null)
                            {
                                return null;
                            }
                            length = (long) SqlType.SqlBigint.ConvertToType(session, data[2], base.nodes[2].DataType);
                        }
                        if ((base.nodes.Length > 3) && (base.nodes[3] != null))
                        {
                            int valueData = (int) base.nodes[2].ValueData;
                        }
                        if (!base.nodes[0].DataType.IsCharacterType())
                        {
                            data[0] = SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        }
                        return ((CharacterType) base.DataType).Substring(session, data[0], offset, length, (base.nodes.Length > 2) && (base.nodes[2] > null), false);
                    }
                    return null;

                case 0x16:
                    return this.ProcessRegExpInStr(session, data);

                case 0x17:
                    return this.ProcessRegExpSubStr(session, data);

                case 0x18:
                    if (data[0] != null)
                    {
                        if (!base.nodes[0].DataType.IsCharacterType())
                        {
                            data[0] = SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        }
                        return ((CharacterType) base.DataType).Lower(session, data[0]);
                    }
                    return null;

                case 0x19:
                    if (data[0] != null)
                    {
                        if (!base.nodes[0].DataType.IsCharacterType())
                        {
                            data[0] = SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        }
                        return ((CharacterType) base.DataType).Upper(session, data[0]);
                    }
                    return null;

                case 0x1d:
                    if ((data[1] != null) && (data[2] != null))
                    {
                        flag = false;
                        flag2 = false;
                        int valueData = (int) base.nodes[0].ValueData;
                        switch (valueData)
                        {
                            case 0x16:
                                flag = flag2 = true;
                                goto Label_079A;

                            case 0x95:
                                flag = true;
                                goto Label_079A;
                        }
                        if (valueData != 0x11c)
                        {
                            throw Error.RuntimeError(0xc9, "FunctionSQL");
                        }
                        flag2 = true;
                        break;
                    }
                    return null;

                case 30:
                    if (((data[0] != null) && (data[1] != null)) && (data[2] != null))
                    {
                        long offset = ((long) SqlType.SqlBigint.ConvertToType(session, data[2], base.nodes[2].DataType)) - 1L;
                        long length = 0L;
                        if (base.nodes[3] != null)
                        {
                            if (data[3] == null)
                            {
                                return null;
                            }
                            length = (long) SqlType.SqlBigint.ConvertToType(session, data[3], base.nodes[3].DataType);
                        }
                        return ((CharacterType) base.DataType).Overlay(null, data[0], data[1], offset, length, base.nodes[3] > null);
                    }
                    return null;

                case 0x20:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        long offset = ((long) SqlType.SqlBigint.ConvertToType(session, data[1], base.nodes[1].DataType)) - 1L;
                        long length = 0L;
                        if (base.nodes[2] != null)
                        {
                            if (data[2] == null)
                            {
                                return null;
                            }
                            length = (long) SqlType.SqlBigint.ConvertToType(session, data[2], base.nodes[2].DataType);
                        }
                        return ((BinaryType) base.DataType).Substring(session, (IBlobData) data[0], offset, length, base.nodes[2] > null);
                    }
                    return null;

                case 0x21:
                    if ((data[1] != null) && (data[2] != null))
                    {
                        flag3 = false;
                        flag4 = false;
                        int valueData = (int) base.nodes[0].ValueData;
                        switch (valueData)
                        {
                            case 0x16:
                                flag3 = flag4 = true;
                                goto Label_09B6;

                            case 0x95:
                                flag3 = true;
                                goto Label_09B6;
                        }
                        if (valueData != 0x11c)
                        {
                            throw Error.RuntimeError(0xc9, "FunctionSQL");
                        }
                        flag4 = true;
                        goto Label_09B6;
                    }
                    return null;

                case 40:
                    if (((data[0] != null) && (data[1] != null)) && (data[2] != null))
                    {
                        long offset = ((long) SqlType.SqlBigint.ConvertToType(session, data[2], base.nodes[2].DataType)) - 1L;
                        long length = 0L;
                        if (base.nodes[3] != null)
                        {
                            if (data[3] == null)
                            {
                                return null;
                            }
                            length = (long) SqlType.SqlBigint.ConvertToType(session, data[3], base.nodes[3].DataType);
                        }
                        return ((BinaryType) base.DataType).Overlay(session, (IBlobData) data[0], (IBlobData) data[1], offset, length, base.nodes[3] > null);
                    }
                    return null;

                case 0x29:
                    return session.GetCurrentDate();

                case 0x2a:
                    return base.DataType.ConvertToTypeLimits(session, session.GetCurrentTime(true));

                case 0x2b:
                    return base.DataType.ConvertToTypeLimits(session, session.GetCurrentTimestamp(true));

                case 0x2c:
                    return base.DataType.ConvertToTypeLimits(session, session.GetCurrentTime(false));

                case 50:
                    return base.DataType.ConvertToTypeLimits(session, session.GetCurrentTimestamp(false));

                case 0x33:
                    return session.database.GetCatalogName().Name;

                case 0x36:
                    if (session.GetRole() == null)
                    {
                        return null;
                    }
                    return session.GetRole().GetNameString();

                case 0x37:
                    return session.GetCurrentSchemaQName().Name;

                case 0x39:
                    return session.GetUser().GetNameString();

                case 0x3a:
                    return session.GetUser().GetNameString();

                case 0x3b:
                    return session.GetUser().GetNameString();

                case 60:
                    return session.GetUser().GetNameString();

                case 0x3d:
                    return session.sessionData.CurrentValue;

                case 0x3e:
                    if (data[0] != null)
                    {
                        if (!base.nodes[0].DataType.IsCharacterType())
                        {
                            data[0] = SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        }
                        return ((CharacterType) base.DataType).Initcap(session, data[0]);
                    }
                    return null;

                case 0x3f:
                    return this.ProcessLpad(session, data);

                case 0x40:
                    return this.ProcessRpad(session, data);

                case 0x41:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        double newBase = Convert.ToDouble(data[0]);
                        double a = Convert.ToDouble(data[1]);
                        if ((a <= 0.0) || (newBase <= 0.0))
                        {
                            throw Error.GetError(0xd74);
                        }
                        return Math.Log(a, newBase);
                    }
                    return null;

                case 0x42:
                    return this.ProcessRegExpLike(session, data);

                case 0x43:
                    return this.ProcessTranslate(session, data);

                case 0x45:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        decimal d = (decimal) SqlType.SqlDecimalDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        string str2 = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
                        if (str2.Equals("YEAR", StringComparison.InvariantCultureIgnoreCase))
                        {
                            d *= 12M;
                        }
                        else if (!str2.Equals("MONTH", StringComparison.InvariantCultureIgnoreCase))
                        {
                            throw Error.GetError(0xd4e);
                        }
                        return new IntervalMonthData((long) Math.Round(d));
                    }
                    return null;

                case 70:
                    if (data[0] != null)
                    {
                        string s = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        using (Scanner scanner = new Scanner())
                        {
                            return scanner.NewInterval(s, SqlType.SqlIntervalYearToMonth);
                        }
                    }
                    return null;

                case 0x47:
                    if ((data[0] != null) && (data[1] != null))
                    {
                        decimal d = (decimal) SqlType.SqlDecimalDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                        string str = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
                        if (str.Equals("DAY", StringComparison.InvariantCultureIgnoreCase))
                        {
                            d = ((d * 24M) * 60M) * 60M;
                        }
                        else if (str.Equals("HOUR", StringComparison.InvariantCultureIgnoreCase))
                        {
                            d = (d * 60M) * 60M;
                        }
                        else if (str.Equals("MINUTE", StringComparison.InvariantCultureIgnoreCase))
                        {
                            d *= 60M;
                        }
                        else if (!str.Equals("SECOND", StringComparison.InvariantCultureIgnoreCase))
                        {
                            throw Error.GetError(0xd4e);
                        }
                        long seconds = (long) Math.Truncate(d);
                        return new IntervalSecondData(seconds, (int) ((d - seconds) * 1000000000M), SqlType.SqlIntervalDayToSecond);
                    }
                    return null;

                case 0x48:
                {
                    if (data[0] == null)
                    {
                        return null;
                    }
                    string s = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
                    using (Scanner scanner2 = new Scanner())
                    {
                        return scanner2.NewInterval(s, SqlType.SqlIntervalDayToSecond);
                    }
                }
                case 0x49:
                    if (data[0] == null)
                    {
                        return null;
                    }
                    return base.nodes[0].DataType.ArrayLimitCardinality();

                case 0x4a:
                    if (data[0] != null)
                    {
                        if (data[1] == null)
                        {
                            return null;
                        }
                        object[] sourceArray = (object[]) data[0];
                        int num26 = Convert.ToInt32(data[1]);
                        if ((num26 < 0) || (num26 > sourceArray.Length))
                        {
                            throw Error.GetError(0xda2);
                        }
                        object[] destinationArray = new object[sourceArray.Length - num26];
                        Array.Copy(sourceArray, 0, destinationArray, 0, destinationArray.Length);
                        return destinationArray;
                    }
                    return null;

                case 0x4b:
                    if (data[0] == null)
                    {
                        return data[2];
                    }
                    return data[1];

                case 0x4c:
                    return Guid.NewGuid();

                default:
                    throw Error.RuntimeError(0xc9, "FunctionSQL");
            }
        Label_079A:
            if (!base.nodes[2].DataType.IsCharacterType())
            {
                data[2] = SqlType.SqlVarcharDefault.ConvertToType(session, data[2], base.nodes[2].DataType);
            }
            if (!base.nodes[1].DataType.IsCharacterType() || base.nodes[1].DataType.IsLobType())
            {
                data[1] = SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
            }
            return ((CharacterType) base.DataType).Trim(session, data[2], (string) data[1], flag, flag2);
        Label_09B6:
            data1 = (IBlobData) data[1];
            if (data1.Length(session) != 1L)
            {
                throw Error.GetError(0xd84);
            }
            byte[] bytes = data1.GetBytes();
            return ((BinaryType) base.DataType).Trim(session, (IBlobData) data[2], bytes[0], flag3, flag4);
        }

        private static Dictionary<string, int> GetValueFuncMap()
        {
            return new Dictionary<string, int> { 
                { 
                    "LOCALTIME",
                    0x2c
                },
                { 
                    "LOCALTIMESTAMP",
                    50
                },
                { 
                    "CURRENT_CATALOG",
                    0x33
                },
                { 
                    "CURRENT_PATH",
                    0x35
                },
                { 
                    "CURRENT_ROLE",
                    0x36
                },
                { 
                    "CURRENT_SCHEMA",
                    0x37
                },
                { 
                    "CURRENT_USER",
                    0x39
                },
                { 
                    "SESSION_USER",
                    0x3a
                },
                { 
                    "SYSTEM_USER",
                    0x3b
                },
                { 
                    "VALUE",
                    0x3d
                },
                { 
                    "NEWID",
                    0x4c
                },
                { 
                    "CURRENT_DATE",
                    0x29
                },
                { 
                    "CURRENT_TIME",
                    0x2a
                },
                { 
                    "CURRENT_TIMESTAMP",
                    0x2b
                },
                { 
                    "USER",
                    60
                }
            };
        }

        public bool IsDeterministic()
        {
            return this._isDeterministic;
        }

        public static bool IsFunction(string token)
        {
            if (!IsRegularFunction(token))
            {
                return IsValueFunction(token);
            }
            return true;
        }

        public static bool IsRegularFunction(string token)
        {
            return RegularFuncMap.ContainsKey(token);
        }

        public bool IsValueFunction()
        {
            return this._isValueFunction;
        }

        public static bool IsValueFunction(string token)
        {
            return ValueFuncMap.ContainsKey(token);
        }

        public static FunctionSQL NewSqlFunction(string token, ParserDQL.CompileContext context)
        {
            int num;
            bool flag = false;
            if (!RegularFuncMap.TryGetValue(token, out num))
            {
                if (!ValueFuncMap.TryGetValue(token, out num))
                {
                    num = -1;
                }
                flag = true;
            }
            if (num == -1)
            {
                return null;
            }
            FunctionSQL nsql2 = new FunctionSQL(num);
            if (num == 0x3d)
            {
                if (context.CurrentDomain == null)
                {
                    return null;
                }
                nsql2.DataType = context.CurrentDomain;
                return nsql2;
            }
            nsql2._isValueFunction = flag;
            return nsql2;
        }

        private object ProcessLpad(Session session, object[] data)
        {
            if (data[0] == null)
            {
                return null;
            }
            if (data[1] == null)
            {
                return data[0];
            }
            if (!base.nodes[0].DataType.IsCharacterType())
            {
                data[0] = SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
            }
            int width = (int) SqlType.SqlInteger.ConvertToType(session, data[1], base.nodes[1].DataType);
            string paddingChar = " ";
            if ((base.nodes.Length > 2) && (base.nodes[2] != null))
            {
                if (data[2] == null)
                {
                    return null;
                }
                paddingChar = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[2], base.nodes[2].DataType);
            }
            return ((CharacterType) base.DataType).LPad(session, data[0], width, paddingChar);
        }

        private object ProcessRegExpInStr(Session session, object[] data)
        {
            if ((data[0] == null) || (data[1] == null))
            {
                return null;
            }
            string s = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
            string pattern = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
            int pos = 1;
            int occurr = 1;
            int retOpt = 0;
            string flags = "";
            if ((base.nodes.Length > 2) && (base.nodes[2] != null))
            {
                if (data[2] == null)
                {
                    return null;
                }
                pos = (int) SqlType.SqlInteger.ConvertToType(session, data[2], base.nodes[2].DataType);
            }
            if ((base.nodes.Length > 3) && (base.nodes[3] != null))
            {
                if (data[3] == null)
                {
                    return null;
                }
                occurr = (int) SqlType.SqlInteger.ConvertToType(session, data[3], base.nodes[3].DataType);
            }
            if ((base.nodes.Length > 4) && (base.nodes[4] != null))
            {
                if (data[4] == null)
                {
                    return null;
                }
                retOpt = (int) SqlType.SqlInteger.ConvertToType(session, data[4], base.nodes[4].DataType);
            }
            if ((base.nodes.Length > 5) && (base.nodes[5] != null))
            {
                if (data[5] == null)
                {
                    return null;
                }
                flags = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[5], base.nodes[5].DataType);
            }
            object o = Library.RegExp_Instr(s, pattern, pos, occurr, retOpt, flags);
            return base.DataType.ConvertToDefaultType(session, o);
        }

        private object ProcessRegExpLike(Session session, object[] data)
        {
            if ((data[0] == null) || (data[1] == null))
            {
                return null;
            }
            string s = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
            string pattern = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
            string flags = "";
            if ((base.nodes.Length > 2) && (base.nodes[2] != null))
            {
                if (data[2] == null)
                {
                    return null;
                }
                flags = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[2], base.nodes[4].DataType);
            }
            return (Library.RegExp_Instr(s, pattern, 1, 1, 0, flags) > 0);
        }

        private object ProcessRegExpReplace(Session session, object[] data)
        {
            if ((data[0] == null) || (data[1] == null))
            {
                return null;
            }
            string s = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
            string pattern = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
            int pos = 1;
            int occurr = 1;
            string flags = "";
            string replace = "";
            if ((base.nodes.Length > 2) && (base.nodes[2] != null))
            {
                if (data[2] == null)
                {
                    return null;
                }
                replace = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[2], base.nodes[2].DataType);
            }
            if ((base.nodes.Length > 3) && (base.nodes[3] != null))
            {
                if (data[3] == null)
                {
                    return null;
                }
                pos = (int) SqlType.SqlInteger.ConvertToType(session, data[3], base.nodes[3].DataType);
            }
            if ((base.nodes.Length > 4) && (base.nodes[4] != null))
            {
                if (data[4] == null)
                {
                    return null;
                }
                occurr = (int) SqlType.SqlInteger.ConvertToType(session, data[4], base.nodes[4].DataType);
            }
            if ((base.nodes.Length > 5) && (base.nodes[5] != null))
            {
                if (data[5] == null)
                {
                    return null;
                }
                flags = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[5], base.nodes[5].DataType);
            }
            object o = Library.RegExp_Replace(s, pattern, replace, pos, occurr, flags);
            return base.DataType.ConvertToDefaultType(session, o);
        }

        private object ProcessRegExpSubStr(Session session, object[] data)
        {
            if ((data[0] == null) || (data[1] == null))
            {
                return null;
            }
            string s = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
            string pattern = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
            int pos = 1;
            int occurr = 1;
            string flags = "";
            if ((base.nodes.Length > 2) && (base.nodes[2] != null))
            {
                if (data[2] == null)
                {
                    return null;
                }
                pos = (int) SqlType.SqlInteger.ConvertToType(session, data[2], base.nodes[2].DataType);
            }
            if ((base.nodes.Length > 3) && (base.nodes[3] != null))
            {
                if (data[3] == null)
                {
                    return null;
                }
                occurr = (int) SqlType.SqlInteger.ConvertToType(session, data[3], base.nodes[3].DataType);
            }
            if ((base.nodes.Length > 4) && (base.nodes[4] != null))
            {
                if (data[4] == null)
                {
                    return null;
                }
                flags = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[4], base.nodes[4].DataType);
            }
            object o = Library.RegExp_Substr(s, pattern, pos, occurr, flags);
            return base.DataType.ConvertToDefaultType(session, o);
        }

        private object ProcessRpad(Session session, object[] data)
        {
            if (data[0] == null)
            {
                return null;
            }
            if (data[1] == null)
            {
                return data[0];
            }
            if (!base.nodes[0].DataType.IsCharacterType())
            {
                data[0] = SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType);
            }
            int width = (int) SqlType.SqlInteger.ConvertToType(session, data[1], base.nodes[1].DataType);
            string paddingChar = " ";
            if ((base.nodes.Length > 2) && (base.nodes[2] != null))
            {
                if (data[2] == null)
                {
                    return null;
                }
                paddingChar = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[2], base.nodes[2].DataType);
            }
            return ((CharacterType) base.DataType).RPad(session, data[0], width, paddingChar);
        }

        private object ProcessTranslate(Session session, object[] data)
        {
            if (((data[0] == null) || (data[1] == null)) || (data[2] == null))
            {
                return null;
            }
            string strIf = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[1], base.nodes[1].DataType);
            string strElse = (string) SqlType.SqlVarcharDefault.ConvertToType(session, data[2], base.nodes[2].DataType);
            return Library.Translate((string) SqlType.SqlVarcharDefault.ConvertToType(session, data[0], base.nodes[0].DataType), strIf, strElse);
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i].ResolveTypes(session, this);
                }
            }
            switch (base.FuncType)
            {
                case 1:
                case 2:
                    if (base.nodes[0].DataType == null)
                    {
                        if (base.nodes[1].DataType == null)
                        {
                            throw Error.GetError(0x15bf);
                        }
                        if ((base.nodes[1].DataType.TypeCode == 40) || base.nodes[1].DataType.IsBinaryType())
                        {
                            base.nodes[0].DataType = base.nodes[1].DataType;
                        }
                        else
                        {
                            base.nodes[0].DataType = SqlType.SqlVarchar;
                        }
                        break;
                    }
                    break;

                case 4:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    if (!base.nodes[1].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        if (base.nodes[2].DataType == null)
                        {
                            base.nodes[2].DataType = SqlType.SqlVarcharDefault;
                        }
                        if (!base.nodes[2].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                    }
                    if ((base.nodes.Length > 3) && (base.nodes[3] != null))
                    {
                        if (base.nodes[3].DataType == null)
                        {
                            base.nodes[3].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[3].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[3].DataType = ((NumberType) base.nodes[3].DataType).GetIntegralType();
                    }
                    if ((base.nodes.Length > 4) && (base.nodes[4] != null))
                    {
                        if (base.nodes[4].DataType == null)
                        {
                            base.nodes[4].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[4].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[4].DataType = ((NumberType) base.nodes[4].DataType).GetIntegralType();
                    }
                    if ((base.nodes.Length > 5) && (base.nodes[5] != null))
                    {
                        if (base.nodes[5].DataType == null)
                        {
                            base.nodes[5].DataType = SqlType.SqlChar;
                        }
                        if (!base.nodes[5].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                    }
                    base.DataType = SqlType.SqlVarcharDefault;
                    return;

                case 5:
                {
                    if (base.nodes[1].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (!base.nodes[1].DataType.IsDateTimeType() && !base.nodes[1].DataType.IsIntervalType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    int valueData = (int) base.nodes[0].ValueData;
                    DTIType dataType = (DTIType) base.nodes[1].DataType;
                    valueData = DTIType.GetFieldNameTypeForToken(valueData);
                    base.DataType = dataType.GetExtractType(valueData);
                    return;
                }
                case 6:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarcharDefault;
                    }
                    if (!base.nodes[0].DataType.IsCharacterType() && !base.nodes[0].DataType.IsBinaryType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    base.DataType = SqlType.SqlBigint;
                    return;

                case 7:
                    if (!base.nodes[0].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    goto Label_17C9;

                case 8:
                    goto Label_17C9;

                case 9:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (!base.nodes[0].DataType.IsArrayType())
                    {
                        throw Error.GetError(0x15bb);
                    }
                    base.DataType = SqlType.SqlInteger;
                    return;

                case 10:
                    if ((base.nodes[0].DataType == null) || !base.nodes[0].DataType.IsIntervalType())
                    {
                        goto Label_1829;
                    }
                    base.DataType = base.nodes[0].DataType;
                    return;

                case 11:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[1].DataType = base.nodes[0].DataType;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[0].DataType = base.nodes[1].DataType;
                    }
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (!base.nodes[0].DataType.IsNumberType() || !base.nodes[1].DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    base.DataType = base.nodes[0].DataType.GetCombinedType(base.nodes[1].DataType, 0x65);
                    return;

                case 12:
                case 13:
                case 15:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlDouble;
                    }
                    if (!base.nodes[0].DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    base.nodes[0].DataType = SqlType.SqlDouble;
                    base.DataType = SqlType.SqlDouble;
                    return;

                case 14:
                case 0x41:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlDouble;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = base.nodes[0].DataType;
                    }
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (!base.nodes[0].DataType.IsNumberType() || !base.nodes[1].DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    base.nodes[0].DataType = SqlType.SqlDouble;
                    base.nodes[1].DataType = SqlType.SqlDouble;
                    base.DataType = SqlType.SqlDouble;
                    return;

                case 0x10:
                case 0x11:
                    goto Label_1829;

                case 0x15:
                case 0x20:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlNumeric;
                    }
                    if (!base.nodes[1].DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        if (base.nodes[2].DataType == null)
                        {
                            base.nodes[2].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[2].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[2].DataType = ((NumberType) base.nodes[2].DataType).GetIntegralType();
                    }
                    base.DataType = base.nodes[0].DataType;
                    if (!base.DataType.IsCharacterType() && !base.DataType.IsBinaryType())
                    {
                        base.DataType = SqlType.SqlVarcharDefault;
                    }
                    if ((base.DataType.IsCharacterType() || base.DataType.IsNumberType()) || base.DataType.IsDateTimeType())
                    {
                        base.FuncType = 0x15;
                    }
                    else
                    {
                        if (!base.DataType.IsBinaryType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.FuncType = 0x20;
                    }
                    if (base.nodes.Length > 3)
                    {
                        Expression expression1 = base.nodes[3];
                        return;
                    }
                    return;

                case 0x16:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    if (!base.nodes[1].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        if (base.nodes[2].DataType == null)
                        {
                            base.nodes[2].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[2].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[2].DataType = ((NumberType) base.nodes[2].DataType).GetIntegralType();
                    }
                    if ((base.nodes.Length > 3) && (base.nodes[3] != null))
                    {
                        if (base.nodes[3].DataType == null)
                        {
                            base.nodes[3].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[3].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[3].DataType = ((NumberType) base.nodes[3].DataType).GetIntegralType();
                    }
                    if ((base.nodes.Length > 4) && (base.nodes[4] != null))
                    {
                        if (base.nodes[4].DataType == null)
                        {
                            base.nodes[4].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[4].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[4].DataType = ((NumberType) base.nodes[4].DataType).GetIntegralType();
                    }
                    if ((base.nodes.Length > 5) && (base.nodes[5] != null))
                    {
                        if (base.nodes[5].DataType == null)
                        {
                            base.nodes[5].DataType = SqlType.SqlChar;
                        }
                        if (!base.nodes[5].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                    }
                    base.DataType = SqlType.SqlNumeric;
                    return;

                case 0x17:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    if (!base.nodes[1].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        if (base.nodes[2].DataType == null)
                        {
                            base.nodes[2].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[2].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[2].DataType = ((NumberType) base.nodes[2].DataType).GetIntegralType();
                    }
                    if ((base.nodes.Length > 3) && (base.nodes[3] != null))
                    {
                        if (base.nodes[3].DataType == null)
                        {
                            base.nodes[3].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[3].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[3].DataType = ((NumberType) base.nodes[3].DataType).GetIntegralType();
                    }
                    if ((base.nodes.Length > 4) && (base.nodes[4] != null))
                    {
                        if (base.nodes[4].DataType == null)
                        {
                            base.nodes[4].DataType = SqlType.SqlChar;
                        }
                        if (!base.nodes[4].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                    }
                    base.DataType = SqlType.SqlVarcharDefault;
                    return;

                case 0x18:
                case 0x19:
                case 0x3e:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    base.DataType = base.nodes[0].DataType;
                    if ((!base.DataType.IsCharacterType() && !base.DataType.IsNumberType()) && !base.DataType.IsDateTimeType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if (!base.DataType.IsCharacterType())
                    {
                        base.DataType = SqlType.SqlVarcharDefault;
                        return;
                    }
                    return;

                case 0x1d:
                case 0x21:
                    if (base.nodes[0] == null)
                    {
                        base.nodes[0] = new ExpressionValue(0x16, SqlType.SqlInteger);
                    }
                    if (base.nodes[2].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    base.DataType = base.nodes[2].DataType;
                    if ((!base.DataType.IsCharacterType() && !base.DataType.IsNumberType()) && !base.DataType.IsDateTimeType())
                    {
                        if (!base.DataType.IsBinaryType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.FuncType = 0x21;
                        if (base.nodes[1] == null)
                        {
                            int index = 1;
                            byte[] data = new byte[1];
                            base.nodes[index] = new ExpressionValue(new BinaryData(data, false), SqlType.SqlBinary);
                            return;
                        }
                        return;
                    }
                    base.FuncType = 0x1d;
                    if (!base.DataType.IsCharacterType())
                    {
                        base.DataType = SqlType.SqlVarcharDefault;
                    }
                    if (base.nodes[1] == null)
                    {
                        base.nodes[1] = new ExpressionValue(" ", SqlType.SqlChar);
                        return;
                    }
                    return;

                case 30:
                case 40:
                    if (base.nodes[0].DataType == null)
                    {
                        if (base.nodes[1].DataType == null)
                        {
                            throw Error.GetError(0x15bf);
                        }
                        if ((base.nodes[1].DataType.TypeCode == 40) || base.nodes[1].DataType.IsBinaryType())
                        {
                            base.nodes[0].DataType = base.nodes[1].DataType;
                        }
                        else
                        {
                            base.nodes[0].DataType = SqlType.SqlVarchar;
                        }
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        if ((base.nodes[0].DataType.TypeCode == 40) || base.nodes[0].DataType.IsBinaryType())
                        {
                            base.nodes[1].DataType = base.nodes[0].DataType;
                        }
                        else
                        {
                            base.nodes[1].DataType = SqlType.SqlVarchar;
                        }
                    }
                    if (base.nodes[0].DataType.IsCharacterType() && base.nodes[1].DataType.IsCharacterType())
                    {
                        base.FuncType = 30;
                        if ((base.nodes[0].DataType.TypeCode == 40) || (base.nodes[1].DataType.TypeCode == 40))
                        {
                            base.DataType = CharacterType.GetCharacterType(40, base.nodes[0].DataType.Precision + base.nodes[1].DataType.Precision);
                        }
                        else
                        {
                            base.DataType = CharacterType.GetCharacterType(12, base.nodes[0].DataType.Precision + base.nodes[1].DataType.Precision);
                        }
                    }
                    else
                    {
                        if (!base.nodes[0].DataType.IsBinaryType() || !base.nodes[1].DataType.IsBinaryType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.FuncType = 40;
                        if ((base.nodes[0].DataType.TypeCode == 30) || (base.nodes[1].DataType.TypeCode == 30))
                        {
                            base.DataType = BinaryType.GetBinaryType(30, base.nodes[0].DataType.Precision + base.nodes[1].DataType.Precision);
                        }
                        else
                        {
                            base.DataType = BinaryType.GetBinaryType(0x3d, base.nodes[0].DataType.Precision + base.nodes[1].DataType.Precision);
                        }
                    }
                    if (base.nodes[2].DataType == null)
                    {
                        base.nodes[2].DataType = SqlType.SqlNumeric;
                    }
                    if (!base.nodes[2].DataType.IsNumberType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    base.nodes[2].DataType = ((NumberType) base.nodes[2].DataType).GetIntegralType();
                    if (base.nodes[3] != null)
                    {
                        if (base.nodes[3].DataType == null)
                        {
                            base.nodes[3].DataType = SqlType.SqlNumeric;
                        }
                        if (!base.nodes[3].DataType.IsNumberType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                        base.nodes[3].DataType = ((NumberType) base.nodes[3].DataType).GetIntegralType();
                    }
                    return;

                case 0x29:
                    base.DataType = SqlType.SqlDate;
                    return;

                case 0x2a:
                {
                    int defaultTimeFractionPrecision = DTIType.DefaultTimeFractionPrecision;
                    if (base.nodes[0] != null)
                    {
                        defaultTimeFractionPrecision = (int) base.nodes[0].ValueData;
                    }
                    base.DataType = DateTimeType.GetDateTimeType(0x5e, defaultTimeFractionPrecision);
                    return;
                }
                case 0x2b:
                {
                    int scale = 6;
                    if ((base.nodes.Length != 0) && (base.nodes[0] != null))
                    {
                        scale = (int) base.nodes[0].ValueData;
                    }
                    base.DataType = DateTimeType.GetDateTimeType(0x5f, scale);
                    return;
                }
                case 0x2c:
                {
                    int defaultTimeFractionPrecision = DTIType.DefaultTimeFractionPrecision;
                    if ((base.nodes.Length != 0) && (base.nodes[0] != null))
                    {
                        defaultTimeFractionPrecision = (int) base.nodes[0].ValueData;
                    }
                    base.DataType = DateTimeType.GetDateTimeType(0x5c, defaultTimeFractionPrecision);
                    return;
                }
                case 50:
                {
                    int scale = 6;
                    if ((base.nodes.Length != 0) && (base.nodes[0] != null))
                    {
                        scale = (int) base.nodes[0].ValueData;
                    }
                    base.DataType = DateTimeType.GetDateTimeType(0x5d, scale);
                    return;
                }
                case 0x33:
                case 0x34:
                case 0x35:
                case 0x36:
                case 0x37:
                case 0x38:
                case 0x39:
                case 0x3a:
                case 0x3b:
                case 60:
                    base.DataType = SqlInvariants.SqlIdentifier;
                    return;

                case 0x3d:
                    return;

                case 0x3f:
                case 0x40:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    base.DataType = base.nodes[0].DataType;
                    if ((!base.DataType.IsCharacterType() && !base.DataType.IsNumberType()) && !base.DataType.IsDateTimeType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if (!base.DataType.IsCharacterType())
                    {
                        base.DataType = SqlType.SqlVarcharDefault;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    base.nodes[1].DataType.IsIntegralType();
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        if (base.nodes[2].DataType == null)
                        {
                            throw Error.GetError(0x15bf);
                        }
                        SqlType dataType = base.nodes[2].DataType;
                        if ((!dataType.IsCharacterType() && !dataType.IsNumberType()) && !dataType.IsDateTimeType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                    }
                    return;

                case 0x42:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    if (!base.nodes[1].DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    if ((base.nodes.Length > 2) && (base.nodes[2] != null))
                    {
                        if (base.nodes[2].DataType == null)
                        {
                            base.nodes[2].DataType = SqlType.SqlChar;
                        }
                        if (!base.nodes[2].DataType.IsCharacterType())
                        {
                            throw Error.GetError(0x15bd);
                        }
                    }
                    base.DataType = SqlType.SqlBoolean;
                    return;

                case 0x43:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarcharDefault;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    if (base.nodes[2].DataType == null)
                    {
                        base.nodes[2].DataType = SqlType.SqlVarcharDefault;
                    }
                    base.DataType = SqlType.SqlVarcharDefault;
                    return;

                case 0x45:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlDecimalDefault;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    base.DataType = SqlType.SqlIntervalYearToMonth;
                    return;

                case 70:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarcharDefault;
                    }
                    base.DataType = SqlType.SqlIntervalYearToMonth;
                    return;

                case 0x47:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlDecimalDefault;
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlVarcharDefault;
                    }
                    base.DataType = SqlType.SqlIntervalDayToSecond;
                    return;

                case 0x48:
                    if (base.nodes[0].DataType == null)
                    {
                        base.nodes[0].DataType = SqlType.SqlVarcharDefault;
                    }
                    base.DataType = SqlType.SqlIntervalDayToSecond;
                    return;

                case 0x49:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (!base.nodes[0].DataType.IsArrayType())
                    {
                        throw Error.GetError(0x15bb);
                    }
                    base.DataType = SqlType.SqlInteger;
                    return;

                case 0x4a:
                    if (base.nodes[0].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (!base.nodes[0].DataType.IsArrayType())
                    {
                        throw Error.GetError(0x15bb);
                    }
                    if (base.nodes[1].DataType == null)
                    {
                        base.nodes[1].DataType = SqlType.SqlInteger;
                    }
                    if (!base.nodes[1].DataType.IsIntegralType())
                    {
                        throw Error.GetError(0x15bb);
                    }
                    base.DataType = base.nodes[0].DataType;
                    return;

                case 0x4b:
                    if (base.nodes[1].DataType == null)
                    {
                        if (base.nodes[2].DataType == null)
                        {
                            throw Error.GetError(0x15bf);
                        }
                        base.DataType = base.nodes[2].DataType;
                        return;
                    }
                    base.DataType = base.nodes[1].DataType;
                    return;

                case 0x4c:
                    base.DataType = SqlType.SqlUniqueIdentifier;
                    return;

                default:
                    throw Error.RuntimeError(0xc9, "FunctionSQL");
            }
            if (base.nodes[1].DataType == null)
            {
                if ((base.nodes[0].DataType.TypeCode == 40) || base.nodes[0].DataType.IsBinaryType())
                {
                    base.nodes[1].DataType = base.nodes[0].DataType;
                }
                else
                {
                    base.nodes[1].DataType = SqlType.SqlVarchar;
                }
            }
            if (base.nodes[0].DataType.IsCharacterType() && base.nodes[1].DataType.IsCharacterType())
            {
                base.FuncType = 1;
            }
            else
            {
                if (!base.nodes[0].DataType.IsBinaryType() || !base.nodes[1].DataType.IsBinaryType())
                {
                    throw Error.GetError(0x15bd);
                }
                base.FuncType = 2;
            }
            base.DataType = SqlType.SqlBigint;
            return;
        Label_17C9:
            if (base.nodes[0].DataType == null)
            {
                base.nodes[0].DataType = SqlType.SqlVarchar;
            }
            if (!base.nodes[0].DataType.IsCharacterType() && !base.nodes[0].DataType.IsBinaryType())
            {
                throw Error.GetError(0x15bd);
            }
            base.DataType = SqlType.SqlBigint;
            return;
        Label_1829:
            if (base.nodes[0].DataType == null)
            {
                base.nodes[0].DataType = SqlType.SqlDouble;
            }
            if (!base.nodes[0].DataType.IsNumberType())
            {
                if (!base.nodes[0].DataType.IsCharacterType())
                {
                    throw Error.GetError(0x15bd);
                }
                base.DataType = SqlType.SqlDecimalDefault;
            }
            else
            {
                base.DataType = base.nodes[0].DataType;
                if (((base.DataType.TypeCode == 3) || (base.DataType.TypeCode == 2)) && (base.DataType.Scale > 0))
                {
                    base.DataType = NumberType.GetNumberType(base.DataType.TypeCode, base.DataType.Precision + 1L, 0);
                }
            }
        }

        public virtual void SetArguments(Expression[] newNodes)
        {
            base.nodes = newNodes;
        }
    }
}

