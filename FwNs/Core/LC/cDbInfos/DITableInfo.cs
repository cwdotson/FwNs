namespace FwNs.Core.LC.cDbInfos
{
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Globalization;

    public sealed class DITableInfo
    {
        public const short TableIndexOther = 3;
        private int _hndColumnRemarks = -1;
        private int _hndTableRemarks = -1;
        private Table _table;
        public int BestRowNotPseudo = 1;
        public int BestRowSession = 2;
        public int BestRowTemporary;
        public int BestRowTransaction = 1;
        public int BestRowUnknown;

        public DITableInfo()
        {
            this.SetLocale(CultureInfo.CurrentCulture);
        }

        public string GetAdoStandardType()
        {
            switch (this._table.GetTableType())
            {
                case 1:
                    return "SYSTEM TABLE";

                case 3:
                    return "GLOBAL TEMPORARY";

                case 8:
                    return "VIEW";
            }
            if (this._table.GetOwner().IsSystem)
            {
                return "SYSTEM TABLE";
            }
            return "TABLE";
        }

        public int GetBriPseudo()
        {
            return this.BestRowNotPseudo;
        }

        public int GetBriScope()
        {
            if (!this._table.IsWritable())
            {
                return this.BestRowSession;
            }
            return this.BestRowTemporary;
        }

        public int GetColDataTypeSub(int i)
        {
            if (this._table.GetColumn(i).GetDataType().GetAdoTypeCode() != 100)
            {
                return 1;
            }
            return 4;
        }

        private string GetColName(int i)
        {
            return this._table.GetColumn(i).GetName().Name;
        }

        public string GetColRemarks(int i)
        {
            if (this._table.GetTableType() != 1)
            {
                return null;
            }
            string key = this.GetName() + "_" + this.GetColName(i);
            return BundleHandler.GetString(this._hndColumnRemarks, key);
        }

        public string GetName()
        {
            return this._table.GetName().Name;
        }

        public string GetRemark()
        {
            if (this._table.GetTableType() != 1)
            {
                return this._table.GetName().Comment;
            }
            return BundleHandler.GetString(this._hndTableRemarks, this.GetName());
        }

        public Table GetTable()
        {
            return this._table;
        }

        public string GetUtlType()
        {
            switch (this._table.GetTableType())
            {
                case 1:
                case 3:
                case 4:
                    return "MEMORY";

                case 5:
                    return "CACHED";
            }
            return null;
        }

        public void SetLocale(CultureInfo l)
        {
            lock (BundleHandler.SetGlobalLock)
            {
                BundleHandler.SetLocale(l);
                this._hndColumnRemarks = BundleHandler.GetBundleHandle("column-remarks", null);
                this._hndTableRemarks = BundleHandler.GetBundleHandle("table-remarks", null);
                BundleHandler.SetLocale(BundleHandler.GetLocale());
            }
        }

        public void SetTable(Table table)
        {
            this._table = table;
        }
    }
}

