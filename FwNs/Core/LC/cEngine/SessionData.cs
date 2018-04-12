namespace FwNs.Core.LC.cEngine
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public sealed class SessionData : IDisposable
    {
        private readonly Database _database;
        private readonly Session _session;
        public PersistentStoreCollectionSession persistentStoreCollection;
        private Dictionary<long, Result> _resultMap;
        private DataFileCacheSession _resultCache;
        public object CurrentValue;
        private HashMap<QNameManager.QName, object> _sequenceMap;
        public OrderedHashSet<NumberSequence> SequenceUpdateSet;
        private readonly Dictionary<long, long> _resultLobs = new Dictionary<long, long>();

        public SessionData(Database database, Session session)
        {
            this._database = database;
            this._session = session;
            this.persistentStoreCollection = new PersistentStoreCollectionSession(session);
        }

        public void AdjustLobUsageCount(object value, int adjust)
        {
            if ((!this._session.IsProcessingLog() && !this._session.IsProcessingScript()) && (value != null))
            {
                this._database.lobManager.AdjustUsageCount(((ILobData) value).GetId(), adjust);
            }
        }

        public void AdjustLobUsageCount(TableBase table, object[] data, int adjust)
        {
            this.AdjustLobUsageCount(table, data, adjust, false);
        }

        public void AdjustLobUsageCount(TableBase table, object[] data, int adjust, bool tx)
        {
            if ((table.hasLobColumn && !this._session.IsProcessingLog()) && !this._session.IsProcessingScript())
            {
                for (int i = 0; i < table.ColumnCount; i++)
                {
                    if (table.ColTypes[i].IsLobType())
                    {
                        object obj2 = data[i];
                        if (obj2 != null)
                        {
                            this._database.lobManager.AdjustUsageCount(((ILobData) obj2).GetId(), adjust, tx);
                        }
                    }
                }
            }
        }

        public void AllocateLobForResult(ResultLob result, Stream inputStream)
        {
            long lobId = result.GetLobId();
            switch (result.GetSubType())
            {
                case 7:
                {
                    long id;
                    long blockLength = result.GetBlockLength();
                    if (inputStream == null)
                    {
                        id = lobId;
                        inputStream = result.GetInputStream();
                    }
                    else
                    {
                        id = this._session.CreateBlob(blockLength).GetId();
                        this._resultLobs.Add(lobId, id);
                    }
                    this._database.lobManager.SetBytesForNewBlob(id, inputStream, result.GetBlockLength());
                    break;
                }
                case 8:
                {
                    long id;
                    long blockLength = result.GetBlockLength();
                    if (inputStream == null)
                    {
                        id = lobId;
                        if (result.GetReader() != null)
                        {
                            inputStream = new ReaderInputStream(result.GetReader());
                        }
                        else
                        {
                            inputStream = result.GetInputStream();
                        }
                    }
                    else
                    {
                        id = this._session.CreateClob(blockLength).GetId();
                        this._resultLobs.Add(lobId, id);
                    }
                    this._database.lobManager.SetCharsForNewClob(id, inputStream, result.GetBlockLength());
                    break;
                }
            }
        }

        public void CloseAllNavigators()
        {
            if (this._resultMap != null)
            {
                using (Dictionary<long, Result>.ValueCollection.Enumerator enumerator = this._resultMap.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.GetNavigator().Close();
                    }
                }
                this._resultMap.Clear();
            }
        }

        public void CloseAllTransactionNavigators()
        {
            if (this._resultMap != null)
            {
                List<long> list = new List<long>();
                foreach (KeyValuePair<long, Result> pair in this._resultMap)
                {
                    Result result = pair.Value;
                    if (!ResultProperties.IsHoldable(result.RsProperties))
                    {
                        result.GetNavigator().Close();
                        list.Add(pair.Key);
                    }
                }
                foreach (long num in list)
                {
                    this._resultMap.Remove(num);
                }
            }
        }

        public void CloseNavigator(long id)
        {
            this._resultMap.Remove(id);
            this._resultMap[id].GetNavigator().Close();
        }

        public void CloseResultCache()
        {
            lock (this)
            {
                if (this._resultCache != null)
                {
                    try
                    {
                        this._resultCache.Close(false);
                    }
                    catch (CoreException)
                    {
                    }
                    this._resultCache = null;
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
            if (disposing && (this._resultCache != null))
            {
                this._resultCache.Dispose();
            }
        }

        public Result GetDataResult(long id)
        {
            Result result;
            if (!this._resultMap.TryGetValue(id, out result))
            {
                return null;
            }
            return result;
        }

        public Result GetDataResultHead(Result command, Result result, bool isNetwork)
        {
            int fetchSize = command.GetFetchSize();
            result.SetResultId(this._session.ActionTimestamp);
            int rsProperties = command.RsProperties;
            int props = result.RsProperties;
            if (rsProperties != props)
            {
                if (ResultProperties.IsReadOnly(rsProperties))
                {
                    props = ResultProperties.AddHoldable(props, ResultProperties.IsHoldable(rsProperties));
                }
                else if (ResultProperties.IsReadOnly(props))
                {
                    props = ResultProperties.AddHoldable(props, ResultProperties.IsHoldable(rsProperties));
                }
                else if (this._session.IsAutoCommit())
                {
                    props = ResultProperties.AddHoldable(props, ResultProperties.IsHoldable(rsProperties));
                }
                else
                {
                    props = ResultProperties.AddHoldable(props, false);
                }
                props = ResultProperties.AddScrollable(props, ResultProperties.IsScrollable(rsProperties));
                result.RsProperties = props;
            }
            bool flag = false;
            bool flag2 = false;
            if (ResultProperties.IsUpdatable(result.RsProperties) || ResultProperties.IsHoldable(result.RsProperties))
            {
                flag = true;
            }
            if (isNetwork)
            {
                if ((fetchSize != 0) && (result.GetNavigator().GetSize() > fetchSize))
                {
                    flag2 = true;
                    flag = true;
                }
            }
            else if (!result.GetNavigator().IsMemory())
            {
                flag = true;
            }
            if (flag)
            {
                if (this._resultMap == null)
                {
                    this._resultMap = new Dictionary<long, Result>();
                }
                this._resultMap.Add(result.GetResultId(), result);
            }
            if (flag2)
            {
                result = Result.NewDataHeadResult(this._session, result, 0, fetchSize);
            }
            return result;
        }

        public Result GetDataResultSlice(long id, int offset, int count)
        {
            return Result.NewDataRowsResult(this.GetRowSetSlice(id, offset, count));
        }

        public IPersistentStore GetNewResultRowStore(TableBase table, bool isCached)
        {
            try
            {
                return this._session.database.logger.NewStore(this._session, this.persistentStoreCollection, table, isCached);
            }
            catch (CoreException)
            {
            }
            throw Error.RuntimeError(0xc9, "SessionData");
        }

        public DataFileCacheSession GetResultCache()
        {
            if (this._resultCache == null)
            {
                string tempDirectoryPath = this._database.logger.GetTempDirectoryPath();
                if (tempDirectoryPath == null)
                {
                    return null;
                }
                try
                {
                    this._resultCache = new DataFileCacheSession();
                    this._resultCache.InitParams(this._database, tempDirectoryPath + "/session_" + this._session.GetId());
                    this._resultCache.Open(false);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return this._resultCache;
        }

        public RowSetNavigatorClient GetRowSetSlice(long id, int offset, int count)
        {
            RowSetNavigator source = this._resultMap[id].GetNavigator();
            if ((offset + count) > source.GetSize())
            {
                count = source.GetSize() - offset;
            }
            return new RowSetNavigatorClient(source, offset, count);
        }

        public IPersistentStore GetRowStore(TableBase table)
        {
            if (table.TableType == 1)
            {
                if (this._session.IsAdmin())
                {
                    return table.Store;
                }
                return this.persistentStoreCollection.GetStore(table);
            }
            if (table.Store != null)
            {
                return table.Store;
            }
            if (table.isSessionBased)
            {
                return this.persistentStoreCollection.GetStore(table);
            }
            return this._database.persistentStoreCollection.GetStore(table);
        }

        public object GetSequenceValue(NumberSequence sequence)
        {
            if (this._sequenceMap == null)
            {
                this._sequenceMap = new HashMap<QNameManager.QName, object>();
                this.SequenceUpdateSet = new OrderedHashSet<NumberSequence>();
            }
            QNameManager.QName key = sequence.GetName();
            object valueObject = this._sequenceMap.Get(key);
            if (valueObject == null)
            {
                valueObject = sequence.GetValueObject();
                this._sequenceMap.Put(key, valueObject);
                this.SequenceUpdateSet.Add(sequence);
            }
            return valueObject;
        }

        public IPersistentStore GetSubqueryRowStore(TableBase table)
        {
            IPersistentStore store = this.persistentStoreCollection.GetStore(table);
            store.RemoveAll();
            return store;
        }

        public void SetResultSetProperties(Result command, Result result)
        {
            int rsProperties = command.RsProperties;
            int props = result.RsProperties;
            if (rsProperties != props)
            {
                if (ResultProperties.IsReadOnly(rsProperties))
                {
                    props = ResultProperties.AddHoldable(props, ResultProperties.IsHoldable(rsProperties));
                }
                else if (ResultProperties.IsUpdatable(props))
                {
                    if (ResultProperties.IsHoldable(rsProperties))
                    {
                        this._session.AddWarning(Error.GetError(0x1269));
                    }
                }
                else
                {
                    props = ResultProperties.AddHoldable(props, ResultProperties.IsHoldable(rsProperties));
                    this._session.AddWarning(Error.GetError(0x1268));
                }
                if (ResultProperties.IsSensitive(rsProperties))
                {
                    this._session.AddWarning(Error.GetError(0x1267));
                }
                props = ResultProperties.AddScrollable(props, ResultProperties.IsScrollable(rsProperties));
                result.RsProperties = props;
            }
        }

        public void StartRowProcessing()
        {
            if (this._sequenceMap != null)
            {
                this._sequenceMap.Clear();
            }
        }
    }
}

