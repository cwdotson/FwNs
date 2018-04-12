namespace FwNs.Core.LC.cSchemas
{
    using System;

    public static class SchemaObjectReferentialAction
    {
        public const int Cascade = 0;
        public const int Restrict = 1;
        public const int SetNull = 2;
        public const int NoAction = 3;
        public const int SetDefault = 4;
    }
}

