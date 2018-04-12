namespace FwNs.Core.LC.cIndexes
{
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Text;
    using System.Threading;

    public class IndexAVL : Index, ISchemaObject, IDisposable
    {
        public const int MemoryIndex = 0;
        public const int DiskIndex = 1;
        public const int PointerIndex = 2;
        public long PersistenceId;
        public QNameManager.QName Name;
        public bool[] ColCheck;
        public int[] ColIndex;
        public int[] DefaultColMap;
        public SqlType[] ColTypes;
        public bool[] ColDesc;
        public bool[] NullsLast;
        protected bool IsSimple;
        protected bool IsSimpleOrder;
        public int[] PkCols;
        public SqlType[] PkTypes;
        protected bool IsPk;
        public bool isUnique;
        public bool UseRowId;
        public bool isConstraint;
        public bool isForward;
        public int Depth;
        public static IndexRowIterator EmptyIterator = new IndexRowIterator(null, null, null, null, false, false);
        protected TableBase table;
        protected int Position;
        protected object _lock = new Dummy();
        private readonly object[] _nullData;
        public static Index[] EmptyArray = new IndexAVL[0];
        public NodeAVL Root;

        public IndexAVL(QNameManager.QName name, long id, TableBase table, int[] columns, bool[] descending, bool[] nullsLast, SqlType[] colTypes, bool pk, bool unique, bool constraint, bool forward)
        {
            this.PersistenceId = id;
            this.Name = name;
            this.ColIndex = columns;
            this.ColTypes = colTypes;
            this.ColDesc = descending ?? new bool[columns.Length];
            this.NullsLast = nullsLast ?? new bool[columns.Length];
            this.IsPk = pk;
            this.isUnique = unique;
            this.isConstraint = constraint;
            this.isForward = forward;
            this.table = table;
            this.ColCheck = table.GetNewColumnCheckList();
            ArrayUtil.IntIndexesToboolArray(this.ColIndex, this.ColCheck);
            this.DefaultColMap = new int[columns.Length];
            ArrayUtil.FillSequence(this.DefaultColMap);
            bool flag = this.ColIndex.Length > 0;
            for (int i = 0; i < this.ColDesc.Length; i++)
            {
                if (this.ColDesc[i] || this.NullsLast[i])
                {
                    flag = false;
                }
            }
            this.IsSimpleOrder = flag;
            this.IsSimple = this.IsSimpleOrder && (this.ColIndex.Length == 1);
            this._nullData = new object[this.ColIndex.Length];
        }

        private void Balance(IPersistentStore store, NodeAVL x, bool isleft)
        {
            while (true)
            {
                int num = isleft ? 1 : -1;
                switch ((x.GetBalance(store) * num))
                {
                    case -1:
                    {
                        NodeAVL n = x.Child(store, isleft);
                        if (n.GetBalance(store) != -num)
                        {
                            NodeAVL eavl2 = n.Child(store, !isleft);
                            x.Replace(store, this, eavl2);
                            n = n.Set(store, !isleft, eavl2.Child(store, isleft));
                            eavl2 = eavl2.Set(store, isleft, n);
                            x = x.Set(store, isleft, eavl2.Child(store, !isleft));
                            eavl2 = eavl2.Set(store, !isleft, x);
                            int balance = eavl2.GetBalance(store);
                            x = x.SetBalance(store, (balance == -num) ? num : 0);
                            n = n.SetBalance(store, (balance == num) ? -num : 0);
                            eavl2 = eavl2.SetBalance(store, 0);
                            return;
                        }
                        x.Replace(store, this, n);
                        x = x.Set(store, isleft, n.Child(store, !isleft));
                        n = n.Set(store, !isleft, x);
                        x = x.SetBalance(store, 0);
                        n = n.SetBalance(store, 0);
                        return;
                    }
                    case 0:
                        x = x.SetBalance(store, -num);
                        break;

                    case 1:
                        x = x.SetBalance(store, 0);
                        return;
                }
                if (x.IsRoot(store))
                {
                    return;
                }
                isleft = x.IsFromLeft(store);
                x = x.GetParent(store);
            }
        }

        public int CompareObject(Session session, object[] a, object[] b, int[] rowColMap, int position)
        {
            return this.ColTypes[position].Compare(session, a[this.ColIndex[position]], b[rowColMap[position]], null, false);
        }

        public int CompareRow(Session session, object[] a, object[] b)
        {
            for (int i = 0; i < this.ColIndex.Length; i++)
            {
                int num3 = this.ColTypes[i].Compare(session, a[this.ColIndex[i]], b[this.ColIndex[i]], null, false);
                if (num3 != 0)
                {
                    if (!this.IsSimpleOrder)
                    {
                        bool flag = (a[this.ColIndex[i]] == null) || (b[this.ColIndex[i]] == null);
                        if (this.ColDesc[i] && !flag)
                        {
                            num3 = -num3;
                        }
                        if (this.NullsLast[i] & flag)
                        {
                            num3 = -num3;
                        }
                    }
                    return num3;
                }
            }
            return 0;
        }

        public int CompareRowForInsertOrDelete(Session session, Row newRow, Row existingRow, bool useRowId, int start)
        {
            object[] rowData = newRow.RowData;
            object[] objArray2 = existingRow.RowData;
            for (int i = start; i < this.ColIndex.Length; i++)
            {
                int index = this.ColIndex[i];
                object a = rowData[index];
                object b = objArray2[index];
                int num4 = this.ColTypes[i].Compare(session, a, b, null, false);
                if (num4 != 0)
                {
                    if (!this.IsSimpleOrder)
                    {
                        bool flag = (a == null) || (b == null);
                        if (this.ColDesc[i] && !flag)
                        {
                            num4 = -num4;
                        }
                        if (this.NullsLast[i] & flag)
                        {
                            num4 = -num4;
                        }
                    }
                    return num4;
                }
            }
            if (useRowId)
            {
                return (newRow.GetPos() - existingRow.GetPos());
            }
            return 0;
        }

        public int CompareRowNonUnique(Session session, object[] a, object[] b, int[] rowColMap)
        {
            int length = rowColMap.Length;
            for (int i = 0; i < length; i++)
            {
                int num3 = this.ColTypes[i].Compare(session, a[this.ColIndex[i]], b[rowColMap[i]], null, false);
                if (num3 != 0)
                {
                    return num3;
                }
            }
            return 0;
        }

        public int CompareRowNonUnique(Session session, object[] a, object[] b, int fieldCount)
        {
            for (int i = 0; i < fieldCount; i++)
            {
                int num2 = this.ColTypes[i].Compare(session, a[this.ColIndex[i]], b[this.ColIndex[i]], null, false);
                if (num2 != 0)
                {
                    return num2;
                }
            }
            return 0;
        }

        public int CompareRowNonUnique(Session session, object[] a, object[] b, int[] rowColMap, int fieldCount)
        {
            for (int i = 0; i < fieldCount; i++)
            {
                int num2 = this.ColTypes[i].Compare(session, a[this.ColIndex[i]], b[rowColMap[i]], null, false);
                if (num2 != 0)
                {
                    return num2;
                }
            }
            return 0;
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public virtual void Delete(IPersistentStore store, NodeAVL x)
        {
            if (x != null)
            {
                Monitor.Enter(this._lock);
                store.LockStore();
                try
                {
                    NodeAVL parent;
                    if (x.GetLeft(store) == null)
                    {
                        parent = x.GetRight(store);
                    }
                    else if (x.GetRight(store) == null)
                    {
                        parent = x.GetLeft(store);
                    }
                    else
                    {
                        NodeAVL node = x;
                        x = x.GetLeft(store);
                        while (true)
                        {
                            NodeAVL right = x.GetRight(store);
                            if (right == null)
                            {
                                break;
                            }
                            x = right;
                        }
                        parent = x.GetLeft(store);
                        int balance = x.GetBalance(store);
                        x = x.SetBalance(store, node.GetBalance(store));
                        node = node.SetBalance(store, balance);
                        NodeAVL eavl3 = x.GetParent(store);
                        NodeAVL n = node.GetParent(store);
                        if (node.IsRoot(store))
                        {
                            store.SetAccessor(this, x);
                        }
                        x = x.SetParent(store, n);
                        if (n != null)
                        {
                            if (!n.IsRight(node))
                            {
                                n.SetLeft(store, x);
                            }
                            else
                            {
                                n.SetRight(store, x);
                            }
                        }
                        if (node.Equals(eavl3))
                        {
                            node = node.SetParent(store, x);
                            if (node.IsLeft(x))
                            {
                                x = x.SetLeft(store, node);
                                NodeAVL right = node.GetRight(store);
                                x = x.SetRight(store, right);
                            }
                            else
                            {
                                x = x.SetRight(store, node);
                                NodeAVL left = node.GetLeft(store);
                                x = x.SetLeft(store, left);
                            }
                        }
                        else
                        {
                            node = node.SetParent(store, eavl3);
                            eavl3 = eavl3.SetRight(store, node);
                            NodeAVL left = node.GetLeft(store);
                            NodeAVL right = node.GetRight(store);
                            x = x.SetLeft(store, left);
                            x = x.SetRight(store, right);
                        }
                        x.GetRight(store).SetParent(store, x);
                        x.GetLeft(store).SetParent(store, x);
                        node = node.SetLeft(store, parent);
                        if (parent != null)
                        {
                            parent = parent.SetParent(store, node);
                        }
                        x = node.SetRight(store, null);
                    }
                    bool isleft = x.IsFromLeft(store);
                    x.Replace(store, this, parent);
                    parent = x.GetParent(store);
                    x.Delete();
                    while (parent != null)
                    {
                        NodeAVL eavl10;
                        int balance;
                        NodeAVL eavl12;
                        x = parent;
                        int b = isleft ? 1 : -1;
                        switch ((x.GetBalance(store) * b))
                        {
                            case -1:
                                x = x.SetBalance(store, 0);
                                goto Label_0359;

                            case 0:
                                x = x.SetBalance(store, b);
                                return;

                            case 1:
                            {
                                eavl10 = x.Child(store, !isleft);
                                balance = eavl10.GetBalance(store);
                                if ((balance * b) < 0)
                                {
                                    goto Label_02B7;
                                }
                                x.Replace(store, this, eavl10);
                                NodeAVL n = eavl10.Child(store, isleft);
                                x = x.Set(store, !isleft, n);
                                eavl10 = eavl10.Set(store, isleft, x);
                                if (balance != 0)
                                {
                                    break;
                                }
                                x = x.SetBalance(store, b);
                                eavl10 = eavl10.SetBalance(store, -b);
                                return;
                            }
                            default:
                                goto Label_0359;
                        }
                        x = x.SetBalance(store, 0);
                        x = eavl10.SetBalance(store, 0);
                        goto Label_0359;
                    Label_02B7:
                        eavl12 = eavl10.Child(store, isleft);
                        x.Replace(store, this, eavl12);
                        balance = eavl12.GetBalance(store);
                        eavl10 = eavl10.Set(store, isleft, eavl12.Child(store, !isleft));
                        eavl12 = eavl12.Set(store, !isleft, eavl10);
                        x = x.Set(store, !isleft, eavl12.Child(store, isleft));
                        eavl12 = eavl12.Set(store, isleft, x);
                        x = x.SetBalance(store, (balance == b) ? -b : 0);
                        eavl10 = eavl10.SetBalance(store, (balance == -b) ? b : 0);
                        x = eavl12.SetBalance(store, 0);
                    Label_0359:
                        isleft = x.IsFromLeft(store);
                        parent = x.GetParent(store);
                    }
                }
                finally
                {
                    store.UnlockStore();
                    Monitor.Exit(this._lock);
                }
            }
        }

        public void Delete(Session session, IPersistentStore store, Row row)
        {
            if (!row.IsInMemory())
            {
                row = (Row) store.Get(row, false);
            }
            NodeAVL node = ((RowAVL) row).GetNode(this.Position);
            if (node != null)
            {
                this.Delete(store, node);
                store.UpdateElementCount(this, -1, -1);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
        }

        public bool ExistsParent(Session session, IPersistentStore store, object[] rowdata, int[] rowColMap)
        {
            return (this.FindNode(session, store, rowdata, rowColMap, rowColMap.Length, 0x29, 2, false) > null);
        }

        public IRowIterator FindFirstRow(Session session, IPersistentStore store, object[] rowdata)
        {
            NodeAVL x = this.FindNode(session, store, rowdata, this.ColIndex, this.ColIndex.Length, 0x29, 0, false);
            return this.GetIterator(session, store, x, false, false);
        }

        public IRowIterator FindFirstRow(Session session, IPersistentStore store, object[] rowdata, int[] rowColMap)
        {
            NodeAVL x = this.FindNode(session, store, rowdata, rowColMap, rowColMap.Length, 0x29, 0, false);
            return this.GetIterator(session, store, x, false, false);
        }

        public IRowIterator FindFirstRow(Session session, IPersistentStore store, object[] rowdata, int matchCount, int compareType, bool reversed, bool[] map)
        {
            if (compareType == 0x4a)
            {
                return this.LastRow(session, store);
            }
            NodeAVL x = this.FindNode(session, store, rowdata, this.DefaultColMap, matchCount, compareType, 0, reversed);
            return this.GetIterator(session, store, x, false, reversed);
        }

        public IRowIterator FindFirstRowNotNull(Session session, IPersistentStore store)
        {
            NodeAVL x = this.FindNode(session, store, this._nullData, this.DefaultColMap, 1, 0x30, 0, false);
            return this.GetIterator(session, store, x, false, false);
        }

        private NodeAVL FindNode(Session session, IPersistentStore store, object[] rowdata, int[] rowColMap, int fieldCount, int compareType, int readMode, bool reversed)
        {
            NodeAVL eavl;
            lock (this._lock)
            {
                Row row2;
                NodeAVL accessor = this.GetAccessor(store);
                NodeAVL left = null;
                NodeAVL x = null;
                if ((compareType != 0x29) && (compareType != 0x2f))
                {
                    fieldCount--;
                }
                while (accessor != null)
                {
                    Row row = accessor.GetRow(store);
                    int num = 0;
                    if (fieldCount > 0)
                    {
                        num = this.CompareRowNonUnique(session, row.RowData, rowdata, rowColMap, fieldCount);
                    }
                    if (num != 0)
                    {
                        goto Label_017B;
                    }
                    switch (compareType)
                    {
                        case 0x29:
                        case 0x2f:
                            x = accessor;
                            left = accessor.GetLeft(store);
                            goto Label_0197;

                        case 0x2a:
                            if (this.CompareObject(session, row.RowData, rowdata, rowColMap, fieldCount) >= 0)
                            {
                                break;
                            }
                            left = accessor.GetRight(store);
                            goto Label_0197;

                        case 0x2b:
                        case 0x30:
                            if (this.CompareObject(session, row.RowData, rowdata, rowColMap, fieldCount) > 0)
                            {
                                goto Label_00FC;
                            }
                            left = accessor.GetRight(store);
                            goto Label_0197;

                        case 0x2c:
                            if (this.CompareObject(session, row.RowData, rowdata, rowColMap, fieldCount) >= 0)
                            {
                                goto Label_0131;
                            }
                            x = accessor;
                            left = accessor.GetRight(store);
                            goto Label_0197;

                        case 0x2d:
                            if (this.CompareObject(session, row.RowData, rowdata, rowColMap, fieldCount) > 0)
                            {
                                goto Label_0161;
                            }
                            x = accessor;
                            left = accessor.GetRight(store);
                            goto Label_0197;

                        default:
                            throw Error.RuntimeError(0xc9, "Index");
                    }
                    x = accessor;
                    left = accessor.GetLeft(store);
                    goto Label_0197;
                Label_00FC:
                    x = accessor;
                    left = accessor.GetLeft(store);
                    goto Label_0197;
                Label_0131:
                    left = accessor.GetLeft(store);
                    goto Label_0197;
                Label_0161:
                    left = accessor.GetLeft(store);
                    goto Label_0197;
                Label_017B:
                    if (num < 0)
                    {
                        left = accessor.GetRight(store);
                    }
                    else if (num > 0)
                    {
                        left = accessor.GetLeft(store);
                    }
                Label_0197:
                    if (left == null)
                    {
                        break;
                    }
                    accessor = left;
                }
                if (session != null)
                {
                    goto Label_0210;
                }
                return x;
            Label_01A9:
                row2 = x.GetRow(store);
                if (session.database.TxManager.CanRead(session, row2, readMode, this.ColIndex))
                {
                    goto Label_0213;
                }
                x = reversed ? this.Last(store, x) : this.Next(store, x);
                if (x == null)
                {
                    goto Label_0213;
                }
                row2 = x.GetRow(store);
                if ((fieldCount > 0) && (this.CompareRowNonUnique(session, row2.RowData, rowdata, rowColMap, fieldCount) != 0))
                {
                    x = null;
                    goto Label_0213;
                }
            Label_0210:
                if (x != null)
                {
                    goto Label_01A9;
                }
            Label_0213:
                eavl = x;
            }
            return eavl;
        }

        public IRowIterator FirstRow(IPersistentStore store)
        {
            IRowIterator iterator;
            int num = 0;
            Monitor.Enter(this._lock);
            try
            {
                NodeAVL accessor = this.GetAccessor(store);
                NodeAVL left = accessor;
                while (left != null)
                {
                    accessor = left;
                    left = accessor.GetLeft(store);
                    num++;
                }
                iterator = this.GetIterator(null, store, accessor, false, false);
            }
            finally
            {
                this.Depth = num;
                Monitor.Exit(this._lock);
            }
            return iterator;
        }

        public IRowIterator FirstRow(Session session, IPersistentStore store)
        {
            IRowIterator iterator;
            int num = 0;
            Monitor.Enter(this._lock);
            try
            {
                NodeAVL accessor = this.GetAccessor(store);
                NodeAVL left = accessor;
                while (left != null)
                {
                    accessor = left;
                    left = accessor.GetLeft(store);
                    num++;
                }
                while ((session != null) && (accessor != null))
                {
                    Row row = accessor.GetRow(store);
                    if (session.database.TxManager.CanRead(session, row, 0, null))
                    {
                        break;
                    }
                    accessor = this.Next(store, accessor);
                }
                iterator = this.GetIterator(session, store, accessor, false, false);
            }
            finally
            {
                this.Depth = num;
                Monitor.Exit(this._lock);
            }
            return iterator;
        }

        protected NodeAVL GetAccessor(IPersistentStore store)
        {
            return (NodeAVL) store.GetAccessor(this);
        }

        public QNameManager.QName GetCatalogName()
        {
            return this.Name.schema.schema;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        public int GetColumnCount()
        {
            return this.ColIndex.Length;
        }

        public bool[] GetColumnDesc()
        {
            return this.ColDesc;
        }

        public int[] GetColumns()
        {
            return this.ColIndex;
        }

        public SqlType[] GetColumnTypes()
        {
            return this.ColTypes;
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public int[] GetDefaultColumnMap()
        {
            return this.DefaultColMap;
        }

        public IRowIterator GetEmptyIterator()
        {
            return EmptyIterator;
        }

        public int GetIndexOrderValue()
        {
            if (!this.IsPk)
            {
                if (!this.isConstraint)
                {
                    return 2;
                }
                if (this.isForward)
                {
                    return 4;
                }
                if (!this.isUnique)
                {
                    return 1;
                }
            }
            return 0;
        }

        private IndexRowIterator GetIterator(Session session, IPersistentStore store, NodeAVL x, bool single, bool reversed)
        {
            if (x == null)
            {
                return EmptyIterator;
            }
            return new IndexRowIterator(session, store, this, x, single, reversed);
        }

        public QNameManager.QName GetName()
        {
            return this.Name;
        }

        public int GetNodeCount(Session session, IPersistentStore store)
        {
            int num = 0;
            lock (this._lock)
            {
                IRowIterator iterator = this.FirstRow(session, store);
                while (iterator.HasNext())
                {
                    iterator.GetNextRow();
                    num++;
                }
                return num;
            }
        }

        public Grantee GetOwner()
        {
            return this.Name.schema.Owner;
        }

        public long GetPersistenceId()
        {
            return this.PersistenceId;
        }

        public int GetPosition()
        {
            return this.Position;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return new OrderedHashSet<QNameManager.QName>();
        }

        public QNameManager.QName GetSchemaName()
        {
            return this.Name.schema;
        }

        public int GetSchemaObjectType()
        {
            return 20;
        }

        public string GetSql()
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append("CREATE").Append(' ');
            if (this.IsUnique())
            {
                builder.Append("UNIQUE").Append(' ');
            }
            builder.Append("INDEX").Append(' ');
            builder.Append(this.GetName().StatementName);
            builder.Append(' ').Append("ON").Append(' ');
            builder.Append(((Table) this.table).GetName().GetSchemaQualifiedStatementName());
            int[] columns = this.GetColumns();
            int visibleColumns = this.GetVisibleColumns();
            builder.Append(((Table) this.table).GetColumnListSql(columns, visibleColumns));
            return builder.ToString();
        }

        public int GetVisibleColumns()
        {
            return this.ColIndex.Length;
        }

        public bool HasNulls(object[] rowData)
        {
            for (int i = 0; i < this.ColIndex.Length; i++)
            {
                if (rowData[this.ColIndex[i]] == null)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void Insert(Session session, IPersistentStore store, Row row)
        {
            bool useRowId = !this.isUnique || this.HasNulls(row.RowData);
            Monitor.Enter(this._lock);
            store.LockStore();
            try
            {
                bool flag2;
                NodeAVL eavl2;
                Constraint uniqueConstraintForIndex;
                Row row2;
                RowAVL wavl = (RowAVL) row;
                NodeAVL accessor = this.GetAccessor(store);
                if (accessor == null)
                {
                    store.SetAccessor(this, wavl.GetNode(this.Position));
                    store.SetElementCount(this, 1, 1);
                    return;
                }
            Label_005C:
                row2 = accessor.GetRow(store);
                int num = this.CompareRowForInsertOrDelete(session, row, row2, useRowId, 0);
                if ((((num == 0) && (session != null)) && (!useRowId && session.database.TxManager.IsMvRows())) && !this.IsEqualReadable(session, store, accessor))
                {
                    useRowId = true;
                    num = this.CompareRowForInsertOrDelete(session, row, row2, useRowId, this.ColIndex.Length);
                }
                if (num != 0)
                {
                    flag2 = num < 0;
                    eavl2 = accessor;
                    accessor = eavl2.Child(store, flag2);
                    if (accessor != null)
                    {
                        goto Label_005C;
                    }
                }
                else
                {
                    uniqueConstraintForIndex = null;
                    if (this.isConstraint)
                    {
                        uniqueConstraintForIndex = ((Table) this.table).GetUniqueConstraintForIndex(this);
                    }
                    goto Label_0119;
                }
                eavl2 = eavl2.Set(store, flag2, wavl.GetNode(this.Position));
                this.Balance(store, eavl2, flag2);
                store.UpdateElementCount(this, 1, 1);
                return;
            Label_0119:
                if (uniqueConstraintForIndex == null)
                {
                    throw Error.GetError(0x68, this.Name.StatementName);
                }
                throw uniqueConstraintForIndex.GetException(row.RowData);
            }
            finally
            {
                store.UnlockStore();
                Monitor.Exit(this._lock);
            }
        }

        public bool IsConstraint()
        {
            return this.isConstraint;
        }

        public bool IsEmpty(IPersistentStore store)
        {
            lock (this._lock)
            {
                return (this.GetAccessor(store) == null);
            }
        }

        protected bool IsEqualReadable(Session session, IPersistentStore store, NodeAVL node)
        {
            object[] data;
            NodeAVL x = node;
            if (!session.database.TxManager.CanRead(session, node.GetRow(store), 1, null))
            {
                Row row;
                data = node.GetData(store);
                do
                {
                    x = this.Last(store, x);
                    if (x == null)
                    {
                        goto Label_006E;
                    }
                    object[] data = x.GetData(store);
                    if (this.CompareRow(session, data, data) != 0)
                    {
                        goto Label_006E;
                    }
                    row = x.GetRow(store);
                }
                while (!session.database.TxManager.CanRead(session, row, 1, null));
            }
            return true;
        Label_006E:
            x = this.Next(session, store, node);
            if (x != null)
            {
                object[] data = x.GetData(store);
                if (this.CompareRow(session, data, data) != 0)
                {
                    return false;
                }
                Row row = x.GetRow(store);
                if (!session.database.TxManager.CanRead(session, row, 1, null))
                {
                    goto Label_006E;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool IsForward()
        {
            return this.isForward;
        }

        public bool IsUnique()
        {
            return this.isUnique;
        }

        public virtual NodeAVL Last(IPersistentStore store, NodeAVL x)
        {
            if (x == null)
            {
                return null;
            }
            NodeAVL left = x.GetLeft(store);
            if (left != null)
            {
                x = left;
                for (NodeAVL eavl3 = x.GetRight(store); eavl3 != null; eavl3 = x.GetRight(store))
                {
                    x = eavl3;
                }
                return x;
            }
            NodeAVL eavl4 = x;
            x = x.GetParent(store);
            while ((x != null) && eavl4.Equals(x.GetLeft(store)))
            {
                eavl4 = x;
                x = x.GetParent(store);
            }
            return x;
        }

        public NodeAVL Last(Session session, IPersistentStore store, NodeAVL x)
        {
            if (x == null)
            {
                return null;
            }
            lock (this._lock)
            {
            Label_0012:
                x = this.Last(store, x);
                if (x != null)
                {
                    if (session == null)
                    {
                        return x;
                    }
                    Row row = x.GetRow(store);
                    if (!session.database.TxManager.CanRead(session, row, 0, null))
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

        public IRowIterator LastRow(Session session, IPersistentStore store)
        {
            lock (this._lock)
            {
                NodeAVL accessor = this.GetAccessor(store);
                for (NodeAVL eavl2 = accessor; eavl2 != null; eavl2 = accessor.GetRight(store))
                {
                    accessor = eavl2;
                }
                while ((session != null) && (accessor != null))
                {
                    Row row = accessor.GetRow(store);
                    if (session.database.TxManager.CanRead(session, row, 0, null))
                    {
                        break;
                    }
                    accessor = this.Last(store, accessor);
                }
                return this.GetIterator(null, store, accessor, false, true);
            }
        }

        public virtual NodeAVL Next(IPersistentStore store, NodeAVL x)
        {
            NodeAVL right = x.GetRight(store);
            if (right != null)
            {
                x = right;
                for (NodeAVL eavl3 = x.GetLeft(store); eavl3 != null; eavl3 = x.GetLeft(store))
                {
                    x = eavl3;
                }
                return x;
            }
            NodeAVL eavl4 = x;
            x = x.GetParent(store);
            while ((x != null) && (eavl4 == x.GetRight(store)))
            {
                eavl4 = x;
                x = x.GetParent(store);
            }
            return x;
        }

        public virtual NodeAVL Next(Session session, IPersistentStore store, NodeAVL x)
        {
            if (x == null)
            {
                return null;
            }
            lock (this._lock)
            {
            Label_0012:
                x = this.Next(store, x);
                if ((x != null) && (session != null))
                {
                    Row row = x.GetRow(store);
                    if (!session.database.TxManager.CanRead(session, row, 0, null))
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

        public void SetName(string name, bool isquoted)
        {
            this.Name.Rename(name, isquoted);
        }

        public void SetPosition(int position)
        {
            this.Position = position;
        }

        public void SetTable(TableBase table)
        {
            this.table = table;
        }

        public int Size(Session session, IPersistentStore store)
        {
            lock (this._lock)
            {
                return store.ElementCount(session);
            }
        }

        public int SizeEstimate(IPersistentStore store)
        {
            this.FirstRow(null, store);
            return (((int) 1) << this.Depth);
        }

        public int SizeUnique(IPersistentStore store)
        {
            lock (this._lock)
            {
                return store.ElementCountUnique(this);
            }
        }

        private class Dummy
        {
        }

        public sealed class IndexRowIterator : IRowIterator
        {
            private readonly IndexAVL _index;
            private readonly Session _session;
            private readonly bool _single;
            private readonly IPersistentStore _store;
            private readonly bool _reversed;
            private Row _lastrow;
            private NodeAVL _nextnode;

            public IndexRowIterator(Session session, IPersistentStore store, IndexAVL index, NodeAVL node, bool single, bool reversed)
            {
                this._session = session;
                this._store = store;
                this._index = index;
                this._single = single;
                this._reversed = reversed;
                if (index != null)
                {
                    this._nextnode = node;
                }
            }

            public object[] GetNext()
            {
                Row nextRow = this.GetNextRow();
                if (nextRow != null)
                {
                    return nextRow.RowData;
                }
                return null;
            }

            public Row GetNextRow()
            {
                if (this._nextnode == null)
                {
                    this.Release();
                    return null;
                }
                this._lastrow = this._nextnode.GetRow(this._store);
                if (this._single)
                {
                    this._nextnode = null;
                }
                else if (this._reversed)
                {
                    this._nextnode = this._index.Last(this._session, this._store, this._nextnode);
                }
                else
                {
                    this._nextnode = this._index.Next(this._session, this._store, this._nextnode);
                }
                return this._lastrow;
            }

            public long GetRowId()
            {
                return (long) this._nextnode.GetPos();
            }

            public bool HasNext()
            {
                return (this._nextnode > null);
            }

            public void Release()
            {
            }

            public void Remove()
            {
                this._store.Delete(this._session, this._lastrow);
                this._store.Remove(this._lastrow.GetPos());
            }

            public bool SetRowColumns(bool[] columns)
            {
                return false;
            }
        }
    }
}

