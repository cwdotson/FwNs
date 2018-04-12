namespace FwNs.Core.LC.cEngine
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cResults;
    using System;
    using System.IO;

    public interface ISessionInterface
    {
        void AddWarning(CoreException warning);
        void AllocateResultLob(ResultLob result, Stream dataInput);
        void Close();
        void CloseNavigator(long id);
        void Commit(bool chain);
        BlobDataId CreateBlob(long length);
        ClobDataId CreateClob(long length);
        Result Execute(Result r);
        object GetAttribute(int id);
        TimestampData GetCurrentDate();
        long GetId();
        string GetInternalConnectionUrl();
        int GetIsolation();
        RowSetNavigatorClient GetRows(long navigatorId, int offset, int size);
        Scanner GetScanner();
        int GetStreamBlockSize();
        TimeZoneInfo GetTimeZone();
        int GetZoneSeconds();
        bool IsAutoCommit();
        bool IsClosed();
        bool IsReadOnlyDefault();
        void PrepareCommit();
        void ReleaseSavepoint(string name);
        void ResetSession();
        void Rollback(bool chain);
        void RollbackToSavepoint(string name);
        void Savepoint(string name);
        void SetAttribute(int id, object value);
        void SetAutoCommit(bool autoCommit);
        void SetIsolationDefault(int level);
        void SetReadOnlyDefault(bool rdy);
        void StartPhasedTransaction();
    }
}

