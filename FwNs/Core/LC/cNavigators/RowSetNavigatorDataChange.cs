namespace FwNs.Core.LC.cNavigators
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using System;
    using System.Collections.Generic;

    public class RowSetNavigatorDataChange : RowSetNavigator
    {
        private readonly OrderedLongKeyHashMap<Row, object[], int[]> _list = new OrderedLongKeyHashMap<Row, object[], int[]>(8, true);

        public override void Add(object[] d)
        {
            throw Error.RuntimeError(0xc9, "RowSetNavigatorDataChange");
        }

        public override bool AddRow(Row row)
        {
            long id = row.GetId();
            int lookup = this._list.GetLookup(id, (int) id);
            if (lookup == -1)
            {
                this._list.Put(row.GetId(), row, null);
                base.Size++;
                return true;
            }
            if (this._list.GetSecondValueByIndex(lookup) != null)
            {
                throw Error.GetError(0xf3c);
            }
            return false;
        }

        public object[] AddRow(Session session, Row row, object[] data, SqlType[] types, int[] columnMap)
        {
            long id = row.GetId();
            int lookup = this._list.GetLookup(id, (int) id);
            if (lookup == -1)
            {
                this._list.Put(id, row, data);
                this._list.SetThirdValueByIndex(base.Size, columnMap);
                base.Size++;
                return data;
            }
            object[] rowData = this._list.GetFirstByLookup(lookup).RowData;
            object[] secondValueByIndex = this._list.GetSecondValueByIndex(lookup);
            if (secondValueByIndex == null)
            {
                throw Error.GetError(0xf3c);
            }
            for (int i = 0; i < columnMap.Length; i++)
            {
                int index = columnMap[i];
                SqlType otherType = types[index];
                if (otherType.Compare(session, data[index], secondValueByIndex[index], otherType, true) != 0)
                {
                    if (otherType.Compare(session, rowData[index], secondValueByIndex[index], otherType, true) != 0)
                    {
                        throw Error.GetError(0xf3c);
                    }
                    secondValueByIndex[index] = data[index];
                }
            }
            int[] numArray = ArrayUtil.Union(this._list.GetThirdValueByIndex(lookup), columnMap);
            this._list.SetThirdValueByIndex(lookup, numArray);
            return secondValueByIndex;
        }

        public override void Clear()
        {
            this.Reset();
            this._list.Clear();
            base.Size = 0;
        }

        public bool ContainsDeletedRow(Row row)
        {
            long id = row.GetId();
            int lookup = this._list.GetLookup(id, (int) id);
            if (lookup == -1)
            {
                return false;
            }
            return (this._list.GetSecondValueByIndex(lookup) == null);
        }

        public override object[] GetCurrent()
        {
            return this.GetCurrentRow().RowData;
        }

        public int[] GetCurrentChangedColumns()
        {
            return this._list.GetThirdValueByIndex(base.CurrentPos);
        }

        public object[] GetCurrentChangedData()
        {
            return this._list.GetSecondValueByIndex(base.CurrentPos);
        }

        public override Row GetCurrentRow()
        {
            return this._list.GetValueByIndex(base.CurrentPos);
        }

        public override Row GetNextRow()
        {
            if (!this.Next())
            {
                return null;
            }
            return this.GetCurrentRow();
        }

        public void Read(IRowInputInterface i, ResultMetaData meta)
        {
            base.Id = i.ReadLong();
            int num = i.ReadInt();
            i.ReadInt();
            i.ReadInt();
            while (num-- > 0)
            {
                this.Add(i.ReadData(meta.ColumnTypes));
            }
        }

        public override void Remove()
        {
            throw new KeyNotFoundException();
        }

        public void Write(IRowOutputInterface o, ResultMetaData meta)
        {
            base.BeforeFirst();
            o.WriteLong(base.Id);
            o.WriteInt(base.Size);
            o.WriteInt(0);
            o.WriteInt(base.Size);
            while (base.HasNext())
            {
                object[] next = base.GetNext();
                o.WriteData(meta.GetColumnCount(), meta.ColumnTypes, next, null, null);
            }
            base.BeforeFirst();
        }
    }
}

