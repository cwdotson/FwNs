namespace FwNs.Core.LC.cNavigators
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cTables;
    using System;

    public class RowSetNavigatorDataTable : RowSetNavigatorData
    {
        private readonly Index _fullIndex;
        private readonly Index _groupIndex;
        private readonly Index _idIndex;
        private readonly bool _isAggregate;
        private readonly bool _isSimpleAggregate;
        private readonly Index _orderIndex;
        private readonly bool _reindexTable;
        private readonly Session _session;
        private readonly object[] _tempRowData;
        private readonly int _visibleColumnCount;
        private Row _currentRow;
        private bool _isClosed;
        private IRowIterator _iterator;
        private Index _mainIndex;
        private object[] _simpleAggregateData;
        public IPersistentStore Store;
        public TableBase table;

        public RowSetNavigatorDataTable(Session session, TableBase table) : base(session)
        {
            this._session = session;
            this.table = table;
            this._visibleColumnCount = table.GetColumnCount();
            this.Store = session.sessionData.GetRowStore(table);
            this._mainIndex = table.GetPrimaryIndex();
            this._fullIndex = table.GetFullIndex();
            base.Size = this._mainIndex.Size(session, this.Store);
        }

        public RowSetNavigatorDataTable(Session session, QueryExpression queryExpression) : base(session)
        {
            this._session = session;
            this.table = queryExpression.ResultTable.Duplicate();
            this._visibleColumnCount = this.table.GetColumnCount();
            this.table.Store = this.Store = session.sessionData.GetNewResultRowStore(this.table, true);
            this._mainIndex = queryExpression.MainIndex;
            this._fullIndex = queryExpression.FullIndex;
        }

        public RowSetNavigatorDataTable(Session session, QuerySpecification select) : base(session)
        {
            this._session = session;
            base.RangePosition = select.ResultRangePosition;
            this._visibleColumnCount = select.IndexLimitVisible;
            this.table = select.ResultTable.Duplicate();
            this.table.Store = this.Store = session.sessionData.GetNewResultRowStore(this.table, !select.IsAggregated);
            this._isAggregate = select.IsAggregated;
            this._isSimpleAggregate = select.IsAggregated && !select.IsGrouped;
            this._reindexTable = select.IsGrouped;
            this._mainIndex = select.MainIndex;
            this._fullIndex = select.FullIndex;
            this._orderIndex = select.OrderIndex;
            this._groupIndex = select.GroupIndex;
            this._idIndex = select.IdIndex;
            this._tempRowData = new object[1];
        }

        public RowSetNavigatorDataTable(Session session, QuerySpecification select, RowSetNavigatorData navigator) : this(session, select)
        {
        }

        public override void Add(object[] data)
        {
            try
            {
                Row newCachedObject = this.Store.GetNewCachedObject(this._session, data);
                this.Store.IndexRow(null, newCachedObject);
                base.Size++;
            }
            catch (CoreException)
            {
            }
        }

        private void AddAdjusted(object[] data, int[] columnMap)
        {
            try
            {
                if (columnMap == null)
                {
                    data = ArrayUtil.ResizeArrayIfDifferent<object>(data, this._visibleColumnCount);
                }
                else
                {
                    object[] newRow = new object[this._visibleColumnCount];
                    ArrayUtil.ProjectRow(data, columnMap, newRow);
                    data = newRow;
                }
                this.Add(data);
            }
            catch (CoreException)
            {
            }
        }

        public override void Clear()
        {
            this.table.ClearAllData(this.Store);
            base.Size = 0;
            this.Reset();
        }

        public override void Close()
        {
            if (!this._isClosed)
            {
                this._iterator.Release();
                this._isClosed = true;
            }
        }

        public override bool ContainsRow(object[] data)
        {
            IRowIterator iterator1 = this._mainIndex.FindFirstRow(this._session, this.Store, data);
            bool flag = iterator1.HasNext();
            iterator1.Release();
            return flag;
        }

        public override void Copy(RowSetNavigatorData other, int[] rightColumnIndexes)
        {
            while (other.HasNext())
            {
                object[] next = other.GetNext();
                this.AddAdjusted(next, rightColumnIndexes);
            }
            other.Close();
        }

        public override void Except(RowSetNavigatorData other)
        {
            this.RemoveDuplicates();
            this.Reset();
            other.SortFull();
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if (other.ContainsRow(next))
                {
                    this.Remove();
                }
            }
            other.Close();
        }

        public override void ExceptAll(RowSetNavigatorData other)
        {
            object[] b = null;
            this.SortFull();
            this.Reset();
            other.SortFull();
            IRowIterator emptyIterator = this._fullIndex.GetEmptyIterator();
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if ((b == null) || (this._fullIndex.CompareRowNonUnique(this._session, next, b, this._fullIndex.GetColumnCount()) > 0))
                {
                    b = next;
                    emptyIterator = other.FindFirstRow(next);
                }
                Row nextRow = emptyIterator.GetNextRow();
                object[] objArray3 = (nextRow == null) ? null : nextRow.RowData;
                if ((objArray3 != null) && (this._fullIndex.CompareRowNonUnique(this._session, next, objArray3, this._fullIndex.GetColumnCount()) == 0))
                {
                    this.Remove();
                }
            }
            other.Close();
        }

        public override IRowIterator FindFirstRow(object[] data)
        {
            return this._mainIndex.FindFirstRow(this._session, this.Store, data);
        }

        public override object[] GetCurrent()
        {
            return this._currentRow.RowData;
        }

        public override Row GetCurrentRow()
        {
            return this._currentRow;
        }

        public override object[] GetData(long rowId)
        {
            this._tempRowData[0] = rowId;
            return this._idIndex.FindFirstRow(this._session, this.Store, this._tempRowData, this._idIndex.GetDefaultColumnMap()).GetNext();
        }

        public override object[] GetGroupData(object[] data)
        {
            if (this._isSimpleAggregate)
            {
                if (this._simpleAggregateData == null)
                {
                    this._simpleAggregateData = data;
                    return null;
                }
                return this._simpleAggregateData;
            }
            IRowIterator iterator = this._groupIndex.FindFirstRow(this._session, this.Store, data);
            if (iterator.HasNext())
            {
                Row nextRow = iterator.GetNextRow();
                if (this._isAggregate)
                {
                    nextRow.SetChanged();
                }
                return nextRow.RowData;
            }
            return null;
        }

        public bool HasNull(object[] data)
        {
            for (int i = 0; i < this._visibleColumnCount; i++)
            {
                if (data[i] == null)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool HasUniqueNotNullRows()
        {
            this.SortFull();
            this.Reset();
            object[] a = null;
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if (!this.HasNull(next))
                {
                    if ((a != null) && (this._fullIndex.CompareRow(this._session, a, next) == 0))
                    {
                        return false;
                    }
                    a = next;
                }
            }
            return true;
        }

        public void Initialize(RowSetNavigatorData navigator)
        {
            navigator.Reset();
            while (navigator.HasNext())
            {
                this.Add(navigator.GetNext());
            }
        }

        public override void Intersect(RowSetNavigatorData other)
        {
            this.RemoveDuplicates();
            this.Reset();
            other.SortFull();
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if (!other.ContainsRow(next))
                {
                    this.Remove();
                }
            }
            other.Close();
        }

        public override void IntersectAll(RowSetNavigatorData other)
        {
            object[] b = null;
            this.SortFull();
            this.Reset();
            other.SortFull();
            IRowIterator emptyIterator = this._fullIndex.GetEmptyIterator();
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if ((b == null) || (this._fullIndex.CompareRowNonUnique(this._session, next, b, this._fullIndex.GetColumnCount()) > 0))
                {
                    b = next;
                    emptyIterator = other.FindFirstRow(next);
                }
                Row nextRow = emptyIterator.GetNextRow();
                object[] objArray3 = (nextRow == null) ? null : nextRow.RowData;
                if ((objArray3 == null) || (this._fullIndex.CompareRowNonUnique(this._session, next, objArray3, this._fullIndex.GetColumnCount()) != 0))
                {
                    this.Remove();
                }
            }
            other.Close();
        }

        public override bool IsMemory()
        {
            return this.Store.IsMemory();
        }

        public override bool Next()
        {
            this._currentRow = this._iterator.GetNextRow();
            return base.Next();
        }

        public override void Remove()
        {
            if (this._currentRow != null)
            {
                this._iterator.Remove();
                this._currentRow = null;
                base.CurrentPos--;
                base.Size--;
            }
        }

        public override void RemoveDuplicates()
        {
            this.SortFull();
            this.Reset();
            object[] a = null;
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if ((a != null) && (this._fullIndex.CompareRow(this._session, a, next) == 0))
                {
                    this.Remove();
                }
                else
                {
                    a = next;
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            this._iterator = this._mainIndex.FirstRow(this.Store);
        }

        public override void SortFull()
        {
            if (this._reindexTable)
            {
                this.Store.IndexRows();
            }
            this._mainIndex = this._fullIndex;
            this.Reset();
        }

        public override void SortOrder()
        {
            if (this._orderIndex != null)
            {
                if (this._reindexTable)
                {
                    this.Store.IndexRows();
                }
                this._mainIndex = this._orderIndex;
                this.Reset();
            }
        }

        public override void SortUnion(SortAndSlice sortAndSlice)
        {
            if (sortAndSlice.index != null)
            {
                this._mainIndex = sortAndSlice.index;
                this.Reset();
            }
        }

        public override void Trim(int limitstart, int limitcount)
        {
            if (base.Size != 0)
            {
                if (limitstart >= base.Size)
                {
                    this.Clear();
                }
                else
                {
                    if (limitstart != 0)
                    {
                        this.Reset();
                        for (int i = 0; i < limitstart; i++)
                        {
                            this.Next();
                            this.Remove();
                        }
                    }
                    if ((limitcount != 0) && (limitcount < base.Size))
                    {
                        this.Reset();
                        for (int i = 0; i < limitcount; i++)
                        {
                            this.Next();
                        }
                        while (base.HasNext())
                        {
                            this.Next();
                            this.Remove();
                        }
                    }
                }
            }
        }

        public override void Union(RowSetNavigatorData other)
        {
            this.RemoveDuplicates();
            this.Reset();
            while (other.HasNext())
            {
                object[] next = other.GetNext();
                if (!this.FindFirstRow(next).HasNext())
                {
                    next = ArrayUtil.ResizeArrayIfDifferent<object>(next, this.table.GetColumnCount());
                    this.Add(next);
                }
            }
            other.Close();
        }

        public override void Update(object[] oldData, object[] newData)
        {
            if (!this._isSimpleAggregate)
            {
                IRowIterator iterator = this._groupIndex.FindFirstRow(this._session, this.Store, oldData);
                if (iterator.HasNext())
                {
                    iterator.GetNextRow();
                    iterator.Remove();
                    iterator.Release();
                    base.Size--;
                    this.Add(newData);
                }
            }
        }
    }
}

