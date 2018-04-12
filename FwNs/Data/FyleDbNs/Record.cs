namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    public class Record : IEnumerable, ICustomMemberProvider
    {
        private Fields _fields;
        private List<object> _values;

        public Record(Fields fields, object[] values)
        {
            this._fields = fields;
            this._values = new List<object>(fields.Count);
            for (int i = 0; i < fields.Count; i++)
            {
                Field local1 = fields[i];
                object item = null;
                if (values != null)
                {
                    item = values[i];
                }
                this._values.Add(item);
            }
        }

        public bool ContainsField(string fieldName)
        {
            return this._fields.ContainsKey(fieldName);
        }

        public bool? GetBoolean(int index)
        {
            return (bool?) this[index];
        }

        public bool? GetBoolean(string fieldName)
        {
            return (bool?) this[fieldName];
        }

        public byte? GetByte(int index)
        {
            return (byte?) this[index];
        }

        public byte? GetByte(string fieldName)
        {
            return (byte?) this[fieldName];
        }

        public DateTime? GetDateTime(int index)
        {
            return (DateTime?) this[index];
        }

        public DateTime? GetDateTime(string fieldName)
        {
            return (DateTime?) this[fieldName];
        }

        public double? GetDouble(int index)
        {
            return (double?) this[index];
        }

        public double? GetDouble(string fieldName)
        {
            return (double?) this[fieldName];
        }

        public ObjectEnumerator GetEnumerator()
        {
            return new ObjectEnumerator(this._values.ToArray());
        }

        public FieldValues GetFieldValues()
        {
            FieldValues values = new FieldValues(this._fields.Count);
            foreach (Field field in this._fields)
            {
                values.Add(field.Name, this[field.Name]);
            }
            return values;
        }

        public int? GetInt(int index)
        {
            return (int?) this[index];
        }

        public int? GetInt(string fieldName)
        {
            return (int?) this[fieldName];
        }

        public IEnumerable<string> GetNames()
        {
            List<string> list = new List<string>(this._fields.Count);
            foreach (Field field in this._fields)
            {
                list.Add(field.Name);
            }
            return list;
        }

        public float? GetSingle(int index)
        {
            return (float?) this[index];
        }

        public float? GetSingle(string fieldName)
        {
            return (float?) this[fieldName];
        }

        public string GetString(int index)
        {
            return (string) this[index];
        }

        public string GetString(string fieldName)
        {
            return (string) this[fieldName];
        }

        public IEnumerable<Type> GetTypes()
        {
            List<Type> list = new List<Type>(this._fields.Count);
            for (int i = 0; i < this._fields.Count; i++)
            {
                Type item = null;
                switch (this._fields[0].DataType)
                {
                    case DataTypeEnum.Byte:
                        item = typeof(byte);
                        break;

                    case DataTypeEnum.Int:
                        item = typeof(int);
                        break;

                    case DataTypeEnum.UInt:
                        item = typeof(uint);
                        break;

                    case DataTypeEnum.Float:
                        item = typeof(float);
                        break;

                    case DataTypeEnum.Double:
                        item = typeof(double);
                        break;

                    case DataTypeEnum.DateTime:
                        item = typeof(DateTime);
                        break;

                    case DataTypeEnum.String:
                        item = typeof(string);
                        break;

                    case DataTypeEnum.Bool:
                        item = typeof(bool);
                        break;
                }
                list.Add(item);
            }
            return list;
        }

        public uint? GetUInt(int index)
        {
            return (uint?) this[index];
        }

        public uint? GetUInt(string fieldName)
        {
            return (uint?) this[fieldName];
        }

        public IEnumerable<object> GetValues()
        {
            List<object> list = new List<object>(this._fields.Count);
            for (int i = 0; i < this._fields.Count; i++)
            {
                list.Add(this._values[i]);
            }
            return list;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < this._fields.Count; i++)
            {
                Field field = this._fields[i];
                if (builder.Length > 0)
                {
                    builder.Append("\r\n");
                }
                builder.Append(string.Format("{0}: {1}", field.Name, this._values[i]));
            }
            return builder.ToString();
        }

        internal List<object> Values
        {
            get
            {
                return this._values;
            }
        }

        public object this[string name]
        {
            get
            {
                return this._values[this._fields[name].Ordinal];
            }
            set
            {
                this._values[this._fields[name].Ordinal] = value;
            }
        }

        public object this[int idx]
        {
            get
            {
                if ((idx < 0) || (idx >= this._values.Count))
                {
                    throw new FileDbException("Index out of range", FileDbExceptionsEnum.IndexOutOfRange);
                }
                return this._values[idx];
            }
            set
            {
                if ((idx < 0) || (idx >= this._values.Count))
                {
                    throw new FileDbException("Index out of range", FileDbExceptionsEnum.IndexOutOfRange);
                }
                this._values[idx] = value;
            }
        }

        public int Length
        {
            get
            {
                return this._values.Count;
            }
        }

        public IList<string> FieldNames
        {
            get
            {
                List<string> list = new List<string>(this._fields.Count);
                foreach (Field field in this._fields)
                {
                    list.Add(field.Name);
                }
                return list;
            }
        }

        public object Data
        {
            get
            {
                return this;
            }
            set
            {
                FieldSetter setter = value as FieldSetter;
                this[setter.PropertyName] = setter.Value;
            }
        }
    }
}

