namespace System.Data.LibCore.Clr
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class UtlDataRecord
    {
        private readonly UtlMetaData[] _metaData;
        private readonly Dictionary<string, int> _nameIndexMap = new Dictionary<string, int>();
        private object[] _values;

        public UtlDataRecord(params UtlMetaData[] metaData)
        {
            this._metaData = metaData;
            this._values = new object[this._metaData.Length];
            for (int i = 0; i < this._metaData.Length; i++)
            {
                this._nameIndexMap.Add(this._metaData[i].Name, i);
            }
        }

        public UtlMetaData[] GetMetaData()
        {
            return this._metaData;
        }

        public object[] GetValuesDuplicate()
        {
            return (object[]) this._values.Clone();
        }

        public virtual void SetBoolean(int ordinal, bool value)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            this._values[ordinal] = value;
        }

        public virtual void SetDecimal(int ordinal, decimal value)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            this._values[ordinal] = value;
        }

        public virtual void SetDouble(int ordinal, double value)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            this._values[ordinal] = value;
        }

        public virtual void SetInt16(int ordinal, short value)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            this._values[ordinal] = value;
        }

        public virtual void SetInt32(int ordinal, int value)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            this._values[ordinal] = value;
        }

        public virtual void SetInt64(int ordinal, long value)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            this._values[ordinal] = value;
        }

        public virtual void SetString(int ordinal, string value)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            this._values[ordinal] = value;
        }

        public virtual void SetValue(int ordinal, object value)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            this._values[ordinal] = value;
        }

        public virtual int SetValues(params object[] values)
        {
            if (this._metaData == null)
            {
                throw new ArgumentNullException();
            }
            if (this._metaData.Length != values.Length)
            {
                throw new ArgumentException();
            }
            this._values = values;
            return this._values.Length;
        }

        public virtual object this[int ordinal]
        {
            get
            {
                if (this._metaData == null)
                {
                    throw new InvalidOperationException();
                }
                return this._values[ordinal];
            }
        }

        public virtual object this[string name]
        {
            get
            {
                if (this._metaData == null)
                {
                    throw new InvalidOperationException();
                }
                return this._values[this._nameIndexMap[name]];
            }
        }

        public virtual int FieldCount
        {
            get
            {
                if (this._metaData == null)
                {
                    throw new InvalidOperationException();
                }
                return this._metaData.Length;
            }
        }
    }
}

