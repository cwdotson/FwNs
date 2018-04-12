namespace System.Data.LibCore
{
    using FwNs.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Reflection;

    [Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ListBindable(false)]
    public sealed class UtlParameterCollection : DbParameterCollection
    {
        private readonly List<UtlParameter> _parameterList = new List<UtlParameter>();

        public UtlParameterCollection(UtlCommand cmd)
        {
        }

        public int Add(UtlParameter parameter)
        {
            int index = -1;
            if (!string.IsNullOrEmpty(parameter.ParameterName))
            {
                index = this.IndexOf(parameter.ParameterName);
            }
            if (index == -1)
            {
                index = this._parameterList.Count;
                this._parameterList.Add(parameter);
            }
            this.SetParameter(index, parameter);
            return index;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int Add(object value)
        {
            return this.Add((UtlParameter) value);
        }

        public UtlParameter Add(string parameterName, UtlType parameterType)
        {
            UtlParameter parameter = new UtlParameter(parameterName, parameterType);
            this.Add(parameter);
            return parameter;
        }

        public UtlParameter Add(string parameterName, UtlType parameterType, int parameterSize)
        {
            UtlParameter parameter = new UtlParameter(parameterName, parameterType, parameterSize);
            this.Add(parameter);
            return parameter;
        }

        public UtlParameter Add(string parameterName, UtlType parameterType, int parameterSize, string sourceColumn)
        {
            UtlParameter parameter = new UtlParameter(parameterName, parameterType, parameterSize, sourceColumn);
            this.Add(parameter);
            return parameter;
        }

        public void AddRange(UtlParameter[] values)
        {
            int length = values.Length;
            for (int i = 0; i < length; i++)
            {
                this.Add(values[i]);
            }
        }

        public override void AddRange(Array values)
        {
            int length = values.Length;
            for (int i = 0; i < length; i++)
            {
                this.Add((UtlParameter) values.GetValue(i));
            }
        }

        public UtlParameter AddWithValue(string parameterName, object value)
        {
            UtlParameter parameter = new UtlParameter(parameterName, value);
            this.Add(parameter);
            return parameter;
        }

        public override void Clear()
        {
            this._parameterList.Clear();
        }

        public override bool Contains(object value)
        {
            return this._parameterList.Contains((UtlParameter) value);
        }

        public override bool Contains(string parameterName)
        {
            return (this.IndexOf(parameterName) != -1);
        }

        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            return this._parameterList.GetEnumerator();
        }

        protected override DbParameter GetParameter(int index)
        {
            if ((index < 0) || (index >= this._parameterList.Count))
            {
                return null;
            }
            return this._parameterList[index];
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return this.GetParameter(this.IndexOf(parameterName));
        }

        public override int IndexOf(object value)
        {
            return this._parameterList.IndexOf((UtlParameter) value);
        }

        public override int IndexOf(string parameterName)
        {
            int count = this._parameterList.Count;
            for (int i = 0; i < count; i++)
            {
                if (((string.Compare(parameterName, this._parameterList[i].ParameterName, StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare("@" + parameterName, this._parameterList[i].ParameterName, StringComparison.OrdinalIgnoreCase) == 0)) || (string.Compare(parameterName, "@" + this._parameterList[i].ParameterName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    return i;
                }
            }
            return -1;
        }

        public override void Insert(int index, object value)
        {
            this._parameterList.Insert(index, (UtlParameter) value);
        }

        public override void Remove(object value)
        {
            this._parameterList.Remove((UtlParameter) value);
        }

        public override void RemoveAt(int index)
        {
            this._parameterList.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            this.RemoveAt(this.IndexOf(parameterName));
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            this._parameterList[index] = (UtlParameter) value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            this.SetParameter(this.IndexOf(parameterName), value);
        }

        public override bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public override bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public override object SyncRoot
        {
            get
            {
                return null;
            }
        }

        public override int Count
        {
            get
            {
                return this._parameterList.Count;
            }
        }

        public UtlParameter this[string parameterName]
        {
            get
            {
                return (UtlParameter) this.GetParameter(parameterName);
            }
            set
            {
                this.SetParameter(parameterName, value);
            }
        }

        public UtlParameter this[int index]
        {
            get
            {
                return (UtlParameter) this.GetParameter(index);
            }
            set
            {
                this.SetParameter(index, value);
            }
        }
    }
}

