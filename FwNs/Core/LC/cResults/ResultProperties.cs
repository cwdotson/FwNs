namespace FwNs.Core.LC.cResults
{
    using System;

    public sealed class ResultProperties
    {
        private const int IdxHoldable = 1;
        private const int IdxScrollable = 2;
        private const int IdxUpdatable = 3;
        private const int IdxSensitive = 4;
        private static int idx_returnable = 0;
        public static int DefaultPropsValue;
        public static int UpdatablePropsValue = 8;

        public static int AddHoldable(int props, bool flag)
        {
            if (!flag)
            {
                return (props & -3);
            }
            return (props | 2);
        }

        public static int AddScrollable(int props, bool flag)
        {
            if (!flag)
            {
                return (props & -5);
            }
            return (props | 4);
        }

        public static int AddUpdatable(int props, bool flag)
        {
            if (!flag)
            {
                return (props & -9);
            }
            return (props | 8);
        }

        public static int GetAdoConcurrency(int props)
        {
            if (!IsReadOnly(props))
            {
                return 0x3f0;
            }
            return 0x3ef;
        }

        public static int GetAdoScrollability(int props)
        {
            if (!IsScrollable(props))
            {
                return 0x3eb;
            }
            return 0x3ec;
        }

        public static int GetJdbcHoldability(int props)
        {
            if (!IsHoldable(props))
            {
                return 2;
            }
            return 1;
        }

        public static int GetProperties(int sensitive, int updatable, int scrollable, int holdable, int returnable)
        {
            return (((((sensitive << 4) | (updatable << 3)) | (scrollable << 2)) | (holdable << 1)) | (returnable << idx_returnable));
        }

        public static int GetValueForAdo(int type, int concurrency, int holdability)
        {
            int num = (type == 0x3eb) ? 0 : 1;
            int num2 = (holdability == 1) ? 1 : 0;
            return (((((concurrency == 0x3f0) ? 1 : 0) << 3) | (num << 2)) | (num2 << 1));
        }

        public static bool IsHoldable(int props)
        {
            return ((props & 2) > 0);
        }

        public static bool IsReadOnly(int props)
        {
            return ((props & 8) == 0);
        }

        public static bool IsScrollable(int props)
        {
            return ((props & 4) > 0);
        }

        public static bool IsSensitive(int props)
        {
            return ((props & 0x10) > 0);
        }

        public static bool IsUpdatable(int props)
        {
            return ((props & 8) > 0);
        }
    }
}

