namespace FwNs.Core.LC.cSchemas
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class SchemaManager
    {
        private readonly Database _database;
        private readonly MultiValueHashMap<QNameManager.QName, QNameManager.QName> _referenceMap = new MultiValueHashMap<QNameManager.QName, QNameManager.QName>();
        private readonly HashMappedList<string, Schema> _schemaMap = new HashMappedList<string, Schema>();
        private QNameManager.QName _defaultSchemaQName;
        private int _defaultTableType = 4;
        private long _schemaChangeTimestamp;
        private int[][] _tempIndexRoots;
        private Table dualTable;

        public SchemaManager(Database database)
        {
            this._database = database;
            this._defaultSchemaQName = SqlInvariants.InformationSchemaQname;
            Schema schema = new Schema(SqlInvariants.InformationSchemaQname, SqlInvariants.InformationSchemaQname.Owner);
            this._schemaMap.Put(schema.GetName().Name, schema);
            try
            {
                schema.TypeLookup.Add(SqlInvariants.CardinalNumber);
                schema.TypeLookup.Add(SqlInvariants.YesOrNo);
                schema.TypeLookup.Add(SqlInvariants.CharacterData);
                schema.TypeLookup.Add(SqlInvariants.SqlIdentifier);
                schema.TypeLookup.Add(SqlInvariants.TimeStamp);
                schema.CharsetLookup.Add(SqlInvariants.SqlText);
                schema.CharsetLookup.Add(SqlInvariants.SqlIdentifierCharset);
                schema.CharsetLookup.Add(SqlInvariants.SqlCharacter);
            }
            catch (CoreException)
            {
            }
        }

        private void AddReferences(ISchemaObject obj)
        {
            OrderedHashSet<QNameManager.QName> references = obj.GetReferences();
            if (references != null)
            {
                for (int i = 0; i < references.Size(); i++)
                {
                    QNameManager.QName key = references.Get(i);
                    QNameManager.QName name = obj.GetName();
                    Routine routine = obj as Routine;
                    if (routine != null)
                    {
                        name = routine.GetSpecificName();
                    }
                    this._referenceMap.Put(key, name);
                }
            }
        }

        public void AddSchemaObject(ISchemaObject obj)
        {
            QNameManager.QName name = obj.GetName();
            Schema schema = this._schemaMap.Get(name.schema.Name);
            SchemaObjectSet schemaObjectSet = GetSchemaObjectSet(schema, name.type);
            int type = name.type;
            if (type > 9)
            {
                if (((type - 0x10) <= 1) || (type == 0x1b))
                {
                    RoutineSchema schema2 = (RoutineSchema) schemaObjectSet.GetObject(name.Name);
                    Routine routine = obj as Routine;
                    if (schema2 == null)
                    {
                        schema2 = new RoutineSchema(name.type, name);
                        schema2.AddSpecificRoutine(this._database, routine);
                        schemaObjectSet.CheckAdd(name);
                        SchemaObjectSet set1 = GetSchemaObjectSet(schema, 0x18);
                        set1.CheckAdd(routine.GetSpecificName());
                        schemaObjectSet.Add(schema2);
                        set1.Add(obj);
                    }
                    else
                    {
                        SchemaObjectSet set2 = GetSchemaObjectSet(schema, 0x18);
                        QNameManager.QName specificName = routine.GetSpecificName();
                        if (specificName != null)
                        {
                            set2.CheckAdd(specificName);
                        }
                        schema2.AddSpecificRoutine(this._database, routine);
                        set2.Add(obj);
                    }
                    this.AddReferences(obj);
                    return;
                }
            }
            else if (type != 3)
            {
                if ((type == 9) && obj.GetReferences().IsEmpty())
                {
                    return;
                }
            }
            else
            {
                OrderedHashSet<QNameManager.QName> references = obj.GetReferences();
                for (int i = 0; i < references.Size(); i++)
                {
                    QNameManager.QName name3 = references.Get(i);
                    if (name3.type == 9)
                    {
                        Table table1 = (Table) obj;
                        ColumnSchema column = table1.GetColumn(table1.FindColumn(name3.Name));
                        this.AddSchemaObject(column);
                    }
                }
            }
            if (schemaObjectSet != null)
            {
                schemaObjectSet.Add(obj);
            }
            this.AddReferences(obj);
        }

        public Iterator<string> AllSchemaNameIterator()
        {
            return this._schemaMap.GetKeySet().GetIterator();
        }

        public void CheckColumnIsReferenced(QNameManager.QName tableName, QNameManager.QName name)
        {
            OrderedHashSet<QNameManager.QName> referencingObjectNames = this.GetReferencingObjectNames(tableName, name);
            if (!referencingObjectNames.IsEmpty())
            {
                QNameManager.QName name2 = referencingObjectNames.Get(0);
                throw Error.GetError(0x157e, name2.GetSchemaQualifiedStatementName());
            }
        }

        public void CheckObjectIsReferenced(QNameManager.QName name)
        {
            OrderedHashSet<QNameManager.QName> referencingObjectNames = this.GetReferencingObjectNames(name);
            QNameManager.QName name2 = null;
            for (int i = 0; i < referencingObjectNames.Size(); i++)
            {
                name2 = referencingObjectNames.Get(i);
                if (name2.Parent != name)
                {
                    break;
                }
                name2 = null;
            }
            if (name2 != null)
            {
                throw Error.GetError(0x157e, name2.GetSchemaQualifiedStatementName());
            }
        }

        public void CheckSchemaNameCanChange(QNameManager.QName name)
        {
            Iterator<QNameManager.QName> iterator = this._referenceMap.GetValues().GetIterator();
            QNameManager.QName name2 = null;
            while (iterator.HasNext())
            {
                name2 = iterator.Next();
                int type = name2.type;
                if (type <= 8)
                {
                    if ((type != 4) && (type != 8))
                    {
                        goto Label_0042;
                    }
                    goto Label_0046;
                }
                if ((((type - 0x10) <= 2) || (type == 0x18)) || (type == 0x1b))
                {
                    goto Label_0046;
                }
            Label_0042:
                name2 = null;
                continue;
            Label_0046:
                if (name2.schema == name)
                {
                    break;
                }
                goto Label_0042;
            }
            if (name2 != null)
            {
                throw Error.GetError(0x157e, name2.GetSchemaQualifiedStatementName());
            }
        }

        public void CheckSchemaObjectNotExists(QNameManager.QName name)
        {
            GetSchemaObjectSet(this._schemaMap.Get(name.schema.Name), name.type).CheckAdd(name);
        }

        public void ClearStructures()
        {
            Iterator<Schema> iterator = this._schemaMap.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                iterator.Next().ClearStructures();
            }
        }

        private void CreateDualTable()
        {
            this.dualTable = TableUtil.NewLookupTable(this._database, SqlInvariants.DualTableQname, 1, SqlInvariants.DualColumnQname, SqlType.SqlVarchar);
            object[] data = new object[] { "X" };
            this.dualTable.InsertSys(this._database.sessionManager.GetSysSession(), this.dualTable.Store, data);
            this.dualTable.SetDataReadOnly(true);
            Right right = new Right();
            right.Set(1, null);
            this._database.GetGranteeManager().GrantSystemToPublic(this.dualTable, right);
        }

        public void CreatePublicSchema()
        {
            Schema schema = new Schema(this._database.NameManager.NewQName(null, "PUBLIC", 2), this._database.GetGranteeManager().GetDBARole());
            this._defaultSchemaQName = schema.GetName();
            this._schemaMap.Put(schema.GetName().Name, schema);
        }

        public void CreateSchema(QNameManager.QName name, Grantee owner)
        {
            SqlInvariants.CheckSchemaNameNotSystem(name.Name);
            Schema schema = new Schema(name, owner);
            this._schemaMap.Add(name.Name, schema);
        }

        public Iterator<object> DatabaseObjectIterator(int type)
        {
            Iterator<Schema> iterator = this._schemaMap.GetValues().GetIterator();
            Iterator<object> iterator2 = new WrapperIterator<object>();
            while (iterator.HasNext())
            {
                Schema schema = iterator.Next();
                iterator2 = new WrapperIterator<object>(iterator2, schema.SchemaObjectIterator(type));
            }
            return iterator2;
        }

        public Iterator<object> DatabaseObjectIterator(string schemaName, int type)
        {
            return this._schemaMap.Get(schemaName).SchemaObjectIterator(type);
        }

        public void DropConstraint(Session session, QNameManager.QName name, bool cascade)
        {
            Table table = this.GetTable(session, name.Parent.Name, name.Parent.schema.Name);
            new TableWorks(session, table).DropConstraint(name.Name, cascade);
        }

        public void DropIndex(Session session, QNameManager.QName name)
        {
            Table table = this.GetTable(session, name.Parent.Name, name.Parent.schema.Name);
            new TableWorks(session, table).DropIndex(name.Name);
        }

        public void DropSchema(Session session, string name, bool cascade)
        {
            Schema schema = this._schemaMap.Get(name);
            if (schema == null)
            {
                throw Error.GetError(0x157d, name);
            }
            if (SqlInvariants.IsLobsSchemaName(name))
            {
                throw Error.GetError(0x157f, name);
            }
            if (!cascade && !schema.IsEmpty())
            {
                throw Error.GetError(0x1068);
            }
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            this.GetCascadingSchemaReferences(schema.GetName(), set);
            this.RemoveSchemaObjects(set);
            Iterator<object> iterator = schema.SchemaObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                foreach (Constraint constraint in table.GetFkConstraints())
                {
                    if (constraint.GetMain().GetSchemaName() != schema.GetName())
                    {
                        constraint.GetMain().RemoveConstraint(constraint.GetMainName().Name);
                    }
                }
                this.RemoveTable(session, table);
            }
            Iterator<object> iterator2 = schema.SchemaObjectIterator(7);
            while (iterator2.HasNext())
            {
                NumberSequence sequence = (NumberSequence) iterator2.Next();
                this._database.GetGranteeManager().RemoveDbObject(sequence.GetName());
            }
            schema.ClearStructures();
            this._schemaMap.Remove(name);
            if (this._defaultSchemaQName.Name.Equals(name))
            {
                schema = new Schema(this._database.NameManager.NewQName(name, false, 2), this._database.GetGranteeManager().GetDBARole());
                this._defaultSchemaQName = schema.GetName();
                this._schemaMap.Put(schema.GetName().Name, schema);
            }
            this._database.GetUserManager().RemoveSchemaReference(name);
            this._database.GetSessionManager().RemoveSchemaReference(schema);
        }

        public void DropSchemas(Session session, Grantee grantee, bool cascade)
        {
            List<Schema> schemas = this.GetSchemas(grantee);
            for (int i = 0; i < schemas.Count; i++)
            {
                Schema schema = schemas[i];
                this.DropSchema(session, schema.GetName().Name, cascade);
            }
        }

        public void DropTable(Session session, Table table, bool cascade)
        {
            Schema schema = this._schemaMap.Get(table.GetSchemaName().Name);
            int index = schema.TableList.GetIndex(table.GetName().Name);
            OrderedHashSet<Constraint> dependentExternalConstraints = table.GetDependentExternalConstraints();
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            this.GetCascadingReferencingObjectNames(table.GetName(), set);
            if (!cascade)
            {
                for (int k = 0; k < dependentExternalConstraints.Size(); k++)
                {
                    Constraint local1 = dependentExternalConstraints.Get(k);
                    QNameManager.QName name = local1.GetRef().GetName();
                    QNameManager.QName refName = local1.GetRefName();
                    if (local1.GetConstraintType() == 1)
                    {
                        object[] objArray1 = new object[] { refName.schema.Name, '.', name.Name, '.', refName.Name };
                        throw Error.GetError(0x159d, string.Concat(objArray1));
                    }
                }
                if (!set.IsEmpty())
                {
                    for (int m = 0; m < set.Size(); m++)
                    {
                        QNameManager.QName name3 = set.Get(m);
                        if (name3.Parent != table.GetName())
                        {
                            throw Error.GetError(0x157e, name3.GetSchemaQualifiedStatementName());
                        }
                    }
                }
            }
            OrderedHashSet<Table> tableSet = new OrderedHashSet<Table>();
            OrderedHashSet<QNameManager.QName> dropConstraintSet = new OrderedHashSet<QNameManager.QName>();
            OrderedHashSet<QNameManager.QName> dropIndexSet = new OrderedHashSet<QNameManager.QName>();
            OrderedHashSet<QNameManager.QName> references = table.GetReferences();
            ISchemaObject[] triggers = table.GetTriggers();
            for (int i = 0; i < triggers.Length; i++)
            {
                references.Add(triggers[i].GetName());
            }
            for (int j = 0; j < dependentExternalConstraints.Size(); j++)
            {
                Constraint constraint = dependentExternalConstraints.Get(j);
                Table main = constraint.GetMain();
                if (main != table)
                {
                    tableSet.Add(main);
                }
                main = constraint.GetRef();
                if (main != table)
                {
                    tableSet.Add(main);
                }
                dropConstraintSet.Add(constraint.GetMainName());
                dropConstraintSet.Add(constraint.GetRefName());
                dropIndexSet.Add(constraint.GetRefIndex().GetName());
            }
            TableWorks works1 = new TableWorks(session, table);
            tableSet = works1.MakeNewTables(tableSet, dropConstraintSet, dropIndexSet);
            works1.SetNewTablesInSchema(tableSet);
            works1.UpdateConstraints(tableSet, dropConstraintSet);
            this.RemoveSchemaObjects(set);
            this.RemoveSchemaObjects(references);
            this.RemoveReferencedObject(table.GetName());
            this.RemoveReferencingObject(table);
            schema.TableList.Remove(index);
            schema.IndexLookup.RemoveParent(table.GetName());
            schema.ConstraintLookup.RemoveParent(table.GetName());
            this.RemoveTable(session, table);
            this.RecompileDependentObjects(tableSet);
        }

        public void DropTableOrView(Session session, Table table, bool cascade)
        {
            session.Commit(false);
            if (table.IsView())
            {
                this.RemoveSchemaObject(table.GetName(), cascade);
            }
            else
            {
                this.DropTable(session, table, cascade);
            }
        }

        public Schema FindSchema(string name)
        {
            return this._schemaMap.Get(name);
        }

        public ISchemaObject FindSchemaObject(string name, string schemaName, int type)
        {
            Schema schema = this._schemaMap.Get(schemaName);
            if (schema == null)
            {
                return null;
            }
            switch (type)
            {
                case 3:
                case 4:
                    return schema.TableLookup.GetObject(name);

                case 5:
                {
                    QNameManager.QName name2 = schema.ConstraintLookup.GetName(name);
                    if (name2 != null)
                    {
                        Table table = (Table) schema.TableList.Get(name2.Parent.Name);
                        if (table == null)
                        {
                            return null;
                        }
                        return table.GetConstraint(name);
                    }
                    return null;
                }
                case 7:
                    return schema.SequenceLookup.GetObject(name);

                case 8:
                {
                    QNameManager.QName name3 = schema.IndexLookup.GetName(name);
                    if (name3 != null)
                    {
                        return ((Table) schema.TableList.Get(name3.Parent.Name)).GetTrigger(name);
                    }
                    return null;
                }
                case 12:
                case 13:
                    return schema.TypeLookup.GetObject(name);

                case 14:
                    if (!name.Equals("SQL_IDENTIFIER"))
                    {
                        if (name.Equals("SQL_TEXT"))
                        {
                            return SqlInvariants.SqlText;
                        }
                        if (name.Equals("LATIN1"))
                        {
                            return SqlInvariants.Latin1;
                        }
                        if (name.Equals("ASCII_GRAPHIC"))
                        {
                            return SqlInvariants.AsciiGraphic;
                        }
                        return schema.CharsetLookup.GetObject(name);
                    }
                    return SqlInvariants.SqlIdentifierCharset;

                case 15:
                    return schema.CollationLookup.GetObject(name);

                case 0x10:
                    return schema.FunctionLookup.GetObject(name);

                case 0x11:
                    return schema.ProcedureLookup.GetObject(name);

                case 0x12:
                    return ((schema.ProcedureLookup.GetObject(name) ?? schema.FunctionLookup.GetObject(name)) ?? schema.AggregateLookup.GetObject(name));

                case 20:
                {
                    QNameManager.QName name4 = schema.IndexLookup.GetName(name);
                    if (name4 != null)
                    {
                        return ((Table) schema.TableList.Get(name4.Parent.Name)).GetIndex(name);
                    }
                    return null;
                }
                case 0x18:
                    return schema.SpecificRoutineLookup.GetObject(name);

                case 0x1b:
                    return schema.AggregateLookup.GetObject(name);
            }
            throw Error.RuntimeError(0xc9, "SchemaManager");
        }

        public QNameManager.QName FindSchemaQName(string name)
        {
            Schema schema = this._schemaMap.Get(name);
            if (schema == null)
            {
                return null;
            }
            return schema.GetName();
        }

        public static Table FindSessionTable(Session session, string name)
        {
            return session.sessionContext.FindSessionTable(name);
        }

        public Table FindUserTable(Session session, string name, string schemaName)
        {
            Schema schema = this._schemaMap.Get(schemaName);
            if (schema == null)
            {
                return null;
            }
            int index = schema.TableList.GetIndex(name);
            if (index == -1)
            {
                return null;
            }
            return (Table) schema.TableList.Get(index);
        }

        public Iterator<string> FullSchemaNamesIterator()
        {
            return this._schemaMap.GetKeySet().GetIterator();
        }

        public List<Table> GetAllTables()
        {
            Iterator<string> iterator = this.AllSchemaNameIterator();
            List<Table> list = new List<Table>();
            while (iterator.HasNext())
            {
                string name = iterator.Next();
                if (!SqlInvariants.IsLobsSchemaName(name) && !SqlInvariants.IsSystemSchemaName(name))
                {
                    Iterator<object> iterator2 = this.GetTables(name).GetValues().GetIterator();
                    while (iterator2.HasNext())
                    {
                        list.Add((Table) iterator2.Next());
                    }
                }
            }
            return list;
        }

        public void GetCascadingReferencingObjectNames(QNameManager.QName obj, OrderedHashSet<QNameManager.QName> set)
        {
            OrderedHashSet<QNameManager.QName> set2 = new OrderedHashSet<QNameManager.QName>();
            Iterator<QNameManager.QName> iterator = this._referenceMap.Get(obj);
            while (iterator.HasNext())
            {
                QNameManager.QName key = iterator.Next();
                if (set.Add(key))
                {
                    set2.Add(key);
                }
            }
            for (int i = 0; i < set2.Size(); i++)
            {
                QNameManager.QName name2 = set2.Get(i);
                this.GetCascadingReferencingObjectNames(name2, set);
            }
        }

        public void GetCascadingSchemaReferences(QNameManager.QName schema, OrderedHashSet<QNameManager.QName> set)
        {
            Iterator<QNameManager.QName> iterator = this._referenceMap.GetKeySet().GetIterator();
            while (iterator.HasNext())
            {
                QNameManager.QName name = iterator.Next();
                if (name.schema == schema)
                {
                    this.GetCascadingReferencingObjectNames(name, set);
                }
            }
            for (int i = 0; i < set.Size(); i++)
            {
                if (set.Get(i).schema == schema)
                {
                    set.Remove(i);
                    i--;
                }
            }
        }

        public QNameManager.QName[] GetCatalogAndBaseTableNames()
        {
            OrderedHashSet<object> set = new OrderedHashSet<object>();
            List<Table> allTables = this.GetAllTables();
            for (int i = 0; i < allTables.Count; i++)
            {
                Table table = allTables[i];
                if (!table.IsTemp())
                {
                    set.Add(table.GetName());
                }
            }
            set.Add(this._database.GetCatalogName());
            QNameManager.QName[] a = new QNameManager.QName[set.Size()];
            set.ToArray(a);
            return a;
        }

        public string[] GetCommentsArray()
        {
            List<Table> allTables = this.GetAllTables();
            List<string> list2 = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < allTables.Count; i++)
            {
                Table table = allTables[i];
                if (table.GetTableType() != 1)
                {
                    int columnCount = table.GetColumnCount();
                    for (int j = 0; j < columnCount; j++)
                    {
                        ColumnSchema column = table.GetColumn(j);
                        if (column.GetName().Comment != null)
                        {
                            builder.Length = 0;
                            builder.Append("COMMENT").Append(' ').Append("ON");
                            builder.Append(' ').Append("COLUMN").Append(' ');
                            builder.Append(table.GetName().GetSchemaQualifiedStatementName());
                            builder.Append('.').Append(column.GetName().StatementName);
                            builder.Append(' ').Append("IS").Append(' ');
                            builder.Append(StringConverter.ToQuotedString(column.GetName().Comment, '\'', true));
                            list2.Add(builder.ToString());
                        }
                    }
                    if (table.GetName().Comment != null)
                    {
                        builder.Length = 0;
                        builder.Append("COMMENT").Append(' ').Append("ON");
                        builder.Append(' ').Append("TABLE").Append(' ');
                        builder.Append(table.GetName().GetSchemaQualifiedStatementName());
                        builder.Append(' ').Append("IS").Append(' ');
                        builder.Append(StringConverter.ToQuotedString(table.GetName().Comment, '\'', true));
                        list2.Add(builder.ToString());
                    }
                }
            }
            Iterator<object> iterator = this.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                ISchemaObject obj2 = (ISchemaObject) iterator.Next();
                if (obj2.GetName().Comment != null)
                {
                    builder.Length = 0;
                    builder.Append("COMMENT").Append(' ').Append("ON");
                    builder.Append(' ').Append("ROUTINE").Append(' ');
                    builder.Append(obj2.GetName().GetSchemaQualifiedStatementName());
                    builder.Append(' ').Append("IS").Append(' ');
                    builder.Append(StringConverter.ToQuotedString(obj2.GetName().Comment, '\'', true));
                    list2.Add(builder.ToString());
                }
            }
            return list2.ToArray();
        }

        public QNameManager.QName GetDefaultSchemaQName()
        {
            return this._defaultSchemaQName;
        }

        public int GetDefaultTableType()
        {
            return this._defaultTableType;
        }

        public SqlType GetDistinctType(string name, string schemaName, bool raise)
        {
            Schema schema = this._schemaMap.Get(schemaName);
            if (schema != null)
            {
                SqlType type = (SqlType) schema.TypeLookup.GetObject(name);
                if ((type != null) && type.IsDistinctType())
                {
                    return type;
                }
            }
            if (raise)
            {
                throw Error.GetError(0x157d, name);
            }
            return null;
        }

        public SqlType GetDomain(string name, string schemaName, bool raise)
        {
            Schema schema = this._schemaMap.Get(schemaName);
            if (schema != null)
            {
                SqlType type = (SqlType) schema.TypeLookup.GetObject(name);
                if ((type != null) && type.IsDomainType())
                {
                    return type;
                }
            }
            if (raise)
            {
                throw Error.GetError(0x157d, name);
            }
            return null;
        }

        public int[][] GetIndexRoots(Session session)
        {
            if (this._tempIndexRoots != null)
            {
                this._tempIndexRoots = null;
                return this._tempIndexRoots;
            }
            List<Table> allTables = this.GetAllTables();
            List<int[]> list2 = new List<int[]>();
            int num = 0;
            int count = allTables.Count;
            while (num < count)
            {
                Table table = allTables[num];
                if (table.GetTableType() == 5)
                {
                    int[] indexRootsArray = table.GetIndexRootsArray();
                    list2.Add(indexRootsArray);
                }
                else
                {
                    list2.Add(null);
                }
                num++;
            }
            return list2.ToArray();
        }

        public string[] GetIndexRootsSql()
        {
            Session sysSession = this._database.sessionManager.GetSysSession();
            int[][] indexRoots = this.GetIndexRoots(sysSession);
            List<Table> allTables = this.GetAllTables();
            List<string> list2 = new List<string>();
            for (int i = 0; i < indexRoots.Length; i++)
            {
                if (((indexRoots[i] != null) && (indexRoots[i].Length != 0)) && (indexRoots[i][0] != -1))
                {
                    string indexRootsSql = allTables[i].GetIndexRootsSql(indexRoots[i]);
                    list2.Add(indexRootsSql);
                }
            }
            return list2.ToArray();
        }

        public OrderedHashSet<QNameManager.QName> GetReferencingObjectNames(QNameManager.QName obj)
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            Iterator<QNameManager.QName> iterator = this._referenceMap.Get(obj);
            while (iterator.HasNext())
            {
                QNameManager.QName key = iterator.Next();
                set.Add(key);
            }
            return set;
        }

        public OrderedHashSet<QNameManager.QName> GetReferencingObjectNames(QNameManager.QName table, QNameManager.QName column)
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            Iterator<QNameManager.QName> iterator = this._referenceMap.Get(table);
            while (iterator.HasNext())
            {
                QNameManager.QName name = iterator.Next();
                if (this.GetSchemaObject(name).GetReferences().Contains(column))
                {
                    set.Add(name);
                }
            }
            return set;
        }

        public long GetSchemaChangeTimestamp()
        {
            return this._schemaChangeTimestamp;
        }

        public string GetSchemaName(string name)
        {
            return this.GetSchemaQName(name).Name;
        }

        public ISchemaObject GetSchemaObject(QNameManager.QName name)
        {
            Schema schema = this._schemaMap.Get(name.schema.Name);
            if (schema != null)
            {
                switch (name.type)
                {
                    case 3:
                    case 4:
                        return (ISchemaObject) schema.TableList.Get(name.Name);

                    case 5:
                        name = schema.ConstraintLookup.GetName(name.Name);
                        if (name != null)
                        {
                            QNameManager.QName parent = name.Parent;
                            return ((Table) schema.TableList.Get(parent.Name)).GetConstraint(name.Name);
                        }
                        return null;

                    case 6:
                        return null;

                    case 7:
                        return (ISchemaObject) schema.SequenceList.Get(name.Name);

                    case 8:
                        name = schema.TriggerLookup.GetName(name.Name);
                        if (name != null)
                        {
                            QNameManager.QName parent = name.Parent;
                            return ((Table) schema.TableList.Get(parent.Name)).GetTrigger(name.Name);
                        }
                        return null;

                    case 12:
                    case 13:
                        return schema.TypeLookup.GetObject(name.Name);

                    case 14:
                        return schema.CharsetLookup.GetObject(name.Name);

                    case 15:
                        return schema.CollationLookup.GetObject(name.Name);

                    case 0x10:
                        return schema.FunctionLookup.GetObject(name.Name);

                    case 0x11:
                        return schema.ProcedureLookup.GetObject(name.Name);

                    case 0x12:
                        return (schema.FunctionLookup.GetObject(name.Name) ?? schema.ProcedureLookup.GetObject(name.Name));

                    case 20:
                        name = schema.IndexLookup.GetName(name.Name);
                        if (name != null)
                        {
                            QNameManager.QName parent = name.Parent;
                            return ((Table) schema.TableList.Get(parent.Name)).GetIndex(name.Name);
                        }
                        return null;

                    case 0x18:
                        return schema.SpecificRoutineLookup.GetObject(name.Name);

                    case 0x1b:
                        return schema.FunctionLookup.GetObject(name.Name);
                }
            }
            return null;
        }

        public ISchemaObject GetSchemaObject(string name, string schemaName, int type)
        {
            ISchemaObject obj1 = this.FindSchemaObject(name, schemaName, type);
            if (obj1 == null)
            {
                throw Error.GetError(SchemaObjectSet.GetGetErrorCode(type), name);
            }
            return obj1;
        }

        public QNameManager.QName GetSchemaObjectName(QNameManager.QName schemaName, string name, int type, bool raise)
        {
            Schema schema = this._schemaMap.Get(schemaName.Name);
            if (schema != null)
            {
                SchemaObjectSet functionLookup;
                if (type == 0x12)
                {
                    functionLookup = schema.FunctionLookup;
                    if (schema.FunctionLookup.GetObject(name) == null)
                    {
                        functionLookup = schema.ProcedureLookup;
                    }
                }
                else
                {
                    functionLookup = GetSchemaObjectSet(schema, type);
                }
                if (raise)
                {
                    functionLookup.CheckExists(name);
                }
                return functionLookup.GetName(name);
            }
            if (raise)
            {
                throw Error.GetError(SchemaObjectSet.GetGetErrorCode(type));
            }
            return null;
        }

        private static SchemaObjectSet GetSchemaObjectSet(Schema schema, int type)
        {
            SchemaObjectSet set = null;
            switch (type)
            {
                case 3:
                case 4:
                    return schema.TableLookup;

                case 5:
                    return schema.ConstraintLookup;

                case 6:
                case 9:
                case 10:
                case 11:
                case 0x12:
                case 0x13:
                case 0x15:
                case 0x16:
                case 0x17:
                case 0x19:
                case 0x1a:
                    return set;

                case 7:
                    return schema.SequenceLookup;

                case 8:
                    return schema.TriggerLookup;

                case 12:
                case 13:
                    return schema.TypeLookup;

                case 14:
                    return schema.CharsetLookup;

                case 15:
                    return schema.CollationLookup;

                case 0x10:
                    return schema.FunctionLookup;

                case 0x11:
                    return schema.ProcedureLookup;

                case 20:
                    return schema.IndexLookup;

                case 0x18:
                    return schema.SpecificRoutineLookup;

                case 0x1b:
                    return schema.FunctionLookup;
            }
            return set;
        }

        public QNameManager.QName GetSchemaQName(string name)
        {
            if (name == null)
            {
                return this._defaultSchemaQName;
            }
            if ("INFORMATION_SCHEMA".Equals(name))
            {
                return SqlInvariants.InformationSchemaQname;
            }
            if ("MODULE".Equals(name))
            {
                return SqlInvariants.ModuleQname;
            }
            Schema local1 = this._schemaMap.Get(name);
            if (local1 == null)
            {
                throw Error.GetError(0x12f2, name);
            }
            return local1.GetName();
        }

        public List<Schema> GetSchemas(Grantee grantee)
        {
            List<Schema> list = new List<Schema>();
            Iterator<Schema> iterator = this._schemaMap.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                Schema item = iterator.Next();
                if (grantee.Equals(item.GetOwner()))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public NumberSequence GetSequence(string name, string schemaName, bool raise)
        {
            Schema schema = this._schemaMap.Get(schemaName);
            if (schema != null)
            {
                NumberSequence sequence = (NumberSequence) schema.SequenceList.Get(name);
                if (sequence != null)
                {
                    return sequence;
                }
            }
            if (raise)
            {
                throw Error.GetError(0x157d, name);
            }
            return null;
        }

        public string[] GetSqlArray()
        {
            OrderedHashSet<object> set3;
            OrderedHashSet<object> set4;
            OrderedHashSet<object> resolved = new OrderedHashSet<object>();
            OrderedHashSet<object> unresolved = new OrderedHashSet<object>();
            List<string> list = new List<string>();
            Iterator<Schema> iterator = this._schemaMap.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                Schema schema = iterator.Next();
                if (!SqlInvariants.IsSystemSchemaName(schema.GetName().Name) && !SqlInvariants.IsLobsSchemaName(schema.GetName().Name))
                {
                    list.Add(schema.GetSql());
                    schema.AddSimpleObjects(unresolved);
                }
            }
            do
            {
                Iterator<object> it = unresolved.GetIterator();
                if (!it.HasNext())
                {
                    break;
                }
                set3 = new OrderedHashSet<object>();
                SchemaObjectSet.AddAllSql(resolved, unresolved, list, it, set3);
                unresolved.RemoveAll(set3);
            }
            while (set3.Size() != 0);
            iterator = this._schemaMap.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                Schema schema2 = iterator.Next();
                if (!SqlInvariants.IsSystemSchemaName(schema2.GetName().Name) && !SqlInvariants.IsLobsSchemaName(schema2.GetName().Name))
                {
                    list.AddRange(schema2.GetSqlArray(resolved, unresolved));
                }
            }
            do
            {
                Iterator<object> it = unresolved.GetIterator();
                if (!it.HasNext())
                {
                    break;
                }
                set4 = new OrderedHashSet<object>();
                SchemaObjectSet.AddAllSql(resolved, unresolved, list, it, set4);
                unresolved.RemoveAll(set4);
            }
            while (set4.Size() != 0);
            iterator = this._schemaMap.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                Schema schema3 = iterator.Next();
                if (!SqlInvariants.IsSystemSchemaName(schema3.GetName().Name) && !SqlInvariants.IsLobsSchemaName(schema3.GetName().Name))
                {
                    string[] triggerSql = schema3.GetTriggerSql();
                    if (triggerSql.Length != 0)
                    {
                        list.Add(Schema.GetSetSchemaSql(schema3.GetName()));
                        list.AddRange(triggerSql);
                    }
                    list.AddRange(schema3.GetSequenceRestartSql());
                }
            }
            iterator = this._schemaMap.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                list.AddRange(iterator.Next().GetSequenceRestartSql());
            }
            if (this._defaultSchemaQName != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SET").Append(' ').Append("DATABASE");
                builder.Append(' ').Append("DEFAULT").Append(' ');
                builder.Append("INITIAL").Append(' ').Append("SCHEMA");
                builder.Append(' ').Append(this._defaultSchemaQName.StatementName);
                list.Add(builder.ToString());
            }
            return list.ToArray();
        }

        public static QNameManager.QName GetSqljSchemaHsqlName()
        {
            return SqlInvariants.SqljSchemaQname;
        }

        public Table GetTable(Session session, string name, string schema)
        {
            Table systemTable = null;
            if ("MODULE".Equals(schema))
            {
                systemTable = FindSessionTable(session, name);
                if (systemTable == null)
                {
                    throw Error.GetError(0x157d, name);
                }
            }
            if (schema == null)
            {
                systemTable = name.Equals("DUAL") ? this._database.DbInfo.GetSystemTable(session, name) : FindSessionTable(session, name);
            }
            if (systemTable == null)
            {
                schema = session.GetSchemaName(schema);
                systemTable = this.FindUserTable(session, name, schema);
            }
            if (((systemTable == null) && "INFORMATION_SCHEMA".Equals(schema)) && (this._database.DbInfo != null))
            {
                systemTable = this._database.DbInfo.GetSystemTable(session, name);
            }
            if (systemTable == null)
            {
                throw Error.GetError(0x157d, name);
            }
            return systemTable;
        }

        public int GetTableIndex(Table table)
        {
            Schema schema = this._schemaMap.Get(table.GetSchemaName().Name);
            if (schema == null)
            {
                return -1;
            }
            QNameManager.QName name = table.GetName();
            return schema.TableList.GetIndex(name.Name);
        }

        public string[] GetTablePropsSql(bool withHeader)
        {
            List<Table> allTables = this.GetAllTables();
            List<string> list2 = new List<string>();
            for (int i = 0; i < allTables.Count; i++)
            {
                string sqlForReadOnly = allTables[i].GetSqlForReadOnly();
                if (sqlForReadOnly != null)
                {
                    list2.Add(sqlForReadOnly);
                }
            }
            return list2.ToArray();
        }

        public HashMappedList<string, object> GetTables(string schema)
        {
            return this._schemaMap.Get(schema).TableList;
        }

        public QNameManager.QName GetUserSchemaQName(string name)
        {
            Schema local1 = this._schemaMap.Get(name);
            if (local1 == null)
            {
                throw Error.GetError(0x12f2, name);
            }
            if (local1.GetName() == SqlInvariants.InformationSchemaQname)
            {
                throw Error.GetError(0x12f2, name);
            }
            return local1.GetName();
        }

        public Table GetUserTable(Session session, QNameManager.QName name)
        {
            return this.GetUserTable(session, name.Name, name.schema.Name);
        }

        public Table GetUserTable(Session session, string name, string schema)
        {
            Table table1 = this.FindUserTable(session, name, schema);
            if (table1 == null)
            {
                throw Error.GetError(0x157d, name);
            }
            return table1;
        }

        public bool HasSchemas(Grantee grantee)
        {
            Iterator<Schema> iterator = this._schemaMap.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                Schema schema = iterator.Next();
                if (grantee.Equals(schema.GetOwner()))
                {
                    return true;
                }
            }
            return false;
        }

        public void RecompileDependentObjects(OrderedHashSet<Table> tableSet)
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            for (int i = 0; i < tableSet.Size(); i++)
            {
                Table table = tableSet.Get(i);
                set.AddAll(this.GetReferencingObjectNames(table.GetName()));
            }
            Session sysSession = this._database.sessionManager.GetSysSession();
            int index = 0;
            while (index < set.Size())
            {
                QNameManager.QName name = set.Get(index);
                int type = name.type;
                if (type <= 0x12)
                {
                    if (((type - 4) <= 2) || ((type - 0x10) <= 2))
                    {
                        goto Label_0084;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case 0x18:
                        case 0x1b:
                            goto Label_0084;
                    }
                }
            Label_007E:
                index++;
                continue;
            Label_0084:
                this.GetSchemaObject(name).Compile(sysSession, null);
                goto Label_007E;
            }
        }

        public void RecompileDependentObjects(Table table)
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            this.GetCascadingReferencingObjectNames(table.GetName(), set);
            Session sysSession = this._database.sessionManager.GetSysSession();
            int index = 0;
            while (index < set.Size())
            {
                QNameManager.QName name = set.Get(index);
                int type = name.type;
                if (type <= 0x12)
                {
                    if (((type - 4) <= 2) || ((type - 0x10) <= 2))
                    {
                        goto Label_0063;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case 0x18:
                        case 0x1b:
                            goto Label_0063;
                    }
                }
            Label_005D:
                index++;
                continue;
            Label_0063:
                this.GetSchemaObject(name).Compile(sysSession, null);
                goto Label_005D;
            }
            List<Table> allTables = this.GetAllTables();
            for (int i = 0; i < allTables.Count; i++)
            {
                allTables[i].VerifyConstraintsIntegrity();
            }
        }

        public void RemoveDependentObjects(QNameManager.QName name)
        {
            Schema local1 = this._schemaMap.Get(name.schema.Name);
            local1.IndexLookup.RemoveParent(name);
            local1.ConstraintLookup.RemoveParent(name);
            local1.TriggerLookup.RemoveParent(name);
        }

        public void RemoveExportedKeys(Table toDrop)
        {
            Schema schema = this._schemaMap.Get(toDrop.GetSchemaName().Name);
            for (int i = 0; i < schema.TableList.Size(); i++)
            {
                Table table = (Table) schema.TableList.Get(i);
                Constraint[] constraints = table.GetConstraints();
                for (int j = constraints.Length - 1; j >= 0; j--)
                {
                    Table table2 = constraints[j].GetRef();
                    if (toDrop == table2)
                    {
                        table.RemoveConstraint(j);
                    }
                }
            }
        }

        private void RemoveReferencedObject(QNameManager.QName referenced)
        {
            this._referenceMap.Remove(referenced);
        }

        private void RemoveReferencingObject(ISchemaObject obj)
        {
            OrderedHashSet<QNameManager.QName> references = obj.GetReferences();
            if (references != null)
            {
                for (int i = 0; i < references.Size(); i++)
                {
                    QNameManager.QName key = references.Get(i);
                    QNameManager.QName name = obj.GetName();
                    Routine routine = obj as Routine;
                    if (routine != null)
                    {
                        name = routine.GetSpecificName();
                    }
                    this._referenceMap.Remove(key, name);
                    if (name.Parent != null)
                    {
                        this._referenceMap.Remove(key, name.Parent);
                    }
                }
            }
        }

        public void RemoveSchemaObject(QNameManager.QName name)
        {
            Schema schema = this._schemaMap.Get(name.schema.Name);
            ISchemaObject constraint = null;
            SchemaObjectSet constraintLookup = null;
            switch (name.type)
            {
                case 3:
                case 4:
                    constraint = schema.TableLookup.GetObject(name.Name);
                    break;

                case 5:
                    constraintLookup = schema.ConstraintLookup;
                    if (name.Parent.type != 3)
                    {
                        if (name.Parent.type == 13)
                        {
                            SqlType type1 = (SqlType) schema.TypeLookup.GetObject(name.Parent.Name);
                            constraint = type1.userTypeModifier.GetConstraint(name.Name);
                            type1.userTypeModifier.RemoveConstraint(name.Name);
                        }
                    }
                    else
                    {
                        Table table1 = (Table) schema.TableList.Get(name.Parent.Name);
                        constraint = table1.GetConstraint(name.Name);
                        table1.RemoveConstraint(name.Name);
                    }
                    break;

                case 7:
                    constraint = schema.SequenceLookup.GetObject(name.Name);
                    break;

                case 8:
                {
                    constraintLookup = schema.TriggerLookup;
                    Table table = (Table) schema.TableList.Get(name.Parent.Name);
                    constraint = table.GetTrigger(name.Name);
                    if (constraint != null)
                    {
                        table.RemoveTrigger((TriggerDef) constraint);
                    }
                    break;
                }
                case 9:
                {
                    Table schemaObject = (Table) this.GetSchemaObject(name.Parent);
                    if (schemaObject != null)
                    {
                        constraint = schemaObject.GetColumn(schemaObject.GetColumnIndex(name.Name));
                    }
                    break;
                }
                case 12:
                case 13:
                    constraint = schema.TypeLookup.GetObject(name.Name);
                    break;

                case 14:
                    constraint = schema.CharsetLookup.GetObject(name.Name);
                    break;

                case 15:
                    constraint = schema.CollationLookup.GetObject(name.Name);
                    break;

                case 0x10:
                {
                    constraintLookup = schema.FunctionLookup;
                    Routine[] specificRoutines = (constraint = (RoutineSchema) constraintLookup.GetObject(name.Name)).GetSpecificRoutines();
                    for (int i = 0; i < specificRoutines.Length; i++)
                    {
                        this.RemoveSchemaObject(specificRoutines[i].GetSpecificName());
                    }
                    break;
                }
                case 0x11:
                {
                    constraintLookup = schema.ProcedureLookup;
                    Routine[] specificRoutines = (constraint = (RoutineSchema) constraintLookup.GetObject(name.Name)).GetSpecificRoutines();
                    for (int i = 0; i < specificRoutines.Length; i++)
                    {
                        this.RemoveSchemaObject(specificRoutines[i].GetSpecificName());
                    }
                    break;
                }
                case 20:
                    constraintLookup = schema.IndexLookup;
                    break;

                case 0x18:
                {
                    constraintLookup = schema.SpecificRoutineLookup;
                    Routine routine = (Routine) constraintLookup.GetObject(name.Name);
                    constraint = routine;
                    routine.routineSchema.RemoveSpecificRoutine(routine);
                    if (routine.routineSchema.GetSpecificRoutines().Length == 0)
                    {
                        this.RemoveSchemaObject(routine.GetName());
                    }
                    break;
                }
                case 0x1b:
                {
                    constraintLookup = schema.AggregateLookup;
                    Routine[] specificRoutines = (constraint = (RoutineSchema) constraintLookup.GetObject(name.Name)).GetSpecificRoutines();
                    for (int i = 0; i < specificRoutines.Length; i++)
                    {
                        this.RemoveSchemaObject(specificRoutines[i].GetSpecificName());
                    }
                    break;
                }
                default:
                    throw Error.RuntimeError(0xc9, "SchemaManager");
            }
            if (constraint != null)
            {
                this._database.GetGranteeManager().RemoveDbObject(name);
                this.RemoveReferencingObject(constraint);
            }
            if (constraintLookup != null)
            {
                constraintLookup.Remove(name.Name);
            }
            this.RemoveReferencedObject(name);
        }

        public void RemoveSchemaObject(QNameManager.QName name, bool cascade)
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            int type = name.type;
            switch (type)
            {
                case 3:
                case 4:
                case 7:
                case 12:
                case 14:
                case 15:
                    goto Label_00FC;

                case 5:
                case 6:
                case 8:
                case 9:
                case 10:
                case 11:
                    goto Label_0104;

                case 13:
                {
                    this.GetCascadingReferencingObjectNames(name, set);
                    OrderedHashSet<QNameManager.QName> referencingObjectNames = this.GetReferencingObjectNames(name);
                    Iterator<QNameManager.QName> iterator = referencingObjectNames.GetIterator();
                    while (iterator.HasNext())
                    {
                        if (iterator.Next().type == 9)
                        {
                            iterator.Remove();
                        }
                    }
                    if (!referencingObjectNames.IsEmpty())
                    {
                        QNameManager.QName name2 = referencingObjectNames.Get(0);
                        throw Error.GetError(0x157e, name2.GetSchemaQualifiedStatementName());
                    }
                    goto Label_0104;
                }
                case 0x10:
                case 0x11:
                case 0x12:
                    break;

                default:
                    if (type == 0x18)
                    {
                        goto Label_00FC;
                    }
                    if (type != 0x1b)
                    {
                        goto Label_0104;
                    }
                    break;
            }
            RoutineSchema schemaObject = (RoutineSchema) this.GetSchemaObject(name);
            if (schemaObject != null)
            {
                Routine[] specificRoutines = schemaObject.GetSpecificRoutines();
                for (int i = 0; i < specificRoutines.Length; i++)
                {
                    this.GetCascadingReferencingObjectNames(specificRoutines[i].GetSpecificName(), set);
                }
            }
            goto Label_0104;
        Label_00FC:
            this.GetCascadingReferencingObjectNames(name, set);
        Label_0104:
            if (set.IsEmpty())
            {
                this.RemoveSchemaObject(name);
            }
            else
            {
                if (!cascade)
                {
                    QNameManager.QName name3 = set.Get(0);
                    throw Error.GetError(0x157e, name3.GetSchemaQualifiedStatementName());
                }
                set.Add(name);
                this.RemoveSchemaObjects(set);
            }
        }

        public void RemoveSchemaObjects(OrderedHashSet<QNameManager.QName> set)
        {
            for (int i = 0; i < set.Size(); i++)
            {
                QNameManager.QName name = set.Get(i);
                this.RemoveSchemaObject(name);
            }
        }

        private void RemoveTable(Session session, Table table)
        {
            this._database.GetGranteeManager().RemoveDbObject(table.GetName());
            table.ReleaseTriggers();
            if (table.HasLobColumn())
            {
                IRowIterator rowIterator = table.GetRowIterator(session);
                while (rowIterator.HasNext())
                {
                    object[] rowData = rowIterator.GetNextRow().RowData;
                    session.sessionData.AdjustLobUsageCount(table, rowData, -1, true);
                }
            }
            this._database.persistentStoreCollection.ReleaseStore(table);
        }

        public void RenameSchema(QNameManager.QName name, QNameManager.QName newName)
        {
            Schema schema = this._schemaMap.Get(name.Name);
            if (schema == null)
            {
                throw Error.GetError(0x157d, name.Name);
            }
            if (this._schemaMap.Get(newName.Name) != null)
            {
                throw Error.GetError(0x1580, newName.Name);
            }
            SqlInvariants.CheckSchemaNameNotSystem(name.Name);
            SqlInvariants.CheckSchemaNameNotSystem(newName.Name);
            int index = this._schemaMap.GetIndex(name.Name);
            schema.GetName().Rename(newName);
            this._schemaMap.Set(index, newName.Name, schema);
        }

        public void RenameSchemaObject(QNameManager.QName name, QNameManager.QName newName)
        {
            if (name.schema != newName.schema)
            {
                throw Error.GetError(0x1581, newName.schema.Name);
            }
            this.CheckObjectIsReferenced(name);
            GetSchemaObjectSet(this._schemaMap.Get(name.schema.Name), name.type).Rename(name, newName);
        }

        public void ReplaceReferences(ISchemaObject oldObject, ISchemaObject newObject)
        {
            this.RemoveReferencingObject(oldObject);
            this.AddReferences(newObject);
        }

        public bool SchemaExists(string name)
        {
            if (!"INFORMATION_SCHEMA".Equals(name))
            {
                return this._schemaMap.ContainsKey(name);
            }
            return true;
        }

        public void SetDefaultSchemaQName(QNameManager.QName name)
        {
            this._defaultSchemaQName = name;
        }

        public void SetDefaultTableType(int type)
        {
            this._defaultTableType = type;
        }

        public void SetIndexRoots(int[][] roots)
        {
            List<Table> allTables = this._database.schemaManager.GetAllTables();
            int index = 0;
            int count = allTables.Count;
            while (index < count)
            {
                Table table = allTables[index];
                if (table.GetTableType() == 5)
                {
                    int[] numArray = roots[index];
                    if (roots != null)
                    {
                        table.SetIndexRoots(numArray);
                    }
                }
                index++;
            }
        }

        public void SetSchemaChangeTimestamp()
        {
            this._schemaChangeTimestamp = this._database.TxManager.GetGlobalChangeTimestamp();
        }

        public void SetTable(int index, Table table)
        {
            this._schemaMap.Get(table.GetSchemaName().Name).TableList.Set(index, table.GetName().Name, table);
        }

        public void SetTempIndexRoots(int[][] roots)
        {
            this._tempIndexRoots = roots;
        }

        public Grantee ToSchemaOwner(string name)
        {
            if (SqlInvariants.InformationSchemaQname.Name.Equals(name))
            {
                return SqlInvariants.InformationSchemaQname.Owner;
            }
            Schema schema = this._schemaMap.Get(name);
            if (schema != null)
            {
                return schema.GetOwner();
            }
            return null;
        }
    }
}

