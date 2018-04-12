namespace FwNs.Core.LC.cParsing
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ParserBase
    {
        [DecimalConstant(0, 0, (uint) 0, (uint) 0x80000000, (uint) 0)]
        public static readonly decimal LongMaxValueIncrement = 9223372036854775808M;
        private static readonly Dictionary<int, int> ExpressionTypeMap = GetExpressionTypeMap();
        private readonly Scanner _scanner;
        private bool _isRecording;
        private int _parsePosition;
        private int _parseSubPosition;
        private List<Token> _recordedStatement;
        public bool IsCheckOrTriggerCondition;
        public bool IsSchemaDefinition;
        public Token token;

        public ParserBase(Scanner t)
        {
            this._scanner = t;
            this.token = this._scanner.token;
        }

        public void CheckIsDelimitedIdentifier()
        {
            if (this.token.TokenType != 0x2eb)
            {
                throw Error.GetError(0x15c1);
            }
        }

        public void CheckIsIdentifier()
        {
            if (!this.IsIdentifier())
            {
                throw this.UnexpectedToken();
            }
        }

        public void CheckIsNamedParameter()
        {
            if (!this.IsSimpleName())
            {
                throw this.UnexpectedToken();
            }
            if (!this.token.IsHostParameter)
            {
                throw this.UnexpectedToken();
            }
        }

        public void CheckIsNonCoreReservedIdentifier()
        {
            if (!this.IsNonCoreReservedIdentifier())
            {
                throw this.UnexpectedToken();
            }
        }

        public void CheckIsNonReservedIdentifier()
        {
            if (!this.IsNonReservedIdentifier())
            {
                throw this.UnexpectedToken();
            }
        }

        public void CheckIsNotQuoted()
        {
            if (this.token.TokenType == 0x2eb)
            {
                throw this.UnexpectedToken();
            }
        }

        public void CheckIsSimpleName()
        {
            if (!this.IsSimpleName())
            {
                throw this.UnexpectedToken();
            }
        }

        public void CheckIsThis(int type)
        {
            if (this.token.TokenType != type)
            {
                throw this.UnexpectedToken();
            }
        }

        public void CheckIsValue()
        {
            if (this.token.TokenType != 0x2e9)
            {
                throw this.UnexpectedToken();
            }
        }

        public void CheckIsValue(int dataTypeCode)
        {
            if ((this.token.TokenType != 0x2e9) || (this.token.DataType.TypeCode != dataTypeCode))
            {
                throw this.UnexpectedToken();
            }
        }

        public object ConvertToNumber(string s, NumberType type)
        {
            return this._scanner.ConvertToNumber(s, type);
        }

        public static int GetExpressionType(int tokenT)
        {
            int num;
            if (!ExpressionTypeMap.TryGetValue(tokenT, out num))
            {
                throw Error.RuntimeError(0xc9, "ParserBase");
            }
            return num;
        }

        private static Dictionary<int, int> GetExpressionTypeMap()
        {
            return new Dictionary<int, int>(0x25) { 
                { 
                    0x18c,
                    0x29
                },
                { 
                    680,
                    0x2b
                },
                { 
                    0x2b3,
                    0x2c
                },
                { 
                    690,
                    0x2a
                },
                { 
                    0x2b4,
                    0x2d
                },
                { 
                    0x2b6,
                    0x2e
                },
                { 
                    0x33,
                    0x47
                },
                { 
                    0xa1,
                    0x4a
                },
                { 
                    0xa6,
                    0x49
                },
                { 
                    0x110,
                    0x48
                },
                { 
                    15,
                    0x4b
                },
                { 
                    0x60,
                    0x4c
                },
                { 
                    6,
                    0x4d
                },
                { 
                    0x100,
                    0x4d
                },
                { 
                    0x10b,
                    0x4e
                },
                { 
                    0x10c,
                    0x4f
                },
                { 
                    0x133,
                    80
                },
                { 
                    0x134,
                    0x51
                }
            };
        }

        public string GetLastPart()
        {
            return this._scanner.GetPart(this._parsePosition, this._scanner.GetTokenPosition());
        }

        public string GetLastPart(int position)
        {
            return this._scanner.GetPart(position, this._scanner.GetTokenPosition());
        }

        public string GetLastPartAndCurrent(int position)
        {
            return this._scanner.GetPart(position, this._scanner.GetPosition());
        }

        public string GetLastSubPart()
        {
            return this._scanner.GetPart(this._parseSubPosition, this._scanner.GetTokenPosition());
        }

        public int GetParsePosition()
        {
            return this._parsePosition;
        }

        public int GetPosition()
        {
            return this._scanner.GetTokenPosition();
        }

        public Token[] GetRecordedStatement()
        {
            this._isRecording = false;
            this._recordedStatement.RemoveAt(this._recordedStatement.Count - 1);
            this._recordedStatement = null;
            return this._recordedStatement.ToArray();
        }

        public Token GetRecordedToken()
        {
            if (this._isRecording)
            {
                return this._recordedStatement[this._recordedStatement.Count - 1];
            }
            return new Token();
        }

        public Scanner GetScanner()
        {
            return this._scanner;
        }

        public string GetStatement(int startPosition, short[] startTokens)
        {
            while (((this.token.TokenType != 0x2bb) && (this.token.TokenType != 0x2ec)) && (ArrayUtil.Find(startTokens, this.token.TokenType) == -1))
            {
                this.Read();
            }
            return this._scanner.GetPart(startPosition, this._scanner.GetTokenPosition());
        }

        public string GetStatementForRoutine(int startPosition, short[] startTokens)
        {
            int num = 0;
            int num2 = -1;
            int position = -1;
            while (true)
            {
                if (this.token.TokenType == 0x2bb)
                {
                    position = this._scanner.GetTokenPosition();
                    num2 = num;
                }
                else
                {
                    if (this.token.TokenType == 0x2ec)
                    {
                        break;
                    }
                    if (ArrayUtil.Find(startTokens, this.token.TokenType) != -1)
                    {
                        goto Label_006B;
                    }
                }
                this.Read();
                num++;
            }
            if ((num2 > 0) && (num2 == (num - 1)))
            {
                this.Rewind(position);
            }
        Label_006B:
            return this._scanner.GetPart(startPosition, this._scanner.GetTokenPosition());
        }

        public int GetSubParsePosition()
        {
            return this._parseSubPosition;
        }

        public bool IsCoreReservedKey()
        {
            return this._scanner.token.IsCoreReservedIdentifier;
        }

        public bool IsDelimitedIdentifier()
        {
            return this._scanner.token.IsDelimitedIdentifier;
        }

        public bool IsDelimitedSimpleName()
        {
            return (this.token.IsDelimitedIdentifier && (this.token.NamePrefix == null));
        }

        public bool IsIdentifier()
        {
            if (!this._scanner.token.IsUndelimitedIdentifier)
            {
                return this._scanner.token.IsDelimitedIdentifier;
            }
            return true;
        }

        public bool IsNonCoreReservedIdentifier()
        {
            if (this._scanner.token.IsCoreReservedIdentifier)
            {
                return false;
            }
            if (!this._scanner.token.IsUndelimitedIdentifier)
            {
                return this._scanner.token.IsDelimitedIdentifier;
            }
            return true;
        }

        public bool IsNonReservedIdentifier()
        {
            if (this._scanner.token.IsReservedIdentifier)
            {
                return false;
            }
            if (!this._scanner.token.IsUndelimitedIdentifier)
            {
                return this._scanner.token.IsDelimitedIdentifier;
            }
            return true;
        }

        public bool IsReservedKey()
        {
            return this._scanner.token.IsReservedIdentifier;
        }

        public bool IsSimpleName()
        {
            return (this.IsNonCoreReservedIdentifier() && (this.token.NamePrefix == null));
        }

        public bool IsUndelimitedSimpleName()
        {
            return (this.token.IsUndelimitedIdentifier && (this.token.NamePrefix == null));
        }

        public void Read()
        {
            this._scanner.ScanNext();
            if (!this.token.IsMalformed)
            {
                if (this._isRecording)
                {
                    Token item = this.token.Duplicate();
                    item.Position = this._scanner.GetTokenPosition();
                    this._recordedStatement.Add(item);
                }
            }
            else
            {
                int code = -1;
                switch (this.token.TokenType)
                {
                    case 0x2f1:
                        code = 0x15d0;
                        break;

                    case 0x2f2:
                        code = 0x15d1;
                        break;

                    case 0x2f3:
                        code = 0x15d4;
                        break;

                    case 0x2f4:
                        code = 0x15d3;
                        break;

                    case 0x2f5:
                        code = 0x15d2;
                        break;

                    case 0x2f6:
                        code = 0x15d5;
                        break;

                    case 0x2f7:
                        code = 0x15cf;
                        break;

                    case -1:
                        code = 0x15ce;
                        break;
                }
                throw Error.GetError(code);
            }
        }

        public long ReadBigint()
        {
            bool flag = false;
            if (this.token.TokenType == 0x2b5)
            {
                flag = true;
                this.Read();
            }
            this.CheckIsValue();
            if (flag && (this.token.DataType.TypeCode == 2))
            {
                decimal num2 = 9223372036854775808M;
                if (num2.Equals(this.token.TokenValue))
                {
                    this.Read();
                    return -9223372036854775808L;
                }
            }
            if ((this.token.DataType.TypeCode != 4) && (this.token.DataType.TypeCode != 0x19))
            {
                throw Error.GetError(0x15bd);
            }
            long num3 = Convert.ToInt64(this.token.TokenValue);
            if (flag)
            {
                num3 = -num3;
            }
            this.Read();
            return num3;
        }

        public Expression ReadDateTimeIntervalLiteral()
        {
            int position = this.GetPosition();
            switch (this.token.TokenType)
            {
                case 0x47:
                    this.Read();
                    if ((this.token.TokenType == 0x2e9) && (this.token.DataType.TypeCode == 12))
                    {
                        string tokenString = this.token.TokenString;
                        this.Read();
                        return new ExpressionValue(this._scanner.NewDate(tokenString), SqlType.SqlDate);
                    }
                    break;

                case 0x8a:
                {
                    bool flag = false;
                    this.Read();
                    if (this.token.TokenType == 0x2b5)
                    {
                        this.Read();
                        flag = true;
                    }
                    else if (this.token.TokenType == 0x2b8)
                    {
                        this.Read();
                    }
                    if ((this.token.TokenType != 0x2e9) || (this.token.DataType.TypeCode != 12))
                    {
                        break;
                    }
                    string tokenString = this.token.TokenString;
                    this.Read();
                    IntervalType type = this.ReadIntervalType(false);
                    object a = this._scanner.NewInterval(tokenString, type);
                    type = (IntervalType) this._scanner.DateTimeType;
                    if (flag)
                    {
                        a = type.Negate(a);
                    }
                    return new ExpressionValue(a, type);
                }
                case 0x117:
                {
                    this.Read();
                    if ((this.token.TokenType != 0x2e9) || (this.token.DataType.TypeCode != 12))
                    {
                        break;
                    }
                    string tokenString = this.token.TokenString;
                    this.Read();
                    return new ExpressionValue(this._scanner.NewTime(tokenString), this._scanner.DateTimeType);
                }
                case 280:
                {
                    this.Read();
                    if ((this.token.TokenType != 0x2e9) || (this.token.DataType.TypeCode != 12))
                    {
                        break;
                    }
                    string tokenString = this.token.TokenString;
                    this.Read();
                    return new ExpressionValue(this._scanner.NewTimestamp(tokenString), this._scanner.DateTimeType);
                }
                default:
                    throw Error.RuntimeError(0xc9, "ParserBase");
            }
            this.Rewind(position);
            return null;
        }

        public bool ReadIfThis(int tokenId)
        {
            if (this.token.TokenType == tokenId)
            {
                this.Read();
                return true;
            }
            return false;
        }

        public int ReadInteger()
        {
            bool flag = false;
            if (this.token.TokenType == 0x2b5)
            {
                flag = true;
                this.Read();
            }
            this.CheckIsValue();
            if ((flag && (this.token.DataType.TypeCode == 0x19)) && (Convert.ToInt64(this.token.TokenValue) == -2147483648L))
            {
                this.Read();
                return -2147483648;
            }
            if (this.token.DataType.TypeCode != 4)
            {
                throw Error.GetError(0x15bd);
            }
            int tokenValue = (int) this.token.TokenValue;
            if (flag)
            {
                tokenValue = -tokenValue;
            }
            this.Read();
            return tokenValue;
        }

        public int ReadIntegerObject()
        {
            return this.ReadInteger();
        }

        public IntervalType ReadIntervalType(bool maxPrecisionDefault)
        {
            int tokenType;
            int num = maxPrecisionDefault ? 9 : -1;
            int fractionPrecision = -1;
            int num4 = tokenType = this.token.TokenType;
            this.Read();
            if (this.token.TokenType == 0x2b7)
            {
                this.Read();
                num = this.ReadInteger();
                if (num <= 0)
                {
                    throw Error.GetError(0x15d8);
                }
                if (this.token.TokenType == 0x2ac)
                {
                    if (num4 != 0xf8)
                    {
                        throw this.UnexpectedToken();
                    }
                    this.Read();
                    fractionPrecision = this.ReadInteger();
                    if (fractionPrecision < 0)
                    {
                        throw Error.GetError(0x15d8);
                    }
                }
                this.ReadThis(0x2aa);
            }
            if (this.token.TokenType == 0x11b)
            {
                this.Read();
                tokenType = this.token.TokenType;
                this.Read();
            }
            if (this.token.TokenType == 0x2b7)
            {
                if ((tokenType != 0xf8) || (tokenType == num4))
                {
                    throw this.UnexpectedToken();
                }
                this.Read();
                fractionPrecision = this.ReadInteger();
                if (fractionPrecision < 0)
                {
                    throw Error.GetError(0x15d8);
                }
                this.ReadThis(0x2aa);
            }
            return IntervalType.GetIntervalType(ArrayUtil.Find(Tokens.SQL_INTERVAL_FIELD_CODES, num4), ArrayUtil.Find(Tokens.SQL_INTERVAL_FIELD_CODES, tokenType), (long) num, fractionPrecision);
        }

        public string ReadQuotedString()
        {
            this.CheckIsValue();
            if (this.token.DataType.TypeCode != 12)
            {
                throw Error.GetError(0x15bd);
            }
            this.Read();
            return this.token.TokenString;
        }

        public void ReadThis(int tokenId)
        {
            if (this.token.TokenType != tokenId)
            {
                string keyword = Tokens.GetKeyword(tokenId);
                throw this.UnexpectedTokenRequire(keyword);
            }
            this.Read();
        }

        public virtual void Reset(string sql)
        {
            this._scanner.Reset(sql);
            this._parsePosition = 0;
            this.IsCheckOrTriggerCondition = false;
            this.IsSchemaDefinition = false;
            this._isRecording = false;
            this._recordedStatement = null;
        }

        public virtual void Rewind(int position)
        {
            if (position != this._scanner.GetTokenPosition())
            {
                this._scanner.Position(position);
                if (this._isRecording)
                {
                    int num = this._recordedStatement.Count - 1;
                    while (num >= 0)
                    {
                        if (this._recordedStatement[num].Position < position)
                        {
                            break;
                        }
                        num--;
                    }
                    this._recordedStatement.RemoveRange(num + 1, (this._recordedStatement.Count - num) - 1);
                }
                this.Read();
            }
        }

        public void SetParsePosition(int parsePosition)
        {
            this._parsePosition = parsePosition;
        }

        public void SetSubParsePosition(int parseSubPosition)
        {
            this._parseSubPosition = parseSubPosition;
        }

        public void StartRecording()
        {
            this._recordedStatement = new List<Token>();
            this._recordedStatement.Add(this.token.Duplicate());
            this._isRecording = true;
        }

        public CoreException TooManyIdentifiers()
        {
            string namePrePrePrefix;
            if (this.token.NamePrePrePrefix != null)
            {
                namePrePrePrefix = this.token.NamePrePrePrefix;
            }
            else if (this.token.NamePrePrefix != null)
            {
                namePrePrePrefix = this.token.NamePrePrefix;
            }
            else if (this.token.NamePrefix != null)
            {
                namePrePrePrefix = this.token.NamePrefix;
            }
            else
            {
                namePrePrePrefix = this.token.TokenString;
            }
            return Error.GetParseError(0x15af, namePrePrePrefix, this._scanner.GetLineNumber());
        }

        public CoreException UnexpectedToken()
        {
            string charsetSchema;
            if (this.token.TokenType == 0x2ec)
            {
                return Error.GetParseError(0x15d6, null, this._scanner.GetLineNumber());
            }
            if (this.token.CharsetSchema != null)
            {
                charsetSchema = this.token.CharsetSchema;
            }
            else if (this.token.CharsetName != null)
            {
                charsetSchema = this.token.CharsetName;
            }
            else if (this.token.NamePrePrefix != null)
            {
                charsetSchema = this.token.NamePrePrefix;
            }
            else if (this.token.NamePrefix != null)
            {
                charsetSchema = this.token.NamePrefix;
            }
            else
            {
                charsetSchema = this.token.TokenString;
            }
            return Error.GetParseError(0x15cd, charsetSchema, this._scanner.GetLineNumber());
        }

        public CoreException UnexpectedToken(string tokenS)
        {
            return Error.GetParseError(0x15cd, tokenS, this._scanner.GetLineNumber());
        }

        public CoreException UnexpectedTokenRequire(string required)
        {
            string charsetSchema;
            if (this.token.TokenType == 0x2ec)
            {
                object[] objArray1 = new object[] { "", required };
                return Error.GetParseError(0x15d6, 1, this._scanner.GetLineNumber(), objArray1);
            }
            if (this.token.CharsetSchema != null)
            {
                charsetSchema = this.token.CharsetSchema;
            }
            else if (this.token.CharsetName != null)
            {
                charsetSchema = this.token.CharsetName;
            }
            else if (this.token.NamePrePrefix != null)
            {
                charsetSchema = this.token.NamePrePrefix;
            }
            else if (this.token.NamePrefix != null)
            {
                charsetSchema = this.token.NamePrefix;
            }
            else
            {
                charsetSchema = this.token.TokenString;
            }
            object[] add = new object[] { charsetSchema, required };
            return Error.GetParseError(0x15cd, 1, this._scanner.GetLineNumber(), add);
        }

        public CoreException UnsupportedFeature()
        {
            return Error.GetError(0x60f, this.token.TokenString);
        }

        public static CoreException UnsupportedFeature(string str)
        {
            return Error.GetError(0x60f, str);
        }
    }
}

