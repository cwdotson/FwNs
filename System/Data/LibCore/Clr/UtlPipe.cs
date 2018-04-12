namespace System.Data.LibCore.Clr
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cResults;
    using System;
    using System.Collections.Generic;
    using System.Data.LibCore;

    public class UtlPipe
    {
        private readonly Session _session;
        private List<UtlDataRecord> _recordList;

        public UtlPipe(Session session)
        {
            this._session = session;
        }

        private static void CheckRecordSchema(UtlDataRecord record1, UtlDataRecord record2)
        {
            if (record1.FieldCount != record2.FieldCount)
            {
                throw new ArgumentException();
            }
            System.Data.LibCore.Clr.UtlMetaData[] metaData = record1.GetMetaData();
            System.Data.LibCore.Clr.UtlMetaData[] dataArray2 = record2.GetMetaData();
            for (int i = 0; i < metaData.Length; i++)
            {
                if (((metaData[i].Name != dataArray2[i].Name) || (metaData[i].UtlDbType != dataArray2[i].UtlDbType)) || (((metaData[i].MaxLength != dataArray2[i].MaxLength) || (metaData[i].Precision != dataArray2[i].Precision)) || (metaData[i].Scale != dataArray2[i].Scale)))
                {
                    throw new ArgumentException();
                }
            }
        }

        public void ExecuteAndSend(UtlCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
            if (this._recordList != null)
            {
                throw new InvalidOperationException();
            }
            Result internalResult = command.ExecuteReader().GetInternalResult();
            this._session.AddResultSet(internalResult);
        }

        private static SqlType GetDataType(System.Data.LibCore.Clr.UtlMetaData metaData)
        {
            SqlType defaultType = SqlType.GetDefaultType((int) metaData.UtlDbType);
            if (defaultType.AcceptsPrecision())
            {
                defaultType.Precision = metaData.Precision;
            }
            if (defaultType.AcceptsScale())
            {
                defaultType.Scale = metaData.Scale;
            }
            return defaultType;
        }

        private static ResultMetaData GetResultMetaData(UtlDataRecord record)
        {
            System.Data.LibCore.Clr.UtlMetaData[] metaData = record.GetMetaData();
            int length = metaData.Length;
            int[] baseColumnIndexes = new int[length];
            SqlType[] types = new SqlType[length];
            for (int i = 0; i < length; i++)
            {
                baseColumnIndexes[i] = i;
                types[i] = GetDataType(metaData[i]);
            }
            return ResultMetaData.NewResultMetaData(types, baseColumnIndexes, length, length);
        }

        public Session GetSession()
        {
            return this._session;
        }

        public void Send(UtlDataRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException();
            }
            Result result = Result.NewDataResult(GetResultMetaData(record));
            result.InitialiseNavigator().Add(record.GetValuesDuplicate());
            this._session.AddResultSet(result);
        }

        public void Send(UtlDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException();
            }
            Result internalResult = reader.GetInternalResult();
            this._session.AddResultSet(internalResult);
        }

        public void SendResultsEnd()
        {
            if (this._recordList == null)
            {
                throw new InvalidOperationException();
            }
            if (this._recordList.Count == 0)
            {
                throw new InvalidOperationException();
            }
            Result result = Result.NewDataResult(GetResultMetaData(this._recordList[0]));
            RowSetNavigator navigator = result.InitialiseNavigator();
            foreach (UtlDataRecord record in this._recordList)
            {
                navigator.Add(record.GetValuesDuplicate());
            }
            this._session.AddResultSet(result);
            this._recordList = null;
        }

        public void SendResultsRow(UtlDataRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException();
            }
            if (this._recordList == null)
            {
                throw new InvalidOperationException();
            }
            CheckRecordSchema(this._recordList[0], record);
            this._recordList.Add(record);
        }

        public void SendResultsStart(UtlDataRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException();
            }
            if (record.FieldCount == 0)
            {
                throw new ArgumentException();
            }
            List<UtlDataRecord> list1 = new List<UtlDataRecord> {
                record
            };
            this._recordList = list1;
        }
    }
}

