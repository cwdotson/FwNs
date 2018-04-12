namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cLib;
    using System;
    using System.IO;

    public sealed class DbBackup
    {
        private bool _abortUponModify = true;
        private DirectoryInfo _archiveDir;
        private DirectoryInfo _dbDir;
        private string _instanceName;
        private bool _overWrite;

        public DbBackup(DirectoryInfo archiveDir, string dbPath)
        {
            this._archiveDir = archiveDir;
            FileInfo info = new FileInfo(dbPath);
            this._dbDir = info.Directory;
            this._instanceName = info.Name;
        }

        public bool GetAbortUponModify()
        {
            return this._abortUponModify;
        }

        public bool GetOverWrite()
        {
            return this._overWrite;
        }

        public void SetAbortUponModify(bool abortUponModify)
        {
            this._abortUponModify = abortUponModify;
        }

        public void SetOverWrite(bool overWrite)
        {
            this._overWrite = overWrite;
        }

        public void Write()
        {
            object[] objArray1 = new object[] { this._dbDir.FullName, Path.DirectorySeparatorChar, this._instanceName, ".properties" };
            FileInfo info = new FileInfo(string.Concat(objArray1));
            object[] objArray2 = new object[] { this._dbDir.FullName, Path.DirectorySeparatorChar, this._instanceName, ".script" };
            FileInfo info2 = new FileInfo(string.Concat(objArray2));
            FileInfo[] infoArray1 = new FileInfo[6];
            infoArray1[0] = info;
            infoArray1[1] = info2;
            object[] objArray3 = new object[] { this._dbDir.FullName, Path.DirectorySeparatorChar, this._instanceName, ".backup" };
            infoArray1[2] = new FileInfo(string.Concat(objArray3));
            object[] objArray4 = new object[] { this._dbDir.FullName, Path.DirectorySeparatorChar, this._instanceName, ".data" };
            infoArray1[3] = new FileInfo(string.Concat(objArray4));
            object[] objArray5 = new object[] { this._dbDir.FullName, Path.DirectorySeparatorChar, this._instanceName, ".log" };
            infoArray1[4] = new FileInfo(string.Concat(objArray5));
            object[] objArray6 = new object[] { this._dbDir.FullName, Path.DirectorySeparatorChar, this._instanceName, ".lobs" };
            infoArray1[5] = new FileInfo(string.Concat(objArray6));
            FileInfo[] infoArray = infoArray1;
            bool[] flagArray = new bool[infoArray.Length];
            long ticks = DateTime.Now.Ticks;
            for (int i = 0; i < flagArray.Length; i++)
            {
                flagArray[i] = infoArray[i].Exists;
                if (!flagArray[i])
                {
                    if (i < 2)
                    {
                        throw new FileNotFoundException(string.Format("Required file is missing: {0}", infoArray[i].FullName));
                    }
                    infoArray[i] = null;
                }
            }
            if (this._abortUponModify)
            {
                Properties properties = new Properties();
                using (Stream stream = info.OpenRead())
                {
                    properties.Load(stream);
                }
                string property = properties.GetProperty("modified");
                if ((property != null) && (property.Equals("yes", StringComparison.OrdinalIgnoreCase) || property.Equals("true", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(string.Format("'modified' DB property is {0}", property));
                }
            }
            if (this._archiveDir.Exists)
            {
                if (!this._overWrite)
                {
                    throw new InvalidOperationException(string.Format("Archive Exists {0}", this._archiveDir.FullName));
                }
                this._archiveDir.Delete(true);
            }
            else
            {
                this._archiveDir.Create();
            }
            for (int j = 0; j < infoArray.Length; j++)
            {
                if ((infoArray[j] != null) && infoArray[j].Exists)
                {
                    string destFileName = Path.Combine(this._archiveDir.FullName, infoArray[j].Name);
                    infoArray[j].CopyTo(destFileName);
                }
            }
            if (this._abortUponModify)
            {
                try
                {
                    for (int k = 0; k < infoArray.Length; k++)
                    {
                        if ((infoArray[k] != null) && infoArray[k].Exists)
                        {
                            if (!flagArray[k])
                            {
                                throw new FileNotFoundException(string.Format("{0} disappeared after backup started", infoArray[k].FullName));
                            }
                            if (infoArray[k].LastWriteTime.Ticks > ticks)
                            {
                                throw new FileNotFoundException(string.Format("{0} changed after backup started", infoArray[k].FullName));
                            }
                        }
                        else if (flagArray[k])
                        {
                            throw new FileNotFoundException(string.Format("{0}  appeared after backup started", infoArray[k].FullName));
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    this._archiveDir.Delete();
                    throw;
                }
            }
        }
    }
}

