namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cLib.IO;
    using FwNs.Core.LC.cResources;
    using System;
    using System.IO;

    public sealed class ScaledRAFile : IScaledRAInterface, IStorage, IDisposable
    {
        public const int DataFileRaf = 0;
        public const int DataFileNio = 1;
        public const int DataFileAssembly = 2;
        public const int DataFileStored = 3;
        public const long MaxNioLength = 0x10000000L;
        public static int CacheHit;
        private readonly Database _database;
        public bool ReadOnly;
        public ByteArrayInputStream Ba;
        public byte[] Buffer;
        public bool BufferDirty;
        public long BufferOffset;
        public Stream File;
        private long _fileLength;
        public string FileName;
        public bool IsNio;
        public long RealPosition;
        public long SeekPosition;

        public ScaledRAFile(Database database, string name, bool rdy)
        {
            this.BufferDirty = true;
            this._database = database;
            this.ReadOnly = rdy;
            this.FileName = name;
            this.File = database.logger.GetFileAccess().OpenOutputStreamElement(name, FileMode.OpenOrCreate, rdy ? FileAccess.Read : FileAccess.ReadWrite);
            int num = 0x1000;
            this.Buffer = new byte[num];
            this.Ba = new ByteArrayInputStream(this.Buffer);
            this._fileLength = this.Length();
        }

        public ScaledRAFile(Database database, string name, Stream file, bool rdy)
        {
            this.BufferDirty = true;
            this._database = database;
            this.ReadOnly = rdy;
            this.FileName = name;
            this.File = file;
            int num = 0x1000000;
            this.Buffer = new byte[num];
            this.Ba = new ByteArrayInputStream(this.Buffer);
            this._fileLength = this.Length();
        }

        public bool CanAccess(int length)
        {
            return true;
        }

        public bool CanSeek(long position)
        {
            return true;
        }

        public void Close()
        {
            Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.ScaledRAFile_Close_cache_hit_ + CacheHit);
            this.File.Dispose();
            this.Ba.Dispose();
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

        public Database GetDatabase()
        {
            return null;
        }

        public long GetFilePointer()
        {
            return this.SeekPosition;
        }

        public bool IsReadOnly()
        {
            return this.ReadOnly;
        }

        public long Length()
        {
            return this.File.Length;
        }

        public static IScaledRAInterface NewScaledRAFile(Database database, string name, bool rdy, bool inAssembly)
        {
            if (inAssembly)
            {
                return new ScaledRAFileInAssembly(name);
            }
            return new ScaledRAFile(database, name, rdy);
        }

        public int Read()
        {
            int num;
            try
            {
                if (this.SeekPosition >= this._fileLength)
                {
                    return -1;
                }
                if ((this.BufferDirty || (this.SeekPosition < this.BufferOffset)) || (this.SeekPosition >= (this.BufferOffset + this.Buffer.Length)))
                {
                    this.ReadIntoBuffer();
                }
                else
                {
                    CacheHit++;
                }
                this.Ba.Reset();
                this.Ba.Skip(this.SeekPosition - this.BufferOffset);
                this.SeekPosition += 1L;
                num = this.Ba.Read();
            }
            catch (IOException exception)
            {
                this.ResetPointer();
                this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.ScaledRAFile_Read_read_failed, exception);
                throw;
            }
            return num;
        }

        public void Read(byte[] b, int offset, int length)
        {
            try
            {
                if ((this.BufferDirty || (this.SeekPosition < this.BufferOffset)) || (this.SeekPosition >= (this.BufferOffset + this.Buffer.Length)))
                {
                    this.ReadIntoBuffer();
                }
                else
                {
                    CacheHit++;
                }
                this.Ba.Reset();
                if ((this.SeekPosition - this.BufferOffset) != this.Ba.Skip(this.SeekPosition - this.BufferOffset))
                {
                    throw new InvalidOperationException();
                }
                int num = this.Ba.Read(b, offset, length);
                this.SeekPosition += num;
                if (num < length)
                {
                    if (this.SeekPosition != this.RealPosition)
                    {
                        this.File.Seek(this.SeekPosition, SeekOrigin.Begin);
                    }
                    this.File.Read(b, offset + num, length - num);
                    this.SeekPosition += length - num;
                    this.RealPosition = this.SeekPosition;
                }
            }
            catch (IOException exception)
            {
                this.ResetPointer();
                this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.ScaledRAFile_Read_faeild_to_read_a_byte_array, exception);
                throw;
            }
        }

        public int ReadInt()
        {
            int num;
            try
            {
                int num2;
                if ((this.BufferDirty || (this.SeekPosition < this.BufferOffset)) || (this.SeekPosition >= (this.BufferOffset + this.Buffer.Length)))
                {
                    this.ReadIntoBuffer();
                }
                else
                {
                    CacheHit++;
                }
                this.Ba.Reset();
                if ((this.SeekPosition - this.BufferOffset) != this.Ba.Skip(this.SeekPosition - this.BufferOffset))
                {
                    throw new InvalidOperationException();
                }
                try
                {
                    num2 = this.Ba.ReadInt();
                }
                catch (InvalidOperationException)
                {
                    this.File.Seek(this.SeekPosition, SeekOrigin.Begin);
                    byte[] buffer = new byte[4];
                    this.File.Read(buffer, 0, 4);
                    num2 = BitConverter.ToInt32(buffer, 0);
                    this.RealPosition = this.File.Position;
                }
                this.SeekPosition += 4L;
                num = num2;
            }
            catch (IOException exception)
            {
                this.ResetPointer();
                this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.ScaledRAFile_ReadInt_failed_to_read_an_Int, exception);
                throw;
            }
            return num;
        }

        private void ReadIntoBuffer()
        {
            long seekPosition = this.SeekPosition;
            long num2 = seekPosition % ((long) this.Buffer.Length);
            long length = this._fileLength - (seekPosition - num2);
            try
            {
                if (length <= 0L)
                {
                    throw new IOException("read beyond end of file");
                }
                if (length > this.Buffer.Length)
                {
                    length = this.Buffer.Length;
                }
                if (this.RealPosition != (seekPosition - num2))
                {
                    this.File.Seek(seekPosition - num2, SeekOrigin.Begin);
                }
                int count = (int) length;
                int offset = 0;
                do
                {
                    int num6 = this.File.Read(this.Buffer, offset, count);
                    count -= num6;
                    offset += num6;
                }
                while (count > 0);
                this.BufferOffset = seekPosition - num2;
                this.RealPosition = this.BufferOffset + length;
                this.BufferDirty = false;
            }
            catch (IOException exception)
            {
                this.ResetPointer();
                this._database.logger.LogWarningEvent(string.Concat(new object[] { FwNs.Core.LC.cResources.SR.Single_Space, this.RealPosition, FwNs.Core.LC.cResources.SR.Single_Space, length }), exception);
                throw;
            }
        }

        public long ReadLong()
        {
            long num;
            try
            {
                long num2;
                if ((this.BufferDirty || (this.SeekPosition < this.BufferOffset)) || (this.SeekPosition >= (this.BufferOffset + this.Buffer.Length)))
                {
                    this.ReadIntoBuffer();
                }
                else
                {
                    CacheHit++;
                }
                this.Ba.Reset();
                if ((this.SeekPosition - this.BufferOffset) != this.Ba.Skip(this.SeekPosition - this.BufferOffset))
                {
                    throw new InvalidOperationException();
                }
                try
                {
                    num2 = this.Ba.ReadLong();
                }
                catch (InvalidOperationException)
                {
                    this.File.Seek(this.SeekPosition, SeekOrigin.Begin);
                    byte[] buffer = new byte[8];
                    this.File.Read(buffer, 0, 8);
                    num2 = BitConverter.ToInt64(buffer, 0);
                    this.RealPosition = this.File.Position;
                }
                this.SeekPosition += 8L;
                num = num2;
            }
            catch (IOException exception)
            {
                this.ResetPointer();
                this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.ScaledRAFile_ReadLong_failed_ot_read_a_Long, exception);
                throw;
            }
            return num;
        }

        private void ResetPointer()
        {
            try
            {
                this.BufferDirty = true;
                this.File.Seek(this.SeekPosition, SeekOrigin.Begin);
                this._fileLength = this.Length();
                this.RealPosition = this.SeekPosition;
            }
            catch (Exception)
            {
            }
        }

        public void Seek(long position)
        {
            if (!this.ReadOnly && (this._fileLength < position))
            {
                long num = position - this._fileLength;
                if (num > 0x10000L)
                {
                    num = 0x10000L;
                }
                byte[] buffer = new byte[(int) num];
                try
                {
                    long offset = this._fileLength;
                    while (offset < (position - num))
                    {
                        this.File.Seek(offset, SeekOrigin.Begin);
                        this.File.Write(buffer, 0, (int) num);
                        offset += num;
                    }
                    this.File.Seek(offset, SeekOrigin.Begin);
                    this.File.Write(buffer, 0, (int) (position - offset));
                    this.RealPosition = position;
                    this._fileLength = position;
                }
                catch (IOException exception)
                {
                    this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.ScaledRAFile_Seek_seek_failed, exception);
                    throw;
                }
            }
            this.SeekPosition = position;
        }

        public void Synch()
        {
            try
            {
                this.File.Flush();
            }
            catch (IOException)
            {
            }
        }

        public bool WasNio()
        {
            return false;
        }

        public void Write(byte[] b, int off, int len)
        {
            try
            {
                if (this.RealPosition != this.SeekPosition)
                {
                    this.File.Seek(this.SeekPosition, SeekOrigin.Begin);
                    this.RealPosition = this.SeekPosition;
                }
                if ((this.SeekPosition < (this.BufferOffset + this.Buffer.Length)) && ((this.SeekPosition + len) > this.BufferOffset))
                {
                    this.BufferDirty = true;
                }
                this.File.Write(b, off, len);
                this.SeekPosition += len;
                this.RealPosition = this.SeekPosition;
                if (this.RealPosition > this._fileLength)
                {
                    this._fileLength = this.RealPosition;
                }
            }
            catch (IOException exception)
            {
                this.ResetPointer();
                this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.ScaledRAFile_Write_failed_to_write_a_byte_array, exception);
                throw;
            }
        }

        public void WriteInt(int i)
        {
            try
            {
                if (this.RealPosition != this.SeekPosition)
                {
                    this.File.Seek(this.SeekPosition, SeekOrigin.Begin);
                    this.RealPosition = this.SeekPosition;
                }
                if ((this.SeekPosition < (this.BufferOffset + this.Buffer.Length)) && ((this.SeekPosition + 4L) > this.BufferOffset))
                {
                    this.BufferDirty = true;
                }
                byte[] bytes = BitConverter.GetBytes(i);
                this.File.Write(bytes, 0, 4);
                this.SeekPosition += 4L;
                this.RealPosition = this.SeekPosition;
                if (this.RealPosition > this._fileLength)
                {
                    this._fileLength = this.RealPosition;
                }
            }
            catch (IOException exception)
            {
                this.ResetPointer();
                this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.ScaledRAFile_WriteInt_failed_to_write_an_int, exception);
                throw;
            }
        }

        public void WriteLong(long i)
        {
            try
            {
                if (this.RealPosition != this.SeekPosition)
                {
                    this.File.Seek(this.SeekPosition, SeekOrigin.Begin);
                    this.RealPosition = this.SeekPosition;
                }
                if ((this.SeekPosition < (this.BufferOffset + this.Buffer.Length)) && ((this.SeekPosition + 8L) > this.BufferOffset))
                {
                    this.BufferDirty = true;
                }
                byte[] bytes = BitConverter.GetBytes(i);
                this.File.Write(bytes, 0, 8);
                this.SeekPosition += 8L;
                this.RealPosition = this.SeekPosition;
                if (this.RealPosition > this._fileLength)
                {
                    this._fileLength = this.RealPosition;
                }
            }
            catch (IOException exception)
            {
                this.ResetPointer();
                this._database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.ScaledRAFile_WriteLong_failed_to_write_a_Long, exception);
                throw;
            }
        }
    }
}

