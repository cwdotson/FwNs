namespace System.Data.LibCore
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class UtlConnectionOptions
    {
        private readonly Dictionary<string, string> _connectionOptions;
        private static readonly HashSet<string> _validOptions;

        static UtlConnectionOptions()
        {
            HashSet<string> set1 = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { 
                "initial catalog",
                "command timeout",
                "connection timeout",
                "sl oob",
                "pooling",
                "auto shutdown",
                "referential intergrity",
                "ignore case",
                "max rows",
                "fetch size",
                "readonly",
                "enlist",
                "auto commit",
                "connection lifetime",
                "min pool size",
                "max pool size",
                "user",
                "password",
                "connect timeout",
                "initial catalog",
                "data source",
                "cipher name",
                "cipher key",
                "cipher iv",
                "context connection",
                "connection type",
                "isolation level"
            };
            _validOptions = set1;
        }

        public UtlConnectionOptions()
        {
            this._connectionOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.SetDefaults();
        }

        public UtlConnectionOptions(string connectionString)
        {
            this._connectionOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.ParseConnectionString(connectionString);
        }

        private static string ExpandFileName(string sourceFile, Dictionary<string, string> connectionOptions)
        {
            string str = "|DataDirectory|";
            if (!string.IsNullOrEmpty(sourceFile))
            {
                if (sourceFile.StartsWith(str, StringComparison.OrdinalIgnoreCase))
                {
                    string data = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
                    if (string.IsNullOrEmpty(data))
                    {
                        data = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    if ((sourceFile.Length > str.Length) && ((sourceFile[str.Length] == Path.DirectorySeparatorChar) || (sourceFile[str.Length] == Path.AltDirectorySeparatorChar)))
                    {
                        sourceFile = sourceFile.Remove(str.Length, 1);
                    }
                    sourceFile = Path.Combine(data, sourceFile.Substring(str.Length));
                    sourceFile = Path.GetFullPath(sourceFile);
                }
                sourceFile = sourceFile.Trim();
                if (((sourceFile[sourceFile.Length - 1] == Path.DirectorySeparatorChar) || (sourceFile[sourceFile.Length - 1] == Path.AltDirectorySeparatorChar)) || (sourceFile[sourceFile.Length - 1] == '"'))
                {
                    sourceFile = sourceFile.Substring(0, sourceFile.Length - 1);
                }
                if (sourceFile[0] == '"')
                {
                    sourceFile = sourceFile.Substring(1);
                }
            }
            return sourceFile;
        }

        private bool GetBool(string key)
        {
            return (this._connectionOptions.ContainsKey(key) && bool.Parse(this._connectionOptions[key]));
        }

        private int GetInt(string key)
        {
            if (this._connectionOptions.ContainsKey(key))
            {
                return Convert.ToInt32(this._connectionOptions[key], CultureInfo.InvariantCulture);
            }
            return 0;
        }

        private long GetLong(string key)
        {
            if (this._connectionOptions.ContainsKey(key))
            {
                return Convert.ToInt64(this._connectionOptions[key], CultureInfo.InvariantCulture);
            }
            return 0L;
        }

        private string GetString(string key)
        {
            if (this._connectionOptions.ContainsKey(key))
            {
                return this._connectionOptions[key];
            }
            return null;
        }

        public void ParseConnectionString(string connectionString)
        {
            this.SetDefaults();
            this.ConnectionString = connectionString;
            char[] separator = new char[] { ';' };
            string[] strArray = connectionString.Split(separator);
            string[] strArray2 = new string[2];
            int length = strArray.Length;
            for (int i = 0; i < length; i++)
            {
                char[] chArray2 = new char[] { '=' };
                strArray2 = strArray[i].Split(chArray2, 2);
                if (strArray2.Length == 2)
                {
                    string str = strArray2[1].Trim();
                    if (str.StartsWith("\"") && str.EndsWith("\""))
                    {
                        str = str.Substring(1, str.Length - 2).Replace("\"\"", "\"");
                    }
                    string item = strArray2[0].Trim();
                    if (item != string.Empty)
                    {
                        if (!_validOptions.Contains(item))
                        {
                            throw new ArgumentException("Invalid Connection String Option: " + item);
                        }
                        this._connectionOptions[item] = str;
                        if (string.Compare(item, "data source", true) == 0)
                        {
                            this._connectionOptions["initial catalog"] = str;
                        }
                    }
                }
                else
                {
                    if (strArray2.Length != 1)
                    {
                        object[] args = new object[] { (strArray2.Length != 0) ? strArray2[0] : "null" };
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid ConnectionString format for parameter \"{0}\"", args));
                    }
                    string item = strArray2[0].Trim();
                    if (item != string.Empty)
                    {
                        if (!_validOptions.Contains(item))
                        {
                            throw new ArgumentException("Invalid Connection String Option: " + item);
                        }
                        this._connectionOptions[item] = "";
                    }
                }
            }
        }

        private void SetDefaults()
        {
            this._connectionOptions.Clear();
            this._connectionOptions.Add("connection type", "FILE");
            this._connectionOptions.Add("data source", string.Empty);
            this._connectionOptions.Add("user", string.Empty);
            this._connectionOptions.Add("password", string.Empty);
            this._connectionOptions.Add("initial catalog", string.Empty);
            this._connectionOptions.Add("pooling", "false");
            this._connectionOptions.Add("min pool size", "0");
            this._connectionOptions.Add("max pool size", "50");
            this._connectionOptions.Add("connection lifetime", "600");
            this._connectionOptions.Add("command timeout", "30");
            this._connectionOptions.Add("server type", "file");
            this._connectionOptions.Add("isolation level", System.Data.IsolationLevel.ReadCommitted.ToString());
            this._connectionOptions.Add("max rows", "0");
            this._connectionOptions.Add("fetch size", "0");
            this._connectionOptions.Add("auto commit", "true");
            this._connectionOptions.Add("readonly", "false");
        }

        public void Validate()
        {
            string str;
            string str2;
            string str3;
            if ((this.GetString("isolation level") != null) && (((str = this.GetString("isolation level").ToUpper(CultureInfo.InvariantCulture)) == null) || (((str != "READCOMMITTED") && (str != "READUNCOMMITTED")) && ((str != "REPEATABLEREAD") && (str != "SERIALIZABLE")))))
            {
                throw new ArgumentException("Specified Isolation Level is not valid.");
            }
            if (this.GetString("connection type") == null)
            {
                throw new ArgumentException("Required attribute connection type is missing.");
            }
            if (((str2 = this.GetString("connection type").ToUpper(CultureInfo.InvariantCulture)) == null) || (((str2 != "MEMORY") && (str2 != "FILE")) && (str2 != "RESOURCE")))
            {
                throw new ArgumentException("Specified connection type is not valid.");
            }
            if ((this.CryptoType != null) && (((str3 = this.CryptoType.ToUpper(CultureInfo.InvariantCulture)) == null) || (((str3 != "DES") && (str3 != "TRIPLEDES")) && ((str3 != "AES") && (str3 != "RIJNDAEL")))))
            {
                throw new ArgumentException("Specified cipher type is not valid.");
            }
            if (string.IsNullOrEmpty(this.ConnectionType))
            {
                throw new ArgumentException("Required connection string arguments missing: connection type");
            }
            if (string.IsNullOrEmpty(this.Database))
            {
                throw new ArgumentException("Required connection string arguments missing: initial catalog");
            }
            if (string.IsNullOrEmpty(this.DataSource) && ((this.ConnectionType == "http://") || (this.ConnectionType == "utl://")))
            {
                throw new ArgumentException("Required connection string arguments missing: data source");
            }
            if (this.MinPoolSize > this.MaxPoolSize)
            {
                throw new ArgumentException("MinPoolSize should be less than MaxPoolSize");
            }
            if (!string.IsNullOrEmpty(this.CryptoType) && (string.IsNullOrEmpty(this.CryptoKey) || string.IsNullOrEmpty(this.CryptoIv)))
            {
                throw new ArgumentException("Required connection string arguments missing: 'crypto key' ,'crypto iv'");
            }
        }

        public string ConnectionString { get; set; }

        public string Database
        {
            get
            {
                string sourceFile = this.GetString("initial catalog");
                if (sourceFile == null)
                {
                    return null;
                }
                return ExpandFileName(sourceFile, this._connectionOptions);
            }
        }

        public int CommandTimeout
        {
            get
            {
                return this.GetInt("command timeout");
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                return this.GetInt("connection timeout");
            }
        }

        public bool SlOob
        {
            get
            {
                return this.GetBool("sl oob");
            }
        }

        public bool Pooling
        {
            get
            {
                return this.GetBool("pooling");
            }
        }

        public bool AutoShutdown
        {
            get
            {
                if (this._connectionOptions.ContainsKey("auto shutdown"))
                {
                    return this.GetBool("auto shutdown");
                }
                return (this.ConnectionType == "file:");
            }
        }

        public bool ReferentialIntergrity
        {
            get
            {
                return this.GetBool("referential intergrity");
            }
        }

        public bool IgnoreCase
        {
            get
            {
                return this.GetBool("ignore case");
            }
        }

        public int MaxRows
        {
            get
            {
                return this.GetInt("max rows");
            }
        }

        public int FetchSize
        {
            get
            {
                return this.GetInt("fetch size");
            }
        }

        public bool Readonly
        {
            get
            {
                return this.GetBool("readonly");
            }
        }

        public bool Enlist
        {
            get
            {
                return this.GetBool("enlist");
            }
        }

        public bool AutoCommit
        {
            get
            {
                return this.GetBool("auto commit");
            }
        }

        public long ConnectionLifeTime
        {
            get
            {
                return this.GetLong("connection lifetime");
            }
        }

        public int MinPoolSize
        {
            get
            {
                return this.GetInt("min pool size");
            }
        }

        public int MaxPoolSize
        {
            get
            {
                return this.GetInt("max pool size");
            }
        }

        public string User
        {
            get
            {
                return this.GetString("user");
            }
        }

        public string Password
        {
            get
            {
                return this.GetString("password");
            }
        }

        public string DataSource
        {
            get
            {
                return this.GetString("data source");
            }
        }

        public string InitialCatalog
        {
            get
            {
                return this.GetString("initial catalog");
            }
        }

        public string CryptoType
        {
            get
            {
                return this.GetString("cipher name");
            }
        }

        public string CryptoKey
        {
            get
            {
                return this.GetString("cipher key");
            }
        }

        public string CryptoIv
        {
            get
            {
                return this.GetString("cipher iv");
            }
        }

        public bool ContextConnection
        {
            get
            {
                return this.GetBool("context connection");
            }
        }

        public string ConnectionType
        {
            get
            {
                switch (this.GetString("connection type").ToUpper(CultureInfo.InvariantCulture))
                {
                    case "MEMORY":
                        return "mem:";

                    case "FILE":
                        return "file:";

                    case "RESOURCE":
                        return "res:";

                    case "SERVER":
                        return "utl://";

                    case "WEBSERVER":
                        return "http://";
                }
                return "file:";
            }
        }

        public System.Data.IsolationLevel IsolationLevel
        {
            get
            {
                switch (this.GetString("isolation level").ToUpper(CultureInfo.InvariantCulture))
                {
                    case "READCOMMITTED":
                        return System.Data.IsolationLevel.ReadCommitted;

                    case "READUNCOMMITTED":
                        return System.Data.IsolationLevel.ReadUncommitted;

                    case "REPEATABLEREAD":
                        return System.Data.IsolationLevel.RepeatableRead;

                    case "SERIALIZABLE":
                        return System.Data.IsolationLevel.Serializable;
                }
                return System.Data.IsolationLevel.ReadCommitted;
            }
        }
    }
}

