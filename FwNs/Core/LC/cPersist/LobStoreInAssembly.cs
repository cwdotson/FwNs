namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.IO;

    public sealed class LobStoreInAssembly : ILobStore, IDisposable
    {
        private Database _database;
        private Stream _file;
        private string _fileName;
        private int _lobBlockSize;

        public LobStoreInAssembly(Database database, int lobBlockSize)
        {
            this._lobBlockSize = lobBlockSize;
            this._database = database;
            try
            {
                this._fileName = database.GetPath() + ".lobs";
                this.ResetStream();
            }
            catch (Exception exception)
            {
                throw Error.GetError(0x1d2, exception);
            }
        }

        public void Close()
        {
            try
            {
                if (this._file != null)
                {
                    this._file.Close();
                }
            }
            catch (Exception exception)
            {
                throw Error.GetError(0x1d2, exception);
            }
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

        private void FileSeek(long position)
        {
            if (this._file == null)
            {
                this.ResetStream();
            }
            this._file.Seek(position, SeekOrigin.Begin);
        }

        public byte[] GetBlockBytes(int blockAddress, int blockCount)
        {
            byte[] buffer;
            if (this._file == null)
            {
                throw Error.GetError(0x1c4);
            }
            try
            {
                long position = blockAddress * this._lobBlockSize;
                int num1 = blockCount * this._lobBlockSize;
                byte[] buffer2 = new byte[num1];
                this.FileSeek(position);
                int count = num1;
                int offset = 0;
                while (count > 0)
                {
                    int num4 = this._file.Read(buffer2, offset, count);
                    offset += num4;
                    count -= num4;
                }
                buffer = buffer2;
            }
            catch (Exception exception)
            {
                throw Error.GetError(0x1d2, exception);
            }
            return buffer;
        }

        public int GetBlockSize()
        {
            return this._lobBlockSize;
        }

        private void ResetStream()
        {
            if (this._file != null)
            {
                this._file.Close();
            }
            this._file = this._database.logger.GetFileAccess().OpenOutputStreamElement(this._fileName, FileMode.OpenOrCreate, FileAccess.Read);
        }

        public void SetBlockBytes(byte[] dataBytes, int blockAddress, int blockCount)
        {
        }
    }
}

