namespace FwNs.Core.LC.cEngine
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using System;
    using System.Collections.Generic;

    public static class DatabaseManager
    {
        private static readonly object DatabaseManagerLock = new object();
        private static int _dbIdCounter;
        private static readonly HashMap<string, Database> MemDatabaseMap = new HashMap<string, Database>();
        private static readonly HashMap<string, Database> FileDatabaseMap = new HashMap<string, Database>();
        private static readonly HashMap<string, Database> ResDatabaseMap = new HashMap<string, Database>();
        private static readonly Dictionary<int, Database> DatabaseIdMap = new Dictionary<int, Database>();

        private static void AddDatabaseObject(string type, string path, Database db)
        {
            lock (DatabaseManagerLock)
            {
                HashMap<string, Database> fileDatabaseMap;
                string key = path;
                if (type == "file:")
                {
                    fileDatabaseMap = FileDatabaseMap;
                    key = FilePathToKey(path);
                }
                else if (type == "mem:")
                {
                    fileDatabaseMap = MemDatabaseMap;
                }
                else
                {
                    if (type != "res:")
                    {
                        throw Error.RuntimeError(0xc9, "DatabaseManager");
                    }
                    fileDatabaseMap = ResDatabaseMap;
                }
                DatabaseIdMap.Add(db.DatabaseId, db);
                fileDatabaseMap.Put(key, db);
            }
        }

        private static string FilePathToKey(string path)
        {
            try
            {
                return FileUtil.GetDefaultInstance().CanonicalOrAbsolutePath(path);
            }
            catch (Exception)
            {
                return path;
            }
        }

        public static Database GetDatabase(string type, string path, LibCoreProperties props)
        {
            Database db = GetDatabaseObject(type, path, props);
            lock (db)
            {
                int state = db.GetState();
                if (state <= 4)
                {
                    switch (state)
                    {
                        case 1:
                            return db;

                        case 4:
                            goto Label_004A;
                    }
                    return db;
                }
                if (state != 8)
                {
                    if (state == 0x10)
                    {
                        if (LookupDatabaseObject(type, path) == null)
                        {
                            AddDatabaseObject(type, path, db);
                        }
                        db.Open();
                    }
                    return db;
                }
            Label_004A:
                throw Error.GetError(0x1c3, 0x17);
            }
            return db;
        }

        private static Database GetDatabaseObject(string type, string path, LibCoreProperties props)
        {
            lock (DatabaseManagerLock)
            {
                HashMap<string, Database> fileDatabaseMap;
                string key = path;
                if (type == "file:")
                {
                    fileDatabaseMap = FileDatabaseMap;
                    key = FilePathToKey(path);
                    if ((fileDatabaseMap.Get(key) == null) && (fileDatabaseMap.Size() > 0))
                    {
                        Iterator<string> iterator = fileDatabaseMap.GetKeySet().GetIterator();
                        while (iterator.HasNext())
                        {
                            string str2 = iterator.Next();
                            if (key.Equals(str2, StringComparison.OrdinalIgnoreCase))
                            {
                                key = str2;
                                break;
                            }
                        }
                    }
                }
                else if (type == "mem:")
                {
                    fileDatabaseMap = MemDatabaseMap;
                }
                else
                {
                    if (type != "res:")
                    {
                        throw Error.RuntimeError(0xc9, "DatabaseManager");
                    }
                    fileDatabaseMap = ResDatabaseMap;
                }
                Database database2 = fileDatabaseMap.Get(key);
                if (database2 == null)
                {
                    database2 = new Database(type, path, key, props) {
                        DatabaseId = _dbIdCounter
                    };
                    DatabaseIdMap.Add(_dbIdCounter, database2);
                    _dbIdCounter++;
                    fileDatabaseMap.Put(key, database2);
                }
                return database2;
            }
        }

        public static Database LookupDatabaseObject(string type, string path)
        {
            lock (DatabaseManagerLock)
            {
                HashMap<string, Database> fileDatabaseMap;
                string key = path;
                if (type == "file:")
                {
                    fileDatabaseMap = FileDatabaseMap;
                    key = FilePathToKey(path);
                }
                else if (type == "mem:")
                {
                    fileDatabaseMap = MemDatabaseMap;
                }
                else
                {
                    if (type != "res:")
                    {
                        throw Error.RuntimeError(0xc9, "DatabaseManager");
                    }
                    fileDatabaseMap = ResDatabaseMap;
                }
                return fileDatabaseMap.Get(key);
            }
        }

        public static Session NewSession(string type, string path, string user, string password, LibCoreProperties props, string zoneString, int timeZoneSeconds)
        {
            Database database = GetDatabase(type, path, props);
            if (database == null)
            {
                return null;
            }
            return database.Connect(user, password, zoneString, timeZoneSeconds);
        }

        public static void RemoveDatabase(Database database)
        {
            HashMap<string, Database> fileDatabaseMap;
            int databaseId = database.DatabaseId;
            string databaseType = database.GetDatabaseType();
            string path = database.GetPath();
            string key = path;
            if (databaseType == "file:")
            {
                fileDatabaseMap = FileDatabaseMap;
                key = FilePathToKey(path);
            }
            else if (databaseType == "mem:")
            {
                fileDatabaseMap = MemDatabaseMap;
            }
            else
            {
                if (databaseType != "res:")
                {
                    throw Error.RuntimeError(0xc9, "DatabaseManager");
                }
                fileDatabaseMap = ResDatabaseMap;
            }
            DatabaseIdMap.Remove(databaseId);
            fileDatabaseMap.Remove(key);
        }
    }
}

