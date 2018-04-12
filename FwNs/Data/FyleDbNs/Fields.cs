namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class Fields : List<Field>
    {
        private Dictionary<string, Field> _fields;

        public Fields()
        {
            this._fields = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
        }

        public Fields(Fields fields) : base(fields.Count)
        {
            this._fields = new Dictionary<string, Field>(fields.Count, StringComparer.OrdinalIgnoreCase);
            foreach (Field field in fields)
            {
                this.Add(field);
            }
        }

        public Fields(int capacity) : base(capacity)
        {
            this._fields = new Dictionary<string, Field>(capacity, StringComparer.OrdinalIgnoreCase);
        }

        public void Add(Field field)
        {
            base.Add(field);
            this._fields.Add(field.Name, field);
        }

        public bool ContainsKey(string fieldName)
        {
            return this._fields.ContainsKey(fieldName);
        }

        public bool Remove(string fieldName)
        {
            bool flag = false;
            Field item = this._fields[fieldName];
            if (item != null)
            {
                flag = base.Remove(item);
                this._fields.Remove(fieldName);
            }
            return flag;
        }

        public Field this[string fieldName]
        {
            get
            {
                if (this._fields.Count > 0)
                {
                    return this._fields[fieldName];
                }
                return null;
            }
        }
    }
}

