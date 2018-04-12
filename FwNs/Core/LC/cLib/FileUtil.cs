namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    public class FileUtil : UtlFileAccess, IDisposable
    {
        private readonly List<string> _lstDeleteOnExit = new List<string>();
        private static readonly UtlFileAccess fileUtil = new FileUtil();

        public override string CanonicalOrAbsolutePath(string path)
        {
            return Path.GetFullPath(path);
        }

        public override void CreateParentDirs(string filename)
        {
            MakeParentDirectories(new FileInfo(filename));
        }

        private static void Delete(string filename)
        {
            File.Delete(filename);
        }

        public override void DeleteOnExit(string file)
        {
            this._lstDeleteOnExit.Add(file);
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
                foreach (string str in this._lstDeleteOnExit)
                {
                    try
                    {
                        File.Delete(str);
                    }
                    catch
                    {
                    }
                }
            }
        }

        ~FileUtil()
        {
            this.Dispose(false);
        }

        public override string[] GetAllFiles(string dirName)
        {
            FileInfo[] files = new DirectoryInfo(dirName).GetFiles();
            string[] strArray = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                strArray[i] = files[i].FullName;
            }
            return strArray;
        }

        public static UtlFileAccess GetDefaultInstance()
        {
            return fileUtil;
        }

        public static UtlFileAccess GetFileAccessResource()
        {
            return new FileAccessRes();
        }

        public override IFileSync GetFileSync(Stream os)
        {
            return new FileSync((FileStream) os);
        }

        public override string GetParentDir(string filename)
        {
            return new FileInfo(filename).Directory.FullName;
        }

        public override bool IsStreamElement(string elementName)
        {
            return File.Exists(elementName);
        }

        public override string MakeDirectories(string path)
        {
            try
            {
                DirectoryInfo info1 = new DirectoryInfo(path);
                info1.Create();
                return info1.FullName;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static void MakeParentDirectories(FileInfo f)
        {
            DirectoryInfo directory = f.Directory;
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        public override Stream OpenInputStreamElement(string streamName)
        {
            Stream stream;
            try
            {
                stream = new FileStream(streamName, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite, 0x3e8000, FileOptions.SequentialScan);
            }
            catch (Exception exception1)
            {
                throw ToIOException(exception1);
            }
            return stream;
        }

        public override Stream OpenOutputStreamElement(string streamName)
        {
            return new FileStream(streamName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite, 0x3e8000, FileOptions.SequentialScan | FileOptions.WriteThrough);
        }

        public override Stream OpenOutputStreamElement(string streamName, FileMode mode, FileAccess access)
        {
            return new FileStream(streamName, mode, access, FileShare.Delete | FileShare.ReadWrite, 0x3e8000, FileOptions.SequentialScan | FileOptions.WriteThrough);
        }

        public override void RemoveElement(string filename)
        {
            if (this.IsStreamElement(filename))
            {
                Delete(filename);
            }
        }

        public override void RenameElement(string oldName, string newName)
        {
            this.RenameOverwrite(oldName, newName);
        }

        private void RenameOverwrite(string oldname, string newname)
        {
            this.RemoveElement(newname);
            if (this.IsStreamElement(oldname))
            {
                try
                {
                    File.Move(oldname, newname);
                }
                catch (IOException)
                {
                    GC.Collect();
                    Thread.Sleep(500);
                    File.Move(oldname, newname);
                }
            }
        }

        public static IOException ToIOException(Exception e)
        {
            IOException exception = e as IOException;
            if (exception != null)
            {
                return exception;
            }
            return new IOException(e.ToString());
        }

        public class FileSync : IFileSync, IDisposable
        {
            private readonly Stream _outStream;

            public FileSync(FileStream os)
            {
                this._outStream = os;
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                this._outStream.Close();
            }

            public void Sync()
            {
                this._outStream.Flush();
            }
        }
    }
}

