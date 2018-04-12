namespace FwNs.Core.LC.cLib
{
    using System;
    using System.IO;
    using System.Reflection;

    public class FileAccessRes : UtlFileAccess
    {
        public override string CanonicalOrAbsolutePath(string path)
        {
            throw new IOException();
        }

        public override void CreateParentDirs(string filename)
        {
        }

        public override void DeleteOnExit(string file)
        {
        }

        public override string[] GetAllFiles(string dirName)
        {
            throw new IOException();
        }

        public override IFileSync GetFileSync(Stream os)
        {
            throw new IOException();
        }

        public override string GetParentDir(string filename)
        {
            throw new IOException();
        }

        public override bool IsStreamElement(string elementName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].GetManifestResourceStream(elementName) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public override string MakeDirectories(string path)
        {
            throw new IOException();
        }

        public override Stream OpenInputStreamElement(string streamName)
        {
            Stream manifestResourceStream = null;
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    manifestResourceStream = assemblies[i].GetManifestResourceStream(streamName);
                    if (manifestResourceStream != null)
                    {
                        return manifestResourceStream;
                    }
                }
            }
            catch (Exception exception1)
            {
                throw FileUtil.ToIOException(exception1);
            }
            return manifestResourceStream;
        }

        public override Stream OpenOutputStreamElement(string streamName)
        {
            throw new IOException();
        }

        public override Stream OpenOutputStreamElement(string streamName, FileMode mode, FileAccess access)
        {
            if (access != FileAccess.Read)
            {
                throw new IOException();
            }
            return this.OpenInputStreamElement(streamName);
        }

        public override void RemoveElement(string filename)
        {
        }

        public override void RenameElement(string oldName, string newName)
        {
        }
    }
}

