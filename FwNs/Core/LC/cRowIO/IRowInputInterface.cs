namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using System;

    public interface IRowInputInterface
    {
        byte[] GetBuffer();
        int GetPos();
        int GetSize();
        object[] ReadData(SqlType[] colTypes);
        int ReadInt();
        long ReadLong();
        int ReadShort();
        string ReadString();
        int ReadType();
        void ResetRow(int filePos, int size);
    }
}

