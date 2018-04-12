namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;

    public sealed class LobStoreRAFile : ILobStore, IDisposable
    {
        private readonly Database _database;
        private readonly int _lobBlockSize = 0x8000;
        private IScaledRAInterface _file;

        public LobStoreRAFile(Database database, int lobBlockSize)
        {
            this._lobBlockSize = lobBlockSize;
            this._database = database;
            try
            {
                string elementName = database.GetPath() + ".lobs";
                if (database.logger.GetFileAccess().IsStreamElement(elementName))
                {
                    this.OpenFile();
                }
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
                this._file.Dispose();
            }
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
                int length = blockCount * this._lobBlockSize;
                byte[] b = new byte[length];
                this._file.Seek(position);
                this._file.Read(b, 0, length);
                buffer = b;
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

        private void OpenFile()
        {
            try
            {
                string name = this._database.GetPath() + ".lobs";
                bool rdy = this._database.IsReadOnly();
                this._file = ScaledRAFile.NewScaledRAFile(this._database, name, rdy, this._database.IsFilesInAssembly());
            }
            catch (Exception exception)
            {
                throw Error.GetError(0x1d2, exception);
            }
        }

        public void SetBlockBytes(byte[] dataBytes, int blockAddress, int blockCount)
        {
            if (this._file == null)
            {
                this.OpenFile();
            }
            try
            {
                long position = blockAddress * this._lobBlockSize;
                int length = blockCount * this._lobBlockSize;
                this._file.Seek(position);
                this._file.Write(dataBytes, 0, length);
            }
            catch (Exception exception)
            {
                throw Error.GetError(0x1d2, exception);
            }
        }
    }
}

