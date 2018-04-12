namespace FwNs.Core.LC.cLib
{
    using System;

    public class ArrayCounter
    {
        private static long CalcInterval(int segments, int start, int limit)
        {
            long num = limit - start;
            if (num < 0L)
            {
                return 0L;
            }
            int num3 = ((num % ((long) segments)) == 0) ? 0 : 1;
            return ((num / ((long) segments)) + num3);
        }

        public static int[] CountSegments(int[] array, int elements, int segments, int start, int limit)
        {
            int[] numArray = new int[segments];
            long num = CalcInterval(segments, start, limit);
            if (num > 0L)
            {
                for (int i = 0; i < elements; i++)
                {
                    int num3 = array[i];
                    if ((num3 >= start) && (num3 < limit))
                    {
                        int index = (int) (((long) (num3 - start)) / num);
                        numArray[index]++;
                    }
                }
            }
            return numArray;
        }

        public static int Rank(int[] array, int elements, int target, int start, int limit, int margin)
        {
            long num3;
            int num = 0;
            int num2 = limit;
        Label_0005:
            num3 = CalcInterval(0x100, start, num2);
            int[] numArray = CountSegments(array, elements, 0x100, start, num2);
            for (int i = 0; (i < numArray.Length) && ((num + numArray[i]) < target); i++)
            {
                num += numArray[i];
                start += (int) num3;
            }
            if ((num + margin) < target)
            {
                if (num3 <= 1L)
                {
                    return start;
                }
                num2 = ((start + num3) < limit) ? (start + ((int) num3)) : limit;
                goto Label_0005;
            }
            return start;
        }
    }
}

