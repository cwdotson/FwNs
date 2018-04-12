namespace FwNs.Data.FyleDbNs
{
    using System;

    public enum DataTypeEnum : short
    {
        Bool = 3,
        Byte = 6,
        Int = 9,
        UInt = 10,
        Int64 = 11,
        Float = 13,
        Double = 14,
        Decimal = 15,
        DateTime = 0x10,
        String = 0x12,
        Guid = 100,
        Undefined = 0x7fff
    }
}

