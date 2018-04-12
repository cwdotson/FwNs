namespace FwNs.Core.LC.cErrors
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cResults;
    using System;
    using System.Text;

    public class Error
    {
        public const bool TraceSystemOut = false;
        private const string ErrPropsName = "sql-state-messages";
        private const string MessageTag = "$$";
        private const int SqlStateDigits = 5;
        private const int SqlCodeDigits = 4;
        private const int ErrorCodeBase = 11;
        private static readonly int BundleHandle = BundleHandler.GetBundleHandle("sql-state-messages", null);

        public static CoreException GetError(Result result)
        {
            return new CoreException(result);
        }

        public static CoreException GetError(int code)
        {
            return GetError(null, code, 0, null);
        }

        public static CoreException GetError(int code, Exception t)
        {
            string str = GetMessage(code, 0, null);
            return new CoreException(t, str.Substring(0, 5), -code);
        }

        public static CoreException GetError(int code, int code2)
        {
            return GetError(code, GetMessage(code2));
        }

        public static CoreException GetError(int code, string add)
        {
            string message = GetMessage(code);
            if (add != null)
            {
                message = message + ": " + add;
            }
            return new CoreException(null, message.Substring(6), message.Substring(0, 5), -code);
        }

        public static CoreException GetError(string message, string sqlState, int i)
        {
            return new CoreException(null, message, sqlState, i);
        }

        public static CoreException GetError(Exception t, int code, int subCode, object[] add)
        {
            string str = GetMessage(code, subCode, add);
            int num = (subCode < 11) ? code : subCode;
            if (t != null)
            {
                return new CoreException(t, str.Substring(6), str.Substring(0, 5), -num);
            }
            return new CoreException(str.Substring(6), str.Substring(0, 5), -num);
        }

        public static string GetMessage(int errorCode)
        {
            return GetResourceString(errorCode);
        }

        public static string GetMessage(int code, int subCode, object[] add)
        {
            string resourceString = GetResourceString(code);
            if (subCode != 0)
            {
                resourceString = resourceString + GetResourceString(subCode);
            }
            if (add != null)
            {
                resourceString = InsertStrings(resourceString, add);
            }
            return resourceString;
        }

        public static CoreException GetParseError(int code, string add, int lineNumber)
        {
            string message = GetMessage(code);
            if (add != null)
            {
                message = message + ": " + add;
            }
            if (lineNumber > 1)
            {
                add = GetMessage(0x18);
                object[] objArray1 = new object[] { message, " :", add, lineNumber };
                message = string.Concat(objArray1);
            }
            return new CoreException(null, message.Substring(6), message.Substring(0, 5), -code);
        }

        public static CoreException GetParseError(int code, int subCode, int lineNumber, object[] add)
        {
            string str = GetMessage(code, subCode, add);
            if (lineNumber > 1)
            {
                string message = GetMessage(0x18);
                object[] objArray1 = new object[] { str, " :", message, lineNumber };
                str = string.Concat(objArray1);
            }
            int num = (subCode < 11) ? code : subCode;
            return new CoreException(null, str.Substring(6), str.Substring(0, 5), -num);
        }

        private static string GetResourceString(int code)
        {
            string str = StringUtil.ToZeroPaddedString((long) code, 4, 4);
            return BundleHandler.GetString(BundleHandle, "E" + str);
        }

        public static string GetStateString(int errorCode)
        {
            return GetMessage(errorCode, 0, null).Substring(0, 5);
        }

        private static string InsertStrings(string message, object[] add)
        {
            StringBuilder builder = new StringBuilder(message.Length + 0x20);
            int startIndex = 0;
            int length = message.Length;
            for (int i = 0; i < add.Length; i++)
            {
                length = message.IndexOf("$$", startIndex);
                if (length == -1)
                {
                    break;
                }
                builder.Append(message.Substring(startIndex, length - startIndex));
                builder.Append((add[i] == null) ? "null exception message" : add[i].ToString());
                startIndex = length + "$$".Length;
            }
            length = message.Length;
            builder.Append(message.Substring(startIndex, length - startIndex));
            return builder.ToString();
        }

        public static void PrintSystemOut(string message)
        {
        }

        public static Exception RuntimeError(int code, string add)
        {
            return new Exception(GetError(code, add).GetMessage());
        }
    }
}

