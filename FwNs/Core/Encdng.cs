namespace FwNs.Core
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public static class Encdng
    {
        public static int GetByteCount(string hexString)
        {
            int num = 0;
            for (int i = 0; i < hexString.Length; i++)
            {
                if (IsHexDigit(hexString[i]))
                {
                    num++;
                }
            }
            if ((num % 2) != 0)
            {
                num--;
            }
            return (num / 2);
        }

        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string str = "";
            for (int i = 0; i < hexString.Length; i++)
            {
                char c = hexString[i];
                if (IsHexDigit(c))
                {
                    str = str + c.ToString();
                }
                else
                {
                    discarded++;
                }
            }
            if ((str.Length % 2) != 0)
            {
                discarded++;
                str = str.Substring(0, str.Length - 1);
            }
            byte[] buffer = new byte[str.Length / 2];
            int num = 0;
            for (int j = 0; j < buffer.Length; j++)
            {
                char[] chArray1 = new char[] { str[num], str[num + 1] };
                string hex = new string(chArray1);
                buffer[j] = HexToByte(hex);
                num += 2;
            }
            return buffer;
        }

        private static byte HexToByte(string hex)
        {
            if ((hex.Length > 2) || (hex.Length <= 0))
            {
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            }
            return byte.Parse(hex, NumberStyles.HexNumber);
        }

        public static bool InHexFormat2(string hexString)
        {
            for (int i = 0; i < hexString.Length; i++)
            {
                if (!IsHexDigit(hexString[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsHexDigit(char c)
        {
            int num = Convert.ToInt32('A');
            int num2 = Convert.ToInt32('0');
            c = char.ToUpper(c);
            int num3 = Convert.ToInt32(c);
            return (((num3 >= num) && (num3 < (num + 6))) || ((num3 >= num2) && (num3 < (num2 + 10))));
        }

        public static string ToStrng(byte[] bytes)
        {
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str = str + bytes[i].ToString("X2");
            }
            return str;
        }
    }
}

