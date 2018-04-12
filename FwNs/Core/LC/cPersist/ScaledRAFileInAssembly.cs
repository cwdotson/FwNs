namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cLib.IO;
    using System;
    using System.IO;
    using System.Reflection;

    public class ScaledRAFileInAssembly : IScaledRAInterface, IStorage, IDisposable
    {
        private Stream file;
        private string fileName;
        private long fileLength;
        private bool bufferDirty = true;
        private byte[] buffer = new byte[0x1000];
        private ByteArrayInputStream ba;
        private long bufferOffset;
        private long seekPosition;
        private long realPosition;

        public ScaledRAFileInAssembly(string name)
        {
            this.fileName = name;
            this.ResetStream();
            this.fileLength = this.GetLength();
        }

        public bool CanAccess(int length)
        {
            return false;
        }

        public bool CanSeek(long position)
        {
            return false;
        }

        public void Close()
        {
            this.file.Close();
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
            this.file.Seek(position, SeekOrigin.Begin);
        }

        public Database GetDatabase()
        {
            return null;
        }

        public long GetFilePointer()
        {
            return this.file.Position;
        }

        private long GetLength()
        {
            return this.file.Length;
        }

        public bool IsReadOnly()
        {
            return true;
        }

        public long Length()
        {
            return this.fileLength;
        }

        public int Read()
        {
            return this.file.ReadByte();
        }

        public void Read(byte[] b, int offset, int length)
        {
            int num2;
            int count = length;
            do
            {
                num2 = this.file.Read(b, offset, count);
                count -= num2;
                offset += num2;
            }
            while (num2 > 0);
        }

        public int ReadInt()
        {
            byte[] b = new byte[4];
            this.Read(b, 0, 4);
            return BitConverter.ToInt32(b, 0);
        }

        public long ReadLong()
        {
            byte[] b = new byte[8];
            this.Read(b, 0, 8);
            return (long) BitConverter.ToInt32(b, 0);
        }

        private void ResetStream()
        {
            if (this.file != null)
            {
                this.file.Close();
            }
            Stream manifestResourceStream = null;
            try
            {
                int index = this.fileName.IndexOf(":");
                if (index != -1)
                {
                    this.fileName = this.fileName.Substring(index + 1);
                    manifestResourceStream = Assembly.Load(this.fileName.Substring(0, index)).GetManifestResourceStream(this.fileName);
                }
                else
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        manifestResourceStream = assemblies[i].GetManifestResourceStream(this.fileName);
                        if (manifestResourceStream != null)
                        {
                            goto Label_009A;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw Error.GetError(0x1d1, exception);
            }
        Label_009A:
            if (manifestResourceStream == null)
            {
                manifestResourceStream = new MemoryStream();
            }
            this.file = manifestResourceStream;
        }

        public void Seek(long position)
        {
            this.file.Seek(position, SeekOrigin.Begin);
        }

        public void Srite(byte[] b, int off, int len)
        {
        }

        public void SriteInt(int i)
        {
        }

        public void SriteLong(long i)
        {
        }

        public void Synch()
        {
        }

        public bool WasNio()
        {
            return false;
        }

        public void Write(byte[] b, int off, int len)
        {
        }

        public void WriteInt(int i)
        {
        }

        public void WriteLong(long i)
        {
        }
    }
}

