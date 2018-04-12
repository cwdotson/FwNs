namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cLib.IO;
    using FwNs.Core.LC.cRows;
    using System;

    public interface IRowOutputInterface
    {
        IRowOutputInterface Duplicate();
        ByteArrayOutputStream GetOutputStream();
        int GetSize(Row row);
        int GetStorageSize(int size);
        void Reset();
        void SetBuffer(byte[] mainBuffer);
        int Size();
        void WriteByte(int i);
        void WriteData(object[] data, SqlType[] types);
        void WriteData(int l, SqlType[] types, object[] data, HashMappedList<string, ColumnSchema> cols, int[] primarykeys);
        void WriteEnd();
        void WriteInt(int i);
        void WriteIntData(int i, int position);
        void WriteLong(long i);
        void WriteShort(int i);
        void WriteSize(int size);
        void WriteString(string value);
        void WriteType(int type);
    }
}

