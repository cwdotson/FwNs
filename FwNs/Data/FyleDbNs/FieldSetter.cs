namespace FwNs.Data.FyleDbNs
{
    using System;

    public class FieldSetter
    {
        private string _propertyName;
        private object _value;

        public FieldSetter(string propertyName, object value)
        {
            this._propertyName = propertyName;
            this._value = value;
        }

        public object Value
        {
            get
            {
                return this._value;
            }
        }

        public string PropertyName
        {
            get
            {
                return this._propertyName;
            }
        }
    }
}

