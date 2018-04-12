namespace FwNs.Core.LC.cDbInfos
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cScriptIO;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public sealed class DatabaseInformationFull : DatabaseInformationMain
    {
        private static readonly object DatabaseInformationFull_Lock = new object();
        private static readonly HashMappedList<string, string> statementMap = GetStatementMap();

        public DatabaseInformationFull(Database db) : base(db)
        {
        }

        private Table ADMINISTRABLE_ROLE_AUTHORIZATIONS(Session session)
        {
            Table t = base.SysTables[0x15];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x15]);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x15].Name, false, 20);
                int[] columns = new int[3];
                columns[1] = 1;
                columns[2] = 2;
                t.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            if (session.IsAdmin())
            {
                this.InsertRoles(session, t, session.GetGrantee(), true);
            }
            return t;
        }

        private Table APPLICABLE_ROLES(Session session)
        {
            Table t = base.SysTables[0x16];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x16]);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x16].Name, false, 20);
                int[] columns = new int[3];
                columns[1] = 1;
                columns[2] = 2;
                t.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            this.InsertRoles(session, t, session.GetGrantee(), session.IsAdmin());
            return t;
        }

        private Table AUTHORIZATIONS(Session session)
        {
            Table t = base.SysTables[0x18];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x18]);
                DatabaseInformationMain.AddColumn(t, "AUTHORIZATION_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "AUTHORIZATION_TYPE", DatabaseInformationMain.SqlIdentifier);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x18].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            foreach (Grantee grantee in session.GetGrantee().VisibleGrantees())
            {
                object[] emptyRowData = t.GetEmptyRowData();
                emptyRowData[0] = grantee.GetNameString();
                emptyRowData[1] = grantee.IsRole ? "ROLE" : "USER";
                Table.InsertSys(rowStore, emptyRowData);
            }
            return t;
        }

        private Table CHARACTER_SETS(Session session)
        {
            Table t = base.SysTables[0x19];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x19]);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_REPERTOIRE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "FORM_OF_USE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DEFAULT_COLLATE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DEFAULT_COLLATE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DEFAULT_COLLATE_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x19].Name, false, 20);
                int[] columns = new int[3];
                columns[1] = 1;
                columns[2] = 2;
                t.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(14);
            while (iterator.HasNext())
            {
                Charset charset = (Charset) iterator.Next();
                if (session.GetGrantee().IsAccessible(charset))
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = charset.GetSchemaName().Name;
                    emptyRowData[num3] = charset.GetName().Name;
                    emptyRowData[num4] = "UCS";
                    emptyRowData[num5] = "UTF16";
                    emptyRowData[num6] = emptyRowData[index];
                    if (charset.BaseName == null)
                    {
                        emptyRowData[num7] = emptyRowData[num2];
                        emptyRowData[num8] = emptyRowData[num3];
                    }
                    else
                    {
                        emptyRowData[num7] = charset.BaseName.schema.Name;
                        emptyRowData[num8] = charset.BaseName.Name;
                    }
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table CHECK_CONSTRAINT_ROUTINE_USAGE(Session session)
        {
            Table t = base.SysTables[0x1a];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x1a]);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x1a].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(5);
            while (iterator.HasNext())
            {
                QNameManager.QName name2 = (QNameManager.QName) iterator.Next();
                if (name2.Parent != null)
                {
                    Table table3;
                    try
                    {
                        table3 = (Table) base.database.schemaManager.GetSchemaObject(name2.Parent.Name, name2.Parent.schema.Name, 3);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    Constraint constraint = table3.GetConstraint(name2.Name);
                    if (constraint.GetConstraintType() == 3)
                    {
                        OrderedHashSet<QNameManager.QName> references = constraint.GetReferences();
                        for (int i = 0; i < references.Size(); i++)
                        {
                            QNameManager.QName name3 = references.Get(i);
                            if (name3.type == 0x18)
                            {
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[index] = base.database.GetCatalogName().Name;
                                emptyRowData[num2] = constraint.GetSchemaName().Name;
                                emptyRowData[num3] = constraint.GetName().Name;
                                emptyRowData[num4] = base.database.GetCatalogName().Name;
                                emptyRowData[num5] = name3.schema.Name;
                                emptyRowData[num6] = name3.Name;
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table CHECK_CONSTRAINTS(Session session)
        {
            Table t = base.SysTables[0x1b];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x1b]);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHECK_CLAUSE", DatabaseInformationMain.CharacterData);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x1b].Name, false, 20);
                int[] columns = new int[3];
                columns[0] = 2;
                columns[1] = 1;
                table3.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table4 = (Table) iterator.Next();
                if (!table4.IsView() && session.GetGrantee().IsFullyAccessibleByRole(table4.GetName()))
                {
                    Constraint[] constraints = table4.GetConstraints();
                    int length = constraints.Length;
                    for (int i = 0; i < length; i++)
                    {
                        Constraint constraint = constraints[i];
                        if (constraint.GetConstraintType() == 3)
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = table4.GetSchemaName().Name;
                            emptyRowData[num3] = constraint.GetName().Name;
                            try
                            {
                                emptyRowData[num4] = constraint.GetCheckSql();
                            }
                            catch (Exception)
                            {
                            }
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                }
            }
            Iterator<object> iterator2 = base.database.schemaManager.DatabaseObjectIterator(13);
            while (iterator2.HasNext())
            {
                SqlType type = (SqlType) iterator2.Next();
                if (type.IsDomainType() && session.GetGrantee().IsFullyAccessibleByRole(type.GetName()))
                {
                    Constraint[] constraints = type.userTypeModifier.GetConstraints();
                    int length = constraints.Length;
                    for (int i = 0; i < length; i++)
                    {
                        Constraint constraint2 = constraints[i];
                        object[] emptyRowData = t.GetEmptyRowData();
                        emptyRowData[index] = base.database.GetCatalogName().Name;
                        emptyRowData[num2] = type.GetSchemaName().Name;
                        emptyRowData[num3] = constraint2.GetName().Name;
                        try
                        {
                            emptyRowData[num4] = constraint2.GetCheckSql();
                        }
                        catch (Exception)
                        {
                        }
                        Table.InsertSys(rowStore, emptyRowData);
                    }
                }
            }
            return t;
        }

        private Table COLLATIONS(Session session)
        {
            Table t = base.SysTables[0x1c];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x1c]);
                DatabaseInformationMain.AddColumn(t, "COLLATION_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PAD_ATTRIBUTE", DatabaseInformationMain.CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x1c].Name, false, 20);
                int[] columns = new int[3];
                columns[1] = 1;
                columns[2] = 2;
                t.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            string str = "PUBLIC";
            string str2 = "NO PAD";
            foreach (string str3 in Collation.NameToCSharpName.Keys)
            {
                object[] emptyRowData = t.GetEmptyRowData();
                emptyRowData[index] = base.database.GetCatalogName().Name;
                emptyRowData[num2] = str;
                emptyRowData[num3] = str3;
                emptyRowData[num4] = str2;
                Table.InsertSys(rowStore, emptyRowData);
            }
            return t;
        }

        private Table COLUMN_COLUMN_USAGE(Session session)
        {
            Table t = base.SysTables[0x1d];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x1d]);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DEPENDENT_COLUMN", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x1d].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4 }, false);
                return t;
            }
            return t;
        }

        private Table COLUMN_DOMAIN_USAGE(Session session)
        {
            Table t = base.SysTables[30];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[30]);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[30].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement("SELECT DOMAIN_CATALOG, DOMAIN_SCHEMA, DOMAIN_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE DOMAIN_NAME IS NOT NULL;", ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table COLUMN_UDT_USAGE(Session session)
        {
            Table t = base.SysTables[0x20];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x20]);
                DatabaseInformationMain.AddColumn(t, "UDT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x20].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement("SELECT UDT_CATALOG, UDT_SCHEMA, UDT_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE UDT_NAME IS NOT NULL;", ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table COLUMNS(Session session)
        {
            Table t = base.SysTables[0x21];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x21]);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ORDINAL_POSITION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "COLUMN_DEFAULT", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_NULLABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_MAXIMUM_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_OCTET_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION_RADIX", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DATETIME_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_CATALOG", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "MAXIMUM_CARDINALITY", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DTD_IDENTIFIER", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "IS_SELF_REFERENCING", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IS_IDENTITY", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IDENTITY_GENERATION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IDENTITY_START", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IDENTITY_INCREMENT", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IDENTITY_MAXIMUM", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IDENTITY_MINIMUM", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IDENTITY_CYCLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IS_GENERATED", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "GENERATION_EXPRESSION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_UPDATABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "DECLARED_DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x21].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 3, 2, 1, 4 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            int num10 = 9;
            int num11 = 10;
            int num12 = 11;
            int num13 = 12;
            int num14 = 13;
            int num15 = 14;
            int num16 = 15;
            int num17 = 0x10;
            int num18 = 0x11;
            int num19 = 0x12;
            int num20 = 0x13;
            int num21 = 20;
            int num22 = 0x15;
            int num23 = 0x16;
            int num24 = 0x17;
            int num25 = 0x18;
            int num26 = 0x19;
            int num27 = 0x1a;
            int num28 = 0x1b;
            int num29 = 0x1c;
            int num30 = 0x1d;
            int num31 = 30;
            int num32 = 0x1f;
            int num33 = 0x20;
            int num34 = 0x21;
            int num35 = 0x22;
            int num36 = 0x23;
            int num37 = 0x24;
            int num38 = 0x25;
            int num39 = 0x26;
            int num40 = 0x27;
            int num41 = 40;
            int num42 = 0x29;
            int num43 = 0x2a;
            int num44 = 0x2b;
            int num45 = 0x2c;
            int num46 = 0x2d;
            int num47 = 0x2e;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.AllTables();
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                OrderedHashSet<string> columnsForAllPrivileges = session.GetGrantee().GetColumnsForAllPrivileges(table);
                if (!columnsForAllPrivileges.IsEmpty())
                {
                    int columnCount = table.GetColumnCount();
                    for (int i = 0; i < columnCount; i++)
                    {
                        ColumnSchema column = table.GetColumn(i);
                        SqlType dataType = column.GetDataType();
                        if (columnsForAllPrivileges.Contains(column.GetName().Name))
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = table.GetSchemaName().Name;
                            emptyRowData[num3] = table.GetName().Name;
                            emptyRowData[num4] = column.GetName().Name;
                            emptyRowData[num5] = i + 1;
                            emptyRowData[num6] = column.GetDefaultSql();
                            emptyRowData[num7] = column.IsNullable() ? "YES" : "NO";
                            emptyRowData[num8] = dataType.GetFullNameString();
                            if (dataType.IsCharacterType())
                            {
                                CharacterType type2 = (CharacterType) dataType;
                                emptyRowData[num9] = dataType.Precision;
                                emptyRowData[num10] = dataType.Precision * 2L;
                                emptyRowData[num17] = base.database.GetCatalogName().Name;
                                emptyRowData[num18] = type2.GetCharacterSet().GetSchemaName().Name;
                                emptyRowData[num19] = type2.GetCharacterSet().GetName().Name;
                                emptyRowData[num20] = base.database.GetCatalogName().Name;
                                emptyRowData[num21] = type2.GetCollation().GetSchemaName().Name;
                                emptyRowData[num22] = type2.GetCollation().GetName().Name;
                            }
                            else if (dataType.IsNumberType())
                            {
                                NumberType type3 = (NumberType) dataType;
                                emptyRowData[num11] = type3.GetNumericPrecisionInRadix();
                                emptyRowData[num46] = type3.GetNumericPrecisionInRadix();
                                if (dataType.IsExactNumberType())
                                {
                                    emptyRowData[num13] = emptyRowData[num47] = dataType.GetAdoScale();
                                }
                                emptyRowData[num12] = dataType.GetPrecisionRadix();
                            }
                            else if (!dataType.IsBooleanType())
                            {
                                if (dataType.IsDateTimeType())
                                {
                                    emptyRowData[num14] = dataType.GetAdoScale();
                                }
                                else if (dataType.IsIntervalType())
                                {
                                    emptyRowData[num8] = "INTERVAL";
                                    emptyRowData[num15] = IntervalType.GetQualifier(dataType.TypeCode);
                                    emptyRowData[num16] = dataType.Precision;
                                    emptyRowData[num14] = dataType.GetAdoScale();
                                }
                                else if (dataType.IsBinaryType())
                                {
                                    emptyRowData[num9] = dataType.Precision;
                                    emptyRowData[num10] = dataType.Precision;
                                }
                            }
                            if (dataType.IsDomainType())
                            {
                                emptyRowData[num23] = base.database.GetCatalogName().Name;
                                emptyRowData[num24] = dataType.GetSchemaName().Name;
                                emptyRowData[num25] = dataType.GetName().Name;
                            }
                            if (dataType.IsDistinctType())
                            {
                                emptyRowData[num26] = base.database.GetCatalogName().Name;
                                emptyRowData[num27] = dataType.GetSchemaName().Name;
                                emptyRowData[num28] = dataType.GetName().Name;
                            }
                            emptyRowData[num29] = null;
                            emptyRowData[num30] = null;
                            emptyRowData[num31] = null;
                            emptyRowData[num32] = null;
                            emptyRowData[num33] = null;
                            emptyRowData[num34] = null;
                            emptyRowData[num35] = column.IsIdentity() ? "YES" : "NO";
                            if (column.IsIdentity())
                            {
                                NumberSequence identitySequence = column.GetIdentitySequence();
                                emptyRowData[num36] = identitySequence.IsAlways() ? "ALWAYS" : "BY DEFAULT";
                                int num50 = num37;
                                emptyRowData[num50] = identitySequence.GetStartValue().ToString();
                                int num51 = num38;
                                emptyRowData[num51] = identitySequence.GetIncrement().ToString();
                                int num52 = num39;
                                emptyRowData[num52] = identitySequence.GetMaxValue().ToString();
                                int num53 = num40;
                                emptyRowData[num53] = identitySequence.GetMinValue().ToString();
                                emptyRowData[num41] = identitySequence.IsCycle() ? "YES" : "NO";
                            }
                            emptyRowData[num42] = "NEVER";
                            emptyRowData[num43] = null;
                            emptyRowData[num44] = table.IsWritable() ? "YES" : "NO";
                            emptyRowData[num45] = emptyRowData[num8];
                            if (dataType.IsNumberType())
                            {
                                emptyRowData[num46] = emptyRowData[num11];
                                emptyRowData[num47] = emptyRowData[num13];
                            }
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                }
            }
            return t;
        }

        private Table CONSTRAINT_COLUMN_USAGE(Session session)
        {
            Table t = base.SysTables[0x22];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x22]);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x22].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table3 = (Table) iterator.Next();
                if (!table3.IsView() && session.GetGrantee().IsFullyAccessibleByRole(table3.GetName()))
                {
                    Constraint[] constraints = table3.GetConstraints();
                    int length = constraints.Length;
                    string name = base.database.GetCatalogName().Name;
                    string str2 = table3.GetSchemaName().Name;
                    for (int i = 0; i < length; i++)
                    {
                        Iterator<Expression> iterator2;
                        QNameManager.QName name2;
                        Constraint constraint = constraints[i];
                        string str3 = constraint.GetName().Name;
                        switch (constraint.GetConstraintType())
                        {
                            case 0:
                            case 2:
                            case 4:
                            {
                                Table main = table3;
                                int[] mainColumns = constraint.GetMainColumns();
                                if (constraint.GetConstraintType() == 0)
                                {
                                    main = constraint.GetMain();
                                }
                                for (int j = 0; j < mainColumns.Length; j++)
                                {
                                    object[] emptyRowData = t.GetEmptyRowData();
                                    emptyRowData[index] = base.database.GetCatalogName().Name;
                                    emptyRowData[num2] = str2;
                                    emptyRowData[num3] = main.GetName().Name;
                                    emptyRowData[num4] = main.GetColumn(mainColumns[j]).GetName().Name;
                                    emptyRowData[num5] = name;
                                    emptyRowData[num6] = str2;
                                    emptyRowData[num7] = str3;
                                    try
                                    {
                                        Table.InsertSys(rowStore, emptyRowData);
                                    }
                                    catch (CoreException)
                                    {
                                    }
                                }
                                continue;
                            }
                            case 1:
                            {
                                continue;
                            }
                            case 3:
                            {
                                OrderedHashSet<Expression> checkColumnExpressions = constraint.GetCheckColumnExpressions();
                                if (checkColumnExpressions == null)
                                {
                                    continue;
                                }
                                iterator2 = checkColumnExpressions.GetIterator();
                                goto Label_02F7;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                    Label_026C:
                        name2 = ((ExpressionColumn) iterator2.Next()).GetBaseColumnQName();
                        if (name2.type == 9)
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = name2.schema.Name;
                            emptyRowData[num3] = name2.Parent.Name;
                            emptyRowData[num4] = name2.Name;
                            emptyRowData[num5] = name;
                            emptyRowData[num6] = str2;
                            emptyRowData[num7] = str3;
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                            catch (CoreException)
                            {
                            }
                        }
                    Label_02F7:
                        if (iterator2.HasNext())
                        {
                            goto Label_026C;
                        }
                    }
                }
            }
            return t;
        }

        private Table CONSTRAINT_TABLE_USAGE(Session session)
        {
            Table t = base.SysTables[0x23];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x23]);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x23].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement("select DISTINCT CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE", ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table DATA_TYPE_PRIVILEGES(Session session)
        {
            Table t = base.SysTables[0x24];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x24]);
                DatabaseInformationMain.AddColumn(t, "OBJECT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_TYPE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DTD_IDENTIFIER", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x24].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            string sql = statementMap.Get("/*data_type_privileges*/");
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement(sql, ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table DOMAIN_CONSTRAINTS(Session session)
        {
            Table t = base.SysTables[0x25];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x25]);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "IS_DEFERRABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "INITIALLY_DEFERRED", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x25].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 4, 5, 6 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(13);
            while (iterator.HasNext())
            {
                SqlType type = (SqlType) iterator.Next();
                if (type.IsDomainType() && session.GetGrantee().IsFullyAccessibleByRole(type.GetName()))
                {
                    Constraint[] constraints = type.userTypeModifier.GetConstraints();
                    for (int i = 0; i < constraints.Length; i++)
                    {
                        object[] emptyRowData = t.GetEmptyRowData();
                        emptyRowData[index] = emptyRowData[num4] = base.database.GetCatalogName().Name;
                        emptyRowData[num2] = emptyRowData[num5] = type.GetSchemaName().Name;
                        emptyRowData[num3] = constraints[i].GetName().Name;
                        emptyRowData[num6] = type.GetName().Name;
                        emptyRowData[num7] = "NO";
                        emptyRowData[num8] = "NO";
                        Table.InsertSys(rowStore, emptyRowData);
                    }
                }
            }
            return t;
        }

        private Table DOMAINS(Session session)
        {
            Table t = base.SysTables[0x26];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x26]);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DATA_TYPE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_MAXIMUM_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_OCTET_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION_RADIX", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DATETIME_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DOMAIN_DEFAULT", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "MAXIMUM_CARDINALITY", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DTD_IDENTIFIER", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DECLARED_DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_SCLAE", DatabaseInformationMain.CardinalNumber);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x26].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 4, 5, 6 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            int num10 = 9;
            int num11 = 10;
            int num12 = 11;
            int num13 = 12;
            int num14 = 13;
            int num15 = 14;
            int num16 = 15;
            int num17 = 0x10;
            int num18 = 0x11;
            int num19 = 0x12;
            int num20 = 0x16;
            int num21 = 0x17;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(13);
            while (iterator.HasNext())
            {
                SqlType type = (SqlType) iterator.Next();
                if (type.IsDomainType() && session.GetGrantee().IsAccessible(type))
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = type.GetSchemaName().Name;
                    emptyRowData[num3] = type.GetName().Name;
                    emptyRowData[num4] = type.GetFullNameString();
                    if (type.IsCharacterType())
                    {
                        CharacterType type2 = (CharacterType) type;
                        emptyRowData[num5] = type.Precision;
                        emptyRowData[num6] = type.Precision * 2L;
                        emptyRowData[num7] = base.database.GetCatalogName().Name;
                        emptyRowData[num8] = type2.GetCharacterSet().GetSchemaName().Name;
                        emptyRowData[num9] = type2.GetCharacterSet().GetName().Name;
                        emptyRowData[num10] = base.database.GetCatalogName().Name;
                        emptyRowData[num11] = type2.GetCollation().GetSchemaName().Name;
                        emptyRowData[num12] = type2.GetCollation().GetName().Name;
                    }
                    else if (type.IsNumberType())
                    {
                        NumberType type3 = (NumberType) type;
                        emptyRowData[num13] = type3.GetNumericPrecisionInRadix();
                        emptyRowData[num20] = type3.GetNumericPrecisionInRadix();
                        if (type.IsExactNumberType())
                        {
                            emptyRowData[num15] = emptyRowData[num21] = type.GetAdoScale();
                        }
                        emptyRowData[num14] = type.GetPrecisionRadix();
                    }
                    else if (!type.IsBooleanType())
                    {
                        if (type.IsDateTimeType())
                        {
                            emptyRowData[num16] = type.GetAdoScale();
                        }
                        else if (type.IsIntervalType())
                        {
                            emptyRowData[num4] = "INTERVAL";
                            emptyRowData[num17] = IntervalType.GetQualifier(type.TypeCode);
                            emptyRowData[num18] = type.Precision;
                            emptyRowData[num16] = type.GetAdoScale();
                        }
                        else if (type.IsBinaryType())
                        {
                            emptyRowData[num5] = type.Precision;
                            emptyRowData[num6] = type.Precision;
                        }
                    }
                    Expression defaultClause = type.userTypeModifier.GetDefaultClause();
                    if (defaultClause != null)
                    {
                        emptyRowData[num19] = defaultClause.GetSql();
                    }
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table ENABLED_ROLES(Session session)
        {
            Table t = base.SysTables[0x27];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x27]);
                DatabaseInformationMain.AddColumn(t, "ROLE_NAME", DatabaseInformationMain.SqlIdentifier);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x27].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<Grantee> iterator = session.GetGrantee().GetAllRoles().GetIterator();
            while (iterator.HasNext())
            {
                Grantee grantee = iterator.Next();
                object[] emptyRowData = t.GetEmptyRowData();
                emptyRowData[0] = grantee.GetNameString();
                Table.InsertSys(rowStore, emptyRowData);
            }
            return t;
        }

        protected override Table GenerateTable(Session session, int tableIndex)
        {
            switch (tableIndex)
            {
                case 5:
                    return this.SYSTEM_PROCEDURECOLUMNS(session);

                case 6:
                    return this.SYSTEM_PROCEDURES(session);

                case 11:
                    return this.SYSTEM_UDTS(session);

                case 15:
                    return this.SYSTEM_CACHEINFO(session);

                case 0x11:
                    return this.SYSTEM_PROPERTIES(session);

                case 0x12:
                    return this.SYSTEM_SESSIONINFO(session);

                case 0x13:
                    return this.SYSTEM_SESSIONS(session);

                case 0x15:
                    return this.ADMINISTRABLE_ROLE_AUTHORIZATIONS(session);

                case 0x16:
                    return this.APPLICABLE_ROLES(session);

                case 0x18:
                    return this.AUTHORIZATIONS(session);

                case 0x19:
                    return this.CHARACTER_SETS(session);

                case 0x1a:
                    return this.CHECK_CONSTRAINT_ROUTINE_USAGE(session);

                case 0x1b:
                    return this.CHECK_CONSTRAINTS(session);

                case 0x1c:
                    return this.COLLATIONS(session);

                case 0x1d:
                    return this.COLUMN_COLUMN_USAGE(session);

                case 30:
                    return this.COLUMN_DOMAIN_USAGE(session);

                case 0x20:
                    return this.COLUMN_UDT_USAGE(session);

                case 0x21:
                    return this.COLUMNS(session);

                case 0x22:
                    return this.CONSTRAINT_COLUMN_USAGE(session);

                case 0x23:
                    return this.CONSTRAINT_TABLE_USAGE(session);

                case 0x24:
                    return this.DATA_TYPE_PRIVILEGES(session);

                case 0x25:
                    return this.DOMAIN_CONSTRAINTS(session);

                case 0x26:
                    return this.DOMAINS(session);

                case 0x27:
                    return this.ENABLED_ROLES(session);

                case 0x2b:
                    return this.KEY_COLUMN_USAGE(session);

                case 0x31:
                    return this.PARAMETERS(session);

                case 50:
                    return this.REFERENTIAL_CONSTRAINTS(session);

                case 0x33:
                    return this.ROLE_AUTHORIZATION_DESCRIPTORS(session);

                case 0x34:
                    return this.ROLE_COLUMN_GRANTS(session);

                case 0x36:
                    return this.ROLE_ROUTINE_GRANTS(session);

                case 0x37:
                    return this.ROLE_TABLE_GRANTS(session);

                case 0x38:
                    return this.ROLE_UDT_GRANTS(session);

                case 0x39:
                    return this.ROLE_USAGE_GRANTS(session);

                case 0x3a:
                    return this.ROUTINE_COLUMN_USAGE(session);

                case 0x3b:
                    return this.ROUTINE_ASSEMBLY_USAGE(session);

                case 60:
                    return this.ROUTINE_PRIVILEGES(session);

                case 0x3d:
                    return this.ROUTINE_ROUTINE_USAGE(session);

                case 0x3e:
                    return this.ROUTINE_SEQUENCE_USAGE(session);

                case 0x3f:
                    return this.ROUTINE_TABLE_USAGE(session);

                case 0x40:
                    return this.ROUTINES(session);

                case 0x41:
                    return this.SCHEMATA(session);

                case 0x42:
                    return base.SEQUENCES(session);

                case 0x49:
                    return this.TABLE_CONSTRAINTS(session);

                case 0x4b:
                    return base.TABLES(session);

                case 0x4d:
                    return this.TRIGGER_COLUMN_USAGE(session);

                case 0x4e:
                    return this.TRIGGER_ROUTINE_USAGE(session);

                case 0x4f:
                    return this.TRIGGER_SEQUENCE_USAGE(session);

                case 80:
                    return this.TRIGGER_TABLE_USAGE(session);

                case 0x51:
                    return this.TRIGGERED_UPDATE_COLUMNS(session);

                case 0x52:
                    return this.TRIGGERS(session);

                case 0x54:
                    return this.UDT_PRIVILEGES(session);

                case 0x55:
                    return this.USAGE_PRIVILEGES(session);

                case 0x56:
                    return this.USER_DEFINED_TYPES(session);

                case 0x57:
                    return this.VIEW_COLUMN_USAGE(session);

                case 0x58:
                    return this.VIEW_ROUTINE_USAGE(session);

                case 0x59:
                    return this.VIEW_TABLE_USAGE(session);

                case 90:
                    return this.VIEWS(session);

                case 0x5d:
                    return this.SYSTEM_COMMENTS(session);
            }
            return base.GenerateTable(session, tableIndex);
        }

        private static HashMappedList<string, string> GetStatementMap()
        {
            lock (DatabaseInformationFull_Lock)
            {
                string name = "LibCore.Resources.information-schema.sql";
                string[] sectionStarts = new string[] { "/*" };
                Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(manifestResourceStream, Encoding.UTF8);
                }
                catch (Exception)
                {
                }
                LineGroupReader reader1 = new LineGroupReader(reader, sectionStarts);
                HashMappedList<string, string> asMap = reader1.GetAsMap();
                reader1.Close();
                return asMap;
            }
        }

        private void InsertRoles(Session session, Table t, Grantee role, bool isGrantable)
        {
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            if (isGrantable)
            {
                Iterator<string> iterator = base.database.GetGranteeManager().GetRoleNames().GetIterator();
                while (iterator.HasNext())
                {
                    string str = iterator.Next();
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = role.GetNameString();
                    emptyRowData[num2] = str;
                    emptyRowData[num3] = "YES";
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            else
            {
                OrderedHashSet<Grantee> directRoles = role.GetDirectRoles();
                for (int i = 0; i < directRoles.Size(); i++)
                {
                    string nameString = directRoles.Get(i).GetNameString();
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = role.GetNameString();
                    emptyRowData[num2] = nameString;
                    emptyRowData[num3] = "NO";
                    Table.InsertSys(rowStore, emptyRowData);
                    role = base.database.GetGranteeManager().GetRole(nameString);
                    this.InsertRoles(session, t, role, isGrantable);
                }
            }
        }

        private Table KEY_COLUMN_USAGE(Session session)
        {
            Table t = base.SysTables[0x2b];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x2b]);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ORDINAL_POSITION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "POSITION_IN_UNIQUE_CONSTRAINT", DatabaseInformationMain.CardinalNumber);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x2b].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 2, 1, 0, 6, 7 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                string name = base.database.GetCatalogName().Name;
                string str2 = table.GetSchemaName().Name;
                string str3 = table.GetName().Name;
                if (!table.IsView() && DatabaseInformationMain.IsAccessibleTable(session, table))
                {
                    foreach (Constraint constraint in table.GetConstraints())
                    {
                        if (((constraint.GetConstraintType() == 4) || (constraint.GetConstraintType() == 2)) || (constraint.GetConstraintType() == 0))
                        {
                            string str4 = constraint.GetName().Name;
                            int[] mainColumns = constraint.GetMainColumns();
                            int[] numArray2 = null;
                            if (constraint.GetConstraintType() == 0)
                            {
                                int[] array = constraint.GetMain().GetConstraint(constraint.GetUniqueName().Name).GetMainColumns();
                                numArray2 = new int[mainColumns.Length];
                                for (int j = 0; j < mainColumns.Length; j++)
                                {
                                    numArray2[j] = ArrayUtil.Find(array, mainColumns[j]);
                                }
                                mainColumns = constraint.GetRefColumns();
                            }
                            for (int i = 0; i < mainColumns.Length; i++)
                            {
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[index] = name;
                                emptyRowData[num2] = str2;
                                emptyRowData[num3] = str4;
                                emptyRowData[num4] = name;
                                emptyRowData[num5] = str2;
                                emptyRowData[num6] = str3;
                                emptyRowData[num7] = table.GetColumn(mainColumns[i]).GetName().Name;
                                emptyRowData[num8] = i + 1;
                                if (constraint.GetConstraintType() == 0)
                                {
                                    emptyRowData[num9] = numArray2[i] + 1;
                                }
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table PARAMETERS(Session session)
        {
            Table t = base.SysTables[0x31];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x31]);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ORDINAL_POSITION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "PARAMETER_MODE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_RESULT", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "AS_LOCATOR", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "PARAMETER_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "FROM_SQL_SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "FROM_SQL_SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "FROM_SQL_SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TO_SQL_SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TO_SQL_SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TO_SQL_SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_MAXIMUM_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_OCTET_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_CATALOG", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION_RADIX", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DATETIME_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "UDT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "MAXIMUM_CARDINALITY", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DTD_IDENTIFIER", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DECLARED_DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x31].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 14;
            int num10 = 15;
            int num11 = 0x10;
            int num12 = 0x11;
            int num13 = 0x12;
            int num14 = 0x13;
            int num15 = 20;
            int num16 = 0x15;
            int num17 = 0x16;
            int num18 = 0x17;
            int num19 = 0x18;
            int num20 = 0x1a;
            int num21 = 0x1b;
            int num22 = 0x1c;
            int num23 = 0x1d;
            int num24 = 30;
            int num25 = 0x1f;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                if (session.GetGrantee().IsAccessible(schema))
                {
                    foreach (Routine routine in schema.GetSpecificRoutines())
                    {
                        int parameterCount = routine.GetParameterCount();
                        for (int i = 0; i < parameterCount; i++)
                        {
                            ColumnSchema parameter = routine.GetParameter(i);
                            SqlType dataType = parameter.GetDataType();
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = routine.GetSchemaName().Name;
                            emptyRowData[num3] = routine.GetSpecificName().Name;
                            emptyRowData[num8] = parameter.GetName().Name;
                            emptyRowData[num4] = i + 1;
                            switch (parameter.GetParameterMode())
                            {
                                case 1:
                                    emptyRowData[num5] = "IN";
                                    break;

                                case 2:
                                    emptyRowData[num5] = "INOUT";
                                    break;

                                case 4:
                                    emptyRowData[num5] = "OUT";
                                    break;
                            }
                            emptyRowData[num6] = "NO";
                            emptyRowData[num7] = "NO";
                            emptyRowData[num9] = dataType.GetFullNameString();
                            if (dataType.IsCharacterType())
                            {
                                CharacterType type2 = (CharacterType) dataType;
                                emptyRowData[num10] = dataType.Precision;
                                emptyRowData[num11] = dataType.Precision * 2L;
                                emptyRowData[num12] = base.database.GetCatalogName().Name;
                                emptyRowData[num13] = type2.GetCharacterSet().GetSchemaName().Name;
                                emptyRowData[num14] = type2.GetCharacterSet().GetName().Name;
                                emptyRowData[num15] = base.database.GetCatalogName().Name;
                                emptyRowData[num16] = type2.GetCollation().GetSchemaName().Name;
                                emptyRowData[num17] = type2.GetCollation().GetName().Name;
                            }
                            else if (dataType.IsNumberType())
                            {
                                emptyRowData[num18] = ((NumberType) dataType).GetNumericPrecisionInRadix();
                                emptyRowData[num19] = dataType.GetPrecisionRadix();
                            }
                            else if (!dataType.IsBooleanType())
                            {
                                if (dataType.IsDateTimeType())
                                {
                                    emptyRowData[num20] = dataType.GetAdoScale();
                                }
                                else if (dataType.IsIntervalType())
                                {
                                    emptyRowData[num9] = "INTERVAL";
                                    emptyRowData[num21] = IntervalType.GetQualifier(dataType.TypeCode);
                                    emptyRowData[num22] = dataType.Precision;
                                    emptyRowData[num20] = dataType.GetAdoScale();
                                }
                                else if (dataType.IsBinaryType())
                                {
                                    emptyRowData[num10] = dataType.Precision;
                                    emptyRowData[num11] = dataType.Precision;
                                }
                            }
                            if (dataType.IsDistinctType())
                            {
                                emptyRowData[num23] = base.database.GetCatalogName().Name;
                                emptyRowData[num24] = dataType.GetSchemaName().Name;
                                emptyRowData[num25] = dataType.GetName().Name;
                            }
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                }
            }
            return t;
        }

        private Table REFERENTIAL_CONSTRAINTS(Session session)
        {
            Table t = base.SysTables[50];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[50]);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UNIQUE_CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UNIQUE_CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UNIQUE_CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "MATCH_OPTION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "UPDATE_RULE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DELETE_RULE", DatabaseInformationMain.CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[50].Name, false, 20);
                int[] columns = new int[3];
                columns[1] = 1;
                columns[2] = 2;
                t.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (!table.IsView() && session.GetGrantee().HasNonSelectTableRight(table))
                {
                    foreach (Constraint constraint in table.GetConstraints())
                    {
                        if (constraint.GetConstraintType() == 0)
                        {
                            QNameManager.QName uniqueName = constraint.GetUniqueName();
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = constraint.GetSchemaName().Name;
                            emptyRowData[num3] = constraint.GetName().Name;
                            if (DatabaseInformationMain.IsAccessibleTable(session, constraint.GetMain()))
                            {
                                emptyRowData[num4] = base.database.GetCatalogName().Name;
                                emptyRowData[num5] = uniqueName.schema.Name;
                                emptyRowData[num6] = uniqueName.Name;
                            }
                            emptyRowData[num7] = "NONE";
                            emptyRowData[num8] = constraint.GetUpdateActionString();
                            emptyRowData[num9] = constraint.GetDeleteActionString();
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                }
            }
            return t;
        }

        private Table ROLE_AUTHORIZATION_DESCRIPTORS(Session session)
        {
            Table t = base.SysTables[0x33];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x33]);
                DatabaseInformationMain.AddColumn(t, "ROLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x33].Name, false, 20);
                int[] columns = new int[2];
                columns[1] = 1;
                t.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            string str = "SYS";
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            foreach (Grantee local1 in session.GetGrantee().VisibleGrantees())
            {
                string nameString = local1.GetNameString();
                Iterator<Grantee> iterator = local1.GetDirectRoles().GetIterator();
                string str3 = local1.IsAdmin() ? "YES" : "NO";
                while (iterator.HasNext())
                {
                    Grantee grantee = iterator.Next();
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = grantee.GetNameString();
                    emptyRowData[num2] = nameString;
                    emptyRowData[num3] = str;
                    emptyRowData[num4] = str3;
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table ROLE_COLUMN_GRANTS(Session session)
        {
            Table t = base.SysTables[0x34];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x34]);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PRIVILEGE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x34].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 5, 6, 1, 0, 4, 3, 2 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement("SELECT GRANTOR, GRANTEE, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, PRIVILEGE_TYPE, IS_GRANTABLE FROM INFORMATION_SCHEMA.COLUMN_PRIVILEGES JOIN INFORMATION_SCHEMA.APPLICABLE_ROLES ON GRANTEE = ROLE_NAME;", ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table ROLE_ROUTINE_GRANTS(Session session)
        {
            Table t = base.SysTables[0x36];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x36]);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PRIVILEGE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x36].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement("SELECT GRANTOR, GRANTEE, SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME, PRIVILEGE_TYPE, IS_GRANTABLE FROM INFORMATION_SCHEMA.ROUTINE_PRIVILEGES JOIN INFORMATION_SCHEMA.APPLICABLE_ROLES ON GRANTEE = ROLE_NAME;", ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table ROLE_TABLE_GRANTS(Session session)
        {
            Table t = base.SysTables[0x37];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x37]);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PRIVILEGE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "WITH_HIERARCHY", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x37].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 3, 4, 5, 0, 1 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement("SELECT GRANTOR, GRANTEE, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, PRIVILEGE_TYPE, IS_GRANTABLE, 'NO' FROM INFORMATION_SCHEMA.TABLE_PRIVILEGES JOIN INFORMATION_SCHEMA.APPLICABLE_ROLES ON GRANTEE = ROLE_NAME;", ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table ROLE_UDT_GRANTS(Session session)
        {
            Table t = base.SysTables[0x38];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x38]);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PRIVILEGE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x37].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, null, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement("SELECT GRANTOR, GRANTEE, UDT_CATALOG, UDT_SCHEMA, UDT_NAME, PRIVILEGE_TYPE, IS_GRANTABLE FROM INFORMATION_SCHEMA.UDT_PRIVILEGES JOIN INFORMATION_SCHEMA.APPLICABLE_ROLES ON GRANTEE = ROLE_NAME;", ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table ROLE_USAGE_GRANTS(Session session)
        {
            Table t = base.SysTables[0x39];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x39]);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "PRIVILEGE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x39].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Session session1 = base.database.sessionManager.NewSysSession(SqlInvariants.InformationSchemaQname, session.GetUser());
            Result ins = session1.ExecuteDirectStatement("SELECT GRANTOR, GRANTEE, OBJECT_CATALOG, OBJECT_SCHEMA, OBJECT_NAME, OBJECT_TYPE, PRIVILEGE_TYPE, IS_GRANTABLE FROM INFORMATION_SCHEMA.USAGE_PRIVILEGES JOIN INFORMATION_SCHEMA.APPLICABLE_ROLES ON GRANTEE = ROLE_NAME;", ResultProperties.DefaultPropsValue);
            Table.InsertSys(rowStore, ins);
            session1.Close();
            return t;
        }

        private Table ROUTINE_ASSEMBLY_USAGE(Session session)
        {
            Table t = base.SysTables[0x3b];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x3b]);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ASSEMBLY_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ASSEMBLY_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ASSEMBLY_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x3b].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                if (session.GetGrantee().IsAccessible(schema))
                {
                    Routine[] specificRoutines = schema.GetSpecificRoutines();
                    for (int i = 0; i < specificRoutines.Length; i++)
                    {
                        if (specificRoutines[i].GetLanguage() == 1)
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = schema.GetSchemaName().Name;
                            emptyRowData[num3] = schema.GetName().Name;
                            emptyRowData[num4] = base.database.GetCatalogName().Name;
                            emptyRowData[num5] = SchemaManager.GetSqljSchemaHsqlName();
                            emptyRowData[num6] = "CLASSPATH";
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                }
            }
            return t;
        }

        private Table ROUTINE_COLUMN_USAGE(Session session)
        {
            Table t = base.SysTables[0x3a];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x3a]);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x3a].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 3, 4, 5, 0, 1, 2, 6, 7, 8, 9 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            int num10 = 9;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                if (session.GetGrantee().IsAccessible(schema))
                {
                    Routine[] specificRoutines = schema.GetSpecificRoutines();
                    for (int i = 0; i < specificRoutines.Length; i++)
                    {
                        OrderedHashSet<QNameManager.QName> references = specificRoutines[i].GetReferences();
                        for (int j = 0; j < references.Size(); j++)
                        {
                            QNameManager.QName name = references.Get(j);
                            if ((name.type == 9) && session.GetGrantee().IsAccessible(name))
                            {
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[index] = base.database.GetCatalogName().Name;
                                emptyRowData[num2] = specificRoutines[i].GetSchemaName().Name;
                                emptyRowData[num3] = specificRoutines[i].GetSpecificName().Name;
                                emptyRowData[num4] = base.database.GetCatalogName().Name;
                                emptyRowData[num5] = schema.GetSchemaName().Name;
                                emptyRowData[num6] = schema.GetName().Name;
                                emptyRowData[num7] = base.database.GetCatalogName().Name;
                                emptyRowData[num8] = name.Parent.schema.Name;
                                emptyRowData[num9] = name.Parent.Name;
                                emptyRowData[num10] = name.Name;
                                try
                                {
                                    Table.InsertSys(rowStore, emptyRowData);
                                }
                                catch (CoreException)
                                {
                                }
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table ROUTINE_PRIVILEGES(Session session)
        {
            Table t = base.SysTables[60];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[60]);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PRIVILEGE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[60].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            int num10 = 9;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            OrderedHashSet<Grantee> granteeAndAllRolesWithPublic = session.GetGrantee().GetGranteeAndAllRolesWithPublic();
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                for (int i = 0; i < granteeAndAllRolesWithPublic.Size(); i++)
                {
                    Grantee local1 = granteeAndAllRolesWithPublic.Get(i);
                    OrderedHashSet<Right> allDirectPrivileges = local1.GetAllDirectPrivileges(schema);
                    OrderedHashSet<Right> allGrantedPrivileges = local1.GetAllGrantedPrivileges(schema);
                    if (!allGrantedPrivileges.IsEmpty())
                    {
                        allGrantedPrivileges.AddAll(allDirectPrivileges);
                        allDirectPrivileges = allGrantedPrivileges;
                    }
                    for (int j = 0; j < allDirectPrivileges.Size(); j++)
                    {
                        Right right = allDirectPrivileges.Get(j);
                        Right grantableRights = right.GetGrantableRights();
                        for (int k = 0; k < Right.PrivilegeTypes.Length; k++)
                        {
                            if (right.CanAccessFully(Right.PrivilegeTypes[k]))
                            {
                                Routine[] specificRoutines = schema.GetSpecificRoutines();
                                for (int m = 0; m < specificRoutines.Length; m++)
                                {
                                    string str = Right.PrivilegeNames[k];
                                    object[] emptyRowData = t.GetEmptyRowData();
                                    emptyRowData[index] = right.GetGrantor().GetName().Name;
                                    emptyRowData[num2] = right.GetGrantee().GetName().Name;
                                    emptyRowData[num3] = base.database.GetCatalogName().Name;
                                    emptyRowData[num4] = specificRoutines[m].GetSchemaName().Name;
                                    emptyRowData[num5] = specificRoutines[m].GetSpecificName().Name;
                                    emptyRowData[num6] = base.database.GetCatalogName().Name;
                                    emptyRowData[num7] = schema.GetSchemaName().Name;
                                    emptyRowData[num8] = schema.GetName().Name;
                                    emptyRowData[num9] = str;
                                    emptyRowData[num10] = ((right.GetGrantee() == schema.GetOwner()) || grantableRights.CanAccessFully(Right.PrivilegeTypes[k])) ? "YES" : "NO";
                                    try
                                    {
                                        Table.InsertSys(rowStore, emptyRowData);
                                    }
                                    catch (CoreException)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table ROUTINE_ROUTINE_USAGE(Session session)
        {
            Table t = base.SysTables[0x3d];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x3d]);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x3d].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                if (session.GetGrantee().IsAccessible(schema))
                {
                    Routine[] specificRoutines = schema.GetSpecificRoutines();
                    for (int i = 0; i < specificRoutines.Length; i++)
                    {
                        OrderedHashSet<QNameManager.QName> references = specificRoutines[i].GetReferences();
                        for (int j = 0; j < references.Size(); j++)
                        {
                            QNameManager.QName name = references.Get(j);
                            if ((name.type == 0x18) && session.GetGrantee().IsAccessible(name))
                            {
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[index] = base.database.GetCatalogName().Name;
                                emptyRowData[num2] = specificRoutines[i].GetSchemaName().Name;
                                emptyRowData[num3] = specificRoutines[i].GetSpecificName().Name;
                                emptyRowData[num4] = base.database.GetCatalogName().Name;
                                emptyRowData[num5] = name.schema.Name;
                                emptyRowData[num6] = name.Name;
                                try
                                {
                                    Table.InsertSys(rowStore, emptyRowData);
                                }
                                catch (CoreException)
                                {
                                }
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table ROUTINE_SEQUENCE_USAGE(Session session)
        {
            Table t = base.SysTables[0x3e];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x3e]);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SEQUENCE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SEQUENCE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SEQUENCE_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x3e].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                if (session.GetGrantee().IsAccessible(schema))
                {
                    Routine[] specificRoutines = schema.GetSpecificRoutines();
                    for (int i = 0; i < specificRoutines.Length; i++)
                    {
                        OrderedHashSet<QNameManager.QName> references = specificRoutines[i].GetReferences();
                        for (int j = 0; j < references.Size(); j++)
                        {
                            QNameManager.QName name = references.Get(j);
                            if ((name.type == 7) && session.GetGrantee().IsAccessible(name))
                            {
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[index] = base.database.GetCatalogName().Name;
                                emptyRowData[num2] = specificRoutines[i].GetSchemaName().Name;
                                emptyRowData[num3] = specificRoutines[i].GetSpecificName().Name;
                                emptyRowData[num4] = base.database.GetCatalogName().Name;
                                emptyRowData[num5] = name.schema.Name;
                                emptyRowData[num6] = name.Name;
                                try
                                {
                                    Table.InsertSys(rowStore, emptyRowData);
                                }
                                catch (CoreException)
                                {
                                }
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table ROUTINE_TABLE_USAGE(Session session)
        {
            Table t = base.SysTables[0x3f];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x3f]);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x3f].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 3, 4, 5, 0, 1, 2, 6, 7, 8 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                if (session.GetGrantee().IsAccessible(schema))
                {
                    Routine[] specificRoutines = schema.GetSpecificRoutines();
                    for (int i = 0; i < specificRoutines.Length; i++)
                    {
                        OrderedHashSet<QNameManager.QName> references = specificRoutines[i].GetReferences();
                        for (int j = 0; j < references.Size(); j++)
                        {
                            QNameManager.QName name = references.Get(j);
                            if (((name.type == 3) || (name.type == 4)) && session.GetGrantee().IsAccessible(name))
                            {
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[index] = base.database.GetCatalogName().Name;
                                emptyRowData[num2] = specificRoutines[i].GetSchemaName().Name;
                                emptyRowData[num3] = specificRoutines[i].GetSpecificName().Name;
                                emptyRowData[num4] = base.database.GetCatalogName().Name;
                                emptyRowData[num5] = schema.GetSchemaName().Name;
                                emptyRowData[num6] = schema.GetName().Name;
                                emptyRowData[num7] = base.database.GetCatalogName().Name;
                                emptyRowData[num8] = name.schema.Name;
                                emptyRowData[num9] = name.Name;
                                try
                                {
                                    Table.InsertSys(rowStore, emptyRowData);
                                }
                                catch (CoreException)
                                {
                                }
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table ROUTINES(Session session)
        {
            Table t = base.SysTables[0x40];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x40]);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "MODULE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "MODULE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "MODULE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_MAXIMUM_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_OCTET_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION_RADIX", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DATETIME_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "TYPE_UDT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TYPE_UDT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TYPE_UDT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCOPE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "MAXIMUM_CARDINALITY", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DTD_IDENTIFIER", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_BODY", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "ROUTINE_DEFINITION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "EXTERNAL_NAME", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "EXTERNAL_LANGUAGE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "PARAMETER_STYLE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_DETERMINISTIC", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "SQL_DATA_ACCESS", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_NULL_CALL", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "SQL_PATH", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "SCHEMA_LEVEL_ROUTINE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "MAX_DYNAMIC_RESULT_SETS", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "IS_USER_DEFINED_CAST", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IS_IMPLICITLY_INVOCABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "SECURITY_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "TO_SQL_SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TO_SQL_SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TO_SQL_SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "AS_LOCATOR", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "CREATED", DatabaseInformationMain.TimeStamp);
                DatabaseInformationMain.AddColumn(t, "LAST_ALTERED", DatabaseInformationMain.TimeStamp);
                DatabaseInformationMain.AddColumn(t, "NEW_SAVEPOINT_LEVEL", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IS_UDT_DEPENDENT", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_FROM_DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_AS_LOCATOR", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_CHAR_MAX_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_CHAR_OCTET_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_CHAR_SET_CATALOG", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_CHAR_SET_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_CHARACTER_SET_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_COLLATION_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_COLLATION_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_COLLATION_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_NUMERIC_RADIX", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_DATETIME_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_INTERVAL_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_INTERVAL_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_TYPE_UDT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_TYPE_UDT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_TYPE_UDT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_SCOPE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_SCOPE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_SCOPE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_MAX_CARDINALITY", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_DTD_IDENTIFIER", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DECLARED_DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_FROM_DECLARED_DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_DECLARED_NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "RESULT_CAST_DECLARED_NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x40].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 3, 4, 5, 0, 1, 2 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            int num10 = 9;
            int num11 = 10;
            int num12 = 11;
            int num13 = 12;
            int num14 = 13;
            int num15 = 14;
            int num16 = 15;
            int num17 = 0x10;
            int num18 = 0x11;
            int num19 = 0x12;
            int num20 = 0x13;
            int num21 = 20;
            int num22 = 0x15;
            int num23 = 0x16;
            int num24 = 0x17;
            int num25 = 0x18;
            int num26 = 0x19;
            int num27 = 0x1a;
            int num28 = 0x1b;
            int num29 = 0x1c;
            int num30 = 0x1d;
            int num31 = 30;
            int num32 = 0x1f;
            int num33 = 0x20;
            int num34 = 0x21;
            int num35 = 0x22;
            int num36 = 0x23;
            int num37 = 0x24;
            int num38 = 0x25;
            int num39 = 0x26;
            int num40 = 0x27;
            int num41 = 40;
            int num42 = 0x29;
            int num43 = 0x2a;
            int num44 = 0x2b;
            int num45 = 0x2c;
            int num46 = 0x2d;
            int num47 = 0x2e;
            int num48 = 0x2f;
            int num49 = 0x30;
            int num50 = 0x31;
            int num51 = 50;
            int num52 = 0x33;
            int num53 = 0x34;
            int num54 = 0x35;
            int num55 = 0x36;
            int num56 = 0x37;
            int num57 = 0x38;
            int num58 = 0x39;
            int num59 = 0x3a;
            int num60 = 0x3b;
            int num61 = 60;
            int num62 = 0x3d;
            int num63 = 0x3e;
            int num64 = 0x3f;
            int num65 = 0x40;
            int num66 = 0x41;
            int num67 = 0x42;
            int num68 = 0x43;
            int num69 = 0x44;
            int num70 = 0x45;
            int num71 = 70;
            int num72 = 0x47;
            int num73 = 0x48;
            int num74 = 0x49;
            int num75 = 0x4a;
            int num76 = 0x4b;
            int num77 = 0x4c;
            int num78 = 0x4d;
            int num79 = 0x4e;
            int num80 = 0x4f;
            int num81 = 80;
            int num82 = 0x51;
            int num83 = 0x52;
            int num84 = 0x53;
            int num85 = 0x54;
            int num86 = 0x55;
            int num87 = 0x56;
            int num88 = 0x57;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                if (session.GetGrantee().IsAccessible(schema))
                {
                    foreach (Routine routine in schema.GetSpecificRoutines())
                    {
                        object[] emptyRowData = t.GetEmptyRowData();
                        SqlType type = routine.IsProcedure() ? null : routine.GetReturnType();
                        emptyRowData[index] = base.database.GetCatalogName().Name;
                        emptyRowData[num2] = routine.GetSchemaName().Name;
                        emptyRowData[num3] = routine.GetSpecificName().Name;
                        emptyRowData[num4] = base.database.GetCatalogName().Name;
                        emptyRowData[num5] = schema.GetSchemaName().Name;
                        emptyRowData[num6] = routine.GetName().Name;
                        emptyRowData[num7] = routine.IsProcedure() ? "PROCEDURE" : "FUNCTION";
                        emptyRowData[num8] = null;
                        emptyRowData[num9] = null;
                        emptyRowData[num10] = null;
                        emptyRowData[num11] = null;
                        emptyRowData[num12] = null;
                        emptyRowData[num13] = null;
                        emptyRowData[num14] = (type == null) ? null : type.GetNameString();
                        if (type != null)
                        {
                            if (type.IsCharacterType())
                            {
                                CharacterType type2 = (CharacterType) type;
                                emptyRowData[num15] = type.Precision;
                                emptyRowData[num16] = type.Precision * 2L;
                                emptyRowData[num17] = base.database.GetCatalogName().Name;
                                emptyRowData[num18] = type2.GetCharacterSet().GetSchemaName().Name;
                                emptyRowData[num19] = type2.GetCharacterSet().GetName().Name;
                                emptyRowData[num20] = base.database.GetCatalogName().Name;
                                emptyRowData[num21] = type2.GetCollation().GetSchemaName().Name;
                                emptyRowData[num22] = type2.GetCollation().GetName().Name;
                            }
                            else if (type.IsNumberType())
                            {
                                NumberType type3 = (NumberType) type;
                                emptyRowData[num23] = type3.GetNumericPrecisionInRadix();
                                emptyRowData[num84] = type3.GetNumericPrecisionInRadix();
                                if (type.IsExactNumberType())
                                {
                                    emptyRowData[num25] = emptyRowData[num85] = type.GetAdoScale();
                                }
                                emptyRowData[num24] = type.GetPrecisionRadix();
                            }
                            else if (!type.IsBooleanType())
                            {
                                if (type.IsDateTimeType())
                                {
                                    emptyRowData[num26] = type.GetAdoScale();
                                }
                                else if (type.IsIntervalType())
                                {
                                    emptyRowData[num14] = "INTERVAL";
                                    emptyRowData[num27] = IntervalType.GetQualifier(type.TypeCode);
                                    emptyRowData[num28] = type.Precision;
                                    emptyRowData[num26] = type.GetAdoScale();
                                }
                                else if (type.IsBinaryType())
                                {
                                    emptyRowData[num15] = type.Precision;
                                    emptyRowData[num16] = type.Precision;
                                }
                            }
                        }
                        emptyRowData[num29] = null;
                        emptyRowData[num30] = null;
                        emptyRowData[num31] = null;
                        emptyRowData[num32] = null;
                        emptyRowData[num33] = null;
                        emptyRowData[num34] = null;
                        emptyRowData[num35] = null;
                        emptyRowData[num36] = null;
                        emptyRowData[num37] = (routine.GetLanguage() == 1) ? "EXTERNAL" : "SQL";
                        emptyRowData[num38] = routine.GetSql();
                        emptyRowData[num39] = (routine.GetLanguage() == 1) ? routine.GetMethod().Name : null;
                        emptyRowData[num40] = (routine.GetLanguage() == 1) ? "JAVA" : null;
                        emptyRowData[num41] = (routine.GetLanguage() == 1) ? "JAVA" : null;
                        emptyRowData[num42] = routine.IsDeterministic() ? "YES" : "NO";
                        emptyRowData[num43] = routine.GetDataImpactString();
                        emptyRowData[num44] = (type == null) ? null : (routine.IsNullInputOutput() ? "YES" : "NO");
                        emptyRowData[num45] = null;
                        emptyRowData[num46] = "YES";
                        emptyRowData[num47] = 0L;
                        emptyRowData[num48] = (type == null) ? null : "NO";
                        emptyRowData[num49] = null;
                        emptyRowData[num50] = "DEFINER";
                        emptyRowData[num51] = null;
                        emptyRowData[num52] = null;
                        emptyRowData[num53] = null;
                        emptyRowData[num54] = (type == null) ? null : "NO";
                        emptyRowData[num55] = null;
                        emptyRowData[num56] = null;
                        emptyRowData[num57] = "YES";
                        emptyRowData[num58] = null;
                        emptyRowData[num59] = null;
                        emptyRowData[num60] = null;
                        emptyRowData[num61] = null;
                        emptyRowData[num62] = null;
                        emptyRowData[num63] = null;
                        emptyRowData[num64] = null;
                        emptyRowData[num65] = null;
                        emptyRowData[num66] = null;
                        emptyRowData[num67] = null;
                        emptyRowData[num68] = null;
                        emptyRowData[num69] = null;
                        emptyRowData[num70] = null;
                        emptyRowData[num71] = null;
                        emptyRowData[num72] = null;
                        emptyRowData[num73] = null;
                        emptyRowData[num74] = null;
                        emptyRowData[num75] = null;
                        emptyRowData[num76] = null;
                        emptyRowData[num77] = null;
                        emptyRowData[num78] = null;
                        emptyRowData[num79] = null;
                        emptyRowData[num80] = null;
                        emptyRowData[num81] = null;
                        emptyRowData[num82] = null;
                        emptyRowData[num83] = emptyRowData[num14];
                        emptyRowData[num84] = emptyRowData[num23];
                        emptyRowData[num85] = emptyRowData[num25];
                        emptyRowData[num86] = null;
                        emptyRowData[num87] = null;
                        emptyRowData[num88] = null;
                        Table.InsertSys(rowStore, emptyRowData);
                    }
                }
            }
            return t;
        }

        private Table SCHEMATA(Session session)
        {
            Table t = base.SysTables[0x41];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x41]);
                DatabaseInformationMain.AddColumn(t, "CATALOG_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCHEMA_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SCHEMA_OWNER", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DEFAULT_CHARACTER_SET_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DEFAULT_CHARACTER_SET_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DEFAULT_CHARACTER_SET_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SQL_PATH", DatabaseInformationMain.CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x41].Name, false, 20);
                int[] columns = new int[2];
                columns[1] = 1;
                t.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            string str = "INFORMATION_SCHEMA";
            string str2 = "UTF16";
            string str3 = null;
            Grantee grantee = session.GetGrantee();
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            Iterator<string> iterator = base.database.schemaManager.FullSchemaNamesIterator();
            while (iterator.HasNext())
            {
                string name = iterator.Next();
                QNameManager.QName schemaQName = base.database.schemaManager.GetSchemaQName(name);
                if (grantee.HasSchemaUpdateOrGrantRights(name))
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = schemaQName.Name;
                    emptyRowData[num3] = base.database.schemaManager.ToSchemaOwner(name).GetNameString();
                    emptyRowData[num4] = base.database.GetCatalogName().Name;
                    emptyRowData[num5] = str;
                    emptyRowData[num6] = str2;
                    emptyRowData[num7] = str3;
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table SYSTEM_CACHEINFO(Session session)
        {
            Table t = base.SysTables[15];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[15]);
                DatabaseInformationMain.AddColumn(t, "CACHE_FILE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "MAX_CACHE_COUNT", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "MAX_CACHE_BYTES", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CACHE_SIZE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CACHE_BYTES", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "FILE_FREE_BYTES", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "FILE_FREE_COUNT", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "FILE_FREE_POS", DatabaseInformationMain.CardinalNumber);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[15].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            DataFileCache item = null;
            HashSet<DataFileCache> set = new HashSet<DataFileCache>();
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                IPersistentStore store2 = session.sessionData.GetRowStore(table);
                if (session.GetGrantee().IsFullyAccessibleByRole(table.GetName()))
                {
                    if (store2 != null)
                    {
                        item = store2.GetCache();
                    }
                    if (item != null)
                    {
                        set.Add(item);
                    }
                }
            }
            foreach (DataFileCache local1 in set)
            {
                object[] emptyRowData = t.GetEmptyRowData();
                emptyRowData[index] = base.database.logger.fileAccess.CanonicalOrAbsolutePath(item.GetFileName());
                emptyRowData[num2] = item.Capacity();
                emptyRowData[num3] = item.BytesCapacity();
                emptyRowData[num4] = item.GetCachedObjectCount();
                emptyRowData[num5] = item.GetTotalCachedBlockSize();
                emptyRowData[num6] = item.GetTotalFreeBlockSize();
                emptyRowData[num7] = item.GetFreeBlockCount();
                emptyRowData[num8] = item.GetFileFreePos();
                Table.InsertSys(rowStore, emptyRowData);
            }
            return t;
        }

        private Table SYSTEM_COMMENTS(Session session)
        {
            Table t = base.SysTables[0x5d];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x5d]);
                DatabaseInformationMain.AddColumn(t, "OBJECT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_TYPE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COMMENT", DatabaseInformationMain.CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x5d].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            DITableInfo info = new DITableInfo();
            Iterator<object> iterator = base.AllTables();
            while (iterator.HasNext())
            {
                Table table3 = (Table) iterator.Next();
                if (session.GetGrantee().IsAccessible(table3))
                {
                    info.SetTable(table3);
                    int columnCount = table3.GetColumnCount();
                    for (int i = 0; i < columnCount; i++)
                    {
                        ColumnSchema column = table3.GetColumn(i);
                        if (column.GetName().Comment != null)
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = table3.GetSchemaName().Name;
                            emptyRowData[num3] = table3.GetName().Name;
                            emptyRowData[num4] = "COLUMN";
                            emptyRowData[num5] = column.GetName().Name;
                            emptyRowData[num6] = column.GetName().Comment;
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                    if ((table3.GetTableType() == 1) || (table3.GetName().Comment != null))
                    {
                        object[] emptyRowData = t.GetEmptyRowData();
                        emptyRowData[index] = base.database.GetCatalogName().Name;
                        emptyRowData[num2] = table3.GetSchemaName().Name;
                        emptyRowData[num3] = table3.GetName().Name;
                        emptyRowData[num4] = (table3.IsView() || (table3.GetTableType() == 1)) ? "VIEW" : "TABLE";
                        emptyRowData[num5] = null;
                        emptyRowData[num6] = info.GetRemark();
                        Table.InsertSys(rowStore, emptyRowData);
                    }
                }
            }
            iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                ISchemaObject obj2 = (ISchemaObject) iterator.Next();
                if (session.GetGrantee().IsAccessible(obj2) && (obj2.GetName().Comment != null))
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = obj2.GetSchemaName().Name;
                    emptyRowData[num3] = obj2.GetName().Name;
                    emptyRowData[num4] = "ROUTINE";
                    emptyRowData[num5] = null;
                    emptyRowData[num6] = obj2.GetName().Comment;
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table SYSTEM_PROCEDURECOLUMNS(Session session)
        {
            Table t = base.SysTables[5];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[5]);
                DatabaseInformationMain.AddColumn(t, "PROCEDURE_CAT", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PROCEDURE_SCHEM", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PROCEDURE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_TYPE", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "DATA_TYPE", SqlType.SqlSmallint);
                DatabaseInformationMain.AddColumn(t, "TYPE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PRECISION", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "LENGTH", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "SCALE", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "RADIX", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "NULLABLE", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "REMARKS", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "COLUMN_DEF", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "SQL_DATA_TYPE", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "SQL_DATETIME_SUB", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "CHAR_OCTET_LENGTH", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "ORDINAL_POSITION", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "IS_NULLABLE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[5].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 0x13, 0x11 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 9;
            int num10 = 10;
            int num11 = 11;
            int num12 = 14;
            int num13 = 0x10;
            int num14 = 0x11;
            int num15 = 0x12;
            int num16 = 0x13;
            bool flag = base.database.GetProperties().IsPropertyTrue("jdbc.translate_dti_types");
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x12);
            while (iterator.HasNext())
            {
                RoutineSchema schema = (RoutineSchema) iterator.Next();
                if (session.GetGrantee().IsAccessible(schema))
                {
                    foreach (Routine routine in schema.GetSpecificRoutines())
                    {
                        int parameterCount = routine.GetParameterCount();
                        for (int i = 0; i < parameterCount; i++)
                        {
                            ColumnSchema parameter = routine.GetParameter(i);
                            SqlType dataType = parameter.GetDataType();
                            object[] emptyRowData = t.GetEmptyRowData();
                            if (flag)
                            {
                                if (dataType.IsIntervalType())
                                {
                                    dataType = CharacterType.GetCharacterType(12, (long) dataType.DisplaySize());
                                }
                                else if (dataType.IsDateTimeTypeWithZone())
                                {
                                    dataType = ((DateTimeType) dataType).GetDateTimeTypeWithoutZone();
                                }
                            }
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = routine.GetSchemaName().Name;
                            emptyRowData[num16] = routine.GetSpecificName().Name;
                            emptyRowData[num3] = routine.GetName().Name;
                            emptyRowData[num4] = parameter.GetName().Name;
                            emptyRowData[num14] = i + 1;
                            emptyRowData[num5] = parameter.GetParameterMode();
                            emptyRowData[num7] = dataType.GetFullNameString();
                            emptyRowData[num6] = dataType.GetAdoTypeCode();
                            emptyRowData[num8] = 0;
                            emptyRowData[num13] = 0;
                            if (dataType.IsCharacterType())
                            {
                                emptyRowData[num8] = dataType.GetAdoPrecision();
                                emptyRowData[num13] = dataType.GetAdoPrecision();
                            }
                            if (dataType.IsBinaryType())
                            {
                                emptyRowData[num8] = dataType.GetAdoPrecision();
                                emptyRowData[num13] = dataType.GetAdoPrecision();
                            }
                            if (dataType.IsNumberType())
                            {
                                emptyRowData[num8] = ((NumberType) dataType).GetNumericPrecisionInRadix();
                                emptyRowData[num10] = dataType.GetPrecisionRadix();
                                if (dataType.IsExactNumberType())
                                {
                                    emptyRowData[num9] = dataType.GetAdoScale();
                                }
                            }
                            if (dataType.IsDateTimeType())
                            {
                                emptyRowData[num8] = parameter.GetDataType().DisplaySize();
                            }
                            emptyRowData[num12] = parameter.GetDataType().TypeCode;
                            emptyRowData[num11] = parameter.GetNullability();
                            emptyRowData[num15] = parameter.IsNullable() ? "YES" : "NO";
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                }
            }
            return t;
        }

        private Table SYSTEM_PROCEDURES(Session session)
        {
            Table t = base.SysTables[6];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[6]);
                DatabaseInformationMain.AddColumn(t, "PROCEDURE_CAT", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PROCEDURE_SCHEM", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PROCEDURE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COL_4", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "COL_5", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "COL_6", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "REMARKS", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "PROCEDURE_TYPE", SqlType.SqlSmallint);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[6].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 8 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 7;
            int num5 = 8;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(0x18);
            while (iterator.HasNext())
            {
                Routine routine = (Routine) iterator.Next();
                object[] emptyRowData = t.GetEmptyRowData();
                emptyRowData[index] = emptyRowData[index] = base.database.GetCatalogName().Name;
                emptyRowData[num2] = routine.GetSchemaName().Name;
                emptyRowData[num3] = routine.GetName().Name;
                emptyRowData[num4] = routine.IsProcedure() ? 1 : 2;
                emptyRowData[num5] = routine.GetSpecificName().Name;
                Table.InsertSys(rowStore, emptyRowData);
            }
            return t;
        }

        private Table SYSTEM_PROPERTIES(Session session)
        {
            object[] emptyRowData;
            Table t = base.SysTables[0x11];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x11]);
                DatabaseInformationMain.AddColumn(t, "PROPERTY_SCOPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "PROPERTY_NAMESPACE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "PROPERTY_NAME", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "PROPERTY_VALUE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "PROPERTY_CLASS", DatabaseInformationMain.CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x11].Name, false, 20);
                int[] columns = new int[3];
                columns[1] = 1;
                columns[2] = 2;
                t.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            string str = "SESSION";
            LibCoreDatabaseProperties properties = base.database.GetProperties();
            string str2 = "database.properties";
            foreach (object[] objArray2 in LibCoreDatabaseProperties.GetUserDefinedPropertyData())
            {
                emptyRowData = t.GetEmptyRowData();
                emptyRowData[index] = str;
                emptyRowData[num2] = str2;
                emptyRowData[num3] = objArray2[0];
                emptyRowData[num4] = properties.GetProperty((string) emptyRowData[num3]);
                emptyRowData[num5] = objArray2[2];
                Table.InsertSys(rowStore, emptyRowData);
            }
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[index] = str;
            emptyRowData[num2] = str2;
            emptyRowData[num3] = "SCRIPT FORMAT";
            try
            {
                emptyRowData[num4] = ScriptWriterBase.ListScriptFormats[base.database.logger.GetScriptType()];
            }
            catch (Exception)
            {
            }
            emptyRowData[num5] = "String";
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[index] = str;
            emptyRowData[num2] = str2;
            emptyRowData[num3] = "WRITE DELAY";
            emptyRowData[num4] = base.database.logger.GetWriteDelay();
            emptyRowData[num5] = "Integer";
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[index] = str;
            emptyRowData[num2] = str2;
            emptyRowData[num3] = "REFERENTIAL INTEGRITY";
            emptyRowData[num4] = base.database.IsReferentialIntegrity() ? "true" : "false";
            emptyRowData[num5] = "Boolean";
            Table.InsertSys(rowStore, emptyRowData);
            return t;
        }

        private Table SYSTEM_SESSIONINFO(Session session)
        {
            Table t = base.SysTables[0x12];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x12]);
                DatabaseInformationMain.AddColumn(t, "KEY", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "VALUE", DatabaseInformationMain.CharacterData);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x12].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            object[] emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "SESSION ID";
            emptyRowData[1] = session.GetId().ToString();
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "AUTOCOMMIT";
            emptyRowData[1] = session.IsAutoCommit() ? "TRUE" : "FALSE";
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "USER";
            emptyRowData[1] = session.GetUsername();
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "SESSION READONLY";
            emptyRowData[1] = session.IsReadOnlyDefault() ? "TRUE" : "FALSE";
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "DATABASE READONLY";
            emptyRowData[1] = base.database.IsReadOnly() ? "TRUE" : "FALSE";
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "MAXROWS";
            emptyRowData[1] = session.GetSqlMaxRows().ToString();
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "DATABASE";
            emptyRowData[1] = base.database.GetUri();
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "IDENTITY";
            emptyRowData[1] = session.GetLastIdentity().ToString();
            Table.InsertSys(rowStore, emptyRowData);
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "CURRENT SCHEMA";
            emptyRowData[1] = session.GetSchemaName(null);
            Table.InsertSys(rowStore, emptyRowData);
            return t;
        }

        private Table SYSTEM_SESSIONS(Session session)
        {
            Table t = base.SysTables[0x13];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x13]);
                DatabaseInformationMain.AddColumn(t, "SESSION_ID", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CONNECTED", DatabaseInformationMain.TimeStamp);
                DatabaseInformationMain.AddColumn(t, "USER_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "IS_ADMIN", SqlType.SqlBoolean);
                DatabaseInformationMain.AddColumn(t, "AUTOCOMMIT", SqlType.SqlBoolean);
                DatabaseInformationMain.AddColumn(t, "READONLY", SqlType.SqlBoolean);
                DatabaseInformationMain.AddColumn(t, "MAXROWS", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "LAST_IDENTITY", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "TRANSACTION_SIZE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "SCHEMA", DatabaseInformationMain.SqlIdentifier);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x13].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            int num10 = 9;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            foreach (Session session2 in base.database.sessionManager.GetVisibleSessions(session))
            {
                object[] emptyRowData = t.GetEmptyRowData();
                emptyRowData[index] = session2.GetId();
                emptyRowData[num2] = new TimestampData(session2.GetConnectTime() / 0x989680L);
                emptyRowData[num3] = session2.GetUsername();
                emptyRowData[num4] = session2.IsAdmin();
                emptyRowData[num5] = session2.IsAutoCommit();
                emptyRowData[num6] = session2.IsReadOnlyDefault();
                emptyRowData[num7] = session2.GetSqlMaxRows();
                emptyRowData[num8] = Convert.ToInt64(session2.GetLastIdentity());
                emptyRowData[num9] = session2.GetTransactionSize();
                emptyRowData[num10] = session2.GetCurrentSchemaQName().Name;
                Table.InsertSys(rowStore, emptyRowData);
            }
            return t;
        }

        private Table SYSTEM_UDTS(Session session)
        {
            Table t = base.SysTables[11];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[11]);
                DatabaseInformationMain.AddColumn(t, "TYPE_CAT", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TYPE_SCHEM", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TYPE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CLASS_NAME", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DATA_TYPE", SqlType.SqlInteger);
                DatabaseInformationMain.AddColumn(t, "REMARKS", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "BASE_TYPE", SqlType.SqlSmallint);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[11].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, null, false);
                return t;
            }
            bool flag = base.database.GetProperties().IsPropertyTrue("jdbc.translate_dti_types");
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(12);
            while (iterator.HasNext())
            {
                SqlType type = (SqlType) iterator.Next();
                if (type.IsDistinctType())
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    SqlType characterType = type;
                    if (flag)
                    {
                        if (characterType.IsIntervalType())
                        {
                            characterType = CharacterType.GetCharacterType(12, (long) characterType.DisplaySize());
                        }
                        else if (characterType.IsDateTimeTypeWithZone())
                        {
                            characterType = ((DateTimeType) characterType).GetDateTimeTypeWithoutZone();
                        }
                    }
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = type.GetSchemaName().Name;
                    emptyRowData[num3] = type.GetName().Name;
                    emptyRowData[num4] = characterType.GetCSharpClassName();
                    emptyRowData[num5] = 0x7d1;
                    emptyRowData[num6] = null;
                    emptyRowData[num7] = characterType.GetAdoTypeCode();
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table TABLE_CONSTRAINTS(Session session)
        {
            Table t = base.SysTables[0x49];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x49]);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CONSTRAINT_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "IS_DEFERRABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "INITIALLY_DEFERRED", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x49].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 4, 5, 6 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (!table.IsView() && DatabaseInformationMain.IsAccessibleTable(session, table))
                {
                    Constraint[] constraints = table.GetConstraints();
                    int length = constraints.Length;
                    int num11 = 0;
                    while (num11 < length)
                    {
                        string str;
                        Constraint constraint = constraints[num11];
                        object[] emptyRowData = t.GetEmptyRowData();
                        switch (constraint.GetConstraintType())
                        {
                            case 0:
                                emptyRowData[num4] = "FOREIGN KEY";
                                table = constraint.GetRef();
                                goto Label_01D9;

                            case 2:
                                emptyRowData[num4] = "UNIQUE";
                                goto Label_01D9;

                            case 3:
                                emptyRowData[num4] = "CHECK";
                                goto Label_01D9;

                            case 4:
                                emptyRowData[num4] = "PRIMARY KEY";
                                goto Label_01D9;
                        }
                    Label_01D1:
                        num11++;
                        continue;
                    Label_01D9:
                        str = base.database.GetCatalogName().Name;
                        string name = table.GetSchemaName().Name;
                        emptyRowData[index] = str;
                        emptyRowData[num2] = name;
                        emptyRowData[num3] = constraint.GetName().Name;
                        emptyRowData[num5] = str;
                        emptyRowData[num6] = name;
                        emptyRowData[num7] = table.GetName().Name;
                        emptyRowData[num8] = "NO";
                        emptyRowData[num9] = "NO";
                        Table.InsertSys(rowStore, emptyRowData);
                        goto Label_01D1;
                    }
                }
            }
            return t;
        }

        private Table TRIGGER_COLUMN_USAGE(Session session)
        {
            Table t = base.SysTables[0x4d];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x4d]);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x4d].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(8);
            while (iterator.HasNext())
            {
                TriggerDef def = (TriggerDef) iterator.Next();
                if (session.GetGrantee().IsAccessible(def))
                {
                    OrderedHashSet<QNameManager.QName> references = def.GetReferences();
                    for (int i = 0; i < references.Size(); i++)
                    {
                        QNameManager.QName name = references.Get(i);
                        if ((name.type == 9) && session.GetGrantee().IsAccessible(name))
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = def.GetSchemaName().Name;
                            emptyRowData[num3] = def.GetName().Name;
                            emptyRowData[num4] = base.database.GetCatalogName().Name;
                            emptyRowData[num5] = name.Parent.schema.Name;
                            emptyRowData[num6] = name.Parent.Name;
                            emptyRowData[num7] = name.Name;
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                            catch (CoreException)
                            {
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table TRIGGER_ROUTINE_USAGE(Session session)
        {
            Table t = base.SysTables[0x4e];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x4e]);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x4e].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(8);
            while (iterator.HasNext())
            {
                TriggerDef def = (TriggerDef) iterator.Next();
                if (session.GetGrantee().IsAccessible(def))
                {
                    OrderedHashSet<QNameManager.QName> references = def.GetReferences();
                    for (int i = 0; i < references.Size(); i++)
                    {
                        QNameManager.QName name = references.Get(i);
                        if ((name.type == 0x18) && session.GetGrantee().IsAccessible(name))
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = def.GetSchemaName().Name;
                            emptyRowData[num3] = def.GetName().Name;
                            emptyRowData[num4] = base.database.GetCatalogName().Name;
                            emptyRowData[num5] = name.schema.Name;
                            emptyRowData[num6] = name.Name;
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                            catch (CoreException)
                            {
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table TRIGGER_SEQUENCE_USAGE(Session session)
        {
            Table t = base.SysTables[0x4f];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x4f]);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SEQUENCE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SEQUENCE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SEQUENCE_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x4f].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(8);
            while (iterator.HasNext())
            {
                TriggerDef def = (TriggerDef) iterator.Next();
                if (session.GetGrantee().IsAccessible(def))
                {
                    OrderedHashSet<QNameManager.QName> references = def.GetReferences();
                    for (int i = 0; i < references.Size(); i++)
                    {
                        QNameManager.QName name = references.Get(i);
                        if ((name.type == 7) && session.GetGrantee().IsAccessible(name))
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = def.GetSchemaName().Name;
                            emptyRowData[num3] = def.GetName().Name;
                            emptyRowData[num4] = base.database.GetCatalogName().Name;
                            emptyRowData[num5] = name.schema.Name;
                            emptyRowData[num6] = name.Name;
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                            catch (CoreException)
                            {
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table TRIGGER_TABLE_USAGE(Session session)
        {
            Table t = base.SysTables[80];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[80]);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[80].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(8);
            while (iterator.HasNext())
            {
                TriggerDef def = (TriggerDef) iterator.Next();
                if (session.GetGrantee().IsAccessible(def))
                {
                    OrderedHashSet<QNameManager.QName> references = def.GetReferences();
                    for (int i = 0; i < references.Size(); i++)
                    {
                        QNameManager.QName name = references.Get(i);
                        if (((name.type == 3) || (name.type == 4)) && session.GetGrantee().IsAccessible(name))
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = def.GetSchemaName().Name;
                            emptyRowData[num3] = def.GetName().Name;
                            emptyRowData[num4] = base.database.GetCatalogName().Name;
                            emptyRowData[num5] = name.schema.Name;
                            emptyRowData[num6] = name.Name;
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                            catch (CoreException)
                            {
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table TRIGGERED_UPDATE_COLUMNS(Session session)
        {
            Table t = base.SysTables[0x51];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x51]);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "EVENT_OBJECT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "EVENT_OBJECT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "EVENT_OBJECT_TABLE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "EVENT_OBJECT_COLUMN", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x51].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(8);
            while (iterator.HasNext())
            {
                TriggerDef def = (TriggerDef) iterator.Next();
                if (session.GetGrantee().IsAccessible(def))
                {
                    int[] updateColumnIndexes = def.GetUpdateColumnIndexes();
                    if (updateColumnIndexes != null)
                    {
                        for (int i = 0; i < updateColumnIndexes.Length; i++)
                        {
                            ColumnSchema column = def.GetTable().GetColumn(updateColumnIndexes[i]);
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = def.GetSchemaName().Name;
                            emptyRowData[num3] = def.GetName().Name;
                            emptyRowData[num4] = base.database.GetCatalogName().Name;
                            emptyRowData[num5] = def.GetTable().GetSchemaName().Name;
                            emptyRowData[num6] = def.GetTable().GetName().Name;
                            emptyRowData[num7] = column.GetNameString();
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                }
            }
            return t;
        }

        private Table TRIGGERS(Session session)
        {
            Table t = base.SysTables[0x52];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x52]);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TRIGGER_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "EVENT_MANIPULATION", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "EVENT_OBJECT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "EVENT_OBJECT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "EVENT_OBJECT_TABLE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ACTION_ORDER", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "ACTION_CONDITION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "ACTION_STATEMENT", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "ACTION_ORIENTATION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "ACTION_TIMING", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "ACTION_REFERENCE_OLD_TABLE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ACTION_REFERENCE_NEW_TABLE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ACTION_REFERENCE_OLD_ROW", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ACTION_REFERENCE_NEW_ROW", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CREATED", DatabaseInformationMain.TimeStamp);
                DatabaseInformationMain.AddColumn(t, "DEFINITION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "UPDATE_COLUMNS", DatabaseInformationMain.CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x52].Name, false, 20);
                int[] columns = new int[3];
                columns[1] = 1;
                columns[2] = 2;
                t.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            int num10 = 9;
            int num11 = 10;
            int num12 = 11;
            int num13 = 12;
            int num14 = 13;
            int num15 = 14;
            int num16 = 15;
            int num17 = 0x10;
            int num18 = 0x11;
            int num19 = 0x12;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(8);
            while (iterator.HasNext())
            {
                TriggerDef def = (TriggerDef) iterator.Next();
                if (session.GetGrantee().IsAccessible(def))
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = def.GetSchemaName().Name;
                    emptyRowData[num3] = def.GetName().Name;
                    emptyRowData[num4] = def.GetEventTypeString();
                    emptyRowData[num5] = base.database.GetCatalogName().Name;
                    emptyRowData[num6] = def.GetTable().GetSchemaName().Name;
                    emptyRowData[num7] = def.GetTable().GetName().Name;
                    emptyRowData[num8] = def.GetTable().GetTriggerIndex(def.GetName().Name).ToString();
                    emptyRowData[num9] = def.GetConditionSql();
                    emptyRowData[num10] = def.GetProcedureSql();
                    emptyRowData[num18] = def.GetSql();
                    emptyRowData[num19] = def.GetUpdateColumnsSql();
                    emptyRowData[num11] = def.GetActionOrientationString();
                    emptyRowData[num12] = def.GetActionTimingString();
                    emptyRowData[num13] = def.GetOldTransitionTableName();
                    emptyRowData[num14] = def.GetNewTransitionTableName();
                    emptyRowData[num15] = def.GetOldTransitionRowName();
                    emptyRowData[num16] = def.GetNewTransitionRowName();
                    emptyRowData[num17] = null;
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table UDT_PRIVILEGES(Session session)
        {
            Table t = base.SysTables[0x54];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x54]);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "UDT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "PRIVILEGE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x54].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(12);
            OrderedHashSet<Grantee> granteeAndAllRolesWithPublic = session.GetGrantee().GetGranteeAndAllRolesWithPublic();
            while (iterator.HasNext())
            {
                ISchemaObject obj2 = (ISchemaObject) iterator.Next();
                if (obj2.GetSchemaObjectType() == 12)
                {
                    for (int i = 0; i < granteeAndAllRolesWithPublic.Size(); i++)
                    {
                        Grantee local1 = granteeAndAllRolesWithPublic.Get(i);
                        OrderedHashSet<Right> allDirectPrivileges = local1.GetAllDirectPrivileges(obj2);
                        OrderedHashSet<Right> allGrantedPrivileges = local1.GetAllGrantedPrivileges(obj2);
                        if (!allGrantedPrivileges.IsEmpty())
                        {
                            allGrantedPrivileges.AddAll(allDirectPrivileges);
                            allDirectPrivileges = allGrantedPrivileges;
                        }
                        for (int j = 0; j < allDirectPrivileges.Size(); j++)
                        {
                            Right right = allDirectPrivileges.Get(j);
                            Right grantableRights = right.GetGrantableRights();
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = right.GetGrantor().GetName().Name;
                            emptyRowData[num2] = right.GetGrantee().GetName().Name;
                            emptyRowData[num3] = base.database.GetCatalogName().Name;
                            emptyRowData[num4] = obj2.GetSchemaName().Name;
                            emptyRowData[num5] = obj2.GetName().Name;
                            emptyRowData[num6] = "USAGE";
                            emptyRowData[num7] = ((right.GetGrantee() == obj2.GetOwner()) || grantableRights.IsFull) ? "YES" : "NO";
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                            catch (CoreException)
                            {
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table USAGE_PRIVILEGES(Session session)
        {
            Table t = base.SysTables[0x55];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x55]);
                DatabaseInformationMain.AddColumn(t, "GRANTOR", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "GRANTEE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "OBJECT_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "PRIVILEGE_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_GRANTABLE", DatabaseInformationMain.YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x55].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = new WrapperIterator<object>(base.database.schemaManager.DatabaseObjectIterator(7), base.database.schemaManager.DatabaseObjectIterator(15));
            iterator = new WrapperIterator<object>(iterator, base.database.schemaManager.DatabaseObjectIterator(14));
            iterator = new WrapperIterator<object>(iterator, base.database.schemaManager.DatabaseObjectIterator(13));
            OrderedHashSet<Grantee> granteeAndAllRolesWithPublic = session.GetGrantee().GetGranteeAndAllRolesWithPublic();
            while (iterator.HasNext())
            {
                ISchemaObject obj2 = (ISchemaObject) iterator.Next();
                for (int i = 0; i < granteeAndAllRolesWithPublic.Size(); i++)
                {
                    Grantee local1 = granteeAndAllRolesWithPublic.Get(i);
                    OrderedHashSet<Right> allDirectPrivileges = local1.GetAllDirectPrivileges(obj2);
                    OrderedHashSet<Right> allGrantedPrivileges = local1.GetAllGrantedPrivileges(obj2);
                    if (!allGrantedPrivileges.IsEmpty())
                    {
                        allGrantedPrivileges.AddAll(allDirectPrivileges);
                        allDirectPrivileges = allGrantedPrivileges;
                    }
                    for (int j = 0; j < allDirectPrivileges.Size(); j++)
                    {
                        Right right = allDirectPrivileges.Get(j);
                        Right grantableRights = right.GetGrantableRights();
                        object[] emptyRowData = t.GetEmptyRowData();
                        emptyRowData[index] = right.GetGrantor().GetName().Name;
                        emptyRowData[num2] = right.GetGrantee().GetName().Name;
                        emptyRowData[num3] = base.database.GetCatalogName().Name;
                        emptyRowData[num4] = obj2.GetSchemaName().Name;
                        emptyRowData[num5] = obj2.GetName().Name;
                        emptyRowData[num6] = SchemaObjectSet.GetName(obj2.GetName().type);
                        emptyRowData[num7] = "USAGE";
                        emptyRowData[num8] = ((right.GetGrantee() == obj2.GetOwner()) || grantableRights.IsFull) ? "YES" : "NO";
                        try
                        {
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                        catch (CoreException)
                        {
                        }
                    }
                }
            }
            return t;
        }

        private Table USER_DEFINED_TYPES(Session session)
        {
            Table t = base.SysTables[0x56];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x56]);
                DatabaseInformationMain.AddColumn(t, "USER_DEFINED_TYPE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "USER_DEFINED_TYPE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "USER_DEFINED_TYPE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "USER_DEFINED_TYPE_CATEGORY", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "IS_INSTANTIABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IS_FINAL", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "ORDERING_FORM", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ORDERING_CATEGORY", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ORDERING_ROUTINE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ORDERING_ROUTINE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "ORDERING_ROUTINE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "REFERENCE_TYPE", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_MAXIMUM_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_OCTET_LENGTH", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "CHARACTER_SET_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLLATION_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_PRECISION_RADIX", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DATETIME_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "INTERVAL_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "SOURCE_DTD_IDENTIFIER", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "REF_DTD_IDENTIFIER", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DECLARED_DATA_TYPE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_PRECISION", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "DECLARED_NUMERIC_SCALE", DatabaseInformationMain.CardinalNumber);
                DatabaseInformationMain.AddColumn(t, "EXTERNAL_NAME", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "EXTERNAL_LANGUAGE", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "JAVA_INTERFACE", DatabaseInformationMain.CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x56].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 4, 5, 6 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 12;
            int num9 = 13;
            int num10 = 14;
            int num11 = 15;
            int num12 = 0x10;
            int num13 = 0x11;
            int num14 = 0x12;
            int num15 = 0x13;
            int num16 = 20;
            int num17 = 0x15;
            int num18 = 0x16;
            int num19 = 0x17;
            int num20 = 0x18;
            int num21 = 0x19;
            int num22 = 0x1a;
            int num23 = 0x1b;
            int num24 = 0x1d;
            int num25 = 30;
            int num26 = 0x1f;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(12);
            while (iterator.HasNext())
            {
                SqlType type = (SqlType) iterator.Next();
                if (type.IsDistinctType())
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = type.GetSchemaName().Name;
                    emptyRowData[num3] = type.GetName().Name;
                    emptyRowData[num8] = type.GetFullNameString();
                    emptyRowData[num24] = type.GetFullNameString();
                    emptyRowData[num4] = "DISTINCT";
                    emptyRowData[num5] = "YES";
                    emptyRowData[num6] = "YES";
                    emptyRowData[num7] = "FULL";
                    if (type.IsCharacterType())
                    {
                        CharacterType type2 = (CharacterType) type;
                        emptyRowData[num9] = type.Precision;
                        emptyRowData[num10] = type.Precision * 2L;
                        emptyRowData[num11] = base.database.GetCatalogName().Name;
                        emptyRowData[num12] = type2.GetCharacterSet().GetSchemaName().Name;
                        emptyRowData[num13] = type2.GetCharacterSet().GetName().Name;
                        emptyRowData[num14] = base.database.GetCatalogName().Name;
                        emptyRowData[num15] = type2.GetCollation().GetSchemaName().Name;
                        emptyRowData[num16] = type2.GetCollation().GetName().Name;
                    }
                    else if (type.IsNumberType())
                    {
                        NumberType type3 = (NumberType) type;
                        emptyRowData[num17] = type3.GetNumericPrecisionInRadix();
                        emptyRowData[num25] = type3.GetNumericPrecisionInRadix();
                        if (type.IsExactNumberType())
                        {
                            emptyRowData[num19] = emptyRowData[num26] = type.GetAdoScale();
                        }
                        emptyRowData[num18] = type.GetPrecisionRadix();
                    }
                    else if (!type.IsBooleanType())
                    {
                        if (type.IsDateTimeType())
                        {
                            emptyRowData[num20] = type.GetAdoScale();
                        }
                        else if (type.IsIntervalType())
                        {
                            emptyRowData[num8] = "INTERVAL";
                            emptyRowData[num21] = IntervalType.GetQualifier(type.TypeCode);
                            emptyRowData[num22] = type.Precision;
                            emptyRowData[num20] = type.GetAdoScale();
                        }
                        else if (type.IsBinaryType())
                        {
                            emptyRowData[num9] = type.Precision;
                            emptyRowData[num10] = type.Precision;
                        }
                    }
                    emptyRowData[num23] = emptyRowData[num3];
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table VIEW_COLUMN_USAGE(Session session)
        {
            Table t = base.SysTables[0x57];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x57]);
                DatabaseInformationMain.AddColumn(t, "VIEW_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "VIEW_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "VIEW_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "COLUMN_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x57].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 6 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table3 = (Table) iterator.Next();
                if (table3.IsView() && session.GetGrantee().IsFullyAccessibleByRole(table3.GetName()))
                {
                    string name = base.database.GetCatalogName().Name;
                    string str2 = table3.GetSchemaName().Name;
                    string str3 = table3.GetName().Name;
                    Iterator<QNameManager.QName> iterator2 = ((View) table3).GetReferences().GetIterator();
                    while (iterator2.HasNext())
                    {
                        QNameManager.QName name2 = iterator2.Next();
                        if (name2.type == 9)
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = name;
                            emptyRowData[num2] = str2;
                            emptyRowData[num3] = str3;
                            emptyRowData[num4] = name;
                            emptyRowData[num5] = name2.Parent.schema.Name;
                            emptyRowData[num6] = name2.Parent.Name;
                            emptyRowData[num7] = name2.Name;
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                                continue;
                            }
                            catch (CoreException)
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table VIEW_ROUTINE_USAGE(Session session)
        {
            Table t = base.SysTables[0x58];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x58]);
                DatabaseInformationMain.AddColumn(t, "VIEW_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "VIEW_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "VIEW_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "SPECIFIC_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x58].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table3 = (Table) iterator.Next();
                if (table3.IsView() && session.GetGrantee().IsFullyAccessibleByRole(table3.GetName()))
                {
                    OrderedHashSet<QNameManager.QName> references = table3.GetReferences();
                    for (int i = 0; i < references.Size(); i++)
                    {
                        QNameManager.QName name = references.Get(i);
                        if (session.GetGrantee().IsFullyAccessibleByRole(name) && (name.type == 0x18))
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = table3.GetSchemaName().Name;
                            emptyRowData[num3] = table3.GetName().Name;
                            emptyRowData[num4] = base.database.GetCatalogName().Name;
                            emptyRowData[num5] = name.schema.Name;
                            emptyRowData[num6] = name.Name;
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                            catch (CoreException)
                            {
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table VIEW_TABLE_USAGE(Session session)
        {
            Table t = base.SysTables[0x59];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[0x59]);
                DatabaseInformationMain.AddColumn(t, "VIEW_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "VIEW_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "VIEW_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[0x59].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5 }, false);
                return t;
            }
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table3 = (Table) iterator.Next();
                if (table3.IsView() && session.GetGrantee().IsFullyAccessibleByRole(table3.GetName()))
                {
                    OrderedHashSet<QNameManager.QName> references = table3.GetReferences();
                    for (int i = 0; i < references.Size(); i++)
                    {
                        QNameManager.QName name = references.Get(i);
                        if (session.GetGrantee().IsFullyAccessibleByRole(name) && (name.type == 3))
                        {
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = base.database.GetCatalogName().Name;
                            emptyRowData[num2] = table3.GetSchemaName().Name;
                            emptyRowData[num3] = table3.GetName().Name;
                            emptyRowData[num4] = base.database.GetCatalogName().Name;
                            emptyRowData[num5] = name.schema.Name;
                            emptyRowData[num6] = name.Name;
                            try
                            {
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                            catch (CoreException)
                            {
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table VIEWS(Session session)
        {
            Table t = base.SysTables[90];
            if (t == null)
            {
                t = base.CreateBlankTable(DatabaseInformationMain.SysTableQNames[90]);
                DatabaseInformationMain.AddColumn(t, "TABLE_CATALOG", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_SCHEMA", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "TABLE_NAME", DatabaseInformationMain.SqlIdentifier);
                DatabaseInformationMain.AddColumn(t, "VIEW_DEFINITION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "CHECK_OPTION", DatabaseInformationMain.CharacterData);
                DatabaseInformationMain.AddColumn(t, "IS_UPDATABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "INSERTABLE_INTO", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IS_TRIGGER_UPDATABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IS_TRIGGER_DELETABLE", DatabaseInformationMain.YesOrNo);
                DatabaseInformationMain.AddColumn(t, "IS_TRIGGER_INSERTABLE_INTO", DatabaseInformationMain.YesOrNo);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(DatabaseInformationMain.SysTableQNames[90].Name, false, 20);
                int[] columns = new int[3];
                columns[0] = 1;
                columns[1] = 2;
                table3.CreatePrimaryKeyConstraint(indexName, columns, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            int num7 = 6;
            int num8 = 7;
            int num9 = 8;
            int num10 = 9;
            Iterator<object> iterator = base.AllTables();
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (((table.GetSchemaName() == SqlInvariants.InformationSchemaQname) || table.IsView()) && DatabaseInformationMain.IsAccessibleTable(session, table))
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = table.GetSchemaName().Name;
                    emptyRowData[num3] = table.GetName().Name;
                    string str = "NONE";
                    View view = table as View;
                    if (view != null)
                    {
                        if (session.GetGrantee().IsFullyAccessibleByRole(table.GetName()))
                        {
                            emptyRowData[num4] = view.GetStatement();
                        }
                        switch (view.GetCheckOption())
                        {
                            case 1:
                                str = "LOCAL";
                                break;

                            case 2:
                                str = "CASCADED";
                                break;
                        }
                    }
                    emptyRowData[num5] = str;
                    emptyRowData[num6] = table.IsUpdatable() ? "YES" : "NO";
                    emptyRowData[num7] = table.IsInsertable() ? "YES" : "NO";
                    emptyRowData[num8] = null;
                    emptyRowData[num9] = null;
                    emptyRowData[num10] = null;
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }
    }
}

