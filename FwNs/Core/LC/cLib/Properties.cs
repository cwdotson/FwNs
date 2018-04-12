namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class Properties : Dictionary<string, string>
    {
        public Properties() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        private Properties(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string GetProperty(string key)
        {
            if (base.ContainsKey(key))
            {
                return base[key];
            }
            return null;
        }

        public void Load(Stream ips)
        {
            foreach (string str in new StreamReader(ips).ReadToEnd().Split(Environment.NewLine.ToCharArray()))
            {
                if (!string.IsNullOrEmpty(str.Trim()) && !str.Trim().StartsWith("#"))
                {
                    char[] separator = new char[] { '=' };
                    string[] strArray2 = str.Split(separator);
                    base[strArray2[0]] = strArray2[1];
                }
            }
        }

        public void Load(string filePath, UtlFileAccess fileAccess)
        {
            List<string> list = new List<string>();
            using (StreamReader reader = new StreamReader(fileAccess.OpenInputStreamElement(filePath)))
            {
                string str;
            Label_0013:
                str = reader.ReadLine();
                if (str != null)
                {
                    list.Add(str);
                    goto Label_0013;
                }
            }
            string[] strArray = list.ToArray();
            for (int i = 0; i < strArray.Length; i++)
            {
                string str2 = strArray[i].Trim();
                if ((!string.IsNullOrEmpty(str2) && !str2.StartsWith("#")) && !str2.StartsWith("#"))
                {
                    char[] separator = new char[] { '=' };
                    string[] strArray2 = str2.Split(separator);
                    if (strArray2.Length != 2)
                    {
                        throw new FieldAccessException(filePath);
                    }
                    base[strArray2[0]] = strArray2[1];
                }
            }
        }

        public Dictionary<string, string>.KeyCollection PropertyNames()
        {
            return base.Keys;
        }

        public void SetProperty(string key, string value)
        {
            base[key] = value;
        }

        public void Store(Stream ips, string comments)
        {
            StreamWriter writer = new StreamWriter(ips);
            writer.WriteLine("# " + comments);
            foreach (KeyValuePair<string, string> pair in this)
            {
                writer.WriteLine(pair.Key + "=" + pair.Value);
            }
            writer.Flush();
        }
    }
}

