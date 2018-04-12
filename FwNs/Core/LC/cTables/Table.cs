namespace FwNs.Core.LC.cTables
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cRows;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Table : TableBase, ISchemaObject
    {
        public static readonly Table[] EmptyArray = new Table[0];
        public QNameManager.QName TableName;
        protected long ChangeTimestamp;
        public HashMappedList<string, ColumnSchema> ColumnList;
        public int IdentityColumn;
        public NumberSequence IdentitySequence;
        public Constraint[] ConstraintList;
        public Constraint[] FkConstraints;
        public Constraint[] FkMainConstraints;
        public Constraint[] CheckConstraints;
        public TriggerDef[] TriggerList;
        public TriggerDef[][] TriggerLists;
        public Expression[] ColDefaults;
        private bool _hasDefaultValues;
        public bool[] ColGenerated;
        private bool _hasGeneratedValues;
        private bool[] _colRefFk;
        private bool[] _colMainFk;
        private bool _hasDomainColumns;
        private bool _hasNotNullColumns;
        protected int[] DefaultColumnMap;
        public RangeVariable[] DefaultRanges;

        public Table(Table table, QNameManager.QName name)
        {
            base.PersistenceScope = 0x15;
            name.schema = SqlInvariants.SystemSchemaQname;
            this.TableName = name;
            base.database = table.database;
            base.TableType = 9;
            this.ColumnList = table.ColumnList;
            base.ColumnCount = table.ColumnCount;
            base.IndexList = IndexAVL.EmptyArray;
            this.ConstraintList = Constraint.EmptyArray;
        }

        public Table(Database database, QNameManager.QName name, int type)
        {
            base.database = database;
            this.TableName = name;
            base.PersistenceId = database.persistentStoreCollection.GetNextId();
            switch (type)
            {
                case 1:
                    base.PersistenceScope = 0x18;
                    base._isSchemaBased = true;
                    goto Label_0152;

                case 2:
                    base.PersistenceScope = 0x15;
                    base.isSessionBased = true;
                    goto Label_0152;

                case 3:
                    base.PersistenceScope = 0x16;
                    base._isTemp = true;
                    base._isSchemaBased = true;
                    base.isSessionBased = true;
                    goto Label_0152;

                case 4:
                    break;

                case 5:
                    if (!DatabaseUrl.IsFileBasedDatabaseType(database.GetDatabaseType()))
                    {
                        type = 4;
                        break;
                    }
                    base.PersistenceScope = 0x18;
                    base._isSchemaBased = true;
                    base._isCached = true;
                    base.isLogged = !database.IsFilesReadOnly();
                    goto Label_0152;

                case 8:
                    base.PersistenceScope = 0x15;
                    base._isSchemaBased = true;
                    base.isSessionBased = true;
                    base._isView = true;
                    goto Label_0152;

                case 9:
                    base.PersistenceScope = 0x17;
                    base.isSessionBased = true;
                    goto Label_0152;

                case 11:
                    base.PersistenceScope = 0x15;
                    base.isSessionBased = true;
                    goto Label_0152;

                default:
                    throw Error.RuntimeError(0xc9, "Table");
            }
            base.PersistenceScope = 0x18;
            base._isSchemaBased = true;
            base.isLogged = !database.IsFilesReadOnly();
        Label_0152:
            base.TableType = type;
            base.PrimaryKeyCols = null;
            base.PrimaryKeyTypes = null;
            this.IdentityColumn = -1;
            this.ColumnList = new HashMappedList<string, ColumnSchema>();
            base.IndexList = IndexAVL.EmptyArray;
            this.ConstraintList = Constraint.EmptyArray;
            this.FkConstraints = Constraint.EmptyArray;
            this.FkMainConstraints = Constraint.EmptyArray;
            this.CheckConstraints = Constraint.EmptyArray;
            this.TriggerList = TriggerDef.EmptyArray;
            this.TriggerLists = new TriggerDef[9][];
            for (int i = 0; i < 9; i++)
            {
                this.TriggerLists[i] = TriggerDef.EmptyArray;
            }
            if (database.IsFilesReadOnly() && this.IsFileBased())
            {
                base._isReadOnly = true;
            }
            if (base._isSchemaBased && !base._isTemp)
            {
                this.CreateDefaultStore();
            }
        }

        public void AddColumn(ColumnSchema column)
        {
            string name = column.GetName().Name;
            if (this.FindColumn(name) >= 0)
            {
                throw Error.GetError(0x1580, name);
            }
            if (column.IsIdentity())
            {
                if (this.IdentityColumn != -1)
                {
                    throw Error.GetError(0x1595, name);
                }
                this.IdentityColumn = base.ColumnCount;
                this.IdentitySequence = column.GetIdentitySequence();
            }
            this.AddColumnNoCheck(column);
        }

        public void AddColumnNoCheck(ColumnSchema column)
        {
            if (base.PrimaryKeyCols != null)
            {
                throw Error.RuntimeError(0xc9, "Table");
            }
            this.ColumnList.Add(column.GetName().Name, column);
            base.ColumnCount++;
            if (column.GetDataType().IsLobType())
            {
                base.hasLobColumn = true;
            }
        }

        public void AddConstraint(Constraint c)
        {
            int colindex = (c.GetConstraintType() == 4) ? 0 : this.ConstraintList.Length;
            this.ConstraintList = ArrayUtil.ToAdjustedArray<Constraint>(this.ConstraintList, c, colindex, 1);
            this.UpdateConstraintLists();
        }

        public virtual void AddTrigger(TriggerDef td, QNameManager.QName otherName)
        {
            int length = this.TriggerList.Length;
            if (otherName != null)
            {
                int triggerIndex = this.GetTriggerIndex(otherName.Name);
                if (triggerIndex != -1)
                {
                    length = triggerIndex + 1;
                }
            }
            this.TriggerList = ArrayUtil.ToAdjustedArray<TriggerDef>(this.TriggerList, td, length, 1);
            TriggerDef[] source = this.TriggerLists[td.TriggerType];
            length = source.Length;
            if (otherName != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i].GetName().Name.Equals(otherName.Name))
                    {
                        length = i + 1;
                        break;
                    }
                }
            }
            this.TriggerLists[td.TriggerType] = ArrayUtil.ToAdjustedArray<TriggerDef>(source, td, length, 1);
        }

        public bool AreColumnsNotNull(int[] indexes)
        {
            return ArrayUtil.AreAllIntIndexesInBooleanArray(indexes, base.ColNotNull);
        }

        public void CheckColumnInCheckConstraint(int colIndex)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint constraint = this.ConstraintList[index];
                if (((constraint.ConstType == 3) && !constraint.IsNotNull()) && constraint.HasColumn(colIndex))
                {
                    QNameManager.QName name = constraint.GetName();
                    throw Error.GetError(0x157e, name.GetSchemaQualifiedStatementName());
                }
                index++;
            }
        }

        public void CheckColumnInFkConstraint(int colIndex)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint constraint = this.ConstraintList[index];
                if (constraint.HasColumn(colIndex) && ((constraint.GetConstraintType() == 1) || (constraint.GetConstraintType() == 0)))
                {
                    QNameManager.QName name = constraint.GetName();
                    throw Error.GetError(0x159d, name.GetSchemaQualifiedStatementName());
                }
                index++;
            }
        }

        public void CheckColumnInFkConstraint(int colIndex, int actionType)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint constraint = this.ConstraintList[index];
                if (((constraint.GetConstraintType() == 0) && constraint.HasColumn(colIndex)) && ((actionType == constraint.GetUpdateAction()) || (actionType == constraint.GetDeleteAction())))
                {
                    QNameManager.QName name = constraint.GetName();
                    throw Error.GetError(0x159d, name.GetSchemaQualifiedStatementName());
                }
                index++;
            }
        }

        public void CheckColumnsMatch(int[] col, Table other, int[] othercol)
        {
            for (int i = 0; i < col.Length; i++)
            {
                SqlType type = other.ColTypes[othercol[i]];
                if (base.ColTypes[col[i]].TypeComparisonGroup != type.TypeComparisonGroup)
                {
                    throw Error.GetError(0x15ba);
                }
            }
        }

        public virtual void CheckDataReadOnly()
        {
            if (base._isReadOnly)
            {
                throw Error.GetError(0x1c8);
            }
        }

        public override void ClearAllData(Session session)
        {
            base.ClearAllData(session);
            if (this.IdentitySequence != null)
            {
                this.IdentitySequence.Reset();
            }
        }

        public override void ClearAllData(IPersistentStore store)
        {
            base.ClearAllData(store);
            if (this.IdentitySequence != null)
            {
                this.IdentitySequence.Reset();
            }
        }

        public void CollectFkReadLocks(int[] columnMap, OrderedHashSet<QNameManager.QName> set)
        {
            for (int i = 0; i < this.FkMainConstraints.Length; i++)
            {
                Constraint constraint = this.FkMainConstraints[i];
                Table table = constraint.GetRef();
                int[] mainColumns = constraint.GetMainColumns();
                if (table != this)
                {
                    if (columnMap == null)
                    {
                        if (constraint.Core.HasDeleteAction)
                        {
                            int[] numArray2 = (constraint.Core.DeleteAction == 0) ? null : constraint.GetRefColumns();
                            if (set.Add(table.GetName()))
                            {
                                table.CollectFkReadLocks(numArray2, set);
                            }
                        }
                    }
                    else if (ArrayUtil.HaveCommonElement(columnMap, mainColumns) && set.Add(table.GetName()))
                    {
                        table.CollectFkReadLocks(constraint.GetRefColumns(), set);
                    }
                }
            }
        }

        public void CollectFkWriteLocks(int[] columnMap, OrderedHashSet<QNameManager.QName> set)
        {
            for (int i = 0; i < this.FkMainConstraints.Length; i++)
            {
                Constraint constraint = this.FkMainConstraints[i];
                Table table = constraint.GetRef();
                int[] mainColumns = constraint.GetMainColumns();
                if (table != this)
                {
                    if (columnMap == null)
                    {
                        if (constraint.Core.HasDeleteAction)
                        {
                            int[] numArray2 = (constraint.Core.DeleteAction == 0) ? null : constraint.GetRefColumns();
                            if (set.Add(table.GetName()))
                            {
                                table.CollectFkWriteLocks(numArray2, set);
                            }
                        }
                    }
                    else if ((ArrayUtil.HaveCommonElement(columnMap, mainColumns) && constraint.Core.HasUpdateAction) && set.Add(table.GetName()))
                    {
                        table.CollectFkWriteLocks(constraint.GetRefColumns(), set);
                    }
                }
            }
        }

        public static int CompareRows(Session session, object[] a, object[] b, int[] cols, SqlType[] coltypes)
        {
            int length = cols.Length;
            for (int i = 0; i < length; i++)
            {
                int index = cols[i];
                SqlType otherType = coltypes[index];
                int num4 = otherType.Compare(session, a[index], b[index], otherType, false);
                if (num4 != 0)
                {
                    return num4;
                }
            }
            return 0;
        }

        public virtual void Compile(Session session, ISchemaObject parentObject)
        {
            for (int i = 0; i < base.ColumnCount; i++)
            {
                this.GetColumn(i).Compile(session, this);
            }
        }

        public void CreateDefaultStore()
        {
            base.Store = base.database.logger.NewStore(null, base.database.persistentStoreCollection, this, true);
        }

        public Index CreateIndexForColumns(Session session, int[] columns)
        {
            Index index;
            QNameManager.QName name = base.database.NameManager.NewAutoName("IDX_T", this.GetSchemaName(), this.GetName(), 20);
            try
            {
                index = base.CreateAndAddIndexStructure(session, name, columns, null, null, false, false, false);
            }
            catch (Exception)
            {
                return null;
            }
            switch (base.TableType)
            {
                case 1:
                    session.sessionData.persistentStoreCollection.RegisterIndex(this);
                    return index;

                case 2:
                    return index;

                case 3:
                {
                    Session[] allSessions = base.database.sessionManager.GetAllSessions();
                    for (int i = 0; i < allSessions.Length; i++)
                    {
                        allSessions[i].sessionData.persistentStoreCollection.RegisterIndex(this);
                    }
                    return index;
                }
            }
            return index;
        }

        public void CreatePrimaryKey()
        {
            this.CreatePrimaryKey(null, null, false);
        }

        public void CreatePrimaryKey(int[] cols)
        {
            this.CreatePrimaryKey(null, cols, false);
        }

        public void CreatePrimaryKey(QNameManager.QName indexName, int[] columns, bool columnsNotNull)
        {
            if (base.PrimaryKeyCols != null)
            {
                throw Error.RuntimeError(0xc9, "Table");
            }
            if (columns == null)
            {
                columns = new int[0];
            }
            else
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    this.GetColumn(columns[i]).SetPrimaryKey(true);
                }
            }
            base.PrimaryKeyCols = columns;
            this.SetColumnStructures();
            base.PrimaryKeyTypes = new SqlType[base.PrimaryKeyCols.Length];
            ArrayUtil.ProjectRow(base.ColTypes, base.PrimaryKeyCols, base.PrimaryKeyTypes);
            base.PrimaryKeyColsSequence = new int[base.PrimaryKeyCols.Length];
            ArrayUtil.FillSequence(base.PrimaryKeyColsSequence);
            QNameManager.QName name = indexName ?? base.database.NameManager.NewAutoName("IDX", this.GetSchemaName(), this.GetName(), 20);
            base.CreatePrimaryIndex(base.PrimaryKeyCols, base.PrimaryKeyTypes, name);
            base.SetBestRowIdentifiers();
        }

        public void CreatePrimaryKeyConstraint(QNameManager.QName indexName, int[] columns, bool columnsNotNull)
        {
            this.CreatePrimaryKey(indexName, columns, columnsNotNull);
            Constraint c = new Constraint(indexName, this, base.GetPrimaryIndex(), 4);
            this.AddConstraint(c);
        }

        public void DeleteNoCheckFromLog(Session session, object[] data)
        {
            Row nextRow;
            IPersistentStore rowStore = session.sessionData.GetRowStore(this);
            if (base.HasPrimaryKey())
            {
                IRowIterator iterator1 = base.GetPrimaryIndex().FindFirstRow(session, rowStore, data, base.PrimaryKeyColsSequence);
                nextRow = iterator1.GetNextRow();
                iterator1.Release();
            }
            else if (base.BestIndex == null)
            {
                IRowIterator rowIterator = base.GetRowIterator(session);
                do
                {
                    nextRow = rowIterator.GetNextRow();
                }
                while ((nextRow != null) && (CompareRows(session, nextRow.RowData, data, this.DefaultColumnMap, base.ColTypes) != 0));
                rowIterator.Release();
            }
            else
            {
                object[] rowData;
                IRowIterator iterator2 = base.BestIndex.FindFirstRow(session, rowStore, data);
                do
                {
                    nextRow = iterator2.GetNextRow();
                    if (nextRow == null)
                    {
                        break;
                    }
                    rowData = nextRow.RowData;
                    if (base.BestIndex.CompareRowNonUnique(session, rowData, data, base.BestIndex.GetColumns()) != 0)
                    {
                        nextRow = null;
                        break;
                    }
                }
                while (CompareRows(session, rowData, data, this.DefaultColumnMap, base.ColTypes) != 0);
                iterator2.Release();
            }
            if (nextRow != null)
            {
                session.AddDeleteAction(this, nextRow, null);
            }
        }

        public void EnforceRowConstraints(Session session, object[] data)
        {
            for (int i = 0; i < base.ColumnCount; i++)
            {
                SqlType type = base.ColTypes[i];
                if (this._hasDomainColumns && type.IsDomainType())
                {
                    Constraint[] constraints = type.userTypeModifier.GetConstraints();
                    for (int j = 0; j < constraints.Length; j++)
                    {
                        constraints[j].CheckCheckConstraint(session, this, data[i]);
                    }
                }
                if (base.ColNotNull[i] && (data[i] == null))
                {
                    Constraint constraint = this.GetNotNullConstraintForColumn(i) ?? this.GetPrimaryConstraint();
                    string[] add = new string[] { constraint.GetName().Name, this.TableName.Name };
                    throw Error.GetError(null, 10, 2, add);
                }
            }
        }

        public void EnforceTypeLimits(Session session, object[] data)
        {
            for (int i = 0; i < base.ColumnCount; i++)
            {
                data[i] = base.ColTypes[i].ConvertToTypeLimits(session, data[i]);
            }
        }

        public int FindColumn(string name)
        {
            return this.ColumnList.GetIndex(name);
        }

        public void FireTriggers(Session session, int trigVecIndex, RowSetNavigator rowSet)
        {
            if (base.database.IsReferentialIntegrity())
            {
                TriggerDef[] defArray = this.TriggerLists[trigVecIndex];
                int index = 0;
                int length = defArray.Length;
                while (index < length)
                {
                    TriggerDef def1 = defArray[index];
                    def1.HasOldTable();
                    def1.PushPair(session, null, null);
                    index++;
                }
            }
        }

        public void FireTriggers(Session session, int trigVecIndex, object[] oldData, object[] newData, int[] cols)
        {
            if (base.database.IsReferentialIntegrity())
            {
                TriggerDef[] defArray = this.TriggerLists[trigVecIndex];
                int index = 0;
                int length = defArray.Length;
                while (index < length)
                {
                    TriggerDef def = defArray[index];
                    bool flag = def is TriggerDefSQL;
                    if (((cols == null) || (def.GetUpdateColumnIndexes() == null)) || ArrayUtil.HaveCommonElement(def.GetUpdateColumnIndexes(), cols))
                    {
                        if (def.IsForEachRow())
                        {
                            switch (def.TriggerType)
                            {
                                case 3:
                                    if (!flag)
                                    {
                                        newData = (object[]) newData.Clone();
                                    }
                                    break;

                                case 4:
                                case 7:
                                case 8:
                                    if (!flag)
                                    {
                                        oldData = (object[]) oldData.Clone();
                                    }
                                    break;

                                case 5:
                                    if (!flag)
                                    {
                                        oldData = (object[]) oldData.Clone();
                                        newData = (object[]) newData.Clone();
                                    }
                                    break;
                            }
                            def.PushPair(session, oldData, newData);
                        }
                        else
                        {
                            def.PushPair(session, null, null);
                        }
                    }
                    index++;
                }
            }
        }

        public virtual Table GetBaseTable()
        {
            return this;
        }

        public virtual int[] GetBaseTableColumnMap()
        {
            return this.DefaultColumnMap;
        }

        public int[] GetBestRowIdentifiers()
        {
            return base.BestRowIdentifierCols;
        }

        public void GetBodySql(StringBuilder sb)
        {
            int[] primaryKey = base.GetPrimaryKey();
            Constraint primaryConstraint = this.GetPrimaryConstraint();
            for (int i = 0; i < base.ColumnCount; i++)
            {
                ColumnSchema column = this.GetColumn(i);
                string statementName = column.GetName().StatementName;
                SqlType dataType = column.GetDataType();
                if (i > 0)
                {
                    sb.Append(',');
                }
                sb.Append(statementName);
                sb.Append(' ');
                sb.Append(dataType.GetTypeDefinition());
                string defaultSql = column.GetDefaultSql();
                if (defaultSql != null)
                {
                    sb.Append(' ').Append("DEFAULT").Append(' ');
                    sb.Append(defaultSql);
                }
                if (column.IsIdentity())
                {
                    sb.Append(' ').Append(column.GetIdentitySequence().GetSql());
                }
                if (column.IsGenerated())
                {
                    sb.Append(' ').Append("GENERATED").Append(' ');
                    sb.Append("ALWAYS").Append(' ').Append("AS").Append("(");
                    sb.Append(column.GetGeneratingExpression().GetSql());
                    sb.Append(")");
                }
                if (!column.IsNullable())
                {
                    Constraint notNullConstraintForColumn = this.GetNotNullConstraintForColumn(i);
                    if ((notNullConstraintForColumn != null) && !notNullConstraintForColumn.GetName().IsReservedName())
                    {
                        sb.Append(' ').Append("CONSTRAINT").Append(' ').Append(notNullConstraintForColumn.GetName().StatementName);
                    }
                    sb.Append(' ').Append("NOT").Append(' ').Append("NULL");
                }
                if (((primaryKey.Length == 1) && (i == primaryKey[0])) && primaryConstraint.GetName().IsReservedName())
                {
                    sb.Append(' ').Append("PRIMARY").Append(' ').Append("KEY");
                }
            }
            Constraint[] constraints = this.GetConstraints();
            int index = 0;
            int length = constraints.Length;
            while (index < length)
            {
                Constraint constraint3 = constraints[index];
                if (!constraint3.IsForward)
                {
                    string sql = constraint3.GetSql();
                    if (sql.Length > 0)
                    {
                        sb.Append(',');
                        sb.Append(sql);
                    }
                }
                index++;
            }
        }

        public virtual QNameManager.QName GetCatalogName()
        {
            return base.database.GetCatalogName();
        }

        public virtual long GetChangeTimestamp()
        {
            return this.ChangeTimestamp;
        }

        public ColumnSchema GetColumn(int i)
        {
            return this.ColumnList.Get(i);
        }

        public bool[] GetColumnCheckList(int[] columnIndexes)
        {
            bool[] flagArray = new bool[base.ColumnCount];
            for (int i = 0; i < columnIndexes.Length; i++)
            {
                int index = columnIndexes[i];
                if (index > -1)
                {
                    flagArray[index] = true;
                }
            }
            return flagArray;
        }

        public int GetColumnIndex(string name)
        {
            int num1 = this.FindColumn(name);
            if (num1 == -1)
            {
                throw Error.GetError(0x157d, name);
            }
            return num1;
        }

        public int[] GetColumnIndexes(OrderedHashSet<string> set)
        {
            int[] numArray = new int[set.Size()];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = this.GetColumnIndex(set.Get(i));
            }
            return numArray;
        }

        public string GetColumnListSql(int[] col, int len)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            for (int i = 0; i < len; i++)
            {
                builder.Append(this.GetColumn(col[i]).GetName().StatementName);
                if (i < (len - 1))
                {
                    builder.Append(',');
                }
            }
            builder.Append(')');
            return builder.ToString();
        }

        public string GetColumnListWithTypeSql()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            for (int i = 0; i < base.ColumnCount; i++)
            {
                ColumnSchema column = this.GetColumn(i);
                string statementName = column.GetName().StatementName;
                SqlType dataType = column.GetDataType();
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(statementName);
                builder.Append(' ');
                builder.Append(dataType.GetTypeDefinition());
            }
            builder.Append(')');
            return builder.ToString();
        }

        public int[] GetColumnMap()
        {
            return this.DefaultColumnMap;
        }

        public void GetColumnNames(bool[] columnCheckList, FwNs.Core.LC.cLib.ISet<QNameManager.QName> set)
        {
            for (int i = 0; i < columnCheckList.Length; i++)
            {
                if (columnCheckList[i])
                {
                    set.Add(this.ColumnList.Get(i).GetName());
                }
            }
        }

        public OrderedHashSet<string> GetColumnNameSet()
        {
            OrderedHashSet<string> set = new OrderedHashSet<string>();
            for (int i = 0; i < base.ColumnCount; i++)
            {
                set.Add(this.ColumnList.Get(i).GetName().Name);
            }
            return set;
        }

        public virtual OrderedHashSet<ISchemaObject> GetComponents()
        {
            OrderedHashSet<ISchemaObject> set = new OrderedHashSet<ISchemaObject>();
            set.AddAll(this.ConstraintList);
            set.AddAll(this.TriggerList);
            for (int i = 0; i < base.IndexList.Length; i++)
            {
                if (!base.IndexList[i].IsConstraint())
                {
                    set.Add(base.IndexList[i]);
                }
            }
            return set;
        }

        public Constraint GetConstraint(string constraintName)
        {
            int constraintIndex = this.GetConstraintIndex(constraintName);
            if (constraintIndex >= 0)
            {
                return this.ConstraintList[constraintIndex];
            }
            return null;
        }

        public int GetConstraintIndex(string constraintName)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                if (this.ConstraintList[index].GetName().Name.Equals(constraintName))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public Constraint[] GetConstraints()
        {
            return this.ConstraintList;
        }

        public OrderedHashSet<Constraint> GetContainingConstraints(int colIndex)
        {
            OrderedHashSet<Constraint> set = new OrderedHashSet<Constraint>();
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint key = this.ConstraintList[index];
                if (key.HasColumnPlus(colIndex))
                {
                    set.Add(key);
                }
                index++;
            }
            return set;
        }

        public OrderedHashSet<QNameManager.QName> GetContainingIndexNames(int colIndex)
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            int num = 0;
            int length = base.IndexList.Length;
            while (num < length)
            {
                Index index = base.IndexList[num];
                if (ArrayUtil.Find(index.GetColumns(), colIndex) != -1)
                {
                    set.Add(index.GetName());
                }
                num++;
            }
            return set;
        }

        public OrderedHashSet<Constraint> GetDependentConstraints(Constraint constraint)
        {
            OrderedHashSet<Constraint> set = new OrderedHashSet<Constraint>();
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint key = this.ConstraintList[index];
                if ((key.GetConstraintType() == 1) && (key.Core.UniqueName == constraint.GetName()))
                {
                    set.Add(key);
                }
                index++;
            }
            return set;
        }

        public OrderedHashSet<Constraint> GetDependentConstraints(int colIndex)
        {
            OrderedHashSet<Constraint> set = new OrderedHashSet<Constraint>();
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint key = this.ConstraintList[index];
                if (key.HasColumnOnly(colIndex))
                {
                    set.Add(key);
                }
                index++;
            }
            return set;
        }

        public OrderedHashSet<Constraint> GetDependentExternalConstraints()
        {
            OrderedHashSet<Constraint> set = new OrderedHashSet<Constraint>();
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint key = this.ConstraintList[index];
                if (((key.GetConstraintType() == 1) || (key.GetConstraintType() == 0)) && (key.Core.MainTable != key.Core.RefTable))
                {
                    set.Add(key);
                }
                index++;
            }
            return set;
        }

        public Constraint GetFkConstraintForColumns(Table tableMain, int[] mainCols, int[] refCols)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint constraint = this.ConstraintList[index];
                if (constraint.IsEquivalent(tableMain, mainCols, this, refCols))
                {
                    return constraint;
                }
                index++;
            }
            return null;
        }

        public Constraint[] GetFkConstraints()
        {
            return this.FkConstraints;
        }

        public Index GetFullIndexForColumns(int[] cols)
        {
            for (int i = 0; i < base.IndexList.Length; i++)
            {
                if (ArrayUtil.HaveEqualArrays(base.IndexList[i].GetColumns(), cols, cols.Length))
                {
                    return base.IndexList[i];
                }
            }
            return null;
        }

        public override int GetId()
        {
            return this.TableName.GetHashCode();
        }

        public int GetIdentityColumnIndex()
        {
            return this.IdentityColumn;
        }

        public Index GetIndex(string indexName)
        {
            Index[] indexList = base.IndexList;
            int indexIndex = this.GetIndexIndex(indexName);
            if (indexIndex != -1)
            {
                return indexList[indexIndex];
            }
            return null;
        }

        public Index GetIndexForColumn(Session session, int col)
        {
            int index = base.BestIndexForColumn[col];
            if (index > -1)
            {
                return base.IndexList[index];
            }
            int tableType = base.TableType;
            if ((((tableType - 1) > 2) && (tableType != 8)) && (tableType != 11))
            {
                return null;
            }
            int[] columns = new int[] { col };
            return this.CreateIndexForColumns(session, columns);
        }

        public Index GetIndexForColumns(Session session, OrderedIntHashSet set, bool ordered)
        {
            int num = 0;
            Index index = null;
            if (set.IsEmpty())
            {
                return null;
            }
            int i = 0;
            int length = base.IndexList.Length;
            while (i < length)
            {
                Index index3 = base.GetIndex(i);
                int[] columns = index3.GetColumns();
                int num4 = ordered ? set.GetOrderedStartMatchCount(columns) : set.GetStartMatchCount(columns);
                if (num4 != 0)
                {
                    if (num4 == columns.Length)
                    {
                        return index3;
                    }
                    if (num4 > num)
                    {
                        num = num4;
                        index = index3;
                    }
                }
                i++;
            }
            if (index != null)
            {
                return index;
            }
            int tableType = base.TableType;
            if ((((tableType - 1) > 2) && (tableType != 8)) && (tableType != 11))
            {
                return index;
            }
            return this.CreateIndexForColumns(session, set.ToArray());
        }

        public int GetIndexIndex(string indexName)
        {
            Index[] indexList = base.IndexList;
            for (int i = 0; i < indexList.Length; i++)
            {
                if (indexName.Equals(indexList[i].GetName().Name))
                {
                    return i;
                }
            }
            return -1;
        }

        public int[] GetIndexRootsArray()
        {
            IPersistentStore store = base.database.persistentStoreCollection.GetStore(this);
            int[] numArray = new int[(base.IndexList.Length * 2) + 1];
            int index = 0;
            for (int i = 0; i < base.IndexList.Length; i++)
            {
                ICachedObject accessor = store.GetAccessor(base.IndexList[i]);
                numArray[index++] = (accessor == null) ? -1 : accessor.GetPos();
            }
            for (int j = 0; j < base.IndexList.Length; j++)
            {
                numArray[index++] = base.IndexList[j].SizeUnique(store);
            }
            numArray[index] = base.IndexList[0].Size(null, store);
            return numArray;
        }

        public string GetIndexRootsSql(int[] roots)
        {
            StringBuilder builder1 = new StringBuilder(0x80);
            builder1.Append("SET").Append(' ').Append("TABLE").Append(' ');
            builder1.Append(this.GetName().GetSchemaQualifiedStatementName());
            builder1.Append(' ').Append("INDEX").Append(' ').Append('\'');
            builder1.Append(StringUtil.GetList(roots, " ", ""));
            builder1.Append('\'');
            return builder1.ToString();
        }

        public virtual QNameManager.QName GetName()
        {
            return this.TableName;
        }

        public int[] GetNewColumnMap()
        {
            return new int[base.ColumnCount];
        }

        public object[] GetNewRowData(Session session)
        {
            object[] objArray = new object[base.ColumnCount];
            if (this._hasDefaultValues)
            {
                for (int i = 0; i < base.ColumnCount; i++)
                {
                    Expression expression = this.ColDefaults[i];
                    if (expression != null)
                    {
                        objArray[i] = expression.GetValue(session, base.ColTypes[i]);
                    }
                }
            }
            return objArray;
        }

        public long GetNextIdentity()
        {
            return this.IdentitySequence.Peek();
        }

        public Constraint GetNotNullConstraintForColumn(int colIndex)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint constraint = this.ConstraintList[index];
                if (constraint.IsNotNull() && (constraint.NotNullColumnIndex == colIndex))
                {
                    return constraint;
                }
                index++;
            }
            return null;
        }

        public virtual Grantee GetOwner()
        {
            return this.TableName.schema.Owner;
        }

        public Constraint GetPrimaryConstraint()
        {
            if (base.PrimaryKeyCols.Length != 0)
            {
                return this.ConstraintList[0];
            }
            return null;
        }

        public virtual QueryExpression GetQueryExpression()
        {
            return null;
        }

        public virtual QueryExpression GetQueryExpressionRecursive()
        {
            return null;
        }

        public virtual OrderedHashSet<QNameManager.QName> GetReferences()
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            for (int i = 0; i < base.ColTypes.Length; i++)
            {
                ColumnSchema column = this.GetColumn(i);
                if (!column.GetReferences().IsEmpty())
                {
                    set.Add(column.GetName());
                }
            }
            return set;
        }

        public override IPersistentStore GetRowStore(Session session)
        {
            if (base.TableType == 1)
            {
                base.database.DbInfo.GetSystemTable(session, this.GetName().Name);
                return session.sessionData.GetRowStore(this);
            }
            return (base.Store ?? session.sessionData.GetRowStore(this));
        }

        public virtual QNameManager.QName GetSchemaName()
        {
            return this.TableName.schema;
        }

        public virtual int GetSchemaObjectType()
        {
            return 3;
        }

        public virtual string GetSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE").Append(' ');
            if (this.IsTemp())
            {
                sb.Append("GLOBAL").Append(' ');
                sb.Append("TEMPORARY").Append(' ');
            }
            else if (this.IsCached())
            {
                sb.Append("CACHED").Append(' ');
            }
            else
            {
                sb.Append("MEMORY").Append(' ');
            }
            sb.Append("TABLE").Append(' ');
            sb.Append(this.GetName().GetSchemaQualifiedStatementName());
            sb.Append('(');
            this.GetBodySql(sb);
            sb.Append(')');
            if (base.OnCommitPreserve())
            {
                sb.Append(' ').Append("ON").Append(' ');
                sb.Append("COMMIT").Append(' ').Append("PRESERVE");
                sb.Append(' ').Append("ROWS");
            }
            return sb.ToString();
        }

        public string[] GetSql(OrderedHashSet<object> resolved, OrderedHashSet<object> unresolved)
        {
            for (int i = 0; i < this.ConstraintList.Length; i++)
            {
                Constraint key = this.ConstraintList[i];
                if (key.IsForward)
                {
                    unresolved.Add(key);
                }
                else if ((key.GetConstraintType() == 2) || (key.GetConstraintType() == 4))
                {
                    resolved.Add(key.GetName());
                }
            }
            List<string> list = new List<string> {
                this.GetSql()
            };
            if (!base._isTemp && this.HasIdentityColumn())
            {
                list.Add(NumberSequence.GetRestartSql(this));
            }
            for (int j = 0; j < base.IndexList.Length; j++)
            {
                if (!base.IndexList[j].IsConstraint() && (base.IndexList[j].GetColumnCount() > 0))
                {
                    list.Add(base.IndexList[j].GetSql());
                }
            }
            return list.ToArray();
        }

        public string GetSqlForReadOnly()
        {
            if (this.IsReadOnly())
            {
                StringBuilder builder1 = new StringBuilder(0x40);
                builder1.Append("SET").Append(' ').Append("TABLE").Append(' ');
                builder1.Append(this.GetName().GetSchemaQualifiedStatementName());
                builder1.Append(' ').Append("READ").Append(' ');
                builder1.Append("ONLY");
                return builder1.ToString();
            }
            return null;
        }

        public virtual SubQuery GetSubQuery()
        {
            return null;
        }

        public TriggerDef GetTrigger(string name)
        {
            for (int i = this.TriggerList.Length - 1; i >= 0; i--)
            {
                if (this.TriggerList[i].GetName().Name.Equals(name))
                {
                    return this.TriggerList[i];
                }
            }
            return null;
        }

        public int GetTriggerIndex(string name)
        {
            for (int i = 0; i < this.TriggerList.Length; i++)
            {
                if (this.TriggerList[i].GetName().Name.Equals(name))
                {
                    return i;
                }
            }
            return -1;
        }

        public TriggerDef[] GetTriggers()
        {
            return this.TriggerList;
        }

        public string[] GetTriggerSql()
        {
            string[] strArray = new string[this.TriggerList.Length];
            for (int i = 0; i < this.TriggerList.Length; i++)
            {
                strArray[i] = this.TriggerList[i].GetSql();
            }
            return strArray;
        }

        public Constraint GetUniqueConstraintForColumns(int[] cols)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint constraint = this.ConstraintList[index];
                if (constraint.IsUniqueWithColumns(cols))
                {
                    return constraint;
                }
                index++;
            }
            return null;
        }

        public Constraint GetUniqueConstraintForColumns(int[] mainTableCols, int[] refTableCols)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint constraint2 = this.ConstraintList[index];
                switch (constraint2.GetConstraintType())
                {
                    case 2:
                    case 4:
                    {
                        int[] mainColumns = constraint2.GetMainColumns();
                        if (mainColumns.Length == mainTableCols.Length)
                        {
                            if (ArrayUtil.AreEqual(mainColumns, mainTableCols, mainTableCols.Length, true))
                            {
                                return constraint2;
                            }
                            if (ArrayUtil.AreEqualSets(mainColumns, mainTableCols))
                            {
                                int[] numArray2 = new int[mainTableCols.Length];
                                for (int i = 0; i < mainTableCols.Length; i++)
                                {
                                    int num5 = ArrayUtil.Find(mainColumns, mainTableCols[i]);
                                    numArray2[num5] = refTableCols[i];
                                }
                                for (int j = 0; j < mainTableCols.Length; j++)
                                {
                                    refTableCols[j] = numArray2[j];
                                }
                                return constraint2;
                            }
                        }
                        break;
                    }
                }
                index++;
            }
            return null;
        }

        public Constraint GetUniqueConstraintForIndex(Index index)
        {
            int num = 0;
            int length = this.ConstraintList.Length;
            while (num < length)
            {
                Constraint constraint = this.ConstraintList[num];
                if ((constraint.GetMainIndex() == index) && ((constraint.GetConstraintType() == 4) || (constraint.GetConstraintType() == 2)))
                {
                    return constraint;
                }
                num++;
            }
            return null;
        }

        public int[] GetUniqueNotNullColumnGroup(bool[] usedColumns)
        {
            int index = 0;
            int length = this.ConstraintList.Length;
            while (index < length)
            {
                Constraint constraint = this.ConstraintList[index];
                if (constraint.ConstType == 2)
                {
                    int[] mainColumns = constraint.GetMainColumns();
                    if (ArrayUtil.AreAllIntIndexesInBooleanArray(mainColumns, base.ColNotNull) && ArrayUtil.AreAllIntIndexesInBooleanArray(mainColumns, usedColumns))
                    {
                        return mainColumns;
                    }
                }
                else if (constraint.ConstType == 4)
                {
                    int[] mainColumns = constraint.GetMainColumns();
                    if (ArrayUtil.AreAllIntIndexesInBooleanArray(mainColumns, usedColumns))
                    {
                        return mainColumns;
                    }
                }
                index++;
            }
            return null;
        }

        public virtual int[] GetUpdatableColumns()
        {
            return this.DefaultColumnMap;
        }

        public bool HasIdentityColumn()
        {
            return (this.IdentityColumn != -1);
        }

        public bool HasLobColumn()
        {
            return base.hasLobColumn;
        }

        public bool HasTrigger(int trigVecIndex)
        {
            return (this.TriggerLists[trigVecIndex].Length > 0);
        }

        public virtual void InsertData(Session session, IPersistentStore store, object[] data)
        {
            Row newCachedObject = store.GetNewCachedObject(base._isTemp ? session : null, data);
            store.IndexRow(session, newCachedObject);
        }

        public void InsertFromNavigator(Session session, IPersistentStore store, RowSetNavigatorData nav)
        {
            while (nav.HasNext())
            {
                object[] data = ArrayUtil.ResizeArrayIfDifferent<object>(nav.GetNext(), base.ColumnCount);
                this.InsertData(session, store, data);
            }
        }

        public void InsertFromScript(Session session, IPersistentStore store, object[] data)
        {
            this.SystemUpdateIdentityValue(data);
            this.InsertData(session, store, data);
        }

        public void InsertIntoTable(Session session, Result result)
        {
            IPersistentStore rowStore = session.sessionData.GetRowStore(this);
            RowSetNavigator navigator = result.InitialiseNavigator();
            while (navigator.HasNext())
            {
                object[] data = ArrayUtil.ResizeArrayIfDifferent<object>(navigator.GetNext(), base.ColumnCount);
                this.InsertData(session, rowStore, data);
            }
        }

        public void InsertNoCheckFromLog(Session session, object[] data)
        {
            this.SystemUpdateIdentityValue(data);
            IPersistentStore rowStore = session.sessionData.GetRowStore(this);
            Row newCachedObject = rowStore.GetNewCachedObject(session, data);
            rowStore.IndexRow(session, newCachedObject);
            session.AddInsertAction(this, newCachedObject);
        }

        public void InsertResult(Session session, IPersistentStore store, Result ins)
        {
            RowSetNavigator navigator = ins.InitialiseNavigator();
            while (navigator.HasNext())
            {
                object[] data = ArrayUtil.ResizeArrayIfDifferent<object>(navigator.GetNext(), base.ColumnCount);
                this.InsertData(session, store, data);
            }
        }

        public Row InsertSingleRow(Session session, IPersistentStore store, object[] data, int[] changedCols)
        {
            if (this.IdentityColumn != -1)
            {
                this.SetIdentityColumn(session, data);
            }
            if (this._hasGeneratedValues)
            {
                this.SetGeneratedColumns(session, data);
            }
            if (this._hasDomainColumns || this._hasNotNullColumns)
            {
                this.EnforceRowConstraints(session, data);
            }
            if (base._isView)
            {
                return null;
            }
            Row newCachedObject = store.GetNewCachedObject(session, data);
            store.IndexRow(session, newCachedObject);
            session.AddInsertAction(this, newCachedObject);
            return newCachedObject;
        }

        public static int InsertSys(IPersistentStore store, Result ins)
        {
            RowSetNavigator navigator = ins.GetNavigator();
            int num = 0;
            while (navigator.HasNext())
            {
                InsertSys(store, navigator.GetNext());
                num++;
            }
            return num;
        }

        public static void InsertSys(IPersistentStore store, object[] data)
        {
            Row newCachedObject = store.GetNewCachedObject(null, data);
            store.IndexRow(null, newCachedObject);
        }

        public void InsertSys(Session session, IPersistentStore store, object[] data)
        {
            Row newCachedObject = store.GetNewCachedObject(null, data);
            store.IndexRow(session, newCachedObject);
        }

        public bool IsBestRowIdentifiersStrict()
        {
            return base.BestRowIdentifierStrict;
        }

        public bool IsCached()
        {
            return base._isCached;
        }

        public virtual bool IsConnected()
        {
            return true;
        }

        public virtual bool IsDataReadOnly()
        {
            return base._isReadOnly;
        }

        public bool IsFileBased()
        {
            return base._isCached;
        }

        public virtual bool IsIndexCached()
        {
            return base._isCached;
        }

        public bool IsIndexed(int colIndex)
        {
            return (base.BestIndexForColumn[colIndex] != -1);
        }

        public bool IsIndexingMutable()
        {
            return !this.IsIndexCached();
        }

        public virtual bool IsInsertable()
        {
            return this.IsWritable();
        }

        public bool IsReadOnly()
        {
            return base._isReadOnly;
        }

        public bool IsTemp()
        {
            return base._isTemp;
        }

        public virtual bool IsTriggerDeletable()
        {
            return false;
        }

        public virtual bool IsTriggerInsertable()
        {
            return false;
        }

        public virtual bool IsTriggerUpdatable()
        {
            return false;
        }

        public virtual bool IsUpdatable()
        {
            return this.IsWritable();
        }

        public bool IsView()
        {
            return base._isView;
        }

        public virtual bool IsWritable()
        {
            if (this.IsReadOnly() || base.database.DatabaseReadOnly)
            {
                return false;
            }
            if (base.database.IsFilesReadOnly())
            {
                return !base._isCached;
            }
            return true;
        }

        public Table MoveDefinition(Session session, int newType, ColumnSchema column, Constraint constraint, Index index, int colIndex, int adjust, OrderedHashSet<QNameManager.QName> dropConstraints, OrderedHashSet<QNameManager.QName> dropIndexes)
        {
            bool flag = false;
            if ((constraint != null) && (constraint.ConstType == 4))
            {
                flag = true;
            }
            Table newTable = new Table(base.database, this.TableName, newType);
            if (base.TableType == 3)
            {
                newTable.PersistenceScope = base.PersistenceScope;
            }
            int num = 0;
            while (num < base.ColumnCount)
            {
                ColumnSchema schema = this.ColumnList.Get(num);
                if (num != colIndex)
                {
                    goto Label_0066;
                }
                if (column != null)
                {
                    newTable.AddColumn(column);
                }
                if (adjust > 0)
                {
                    goto Label_0066;
                }
            Label_0060:
                num++;
                continue;
            Label_0066:
                newTable.AddColumn(schema);
                goto Label_0060;
            }
            if (base.ColumnCount == colIndex)
            {
                newTable.AddColumn(column);
            }
            int[] columns = null;
            if (base.HasPrimaryKey() && !dropConstraints.Contains(this.GetPrimaryConstraint().GetName()))
            {
                columns = ArrayUtil.ToAdjustedColumnArray(base.PrimaryKeyCols, colIndex, adjust);
            }
            else if (flag)
            {
                columns = constraint.GetMainColumns();
            }
            newTable.CreatePrimaryKey(base.GetIndex(0).GetName(), columns, false);
            for (int i = 1; i < base.IndexList.Length; i++)
            {
                Index index2 = base.IndexList[i];
                if (!dropIndexes.Contains(index2.GetName()))
                {
                    int[] numArray2 = ArrayUtil.ToAdjustedColumnArray(index2.GetColumns(), colIndex, adjust);
                    index2 = newTable.CreateIndexStructure(index2.GetName(), numArray2, index2.GetColumnDesc(), null, index2.IsUnique(), index2.IsConstraint(), index2.IsForward());
                    newTable.AddIndex(session, index2);
                }
            }
            if (index != null)
            {
                newTable.AddIndex(session, index);
            }
            List<Constraint> list = new List<Constraint>();
            if (flag)
            {
                constraint.Core.MainIndex = newTable.IndexList[0];
                constraint.Core.MainTable = newTable;
                constraint.Core.MainTableName = newTable.TableName;
                list.Add(constraint);
            }
            for (int j = 0; j < this.ConstraintList.Length; j++)
            {
                Constraint item = this.ConstraintList[j];
                if (!dropConstraints.Contains(item.GetName()))
                {
                    item = item.Duplicate();
                    item.UpdateTable(session, this, newTable, colIndex, adjust);
                    list.Add(item);
                }
            }
            if (!flag && (constraint != null))
            {
                constraint.UpdateTable(session, this, newTable, -1, 0);
                list.Add(constraint);
            }
            newTable.ConstraintList = list.ToArray();
            newTable.UpdateConstraintLists();
            newTable.SetBestRowIdentifiers();
            newTable.TriggerList = this.TriggerList;
            newTable.TriggerLists = this.TriggerLists;
            return newTable;
        }

        public void ReleaseTriggers()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < this.TriggerLists[i].Length; j++)
                {
                    this.TriggerLists[i][j].Terminate();
                }
                this.TriggerLists[i] = TriggerDef.EmptyArray;
            }
        }

        public void RemoveConstraint(int index)
        {
            this.ConstraintList = ArrayUtil.ToAdjustedArray<Constraint>(this.ConstraintList, null, index, -1);
            this.UpdateConstraintLists();
        }

        public void RemoveConstraint(string name)
        {
            int constraintIndex = this.GetConstraintIndex(name);
            if (constraintIndex != -1)
            {
                this.RemoveConstraint(constraintIndex);
            }
        }

        public virtual void RemoveTrigger(TriggerDef trigger)
        {
            TriggerDef def = null;
            for (int i = 0; i < this.TriggerList.Length; i++)
            {
                def = this.TriggerList[i];
                if (def.GetName().Name.Equals(trigger.GetName().Name))
                {
                    def.Terminate();
                    this.TriggerList = ArrayUtil.ToAdjustedArray<TriggerDef>(this.TriggerList, null, i, -1);
                    break;
                }
            }
            if (def != null)
            {
                int triggerType = def.TriggerType;
                for (int j = 0; j < this.TriggerLists[triggerType].Length; j++)
                {
                    def = this.TriggerLists[triggerType][j];
                    if (def.GetName().Name.Equals(trigger.GetName().Name))
                    {
                        def.Terminate();
                        this.TriggerLists[triggerType] = ArrayUtil.ToAdjustedArray<TriggerDef>(this.TriggerLists[triggerType], null, j, -1);
                        return;
                    }
                }
            }
        }

        public void RenameColumn(ColumnSchema column, QNameManager.QName newName)
        {
            string name = column.GetName().Name;
            int columnIndex = this.GetColumnIndex(name);
            if (this.FindColumn(newName.Name) != -1)
            {
                throw Error.GetError(0x1580);
            }
            this.ColumnList.SetKey(columnIndex, newName.Name);
            column.GetName().Rename(newName);
        }

        public void ResetDefaultsFlag()
        {
            this._hasDefaultValues = false;
            this._hasGeneratedValues = false;
            this._hasNotNullColumns = false;
            for (int i = 0; i < base.ColumnCount; i++)
            {
                this._hasDefaultValues |= this.ColDefaults[i] > null;
                this._hasGeneratedValues |= this.ColGenerated[i];
                this._hasNotNullColumns |= base.ColNotNull[i];
            }
        }

        public void SetColumnStructures()
        {
            base.ColTypes = new SqlType[base.ColumnCount];
            this.ColDefaults = new Expression[base.ColumnCount];
            base.ColNotNull = new bool[base.ColumnCount];
            this.ColGenerated = new bool[base.ColumnCount];
            this.DefaultColumnMap = new int[base.ColumnCount];
            this._hasDomainColumns = false;
            for (int i = 0; i < base.ColumnCount; i++)
            {
                this.SetColumnTypeVars(i);
            }
            this.ResetDefaultsFlag();
            this.DefaultRanges = new RangeVariable[] { new RangeVariable(this, 1) };
        }

        public void SetColumnTypeVars(int i)
        {
            ColumnSchema column = this.GetColumn(i);
            SqlType dataType = column.GetDataType();
            if (dataType.IsDomainType())
            {
                this._hasDomainColumns = true;
            }
            base.ColTypes[i] = dataType;
            base.ColNotNull[i] = column.IsPrimaryKey() || !column.IsNullable();
            this.DefaultColumnMap[i] = i;
            if (column.IsIdentity())
            {
                this.IdentitySequence = column.GetIdentitySequence();
                this.IdentityColumn = i;
            }
            else if (this.IdentityColumn == i)
            {
                this.IdentityColumn = -1;
            }
            this.ColDefaults[i] = column.GetDefaultExpression();
            this.ColGenerated[i] = column.IsGenerated();
            this.ResetDefaultsFlag();
        }

        public virtual void SetDataReadOnly(bool value)
        {
            if ((!value && base.database.IsFilesReadOnly()) && this.IsFileBased())
            {
                throw Error.GetError(0x1c8);
            }
            base._isReadOnly = value;
        }

        public void SetDefaultExpression(int columnIndex, Expression def)
        {
            this.GetColumn(columnIndex).SetDefaultExpression(def);
            this.SetColumnTypeVars(columnIndex);
        }

        public void SetGeneratedColumns(Session session, object[] data)
        {
            if (this._hasGeneratedValues)
            {
                for (int i = 0; i < this.ColGenerated.Length; i++)
                {
                    if (this.ColGenerated[i])
                    {
                        Expression generatingExpression = this.GetColumn(i).GetGeneratingExpression();
                        session.sessionContext.GetCheckIterator(this.DefaultRanges[0]).CurrentData = data;
                        data[i] = generatingExpression.GetValue(session, base.ColTypes[i]);
                    }
                }
            }
        }

        public void SetIdentityColumn(Session session, object[] data)
        {
            if (this.IdentityColumn != -1)
            {
                object valueObject = data[this.IdentityColumn];
                if (valueObject == null)
                {
                    valueObject = this.IdentitySequence.GetValueObject();
                    data[this.IdentityColumn] = valueObject;
                }
                else
                {
                    this.IdentitySequence.UserUpdate(Convert.ToInt64(valueObject));
                }
                if (session != null)
                {
                    session.SetLastIdentity(valueObject);
                }
            }
        }

        public void SetIndexRoots(int[] roots)
        {
            if (!base._isCached)
            {
                throw Error.GetError(0x157d, this.TableName.Name);
            }
            IPersistentStore store = base.database.persistentStoreCollection.GetStore(this);
            int num = 0;
            for (int i = 0; i < base.IndexList.Length; i++)
            {
                store.SetAccessor(base.IndexList[i], roots[num++]);
            }
            int size = roots[base.IndexList.Length * 2];
            for (int j = 0; j < base.IndexList.Length; j++)
            {
                store.SetElementCount(base.IndexList[j], size, roots[num++]);
            }
        }

        public void SetIndexRoots(Session session, string s)
        {
            if (!base._isCached)
            {
                throw Error.GetError(0x157d, this.TableName.Name);
            }
            using (Scanner scanner = new Scanner(s))
            {
                ParserDQL rdql = new ParserDQL(session, scanner);
                int[] roots = new int[(base.GetIndexCount() * 2) + 1];
                rdql.Read();
                int index = 0;
                for (int i = 0; i < base.GetIndexCount(); i++)
                {
                    int num3 = rdql.ReadInteger();
                    roots[index++] = num3;
                }
                try
                {
                    for (int j = 0; j < (base.GetIndexCount() + 1); j++)
                    {
                        int num5 = rdql.ReadInteger();
                        roots[index++] = num5;
                    }
                }
                catch (Exception)
                {
                    for (index = base.GetIndexCount(); index < roots.Length; index++)
                    {
                        roots[index] = -1;
                    }
                }
                this.SetIndexRoots(roots);
            }
        }

        public void SetName(QNameManager.QName name)
        {
            this.TableName = name;
        }

        public void SystemSetIdentityColumn(Session session, object[] data)
        {
            if (this.IdentityColumn != -1)
            {
                object valueObject = data[this.IdentityColumn];
                if (valueObject == null)
                {
                    valueObject = this.IdentitySequence.GetValueObject();
                    data[this.IdentityColumn] = valueObject;
                }
                else
                {
                    this.IdentitySequence.UserUpdate(Convert.ToInt64(valueObject));
                }
            }
        }

        protected void SystemUpdateIdentityValue(object[] data)
        {
            if (this.IdentityColumn != -1)
            {
                object obj2 = data[this.IdentityColumn];
                if (obj2 != null)
                {
                    this.IdentitySequence.SystemUpdate(Convert.ToInt64(obj2));
                }
            }
        }

        public void UpdateConstraintLists()
        {
            int index = 0;
            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < this.ConstraintList.Length; i++)
            {
                switch (this.ConstraintList[i].GetConstraintType())
                {
                    case 0:
                        index++;
                        break;

                    case 1:
                        num2++;
                        break;

                    case 3:
                        if (!this.ConstraintList[i].IsNotNull())
                        {
                            num3++;
                        }
                        break;
                }
            }
            this.FkConstraints = (index == 0) ? Constraint.EmptyArray : new Constraint[index];
            index = 0;
            this.FkMainConstraints = (num2 == 0) ? Constraint.EmptyArray : new Constraint[num2];
            num2 = 0;
            this.CheckConstraints = (num3 == 0) ? Constraint.EmptyArray : new Constraint[num3];
            num3 = 0;
            this._colRefFk = new bool[base.ColumnCount];
            this._colMainFk = new bool[base.ColumnCount];
            for (int j = 0; j < this.ConstraintList.Length; j++)
            {
                switch (this.ConstraintList[j].GetConstraintType())
                {
                    case 0:
                        this.FkConstraints[index] = this.ConstraintList[j];
                        ArrayUtil.IntIndexesToBooleanArray(this.ConstraintList[j].GetRefColumns(), this._colRefFk);
                        index++;
                        break;

                    case 1:
                        this.FkMainConstraints[num2] = this.ConstraintList[j];
                        ArrayUtil.IntIndexesToBooleanArray(this.ConstraintList[j].GetMainColumns(), this._colMainFk);
                        this.ConstraintList[j].HasTriggeredAction();
                        num2++;
                        break;

                    case 3:
                        if (!this.ConstraintList[j].IsNotNull())
                        {
                            this.CheckConstraints[num3] = this.ConstraintList[j];
                            num3++;
                        }
                        break;
                }
            }
        }

        public void VerifyConstraintsIntegrity()
        {
            for (int i = 0; i < this.ConstraintList.Length; i++)
            {
                Constraint constraint = this.ConstraintList[i];
                if (constraint.IsForeignKeyOrMain())
                {
                    Table main = constraint.GetMain();
                    QNameManager.QName name = main.GetName();
                    if (main != base.database.schemaManager.FindUserTable(null, name.Name, name.schema.Name))
                    {
                        throw Error.RuntimeError(0xc9, "FK mismatch : " + constraint.GetName().Name);
                    }
                    Table table2 = constraint.GetRef();
                    QNameManager.QName name2 = table2.GetName();
                    if (table2 != base.database.schemaManager.FindUserTable(null, name2.Name, name2.schema.Name))
                    {
                        throw Error.RuntimeError(0xc9, "FK mismatch : " + constraint.GetName().Name);
                    }
                }
            }
        }
    }
}

