namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class LibCoreProperties
    {
        public int[] ErrorCodes;
        public string[] ErrorKeys;
        public UtlFileAccess Fa;
        protected string FileName;
        protected Properties StringProps;

        public LibCoreProperties()
        {
            this.ErrorCodes = new int[0];
            this.ErrorKeys = new string[0];
            this.StringProps = new Properties();
            this.FileName = null;
        }

        public LibCoreProperties(Properties props)
        {
            this.ErrorCodes = new int[0];
            this.ErrorKeys = new string[0];
            this.StringProps = props;
        }

        public LibCoreProperties(string name)
        {
            this.ErrorCodes = new int[0];
            this.ErrorKeys = new string[0];
            this.StringProps = new Properties();
            this.FileName = name;
            this.Fa = FileUtil.GetDefaultInstance();
        }

        public LibCoreProperties(string name, UtlFileAccess accessor)
        {
            this.ErrorCodes = new int[0];
            this.ErrorKeys = new string[0];
            this.StringProps = new Properties();
            this.FileName = name;
            this.Fa = accessor;
        }

        public bool CheckFileExists()
        {
            string elementName = this.FileName + ".properties";
            return this.Fa.IsStreamElement(elementName);
        }

        public string[] GetErrorKeys()
        {
            return this.ErrorKeys;
        }

        public int GetIntegerProperty(string key, int defaultValue)
        {
            string property = this.GetProperty(key);
            try
            {
                if (property != null)
                {
                    defaultValue = int.Parse(property);
                }
            }
            catch (Exception)
            {
            }
            return defaultValue;
        }

        public int GetIntegerProperty(string key, int defaultValue, int[] values)
        {
            string property = this.GetProperty(key);
            int num = defaultValue;
            try
            {
                if (property != null)
                {
                    num = int.Parse(property);
                }
            }
            catch (FormatException)
            {
            }
            if (Array.IndexOf<int>(values, num) == -1)
            {
                return defaultValue;
            }
            return num;
        }

        public int GetIntegerProperty(string key, int defaultValue, int minimum, int maximum)
        {
            string property = this.GetProperty(key);
            try
            {
                defaultValue = int.Parse(property);
            }
            catch (Exception)
            {
            }
            if (defaultValue < minimum)
            {
                defaultValue = minimum;
                return defaultValue;
            }
            if (defaultValue > maximum)
            {
                defaultValue = maximum;
            }
            return defaultValue;
        }

        public Properties GetProperties()
        {
            return this.StringProps;
        }

        public virtual string GetProperty(string key)
        {
            return this.StringProps.GetProperty(key);
        }

        public string GetProperty(string key, string defaultValue)
        {
            if (!this.StringProps.ContainsKey(key))
            {
                return defaultValue;
            }
            return this.StringProps[key];
        }

        public bool IsEmpty()
        {
            return (this.StringProps.Count == 0);
        }

        public virtual bool IsPropertyTrue(string key)
        {
            return this.IsPropertyTrue(key, false);
        }

        public virtual bool IsPropertyTrue(string key, bool defaultValue)
        {
            string str;
            try
            {
                str = this.StringProps[key];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
            return (string.Compare(str, "TRUE", true) == 0);
        }

        public virtual bool Load(UtlFileAccess fileAccess)
        {
            if (!this.CheckFileExists())
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.FileName))
            {
                throw new FileNotFoundException(Error.GetMessage(0x1c));
            }
            string filePath = this.FileName + ".properties";
            this.StringProps.Load(filePath, fileAccess);
            return true;
        }

        public IEnumerable<string> PropertyNames()
        {
            return this.StringProps.PropertyNames();
        }

        public void RemoveProperty(string key)
        {
            this.StringProps.Remove(key);
        }

        public virtual void Save()
        {
            if (string.IsNullOrEmpty(this.FileName))
            {
                throw new FileNotFoundException(Error.GetMessage(0x1c));
            }
            string fileString = this.FileName + ".properties";
            this.Save(fileString);
        }

        public void Save(string fileString)
        {
            this.Fa.CreateParentDirs(fileString);
            using (Stream stream = this.Fa.OpenOutputStreamElement(fileString))
            {
                this.StringProps.Store(stream, "LibCore Database Engine " + LibCoreDatabaseProperties.ThisFullVersion);
                this.Fa.GetFileSync(stream).Sync();
            }
        }

        public void SetFileName(string name)
        {
            this.FileName = name;
        }

        public string SetProperty(string key, bool value)
        {
            return this.SetProperty(key, value.ToString());
        }

        public string SetProperty(string key, int value)
        {
            return this.SetProperty(key, value.ToString());
        }

        public string SetProperty(string key, string value)
        {
            this.StringProps[key] = value;
            return key;
        }

        public string SetPropertyIfNotExists(string key, string value)
        {
            if (!this.StringProps.ContainsKey(key))
            {
                return this.SetProperty(key, value);
            }
            return key;
        }
    }
}

