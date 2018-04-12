namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cLib.IO;
    using FwNs.Core.LC.cResources;
    using System;
    using System.Collections;
    using System.IO;

    public sealed class RAShadowFile : IDisposable
    {
        private readonly BitArray _bitMap;
        private readonly ByteArrayOutputStream _byteArrayOutputStream = new ByteArrayOutputStream(new byte[0]);
        private readonly Database _database;
        private readonly long _maxSize;
        private readonly int _pageSize;
        private readonly string _pathName;
        private readonly IStorage _source;
        private IScaledRAInterface _dest;
        private bool _zeroPageSet;

        public RAShadowFile(Database database, IStorage source, string pathName, long maxSize, int pageSize)
        {
            this._database = database;
            this._pathName = pathName;
            this._source = source;
            this._pageSize = pageSize;
            this._maxSize = maxSize;
            int length = (int) (maxSize / ((long) pageSize));
            if ((maxSize % ((long) pageSize)) != 0)
            {
                length++;
            }
            this._bitMap = new BitArray(length, false);
        }

        public void Close()
        {
            if (this._dest != null)
            {
                this._dest.Synch();
                this._dest.Close();
                this._dest = null;
            }
        }

        private void Copy(int pageOffset)
        {
            if (!this._bitMap.Get(pageOffset))
            {
                this._bitMap.Set(pageOffset, true);
                long v = pageOffset * this._pageSize;
                int length = this._pageSize;
                if ((this._maxSize - v) < this._pageSize)
                {
                    length = (int) (this._maxSize - v);
                }
                try
                {
                    if (this._dest == null)
                    {
                        this.Open();
                    }
                    long position = this._dest.Length();
                    byte[] buffer = new byte[this._pageSize + 12];
                    this._byteArrayOutputStream.SetBuffer(buffer);
                    this._byteArrayOutputStream.WriteInt(this._pageSize);
                    this._byteArrayOutputStream.WriteLong(v);
                    this._source.Seek(v);
                    this._source.Read(buffer, 12, length);
                    this._dest.Seek(position);
                    this._dest.Write(buffer, 0, buffer.Length);
                }
                catch (Exception exception)
                {
                    this._bitMap.Set(pageOffset, false);
                    this.Close();
                    this._database.logger.LogWarningEvent(string.Concat(new object[] { FwNs.Core.LC.cResources.SR.RAShadowFile_Copy_pos, v, FwNs.Core.LC.cResources.SR.Single_Space, length }), exception);
                    throw new IOException(exception.Message, exception);
                }
            }
        }

        public void Copy(long fileOffset, int size)
        {
            if (!this._zeroPageSet)
            {
                this.Copy(0);
                this._bitMap.Set(0, true);
                this._zeroPageSet = true;
            }
            if (fileOffset < this._maxSize)
            {
                int pageOffset = (int) (fileOffset / ((long) this._pageSize));
                long num1 = fileOffset + size;
                int num2 = (int) (num1 / ((long) this._pageSize));
                if ((num1 % ((long) this._pageSize)) == 0)
                {
                    num2--;
                }
                while (pageOffset <= num2)
                {
                    this.Copy(pageOffset);
                    pageOffset++;
                }
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
                this._byteArrayOutputStream.Dispose();
                this._dest.Dispose();
            }
        }

        private void Open()
        {
            this._dest = ScaledRAFile.NewScaledRAFile(this._database, this._pathName, false, this._database.IsFilesInAssembly());
        }

        public static void RestoreFile(Database database, string sourceName, string destName)
        {
            using (IScaledRAInterface interface2 = ScaledRAFile.NewScaledRAFile(database, sourceName, true, database.IsFilesInAssembly()))
            {
                using (IScaledRAInterface interface3 = ScaledRAFile.NewScaledRAFile(database, destName, false, database.IsFilesInAssembly()))
                {
                    while (interface2.GetFilePointer() != interface2.Length())
                    {
                        int length = interface2.ReadInt();
                        long position = interface2.ReadLong();
                        byte[] b = new byte[length];
                        interface2.Read(b, 0, length);
                        interface3.Seek(position);
                        interface3.Write(b, 0, length);
                    }
                }
            }
        }
    }
}

