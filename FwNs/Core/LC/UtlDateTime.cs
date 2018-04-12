namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Globalization;
    using System.Text;

    public sealed class UtlDateTime
    {
        private const string SdfdPattern = "dd-MMM-yy HH:mm:ss";
        private const string SdftsPattern = "yyyy-MM-dd HH:mm:ss";
        private const char E = '\x00ef';
        private static DateTime _tempDate = new DateTime(0L);
        private static readonly char[][] DateTokens;
        private static readonly string[] CSharpDateTokens;

        static UtlDateTime()
        {
            char[][] chArrayArray1 = new char[0x27][];
            chArrayArray1[0] = new char[] { 'R', 'R', 'R', 'R' };
            chArrayArray1[1] = new char[] { 'I', 'Y', 'Y', 'Y' };
            chArrayArray1[2] = new char[] { 'I', 'Y' };
            chArrayArray1[3] = new char[] { 'I' };
            chArrayArray1[4] = new char[] { 'Y', 'Y', 'Y', 'Y' };
            chArrayArray1[5] = new char[] { 'Y', 'Y' };
            chArrayArray1[6] = new char[] { 'Y' };
            chArrayArray1[7] = new char[] { 'B', 'C' };
            chArrayArray1[8] = new char[] { 'B', '.', 'C', '.' };
            chArrayArray1[9] = new char[] { 'A', 'D' };
            chArrayArray1[10] = new char[] { 'A', '.', 'D', '.' };
            chArrayArray1[11] = new char[] { 'M', 'M' };
            chArrayArray1[12] = new char[] { 'M', 'O', 'N', 'T', 'H' };
            chArrayArray1[13] = new char[] { 'M', 'O', 'N' };
            chArrayArray1[14] = new char[] { 'F', 'M', 'M', 'O', 'N', 'T', 'H' };
            chArrayArray1[15] = new char[] { 'D', 'D', 'T', 'H' };
            chArrayArray1[0x10] = new char[] { 'D', 'D' };
            chArrayArray1[0x11] = new char[] { 'D', 'A', 'Y' };
            chArrayArray1[0x12] = new char[] { 'D', 'Y' };
            chArrayArray1[0x13] = new char[] { 'F', 'M', 'D', 'A', 'Y' };
            chArrayArray1[20] = new char[] { 'H', 'H', '2', '4' };
            chArrayArray1[0x15] = new char[] { 'H', 'H', '1', '2' };
            chArrayArray1[0x16] = new char[] { 'H', 'H' };
            chArrayArray1[0x17] = new char[] { 'M', 'I' };
            chArrayArray1[0x18] = new char[] { 'S', 'S' };
            chArrayArray1[0x19] = new char[] { 'A', 'M' };
            chArrayArray1[0x1a] = new char[] { 'P', 'M' };
            chArrayArray1[0x1b] = new char[] { 'A', '.', 'M', '.' };
            chArrayArray1[0x1c] = new char[] { 'P', '.', 'M', '.' };
            chArrayArray1[0x1d] = new char[] { 'F', 'F', '1' };
            chArrayArray1[30] = new char[] { 'F', 'F', '2' };
            chArrayArray1[0x1f] = new char[] { 'F', 'F', '3' };
            chArrayArray1[0x20] = new char[] { 'F', 'F', '4' };
            chArrayArray1[0x21] = new char[] { 'F', 'F', '5' };
            chArrayArray1[0x22] = new char[] { 'F', 'F', '6' };
            chArrayArray1[0x23] = new char[] { 'F', 'F', '7' };
            chArrayArray1[0x24] = new char[] { 'F', 'F' };
            chArrayArray1[0x25] = new char[] { 'T', 'Z', 'H' };
            chArrayArray1[0x26] = new char[] { 'T', 'Z', 'M' };
            DateTokens = chArrayArray1;
            CSharpDateTokens = new string[] { 
                "yyyy", "yyyy", "yy", "y", "yyyy", "yy", "y", "gg", "gg", "gg", "gg", "MM", "MMMM", "MMM", "MMMM", "dd",
                "dd", "dddd", "ddd", "dddd", "HH", "H", "H", "mm", "ss", "tt", "tt", "tt", "tt", "f", "ff", "fff",
                "fffff", "fffff", "ffffff", "fffffff", "ffffffff", "zz", "zzz"
            };
        }

        public static long ConvertTicksFromTimeZone(TimeZoneInfo zone, long ticks)
        {
            return TimeZoneInfo.ConvertTimeToUtc(new DateTime(ticks), zone).Ticks;
        }

        public static long ConvertTicksToTimeZone(TimeZoneInfo zone, long ticks)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(ticks), zone).Ticks;
        }

        public static long GetDateSeconds(string s)
        {
            long num;
            try
            {
                DateTime time;
                try
                {
                    time = DateTime.Parse(s);
                }
                catch (Exception)
                {
                    time = DateTime.Parse(s, CultureInfo.InvariantCulture);
                }
                num = time.Ticks / 0x989680L;
            }
            catch (Exception)
            {
                throw Error.GetError(0xd4f, s);
            }
            return num;
        }

        public static string GetDateString(long seconds)
        {
            DateTime time = new DateTime(seconds * 0x989680L);
            return time.ToString("dd-MMM-yy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public static long GetNormalisedDate(long secs)
        {
            return secs;
        }

        public static long GetNormalisedTime(long t)
        {
            DateTime time = new DateTime(t * 0x989680L);
            DateTime time2 = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
            return (time2.Ticks / 0x989680L);
        }

        public static long GetTimestampSeconds(string s)
        {
            long num;
            try
            {
                DateTime time;
                try
                {
                    time = DateTime.Parse(s);
                }
                catch (Exception)
                {
                    time = DateTime.Parse(s, CultureInfo.InvariantCulture);
                }
                num = time.Ticks / 0x989680L;
            }
            catch (Exception)
            {
                throw Error.GetError(0xd4f, s);
            }
            return num;
        }

        public static void GetTimestampString(StringBuilder sb, long seconds, int nanos, int scale)
        {
            _tempDate = new DateTime(seconds * 0x989680L);
            sb.Append(_tempDate.ToString("yyyy-MM-dd HH:mm:ss"));
            if (scale > 0)
            {
                sb.Append('.');
                sb.Append(StringUtil.ToZeroPaddedString((long) nanos, 9, scale));
            }
        }

        public static int GetZoneMillis(TimeZoneInfo zone, long millis)
        {
            return (int) (zone.GetUtcOffset(new DateTime(millis * 0x2710L)).Ticks / 0x2710L);
        }

        public static int GetZoneSeconds(TimeZoneInfo zone)
        {
            return (int) (zone.get_BaseUtcOffset().Ticks / 0x2710L);
        }

        public static string ToCSharpDatePattern(string format)
        {
            int length = format.Length;
            StringBuilder builder = new StringBuilder(length);
            Tokenizer tokenizer = new Tokenizer();
            bool flag = false;
            int num2 = 0;
            while (num2 <= length)
            {
                char ch = (num2 == length) ? '\x00ef' : format[num2];
                if ((ch == '"') && !flag)
                {
                    flag = true;
                    builder.Append(ch);
                    num2++;
                }
                else
                {
                    if ((ch == '"') & flag)
                    {
                        flag = false;
                        builder.Append(ch);
                        num2++;
                        continue;
                    }
                    if (flag)
                    {
                        builder.Append(ch);
                        num2++;
                        continue;
                    }
                    if (!tokenizer.Next(char.ToUpper(ch, CultureInfo.InvariantCulture), DateTokens))
                    {
                        int lastMatch = tokenizer.GetLastMatch();
                        if (lastMatch >= 0)
                        {
                            builder.Append(CSharpDateTokens[lastMatch]);
                        }
                        tokenizer.Reset();
                        if (!tokenizer.IsConsumed() && (lastMatch >= 0))
                        {
                            continue;
                        }
                    }
                    if (tokenizer.IsConsumed())
                    {
                        num2++;
                    }
                    else
                    {
                        builder.Append(ch);
                        num2++;
                    }
                }
            }
            builder.Length--;
            return builder.ToString();
        }

        private class Tokenizer
        {
            private bool _consumed;
            private int _last;
            private int _offset;
            private long _state;

            public Tokenizer()
            {
                this.Reset();
            }

            public int GetLastMatch()
            {
                return this._last;
            }

            public bool IsConsumed()
            {
                return this._consumed;
            }

            private bool IsZeroBit(int bit)
            {
                return ((this._state & (((long) 1L) << bit)) == 0L);
            }

            public bool Next(char ch, char[][] tokens)
            {
                int num5 = this._offset + 1;
                this._offset = num5;
                int index = num5;
                int num2 = this._offset + 1;
                int num3 = 0;
                this._consumed = false;
                int length = tokens.Length;
                while (--length >= 0)
                {
                    if (this.IsZeroBit(length))
                    {
                        if (tokens[length][index] == ch)
                        {
                            this._consumed = true;
                            if (tokens[length].Length == num2)
                            {
                                this.SetBit(length);
                                this._last = length;
                                num3 = 0;
                            }
                            else
                            {
                                num3++;
                            }
                        }
                        else
                        {
                            this.SetBit(length);
                        }
                    }
                }
                return (num3 > 0);
            }

            public void Reset()
            {
                this._last = -1;
                this._offset = -1;
                this._state = 0L;
            }

            private void SetBit(int bit)
            {
                this._state |= ((long) 1L) << bit;
            }
        }
    }
}

