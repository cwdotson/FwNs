namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public sealed class DataFileDefrag : IDisposable
    {
        private readonly DataFileCache _cache;
        private readonly string _dataFileName;
        private readonly Database _database;
        private readonly int _scale;
        private readonly StopWatch _stopw = new StopWatch();
        private long _fileOffset;
        private Stream _fileStreamOut;
        private int[][] _rootsList;
        private DoubleIntIndex _transactionRowLookup;

        public DataFileDefrag(Database db, DataFileCache cache, string dataFileName)
        {
            this._database = db;
            this._cache = cache;
            this._scale = cache.CacheFileScale;
            this._dataFileName = dataFileName;
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
                this._fileStreamOut.Dispose();
            }
        }

        public int[][] GetIndexRoots()
        {
            return this._rootsList;
        }

        public void Process()
        {
            bool flag = false;
            Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.DataFileDefrag_Process_Defrag_process_begins);
            this._transactionRowLookup = this._database.TxManager.GetTransactionIDList();
            Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.DataFileDefrag_Process_transaction_count__ + this._transactionRowLookup.Size());
            List<Table> allTables = this._database.schemaManager.GetAllTables();
            this._rootsList = new int[allTables.Count][];
            IScaledRAInterface interface2 = null;
            try
            {
                Stream stream = this._database.logger.GetFileAccess().OpenOutputStreamElement(this._dataFileName + ".new");
                this._fileStreamOut = new BufferedStream(stream, 0x1000);
                for (int i = 0; i < this._cache.InitialFreePos; i++)
                {
                    this._fileStreamOut.WriteByte(0);
                }
                this._fileOffset = this._cache.InitialFreePos;
                int index = 0;
                int count = allTables.Count;
                while (index < count)
                {
                    Table table = allTables[index];
                    if (table.GetTableType() == 5)
                    {
                        this._rootsList[index] = this.WriteTableToDataFile(table);
                    }
                    else
                    {
                        this._rootsList[index] = null;
                    }
                    Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.DataFileDefrag_WriteTableToDataFile_table__ + table.GetName().Name + FwNs.Core.LC.cResources.SR.DataFileDefrag_Process__complete);
                    index++;
                }
                this._fileStreamOut.Flush();
                this._fileStreamOut.Close();
                this._fileStreamOut = null;
                interface2 = ScaledRAFile.NewScaledRAFile(this._database, this._dataFileName + ".new", false, this._database.IsFilesInAssembly());
                interface2.Seek(12L);
                interface2.WriteLong(this._fileOffset);
                int map = 0;
                if (this._database.logger.PropIncrementBackup)
                {
                    map = BitMap.Set(map, 1);
                }
                map = BitMap.Set(BitMap.Set(map, 4), 2);
                interface2.Seek(0x1cL);
                interface2.WriteInt(map);
                int num4 = 0;
                int length = this._rootsList.Length;
                while (num4 < length)
                {
                    int[] s = this._rootsList[num4];
                    if (s != null)
                    {
                        Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.DataFileDefrag_Process_roots__ + StringUtil.GetList(s, ",", ""));
                    }
                    num4++;
                }
                flag = true;
            }
            catch (IOException exception)
            {
                throw Error.GetError(0x1c4, exception);
            }
            catch (OutOfMemoryException exception2)
            {
                throw Error.GetError(460, exception2);
            }
            catch (Exception exception3)
            {
                throw Error.GetError(0x1ca, exception3);
            }
            finally
            {
                try
                {
                    if (this._fileStreamOut != null)
                    {
                        this._fileStreamOut.Close();
                    }
                    if (interface2 != null)
                    {
                        interface2.Dispose();
                    }
                }
                catch (Exception exception4)
                {
                    this._database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.DataFileDefrag_Process_backupFile_failed, exception4);
                }
                if (!flag)
                {
                    this._database.logger.GetFileAccess().RemoveElement(this._dataFileName + ".new");
                }
            }
            Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.DataFileDefrag_Process_Defrag_transfer_complete__ + this._stopw.ElapsedTime());
        }

        public void SetTransactionRowLookups(DoubleIntIndex pointerLookup)
        {
            int i = 0;
            int num2 = this._transactionRowLookup.Size();
            while (i < num2)
            {
                int key = this._transactionRowLookup.GetKey(i);
                int num4 = pointerLookup.FindFirstEqualKeyIndex(key);
                if (num4 != -1)
                {
                    this._transactionRowLookup.SetValue(i, pointerLookup.GetValue(num4));
                }
                i++;
            }
        }

        public void UpdateTransactionRowIDs()
        {
            this._database.TxManager.ConvertTransactionIDs(this._transactionRowLookup);
        }

        public int[] WriteTableToDataFile(Table table)
        {
            IPersistentStore rowStore = this._database.GetSessionManager().GetSysSession().sessionData.GetRowStore(table);
            IRowOutputInterface output = this._cache.RowOut.Duplicate();
            DoubleIntIndex lookup = new DoubleIntIndex(table.GetPrimaryIndex().SizeUnique(rowStore), false);
            int[] indexRootsArray = table.GetIndexRootsArray();
            long num = this._fileOffset;
            int num2 = 0;
            lookup.SetKeysSearchTarget();
            Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.DataFileDefrag_WriteTableToDataFile_lookup_begins__ + this._stopw.ElapsedTime());
            IRowIterator rowIterator = table.GetRowIterator(rowStore);
            while (rowIterator.HasNext())
            {
                ICachedObject nextRow = rowIterator.GetNextRow();
                lookup.AddUnsorted(nextRow.GetPos(), (int) (num / ((long) this._scale)));
                if ((num2 % 0xc350) == 0)
                {
                    Error.PrintSystemOut(string.Concat(new object[] { FwNs.Core.LC.cResources.SR.DataFileDefrag_WriteTableToDataFile_pointer_pair_for_row_, num2, FwNs.Core.LC.cResources.SR.Single_Space, nextRow.GetPos(), FwNs.Core.LC.cResources.SR.Single_Space, num }));
                }
                num += nextRow.GetStorageSize();
                num2++;
            }
            Error.PrintSystemOut(string.Concat(new object[] { FwNs.Core.LC.cResources.SR.DataFileDefrag_WriteTableToDataFile_table__, table.GetName().Name, FwNs.Core.LC.cResources.SR.DataFileDefrag_WriteTableToDataFile__list_done__, this._stopw.ElapsedTime() }));
            num2 = 0;
            rowIterator = table.GetRowIterator(rowStore);
            while (rowIterator.HasNext())
            {
                ICachedObject nextRow = rowIterator.GetNextRow();
                output.Reset();
                nextRow.Write(output, lookup);
                this._fileStreamOut.Write(output.GetOutputStream().GetBuffer(), 0, output.Size());
                this._fileOffset += nextRow.GetStorageSize();
                if ((num2 % 0xc350) == 0)
                {
                    Error.PrintSystemOut(num2 + FwNs.Core.LC.cResources.SR.DataFileDefrag_WriteTableToDataFile__rows_ + this._stopw.ElapsedTime());
                }
                num2++;
            }
            for (int i = 0; i < table.GetIndexCount(); i++)
            {
                if (indexRootsArray[i] != -1)
                {
                    int num4 = lookup.FindFirstEqualKeyIndex(indexRootsArray[i]);
                    if (num4 == -1)
                    {
                        throw Error.GetError(0x1d2);
                    }
                    indexRootsArray[i] = lookup.GetValue(num4);
                }
            }
            this.SetTransactionRowLookups(lookup);
            Error.PrintSystemOut(FwNs.Core.LC.cResources.SR.DataFileDefrag_WriteTableToDataFile_table__ + table.GetName().Name + FwNs.Core.LC.cResources.SR.DataFileDefrag_WriteTableToDataFile____table_converted);
            return indexRootsArray;
        }
    }
}

