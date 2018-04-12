namespace FwNs.Core.LC.cIndexes
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;

    public interface Index : ISchemaObject
    {
        int CompareRow(Session session, object[] a, object[] b);
        int CompareRowNonUnique(Session session, object[] a, object[] b, int[] rowColMap);
        int CompareRowNonUnique(Session session, object[] a, object[] b, int fieldcount);
        int CompareRowNonUnique(Session session, object[] a, object[] b, int[] rowColMap, int fieldCount);
        void Delete(Session session, IPersistentStore store, Row row);
        bool ExistsParent(Session session, IPersistentStore store, object[] rowdata, int[] rowColMap);
        IRowIterator FindFirstRow(Session session, IPersistentStore store, object[] rowdata);
        IRowIterator FindFirstRow(Session session, IPersistentStore store, object[] rowdata, int[] rowColMap);
        IRowIterator FindFirstRow(Session session, IPersistentStore store, object[] rowdata, int matchCount, int compareType, bool reversed, bool[] map);
        IRowIterator FindFirstRowNotNull(Session session, IPersistentStore store);
        IRowIterator FirstRow(IPersistentStore store);
        IRowIterator FirstRow(Session session, IPersistentStore store);
        int GetColumnCount();
        bool[] GetColumnDesc();
        int[] GetColumns();
        SqlType[] GetColumnTypes();
        int[] GetDefaultColumnMap();
        IRowIterator GetEmptyIterator();
        int GetIndexOrderValue();
        long GetPersistenceId();
        int GetPosition();
        int GetVisibleColumns();
        void Insert(Session session, IPersistentStore store, Row row);
        bool IsConstraint();
        bool IsEmpty(IPersistentStore store);
        bool IsForward();
        bool IsUnique();
        IRowIterator LastRow(Session session, IPersistentStore store);
        void SetPosition(int position);
        void SetTable(TableBase table);
        int Size(Session session, IPersistentStore store);
        int SizeUnique(IPersistentStore store);
    }
}

