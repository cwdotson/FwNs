namespace FwNs.Core.LC.cPersist
{
    using System;

    public interface ILobStore : IDisposable
    {
        void Close();
        byte[] GetBlockBytes(int blockAddress, int blockCount);
        int GetBlockSize();
        void SetBlockBytes(byte[] dataBytes, int blockAddress, int blockCount);
    }
}

