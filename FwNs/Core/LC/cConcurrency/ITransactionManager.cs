namespace FwNs.Core.LC.cConcurrency
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;

    public interface ITransactionManager
    {
        RowAction AddDeleteAction(Session session, Table table, Row row, int[] colMap);
        void AddInsertAction(Session session, Table table, Row row);
        void BeginAction(Session session, Statement cs);
        void BeginActionResume(Session session);
        void BeginTransaction(Session session);
        bool CanRead(Session session, int id, int mode);
        bool CanRead(Session session, Row row, int mode, int[] colMap);
        bool CommitTransaction(Session session);
        void CompleteActions(Session session);
        void ConvertTransactionIDs(DoubleIntIndex lookup);
        long GetGlobalChangeTimestamp();
        int GetTransactionControl();
        DoubleIntIndex GetTransactionIDList();
        bool IsMvRows();
        bool PrepareCommitActions(Session session);
        void RemoveTransactionInfo(ICachedObject obj);
        void Rollback(Session session);
        void RollbackAction(Session session);
        void RollbackSavepoint(Session session, int index);
        void SetTransactionControl(Session session, int mode);
        void SetTransactionInfo(ICachedObject obj);
    }
}

