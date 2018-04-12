namespace System.Data.LibCore.Clr
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class SqlFunction : Attribute
    {
        public DataAccessKind DataAccess { get; set; }

        public string FillRowMethodName { get; set; }
    }
}

