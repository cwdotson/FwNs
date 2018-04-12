namespace FwNs.Core.LC.cEngine
{
    using System;

    public static class SessionTxConsts
    {
        public const int TxReadUncommitted = 0x100;
        public const int TxReadCommitted = 0x1000;
        public const int TxRepeatableRead = 0x10000;
        public const int TxSerializable = 0x100000;
    }
}

