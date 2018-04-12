namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class StringUtil
    {
        public static string ArrayToString(Array array)
        {
            int length = array.Length;
            int num2 = length - 1;
            StringBuilder builder = new StringBuilder(2 * (length + 1));
            builder.Append('{');
            for (int i = 0; i < length; i++)
            {
                builder.Append(array.GetValue(i));
                if (i != num2)
                {
                    builder.Append(',');
                }
            }
            builder.Append('}');
            return builder.ToString();
        }

        public static string GetList(int[] s, string separator, string quote)
        {
            int length = s.Length;
            StringBuilder builder = new StringBuilder(length * 8);
            for (int i = 0; i < length; i++)
            {
                builder.Append(quote);
                builder.Append(s[i]);
                builder.Append(quote);
                if ((i + 1) < length)
                {
                    builder.Append(separator);
                }
            }
            return builder.ToString();
        }

        public static int RightTrimSize(string s)
        {
            int length = s.Length;
            while (length > 0)
            {
                length--;
                if (s[length] != ' ')
                {
                    return (length + 1);
                }
            }
            return 0;
        }

        public static int RTrimSize(string s)
        {
            int length = s.Length;
            while (length > 0)
            {
                length--;
                if (s[length] != ' ')
                {
                    return (length + 1);
                }
            }
            return 0;
        }

        public static int SkipSpaces(string s, int start)
        {
            int length = s.Length;
            int num2 = start;
            while ((num2 < length) && (s[num2] == ' '))
            {
                num2++;
            }
            return num2;
        }

        public static string[] Split(string s, string separator)
        {
            List<string> list = new List<string>();
            int startIndex = 0;
            bool flag = true;
            while (flag)
            {
                int index = s.IndexOf(separator, startIndex);
                if (index == -1)
                {
                    index = s.Length;
                    flag = false;
                }
                list.Add(s.Substring(startIndex, index - startIndex));
                startIndex = index + separator.Length;
            }
            return list.ToArray();
        }

        public static string ToPaddedString(string source, int length, char pad, bool trailing)
        {
            int num = source.Length;
            if (num >= length)
            {
                return source;
            }
            StringBuilder builder = new StringBuilder(length);
            if (trailing)
            {
                builder.Append(source);
            }
            for (int i = num; i < length; i++)
            {
                builder.Append(pad);
            }
            if (!trailing)
            {
                builder.Append(source);
            }
            return builder.ToString();
        }

        public static string ToZeroPaddedString(long value, int precision, int maxSize)
        {
            StringBuilder builder = new StringBuilder();
            if (value < 0L)
            {
                value = -value;
            }
            string str = value.ToString();
            if (str.Length > precision)
            {
                str = str.Substring(precision);
            }
            for (int i = str.Length; i < precision; i++)
            {
                builder.Append('0');
            }
            builder.Append(str);
            if (maxSize < precision)
            {
                builder.Length = maxSize;
            }
            return builder.ToString();
        }
    }
}

