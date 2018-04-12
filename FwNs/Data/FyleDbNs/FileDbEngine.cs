namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    internal class FileDbEngine
    {
        private const string StrIndex = "index";
        private const int NoLock = 0;
        private const int ReadLock = 1;
        private const int WriteLock = 2;
        private const int VerNullValueSupport = 0xca;
        private const int DateTimeByteLen = 10;
        private const int GuidByteLen = 0x10;
        internal const int AutoIncField = 1;
        internal const int ArrayField = 2;
        private const byte VERSION_MAJOR = 3;
        private const byte VERSION_MINOR = 0;
        private const int SIGNATURE = 0x123babe;
        private const int SCHEMA_OFFSET = 6;
        private const int NUM_RECS_OFFSET = 6;
        private const int INDEX_DELETED_OFFSET = 10;
        private const int INDEX_OFFSET = 14;
        private const int INDEX_RBLOCK_SIZE = 4;
        private bool _isOpen = false;
        private bool _disposed;
        private bool _openReadOnly = false;
        private bool _autoFlush = false;
        private string _dbName;
        private FileStream _dataStrm;
        private BinaryReader _dataReader;
        private BinaryWriter _dataWriter;
        private MemoryStream _testStrm;
        private BinaryWriter _testWriter;
        private Encryptor _encryptor;
        private int _numRecords;
        private int _numDeleted;
        private int _autoCleanThreshold = -1;
        private int _dataStartPos;
        private int _indexStartPos;
        private int _iteratorIndex;
        private int _ver_major;
        private int _ver_minor;
        private int _ver;
        private FwNs.Data.FyleDbNs.Fields _fields;
        private string _primaryKey;
        private string _userVersion;
        private Field _primaryKeyField;
        private List<int> _index;
        private List<int> _deletedRecords;
        private object _metaData;
        private static byte[] s_bitmask = new byte[] { 1, 2, 4, 8, 0x10, 0x20, 0x40, 0x80 };

        internal FileDbEngine()
        {
        }

        internal int addRecord(FieldValues record)
        {
            int index = -1;
            this.checkIsDbOpen();
            this.checkReadOnly();
            this.verifyRecordSchema(record);
            try
            {
                byte[] buffer;
                foreach (Field field in this._fields)
                {
                    if (field.IsAutoInc)
                    {
                        if (this._numRecords == 0)
                        {
                            field.CurAutoIncVal = field.AutoIncStart;
                        }
                        record[field.Name] = field.CurAutoIncVal;
                    }
                }
                if (!string.IsNullOrEmpty(this._primaryKey) && (this._numRecords > 0))
                {
                    object target = null;
                    if (record.ContainsKey(this._primaryKey))
                    {
                        target = record[this._primaryKey];
                    }
                    if (target == null)
                    {
                        throw new FileDbException(string.Format("Primary key field {0} cannot be null or missing", this._primaryKey), FileDbExceptionsEnum.MissingPrimaryKey);
                    }
                    int num1 = this.bsearch(this._index, 0, this._index.Count - 1, target);
                    if (num1 > 0)
                    {
                        throw new FileDbException(string.Format("Duplicate key violation - Field: '{0}' - Value: '{1}'", this._primaryKey, target.ToString()), FileDbExceptionsEnum.DuplicatePrimaryKey);
                    }
                    index = -num1 - 1;
                }
                int item = this._indexStartPos;
                int size = this.getRecordSize(record, out buffer);
                int num4 = -1;
                if (this._numDeleted > 0)
                {
                    for (int i = 0; i < this._deletedRecords.Count; i++)
                    {
                        int num7 = this._deletedRecords[i];
                        this._dataStrm.Seek((long) num7, SeekOrigin.Begin);
                        if (-this._dataReader.ReadInt32() >= size)
                        {
                            item = num7;
                            num4 = i;
                            break;
                        }
                    }
                }
                this._dataStrm.Seek((long) item, SeekOrigin.Begin);
                this.writeRecord(this._dataWriter, record, size, buffer, false);
                if (index < 0)
                {
                    this._index.Add(item);
                    index = this._index.Count - 1;
                }
                else
                {
                    this._index.Insert(index, item);
                }
                if (num4 > -1)
                {
                    this._deletedRecords.RemoveAt(num4);
                    this._numDeleted--;
                }
                foreach (Field field2 in this._fields)
                {
                    if (field2.IsAutoInc)
                    {
                        field2.CurAutoIncVal++;
                    }
                }
                int position = (int) this._dataStrm.Position;
                if (position > this._indexStartPos)
                {
                    this._indexStartPos = position;
                }
                this._numRecords++;
                this.writeSchema(this._dataWriter);
            }
            finally
            {
                if (this.AutoFlush)
                {
                    this.flush(true);
                }
            }
            return index;
        }

        private int bsearch(List<int> lstIndex, int left, int right, object target)
        {
            int num = 0;
            while (left <= right)
            {
                num = (left + right) / 2;
                object obj2 = this.readRecordKey(this._dataReader, lstIndex[num]);
                int num2 = this.compareKeys(target, obj2);
                if ((left == right) && (num2 != 0))
                {
                    if (num2 < 0)
                    {
                        return -(left + 1);
                    }
                    return -((left + 1) + 1);
                }
                if (num2 == 0)
                {
                    return (num + 1);
                }
                if (num2 < 0)
                {
                    right = num - 1;
                }
                else
                {
                    left = num + 1;
                }
            }
            return -(num + 1);
        }

        private void checkAutoClean()
        {
            if ((this._isOpen && (this._autoCleanThreshold >= 0)) && (this._numDeleted > this._autoCleanThreshold))
            {
                this.cleanup(false);
            }
        }

        private void checkIsDbOpen()
        {
            if (!this._isOpen)
            {
                throw new FileDbException("No open database", FileDbExceptionsEnum.NoOpenDatabase);
            }
        }

        private void checkReadOnly()
        {
            if (this._openReadOnly)
            {
                throw new FileDbException("Database is open in read-only mode", FileDbExceptionsEnum.NoOpenDatabase);
            }
        }

        internal void cleanup(bool schemaChange)
        {
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (schemaChange || (this._numDeleted != 0))
            {
                string str = Path.GetFileNameWithoutExtension(this._dbName) + ".tmp.fdb";
                str = Path.Combine(Path.GetDirectoryName(this._dbName), str);
                FileStream output = File.Open(str, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                output.SetLength(0L);
                int num = this._numDeleted;
                int num2 = this._indexStartPos;
                try
                {
                    BinaryWriter writer = new BinaryWriter(output);
                    List<int> list = new List<int>(this._numRecords);
                    this._numDeleted = 0;
                    this.writeDbHeader(writer);
                    this.writeSchema(writer);
                    new FieldValues();
                    for (int i = 0; i < this._index.Count; i++)
                    {
                        int offset = this._index[i];
                        list.Add((int) output.Position);
                        if (schemaChange)
                        {
                            int num5;
                            bool flag;
                            object[] objArray = this.readRecord(this._dataReader, offset, false, out num5, out flag);
                            FieldValues record = new FieldValues(this._fields.Count);
                            foreach (Field field in this._fields)
                            {
                                record.Add(field.Name, objArray[field.Ordinal]);
                            }
                            this.writeRecord(writer, record, -1, null, false);
                        }
                        else
                        {
                            bool flag2;
                            byte[] record = this.readRecordRaw(this._dataReader, offset, out flag2);
                            this.writeRecordRaw(writer, record, false);
                        }
                    }
                    this._indexStartPos = (int) output.Position;
                    this.writeIndexStart(writer);
                    this._deletedRecords = new List<int>();
                    this._index = list;
                    output.Flush();
                    this.writeIndex(output, writer, this._index);
                    writer.Flush();
                    output.Flush();
                }
                catch
                {
                    this._indexStartPos = num2;
                    this._numDeleted = num;
                    output.Close();
                    File.Delete(str);
                    throw;
                }
                string path = this._dbName;
                Encryptor encryptor = this._encryptor;
                this.close();
                output.Close();
                output = null;
                File.Delete(path);
                File.Move(str, path);
                this.open(path, null, encryptor, this._openReadOnly);
            }
        }

        internal void close()
        {
            if (this._isOpen)
            {
                try
                {
                    this.flush(!this.AutoFlush);
                    this._dataStrm.Close();
                    this._dataStrm.Dispose();
                }
                finally
                {
                    this._autoCleanThreshold = -1;
                    this._dataStartPos = 0;
                    this._dataStrm = null;
                    this._dataWriter = null;
                    this._dataReader = null;
                    this._dataReader = null;
                    this._isOpen = false;
                    this._dbName = null;
                    this._fields = null;
                    this._primaryKey = null;
                    this._primaryKeyField = null;
                    this._encryptor = null;
                    this._metaData = null;
                }
            }
        }

        private int compareKeys(object left, object right)
        {
            Type type = left.GetType();
            Type type2 = right.GetType();
            if (type != type2)
            {
                throw new FileDbException("Mismatched key field types", FileDbExceptionsEnum.MismatchedKeyFieldTypes);
            }
            if (type == typeof(string))
            {
                return string.Compare(left as string, right as string);
            }
            if (type != typeof(int))
            {
                throw new FileDbException("Invalid key field type (record number) - must be type Int32", FileDbExceptionsEnum.InvalidKeyFieldType);
            }
            int num = (int) left;
            int num2 = (int) right;
            if (num < num2)
            {
                return -1;
            }
            if (num <= num2)
            {
                return 0;
            }
            return 1;
        }

        private static EqualityEnum compareVals(Field field, object val1, object val2)
        {
            EqualityEnum notEqual = EqualityEnum.NotEqual;
            DataTypeEnum dataType = field.DataType;
            switch (dataType)
            {
                case DataTypeEnum.Bool:
                    return ((Convert.ToBoolean(val1) == Convert.ToBoolean(val2)) ? EqualityEnum.Equal : EqualityEnum.NotEqual);

                case ((DataTypeEnum) 4):
                case ((DataTypeEnum) 5):
                case ((DataTypeEnum) 7):
                case ((DataTypeEnum) 8):
                case ((DataTypeEnum) 12):
                case ((DataTypeEnum) 0x11):
                    return notEqual;

                case DataTypeEnum.Byte:
                {
                    byte num = Convert.ToByte(val1);
                    byte num2 = Convert.ToByte(val2);
                    return ((num == num2) ? EqualityEnum.Equal : ((num > num2) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
                case DataTypeEnum.Int:
                {
                    int num3 = Convert.ToInt32(val1);
                    int num4 = Convert.ToInt32(val2);
                    return ((num3 == num4) ? EqualityEnum.Equal : ((num3 > num4) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
                case DataTypeEnum.UInt:
                {
                    uint num5 = Convert.ToUInt32(val1);
                    uint num6 = Convert.ToUInt32(val2);
                    return ((num5 == num6) ? EqualityEnum.Equal : ((num5 > num6) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
                case DataTypeEnum.Int64:
                {
                    long num7 = Convert.ToInt64(val1);
                    long num8 = Convert.ToInt64(val2);
                    return ((num7 == num8) ? EqualityEnum.Equal : ((num7 > num8) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
                case DataTypeEnum.Float:
                {
                    float num9 = Convert.ToSingle(val1);
                    float num10 = Convert.ToSingle(val2);
                    return ((num9 == num10) ? EqualityEnum.Equal : ((num9 > num10) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
                case DataTypeEnum.Double:
                {
                    double num11 = Convert.ToDouble(val1);
                    double num12 = Convert.ToDouble(val2);
                    return ((num11 == num12) ? EqualityEnum.Equal : ((num11 > num12) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
                case DataTypeEnum.Decimal:
                {
                    decimal num13 = Convert.ToDecimal(val1);
                    decimal num14 = Convert.ToDecimal(val2);
                    return ((num13 == num14) ? EqualityEnum.Equal : ((num13 > num14) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
                case DataTypeEnum.DateTime:
                {
                    DateTime time = Convert.ToDateTime(val1);
                    DateTime time2 = Convert.ToDateTime(val2);
                    return ((time == time2) ? EqualityEnum.Equal : ((time > time2) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
                case DataTypeEnum.String:
                {
                    int num15 = string.Compare(val1.ToString(), val2.ToString(), StringComparison.CurrentCulture);
                    return ((num15 == 0) ? EqualityEnum.Equal : ((num15 > 0) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan));
                }
            }
            if (dataType == DataTypeEnum.Guid)
            {
                Guid guid = convertToGuid(val1);
                Guid guid2 = convertToGuid(val2);
                notEqual = (guid.CompareTo(guid2) == 0) ? EqualityEnum.Equal : EqualityEnum.NotEqual;
            }
            return notEqual;
        }

        internal static int CompareVals(object v1, object v2, DataTypeEnum dataType, bool caseInsensitive)
        {
            DateTime time;
            DateTime time2;
            int num = 0;
            switch (dataType)
            {
                case DataTypeEnum.Bool:
                {
                    byte num2 = (byte) v1;
                    byte num3 = (byte) v2;
                    return ((num2 < num3) ? -1 : ((num2 > num3) ? 1 : 0));
                }
                case ((DataTypeEnum) 4):
                case ((DataTypeEnum) 5):
                case ((DataTypeEnum) 7):
                case ((DataTypeEnum) 8):
                case ((DataTypeEnum) 12):
                case ((DataTypeEnum) 0x11):
                    return num;

                case DataTypeEnum.Byte:
                {
                    byte num4 = (byte) v1;
                    byte num5 = (byte) v2;
                    return ((num4 < num5) ? -1 : ((num4 > num5) ? 1 : 0));
                }
                case DataTypeEnum.Int:
                {
                    int num6 = (int) v1;
                    int num7 = (int) v2;
                    return ((num6 < num7) ? -1 : ((num6 > num7) ? 1 : 0));
                }
                case DataTypeEnum.UInt:
                {
                    uint num8 = (uint) v1;
                    uint num9 = (uint) v2;
                    return ((num8 < num9) ? -1 : ((num8 > num9) ? 1 : 0));
                }
                case DataTypeEnum.Int64:
                {
                    long num10 = (long) v1;
                    long num11 = (long) v2;
                    return ((num10 < num11) ? -1 : ((num10 > num11) ? 1 : 0));
                }
                case DataTypeEnum.Float:
                {
                    float num12 = (float) v1;
                    float num13 = (float) v2;
                    return ((num12 < num13) ? -1 : ((num12 > num13) ? 1 : 0));
                }
                case DataTypeEnum.Double:
                {
                    double num14 = (double) v1;
                    double num15 = (double) v2;
                    return ((num14 < num15) ? -1 : ((num14 > num15) ? 1 : 0));
                }
                case DataTypeEnum.Decimal:
                {
                    decimal num16 = (decimal) v1;
                    decimal num17 = (decimal) v2;
                    return ((num16 < num17) ? -1 : ((num16 > num17) ? 1 : 0));
                }
                case DataTypeEnum.DateTime:
                    if (v1.GetType() != typeof(string))
                    {
                        if (v1.GetType() != typeof(DateTime))
                        {
                            throw new FileDbException("Invalid DateTime type", FileDbExceptionsEnum.InvalidDataType);
                        }
                        time = (DateTime) v1;
                        time2 = (DateTime) v2;
                        break;
                    }
                    time = DateTime.Parse(v1.ToString());
                    time2 = DateTime.Parse(v2.ToString());
                    break;

                case DataTypeEnum.String:
                {
                    string strA = (string) v1;
                    string strB = (string) v2;
                    if (!caseInsensitive)
                    {
                        return string.Compare(strA, strB, StringComparison.CurrentCulture);
                    }
                    return string.Compare(strA, strB, StringComparison.CurrentCultureIgnoreCase);
                }
                default:
                    if (dataType == DataTypeEnum.Guid)
                    {
                        Guid guid = (Guid) v1;
                        Guid guid2 = (Guid) v2;
                        num = guid.CompareTo(guid2);
                    }
                    return num;
            }
            return ((time < time2) ? -1 : ((time > time2) ? 1 : 0));
        }

        private static Guid convertToGuid(object val)
        {
            Type type = val.GetType();
            if (type == typeof(Guid))
            {
                return (Guid) val;
            }
            if (type == typeof(byte[]))
            {
                return new Guid((byte[]) val);
            }
            if (type != typeof(string))
            {
                throw new FileDbException(string.Format("Cannot convert type {0} to Guid", type.ToString()), FileDbExceptionsEnum.CantConvertTypeToGuid);
            }
            return new Guid((string) val);
        }

        private static object convertValueToType(object value, DataTypeEnum dataType)
        {
            object obj2 = null;
            switch (dataType)
            {
                case DataTypeEnum.Bool:
                    return Convert.ToBoolean(value);

                case ((DataTypeEnum) 4):
                case ((DataTypeEnum) 5):
                case ((DataTypeEnum) 7):
                case ((DataTypeEnum) 8):
                case ((DataTypeEnum) 12):
                case ((DataTypeEnum) 0x11):
                    return obj2;

                case DataTypeEnum.Byte:
                    return Convert.ToByte(value);

                case DataTypeEnum.Int:
                    return Convert.ToInt32(value);

                case DataTypeEnum.UInt:
                    return Convert.ToUInt32(value);

                case DataTypeEnum.Int64:
                    return Convert.ToInt64(value);

                case DataTypeEnum.Float:
                    return Convert.ToSingle(value);

                case DataTypeEnum.Double:
                    return Convert.ToDouble(value);

                case DataTypeEnum.Decimal:
                    return Convert.ToDecimal(value);

                case DataTypeEnum.DateTime:
                    return Convert.ToDateTime(value);

                case DataTypeEnum.String:
                    return value.ToString();
            }
            if (dataType == DataTypeEnum.Guid)
            {
                obj2 = convertToGuid(value);
            }
            return obj2;
        }

        internal void create(string dbName, Field[] schema)
        {
            if (this._isOpen)
            {
                this.close();
            }
            this._fields = new FwNs.Data.FyleDbNs.Fields();
            this._primaryKey = string.Empty;
            int index = 0;
            while (index < schema.Length)
            {
                Field field = schema[index];
                switch (field.DataType)
                {
                    case DataTypeEnum.Bool:
                    case DataTypeEnum.Byte:
                    case DataTypeEnum.Int:
                    case DataTypeEnum.UInt:
                    case DataTypeEnum.Int64:
                    case DataTypeEnum.Float:
                    case DataTypeEnum.Double:
                    case DataTypeEnum.Decimal:
                    case DataTypeEnum.DateTime:
                    case DataTypeEnum.String:
                    case DataTypeEnum.Guid:
                    {
                        if (field.IsPrimaryKey && string.IsNullOrEmpty(this._primaryKey))
                        {
                            if ((field.DataType != DataTypeEnum.Int) && (field.DataType != DataTypeEnum.String))
                            {
                                throw new FileDbException(string.Format("Primary key field '{0}' must be type Int or String and must not be Array type", field.Name), FileDbExceptionsEnum.InvalidPrimaryKeyType);
                            }
                            if (field.IsArray)
                            {
                                throw new FileDbException(string.Format("Primary key field '{0}' must be type Int or String and must not be Array type", field.Name), FileDbExceptionsEnum.InvalidPrimaryKeyType);
                            }
                            this._primaryKey = field.Name.ToUpper();
                            this._primaryKeyField = field;
                        }
                        this._fields.Add(field);
                        index++;
                        continue;
                    }
                }
                throw new FileDbException(string.Format("Invalid type in schema: {0}", (int) field.DataType), FileDbExceptionsEnum.InvalidTypeInSchema);
            }
            this.openFiles(dbName, FileMode.Create);
            this._isOpen = true;
            this._dbName = dbName;
            this._iteratorIndex = 0;
            this._ver_major = 3;
            this._ver_minor = 0;
            this._ver = (this._ver_major * 100) + this._ver_minor;
            this._numRecords = 0;
            this._numDeleted = 0;
            this.writeDbHeader(this._dataWriter);
            this.writeSchema(this._dataWriter);
            this._indexStartPos = this._dataStartPos;
            this.writeIndexStart(this._dataWriter);
            this.readSchema();
            this._index = new List<int>(100);
            this._deletedRecords = new List<int>(3);
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
                    this.close();
                }
                this._disposed = true;
            }
        }

        internal void drop(string dbName)
        {
            if ((dbName == this._dbName) && this._isOpen)
            {
                this.close();
            }
            File.Delete(dbName);
        }

        internal static bool evaluate(FilterExpressionGroup searchExpGrp, object[] record, FwNs.Data.FyleDbNs.Fields fields)
        {
            if (searchExpGrp.Expressions.Count == 0)
            {
                return true;
            }
            bool flag = false;
            int num = 0;
            foreach (object obj2 in searchExpGrp.Expressions)
            {
                if (obj2 != null)
                {
                    bool flag2;
                    BoolOpEnum boolOp;
                    if (obj2.GetType() == typeof(FilterExpressionGroup))
                    {
                        FilterExpressionGroup group1 = obj2 as FilterExpressionGroup;
                        flag2 = evaluate(group1, record, fields);
                        boolOp = group1.BoolOp;
                    }
                    else
                    {
                        FilterExpression searchExp = obj2 as FilterExpression;
                        boolOp = searchExp.BoolOp;
                        string fieldName = searchExp.FieldName;
                        if (fieldName[0] == '~')
                        {
                            fieldName = fieldName.Substring(1);
                            searchExp.MatchType = MatchTypeEnum.IgnoreCase;
                        }
                        if (!fields.ContainsKey(fieldName))
                        {
                            throw new FileDbException(string.Format("Invalid field name: {0}", searchExp.FieldName), FileDbExceptionsEnum.InvalidFieldName);
                        }
                        flag2 = evaluate(fields[fieldName], searchExp, record, null);
                    }
                    if (num == 0)
                    {
                        flag = flag2;
                    }
                    else if (boolOp == BoolOpEnum.And)
                    {
                        flag &= flag2;
                        if (!flag)
                        {
                            return flag;
                        }
                    }
                    else
                    {
                        flag |= flag2;
                        if (flag)
                        {
                            return flag;
                        }
                    }
                    num++;
                }
            }
            return flag;
        }

        internal static bool evaluate(Field field, FilterExpression searchExp, object[] record, Regex regex)
        {
            if (field.IsArray)
            {
                return false;
            }
            EqualityEnum notEqual = EqualityEnum.NotEqual;
            object item = record[field.Ordinal];
            if ((item != null) || (searchExp.SearchVal != null))
            {
                if ((item != null) && (searchExp.SearchVal != null))
                {
                    if ((searchExp.Equality == EqualityEnum.Like) || (searchExp.Equality == EqualityEnum.NotLike))
                    {
                        if (regex == null)
                        {
                            regex = new Regex(searchExp.SearchVal.ToString(), RegexOptions.IgnoreCase);
                        }
                        notEqual = regex.IsMatch(item.ToString()) ? EqualityEnum.Like : EqualityEnum.NotLike;
                    }
                    else if ((searchExp.Equality == EqualityEnum.In) || (searchExp.Equality == EqualityEnum.NotIn))
                    {
                        HashSet<object> searchVal = searchExp.SearchVal as HashSet<object>;
                        if (searchVal == null)
                        {
                            throw new FileDbException("HashSet<object> expected as the SearchVal when using Equality.In", FileDbExceptionsEnum.HashSetExpected);
                        }
                        if (field.DataType != DataTypeEnum.String)
                        {
                            HashSet<object> set2 = new HashSet<object>();
                            foreach (object obj3 in searchVal)
                            {
                                set2.Add(convertValueToType(obj3, field.DataType));
                            }
                            searchVal = set2;
                            searchExp.SearchVal = set2;
                        }
                        else if (searchExp.MatchType == MatchTypeEnum.IgnoreCase)
                        {
                            item = item.ToString().ToUpper();
                        }
                        notEqual = searchVal.Contains(item) ? EqualityEnum.In : EqualityEnum.NotIn;
                    }
                    else if (field.DataType == DataTypeEnum.String)
                    {
                        int num = string.Compare(searchExp.SearchVal.ToString(), item.ToString(), (searchExp.MatchType == MatchTypeEnum.UseCase) ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
                        notEqual = (num == 0) ? EqualityEnum.Equal : ((num > 0) ? EqualityEnum.GreaterThan : EqualityEnum.LessThan);
                    }
                    else
                    {
                        notEqual = compareVals(field, item, searchExp.SearchVal);
                    }
                }
                bool flag = false;
                if (searchExp.Equality == EqualityEnum.NotEqual)
                {
                    if (notEqual != EqualityEnum.Equal)
                    {
                        flag = true;
                    }
                    return flag;
                }
                if (notEqual != searchExp.Equality)
                {
                    if (notEqual == EqualityEnum.Equal)
                    {
                        if (((searchExp.Equality != EqualityEnum.Equal) && (searchExp.Equality != EqualityEnum.LessThanOrEqual)) && (searchExp.Equality != EqualityEnum.GreaterThanOrEqual))
                        {
                            return flag;
                        }
                        return true;
                    }
                    if (notEqual == EqualityEnum.NotEqual)
                    {
                        if (((searchExp.Equality != EqualityEnum.NotEqual) && (searchExp.Equality != EqualityEnum.LessThan)) && (searchExp.Equality != EqualityEnum.GreaterThan))
                        {
                            return flag;
                        }
                        return true;
                    }
                    if ((notEqual == EqualityEnum.LessThan) && ((searchExp.Equality == EqualityEnum.LessThan) || (searchExp.Equality == EqualityEnum.LessThanOrEqual)))
                    {
                        return true;
                    }
                    if ((notEqual != EqualityEnum.GreaterThan) || ((searchExp.Equality != EqualityEnum.GreaterThan) && (searchExp.Equality != EqualityEnum.GreaterThanOrEqual)))
                    {
                        return flag;
                    }
                }
            }
            return true;
        }

        ~FileDbEngine()
        {
            this.Dispose(false);
        }

        internal void flush(bool saveIndex)
        {
            if (!this._openReadOnly)
            {
                if (saveIndex)
                {
                    this.writeIndex(this._dataStrm, this._dataWriter, this._index);
                }
                this._dataWriter.Flush();
                this._dataStrm.Flush();
            }
        }

        internal object[][] getAllRecords(string[] fieldList, bool includeIndex, string[] orderByList)
        {
            this.checkIsDbOpen();
            if (this._numRecords == 0)
            {
                return null;
            }
            object[][] result = null;
            result = new object[this._numRecords][];
            int num = 0;
            int num2 = 0;
            foreach (int num3 in this._index)
            {
                bool flag;
                object[] objArray2 = this.readRecord(num3, includeIndex, out flag);
                if (fieldList != null)
                {
                    object[] objArray3 = new object[fieldList.Length + (includeIndex ? 1 : 0)];
                    int num4 = 0;
                    for (int i = 0; i < fieldList.Length; i++)
                    {
                        string fieldName = fieldList[i];
                        if (!this._fields.ContainsKey(fieldName))
                        {
                            throw new FileDbException(string.Format("Invalid field name: {0}", fieldName), FileDbExceptionsEnum.InvalidFieldName);
                        }
                        Field field = this._fields[fieldName];
                        objArray3[num4++] = objArray2[field.Ordinal];
                    }
                    objArray2 = objArray3;
                }
                if (includeIndex)
                {
                    objArray2[objArray2.Length - 1] = num++;
                }
                result[num2++] = objArray2;
            }
            if ((result != null) && (orderByList != null))
            {
                this.orderBy(result, fieldList, orderByList);
            }
            return result;
        }

        internal int getAutoCleanThreshold()
        {
            return this._autoCleanThreshold;
        }

        internal object[] getCurrentRecord(bool includeIndex)
        {
            this.checkIsDbOpen();
            if (this._numRecords == 0)
            {
                return null;
            }
            if (this.isEof)
            {
                throw new FileDbException("The current position is past the last record - call MoveFirst to reset the current position", FileDbExceptionsEnum.IteratorPastEndOfFile);
            }
            int offset = this._index[this._iteratorIndex];
            object[] objArray = this.readRecord(offset, includeIndex);
            if (includeIndex)
            {
                objArray[objArray.Length - 1] = this._iteratorIndex;
            }
            return objArray;
        }

        internal int getDeletedRecordCount()
        {
            this.checkIsDbOpen();
            return this._numDeleted;
        }

        private int getItemSize(Field field, object data)
        {
            int num = 0;
            if (data != null)
            {
                DataTypeEnum dataType = field.DataType;
                switch (dataType)
                {
                    case DataTypeEnum.Bool:
                        if (field.IsArray)
                        {
                            num = 4;
                            if (data.GetType() == typeof(bool[]))
                            {
                                bool[] flagArray = (bool[]) data;
                                return (num + flagArray.Length);
                            }
                            if (data.GetType() == typeof(byte[]))
                            {
                                byte[] buffer = (byte[]) data;
                                return (num + buffer.Length);
                            }
                            return num;
                        }
                        return 1;

                    case ((DataTypeEnum) 4):
                    case ((DataTypeEnum) 5):
                    case ((DataTypeEnum) 7):
                    case ((DataTypeEnum) 8):
                    case ((DataTypeEnum) 12):
                    case ((DataTypeEnum) 0x11):
                        goto Label_033D;

                    case DataTypeEnum.Byte:
                        if (field.IsArray)
                        {
                            num = 4;
                            byte[] buffer2 = (byte[]) data;
                            if (buffer2 != null)
                            {
                                return (num + buffer2.Length);
                            }
                            return num;
                        }
                        return 1;

                    case DataTypeEnum.Int:
                        if (field.IsArray)
                        {
                            num = 4;
                            int[] numArray = (int[]) data;
                            if (numArray != null)
                            {
                                return (num + (numArray.Length * 4));
                            }
                            return num;
                        }
                        return 4;

                    case DataTypeEnum.UInt:
                        if (field.IsArray)
                        {
                            num = 4;
                            uint[] numArray2 = (uint[]) data;
                            if (numArray2 != null)
                            {
                                return (num + (numArray2.Length * 4));
                            }
                            return num;
                        }
                        return 4;

                    case DataTypeEnum.Int64:
                    {
                        if (!field.IsArray)
                        {
                            return 8;
                        }
                        num = 4;
                        long[] numArray3 = (long[]) data;
                        return (num + (numArray3.Length * 8));
                    }
                    case DataTypeEnum.Float:
                    {
                        if (!field.IsArray)
                        {
                            return 4;
                        }
                        num = 4;
                        float[] numArray4 = (float[]) data;
                        return (num + (numArray4.Length * 4));
                    }
                    case DataTypeEnum.Double:
                    {
                        if (!field.IsArray)
                        {
                            return 8;
                        }
                        num = 4;
                        double[] numArray5 = (double[]) data;
                        return (num + (numArray5.Length * 8));
                    }
                    case DataTypeEnum.Decimal:
                    {
                        if (!field.IsArray)
                        {
                            return 0x10;
                        }
                        num = 4;
                        decimal[] numArray6 = (decimal[]) data;
                        return (num + (numArray6.Length * 0x10));
                    }
                    case DataTypeEnum.DateTime:
                        if (field.IsArray)
                        {
                            num = 4;
                            if (data.GetType() == typeof(DateTime[]))
                            {
                                DateTime[] timeArray = (DateTime[]) data;
                                return (num + (timeArray.Length * 10));
                            }
                            string[] strArray = (string[]) data;
                            return (num + (strArray.Length * 10));
                        }
                        return (num + 10);

                    case DataTypeEnum.String:
                        this._testWriter = this.getTestWriter();
                        if (!field.IsArray)
                        {
                            this._testWriter.Write(data.ToString());
                            break;
                        }
                        num = 4;
                        foreach (string str in (string[]) data)
                        {
                            this._testWriter.Write((str == null) ? string.Empty : str);
                        }
                        break;

                    default:
                        if (dataType == DataTypeEnum.Guid)
                        {
                            this._testWriter = this.getTestWriter();
                            if (field.IsArray)
                            {
                                num = 4;
                                if (data.GetType() == typeof(Guid[]))
                                {
                                    Guid[] guidArray = (Guid[]) data;
                                    if (guidArray != null)
                                    {
                                        foreach (Guid guid in guidArray)
                                        {
                                            this._testWriter.Write(guid.ToByteArray());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this._testWriter.Write(((Guid) data).ToByteArray());
                            }
                            this._testWriter.Flush();
                            return (num + ((int) this._testStrm.Position));
                        }
                        goto Label_033D;
                }
                if (this._testStrm.Position > 0L)
                {
                    this._testWriter.Flush();
                    return (num + ((int) this._testStrm.Position));
                }
            }
            return num;
        Label_033D:
            throw new FileDbException(string.Format("Invalid data type encountered in data file ({0})", (int) field.DataType), FileDbExceptionsEnum.InvalidDataType);
        }

        internal static void GetOrderByLists(FwNs.Data.FyleDbNs.Fields fields, string[] fieldList, string[] orderByList, List<Field> sortFields, List<bool> sortDirLst, List<bool> caseLst)
        {
            for (int i = 0; i < orderByList.Length; i++)
            {
                string fieldName = orderByList[i];
                bool item = false;
                bool flag2 = false;
                int startIndex = 0;
                if (fieldName.Length > 0)
                {
                    item = fieldName[0] == '!';
                    flag2 = fieldName[0] == '~';
                    if (fieldName.Length > 1)
                    {
                        if (!item)
                        {
                            item = fieldName[1] == '!';
                        }
                        if (!flag2)
                        {
                            flag2 = fieldName[1] == '~';
                        }
                    }
                }
                if (item)
                {
                    startIndex++;
                }
                if (flag2)
                {
                    startIndex++;
                }
                if (startIndex > 0)
                {
                    fieldName = fieldName.Substring(startIndex);
                }
                sortDirLst.Add(item);
                caseLst.Add(flag2);
                string str2 = fieldName;
                if (!fields.ContainsKey(fieldName))
                {
                    throw new Exception(string.Format("Invalid OrderBy field name - {0}", str2));
                }
                Field field = fields[fieldName];
                if (field.IsArray)
                {
                    throw new Exception("Cannot OrderBy on an array field.");
                }
                if (fieldList != null)
                {
                    field = new Field(field.Name, field.DataType, -1);
                    for (int j = 0; j < fieldList.Length; j++)
                    {
                        if (string.Compare(fieldList[j], field.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            field.Ordinal = j;
                            break;
                        }
                    }
                    if (field.Ordinal == -1)
                    {
                        throw new Exception(string.Format("Invalid OrderBy field name - {0}", field));
                    }
                }
                sortFields.Add(field);
            }
        }

        internal object[][] getRecordByField(FilterExpression searchExp, string[] fieldList, bool includeIndex, string[] orderByList)
        {
            this.checkIsDbOpen();
            string fieldName = searchExp.FieldName;
            if (fieldName[0] == '~')
            {
                fieldName = fieldName.Substring(1);
                searchExp.MatchType = MatchTypeEnum.IgnoreCase;
            }
            if (!this._fields.ContainsKey(fieldName))
            {
                throw new FileDbException(string.Format("Invalid field name: {0}", searchExp.FieldName), FileDbExceptionsEnum.InvalidFieldName);
            }
            Field field = this._fields[fieldName];
            if (this._numRecords == 0)
            {
                return null;
            }
            List<object[]> list = new List<object[]>();
            int num = 0;
            Regex regex = null;
            foreach (int num2 in this._index)
            {
                bool flag;
                object[] record = this.readRecord(num2, includeIndex, out flag);
                if (((searchExp.Equality == EqualityEnum.Like) || (searchExp.Equality == EqualityEnum.NotLike)) && (regex == null))
                {
                    regex = new Regex(searchExp.SearchVal.ToString(), RegexOptions.IgnoreCase);
                }
                if (evaluate(field, searchExp, record, regex))
                {
                    if (fieldList != null)
                    {
                        object[] objArray3 = new object[fieldList.Length + (includeIndex ? 1 : 0)];
                        int num3 = 0;
                        for (int i = 0; i < fieldList.Length; i++)
                        {
                            string str2 = fieldList[i];
                            if (!this._fields.ContainsKey(str2))
                            {
                                throw new FileDbException(string.Format("Invalid field name: {0}", str2), FileDbExceptionsEnum.InvalidFieldName);
                            }
                            Field field2 = this._fields[str2];
                            objArray3[num3++] = record[field2.Ordinal];
                        }
                        record = objArray3;
                    }
                    if (includeIndex)
                    {
                        record[record.Length - 1] = num++;
                    }
                    list.Add(record);
                }
                num++;
            }
            object[][] result = list.ToArray();
            if ((result != null) && (orderByList != null))
            {
                this.orderBy(result, fieldList, orderByList);
            }
            return result;
        }

        internal object[][] getRecordByFields(FilterExpressionGroup searchExpGrp, string[] fieldList, bool includeIndex, string[] orderByList)
        {
            this.checkIsDbOpen();
            if (this._numRecords == 0)
            {
                return null;
            }
            List<object[]> list = new List<object[]>();
            int num = 0;
            foreach (int num2 in this._index)
            {
                bool flag;
                object[] record = this.readRecord(num2, includeIndex, out flag);
                if (evaluate(searchExpGrp, record, this._fields))
                {
                    if (fieldList != null)
                    {
                        object[] objArray3 = new object[fieldList.Length + (includeIndex ? 1 : 0)];
                        int num3 = 0;
                        for (int i = 0; i < fieldList.Length; i++)
                        {
                            string fieldName = fieldList[i];
                            if (!this._fields.ContainsKey(fieldName))
                            {
                                throw new FileDbException(string.Format("Invalid field name: {0}", fieldName), FileDbExceptionsEnum.InvalidFieldName);
                            }
                            Field field = this._fields[fieldName];
                            objArray3[num3++] = record[field.Ordinal];
                        }
                        record = objArray3;
                    }
                    if (includeIndex)
                    {
                        record[record.Length - 1] = num++;
                    }
                    list.Add(record);
                }
                num++;
            }
            object[][] result = list.ToArray();
            if ((result != null) && (orderByList != null))
            {
                this.orderBy(result, fieldList, orderByList);
            }
            return result;
        }

        internal object[] getRecordByIndex(int idx, string[] fieldList, bool includeIndex)
        {
            bool flag;
            this.checkIsDbOpen();
            if ((idx < 0) || (idx >= this._numRecords))
            {
                return null;
            }
            int offset = this._index[idx];
            object[] objArray = this.readRecord(offset, includeIndex, out flag);
            if (fieldList != null)
            {
                object[] objArray2 = new object[fieldList.Length + (includeIndex ? 1 : 0)];
                int num2 = 0;
                for (int i = 0; i < fieldList.Length; i++)
                {
                    string fieldName = fieldList[i];
                    if (!this._fields.ContainsKey(fieldName))
                    {
                        throw new FileDbException(string.Format("Invalid field name: {0}", fieldName), FileDbExceptionsEnum.InvalidFieldName);
                    }
                    Field field = this._fields[fieldName];
                    objArray2[num2++] = objArray[field.Ordinal];
                }
                objArray = objArray2;
            }
            if (includeIndex)
            {
                objArray[objArray.Length - 1] = idx++;
            }
            return objArray;
        }

        internal object[] getRecordByKey(object key, string[] fieldList, bool includeIndex)
        {
            int num;
            int num2;
            bool flag;
            this.checkIsDbOpen();
            if (!string.IsNullOrEmpty(this._primaryKey))
            {
                num = this.bsearch(this._index, 0, this._numRecords - 1, key);
                if (num < 0)
                {
                    return null;
                }
                num--;
                num2 = this._index[num];
            }
            else
            {
                num = (int) key;
                if ((num < 0) || (num >= this._numRecords))
                {
                    return null;
                }
                num2 = this._index[num];
            }
            object[] objArray = this.readRecord(num2, includeIndex, out flag);
            if (fieldList != null)
            {
                object[] objArray2 = new object[fieldList.Length + (includeIndex ? 1 : 0)];
                int num3 = 0;
                for (int i = 0; i < fieldList.Length; i++)
                {
                    string fieldName = fieldList[i];
                    if (!this._fields.ContainsKey(fieldName))
                    {
                        throw new FileDbException(string.Format("Invalid field name: {0}", fieldName), FileDbExceptionsEnum.InvalidFieldName);
                    }
                    Field field = this._fields[fieldName];
                    objArray2[num3++] = objArray[field.Ordinal];
                }
                objArray = objArray2;
            }
            if (includeIndex)
            {
                objArray[objArray.Length - 1] = num++;
            }
            return objArray;
        }

        internal int getRecordCount()
        {
            this.checkIsDbOpen();
            return this._numRecords;
        }

        private int getRecordSize(FieldValues record, out byte[] nullmask)
        {
            int num = 0;
            int num2 = this._fields.Count / 8;
            if ((this._fields.Count % 8) > 0)
            {
                num2++;
            }
            nullmask = new byte[num2];
            num += num2;
            foreach (Field field in this._fields)
            {
                object data = null;
                if (record.ContainsKey(field.Name))
                {
                    data = record[field.Name];
                }
                if (data != null)
                {
                    num += this.getItemSize(field, data);
                }
            }
            return num;
        }

        internal Field[] getSchema()
        {
            this.checkIsDbOpen();
            return this._fields.ToArray();
        }

        private BinaryWriter getTestWriter()
        {
            if (this._testStrm == null)
            {
                this._testStrm = new MemoryStream();
                this._testWriter = new BinaryWriter(this._testStrm);
            }
            else
            {
                this._testStrm.Seek(0L, SeekOrigin.Begin);
            }
            return this._testWriter;
        }

        internal bool moveFirst()
        {
            this.checkIsDbOpen();
            this._iteratorIndex = 0;
            return !this.isEof;
        }

        internal bool moveNext()
        {
            this.checkIsDbOpen();
            bool flag = false;
            if ((this._numRecords == 0) || this.isEof)
            {
                return flag;
            }
            this._iteratorIndex++;
            return (this._iteratorIndex < this._numRecords);
        }

        internal void open(string dbName, string encryptionKey, Encryptor encryptor, bool readOnly)
        {
            if (this._isOpen)
            {
                this.close();
            }
            this._openReadOnly = readOnly;
            try
            {
                this.openFiles(dbName, FileMode.Open);
                this._isOpen = true;
                this._dbName = dbName;
                this._iteratorIndex = 0;
                this._ver_major = 0;
                this._ver_minor = 0;
                this._ver = 0;
                if (this._dataReader.ReadInt32() != 0x123babe)
                {
                    throw new FileDbException("Invalid signature in database", FileDbExceptionsEnum.InvalidDatabaseSignature);
                }
                this._ver_major = this._dataReader.ReadByte();
                this._ver_minor = this._dataReader.ReadByte();
                this._ver = (this._ver_major * 100) + this._ver_minor;
                if (this._ver_major > 3)
                {
                    throw new FileDbException(string.Format("Cannot open newer database version {0}.{1}.  Current version is {2}", this._ver_major, this._ver_minor, 3), FileDbExceptionsEnum.CantOpenNewerDbVersion);
                }
                this.readSchema(this._dataReader);
                this._index = this.readIndex();
                if (encryptor != null)
                {
                    this._encryptor = encryptor;
                }
                else if (!string.IsNullOrEmpty(encryptionKey))
                {
                    this._encryptor = new Encryptor(encryptionKey, base.GetType().ToString());
                }
                if ((this._ver_major < 3) || (this._ver < 0xca))
                {
                    this.cleanup(true);
                }
            }
            catch (FileDbException exception1)
            {
                this.close();
                throw exception1;
            }
        }

        private void openFiles(string dbName, FileMode mode)
        {
            FileAccess read;
            if (((mode != FileMode.Create) && (mode != FileMode.CreateNew)) && ((mode != FileMode.OpenOrCreate) && !File.Exists(dbName)))
            {
                throw new FileDbException("The database file doesn't exist", FileDbExceptionsEnum.DatabaseFileNotFound);
            }
            if (this._openReadOnly)
            {
                read = FileAccess.Read;
            }
            else
            {
                read = FileAccess.ReadWrite;
            }
            this._dataStrm = File.Open(dbName, mode, read, FileShare.None);
            this._dataReader = new BinaryReader(this._dataStrm);
            if (!this._openReadOnly)
            {
                this._dataWriter = new BinaryWriter(this._dataStrm);
            }
        }

        private void orderBy(object[][] result, string[] fieldList, string[] orderByList)
        {
            List<Field> sortFields = new List<Field>(orderByList.Length);
            List<bool> sortDirLst = new List<bool>(orderByList.Length);
            List<bool> caseLst = new List<bool>(orderByList.Length);
            GetOrderByLists(this._fields, fieldList, orderByList, sortFields, sortDirLst, caseLst);
            Array.Sort(result, new RowComparer(sortFields, sortDirLst, caseLst));
        }

        private DateTime readDate(BinaryReader reader)
        {
            short year = reader.ReadInt16();
            byte month = reader.ReadByte();
            byte day = reader.ReadByte();
            byte hour = reader.ReadByte();
            byte minute = reader.ReadByte();
            byte second = reader.ReadByte();
            ushort millisecond = reader.ReadUInt16();
            return new DateTime(year, month, day, hour, minute, second, millisecond, (DateTimeKind) reader.ReadByte());
        }

        private decimal readDecimal(BinaryReader reader)
        {
            return new decimal(new int[] { reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32() });
        }

        private List<int> readIndex()
        {
            this._dataStrm.Seek((long) this._indexStartPos, SeekOrigin.Begin);
            List<int> list = new List<int>(this._numRecords);
            try
            {
                for (int i = 0; i < this._numRecords; i++)
                {
                    list.Add(this._dataReader.ReadInt32());
                }
            }
            catch (Exception)
            {
            }
            this._deletedRecords = new List<int>(this._numDeleted);
            if (this._numDeleted > 0)
            {
                try
                {
                    for (int i = 0; i < this._numDeleted; i++)
                    {
                        this._deletedRecords.Add(this._dataReader.ReadInt32());
                    }
                }
                catch
                {
                }
            }
            this.readMetaData(this._dataReader);
            return list;
        }

        private object readItem(BinaryReader dataReader, Field field)
        {
            DataTypeEnum dataType = field.DataType;
            switch (dataType)
            {
                case DataTypeEnum.Bool:
                {
                    if (!field.IsArray)
                    {
                        return (dataReader.ReadByte() == 1);
                    }
                    int num = dataReader.ReadInt32();
                    bool[] flagArray = null;
                    if (num >= 0)
                    {
                        flagArray = new bool[num];
                        for (int i = 0; i < num; i++)
                        {
                            flagArray[i] = dataReader.ReadByte() == 1;
                        }
                    }
                    return flagArray;
                }
                case ((DataTypeEnum) 4):
                case ((DataTypeEnum) 5):
                case ((DataTypeEnum) 7):
                case ((DataTypeEnum) 8):
                case ((DataTypeEnum) 12):
                case ((DataTypeEnum) 0x11):
                    break;

                case DataTypeEnum.Byte:
                {
                    if (!field.IsArray)
                    {
                        return dataReader.ReadByte();
                    }
                    int count = dataReader.ReadInt32();
                    byte[] buffer = null;
                    if (count >= 0)
                    {
                        buffer = dataReader.ReadBytes(count);
                    }
                    return buffer;
                }
                case DataTypeEnum.Int:
                {
                    if (!field.IsArray)
                    {
                        return dataReader.ReadInt32();
                    }
                    int num4 = dataReader.ReadInt32();
                    int[] numArray = null;
                    if (num4 >= 0)
                    {
                        numArray = new int[num4];
                        for (int i = 0; i < num4; i++)
                        {
                            numArray[i] = dataReader.ReadInt32();
                        }
                    }
                    return numArray;
                }
                case DataTypeEnum.UInt:
                {
                    if (!field.IsArray)
                    {
                        return dataReader.ReadUInt32();
                    }
                    int num6 = dataReader.ReadInt32();
                    uint[] numArray2 = null;
                    if (num6 >= 0)
                    {
                        numArray2 = new uint[num6];
                        for (uint i = 0; i < num6; i++)
                        {
                            numArray2[(uint) ((UIntPtr) i)] = dataReader.ReadUInt32();
                        }
                    }
                    return numArray2;
                }
                case DataTypeEnum.Int64:
                {
                    if (!field.IsArray)
                    {
                        return dataReader.ReadInt64();
                    }
                    int num8 = dataReader.ReadInt32();
                    long[] numArray3 = null;
                    if (num8 >= 0)
                    {
                        numArray3 = new long[num8];
                        for (int i = 0; i < num8; i++)
                        {
                            numArray3[i] = dataReader.ReadInt64();
                        }
                    }
                    return numArray3;
                }
                case DataTypeEnum.Float:
                {
                    if (!field.IsArray)
                    {
                        return dataReader.ReadSingle();
                    }
                    int num10 = dataReader.ReadInt32();
                    float[] numArray4 = null;
                    if (num10 >= 0)
                    {
                        numArray4 = new float[num10];
                        for (int i = 0; i < num10; i++)
                        {
                            numArray4[i] = dataReader.ReadSingle();
                        }
                    }
                    return numArray4;
                }
                case DataTypeEnum.Double:
                {
                    if (!field.IsArray)
                    {
                        return dataReader.ReadDouble();
                    }
                    int num12 = dataReader.ReadInt32();
                    double[] numArray5 = null;
                    if (num12 >= 0)
                    {
                        numArray5 = new double[num12];
                        for (int i = 0; i < num12; i++)
                        {
                            numArray5[i] = dataReader.ReadDouble();
                        }
                    }
                    return numArray5;
                }
                case DataTypeEnum.Decimal:
                {
                    if (!field.IsArray)
                    {
                        return this.readDecimal(dataReader);
                    }
                    int num14 = dataReader.ReadInt32();
                    decimal[] numArray6 = null;
                    if (num14 >= 0)
                    {
                        numArray6 = new decimal[num14];
                        for (int i = 0; i < num14; i++)
                        {
                            numArray6[i] = this.readDecimal(dataReader);
                        }
                    }
                    return numArray6;
                }
                case DataTypeEnum.DateTime:
                {
                    if (!field.IsArray)
                    {
                        if (this._ver >= 0xca)
                        {
                            return this.readDate(dataReader);
                        }
                        string s = dataReader.ReadString();
                        if (s.Length == 0)
                        {
                            return DateTime.MinValue;
                        }
                        return DateTime.Parse(s);
                    }
                    int num16 = dataReader.ReadInt32();
                    DateTime[] timeArray = null;
                    if (num16 >= 0)
                    {
                        timeArray = new DateTime[num16];
                        for (int i = 0; i < num16; i++)
                        {
                            if (this._ver < 0xca)
                            {
                                string s = dataReader.ReadString();
                                timeArray[i] = DateTime.Parse(s);
                            }
                            else
                            {
                                timeArray[i] = this.readDate(dataReader);
                            }
                        }
                    }
                    return timeArray;
                }
                case DataTypeEnum.String:
                {
                    if (!field.IsArray)
                    {
                        return dataReader.ReadString();
                    }
                    int num18 = dataReader.ReadInt32();
                    string[] strArray = null;
                    if (num18 >= 0)
                    {
                        strArray = new string[num18];
                        for (int i = 0; i < num18; i++)
                        {
                            strArray[i] = dataReader.ReadString();
                        }
                    }
                    return strArray;
                }
                default:
                    if (dataType == DataTypeEnum.Guid)
                    {
                        if (!field.IsArray)
                        {
                            return new Guid(dataReader.ReadBytes(0x10));
                        }
                        int num20 = dataReader.ReadInt32();
                        Guid[] guidArray = null;
                        if (num20 >= 0)
                        {
                            guidArray = new Guid[num20];
                            for (int i = 0; i < num20; i++)
                            {
                                byte[] b = dataReader.ReadBytes(0x10);
                                guidArray[i] = new Guid(b);
                            }
                        }
                        return guidArray;
                    }
                    break;
            }
            throw new FileDbException(string.Format("Invalid data type encountered in data file ({0})", (int) field.DataType), FileDbExceptionsEnum.InvalidDataType);
        }

        private void readMetaData(BinaryReader reader)
        {
            this._metaData = null;
            try
            {
                if (this._ver_major >= 2)
                {
                    DataTypeEnum enum2;
                    try
                    {
                        enum2 = (DataTypeEnum) ((short) reader.ReadInt32());
                    }
                    catch (EndOfStreamException)
                    {
                        return;
                    }
                    switch (enum2)
                    {
                        case DataTypeEnum.Byte:
                        {
                            int count = reader.ReadInt32();
                            this._metaData = reader.ReadBytes(count);
                            break;
                        }
                        case DataTypeEnum.String:
                            this._metaData = reader.ReadString();
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private byte[] readNullmask(BinaryReader dataReader)
        {
            int count = this._fields.Count / 8;
            if ((this._fields.Count % 8) > 0)
            {
                count++;
            }
            return dataReader.ReadBytes(count);
        }

        private object[] readRecord(int offset, bool includeIndex)
        {
            int num;
            bool flag;
            return this.readRecord(this._dataReader, offset, includeIndex, out num, out flag);
        }

        private object[] readRecord(int offset, bool includeIndex, out bool deleted)
        {
            int num;
            return this.readRecord(this._dataReader, offset, includeIndex, out num, out deleted);
        }

        private object[] readRecord(int offset, bool includeIndex, out int size, out bool deleted)
        {
            return this.readRecord(this._dataReader, offset, includeIndex, out size, out deleted);
        }

        private object[] readRecord(BinaryReader dataReader, int offset, bool includeIndex, out int size, out bool deleted)
        {
            this._dataStrm.Seek((long) offset, SeekOrigin.Begin);
            size = dataReader.ReadInt32();
            if (size < 0)
            {
                deleted = true;
                size = -size;
            }
            else
            {
                deleted = false;
            }
            int count = this._fields.Count;
            if (includeIndex)
            {
                count++;
            }
            object[] objArray = new object[count];
            if (this._encryptor != null)
            {
                byte[] encryptedData = dataReader.ReadBytes(size);
                dataReader = new BinaryReader(new MemoryStream(this._encryptor.Decrypt(encryptedData)));
            }
            byte[] buffer = null;
            if (this._ver >= 0xca)
            {
                buffer = this.readNullmask(dataReader);
            }
            int index = 0;
            while (index < this._fields.Count)
            {
                Field field;
                if (buffer == null)
                {
                    goto Label_00B6;
                }
                int num3 = index / 8;
                num3 = index % 8;
                if ((buffer[num3] & s_bitmask[num3]) == 0)
                {
                    goto Label_00B6;
                }
            Label_00B0:
                index++;
                continue;
            Label_00B6:
                field = this._fields[index];
                objArray[index] = this.readItem(dataReader, field);
                goto Label_00B0;
            }
            return objArray;
        }

        private object readRecordKey(BinaryReader dataReader, int offset)
        {
            if (this._encryptor == null)
            {
                this._dataStrm.Seek((long) (offset + 4), SeekOrigin.Begin);
            }
            else
            {
                this._dataStrm.Seek((long) offset, SeekOrigin.Begin);
                int count = dataReader.ReadInt32();
                if (count < 0)
                {
                    count = -count;
                }
                byte[] encryptedData = dataReader.ReadBytes(count);
                dataReader = new BinaryReader(new MemoryStream(this._encryptor.Decrypt(encryptedData)));
            }
            if (this._ver >= 0xca)
            {
                this.readNullmask(dataReader);
            }
            return this.readItem(dataReader, this._primaryKeyField);
        }

        private byte[] readRecordRaw(BinaryReader dataReader, int offset, out bool deleted)
        {
            this._dataStrm.Seek((long) offset, SeekOrigin.Begin);
            int count = dataReader.ReadInt32();
            if (count < 0)
            {
                deleted = true;
                count = -count;
            }
            else
            {
                deleted = false;
            }
            byte[] buffer = null;
            if (count > 0)
            {
                buffer = dataReader.ReadBytes(count);
            }
            return buffer;
        }

        private void readSchema()
        {
            this.readSchema(this._dataReader);
        }

        private void readSchema(BinaryReader reader)
        {
            this._dataStrm.Seek(6L, SeekOrigin.Begin);
            this._numRecords = reader.ReadInt32();
            this._numDeleted = reader.ReadInt32();
            this._indexStartPos = reader.ReadInt32();
            if (this._ver >= 300)
            {
                this._userVersion = reader.ReadString();
            }
            this._primaryKey = reader.ReadString();
            int num = reader.ReadInt32();
            this._fields = new FwNs.Data.FyleDbNs.Fields();
            for (int i = 0; i < num; i++)
            {
                string name = reader.ReadString();
                DataTypeEnum type = (DataTypeEnum) reader.ReadInt16();
                if (this._ver < 0xc9)
                {
                    if (type == DataTypeEnum.Byte)
                    {
                        type = DataTypeEnum.Bool;
                    }
                    else if (type == ((DataTypeEnum) 1))
                    {
                        type = DataTypeEnum.Byte;
                    }
                    else if (type == ((DataTypeEnum) 2))
                    {
                        type = DataTypeEnum.Int;
                    }
                    else if (type == DataTypeEnum.Bool)
                    {
                        type = DataTypeEnum.UInt;
                    }
                    else if (type == ((DataTypeEnum) 8))
                    {
                        type = DataTypeEnum.Int64;
                    }
                    else if (type == ((DataTypeEnum) 4))
                    {
                        type = DataTypeEnum.Float;
                    }
                    else if (type == ((DataTypeEnum) 5))
                    {
                        type = DataTypeEnum.Double;
                    }
                    else if (type == DataTypeEnum.Int)
                    {
                        type = DataTypeEnum.Decimal;
                    }
                    else if (type == ((DataTypeEnum) 7))
                    {
                        type = DataTypeEnum.DateTime;
                    }
                    else if (type == ((DataTypeEnum) 0))
                    {
                        type = DataTypeEnum.String;
                    }
                    else if (type == DataTypeEnum.UInt)
                    {
                        type = DataTypeEnum.Guid;
                    }
                }
                Field field = new Field(name, type, i);
                this._fields.Add(field);
                if (string.Compare(this._primaryKey, name, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    field.IsPrimaryKey = true;
                    this._primaryKeyField = field;
                }
                int num1 = reader.ReadInt32();
                if ((num1 & 1) == 1)
                {
                    field.AutoIncStart = reader.ReadInt32();
                    field.CurAutoIncVal = reader.ReadInt32();
                }
                if ((num1 & 2) == 2)
                {
                    field.IsArray = true;
                }
                if (this._ver_major >= 2)
                {
                    field.Comment = reader.ReadString();
                }
                else
                {
                    field.Comment = string.Empty;
                }
            }
            this._dataStartPos = (int) this._dataStrm.Position;
        }

        internal bool recordExists(int key)
        {
            this.checkIsDbOpen();
            bool flag = false;
            if (!string.IsNullOrEmpty(this._primaryKey))
            {
                if (this.bsearch(this._index, 0, this._numRecords - 1, key) > 0)
                {
                    flag = true;
                }
                return flag;
            }
            if (key.GetType() != typeof(int))
            {
                throw new FileDbException("If there is no primary key on the database, the key must be the integer record number", FileDbExceptionsEnum.NeedIntegerKey);
            }
            if ((key < 0) || (key >= this._numRecords))
            {
                throw new FileDbException(string.Format("Record index out of range - {0}.", key), FileDbExceptionsEnum.IndexOutOfRange);
            }
            return true;
        }

        internal void reindex()
        {
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords != 0)
            {
                try
                {
                    int capacity = this._numRecords + this._numDeleted;
                    List<int> lstIndex = new List<int>(capacity);
                    this._deletedRecords = new List<int>();
                    this._dataStrm.Seek((long) this._dataStartPos, SeekOrigin.Begin);
                    int offset = this._dataStartPos;
                    for (int i = 0; i < capacity; i++)
                    {
                        int num4;
                        bool flag;
                        this._dataStrm.Seek((long) offset, SeekOrigin.Begin);
                        object[] objArray = this.readRecord(offset, false, out num4, out flag);
                        if (!flag)
                        {
                            if (this._primaryKeyField != null)
                            {
                                object target = objArray[this._primaryKeyField.Ordinal];
                                int index = -this.bsearch(lstIndex, 0, lstIndex.Count - 2, target) - 1;
                                lstIndex.Insert(index, offset);
                            }
                            else
                            {
                                lstIndex.Add(offset);
                            }
                        }
                        else
                        {
                            this._deletedRecords.Add(offset);
                        }
                        offset += num4 + 4;
                    }
                    this._indexStartPos = offset;
                    this._dataStrm.Seek(6L, SeekOrigin.Begin);
                    this._dataWriter.Write(this._numRecords = lstIndex.Count);
                    this._dataWriter.Write(this._numDeleted = this._deletedRecords.Count);
                    this._dataWriter.Write(this._indexStartPos);
                    this._index = lstIndex;
                }
                finally
                {
                    this.flush(true);
                }
            }
        }

        internal int removeAll()
        {
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords != 0)
            {
                this._dataStrm.Seek(6L, SeekOrigin.Begin);
                this._numRecords = this._numDeleted = 0;
                this._indexStartPos = this._dataStartPos;
                this._index.Clear();
                this._deletedRecords.Clear();
                this.writeSchema(this._dataWriter);
                this.flush(true);
            }
            return 0;
        }

        internal bool removeByIndex(int recordNum)
        {
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords == 0)
            {
                return false;
            }
            try
            {
                if ((recordNum < 0) || (recordNum >= this._numRecords))
                {
                    throw new FileDbException(string.Format("Record index out of range - {0}.", recordNum), FileDbExceptionsEnum.IndexOutOfRange);
                }
                this.setRecordDeleted(this._index[recordNum], true);
                this._deletedRecords.Add(this._index[recordNum]);
                this._index.RemoveAt(recordNum);
                this._dataStrm.Seek(6L, SeekOrigin.Begin);
                int num = this._numRecords - 1;
                this._numRecords = num;
                this._dataWriter.Write(num);
                num = this._numDeleted + 1;
                this._numDeleted = num;
                this._dataWriter.Write(num);
            }
            finally
            {
                if (this.AutoFlush)
                {
                    this.flush(true);
                }
            }
            this.checkAutoClean();
            return true;
        }

        internal bool removeByKey(object key)
        {
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords == 0)
            {
                return false;
            }
            try
            {
                int num;
                if (!string.IsNullOrEmpty(this._primaryKey))
                {
                    num = this.bsearch(this._index, 0, this._numRecords - 1, key);
                    if (num < 0)
                    {
                        return false;
                    }
                    num--;
                }
                else
                {
                    if (key.GetType() != typeof(int))
                    {
                        throw new FileDbException("Invalid key field type (record number) - must be type Int32", FileDbExceptionsEnum.InvalidKeyFieldType);
                    }
                    num = (int) key;
                    if ((num < 0) || (num >= this._numRecords))
                    {
                        throw new FileDbException(string.Format("Record index out of range - {0}.", num), FileDbExceptionsEnum.IndexOutOfRange);
                    }
                }
                this.setRecordDeleted(this._index[num], true);
                this._deletedRecords.Add(this._index[num]);
                this._index.RemoveAt(num);
                this._dataStrm.Seek(6L, SeekOrigin.Begin);
                int num2 = this._numRecords - 1;
                this._numRecords = num2;
                this._dataWriter.Write(num2);
                num2 = this._numDeleted + 1;
                this._numDeleted = num2;
                this._dataWriter.Write(num2);
            }
            finally
            {
                if (this.AutoFlush)
                {
                    this.flush(true);
                }
            }
            this.checkAutoClean();
            return true;
        }

        internal int removeByValue(FilterExpression searchExp)
        {
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords == 0)
            {
                return 0;
            }
            string fieldName = searchExp.FieldName;
            if (fieldName[0] == '~')
            {
                fieldName = fieldName.Substring(1);
                searchExp.MatchType = MatchTypeEnum.IgnoreCase;
            }
            if (!this._fields.ContainsKey(fieldName))
            {
                throw new FileDbException(string.Format("Invalid field name: {0}", searchExp.FieldName), FileDbExceptionsEnum.InvalidFieldName);
            }
            Field field = this._fields[fieldName];
            int num = 0;
            try
            {
                Regex regex = null;
                for (int i = 0; i < this._numRecords; i++)
                {
                    bool flag;
                    object[] record = this.readRecord(this._index[i], false, out flag);
                    record[field.Ordinal].ToString();
                    if (((searchExp.Equality == EqualityEnum.Like) || (searchExp.Equality == EqualityEnum.NotLike)) && (regex == null))
                    {
                        regex = new Regex(searchExp.SearchVal.ToString(), RegexOptions.IgnoreCase);
                    }
                    if (evaluate(field, searchExp, record, regex))
                    {
                        this.setRecordDeleted(this._index[i], true);
                        this._deletedRecords.Add(this._index[i]);
                        this._index.RemoveAt(i);
                        this._numRecords--;
                        this._numDeleted++;
                        i--;
                        num++;
                    }
                }
                if (num > 0)
                {
                    this._dataStrm.Seek(6L, SeekOrigin.Begin);
                    this._dataWriter.Write(this._numRecords);
                    this._dataWriter.Write(this._numDeleted);
                }
            }
            finally
            {
                if (this.AutoFlush)
                {
                    this.flush(true);
                }
            }
            this.checkAutoClean();
            return num;
        }

        internal int removeByValues(FilterExpressionGroup searchExpGrp)
        {
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords == 0)
            {
                return 0;
            }
            int num = 0;
            try
            {
                for (int i = 0; i < this._numRecords; i++)
                {
                    bool flag;
                    object[] record = this.readRecord(this._index[i], false, out flag);
                    if (evaluate(searchExpGrp, record, this._fields))
                    {
                        this.setRecordDeleted(this._index[i], true);
                        this._deletedRecords.Add(this._index[i]);
                        this._index.RemoveAt(i);
                        this._numRecords--;
                        this._numDeleted++;
                        i--;
                        num++;
                    }
                }
                if (num > 0)
                {
                    this._dataStrm.Seek(6L, SeekOrigin.Begin);
                    this._dataWriter.Write(this._numRecords);
                    this._dataWriter.Write(this._numDeleted);
                }
            }
            finally
            {
                if (this.AutoFlush)
                {
                    this.flush(true);
                }
            }
            this.checkAutoClean();
            return num;
        }

        internal void setAutoCleanThreshold(int threshold)
        {
            this._autoCleanThreshold = threshold;
            if ((this._isOpen && (this._autoCleanThreshold >= 0)) && (this._numDeleted > this._autoCleanThreshold))
            {
                this.cleanup(false);
            }
        }

        internal void setEncryptionKey(string encryptionKey)
        {
            this._encryptor = new Encryptor(encryptionKey, base.GetType().ToString());
        }

        private void setNullMask(byte[] nullmask, int pos)
        {
            int index = pos / 8;
            pos = pos % 8;
            byte num2 = nullmask[index];
            nullmask[index] = (byte) (num2 | s_bitmask[pos]);
        }

        private void setRecordDeleted(int pos, bool deleted)
        {
            this._dataStrm.Seek((long) pos, SeekOrigin.Begin);
            int num = this._dataReader.ReadInt32();
            if (num > 0)
            {
                num = -num;
            }
            this._dataStrm.Seek((long) pos, SeekOrigin.Begin);
            this._dataWriter.Write(num);
        }

        internal void updateRecordByIndex(FieldValues record, int index)
        {
            bool flag;
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords == 0)
            {
                throw new FileDbException("There are no records in the database", FileDbExceptionsEnum.DatabaseEmpty);
            }
            this.updateRecordByIndex(record, index, this._index, true, true, out flag);
            if (this.AutoFlush)
            {
                this.flush(flag);
            }
            this.checkAutoClean();
        }

        private void updateRecordByIndex(FieldValues record, int index, List<int> lstIndex, bool bNormalizeFieldNames, bool bVerifyRecordSchema, out bool indexUpdated)
        {
            byte[] buffer;
            indexUpdated = false;
            if (bVerifyRecordSchema)
            {
                this.verifyRecordSchema(record);
            }
            int num = 0;
            if (!string.IsNullOrEmpty(this._primaryKey) && record.ContainsKey(this._primaryKey))
            {
                int num7 = this.bsearch(lstIndex, 0, this._numRecords - 1, record[this._primaryKey]);
                if (num7 >= 0)
                {
                    num7--;
                    if (num7 != index)
                    {
                        throw new FileDbException(string.Format("Duplicate key violation - Field: '{0}' - Value: '{1}'", this._primaryKey, record[this._primaryKey].ToString()), FileDbExceptionsEnum.DuplicatePrimaryKey);
                    }
                }
            }
            else if ((index < 0) || (index > (this._numRecords - 1)))
            {
                throw new FileDbException(string.Format("Record index out of range - {0}.", index), FileDbExceptionsEnum.IndexOutOfRange);
            }
            int offset = lstIndex[index];
            this._dataStrm.Seek((long) offset, SeekOrigin.Begin);
            num = this._dataReader.ReadInt32();
            FieldValues values = record;
            if (record.Count < this._fields.Count)
            {
                object[] objArray = this.readRecord(offset, false);
                values = new FieldValues(objArray.Length);
                foreach (string str in record.Keys)
                {
                    values.Add(str, record[str]);
                }
                foreach (Field field in this._fields)
                {
                    if (!values.ContainsKey(field.Name))
                    {
                        values.Add(field.Name, objArray[field.Ordinal]);
                    }
                }
            }
            int size = this.getRecordSize(values, out buffer);
            int num4 = -1;
            int num5 = this._indexStartPos;
            if (size > num)
            {
                int num8 = 0;
                foreach (int num9 in this._deletedRecords)
                {
                    this._dataStrm.Seek((long) num9, SeekOrigin.Begin);
                    if (-this._dataReader.ReadInt32() >= size)
                    {
                        num5 = num9;
                        num4 = num8;
                        break;
                    }
                    num8++;
                }
            }
            else
            {
                num5 = offset;
            }
            this._dataStrm.Seek((long) num5, SeekOrigin.Begin);
            this.writeRecord(this._dataWriter, values, size, buffer, false);
            int position = (int) this._dataStrm.Position;
            if (position > this._indexStartPos)
            {
                this._indexStartPos = position;
                this.writeIndexStart(this._dataWriter);
            }
            if (size > num)
            {
                this._deletedRecords.Add(offset);
                if (num4 < 0)
                {
                    this._numDeleted++;
                }
                else
                {
                    this._deletedRecords.RemoveAt(num4);
                }
                lstIndex[index] = num5;
                indexUpdated = true;
                this._dataStrm.Seek((long) offset, SeekOrigin.Begin);
                this._dataWriter.Write(-num);
                this._dataStrm.Seek(10L, SeekOrigin.Begin);
                this._dataWriter.Write(this._numDeleted);
            }
        }

        internal void updateRecordByKey(FieldValues record, object key)
        {
            string name;
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords == 0)
            {
                throw new FileDbException("There are no records in the database", FileDbExceptionsEnum.DatabaseEmpty);
            }
            if (string.IsNullOrEmpty(this._primaryKey))
            {
                name = this._fields[0].Name;
            }
            else
            {
                name = this._fields[this._primaryKey].Name;
            }
            string[] fieldList = new string[] { name };
            object[] objArray = this.getRecordByKey(key, fieldList, true);
            if (objArray == null)
            {
                throw new FileDbException("Primary key field value not found", FileDbExceptionsEnum.PrimaryKeyValueNotFound);
            }
            this.updateRecordByIndex(record, (int) objArray[objArray.Length - 1]);
        }

        internal int updateRecords(FilterExpression searchExp, FieldValues record)
        {
            FilterExpressionGroup searchExpGrp = new FilterExpressionGroup();
            searchExpGrp.Add(BoolOpEnum.And, searchExp);
            return this.updateRecords(searchExpGrp, record);
        }

        internal int updateRecords(FilterExpressionGroup searchExpGrp, FieldValues record)
        {
            this.checkIsDbOpen();
            this.checkReadOnly();
            if (this._numRecords == 0)
            {
                return 0;
            }
            this.verifyRecordSchema(record);
            bool flag = record.Count >= this._fields.Count;
            int num = 0;
            bool saveIndex = false;
            try
            {
                for (int i = 0; i < this._numRecords; i++)
                {
                    object[] objArray = this.readRecord(this._index[i], false);
                    if (evaluate(searchExpGrp, objArray, this._fields))
                    {
                        bool flag3;
                        FieldValues values = record;
                        if (!flag)
                        {
                            values = new FieldValues(objArray.Length);
                            foreach (string str in record.Keys)
                            {
                                values.Add(str, record[str]);
                            }
                            foreach (Field field in this._fields)
                            {
                                if (!values.ContainsKey(field.Name))
                                {
                                    values.Add(field.Name, objArray[field.Ordinal]);
                                }
                            }
                        }
                        this.updateRecordByIndex(values, i, this._index, false, false, out flag3);
                        if (flag3)
                        {
                            saveIndex = flag3;
                        }
                        num++;
                    }
                }
            }
            finally
            {
                if (this.AutoFlush)
                {
                    this.flush(saveIndex);
                }
            }
            if (num > 0)
            {
                this.checkAutoClean();
            }
            return num;
        }

        private void verifyFieldSchema(Field field, object value)
        {
            if (!field.IsAutoInc && (value != null))
            {
                if (field.IsArray && !value.GetType().IsArray)
                {
                    throw new FileDbException(string.Format("Non array value passed for array field '{0}'", field.Name), FileDbExceptionsEnum.NonArrayValue);
                }
                DataTypeEnum dataType = field.DataType;
                switch (dataType)
                {
                    case DataTypeEnum.Bool:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToBoolean(value);
                                return;
                            }
                            return;
                        }
                        if (value.GetType() != typeof(byte[]))
                        {
                            value = (bool[]) value;
                            return;
                        }
                        value = (byte[]) value;
                        return;

                    case ((DataTypeEnum) 4):
                    case ((DataTypeEnum) 5):
                    case ((DataTypeEnum) 7):
                    case ((DataTypeEnum) 8):
                    case ((DataTypeEnum) 12):
                    case ((DataTypeEnum) 0x11):
                        break;

                    case DataTypeEnum.Byte:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToByte(value);
                                return;
                            }
                            return;
                        }
                        value = (byte[]) value;
                        return;

                    case DataTypeEnum.Int:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToInt32(value);
                                return;
                            }
                            return;
                        }
                        value = (int[]) value;
                        return;

                    case DataTypeEnum.UInt:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToUInt32(value);
                                return;
                            }
                            return;
                        }
                        value = (uint[]) value;
                        return;

                    case DataTypeEnum.Int64:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToInt64(value);
                                return;
                            }
                            return;
                        }
                        value = (long[]) value;
                        return;

                    case DataTypeEnum.Float:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToSingle(value);
                                return;
                            }
                            return;
                        }
                        value = (float[]) value;
                        return;

                    case DataTypeEnum.Double:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToDouble(value);
                                return;
                            }
                            return;
                        }
                        value = (double[]) value;
                        return;

                    case DataTypeEnum.Decimal:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToDecimal(value);
                                return;
                            }
                            return;
                        }
                        value = (decimal[]) value;
                        return;

                    case DataTypeEnum.DateTime:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                Convert.ToDateTime(value);
                                return;
                            }
                            return;
                        }
                        if (value.GetType() != typeof(string[]))
                        {
                            value = (DateTime[]) value;
                            return;
                        }
                        value = (string[]) value;
                        return;

                    case DataTypeEnum.String:
                        if (!field.IsArray)
                        {
                            if (value != null)
                            {
                                value.ToString();
                                return;
                            }
                            return;
                        }
                        value = (string[]) value;
                        return;

                    default:
                        if (dataType == DataTypeEnum.Guid)
                        {
                            if (field.IsArray)
                            {
                                value = (Guid[]) value;
                                return;
                            }
                            if (value != null)
                            {
                                convertToGuid(value);
                                return;
                            }
                            Guid.NewGuid();
                            return;
                        }
                        break;
                }
                throw new FileDbException(string.Format("Invalid data type for field '{0}' - expected '{1}' but got '{2}'", field.Name, field.DataType.ToString(), value.GetType().Name), FileDbExceptionsEnum.InvalidDataType);
            }
        }

        private void verifyRecordSchema(FieldValues record)
        {
            foreach (string str in record.Keys)
            {
                if (string.Compare(str, "index", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (!this._fields.ContainsKey(str))
                    {
                        throw new FileDbException(string.Format("Invalid field name: {0}", str), FileDbExceptionsEnum.InvalidFieldName);
                    }
                    Field field = this._fields[str];
                    this.verifyFieldSchema(field, record[str]);
                }
            }
        }

        private void writeDate(DateTime dt, BinaryWriter writer)
        {
            writer.Write((short) dt.Year);
            writer.Write((byte) dt.Month);
            writer.Write((byte) dt.Day);
            writer.Write((byte) dt.Hour);
            writer.Write((byte) dt.Minute);
            writer.Write((byte) dt.Second);
            writer.Write((ushort) dt.Millisecond);
            writer.Write((byte) dt.Kind);
        }

        private void writeDbHeader(BinaryWriter writer)
        {
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(0x123babe);
            writer.Write(3);
            writer.Write(0);
        }

        private void writeDecimal(BinaryWriter writer, decimal dec)
        {
            int[] bits = decimal.GetBits(dec);
            writer.Write(bits[0]);
            writer.Write(bits[1]);
            writer.Write(bits[2]);
            writer.Write(bits[3]);
        }

        private void writeField(BinaryWriter writer, Field field)
        {
            writer.Write(field.Name);
            writer.Write((short) field.DataType);
            int num = 0;
            if (field.IsAutoInc)
            {
                num |= 1;
            }
            if (field.IsArray)
            {
                num |= 2;
            }
            writer.Write(num);
            if (field.IsAutoInc)
            {
                writer.Write(field.AutoIncStart);
                writer.Write(field.CurAutoIncVal);
            }
            writer.Write((field.Comment == null) ? string.Empty : field.Comment);
        }

        private void writeIndex(Stream fileStrm, BinaryWriter writer, List<int> index)
        {
            fileStrm.Seek((long) this._indexStartPos, SeekOrigin.Begin);
            for (int i = 0; i < Math.Min(this._numRecords, index.Count); i++)
            {
                writer.Write(index[i]);
            }
            for (int j = 0; j < Math.Min(this._numDeleted, this._deletedRecords.Count); j++)
            {
                writer.Write(this._deletedRecords[j]);
            }
            this.writeMetaData(writer);
            fileStrm.SetLength(fileStrm.Position);
        }

        private void writeIndexStart(BinaryWriter writer)
        {
            writer.Seek(14, SeekOrigin.Begin);
            writer.Write(this._indexStartPos);
        }

        private void writeItem(BinaryWriter dataWriter, Field field, object data)
        {
            if (data != null)
            {
                bool flag;
                DataTypeEnum dataType = field.DataType;
                switch (dataType)
                {
                    case DataTypeEnum.Bool:
                        if (field.IsArray)
                        {
                            if (data.GetType() == typeof(bool[]))
                            {
                                bool[] flagArray = (bool[]) data;
                                dataWriter.Write(flagArray.Length);
                                bool[] flagArray2 = flagArray;
                                for (int i = 0; i < flagArray2.Length; i++)
                                {
                                    dataWriter.Write(flagArray2[i] ? 1 : 0);
                                }
                                return;
                            }
                            if (data.GetType() != typeof(byte[]))
                            {
                                throw new FileDbException("Invalid Bool type", FileDbExceptionsEnum.InvalidDataType);
                            }
                            byte[] buffer = (byte[]) data;
                            dataWriter.Write(buffer.Length);
                            foreach (byte num3 in buffer)
                            {
                                dataWriter.Write(num3);
                            }
                            return;
                        }
                        if (data.GetType() != typeof(bool))
                        {
                            if (data.GetType() != typeof(byte))
                            {
                                throw new FileDbException("Invalid Bool type", FileDbExceptionsEnum.InvalidDataType);
                            }
                            flag = ((byte) data) == 0;
                            break;
                        }
                        flag = (bool) data;
                        break;

                    case ((DataTypeEnum) 4):
                    case ((DataTypeEnum) 5):
                    case ((DataTypeEnum) 7):
                    case ((DataTypeEnum) 8):
                    case ((DataTypeEnum) 12):
                    case ((DataTypeEnum) 0x11):
                        goto Label_0721;

                    case DataTypeEnum.Byte:
                    {
                        if (!field.IsArray)
                        {
                            byte num4;
                            if (data.GetType() != typeof(byte))
                            {
                                num4 = Convert.ToByte(data);
                            }
                            else
                            {
                                num4 = (byte) data;
                            }
                            dataWriter.Write(num4);
                            return;
                        }
                        byte[] buffer3 = (byte[]) data;
                        dataWriter.Write(buffer3.Length);
                        foreach (byte num6 in buffer3)
                        {
                            dataWriter.Write(num6);
                        }
                        return;
                    }
                    case DataTypeEnum.Int:
                    {
                        if (!field.IsArray)
                        {
                            int num7;
                            if (data.GetType() != typeof(int))
                            {
                                num7 = Convert.ToInt32(data);
                            }
                            else
                            {
                                num7 = (int) data;
                            }
                            dataWriter.Write(num7);
                            return;
                        }
                        int[] numArray = (int[]) data;
                        dataWriter.Write(numArray.Length);
                        foreach (int num9 in numArray)
                        {
                            dataWriter.Write(num9);
                        }
                        return;
                    }
                    case DataTypeEnum.UInt:
                    {
                        if (!field.IsArray)
                        {
                            uint num10;
                            if (data.GetType() != typeof(uint))
                            {
                                num10 = Convert.ToUInt32(data);
                            }
                            else
                            {
                                num10 = (uint) data;
                            }
                            dataWriter.Write(num10);
                            return;
                        }
                        uint[] numArray3 = (uint[]) data;
                        dataWriter.Write(numArray3.Length);
                        foreach (uint num12 in numArray3)
                        {
                            dataWriter.Write(num12);
                        }
                        return;
                    }
                    case DataTypeEnum.Int64:
                    {
                        if (!field.IsArray)
                        {
                            long num13;
                            if (data.GetType() != typeof(long))
                            {
                                num13 = Convert.ToInt64(data);
                            }
                            else
                            {
                                num13 = (long) data;
                            }
                            dataWriter.Write(num13);
                            return;
                        }
                        long[] numArray5 = (long[]) data;
                        dataWriter.Write(numArray5.Length);
                        foreach (long num15 in numArray5)
                        {
                            dataWriter.Write(num15);
                        }
                        return;
                    }
                    case DataTypeEnum.Float:
                    {
                        if (!field.IsArray)
                        {
                            float num16;
                            if (data.GetType() != typeof(float))
                            {
                                num16 = Convert.ToSingle(data);
                            }
                            else
                            {
                                num16 = (float) data;
                            }
                            dataWriter.Write(num16);
                            return;
                        }
                        float[] numArray7 = (float[]) data;
                        dataWriter.Write(numArray7.Length);
                        foreach (float num18 in numArray7)
                        {
                            dataWriter.Write(num18);
                        }
                        return;
                    }
                    case DataTypeEnum.Double:
                    {
                        if (!field.IsArray)
                        {
                            double num19;
                            if (data.GetType() != typeof(double))
                            {
                                num19 = Convert.ToDouble(data);
                            }
                            else
                            {
                                num19 = (double) data;
                            }
                            dataWriter.Write(num19);
                            return;
                        }
                        double[] numArray9 = (double[]) data;
                        dataWriter.Write(numArray9.Length);
                        foreach (double num21 in numArray9)
                        {
                            dataWriter.Write(num21);
                        }
                        return;
                    }
                    case DataTypeEnum.Decimal:
                    {
                        if (!field.IsArray)
                        {
                            decimal num22;
                            if (data.GetType() != typeof(decimal))
                            {
                                num22 = Convert.ToDecimal(data);
                            }
                            else
                            {
                                num22 = (decimal) data;
                            }
                            this.writeDecimal(dataWriter, num22);
                            return;
                        }
                        decimal[] numArray11 = (decimal[]) data;
                        dataWriter.Write(numArray11.Length);
                        foreach (decimal num24 in numArray11)
                        {
                            this.writeDecimal(dataWriter, num24);
                        }
                        return;
                    }
                    case DataTypeEnum.DateTime:
                    {
                        if (!field.IsArray)
                        {
                            if (data.GetType() == typeof(DateTime))
                            {
                                DateTime time3 = (DateTime) data;
                                this.writeDate(time3, dataWriter);
                                return;
                            }
                            if (data.GetType() != typeof(string))
                            {
                                throw new FileDbException("Invalid DateTime type", FileDbExceptionsEnum.InvalidDataType);
                            }
                            DateTime dt = DateTime.Parse(data.ToString());
                            this.writeDate(dt, dataWriter);
                            return;
                        }
                        if (data.GetType() != typeof(DateTime[]))
                        {
                            if (data.GetType() != typeof(string[]))
                            {
                                throw new FileDbException("Invalid DateTime type", FileDbExceptionsEnum.InvalidDataType);
                            }
                            string[] strArray = (string[]) data;
                            dataWriter.Write(strArray.Length);
                            string[] strArray2 = strArray;
                            for (int i = 0; i < strArray2.Length; i++)
                            {
                                DateTime dt = DateTime.Parse(strArray2[i]);
                                this.writeDate(dt, dataWriter);
                            }
                            return;
                        }
                        DateTime[] timeArray = (DateTime[]) data;
                        dataWriter.Write(timeArray.Length);
                        foreach (DateTime time in timeArray)
                        {
                            this.writeDate(time, dataWriter);
                        }
                        return;
                    }
                    case DataTypeEnum.String:
                    {
                        if (!field.IsArray)
                        {
                            dataWriter.Write(data.ToString());
                            return;
                        }
                        string[] arr = (string[]) data;
                        this.writeStringArray(dataWriter, arr);
                        return;
                    }
                    default:
                        if (dataType == DataTypeEnum.Guid)
                        {
                            if (field.IsArray)
                            {
                                if (data.GetType() == typeof(Guid[]))
                                {
                                    Guid[] guidArray = (Guid[]) data;
                                    dataWriter.Write(guidArray.Length);
                                    foreach (Guid guid in guidArray)
                                    {
                                        dataWriter.Write(guid.ToByteArray());
                                    }
                                    return;
                                }
                                if (data.GetType() != typeof(string[]))
                                {
                                    throw new FileDbException("Invalid DateTime type", FileDbExceptionsEnum.InvalidDataType);
                                }
                                string[] strArray4 = (string[]) data;
                                dataWriter.Write(strArray4.Length);
                                foreach (string str in strArray4)
                                {
                                    dataWriter.Write(new Guid(str).ToByteArray());
                                }
                                return;
                            }
                            if (data.GetType() == typeof(Guid))
                            {
                                dataWriter.Write(((Guid) data).ToByteArray());
                                return;
                            }
                            if (data.GetType() != typeof(string))
                            {
                                throw new FileDbException("Invalid DateTime type", FileDbExceptionsEnum.InvalidDataType);
                            }
                            dataWriter.Write(new Guid(data.ToString()).ToByteArray());
                            return;
                        }
                        goto Label_0721;
                }
                dataWriter.Write(flag ? 1 : 0);
            }
            return;
        Label_0721:
            throw new FileDbException(string.Format("Invalid data type for field '{0}' - expected '{1}' but got '{2}'", field.Name, field.DataType.ToString(), data.GetType().Name), FileDbExceptionsEnum.InvalidDataType);
        }

        private void writeMetaData(BinaryWriter dataWriter)
        {
            if (this._metaData != null)
            {
                Type type = this._metaData.GetType();
                if (type == typeof(string))
                {
                    dataWriter.Write(0x12);
                    dataWriter.Write((string) this._metaData);
                }
                else if (type == typeof(byte[]))
                {
                    dataWriter.Write(6);
                    byte[] buffer = (byte[]) this._metaData;
                    dataWriter.Write(buffer.Length);
                    dataWriter.Write(buffer);
                }
            }
        }

        private void writeNumRecords(BinaryWriter writer)
        {
            writer.Seek(6, SeekOrigin.Begin);
            writer.Write(this._numRecords);
        }

        private void writeRecord(BinaryWriter dataWriter, FieldValues record, int size, byte[] nullmask, bool deleted)
        {
            if ((size < 0) && (this._encryptor == null))
            {
                size = this.getRecordSize(record, out nullmask);
            }
            int pos = 0;
            foreach (Field field in this._fields)
            {
                object obj2 = null;
                if (record.ContainsKey(field.Name))
                {
                    obj2 = record[field.Name];
                }
                if (obj2 == null)
                {
                    this.setNullMask(nullmask, pos);
                }
                pos++;
            }
            MemoryStream output = null;
            BinaryWriter writer = dataWriter;
            if (this._encryptor != null)
            {
                output = new MemoryStream((size + nullmask.Length) + 100);
                dataWriter = new BinaryWriter(output);
            }
            else
            {
                if (deleted)
                {
                    size = -size;
                }
                dataWriter.Write(size);
            }
            dataWriter.Write(nullmask);
            pos = 0;
            foreach (Field field2 in this._fields)
            {
                object data = null;
                if (record.ContainsKey(field2.Name))
                {
                    data = record[field2.Name];
                }
                if (data != null)
                {
                    this.writeItem(dataWriter, field2, data);
                }
                pos++;
            }
            if (this._encryptor != null)
            {
                output.Seek(0L, SeekOrigin.Begin);
                byte[] buffer = this._encryptor.Encrypt(output.ToArray());
                size = buffer.Length;
                if (deleted)
                {
                    size = -size;
                }
                writer.Write(size);
                writer.Write(buffer);
            }
        }

        private void writeRecordRaw(BinaryWriter dataWriter, byte[] record, bool deleted)
        {
            int length = record.Length;
            if (deleted)
            {
                length = -length;
            }
            dataWriter.Write(length);
            dataWriter.Write(record);
        }

        private void writeSchema(BinaryWriter writer)
        {
            writer.Seek(6, SeekOrigin.Begin);
            writer.Write(this._numRecords);
            writer.Write(this._numDeleted);
            writer.Write(this._indexStartPos);
            string str = this._userVersion;
            if (str == null)
            {
                str = string.Empty;
            }
            writer.Write(str);
            writer.Write(this._primaryKey);
            writer.Write(this._fields.Count);
            if (!string.IsNullOrEmpty(this._primaryKey))
            {
                this.writeField(writer, this._primaryKeyField);
            }
            foreach (Field field in this._fields)
            {
                if (field != this._primaryKeyField)
                {
                    this.writeField(writer, field);
                }
            }
            this._dataStartPos = (int) this._dataStrm.Position;
        }

        private void writeStringArray(BinaryWriter dataWriter, string[] arr)
        {
            dataWriter.Write(arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                string str = arr[i];
                dataWriter.Write((str == null) ? string.Empty : str);
            }
        }

        internal string UserVersion
        {
            get
            {
                return this._userVersion;
            }
            set
            {
                this._userVersion = value;
            }
        }

        internal FwNs.Data.FyleDbNs.Fields Fields
        {
            get
            {
                this.checkIsDbOpen();
                return this._fields;
            }
        }

        internal int NumDeleted
        {
            get
            {
                this.checkIsDbOpen();
                return this._numDeleted;
            }
            set
            {
                this._numDeleted = value;
            }
        }

        internal int NumRecords
        {
            get
            {
                this.checkIsDbOpen();
                return this._numRecords;
            }
            set
            {
                this._numRecords = value;
            }
        }

        internal bool IsOpen
        {
            get
            {
                return this._isOpen;
            }
            set
            {
                this._isOpen = value;
            }
        }

        internal bool AutoFlush
        {
            get
            {
                return this._autoFlush;
            }
            set
            {
                if ((this._isOpen && !this._autoFlush) & value)
                {
                    this.flush(true);
                }
                this._autoFlush = value;
            }
        }

        internal object MetaData
        {
            get
            {
                return this._metaData;
            }
            set
            {
                this._metaData = value;
                if (this.AutoFlush)
                {
                    this.flush(true);
                }
            }
        }

        private bool isEof
        {
            get
            {
                return (this._iteratorIndex >= this._numRecords);
            }
        }

        private class RowComparer : IComparer
        {
            private List<Field> _fieldLst;
            private List<bool> _sortDirLst;
            private List<bool> _caseLst;

            internal RowComparer(List<Field> fieldLst, List<bool> sortDirLst, List<bool> caseLst)
            {
                this._fieldLst = fieldLst;
                this._caseLst = caseLst;
                this._sortDirLst = sortDirLst;
            }

            int IComparer.Compare(object x, object y)
            {
                object[] objArray = x as object[];
                object[] objArray2 = y as object[];
                if ((objArray == null) || (objArray2 == null))
                {
                    return 0;
                }
                for (int i = 0; i < this._fieldLst.Count; i++)
                {
                    Field field = this._fieldLst[i];
                    bool caseInsensitive = this._caseLst[i];
                    object obj2 = objArray2[field.Ordinal];
                    int num3 = FileDbEngine.CompareVals(objArray[field.Ordinal], obj2, field.DataType, caseInsensitive);
                    if (this._sortDirLst[i])
                    {
                        num3 = -num3;
                    }
                    if (num3 != 0)
                    {
                        return num3;
                    }
                }
                return 0;
            }
        }
    }
}

