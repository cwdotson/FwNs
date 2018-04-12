namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRows;
    using System;

    public sealed class RowOutputBinaryEncode : RowOutputBinary
    {
        private readonly Crypto _crypto;

        public RowOutputBinaryEncode(Crypto crypto, int initialSize, int scale) : base(initialSize, scale)
        {
            this._crypto = crypto;
        }

        public override IRowOutputInterface Duplicate()
        {
            return new RowOutputBinaryEncode(this._crypto, 0x80, base.Scale);
        }

        public override int GetSize(Row row)
        {
            int size = base.GetSize(row);
            if (this._crypto != null)
            {
                size = this._crypto.GetEncodedSize(size - 4) + 8;
            }
            return size;
        }

        public override void WriteData(object[] data, SqlType[] types)
        {
            if (this._crypto == null)
            {
                base.WriteData(data, types);
            }
            else
            {
                int count = base.Count;
                this.WriteInt(0);
                base.WriteData(data, types);
                int length = (base.Count - count) - 4;
                byte[] sourceArray = this._crypto.Encode(base.Buffer, count + 4, length);
                int num3 = sourceArray.Length;
                Array.Copy(sourceArray, 0, base.Buffer, count + 4, num3);
                this.WriteIntData(num3, count);
                base.Count = (count + 4) + num3;
            }
        }
    }
}

