namespace FwNs.Data.FyleDbNs
{
    using FwNs.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class FileDb : IDisposable
    {
        private const string StrIndex = "index";
        private FileDbEngine _db = new FileDbEngine();
        private Encryptor _encryptor;
        private int _encryptKeyHashCode;
        private bool _disposed;

        public FileDb()
        {
            this.AutoFlush = true;
        }

        public int AddRecord(FieldValues values)
        {
            lock (this)
            {
                return this._db.addRecord(values);
            }
        }

        public void Clean()
        {
            lock (this)
            {
                this._db.cleanup(false);
            }
        }

        public void Close()
        {
            lock (this)
            {
                this._db.close();
            }
        }

        internal void Create(Table table, string dbName)
        {
            this._db.create(dbName, table.Fields.ToArray());
            using (List<Record>.Enumerator enumerator = table.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    FieldValues fieldValues = enumerator.Current.GetFieldValues();
                    this.AddRecord(fieldValues);
                }
            }
        }

        public void Create(string dbName, Field[] fields)
        {
            lock (this)
            {
                this._db.create(dbName, fields);
            }
        }

        private Record createRecord(object[] record, string[] fieldList, bool includeIndex)
        {
            Record record2 = null;
            if (record == null)
            {
                return record2;
            }
            int num = includeIndex ? 1 : 0;
            FwNs.Data.FyleDbNs.Fields fields = null;
            if (fieldList != null)
            {
                fields = new FwNs.Data.FyleDbNs.Fields(fieldList.Length + num);
                for (int i = 0; i < fieldList.Length; i++)
                {
                    string fieldName = fieldList[i];
                    if (fields.ContainsKey(fieldName))
                    {
                        throw new FileDbException(string.Format("Field name cannot be specified twice in list - {0}", fieldName), FileDbExceptionsEnum.FieldSpecifiedTwice);
                    }
                    fields.Add(this._db.Fields[fieldName]);
                }
            }
            else
            {
                fields = new FwNs.Data.FyleDbNs.Fields(this._db.Fields.Count + num);
                foreach (Field field in this._db.Fields)
                {
                    fields.Add(field);
                }
            }
            if (includeIndex)
            {
                fields.Add(new Field("index", DataTypeEnum.Int, fields.Count));
            }
            return new Record(fields, record);
        }

        private T createT<T>(object[] record, string[] fieldList, bool includeIndex) where T: class, new()
        {
            T local = default(T);
            if (record != null)
            {
                int num = includeIndex ? 1 : 0;
                FwNs.Data.FyleDbNs.Fields fields = null;
                if (fieldList != null)
                {
                    fields = new FwNs.Data.FyleDbNs.Fields(fieldList.Length + num);
                    for (int j = 0; j < fieldList.Length; j++)
                    {
                        string fieldName = fieldList[j];
                        if (fields.ContainsKey(fieldName))
                        {
                            throw new FileDbException(string.Format("Field name cannot be specified twice in list - {0}", fieldName), FileDbExceptionsEnum.FieldSpecifiedTwice);
                        }
                        fields.Add(this._db.Fields[fieldName]);
                    }
                }
                else
                {
                    fields = new FwNs.Data.FyleDbNs.Fields(this._db.Fields.Count + num);
                    foreach (Field field in this._db.Fields)
                    {
                        fields.Add(field);
                    }
                }
                if (includeIndex)
                {
                    fields.Add(new Field("index", DataTypeEnum.Int, fields.Count));
                }
                local = Activator.CreateInstance<T>();
                PropertyInfo[] properties = typeof(T).GetProperties(~BindingFlags.Static);
                new Dictionary<string, PropertyInfo>(properties.Length);
                for (int i = 0; i < fields.Count; i++)
                {
                    <>c__DisplayClass98_0<T> class_;
                    Field field = fields[i];
                    PropertyInfo info = Enumerable.FirstOrDefault<PropertyInfo>(Enumerable.Where<PropertyInfo>(properties, new Func<PropertyInfo, bool>(class_, this.<createT>b__0)));
                    if ((info != null) && info.CanWrite)
                    {
                        object obj2 = record[i];
                        info.SetValue(local, obj2, null);
                    }
                }
            }
            return local;
        }

        private Table createTable(object[][] records, string[] fieldList, bool includeIndex, string[] orderByList)
        {
            int num = includeIndex ? 1 : 0;
            FwNs.Data.FyleDbNs.Fields fields = null;
            if (fieldList != null)
            {
                fields = new FwNs.Data.FyleDbNs.Fields(fieldList.Length + num);
                for (int i = 0; i < fieldList.Length; i++)
                {
                    string fieldName = fieldList[i];
                    if (fields.ContainsKey(fieldName))
                    {
                        throw new FileDbException(string.Format("Field name cannot be specified twice in list - {0}", fieldName), FileDbExceptionsEnum.FieldSpecifiedTwice);
                    }
                    fields.Add(this._db.Fields[fieldName]);
                }
            }
            else
            {
                fields = new FwNs.Data.FyleDbNs.Fields(this._db.Fields.Count + num);
                foreach (Field field in this._db.Fields)
                {
                    fields.Add(field);
                }
            }
            if (includeIndex)
            {
                fields.Add(new Field("index", DataTypeEnum.Int, fields.Count));
            }
            return new Table(fields, records, true);
        }

        private IList<T> createTList<T>(object[][] records, string[] fieldList, bool includeIndex, string[] orderByList) where T: class, new()
        {
            int num = includeIndex ? 1 : 0;
            FwNs.Data.FyleDbNs.Fields fields = null;
            if (fieldList != null)
            {
                fields = new FwNs.Data.FyleDbNs.Fields(fieldList.Length + num);
                for (int i = 0; i < fieldList.Length; i++)
                {
                    string fieldName = fieldList[i];
                    if (fields.ContainsKey(fieldName))
                    {
                        throw new FileDbException(string.Format("Field name cannot be specified twice in list - {0}", fieldName), FileDbExceptionsEnum.FieldSpecifiedTwice);
                    }
                    fields.Add(this._db.Fields[fieldName]);
                }
            }
            else
            {
                fields = new FwNs.Data.FyleDbNs.Fields(this._db.Fields.Count + num);
                foreach (Field field in this._db.Fields)
                {
                    fields.Add(field);
                }
            }
            if (includeIndex)
            {
                fields.Add(new Field("index", DataTypeEnum.Int, fields.Count));
            }
            PropertyInfo[] properties = typeof(T).GetProperties(~BindingFlags.Static);
            Dictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>(properties.Length);
            foreach (Field field in fields)
            {
                <>c__DisplayClass77_0<T> class_;
                PropertyInfo info = Enumerable.FirstOrDefault<PropertyInfo>(Enumerable.Where<PropertyInfo>(properties, new Func<PropertyInfo, bool>(class_, this.<createTList>b__0)));
                if (info != null)
                {
                    Type type = null;
                    switch (field.DataType)
                    {
                        case DataTypeEnum.Byte:
                            type = field.IsArray ? typeof(byte[]) : typeof(byte);
                            break;

                        case DataTypeEnum.Int:
                            type = field.IsArray ? typeof(int[]) : typeof(int);
                            break;

                        case DataTypeEnum.UInt:
                            type = field.IsArray ? typeof(uint[]) : typeof(uint);
                            break;

                        case DataTypeEnum.Float:
                            type = field.IsArray ? typeof(float[]) : typeof(float);
                            break;

                        case DataTypeEnum.Double:
                            type = field.IsArray ? typeof(double[]) : typeof(double);
                            break;

                        case DataTypeEnum.DateTime:
                            type = field.IsArray ? typeof(DateTime[]) : typeof(DateTime);
                            break;

                        case DataTypeEnum.String:
                            type = field.IsArray ? typeof(string[]) : typeof(string);
                            break;

                        case DataTypeEnum.Bool:
                            type = field.IsArray ? typeof(bool[]) : typeof(bool);
                            break;
                    }
                    if (info.PropertyType != type)
                    {
                        throw new Exception(string.Format("The type of Property {0} doesn't match the Field DataType {1}", info.Name, field.DataType.ToString()));
                    }
                    dictionary.Add(field.Name, info);
                }
            }
            List<T> list = new List<T>((records != null) ? records.Length : 0);
            if (records != null)
            {
                for (int i = 0; i < records.Length; i++)
                {
                    object[] objArray = records[i];
                    T local = Activator.CreateInstance<T>();
                    for (int j = 0; j < fields.Count; j++)
                    {
                        Field field2 = fields[j];
                        if (dictionary.ContainsKey(field2.Name))
                        {
                            PropertyInfo info2 = dictionary[field2.Name];
                            if (info2.CanWrite)
                            {
                                object obj2 = objArray[j];
                                info2.SetValue(local, obj2, null);
                            }
                        }
                    }
                    list.Add(local);
                }
            }
            return list;
        }

        public string DecryptString(string encryptKey, string value)
        {
            int num2;
            int hashCode = encryptKey.GetHashCode();
            if ((this._encryptor == null) || (this._encryptKeyHashCode != hashCode))
            {
                this._encryptor = new Encryptor(encryptKey, base.GetType().ToString());
            }
            this._encryptKeyHashCode = hashCode;
            byte[] bytes = Encdng.GetBytes(value, out num2);
            using (BinaryReader reader = new BinaryReader(new MemoryStream(this._encryptor.Decrypt(bytes))))
            {
                return reader.ReadString();
            }
        }

        public int DeleteAllRecords()
        {
            lock (this)
            {
                return this._db.removeAll();
            }
        }

        public bool DeleteRecordByIndex(int index)
        {
            lock (this)
            {
                return this._db.removeByIndex(index);
            }
        }

        public bool DeleteRecordByKey(object key)
        {
            lock (this)
            {
                return this._db.removeByKey(key);
            }
        }

        public int DeleteRecords(FilterExpression filter)
        {
            lock (this)
            {
                return this._db.removeByValue(filter);
            }
        }

        public int DeleteRecords(FilterExpressionGroup filter)
        {
            lock (this)
            {
                return this._db.removeByValues(filter);
            }
        }

        public int DeleteRecords(string filter)
        {
            lock (this)
            {
                FilterExpressionGroup searchExpGrp = FilterExpressionGroup.Parse(filter);
                return this._db.removeByValues(searchExpGrp);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.Close();
                }
                this._disposed = true;
            }
        }

        public void Drop(string dbName)
        {
            lock (this)
            {
                this._db.drop(dbName);
            }
        }

        public string EncryptString(string encryptKey, string value)
        {
            MemoryStream output = new MemoryStream();
            int hashCode = encryptKey.GetHashCode();
            if ((this._encryptor == null) || (this._encryptKeyHashCode != hashCode))
            {
                this._encryptor = new Encryptor(encryptKey, base.GetType().ToString());
            }
            this._encryptKeyHashCode = hashCode;
            using (BinaryWriter writer = new BinaryWriter(output))
            {
                writer.Write(value);
                this._encryptor.Encrypt(output.ToArray());
                throw new Exception();
            }
        }

        ~FileDb()
        {
            this.Dispose(false);
        }

        public void Flush()
        {
            lock (this)
            {
                this._db.flush(true);
            }
        }

        public Record GetCurrentRecord(string[] fieldList, bool includeIndex)
        {
            lock (this)
            {
                object[] record = this._db.getCurrentRecord(includeIndex);
                return this.createRecord(record, fieldList, includeIndex);
            }
        }

        public T GetCurrentRecord<T>(string[] fieldList, bool includeIndex) where T: class, new()
        {
            lock (this)
            {
                object[] record = this._db.getCurrentRecord(includeIndex);
                return this.createT<T>(record, fieldList, false);
            }
        }

        public Record GetRecordByIndex(int index, string[] fieldList)
        {
            lock (this)
            {
                object[] record = this._db.getRecordByIndex(index, fieldList, false);
                return this.createRecord(record, fieldList, false);
            }
        }

        public T GetRecordByIndex<T>(int index, string[] fieldList) where T: class, new()
        {
            lock (this)
            {
                object[] record = this._db.getRecordByIndex(index, fieldList, false);
                return this.createT<T>(record, fieldList, false);
            }
        }

        public Record GetRecordByKey(object key, string[] fieldList, bool includeIndex)
        {
            lock (this)
            {
                object[] record = this._db.getRecordByKey(key, fieldList, includeIndex);
                return this.createRecord(record, fieldList, includeIndex);
            }
        }

        public T GetRecordByKey<T>(object key, string[] fieldList, bool includeIndex) where T: class, new()
        {
            lock (this)
            {
                object[] record = this._db.getRecordByKey(key, fieldList, includeIndex);
                return this.createT<T>(record, fieldList, includeIndex);
            }
        }

        public bool MoveFirst()
        {
            lock (this)
            {
                return this._db.moveFirst();
            }
        }

        public bool MoveNext()
        {
            lock (this)
            {
                return this._db.moveNext();
            }
        }

        public void Open(string dbName, bool readOnly)
        {
            lock (this)
            {
                this._db.open(dbName, null, null, readOnly);
            }
        }

        public void Open(string dbName, string encryptionKey, bool readOnly)
        {
            lock (this)
            {
                this._db.open(dbName, encryptionKey, null, readOnly);
            }
        }

        public void Reindex()
        {
            lock (this)
            {
                this._db.reindex();
            }
        }

        public Table SelectAllRecords()
        {
            return this.SelectAllRecords(null, null, false);
        }

        public IList<T> SelectAllRecords<T>() where T: class, new()
        {
            return this.SelectAllRecords<T>(null, null, false);
        }

        public Table SelectAllRecords(string[] fieldList)
        {
            return this.SelectAllRecords(fieldList, null, false);
        }

        public Table SelectAllRecords(bool includeIndex)
        {
            return this.SelectAllRecords(null, null, includeIndex);
        }

        public IList<T> SelectAllRecords<T>(string[] fieldList) where T: class, new()
        {
            return this.SelectAllRecords<T>(fieldList, null, false);
        }

        public IList<T> SelectAllRecords<T>(bool includeIndex) where T: class, new()
        {
            return this.SelectAllRecords<T>(null, null, includeIndex);
        }

        public Table SelectAllRecords(string[] fieldList, string[] orderByList)
        {
            return this.SelectAllRecords(fieldList, orderByList, false);
        }

        public IList<T> SelectAllRecords<T>(string[] fieldList, string[] orderByList) where T: class, new()
        {
            return this.SelectAllRecords<T>(fieldList, orderByList, false);
        }

        public Table SelectAllRecords(string[] fieldList, string[] orderByList, bool includeIndex)
        {
            lock (this)
            {
                object[][] records = this._db.getAllRecords(fieldList, includeIndex, orderByList);
                return this.createTable(records, fieldList, includeIndex, orderByList);
            }
        }

        public IList<T> SelectAllRecords<T>(string[] fieldList, string[] orderByList, bool includeIndex) where T: class, new()
        {
            lock (this)
            {
                object[][] records = this._db.getAllRecords(fieldList, includeIndex, orderByList);
                return this.createTList<T>(records, fieldList, includeIndex, orderByList);
            }
        }

        public Table SelectRecords(FilterExpression filter)
        {
            return this.SelectRecords(filter, null, null, false);
        }

        public IList<T> SelectRecords<T>(FilterExpression filter) where T: class, new()
        {
            return this.SelectRecords<T>(filter, null, null, false);
        }

        public Table SelectRecords(FilterExpressionGroup filter)
        {
            return this.SelectRecords(filter, null, null, false);
        }

        public IList<T> SelectRecords<T>(FilterExpressionGroup filter) where T: class, new()
        {
            return this.SelectRecords<T>(filter, null, null, false);
        }

        public Table SelectRecords(string filter)
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords(group);
        }

        public IList<T> SelectRecords<T>(string filter) where T: class, new()
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords<T>(group);
        }

        public Table SelectRecords(FilterExpression filter, string[] fieldList)
        {
            return this.SelectRecords(filter, fieldList, null, false);
        }

        public IList<T> SelectRecords<T>(FilterExpression filter, string[] fieldList) where T: class, new()
        {
            return this.SelectRecords<T>(filter, fieldList, null, false);
        }

        public Table SelectRecords(FilterExpressionGroup filter, string[] fieldList)
        {
            return this.SelectRecords(filter, fieldList, null, false);
        }

        public IList<T> SelectRecords<T>(FilterExpressionGroup filter, string[] fieldList) where T: class, new()
        {
            return this.SelectRecords<T>(filter, fieldList, null, false);
        }

        public Table SelectRecords(string filter, string[] fieldList)
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords(group, fieldList);
        }

        public IList<T> SelectRecords<T>(string filter, string[] fieldList) where T: class, new()
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords<T>(group, fieldList);
        }

        public Table SelectRecords(FilterExpression filter, string[] fieldList, string[] orderByList)
        {
            return this.SelectRecords(filter, fieldList, orderByList, false);
        }

        public IList<T> SelectRecords<T>(FilterExpression filter, string[] fieldList, string[] orderByList) where T: class, new()
        {
            return this.SelectRecords<T>(filter, fieldList, orderByList, false);
        }

        public Table SelectRecords(FilterExpressionGroup filter, string[] fieldList, string[] orderByList)
        {
            return this.SelectRecords(filter, fieldList, orderByList, false);
        }

        public IList<T> SelectRecords<T>(FilterExpressionGroup filter, string[] fieldList, string[] orderByList) where T: class, new()
        {
            return this.SelectRecords<T>(filter, fieldList, orderByList, false);
        }

        public Table SelectRecords(string filter, string[] fieldList, string[] orderByList)
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords(group, fieldList, orderByList);
        }

        public IList<T> SelectRecords<T>(string filter, string[] fieldList, string[] orderByList) where T: class, new()
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords<T>(group, fieldList, orderByList);
        }

        public Table SelectRecords(FilterExpression filter, string[] fieldList, string[] orderByList, bool includeIndex)
        {
            lock (this)
            {
                object[][] records = this._db.getRecordByField(filter, fieldList, includeIndex, orderByList);
                return this.createTable(records, fieldList, includeIndex, orderByList);
            }
        }

        public IList<T> SelectRecords<T>(FilterExpression filter, string[] fieldList, string[] orderByList, bool includeIndex) where T: class, new()
        {
            lock (this)
            {
                object[][] records = this._db.getRecordByField(filter, fieldList, includeIndex, orderByList);
                return this.createTList<T>(records, fieldList, includeIndex, orderByList);
            }
        }

        public Table SelectRecords(FilterExpressionGroup filter, string[] fieldList, string[] orderByList, bool includeIndex)
        {
            lock (this)
            {
                object[][] records = this._db.getRecordByFields(filter, fieldList, includeIndex, orderByList);
                return this.createTable(records, fieldList, includeIndex, orderByList);
            }
        }

        public IList<T> SelectRecords<T>(FilterExpressionGroup filter, string[] fieldList, string[] orderByList, bool includeIndex) where T: class, new()
        {
            lock (this)
            {
                object[][] records = this._db.getRecordByFields(filter, fieldList, includeIndex, orderByList);
                return this.createTList<T>(records, fieldList, includeIndex, orderByList);
            }
        }

        public Table SelectRecords(string filter, string[] fieldList, string[] orderByList, bool includeIndex)
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords(group, fieldList, orderByList, includeIndex);
        }

        public IList<T> SelectRecords<T>(string filter, string[] fieldList, string[] orderByList, bool includeIndex) where T: class, new()
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords<T>(group, fieldList, orderByList, includeIndex);
        }

        public void SetEncryptionKey(string encryptionKey)
        {
            lock (this)
            {
                this._db.setEncryptionKey(encryptionKey);
            }
        }

        public void UpdateRecordByIndex(int index, FieldValues values)
        {
            lock (this)
            {
                this._db.updateRecordByIndex(values, index);
            }
        }

        public void UpdateRecordByKey(object key, FieldValues values)
        {
            lock (this)
            {
                this._db.updateRecordByKey(values, key);
            }
        }

        public int UpdateRecords(FilterExpression filter, FieldValues values)
        {
            lock (this)
            {
                return this._db.updateRecords(filter, values);
            }
        }

        public int UpdateRecords(FilterExpressionGroup filter, FieldValues values)
        {
            lock (this)
            {
                return this._db.updateRecords(filter, values);
            }
        }

        public int UpdateRecords(string filter, FieldValues values)
        {
            lock (this)
            {
                FilterExpressionGroup searchExpGrp = FilterExpressionGroup.Parse(filter);
                return this._db.updateRecords(searchExpGrp, values);
            }
        }

        public string UserVersion
        {
            get
            {
                return this._db.UserVersion;
            }
            set
            {
                this._db.UserVersion = value;
            }
        }

        public FwNs.Data.FyleDbNs.Fields Fields
        {
            get
            {
                return this._db.Fields;
            }
        }

        public int NumRecords
        {
            get
            {
                return this._db.NumRecords;
            }
        }

        public int NumDeleted
        {
            get
            {
                return this._db.NumDeleted;
            }
        }

        public int AutoCleanThreshold
        {
            get
            {
                return this._db.getAutoCleanThreshold();
            }
            set
            {
                this._db.setAutoCleanThreshold(value);
            }
        }

        public bool AutoFlush
        {
            get
            {
                return this._db.AutoFlush;
            }
            set
            {
                this._db.AutoFlush = value;
            }
        }

        public bool IsOpen
        {
            get
            {
                return this._db.IsOpen;
            }
        }

        public object MetaData
        {
            get
            {
                return this._db.MetaData;
            }
            set
            {
                if (value != null)
                {
                    Type type = value.GetType();
                    if ((type != typeof(string)) && (type != typeof(byte[])))
                    {
                        throw new FileDbException("Invalid meta data type - must be String or Byte[]", FileDbExceptionsEnum.InvalidMetaDataType);
                    }
                }
                this._db.MetaData = value;
            }
        }
    }
}

