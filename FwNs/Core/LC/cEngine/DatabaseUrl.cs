namespace FwNs.Core.LC.cEngine
{
    using System;

    public class DatabaseUrl
    {
        public const string SMem = "mem:";
        public const string SFile = "file:";
        public const string SRes = "res:";
        public const string SAlias = "alias:";
        public const string SUtl = "utl://";
        public const string SUtls = "utls://";
        public const string SHttp = "http://";
        public const string SHttps = "https://";
        public const string SUrlPrefix = "jdbc:hsqldb:";

        public static bool IsFileBasedDatabaseType(string type)
        {
            if (type != "file:")
            {
                return (type == "res:");
            }
            return true;
        }

        public static bool IsInProcessDatabaseType(string type)
        {
            if ((type != "file:") && (type != "mem:"))
            {
                return (type == "res:");
            }
            return true;
        }
    }
}

