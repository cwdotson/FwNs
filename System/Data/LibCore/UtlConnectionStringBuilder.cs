namespace System.Data.LibCore
{
    using FwNs.Core.LC.cPersist;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [DefaultProperty("DataSource"), DefaultMember("Item")]
    public sealed class UtlConnectionStringBuilder : DbConnectionStringBuilder
    {
        private Hashtable _properties;

        public UtlConnectionStringBuilder()
        {
            this.Initialize(null);
        }

        public UtlConnectionStringBuilder(string connectionString)
        {
            this.Initialize(connectionString);
        }

        private void FallbackGetProperties(Hashtable propertyList)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this, true))
            {
                if ((descriptor.Name != "ConnectionString") && !propertyList.ContainsKey(descriptor.DisplayName))
                {
                    propertyList.Add(descriptor.DisplayName, descriptor);
                }
            }
        }

        public static string GetCipherIV(string cipherName)
        {
            return Crypto.GetNewStrIv(cipherName);
        }

        public static string GetCipherKey(string cipherName)
        {
            return Crypto.GetNewStrKey(cipherName);
        }

        private void Initialize(string cnnString)
        {
            this._properties = new Hashtable(StringComparer.OrdinalIgnoreCase);
            try
            {
                this.GetProperties(this._properties);
            }
            catch (NotImplementedException)
            {
                this.FallbackGetProperties(this._properties);
            }
            if (!string.IsNullOrEmpty(cnnString))
            {
                base.ConnectionString = cnnString;
            }
        }

        public override bool TryGetValue(string keyword, out object value)
        {
            bool flag = base.TryGetValue(keyword, out value);
            if (this._properties.ContainsKey(keyword))
            {
                PropertyDescriptor descriptor = this._properties[keyword] as PropertyDescriptor;
                if (descriptor == null)
                {
                    return flag;
                }
                if (flag)
                {
                    if (descriptor.PropertyType == typeof(bool))
                    {
                        value = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        value = TypeDescriptor.GetConverter(descriptor.PropertyType).ConvertFrom(value);
                    }
                    return flag;
                }
                DefaultValueAttribute attribute = descriptor.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
                if (attribute != null)
                {
                    value = attribute.Value;
                    flag = true;
                }
            }
            return flag;
        }

        [Browsable(true), DefaultValue(true), DisplayName("auto commit")]
        public bool AutoCommit
        {
            get
            {
                object obj2;
                this.TryGetValue("auto commit", out obj2);
                return Convert.ToBoolean(obj2, CultureInfo.InvariantCulture);
            }
            set
            {
                this["auto commit"] = value;
            }
        }

        [Browsable(true), DefaultValue(true), DisplayName("auto shutdown")]
        public bool AutoShutdown
        {
            get
            {
                object obj2;
                this.TryGetValue("auto shutdown", out obj2);
                return Convert.ToBoolean(obj2, CultureInfo.InvariantCulture);
            }
            set
            {
                this["auto shutdown"] = value;
            }
        }

        [Browsable(true), DefaultValue(false), DisplayName("pooling")]
        public bool Pooling
        {
            get
            {
                object obj2;
                this.TryGetValue("pooling", out obj2);
                return Convert.ToBoolean(obj2, CultureInfo.InvariantCulture);
            }
            set
            {
                this["pooling"] = value;
            }
        }

        [Browsable(true), DefaultValue(false), DisplayName("readonly")]
        public bool ReadOnly
        {
            get
            {
                object obj2;
                this.TryGetValue("readonly", out obj2);
                return Convert.ToBoolean(obj2, CultureInfo.InvariantCulture);
            }
            set
            {
                this["readonly"] = value;
            }
        }

        [Browsable(true), DefaultValue(""), DisplayName("data source")]
        public string DataSource
        {
            get
            {
                object obj2;
                this.TryGetValue("data source", out obj2);
                return obj2.ToString();
            }
            set
            {
                this["data source"] = value;
            }
        }

        [Browsable(true), DefaultValue(""), DisplayName("initial catalog")]
        public string InitialCatalog
        {
            get
            {
                object obj2;
                this.TryGetValue("initial catalog", out obj2);
                return obj2.ToString();
            }
            set
            {
                this["initial catalog"] = value;
            }
        }

        [Browsable(true), DefaultValue("memory"), DisplayName("connection type")]
        public string ConnectionType
        {
            get
            {
                object obj2;
                this.TryGetValue("connection type", out obj2);
                return obj2.ToString();
            }
            set
            {
                if (!value.Equals("memory", StringComparison.OrdinalIgnoreCase) && !value.Equals("file", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException();
                }
                this["connection type"] = value;
            }
        }

        [Browsable(true), DefaultValue(false), DisplayName("enlist")]
        public bool Enlist
        {
            get
            {
                object obj2;
                this.TryGetValue("enlist", out obj2);
                return Convert.ToBoolean(obj2, CultureInfo.InvariantCulture);
            }
            set
            {
                this["enlist"] = value;
            }
        }

        [Browsable(true), DefaultValue(""), DisplayName("password"), PasswordPropertyText(true)]
        public string Password
        {
            get
            {
                object obj2;
                this.TryGetValue("password", out obj2);
                return obj2.ToString();
            }
            set
            {
                this["password"] = value;
            }
        }

        [Browsable(true), DefaultValue(""), DisplayName("user")]
        public string User
        {
            get
            {
                object obj2;
                this.TryGetValue("user", out obj2);
                return obj2.ToString();
            }
            set
            {
                this["user"] = value;
            }
        }

        [Browsable(true), DefaultValue(0x1000), DisplayName("isolation level")]
        public IsolationLevel DefaultIsolationLevel
        {
            get
            {
                object obj2;
                this.TryGetValue("isolation level", out obj2);
                if (obj2 is string)
                {
                    return (IsolationLevel) TypeDescriptor.GetConverter(typeof(IsolationLevel)).ConvertFrom(obj2);
                }
                return (IsolationLevel) obj2;
            }
            set
            {
                this["isolation level"] = value;
            }
        }

        [Browsable(true), DefaultValue("Rijndael"), DisplayName("cipher name")]
        public string CipherName
        {
            get
            {
                object obj2;
                this.TryGetValue("cipher name", out obj2);
                return obj2.ToString();
            }
            set
            {
                this["cipher name"] = value;
            }
        }

        [Browsable(true), DefaultValue(""), DisplayName("cipher key")]
        public string CipherKey
        {
            get
            {
                object obj2;
                this.TryGetValue("cipher key", out obj2);
                return obj2.ToString();
            }
            set
            {
                this["cipher key"] = value;
            }
        }

        [Browsable(true), DefaultValue(""), DisplayName("cipher iv")]
        public string CipherIV
        {
            get
            {
                object obj2;
                this.TryGetValue("cipher iv", out obj2);
                return obj2.ToString();
            }
            set
            {
                this["cipher iv"] = value;
            }
        }
    }
}

