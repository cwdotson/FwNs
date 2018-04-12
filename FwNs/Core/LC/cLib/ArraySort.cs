namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;

    public class ArraySort
    {
        public static void InsertionSort<T>(T[] array, IComparer<T> comparator, int lo0, int hi0)
        {
            for (int i = lo0 + 1; i <= hi0; i++)
            {
                int j = i;
                while ((j > lo0) && (comparator.Compare(array[i], array[j - 1]) < 0))
                {
                    j--;
                }
                if (i != j)
                {
                    MoveAndInsertRow<T>(array, i, j);
                }
            }
        }

        private static void MoveAndInsertRow<T>(T[] array, int i, int j)
        {
            T local = array[i];
            MoveRows<T>(array, j, j + 1, i - j);
            array[j] = local;
        }

        private static void MoveRows<T>(T[] array, int fromIndex, int toIndex, int rows)
        {
            Array.Copy(array, fromIndex, array, toIndex, rows);
        }

        public static int SearchFirst<T>(T[] array, int start, int limit, T value, IComparer<T> c)
        {
            int num = start;
            int num2 = limit;
            int num3 = limit;
            while (num < num2)
            {
                int index = (num + num2) / 2;
                int num6 = c.Compare(value, array[index]);
                if (num6 < 0)
                {
                    num2 = index;
                }
                else
                {
                    if (num6 > 0)
                    {
                        num = index + 1;
                        continue;
                    }
                    num2 = index;
                    num3 = index;
                }
            }
            if (num3 != limit)
            {
                return num3;
            }
            return (-num - 1);
        }

        public static void Sort<T>(T[] array, int start, int limit, IComparer<T> comparator)
        {
            if ((start + 1) < limit)
            {
                InsertionSort<T>(array, comparator, start, limit - 1);
            }
        }

        private static void Swap(object[] array, int i1, int i2)
        {
            object obj2 = array[i1];
            array[i1] = array[i2];
            array[i2] = obj2;
        }
    }
}

