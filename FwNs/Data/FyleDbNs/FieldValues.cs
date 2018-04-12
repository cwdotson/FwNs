namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class FieldValues : Dictionary<string, object>
    {
        public FieldValues() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public FieldValues(int count) : base(count, StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(string fieldName, object value)
        {
            base.Add(fieldName, value);
        }

        public bool ContainsKey(string fieldName)
        {
            return base.ContainsKey(fieldName);
        }

        public object this[string idx]
        {
            get
            {
                return base[idx];
            }
            set
            {
                base[idx] = value;
            }
        }
    }
}

