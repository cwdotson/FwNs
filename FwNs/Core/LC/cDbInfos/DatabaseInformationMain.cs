namespace FwNs.Core.LC.cDbInfos
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public class DatabaseInformationMain : DatabaseInformation
    {
        public static SqlType CardinalNumber = SqlInvariants.CardinalNumber;
        public static SqlType YesOrNo = SqlInvariants.YesOrNo;
        public static SqlType CharacterData = SqlInvariants.CharacterData;
        public static SqlType SqlIdentifier = SqlInvariants.SqlIdentifier;
        public static SqlType TimeStamp = SqlInvariants.TimeStamp;
        private static readonly object DatabaseInformationMainLock = new object();
        protected static bool[] SysTableSessionDependent = new bool[DatabaseInformation.sysTableNames.Length];
        protected static QNameManager.QName[] SysTableQNames = GetSysTableQNames();
        protected static HashSet<string> NonCachedTablesSet = GetNonCachedTablesSet();
        protected static string[] TableTypes = new string[] { "GLOBAL TEMPORARY", "SYSTEM TABLE", "TABLE", "VIEW" };
        protected Table[] SysTables;

        public DatabaseInformationMain(Database db) : base(db)
        {
            this.SysTables = new Table[DatabaseInformation.sysTableNames.Length];
        }

        protected static void AddColumn(Table t, string name, SqlType type)
        {
            ColumnSchema column = new ColumnSchema(QNameManager.NewInfoSchemaColumnName(name, t.GetName()), type, true, false, null);
            t.AddColumn(column);
        }

        protected Iterator<object> AllTables()
        {
            return new WrapperIterator<object>(base.database.schemaManager.DatabaseObjectIterator(3), new WrapperIterator<object>(this.SysTables, true));
        }

        protected void CacheClear(Session session)
        {
            int length = this.SysTables.Length;
            while (length-- > 0)
            {
                Table table = this.SysTables[length];
                if (table != null)
                {
                    table.ClearAllData(session);
                }
            }
        }

        private Table COLUMN_PRIVILEGES(Session session)
        {
            Table t = this.SysTables[0x1f];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[0x1f]);
                AddColumn(t, "GRANTOR", SqlIdentifier);
                AddColumn(t, "GRANTEE", SqlIdentifier);
                AddColumn(t, "TABLE_CATALOG", SqlIdentifier);
                AddColumn(t, "TABLE_SCHEMA", SqlIdentifier);
                AddColumn(t, "TABLE_NAME", SqlIdentifier);
                AddColumn(t, "COLUMN_NAME", SqlIdentifier);
                AddColumn(t, "PRIVILEGE_TYPE", CharacterData);
                AddColumn(t, "IS_GRANTABLE", YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[0x1f].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 2, 3, 4, 5, 6, 1, 0 }, false);
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
            OrderedHashSet<Grantee> granteeAndAllRolesWithPublic = session.GetGrantee().GetGranteeAndAllRolesWithPublic();
            Iterator<object> iterator = this.AllTables();
            while (iterator.HasNext())
            {
                Table table3 = (Table) iterator.Next();
                string name = table3.GetName().Name;
                string str2 = base.database.GetCatalogName().Name;
                string str3 = table3.GetSchemaName().Name;
                for (int i = 0; i < granteeAndAllRolesWithPublic.Size(); i++)
                {
                    Grantee local1 = granteeAndAllRolesWithPublic.Get(i);
                    OrderedHashSet<Right> allDirectPrivileges = local1.GetAllDirectPrivileges(table3);
                    OrderedHashSet<Right> allGrantedPrivileges = local1.GetAllGrantedPrivileges(table3);
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
                            OrderedHashSet<string> columnsForPrivilege = right.GetColumnsForPrivilege(table3, Right.PrivilegeTypes[k]);
                            OrderedHashSet<string> set5 = grantableRights.GetColumnsForPrivilege(table3, Right.PrivilegeTypes[k]);
                            for (int m = 0; m < columnsForPrivilege.Size(); m++)
                            {
                                string key = columnsForPrivilege.Get(m);
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[index] = right.GetGrantor().GetName().Name;
                                emptyRowData[num2] = right.GetGrantee().GetName().Name;
                                emptyRowData[num3] = str2;
                                emptyRowData[num4] = str3;
                                emptyRowData[num5] = name;
                                emptyRowData[num6] = key;
                                emptyRowData[num7] = Right.PrivilegeNames[k];
                                emptyRowData[num8] = ((right.GetGrantee() == table3.GetOwner()) || set5.Contains(key)) ? "YES" : "NO";
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

        protected Table CreateBlankTable(QNameManager.QName name)
        {
            return new Table(base.database, name, 1);
        }

        private Table DUAL(Session session)
        {
            Table t = this.SysTables[0x5c];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[0x5c]);
                AddColumn(t, "DUMMY", SqlType.SqlChar);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[0x5c].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            object[] emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "X";
            Table.InsertSys(session.sessionData.GetRowStore(t), emptyRowData);
            return t;
        }

        protected virtual Table GenerateTable(Session session, int tableIndex)
        {
            Table table1 = this.SysTables[tableIndex];
            if (tableIndex <= 40)
            {
                switch (tableIndex)
                {
                    case 0:
                        return this.SYSTEM_BESTROWIDENTIFIER(session);

                    case 1:
                        return this.SYSTEM_COLUMNS(session);

                    case 2:
                        return this.SYSTEM_CROSSREFERENCE(session);

                    case 3:
                        return this.SYSTEM_INDEXINFO(session);

                    case 4:
                        return this.SYSTEM_PRIMARYKEYS(session);

                    case 7:
                        return this.SYSTEM_SCHEMAS(session);

                    case 8:
                        return this.SYSTEM_TABLES(session);

                    case 10:
                        return this.SYSTEM_TYPEINFO(session);

                    case 12:
                        return this.SYSTEM_USERS(session);

                    case 14:
                        return this.SYSTEM_SEQUENCES(session);

                    case 0x10:
                        return this.SYSTEM_CONNECTION_PROPERTIES(session);

                    case 0x1f:
                        return this.COLUMN_PRIVILEGES(session);

                    case 40:
                        return this.INFORMATION_SCHEMA_CATALOG_NAME(session);
                }
            }
            else
            {
                if (tableIndex == 0x42)
                {
                    return this.SEQUENCES(session);
                }
                if (tableIndex == 0x4a)
                {
                    return this.TABLE_PRIVILEGES(session);
                }
                if (tableIndex != 0x5b)
                {
                    if (tableIndex == 0x5c)
                    {
                        return this.DUAL(session);
                    }
                }
                else
                {
                    return this.SYSTEM_CATALOGS(session);
                }
            }
            return null;
        }

        private static HashSet<string> GetNonCachedTablesSet()
        {
            lock (DatabaseInformationMainLock)
            {
                return new HashSet<string> { 
                    "SYSTEM_CACHEINFO",
                    "SYSTEM_SESSIONINFO",
                    "SYSTEM_SESSIONS",
                    "SYSTEM_PROPERTIES",
                    "SYSTEM_SEQUENCES"
                };
            }
        }

        private static QNameManager.QName[] GetSysTableQNames()
        {
            lock (DatabaseInformationMainLock)
            {
                QNameManager.QName[] nameArray2 = new QNameManager.QName[DatabaseInformation.sysTableNames.Length];
                for (int i = 0; i < DatabaseInformation.sysTableNames.Length; i++)
                {
                    nameArray2[i] = QNameManager.NewInfoSchemaTableName(DatabaseInformation.sysTableNames[i]);
                    nameArray2[i].schema = SqlInvariants.InformationSchemaQname;
                    SysTableSessionDependent[i] = true;
                }
                return nameArray2;
            }
        }

        public override Table GetSystemTable(Session session, string name)
        {
            lock (this)
            {
                if (!DatabaseInformation.IsSystemTable(name))
                {
                    return null;
                }
                int sysTableId = DatabaseInformation.GetSysTableId(name);
                Table table = this.SysTables[sysTableId];
                if (table == null)
                {
                    return table;
                }
                if (!session.IsAdmin())
                {
                    session.GetUser();
                }
                long schemaChangeTimestamp = base.database.schemaManager.GetSchemaChangeTimestamp();
                IPersistentStore rowStore = session.sessionData.GetRowStore(table);
                if (((schemaChangeTimestamp != 0) && (rowStore.GetTimestamp() == schemaChangeTimestamp)) && !NonCachedTablesSet.Contains(name))
                {
                    return table;
                }
                table.ClearAllData(session);
                rowStore.SetTimestamp(schemaChangeTimestamp);
                return this.GenerateTable(session, sysTableId);
            }
        }

        private Table INFORMATION_SCHEMA_CATALOG_NAME(Session session)
        {
            Table t = this.SysTables[40];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[40]);
                AddColumn(t, "CATALOG_NAME", SqlIdentifier);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[40].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            object[] emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = base.database.GetCatalogName().Name;
            Table.InsertSys(session.sessionData.GetRowStore(t), emptyRowData);
            return t;
        }

        public void Init(Session session)
        {
            for (int i = 0; i < this.SysTables.Length; i++)
            {
                Table table = this.SysTables[i] = this.GenerateTable(session, i);
                if (table != null)
                {
                    table.SetDataReadOnly(true);
                }
            }
            GranteeManager granteeManager = base.database.GetGranteeManager();
            Right fullRights = new Right();
            fullRights.Set(1, null);
            for (int j = 0; j < SysTableQNames.Length; j++)
            {
                if (this.SysTables[j] != null)
                {
                    granteeManager.GrantSystemToPublic(this.SysTables[j], fullRights);
                }
            }
            fullRights = Right.FullRights;
            granteeManager.GrantSystemToPublic(SqlInvariants.YesOrNo, fullRights);
            granteeManager.GrantSystemToPublic(SqlInvariants.TimeStamp, fullRights);
            granteeManager.GrantSystemToPublic(SqlInvariants.CardinalNumber, fullRights);
            granteeManager.GrantSystemToPublic(SqlInvariants.CharacterData, fullRights);
            granteeManager.GrantSystemToPublic(SqlInvariants.SqlCharacter, fullRights);
            granteeManager.GrantSystemToPublic(SqlInvariants.SqlIdentifierCharset, fullRights);
            granteeManager.GrantSystemToPublic(SqlInvariants.SqlIdentifier, fullRights);
            granteeManager.GrantSystemToPublic(SqlInvariants.SqlText, fullRights);
        }

        protected static bool IsAccessibleTable(Session sesion, Table table)
        {
            return sesion.GetGrantee().IsAccessible(table);
        }

        protected Table SEQUENCES(Session session)
        {
            Table t = this.SysTables[0x42];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[0x42]);
                AddColumn(t, "SEQUENCE_CATALOG", SqlIdentifier);
                AddColumn(t, "SEQUENCE_SCHEMA", SqlIdentifier);
                AddColumn(t, "SEQUENCE_NAME", SqlIdentifier);
                AddColumn(t, "DATA_TYPE", CharacterData);
                AddColumn(t, "NUMERIC_PRECISION", CardinalNumber);
                AddColumn(t, "NUMERIC_PRECISION_RADIX", CardinalNumber);
                AddColumn(t, "NUMERIC_SCALE", CardinalNumber);
                AddColumn(t, "MAXIMUM_VALUE", CharacterData);
                AddColumn(t, "MINIMUM_VALUE", CharacterData);
                AddColumn(t, "INCREMENT", CharacterData);
                AddColumn(t, "CYCLE_OPTION", YesOrNo);
                AddColumn(t, "DECLARED_DATA_TYPE", CharacterData);
                AddColumn(t, "DECLARED_NUMERIC_PRECISION", CardinalNumber);
                AddColumn(t, "DECLARED_NUMERIC_SCLAE", CardinalNumber);
                AddColumn(t, "START_WITH", CharacterData);
                AddColumn(t, "NEXT_VALUE", CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[0x42].Name, false, 20);
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
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(7);
            while (iterator.HasNext())
            {
                NumberSequence sequence = (NumberSequence) iterator.Next();
                if (session.GetGrantee().IsAccessible(sequence))
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    NumberType dataType = (NumberType) sequence.GetDataType();
                    int num17 = ((dataType.TypeCode == 2) || (dataType.TypeCode == 3)) ? 10 : 2;
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = sequence.GetSchemaName().Name;
                    emptyRowData[num3] = sequence.GetName().Name;
                    emptyRowData[num4] = sequence.GetDataType().GetFullNameString();
                    emptyRowData[num5] = dataType.GetPrecision();
                    emptyRowData[num6] = num17;
                    emptyRowData[num7] = 0L;
                    emptyRowData[num8] = sequence.GetMaxValue().ToString();
                    emptyRowData[num9] = sequence.GetMinValue().ToString();
                    emptyRowData[num10] = sequence.GetIncrement().ToString();
                    emptyRowData[num11] = sequence.IsCycle() ? "YES" : "NO";
                    emptyRowData[num12] = emptyRowData[num4];
                    emptyRowData[num13] = emptyRowData[num5];
                    emptyRowData[num14] = emptyRowData[num14];
                    emptyRowData[num15] = sequence.GetStartValue().ToString();
                    emptyRowData[num16] = sequence.Peek().ToString();
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table SYSTEM_BESTROWIDENTIFIER(Session session)
        {
            Table t = this.SysTables[0];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[0]);
                AddColumn(t, "SCOPE", SqlType.SqlSmallint);
                AddColumn(t, "COLUMN_NAME", SqlIdentifier);
                AddColumn(t, "DATA_TYPE", SqlType.SqlSmallint);
                AddColumn(t, "TYPE_NAME", SqlIdentifier);
                AddColumn(t, "COLUMN_SIZE", SqlType.SqlInteger);
                AddColumn(t, "BUFFER_LENGTH", SqlType.SqlInteger);
                AddColumn(t, "DECIMAL_DIGITS", SqlType.SqlSmallint);
                AddColumn(t, "PSEUDO_COLUMN", SqlType.SqlSmallint);
                AddColumn(t, "TABLE_CAT", SqlIdentifier);
                AddColumn(t, "TABLE_SCHEM", SqlIdentifier);
                AddColumn(t, "TABLE_NAME", SqlIdentifier);
                AddColumn(t, "NULLABLE", SqlType.SqlSmallint);
                AddColumn(t, "IN_KEY", SqlType.SqlBoolean);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[0].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 8, 9, 10, 1 }, false);
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
            DITableInfo info = new DITableInfo();
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            bool flag = base.database.GetProperties().IsPropertyTrue("jdbc.translate_dti_types");
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (!table.IsView() && IsAccessibleTable(session, table))
                {
                    int[] bestRowIdentifiers = table.GetBestRowIdentifiers();
                    if (bestRowIdentifiers != null)
                    {
                        info.SetTable(table);
                        bool flag2 = table.IsBestRowIdentifiersStrict();
                        string name = table.GetCatalogName().Name;
                        string str2 = table.GetSchemaName().Name;
                        string str3 = table.GetName().Name;
                        SqlType[] columnTypes = table.GetColumnTypes();
                        int briScope = info.GetBriScope();
                        int briPseudo = info.GetBriPseudo();
                        for (int i = 0; i < bestRowIdentifiers.Length; i++)
                        {
                            ColumnSchema column = table.GetColumn(i);
                            SqlType characterType = columnTypes[i];
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
                            object[] emptyRowData = t.GetEmptyRowData();
                            emptyRowData[index] = briScope;
                            emptyRowData[num2] = column.GetName().Name;
                            emptyRowData[num3] = characterType.GetAdoTypeCode();
                            emptyRowData[num4] = characterType.GetNameString();
                            emptyRowData[num5] = columnTypes[i].GetAdoPrecision();
                            emptyRowData[num6] = null;
                            emptyRowData[num7] = characterType.GetAdoScale();
                            emptyRowData[num8] = briPseudo;
                            emptyRowData[num9] = name;
                            emptyRowData[num10] = str2;
                            emptyRowData[num11] = str3;
                            emptyRowData[num12] = column.GetNullability();
                            emptyRowData[num13] = flag2;
                            Table.InsertSys(rowStore, emptyRowData);
                        }
                    }
                }
            }
            return t;
        }

        private Table SYSTEM_CATALOGS(Session session)
        {
            Table t = this.SysTables[0x5b];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[0x5b]);
                AddColumn(t, "SYSTEM_CATALOG_NAME", SqlIdentifier);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[0x5b].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            object[] emptyRowData = t.GetEmptyRowData();
            emptyRowData[0] = "PUBLIC";
            Table.InsertSys(session.sessionData.GetRowStore(t), emptyRowData);
            return t;
        }

        private Table SYSTEM_COLUMNS(Session session)
        {
            Table t = this.SysTables[1];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[1]);
                AddColumn(t, "TABLE_CAT", SqlIdentifier);
                AddColumn(t, "TABLE_SCHEM", SqlIdentifier);
                AddColumn(t, "TABLE_NAME", SqlIdentifier);
                AddColumn(t, "COLUMN_NAME", SqlIdentifier);
                AddColumn(t, "DATA_TYPE", SqlType.SqlSmallint);
                AddColumn(t, "TYPE_NAME", SqlIdentifier);
                AddColumn(t, "COLUMN_SIZE", SqlType.SqlInteger);
                AddColumn(t, "BUFFER_LENGTH", SqlType.SqlInteger);
                AddColumn(t, "DECIMAL_DIGITS", SqlType.SqlInteger);
                AddColumn(t, "NUM_PREC_RADIX", SqlType.SqlInteger);
                AddColumn(t, "NULLABLE", SqlType.SqlInteger);
                AddColumn(t, "REMARKS", CharacterData);
                AddColumn(t, "COLUMN_DEF", CharacterData);
                AddColumn(t, "SQL_DATA_TYPE", SqlType.SqlInteger);
                AddColumn(t, "SQL_DATETIME_SUB", SqlType.SqlInteger);
                AddColumn(t, "CHAR_OCTET_LENGTH", SqlType.SqlInteger);
                AddColumn(t, "ORDINAL_POSITION", SqlType.SqlInteger);
                AddColumn(t, "IS_NULLABLE", SqlType.SqlBoolean);
                AddColumn(t, "SCOPE_CATLOG", SqlIdentifier);
                AddColumn(t, "SCOPE_SCHEMA", SqlIdentifier);
                AddColumn(t, "SCOPE_TABLE", SqlIdentifier);
                AddColumn(t, "SOURCE_DATA_TYPE", SqlIdentifier);
                AddColumn(t, "IS_AUTOINCREMENT", SqlType.SqlBoolean);
                AddColumn(t, "TYPE_SUB", SqlType.SqlInteger);
                AddColumn(t, "IS_PRIMARY_KEY", SqlType.SqlBoolean);
                AddColumn(t, "IDENTITY_START", SqlType.SqlBigint);
                AddColumn(t, "IDENTITY_INCREMENT", SqlType.SqlBigint);
                AddColumn(t, "IS_GENERATED", SqlType.SqlBoolean);
                AddColumn(t, "GENERATION_EXPRESSION", CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[1].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 0x10 }, false);
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
            int num8 = 8;
            int num9 = 9;
            int num10 = 10;
            int num11 = 11;
            int num12 = 12;
            int num13 = 13;
            int num14 = 14;
            int num15 = 15;
            int num16 = 0x10;
            int num17 = 0x11;
            int num18 = 0x15;
            int num19 = 0x16;
            int num20 = 0x17;
            int num21 = 0x18;
            int num22 = 0x19;
            int num23 = 0x1a;
            int num24 = 0x1b;
            int num25 = 0x1c;
            Iterator<object> iterator = this.AllTables();
            DITableInfo info = new DITableInfo();
            bool flag = base.database.GetProperties().IsPropertyTrue("jdbc.translate_dti_types");
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (IsAccessibleTable(session, table))
                {
                    info.SetTable(table);
                    string name = table.GetCatalogName().Name;
                    string str2 = table.GetSchemaName().Name;
                    string str3 = table.GetName().Name;
                    int columnCount = table.GetColumnCount();
                    for (int i = 0; i < columnCount; i++)
                    {
                        ColumnSchema column = table.GetColumn(i);
                        SqlType dataType = column.GetDataType();
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
                        object[] emptyRowData = t.GetEmptyRowData();
                        emptyRowData[index] = name;
                        emptyRowData[num2] = str2;
                        emptyRowData[num3] = str3;
                        emptyRowData[num4] = column.GetName().Name;
                        emptyRowData[num5] = dataType.GetAdoTypeCode();
                        emptyRowData[num6] = dataType.GetNameString();
                        emptyRowData[num7] = 0;
                        emptyRowData[num15] = 0;
                        if (dataType.IsDomainType())
                        {
                            emptyRowData[num18] = dataType.userTypeModifier.GetName().Name;
                        }
                        if (dataType.IsCharacterType())
                        {
                            emptyRowData[num7] = dataType.GetAdoPrecision();
                            emptyRowData[num15] = dataType.GetAdoPrecision();
                        }
                        if (dataType.IsBinaryType())
                        {
                            emptyRowData[num7] = dataType.GetAdoPrecision();
                            emptyRowData[num15] = dataType.GetAdoPrecision();
                        }
                        if (dataType.IsNumberType())
                        {
                            emptyRowData[num7] = ((NumberType) dataType).GetNumericPrecisionInRadix();
                            emptyRowData[num9] = dataType.GetPrecisionRadix();
                            if (dataType.IsExactNumberType())
                            {
                                emptyRowData[num8] = dataType.GetAdoScale();
                            }
                        }
                        if (dataType.IsDateTimeType())
                        {
                            emptyRowData[num7] = column.GetDataType().DisplaySize();
                            emptyRowData[num14] = ((DateTimeType) dataType).GetSqlDateTimeSub();
                        }
                        emptyRowData[num9] = dataType.GetPrecisionRadix();
                        emptyRowData[num10] = column.GetNullability();
                        emptyRowData[num11] = info.GetColRemarks(i);
                        emptyRowData[num12] = column.GetDefaultSql();
                        emptyRowData[num13] = dataType.TypeCode;
                        emptyRowData[num16] = i + 1;
                        emptyRowData[num17] = column.GetNullability() > 0;
                        emptyRowData[num19] = column.IsIdentity();
                        emptyRowData[num20] = info.GetColDataTypeSub(i);
                        emptyRowData[num21] = column.IsPrimaryKey();
                        if (column.IsIdentity())
                        {
                            NumberSequence identitySequence = column.GetIdentitySequence();
                            emptyRowData[num22] = identitySequence.GetStartValue();
                            emptyRowData[num23] = identitySequence.GetIncrement();
                        }
                        emptyRowData[num24] = column.IsGenerated();
                        if (column.IsGenerated())
                        {
                            emptyRowData[num25] = column.GetGeneratingExpression().GetSql();
                        }
                        Table.InsertSys(rowStore, emptyRowData);
                    }
                }
            }
            return t;
        }

        private Table SYSTEM_CONNECTION_PROPERTIES(Session session)
        {
            Table t = this.SysTables[0x10];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[0x10]);
                AddColumn(t, "NAME", SqlIdentifier);
                AddColumn(t, "MAX_LEN", SqlType.SqlInteger);
                AddColumn(t, "DEFAULT_VALUE", SqlIdentifier);
                AddColumn(t, "DESCRIPTION", SqlIdentifier);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[0x10].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            foreach (object[] objArray in LibCoreDatabaseProperties.GetPropertiesMetaIterator())
            {
                object[] objArray2;
                int num5 = Convert.ToInt32(objArray[1]);
                if (num5 == 1)
                {
                    if ("readonly".Equals(objArray[0]) || "LibCore.files_readonly".Equals(objArray[0]))
                    {
                        goto Label_00FB;
                    }
                    continue;
                }
                if (num5 != 2)
                {
                    continue;
                }
            Label_00FB:
                objArray2 = t.GetEmptyRowData();
                object obj2 = objArray[4];
                objArray2[index] = objArray[0];
                objArray2[num2] = 8;
                objArray2[num3] = (obj2 == null) ? null : obj2.ToString();
                objArray2[num4] = "see LibCore guide";
                Table.InsertSys(rowStore, objArray2);
            }
            return t;
        }

        private Table SYSTEM_CROSSREFERENCE(Session session)
        {
            Table t = this.SysTables[2];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[2]);
                AddColumn(t, "PKTABLE_CAT", SqlIdentifier);
                AddColumn(t, "PKTABLE_SCHEM", SqlIdentifier);
                AddColumn(t, "PKTABLE_NAME", SqlIdentifier);
                AddColumn(t, "PKCOLUMN_NAME", SqlIdentifier);
                AddColumn(t, "FKTABLE_CAT", SqlIdentifier);
                AddColumn(t, "FKTABLE_SCHEM", SqlIdentifier);
                AddColumn(t, "FKTABLE_NAME", SqlIdentifier);
                AddColumn(t, "FKCOLUMN_NAME", SqlIdentifier);
                AddColumn(t, "KEY_SEQ", SqlType.SqlSmallint);
                AddColumn(t, "UPDATE_RULE", SqlType.SqlSmallint);
                AddColumn(t, "DELETE_RULE", SqlType.SqlSmallint);
                AddColumn(t, "FK_NAME", SqlIdentifier);
                AddColumn(t, "PK_NAME", SqlIdentifier);
                AddColumn(t, "DEFERRABILITY", SqlType.SqlSmallint);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[2].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 4, 5, 6, 8, 11 }, false);
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
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            List<Constraint> list = new List<Constraint>();
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (!table.IsView() && IsAccessibleTable(session, table))
                {
                    Constraint[] constraints = table.GetConstraints();
                    int length = constraints.Length;
                    for (int j = 0; j < length; j++)
                    {
                        Constraint item = constraints[j];
                        if ((item.GetConstraintType() == 0) && IsAccessibleTable(session, item.GetRef()))
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                Constraint constraint2 = list[i];
                Table main = constraint2.GetMain();
                string name = main.GetName().Name;
                Table table5 = constraint2.GetRef();
                string str2 = table5.GetName().Name;
                string str3 = main.GetCatalogName().Name;
                string str4 = main.GetSchemaName().Name;
                string str5 = table5.GetCatalogName().Name;
                string str6 = table5.GetSchemaName().Name;
                int[] mainColumns = constraint2.GetMainColumns();
                int[] refColumns = constraint2.GetRefColumns();
                int length = refColumns.Length;
                string str7 = constraint2.GetRefName().Name;
                string str8 = constraint2.GetMainName().Name;
                int? nullable = 0;
                int? nullable2 = new int?(constraint2.GetDeleteAction());
                int? nullable3 = new int?(constraint2.GetUpdateAction());
                for (int j = 0; j < length; j++)
                {
                    int? nullable4 = new int?(j + 1);
                    string nameString = main.GetColumn(mainColumns[j]).GetNameString();
                    string str10 = table5.GetColumn(refColumns[j]).GetNameString();
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = str3;
                    emptyRowData[num2] = str4;
                    emptyRowData[num3] = name;
                    emptyRowData[num4] = nameString;
                    emptyRowData[num5] = str5;
                    emptyRowData[num6] = str6;
                    emptyRowData[num7] = str2;
                    emptyRowData[num8] = str10;
                    emptyRowData[num9] = nullable4;
                    emptyRowData[num10] = nullable3;
                    emptyRowData[num11] = nullable2;
                    emptyRowData[num12] = str7;
                    emptyRowData[num13] = str8;
                    emptyRowData[num14] = nullable;
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table SYSTEM_INDEXINFO(Session session)
        {
            Table t = this.SysTables[3];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[3]);
                AddColumn(t, "TABLE_CAT", SqlIdentifier);
                AddColumn(t, "TABLE_SCHEM", SqlIdentifier);
                AddColumn(t, "TABLE_NAME", SqlIdentifier);
                AddColumn(t, "NON_UNIQUE", SqlType.SqlBoolean);
                AddColumn(t, "INDEX_QUALIFIER", SqlIdentifier);
                AddColumn(t, "INDEX_NAME", SqlIdentifier);
                AddColumn(t, "TYPE", SqlType.SqlSmallint);
                AddColumn(t, "ORDINAL_POSITION", SqlType.SqlSmallint);
                AddColumn(t, "COLUMN_NAME", SqlIdentifier);
                AddColumn(t, "ASC_OR_DESC", CharacterData);
                AddColumn(t, "CARDINALITY", SqlType.SqlInteger);
                AddColumn(t, "PAGES", SqlType.SqlInteger);
                AddColumn(t, "FILTER_CONDITION", CharacterData);
                AddColumn(t, "ROW_CARDINALITY", SqlType.SqlInteger);
                AddColumn(t, "UNIQUE_INDEX", SqlType.SqlBoolean);
                AddColumn(t, "PRIMARY_INDEX", SqlType.SqlBoolean);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[3].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 0, 1, 2, 3, 4, 5, 7 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int num = 0;
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
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (!table.IsView() && IsAccessibleTable(session, table))
                {
                    string name = table.GetCatalogName().Name;
                    string str2 = table.GetSchemaName().Name;
                    string str3 = table.GetName().Name;
                    string str4 = null;
                    string str5 = name;
                    int indexCount = table.GetIndexCount();
                    for (int i = 0; i < indexCount; i++)
                    {
                        Index index = table.GetIndex(i);
                        int columnCount = table.GetIndex(i).GetColumnCount();
                        if (columnCount >= 1)
                        {
                            string str6 = index.GetName().Name;
                            bool flag = !index.IsUnique();
                            int? nullable = null;
                            int? nullable2 = 0;
                            int? nullable3 = null;
                            int[] columns = index.GetColumns();
                            int? nullable4 = 3;
                            bool flag2 = !flag;
                            bool flag3 = table.HasPrimaryKey() && (index == table.GetPrimaryIndex());
                            for (int j = 0; j < columnCount; j++)
                            {
                                int num1 = columns[j];
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[num] = name;
                                emptyRowData[num2] = str2;
                                emptyRowData[num3] = str3;
                                emptyRowData[num4] = flag;
                                emptyRowData[num5] = str5;
                                emptyRowData[num6] = str6;
                                emptyRowData[num7] = nullable4;
                                emptyRowData[num8] = j + 1;
                                emptyRowData[num9] = table.GetColumn(columns[j]).GetName().Name;
                                emptyRowData[num10] = "ASC";
                                emptyRowData[num11] = nullable;
                                emptyRowData[num12] = nullable2;
                                emptyRowData[num14] = nullable3;
                                emptyRowData[num13] = str4;
                                emptyRowData[num15] = flag2;
                                emptyRowData[num16] = flag3;
                                Table.InsertSys(rowStore, emptyRowData);
                            }
                        }
                    }
                }
            }
            return t;
        }

        private Table SYSTEM_PRIMARYKEYS(Session session)
        {
            Table t = this.SysTables[4];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[4]);
                AddColumn(t, "TABLE_CAT", SqlIdentifier);
                AddColumn(t, "TABLE_SCHEM", SqlIdentifier);
                AddColumn(t, "TABLE_NAME", SqlIdentifier);
                AddColumn(t, "COLUMN_NAME", SqlIdentifier);
                AddColumn(t, "KEY_SEQ", SqlType.SqlSmallint);
                AddColumn(t, "PK_NAME", SqlIdentifier);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[4].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 3, 2, 1, 0 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 5;
            base.database.GetProperties();
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(3);
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if ((!table.IsView() && IsAccessibleTable(session, table)) && table.HasPrimaryKey())
                {
                    string name = table.GetCatalogName().Name;
                    string str2 = table.GetSchemaName().Name;
                    string str3 = table.GetName().Name;
                    Constraint primaryConstraint = table.GetPrimaryConstraint();
                    string str4 = primaryConstraint.GetName().Name;
                    int[] mainColumns = primaryConstraint.GetMainColumns();
                    int length = mainColumns.Length;
                    for (int i = 0; i < length; i++)
                    {
                        object[] emptyRowData = t.GetEmptyRowData();
                        emptyRowData[index] = name;
                        emptyRowData[num2] = str2;
                        emptyRowData[num3] = str3;
                        emptyRowData[num4] = table.GetColumn(mainColumns[i]).GetName().Name;
                        emptyRowData[num5] = i;
                        emptyRowData[num6] = str4;
                        Table.InsertSys(rowStore, emptyRowData);
                    }
                }
            }
            return t;
        }

        private Table SYSTEM_SCHEMAS(Session session)
        {
            Table t = this.SysTables[7];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[7]);
                AddColumn(t, "TABLE_SCHEM", SqlIdentifier);
                AddColumn(t, "TABLE_CATALOG", SqlIdentifier);
                AddColumn(t, "IS_DEFAULT", SqlType.SqlBoolean);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[7].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            Iterator<string> iterator = base.database.schemaManager.FullSchemaNamesIterator();
            string str = base.database.schemaManager.GetDefaultSchemaQName().Name;
            while (iterator.HasNext())
            {
                object[] emptyRowData = t.GetEmptyRowData();
                string str2 = iterator.Next();
                emptyRowData[0] = str2;
                emptyRowData[1] = base.database.GetCatalogName().Name;
                emptyRowData[2] = str2.Equals(str);
                Table.InsertSys(rowStore, emptyRowData);
            }
            return t;
        }

        private Table SYSTEM_SEQUENCES(Session session)
        {
            Table t = this.SysTables[14];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[14]);
                AddColumn(t, "SEQUENCE_CATALOG", SqlIdentifier);
                AddColumn(t, "SEQUENCE_SCHEMA", SqlIdentifier);
                AddColumn(t, "SEQUENCE_NAME", SqlIdentifier);
                AddColumn(t, "DATA_TYPE", CharacterData);
                AddColumn(t, "NUMERIC_PRECISION", CardinalNumber);
                AddColumn(t, "NUMERIC_PRECISION_RADIX", CardinalNumber);
                AddColumn(t, "NUMERIC_SCALE", CardinalNumber);
                AddColumn(t, "MAXIMUM_VALUE", CharacterData);
                AddColumn(t, "MINIMUM_VALUE", CharacterData);
                AddColumn(t, "INCREMENT", CharacterData);
                AddColumn(t, "CYCLE_OPTION", YesOrNo);
                AddColumn(t, "DECLARED_DATA_TYPE", CharacterData);
                AddColumn(t, "DECLARED_NUMERIC_PRECISION", CardinalNumber);
                AddColumn(t, "DECLARED_NUMERIC_SCLAE", CardinalNumber);
                AddColumn(t, "START_WITH", CharacterData);
                AddColumn(t, "NEXT_VALUE", CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[14].Name, false, 20);
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
            Iterator<object> iterator = base.database.schemaManager.DatabaseObjectIterator(7);
            while (iterator.HasNext())
            {
                NumberSequence sequence = (NumberSequence) iterator.Next();
                if (session.GetGrantee().IsAccessible(sequence))
                {
                    object[] emptyRowData = t.GetEmptyRowData();
                    NumberType dataType = (NumberType) sequence.GetDataType();
                    int num17 = ((dataType.TypeCode == 2) || (dataType.TypeCode == 3)) ? 10 : 2;
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = sequence.GetSchemaName().Name;
                    emptyRowData[num3] = sequence.GetName().Name;
                    emptyRowData[num4] = sequence.GetDataType().GetFullNameString();
                    emptyRowData[num5] = dataType.GetPrecision();
                    emptyRowData[num6] = num17;
                    emptyRowData[num7] = 0L;
                    emptyRowData[num8] = sequence.GetMaxValue().ToString();
                    emptyRowData[num9] = sequence.GetMinValue().ToString();
                    emptyRowData[num10] = sequence.GetIncrement().ToString();
                    emptyRowData[num11] = sequence.IsCycle() ? "YES" : "NO";
                    emptyRowData[num12] = emptyRowData[num4];
                    emptyRowData[num13] = emptyRowData[num5];
                    emptyRowData[num14] = emptyRowData[num14];
                    emptyRowData[num15] = sequence.GetStartValue().ToString();
                    emptyRowData[num16] = sequence.Peek().ToString();
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table SYSTEM_TABLES(Session session)
        {
            Table t = this.SysTables[8];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[8]);
                AddColumn(t, "TABLE_CAT", SqlIdentifier);
                AddColumn(t, "TABLE_SCHEM", SqlIdentifier);
                AddColumn(t, "TABLE_NAME", SqlIdentifier);
                AddColumn(t, "TABLE_TYPE", CharacterData);
                AddColumn(t, "REMARKS", CharacterData);
                AddColumn(t, "TYPE_CAT", SqlIdentifier);
                AddColumn(t, "TYPE_SCHEM", SqlIdentifier);
                AddColumn(t, "TYPE_NAME", SqlIdentifier);
                AddColumn(t, "SELF_REFERENCING_COL_NAME", SqlIdentifier);
                AddColumn(t, "REF_GENERATION", CharacterData);
                AddColumn(t, "LibCore_TYPE", SqlIdentifier);
                AddColumn(t, "READ_ONLY", SqlType.SqlBoolean);
                AddColumn(t, "COMMIT_ACTION", CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[8].Name, false, 20);
                t.CreatePrimaryKeyConstraint(indexName, new int[] { 3, 1, 2, 0 }, false);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            int index = 0;
            int num2 = 1;
            int num3 = 2;
            int num4 = 3;
            int num5 = 4;
            int num6 = 10;
            int num7 = 11;
            int num8 = 12;
            Iterator<object> iterator = this.AllTables();
            DITableInfo info = new DITableInfo();
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (IsAccessibleTable(session, table))
                {
                    info.SetTable(table);
                    object[] emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = base.database.GetCatalogName().Name;
                    emptyRowData[num2] = table.GetSchemaName().Name;
                    emptyRowData[num3] = table.GetName().Name;
                    emptyRowData[num4] = info.GetAdoStandardType();
                    emptyRowData[num5] = info.GetRemark();
                    emptyRowData[num6] = info.GetUtlType();
                    emptyRowData[num7] = table.IsDataReadOnly();
                    emptyRowData[num8] = table.IsTemp() ? (table.OnCommitPreserve() ? "PRESERVE" : "DELETE") : null;
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            return t;
        }

        private Table SYSTEM_TYPEINFO(Session session)
        {
            object[] emptyRowData;
            Table t = this.SysTables[10];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[10]);
                AddColumn(t, "TYPE_NAME", SqlIdentifier);
                AddColumn(t, "DATA_TYPE", SqlType.SqlSmallint);
                AddColumn(t, "PRECISION", SqlType.SqlInteger);
                AddColumn(t, "LITERAL_PREFIX", CharacterData);
                AddColumn(t, "LITERAL_SUFFIX", CharacterData);
                AddColumn(t, "CREATE_PARAMS", CharacterData);
                AddColumn(t, "NULLABLE", SqlType.SqlSmallint);
                AddColumn(t, "CASE_SENSITIVE", SqlType.SqlBoolean);
                AddColumn(t, "SEARCHABLE", SqlType.SqlInteger);
                AddColumn(t, "UNSIGNED_ATTRIBUTE", SqlType.SqlBoolean);
                AddColumn(t, "FIXED_PREC_SCALE", SqlType.SqlBoolean);
                AddColumn(t, "AUTO_INCREMENT", SqlType.SqlBoolean);
                AddColumn(t, "LOCAL_TYPE_NAME", SqlIdentifier);
                AddColumn(t, "MINIMUM_SCALE", SqlType.SqlSmallint);
                AddColumn(t, "MAXIMUM_SCALE", SqlType.SqlSmallint);
                AddColumn(t, "SQL_DATA_TYPE", SqlType.SqlInteger);
                AddColumn(t, "SQL_DATETIME_SUB", SqlType.SqlInteger);
                AddColumn(t, "NUM_PREC_RADIX", SqlType.SqlInteger);
                AddColumn(t, "CREATE_FORMAT", CharacterData);
                AddColumn(t, "INTERVAL_PRECISION", SqlType.SqlInteger);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[10].Name, false, 20);
                int[] columns = new int[2];
                columns[0] = 1;
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
            int num11 = 10;
            int num12 = 11;
            int num13 = 12;
            int num14 = 13;
            int num15 = 14;
            int num16 = 15;
            int num17 = 0x10;
            int num18 = 0x11;
            int num19 = 0x12;
            int num20 = 0x12;
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            bool flag = base.database.GetProperties().IsPropertyTrue("jdbc.translate_dti_types");
            foreach (string str in SqlType.TypeNames.Keys)
            {
                SqlType defaultType = SqlType.GetDefaultType(SqlType.TypeNames[str]);
                if (defaultType != null)
                {
                    if (flag)
                    {
                        if (defaultType.IsIntervalType())
                        {
                            defaultType = CharacterType.GetCharacterType(12, (long) defaultType.DisplaySize());
                        }
                        else if (defaultType.IsDateTimeTypeWithZone())
                        {
                            defaultType = ((DateTimeType) defaultType).GetDateTimeTypeWithoutZone();
                        }
                    }
                    emptyRowData = t.GetEmptyRowData();
                    emptyRowData[index] = str;
                    emptyRowData[num2] = defaultType.TypeCode;
                    long maxPrecision = defaultType.GetMaxPrecision();
                    emptyRowData[num3] = (maxPrecision > 0x7fffffffL) ? 0x7fffffff : ((int) maxPrecision);
                    if ((defaultType.IsBinaryType() || defaultType.IsCharacterType()) || (defaultType.IsDateTimeType() || defaultType.IsIntervalType()))
                    {
                        emptyRowData[num4] = "'";
                        emptyRowData[num5] = "'";
                    }
                    if (defaultType.AcceptsPrecision() && defaultType.AcceptsScale())
                    {
                        emptyRowData[num6] = "PRECISION,SCALE";
                        emptyRowData[num19] = str + "({0},{1})";
                    }
                    else if (defaultType.AcceptsPrecision())
                    {
                        emptyRowData[num6] = defaultType.IsNumberType() ? "PRECISION" : "LENGTH";
                        emptyRowData[num19] = str + "({0})";
                    }
                    else if (defaultType.AcceptsScale())
                    {
                        emptyRowData[num6] = "SCALE";
                        emptyRowData[num19] = str + "({0})";
                    }
                    emptyRowData[num7] = 1;
                    emptyRowData[num8] = !defaultType.IsCharacterType() ? ((object) 0) : ((object) (defaultType.TypeCode != 100));
                    if (defaultType.IsLobType())
                    {
                        emptyRowData[num9] = 0;
                    }
                    else if (defaultType.IsCharacterType() || defaultType.IsBinaryType())
                    {
                        emptyRowData[num9] = 3;
                    }
                    else
                    {
                        emptyRowData[num9] = 2;
                    }
                    emptyRowData[num10] = false;
                    emptyRowData[num11] = (defaultType.TypeCode == 2) ? ((object) 1) : ((object) (defaultType.TypeCode == 3));
                    emptyRowData[num12] = defaultType.IsIntegralType();
                    emptyRowData[num13] = defaultType.GetCSharpClassName();
                    emptyRowData[num14] = 0;
                    emptyRowData[num15] = defaultType.GetMaxScale();
                    emptyRowData[num16] = defaultType.GetAdoTypeCode();
                    emptyRowData[num17] = null;
                    emptyRowData[num18] = defaultType.GetPrecisionRadix();
                    if (defaultType.IsIntervalType())
                    {
                        emptyRowData[num20] = null;
                    }
                    Table.InsertSys(rowStore, emptyRowData);
                }
            }
            emptyRowData = t.GetEmptyRowData();
            emptyRowData[index] = "DISTINCT";
            emptyRowData[num2] = 0x7d1;
            return t;
        }

        private Table SYSTEM_UDTS(Session session)
        {
            Table t = this.SysTables[11];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[11]);
                AddColumn(t, "TYPE_CAT", SqlIdentifier);
                AddColumn(t, "TYPE_SCHEM", SqlIdentifier);
                AddColumn(t, "TYPE_NAME", SqlIdentifier);
                AddColumn(t, "CLASS_NAME", CharacterData);
                AddColumn(t, "DATA_TYPE", SqlType.SqlInteger);
                AddColumn(t, "REMARKS", CharacterData);
                AddColumn(t, "BASE_TYPE", SqlType.SqlInteger);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[11].Name, false, 20);
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

        private Table SYSTEM_USERS(Session session)
        {
            Table t = this.SysTables[12];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[12]);
                AddColumn(t, "USER_NAME", SqlIdentifier);
                AddColumn(t, "ADMIN", SqlType.SqlBoolean);
                AddColumn(t, "INITIAL_SCHEMA", SqlIdentifier);
                Table table3 = t;
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[12].Name, false, 20);
                int[] columns = new int[1];
                table3.CreatePrimaryKeyConstraint(indexName, columns, true);
                return t;
            }
            IPersistentStore rowStore = session.sessionData.GetRowStore(t);
            List<User> list = base.database.GetUserManager().ListVisibleUsers(session);
            for (int i = 0; i < list.Count; i++)
            {
                object[] emptyRowData = t.GetEmptyRowData();
                User user = list[i];
                QNameManager.QName initialSchema = user.GetInitialSchema();
                emptyRowData[0] = user.GetNameString();
                emptyRowData[1] = user.IsAdmin();
                emptyRowData[2] = (initialSchema == null) ? null : initialSchema.Name;
                Table.InsertSys(rowStore, emptyRowData);
            }
            return t;
        }

        private Table TABLE_PRIVILEGES(Session session)
        {
            Table t = this.SysTables[0x4a];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[0x4a]);
                AddColumn(t, "GRANTOR", SqlIdentifier);
                AddColumn(t, "GRANTEE", SqlIdentifier);
                AddColumn(t, "TABLE_CATALOG", SqlIdentifier);
                AddColumn(t, "TABLE_SCHEMA", SqlIdentifier);
                AddColumn(t, "TABLE_NAME", SqlIdentifier);
                AddColumn(t, "PRIVILEGE_TYPE", CharacterData);
                AddColumn(t, "IS_GRANTABLE", YesOrNo);
                AddColumn(t, "WITH_HIERARCHY", YesOrNo);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[0x4a].Name, false, 20);
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
            int num8 = 7;
            OrderedHashSet<Grantee> granteeAndAllRolesWithPublic = session.GetGrantee().GetGranteeAndAllRolesWithPublic();
            Iterator<object> iterator = this.AllTables();
            while (iterator.HasNext())
            {
                Table table3 = (Table) iterator.Next();
                string name = table3.GetName().Name;
                string str2 = table3.GetCatalogName().Name;
                string str3 = table3.GetSchemaName().Name;
                for (int i = 0; i < granteeAndAllRolesWithPublic.Size(); i++)
                {
                    Grantee local1 = granteeAndAllRolesWithPublic.Get(i);
                    OrderedHashSet<Right> allDirectPrivileges = local1.GetAllDirectPrivileges(table3);
                    OrderedHashSet<Right> allGrantedPrivileges = local1.GetAllGrantedPrivileges(table3);
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
                                string str4 = Right.PrivilegeNames[k];
                                object[] emptyRowData = t.GetEmptyRowData();
                                emptyRowData[index] = right.GetGrantor().GetName().Name;
                                emptyRowData[num2] = right.GetGrantee().GetName().Name;
                                emptyRowData[num3] = str2;
                                emptyRowData[num4] = str3;
                                emptyRowData[num5] = name;
                                emptyRowData[num6] = str4;
                                emptyRowData[num7] = ((right.GetGrantee() == table3.GetOwner()) || grantableRights.CanAccessFully(Right.PrivilegeTypes[k])) ? "YES" : "NO";
                                emptyRowData[num8] = "NO";
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

        public Table TABLES(Session session)
        {
            Table t = this.SysTables[0x4b];
            if (t == null)
            {
                t = this.CreateBlankTable(SysTableQNames[0x4b]);
                AddColumn(t, "TABLE_CATALOG", SqlIdentifier);
                AddColumn(t, "TABLE_SCHEMA", SqlIdentifier);
                AddColumn(t, "TABLE_NAME", SqlIdentifier);
                AddColumn(t, "TABLE_TYPE", CharacterData);
                AddColumn(t, "SELF_REFERENCING_COLUMN_NAME", SqlIdentifier);
                AddColumn(t, "REFERENCE_GENERATION", CharacterData);
                AddColumn(t, "USER_DEFINED_TYPE_CATALOG", SqlIdentifier);
                AddColumn(t, "USER_DEFINED_TYPE_SCHEMA", SqlIdentifier);
                AddColumn(t, "USER_DEFINED_TYPE_NAME", SqlIdentifier);
                AddColumn(t, "IS_INSERTABLE_INTO", YesOrNo);
                AddColumn(t, "IS_TYPED", YesOrNo);
                AddColumn(t, "COMMIT_ACTION", CharacterData);
                QNameManager.QName indexName = QNameManager.NewInfoSchemaObjectName(SysTableQNames[0x4b].Name, false, 20);
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
            Iterator<object> iterator = this.AllTables();
            while (iterator.HasNext())
            {
                Table table = (Table) iterator.Next();
                if (!IsAccessibleTable(session, table))
                {
                    continue;
                }
                object[] emptyRowData = t.GetEmptyRowData();
                emptyRowData[index] = base.database.GetCatalogName().Name;
                emptyRowData[num2] = table.GetSchemaName().Name;
                emptyRowData[num3] = table.GetName().Name;
                int tableType = table.GetTableType();
                switch (tableType)
                {
                    case 1:
                        goto Label_0242;

                    case 2:
                        goto Label_0258;

                    case 3:
                        emptyRowData[num4] = "GLOBAL TEMPORARY";
                        emptyRowData[num10] = "YES";
                        break;

                    default:
                        if (tableType != 8)
                        {
                            goto Label_0258;
                        }
                        goto Label_0242;
                }
            Label_01EA:
                emptyRowData[num5] = null;
                emptyRowData[num6] = null;
                emptyRowData[num7] = null;
                emptyRowData[num8] = null;
                emptyRowData[num9] = null;
                emptyRowData[num11] = "NO";
                emptyRowData[num12] = table.IsTemp() ? (table.OnCommitPreserve() ? "PRESERVE" : "DELETE") : null;
                Table.InsertSys(rowStore, emptyRowData);
                continue;
            Label_0242:
                emptyRowData[num4] = "VIEW";
                emptyRowData[num10] = "NO";
                goto Label_01EA;
            Label_0258:
                emptyRowData[num4] = "BASE TABLE";
                emptyRowData[num10] = table.IsWritable() ? "YES" : "NO";
                goto Label_01EA;
            }
            return t;
        }
    }
}

