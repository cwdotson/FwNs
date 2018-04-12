namespace FwNs.Core.LC.cLib
{
    using System;
    using System.IO;

    public abstract class UtlFileAccess
    {
        public const int ElementRead = 1;
        public const int ElementSeekableread = 3;
        public const int ElementWrite = 4;
        public const int ElementReadwrite = 7;
        public const int ElementTruncate = 8;

        protected UtlFileAccess()
        {
        }

        public abstract string CanonicalOrAbsolutePath(string path);
        public abstract void CreateParentDirs(string filename);
        public abstract void DeleteOnExit(string file);
        public abstract string[] GetAllFiles(string dirName);
        public abstract IFileSync GetFileSync(Stream s);
        public abstract string GetParentDir(string filename);
        public abstract bool IsStreamElement(string elementName);
        public abstract string MakeDirectories(string path);
        public abstract Stream OpenInputStreamElement(string streamName);
        public abstract Stream OpenOutputStreamElement(string streamName);
        public abstract Stream OpenOutputStreamElement(string streamName, FileMode mode, FileAccess access);
        public abstract void RemoveElement(string filename);
        public abstract void RenameElement(string oldName, string newName);
    }
}

