namespace FwNs.Txt.JSon
{
    using System;
    using System.Text;

    public static class Frmttr
    {
        public static string Indent = "   ";

        public static void AppendIndent(StringBuilder sb, int count)
        {
            while (count > 0)
            {
                sb.Append(Indent);
                count--;
            }
        }

        public static bool IsEscaped(StringBuilder sb, int index)
        {
            bool flag = false;
            while ((index > 0) && (sb[--index] == '\\'))
            {
                flag = !flag;
            }
            return flag;
        }

        public static string PrettyPrint(string input)
        {
            StringBuilder sb = new StringBuilder(input.Length * 2);
            char? nullable = null;
            int count = 0;
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch == '"')
                {
                    bool flag = true;
                    while (flag)
                    {
                        sb.Append(ch);
                        ch = input[++i];
                        switch (ch)
                        {
                            case '\\':
                            {
                                if (input[i + 1] == '"')
                                {
                                    sb.Append(ch);
                                    ch = input[++i];
                                }
                                continue;
                            }
                            case '"':
                                flag = false;
                                break;
                        }
                    }
                }
                switch (ch)
                {
                    case ',':
                    {
                        sb.Append(ch);
                        if (!nullable.HasValue)
                        {
                            sb.AppendLine();
                            AppendIndent(sb, count);
                        }
                        continue;
                    }
                    case ':':
                    {
                        if (nullable.HasValue)
                        {
                            sb.Append(ch);
                        }
                        else
                        {
                            sb.Append(" : ");
                        }
                        continue;
                    }
                    case '[':
                    case '{':
                    {
                        sb.Append(ch);
                        if (!nullable.HasValue)
                        {
                            sb.AppendLine();
                            AppendIndent(sb, ++count);
                        }
                        continue;
                    }
                    case ']':
                    case '}':
                    {
                        if (nullable.HasValue)
                        {
                            sb.Append(ch);
                        }
                        else
                        {
                            sb.AppendLine();
                            AppendIndent(sb, --count);
                            sb.Append(ch);
                        }
                        continue;
                    }
                }
                if (nullable.HasValue || !char.IsWhiteSpace(ch))
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }
    }
}

