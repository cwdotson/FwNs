namespace FwNs.Core.LC.cConstraints
{
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class ConstraintCore
    {
        public QNameManager.QName RefName;
        public QNameManager.QName MainName;
        public QNameManager.QName UniqueName;
        public QNameManager.QName RefTableName;
        public QNameManager.QName MainTableName;
        public Table MainTable;
        public int[] MainCols;
        public Index MainIndex;
        public Table RefTable;
        public int[] RefCols;
        public Index RefIndex;
        public int DeleteAction;
        public int UpdateAction;
        public bool HasUpdateAction;
        public bool HasDeleteAction;
        public int MatchType;

        public ConstraintCore Duplicate()
        {
            return new ConstraintCore { 
                RefName = this.RefName,
                MainName = this.MainName,
                UniqueName = this.UniqueName,
                MainTable = this.MainTable,
                MainCols = this.MainCols,
                MainIndex = this.MainIndex,
                RefTable = this.RefTable,
                RefCols = this.RefCols,
                RefIndex = this.RefIndex,
                DeleteAction = this.DeleteAction,
                UpdateAction = this.UpdateAction,
                HasDeleteAction = this.HasDeleteAction,
                HasUpdateAction = this.HasUpdateAction,
                MatchType = this.MatchType
            };
        }
    }
}

