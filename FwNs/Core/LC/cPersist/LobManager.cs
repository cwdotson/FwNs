namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cStatements;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    public sealed class LobManager : IDisposable
    {
        private const string ResourceFileName = "LibCore.Resources.lob-schema.sql";
        private const string InitialiseBlocksSql = "INSERT INTO SYSTEM_LOBS.BLOCKS VALUES(?,?,?)";
        private const string GetLobSql = "SELECT * FROM SYSTEM_LOBS.LOB_IDS WHERE LOB_ID = ?";
        private const string GetLobPartSql = "SELECT * FROM SYSTEM_LOBS.LOBS WHERE LOB_ID = ? AND BLOCK_OFFSET + BLOCK_COUNT > ? AND BLOCK_OFFSET < ? ORDER BY BLOCK_OFFSET";
        private const string DeleteLobPartCallSql = "CALL SYSTEM_LOBS.DELETE_BLOCKS(?,?,?,?)";
        private const string CreateLobSql = "INSERT INTO SYSTEM_LOBS.LOB_IDS VALUES(?, ?, ?, ?)";
        private const string UpdateLobLengthSql = "UPDATE SYSTEM_LOBS.LOB_IDS SET LOB_LENGTH = ? WHERE LOB_ID = ?";
        private const string CreateLobPartCallSql = "CALL SYSTEM_LOBS.ALLOC_BLOCKS(?, ?, ?)";
        private const string DivideLobPartCallSql = "CALL SYSTEM_LOBS.DIVIDE_BLOCK(?, ?)";
        private const string UpdateLobUsageSql = "UPDATE SYSTEM_LOBS.LOB_IDS SET LOB_USAGE_COUNT = ? WHERE LOB_ID = ?";
        private const string GetNextLobIdSql = "VALUES NEXT VALUE FOR SYSTEM_LOBS.LOB_ID";
        private const string DeleteLobCallSql = "CALL SYSTEM_LOBS.DELETE_LOB(?, ?)";
        private const string DeleteUnusedCallSql = "CALL SYSTEM_LOBS.DELETE_UNUSED()";
        private const string GetLobCountSql = "SELECT COUNT(*) FROM SYSTEM_LOBS.LOB_IDS";
        private static readonly string[] Starters = new string[] { "/*" };
        private readonly Database _database;
        private Statement _createLob;
        private Statement _createLobPartCall;
        private Statement _deleteLobPartCall;
        private Statement _deleteUnusedLobs;
        private Statement _divideLobPartCall;
        private Statement _getLob;
        private Statement _getLobCount;
        private Statement _getLobPart;
        private Statement _getNextLobId;
        private int _lobBlockSize;
        private ILobStore _lobStore;
        private Session _sysLobSession;
        private int _totalBlockLimitCount = 0x7fffffff;
        private Statement _updateLobLength;
        private Statement _updateLobUsage;
        private int _deletedLobCount;

        public LobManager(Database database)
        {
            this._database = database;
        }

        public Result AdjustUsageCount(long lobId, int delta)
        {
            return this.AdjustUsageCount(lobId, delta, false);
        }

        public Result AdjustUsageCount(long lobId, int delta, bool tx)
        {
            lock (this)
            {
                int num = Convert.ToInt32(this.GetLobHeader(lobId)[2]);
                if ((num + delta) == 0)
                {
                    this._deletedLobCount++;
                }
                object[] args = new object[this._updateLobUsage.GetParametersMetaData().GetColumnCount()];
                args[0] = num + delta;
                args[1] = lobId;
                this._sysLobSession.sessionContext.PushDynamicArguments(args);
                this._sysLobSession.sessionContext.Pop(false);
                return (tx ? this._sysLobSession.ExecuteCompiledStatement(this._updateLobUsage, args) : this._updateLobUsage.Execute(this._sysLobSession));
            }
        }

        public void Close()
        {
            lock (this)
            {
                this._lobStore.Close();
            }
        }

        public int Compare(IBlobData a, byte[] b)
        {
            int num;
            lock (this)
            {
                long num2 = (long) this.GetLobHeader(a.GetId())[1];
                int[][] numArray = this.GetBlockAddresses(a.GetId(), 0, 0x7fffffff);
                int index = 0;
                int num4 = 0;
                int num5 = 0;
                do
                {
                    int blockAddress = numArray[index][0] + num5;
                    byte[] blockBytes = this._lobStore.GetBlockBytes(blockAddress, 1);
                    for (int i = 0; i < blockBytes.Length; i++)
                    {
                        if ((num4 + i) >= b.Length)
                        {
                            goto Label_00A3;
                        }
                        if (blockBytes[i] != b[num4 + i])
                        {
                            return (((blockBytes[i] & 0xff) > (b[num4 + i] & 0xff)) ? 1 : -1);
                        }
                    }
                    num5++;
                    num4 += this._lobBlockSize;
                    if (num5 == numArray[index][1])
                    {
                        num5 = 0;
                        index++;
                    }
                }
                while (index != numArray.Length);
                goto Label_00DC;
            Label_00A3:
                if (num2 == b.Length)
                {
                    return 0;
                }
                return 1;
            Label_00DC:
                num = -1;
            }
            return num;
        }

        public int Compare(IBlobData a, IBlobData b)
        {
            lock (this)
            {
                if (a.GetId() == b.GetId())
                {
                    return 0;
                }
                object[] lobHeader = this.GetLobHeader(a.GetId());
                if (lobHeader == null)
                {
                    return 1;
                }
                long num2 = (long) lobHeader[1];
                lobHeader = this.GetLobHeader(b.GetId());
                if (lobHeader == null)
                {
                    return -1;
                }
                long num3 = (long) lobHeader[1];
                if (num2 > num3)
                {
                    return 1;
                }
                if (num2 < num3)
                {
                    return -1;
                }
                return this.CompareBytes(a.GetId(), b.GetId());
            }
        }

        public int Compare(IClobData a, IClobData b)
        {
            lock (this)
            {
                if (a.GetId() == b.GetId())
                {
                    return 0;
                }
                return this.CompareText(a.GetId(), b.GetId());
            }
        }

        public int Compare(IClobData a, string b)
        {
            lock (this)
            {
                long num2 = (long) this.GetLobHeader(a.GetId())[1];
                int[][] numArray = this.GetBlockAddresses(a.GetId(), 0, 0x7fffffff);
                int index = 0;
                int startIndex = 0;
                int num5 = 0;
                do
                {
                    int blockAddress = numArray[index][0] + num5;
                    long num8 = num2 - (((numArray[index][2] + num5) * this._lobBlockSize) / 2);
                    if (num8 > (this._lobBlockSize / 2))
                    {
                        num8 = this._lobBlockSize / 2;
                    }
                    string str = new string(ArrayUtil.ByteArrayToChars(this._lobStore.GetBlockBytes(blockAddress, 1)), 0, (int) num8);
                    int length = b.Length - startIndex;
                    if (length > (this._lobBlockSize / 2))
                    {
                        length = this._lobBlockSize / 2;
                    }
                    string str2 = b.Substring(startIndex, length);
                    int num6 = this._database.collation.Compare(str, str2);
                    if (num6 != 0)
                    {
                        return num6;
                    }
                    num5++;
                    startIndex += this._lobBlockSize / 2;
                    if (num5 == numArray[index][1])
                    {
                        num5 = 0;
                        index++;
                    }
                }
                while (index != numArray.Length);
                return 0;
            }
        }

        public int CompareBytes(long aId, long bId)
        {
            lock (this)
            {
                int[][] numArray = this.GetBlockAddresses(aId, 0, 0x7fffffff);
                int[][] numArray2 = this.GetBlockAddresses(bId, 0, 0x7fffffff);
                int index = 0;
                int num3 = 0;
                int num4 = 0;
                int num5 = 0;
                do
                {
                    int blockAddress = numArray[index][0] + num4;
                    int num8 = numArray2[num3][0] + num5;
                    byte[] blockBytes = this._lobStore.GetBlockBytes(blockAddress, 1);
                    byte[] buffer2 = this._lobStore.GetBlockBytes(num8, 1);
                    for (int i = 0; i < blockBytes.Length; i++)
                    {
                        if (blockBytes[i] != buffer2[i])
                        {
                            return (((blockBytes[i] & 0xff) > (buffer2[i] & 0xff)) ? 1 : -1);
                        }
                    }
                    num4++;
                    num5++;
                    if (num4 == numArray[index][1])
                    {
                        num4 = 0;
                        index++;
                    }
                    if (num5 == numArray2[num3][1])
                    {
                        num5 = 0;
                        num3++;
                    }
                }
                while (index != numArray.Length);
                return 0;
            }
        }

        public int CompareText(long aId, long bId)
        {
            lock (this)
            {
                long num2 = (long) this.GetLobHeader(aId)[1];
                long num3 = (long) this.GetLobHeader(bId)[1];
                int[][] numArray = this.GetBlockAddresses(aId, 0, 0x7fffffff);
                int[][] numArray2 = this.GetBlockAddresses(bId, 0, 0x7fffffff);
                int index = 0;
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                do
                {
                    int blockAddress = numArray[index][0] + num6;
                    int num10 = numArray2[num5][0] + num7;
                    byte[] blockBytes = this._lobStore.GetBlockBytes(num10, 1);
                    long num11 = num2 - (((numArray[index][2] + num6) * this._lobBlockSize) / 2);
                    if (num11 > (this._lobBlockSize / 2))
                    {
                        num11 = this._lobBlockSize / 2;
                    }
                    long num12 = num3 - (((numArray2[num5][2] + num7) * this._lobBlockSize) / 2);
                    if (num12 > (this._lobBlockSize / 2))
                    {
                        num12 = this._lobBlockSize / 2;
                    }
                    string a = new string(ArrayUtil.ByteArrayToChars(this._lobStore.GetBlockBytes(blockAddress, 1)), 0, (int) num11);
                    string b = new string(ArrayUtil.ByteArrayToChars(blockBytes), 0, (int) num12);
                    int num8 = this._database.collation.Compare(a, b);
                    if (num8 != 0)
                    {
                        return num8;
                    }
                    num6++;
                    num7++;
                    if (num6 == numArray[index][1])
                    {
                        num6 = 0;
                        index++;
                    }
                    if (num7 == numArray2[num5][1])
                    {
                        num7 = 0;
                        num5++;
                    }
                }
                while (index != numArray.Length);
                return 0;
            }
        }

        public long CreateBlob(long length)
        {
            lock (this)
            {
                long newLobId = this.GetNewLobId();
                object[] pvals = new object[this._createLob.GetParametersMetaData().GetColumnCount()];
                pvals[0] = newLobId;
                pvals[1] = length;
                pvals[2] = 0L;
                pvals[3] = 30;
                this._sysLobSession.ExecuteCompiledStatement(this._createLob, pvals);
                return newLobId;
            }
        }

        private void CreateBlockAddresses(long lobId, int offset, int count)
        {
            object[] pvals = new object[this._createLobPartCall.GetParametersMetaData().GetColumnCount()];
            pvals[0] = count;
            pvals[1] = offset;
            pvals[2] = lobId;
            this._sysLobSession.ExecuteCompiledStatement(this._createLobPartCall, pvals);
        }

        public long CreateClob(long length)
        {
            lock (this)
            {
                long newLobId = this.GetNewLobId();
                object[] pvals = new object[this._createLob.GetParametersMetaData().GetColumnCount()];
                pvals[0] = newLobId;
                pvals[1] = length;
                pvals[2] = 0;
                pvals[3] = 40;
                this._sysLobSession.ExecuteCompiledStatement(this._createLob, pvals);
                return newLobId;
            }
        }

        public void CreateSchema()
        {
            lock (this)
            {
                this._sysLobSession = this._database.sessionManager.GetSysLobSession();
                Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LibCore.Resources.lob-schema.sql");
                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(manifestResourceStream);
                }
                catch (Exception)
                {
                }
                LineGroupReader reader1 = new LineGroupReader(reader, Starters);
                HashMappedList<string, string> asMap = reader1.GetAsMap();
                reader1.Close();
                string sql = asMap.Get("/*lob_schema_definition*/");
                Result result = this._sysLobSession.CompileStatement(sql, ResultProperties.DefaultPropsValue).Execute(this._sysLobSession);
                if (result.IsError())
                {
                    throw result.GetException();
                }
                this._getLob = this._sysLobSession.CompileStatement("SELECT * FROM SYSTEM_LOBS.LOB_IDS WHERE LOB_ID = ?", ResultProperties.DefaultPropsValue);
                this._getLobPart = this._sysLobSession.CompileStatement("SELECT * FROM SYSTEM_LOBS.LOBS WHERE LOB_ID = ? AND BLOCK_OFFSET + BLOCK_COUNT > ? AND BLOCK_OFFSET < ? ORDER BY BLOCK_OFFSET", ResultProperties.DefaultPropsValue);
                this._createLob = this._sysLobSession.CompileStatement("INSERT INTO SYSTEM_LOBS.LOB_IDS VALUES(?, ?, ?, ?)", ResultProperties.DefaultPropsValue);
                this._createLobPartCall = this._sysLobSession.CompileStatement("CALL SYSTEM_LOBS.ALLOC_BLOCKS(?, ?, ?)", ResultProperties.DefaultPropsValue);
                this._divideLobPartCall = this._sysLobSession.CompileStatement("CALL SYSTEM_LOBS.DIVIDE_BLOCK(?, ?)", ResultProperties.DefaultPropsValue);
                this._deleteLobPartCall = this._sysLobSession.CompileStatement("CALL SYSTEM_LOBS.DELETE_BLOCKS(?,?,?,?)", ResultProperties.DefaultPropsValue);
                this._updateLobLength = this._sysLobSession.CompileStatement("UPDATE SYSTEM_LOBS.LOB_IDS SET LOB_LENGTH = ? WHERE LOB_ID = ?", ResultProperties.DefaultPropsValue);
                this._updateLobUsage = this._sysLobSession.CompileStatement("UPDATE SYSTEM_LOBS.LOB_IDS SET LOB_USAGE_COUNT = ? WHERE LOB_ID = ?", ResultProperties.DefaultPropsValue);
                this._getNextLobId = this._sysLobSession.CompileStatement("VALUES NEXT VALUE FOR SYSTEM_LOBS.LOB_ID", ResultProperties.DefaultPropsValue);
                this._deleteUnusedLobs = this._sysLobSession.CompileStatement("CALL SYSTEM_LOBS.DELETE_UNUSED()", ResultProperties.DefaultPropsValue);
                this._getLobCount = this._sysLobSession.CompileStatement("SELECT COUNT(*) FROM SYSTEM_LOBS.LOB_IDS", ResultProperties.DefaultPropsValue);
            }
        }

        private void DeleteBlockAddresses(long lobId, int offset, int count)
        {
            object[] pvals = new object[this._deleteLobPartCall.GetParametersMetaData().GetColumnCount()];
            pvals[0] = lobId;
            pvals[1] = offset;
            pvals[2] = count;
            pvals[3] = this._sysLobSession.GetTransactionTimestamp();
            this._sysLobSession.ExecuteCompiledStatement(this._deleteLobPartCall, pvals);
        }

        public Result DeleteUnusedLobs()
        {
            lock (this)
            {
                this._deletedLobCount = 0;
                return this._sysLobSession.ExecuteCompiledStatement(this._deleteUnusedLobs, new object[0]);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._lobStore.Dispose();
            }
        }

        private void DivideBlockAddresses(long lobId, int offset)
        {
            object[] pvals = new object[this._divideLobPartCall.GetParametersMetaData().GetColumnCount()];
            pvals[0] = lobId;
            pvals[1] = offset;
            this._sysLobSession.ExecuteCompiledStatement(this._divideLobPartCall, pvals);
        }

        private int[][] GetBlockAddresses(long lobId, int offset, int limit)
        {
            object[] args = new object[this._getLobPart.GetParametersMetaData().GetColumnCount()];
            args[0] = lobId;
            args[1] = offset;
            args[2] = limit;
            this._sysLobSession.sessionContext.PushDynamicArguments(args);
            this._sysLobSession.sessionContext.Pop(false);
            RowSetNavigator navigator = this._getLobPart.Execute(this._sysLobSession).GetNavigator();
            int size = navigator.GetSize();
            int[][] numArray = new int[size][];
            for (int i = 0; i < size; i++)
            {
                navigator.Absolute(i);
                object[] current = navigator.GetCurrent();
                numArray[i] = new int[] { Convert.ToInt32(current[0]), Convert.ToInt32(current[1]), Convert.ToInt32(current[2]) };
            }
            navigator.Close();
            return numArray;
        }

        public Result GetBytes(long lobId, long offset, int length)
        {
            lock (this)
            {
                byte[] blockBytes;
                int num = (int) (offset / ((long) this._lobBlockSize));
                int sourceIndex = (int) (offset % ((long) this._lobBlockSize));
                int limit = (int) ((offset + length) / ((long) this._lobBlockSize));
                if (((int) ((offset + length) % ((long) this._lobBlockSize))) == 0)
                {
                    int num1 = this._lobBlockSize;
                }
                else
                {
                    limit++;
                }
                int destinationIndex = 0;
                byte[] destinationArray = new byte[length];
                int[][] numArray = this.GetBlockAddresses(lobId, num, limit);
                if (numArray.Length == 0)
                {
                    return Result.NewErrorResult(Error.GetError(0xd92));
                }
                int index = 0;
                int blockCount = (numArray[index][1] + numArray[index][2]) - num;
                if ((numArray[index][1] + numArray[index][2]) > limit)
                {
                    blockCount -= (numArray[index][1] + numArray[index][2]) - limit;
                }
                try
                {
                    blockBytes = this._lobStore.GetBlockBytes(numArray[index][0] + num, blockCount);
                }
                catch (CoreException exception1)
                {
                    return Result.NewErrorResult(exception1);
                }
                int num7 = (this._lobBlockSize * blockCount) - sourceIndex;
                if (num7 > length)
                {
                    num7 = length;
                }
                Array.Copy(blockBytes, sourceIndex, destinationArray, destinationIndex, num7);
                destinationIndex += num7;
                index++;
                while ((index < numArray.Length) && (destinationIndex < length))
                {
                    blockCount = numArray[index][1];
                    if ((numArray[index][1] + numArray[index][2]) > limit)
                    {
                        blockCount -= (numArray[index][1] + numArray[index][2]) - limit;
                    }
                    try
                    {
                        blockBytes = this._lobStore.GetBlockBytes(numArray[index][0], blockCount);
                    }
                    catch (CoreException exception2)
                    {
                        return Result.NewErrorResult(exception2);
                    }
                    num7 = this._lobBlockSize * blockCount;
                    if (num7 > (length - destinationIndex))
                    {
                        num7 = length - destinationIndex;
                    }
                    Array.Copy(blockBytes, 0, destinationArray, destinationIndex, num7);
                    destinationIndex += num7;
                    index++;
                }
                return ResultLob.NewLobGetBytesResponse(lobId, offset, destinationArray);
            }
        }

        public Result GetChars(Session session, long lobId, long offset, int length)
        {
            lock (this)
            {
                Result result2 = this.GetBytes(lobId, offset * 2L, length * 2);
                if (result2.IsError())
                {
                    return result2;
                }
                char[] chars = ArrayUtil.ByteArrayToChars(((ResultLob) result2).GetByteArray());
                return ResultLob.NewLobGetCharsResponse(lobId, offset, chars);
            }
        }

        public Result GetLength(long lobId)
        {
            Result result;
            Monitor.Enter(this);
            try
            {
                object[] lobHeader = this.GetLobHeader(lobId);
                if (lobHeader == null)
                {
                    throw Error.GetError(0xd92);
                }
                long length = (long) lobHeader[1];
                int num1 = (int) lobHeader[3];
                result = ResultLob.NewLobSetResponse(lobId, length);
            }
            catch (CoreException exception1)
            {
                result = Result.NewErrorResult(exception1);
            }
            finally
            {
                Monitor.Exit(this);
            }
            return result;
        }

        public Result GetLob(long lobId, long offset, long length)
        {
            lock (this)
            {
                throw Error.RuntimeError(0xc9, "LobManager");
            }
        }

        public int GetLobCount()
        {
            lock (this)
            {
                this._sysLobSession.sessionContext.PushDynamicArguments(new object[0]);
                this._sysLobSession.sessionContext.Pop(false);
                RowSetNavigator navigator = this._getLobCount.Execute(this._sysLobSession).GetNavigator();
                if (!navigator.Next())
                {
                    navigator.Close();
                    return 0;
                }
                return Convert.ToInt32(navigator.GetCurrent()[0]);
            }
        }

        private object[] GetLobHeader(long lobId)
        {
            object[] args = new object[this._getLob.GetParametersMetaData().GetColumnCount()];
            args[0] = lobId;
            this._sysLobSession.sessionContext.PushDynamicArguments(args);
            Result result = this._getLob.Execute(this._sysLobSession);
            this._sysLobSession.sessionContext.Pop(false);
            if (result.IsError())
            {
                return null;
            }
            RowSetNavigator navigator = result.GetNavigator();
            if (!navigator.Next())
            {
                navigator.Close();
                return null;
            }
            return navigator.GetCurrent();
        }

        private long GetNewLobId()
        {
            Result result = this._getNextLobId.Execute(this._sysLobSession);
            if (result.IsError())
            {
                return 0L;
            }
            RowSetNavigator navigator = result.GetNavigator();
            if (!navigator.Next())
            {
                navigator.Close();
                return 0L;
            }
            return Convert.ToInt64(navigator.GetCurrent()[0]);
        }

        public void InitialiseLobSpace()
        {
            lock (this)
            {
                Statement cs = this._sysLobSession.CompileStatement("INSERT INTO SYSTEM_LOBS.BLOCKS VALUES(?,?,?)", ResultProperties.DefaultPropsValue);
                object[] pvals = new object[] { 0, this._totalBlockLimitCount, 0L };
                this._sysLobSession.ExecuteCompiledStatement(cs, pvals);
            }
        }

        public void Open()
        {
            lock (this)
            {
                this._lobBlockSize = this._database.logger.GetLobBlockSize();
                if (this._database.GetDatabaseType() == "res:")
                {
                    this._lobStore = new LobStoreInAssembly(this._database, this._lobBlockSize);
                }
                else if (this._database.GetDatabaseType() == "file:")
                {
                    this._lobStore = new LobStoreRAFile(this._database, this._lobBlockSize);
                }
                else
                {
                    this._lobStore = new LobStoreMem(this._lobBlockSize);
                }
            }
        }

        public Result SetBytes(long lobId, byte[] dataBytes, long offset)
        {
            lock (this)
            {
                if (dataBytes.Length == 0)
                {
                    return ResultLob.NewLobSetResponse(lobId, 0L);
                }
                object[] lobHeader = this.GetLobHeader(lobId);
                if (lobHeader == null)
                {
                    return Result.NewErrorResult(Error.GetError(0xd92));
                }
                long num = Convert.ToInt64(lobHeader[1]);
                if ((offset + dataBytes.Length) > num)
                {
                    this.SetLength(lobId, offset + dataBytes.Length);
                }
                return this.SetBytesBa(lobId, dataBytes, offset, dataBytes.Length);
            }
        }

        public Result SetBytesBa(long lobId, byte[] dataBytes, long offset, int length)
        {
            lock (this)
            {
                int num = (int) (offset / ((long) this._lobBlockSize));
                int destinationIndex = (int) (offset % ((long) this._lobBlockSize));
                int limit = (int) ((offset + length) / ((long) this._lobBlockSize));
                if (((int) ((offset + length) % ((long) this._lobBlockSize))) == 0)
                {
                    int num1 = this._lobBlockSize;
                }
                else
                {
                    limit++;
                }
                int[][] numArray = this.GetBlockAddresses(lobId, num, limit);
                byte[] destinationArray = new byte[(limit - num) * this._lobBlockSize];
                if (numArray.Length != 0)
                {
                    int blockAddress = numArray[0][0] + (num - numArray[0][2]);
                    try
                    {
                        Array.Copy(this._lobStore.GetBlockBytes(blockAddress, 1), 0, destinationArray, 0, this._lobBlockSize);
                        if (numArray.Length > 1)
                        {
                            blockAddress = numArray[numArray.Length - 1][0] + ((limit - numArray[numArray.Length - 1][2]) - 1);
                            Array.Copy(this._lobStore.GetBlockBytes(blockAddress, 1), 0, destinationArray, (limit - num) - 1, this._lobBlockSize);
                        }
                        else if ((limit - num) > 1)
                        {
                            blockAddress = numArray[0][0] + ((limit - numArray[0][2]) - 1);
                            Array.Copy(this._lobStore.GetBlockBytes(blockAddress, 1), 0, destinationArray, ((limit - num) - 1) * this._lobBlockSize, this._lobBlockSize);
                        }
                    }
                    catch (CoreException exception1)
                    {
                        return Result.NewErrorResult(exception1);
                    }
                    this.DivideBlockAddresses(lobId, num);
                    this.DivideBlockAddresses(lobId, limit);
                    this.DeleteBlockAddresses(lobId, num, limit);
                }
                this.CreateBlockAddresses(lobId, num, limit - num);
                Array.Copy(dataBytes, 0, destinationArray, destinationIndex, length);
                numArray = this.GetBlockAddresses(lobId, num, limit);
                try
                {
                    for (int i = 0; i < numArray.Length; i++)
                    {
                        this._lobStore.SetBlockBytes(destinationArray, numArray[i][0], numArray[i][1]);
                    }
                }
                catch (CoreException exception2)
                {
                    return Result.NewErrorResult(exception2);
                }
                return ResultLob.NewLobSetResponse(lobId, 0L);
            }
        }

        public Result SetBytesForNewBlob(long lobId, Stream inputStream, long length)
        {
            lock (this)
            {
                if (length == 0)
                {
                    return ResultLob.NewLobSetResponse(lobId, 0L);
                }
                return this.SetBytesIs(lobId, inputStream, length);
            }
        }

        private Result SetBytesIs(long lobId, Stream inputStream, long length)
        {
            int count = (int) (length / ((long) this._lobBlockSize));
            int num2 = (int) (length % ((long) this._lobBlockSize));
            if (num2 == 0)
            {
                num2 = this._lobBlockSize;
            }
            else
            {
                count++;
            }
            this.CreateBlockAddresses(lobId, 0, count);
            int[][] numArray = this.GetBlockAddresses(lobId, 0, count);
            byte[] buffer = new byte[this._lobBlockSize];
            for (int i = 0; i < numArray.Length; i++)
            {
                for (int j = 0; j < numArray[i][1]; j++)
                {
                    int num5 = this._lobBlockSize;
                    if ((i == (numArray.Length - 1)) && (j == (numArray[i][1] - 1)))
                    {
                        num5 = num2;
                        for (int k = num5; k < this._lobBlockSize; k++)
                        {
                            buffer[k] = 0;
                        }
                    }
                    try
                    {
                        int num8;
                        for (int k = 0; num5 > 0; k += num8)
                        {
                            num8 = inputStream.Read(buffer, k, num5);
                            if (num8 == 0)
                            {
                                return Result.NewErrorResult(new EndOfStreamException());
                            }
                            num5 -= num8;
                        }
                    }
                    catch (IOException exception1)
                    {
                        return Result.NewErrorResult(exception1);
                    }
                    try
                    {
                        this._lobStore.SetBlockBytes(buffer, numArray[i][0] + j, 1);
                    }
                    catch (CoreException exception2)
                    {
                        return Result.NewErrorResult(exception2);
                    }
                }
            }
            return ResultLob.NewLobSetResponse(lobId, 0L);
        }

        public Result SetChars(long lobId, long offset, char[] chars)
        {
            lock (this)
            {
                if (chars.Length != 0)
                {
                    object[] lobHeader = this.GetLobHeader(lobId);
                    if (lobHeader == null)
                    {
                        return Result.NewErrorResult(Error.GetError(0xd92));
                    }
                    long num = Convert.ToInt64(lobHeader[1]);
                    byte[] dataBytes = ArrayUtil.CharArrayToBytes(chars);
                    Result result2 = this.SetBytesBa(lobId, dataBytes, offset * 2L, chars.Length * 2);
                    if (result2.IsError())
                    {
                        return result2;
                    }
                    if ((offset + chars.Length) > num)
                    {
                        result2 = this.SetLength(lobId, offset + chars.Length);
                        if (result2.IsError())
                        {
                            return result2;
                        }
                    }
                }
                return ResultLob.NewLobSetResponse(lobId, 0L);
            }
        }

        public Result SetCharsForNewClob(long lobId, Stream inputStream, long length)
        {
            lock (this)
            {
                if (length != 0)
                {
                    Result result2 = this.SetBytesIs(lobId, inputStream, length * 2L);
                    if (result2.IsError())
                    {
                        return result2;
                    }
                }
                return ResultLob.NewLobSetResponse(lobId, 0L);
            }
        }

        public Result SetLength(long lobId, long length)
        {
            lock (this)
            {
                object[] pvals = new object[this._updateLobLength.GetParametersMetaData().GetColumnCount()];
                pvals[0] = length;
                pvals[1] = lobId;
                return this._sysLobSession.ExecuteCompiledStatement(this._updateLobLength, pvals);
            }
        }

        private static class AllocBlocks
        {
            public const int BlockCount = 0;
            public const int BlockOffset = 1;
            public const int LobId = 2;
        }

        private static class DeleteBlocks
        {
            public const int LobId = 0;
            public const int BlockOffset = 1;
            public const int BlockLimit = 2;
            public const int TxId = 3;
        }

        private static class DivideBlock
        {
            public const int BlockOffset = 0;
            public const int LobId = 1;
        }

        private static class GetLobPart
        {
            public const int LobId = 0;
            public const int BlockOffset = 1;
            public const int BlockLimit = 2;
        }

        private static class LobIds
        {
            public const int LobId = 0;
            public const int LobLength = 1;
            public const int LobUsageCount = 2;
            public const int LobType = 3;
        }

        private static class Lobs
        {
            public const int BlockAddr = 0;
            public const int BlockCount = 1;
            public const int BlockOffset = 2;
            public const int LobId = 3;
        }

        private static class UpdateLength
        {
            public const int LobLength = 0;
            public const int LobId = 1;
        }

        private static class UpdateUsage
        {
            public const int BlockCount = 0;
            public const int LobId = 1;
        }
    }
}

