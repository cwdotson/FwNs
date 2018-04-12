namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cRowIO;
    using System;
    using System.IO;
    using System.Threading;

    public class DataFileCache : IDisposable
    {
        public const int FlagIsshadowed = 1;
        public const int FlagIssaved = 2;
        public const int FlagRowinfo = 3;
        public const int Flag190 = 4;
        public const int LongEmptySize = 4;
        public const int LongFreePosPos = 12;
        public const int LongEmptyIndexPos = 20;
        public const int FlagsPos = 0x1c;
        public const int MinInitialFreePos = 0x20;
        private const int InitIoBufferSize = 0x100;
        protected UtlFileAccess Fa;
        public DataFileBlockManager FreeBlocks;
        protected string DataFileName;
        protected string BackupFileName;
        protected Database database;
        public bool FileModified;
        public int CacheFileScale;
        protected bool CacheReadonly;
        protected int CachedRowPadding = 8;
        public int InitialFreePos = 0x20;
        protected bool hasRowInfo;
        protected IRowInputInterface RowIn;
        public IRowOutputInterface RowOut;
        public long MaxDataFileSize;
        protected IScaledRAInterface DataFile;
        public long FileFreePosition;
        protected int MaxCacheRows;
        protected long MaxCacheBytes;
        protected int MaxFreeBlocks;
        protected Cache cache;
        private RAShadowFile _shadowFile;
        public object Lock = new Dummy();

        public virtual void Add(ICachedObject obj)
        {
            lock (this.Lock)
            {
                int key = this.SetFilePos(obj);
                this.cache.Put(key, obj);
            }
        }

        public void BackupFile()
        {
            Monitor.Enter(this.Lock);
            try
            {
                if (this.database.logger.PropIncrementBackup)
                {
                    if (this.Fa.IsStreamElement(this.BackupFileName))
                    {
                        this.DeleteBackup();
                    }
                }
                else if (this.Fa.IsStreamElement(this.DataFileName))
                {
                    FileArchiver.Archive(this.DataFileName, this.BackupFileName + ".new", this.database.logger.GetFileAccess(), 1);
                }
            }
            catch (IOException exception)
            {
                this.database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.DataFileCache_SetIncrementBackup_backupFile_failed, exception);
                throw Error.GetError(0x1d2, exception);
            }
            finally
            {
                Monitor.Exit(this.Lock);
            }
        }

        public long BytesCapacity()
        {
            return this.MaxCacheBytes;
        }

        public int Capacity()
        {
            return this.MaxCacheRows;
        }

        public virtual void Close(bool write)
        {
            Monitor.Enter(this.Lock);
            try
            {
                if (this.CacheReadonly)
                {
                    if (this.DataFile != null)
                    {
                        this.DataFile.Close();
                        this.DataFile = null;
                    }
                }
                else
                {
                    this.database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Close_DataFileCache_close_ + write.ToString() + FwNs.Core.LC.cResources.SR.DataFileCache_Close_____start);
                    if (write)
                    {
                        this.cache.SaveAll();
                        this.database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Close_DataFileCache_close_____save_data);
                        if (this.FileModified || this.FreeBlocks.IsModified())
                        {
                            this.DataFile.Seek(4L);
                            this.DataFile.WriteLong(this.FreeBlocks.GetLostBlocksSize());
                            this.DataFile.Seek(12L);
                            this.DataFile.WriteLong(this.FileFreePosition);
                            this.DataFile.Seek(0x1cL);
                            int i = BitMap.Set(this.DataFile.ReadInt(), 2);
                            this.DataFile.Seek(0x1cL);
                            this.DataFile.WriteInt(i);
                            this.database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Close_DataFileCache_close_____flags);
                            this.DataFile.Seek(this.FileFreePosition);
                            this.database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Close_DataFileCache_close_____seek_end);
                        }
                    }
                    if (this.DataFile != null)
                    {
                        this.DataFile.Close();
                        this.database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Close_DataFileCache_close_____close);
                        this.DataFile = null;
                    }
                    if (this._shadowFile != null)
                    {
                        this._shadowFile.Close();
                        this._shadowFile = null;
                    }
                    if (this.FileFreePosition == this.InitialFreePos)
                    {
                        this.DeleteFile();
                        this.DeleteBackup();
                    }
                }
            }
            catch (Exception exception)
            {
                this.database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Close_Close_failed, exception);
                object[] add = new object[] { exception.Message, this.DataFileName };
                throw Error.GetError(exception, 0x1c4, 0x35, add);
            }
            finally
            {
                Monitor.Exit(this.Lock);
            }
        }

        protected void CopyShadow(ICachedObject[] rows, int offset, int count)
        {
            if (this._shadowFile != null)
            {
                for (int i = offset; i < (offset + count); i++)
                {
                    ICachedObject obj2 = rows[i];
                    long fileOffset = obj2.GetPos() * this.CacheFileScale;
                    this._shadowFile.Copy(fileOffset, obj2.GetStorageSize());
                }
                this._shadowFile.Close();
            }
        }

        public void Defrag()
        {
            lock (this.Lock)
            {
                this.cache.SaveAll();
                using (DataFileDefrag defrag = new DataFileDefrag(this.database, this, this.DataFileName))
                {
                    defrag.Process();
                    this.Close(true);
                    this.cache.Clear();
                    if (!this.database.logger.PropIncrementBackup)
                    {
                        this.BackupFile();
                    }
                    this.database.schemaManager.SetTempIndexRoots(defrag.GetIndexRoots());
                    this.database.logger.log.WriteScript(false);
                    this.database.GetProperties().SetDbModified(2);
                    this.database.logger.log.CloseLog();
                    this.database.logger.log.DeleteLog();
                    this.database.logger.log.RenameNewScript();
                    this.RenameDataFile();
                    this.RenameBackupFile();
                    this.database.GetProperties().SetDbModified(0);
                    this.Open(false);
                    defrag.UpdateTransactionRowIDs();
                    this.database.schemaManager.SetIndexRoots(defrag.GetIndexRoots());
                    if (this.database.logger.log.DbLogWriter != null)
                    {
                        this.database.logger.log.OpenLog();
                    }
                    this.database.GetProperties().SetDbModified(1);
                }
            }
        }

        private void DeleteBackup()
        {
            lock (this.Lock)
            {
                if (this.Fa.IsStreamElement(this.BackupFileName))
                {
                    this.Fa.RemoveElement(this.BackupFileName);
                }
            }
        }

        private void DeleteFile()
        {
            lock (this.Lock)
            {
                this.Fa.RemoveElement(this.DataFileName);
                if (!this.database.logger.IsStoredFileAccess() && this.Fa.IsStreamElement(this.DataFileName))
                {
                    try
                    {
                        FileInfo info = new FileInfo(this.database.GetCanonicalPath());
                        FileInfo[] files = info.Directory.GetFiles();
                        for (int i = 0; i < files.Length; i++)
                        {
                            if (files[i].Name.EndsWith(".old") && files[i].Name.StartsWith(info.Name))
                            {
                                files[i].Delete();
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    this.Fa.RemoveElement(this.DataFileName);
                    if (this.Fa.IsStreamElement(this.DataFileName))
                    {
                        string newName = this.NewDiscardFileName();
                        this.Fa.RenameElement(this.DataFileName, newName);
                    }
                }
            }
        }

        public void DeleteOrResetFreePos()
        {
            this.DeleteFile();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._shadowFile != null)
                {
                    this._shadowFile.Dispose();
                }
                ((RowInputBinaryDecode) this.RowIn).Dispose();
                ((RowOutputBinaryEncode) this.RowOut).Dispose();
                if (this.DataFile != null)
                {
                    this.DataFile.Dispose();
                    this.DataFile = null;
                }
            }
        }

        public virtual ICachedObject Get(ICachedObject obj, IPersistentStore store, bool keep)
        {
            int pos;
            lock (this.Lock)
            {
                if (obj.IsInMemory())
                {
                    if (keep)
                    {
                        obj.KeepInMemory(true);
                    }
                    return obj;
                }
                pos = obj.GetPos();
                if (pos < 0)
                {
                    return null;
                }
                obj = this.cache.Get(pos);
                if (obj != null)
                {
                    if (keep)
                    {
                        obj.KeepInMemory(true);
                    }
                    return obj;
                }
            }
            return this.GetFromFile(pos, store, keep);
        }

        public virtual ICachedObject Get(int pos, IPersistentStore store, bool keep)
        {
            if (pos < 0)
            {
                return null;
            }
            lock (this.Lock)
            {
                ICachedObject obj3 = this.cache.Get(pos);
                if (obj3 != null)
                {
                    if (keep)
                    {
                        obj3.KeepInMemory(true);
                    }
                    return obj3;
                }
            }
            return this.GetFromFile(pos, store, keep);
        }

        public int GetAccessCount()
        {
            return this.cache.IncrementAccessCount();
        }

        public int GetCachedObjectCount()
        {
            return this.cache.Size();
        }

        public long GetFileFreePos()
        {
            return this.FileFreePosition;
        }

        public string GetFileName()
        {
            return this.DataFileName;
        }

        public int GetFreeBlockCount()
        {
            return this.FreeBlocks.Size();
        }

        private ICachedObject GetFromFile(int pos, IPersistentStore store, bool keep)
        {
            ICachedObject obj2;
            Monitor.Enter(this.Lock);
            try
            {
                ICachedObject row = this.cache.Get(pos);
                if (row != null)
                {
                    if (keep)
                    {
                        row.KeepInMemory(true);
                    }
                    return row;
                }
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        IRowInputInterface interface2 = this.ReadObject(pos);
                        if (interface2 == null)
                        {
                            return null;
                        }
                        row = store.Get(interface2);
                        break;
                    }
                    catch (OutOfMemoryException)
                    {
                        this.cache.ForceCleanUp();
                        if (i > 0)
                        {
                            throw;
                        }
                    }
                }
                pos = row.GetPos();
                this.cache.Put(pos, row);
                if (keep)
                {
                    row.KeepInMemory(true);
                }
                store.Set(row);
                obj2 = row;
            }
            catch (CoreException exception)
            {
                this.database.logger.LogSevereEvent(this.DataFileName + FwNs.Core.LC.cResources.SR.DataFileCache_GetFromFile__getFromFile_ + pos, exception);
                throw;
            }
            finally
            {
                Monitor.Exit(this.Lock);
            }
            return obj2;
        }

        public long GetTotalCachedBlockSize()
        {
            return this.cache.GetTotalCachedBlockSize();
        }

        public int GetTotalFreeBlockSize()
        {
            return 0;
        }

        public bool HasRowInfo()
        {
            return this.hasRowInfo;
        }

        protected virtual void InitBuffers()
        {
            if ((this.RowOut == null) || (this.RowOut.GetOutputStream().GetBuffer().Length > 0x100))
            {
                this.RowOut = new RowOutputBinaryEncode(this.database.logger.GetCrypto(), 0x100, this.CachedRowPadding);
            }
            if ((this.RowIn == null) || (this.RowIn.GetBuffer().Length > 0x100))
            {
                this.RowIn = new RowInputBinaryDecode(this.database.logger.GetCrypto(), new byte[0x100]);
            }
        }

        private void InitNewFile()
        {
            this.FileFreePosition = this.InitialFreePos;
            this.DataFile.Seek(12L);
            this.DataFile.WriteLong((long) this.InitialFreePos);
            int map = 0;
            if (this.database.logger.PropIncrementBackup)
            {
                map = BitMap.Set(map, 1);
            }
            map = BitMap.Set(map, 4);
            this.DataFile.Seek(0x1cL);
            this.DataFile.WriteInt(map);
            this.DataFile.Synch();
        }

        public virtual void InitParams(Database database, string baseFileName)
        {
            this.DataFileName = baseFileName + ".data";
            this.BackupFileName = baseFileName + ".backup";
            this.database = database;
            this.Fa = database.logger.GetFileAccess();
            this.CacheFileScale = database.logger.GetCacheFileScale();
            this.CachedRowPadding = 8;
            if (this.CacheFileScale > 8)
            {
                this.CachedRowPadding = this.CacheFileScale;
            }
            if (this.InitialFreePos < this.CacheFileScale)
            {
                this.InitialFreePos = this.CacheFileScale;
            }
            this.CacheReadonly = database.logger.PropFilesReadOnly;
            this.MaxCacheRows = database.logger.PropCacheMaxRows;
            this.MaxCacheBytes = database.logger.PropCacheMaxSize;
            this.MaxDataFileSize = 0x7fffffffL * this.CacheFileScale;
            this.MaxFreeBlocks = database.logger.PropMaxFreeBlocks;
            this.DataFile = null;
            this._shadowFile = null;
            this.cache = new Cache(this);
        }

        public bool IsDataReadOnly()
        {
            return this.CacheReadonly;
        }

        public bool IsFileModified()
        {
            return this.FileModified;
        }

        public bool IsFileOpen()
        {
            return (this.DataFile > null);
        }

        private string NewDiscardFileName()
        {
            string str = StringUtil.ToPaddedString(string.Format("{0:X}", DateTime.Now.Ticks / 0x2710L), 8, '0', true);
            return (this.DataFileName + "." + str + ".old");
        }

        public virtual void Open(bool rdy)
        {
            this.FileFreePosition = 0L;
            this.database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Open_open_start);
            try
            {
                if (rdy || this.database.IsFilesInAssembly())
                {
                    this.DataFile = ScaledRAFile.NewScaledRAFile(this.database, this.DataFileName, rdy, this.database.IsFilesInAssembly());
                    this.InitBuffers();
                }
                else
                {
                    bool flag = false;
                    long lostSize = 0L;
                    if (this.Fa.IsStreamElement(this.DataFileName))
                    {
                        flag = true;
                    }
                    this.DataFile = ScaledRAFile.NewScaledRAFile(this.database, this.DataFileName, rdy, this.database.IsFilesInAssembly());
                    if (flag)
                    {
                        this.DataFile.Seek(0x1cL);
                        int map = this.DataFile.ReadInt();
                        this.database.logger.PropIncrementBackup = BitMap.IsSet(map, 1);
                        if (!BitMap.IsSet(map, 2))
                        {
                            bool flag2;
                            this.DataFile.Close();
                            if (this.database.logger.PropIncrementBackup)
                            {
                                flag2 = this.RestoreBackupIncremental();
                            }
                            else
                            {
                                flag2 = this.RestoreBackup();
                            }
                            this.DataFile = ScaledRAFile.NewScaledRAFile(this.database, this.DataFileName, rdy, this.database.IsFilesInAssembly());
                            if (!flag2)
                            {
                                this.InitNewFile();
                            }
                        }
                        this.DataFile.Seek(4L);
                        lostSize = this.DataFile.ReadLong();
                        this.DataFile.Seek(12L);
                        this.FileFreePosition = this.DataFile.ReadLong();
                        if (this.FileFreePosition < this.InitialFreePos)
                        {
                            this.FileFreePosition = this.InitialFreePos;
                        }
                        if (this.database.logger.PropIncrementBackup && (this.FileFreePosition != this.InitialFreePos))
                        {
                            this._shadowFile = new RAShadowFile(this.database, this.DataFile, this.BackupFileName, this.FileFreePosition, 0x4000);
                        }
                    }
                    else
                    {
                        this.InitNewFile();
                    }
                    this.InitBuffers();
                    this.FileModified = false;
                    this.FreeBlocks = new DataFileBlockManager(this.MaxFreeBlocks, this.CacheFileScale, lostSize);
                    this.database.logger.LogInfoEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Open_open_end);
                }
            }
            catch (Exception exception)
            {
                this.database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.DataFileCache_Open_open_failed, exception);
                this.Close(false);
                object[] add = new object[] { exception.Message, this.DataFileName };
                throw Error.GetError(exception, 0x1c4, 0x34, add);
            }
        }

        protected virtual IRowInputInterface ReadObject(int pos)
        {
            IRowInputInterface rowIn;
            try
            {
                this.DataFile.Seek(pos * this.CacheFileScale);
                int size = this.DataFile.ReadInt();
                this.RowIn.ResetRow(pos, size);
                this.DataFile.Read(this.RowIn.GetBuffer(), 4, size - 4);
                rowIn = this.RowIn;
            }
            catch (IOException exception)
            {
                throw Error.GetError(0x1d2, exception);
            }
            return rowIn;
        }

        public ICachedObject Release(int pos)
        {
            lock (this.Lock)
            {
                return this.cache.Release(pos);
            }
        }

        public virtual void Remove(int i, IPersistentStore store)
        {
            lock (this.Lock)
            {
                ICachedObject obj2 = this.Release(i);
                if (obj2 != null)
                {
                    int storageSize = obj2.GetStorageSize();
                    this.FreeBlocks.Add(i, storageSize);
                }
            }
        }

        public virtual void RemovePersistence(int i, IPersistentStore store)
        {
        }

        private void RenameBackupFile()
        {
            lock (this.Lock)
            {
                if (this.database.logger.PropIncrementBackup)
                {
                    if (this.Fa.IsStreamElement(this.BackupFileName))
                    {
                        this.DeleteBackup();
                    }
                }
                else if (this.Fa.IsStreamElement(this.BackupFileName + ".new"))
                {
                    this.DeleteBackup();
                    this.Fa.RenameElement(this.BackupFileName + ".new", this.BackupFileName);
                }
            }
        }

        private void RenameDataFile()
        {
            lock (this.Lock)
            {
                if (this.Fa.IsStreamElement(this.DataFileName + ".new"))
                {
                    this.DeleteFile();
                    this.Fa.RenameElement(this.DataFileName + ".new", this.DataFileName);
                }
            }
        }

        private bool RestoreBackup()
        {
            bool flag;
            this.DeleteOrResetFreePos();
            try
            {
                UtlFileAccess fileAccess = this.database.logger.GetFileAccess();
                if (fileAccess.IsStreamElement(this.BackupFileName))
                {
                    FileArchiver.Unarchive(this.BackupFileName, this.DataFileName, fileAccess, 1);
                    return true;
                }
                flag = false;
            }
            catch (Exception exception)
            {
                object[] add = new object[] { exception.Message, this.BackupFileName };
                throw Error.GetError(exception, 0x1c4, 0x1a, add);
            }
            return flag;
        }

        private bool RestoreBackupIncremental()
        {
            bool flag;
            try
            {
                if (this.Fa.IsStreamElement(this.BackupFileName))
                {
                    RAShadowFile.RestoreFile(this.database, this.BackupFileName, this.DataFileName);
                    this.DeleteBackup();
                    return true;
                }
                flag = false;
            }
            catch (IOException exception)
            {
                throw Error.GetError(0x1c4, exception);
            }
            return flag;
        }

        private void SaveRowNoLock(ICachedObject row)
        {
            try
            {
                this.RowOut.Reset();
                row.Write(this.RowOut);
                this.DataFile.Seek(row.GetPos() * this.CacheFileScale);
                this.DataFile.Write(this.RowOut.GetOutputStream().GetBuffer(), 0, this.RowOut.GetOutputStream().Size());
            }
            catch (IOException exception)
            {
                throw Error.GetError(0x1d2, exception);
            }
        }

        public virtual void SaveRows(ICachedObject[] rows, int offset, int count)
        {
            try
            {
                this.CopyShadow(rows, offset, count);
                this.SetFileModified();
                for (int i = offset; i < (offset + count); i++)
                {
                    ICachedObject row = rows[i];
                    this.SaveRowNoLock(row);
                    rows[i] = null;
                }
            }
            catch (CoreException exception)
            {
                this.database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.DataFileCache_SaveRows_saveRows_failed, exception);
                throw;
            }
            catch (Exception exception2)
            {
                this.database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.DataFileCache_SaveRows_saveRows_failed, exception2);
                throw Error.GetError(0x1d2, exception2);
            }
            finally
            {
                this.InitBuffers();
            }
        }

        protected virtual void SetFileModified()
        {
            Monitor.Enter(this.Lock);
            try
            {
                if (!this.FileModified)
                {
                    this.DataFile.Seek(0x1cL);
                    int i = BitMap.Unset(this.DataFile.ReadInt(), 2);
                    this.DataFile.Seek(0x1cL);
                    this.DataFile.WriteInt(i);
                    this.DataFile.Synch();
                    this.FileModified = true;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                Monitor.Exit(this.Lock);
            }
        }

        public int SetFilePos(ICachedObject r)
        {
            int storageSize = r.GetStorageSize();
            int pos = (this.FreeBlocks == null) ? -1 : this.FreeBlocks.Get(storageSize);
            if (pos == -1)
            {
                pos = (int) (this.FileFreePosition / ((long) this.CacheFileScale));
                long num3 = this.FileFreePosition + storageSize;
                if (num3 > this.MaxDataFileSize)
                {
                    throw Error.GetError(0x1d4);
                }
                this.FileFreePosition = num3;
            }
            r.SetPos(pos);
            return pos;
        }

        public void SetIncrementBackup(bool value)
        {
            Monitor.Enter(this.Lock);
            try
            {
                this.DataFile.Seek(0x1cL);
                int map = this.DataFile.ReadInt();
                map = value ? BitMap.Set(map, 1) : BitMap.Unset(map, 1);
                this.DataFile.Seek(0x1cL);
                this.DataFile.WriteInt(map);
                this.DataFile.Synch();
            }
            catch (Exception exception)
            {
                this.database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.DataFileCache_SetIncrementBackup_backupFile_failed, exception);
            }
            finally
            {
                Monitor.Exit(this.Lock);
            }
        }

        private class Dummy
        {
        }
    }
}

