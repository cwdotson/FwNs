namespace FwNs.Core.LC
{
    using System;

    public interface ITrigger
    {
        void Fire(int type, string trigName, string tabName, object[] oldRow, object[] newRow);
    }
}

