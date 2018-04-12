namespace FwNs.Core.LC.cLib
{
    using System;

    public class BitMap
    {
        public static bool IsSet(int map, int pos)
        {
            int num = ((int) (-2147483648)) >> pos;
            return ((map & num) > 0);
        }

        public static int Set(int map, int pos)
        {
            int num = ((int) (-2147483648)) >> pos;
            return (map | num);
        }

        public static int SetByte(int map, byte value, int pos)
        {
            int num = (value & 0xff) << (0x18 - pos);
            int num2 = ((int) (-16777216)) >> pos;
            num2 = ~num2;
            map &= num2;
            return (map | num);
        }

        public static int Unset(int map, int pos)
        {
            int num = ((int) (-2147483648)) >> pos;
            num = ~num;
            return (map & num);
        }
    }
}

