namespace FwNs.Core.LC.cResults
{
    using System;

    public static class ResultConstants
    {
        public const int UtlApiBase = 0;
        public const int None = 0;
        public const int Updatecount = 1;
        public const int Error = 2;
        public const int Data = 3;
        public const int PrepareAck = 4;
        public const int SetSessionAttr = 6;
        public const int GetSessionAttr = 7;
        public const int BatchExecDirect = 8;
        public const int ResetSession = 10;
        public const int ConnectAcknowledge = 11;
        public const int PrepareCommit = 12;
        public const int RequestData = 13;
        public const int DataRows = 14;
        public const int DataHead = 15;
        public const int BatchExecResponse = 0x10;
        public const int ParamMetadata = 0x11;
        public const int LargeObjectOp = 0x12;
        public const int Warning = 0x13;
        public const int Connect = 0x1f;
        public const int Disconnect = 0x20;
        public const int EndTran = 0x21;
        public const int ExecDirect = 0x22;
        public const int Execute = 0x23;
        public const int FreeStmt = 0x24;
        public const int Prepare = 0x25;
        public const int SetconnectAttr = 0x26;
        public const int StartTran = 0x27;
        public const int CloseResult = 40;
        public const int UpdateResult = 0x29;
        public const int Value = 0x2a;
        public const int CallResponse = 0x2b;
        public const int ModeUpperLimit = 0x2c;
        public const int TxCommit = 0;
        public const int TxRollback = 1;
        public const int TxSavepointNameRollback = 2;
        public const int TxSavepointNameRelease = 4;
        public const int TxCommitAndChain = 6;
        public const int TxRollbackAndChain = 7;
        public const int UpdateCursor = 0x51;
        public const int DeleteCursor = 0x12;
        public const int InsertCursor = 50;
        public const int SqlAttrSavepointName = 0x272b;
        public const int ExecuteFailed = -3;
        public const int SuccessNoInfo = -2;
        public const int SqlAsensitive = 0;
        public const int SqlInsensitive = 1;
        public const int SqlSensitive = 2;
        public const int SqlNonscrollable = 0;
        public const int SqlScrollable = 1;
        public const int SqlNonholdable = 0;
        public const int SqlHoldable = 1;
        public const int SqlWithoutReturn = 0;
        public const int SqlWithReturn = 1;
        public const int SqlNotUpdatable = 0;
        public const int SqlUpdatable = 1;
        public const int TypeForwardOnly = 0x3eb;
        public const int TypeScrollInsensitive = 0x3ec;
        public const int TypeScrollSensitive = 0x3ed;
        public const int ConcurReadOnly = 0x3ef;
        public const int ConcurUpdatable = 0x3f0;
        public const int HoldCursorsOverCommit = 1;
        public const int CloseCursorsAtCommit = 2;
        public const int ReturnGeneratedKeys = 1;
        public const int ReturnNoGeneratedKeys = 2;
    }
}

