namespace FwNs.Data.FyleDbNs
{
    using FwNs;
    using System;
    using System.Runtime.CompilerServices;

    public delegate IKv<T, U> KV<T, U>() where T: IComparable;
}

