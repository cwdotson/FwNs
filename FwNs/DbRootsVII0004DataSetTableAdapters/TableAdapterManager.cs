namespace FwNs.DbRootsVII0004DataSetTableAdapters
{
    using FwNs;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [DesignerCategory("code"), ToolboxItem(true), Designer("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerDesigner, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), HelpKeyword("vs.data.TableAdapterManager")]
    public class TableAdapterManager : Component
    {
        private UpdateOrderOption _updateOrder;
        private FwNs.DbRootsVII0004DataSetTableAdapters.CtnrsTableAdapter _ctnrsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.CTypsTableAdapter _cTypsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.DbRootsTableAdapter _dbRootsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.DrvsTableAdapter _drvsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.FTypsTableAdapter _fTypsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.ItmsTableAdapter _itmsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.MchnsTableAdapter _mchnsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.PartsTableAdapter _partsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.TblIdsTableAdapter _tblIdsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.UsrsTableAdapter _usrsTableAdapter;
        private FwNs.DbRootsVII0004DataSetTableAdapters.WrdsTableAdapter _wrdsTableAdapter;
        private bool _backupDataSetBeforeUpdate;
        private IDbConnection _connection;

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private DataRow[] GetRealUpdatedRows(DataRow[] updatedRows, List<DataRow> allAddedRows)
        {
            if ((updatedRows == null) || (updatedRows.Length < 1))
            {
                return updatedRows;
            }
            if ((allAddedRows == null) || (allAddedRows.Count < 1))
            {
                return updatedRows;
            }
            List<DataRow> list = new List<DataRow>();
            for (int i = 0; i < updatedRows.Length; i++)
            {
                DataRow item = updatedRows[i];
                if (!allAddedRows.Contains(item))
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected virtual bool MatchTableAdapterConnection(IDbConnection inputConnection)
        {
            return ((this._connection != null) || (((this.Connection == null) || (inputConnection == null)) || string.Equals(this.Connection.ConnectionString, inputConnection.ConnectionString, StringComparison.Ordinal)));
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected virtual void SortSelfReferenceRows(DataRow[] rows, DataRelation relation, bool childFirst)
        {
            Array.Sort<DataRow>(rows, new SelfReferenceComparer(relation, childFirst));
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public virtual int UpdateAll(DbRootsVII0004DataSet dataSet)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            if (!dataSet.HasChanges())
            {
                return 0;
            }
            if ((this._ctnrsTableAdapter != null) && !this.MatchTableAdapterConnection(this._ctnrsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._cTypsTableAdapter != null) && !this.MatchTableAdapterConnection(this._cTypsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._dbRootsTableAdapter != null) && !this.MatchTableAdapterConnection(this._dbRootsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._drvsTableAdapter != null) && !this.MatchTableAdapterConnection(this._drvsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._fTypsTableAdapter != null) && !this.MatchTableAdapterConnection(this._fTypsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._itmsTableAdapter != null) && !this.MatchTableAdapterConnection(this._itmsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._mchnsTableAdapter != null) && !this.MatchTableAdapterConnection(this._mchnsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._partsTableAdapter != null) && !this.MatchTableAdapterConnection(this._partsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._tblIdsTableAdapter != null) && !this.MatchTableAdapterConnection(this._tblIdsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._usrsTableAdapter != null) && !this.MatchTableAdapterConnection(this._usrsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            if ((this._wrdsTableAdapter != null) && !this.MatchTableAdapterConnection(this._wrdsTableAdapter.Connection))
            {
                throw new ArgumentException("All TableAdapters managed by a TableAdapterManager must use the same connection string.");
            }
            IDbConnection connection = this.Connection;
            if (connection == null)
            {
                throw new ApplicationException("TableAdapterManager contains no connection information. Set each TableAdapterManager TableAdapter property to a valid TableAdapter instance.");
            }
            bool flag = false;
            if ((connection.State & ConnectionState.Broken) == ConnectionState.Broken)
            {
                connection.Close();
            }
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                flag = true;
            }
            IDbTransaction transaction = connection.BeginTransaction();
            if (transaction == null)
            {
                throw new ApplicationException("The transaction cannot begin. The current data connection does not support transactions or the current state is not allowing the transaction to begin.");
            }
            List<DataRow> allChangedRows = new List<DataRow>();
            List<DataRow> allAddedRows = new List<DataRow>();
            List<DataAdapter> list3 = new List<DataAdapter>();
            Dictionary<object, IDbConnection> dictionary = new Dictionary<object, IDbConnection>();
            int num = 0;
            DataSet set = null;
            if (this.BackupDataSetBeforeUpdate)
            {
                set = new DataSet();
                set.Merge(dataSet);
            }
            try
            {
                if (this._ctnrsTableAdapter != null)
                {
                    dictionary.Add(this._ctnrsTableAdapter, this._ctnrsTableAdapter.Connection);
                    this._ctnrsTableAdapter.Connection = (SqlConnection) connection;
                    this._ctnrsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._ctnrsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._ctnrsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._ctnrsTableAdapter.Adapter);
                    }
                }
                if (this._cTypsTableAdapter != null)
                {
                    dictionary.Add(this._cTypsTableAdapter, this._cTypsTableAdapter.Connection);
                    this._cTypsTableAdapter.Connection = (SqlConnection) connection;
                    this._cTypsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._cTypsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._cTypsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._cTypsTableAdapter.Adapter);
                    }
                }
                if (this._dbRootsTableAdapter != null)
                {
                    dictionary.Add(this._dbRootsTableAdapter, this._dbRootsTableAdapter.Connection);
                    this._dbRootsTableAdapter.Connection = (SqlConnection) connection;
                    this._dbRootsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._dbRootsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._dbRootsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._dbRootsTableAdapter.Adapter);
                    }
                }
                if (this._drvsTableAdapter != null)
                {
                    dictionary.Add(this._drvsTableAdapter, this._drvsTableAdapter.Connection);
                    this._drvsTableAdapter.Connection = (SqlConnection) connection;
                    this._drvsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._drvsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._drvsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._drvsTableAdapter.Adapter);
                    }
                }
                if (this._fTypsTableAdapter != null)
                {
                    dictionary.Add(this._fTypsTableAdapter, this._fTypsTableAdapter.Connection);
                    this._fTypsTableAdapter.Connection = (SqlConnection) connection;
                    this._fTypsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._fTypsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._fTypsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._fTypsTableAdapter.Adapter);
                    }
                }
                if (this._itmsTableAdapter != null)
                {
                    dictionary.Add(this._itmsTableAdapter, this._itmsTableAdapter.Connection);
                    this._itmsTableAdapter.Connection = (SqlConnection) connection;
                    this._itmsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._itmsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._itmsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._itmsTableAdapter.Adapter);
                    }
                }
                if (this._mchnsTableAdapter != null)
                {
                    dictionary.Add(this._mchnsTableAdapter, this._mchnsTableAdapter.Connection);
                    this._mchnsTableAdapter.Connection = (SqlConnection) connection;
                    this._mchnsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._mchnsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._mchnsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._mchnsTableAdapter.Adapter);
                    }
                }
                if (this._partsTableAdapter != null)
                {
                    dictionary.Add(this._partsTableAdapter, this._partsTableAdapter.Connection);
                    this._partsTableAdapter.Connection = (SqlConnection) connection;
                    this._partsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._partsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._partsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._partsTableAdapter.Adapter);
                    }
                }
                if (this._tblIdsTableAdapter != null)
                {
                    dictionary.Add(this._tblIdsTableAdapter, this._tblIdsTableAdapter.Connection);
                    this._tblIdsTableAdapter.Connection = (SqlConnection) connection;
                    this._tblIdsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._tblIdsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._tblIdsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._tblIdsTableAdapter.Adapter);
                    }
                }
                if (this._usrsTableAdapter != null)
                {
                    dictionary.Add(this._usrsTableAdapter, this._usrsTableAdapter.Connection);
                    this._usrsTableAdapter.Connection = (SqlConnection) connection;
                    this._usrsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._usrsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._usrsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._usrsTableAdapter.Adapter);
                    }
                }
                if (this._wrdsTableAdapter != null)
                {
                    dictionary.Add(this._wrdsTableAdapter, this._wrdsTableAdapter.Connection);
                    this._wrdsTableAdapter.Connection = (SqlConnection) connection;
                    this._wrdsTableAdapter.Transaction = (SqlTransaction) transaction;
                    if (this._wrdsTableAdapter.Adapter.AcceptChangesDuringUpdate)
                    {
                        this._wrdsTableAdapter.Adapter.AcceptChangesDuringUpdate = false;
                        list3.Add(this._wrdsTableAdapter.Adapter);
                    }
                }
                if (this.UpdateOrder == UpdateOrderOption.UpdateInsertDelete)
                {
                    num += this.UpdateUpdatedRows(dataSet, allChangedRows, allAddedRows);
                    num += this.UpdateInsertedRows(dataSet, allAddedRows);
                }
                else
                {
                    num += this.UpdateInsertedRows(dataSet, allAddedRows);
                    num += this.UpdateUpdatedRows(dataSet, allChangedRows, allAddedRows);
                }
                num += this.UpdateDeletedRows(dataSet, allChangedRows);
                transaction.Commit();
                if (0 < allAddedRows.Count)
                {
                    DataRow[] rowArray = new DataRow[allAddedRows.Count];
                    allAddedRows.CopyTo(rowArray);
                    for (int j = 0; j < rowArray.Length; j++)
                    {
                        rowArray[j].AcceptChanges();
                    }
                }
                if (0 >= allChangedRows.Count)
                {
                    return num;
                }
                DataRow[] array = new DataRow[allChangedRows.Count];
                allChangedRows.CopyTo(array);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].AcceptChanges();
                }
            }
            catch (Exception exception)
            {
                transaction.Rollback();
                if (this.BackupDataSetBeforeUpdate)
                {
                    dataSet.Clear();
                    dataSet.Merge(set);
                }
                else if (0 < allAddedRows.Count)
                {
                    DataRow[] array = new DataRow[allAddedRows.Count];
                    allAddedRows.CopyTo(array);
                    for (int i = 0; i < array.Length; i++)
                    {
                        DataRow row1 = array[i];
                        row1.AcceptChanges();
                        row1.SetAdded();
                    }
                }
                throw exception;
            }
            finally
            {
                if (flag)
                {
                    connection.Close();
                }
                if (this._ctnrsTableAdapter != null)
                {
                    this._ctnrsTableAdapter.Connection = (SqlConnection) dictionary[this._ctnrsTableAdapter];
                    this._ctnrsTableAdapter.Transaction = null;
                }
                if (this._cTypsTableAdapter != null)
                {
                    this._cTypsTableAdapter.Connection = (SqlConnection) dictionary[this._cTypsTableAdapter];
                    this._cTypsTableAdapter.Transaction = null;
                }
                if (this._dbRootsTableAdapter != null)
                {
                    this._dbRootsTableAdapter.Connection = (SqlConnection) dictionary[this._dbRootsTableAdapter];
                    this._dbRootsTableAdapter.Transaction = null;
                }
                if (this._drvsTableAdapter != null)
                {
                    this._drvsTableAdapter.Connection = (SqlConnection) dictionary[this._drvsTableAdapter];
                    this._drvsTableAdapter.Transaction = null;
                }
                if (this._fTypsTableAdapter != null)
                {
                    this._fTypsTableAdapter.Connection = (SqlConnection) dictionary[this._fTypsTableAdapter];
                    this._fTypsTableAdapter.Transaction = null;
                }
                if (this._itmsTableAdapter != null)
                {
                    this._itmsTableAdapter.Connection = (SqlConnection) dictionary[this._itmsTableAdapter];
                    this._itmsTableAdapter.Transaction = null;
                }
                if (this._mchnsTableAdapter != null)
                {
                    this._mchnsTableAdapter.Connection = (SqlConnection) dictionary[this._mchnsTableAdapter];
                    this._mchnsTableAdapter.Transaction = null;
                }
                if (this._partsTableAdapter != null)
                {
                    this._partsTableAdapter.Connection = (SqlConnection) dictionary[this._partsTableAdapter];
                    this._partsTableAdapter.Transaction = null;
                }
                if (this._tblIdsTableAdapter != null)
                {
                    this._tblIdsTableAdapter.Connection = (SqlConnection) dictionary[this._tblIdsTableAdapter];
                    this._tblIdsTableAdapter.Transaction = null;
                }
                if (this._usrsTableAdapter != null)
                {
                    this._usrsTableAdapter.Connection = (SqlConnection) dictionary[this._usrsTableAdapter];
                    this._usrsTableAdapter.Transaction = null;
                }
                if (this._wrdsTableAdapter != null)
                {
                    this._wrdsTableAdapter.Connection = (SqlConnection) dictionary[this._wrdsTableAdapter];
                    this._wrdsTableAdapter.Transaction = null;
                }
                if (0 < list3.Count)
                {
                    DataAdapter[] array = new DataAdapter[list3.Count];
                    list3.CopyTo(array);
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].AcceptChangesDuringUpdate = true;
                    }
                }
            }
            return num;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private int UpdateDeletedRows(DbRootsVII0004DataSet dataSet, List<DataRow> allChangedRows)
        {
            int num = 0;
            if (this._usrsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Usrs.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._usrsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._tblIdsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.TblIds.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._tblIdsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._partsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Parts.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._partsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._mchnsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Mchns.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._mchnsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._itmsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Itms.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._itmsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._cTypsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.CTyps.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._cTypsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._fTypsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.FTyps.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._fTypsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._ctnrsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Ctnrs.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._ctnrsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._wrdsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Wrds.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._wrdsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._drvsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Drvs.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._drvsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            if (this._dbRootsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.DbRoots.Select(null, null, DataViewRowState.Deleted);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._dbRootsTableAdapter.Update(dataRows);
                    allChangedRows.AddRange(dataRows);
                }
            }
            return num;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private int UpdateInsertedRows(DbRootsVII0004DataSet dataSet, List<DataRow> allAddedRows)
        {
            int num = 0;
            if (this._dbRootsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.DbRoots.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._dbRootsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._drvsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Drvs.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._drvsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._wrdsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Wrds.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._wrdsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._ctnrsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Ctnrs.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._ctnrsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._fTypsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.FTyps.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._fTypsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._cTypsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.CTyps.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._cTypsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._itmsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Itms.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._itmsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._mchnsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Mchns.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._mchnsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._partsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Parts.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._partsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._tblIdsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.TblIds.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._tblIdsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            if (this._usrsTableAdapter != null)
            {
                DataRow[] dataRows = dataSet.Usrs.Select(null, null, DataViewRowState.Added);
                if ((dataRows != null) && (dataRows.Length != 0))
                {
                    num += this._usrsTableAdapter.Update(dataRows);
                    allAddedRows.AddRange(dataRows);
                }
            }
            return num;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private int UpdateUpdatedRows(DbRootsVII0004DataSet dataSet, List<DataRow> allChangedRows, List<DataRow> allAddedRows)
        {
            int num = 0;
            if (this._dbRootsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.DbRoots.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._dbRootsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._drvsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.Drvs.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._drvsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._wrdsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.Wrds.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._wrdsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._ctnrsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.Ctnrs.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._ctnrsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._fTypsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.FTyps.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._fTypsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._cTypsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.CTyps.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._cTypsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._itmsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.Itms.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._itmsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._mchnsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.Mchns.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._mchnsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._partsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.Parts.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._partsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._tblIdsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.TblIds.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._tblIdsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            if (this._usrsTableAdapter != null)
            {
                DataRow[] updatedRows = dataSet.Usrs.Select(null, null, DataViewRowState.ModifiedCurrent);
                updatedRows = this.GetRealUpdatedRows(updatedRows, allAddedRows);
                if ((updatedRows != null) && (updatedRows.Length != 0))
                {
                    num += this._usrsTableAdapter.Update(updatedRows);
                    allChangedRows.AddRange(updatedRows);
                }
            }
            return num;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public UpdateOrderOption UpdateOrder
        {
            get
            {
                return this._updateOrder;
            }
            set
            {
                this._updateOrder = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.CtnrsTableAdapter CtnrsTableAdapter
        {
            get
            {
                return this._ctnrsTableAdapter;
            }
            set
            {
                this._ctnrsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.CTypsTableAdapter CTypsTableAdapter
        {
            get
            {
                return this._cTypsTableAdapter;
            }
            set
            {
                this._cTypsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.DbRootsTableAdapter DbRootsTableAdapter
        {
            get
            {
                return this._dbRootsTableAdapter;
            }
            set
            {
                this._dbRootsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.DrvsTableAdapter DrvsTableAdapter
        {
            get
            {
                return this._drvsTableAdapter;
            }
            set
            {
                this._drvsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.FTypsTableAdapter FTypsTableAdapter
        {
            get
            {
                return this._fTypsTableAdapter;
            }
            set
            {
                this._fTypsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.ItmsTableAdapter ItmsTableAdapter
        {
            get
            {
                return this._itmsTableAdapter;
            }
            set
            {
                this._itmsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.MchnsTableAdapter MchnsTableAdapter
        {
            get
            {
                return this._mchnsTableAdapter;
            }
            set
            {
                this._mchnsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.PartsTableAdapter PartsTableAdapter
        {
            get
            {
                return this._partsTableAdapter;
            }
            set
            {
                this._partsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.TblIdsTableAdapter TblIdsTableAdapter
        {
            get
            {
                return this._tblIdsTableAdapter;
            }
            set
            {
                this._tblIdsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.UsrsTableAdapter UsrsTableAdapter
        {
            get
            {
                return this._usrsTableAdapter;
            }
            set
            {
                this._usrsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Editor("Microsoft.VSDesigner.DataSource.Design.TableAdapterManagerPropertyEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor")]
        public FwNs.DbRootsVII0004DataSetTableAdapters.WrdsTableAdapter WrdsTableAdapter
        {
            get
            {
                return this._wrdsTableAdapter;
            }
            set
            {
                this._wrdsTableAdapter = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public bool BackupDataSetBeforeUpdate
        {
            get
            {
                return this._backupDataSetBeforeUpdate;
            }
            set
            {
                this._backupDataSetBeforeUpdate = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
        public IDbConnection Connection
        {
            get
            {
                if (this._connection != null)
                {
                    return this._connection;
                }
                if ((this._ctnrsTableAdapter != null) && (this._ctnrsTableAdapter.Connection != null))
                {
                    return this._ctnrsTableAdapter.Connection;
                }
                if ((this._cTypsTableAdapter != null) && (this._cTypsTableAdapter.Connection != null))
                {
                    return this._cTypsTableAdapter.Connection;
                }
                if ((this._dbRootsTableAdapter != null) && (this._dbRootsTableAdapter.Connection != null))
                {
                    return this._dbRootsTableAdapter.Connection;
                }
                if ((this._drvsTableAdapter != null) && (this._drvsTableAdapter.Connection != null))
                {
                    return this._drvsTableAdapter.Connection;
                }
                if ((this._fTypsTableAdapter != null) && (this._fTypsTableAdapter.Connection != null))
                {
                    return this._fTypsTableAdapter.Connection;
                }
                if ((this._itmsTableAdapter != null) && (this._itmsTableAdapter.Connection != null))
                {
                    return this._itmsTableAdapter.Connection;
                }
                if ((this._mchnsTableAdapter != null) && (this._mchnsTableAdapter.Connection != null))
                {
                    return this._mchnsTableAdapter.Connection;
                }
                if ((this._partsTableAdapter != null) && (this._partsTableAdapter.Connection != null))
                {
                    return this._partsTableAdapter.Connection;
                }
                if ((this._tblIdsTableAdapter != null) && (this._tblIdsTableAdapter.Connection != null))
                {
                    return this._tblIdsTableAdapter.Connection;
                }
                if ((this._usrsTableAdapter != null) && (this._usrsTableAdapter.Connection != null))
                {
                    return this._usrsTableAdapter.Connection;
                }
                if ((this._wrdsTableAdapter != null) && (this._wrdsTableAdapter.Connection != null))
                {
                    return this._wrdsTableAdapter.Connection;
                }
                return null;
            }
            set
            {
                this._connection = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
        public int TableAdapterInstanceCount
        {
            get
            {
                int num = 0;
                if (this._ctnrsTableAdapter != null)
                {
                    num++;
                }
                if (this._cTypsTableAdapter != null)
                {
                    num++;
                }
                if (this._dbRootsTableAdapter != null)
                {
                    num++;
                }
                if (this._drvsTableAdapter != null)
                {
                    num++;
                }
                if (this._fTypsTableAdapter != null)
                {
                    num++;
                }
                if (this._itmsTableAdapter != null)
                {
                    num++;
                }
                if (this._mchnsTableAdapter != null)
                {
                    num++;
                }
                if (this._partsTableAdapter != null)
                {
                    num++;
                }
                if (this._tblIdsTableAdapter != null)
                {
                    num++;
                }
                if (this._usrsTableAdapter != null)
                {
                    num++;
                }
                if (this._wrdsTableAdapter != null)
                {
                    num++;
                }
                return num;
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private class SelfReferenceComparer : IComparer<DataRow>
        {
            private DataRelation _relation;
            private int _childFirst;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal SelfReferenceComparer(DataRelation relation, bool childFirst)
            {
                this._relation = relation;
                if (childFirst)
                {
                    this._childFirst = -1;
                }
                else
                {
                    this._childFirst = 1;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Compare(DataRow row1, DataRow row2)
            {
                if (row1 == row2)
                {
                    return 0;
                }
                if (row1 == null)
                {
                    return -1;
                }
                if (row2 != null)
                {
                    int distance = 0;
                    DataRow root = this.GetRoot(row1, out distance);
                    int num2 = 0;
                    DataRow row = this.GetRoot(row2, out num2);
                    if (root == row)
                    {
                        return (this._childFirst * distance.CompareTo(num2));
                    }
                    if (root.Table.Rows.IndexOf(root) < row.Table.Rows.IndexOf(row))
                    {
                        return -1;
                    }
                }
                return 1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private DataRow GetRoot(DataRow row, out int distance)
            {
                DataRow row3;
                DataRow row2 = row;
                distance = 0;
                IDictionary<DataRow, DataRow> dictionary = new Dictionary<DataRow, DataRow>();
                dictionary[row] = row;
                for (row3 = row.GetParentRow(this._relation, DataRowVersion.Default); (row3 != null) && !dictionary.ContainsKey(row3); row3 = row3.GetParentRow(this._relation, DataRowVersion.Default))
                {
                    distance++;
                    row2 = row3;
                    dictionary[row3] = row3;
                }
                if (distance == 0)
                {
                    dictionary.Clear();
                    dictionary[row] = row;
                    for (row3 = row.GetParentRow(this._relation, DataRowVersion.Original); (row3 != null) && !dictionary.ContainsKey(row3); row3 = row3.GetParentRow(this._relation, DataRowVersion.Original))
                    {
                        distance++;
                        row2 = row3;
                        dictionary[row3] = row3;
                    }
                }
                return row2;
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public enum UpdateOrderOption
        {
            InsertUpdateDelete,
            UpdateInsertDelete
        }
    }
}

