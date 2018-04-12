namespace FwNs.Core.LC.cIndexes
{
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class IndexAVLMemory : IndexAVL
    {
        public IndexAVLMemory(QNameManager.QName name, long id, TableBase table, int[] columns, bool[] descending, bool[] nullsLast, SqlType[] colTypes, bool pk, bool unique, bool constraint, bool forward) : base(name, id, table, columns, descending, nullsLast, colTypes, pk, unique, constraint, forward)
        {
        }

        private void Balance(IPersistentStore store, NodeAVL x, bool isleft)
        {
            while (true)
            {
                int num = isleft ? 1 : -1;
                switch ((x.IBalance * num))
                {
                    case -1:
                    {
                        NodeAVL n = isleft ? x.nLeft : x.nRight;
                        if (n.IBalance != -num)
                        {
                            NodeAVL eavl2 = !isleft ? n.nLeft : n.nRight;
                            x.Replace(store, this, eavl2);
                            n.Set(store, !isleft, isleft ? eavl2.nLeft : eavl2.nRight);
                            eavl2.Set(store, isleft, n);
                            x.Set(store, isleft, isleft ? eavl2.nRight : eavl2.nLeft);
                            eavl2.Set(store, !isleft, x);
                            int iBalance = eavl2.IBalance;
                            x.IBalance = (iBalance == -num) ? num : 0;
                            n.IBalance = (iBalance == num) ? -num : 0;
                            eavl2.IBalance = 0;
                            return;
                        }
                        x.Replace(store, this, n);
                        x.Set(store, isleft, isleft ? n.nRight : n.nLeft);
                        n.Set(store, !isleft, x);
                        x.IBalance = 0;
                        n.IBalance = 0;
                        return;
                    }
                    case 0:
                        x.IBalance = -num;
                        break;

                    case 1:
                        x.IBalance = 0;
                        return;
                }
                if (x.nParent == null)
                {
                    return;
                }
                isleft = x == x.nParent.nLeft;
                x = x.nParent;
            }
        }

        public override void Delete(IPersistentStore store, NodeAVL x)
        {
            if (x != null)
            {
                lock (base._lock)
                {
                    NodeAVL nParent;
                    if (x.nLeft == null)
                    {
                        nParent = x.nRight;
                    }
                    else if (x.nRight == null)
                    {
                        nParent = x.nLeft;
                    }
                    else
                    {
                        NodeAVL eavl2 = x;
                        x = x.nLeft;
                        while (true)
                        {
                            NodeAVL nRight = x.nRight;
                            if (nRight == null)
                            {
                                break;
                            }
                            x = nRight;
                        }
                        nParent = x.nLeft;
                        int iBalance = x.IBalance;
                        x.IBalance = eavl2.IBalance;
                        eavl2.IBalance = iBalance;
                        NodeAVL eavl3 = x.nParent;
                        NodeAVL eavl4 = eavl2.nParent;
                        if (eavl2.IsRoot(store))
                        {
                            store.SetAccessor(this, x);
                        }
                        x.nParent = eavl4;
                        if (eavl4 != null)
                        {
                            if (eavl4.nRight == eavl2)
                            {
                                eavl4.nRight = x;
                            }
                            else
                            {
                                eavl4.nLeft = x;
                            }
                        }
                        if (eavl2 == eavl3)
                        {
                            eavl2.nParent = x;
                            if (eavl2.nLeft == x)
                            {
                                x.nLeft = eavl2;
                                x.nRight = eavl2.nRight;
                            }
                            else
                            {
                                x.nRight = eavl2;
                                x.nLeft = eavl2.nLeft;
                            }
                        }
                        else
                        {
                            eavl2.nParent = eavl3;
                            eavl3.nRight = eavl2;
                            NodeAVL nLeft = eavl2.nLeft;
                            NodeAVL nRight = eavl2.nRight;
                            x.nLeft = nLeft;
                            x.nRight = nRight;
                        }
                        x.nRight.nParent = x;
                        x.nLeft.nParent = x;
                        eavl2.nLeft = nParent;
                        if (nParent != null)
                        {
                            nParent.nParent = eavl2;
                        }
                        eavl2.nRight = null;
                        x = eavl2;
                    }
                    bool isleft = x.IsFromLeft(store);
                    x.Replace(store, this, nParent);
                    nParent = x.nParent;
                    x.Delete();
                    while (nParent != null)
                    {
                        NodeAVL eavl10;
                        int iBalance;
                        NodeAVL eavl12;
                        x = nParent;
                        int num2 = isleft ? 1 : -1;
                        switch ((x.IBalance * num2))
                        {
                            case -1:
                                x.IBalance = 0;
                                goto Label_02E9;

                            case 0:
                                x.IBalance = num2;
                                return;

                            case 1:
                            {
                                eavl10 = x.Child(store, !isleft);
                                iBalance = eavl10.IBalance;
                                if ((iBalance * num2) < 0)
                                {
                                    goto Label_0255;
                                }
                                x.Replace(store, this, eavl10);
                                NodeAVL n = eavl10.Child(store, isleft);
                                x.Set(store, !isleft, n);
                                eavl10.Set(store, isleft, x);
                                if (iBalance != 0)
                                {
                                    break;
                                }
                                x.IBalance = num2;
                                eavl10.IBalance = -num2;
                                return;
                            }
                            default:
                                goto Label_02E9;
                        }
                        x.IBalance = 0;
                        eavl10.IBalance = 0;
                        x = eavl10;
                        goto Label_02E9;
                    Label_0255:
                        eavl12 = eavl10.Child(store, isleft);
                        x.Replace(store, this, eavl12);
                        iBalance = eavl12.IBalance;
                        eavl10.Set(store, isleft, eavl12.Child(store, !isleft));
                        eavl12.Set(store, !isleft, eavl10);
                        x.Set(store, !isleft, eavl12.Child(store, isleft));
                        eavl12.Set(store, isleft, x);
                        x.IBalance = (iBalance == num2) ? -num2 : 0;
                        eavl10.IBalance = (iBalance == -num2) ? num2 : 0;
                        eavl12.IBalance = 0;
                        x = eavl12;
                    Label_02E9:
                        isleft = x.IsFromLeft(store);
                        nParent = x.nParent;
                    }
                }
            }
        }

        public override void Insert(Session session, IPersistentStore store, Row row)
        {
            object[] rowData = row.RowData;
            bool useRowId = !base.isUnique || base.HasNulls(rowData);
            bool isSimple = base.IsSimple;
            lock (base._lock)
            {
                NodeAVL eavl2;
                Constraint uniqueConstraintForIndex;
                RowAVL wavl = (RowAVL) row;
                NodeAVL accessor = base.GetAccessor(store);
                if (accessor == null)
                {
                    store.SetAccessor(this, wavl.GetNode(base.Position));
                    store.SetElementCount(this, 1, 1);
                    goto Label_01BB;
                }
                bool isLeft = false;
                do
                {
                    if (base.ColTypes.Length != 0)
                    {
                        int num;
                        Row memoryRow = accessor.MemoryRow;
                        if (isSimple)
                        {
                            num = base.ColTypes[0].Compare(session, rowData[base.ColIndex[0]], memoryRow.RowData[base.ColIndex[0]], null, false);
                            if ((num == 0) & useRowId)
                            {
                                num = base.CompareRowForInsertOrDelete(session, row, memoryRow, useRowId, 1);
                            }
                        }
                        else
                        {
                            num = base.CompareRowForInsertOrDelete(session, row, memoryRow, useRowId, 0);
                        }
                        if ((((num == 0) && (session != null)) && (!useRowId && session.database.TxManager.IsMvRows())) && !base.IsEqualReadable(session, store, accessor))
                        {
                            useRowId = true;
                            num = base.CompareRowForInsertOrDelete(session, row, memoryRow, useRowId, base.ColIndex.Length);
                        }
                        if (num == 0)
                        {
                            goto Label_013C;
                        }
                        isLeft = num < 0;
                    }
                    eavl2 = accessor;
                    accessor = isLeft ? eavl2.nLeft : eavl2.nRight;
                }
                while (accessor != null);
                goto Label_015C;
            Label_013C:
                uniqueConstraintForIndex = null;
                if (base.isConstraint)
                {
                    uniqueConstraintForIndex = ((Table) base.table).GetUniqueConstraintForIndex(this);
                }
                goto Label_018A;
            Label_015C:
                eavl2 = eavl2.Set(store, isLeft, wavl.GetNode(base.Position));
                this.Balance(store, eavl2, isLeft);
                store.UpdateElementCount(this, 1, 1);
                goto Label_01BB;
            Label_018A:
                if (uniqueConstraintForIndex == null)
                {
                    throw Error.GetError(0x68, base.Name.StatementName);
                }
                throw uniqueConstraintForIndex.GetException(row.RowData);
            Label_01BB:;
            }
        }

        public override NodeAVL Last(IPersistentStore store, NodeAVL x)
        {
            if (x == null)
            {
                return null;
            }
            NodeAVL nLeft = x.nLeft;
            if (nLeft != null)
            {
                x = nLeft;
                for (NodeAVL eavl3 = x.nRight; eavl3 != null; eavl3 = x.nRight)
                {
                    x = eavl3;
                }
                return x;
            }
            NodeAVL eavl4 = x;
            x = x.nParent;
            while ((x != null) && (eavl4 == x.nLeft))
            {
                eavl4 = x;
                x = x.nParent;
            }
            return x;
        }

        public override NodeAVL Next(IPersistentStore store, NodeAVL x)
        {
            NodeAVL nRight = x.nRight;
            if (nRight != null)
            {
                x = nRight;
                for (NodeAVL eavl3 = x.nLeft; eavl3 != null; eavl3 = x.nLeft)
                {
                    x = eavl3;
                }
                return x;
            }
            NodeAVL eavl4 = x;
            x = x.nParent;
            while ((x != null) && (eavl4 == x.nRight))
            {
                eavl4 = x;
                x = x.nParent;
            }
            return x;
        }

        public override NodeAVL Next(Session session, IPersistentStore store, NodeAVL x)
        {
            if (x == null)
            {
                return null;
            }
            lock (base._lock)
            {
            Label_0012:
                x = this.Next(store, x);
                if ((x != null) && (session != null))
                {
                    Row memoryRow = x.MemoryRow;
                    if (!session.database.TxManager.CanRead(session, memoryRow, 0, null))
                    {
                        goto Label_0012;
                    }
                }
                else
                {
                    return x;
                }
                return x;
            }
        }
    }
}

