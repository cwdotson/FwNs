namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResources;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class LibCoreDatabaseProperties : LibCoreProperties
    {
        public const int SystemProperty = 0;
        public const int SqlProperty = 2;
        public const int FileProperty = 1;
        public const int FilesNotModified = 0;
        public const int FilesModified = 1;
        public const int FilesNew = 2;
        private const string ModifiedNo = "no";
        private const string ModifiedYes = "yes";
        private const string ModifiedNew = "no-new-files";
        public const string ThisCacheVersion = "1.9.0";
        public const string ProductName = "LibCore Database Engine";
        public const int Major = 1;
        public const int Minor = 9;
        public const int Revision = 0;
        public const string SystemLockfilePollRetriesProperty = "LibCore.lockfile_poll_retries";
        public const string SystemMaxCharOrVarcharDisplaySize = "LibCore.max_char_or_varchar_display_size";
        public const string LibCoreIncBackup = "LibCore.incremental_backup";
        public const string LibCoreVersion = "version";
        public const string LibCoreReadonly = "readonly";
        private const string LibCoreModified = "modified";
        public const string RuntimeGcInterval = "runtime.gc_interval";
        public const string UrlIfexists = "ifexists";
        public const string UrlDefaultSchema = "default_schema";
        public const string LibCoreTx = "LibCore.tx";
        public const string LibCoreTxLevel = "LibCore.tx_level";
        public const string LibCoreApplog = "LibCore.applog";
        public const string LibCoreLobFileScale = "LibCore.lob_file_scale";
        public const string LibCoreCacheFileScale = "LibCore.cache_file_scale";
        public const string LibCoreCacheFreeCountScale = "LibCore.cache_free_count_scale";
        public const string LibCoreCacheRows = "LibCore.cache_rows";
        public const string LibCoreCacheSize = "LibCore.cache_size";
        public const string LibCoreDefaultTableType = "LibCore.default_table_type";
        public const string LibCoreDefragLimit = "LibCore.defrag_limit";
        public const string LibCoreFilesReadonly = "LibCore.files_readonly";
        public const string LibCoreLockFile = "LibCore.lock_file";
        public const string LibCoreLogData = "LibCore.log_data";
        public const string LibCoreLogSize = "LibCore.log_size";
        public const string LibCoreNioDataFile = "LibCore.nio_data_file";
        public const string LibCoreMaxNioScale = "LibCore.max_nio_scale";
        public const string LibCoreRafBufferScale = "LibCore.raf_buffer_scale";
        public const string LibCoreScriptFormat = "LibCore.script_format";
        public const string LibCoreTempDirectory = "LibCore.temp_directory";
        public const string LibCoreResultMaxMemoryRows = "LibCore.result_max_memory_rows";
        public const string LibCoreWriteDelay = "LibCore.write_delay";
        public const string LibCoreWriteDelayMillis = "LibCore.write_delay_millis";
        public const string SqlRefIntegrity = "sql.ref_integrity";
        public const string SqlCompareInLocale = "sql.compare_in_locale";
        public const string SqlEnforceSize = "sql.enforce_size";
        public const string SqlEnforceStrictSize = "sql.enforce_strict_size";
        public const string SqlTxNoMultiWrite = "sql.tx_no_multi_rewrite";
        public const string SqlEnforceRefs = "sql.enforce_refs";
        public const string SqlEnforceNames = "sql.enforce_names";
        public const string JdbcTranslateDtiTypes = "jdbc.translate_dti_types";
        public const string SqlIdentityIsPk = "sql.identity_is_pk";
        public const string TextdbCacheScale = "textdb.cache_scale";
        public const string TextdbCacheSizeScale = "textdb.cache_size_scale";
        public const string TextdbAllQuoted = "textdb.all_quoted";
        public const string TextdbAllowFullPath = "textdb.allow_full_path";
        public const string TextdbEncoding = "textdb.encoding";
        public const string TextdbIgnoreFirst = "textdb.ignore_first";
        public const string TextdbQuoted = "textdb.quoted";
        public const string TextdbFs = "textdb.fs";
        public const string TextdbVs = "textdb.vs";
        public const string TextdbLvs = "textdb.lvs";
        public const string UrlStorageClassName = "storage_class_name";
        public const string UrlFileaccessClassName = "fileaccess_class_name";
        public const string UrlStorageKey = "storage_key";
        public const string UrlShutdown = "shutdown";
        public const string UrlCryptKey = "crypt_key";
        public const string UrlCryptType = "crypt_type";
        public const string UrlCryptIv = "crypt_iv";
        public const string UrlSlOob = "url_sl_oob";
        public const int IndexName = 0;
        public const int IndexType = 1;
        public const int IndexClass = 2;
        public const int IndexIsRange = 3;
        public const int IndexDefaultValue = 4;
        public const int IndexRangeLow = 5;
        public const int IndexRangeHigh = 6;
        public const int IndexValues = 7;
        public const int IndexLimit = 9;
        public const int NoMessage = 1;
        private static readonly Dictionary<string, object[]> DbMeta = GetDbMeta();
        private static string _thisVersion;
        private static string _thisFullVersion;
        private readonly Database _database;

        public LibCoreDatabaseProperties(Database db) : base(db.GetPath(), db.logger.GetFileAccess())
        {
            this._database = db;
            this.SetNewDatabaseProperties();
        }

        private void FilterLoadedProperties()
        {
            List<string> list = new List<string>();
            foreach (string str in base.StringProps.Keys)
            {
                if (!DbMeta.ContainsKey(str))
                {
                    list.Add(str);
                }
            }
            foreach (string str2 in list)
            {
                base.StringProps.Remove(str2);
            }
        }

        private static Dictionary<string, object[]> GetDbMeta()
        {
            Dictionary<string, object[]> dictionary = new Dictionary<string, object[]>(StringComparer.InvariantCultureIgnoreCase) {
                { 
                    "version",
                    GetMeta("version", 1, (string) null)
                },
                { 
                    "modified",
                    GetMeta("modified", 1, (string) null)
                },
                { 
                    "readonly",
                    GetMeta("readonly", 1, false)
                },
                { 
                    "LibCore.files_readonly",
                    GetMeta("LibCore.files_readonly", 1, false)
                }
            };
            byte[] values = new byte[] { 1, 8 };
            dictionary.Add("LibCore.cache_file_scale", GetMeta("LibCore.cache_file_scale", 1, 8, values));
            dictionary.Add("LibCore.tx", GetMeta("LibCore.tx", 2, "MVCC"));
            dictionary.Add("LibCore.temp_directory", GetMeta("LibCore.temp_directory", 2, (string) null));
            dictionary.Add("LibCore.default_table_type", GetMeta("LibCore.default_table_type", 2, "MEMORY"));
            dictionary.Add("LibCore.incremental_backup", GetMeta("LibCore.incremental_backup", 2, true));
            dictionary.Add("LibCore.lock_file", GetMeta("LibCore.lock_file", 2, true));
            dictionary.Add("LibCore.nio_data_file", GetMeta("LibCore.nio_data_file", 2, true));
            dictionary.Add("sql.enforce_size", GetMeta("sql.enforce_size", 2, true));
            dictionary.Add("sql.enforce_strict_size", GetMeta("sql.enforce_strict_size", 2, true));
            dictionary.Add("sql.enforce_names", GetMeta("sql.enforce_names", 2, false));
            dictionary.Add("sql.compare_in_locale", GetMeta("sql.compare_in_locale", 2, false));
            dictionary.Add("LibCore.write_delay", GetMeta("LibCore.write_delay", 2, false));
            dictionary.Add("LibCore.write_delay_millis", GetMeta("LibCore.write_delay_millis", 2, 0, 20, 0x2710));
            dictionary.Add("LibCore.applog", GetMeta("LibCore.applog", 2, 2, 0, 2));
            byte[] buffer2 = new byte[3];
            buffer2[1] = 1;
            buffer2[2] = 3;
            dictionary.Add("LibCore.script_format", GetMeta("LibCore.script_format", 2, 0, buffer2));
            dictionary.Add("LibCore.log_size", GetMeta("LibCore.log_size", 2, 50, 0, 0x3e8));
            dictionary.Add("LibCore.defrag_limit", GetMeta("LibCore.defrag_limit", 2, 20, 0, 100));
            dictionary.Add("runtime.gc_interval", GetMeta("runtime.gc_interval", 2, 0, 0, 0xf4240));
            dictionary.Add("LibCore.cache_size", GetMeta("LibCore.cache_size", 2, 0x2710, 100, 0xf4240));
            dictionary.Add("LibCore.cache_rows", GetMeta("LibCore.cache_rows", 2, 0xc350, 100, 0xf4240));
            dictionary.Add("LibCore.cache_free_count_scale", GetMeta("LibCore.cache_free_count_scale", 2, 9, 6, 12));
            dictionary.Add("LibCore.result_max_memory_rows", GetMeta("LibCore.result_max_memory_rows", 2, 0, 0, 0xf4240));
            dictionary.Add("LibCore.max_nio_scale", GetMeta("LibCore.max_nio_scale", 2, 0x1c, 0x18, 0x1f));
            dictionary.Add("sql.identity_is_pk", GetMeta("sql.identity_is_pk", 2, false));
            dictionary.Add("sql.ref_integrity", GetMeta("sql.ref_integrity", 2, true));
            dictionary.Add("sql.enforce_refs", GetMeta("sql.enforce_refs", 2, false));
            dictionary.Add("jdbc.translate_dti_types", GetMeta("jdbc.translate_dti_types", 2, false));
            dictionary.Add("LibCore.tx_level", GetMeta("LibCore.tx_level", 2, "READ_COMMITTED"));
            dictionary.Add("LibCore.lob_file_scale", GetMeta("LibCore.lob_file_scale", 1, 0x20, new byte[] { 1, 2, 4, 8, 0x10, 0x20 }));
            dictionary.Add("LibCore.log_data", GetMeta("LibCore.log_data", 2, true));
            dictionary.Add("url_sl_oob", GetMeta("url_sl_oob", 2, false));
            return dictionary;
        }

        public int GetDbModified()
        {
            string property = this.GetProperty("modified");
            if ("yes".Equals(property))
            {
                return 1;
            }
            if ("no-new-files".Equals(property))
            {
                return 2;
            }
            return 0;
        }

        public int GetDefaultWriteDelay()
        {
            if (this._database.logger.IsStoredFileAccess())
            {
                return 0x7d0;
            }
            return 0x2710;
        }

        public static int GetErrorLevel()
        {
            return 1;
        }

        public int GetIntegerProperty(string key)
        {
            object[] objArray;
            if (!DbMeta.TryGetValue(key, out objArray))
            {
                throw Error.GetError(0x15b3);
            }
            int num = (int) objArray[4];
            string property = base.StringProps.GetProperty(key);
            if (property != null)
            {
                try
                {
                    num = int.Parse(property);
                }
                catch (FormatException)
                {
                }
            }
            return num;
        }

        private static object[] GetMeta(string name, int accessLevel, bool defaultValue)
        {
            object[] objArray1 = new object[9];
            objArray1[0] = name;
            objArray1[1] = accessLevel;
            objArray1[2] = "bool";
            objArray1[4] = defaultValue;
            return objArray1;
        }

        private static object[] GetMeta(string name, int accessLevel, string defaultValue)
        {
            object[] objArray1 = new object[9];
            objArray1[0] = name;
            objArray1[1] = accessLevel;
            objArray1[2] = "string";
            objArray1[4] = defaultValue;
            return objArray1;
        }

        private static object[] GetMeta(string name, int accessLevel, int defaultValue, byte[] values)
        {
            object[] objArray1 = new object[9];
            objArray1[0] = name;
            objArray1[1] = accessLevel;
            objArray1[2] = "int";
            objArray1[4] = defaultValue;
            objArray1[7] = values;
            return objArray1;
        }

        private static object[] GetMeta(string name, int accessLevel, int defaultValue, int rangeLow, int rangeHigh)
        {
            object[] objArray1 = new object[9];
            objArray1[0] = name;
            objArray1[1] = accessLevel;
            objArray1[2] = "int";
            objArray1[4] = defaultValue;
            objArray1[3] = true;
            objArray1[5] = rangeLow;
            objArray1[6] = rangeHigh;
            return objArray1;
        }

        public static IEnumerable<object[]> GetPropertiesMetaIterator()
        {
            return DbMeta.Values;
        }

        public override string GetProperty(string key)
        {
            if (DbMeta[key] == null)
            {
                throw Error.GetError(0x15b3, key);
            }
            return base.StringProps.GetProperty(key);
        }

        public string GetStringProperty(string key)
        {
            object[] objArray;
            if (!DbMeta.TryGetValue(key, out objArray))
            {
                throw Error.GetError(0x15b3);
            }
            string str = (string) objArray[4];
            string property = base.StringProps.GetProperty(key);
            if (property != null)
            {
                str = property;
            }
            return str;
        }

        public static HashSet<object[]> GetUserDefinedPropertyData()
        {
            HashSet<object[]> set = new HashSet<object[]>();
            foreach (object[] objArray in DbMeta.Values)
            {
                if (((int) objArray[1]) == 2)
                {
                    set.Add(objArray);
                }
            }
            return set;
        }

        public static bool IsBoolean(string key)
        {
            object[] objArray = DbMeta[key];
            return (((objArray != null) && objArray[2].Equals("bool")) && (((int) objArray[1]) == 2));
        }

        public static bool IsIntegral(string key)
        {
            object[] objArray = DbMeta[key];
            return (((objArray != null) && objArray[2].Equals("int")) && (((int) objArray[1]) == 2));
        }

        public override bool IsPropertyTrue(string key)
        {
            object[] objArray;
            if (!DbMeta.TryGetValue(key, out objArray))
            {
                throw Error.GetError(0x15b3);
            }
            bool flag = (bool) objArray[4];
            string property = base.StringProps.GetProperty(key);
            int num1 = (int) objArray[1];
            if (property != null)
            {
                flag = bool.Parse(property);
            }
            return flag;
        }

        public static bool IsString(string key)
        {
            object[] objArray = DbMeta[key];
            return (((objArray != null) && objArray[2].Equals("string")) && (((int) objArray[1]) == 2));
        }

        public static bool IsUserDefinedProperty(string key)
        {
            object[] objArray = DbMeta[key];
            return ((objArray != null) && (((int) objArray[1]) == 2));
        }

        public override bool Load(UtlFileAccess fileAccess)
        {
            if (DatabaseUrl.IsFileBasedDatabaseType(this._database.GetDatabaseType()))
            {
                bool flag2;
                try
                {
                    flag2 = base.Load(fileAccess);
                }
                catch (Exception exception)
                {
                    object[] add = new object[] { exception.Message, base.FileName };
                    throw Error.GetError(exception, 0x1c4, 0x1b, add);
                }
                if (!flag2)
                {
                    return false;
                }
                this.FilterLoadedProperties();
                if (this.GetProperty("version").CompareTo(ThisVersion) > 0)
                {
                    throw Error.GetError(0x1c5);
                }
                if (this.GetIntegerProperty("LibCore.script_format") != 0)
                {
                    throw Error.GetError(0x1c5);
                }
            }
            return true;
        }

        public override void Save()
        {
            if ((DatabaseUrl.IsFileBasedDatabaseType(this._database.GetDatabaseType()) && !this._database.IsFilesReadOnly()) && !this._database.IsFilesInAssembly())
            {
                try
                {
                    LibCoreProperties properties1 = new LibCoreProperties(this._database.GetPath(), this._database.logger.GetFileAccess());
                    properties1.SetProperty("version", ThisVersion);
                    properties1.SetProperty("modified", this.GetProperty("modified"));
                    properties1.Save(base.FileName + ".properties.new");
                    base.Fa.RenameElement(base.FileName + ".properties.new", base.FileName + ".properties");
                }
                catch (Exception exception)
                {
                    this._database.logger.LogSevereEvent(FwNs.Core.LC.cResources.SR.LibCoreDatabaseProperties_Save_save_failed, exception);
                    object[] add = new object[] { exception.Message, base.FileName };
                    throw Error.GetError(exception, 0x1c4, 0x1b, add);
                }
            }
        }

        public void SetDbModified(int mode)
        {
            string str = "no";
            if (mode == 1)
            {
                str = "yes";
            }
            else if (mode == 2)
            {
                str = "no-new-files";
            }
            base.SetProperty("modified", str);
            this.Save();
        }

        private void SetNewDatabaseProperties()
        {
            base.SetProperty("version", ThisVersion);
            base.SetProperty("modified", "no-new-files");
            if (this._database.logger.isStoredFileAccess)
            {
                base.SetProperty("LibCore.cache_rows", 0x61a8);
                base.SetProperty("LibCore.cache_size", 0x1770);
                base.SetProperty("LibCore.log_size", 10);
                base.SetProperty("sql.enforce_size", true);
                base.SetProperty("LibCore.nio_data_file", false);
            }
            base.SetProperty("shutdown", true);
        }

        public void SetUrlProperties(LibCoreProperties p)
        {
            if (p != null)
            {
                foreach (string str in p.PropertyNames())
                {
                    object[] objArray;
                    if (DbMeta.TryGetValue(str, out objArray) && ((objArray != null) || (((int) objArray[1]) == 2)))
                    {
                        base.SetProperty(str, p.GetProperty(str));
                    }
                }
            }
        }

        public static string ThisVersion
        {
            get
            {
                if (_thisVersion == null)
                {
                    char[] separator = new char[] { ',' };
                    string text1 = Assembly.GetExecutingAssembly().FullName.Split(separator)[1];
                    string text2 = text1.Substring(text1.IndexOf('=') + 1);
                    int index = text2.IndexOf('.');
                    index = text2.IndexOf('.', index + 1);
                    _thisVersion = text2.Substring(0, index);
                }
                return _thisVersion;
            }
        }

        public static string ThisFullVersion
        {
            get
            {
                if (_thisFullVersion == null)
                {
                    char[] separator = new char[] { ',' };
                    string text1 = Assembly.GetExecutingAssembly().FullName.Split(separator)[1];
                    _thisFullVersion = text1.Substring(text1.IndexOf('=') + 1);
                }
                return _thisFullVersion;
            }
        }
    }
}

