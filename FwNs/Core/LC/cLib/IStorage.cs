namespace FwNs.Core.LC.cLib
{
    using System;

    public interface IStorage
    {
        void Close();
        long GetFilePointer();
        bool IsReadOnly();
        long Length();
        int Read();
        void Read(byte[] b, int offset, int length);
        int ReadInt();
        long ReadLong();
        void Seek(long position);
        void Synch();
        bool WasNio();
        void Write(byte[] b, int offset, int length);
        void WriteInt(int i);
        void WriteLong(long i);
    }
}

