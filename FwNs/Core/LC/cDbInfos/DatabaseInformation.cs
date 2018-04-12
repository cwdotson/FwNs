namespace FwNs.Core.LC.cDbInfos
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public class DatabaseInformation
    {
        protected const int ID_SYSTEM_BESTROWIDENTIFIER = 0;
        protected const int ID_SYSTEM_COLUMNS = 1;
        protected const int ID_SYSTEM_CROSSREFERENCE = 2;
        protected const int ID_SYSTEM_INDEXINFO = 3;
        protected const int ID_SYSTEM_PRIMARYKEYS = 4;
        protected const int ID_SYSTEM_PROCEDURECOLUMNS = 5;
        protected const int ID_SYSTEM_PROCEDURES = 6;
        protected const int ID_SYSTEM_SCHEMAS = 7;
        protected const int ID_SYSTEM_TABLES = 8;
        protected const int ID_SYSTEM_TABLETYPES = 9;
        protected const int ID_SYSTEM_TYPEINFO = 10;
        protected const int ID_SYSTEM_UDTS = 11;
        protected const int ID_SYSTEM_USERS = 12;
        protected const int ID_SYSTEM_VERSIONCOLUMNS = 13;
        protected const int ID_SYSTEM_SEQUENCES = 14;
        protected const int ID_SYSTEM_CACHEINFO = 15;
        protected const int ID_SYSTEM_CONNECTION_PROPERTIES = 0x10;
        protected const int ID_SYSTEM_PROPERTIES = 0x11;
        protected const int ID_SYSTEM_SESSIONINFO = 0x12;
        protected const int ID_SYSTEM_SESSIONS = 0x13;
        protected const int ID_ADMINISTRABLE_ROLE_AUTHORIZATIONS = 0x15;
        protected const int ID_APPLICABLE_ROLES = 0x16;
        protected const int ID_ASSERTIONS = 0x17;
        protected const int ID_AUTHORIZATIONS = 0x18;
        protected const int ID_CHARACTER_SETS = 0x19;
        protected const int ID_CHECK_CONSTRAINT_ROUTINE_USAGE = 0x1a;
        protected const int ID_CHECK_CONSTRAINTS = 0x1b;
        protected const int ID_COLLATIONS = 0x1c;
        protected const int ID_COLUMN_COLUMN_USAGE = 0x1d;
        protected const int ID_COLUMN_DOMAIN_USAGE = 30;
        protected const int ID_COLUMN_PRIVILEGES = 0x1f;
        protected const int ID_COLUMN_UDT_USAGE = 0x20;
        protected const int ID_COLUMNS = 0x21;
        protected const int ID_CONSTRAINT_COLUMN_USAGE = 0x22;
        protected const int ID_CONSTRAINT_TABLE_USAGE = 0x23;
        protected const int ID_DATA_TYPE_PRIVILEGES = 0x24;
        protected const int ID_DOMAIN_CONSTRAINTS = 0x25;
        protected const int ID_DOMAINS = 0x26;
        protected const int ID_ENABLED_ROLES = 0x27;
        protected const int ID_INFORMATION_SCHEMA_CATALOG_NAME = 40;
        protected const int ID_ASSEMBLY_ASSEMBLY_USAGE = 0x29;
        protected const int ID_ASSEMBLIES = 0x2a;
        protected const int ID_KEY_COLUMN_USAGE = 0x2b;
        protected const int ID_METHOD_SPECIFICATIONS = 0x2c;
        protected const int ID_MODULE_COLUMN_USAGE = 0x2d;
        protected const int ID_MODULE_PRIVILEGES = 0x2e;
        protected const int ID_MODULE_TABLE_USAGE = 0x2f;
        protected const int ID_MODULES = 0x30;
        protected const int ID_PARAMETERS = 0x31;
        protected const int ID_REFERENTIAL_CONSTRAINTS = 50;
        protected const int ID_ROLE_AUTHORIZATION_DESCRIPTORS = 0x33;
        protected const int ID_ROLE_COLUMN_GRANTS = 0x34;
        protected const int ID_ROLE_MODULE_GRANTS = 0x35;
        protected const int ID_ROLE_ROUTINE_GRANTS = 0x36;
        protected const int ID_ROLE_TABLE_GRANTS = 0x37;
        protected const int ID_ROLE_UDT_GRANTS = 0x38;
        protected const int ID_ROLE_USAGE_GRANTS = 0x39;
        protected const int ID_ROUTINE_COLUMN_USAGE = 0x3a;
        protected const int ID_ROUTINE_ASSEMBLY_USAGE = 0x3b;
        protected const int ID_ROUTINE_PRIVILEGES = 60;
        protected const int ID_ROUTINE_ROUTINE_USAGE = 0x3d;
        protected const int ID_ROUTINE_SEQUENCE_USAGE = 0x3e;
        protected const int ID_ROUTINE_TABLE_USAGE = 0x3f;
        protected const int ID_ROUTINES = 0x40;
        protected const int ID_SCHEMATA = 0x41;
        protected const int ID_SEQUENCES = 0x42;
        protected const int ID_SQL_FEATURES = 0x43;
        protected const int ID_SQL_IMPLEMENTATION_INFO = 0x44;
        protected const int ID_SQL_PACKAGES = 0x45;
        protected const int ID_SQL_PARTS = 70;
        protected const int ID_SQL_SIZING = 0x47;
        protected const int ID_SQL_SIZING_PROFILES = 0x48;
        protected const int ID_TABLE_CONSTRAINTS = 0x49;
        protected const int ID_TABLE_PRIVILEGES = 0x4a;
        protected const int ID_TABLES = 0x4b;
        protected const int ID_TRANSLATIONS = 0x4c;
        protected const int ID_TRIGGER_COLUMN_USAGE = 0x4d;
        protected const int ID_TRIGGER_ROUTINE_USAGE = 0x4e;
        protected const int ID_TRIGGER_SEQUENCE_USAGE = 0x4f;
        protected const int ID_TRIGGER_TABLE_USAGE = 80;
        protected const int ID_TRIGGERED_UPDATE_COLUMNS = 0x51;
        protected const int ID_TRIGGERS = 0x52;
        protected const int ID_TYPE_JAR_USAGE = 0x53;
        protected const int ID_UDT_PRIVILEGES = 0x54;
        protected const int ID_USAGE_PRIVILEGES = 0x55;
        protected const int ID_USER_DEFINED_TYPES = 0x56;
        protected const int ID_VIEW_COLUMN_USAGE = 0x57;
        protected const int ID_VIEW_ROUTINE_USAGE = 0x58;
        protected const int ID_VIEW_TABLE_USAGE = 0x59;
        protected const int ID_VIEWS = 90;
        protected const int ID_SYSTEM_CATALOGS = 0x5b;
        protected const int ID_DUAL = 0x5c;
        protected const int ID_SYSTEM_COMMENTS = 0x5d;
        private static readonly object DatabaseInformation_Lock = new object();
        public static string[] sysTableNames = new string[] { 
            "SYSTEM_BESTROWIDENTIFIER", "SYSTEM_COLUMNS", "SYSTEM_CROSSREFERENCE", "SYSTEM_INDEXINFO", "SYSTEM_PRIMARYKEYS", "SYSTEM_PROCEDURECOLUMNS", "SYSTEM_PROCEDURES", "SYSTEM_SCHEMAS", "SYSTEM_TABLES", "SYSTEM_TABLETYPES", "SYSTEM_TYPEINFO", "SYSTEM_UDTS", "SYSTEM_USERS", "SYSTEM_VERSIONCOLUMNS", "SYSTEM_SEQUENCES", "SYSTEM_CACHEINFO",
            "SYSTEM_CONNECTION_PROPERTIES", "SYSTEM_PROPERTIES", "SYSTEM_SESSIONINFO", "SYSTEM_SESSIONS", "SYSTEM_TEXT_TABLES", "ADMINISTRABLE_ROLE_AUTHORIZATIONS", "APPLICABLE_ROLES", "ASSERTIONS", "AUTHORIZATIONS", "CHARACTER_SETS", "CHECK_CONSTRAINT_ROUTINE_USAGE", "CHECK_CONSTRAINTS", "COLLATIONS", "COLUMN_COLUMN_USAGE", "COLUMN_DOMAIN_USAGE", "COLUMN_PRIVILEGES",
            "COLUMN_UDT_USAGE", "COLUMNS", "CONSTRAINT_COLUMN_USAGE", "CONSTRAINT_TABLE_USAGE", "DATA_TYPE_PRIVILEGES", "DOMAIN_CONSTRAINTS", "DOMAINS", "ENABLED_ROLES", "INFORMATION_SCHEMA_CATALOG_NAME", "ASSEMBLY_ASSEMBLY_USAGE", "ASSEMBLIES", "KEY_COLUMN_USAGE", "METHOD_SPECIFICATIONS", "MODULE_COLUMN_USAGE", "MODULE_PRIVILEGES", "MODULE_TABLE_USAGE",
            "MODULES", "PARAMETERS", "REFERENTIAL_CONSTRAINTS", "ROLE_AUTHORIZATION_DESCRIPTORS", "ROLE_COLUMN_GRANTS", "ROLE_MODULE_GRANTS", "ROLE_ROUTINE_GRANTS", "ROLE_TABLE_GRANTS", "ROLE_UDT_GRANTS", "ROLE_USAGE_GRANTS", "ROUTINE_COLUMN_USAGE", "ROUTINE_ASSEMBLY_USAGE", "ROUTINE_PRIVILEGES", "ROUTINE_ROUTINE_USAGE", "ROUTINE_SEQUENCE_USAGE", "ROUTINE_TABLE_USAGE",
            "ROUTINES", "SCHEMATA", "SEQUENCES", "SQL_FEATURES", "SQL_IMPLEMENTATION_INFO", "SQL_PACKAGES", "SQL_PARTS", "SQL_SIZING", "SQL_SIZING_PROFILES", "TABLE_CONSTRAINTS", "TABLE_PRIVILEGES", "TABLES", "TRANSLATIONS", "TRIGGER_COLUMN_USAGE", "TRIGGER_ROUTINE_USAGE", "TRIGGER_SEQUENCE_USAGE",
            "TRIGGER_TABLE_USAGE", "TRIGGERED_UPDATE_COLUMNS", "TRIGGERS", "TYPE_JAR_USAGE", "UDT_PRIVILEGES", "USAGE_PRIVILEGES", "USER_DEFINED_TYPES", "VIEW_COLUMN_USAGE", "VIEW_ROUTINE_USAGE", "VIEW_TABLE_USAGE", "VIEWS", "SYSTEM_CATALOGS", "DUAL", "SYSTEM_COMMENTS"
        };
        public static Dictionary<string, int> SysTableNamesMap = GetSysTableNamesMap();
        protected Database database;
        protected bool WithContent;

        public DatabaseInformation(Database db)
        {
            this.database = db;
        }

        protected static int GetSysTableId(string token)
        {
            int num;
            if (SysTableNamesMap.TryGetValue(token, out num))
            {
                return num;
            }
            return -1;
        }

        public static Dictionary<string, int> GetSysTableNamesMap()
        {
            lock (DatabaseInformation_Lock)
            {
                Dictionary<string, int> dictionary2 = new Dictionary<string, int>(0x5b);
                for (int i = 0; i < sysTableNames.Length; i++)
                {
                    dictionary2.Add(sysTableNames[i], i);
                }
                return dictionary2;
            }
        }

        public virtual Table GetSystemTable(Session session, string name)
        {
            lock (this)
            {
                return null;
            }
        }

        public static bool IsSystemTable(string name)
        {
            return SysTableNamesMap.ContainsKey(name);
        }

        public static DatabaseInformation NewDatabaseInformation(Database db)
        {
            Session sysSession = db.sessionManager.GetSysSession();
            DatabaseInformationFull full1 = new DatabaseInformationFull(db);
            full1.Init(sysSession);
            return full1;
        }

        public void SetWithContent(bool withContent)
        {
            lock (this)
            {
                this.WithContent = withContent;
            }
        }
    }
}

