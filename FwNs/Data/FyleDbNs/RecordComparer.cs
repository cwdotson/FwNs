namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections.Generic;

    internal class RecordComparer : IComparer<Record>
    {
        private List<Field> _fieldLst;
        private List<bool> _sortDirLst;
        private List<bool> _caseLst;

        internal RecordComparer(List<Field> fieldLst, List<bool> sortDirLst, List<bool> caseLst)
        {
            this._fieldLst = fieldLst;
            this._caseLst = caseLst;
            this._sortDirLst = sortDirLst;
        }

        public int Compare(Record x, Record y)
        {
            if ((x == null) || (y == null))
            {
                return 0;
            }
            for (int i = 0; i < this._fieldLst.Count; i++)
            {
                Field field = this._fieldLst[i];
                bool caseInsensitive = this._caseLst[i];
                object obj2 = y[field.Ordinal];
                int num3 = FileDbEngine.CompareVals(x[field.Ordinal], obj2, field.DataType, caseInsensitive);
                if (this._sortDirLst[i])
                {
                    num3 = -num3;
                }
                if (num3 != 0)
                {
                    return num3;
                }
            }
            return 0;
        }
    }
}

