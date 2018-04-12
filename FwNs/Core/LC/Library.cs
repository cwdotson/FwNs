namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cPersist;
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Library
    {
        private Library()
        {
        }

        public static string Character(int code)
        {
            return Convert.ToChar(code).ToString();
        }

        public static int Difference(string s1, string s2)
        {
            if ((s1 == null) || (s2 == null))
            {
                return 0;
            }
            s1 = Soundex(s1);
            s2 = Soundex(s2);
            int num2 = 0;
            for (int i = 0; i < 4; i++)
            {
                if (s1[i] != s2[i])
                {
                    num2++;
                }
            }
            return num2;
        }

        public static string GetDatabaseFullProductVersion()
        {
            return LibCoreDatabaseProperties.ThisFullVersion;
        }

        public static int GetDatabaseMajorVersion()
        {
            return 1;
        }

        public static int GetDatabaseMinorVersion()
        {
            return 9;
        }

        public static string GetDatabaseProductName()
        {
            return "LibCore Database Engine";
        }

        public static string GetDatabaseProductVersion()
        {
            return LibCoreDatabaseProperties.ThisVersion;
        }

        private static RegexOptions GetRegExpFlag(string flags)
        {
            RegexOptions none = RegexOptions.None;
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i] == 'i')
                {
                    none |= RegexOptions.IgnoreCase;
                }
                else if (flags[i] == 'c')
                {
                    none &= ~RegexOptions.IgnoreCase;
                }
                else if (flags[i] == 'n')
                {
                    none |= RegexOptions.Singleline;
                }
                else if (flags[i] == 'm')
                {
                    none |= RegexOptions.Multiline;
                }
            }
            return none;
        }

        public static int Locate(string s, string search, int? start, int? occur)
        {
            int? nullable;
            if ((s == null) || (search == null))
            {
                return 0;
            }
            int num2 = !start.HasValue ? 0 : (start.Value - 1);
            int num3 = 0;
            int num4 = 0;
        Label_003E:
            nullable = occur;
            if ((num4 < nullable.GetValueOrDefault()) ? nullable.HasValue : false)
            {
                num3 = s.IndexOf(search, (num2 < 0) ? 0 : num2) + 1;
                num2 = num3;
                num4++;
                goto Label_003E;
            }
            return num3;
        }

        public static int RegExp_Instr(string s, string pattern, int pos, int occurr, int retOpt, string flags)
        {
            RegexOptions regExpFlag = GetRegExpFlag(flags);
            Regex regex = new Regex(pattern, regExpFlag);
            Match match = null;
            if (occurr <= 1)
            {
                match = regex.Match(s, (int) (pos - 1));
            }
            else
            {
                foreach (Match match2 in regex.Matches(s, (int) (pos - 1)))
                {
                    occurr--;
                    if (occurr == 0)
                    {
                        match = match2;
                        break;
                    }
                }
            }
            if ((match == null) || !match.Success)
            {
                return 0;
            }
            return (match.Index + 1);
        }

        public static string RegExp_Replace(string s, string pattern, string replace, int pos, int occurr, string flags)
        {
            RegexOptions regExpFlag = GetRegExpFlag(flags);
            Regex regex = new Regex(pattern, regExpFlag);
            if (occurr > 1)
            {
                pos = RegExp_Instr(s, pattern, pos, occurr, 0, flags);
            }
            if (pos <= 0)
            {
                return s;
            }
            int count = 1;
            if (occurr == 0)
            {
                count = 0x7fff;
            }
            return regex.Replace(s, replace, count, (int) (pos - 1));
        }

        public static string RegExp_Substr(string s, string pattern, int pos, int occurr, string flags)
        {
            RegexOptions regExpFlag = GetRegExpFlag(flags);
            Regex regex = new Regex(pattern, regExpFlag);
            Match match = null;
            if (occurr <= 1)
            {
                match = regex.Match(s, (int) (pos - 1));
            }
            else
            {
                foreach (Match match2 in regex.Matches(s, (int) (pos - 1)))
                {
                    occurr--;
                    if (occurr == 0)
                    {
                        match = match2;
                        break;
                    }
                }
            }
            if ((match == null) || !match.Success)
            {
                return null;
            }
            return match.Value;
        }

        public static string Repeat(string s, int? count)
        {
            if ((s != null) && count.HasValue)
            {
                int? nullable = count;
                int num = 0;
                if (!((nullable.GetValueOrDefault() < num) ? nullable.HasValue : false))
                {
                    int num2 = count.Value;
                    StringBuilder builder = new StringBuilder(s.Length * num2);
                    while (num2-- > 0)
                    {
                        builder.Append(s);
                    }
                    return builder.ToString();
                }
            }
            return null;
        }

        public static string Replace(string s, string replace, string with)
        {
            if ((s == null) || (replace == null))
            {
                return s;
            }
            if (with == null)
            {
                with = "";
            }
            StringBuilder builder = new StringBuilder();
            int startIndex = 0;
            int length = replace.Length;
            while (true)
            {
                int index = s.IndexOf(replace, startIndex);
                if (index == -1)
                {
                    break;
                }
                builder.Append(s.Substring(startIndex, index - startIndex));
                builder.Append(with);
                startIndex = index + length;
            }
            builder.Append(s.Substring(startIndex));
            return builder.ToString();
        }

        public static double Round(double d, int p)
        {
            if (p < 0)
            {
                double num2 = Math.Pow(10.0, -((double) p));
                return (Math.Round((double) (d / num2), 0) * num2);
            }
            return Math.Round(d, p);
        }

        public static string Soundex(string s)
        {
            if (s == null)
            {
                return s;
            }
            s = s.ToUpper(CultureInfo.InvariantCulture);
            int length = s.Length;
            char[] chArray = new char[] { '0', '0', '0', '0' };
            char ch = '0';
            int num2 = 0;
            int num3 = 0;
            while ((num2 < length) && (num3 < 4))
            {
                char ch3;
                char ch2 = s[num2];
                if ("AEIOUY".IndexOf(ch2) != -1)
                {
                    ch3 = '7';
                    goto Label_00E0;
                }
                if ((ch2 == 'H') || (ch2 == 'W'))
                {
                    ch3 = '8';
                    goto Label_00E0;
                }
                if ("BFPV".IndexOf(ch2) != -1)
                {
                    ch3 = '1';
                    goto Label_00E0;
                }
                if ("CGJKQSXZ".IndexOf(ch2) != -1)
                {
                    ch3 = '2';
                    goto Label_00E0;
                }
                if ((ch2 == 'D') || (ch2 == 'T'))
                {
                    ch3 = '3';
                    goto Label_00E0;
                }
                if (ch2 == 'L')
                {
                    ch3 = '4';
                    goto Label_00E0;
                }
                if ((ch2 == 'M') || (ch2 == 'N'))
                {
                    ch3 = '5';
                    goto Label_00E0;
                }
                if (ch2 == 'R')
                {
                    ch3 = '6';
                    goto Label_00E0;
                }
            Label_00D8:
                num2++;
                continue;
            Label_00E0:
                switch (num3)
                {
                    case 0:
                        chArray[num3++] = ch2;
                        ch = ch3;
                        break;
                }
                goto Label_00D8;
            }
            return new string(chArray, 0, 4);
        }

        public static string Translate(string s, string strIf, string strElse)
        {
            StringBuilder builder = new StringBuilder();
            int length = strElse.Length;
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                int index = strIf.IndexOf(ch);
                if ((index != -1) && (index < length))
                {
                    builder.Append(strElse[index]);
                }
                else if (index == -1)
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }
    }
}

