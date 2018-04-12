namespace FwNs.Core.LC.cParsing
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public sealed class Scanner : IDisposable
    {
        private CultureInfo _culture;
        private static readonly char[] Whitespace = new char[] { 
            '\t', '\n', '\v', '\f', '\r', ' ', '\x0085', ' ', '\x00a0', ' ', '᠎', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '　', '\u2028', '\u2029'
        };
        private static readonly HashSet<char> WhiteSpaceSet = GetWhiteSpaceSet();
        private readonly MemoryStream _byteOutputStream;
        private readonly StringBuilder _charWriter;
        private int _currentPosition;
        private int _fractionPrecision;
        private int _intervalPosition;
        private int _intervalPrecision;
        private string _intervalString;
        private int _limit;
        private bool _nullAndBooleanAsValue;
        private string _sqlString;
        private int _tokenPosition;
        public SqlType DateTimeType;
        private int _eolCode;
        private int _eolPosition;
        private bool _hasNonSpaceSeparator;
        private int _lineNumber;
        public Token token;

        public Scanner()
        {
            this._culture = CultureInfo.CurrentCulture;
            this.token = new Token();
            this._charWriter = new StringBuilder();
            this._byteOutputStream = new MemoryStream();
        }

        public Scanner(CultureInfo culture) : this()
        {
            this._culture = culture;
        }

        public Scanner(string sql)
        {
            this._culture = CultureInfo.CurrentCulture;
            this.token = new Token();
            this._charWriter = new StringBuilder();
            this._byteOutputStream = new MemoryStream();
            this.Reset(sql);
        }

        private int CharAt(int i)
        {
            if (i >= this._limit)
            {
                return -1;
            }
            return this._sqlString[i];
        }

        public BinaryData ConvertToBinary(string s)
        {
            lock (this)
            {
                bool flag = true;
                byte num = 0;
                this.Reset(s);
                this.ResetState();
                this._byteOutputStream.SetLength(0L);
                while (this._currentPosition < this._limit)
                {
                    int c = this._sqlString[this._currentPosition];
                    c = GetHexValue(c);
                    if (c == -1)
                    {
                        this.token.TokenType = 0x2f4;
                        this.token.IsMalformed = true;
                        break;
                    }
                    if (flag)
                    {
                        num = (byte) (c << 4);
                    }
                    else
                    {
                        num = (byte) (num + ((byte) c));
                        this._byteOutputStream.WriteByte(num);
                    }
                    this._currentPosition++;
                    flag = !flag;
                }
                if (!flag)
                {
                    this.token.TokenType = 0x2f4;
                    this.token.IsMalformed = true;
                }
                if (this.token.IsMalformed)
                {
                    throw Error.GetError(0xd6e);
                }
                this._byteOutputStream.SetLength(0L);
                return new BinaryData(this.GetByteArray(), false);
            }
        }

        public object ConvertToDatetimeInterval(ISessionInterface session, string s, DTIType type)
        {
            lock (this)
            {
                object obj4;
                object obj5;
                IntervalType type2 = null;
                int num = -1;
                int code = type.IsDateTimeType() ? 0xd4f : 0xd4e;
                this.Reset(s);
                this.ResetState();
                this.ScanToken();
                this.ScanWhitespace();
                int tokenType = this.token.TokenType;
                if (((tokenType == 0x47) || (tokenType == 0x8a)) || ((tokenType - 0x117) <= 1))
                {
                    num = this.token.TokenType;
                    this.ScanToken();
                    if ((this.token.TokenType != 0x2e9) || (this.token.DataType.TypeCode != 12))
                    {
                        throw Error.GetError(code);
                    }
                    s = this.token.TokenString;
                    this.ScanNext(0xd4f);
                    if (type.IsIntervalType())
                    {
                        type2 = this.ScanIntervalType();
                    }
                    if (this.token.TokenType != 0x2ec)
                    {
                        throw Error.GetError(code);
                    }
                }
                switch (type.TypeCode)
                {
                    case 0x5b:
                        if ((num != -1) && (num != 0x47))
                        {
                            throw Error.GetError(code);
                        }
                        break;

                    case 0x5c:
                    case 0x5e:
                        if ((num != -1) && (num != 0x117))
                        {
                            throw Error.GetError(code);
                        }
                        goto Label_014A;

                    case 0x5d:
                    case 0x5f:
                        if ((num != -1) && (num != 280))
                        {
                            throw Error.GetError(code);
                        }
                        goto Label_017B;

                    default:
                    {
                        if ((num != -1) && (num != 0x8a))
                        {
                            throw Error.GetError(code);
                        }
                        if (!type.IsIntervalType())
                        {
                            throw Error.RuntimeError(0xc9, "Scanner");
                        }
                        object obj6 = this.NewInterval(s, (IntervalType) type);
                        if ((type2 != null) && ((type2.StartIntervalType != type.StartIntervalType) || (type2.EndIntervalType != type.EndIntervalType)))
                        {
                            throw Error.GetError(code);
                        }
                        return type.ConvertToType(session, obj6, this.DateTimeType);
                    }
                }
                object a = this.NewDate(s);
                return type.ConvertToType(session, a, SqlType.SqlDate);
            Label_014A:
                obj4 = this.NewTime(s);
                return type.ConvertToType(session, obj4, this.DateTimeType);
            Label_017B:
                obj5 = this.NewTimestamp(s);
                return type.ConvertToType(session, obj5, this.DateTimeType);
            }
        }

        public object ConvertToNumber(string s, NumberType numberType)
        {
            object obj4;
            lock (this)
            {
                bool flag = false;
                this.Reset(s);
                this.ResetState();
                this.ScanWhitespace();
                this.ScanToken();
                this.ScanWhitespace();
                if (this.token.TokenType == 0x2b8)
                {
                    this.ScanToken();
                    this.ScanWhitespace();
                }
                else if (this.token.TokenType == 0x2b5)
                {
                    flag = true;
                    this.ScanToken();
                    this.ScanWhitespace();
                }
                if ((!this._hasNonSpaceSeparator && (this.token.TokenType == 0x2e9)) && (this.token.TokenValue is ValueType))
                {
                    object tokenValue = this.token.TokenValue;
                    SqlType dataType = this.token.DataType;
                    if (flag)
                    {
                        tokenValue = this.token.DataType.Negate(tokenValue);
                    }
                    this.ScanEnd();
                    if (this.token.TokenType == 0x2ec)
                    {
                        return numberType.ConvertToType(null, tokenValue, dataType);
                    }
                }
                throw Error.GetError(0xd6e);
            }
            return obj4;
        }

        private void ConvertUnicodeString(int escape)
        {
            int length;
            this._charWriter.Length = 0;
            int startIndex = 0;
        Label_000E:
            length = this.token.TokenString.IndexOf((char) escape, startIndex);
            if (length < 0)
            {
                length = this.token.TokenString.Length;
            }
            this._charWriter.Append(this.token.TokenString, startIndex, length - startIndex);
            if (length != this.token.TokenString.Length)
            {
                length++;
                if (length == this.token.TokenString.Length)
                {
                    this.token.TokenType = 0x2f5;
                    this.token.IsMalformed = true;
                    return;
                }
                if (this.token.TokenString[length] == escape)
                {
                    this._charWriter.Append((char) escape);
                    length++;
                    startIndex = length;
                }
                else
                {
                    startIndex = length;
                    if (char.ToUpper(this.token.TokenString[startIndex]) == 'U')
                    {
                        startIndex++;
                    }
                    if (length > (this.token.TokenString.Length - 4))
                    {
                        this.token.TokenType = 0x2f5;
                        this.token.IsMalformed = true;
                        return;
                    }
                    int num3 = 4;
                    int num4 = 0;
                    int num5 = 0;
                    if (this.token.TokenString[startIndex] == '+')
                    {
                        startIndex++;
                        if (length > (this.token.TokenString.Length - 6))
                        {
                            this.token.TokenType = 0x2f5;
                            this.token.IsMalformed = true;
                            return;
                        }
                        num4 = 2;
                        num3 = 8;
                    }
                    while (num4 < num3)
                    {
                        int c = this.token.TokenString[startIndex++];
                        c = GetHexValue(c);
                        if (c == -1)
                        {
                            this.token.TokenType = 0x2f5;
                            this.token.IsMalformed = true;
                            return;
                        }
                        num5 |= c << (((num3 - num4) - 1) * 4);
                        num4++;
                    }
                    if (num3 == 8)
                    {
                        this._charWriter.Append((char) num5);
                    }
                    else
                    {
                        this._charWriter.Append((char) (num5 & 0xffff));
                    }
                }
                goto Label_000E;
            }
            this.token.TokenValue = this._charWriter.ToString();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._byteOutputStream.Dispose();
            }
        }

        private byte[] GetByteArray()
        {
            byte[] buffer = new byte[(int) ((IntPtr) this._byteOutputStream.Length)];
            this._byteOutputStream.Position = 0L;
            this._byteOutputStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public SqlType GetDataType()
        {
            return this.token.DataType;
        }

        private static int GetHexValue(int c)
        {
            if ((c >= 0x30) && (c <= 0x39))
            {
                c -= 0x30;
                return c;
            }
            if (c > 0x7a)
            {
                c = 0x10;
                return c;
            }
            if (c >= 0x61)
            {
                c -= 0x57;
                return c;
            }
            if (c > 90)
            {
                c = 0x10;
                return c;
            }
            if (c >= 0x41)
            {
                c -= 0x37;
                return c;
            }
            c = -1;
            return c;
        }

        public int GetLineNumber()
        {
            return this._lineNumber;
        }

        public string GetPart(int start, int end)
        {
            return this._sqlString.Substring(start, end - start);
        }

        public int GetPosition()
        {
            return this._tokenPosition;
        }

        public string GetString()
        {
            return this.token.TokenString;
        }

        public Token GetToken()
        {
            return this.token;
        }

        public int GetTokenPosition()
        {
            return this._tokenPosition;
        }

        public int GetTokenType()
        {
            return this.token.TokenType;
        }

        public object GetValue()
        {
            return this.token.TokenValue;
        }

        private static HashSet<char> GetWhiteSpaceSet()
        {
            HashSet<char> set = new HashSet<char>();
            for (int i = 0; i < Whitespace.Length; i++)
            {
                set.Add(Whitespace[i]);
            }
            return set;
        }

        public TimestampData NewDate(string s)
        {
            this._intervalPosition = 0;
            this._fractionPrecision = 0;
            this.DateTimeType = null;
            this._intervalString = s;
            return new TimestampData(UtlDateTime.GetDateSeconds(s));
        }

        public object NewInterval(string s, IntervalType type)
        {
            this._intervalPosition = 0;
            this._intervalString = s;
            long months = this.ScanIntervalValue(type);
            int nanos = 0;
            if (type.EndIntervalType == 0x6a)
            {
                nanos = this.ScanIntervalFraction(type.Scale);
            }
            if (this._intervalPosition != s.Length)
            {
                throw Error.GetError(0xd4e);
            }
            if (this.ScanIntervalSign())
            {
                months = -months;
                nanos = -nanos;
            }
            this.DateTimeType = type;
            if (type.DefaultPrecision)
            {
                this.DateTimeType = IntervalType.GetIntervalType(type.TypeCode, type.StartIntervalType, type.EndIntervalType, (long) this._intervalPrecision, this._fractionPrecision, false);
            }
            if (type.EndPartIndex <= DTIType.IntervalMonthIndex)
            {
                return new IntervalMonthData(months);
            }
            return new IntervalSecondData(months, nanos);
        }

        public TimeData NewTime(string s)
        {
            this._intervalPosition = 0;
            this._fractionPrecision = 0;
            this.DateTimeType = null;
            this._intervalString = s;
            long num = this.ScanIntervalValue(SqlType.SqlIntervalHourToSecond);
            int nanos = this.ScanIntervalFraction(9);
            long num3 = 0L;
            bool flag = false;
            bool flag2 = this.ScanIntervalSign();
            if (this._intervalPosition != this._intervalPosition)
            {
                num3 = this.ScanIntervalValue(SqlType.SqlIntervalHourToMinute);
                flag = true;
            }
            if (this._intervalPosition != s.Length)
            {
                throw Error.GetError(0xd51);
            }
            if (num >= DTIType.YearToSecondFactors[2])
            {
                throw Error.GetError(0xd50);
            }
            if (num3 > DTIType.TimezoneSecondsLimit)
            {
                throw Error.GetError(0xd51);
            }
            if (flag2)
            {
                num3 = -num3;
            }
            int type = flag ? 0x5e : 0x5c;
            this.DateTimeType = FwNs.Core.LC.cDataTypes.DateTimeType.GetDateTimeType(type, this._fractionPrecision);
            if (flag)
            {
                num -= num3;
            }
            return new TimeData((long) ((int) num), nanos, (int) num3);
        }

        public TimestampData NewTimestamp(string s)
        {
            long timestampSeconds;
            long num = 0L;
            int length = s.Length;
            bool flag = false;
            this._intervalPosition = 0;
            this._fractionPrecision = 0;
            this.DateTimeType = null;
            this._intervalString = s;
            if (this._intervalString.Contains("."))
            {
                this._intervalPosition = this._intervalString.LastIndexOf('.');
            }
            else if (this._intervalString.Contains("+"))
            {
                this._intervalPosition = this._intervalString.LastIndexOf('+');
            }
            else if (this._intervalString.Contains("-"))
            {
                if (!this._intervalString.Contains(" "))
                {
                    this._intervalPosition = this._intervalString.Length;
                }
                else if (this._intervalString.LastIndexOf(" ") < this._intervalString.LastIndexOf('-'))
                {
                    this._intervalPosition = this._intervalString.LastIndexOf('-');
                }
                else
                {
                    this._intervalPosition = this._intervalString.Length;
                }
            }
            else
            {
                this._intervalPosition = this._intervalString.Length;
            }
            try
            {
                timestampSeconds = UtlDateTime.GetTimestampSeconds(s.Substring(0, this._intervalPosition));
            }
            catch (Exception)
            {
                throw Error.GetError(0xd4f);
            }
            int nanos = this.ScanIntervalFraction(9);
            int num5 = this._intervalPosition;
            bool flag2 = this.ScanIntervalSign();
            if (flag2 || (num5 != this._intervalPosition))
            {
                num = this.ScanIntervalValue(SqlType.SqlIntervalHourToMinute);
                flag = true;
                if (flag2)
                {
                    num = -num;
                }
            }
            if (((num >= DTIType.YearToSecondFactors[2]) || (num > DTIType.TimezoneSecondsLimit)) || (-num > DTIType.TimezoneSecondsLimit))
            {
                throw Error.GetError(0xd51);
            }
            if (this._intervalPosition != length)
            {
                throw Error.GetError(0xd4f);
            }
            int type = flag ? 0x5f : 0x5d;
            this.DateTimeType = FwNs.Core.LC.cDataTypes.DateTimeType.GetDateTimeType(type, this._fractionPrecision);
            if (flag)
            {
                timestampSeconds -= num;
            }
            return new TimestampData(timestampSeconds, nanos, (int) num);
        }

        public void Position(int position)
        {
            this._tokenPosition = position;
            this._currentPosition = position;
        }

        public void Reset(string sql)
        {
            this._sqlString = sql;
            this._currentPosition = 0;
            this._tokenPosition = 0;
            this._limit = this._sqlString.Length;
            this._hasNonSpaceSeparator = false;
            this._eolPosition = -1;
            this._lineNumber = 1;
            this.token.Reset();
            this.token.TokenType = 0x2ed;
        }

        private void ResetState()
        {
            this._tokenPosition = this._currentPosition;
            this.token.Reset();
        }

        private void ScanBinaryString()
        {
            this._byteOutputStream.SetLength(0L);
            do
            {
                this.ScanBinaryStringPart();
                if (this.token.IsMalformed)
                {
                    return;
                }
                if (!this.ScanSeparator())
                {
                    break;
                }
            }
            while (this.CharAt(this._currentPosition) == 0x27);
            this.token.TokenValue = new BinaryData(this.GetByteArray(), false);
            this._byteOutputStream.SetLength(0L);
        }

        private void ScanBinaryStringPart()
        {
            bool flag = false;
            bool flag2 = true;
            byte num = 0;
            this._currentPosition++;
            while (this._currentPosition < this._limit)
            {
                int c = this._sqlString[this._currentPosition];
                if (c != 0x20)
                {
                    if (c == 0x27)
                    {
                        flag = true;
                        this._currentPosition++;
                        break;
                    }
                    c = GetHexValue(c);
                    if (c == -1)
                    {
                        this.token.TokenType = 0x2f4;
                        this.token.IsMalformed = true;
                        return;
                    }
                    if (flag2)
                    {
                        num = (byte) (c << 4);
                        flag2 = false;
                    }
                    else
                    {
                        num = (byte) (num + ((byte) c));
                        this._byteOutputStream.WriteByte(num);
                        flag2 = true;
                    }
                }
                this._currentPosition++;
            }
            if (!flag2)
            {
                this.token.TokenType = 0x2f4;
                this.token.IsMalformed = true;
            }
            else if (!flag)
            {
                this.token.TokenType = 0x2f4;
                this.token.IsMalformed = true;
            }
        }

        public void ScanBinaryStringWithQuote()
        {
            this.ResetState();
            this.ScanSeparator();
            if (this.CharAt(this._currentPosition) != 0x27)
            {
                this.token.TokenType = 0x2f4;
                this.token.IsMalformed = true;
            }
            else
            {
                this.ScanBinaryString();
            }
        }

        private void ScanCharacterString()
        {
            this._charWriter.Length = 0;
            do
            {
                this.ScanStringPart('\'');
                if (this.token.IsMalformed)
                {
                    return;
                }
                if (!this.ScanSeparator())
                {
                    break;
                }
            }
            while (this.CharAt(this._currentPosition) == 0x27);
            this.token.TokenString = this._charWriter.ToString();
            this.token.TokenValue = this.token.TokenString;
        }

        private bool ScanCommentAsInlineSeparator()
        {
            int num = this.CharAt(this._currentPosition);
            if ((num == 0x2d) && (this.CharAt(this._currentPosition + 1) == 0x2d))
            {
                int num2 = this._sqlString.IndexOf('\r', this._currentPosition + 2);
                if (num2 == -1)
                {
                    num2 = this._sqlString.IndexOf('\n', this._currentPosition + 2);
                }
                else if (this.CharAt(num2 + 1) == 10)
                {
                    num2++;
                }
                if (num2 == -1)
                {
                    this._currentPosition = this._limit;
                }
                else
                {
                    this._currentPosition = num2 + 1;
                }
                return true;
            }
            if ((num != 0x2f) || (this.CharAt(this._currentPosition + 1) != 0x2a))
            {
                return false;
            }
            int index = this._sqlString.IndexOf("*/", (int) (this._currentPosition + 2));
            if (index == -1)
            {
                this.token.TokenString = this._sqlString.Substring(this._currentPosition, (this._currentPosition + 2) - this._currentPosition);
                this.token.TokenType = 0x2f6;
                this.token.IsMalformed = true;
                return false;
            }
            this._currentPosition = index + 2;
            return true;
        }

        private void ScanDateParts(int lastPart)
        {
            byte[] yearToSecondSeparators = DTIType.YearToSecondSeparators;
            int num = this._intervalPosition;
            int index = 0;
            int num3 = 0;
            while (index <= lastPart)
            {
                bool flag = false;
                if (num == this._intervalString.Length)
                {
                    if (index != lastPart)
                    {
                        throw Error.GetError(0xd4f);
                    }
                    flag = true;
                }
                else
                {
                    int num4 = this._intervalString[num];
                    if ((num4 >= 0x30) && (num4 <= 0x39))
                    {
                        num3++;
                        num++;
                    }
                    else if (((num4 >= 0x41) && (num4 <= 90)) || ((num4 >= 0x61) && (num4 <= 0x7a)))
                    {
                        num++;
                        num3++;
                    }
                    else if (num4 == yearToSecondSeparators[index])
                    {
                        flag = true;
                        if (index != lastPart)
                        {
                            num++;
                        }
                    }
                    else
                    {
                        if (index != lastPart)
                        {
                            throw Error.GetError(0xd4f);
                        }
                        flag = true;
                    }
                }
                if (flag)
                {
                    index++;
                    num3 = 0;
                    if (num == this._intervalString.Length)
                    {
                        break;
                    }
                }
            }
            this._intervalPosition = num;
        }

        public void ScanEnd()
        {
            if (this._currentPosition == this._limit)
            {
                this.ResetState();
                this.token.TokenType = 0x2ec;
            }
        }

        private int ScanEscapeDefinition()
        {
            if (this.CharAt(this._currentPosition) == 0x27)
            {
                this._currentPosition++;
                if (!this.ScanWhitespace())
                {
                    int c = this.CharAt(this._currentPosition);
                    if (((GetHexValue(c) == -1) && (c != 0x2b)) && ((c != 0x27) && (c != 0x22)))
                    {
                        int num2 = c;
                        this._currentPosition++;
                        if (this.CharAt(this._currentPosition) == 0x27)
                        {
                            this._currentPosition++;
                            return num2;
                        }
                    }
                }
            }
            return -1;
        }

        private void ScanIdentifierChain()
        {
            int num = this.CharAt(this._currentPosition);
            if (num != 0x22)
            {
                if (((num == 0x55) || (num == 0x75)) && ((this.CharAt(this._currentPosition + 1) == 0x26) && (this.CharAt(this._currentPosition + 2) == 0x22)))
                {
                    this._currentPosition += 2;
                    if (!this.ScanUnicodeIdentifier())
                    {
                        return;
                    }
                    this.token.TokenType = 0x2eb;
                    this.token.IsDelimiter = false;
                }
                else
                {
                    if (!this.ScanUndelimitedIdentifier())
                    {
                        return;
                    }
                    this.token.TokenType = 0x2ea;
                    this.token.IsDelimiter = false;
                }
            }
            else
            {
                this._charWriter.Length = 0;
                this.ScanStringPart('"');
                if (this.token.IsMalformed)
                {
                    return;
                }
                this.token.TokenType = 0x2eb;
                this.token.TokenString = this._charWriter.ToString();
                this.token.IsDelimiter = true;
            }
            if (this.CharAt(this._currentPosition) == 0x2e)
            {
                this._currentPosition++;
                if (this.CharAt(this._currentPosition) == 0x2a)
                {
                    this._currentPosition++;
                    this.ShiftPrefixes();
                    this.token.TokenString = "*";
                    this.token.TokenType = 0x2a9;
                }
                else
                {
                    this.ShiftPrefixes();
                    this.ScanIdentifierChain();
                }
            }
        }

        private int ScanIntervalFraction(int decimalPrecision)
        {
            if (this._intervalPosition == this._intervalString.Length)
            {
                return 0;
            }
            if (this._intervalString[this._intervalPosition] != '.')
            {
                return 0;
            }
            this._intervalPosition++;
            int fraction = 0;
            int index = 0;
            while (this._intervalPosition < this._intervalString.Length)
            {
                int num4 = this._intervalString[this._intervalPosition];
                if ((num4 < 0x30) || (num4 > 0x39))
                {
                    break;
                }
                int num5 = num4 - 0x30;
                fraction *= 10;
                fraction += num5;
                this._intervalPosition++;
                index++;
                if (index == 9)
                {
                    break;
                }
            }
            this._fractionPrecision = index;
            fraction *= DTIType.NanoScaleFactors[index];
            return DTIType.NormaliseFraction(fraction, decimalPrecision);
        }

        private bool ScanIntervalSign()
        {
            bool flag = false;
            if (this._intervalPosition == this._intervalString.Length)
            {
                return false;
            }
            if (this._intervalString[this._intervalPosition] == '-')
            {
                flag = true;
                this._intervalPosition++;
            }
            else if (this._intervalString[this._intervalPosition] == '+')
            {
                this._intervalPosition++;
            }
            return flag;
        }

        private IntervalType ScanIntervalType()
        {
            int tokenType;
            int num = -1;
            int fractionPrecision = -1;
            int num4 = tokenType = this.token.TokenType;
            this.ScanNext(0xd4e);
            if (this.token.TokenType == 0x2b7)
            {
                this.ScanNext(0xd4e);
                if ((this.token.DataType == null) || (this.token.DataType.TypeCode != 4))
                {
                    throw Error.GetError(0xd4e);
                }
                num = Convert.ToInt32(this.token.TokenValue);
                this.ScanNext(0xd4e);
                if (this.token.TokenType == 0x2ac)
                {
                    if (num4 != 0xf8)
                    {
                        throw Error.GetError(0xd4e);
                    }
                    this.ScanNext(0xd4e);
                    if ((this.token.DataType == null) || (this.token.DataType.TypeCode != 4))
                    {
                        throw Error.GetError(0xd4e);
                    }
                    fractionPrecision = Convert.ToInt32(this.token.TokenValue);
                    this.ScanNext(0xd4e);
                }
                if (this.token.TokenType != 0x2aa)
                {
                    throw Error.GetError(0xd4e);
                }
                this.ScanNext(0xd4e);
            }
            if (this.token.TokenType == 0x11b)
            {
                this.ScanNext(0xd4e);
                tokenType = this.token.TokenType;
                this.ScanNext(0xd4e);
            }
            if (this.token.TokenType == 0x2b7)
            {
                if ((tokenType != 0xf8) || (tokenType == num4))
                {
                    throw Error.GetError(0xd4e);
                }
                this.ScanNext(0xd4e);
                if ((this.token.DataType == null) || (this.token.DataType.TypeCode != 4))
                {
                    throw Error.GetError(0xd4e);
                }
                fractionPrecision = Convert.ToInt32(this.token.TokenValue);
                this.ScanNext(0xd4e);
                if (this.token.TokenType != 0x2aa)
                {
                    throw Error.GetError(0xd4e);
                }
                this.ScanNext(0xd4e);
            }
            return IntervalType.GetIntervalType(ArrayUtil.Find(Tokens.SQL_INTERVAL_FIELD_CODES, num4), ArrayUtil.Find(Tokens.SQL_INTERVAL_FIELD_CODES, tokenType), (long) num, fractionPrecision);
        }

        public long ScanIntervalValue(IntervalType type)
        {
            byte[] yearToSecondSeparators = DTIType.YearToSecondSeparators;
            int[] yearToSecondFactors = DTIType.YearToSecondFactors;
            int[] yearToSecondLimits = DTIType.YearToSecondLimits;
            int startPartIndex = type.StartPartIndex;
            int endPartIndex = type.EndPartIndex;
            long num3 = 0L;
            int num4 = 0;
            int num5 = this._intervalPosition;
            int index = startPartIndex;
            int num7 = 0;
            while (index <= endPartIndex)
            {
                bool flag = false;
                if (num5 == this._intervalString.Length)
                {
                    if (index != endPartIndex)
                    {
                        throw Error.GetError(0xd4e);
                    }
                    flag = true;
                }
                else
                {
                    int num8 = this._intervalString[num5];
                    if ((num8 >= 0x30) && (num8 <= 0x39))
                    {
                        int num9 = num8 - 0x30;
                        num4 *= 10;
                        num4 += num9;
                        num7++;
                        num5++;
                    }
                    else if (num8 == yearToSecondSeparators[index])
                    {
                        flag = true;
                        if (index != endPartIndex)
                        {
                            num5++;
                        }
                    }
                    else
                    {
                        if (index != endPartIndex)
                        {
                            throw Error.GetError(0xd4e);
                        }
                        flag = true;
                    }
                }
                if (flag)
                {
                    if (index == startPartIndex)
                    {
                        if (!type.DefaultPrecision && (num7 > type.Precision))
                        {
                            throw Error.GetError(0xd6b);
                        }
                        if (num7 == 0)
                        {
                            throw Error.GetError(0xd4e);
                        }
                        int num10 = yearToSecondFactors[index];
                        num3 += num4 * num10;
                        this._intervalPrecision = num7;
                    }
                    else
                    {
                        if (num4 >= yearToSecondLimits[index])
                        {
                            throw Error.GetError(0xd6b);
                        }
                        num3 += num4 * yearToSecondFactors[index];
                    }
                    index++;
                    num4 = 0;
                    num7 = 0;
                    if (num5 == this._intervalString.Length)
                    {
                        break;
                    }
                }
            }
            this._intervalPosition = num5;
            return num3;
        }

        public void ScanNext()
        {
            if (this._currentPosition == this._limit)
            {
                this.ResetState();
                this.token.TokenType = 0x2ec;
            }
            else
            {
                this.ScanSeparator();
                if (this._currentPosition == this._limit)
                {
                    this.ResetState();
                    this.token.TokenType = 0x2ec;
                }
                else
                {
                    this.ScanToken();
                    if (this.token.IsMalformed)
                    {
                        this.token.FullString = this.GetPart(this._tokenPosition, this._currentPosition);
                    }
                }
            }
        }

        private void ScanNext(int error)
        {
            this.ScanNext();
            if (this.token.IsMalformed)
            {
                throw Error.GetError(error);
            }
        }

        public bool ScanNull()
        {
            this.ScanSeparator();
            int num = this.CharAt(this._currentPosition);
            if ((num != 0x4e) && (num != 110))
            {
                return false;
            }
            return this.ScanSpecialIdentifier("NULL");
        }

        private void ScanNumber()
        {
            bool flag = false;
            bool flag2 = false;
            int num = -1;
            this.token.TokenType = 0x2e9;
            this.token.DataType = SqlType.SqlInteger;
            int startIndex = this._currentPosition;
            while (this._currentPosition < this._limit)
            {
                bool flag3 = false;
                int num4 = this.CharAt(this._currentPosition);
                int num5 = num4;
                if (num5 > 0x54)
                {
                    goto Label_019E;
                }
                if (num5 > 0x4d)
                {
                    goto Label_0187;
                }
                switch (num5)
                {
                    case 0x2b:
                    case 0x2d:
                        if (num != (this._currentPosition - 1))
                        {
                            flag3 = true;
                        }
                        goto Label_01E9;

                    case 0x2c:
                    case 0x2f:
                    case 0x3a:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x40:
                    case 0x41:
                    case 0x42:
                    case 0x43:
                    case 0x44:
                    case 70:
                        goto Label_0263;

                    case 0x2e:
                        if (this.CharAt(this._currentPosition + 1) != 0x2e)
                        {
                            break;
                        }
                        flag3 = true;
                        goto Label_01E9;

                    case 0x30:
                    case 0x31:
                    case 50:
                    case 0x33:
                    case 0x34:
                    case 0x35:
                    case 0x36:
                    case 0x37:
                    case 0x38:
                    case 0x39:
                        flag = true;
                        goto Label_01E9;

                    case 0x45:
                        goto Label_0203;

                    case 0x47:
                        goto Label_0268;

                    default:
                        goto Label_016C;
                }
                this.token.DataType = SqlType.SqlNumeric;
                if (flag2 || (num != -1))
                {
                    this.token.TokenString = this._sqlString.Substring(startIndex, (this._currentPosition + 1) - startIndex);
                    this.token.TokenType = 0x2f2;
                    this.token.IsMalformed = true;
                    return;
                }
                flag2 = true;
                goto Label_01E9;
            Label_016C:
                switch (num5)
                {
                    case 0x4b:
                    case 0x4d:
                        goto Label_0268;

                    default:
                        goto Label_0263;
                }
            Label_0187:
                if ((num5 == 80) || (num5 == 0x54))
                {
                    goto Label_0268;
                }
                goto Label_0263;
            Label_019E:
                if (num5 <= 0x6d)
                {
                    switch (num5)
                    {
                        case 0x65:
                            goto Label_0203;

                        case 0x67:
                        case 0x6b:
                        case 0x6d:
                            goto Label_0268;
                    }
                }
                else
                {
                    switch (num5)
                    {
                        case 0x70:
                        case 0x74:
                            goto Label_0268;
                    }
                }
                goto Label_0263;
            Label_01E9:
                if (flag3)
                {
                    break;
                }
                this._currentPosition++;
                continue;
            Label_0203:
                this.token.DataType = SqlType.SqlDouble;
                if ((num != -1) || !flag)
                {
                    this.token.TokenString = this._sqlString.Substring(startIndex, (this._currentPosition + 1) - startIndex);
                    this.token.TokenType = 0x2f2;
                    this.token.IsMalformed = true;
                    return;
                }
                flag2 = true;
                num = this._currentPosition;
                goto Label_01E9;
            Label_0263:
                flag3 = true;
                goto Label_01E9;
            Label_0268:
                if (!flag | flag2)
                {
                    this.token.TokenType = 0x2f2;
                    this.token.IsMalformed = true;
                    return;
                }
                char ch = (char) num4;
                string token = ch.ToString().ToUpper();
                this.token.LobMultiplierType = Tokens.GetNonKeywordID(token, 0x2f2);
                if (this.token.LobMultiplierType == 0x2f2)
                {
                    this.token.TokenType = 0x2f2;
                    this.token.IsMalformed = true;
                    return;
                }
                try
                {
                    this.token.TokenValue = int.Parse(this._sqlString.Substring(startIndex, this._currentPosition - startIndex), this._culture);
                    this.token.TokenType = 0x2f0;
                    this._currentPosition++;
                    this.token.FullString = this.GetPart(this._tokenPosition, this._currentPosition);
                }
                catch (FormatException)
                {
                    this.token.TokenType = 0x2f2;
                    this.token.IsMalformed = true;
                }
                return;
            }
            this.token.TokenString = this._sqlString.Substring(startIndex, this._currentPosition - startIndex);
            int typeCode = this.token.DataType.TypeCode;
            switch (typeCode)
            {
                case 2:
                    break;

                case 3:
                    return;

                case 4:
                    if (this.token.TokenString.Length < 11)
                    {
                        try
                        {
                            this.token.TokenValue = int.Parse(this.token.TokenString, this._culture);
                            return;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (this.token.TokenString.Length < 20)
                    {
                        try
                        {
                            this.token.DataType = SqlType.SqlBigint;
                            this.token.TokenValue = long.Parse(this.token.TokenString, this._culture);
                            return;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    this.token.DataType = SqlType.SqlNumeric;
                    break;

                default:
                    if (typeCode == 8)
                    {
                        try
                        {
                            double num6 = double.Parse(this.token.TokenString, this._culture);
                            this.token.TokenValue = num6;
                        }
                        catch (Exception)
                        {
                            this.token.TokenType = 0x2f2;
                            this.token.IsMalformed = true;
                        }
                    }
                    return;
            }
            try
            {
                decimal num7 = decimal.Parse(this.token.TokenString, this._culture);
                this.token.TokenValue = num7;
                this.token.DataType = NumberType.GetNumberType(2, 0x7fffffffL, 0x7fff);
            }
            catch (Exception)
            {
                this.token.TokenType = 0x2f2;
                this.token.IsMalformed = true;
            }
        }

        private bool ScanSeparator()
        {
            bool flag = false;
            while (true)
            {
                bool flag2 = this.ScanWhitespace();
                flag |= flag2;
                if (!this.ScanCommentAsInlineSeparator())
                {
                    return flag;
                }
                flag = true;
                this._hasNonSpaceSeparator = true;
            }
        }

        public bool ScanSpecialIdentifier(string identifier)
        {
            int length = identifier.Length;
            if ((this._limit - this._currentPosition) < length)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                int num3 = identifier[i];
                if ((num3 != this._sqlString[this._currentPosition + i]) && (num3 != char.ToUpper(this._sqlString[this._currentPosition + i])))
                {
                    return false;
                }
            }
            this._currentPosition += length;
            return true;
        }

        public void ScanStringPart(char quoteChar)
        {
            this._currentPosition++;
            while (true)
            {
                int index = this._sqlString.IndexOf(quoteChar, this._currentPosition);
                if (index < 0)
                {
                    break;
                }
                if (this.CharAt(index + 1) != quoteChar)
                {
                    this._charWriter.Append(this._sqlString, this._currentPosition, index - this._currentPosition);
                    this._currentPosition = index + 1;
                    return;
                }
                index++;
                this._charWriter.Append(this._sqlString, this._currentPosition, index - this._currentPosition);
                this._currentPosition = index + 1;
            }
            this.token.TokenString = this._sqlString.Substring(this._currentPosition, this._limit - this._currentPosition);
            this.token.TokenType = (quoteChar == '\'') ? 0x2f1 : 0x2f7;
            this.token.IsMalformed = true;
        }

        private void ScanToken()
        {
            this.ResetState();
            this.token.TokenType = 0x2ea;
            int num = this.CharAt(this._currentPosition);
            if (num > 110)
            {
                switch (num)
                {
                    case 0x75:
                        goto Label_0A6D;

                    case 120:
                        goto Label_0942;
                }
                if (this.CharAt(this._currentPosition + 1) == 0x7c)
                {
                    this.token.TokenString = "||";
                    this.token.TokenType = 0x2ad;
                    this._currentPosition += 2;
                    this.token.IsDelimiter = true;
                    return;
                }
                this.token.TokenString = this._sqlString.Substring(this._currentPosition, (this._currentPosition + 2) - this._currentPosition);
                this.token.TokenType = -1;
                this.token.IsDelimiter = true;
                return;
            }
            switch (num)
            {
                case 0x21:
                    if (this.CharAt(this._currentPosition + 1) != 0x3d)
                    {
                        this.token.TokenString = this._sqlString.Substring(this._currentPosition, (this._currentPosition + 2) - this._currentPosition);
                        this.token.TokenType = -1;
                        this.token.IsDelimiter = true;
                        return;
                    }
                    this.token.TokenString = "<>";
                    this.token.TokenType = 0x2b6;
                    this._currentPosition += 2;
                    this.token.IsDelimiter = true;
                    return;

                case 0x22:
                    this.token.TokenType = 0x2eb;
                    goto Label_081D;

                case 0x25:
                    this.token.TokenString = "%";
                    this.token.TokenType = 0x330;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x27:
                    this.ScanCharacterString();
                    if (!this.token.IsMalformed)
                    {
                        this.token.DataType = CharacterType.GetCharacterType(12, (long) this.token.TokenString.Length);
                        this.token.TokenType = 0x2e9;
                        this.token.IsDelimiter = true;
                        return;
                    }
                    return;

                case 40:
                    this.token.TokenString = "(";
                    this.token.TokenType = 0x2b7;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x29:
                    this.token.TokenString = ")";
                    this.token.TokenType = 0x2aa;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x2a:
                    this.token.TokenString = "*";
                    this.token.TokenType = 0x2a9;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x2b:
                    this.token.TokenString = "+";
                    this.token.TokenType = 0x2b8;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x2c:
                    this.token.TokenString = ",";
                    this.token.TokenType = 0x2ac;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x2d:
                    this.token.TokenString = "-";
                    this.token.TokenType = 0x2b5;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x2e:
                    if (this.CharAt(this._currentPosition + 1) != 0x2e)
                    {
                        goto Label_0885;
                    }
                    this.token.TokenString = "..";
                    this.token.TokenType = 0x322;
                    this._currentPosition += 2;
                    this.token.IsDelimiter = true;
                    return;

                case 0x2f:
                    this.token.TokenString = "/";
                    this.token.TokenType = 0x2ae;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x30:
                case 0x31:
                case 50:
                case 0x33:
                case 0x34:
                case 0x35:
                case 0x36:
                case 0x37:
                case 0x38:
                case 0x39:
                    goto Label_0885;

                case 0x3a:
                    if (this.CharAt(this._currentPosition + 1) != 0x3d)
                    {
                        this.token.TokenString = ":";
                        this.token.TokenType = 0x2ab;
                        this._currentPosition++;
                        this.token.IsDelimiter = true;
                        return;
                    }
                    this._currentPosition += 2;
                    this.token.TokenString = ":=";
                    this.token.TokenType = 0x223;
                    this.token.IsDelimiter = true;
                    return;

                case 0x3b:
                    this.token.TokenString = ";";
                    this.token.TokenType = 0x2bb;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 60:
                    if (this.CharAt(this._currentPosition + 1) != 0x3e)
                    {
                        if (this.CharAt(this._currentPosition + 1) == 0x3d)
                        {
                            this.token.TokenString = "<=";
                            this.token.TokenType = 0x2b4;
                            this._currentPosition += 2;
                            this.token.IsDelimiter = true;
                            return;
                        }
                        this.token.TokenString = "<";
                        this.token.TokenType = 0x2b3;
                        this._currentPosition++;
                        this.token.IsDelimiter = true;
                        return;
                    }
                    this.token.TokenString = "<>";
                    this.token.TokenType = 0x2b6;
                    this._currentPosition += 2;
                    this.token.IsDelimiter = true;
                    return;

                case 0x3d:
                    this.token.TokenString = "=";
                    this.token.TokenType = 0x18c;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x3e:
                    if (this.CharAt(this._currentPosition + 1) != 0x3d)
                    {
                        this.token.TokenString = ">";
                        this.token.TokenType = 680;
                        this._currentPosition++;
                        this.token.IsDelimiter = true;
                        return;
                    }
                    this.token.TokenString = ">=";
                    this.token.TokenType = 690;
                    this._currentPosition += 2;
                    this.token.IsDelimiter = true;
                    return;

                case 0x3f:
                    if (this.CharAt(this._currentPosition + 1) != 0x3f)
                    {
                        break;
                    }
                    if (this.CharAt(this._currentPosition + 2) != 40)
                    {
                        if (this.CharAt(this._currentPosition + 2) == 0x29)
                        {
                            this.token.TokenString = ")";
                            this.token.TokenType = 0x2aa;
                            this._currentPosition += 3;
                            this.token.IsDelimiter = true;
                            return;
                        }
                        break;
                    }
                    this.token.TokenString = "(";
                    this.token.TokenType = 0x2b7;
                    this._currentPosition += 3;
                    this.token.IsDelimiter = true;
                    return;

                case 0x40:
                    this.token.IsHostParameter = true;
                    goto Label_081D;

                case 0x55:
                    goto Label_0A6D;

                case 0x58:
                    goto Label_0942;

                case 0x5b:
                    this.token.TokenString = "[";
                    this.token.TokenType = 820;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                case 0x5d:
                    this.token.TokenString = "]";
                    this.token.TokenType = 0x333;
                    this._currentPosition++;
                    this.token.IsDelimiter = true;
                    return;

                default:
                    goto Label_081D;
            }
            this.token.TokenString = "?";
            this.token.TokenType = 0x2b9;
            this._currentPosition++;
            this.token.IsDelimiter = true;
            return;
        Label_081D:
            this.CharAt(this._currentPosition + 1);
            this._currentPosition++;
            this.ScanCharacterString();
            if (!this.token.IsMalformed)
            {
                this.token.DataType = CharacterType.GetCharacterType(12, (long) this.token.TokenString.Length);
                this.token.TokenType = 0x2e9;
            }
            return;
        Label_0885:
            this.token.TokenType = 0x2e9;
            this.ScanNumber();
            return;
        Label_0942:
            if (this.CharAt(this._currentPosition + 1) == 0x27)
            {
                this._currentPosition++;
                this.ScanBinaryString();
                if (!this.token.IsMalformed)
                {
                    this.token.DataType = BinaryType.GetBinaryType(0x3d, ((BinaryData) this.token.TokenValue).Length(null));
                    this.token.TokenType = 0x2e9;
                }
                return;
            }
            this.ScanIdentifierChain();
            if (this.token.TokenType != 0x2ea)
            {
                if (this.token.TokenType == 0x2eb)
                {
                    this.token.IsDelimitedIdentifier = true;
                }
                return;
            }
            this.token.IsUndelimitedIdentifier = true;
            this.token.TokenType = Tokens.GetKeywordID(this.token.TokenString, 0x2ea);
            if (this.token.TokenType == 0x2ea)
            {
                this.token.TokenType = Tokens.GetNonKeywordID(this.token.TokenString, 0x2ea);
                return;
            }
            this.token.IsReservedIdentifier = true;
            this.token.IsCoreReservedIdentifier = Tokens.IsCoreKeyword(this.token.TokenType);
            return;
        Label_0A6D:
            if (this.CharAt(this._currentPosition + 1) == 0x26)
            {
                this.CharAt(this._currentPosition + 2);
            }
            this._currentPosition += 2;
            this.token.DataType = SqlType.SqlChar;
            this.token.TokenType = 0x2e9;
            this.ScanUnicodeString();
            if (!this.token.IsMalformed)
            {
                this.token.DataType = CharacterType.GetCharacterType(12, (long) ((string) this.token.TokenValue).Length);
            }
        }

        public bool ScanUndelimitedIdentifier()
        {
            if (this._currentPosition == this._limit)
            {
                return false;
            }
            char c = this._sqlString[this._currentPosition];
            if ((!char.IsLetter(c) && (c != '_')) && (!this.token.IsHostParameter || (c != '@')))
            {
                this.token.TokenString = c.ToString();
                this.token.TokenType = -1;
                this.token.IsMalformed = true;
                return false;
            }
            int num = this._currentPosition + 1;
            while (num < this._limit)
            {
                char ch2 = this._sqlString[num];
                if ((ch2 != '_') && !char.IsLetterOrDigit(ch2))
                {
                    break;
                }
                num++;
            }
            this.token.TokenString = this._sqlString.Substring(this._currentPosition, num - this._currentPosition).ToUpper();
            this._currentPosition = num;
            if (!this._nullAndBooleanAsValue)
            {
                goto Label_024A;
            }
            int num2 = this._currentPosition - this._tokenPosition;
            if ((num2 != 4) && (num2 != 5))
            {
                goto Label_024A;
            }
            char ch3 = c;
            if (ch3 <= 'T')
            {
                switch (ch3)
                {
                    case 'F':
                        goto Label_01A3;

                    case 'N':
                        goto Label_0203;
                }
                if (ch3 != 'T')
                {
                    return true;
                }
            }
            else
            {
                switch (ch3)
                {
                    case 'f':
                        goto Label_01A3;

                    case 'n':
                        goto Label_0203;

                    case 't':
                        goto Label_0143;
                }
                return true;
            }
        Label_0143:
            if ("TRUE".Equals(this.token.TokenString))
            {
                this.token.TokenString = "TRUE";
                this.token.TokenType = 0x2e9;
                this.token.TokenValue = true;
                this.token.DataType = SqlType.SqlBoolean;
                return false;
            }
            return true;
        Label_01A3:
            if ("FALSE".Equals(this.token.TokenString))
            {
                this.token.TokenString = "FALSE";
                this.token.TokenType = 0x2e9;
                this.token.TokenValue = false;
                this.token.DataType = SqlType.SqlBoolean;
                return false;
            }
            return true;
        Label_0203:
            if ("NULL".Equals(this.token.TokenString))
            {
                this.token.TokenString = "NULL";
                this.token.TokenType = 0x2e9;
                this.token.TokenValue = null;
                return false;
            }
        Label_024A:
            return true;
        }

        private bool ScanUnicodeIdentifier()
        {
            int escape = 0x5c;
            this._charWriter.Length = 0;
            this.ScanStringPart('"');
            if (this.token.IsMalformed)
            {
                return false;
            }
            this.token.TokenString = this._charWriter.ToString();
            this.ScanSeparator();
            int num2 = this.CharAt(this._currentPosition);
            if (((num2 == 0x75) || (num2 == 0x55)) && this.ScanSpecialIdentifier("UESCAPE"))
            {
                this.ScanSeparator();
                escape = this.ScanEscapeDefinition();
                if (escape == -1)
                {
                    this.token.TokenType = 760;
                    this.token.IsMalformed = true;
                    return false;
                }
            }
            this.ConvertUnicodeString(escape);
            this.token.TokenString = (string) this.token.TokenValue;
            return !this.token.IsMalformed;
        }

        private void ScanUnicodeString()
        {
            int escape = 0x5c;
            this.ScanCharacterString();
            this.ScanSeparator();
            int num2 = this.CharAt(this._currentPosition);
            if (((num2 == 0x75) || (num2 == 0x55)) && this.ScanSpecialIdentifier("UESCAPE"))
            {
                this.ScanSeparator();
                escape = this.ScanEscapeDefinition();
                if (escape == -1)
                {
                    this.token.TokenType = 760;
                    this.token.IsMalformed = true;
                    return;
                }
            }
            this.ConvertUnicodeString(escape);
        }

        public bool ScanWhitespace()
        {
            bool flag = false;
            while (this._currentPosition < this._limit)
            {
                char c = this._sqlString[this._currentPosition];
                if (!char.IsWhiteSpace(c))
                {
                    return flag;
                }
                if (c == ' ')
                {
                    flag = true;
                }
                else
                {
                    this._hasNonSpaceSeparator = true;
                    flag = true;
                    this.SetLineNumber(c);
                }
                this._currentPosition++;
            }
            return flag;
        }

        private void SetLineNumber(int c)
        {
            if ((c == 13) || (c == 10))
            {
                if (this._currentPosition == (this._eolPosition + 1))
                {
                    if ((c != 10) || (this._eolCode == c))
                    {
                        this._lineNumber++;
                    }
                }
                else
                {
                    this._lineNumber++;
                }
                this._eolPosition = this._currentPosition;
                this._eolCode = c;
            }
        }

        public void SetNullAndBooleanAsValue()
        {
            this._nullAndBooleanAsValue = true;
        }

        private bool ShiftPrefixes()
        {
            if (this.token.NamePrePrePrefix != null)
            {
                return false;
            }
            this.token.NamePrePrePrefix = this.token.NamePrePrefix;
            this.token.IsDelimitedPrePrePrefix = this.token.IsDelimitedPrePrefix;
            this.token.NamePrePrefix = this.token.NamePrefix;
            this.token.IsDelimitedPrePrefix = this.token.IsDelimitedPrefix;
            this.token.NamePrefix = this.token.TokenString;
            this.token.IsDelimitedPrefix = this.token.TokenType == 0x2eb;
            return true;
        }
    }
}

