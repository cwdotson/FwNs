namespace FwNs.Core.LC.cTables
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;

    public sealed class TableWorks
    {
        public OrderedHashSet<QNameManager.QName> EmptySetQName = new OrderedHashSet<QNameManager.QName>();
        private readonly Database _database;
        private Table _table;
        private readonly Session _session;

        public TableWorks(Session session, Table table)
        {
            this._database = table.database;
            this._table = table;
            this._session = session;
        }

        public void AddCheckConstraint(Constraint c)
        {
            this._database.schemaManager.CheckSchemaObjectNotExists(c.GetName());
            c.PrepareCheckConstraint(this._session, this._table, true);
            this._table.AddConstraint(c);
            if (c.IsNotNull())
            {
                this._table.GetColumn(c.NotNullColumnIndex).SetNullable(false);
                this._table.SetColumnTypeVars(c.NotNullColumnIndex);
            }
            this._database.schemaManager.AddSchemaObject(c);
        }

        public void AddColumn(ColumnSchema column, int colIndex, List<Constraint> constraints)
        {
            Table table = this._table;
            Constraint c = null;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            this.CheckAddColumn(column);
            Constraint constraint = constraints[0];
            if (constraint.GetConstraintType() == 4)
            {
                if (column.GetDataType().IsLobType())
                {
                    throw Error.GetError(0x159e);
                }
                constraint.Core.MainCols = new int[] { colIndex };
                this._database.schemaManager.CheckSchemaObjectNotExists(constraint.GetName());
                if (this._table.HasPrimaryKey())
                {
                    throw Error.GetError(0x159a);
                }
                flag2 = true;
            }
            else
            {
                constraint = null;
            }
            this._table = this._table.MoveDefinition(this._session, this._table.TableType, column, constraint, null, colIndex, 1, this.EmptySetQName, this.EmptySetQName);
            for (int i = 1; i < constraints.Count; i++)
            {
                constraint = constraints[i];
                switch (constraint.ConstType)
                {
                    case 0:
                        if (flag)
                        {
                            throw Error.GetError(0x1598);
                        }
                        break;

                    case 1:
                    {
                        continue;
                    }
                    case 2:
                        if (flag2)
                        {
                            throw Error.GetError(0x1592);
                        }
                        goto Label_036B;

                    case 3:
                        if (flag3)
                        {
                            throw Error.GetError(0x1598);
                        }
                        goto Label_044C;

                    default:
                    {
                        continue;
                    }
                }
                flag = true;
                constraint.Core.RefCols = new int[] { colIndex };
                constraint.Core.RefTable = this._table;
                bool flag1 = table.GetName() == constraint.Core.MainTableName;
                if (flag1)
                {
                    constraint.Core.MainTable = this._table;
                }
                else
                {
                    constraint.Core.MainTable = this._database.schemaManager.GetTable(this._session, constraint.Core.MainTableName.Name, constraint.Core.MainTableName.schema.Name);
                }
                constraint.SetColumnsIndexes(this._table);
                this.CheckCreateForeignKey(constraint);
                Constraint uniqueConstraintForColumns = constraint.Core.MainTable.GetUniqueConstraintForColumns(constraint.Core.MainCols, constraint.Core.RefCols);
                bool forward = constraint.Core.MainTable.GetSchemaName() != this._table.GetSchemaName();
                int tableIndex = this._database.schemaManager.GetTableIndex(table);
                if (!flag1 && (tableIndex < this._database.schemaManager.GetTableIndex(constraint.Core.MainTable)))
                {
                    forward = true;
                }
                QNameManager.QName name = this._database.NameManager.NewAutoName("IDX", constraint.GetName().Name, this._table.GetSchemaName(), this._table.GetName(), 20);
                Index index = this._table.CreateAndAddIndexStructure(this._session, name, constraint.GetRefColumns(), null, null, false, true, forward);
                constraint.Core.RefName = constraint.GetName();
                constraint.Core.UniqueName = uniqueConstraintForColumns.GetName();
                constraint.Core.MainName = this._database.NameManager.NewAutoName("REF", constraint.Core.RefName.Name, this._table.GetSchemaName(), this._table.GetName(), 20);
                constraint.Core.MainIndex = uniqueConstraintForColumns.GetMainIndex();
                constraint.Core.RefIndex = index;
                constraint.IsForward = forward;
                this._table.AddConstraint(constraint);
                c = new Constraint(constraint.Core.MainName, constraint);
                continue;
            Label_036B:
                if (column.GetDataType().IsLobType())
                {
                    throw Error.GetError(0x159e);
                }
                flag2 = true;
                constraint.Core.MainCols = new int[] { colIndex };
                this._database.schemaManager.CheckSchemaObjectNotExists(constraint.GetName());
                QNameManager.QName name2 = this._database.NameManager.NewAutoName("IDX", constraint.GetName().Name, this._table.GetSchemaName(), this._table.GetName(), 20);
                Index index2 = this._table.CreateAndAddIndexStructure(this._session, name2, constraint.GetMainColumns(), null, null, true, true, false);
                constraint.Core.MainTable = this._table;
                constraint.Core.MainIndex = index2;
                this._table.AddConstraint(constraint);
                continue;
            Label_044C:
                flag3 = true;
                constraint.PrepareCheckConstraint(this._session, this._table, false);
                this._table.AddConstraint(constraint);
                if (constraint.IsNotNull())
                {
                    column.SetNullable(false);
                    this._table.SetColumnTypeVars(colIndex);
                }
            }
            column.Compile(this._session, this._table);
            this.MoveData(table, this._table, colIndex, 1);
            if (c != null)
            {
                c.GetMain().AddConstraint(c);
            }
            this.RegisterConstraintNames(constraints);
            this.SetNewTableInSchema(this._table);
            this.UpdateConstraints(this._table, this.EmptySetQName);
            this._database.schemaManager.AddSchemaObject(column);
            this._database.schemaManager.RecompileDependentObjects(this._table);
            this._table.Compile(this._session, null);
        }

        public void AddForeignKey(Constraint c)
        {
            this.CheckModifyTable();
            this.CheckCreateForeignKey(c);
            Constraint uniqueConstraintForColumns = c.Core.MainTable.GetUniqueConstraintForColumns(c.Core.MainCols, c.Core.RefCols);
            Index mainIndex = uniqueConstraintForColumns.GetMainIndex();
            uniqueConstraintForColumns.CheckReferencedRows(this._session, this._table, c.Core.RefCols);
            bool forward = false;
            if (c.Core.MainTable.GetSchemaName() == this._table.GetSchemaName())
            {
                int tableIndex = this._database.schemaManager.GetTableIndex(this._table);
                if ((tableIndex != -1) && (tableIndex < this._database.schemaManager.GetTableIndex(c.Core.MainTable)))
                {
                    forward = true;
                }
            }
            else
            {
                forward = true;
            }
            QNameManager.QName name = this._database.NameManager.NewAutoName("IDX", this._table.GetSchemaName(), this._table.GetName(), 20);
            Index index = this._table.CreateIndexStructure(name, c.Core.RefCols, null, null, false, true, forward);
            QNameManager.QName name2 = this._database.NameManager.NewAutoName("REF", c.GetName().Name, this._table.GetSchemaName(), this._table.GetName(), 20);
            c.Core.UniqueName = uniqueConstraintForColumns.GetName();
            c.Core.MainName = name2;
            c.Core.MainIndex = mainIndex;
            c.Core.RefTable = this._table;
            c.Core.RefName = c.GetName();
            c.Core.RefIndex = index;
            c.IsForward = forward;
            Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, null, c, index, -1, 0, this.EmptySetQName, this.EmptySetQName);
            this.MoveData(this._table, newTable, -1, 0);
            this._database.schemaManager.AddSchemaObject(c);
            this.SetNewTableInSchema(newTable);
            this._database.schemaManager.GetTable(this._session, c.Core.MainTable.GetName().Name, c.Core.MainTable.GetSchemaName().Name).AddConstraint(new Constraint(name2, c));
            this.UpdateConstraints(newTable, this.EmptySetQName);
            this._database.schemaManager.RecompileDependentObjects(newTable);
            this._table = newTable;
        }

        public Index AddIndex(int[] col, QNameManager.QName name, bool unique)
        {
            Index index;
            this.CheckModifyTable();
            if (this._table.IsEmpty(this._session) || this._table.IsIndexingMutable())
            {
                index = this._table.CreateIndex(this._session, name, col, null, null, unique, false, false);
            }
            else
            {
                index = this._table.CreateIndexStructure(name, col, null, null, unique, false, false);
                Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, null, null, index, -1, 0, this.EmptySetQName, this.EmptySetQName);
                this.MoveData(this._table, newTable, -1, 0);
                this._table = newTable;
                this.SetNewTableInSchema(this._table);
                this.UpdateConstraints(this._table, this.EmptySetQName);
            }
            this._database.schemaManager.AddSchemaObject(index);
            this._database.schemaManager.RecompileDependentObjects(this._table);
            return index;
        }

        public void AddPrimaryKey(Constraint constraint, QNameManager.QName name)
        {
            this.CheckModifyTable();
            if (this._table.HasPrimaryKey())
            {
                throw Error.GetError(0x159c);
            }
            this._database.schemaManager.CheckSchemaObjectNotExists(name);
            Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, null, constraint, null, -1, 0, this.EmptySetQName, this.EmptySetQName);
            this.MoveData(this._table, newTable, -1, 0);
            this._table = newTable;
            this._database.schemaManager.AddSchemaObject(constraint);
            this.SetNewTableInSchema(this._table);
            this.UpdateConstraints(this._table, this.EmptySetQName);
            this._database.schemaManager.RecompileDependentObjects(this._table);
        }

        public void AddUniqueConstraint(int[] cols, QNameManager.QName name)
        {
            this.CheckModifyTable();
            this._database.schemaManager.CheckSchemaObjectNotExists(name);
            if (this._table.GetUniqueConstraintForColumns(cols) != null)
            {
                throw Error.GetError(0x1592);
            }
            QNameManager.QName name2 = this._database.NameManager.NewAutoName("IDX", name.Name, this._table.GetSchemaName(), this._table.GetName(), 20);
            Index index = this._table.CreateIndexStructure(name2, cols, null, null, true, true, false);
            Constraint constraint = new Constraint(name, this._table, index, 2);
            Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, null, constraint, index, -1, 0, this.EmptySetQName, this.EmptySetQName);
            this.MoveData(this._table, newTable, -1, 0);
            this._table = newTable;
            this._database.schemaManager.AddSchemaObject(constraint);
            this.SetNewTableInSchema(this._table);
            this.UpdateConstraints(this._table, this.EmptySetQName);
            this._database.schemaManager.RecompileDependentObjects(this._table);
        }

        public void CheckAddColumn(ColumnSchema col)
        {
            this.CheckModifyTable();
            if (this._table.FindColumn(col.GetName().Name) != -1)
            {
                throw Error.GetError(0x1580);
            }
            if (col.IsPrimaryKey() && this._table.HasPrimaryKey())
            {
                throw Error.GetError(0x159a);
            }
            if (col.IsIdentity() && this._table.HasIdentityColumn())
            {
                throw Error.GetError(0x1595);
            }
            if (((!this._table.IsEmpty(this._session) && !col.HasDefault) && (!col.IsNullable() || col.IsPrimaryKey())) && !col.IsIdentity())
            {
                throw Error.GetError(0x159b);
            }
        }

        public void CheckConvertColDataType(ColumnSchema oldCol, ColumnSchema newCol)
        {
            int columnIndex = this._table.GetColumnIndex(oldCol.GetName().Name);
            IRowIterator rowIterator = this._table.GetRowIterator(this._session);
            while (rowIterator.HasNext())
            {
                object a = rowIterator.GetNextRow().RowData[columnIndex];
                newCol.GetDataType().ConvertToType(this._session, a, oldCol.GetDataType());
            }
        }

        public void CheckCreateForeignKey(Constraint c)
        {
            if ((((c.Core.UpdateAction == 4) || (c.Core.UpdateAction == 2)) || ((c.Core.UpdateAction == 0) || (c.Core.DeleteAction == 4))) || (c.Core.DeleteAction == 2))
            {
                for (int i = 0; i < c.Core.RefCols.Length; i++)
                {
                    ColumnSchema column = this._table.GetColumn(c.Core.RefCols[i]);
                    if (column.IsGenerated())
                    {
                        throw Error.GetError(0x1594, column.GetNameString());
                    }
                }
            }
            if ((c.Core.MainName == this._table.GetName()) && ArrayUtil.HaveCommonElement(c.Core.RefCols, c.Core.MainCols))
            {
                throw Error.GetError(0x1597);
            }
            if ((c.Core.UpdateAction == 4) || (c.Core.DeleteAction == 4))
            {
                for (int i = 0; i < c.Core.RefCols.Length; i++)
                {
                    ColumnSchema column = this._table.GetColumn(c.Core.RefCols[i]);
                    if (column.GetDefaultExpression() == null)
                    {
                        string statementName = column.GetName().StatementName;
                        throw Error.GetError(0x1591, statementName);
                    }
                }
            }
            if (((c.Core.UpdateAction == 2) || (c.Core.DeleteAction == 2)) && !this._session.IsProcessingScript())
            {
                for (int i = 0; i < c.Core.RefCols.Length; i++)
                {
                    ColumnSchema column = this._table.GetColumn(c.Core.RefCols[i]);
                    if (!column.IsNullable())
                    {
                        string statementName = column.GetName().StatementName;
                        throw Error.GetError(0x1590, statementName);
                    }
                }
            }
            this._database.schemaManager.CheckSchemaObjectNotExists(c.GetName());
            if (this._table.GetConstraint(c.GetName().Name) != null)
            {
                throw Error.GetError(0x1580, c.GetName().StatementName);
            }
            if (this._table.GetFkConstraintForColumns(c.Core.MainTable, c.Core.MainCols, c.Core.RefCols) != null)
            {
                throw Error.GetError(0x1598, c.GetName().StatementName);
            }
            if (c.Core.MainTable.IsTemp() != this._table.IsTemp())
            {
                throw Error.GetError(0x1594, c.GetName().StatementName);
            }
            if (c.Core.MainTable.GetUniqueConstraintForColumns(c.Core.MainCols, c.Core.RefCols) == null)
            {
                throw Error.GetError(0x1599, c.GetMain().GetName().StatementName);
            }
            c.Core.MainTable.CheckColumnsMatch(c.Core.MainCols, this._table, c.Core.RefCols);
            bool[] columnCheckList = c.Core.MainTable.GetColumnCheckList(c.Core.MainCols);
            this._session.GetGrantee().CheckReferences(c.Core.MainTable, columnCheckList);
        }

        private void CheckModifyTable()
        {
            if ((!this._session.GetUser().IsSystem && !this._session.IsProcessingScript()) && (this._database.IsFilesReadOnly() || this._table.IsReadOnly()))
            {
                throw Error.GetError(0x1c8);
            }
        }

        public void DropColumn(int colIndex, bool cascade)
        {
            OrderedHashSet<QNameManager.QName> dropConstraintSet = new OrderedHashSet<QNameManager.QName>();
            OrderedHashSet<Constraint> dependentConstraints = this._table.GetDependentConstraints(colIndex);
            OrderedHashSet<Constraint> containingConstraints = this._table.GetContainingConstraints(colIndex);
            OrderedHashSet<QNameManager.QName> containingIndexNames = this._table.GetContainingIndexNames(colIndex);
            ColumnSchema column = this._table.GetColumn(colIndex);
            QNameManager.QName name = column.GetName();
            OrderedHashSet<QNameManager.QName> referencingObjectNames = this._database.schemaManager.GetReferencingObjectNames(this._table.GetName(), name);
            this.CheckModifyTable();
            if (!cascade)
            {
                if (!containingConstraints.IsEmpty())
                {
                    QNameManager.QName name2 = containingConstraints.Get(0).GetName();
                    throw Error.GetError(0x15a0, name2.GetSchemaQualifiedStatementName());
                }
                if (!referencingObjectNames.IsEmpty())
                {
                    for (int j = 0; j < referencingObjectNames.Size(); j++)
                    {
                        QNameManager.QName name3 = referencingObjectNames.Get(j);
                        if (name3 != name)
                        {
                            for (int k = 0; k < dependentConstraints.Size(); k++)
                            {
                                if (dependentConstraints.Get(k).GetName() == name3)
                                {
                                    continue;
                                }
                            }
                            throw Error.GetError(0x15a0, name3.GetSchemaQualifiedStatementName());
                        }
                    }
                }
            }
            dependentConstraints.AddAll(containingConstraints);
            containingConstraints.Clear();
            OrderedHashSet<Table> tableSet = new OrderedHashSet<Table>();
            for (int i = 0; i < dependentConstraints.Size(); i++)
            {
                Constraint constraint = dependentConstraints.Get(i);
                if (constraint.ConstType == 0)
                {
                    tableSet.Add(constraint.GetMain());
                    dropConstraintSet.Add(constraint.GetMainName());
                    dropConstraintSet.Add(constraint.GetRefName());
                    containingIndexNames.Add(constraint.GetRefIndex().GetName());
                }
                if (constraint.ConstType == 1)
                {
                    tableSet.Add(constraint.GetRef());
                    dropConstraintSet.Add(constraint.GetMainName());
                    dropConstraintSet.Add(constraint.GetRefName());
                    containingIndexNames.Add(constraint.GetRefIndex().GetName());
                }
                dropConstraintSet.Add(constraint.GetName());
            }
            tableSet = this.MakeNewTables(tableSet, dropConstraintSet, containingIndexNames);
            Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, null, null, null, colIndex, -1, dropConstraintSet, containingIndexNames);
            this.MoveData(this._table, newTable, colIndex, -1);
            this._database.schemaManager.RemoveSchemaObjects(referencingObjectNames);
            this._database.schemaManager.RemoveSchemaObjects(dropConstraintSet);
            this._database.schemaManager.RemoveSchemaObject(name);
            this.SetNewTableInSchema(newTable);
            this.SetNewTablesInSchema(tableSet);
            this.UpdateConstraints(newTable, this.EmptySetQName);
            this.UpdateConstraints(tableSet, dropConstraintSet);
            this._database.schemaManager.RecompileDependentObjects(tableSet);
            this._database.schemaManager.RecompileDependentObjects(newTable);
            newTable.Compile(this._session, null);
            if (column.GetDataType().IsLobType())
            {
                IRowIterator rowIterator = this._table.GetRowIterator(this._session);
                while (rowIterator.HasNext())
                {
                    object[] rowData = rowIterator.GetNextRow().RowData;
                    if (rowData[colIndex] != null)
                    {
                        this._session.sessionData.AdjustLobUsageCount(rowData[colIndex], -1);
                    }
                }
            }
            this._table = newTable;
        }

        public void DropConstraint(string name, bool cascade)
        {
            Constraint constraint = this._table.GetConstraint(name);
            if (constraint == null)
            {
                throw Error.GetError(0x157d, name);
            }
            switch (constraint.GetConstraintType())
            {
                case 0:
                {
                    this.CheckModifyTable();
                    OrderedHashSet<QNameManager.QName> dropConstraints = new OrderedHashSet<QNameManager.QName>();
                    QNameManager.QName mainName = constraint.GetMainName();
                    dropConstraints.Add(mainName);
                    dropConstraints.Add(constraint.GetRefName());
                    OrderedHashSet<QNameManager.QName> dropIndexes = new OrderedHashSet<QNameManager.QName>();
                    dropIndexes.Add(constraint.GetRefIndex().GetName());
                    Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, null, null, null, -1, 0, dropConstraints, dropIndexes);
                    this.MoveData(this._table, newTable, -1, 0);
                    this._database.schemaManager.RemoveSchemaObject(constraint.GetName());
                    this.SetNewTableInSchema(newTable);
                    constraint.GetMain().RemoveConstraint(mainName.Name);
                    this.UpdateConstraints(newTable, this.EmptySetQName);
                    this._database.schemaManager.RecompileDependentObjects(this._table);
                    this._table = newTable;
                    return;
                }
                case 1:
                    throw Error.GetError(0xfa2);

                case 2:
                case 4:
                {
                    this.CheckModifyTable();
                    OrderedHashSet<Constraint> dependentConstraints = this._table.GetDependentConstraints(constraint);
                    if (!cascade && !dependentConstraints.IsEmpty())
                    {
                        Constraint constraint2 = dependentConstraints.Get(0);
                        throw Error.GetError(0x159d, constraint2.GetName().GetSchemaQualifiedStatementName());
                    }
                    OrderedHashSet<Table> tableSet = new OrderedHashSet<Table>();
                    OrderedHashSet<QNameManager.QName> dropConstraints = new OrderedHashSet<QNameManager.QName>();
                    OrderedHashSet<QNameManager.QName> dropIndexes = new OrderedHashSet<QNameManager.QName>();
                    for (int i = 0; i < dependentConstraints.Size(); i++)
                    {
                        Constraint constraint3 = dependentConstraints.Get(i);
                        Table main = constraint3.GetMain();
                        if (main != this._table)
                        {
                            tableSet.Add(main);
                        }
                        main = constraint3.GetRef();
                        if (main != this._table)
                        {
                            tableSet.Add(main);
                        }
                        dropConstraints.Add(constraint3.GetMainName());
                        dropConstraints.Add(constraint3.GetRefName());
                        dropIndexes.Add(constraint3.GetRefIndex().GetName());
                    }
                    dropConstraints.Add(constraint.GetName());
                    if (constraint.GetConstraintType() == 2)
                    {
                        dropIndexes.Add(constraint.GetMainIndex().GetName());
                    }
                    Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, null, null, null, -1, 0, dropConstraints, dropIndexes);
                    this.MoveData(this._table, newTable, -1, 0);
                    tableSet = this.MakeNewTables(tableSet, dropConstraints, dropIndexes);
                    if (constraint.GetConstraintType() == 4)
                    {
                        int[] mainColumns = constraint.GetMainColumns();
                        for (int j = 0; j < mainColumns.Length; j++)
                        {
                            newTable.GetColumn(mainColumns[j]).SetPrimaryKey(false);
                            newTable.SetColumnTypeVars(mainColumns[j]);
                        }
                    }
                    this._database.schemaManager.RemoveSchemaObjects(dropConstraints);
                    this.SetNewTableInSchema(newTable);
                    this.SetNewTablesInSchema(tableSet);
                    this.UpdateConstraints(newTable, this.EmptySetQName);
                    this.UpdateConstraints(tableSet, dropConstraints);
                    this._database.schemaManager.RecompileDependentObjects(tableSet);
                    this._database.schemaManager.RecompileDependentObjects(newTable);
                    this._table = newTable;
                    return;
                }
                case 3:
                    this._database.schemaManager.RemoveSchemaObject(constraint.GetName());
                    if (constraint.IsNotNull())
                    {
                        this._table.GetColumn(constraint.NotNullColumnIndex).SetNullable(false);
                        this._table.SetColumnTypeVars(constraint.NotNullColumnIndex);
                    }
                    return;
            }
        }

        public void DropIndex(string indexName)
        {
            this.CheckModifyTable();
            Index index = this._table.GetIndex(indexName);
            if (this._table.IsIndexingMutable())
            {
                this._table.DropIndex(index.GetPosition());
            }
            else
            {
                OrderedHashSet<QNameManager.QName> dropIndexes = new OrderedHashSet<QNameManager.QName>();
                dropIndexes.Add(this._table.GetIndex(indexName).GetName());
                Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, null, null, null, -1, 0, this.EmptySetQName, dropIndexes);
                this.MoveData(this._table, newTable, -1, 0);
                this._table = newTable;
            }
            if (!index.IsConstraint())
            {
                this._database.schemaManager.RemoveSchemaObject(index.GetName());
            }
            this._database.schemaManager.RecompileDependentObjects(this._table);
        }

        public Table GetTable()
        {
            return this._table;
        }

        public void MakeNewTable(OrderedHashSet<QNameManager.QName> dropConstraintSet, OrderedHashSet<QNameManager.QName> dropIndexSet)
        {
            Table table = this._table.MoveDefinition(this._session, this._table.TableType, null, null, null, -1, 0, dropConstraintSet, dropIndexSet);
            if (table.IndexList.Length == this._table.IndexList.Length)
            {
                this._database.persistentStoreCollection.ReleaseStore(table);
            }
            else
            {
                this.MoveData(this._table, table, -1, 0);
                this._table = table;
            }
        }

        public OrderedHashSet<Table> MakeNewTables(OrderedHashSet<Table> tableSet, OrderedHashSet<QNameManager.QName> dropConstraintSet, OrderedHashSet<QNameManager.QName> dropIndexSet)
        {
            OrderedHashSet<Table> set = new OrderedHashSet<Table>();
            for (int i = 0; i < tableSet.Size(); i++)
            {
                Table table = tableSet.Get(i);
                TableWorks works = new TableWorks(this._session, table);
                works.MakeNewTable(dropConstraintSet, dropIndexSet);
                set.Add(works.GetTable());
            }
            return set;
        }

        public void MoveData(Table oldTable, Table newTable, int colIndex, int adjust)
        {
            if (oldTable.GetTableType() == 3)
            {
                Session[] allSessions = this._database.sessionManager.GetAllSessions();
                for (int i = 0; i < allSessions.Length; i++)
                {
                    allSessions[i].sessionData.persistentStoreCollection.MoveData(oldTable, newTable, colIndex, adjust);
                }
            }
            else
            {
                IPersistentStore other = this._database.persistentStoreCollection.GetStore(oldTable);
                this._database.persistentStoreCollection.GetStore(newTable).MoveData(this._session, other, colIndex, adjust);
                this._database.persistentStoreCollection.ReleaseStore(oldTable);
            }
        }

        public void RegisterConstraintNames(List<Constraint> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
            {
                Constraint constraint = constraints[i];
                if ((constraint.ConstType - 2) <= 2)
                {
                    this._database.schemaManager.AddSchemaObject(constraint);
                }
            }
        }

        public void RemoveColumnNotNullConstraints(int colIndex)
        {
            for (int i = this._table.ConstraintList.Length - 1; i >= 0; i--)
            {
                Constraint constraint = this._table.ConstraintList[i];
                if (constraint.IsNotNull() && (constraint.NotNullColumnIndex == colIndex))
                {
                    this._database.schemaManager.RemoveSchemaObject(constraint.GetName());
                }
            }
            this._table.GetColumn(colIndex).SetNullable(true);
            this._table.SetColumnTypeVars(colIndex);
        }

        public void RetypeColumn(ColumnSchema oldCol, ColumnSchema newCol)
        {
            bool flag = true;
            int typeCode = oldCol.GetDataType().TypeCode;
            int num2 = newCol.GetDataType().TypeCode;
            this.CheckModifyTable();
            if (!this._table.IsEmpty(this._session) && (typeCode != num2))
            {
                flag = newCol.GetDataType().CanConvertFrom(oldCol.GetDataType());
                switch (typeCode)
                {
                    case 0x457:
                    case 0x7d0:
                        flag = false;
                        break;
                }
            }
            if (!flag)
            {
                throw Error.GetError(0x15b9);
            }
            int columnIndex = this._table.GetColumnIndex(oldCol.GetName().Name);
            if ((newCol.IsIdentity() && this._table.HasIdentityColumn()) && (this._table.IdentityColumn != columnIndex))
            {
                throw Error.GetError(0x1595);
            }
            if (this._table.GetPrimaryKey().Length > 1)
            {
                newCol.SetPrimaryKey(oldCol.IsPrimaryKey());
                if (ArrayUtil.Find(this._table.GetPrimaryKey(), columnIndex) == -1)
                {
                }
            }
            else if (this._table.HasPrimaryKey())
            {
                if (!oldCol.IsPrimaryKey())
                {
                    if (newCol.IsPrimaryKey())
                    {
                        throw Error.GetError(0x159c);
                    }
                }
                else
                {
                    newCol.SetPrimaryKey(true);
                }
            }
            else if (newCol.IsPrimaryKey())
            {
                throw Error.GetError(0x159a);
            }
            if (((((num2 == typeCode) & (oldCol.IsNullable() == newCol.IsNullable())) & (oldCol.GetDataType().Scale == newCol.GetDataType().Scale)) & (oldCol.IsIdentity() == newCol.IsIdentity())) & ((oldCol.GetDataType().Precision == newCol.GetDataType().Precision) || ((oldCol.GetDataType().Precision < newCol.GetDataType().Precision) && ((typeCode == 12) || (typeCode == 0x3d)))))
            {
                oldCol.SetType(newCol);
                oldCol.SetDefaultExpression(newCol.GetDefaultExpression());
                if (newCol.IsIdentity())
                {
                    oldCol.SetIdentity(newCol.GetIdentitySequence());
                }
                this._table.SetColumnTypeVars(columnIndex);
                this._table.ResetDefaultsFlag();
            }
            else
            {
                this._database.schemaManager.CheckColumnIsReferenced(this._table.GetName(), this._table.GetColumn(columnIndex).GetName());
                this._table.CheckColumnInCheckConstraint(columnIndex);
                this._table.CheckColumnInFkConstraint(columnIndex);
                this.CheckConvertColDataType(oldCol, newCol);
                this.RetypeColumn(newCol, columnIndex);
            }
        }

        public void RetypeColumn(ColumnSchema column, int colIndex)
        {
            Table newTable = this._table.MoveDefinition(this._session, this._table.TableType, column, null, null, colIndex, 0, this.EmptySetQName, this.EmptySetQName);
            this.MoveData(this._table, newTable, colIndex, 0);
            this.UpdateConstraints(newTable, this.EmptySetQName);
            this.SetNewTableInSchema(newTable);
            this._database.schemaManager.RecompileDependentObjects(this._table);
            this._table = newTable;
        }

        public void SetColDefaultExpression(int colIndex, Expression def)
        {
            if (def == null)
            {
                this._table.CheckColumnInFkConstraint(colIndex, 4);
            }
            this._table.SetDefaultExpression(colIndex, def);
        }

        public void SetColNullability(ColumnSchema column, bool nullable)
        {
            int columnIndex = this._table.GetColumnIndex(column.GetName().Name);
            if (column.IsNullable() != nullable)
            {
                if (!nullable)
                {
                    Constraint c = new Constraint(this._database.NameManager.NewAutoName("CT", this._table.GetSchemaName(), this._table.GetName(), 5), null, 3) {
                        Check = new ExpressionLogical(column)
                    };
                    c.PrepareCheckConstraint(this._session, this._table, true);
                    column.SetNullable(false);
                    this._table.AddConstraint(c);
                    this._table.SetColumnTypeVars(columnIndex);
                    this._database.schemaManager.AddSchemaObject(c);
                }
                else
                {
                    if (column.IsPrimaryKey())
                    {
                        throw Error.GetError(0x1596);
                    }
                    this._table.CheckColumnInFkConstraint(columnIndex, 2);
                    this.RemoveColumnNotNullConstraints(columnIndex);
                }
            }
        }

        public void SetNewTableInSchema(Table newTable)
        {
            int tableIndex = this._database.schemaManager.GetTableIndex(newTable);
            if (tableIndex != -1)
            {
                this._database.schemaManager.SetTable(tableIndex, newTable);
            }
        }

        public void SetNewTablesInSchema(OrderedHashSet<Table> tableSet)
        {
            for (int i = 0; i < tableSet.Size(); i++)
            {
                Table newTable = tableSet.Get(i);
                this.SetNewTableInSchema(newTable);
            }
        }

        public bool SetTableType(Session session, int newType)
        {
            if ((this._table.GetTableType() != newType) && ((newType - 4) <= 1))
            {
                Table table;
                try
                {
                    table = this._table.MoveDefinition(session, newType, null, null, null, -1, 0, this.EmptySetQName, this.EmptySetQName);
                    this.MoveData(this._table, table, -1, 0);
                    this.UpdateConstraints(table, this.EmptySetQName);
                }
                catch (CoreException)
                {
                    return false;
                }
                this.SetNewTableInSchema(table);
                this._table = table;
                this._database.schemaManager.RecompileDependentObjects(this._table);
                return true;
            }
            return false;
        }

        public void UpdateConstraints(OrderedHashSet<Table> tableSet, OrderedHashSet<QNameManager.QName> dropConstraints)
        {
            for (int i = 0; i < tableSet.Size(); i++)
            {
                Table t = tableSet.Get(i);
                this.UpdateConstraints(t, dropConstraints);
            }
        }

        public void UpdateConstraints(Table t, OrderedHashSet<QNameManager.QName> dropConstraints)
        {
            for (int i = t.ConstraintList.Length - 1; i >= 0; i--)
            {
                Constraint constraint = t.ConstraintList[i];
                if (dropConstraints.Contains(constraint.GetName()))
                {
                    t.RemoveConstraint(i);
                }
                else if (constraint.GetConstraintType() == 0)
                {
                    this._database.schemaManager.GetUserTable(this._session, constraint.Core.MainTable.GetName()).GetConstraint(constraint.GetMainName().Name).Core = constraint.Core;
                }
                else if (constraint.GetConstraintType() == 1)
                {
                    this._database.schemaManager.GetUserTable(this._session, constraint.Core.RefTable.GetName()).GetConstraint(constraint.GetRefName().Name).Core = constraint.Core;
                }
            }
        }
    }
}

