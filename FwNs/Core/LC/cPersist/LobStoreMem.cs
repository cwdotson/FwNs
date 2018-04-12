namespace FwNs.Core.LC.cPersist
{
    using System;
    using System.Collections.Generic;

    public sealed class LobStoreMem : ILobStore, IDisposable
    {
        private readonly List<byte[]> _byteStoreList;
        private readonly int _largeBlockSize;
        private readonly int _lobBlockSize;
        private int _blocksInLargeBlock = 0x80;

        public LobStoreMem(int lobBlockSize)
        {
            this._lobBlockSize = lobBlockSize;
            this._largeBlockSize = lobBlockSize * this._blocksInLargeBlock;
            this._byteStoreList = new List<byte[]>();
        }

        public void Close()
        {
            this._byteStoreList.Clear();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }
        }

        public byte[] GetBlockBytes(int blockAddress, int blockCount)
        {
            byte[] destinationArray = new byte[blockCount * this._lobBlockSize];
            int num = 0;
            while (blockCount > 0)
            {
                int num2 = blockAddress / this._blocksInLargeBlock;
                int num3 = blockAddress % this._blocksInLargeBlock;
                int num4 = blockCount;
                if ((num3 + num4) > this._blocksInLargeBlock)
                {
                    num4 = this._blocksInLargeBlock - num3;
                }
                Array.Copy(this._byteStoreList[num2], (int) (num3 * this._lobBlockSize), destinationArray, (int) (num * this._lobBlockSize), (int) (num4 * this._lobBlockSize));
                blockAddress += num4;
                num += num4;
                blockCount -= num4;
            }
            return destinationArray;
        }

        public int GetBlockSize()
        {
            return this._lobBlockSize;
        }

        public void SetBlockBytes(byte[] dataBytes, int blockAddress, int blockCount)
        {
            int num = 0;
            while (blockCount > 0)
            {
                int num2 = blockAddress / this._blocksInLargeBlock;
                int num3 = (blockAddress + blockCount) / this._blocksInLargeBlock;
                if (((blockAddress + blockCount) % this._blocksInLargeBlock) != 0)
                {
                    num3++;
                }
                if (num2 >= this._byteStoreList.Count)
                {
                    this._byteStoreList.Add(new byte[this._largeBlockSize]);
                }
                byte[] destinationArray = this._byteStoreList[num2];
                int num4 = blockAddress % this._blocksInLargeBlock;
                int num5 = blockCount;
                if ((num4 + num5) > this._blocksInLargeBlock)
                {
                    num5 = this._blocksInLargeBlock - num4;
                }
                Array.Copy(dataBytes, (int) (num * this._lobBlockSize), destinationArray, (int) (num4 * this._lobBlockSize), (int) (num5 * this._lobBlockSize));
                blockAddress += num5;
                num += num5;
                blockCount -= num5;
            }
        }
    }
}

