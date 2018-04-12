namespace FwNs.Core.LC.cLib
{
    using System;

    public class ArrayUtil
    {
        public const int ClassCodeByte = 0x42;
        public const int ClassCodeChar = 0x43;
        public const int ClassCodeDouble = 0x44;
        public const int ClassCodeFloat = 70;
        public const int ClassCodeInt = 0x49;
        public const int ClassCodeLong = 0x4a;
        public const int ClassCodeObject = 0x4c;
        public const int ClassCodeShort = 0x53;
        public const int ClassCodeBool = 90;

        public static bool AreAllIntIndexesInBooleanArray(int[] arra, bool[] arrb)
        {
            for (int i = 0; i < arra.Length; i++)
            {
                if (!arrb[arra[i]])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool AreEqual(int[] arra, int[] arrb, int count, bool full)
        {
            if (!HaveEqualArrays(arra, arrb, count))
            {
                return false;
            }
            return (!full || ((arra.Length == arrb.Length) && (count == arra.Length)));
        }

        public static bool AreEqualSets(int[] arra, int[] arrb)
        {
            return ((arra.Length == arrb.Length) && HaveEqualSets(arra, arrb, arra.Length));
        }

        public static int[] ArraySlice(int[] source, int start, int count)
        {
            int[] destinationArray = new int[count];
            Array.Copy(source, start, destinationArray, 0, count);
            return destinationArray;
        }

        public static int[] BooleanArrayToIntIndexes(bool[] arrb)
        {
            int num = 0;
            for (int i = 0; i < arrb.Length; i++)
            {
                if (arrb[i])
                {
                    num++;
                }
            }
            int[] numArray = new int[num];
            num = 0;
            for (int j = 0; j < arrb.Length; j++)
            {
                if (arrb[j])
                {
                    numArray[num++] = j;
                }
            }
            return numArray;
        }

        public static char[] ByteArrayToChars(byte[] bytes)
        {
            char[] chArray = new char[bytes.Length / 2];
            int index = 0;
            for (int i = 0; i < chArray.Length; i++)
            {
                chArray[i] = (char) ((bytes[index] & 0xff) + (bytes[index + 1] << 8));
                index += 2;
            }
            return chArray;
        }

        public static byte[] CharArrayToBytes(char[] chars)
        {
            byte[] buffer = new byte[chars.Length * 2];
            int index = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                int num3 = chars[i];
                buffer[index] = (byte) num3;
                buffer[index + 1] = (byte) (num3 >> 8);
                index += 2;
            }
            return buffer;
        }

        public static void ClearArray<T>(T[] data, int from, int to)
        {
            while (--to >= from)
            {
                data[to] = default(T);
            }
        }

        public static bool ContainsAny(object[] arra, object[] arrb)
        {
            for (int i = 0; i < arrb.Length; i++)
            {
                for (int j = 0; j < arra.Length; j++)
                {
                    if ((arrb[i] == arra[j]) || arrb[i].Equals(arra[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ContainsAt(byte[] arra, int start, byte[] arrb)
        {
            return (CountSameElements(arra, start, arrb) == arrb.Length);
        }

        public static void CopyAdjustArray<T>(T[] source, T[] dest, T addition, int colindex, int adjust)
        {
            int length = source.Length;
            if (colindex < 0)
            {
                Array.Copy(source, 0, dest, 0, length);
            }
            else
            {
                Array.Copy(source, 0, dest, 0, colindex);
                if (adjust == 0)
                {
                    int num2 = (length - colindex) - 1;
                    dest[colindex] = addition;
                    if (num2 > 0)
                    {
                        Array.Copy(source, colindex + 1, dest, colindex + 1, num2);
                    }
                }
                else if (adjust < 0)
                {
                    int num3 = (length - colindex) - 1;
                    if (num3 > 0)
                    {
                        Array.Copy(source, colindex + 1, dest, colindex, num3);
                    }
                }
                else
                {
                    int num4 = length - colindex;
                    dest[colindex] = addition;
                    if (num4 > 0)
                    {
                        Array.Copy(source, colindex, dest, colindex + 1, num4);
                    }
                }
            }
        }

        public static int CountCommonElements(int[] arra, int[] arrb)
        {
            int num = 0;
            for (int i = 0; i < arra.Length; i++)
            {
                for (int j = 0; j < arrb.Length; j++)
                {
                    if (arra[i] == arrb[j])
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public static int CountSameElements(byte[] arra, int start, byte[] arrb)
        {
            int num = 0;
            int length = arra.Length - start;
            if (length > arrb.Length)
            {
                length = arrb.Length;
            }
            for (int i = 0; (i < length) && (arra[i + start] == arrb[i]); i++)
            {
                num++;
            }
            return num;
        }

        public static int CountTrueElements(bool[] arra)
        {
            int num = 0;
            for (int i = 0; i < arra.Length; i++)
            {
                if (arra[i])
                {
                    num++;
                }
            }
            return num;
        }

        public static void FillArray(int[] array, int value)
        {
            int length = array.Length;
            while (--length >= 0)
            {
                array[length] = value;
            }
        }

        public static void FillArray(object[] array, object value)
        {
            int length = array.Length;
            while (--length >= 0)
            {
                array[length] = value;
            }
        }

        public static void FillArray(char[] array, int offset, char value)
        {
            int length = array.Length;
            while (--length >= offset)
            {
                array[length] = value;
            }
        }

        public static void FillSequence(int[] colindex)
        {
            for (int i = 0; i < colindex.Length; i++)
            {
                colindex[i] = i;
            }
        }

        public static int Find(short[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int Find(int[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int Find(object[] array, object obj)
        {
            int index = 0;
            while (index < array.Length)
            {
                if ((array[index] != obj) && ((obj == null) || !obj.Equals(array[index])))
                {
                    index++;
                }
                else
                {
                    return index;
                }
            }
            return -1;
        }

        public static int Find(byte[] arra, int start, int limit, byte[] arrb)
        {
            int index = start;
            limit = (limit - arrb.Length) + 1;
            int num2 = arrb[0];
            while (index < limit)
            {
                if (arra[index] == num2)
                {
                    if (arrb.Length == 1)
                    {
                        return index;
                    }
                    if (ContainsAt(arra, index, arrb))
                    {
                        return index;
                    }
                }
                index++;
            }
            return -1;
        }

        public static int Find(short[] array, int value, int offset, int count)
        {
            for (int i = offset; i < (offset + count); i++)
            {
                if (array[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool HasAllNull(object[] array, int[] columnMap)
        {
            int length = columnMap.Length;
            for (int i = 0; i < length; i++)
            {
                if (array[columnMap[i]] != null)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool HasNull(object[] array, int[] columnMap)
        {
            int length = columnMap.Length;
            for (int i = 0; i < length; i++)
            {
                if (array[columnMap[i]] == null)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HaveCommonElement(int[] arra, int[] arrb)
        {
            for (int i = 0; i < arra.Length; i++)
            {
                int num2 = arra[i];
                for (int j = 0; j < arrb.Length; j++)
                {
                    if (num2 == arrb[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HaveEqualArrays(int[] arra, int[] arrb, int count)
        {
            if ((count > arra.Length) || (count > arrb.Length))
            {
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                if (arra[i] != arrb[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool HaveEqualSets(int[] arra, int[] arrb, int count)
        {
            if ((count > arra.Length) || (count > arrb.Length))
            {
                return false;
            }
            if (count == 1)
            {
                return (arra[0] == arrb[0]);
            }
            int[] array = ResizeArray<int>(arra, count);
            int[] numArray2 = ResizeArray<int>(arrb, count);
            SortArray(array);
            SortArray(numArray2);
            for (int i = 0; i < count; i++)
            {
                if (array[i] != numArray2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static void IntIndexesToboolArray(int[] arra, bool[] arrb)
        {
            for (int i = 0; i < arra.Length; i++)
            {
                if (arra[i] < arrb.Length)
                {
                    arrb[arra[i]] = true;
                }
            }
        }

        public static void IntIndexesToBooleanArray(int[] arra, bool[] arrb)
        {
            for (int i = 0; i < arra.Length; i++)
            {
                if (arra[i] < arrb.Length)
                {
                    arrb[arra[i]] = true;
                }
            }
        }

        public static unsafe void OrBooleanArray(bool[] source, bool[] dest)
        {
            for (int i = 0; i < dest.Length; i++)
            {
                *((sbyte*) &(dest[i])) = *(((byte*) &(dest[i]))) | source[i];
            }
        }

        public static void ProjectRow(byte[] row, int[] columnMap, byte[] newRow)
        {
            for (int i = 0; i < columnMap.Length; i++)
            {
                newRow[i] = row[columnMap[i]];
            }
        }

        public static void ProjectRow(int[] row, int[] columnMap, int[] newRow)
        {
            for (int i = 0; i < columnMap.Length; i++)
            {
                newRow[i] = row[columnMap[i]];
            }
        }

        public static void ProjectRow(object[] row, int[] columnMap, object[] newRow)
        {
            for (int i = 0; i < columnMap.Length; i++)
            {
                newRow[i] = row[columnMap[i]];
            }
        }

        public static void ProjectRowReverse(byte[] row, int[] columnMap, byte[] newRow)
        {
            for (int i = 0; i < columnMap.Length; i++)
            {
                row[columnMap[i]] = newRow[i];
            }
        }

        public static void ProjectRowReverse(object[] row, int[] columnMap, object[] newRow)
        {
            for (int i = 0; i < columnMap.Length; i++)
            {
                row[columnMap[i]] = newRow[i];
            }
        }

        public static T[] ResizeArray<T>(T[] source, int newsize)
        {
            T[] destinationArray = new T[newsize];
            int length = source.Length;
            if (length < newsize)
            {
                newsize = length;
            }
            Array.Copy(source, 0, destinationArray, 0, newsize);
            return destinationArray;
        }

        public static T[] ResizeArrayIfDifferent<T>(T[] source, int newsize)
        {
            int length = source.Length;
            if (length == newsize)
            {
                return source;
            }
            T[] destinationArray = new T[newsize];
            if (length < newsize)
            {
                newsize = length;
            }
            Array.Copy(source, 0, destinationArray, 0, newsize);
            return destinationArray;
        }

        public static void SortArray(int[] array)
        {
            bool flag;
            do
            {
                flag = false;
                for (int i = 0; i < (array.Length - 1); i++)
                {
                    if (array[i] > array[i + 1])
                    {
                        int num2 = array[i + 1];
                        array[i + 1] = array[i];
                        array[i] = num2;
                        flag = true;
                    }
                }
            }
            while (flag);
        }

        public static T[] ToAdjustedArray<T>(T[] source, T addition, int colindex, int adjust)
        {
            T[] dest = new T[source.Length + adjust];
            CopyAdjustArray<T>(source, dest, addition, colindex, adjust);
            return dest;
        }

        public static int[] ToAdjustedColumnArray(int[] colarr, int colindex, int adjust)
        {
            if (colarr == null)
            {
                return null;
            }
            int[] sourceArray = new int[colarr.Length];
            int index = 0;
            for (int i = 0; i < colarr.Length; i++)
            {
                if (colarr[i] > colindex)
                {
                    sourceArray[index] = colarr[i] + adjust;
                    index++;
                }
                else if (colarr[i] == colindex)
                {
                    if (adjust >= 0)
                    {
                        sourceArray[index] = colarr[i] + adjust;
                        index++;
                    }
                }
                else
                {
                    sourceArray[index] = colarr[i];
                    index++;
                }
            }
            if (colarr.Length != index)
            {
                int[] destinationArray = new int[index];
                Array.Copy(sourceArray, destinationArray, index);
                return destinationArray;
            }
            return sourceArray;
        }

        public static int[] Union(int[] arra, int[] arrb)
        {
            int newsize = (arra.Length + arrb.Length) - CountCommonElements(arra, arrb);
            if ((newsize <= arra.Length) || (newsize <= arrb.Length))
            {
                if (arra.Length <= arrb.Length)
                {
                    return arrb;
                }
                return arra;
            }
            int[] numArray2 = ResizeArray<int>(arrb, newsize);
            int length = arrb.Length;
            int index = 0;
            while (index < arra.Length)
            {
                for (int i = 0; i < arrb.Length; i++)
                {
                    if (arra[index] == arrb[i])
                    {
                        index++;
                        continue;
                    }
                }
                numArray2[length++] = arra[index];
            }
            return numArray2;
        }
    }
}

