namespace FwNs.Core.LC.cRights
{
    using System;

    public class GrantConstants
    {
        public const int Select = 1;
        public const int Delete = 2;
        public const int Insert = 4;
        public const int Update = 8;
        public const int Usage = 0x10;
        public const int Execute = 0x20;
        public const int References = 0x40;
        public const int Trigger = 0x80;
        public const int All = 0x3f;
        public const string SrAll = "ALL";
        public const string SrSelect = "SELECT";
        public const string SrUpdate = "UPDATE";
        public const string SrDelete = "DELETE";
        public const string SrInsert = "INSERT";
        public const string SrUsage = "USAGE";
        public const string SrExecute = "EXECUTE";
    }
}

