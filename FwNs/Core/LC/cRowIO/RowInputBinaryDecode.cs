namespace FwNs.Core.LC.cRowIO
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cPersist;
    using System;

    public sealed class RowInputBinaryDecode : RowInputBinary
    {
        private readonly Crypto _crypto;

        public RowInputBinaryDecode(Crypto crypto, byte[] buf) : base(buf)
        {
            this._crypto = crypto;
        }

        public override object[] ReadData(SqlType[] colTypes)
        {
            if (this._crypto != null)
            {
                int pos = base.Pos;
                int length = this.ReadInt();
                byte[] sourceArray = this._crypto.Decode(base.Buf, base.Pos, length);
                Array.Copy(sourceArray, 0, base.Buf, pos, sourceArray.Length);
                base.Pos = pos;
            }
            return base.ReadData(colTypes);
        }
    }
}

