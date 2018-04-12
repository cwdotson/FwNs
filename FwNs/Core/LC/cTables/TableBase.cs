namespace FwNs.Core.LC.cTables
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cSchemas;
    using System;

    public class TableBase
    {
        public const int SystemTable = 1;
        public const int SystemSubquery = 2;
        public const int TempTable = 3;
        public const int MemoryTable = 4;
        public const int CachedTable = 5;
        public const int TempTextTable = 6;
        public const int TextTable = 7;
        public const int ViewTable = 8;
        public const int ResultTable = 9;
        public const int TransitionTable = 10;
        public const int FunctionTable = 11;
        public const int ScopeStatement = 0x15;
        public const int ScopeTransaction = 0x16;
        public const int ScopeSession = 0x17;
        public const int ScopeFull = 0x18;
        public IPersistentStore Store;
        public int PersistenceScope;
        public long PersistenceId;
        public int[] PrimaryKeyCols;
        public SqlType[] PrimaryKeyTypes;
        public int[] PrimaryKeyColsSequence;
        public Index[] IndexList;
        public Database database;
        public int[] BestRowIdentifierCols;
        public bool BestRowIdentifierStrict;
        public int[] BestIndexForColumn;
        public Index BestIndex;
        public Index FullIndex;
        public bool[] ColNotNull;
        public SqlType[] ColTypes;
        public int ColumnCount;
        public int TableType;
        protected bool _isReadOnly;
        protected bool _isTemp;
        protected bool _isCached;
        public bool _isView;
        public bool isSessionBased;
        protected bool _isSchemaBased;
        public bool isLogged;
        private bool _isTransactional;
        public bool hasLobColumn;

        public TableBase()
        {
            this._isTransactional = true;
        }

        public TableBase(Session session, Database database, int scope, int type, SqlType[] colTypes)
        {
            this._isTransactional = true;
            this.TableType = type;
            this.PersistenceScope = scope;
            this.isSessionBased = true;
            this.PersistenceId = database.persistentStoreCollection.GetNextId();
            this.database = database;
            this.ColTypes = colTypes;
            this.ColumnCount = colTypes.Length;
            this.PrimaryKeyCols = new int[0];
            this.PrimaryKeyTypes = new SqlType[0];
            this.IndexList = new Index[0];
            this.CreatePrimaryIndex(this.PrimaryKeyCols, this.PrimaryKeyTypes, null);
        }

        public void AddIndex(Session session, Index index)
        {
            int num = 0;
            while (num < this.IndexList.Length)
            {
                Index index2 = this.IndexList[num];
                if ((index.GetIndexOrderValue() - index2.GetIndexOrderValue()) < 0)
                {
                    break;
                }
                num++;
            }
            this.IndexList = ArrayUtil.ToAdjustedArray<Index>(this.IndexList, index, num, 1);
            for (num = 0; num < this.IndexList.Length; num++)
            {
                this.IndexList[num].SetPosition(num);
            }
            if (this.Store != null)
            {
                try
                {
                    this.Store.ResetAccessorKeys(this.IndexList);
                }
                catch (CoreException)
                {
                    this.IndexList = ArrayUtil.ToAdjustedArray<Index>(this.IndexList, null, index.GetPosition(), -1);
                    for (num = 0; num < this.IndexList.Length; num++)
                    {
                        this.IndexList[num].SetPosition(num);
                    }
                    throw;
                }
            }
            this.SetBestRowIdentifiers();
        }

        public virtual void ClearAllData(Session session)
        {
            session.sessionData.GetRowStore(this).RemoveAll();
        }

        public virtual void ClearAllData(IPersistentStore store)
        {
            store.RemoveAll();
        }

        public Index CreateAndAddIndexStructure(Session session, QNameManager.QName name, int[] columns, bool[] descending, bool[] nullsLast, bool unique, bool constraint, bool forward)
        {
            Index index = this.CreateIndexStructure(name, columns, descending, nullsLast, unique, constraint, forward);
            this.AddIndex(session, index);
            return index;
        }

        public Index CreateIndex(Session session, QNameManager.QName name, int[] columns, bool[] descending, bool[] nullsLast, bool unique, bool constraint, bool forward)
        {
            return this.CreateAndAddIndexStructure(session, name, columns, descending, nullsLast, unique, constraint, forward);
        }

        public Index CreateIndexStructure(QNameManager.QName name, int[] columns, bool[] descending, bool[] nullsLast, bool unique, bool constraint, bool forward)
        {
            if (this.PrimaryKeyCols == null)
            {
                throw Error.RuntimeError(0xc9, "createIndex");
            }
            int length = columns.Length;
            int[] numArray = new int[length];
            SqlType[] colTypes = new SqlType[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = columns[i];
                colTypes[i] = this.ColTypes[numArray[i]];
            }
            long nextId = this.database.persistentStoreCollection.GetNextId();
            return Logger.NewIndex(name, nextId, this, numArray, descending, nullsLast, colTypes, false, unique, constraint, forward);
        }

        public void CreatePrimaryIndex(int[] pkcols, SqlType[] pktypes, QNameManager.QName name)
        {
            long nextId = this.database.persistentStoreCollection.GetNextId();
            Index index = Logger.NewIndex(name, nextId, this, pkcols, null, null, pktypes, true, pkcols.Length > 0, pkcols.Length > 0, false);
            try
            {
                this.AddIndex(null, index);
            }
            catch (CoreException)
            {
            }
        }

        public void DropIndex(int todrop)
        {
            this.IndexList = ArrayUtil.ToAdjustedArray<Index>(this.IndexList, null, todrop, -1);
            for (int i = 0; i < this.IndexList.Length; i++)
            {
                this.IndexList[i].SetPosition(i);
            }
            this.SetBestRowIdentifiers();
            if (this.Store != null)
            {
                this.Store.ResetAccessorKeys(this.IndexList);
            }
        }

        public TableBase Duplicate()
        {
            return new TableBase { 
                TableType = this.TableType,
                PersistenceScope = this.PersistenceScope,
                isSessionBased = this.isSessionBased,
                PersistenceId = this.database.persistentStoreCollection.GetNextId(),
                database = this.database,
                ColTypes = this.ColTypes,
                ColumnCount = this.ColTypes.Length,
                PrimaryKeyCols = this.PrimaryKeyCols,
                PrimaryKeyTypes = this.PrimaryKeyTypes,
                IndexList = this.IndexList
            };
        }

        public int GetColumnCount()
        {
            return this.ColumnCount;
        }

        public SqlType[] GetColumnTypes()
        {
            return this.ColTypes;
        }

        public int GetDataColumnCount()
        {
            return this.ColTypes.Length;
        }

        public object[] GetEmptyRowData()
        {
            return new object[this.GetDataColumnCount()];
        }

        public Index GetFullIndex()
        {
            return this.FullIndex;
        }

        public virtual int GetId()
        {
            return 0;
        }

        public Index GetIndex(int i)
        {
            return this.IndexList[i];
        }

        public int GetIndexCount()
        {
            return this.IndexList.Length;
        }

        public Index[] GetIndexList()
        {
            return this.IndexList;
        }

        public bool[] GetNewColumnCheckList()
        {
            return new bool[this.ColumnCount];
        }

        public long GetPersistenceId()
        {
            return this.PersistenceId;
        }

        public Index GetPrimaryIndex()
        {
            if (this.IndexList.Length == 0)
            {
                return null;
            }
            return this.IndexList[0];
        }

        public int[] GetPrimaryKey()
        {
            return this.PrimaryKeyCols;
        }

        public SqlType[] GetPrimaryKeyTypes()
        {
            return this.PrimaryKeyTypes;
        }

        public int GetRowCount(IPersistentStore store)
        {
            return this.GetPrimaryIndex().Size(null, store);
        }

        public IRowIterator GetRowIterator(Session session)
        {
            IPersistentStore rowStore = session.sessionData.GetRowStore(this);
            return this.GetPrimaryIndex().FirstRow(session, rowStore);
        }

        public IRowIterator GetRowIterator(IPersistentStore store)
        {
            return this.GetPrimaryIndex().FirstRow(store);
        }

        public virtual IPersistentStore GetRowStore(Session session)
        {
            return (this.Store ?? session.sessionData.GetRowStore(this));
        }

        public int GetTableType()
        {
            return this.TableType;
        }

        public bool HasPrimaryKey()
        {
            return (this.PrimaryKeyCols.Length > 0);
        }

        public bool IsEmpty(Session session)
        {
            if (this.GetIndexCount() == 0)
            {
                return true;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(this);
            return this.GetIndex(0).IsEmpty(rowStore);
        }

        public bool IsTransactional()
        {
            return this._isTransactional;
        }

        public bool OnCommitPreserve()
        {
            return (this.PersistenceScope == 0x17);
        }

        public void RemoveIndex(int position)
        {
            this.SetBestRowIdentifiers();
        }

        public void SetBestRowIdentifiers()
        {
            int[] source = null;
            int count = 0;
            bool flag = false;
            int num2 = 0;
            if (this.ColNotNull != null)
            {
                this.BestIndex = null;
                this.BestIndexForColumn = new int[this.ColTypes.Length];
                ArrayUtil.FillArray(this.BestIndexForColumn, -1);
                for (int i = 0; i < this.IndexList.Length; i++)
                {
                    Index index = this.IndexList[i];
                    int[] columns = index.GetColumns();
                    int visibleColumns = index.GetVisibleColumns();
                    if (visibleColumns != 0)
                    {
                        if (i == 0)
                        {
                            flag = true;
                        }
                        if (this.BestIndexForColumn[columns[0]] == -1)
                        {
                            this.BestIndexForColumn[columns[0]] = i;
                        }
                        else
                        {
                            Index index2 = this.IndexList[this.BestIndexForColumn[columns[0]]];
                            if (visibleColumns > index2.GetColumns().Length)
                            {
                                this.BestIndexForColumn[columns[0]] = i;
                            }
                        }
                        if (!index.IsUnique())
                        {
                            if (this.BestIndex == null)
                            {
                                this.BestIndex = index;
                            }
                        }
                        else
                        {
                            int num5 = 0;
                            for (int j = 0; j < visibleColumns; j++)
                            {
                                if (this.ColNotNull[columns[j]])
                                {
                                    num5++;
                                }
                            }
                            if (this.BestIndex != null)
                            {
                                this.BestIndex = index;
                            }
                            if (num5 == visibleColumns)
                            {
                                if (((source == null) || (count != num2)) || (visibleColumns < count))
                                {
                                    source = columns;
                                    count = visibleColumns;
                                    num2 = visibleColumns;
                                    flag = true;
                                }
                            }
                            else if (!flag && (((source == null) || (visibleColumns < count)) || (num5 > num2)))
                            {
                                source = columns;
                                count = visibleColumns;
                                num2 = num5;
                            }
                        }
                    }
                }
                if ((source == null) || (count == source.Length))
                {
                    this.BestRowIdentifierCols = source;
                }
                else
                {
                    this.BestRowIdentifierCols = ArrayUtil.ArraySlice(source, 0, count);
                }
                this.BestRowIdentifierStrict = flag;
                if (this.IndexList[0].GetColumnCount() > 0)
                {
                    this.BestIndex = this.IndexList[0];
                }
            }
        }

        public void SetIndexes(Index[] indexes)
        {
            this.IndexList = indexes;
        }

        public void SetTransactional(bool value)
        {
            this._isTransactional = value;
        }
    }
}

