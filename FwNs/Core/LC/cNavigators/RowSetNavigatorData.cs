namespace FwNs.Core.LC.cNavigators
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRows;
    using System;
    using System.Collections.Generic;

    public class RowSetNavigatorData : RowSetNavigator, IComparer<object[]>
    {
        public static object[][] EmptyTable = new object[0][];
        private int _currentOffset;
        private object[][] _table;
        private Session _session;
        private QueryExpression _queryExpression;
        private readonly int _visibleColumnCount;
        private readonly bool _isSimpleAggregate;
        private object[] _simpleAggregateData;
        private Index _mainIndex;
        private SortedList<object[], object[]> _rowMap;
        private readonly LongKeyHashMap<object[]> _idMap;

        public RowSetNavigatorData(Session session)
        {
            this._table = EmptyTable;
            this._session = session;
        }

        public RowSetNavigatorData(Session session, QueryExpression queryExpression)
        {
            this._table = EmptyTable;
            this._session = session;
            this._queryExpression = queryExpression;
            this._visibleColumnCount = queryExpression.GetColumnCount();
        }

        public RowSetNavigatorData(Session session, QuerySpecification select)
        {
            this._table = EmptyTable;
            this._session = session;
            this._queryExpression = select;
            base.RangePosition = select.ResultRangePosition;
            this._visibleColumnCount = select.GetColumnCount();
            this._isSimpleAggregate = select.IsAggregated && !select.IsGrouped;
            if (select.IsGrouped)
            {
                this._mainIndex = select.GroupIndex;
                this._rowMap = new SortedList<object[], object[]>(this);
            }
            if (select.IdIndex != null)
            {
                this._idMap = new LongKeyHashMap<object[]>();
            }
        }

        public override void Add(object[] data)
        {
            this.EnsureCapacity();
            this._table[base.Size] = data;
            base.Size++;
            if (this._rowMap != null)
            {
                this._rowMap.Add(data, data);
            }
            if (this._idMap != null)
            {
                long key = (long) data[this._visibleColumnCount];
                this._idMap.Put(key, data);
            }
        }

        private void AddAdjusted(object[] data, int[] columnMap)
        {
            data = this.ProjectData(data, columnMap);
            this.Add(data);
        }

        public override bool AddRow(Row row)
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigatorData");
        }

        public override void Clear()
        {
            this._table = EmptyTable;
            base.Size = this._table.Length;
            this.Reset();
        }

        public override void ClearStructures()
        {
            this._queryExpression = null;
            this._mainIndex = null;
            if (this._rowMap != null)
            {
                this._rowMap.Clear();
            }
            this._rowMap = null;
            this._session = null;
            this._simpleAggregateData = null;
        }

        public int Compare(object[] a, object[] b)
        {
            return this._mainIndex.CompareRow(this._session, a, b);
        }

        public virtual bool ContainsRow(object[] data)
        {
            return (ArraySort.SearchFirst<object[]>(this._table, 0, base.Size, data, this) >= 0);
        }

        public virtual void Copy(RowSetNavigatorData other, int[] rightColumnIndexes)
        {
            while (other.HasNext())
            {
                object[] next = other.GetNext();
                this.AddAdjusted(next, rightColumnIndexes);
            }
            other.Close();
        }

        private void EnsureCapacity()
        {
            if (base.Size == this._table.Length)
            {
                object[][] destinationArray = new object[(base.Size == 0) ? 4 : (base.Size * 2)][];
                Array.Copy(this._table, 0, destinationArray, 0, base.Size);
                this._table = destinationArray;
            }
        }

        public virtual void Except(RowSetNavigatorData other)
        {
            this.RemoveDuplicates();
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
            this.Reset();
        }

        public virtual void ExceptAll(RowSetNavigatorData other)
        {
            object[] b = null;
            this.SortFull();
            other.SortFull();
            IRowIterator emptyIterator = this._queryExpression.FullIndex.GetEmptyIterator();
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if ((b == null) || (this._queryExpression.FullIndex.CompareRowNonUnique(this._session, next, b, this._queryExpression.FullIndex.GetColumnCount()) > 0))
                {
                    b = next;
                    emptyIterator = other.FindFirstRow(next);
                }
                object[] objArray3 = emptyIterator.GetNext();
                if ((objArray3 != null) && (this._queryExpression.FullIndex.CompareRowNonUnique(this._session, next, objArray3, this._queryExpression.FullIndex.GetColumnCount()) == 0))
                {
                    this.Remove();
                }
            }
            other.Close();
            this.Reset();
        }

        public virtual IRowIterator FindFirstRow(object[] data)
        {
            int position = ArraySort.SearchFirst<object[]>(this._table, 0, base.Size, data, this);
            if (position < 0)
            {
                position = base.Size;
            }
            else
            {
                position--;
            }
            return new DataIterator(position, this);
        }

        public static void GetBlock(int offset)
        {
        }

        public SqlType[] GetColumnDataTypes()
        {
            return this._queryExpression.resultMetaData.ColumnTypes;
        }

        public override object[] GetCurrent()
        {
            if ((base.CurrentPos < 0) || (base.CurrentPos >= base.Size))
            {
                return null;
            }
            if (base.CurrentPos == (this._currentOffset + this._table.Length))
            {
                GetBlock(this._currentOffset + this._table.Length);
            }
            return this._table[base.CurrentPos - this._currentOffset];
        }

        public override Row GetCurrentRow()
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigatorData");
        }

        public virtual object[] GetData(long rowId)
        {
            return this._idMap.Get(rowId);
        }

        public virtual object[] GetGroupData(object[] data)
        {
            object[] objArray2;
            if (this._isSimpleAggregate)
            {
                if (this._simpleAggregateData == null)
                {
                    this._simpleAggregateData = data;
                    return null;
                }
                return this._simpleAggregateData;
            }
            if (this._rowMap.TryGetValue(data, out objArray2))
            {
                return objArray2;
            }
            return null;
        }

        public object[] GetNextRowData()
        {
            if (!this.Next())
            {
                return null;
            }
            return this.GetCurrent();
        }

        private bool HasNull(object[] data)
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

        public virtual bool HasUniqueNotNullRows()
        {
            this.SortFull();
            this.Reset();
            object[] a = null;
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if (!this.HasNull(next))
                {
                    if ((a != null) && (this._queryExpression.FullIndex.CompareRow(this._session, a, next) == 0))
                    {
                        return false;
                    }
                    a = next;
                }
            }
            return true;
        }

        public static void Implement()
        {
            throw Error.GetError(0xc9, "RSND");
        }

        public void Insert(object[] data)
        {
            this.EnsureCapacity();
            Array.Copy(this._table, base.CurrentPos, this._table, base.CurrentPos + 1, base.Size - base.CurrentPos);
            this._table[base.CurrentPos] = data;
            base.Size++;
        }

        public void InsertAdjusted(object[] data, int[] columnMap)
        {
            this.ProjectData(data, columnMap);
            this.Insert(data);
        }

        public virtual void Intersect(RowSetNavigatorData other)
        {
            this.RemoveDuplicates();
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
            this.Reset();
        }

        public virtual void IntersectAll(RowSetNavigatorData other)
        {
            object[] b = null;
            this.SortFull();
            other.SortFull();
            IRowIterator emptyIterator = this._queryExpression.FullIndex.GetEmptyIterator();
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if ((b == null) || (this._queryExpression.FullIndex.CompareRowNonUnique((Session) base.session, next, b, this._visibleColumnCount) > 0))
                {
                    b = next;
                    emptyIterator = other.FindFirstRow(next);
                }
                object[] objArray3 = emptyIterator.GetNext();
                if ((objArray3 == null) || (this._queryExpression.FullIndex.CompareRowNonUnique((Session) base.session, next, objArray3, this._visibleColumnCount) != 0))
                {
                    this.Remove();
                }
            }
            other.Close();
            this.Reset();
        }

        public override bool IsMemory()
        {
            return true;
        }

        public object[] ProjectData(object[] data, int[] columnMap)
        {
            if (columnMap == null)
            {
                data = ArrayUtil.ResizeArrayIfDifferent<object>(data, this._visibleColumnCount);
                return data;
            }
            object[] newRow = new object[this._visibleColumnCount];
            ArrayUtil.ProjectRow(data, columnMap, newRow);
            data = newRow;
            return data;
        }

        public override void Remove()
        {
            Array.Copy(this._table, base.CurrentPos + 1, this._table, base.CurrentPos, (base.Size - base.CurrentPos) - 1);
            this._table[base.Size - 1] = null;
            base.CurrentPos--;
            base.Size--;
        }

        public virtual void RemoveDuplicates()
        {
            this.SortFull();
            this.Reset();
            object[] a = null;
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                if ((a != null) && (this._queryExpression.FullIndex.CompareRow(this._session, a, next) == 0))
                {
                    this.Remove();
                }
                else
                {
                    a = next;
                }
            }
            this.Reset();
        }

        public virtual void SortFull()
        {
            this._mainIndex = this._queryExpression.FullIndex;
            ArraySort.Sort<object[]>(this._table, 0, base.Size, this);
            this.Reset();
        }

        public virtual void SortOrder()
        {
            if (this._queryExpression.OrderIndex != null)
            {
                this._mainIndex = this._queryExpression.OrderIndex;
                ArraySort.Sort<object[]>(this._table, 0, base.Size, this);
            }
            this.Reset();
        }

        public virtual void SortUnion(SortAndSlice sortAndSlice)
        {
            if (sortAndSlice.index != null)
            {
                this._mainIndex = sortAndSlice.index;
                ArraySort.Sort<object[]>(this._table, 0, base.Size, this);
                this.Reset();
            }
        }

        public virtual void Trim(int limitstart, int limitcount)
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
                        this.Reset();
                    }
                }
            }
        }

        public virtual void Union(RowSetNavigatorData other)
        {
            this.RemoveDuplicates();
            other.RemoveDuplicates();
            while (other.HasNext())
            {
                object[] next = other.GetNext();
                int num = ArraySort.SearchFirst<object[]>(this._table, 0, base.Size, next, this);
                if (num < 0)
                {
                    num = -num - 1;
                    base.CurrentPos = num;
                    this.Insert(next);
                }
            }
            other.Close();
            this.Reset();
        }

        public void UnionAll(RowSetNavigatorData other)
        {
            other.Reset();
            while (other.HasNext())
            {
                object[] next = other.GetNext();
                this.Add(next);
            }
            other.Close();
            this.Reset();
        }

        public virtual void Update(object[] oldData, object[] newData)
        {
        }

        private class DataIterator : IRowIterator
        {
            private readonly RowSetNavigatorData _o;
            private int _pos;

            public DataIterator(int position, RowSetNavigatorData o)
            {
                this._pos = position;
                this._o = o;
            }

            public object[] GetNext()
            {
                if (this.HasNext())
                {
                    this._pos++;
                    return this._o._table[this._pos];
                }
                return null;
            }

            public Row GetNextRow()
            {
                return null;
            }

            public long GetRowId()
            {
                return 0L;
            }

            public bool HasNext()
            {
                return (this._pos < (this._o.Size - 1));
            }

            public void Release()
            {
            }

            public void Remove()
            {
            }

            public bool SetRowColumns(bool[] columns)
            {
                return false;
            }
        }
    }
}

