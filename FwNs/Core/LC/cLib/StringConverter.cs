namespace FwNs.Core.LC.cLib
{
    using FwNs.Core.LC.cLib.IO;
    using System;
    using System.Text;

    public sealed class StringConverter
    {
        private static readonly byte[] Hexbytes = new byte[] { 0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x61, 0x62, 0x63, 100, 0x65, 0x66 };

        public static string ByteArrayToHexString(byte[] b)
        {
            int length = b.Length;
            StringBuilder builder = new StringBuilder(b.Length * 2);
            for (int i = 0; i < length; i++)
            {
                int num3 = b[i] & 0xff;
                builder.Append((char) Hexbytes[(num3 >> 4) & 15]);
                builder.Append((char) Hexbytes[num3 & 15]);
            }
            return builder.ToString();
        }

        public static string ByteArrayToSqlHexString(byte[] b)
        {
            int length = b.Length;
            char[] chArray = new char[(length * 2) + 3];
            chArray[0] = 'X';
            chArray[1] = '\'';
            int index = 2;
            for (int i = 0; i < length; i++)
            {
                int num4 = b[i] & 0xff;
                chArray[index++] = (char) Hexbytes[(num4 >> 4) & 15];
                chArray[index++] = (char) Hexbytes[num4 & 15];
            }
            chArray[index] = '\'';
            return new string(chArray);
        }

        public static string ByteToHex(byte[] b)
        {
            int length = b.Length;
            char[] chArray = new char[length * 2];
            int index = 0;
            int num3 = 0;
            while (index < length)
            {
                int num4 = b[index] & 0xff;
                chArray[num3++] = (char) Hexbytes[(num4 >> 4) & 15];
                chArray[num3++] = (char) Hexbytes[num4 & 15];
                index++;
            }
            return new string(chArray);
        }

        private static int Count(string s, char c)
        {
            int startIndex = 0;
            int num2 = 0;
            if (s != null)
            {
                while ((startIndex = s.IndexOf(c, startIndex)) > -1)
                {
                    num2++;
                    startIndex++;
                }
            }
            return num2;
        }

        private static int GetNibble(int value)
        {
            if ((value >= 0x30) && (value <= 0x39))
            {
                return (value - 0x30);
            }
            if ((value >= 0x61) && (value <= 0x66))
            {
                return ((10 + value) - 0x61);
            }
            if ((value >= 0x41) && (value <= 70))
            {
                return ((10 + value) - 0x41);
            }
            return -1;
        }

        public static int GetUtfSize(string s)
        {
            return Encoding.UTF8.GetByteCount(s);
        }

        public static string ReadUtf(byte[] bytearr, int offset, int length)
        {
            return Encoding.UTF8.GetString(bytearr, offset, length);
        }

        public static void StringToUnicodeBytes(ByteArrayOutputStream b, string s, bool doubleSingleQuotes)
        {
            if (s != null)
            {
                int length = s.Length;
                int num2 = 0;
                if (length != 0)
                {
                    char[] chArray = s.ToCharArray();
                    b.EnsureRoom((length * 2) + 5);
                    for (int i = 0; i < length; i++)
                    {
                        char ch = chArray[i];
                        if (ch == '\\')
                        {
                            if ((i < (length - 1)) && (chArray[i + 1] == 'u'))
                            {
                                b.WriteNoCheck(ch);
                                b.WriteNoCheck(0x75);
                                b.WriteNoCheck(0x30);
                                b.WriteNoCheck(0x30);
                                b.WriteNoCheck(0x35);
                                b.WriteNoCheck(0x63);
                                num2 += 5;
                            }
                            else
                            {
                                b.Write(ch);
                            }
                        }
                        else if ((ch >= ' ') && (ch <= '\x007f'))
                        {
                            b.WriteNoCheck(ch);
                            if ((ch == '\'') & doubleSingleQuotes)
                            {
                                b.WriteNoCheck(ch);
                                num2++;
                            }
                        }
                        else
                        {
                            b.WriteNoCheck(0x5c);
                            b.WriteNoCheck(0x75);
                            b.WriteNoCheck(Hexbytes[(ch >> 12) & '\x000f']);
                            b.WriteNoCheck(Hexbytes[(ch >> 8) & '\x000f']);
                            b.WriteNoCheck(Hexbytes[(ch >> 4) & '\x000f']);
                            b.WriteNoCheck(Hexbytes[ch & '\x000f']);
                            num2 += 5;
                        }
                        if (num2 > length)
                        {
                            b.EnsureRoom((length + num2) + 5);
                            num2 = 0;
                        }
                    }
                }
            }
        }

        public static int StringToUtfBytes(string str, ByteArrayOutputStream o)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            o.EnsureRoom(bytes.Length);
            o.Write(bytes);
            return bytes.Length;
        }

        public static string ToQuotedString(string s, char quoteChar, bool extraQuote)
        {
            if (s == null)
            {
                return null;
            }
            int num = extraQuote ? Count(s, quoteChar) : 0;
            int length = s.Length;
            char[] chArray = new char[(2 + num) + length];
            int num3 = 0;
            int index = 0;
            chArray[index++] = quoteChar;
            while (num3 < length)
            {
                char ch = s[num3];
                chArray[index++] = ch;
                if (extraQuote && (ch == quoteChar))
                {
                    chArray[index++] = ch;
                }
                num3++;
            }
            chArray[index] = quoteChar;
            return new string(chArray);
        }

        public static string UnicodeStringToString(string s)
        {
            if ((s == null) || !s.Contains(@"\u"))
            {
                return s;
            }
            int length = s.Length;
            char[] chArray = new char[length];
            int num2 = 0;
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                if ((ch == '\\') && (i < (length - 5)))
                {
                    if (s[i + 1] == 'u')
                    {
                        i++;
                        int num4 = GetNibble(s[++i]) << 12;
                        num4 += GetNibble(s[++i]) << 8;
                        num4 += GetNibble(s[++i]) << 4;
                        num4 += GetNibble(s[++i]);
                        chArray[num2++] = (char) num4;
                    }
                    else
                    {
                        chArray[num2++] = ch;
                    }
                }
                else
                {
                    chArray[num2++] = ch;
                }
            }
            return new string(chArray, 0, num2);
        }

        public static void WriteHexBytes(byte[] o, int from, byte[] b)
        {
            int length = b.Length;
            for (int i = 0; i < length; i++)
            {
                int num3 = b[i] & 0xff;
                o[from++] = Hexbytes[(num3 >> 4) & 15];
                o[from++] = Hexbytes[num3 & 15];
            }
        }
    }
}

