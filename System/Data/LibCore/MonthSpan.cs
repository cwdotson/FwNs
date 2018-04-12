namespace System.Data.LibCore
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MonthSpan
    {
        private static MonthSpan Zero;
        private static MonthSpan MaxValue;
        private static readonly MonthSpan MinValue;
        private readonly int _months;
        public int Months
        {
            get
            {
                return (this._months % 12);
            }
        }
        public int TotalMonths
        {
            get
            {
                return this._months;
            }
        }
        public int Years
        {
            get
            {
                return (this._months / 12);
            }
        }
        public double TotalYears
        {
            get
            {
                return (((double) this._months) / 12.0);
            }
        }
        public MonthSpan(MonthSpan from)
        {
            this._months = from.TotalMonths;
        }

        public MonthSpan(int months)
        {
            if ((months > 0x7fffffff) || (months < -2147483648))
            {
                throw new ArgumentOutOfRangeException();
            }
            this._months = months;
        }

        public MonthSpan(int years, int months)
        {
            if ((Math.Abs(months) > 11) || (Math.Abs(months) < 0))
            {
                throw new ArgumentOutOfRangeException();
            }
            months = (years * 12) + months;
            if ((months > 0x7fffffff) || (months < -2147483648))
            {
                throw new ArgumentOutOfRangeException();
            }
            this._months = months;
        }

        public static int Compare(MonthSpan t1, MonthSpan t2)
        {
            if (t1.TotalMonths == t2.TotalMonths)
            {
                return 0;
            }
            if (t1.TotalMonths < t2.TotalMonths)
            {
                return -1;
            }
            return 1;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", this.Years, Math.Abs(this.Months));
        }

        public override bool Equals(object obj)
        {
            if (obj is MonthSpan)
            {
                MonthSpan span = (MonthSpan) obj;
                return (this.TotalMonths == span.TotalMonths);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator <=(MonthSpan x, MonthSpan y)
        {
            return (x.TotalMonths <= y.TotalMonths);
        }

        public static bool operator >=(MonthSpan x, MonthSpan y)
        {
            return (x.TotalMonths >= y.TotalMonths);
        }

        public static bool operator <(MonthSpan x, MonthSpan y)
        {
            return (x.TotalMonths < y.TotalMonths);
        }

        public static bool operator >(MonthSpan x, MonthSpan y)
        {
            return (x.TotalMonths > y.TotalMonths);
        }

        public static bool operator ==(MonthSpan x, MonthSpan y)
        {
            return (x.TotalMonths == y.TotalMonths);
        }

        public static bool operator !=(MonthSpan x, MonthSpan y)
        {
            return (x.TotalMonths != y.TotalMonths);
        }

        public static MonthSpan operator +(MonthSpan t1, MonthSpan t2)
        {
            long num = t1.TotalMonths + t2.TotalMonths;
            if ((num > 0x7fffffffL) || (num < -2147483648L))
            {
                throw new OverflowException();
            }
            return new MonthSpan((int) num);
        }

        public static MonthSpan Add(MonthSpan t1, MonthSpan t2)
        {
            return (t1 + t2);
        }

        public static MonthSpan operator -(MonthSpan t1, MonthSpan t2)
        {
            long num = t1.TotalMonths - t2.TotalMonths;
            if ((num > 0x7fffffffL) || (num < -2147483648L))
            {
                throw new OverflowException();
            }
            return new MonthSpan((int) num);
        }

        public static MonthSpan Subtract(MonthSpan t1, MonthSpan t2)
        {
            return (t1 - t2);
        }

        public static MonthSpan operator +(MonthSpan t)
        {
            return new MonthSpan(t.TotalMonths);
        }

        public static MonthSpan Plus(MonthSpan t)
        {
            return new MonthSpan(t.TotalMonths);
        }

        public static MonthSpan operator -(MonthSpan t)
        {
            return new MonthSpan(-t.TotalMonths);
        }

        public static MonthSpan Negate(MonthSpan t)
        {
            return new MonthSpan(-t.TotalMonths);
        }

        public static explicit operator MonthSpan(string x)
        {
            return Parse(x);
        }

        public static bool TryParse(string s, out MonthSpan val)
        {
            val = MinValue;
            if (s == null)
            {
                return false;
            }
            s = s.Trim();
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException();
            }
            int length = s.LastIndexOf('-');
            switch (length)
            {
                case -1:
                case 0:
                    int num4;
                    if (!int.TryParse(s, out num4))
                    {
                        return false;
                    }
                    val = new MonthSpan(num4);
                    break;

                default:
                    int num2;
                    int num3;
                    if (!int.TryParse(s.Substring(length + 1), out num2))
                    {
                        return false;
                    }
                    if (!int.TryParse(s.Substring(0, length), out num3))
                    {
                        return false;
                    }
                    if ((num2 > 11) || (num2 < 0))
                    {
                        return false;
                    }
                    if (s[0] == '-')
                    {
                        num2 = -num2;
                    }
                    val = new MonthSpan(num3, num2);
                    break;
            }
            return true;
        }

        public static MonthSpan Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }
            s = s.Trim();
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException();
            }
            int length = s.LastIndexOf('-');
            switch (length)
            {
                case -1:
                case 0:
                    return new MonthSpan(int.Parse(s));
            }
            int months = int.Parse(s.Substring(length + 1));
            int years = int.Parse(s.Substring(0, length));
            if ((months > 11) || (months < 0))
            {
                throw new OverflowException();
            }
            if (s[0] == '-')
            {
                months = -months;
            }
            return new MonthSpan(years, months);
        }

        static MonthSpan()
        {
            Zero = new MonthSpan(0);
            MaxValue = new MonthSpan(0x7fffffff);
            MinValue = new MonthSpan(-2147483648);
        }
    }
}

