namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cResources;
    using System;
    using System.Threading;

    public sealed class DataFileCacheSession : DataFileCache
    {
        public int StoreCount;

        public override void Add(ICachedObject obj)
        {
            lock (this)
            {
                base.Add(obj);
            }
        }

        public void Clear()
        {
            base.cache.Clear();
            base.FileFreePosition = 0x20L;
        }

        public override void Close(bool write)
        {
            Monitor.Enter(this);
            try
            {
                if (base.DataFile != null)
                {
                    base.DataFile.Close();
                    base.DataFile = null;
                    base.Fa.RemoveElement(base.DataFileName);
                }
            }
            catch (Exception exception)
            {
                base.database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.DataFileCacheSession_Close_Failed_to_close_RA_file, exception);
                object[] add = new object[] { exception.Message, base.DataFileName };
                throw Error.GetError(exception, 0x1c4, 0x35, add);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void InitParams(Database database, string baseFileName)
        {
            base.DataFileName = baseFileName + ".data.tmp";
            base.database = database;
            base.Fa = database.logger.GetFileAccess();
            int num = 10;
            base.MaxCacheRows = 0x800;
            base.CacheFileScale = 8;
            int num2 = ((int) 1) << num;
            base.MaxCacheBytes = base.MaxCacheRows * num2;
            base.MaxDataFileSize = 0x1fffffffcL;
            base.DataFile = null;
            base.cache = new Cache(this);
        }

        public override void Open(bool rdy)
        {
            try
            {
                base.DataFile = ScaledRAFile.NewScaledRAFile(base.database, base.DataFileName, rdy, base.database.IsFilesInAssembly());
                base.FileFreePosition = 0x20L;
                this.InitBuffers();
                base.FreeBlocks = new DataFileBlockManager(0, base.CacheFileScale, 0L);
            }
            catch (Exception exception)
            {
                base.database.logger.LogWarningEvent(FwNs.Core.LC.cResources.SR.DataFileCacheSession_Open_Failed_to_open_RA_file, exception);
                this.Close(false);
                object[] add = new object[] { exception.Message, base.DataFileName };
                throw Error.GetError(exception, 0x1c4, 0x34, add);
            }
        }
    }
}

