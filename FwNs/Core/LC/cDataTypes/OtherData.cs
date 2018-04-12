namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;

    public sealed class OtherData
    {
        private readonly byte[] _data;

        public OtherData(byte[] data)
        {
            this._data = data;
        }

        public OtherData(object o)
        {
            try
            {
                this._data = InOutUtil.Serialize(o);
            }
            catch (Exception exception)
            {
                throw Error.GetError(0xd91, exception.ToString());
            }
        }

        public byte[] GetBytes()
        {
            return this._data;
        }

        public int GetBytesLength()
        {
            return this._data.Length;
        }

        public object GetObject()
        {
            object obj2;
            try
            {
                obj2 = InOutUtil.Deserialize(this._data);
            }
            catch (Exception exception)
            {
                throw Error.GetError(0xd91, exception.ToString());
            }
            return obj2;
        }
    }
}

