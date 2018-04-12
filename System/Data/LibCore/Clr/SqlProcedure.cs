namespace System.Data.LibCore.Clr
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class SqlProcedure : Attribute
    {
        public string Name { get; set; }
    }
}

