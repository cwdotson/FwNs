namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Text;

    public sealed class Like
    {
        private const int UnderscoreChar = 1;
        private const int PercentChar = 2;
        private char[] _cLike;
        private int _escapeChar;
        private int _iFirstWildCard;
        private int _iLen;
        private bool _isIgnoreCase;
        private bool _isNull;
        private int[] _wildCardType;
        public SqlType DataType;
        public bool HasCollation;
        public bool IsBinary;
        public bool IsVariable = true;

        public object Compare(Session session, object o)
        {
            if ((o == null) || this._isNull)
            {
                return null;
            }
            return this.CompareCore(session, o);
        }

        private bool CompareAt(object o, int i, int j, int iLen, int jLen, char[] cLike, int[] wildCardType)
        {
            while (i < iLen)
            {
                switch (wildCardType[i])
                {
                    case 0:
                        if ((j < jLen) && (cLike[i] == this.GetChar(o, j++)))
                        {
                            goto Label_007C;
                        }
                        return false;

                    case 1:
                        if (j++ < jLen)
                        {
                            goto Label_007C;
                        }
                        return false;

                    case 2:
                        if (++i < iLen)
                        {
                            goto Label_0075;
                        }
                        return true;

                    default:
                        goto Label_007C;
                }
            Label_004D:
                if ((cLike[i] == this.GetChar(o, j)) && this.CompareAt(o, i, j, iLen, jLen, cLike, wildCardType))
                {
                    return true;
                }
                j++;
            Label_0075:
                if (j < jLen)
                {
                    goto Label_004D;
                }
                return false;
            Label_007C:
                i++;
            }
            return (j == jLen);
        }

        private bool CompareAt(string o, int i, int j, int iLen, int jLen, char[] cLike, int[] wildCardType)
        {
            while (i < iLen)
            {
                switch (wildCardType[i])
                {
                    case 0:
                        if ((j < jLen) && (cLike[i] == o[j++]))
                        {
                            goto Label_007A;
                        }
                        return false;

                    case 1:
                        if (j++ < jLen)
                        {
                            goto Label_007A;
                        }
                        return false;

                    case 2:
                        if (++i < iLen)
                        {
                            goto Label_0073;
                        }
                        return true;

                    default:
                        goto Label_007A;
                }
            Label_004C:
                if ((cLike[i] == o[j]) && this.CompareAt(o, i, j, iLen, jLen, cLike, wildCardType))
                {
                    return true;
                }
                j++;
            Label_0073:
                if (j < jLen)
                {
                    goto Label_004C;
                }
                return false;
            Label_007A:
                i++;
            }
            return (j == jLen);
        }

        public bool CompareBool(Session session, object o)
        {
            return (((o != null) && !this._isNull) && this.CompareCore(session, o));
        }

        private bool CompareCore(Session session, object o)
        {
            if (this._isIgnoreCase && !this.IsBinary)
            {
                o = ((CharacterType) this.DataType).Upper(session, o);
            }
            if (this.IsBinary)
            {
                if (this.DataType.IsLobType())
                {
                    o = SqlType.SqlVarbinaryDefault.ConvertToType(session, o, this.DataType);
                }
                return this.CompareAt(o, 0, 0, this._iLen, this.GetLength(session, o), this._cLike, this._wildCardType);
            }
            if (this.DataType.IsLobType())
            {
                o = SqlType.SqlVarcharDefault.ConvertToType(session, o, this.DataType);
            }
            string str = (string) o;
            return this.CompareAt(str, 0, 0, this._iLen, str.Length, this._cLike, this._wildCardType);
        }

        public string Describe(Session session)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append(base.ToString()).Append("[\n");
            builder1.Append("escapeChar=").Append(this._escapeChar).Append('\n');
            builder1.Append("isNull=").Append(this._isNull).Append('\n');
            builder1.Append("isIgnoreCase=").Append(this._isIgnoreCase).Append('\n');
            builder1.Append("iLen=").Append(this._iLen).Append('\n');
            builder1.Append("iFirstWildCard=").Append(this._iFirstWildCard).Append('\n');
            builder1.Append("cLike=");
            builder1.Append(StringUtil.ArrayToString(this._cLike));
            builder1.Append('\n');
            builder1.Append("wildCardType=");
            builder1.Append(StringUtil.ArrayToString(this._wildCardType));
            builder1.Append(']');
            return builder1.ToString();
        }

        private char GetChar(object o, int i)
        {
            if (this.IsBinary)
            {
                return (char) ((BinaryData) o).GetBytes()[i];
            }
            return ((string) o)[i];
        }

        private int GetLength(ISessionInterface session, object o)
        {
            if (this.IsBinary)
            {
                return (int) ((BinaryData) o).Length(session);
            }
            return ((string) o).Length;
        }

        public void SetIgnoreCase(bool flag)
        {
            this._isIgnoreCase = flag;
        }

        public void SetPattern(Session session, object pattern, object escape, bool hasEscape)
        {
            this._isNull = pattern == null;
            if (!hasEscape)
            {
                this._escapeChar = -1;
            }
            else
            {
                if (escape == null)
                {
                    this._isNull = true;
                    return;
                }
                if (this.GetLength(session, escape) != 1)
                {
                    if (this.IsBinary)
                    {
                        throw Error.GetError(0xd54);
                    }
                    throw Error.GetError(0xd6f);
                }
                this._escapeChar = this.GetChar(escape, 0);
            }
            if (!this._isNull)
            {
                if (this._isIgnoreCase)
                {
                    pattern = ((CharacterType) this.DataType).Upper(null, pattern);
                }
                this._iLen = 0;
                this._iFirstWildCard = -1;
                int length = this.GetLength(session, pattern);
                this._cLike = new char[length];
                this._wildCardType = new int[length];
                bool flag = false;
                bool flag2 = false;
                int i = 0;
                while (i < length)
                {
                    int num3;
                    char ch = this.GetChar(pattern, i);
                    if (!flag)
                    {
                        if (this._escapeChar == ch)
                        {
                            flag = true;
                            goto Label_015E;
                        }
                        if (ch == '_')
                        {
                            this._wildCardType[this._iLen] = 1;
                            if (this._iFirstWildCard == -1)
                            {
                                this._iFirstWildCard = this._iLen;
                            }
                        }
                        else if (ch != '%')
                        {
                            flag2 = false;
                        }
                        else
                        {
                            if (flag2)
                            {
                                goto Label_015E;
                            }
                            flag2 = true;
                            this._wildCardType[this._iLen] = 2;
                            if (this._iFirstWildCard == -1)
                            {
                                this._iFirstWildCard = this._iLen;
                            }
                        }
                    }
                    else
                    {
                        if (((ch != this._escapeChar) && (ch != '_')) && (ch != '%'))
                        {
                            throw Error.GetError(0xd82);
                        }
                        flag2 = false;
                        flag = false;
                    }
                    goto Label_0164;
                Label_015E:
                    i++;
                    continue;
                Label_0164:
                    num3 = this._iLen;
                    this._iLen = num3 + 1;
                    this._cLike[num3] = ch;
                    goto Label_015E;
                }
                if (flag)
                {
                    throw Error.GetError(0xd82);
                }
                for (int j = 0; j < (this._iLen - 1); j++)
                {
                    if ((this._wildCardType[j] == 2) && (this._wildCardType[j + 1] == 1))
                    {
                        this._wildCardType[j] = 1;
                        this._wildCardType[j + 1] = 2;
                    }
                }
            }
        }
    }
}

