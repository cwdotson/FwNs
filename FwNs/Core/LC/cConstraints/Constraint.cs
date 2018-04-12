namespace FwNs.Core.LC.cConstraints
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Text;

    public sealed class Constraint : ISchemaObject
    {
        public ConstraintCore Core;
        private QNameManager.QName _name;
        public int ConstType;
        public bool IsForward;
        public Expression Check;
        private bool _isNotNull;
        public int NotNullColumnIndex;
        public RangeVariable RangeVar;
        public OrderedHashSet<string> MainColSet;
        public OrderedHashSet<string> RefColSet;
        public static Constraint[] EmptyArray = new Constraint[0];

        private Constraint()
        {
        }

        public Constraint(QNameManager.QName name, Constraint fkconstraint)
        {
            this._name = name;
            this.ConstType = 1;
            this.Core = fkconstraint.Core;
        }

        public Constraint(QNameManager.QName name, OrderedHashSet<string> mainCols, int type)
        {
            this.Core = new ConstraintCore();
            this._name = name;
            this.ConstType = type;
            this.MainColSet = mainCols;
        }

        public Constraint(QNameManager.QName name, Table t, Index index, int type)
        {
            this.Core = new ConstraintCore();
            this._name = name;
            this.ConstType = type;
            this.Core.MainTable = t;
            this.Core.MainIndex = index;
            this.Core.MainCols = index.GetColumns();
            for (int i = 0; i < this.Core.MainCols.Length; i++)
            {
                if (t.GetColumn(this.Core.MainCols[i]).GetDataType().IsLobType())
                {
                    throw Error.GetError(0x159e);
                }
            }
        }

        public Constraint(QNameManager.QName name, Table table, int[] cols, int type)
        {
            this._name = name;
            this.ConstType = type;
            ConstraintCore core1 = new ConstraintCore {
                MainTable = table,
                MainCols = cols
            };
            this.Core = core1;
        }

        public Constraint(QNameManager.QName name, QNameManager.QName refTableName, OrderedHashSet<string> refCols, QNameManager.QName mainTableName, OrderedHashSet<string> mainCols, int type, int deleteAction, int updateAction, int matchType)
        {
            this.Core = new ConstraintCore();
            this._name = name;
            this.ConstType = type;
            this.MainColSet = mainCols;
            this.Core.RefTableName = refTableName;
            this.Core.MainTableName = mainTableName;
            this.RefColSet = refCols;
            this.Core.DeleteAction = deleteAction;
            this.Core.UpdateAction = updateAction;
            this.Core.MatchType = matchType;
            switch (this.Core.DeleteAction)
            {
                case 0:
                case 2:
                case 4:
                    this.Core.HasDeleteAction = true;
                    break;
            }
            switch (this.Core.UpdateAction)
            {
                case 0:
                case 2:
                case 4:
                    this.Core.HasUpdateAction = true;
                    break;

                case 1:
                case 3:
                    break;

                default:
                    return;
            }
        }

        public void CheckCheckConstraint(Session session, Table table, object[] data)
        {
            RangeVariable.RangeIteratorBase checkIterator = session.sessionContext.GetCheckIterator(this.RangeVar);
            checkIterator.CurrentData = data;
            bool flag = false;
            checkIterator.CurrentData = null;
            if (flag.Equals(this.Check.GetValue(session)))
            {
                string[] add = new string[] { this._name.Name, table.GetName().Name };
                throw Error.GetError(null, 0x9d, 2, add);
            }
        }

        public void CheckCheckConstraint(Session session, Table table, object data)
        {
            session.sessionData.CurrentValue = data;
            bool flag = false;
            session.sessionData.CurrentValue = null;
            if (flag.Equals(this.Check.GetValue(session)))
            {
                if (table == null)
                {
                    throw Error.GetError(0x9d, this._name.Name);
                }
                string[] add = new string[] { this._name.Name, table.GetName().Name };
                throw Error.GetError(null, 0x9d, 2, add);
            }
        }

        public void CheckInsert(Session session, Table table, object[] data, bool isNew)
        {
            int constType = this.ConstType;
            if (constType == 0)
            {
                IPersistentStore rowStore = session.sessionData.GetRowStore(this.Core.MainTable);
                if (ArrayUtil.HasNull(data, this.Core.RefCols))
                {
                    if (this.Core.MatchType == 0x3b)
                    {
                        return;
                    }
                    if (this.Core.RefCols.Length == 1)
                    {
                        return;
                    }
                    if (ArrayUtil.HasAllNull(data, this.Core.RefCols))
                    {
                        return;
                    }
                }
                else if (this.Core.MainIndex.ExistsParent(session, rowStore, data, this.Core.RefCols))
                {
                    return;
                }
                throw this.GetException(data);
            }
            if ((constType == 3) && !this._isNotNull)
            {
                this.CheckCheckConstraint(session, table, data);
            }
        }

        public void CheckReferencedRows(Session session, Table table, int[] rowColArray)
        {
            IRowIterator rowIterator = table.GetRowIterator(session);
            while (true)
            {
                Row nextRow = rowIterator.GetNextRow();
                if (nextRow == null)
                {
                    break;
                }
                object[] rowData = nextRow.RowData;
                this.CheckInsert(session, table, rowData, false);
            }
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public Constraint Duplicate()
        {
            return new Constraint { 
                Core = this.Core.Duplicate(),
                _name = this._name,
                ConstType = this.ConstType,
                IsForward = this.IsForward,
                Check = this.Check,
                _isNotNull = this._isNotNull,
                NotNullColumnIndex = this.NotNullColumnIndex,
                RangeVar = this.RangeVar
            };
        }

        public IRowIterator FindFkRef(Session session, object[] row)
        {
            if ((row == null) || ArrayUtil.HasNull(row, this.Core.MainCols))
            {
                return this.Core.RefIndex.GetEmptyIterator();
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(this.Core.RefTable);
            return this.Core.RefIndex.FindFirstRow(session, rowStore, row, this.Core.MainCols);
        }

        private static string GetActionString(int action)
        {
            switch (action)
            {
                case 0:
                    return "CASCADE";

                case 1:
                    return "RESTRICT";

                case 2:
                    return "SET NULL";

                case 4:
                    return "SET DEFAULT";
            }
            return "NO ACTION";
        }

        public QNameManager.QName GetCatalogName()
        {
            return this._name.schema.schema;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        public OrderedHashSet<Expression> GetCheckColumnExpressions()
        {
            OrderedHashSet<Expression> set = new OrderedHashSet<Expression>();
            this.Check.CollectAllExpressions(set, Expression.ColumnExpressionSet, Expression.EmptyExpressionSet);
            return set;
        }

        public Expression GetCheckExpression()
        {
            return this.Check;
        }

        public string GetCheckSql()
        {
            return this.Check.GetSql();
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public int GetConstraintType()
        {
            return this.ConstType;
        }

        public int GetDeleteAction()
        {
            return this.Core.DeleteAction;
        }

        public string GetDeleteActionString()
        {
            return GetActionString(this.Core.DeleteAction);
        }

        public CoreException GetException(object[] data)
        {
            switch (this.ConstType)
            {
                case 0:
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < this.Core.RefCols.Length; i++)
                    {
                        object a = data[this.Core.RefCols[i]];
                        builder.Append(this.Core.RefTable.GetColumnTypes()[this.Core.RefCols[i]].ConvertToString(a));
                        builder.Append(',');
                    }
                    string[] add = new string[] { this.GetName().StatementName, this.Core.RefTable.GetName().StatementName, builder.ToString() };
                    return Error.GetError(null, 0xb1, 2, add);
                }
                case 2:
                case 4:
                {
                    StringBuilder builder2 = new StringBuilder();
                    for (int i = 0; i < this.Core.MainCols.Length; i++)
                    {
                        object a = data[this.Core.MainCols[i]];
                        builder2.Append(this.Core.MainTable.ColTypes[this.Core.MainCols[i]].ConvertToString(a));
                        builder2.Append(',');
                    }
                    string[] add = new string[] { this.GetName().StatementName, this.Core.MainTable.GetName().StatementName, builder2.ToString() };
                    return Error.GetError(null, 0x68, 2, add);
                }
                case 3:
                {
                    string[] add = new string[] { this._name.Name };
                    return Error.GetError(null, 0x9d, 2, add);
                }
            }
            throw Error.RuntimeError(0xc9, "Constraint");
        }

        private void GetFkStatement(StringBuilder sb)
        {
            if (!this.GetName().IsReservedName())
            {
                sb.Append("CONSTRAINT").Append(' ');
                sb.Append(this.GetName().StatementName);
                sb.Append(' ');
            }
            sb.Append("FOREIGN").Append(' ').Append("KEY");
            int[] refColumns = this.GetRefColumns();
            sb.Append(this.GetRef().GetColumnListSql(refColumns, refColumns.Length));
            sb.Append(' ').Append("REFERENCES").Append(' ');
            sb.Append(this.GetMain().GetName().GetSchemaQualifiedStatementName());
            refColumns = this.GetMainColumns();
            sb.Append(this.GetMain().GetColumnListSql(refColumns, refColumns.Length));
            if (this.GetDeleteAction() != 3)
            {
                sb.Append(' ').Append("ON").Append(' ').Append("DELETE").Append(' ');
                sb.Append(this.GetDeleteActionString());
            }
            if (this.GetUpdateAction() != 3)
            {
                sb.Append(' ').Append("ON").Append(' ').Append("UPDATE").Append(' ');
                sb.Append(this.GetUpdateActionString());
            }
        }

        public Table GetMain()
        {
            return this.Core.MainTable;
        }

        public int[] GetMainColumns()
        {
            return this.Core.MainCols;
        }

        public Index GetMainIndex()
        {
            return this.Core.MainIndex;
        }

        public QNameManager.QName GetMainName()
        {
            return this.Core.MainName;
        }

        public QNameManager.QName GetMainTableName()
        {
            return this.Core.MainTableName;
        }

        public QNameManager.QName GetName()
        {
            return this._name;
        }

        public Grantee GetOwner()
        {
            return this._name.schema.Owner;
        }

        public Table GetRef()
        {
            return this.Core.RefTable;
        }

        public int[] GetRefColumns()
        {
            return this.Core.RefCols;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            switch (this.ConstType)
            {
                case 0:
                {
                    OrderedHashSet<QNameManager.QName> set1 = new OrderedHashSet<QNameManager.QName>();
                    set1.Add(this.Core.UniqueName);
                    return set1;
                }
                case 3:
                {
                    OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
                    this.Check.CollectObjectNames(set);
                    for (int i = set.Size() - 1; i >= 0; i--)
                    {
                        QNameManager.QName name = set.Get(i);
                        if ((name.type == 9) || (name.type == 3))
                        {
                            set.Remove(i);
                        }
                    }
                    return set;
                }
            }
            return new OrderedHashSet<QNameManager.QName>();
        }

        public Index GetRefIndex()
        {
            return this.Core.RefIndex;
        }

        public QNameManager.QName GetRefName()
        {
            return this.Core.RefName;
        }

        public QNameManager.QName GetSchemaName()
        {
            return this._name.schema;
        }

        public int GetSchemaObjectType()
        {
            return 5;
        }

        public string GetSql()
        {
            StringBuilder sb = new StringBuilder();
            switch (this.GetConstraintType())
            {
                case 0:
                    if (!this.IsForward)
                    {
                        this.GetFkStatement(sb);
                        break;
                    }
                    sb.Append("ALTER").Append(' ').Append("TABLE").Append(' ');
                    sb.Append(this.GetRef().GetName().GetSchemaQualifiedStatementName());
                    sb.Append(' ').Append("ADD").Append(' ');
                    this.GetFkStatement(sb);
                    break;

                case 2:
                {
                    if (!this.GetName().IsReservedName())
                    {
                        sb.Append("CONSTRAINT").Append(' ');
                        sb.Append(this.GetName().StatementName);
                        sb.Append(' ');
                    }
                    sb.Append("UNIQUE");
                    int[] mainColumns = this.GetMainColumns();
                    sb.Append(this.GetMain().GetColumnListSql(mainColumns, mainColumns.Length));
                    break;
                }
                case 3:
                    if (!this.IsNotNull())
                    {
                        if (!this.GetName().IsReservedName())
                        {
                            sb.Append("CONSTRAINT").Append(' ');
                            sb.Append(this.GetName().StatementName).Append(' ');
                        }
                        sb.Append("CHECK").Append('(');
                        sb.Append(this.Check.GetSql());
                        sb.Append(')');
                    }
                    break;

                case 4:
                    if ((this.GetMainColumns().Length > 1) || ((this.GetMainColumns().Length == 1) && !this.GetName().IsReservedName()))
                    {
                        if (!this.GetName().IsReservedName())
                        {
                            sb.Append("CONSTRAINT").Append(' ');
                            sb.Append(this.GetName().StatementName).Append(' ');
                        }
                        sb.Append("PRIMARY").Append(' ').Append("KEY");
                        sb.Append(this.GetMain().GetColumnListSql(this.GetMainColumns(), this.GetMainColumns().Length));
                    }
                    break;
            }
            return sb.ToString();
        }

        public QNameManager.QName GetUniqueName()
        {
            return this.Core.UniqueName;
        }

        public int GetUpdateAction()
        {
            return this.Core.UpdateAction;
        }

        public string GetUpdateActionString()
        {
            return GetActionString(this.Core.UpdateAction);
        }

        public bool HasColumn(int colIndex)
        {
            switch (this.ConstType)
            {
                case 0:
                    return (ArrayUtil.Find(this.Core.RefCols, colIndex) != -1);

                case 1:
                case 2:
                case 4:
                    return (ArrayUtil.Find(this.Core.MainCols, colIndex) != -1);

                case 3:
                    return this.RangeVar.UsedColumns[colIndex];
            }
            throw Error.RuntimeError(0xc9, "Constraint");
        }

        public bool HasColumnOnly(int colIndex)
        {
            switch (this.ConstType)
            {
                case 0:
                    return (((this.Core.RefCols.Length == 1) && (this.Core.RefCols[0] == colIndex)) && (this.Core.MainTable == this.Core.RefTable));

                case 1:
                    return (((this.Core.MainCols.Length == 1) && (this.Core.MainCols[0] == colIndex)) && (this.Core.MainTable == this.Core.RefTable));

                case 2:
                case 4:
                    return ((this.Core.MainCols.Length == 1) && (this.Core.MainCols[0] == colIndex));

                case 3:
                    return (this.RangeVar.UsedColumns[colIndex] && (ArrayUtil.CountTrueElements(this.RangeVar.UsedColumns) == 1));
            }
            throw Error.RuntimeError(0xc9, "Constraint");
        }

        public bool HasColumnPlus(int colIndex)
        {
            switch (this.ConstType)
            {
                case 0:
                    return ((ArrayUtil.Find(this.Core.RefCols, colIndex) != -1) && ((this.Core.MainCols.Length != 1) || (this.Core.MainTable != this.Core.RefTable)));

                case 1:
                    return ((ArrayUtil.Find(this.Core.MainCols, colIndex) != -1) && ((this.Core.MainCols.Length != 1) || (this.Core.MainTable != this.Core.RefTable)));

                case 2:
                case 4:
                    return ((this.Core.MainCols.Length != 1) && (ArrayUtil.Find(this.Core.MainCols, colIndex) != -1));

                case 3:
                    return (this.RangeVar.UsedColumns[colIndex] && (ArrayUtil.CountTrueElements(this.RangeVar.UsedColumns) > 1));
            }
            throw Error.RuntimeError(0xc9, "Constraint");
        }

        public bool HasTriggeredAction()
        {
            if (this.ConstType == 0)
            {
                switch (this.Core.DeleteAction)
                {
                    case 0:
                    case 2:
                    case 4:
                        return true;
                }
                switch (this.Core.UpdateAction)
                {
                    case 0:
                    case 2:
                    case 4:
                        return true;
                }
            }
            return false;
        }

        public bool IsEquivalent(Table mainTable, int[] mainCols, Table refTable, int[] refCols)
        {
            if (this.ConstType <= 1)
            {
                if ((mainTable != this.Core.MainTable) || (refTable != this.Core.RefTable))
                {
                    return false;
                }
                if ((this.Core.MainCols.Length == mainCols.Length) && (this.Core.RefCols.Length == refCols.Length))
                {
                    return (ArrayUtil.AreEqualSets(this.Core.MainCols, mainCols) && ArrayUtil.AreEqualSets(this.Core.RefCols, refCols));
                }
            }
            return false;
        }

        public bool IsForeignKeyOrMain()
        {
            if (this.ConstType != 0)
            {
                return (this.ConstType == 1);
            }
            return true;
        }

        public bool IsNotNull()
        {
            return this._isNotNull;
        }

        public bool IsUniqueWithColumns(int[] cols)
        {
            int constType = this.ConstType;
            return ((((constType == 2) || (constType == 4)) && (this.Core.MainCols.Length == cols.Length)) && ArrayUtil.HaveEqualSets(this.Core.MainCols, cols, cols.Length));
        }

        public void PrepareCheckConstraint(Session session, Table table, bool checkValues)
        {
            this.Check.CheckValidCheckConstraint();
            if (table == null)
            {
                this.Check.ResolveTypes(session, null);
            }
            else
            {
                QuerySpecification specification = Expression.GetCheckSelect(session, table, this.Check);
                if (specification.GetResult(session, 1).GetNavigator().GetSize() != 0)
                {
                    string[] add = new string[] { table.GetName().Name, "" };
                    throw Error.GetError(null, 0x9d, 2, add);
                }
                this.RangeVar = specification.RangeVariables[0];
                this.RangeVar.SetForCheckConstraint();
            }
            if (((this.Check.GetExprType() == 0x30) && (this.Check.GetLeftNode().GetExprType() == 0x2f)) && (this.Check.GetLeftNode().GetLeftNode().GetExprType() == 2))
            {
                this.NotNullColumnIndex = this.Check.GetLeftNode().GetLeftNode().GetColumnIndex();
                this._isNotNull = true;
            }
        }

        private void Recompile(Session session, Table newTable)
        {
            using (Scanner scanner = new Scanner(this.Check.GetSql()))
            {
                ParserDQL rdql1 = new ParserDQL(session, scanner);
                rdql1.compileContext.Reset(0);
                rdql1.Read();
                rdql1.IsCheckOrTriggerCondition = true;
                Expression expression = rdql1.XreadBooleanValueExpression();
                this.Check = expression;
                QuerySpecification specification = Expression.GetCheckSelect(session, newTable, this.Check);
                this.RangeVar = specification.RangeVariables[0];
                this.RangeVar.SetForCheckConstraint();
            }
        }

        public void SetColumnsIndexes(Table table)
        {
            if (this.ConstType == 0)
            {
                if (this.MainColSet == null)
                {
                    this.Core.MainCols = this.Core.MainTable.GetPrimaryKey();
                    if (this.Core.MainCols == null)
                    {
                        throw Error.GetError(0x15cd);
                    }
                }
                else if (this.Core.MainCols == null)
                {
                    this.Core.MainCols = this.Core.MainTable.GetColumnIndexes(this.MainColSet);
                }
                if (this.Core.RefCols == null)
                {
                    this.Core.RefCols = table.GetColumnIndexes(this.RefColSet);
                }
                for (int i = 0; i < this.Core.RefCols.Length; i++)
                {
                    if (table.GetColumn(this.Core.RefCols[i]).GetDataType().IsLobType())
                    {
                        throw Error.GetError(0x159e);
                    }
                }
            }
            else if (this.MainColSet != null)
            {
                this.Core.MainCols = table.GetColumnIndexes(this.MainColSet);
                for (int i = 0; i < this.Core.MainCols.Length; i++)
                {
                    if (table.GetColumn(this.Core.MainCols[i]).GetDataType().IsLobType())
                    {
                        throw Error.GetError(0x159e);
                    }
                }
            }
        }

        public void UpdateTable(Session session, Table oldTable, Table newTable, int colIndex, int adjust)
        {
            if (oldTable == this.Core.MainTable)
            {
                this.Core.MainTable = newTable;
                if (this.Core.MainIndex != null)
                {
                    this.Core.MainIndex = this.Core.MainTable.GetIndex(this.Core.MainIndex.GetName().Name);
                    this.Core.MainCols = ArrayUtil.ToAdjustedColumnArray(this.Core.MainCols, colIndex, adjust);
                    this.Core.MainIndex.SetTable(newTable);
                }
            }
            if (oldTable == this.Core.RefTable)
            {
                this.Core.RefTable = newTable;
                if (this.Core.RefIndex != null)
                {
                    this.Core.RefIndex = this.Core.RefTable.GetIndex(this.Core.RefIndex.GetName().Name);
                    this.Core.RefCols = ArrayUtil.ToAdjustedColumnArray(this.Core.RefCols, colIndex, adjust);
                    this.Core.RefIndex.SetTable(newTable);
                }
            }
            if (this.ConstType == 3)
            {
                this.Recompile(session, newTable);
            }
        }
    }
}

